﻿using static Domain.ValueObjects.Enums.TicketStatusEnum;

namespace Application.DTO
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Content { get; set; }
        public TicketStatus Status { get; set; } = TicketStatus.Pending;
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? LastModifiedAt { get; set; }
        public string LastModifiedBy { get; set; }

        public string Email { get; set; }
        public string Tag { get; set; }
    }
}
