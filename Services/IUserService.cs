using Database;
using RecipeLewis.Models.Results;
using RecipeLewis.Models;
using RecipeLewis.Models.Requests;

namespace RecipeLewis.Services
{
    public interface IUserService
    {
        public List<UserModel> SearchUsers(UserId userId, string query);
        public UserModel? GetUser(UserId userId);

        public UserModel? GetUserByGUID(Guid userGUID, UserId userId);
        public Task<bool> UpdateProfile(UpdateUserProfileRequest request, UserId userId);
        public Task<bool> LoginUser(LoginUserRequest request, string ipAddress, UserId userId);
        public Task<bool> LogoutUser(UserId userId);
        public Task<bool> DeleteUser(UserId userId);
        public Task<bool> HardDeleteUser(UserId userId);
        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
        public void RevokeToken(string token, string ipAddress);
        public AuthenticateResponse RefreshToken(string token, string ipAddress);
    }
}
