using Domain.Common.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Report : BaseAuditableEntity
    {
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public string? Reason { get; set; }

        [ForeignKey(nameof(Post))]
        public Guid PostId { get; set; }

        public bool? IsApproved {  get; set; }

        public string Note { get; set; } = string.Empty;

        // Navigation properties
        public virtual User? User { get; set; }

        public virtual Post? Post { get; set; }
               public virtual PostCategory? Category { get; set; }
    }
}
