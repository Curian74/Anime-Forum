using Application.Common.MessageOperations;
using System.ComponentModel.DataAnnotations;

namespace WibuBlog.ViewModels.Authentication
{
    public class OtpVM
    {
        [Required(ErrorMessage = MessageConstants.ME002)]
        [MaxLength(6, ErrorMessage = MessageConstants.MEN005)]
        [MinLength(6, ErrorMessage = MessageConstants.MEN005)]
        public string OTP { get; set; }
    }
}
