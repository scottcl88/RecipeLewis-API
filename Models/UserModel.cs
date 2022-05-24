using System.Text.Json.Serialization;

namespace RecipeLewis.Models;
public enum Role
{
    Admin,
    User
}
public record class UserId(int Value);

public class UserModel : EntityDataModel
{
    public UserId UserId { get; set; }
    public Guid? UserGUID { get; set; }
    public string? LastIPAddress { get; set; }
    public DateTime? LastLogin { get; set; }
    public DateTime? LastLogout { get; set; }
    public string? TimeZone { get; set; }
    public int? UtcOffset { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public Role Role { get; set; }
    [JsonIgnore]
    public List<RefreshTokenModel>? RefreshTokens { get; set; }
    public bool OwnsToken(string token)
    {
        return this.RefreshTokens?.Find(x => x.Token == token) != null;
    }
}
