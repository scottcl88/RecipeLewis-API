using System.Text.Json.Serialization;

namespace RecipeLewis.Models.Results;
public class AuthenticateResponse
{
    public UserId UserId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public Role Role { get; set; }
    public bool IsVerified { get; set; }
    public string? Token { get; set; }

    [JsonIgnore] // refresh token is returned in http only cookie
    public string? RefreshToken { get; set; }

    public AuthenticateResponse(UserModel user, string jwtToken, string refreshToken)
    {
        UserId = user.UserId;
        Email = user.Email;
        Name = user?.Name;
        Role = user.Role;
        Token = jwtToken;
        RefreshToken = refreshToken;
    }
}