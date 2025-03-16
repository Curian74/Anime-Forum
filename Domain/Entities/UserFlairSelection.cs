using Domain.Common.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;
namespace Domain.Entities
{
    public class UserFlairSelection : BaseAuditableEntity
    {

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserFlair))]
        public Guid FlairId { get; set; }

        // Navigation properties
        public virtual UserFlair? Flair { get; set; }
    }
}
