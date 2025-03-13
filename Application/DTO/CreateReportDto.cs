using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class CreateReportDto
    {
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public string? Reason { get; set; }
    }
}
