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

        public List<UserModel> SearchUsers(SubUserId userId, string query)
        {
            var users = _dbContext.Users.Where(x => x.SubUserId.Value != userId.Value && (x.Name.ToLower().Contains(query) || x.Email.ToLower().Contains(query)) && x.DeletedDateTime == null);
            var userModels = _mapper.Map<List<UserModel>>(users.ToList());
            return userModels;
        }

        public User? GetUser(SubUserId userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.SubUserId.Value == userId.Value && x.DeletedDateTime == null);
            if (user == null)
            {
                _logService.Error("GetUser failed - Unable to find user", userId);
            }
            return user;
        }

        public User? GetUserByGUID(Guid userGUID, SubUserId userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.UserGUID == userGUID && x.DeletedDateTime == null);
            if (user == null)
            {
                _logService.Error("GetUserByGUID failed - Unable to find user", userId, userGUID);
            }
            return user;
        }

        public async Task<bool> UpdateProfile(UpdateUserProfileRequest request, SubUserId userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.SubUserId.Value == userId.Value && x.DeletedDateTime == null);
            if (user == null)
            {
                _logService.Error("UpdateProfile failed - Unable to find user", userId);
                return false;
            }
            user.AllowNotifications = request.AllowNotifications;
            user.Name = request.Name;
            user.ShowFeedback = request.ShowFeedback;
            user.SubscribeMarketingEmail = request.SubscribeMarketingEmail;
            if (request.MaxRadius > 0)
            {
                user.MaxRadius = request.MaxRadius;
            }
            user.ModifiedDateTime = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateHelp(UpdateUserHelpRequest request, SubUserId userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.SubUserId.Value == userId.Value && x.DeletedDateTime == null);
            if (user == null)
            {
                _logService.Error("UpdateHelp failed - Unable to find user", userId);
                return false;
            }
            if (user.Help == null)
            {
                user.Help = new Help();
            }
            user.Help.SeenRatings = request.SeenRatings;
            user.Help.SeenHomeTutorial = request.SeenHomeTutorial;
            user.Help.SeenInviteTutorial = request.SeenInviteTutorial;
            user.Help.SeenResultsTutorial = request.SeenResultsTutorial;
            user.Help.SeenStartOutingTutorial = request.SeenStartOutingTutorial;
            user.Help.ModifiedDateTime = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LoginUser(LoginUserRequest request, string ipAddress, SubUserId userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.SubUserId.Value == userId.Value);
            if (user != null)
            {
                if (user.DeletedDateTime != null)
                {
                    _logService.Info("User attempted to login failed - User was deleted", userId, ipAddress);
                    return false;
                }
                user.TimeZone = request.TimeZone;
                user.UtcOffset = request.UtcOffset;
                if (request.DeviceToken != null)
                {
                    user.DeviceToken = request.DeviceToken;
                }
                user.ModifiedDateTime = DateTime.UtcNow;
                user.LastLogin = DateTime.UtcNow;
                user.Email = request.EmailAddress;
                user.LastIPAddress = ipAddress;
                user.DeviceId = request.DeviceId;

                if (user.DeviceInfo == null)
                {
                    user.DeviceInfo = new UserDeviceInfo();
                    user.DeviceInfo.CreatedDateTime = DateTime.UtcNow;
                    user.DeviceInfo.ModifiedDateTime = DateTime.UtcNow;
                    user.DeviceInfo.Model = request.Model;
                    user.DeviceInfo.Manufacturer = request.Manufacturer;
                    user.DeviceInfo.OperatingSystem = request.OperatingSystem;
                    user.DeviceInfo.OperatingSystemVersion = request.OperatingSystemVersion;
                    user.DeviceInfo.Platform = request.Platform;
                    user.DeviceInfo.WebViewVersion = request.WebViewVersion;
                    user.DeviceInfo.IsVirtual = request.IsVirtual;
                }
            }
            else
            {
                var newUser = new User()
                {
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow,
                    LastLogin = DateTime.UtcNow,
                    SubUserId = new SubUserIdEntity() { Value = userId.Value },
                    DeviceToken = request.DeviceToken,
                    UserGUID = Guid.NewGuid(),
                    Email = request.EmailAddress,
                    MaxRadius = 25,
                    TimeZone = request.TimeZone,
                    UtcOffset = request.UtcOffset,
                    ShowFeedback = true,
                    ShowAds = true,
                    LastIPAddress = ipAddress,
                    DeviceId = request.DeviceId
                };
                newUser.DeviceInfo = new UserDeviceInfo();
                newUser.DeviceInfo.CreatedDateTime = DateTime.UtcNow;
                newUser.DeviceInfo.ModifiedDateTime = DateTime.UtcNow;
                newUser.DeviceInfo.Model = request.Model;
                newUser.DeviceInfo.Manufacturer = request.Manufacturer;
                newUser.DeviceInfo.OperatingSystem = request.OperatingSystem;
                newUser.DeviceInfo.OperatingSystemVersion = request.OperatingSystemVersion;
                newUser.DeviceInfo.Platform = request.Platform;
                newUser.DeviceInfo.WebViewVersion = request.WebViewVersion;
                newUser.DeviceInfo.IsVirtual = request.IsVirtual;
                await _dbContext.AddAsync(newUser);
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserSeenSetupScreens(SubUserId userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.SubUserId.Value == userId.Value && x.DeletedDateTime == null);
            if (user == null)
            {
                _logService.Error("UpdateUserSeenSetupScreens failed - Unable to find user", userId);
                return false;
            }
            user.SeenStartupScreen = true;
            user.ModifiedDateTime = DateTime.UtcNow;
            user.LastLogout = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateDeviceToken(string deviceToken, SubUserId userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.SubUserId.Value == userId.Value && x.DeletedDateTime == null);
            if (user == null)
            {
                _logService.Error("UpdateDeviceToken failed - Unable to find user", userId);
                return false;
            }
            user.DeviceToken = deviceToken;
            user.ModifiedDateTime = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LogoutUser(SubUserId userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.SubUserId.Value == userId.Value && x.DeletedDateTime == null);
            if (user == null)
            {
                _logService.Error("LogoutUser failed finding user", userId);
                return false;
            }
            user.DeviceToken = null;
            user.ModifiedDateTime = DateTime.UtcNow;
            user.LastLogout = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUser(SubUserId userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.SubUserId.Value == userId.Value && x.DeletedDateTime == null);
            if (user == null)
            {
                _logService.Error("DeleteUser failed finding user", userId);
                return false;
            }
            user.DeviceToken = null;
            user.DeletedDateTime = DateTime.UtcNow;
            user.LastLogout = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HardDeleteUser(SubUserId userId)
        {
            _logService.Info("Deleting userId: ", userId);
            var userIdParam = new SqlParameter("string", userId.Value);
            await _dbContext.Database.ExecuteSqlRawAsync("EXEC [dbo].[HardDeleteUserAccount] @string", userIdParam);
            return true;
        }
    }
}