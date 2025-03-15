using Application.Common.MessageOperations;
using System.ComponentModel.DataAnnotations;
namespace WibuBlog.ViewModels.Users
{
    public class UpdateUserVM 
    {
        public Guid UserId { get; set; }
        public string Bio {  get; set; }
        [Phone(ErrorMessage = MessageConstants.MEN011)]
        public string Phone { get; set; }
    }
}
