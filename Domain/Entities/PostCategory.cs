using Domain.Common.BaseEntities;

namespace Domain.Entities
{
    public class PostCategory : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<Post> Posts { get; set; } = [];
        public bool IsRestricted { get; set; } = false;
    }
}
