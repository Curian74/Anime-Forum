using System.ComponentModel.DataAnnotations;

namespace WibuBlog.ViewModels.Ticket
{
    public class AddTicketVM
    {
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;

        public string Tag { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}