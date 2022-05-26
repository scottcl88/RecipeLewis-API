using AutoMapper;
using Database;
using RecipeLewis.Models;
using RecipeLewis.Models.Requests;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using RecipeLewis.Models.Results;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;

namespace RecipeLewis.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IJwtUtils _jwtUtils;
    private readonly AppSettings _appSettings;
    private readonly LogService _logService;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;

    public UserService(ApplicationDbContext dbContext, LogService logService, IOptions<AppSettings> appSettings, IJwtUtils jwtUtils, IEmailService emailService, IMapper mapper)
    {
        _dbContext = dbContext;
        _logService = logService;
        _mapper = mapper;
        _jwtUtils = jwtUtils;
        _emailService = emailService;
        _appSettings = appSettings.Value;
    }
    public AuthenticateResponse Authenticate(AuthenticateRequest request, string ipAddress)
    {
        var user = _dbContext.Users.SingleOrDefault(x => x.Email == request.Email);

        // validate
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new AppException("Email or password is incorrect");

        // authentication successful so generate jwt and refresh tokens
        var jwtToken = _jwtUtils.GenerateJwtToken(user);
        var refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
        user.RefreshTokens.Add(refreshToken);

        user.LastLogin = DateTime.UtcNow;

        // remove old refresh tokens from user
        removeOldRefreshTokens(user);

        // save changes to db
        _dbContext.Update(user);
        _dbContext.SaveChanges();

        var userModel = _mapper.Map<UserModel>(user);
        return new AuthenticateResponse(userModel, jwtToken, refreshToken.Token);
    }

    public AuthenticateResponse RefreshToken(string token, string ipAddress)
    {
        var user = getUserByRefreshToken(token);
        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

        if (refreshToken.IsRevoked)
        {
            // revoke all descendant tokens in case this token has been compromised
            revokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
            _dbContext.Update(user);
            _dbContext.SaveChanges();
        }

        if (!refreshToken.IsActive)
            throw new AppException("Invalid token");

        // replace old refresh token with a new one (rotate token)
        var newRefreshToken = rotateRefreshToken(refreshToken, ipAddress);
        user.RefreshTokens.Add(newRefreshToken);

        // remove old refresh tokens from user
        removeOldRefreshTokens(user);

        // save changes to db
        _dbContext.Update(user);
        _dbContext.SaveChanges();

        // generate new jwt
        var jwtToken = _jwtUtils.GenerateJwtToken(user);

        var userModel = _mapper.Map<UserModel>(user);
        return new AuthenticateResponse(userModel, jwtToken, newRefreshToken.Token);
    }
    public void RevokeToken(string token, string ipAddress)
    {
        var user = getUserByRefreshToken(token);
        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

        if (!refreshToken.IsActive)
            throw new AppException("Invalid token");

        // revoke token and save
        revokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
        _dbContext.Update(user);
        _dbContext.SaveChanges();
    }

    private User getUserByRefreshToken(string token)
    {
        var user = _dbContext.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

        if (user == null)
            throw new AppException("Invalid token");

        return user;
    }

    private RefreshToken rotateRefreshToken(RefreshToken refreshToken, string ipAddress)
    {
        var newRefreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
        revokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
        return newRefreshToken;
    }

    private void removeOldRefreshTokens(User user)
    {
        // remove old inactive refresh tokens from user based on TTL in app settings
        user.RefreshTokens.RemoveAll(x =>
            !x.IsActive &&
            x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
    }

    private void revokeDescendantRefreshTokens(RefreshToken refreshToken, User user, string ipAddress, string reason)
    {
        // recursively traverse the refresh token chain and ensure all descendants are revoked
        if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
        {
            var childToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
            if (childToken.IsActive)
                revokeRefreshToken(childToken, ipAddress, reason);
            else
                revokeDescendantRefreshTokens(childToken, user, ipAddress, reason);
        }
    }

    private void revokeRefreshToken(RefreshToken token, string ipAddress, string reason = null, string replacedByToken = null)
    {
        token.Revoked = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReasonRevoked = reason;
        token.ReplacedByToken = replacedByToken;
    }

    public void Register(RegisterRequest request, string ipAddress, string origin)
    {
        // validate
        if (_dbContext.Users.Any(x => x.Email == request.Email))
        {
            // send already registered error in email to prevent user enumeration
            sendAlreadyRegisteredEmail(request.Email, origin);
            return;
        }

        // map model to new user object
        var user = _mapper.Map<User>(request);
        user.LastIPAddress = ipAddress;

        // first registered user is an admin
        var isFirstuser = _dbContext.Users.Count() == 0;
        user.Role = isFirstuser ? Database.Role.Admin : Database.Role.User;
        user.CreatedDateTime = DateTime.UtcNow;
        user.VerificationToken = generateVerificationToken();

        // hash password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // save user
        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();

        // send email
        sendVerificationEmail(user, origin);
    }

    public void VerifyEmail(string token)
    {
        var user = _dbContext.Users.SingleOrDefault(x => x.VerificationToken == token);

        if (user == null)
            throw new AppException("Verification failed");

        user.Verified = DateTime.UtcNow;
        user.VerificationToken = null;

        _dbContext.Users.Update(user);
        _dbContext.SaveChanges();
    }

    public void ForgotPassword(ForgotPasswordRequest request, string origin)
    {
        var user = _dbContext.Users.SingleOrDefault(x => x.Email == request.Email);

        // always return ok response to prevent email enumeration
        if (user == null) return;

        // create reset token that expires after 1 day
        user.ResetToken = generateResetToken();
        user.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

        _dbContext.Users.Update(user);
        _dbContext.SaveChanges();

        // send email
        sendPasswordResetEmail(user, origin);
    }

    public void ValidateResetToken(ValidateResetTokenRequest request)
    {
        getUserByResetToken(request.Token);
    }

    public void ResetPassword(ResetPasswordRequest request)
    {
        var user = getUserByResetToken(request.Token);

        // update password and remove reset token
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        user.PasswordReset = DateTime.UtcNow;
        user.ResetToken = null;
        user.ResetTokenExpires = null;

        _dbContext.Users.Update(user);
        _dbContext.SaveChanges();
    }

    public void RequestEditAccess(UserId userId)
    {
        var user = GetDbUserById(userId);

        user.ResetTokenExpires = DateTime.UtcNow.AddDays(1);
        user.RequestedAccess = true;

        _dbContext.Users.Update(user);
        _dbContext.SaveChanges();

        sendRequestEditAccessEmail(user);
    }

    public UserModel Create(CreateUserRequest request)
    {
        // validate
        if (_dbContext.Users.Any(x => x.Email == request.Email))
            throw new AppException($"Email '{request.Email}' is already registered");

        // map model to new user object
        var user = _mapper.Map<User>(request);
        user.CreatedDateTime = DateTime.UtcNow;
        user.Verified = DateTime.UtcNow;

        // hash password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // save user
        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();

        return _mapper.Map<UserModel>(user);
    }

    public UserModel Update(UserId id, UpdateUserRequest request)
    {
        var user = GetDbUserById(id);

        // validate
        if (user.Email != request.Email && _dbContext.Users.Any(x => x.Email == request.Email))
            throw new AppException($"Email '{request.Email}' is already registered");

        // hash password if it was entered
        if (!string.IsNullOrEmpty(request.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // copy model to user and save
        _mapper.Map(request, user);
        user.ModifiedDateTime = DateTime.UtcNow;
        _dbContext.Users.Update(user);
        _dbContext.SaveChanges();

        return _mapper.Map<UserModel>(user);
    }

    public void Delete(UserId id)
    {
        var user = GetDbUserById(id);
        _dbContext.Users.Remove(user);
        _dbContext.SaveChanges();
    }

    private User getUserByResetToken(string token)
    {
        var user = _dbContext.Users.SingleOrDefault(x =>
            x.ResetToken == token && x.ResetTokenExpires > DateTime.UtcNow);
        if (user == null) throw new AppException("Invalid token");
        return user;
    }

    private string generateResetToken()
    {
        // token is a cryptographically strong random sequence of values
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        // ensure token is unique by checking against db
        var tokenIsUnique = !_dbContext.Users.Any(x => x.ResetToken == token);
        if (!tokenIsUnique)
            return generateResetToken();

        return token;
    }

    private string generateVerificationToken()
    {
        // token is a cryptographically strong random sequence of values
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        // ensure token is unique by checking against db
        var tokenIsUnique = !_dbContext.Users.Any(x => x.VerificationToken == token);
        if (!tokenIsUnique)
            return generateVerificationToken();

        return token;
    }

    private void sendVerificationEmail(User user, string origin)
    {
        string message;
        if (!string.IsNullOrEmpty(origin))
        {
            // origin exists if request sent from browser single page app (e.g. Angular or React)
            // so send link to verify via single page app
            var verifyUrl = $"{origin}/user/verify-email?token={user.VerificationToken}";
            message = $@"<p>Please click the below link to verify your email address:</p>
                            <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
        }
        else
        {
            // origin missing if request sent directly to api (e.g. from Postman)
            // so send instructions to verify directly with api
            message = $@"<p>Please use the below token to verify your email address with the <code>/users/verify-email</code> api route:</p>
                            <p><code>{user.VerificationToken}</code></p>";
        }

        _emailService.Send(
            to: user.Email,
            subject: "Sign-up Verification API - Verify Email",
            html: $@"<h4>Verify Email</h4>
                        <p>Thanks for registering!</p>
                        {message}"
        );
    }

    private void sendAlreadyRegisteredEmail(string email, string origin)
    {
        string message;
        if (!string.IsNullOrEmpty(origin))
            message = $@"<p>If you don't know your password please visit the <a href=""{origin}/user/forgot-password"">forgot password</a> page.</p>";
        else
            message = "<p>If you don't know your password you can reset it via the <code>/users/forgot-password</code> api route.</p>";

        _emailService.Send(
            to: email,
            subject: "Sign-up Verification API - Email Already Registered",
            html: $@"<h4>Email Already Registered</h4>
                        <p>Your email <strong>{email}</strong> is already registered.</p>
                        {message}"
        );
    }

    private void sendPasswordResetEmail(User user, string origin)
    {
        string message;
        if (!string.IsNullOrEmpty(origin))
        {
            var resetUrl = $"{origin}/user/reset-password?token={user.ResetToken}";
            message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                            <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
        }
        else
        {
            message = $@"<p>Please use the below token to reset your password with the <code>/users/reset-password</code> api route:</p>
                            <p><code>{user.ResetToken}</code></p>";
        }

        _emailService.Send(
            to: user.Email,
            subject: "Sign-up Verification API - Reset Password",
            html: $@"<h4>Reset Password Email</h4>
                        {message}"
        );
    }

    private void sendRequestEditAccessEmail(User user)
    {
        string message;
        message = $@"<p>The user {user.Email} has requested edit access. UserId = {user.UserId}</p>";
        _emailService.Send(
            to: user.Email,
            subject: "Access Requested",
            html: $@"<h4>Access Requested</h4>
                        {message}"
        );
    }

    public List<UserModel> SearchUsers(UserId userId, string query)
    {
        var users = _dbContext.Users.Where(x => x.UserId != userId.Value && (x.Name.ToLower().Contains(query) || x.Email.ToLower().Contains(query)) && x.DeletedDateTime == null);
        var userModels = _mapper.Map<List<UserModel>>(users.ToList());
        return userModels;
    }

    public UserModel? GetUser(UserId userId)
    {
        var user = GetDbUserById(userId);
        var userModel = _mapper.Map<UserModel>(user);
        return userModel;
    }
    public User? GetDbUserById(UserId userId)
    {
        var user = _dbContext.Users.FirstOrDefault(x => x.UserId == userId.Value && x.DeletedDateTime == null);
        if (user == null)
        {
            _logService.Error("getUserById failed - Unable to find user", userId);
        }
        return user;
    }

    public UserModel? GetUserByGUID(Guid userGUID, UserId userId)
    {
        var user = _dbContext.Users.FirstOrDefault(x => x.UserGUID == userGUID && x.DeletedDateTime == null);
        if (user == null)
        {
            _logService.Error("GetUserByGUID failed - Unable to find user", userId, userGUID);
        }
        var userModel = _mapper.Map<UserModel>(user);
        return userModel;
    }

}
