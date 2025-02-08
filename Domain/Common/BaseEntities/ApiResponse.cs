using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.BaseEntities
{
    public class ApiResponse<T>
    {
        public List<T>? Value { get; set; } //Mac dinh thuoc tinh value de lay data tu json
    }
}
