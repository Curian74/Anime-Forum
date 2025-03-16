namespace WibuBlog.ViewModels.Users
{
	public class UpdatePasswordVM
	{
		public Guid UserId { get; set; }
		public string OldPassword { get; set; }
		public string NewPassword { get; set; }
		public string ConfirmPassword { get; set; }

	}
}
