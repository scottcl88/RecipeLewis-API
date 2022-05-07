using Database;
using RecipeLewis.Models;
using RecipeLewis.Models.Requests;

namespace RecipeLewis.Services
{
    public interface IUserService
    {
        public List<UserModel> SearchUsers(UserId userId, string query);
        public User? GetUser(UserId userId);

        public User? GetUserByGUID(Guid userGUID, UserId userId);
        public Task<bool> UpdateProfile(UpdateUserProfileRequest request, UserId userId);
        public Task<bool> LoginUser(LoginUserRequest request, string ipAddress, UserId userId);
        public Task<bool> LogoutUser(UserId userId);
        public Task<bool> DeleteUser(UserId userId);
        public Task<bool> HardDeleteUser(UserId userId);
    }
}
