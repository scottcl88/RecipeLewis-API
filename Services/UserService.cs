using AutoMapper;
using Database;
using RecipeLewis.Models;
using RecipeLewis.Models.Requests;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using RecipeLewis.Models.Results;

namespace RecipeLewis.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        private IJwtUtils _jwtUtils;
        private readonly AppSettings _appSettings;
        private readonly LogService _logService;
        private readonly IMapper _mapper;

        public UserService(ApplicationDbContext dbContext, LogService logService, IMapper mapper)
        {
            _dbContext = dbContext;
            _logService = logService;
            _mapper = mapper;
        }
        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var user = _dbContext.Users.SingleOrDefault(x => x.Username == model.Username);

            // validate
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                throw new AppException("Username or password is incorrect");

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = _jwtUtils.GenerateJwtToken(user);
            var refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            user.RefreshTokens.Add(refreshToken);

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

        public List<UserModel> SearchUsers(UserId userId, string query)
        {
            var users = _dbContext.Users.Where(x => x.UserId != userId.Value && (x.Name.ToLower().Contains(query) || x.Email.ToLower().Contains(query)) && x.DeletedDateTime == null);
            var userModels = _mapper.Map<List<UserModel>>(users.ToList());
            return userModels;
        }

        public UserModel? GetUser(UserId userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.UserId == userId.Value && x.DeletedDateTime == null);
            if (user == null)
            {
                _logService.Error("GetUser failed - Unable to find user", userId);
            }
            var userModel = _mapper.Map<UserModel>(user);
            return userModel;
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

        public async Task<bool> UpdateProfile(UpdateUserProfileRequest request, UserId userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.UserId == userId.Value && x.DeletedDateTime == null);
            if (user == null)
            {
                _logService.Error("UpdateProfile failed - Unable to find user", userId);
                return false;
            }
            user.Name = request.Name;
            user.ModifiedDateTime = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LoginUser(LoginUserRequest request, string ipAddress, UserId userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.UserId == userId.Value);
            if (user != null)
            {
                if (user.DeletedDateTime != null)
                {
                    _logService.Info("User attempted to login failed - User was deleted", userId, ipAddress);
                    return false;
                }
                user.TimeZone = request.TimeZone;
                user.UtcOffset = request.UtcOffset;
                user.ModifiedDateTime = DateTime.UtcNow;
                user.LastLogin = DateTime.UtcNow;
                user.Email = request.EmailAddress;
                user.LastIPAddress = ipAddress;
            }
            else
            {
                var newUser = new User()
                {
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow,
                    LastLogin = DateTime.UtcNow,
                    UserGUID = Guid.NewGuid(),
                    Email = request.EmailAddress,
                    TimeZone = request.TimeZone,
                    UtcOffset = request.UtcOffset,
                    LastIPAddress = ipAddress,
                };
                await _dbContext.AddAsync(newUser);
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserSeenSetupScreens(UserId userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.UserId == userId.Value && x.DeletedDateTime == null);
            if (user == null)
            {
                _logService.Error("UpdateUserSeenSetupScreens failed - Unable to find user", userId);
                return false;
            }
            user.ModifiedDateTime = DateTime.UtcNow;
            user.LastLogout = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateDeviceToken(string deviceToken, UserId userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.UserId == userId.Value && x.DeletedDateTime == null);
            if (user == null)
            {
                _logService.Error("UpdateDeviceToken failed - Unable to find user", userId);
                return false;
            }
            user.ModifiedDateTime = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LogoutUser(UserId userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.UserId == userId.Value && x.DeletedDateTime == null);
            if (user == null)
            {
                _logService.Error("LogoutUser failed finding user", userId);
                return false;
            }
            user.ModifiedDateTime = DateTime.UtcNow;
            user.LastLogout = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUser(UserId userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.UserId == userId.Value && x.DeletedDateTime == null);
            if (user == null)
            {
                _logService.Error("DeleteUser failed finding user", userId);
                return false;
            }
            user.DeletedDateTime = DateTime.UtcNow;
            user.LastLogout = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HardDeleteUser(UserId userId)
        {
            _logService.Info("Deleting userId: ", userId);
            var userIdParam = new SqlParameter("string", userId.Value);
            await _dbContext.Database.ExecuteSqlRawAsync("EXEC [dbo].[HardDeleteUserAccount] @string", userIdParam);
            return true;
        }
    }
}
