using Domain.Common.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Vote : BaseAuditableEntity
    {
        public bool IsUpvote { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(Post))]
        public Guid PostId { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }

        public virtual Post? Post { get; set; }
    }
}
