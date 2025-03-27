namespace WibuBlog.ViewModels.Users
{
    public class ChangePasswordVM
    {
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
        public string confirmPassword { get; set; }
    }
}
