using Domain.Common.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class UserInventory : BaseAuditableEntity
    {
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }


        // Navigation properties
        public virtual ICollection<UserFlair>? Flairs { get; set; } = [];

        // public User? User { get; set; }
    }
}
