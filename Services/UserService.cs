using AutoMapper;
using Database;
using RecipeLewis.Models;
using RecipeLewis.Models.Requests;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace RecipeLewis.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly LogService _logService;
        private readonly IMapper _mapper;

        public UserService(ApplicationDbContext dbContext, LogService logService, IMapper mapper)
        {
            _dbContext = dbContext;
            _logService = logService;
            _mapper = mapper;
        }

        public List<UserModel> SearchUsers(UserId userId, string query)
        {
            var users = _dbContext.Users.Where(x => x.UserId != userId.Value && (x.Name.ToLower().Contains(query) || x.Email.ToLower().Contains(query)) && x.DeletedDateTime == null);
            var userModels = _mapper.Map<List<UserModel>>(users.ToList());
            return userModels;
        }

        public User? GetUser(UserId userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.UserId == userId.Value && x.DeletedDateTime == null);
            if (user == null)
            {
                _logService.Error("GetUser failed - Unable to find user", userId);
            }
            return user;
        }

        public User? GetUserByGUID(Guid userGUID, UserId userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.UserGUID == userGUID && x.DeletedDateTime == null);
            if (user == null)
            {
                _logService.Error("GetUserByGUID failed - Unable to find user", userId, userGUID);
            }
            return user;
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
