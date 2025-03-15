using Domain.Common.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class User : BaseUser
    {
        public int Points { get; set; } = 0;

        public bool IsBanned { get; set; } = false;

        public DateTime? DateOfBirth { get; set; }

        public string? Bio { get; set; }

        [ForeignKey(nameof(Rank))]
        public Guid? RankId { get; set; }

        // Navigation properties
        public virtual Rank? Rank { get; set; }

        public virtual Media? ProfilePhoto { get; set; }

        public virtual UserInventory? UserInventory { get; set; }

        public virtual UserFlairSelection? UserFlairSelection { get; set; }
    }
}
