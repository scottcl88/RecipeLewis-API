using System.Text.Json.Serialization;

namespace RecipeLewis.Models;

public class UserId
{
    public int Value { get; set; }
}

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
    public string Username { get; set; }
    [JsonIgnore]
    public List<RefreshTokenModel> RefreshTokens { get; set; }
}
