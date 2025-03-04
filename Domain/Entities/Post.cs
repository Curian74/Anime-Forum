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

        public ICollection<Vote> Votes { get; set; } = [];

        public ICollection<Comment> Comments { get; set; } = [];

        public ICollection<Report> Reports { get; set; } = [];

        public int Views {  get; set; }

        // Navigation properties
        public User? User { get; set; }
        public PostCategory? Category { get; set; }
        public Media? Media { get; set; }
    }
}
