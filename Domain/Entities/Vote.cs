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
        public User? User { get; set; } = null!;

        public Post? Post { get; set; } = null!;
    }
}
