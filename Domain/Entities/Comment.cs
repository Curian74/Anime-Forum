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

        public bool Hidden { get; set; } = false;

        // Navigation properties
        public User User { get; set; } = null!;

        public Post Post { get; set; } = null!;
    }
}
