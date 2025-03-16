using Domain.Common.BaseEntities;

namespace Domain.Entities
{
    public class PostCategory : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;

        public bool IsRestricted { get; set; } = false;

        // Navigation properties
        public virtual ICollection<Post>? Posts { get; set; } = [];
    }
}
