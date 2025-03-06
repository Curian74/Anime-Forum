using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class EditUserDto
    {
        public Guid userId { get; set; }
        public string field { get; set; }
        public string value { get; set; }
    }
}
