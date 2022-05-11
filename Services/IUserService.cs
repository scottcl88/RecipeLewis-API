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
        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
        public void RevokeToken(string token, string ipAddress);
        public AuthenticateResponse RefreshToken(string token, string ipAddress);
        public UserModel Create(CreateRequest model);
        public UserModel Update(UserId id, UpdateRequest model);
        public void Delete(UserId id);
        public void ResetPassword(ResetPasswordRequest model);
        public void ValidateResetToken(ValidateResetTokenRequest model);
        public void ForgotPassword(ForgotPasswordRequest model, string origin);
        public void VerifyEmail(string token);
        public void Register(RegisterRequest model, string ipAddress, string origin);
    }
}
