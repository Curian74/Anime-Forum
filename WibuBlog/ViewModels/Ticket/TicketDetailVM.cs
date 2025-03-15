using static Domain.ValueObjects.Enums.TicketStatusEnum;

namespace WibuBlog.ViewModels.Ticket
{
    public class TicketDetailVM
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string? Content { get; set; }
        public string Tag { get; set; } = null!;
        public TicketStatus Status { get; set; }
        public string? Note { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastModifiedAt { get; set; } = DateTime.Now;
   
    }
}
