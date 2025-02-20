using Domain.Common.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class UserInventory : BaseAuditableEntity
    {
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public ICollection<UserFlair> Flairs { get; set; } = [];

        // Navigation properties
        public User User { get; set; } = null!;
    }
}
