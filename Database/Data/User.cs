using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Database;
public class User : EntityData
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }
    public Guid? UserGUID { get; set; }
    public string LastIPAddress { get; set; }
    public DateTime LastLogin { get; set; }
    public DateTime LastLogout { get; set; }
    public string TimeZone { get; set; }
    public int UtcOffset { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    [JsonIgnore]
    public string PasswordHash { get; set; }

    [JsonIgnore]
    public virtual List<RefreshToken> RefreshTokens { get; set; }
}

