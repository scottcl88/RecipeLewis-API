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
        public AuthenticateResponse Authenticate(AuthenticateRequest request, string ipAddress);
        public void RevokeToken(string token, string ipAddress);
        public AuthenticateResponse RefreshToken(string token, string ipAddress);
        public UserModel Create(CreateUserRequest request);
        public UserModel Update(UserId id, UpdateUserRequest request);
        public void Delete(UserId id);
        public void ResetPassword(ResetPasswordRequest request);
        public void ValidateResetToken(ValidateResetTokenRequest request);
        public void ForgotPassword(ForgotPasswordRequest request, string origin);
        public void VerifyEmail(string token);
        public void Register(RegisterRequest request, string ipAddress, string origin);
    }
}
