using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class ReportDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public string? Reason { get; set; }
        public bool? IsApproved { get; set; }
        public string Note { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PostTitle { get; set; } = string.Empty;
    }
}