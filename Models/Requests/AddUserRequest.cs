namespace RecipeLewis.Models.Requests
{
    public class UpdateUserProfileRequest
    {
        public string? Name { get; set; }
    }

    public class LoginUserRequest
    {
        public string? TimeZone { get; set; }
        public int UtcOffset { get; set; }
        public string? EmailAddress { get; set; }
    }

    public class LoginUserResult
    {
        public bool Success { get; set; }
    }

    public class InviteUserRequest
    {
        public string? Name { get; set; }
    }

}