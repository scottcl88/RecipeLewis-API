using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RecipeLewis.Models;

public class RefreshTokenRequest
{
    public string Token { get; set; }
}

public class RevokeTokenRequest
{
    public string Token { get; set; }
}

public class ValidateResetTokenRequest
{
    [Required]
    public string Token { get; set; }
}

public class RefreshTokenModel : EntityDataModel
{
    [JsonIgnore]
    public int Id { get; set; }

    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; }
    public string CreatedByIp { get; set; }
    public DateTime? Revoked { get; set; }
    public string RevokedByIp { get; set; }
    public string ReplacedByToken { get; set; }
    public string ReasonRevoked { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsRevoked => Revoked != null;
    public bool IsActive => !IsRevoked && !IsExpired;
}