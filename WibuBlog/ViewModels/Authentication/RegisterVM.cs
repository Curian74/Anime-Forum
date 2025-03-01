using Application.Common.MessageOperations;
using System.ComponentModel.DataAnnotations;

namespace WibuBlog.ViewModels.Authentication
{
    public class RegisterVM
    {

        [Required(ErrorMessage = MessageConstants.ME002)]
        [StringLength(50, ErrorMessage = MessageConstants.MEN001)]
        [MinLength(5, ErrorMessage = MessageConstants.MEN002)]
        public string username {  get; set; }

        [Required(ErrorMessage = MessageConstants.ME002)]
        [EmailAddress(ErrorMessage = MessageConstants.ME009)]
        public string email {  get; set; }

        [Required(ErrorMessage = MessageConstants.ME002)]
        [MinLength(6, ErrorMessage = MessageConstants.ME007)]
        public string password { get; set; }


    }
}
