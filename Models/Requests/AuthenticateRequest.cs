using System.ComponentModel.DataAnnotations;

namespace RecipeLewis.Models.Requests;
public class AuthenticateRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}
public record class RevokeTokenRequest(string Token);
