using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class UpdateTicketDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Tag { get; set; }
        public bool? IsApproved { get; set; }
        public string? Note { get; set; }
    }
}
