using Domain.Common.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Ticket : BaseAuditableEntity
    {
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public string? Content { get; set; }
        public string Tag { get; set; } = null!;
        public string Email { get; set; } = null!;

        public bool? IsApproved { get; set; } = null;

        public string? Note { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }
    }
}
