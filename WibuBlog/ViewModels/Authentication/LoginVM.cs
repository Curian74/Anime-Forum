using Application.Common.MessageOperations;
using System.ComponentModel.DataAnnotations;

namespace WibuBlog.ViewModels.Authentication
{
    public class LoginVM
    {
        [Required(ErrorMessage = MessageConstants.ME002)]
        public string Login { get; set; }
        [Required(ErrorMessage = MessageConstants.ME002)]
        public string Password { get; set; }
    }
}
