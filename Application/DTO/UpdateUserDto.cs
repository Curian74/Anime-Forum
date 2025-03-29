using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class UpdateUserDto
    {
        public Guid Id { get; set; }
        public string? Bio { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
