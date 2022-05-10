using System.Text.Json.Serialization;

namespace RecipeLewis.Models.Results;
public class AuthenticateResponse
{
    public UserId UserId { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string JwtToken { get; set; }

    [JsonIgnore] // refresh token is returned in http only cookie
    public string RefreshToken { get; set; }

    public AuthenticateResponse(UserModel user, string jwtToken, string refreshToken)
    {
        UserId = user.UserId;
        Username = user.Username;
        Name = user?.Name;
        JwtToken = jwtToken;
        RefreshToken = refreshToken;
    }
}