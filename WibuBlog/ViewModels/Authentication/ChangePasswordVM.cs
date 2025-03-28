using Application.Common.MessageOperations;
using System.ComponentModel.DataAnnotations;

namespace WibuBlog.ViewModels.Authentication
{
    public class ChangePasswordVM
    {
       
        public string? Email { get; set; }

        [Required(ErrorMessage = MessageConstants.ME002)]
        [MinLength(6, ErrorMessage = MessageConstants.MEN014)]
        [MaxLength(20, ErrorMessage = MessageConstants.MEN014)]

        public string NewPassword { get; set; }

        [Required(ErrorMessage = MessageConstants.ME002)]
        [MinLength(6, ErrorMessage = MessageConstants.MEN014)]
        [MaxLength(20, ErrorMessage = MessageConstants.MEN014)]
        public string ConfirmPassword { get; set; }
    }
}
