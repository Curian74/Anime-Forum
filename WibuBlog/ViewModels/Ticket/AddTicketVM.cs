using System.ComponentModel.DataAnnotations;

namespace WibuBlog.ViewModels.Ticket
{
    public class AddTicketVM
    {
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;

        public string? Note { get; set; }
    }
}