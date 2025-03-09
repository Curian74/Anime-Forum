using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class UpdateUserDto
    {
        public Guid userId { get; set; }
        public string bio { get; set; }
        public string phone { get; set; }
    }
}
