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

        public User User { get; set; } = null!;

        public UserFlair Flair { get; set; } = null!;
    }
}
