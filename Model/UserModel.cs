namespace RecipeLewis.Models
{
    public class SubUserId
    {
        public string? Value { get; set; }
    }

    public class UserModel : EntityDataModel
    {
        public long? UserID { get; set; }
        public SubUserId? SubUserId { get; set; }
        public Guid? UserGUID { get; set; }
        public string? LastIPAddress { get; set; }
        public string? FirebaseUID { get; set; }
        public string? DeviceToken { get; set; }
        public string? DeviceId { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? LastLogout { get; set; }
        public string? TimeZone { get; set; }
        public int? UtcOffset { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public int? MaxRadius { get; set; } = 25;
        public bool? AllowNotifications { get; set; }
        public bool? SeenStartupScreen { get; set; }
        public bool? ShowFeedback { get; set; }
        public bool? SubscribeMarketingEmail { get; set; }
        public bool? AlreadyInvited { get; set; }
        public bool ShowAds { get; set; } = true;
    }
}