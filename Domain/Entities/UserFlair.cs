using Domain.Common.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class UserFlair : BaseAuditableEntity
    {
        public string ColorHex { get; set; } = string.Empty; // Example: FF0000 (Red)

        // Navigation properties
        public Media Image { get; set; } = null!;
    }
}
