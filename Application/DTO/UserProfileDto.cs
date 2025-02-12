namespace Application.DTO
{
    public class UserProfileDto
    {
        public string? UserName { get; set; }
        public string? Email {  get; set; }
        public bool? EmailConfirmed { get; set; }
        public string? PhoneNumber {  get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public bool? TwoFactorEnabled { get; set; }
        public IList<string>? Roles { get; set; }
    }
}
