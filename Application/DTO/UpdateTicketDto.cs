using static Domain.ValueObjects.Enums.TicketStatusEnum;

namespace Application.DTO
{
    public class UpdateTicketDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Tag { get; set; }
        public TicketStatus Status { get; set; } = TicketStatus.Pending;
        public string? Note { get; set; }
    }
}
