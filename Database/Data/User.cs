using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Database;
public enum Role
{
    Unknown,
    User,
    Editor,
    Admin
}
public class User : EntityData
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }
    public Guid? UserGUID { get; set; }
    public string LastIPAddress { get; set; }
    public DateTime LastLogin { get; set; }
    public DateTime LastLogout { get; set; }
    public string? TimeZone { get; set; }
    public int? UtcOffset { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public Role Role { get; set; }
    public string? VerificationToken { get; set; }
    public DateTime? Verified { get; set; }
    [JsonIgnore]
    public string PasswordHash { get; set; }
    public bool IsVerified => Verified.HasValue || PasswordReset.HasValue;
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpires { get; set; }
    public DateTime? PasswordReset { get; set; }
    public bool RequestedAccess { get; set; }
    public DateTime? RequestedAccessExpires { get; set; }

    [JsonIgnore]
    public virtual List<RefreshToken> RefreshTokens { get; set; }
}

