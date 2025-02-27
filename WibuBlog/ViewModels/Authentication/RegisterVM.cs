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
        public string email {  get; set; }
        public string password { get; set; }


    }
}
