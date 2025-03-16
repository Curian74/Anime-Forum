using Domain.Common.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Post : BaseAuditableEntity
    {
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(PostCategory))]
        public Guid? PostCategoryId { get; set; }

        public bool IsHidden { get; set; } = false;

        public int TotalVotes { get; set; } = 0;

        public int Views {  get; set; }

        // Navigation properties
        public virtual ICollection<Vote>? Votes { get; set; } = [];

        public virtual ICollection<Comment>? Comments { get; set; } = [];

        public virtual ICollection<Report>? Reports { get; set; } = [];

        public virtual User? User { get; set; }

        public virtual PostCategory? Category { get; set; }

        public virtual Media? Media { get; set; }
    }
}
