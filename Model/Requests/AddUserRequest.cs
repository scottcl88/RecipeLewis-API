namespace RecipeLewis.Models.Requests
{
    public class UpdateUserProfileRequest
    {
        public string? Name { get; set; }
        public bool AllowNotifications { get; set; }
        public bool ShowFeedback { get; set; }
        public int MaxRadius { get; set; }
        public bool SubscribeMarketingEmail { get; set; }
    }
    public class UpdateUserHelpRequest
    {
        public bool SeenRatings { get; set; }
        public bool SeenHomeTutorial { get; set; }
        public bool SeenResultsTutorial { get; set; }
        public bool SeenInviteTutorial { get; set; }
        public bool SeenStartOutingTutorial { get; set; }
    }

    public class LoginUserRequest
    {
        public string? DeviceId { get; set; }
        public string? DeviceToken { get; set; }
        public string? TimeZone { get; set; }
        public int UtcOffset { get; set; }
        public string? EmailAddress { get; set; }
        public bool ShowFeedback { get; set; }
        public string? Model { get; set; }
        public string? Manufacturer { get; set; }
        public string? OperatingSystem { get; set; }
        public string? OperatingSystemVersion { get; set; }
        public string? Platform { get; set; }
        public string? WebViewVersion { get; set; }
        public bool IsVirtual { get; set; }
    }

    public class LoginUserResult
    {
        public bool Success { get; set; }
        public bool SeenStartupScreen { get; set; }
        public bool ShowFeedback { get; set; }
        public bool ShowAds { get; set; } = true;
    }

    public class InviteUserRequest
    {
        public string? Name { get; set; }
    }
}