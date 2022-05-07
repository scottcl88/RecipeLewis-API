using Database;
using RecipeLewis.Models;
using RecipeLewis.Models.Requests;

namespace RecipeLewis.Services
{
    public interface IUserService
    {
        public List<UserModel> SearchUsers(SubUserId userId, string query);
        public User? GetUser(SubUserId userId);

        public User? GetUserByGUID(Guid userGUID, SubUserId userId);
        public Task<bool> UpdateProfile(UpdateUserProfileRequest request, SubUserId userId);
        public Task<bool> UpdateHelp(UpdateUserHelpRequest request, SubUserId userId);
        public Task<bool> LoginUser(LoginUserRequest request, string ipAddress, SubUserId userId);
        public Task<bool> UpdateUserSeenSetupScreens(SubUserId userId);
        public Task<bool> UpdateDeviceToken(string deviceToken, SubUserId userId);
        public Task<bool> LogoutUser(SubUserId userId);
        public Task<bool> DeleteUser(SubUserId userId);
        public Task<bool> HardDeleteUser(SubUserId userId);
    }
}