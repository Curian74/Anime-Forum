using Domain.Common.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Comment : BaseAuditableEntity
    {
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(Post))]
        public Guid PostId { get; set; }

        public string Content { get; set; } = string.Empty;

        public bool IsHidden { get; set; } = false;

        // Navigation properties
        public virtual User? User { get; set; }

        public virtual Post? Post { get; set; }
    }
}
