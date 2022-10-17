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

        if (user == null)
        {
            _logService.Info($"Email is incorrect. Email: {request.Email}", null);
            throw new AppException("Incorrect email or password");
        }

        if (string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            _logService.Info($"Password Reset is required. Email: {request.Email}", null);
            //Old user with Auth0 or something else went wrong, force password reset
            throw new AppException("Password reset required");
        }

        if (!user.IsVerified)
        {
            _logService.Info($"Email verification. Email: {request.Email}", null);
            //Old user with Auth0 or something else went wrong, force password reset
            throw new AppException("Email verification required");
        }

        // validate
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            _logService.Info($"Password is incorrect. Email: {request.Email}", null);
            throw new AppException("Incorrect email or password");
        }

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

        user.Role = Database.Role.User;
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

        user.RequestedAccessExpires = DateTime.UtcNow.AddDays(7);
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

        //// validate
        //if (user.Email != request.Email && _dbContext.Users.Any(x => x.Email == request.Email))
        //    throw new AppException($"Email '{request.Email}' is already registered");

        //// hash password if it was entered
        //if (!string.IsNullOrEmpty(request.Password))
        //    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // copy model to user and save
        //_mapper.Map(request, user);
        user.Name = request.Name;
        user.ModifiedDateTime = DateTime.UtcNow;
        _dbContext.Users.Update(user);
        _dbContext.SaveChanges();

        return _mapper.Map<UserModel>(user);
    }

    public UserModel Promote(UserId id, UserId? currentUser)
    {
        var user = GetDbUserById(id);
        if (user == null)
        {
            throw new AppException("Cannot find user to promote");
        }
        var currentRole = user.Role;
        var roleValue = (int)currentRole;
        var prommoteValue = roleValue + 1;
        var newRole = (Database.Role)(prommoteValue);
        user.Role = newRole;
        user.ModifiedDateTime = DateTime.UtcNow;
        _dbContext.SaveChanges();
        _logService.Info($"User {id.Value} promoted to {newRole} by User {currentUser?.Value}", currentUser);
        return _mapper.Map<UserModel>(user);
    }
    public UserModel Demote(UserId id, UserId? currentUser)
    {
        var user = GetDbUserById(id);
        if(user == null)
        {
            throw new AppException("Cannot find user to demote");
        }
        var currentRole = user.Role;
        var roleValue = (int)currentRole;
        var demoteValue = roleValue - 1;
        var newRole = (Database.Role)(demoteValue);
        user.Role = newRole;
        user.ModifiedDateTime = DateTime.UtcNow;
        _dbContext.SaveChanges();
        _logService.Info($"User {id.Value} demoted to {newRole} by User {currentUser?.Value}", currentUser);
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
        if (string.IsNullOrEmpty(origin))
        {
            throw new AppException("Invalid origin when sending password reset email. " + origin);
        }

        var verifyUrl = $"{origin}/verify-email?token={user.VerificationToken}";
        string message = $@"<p>Please click the below link to verify your email address:</p>
                            <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";

        _emailService.Send(
            to: user.Email,
            subject: "Recipe Lewis - Verify Email",
            html: $@"<h4>Recipe Lewis - Verify Email</h4>
                        <p>Hello {user.Name},</p>
                        <p>Thanks for registering!</p>
                        {message}
                        <p>Thank you,<br>
                        <p>Recipe Lewis</p>"
        );
    }

    private void sendAlreadyRegisteredEmail(string email, string origin)
    {
        if (string.IsNullOrEmpty(origin))
        {
            throw new AppException("Invalid origin when sending password reset email. " + origin);
        }
        string message = $@"<p>If you don't know your password please visit the <a href=""{origin}/forgot-password"">forgot password</a> page.</p>";

        _emailService.Send(
            to: email,
            subject: "Recipe Lewis - Email Already Registered",
            html: $@"<h4>Recipe Lewis - Email Already Registered</h4>
                        <p>Hello,</p>
                        <p>Your email <strong>{email}</strong> is already registered with Recipe Lewis.</p>
                        {message}
                        <p>Thank you,<br>
                        <p>Recipe Lewis</p>"
        );
    }

    private void sendPasswordResetEmail(User user, string origin)
    {
        if (string.IsNullOrEmpty(origin))
        {
            throw new AppException("Invalid origin when sending password reset email. " + origin);
        }
        var resetUrl = $"{origin}/reset-password?token={user.ResetToken}";
        string message = $@"<p>Hello {user.Name},</p>
                        <p>Please click the below link to reset your password, the link will be valid for 1 day.</p>
                        <p><a href=""{resetUrl}"">{resetUrl}</a></p>
                        <p>If you didn't request this email or have any questions please contact us at <a href=""mailto:support@recipelewis.com"">support@recipelewis.com</a></p>
                        <p>Thank you,<br>
                        <p>Recipe Lewis</p>";

        _emailService.Send(
            to: user.Email,
            subject: "Recipe Lewis - Reset Password",
            html: $@"<h4>Recipe Lewis - Reset Password</h4>
                        {message}"
        );
    }

    private void sendRequestEditAccessEmail(User user)
    {
        string message;
        message = $@"<p>The user {user.Email} has requested edit access. UserId = {user.UserId}</p>";
        _emailService.Send(
            to: "support@recipelewis.com",
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
    public List<UserModel> GetAll()
    {
        var users = _dbContext.Users.Where(x => x.DeletedDateTime == null);
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
