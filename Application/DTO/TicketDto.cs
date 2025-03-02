using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Content { get; set; }
        public bool? Approved { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? LastModifiedAt { get; set; }
        public string LastModifiedBy { get; set; }

        public string Email { get; set; }
        public string Tag { get; set; }
    }
}
