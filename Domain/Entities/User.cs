using Domain.Common.BaseEntities;

namespace Domain.Entities
{
    public class User : BaseUser
    {
        public int Points { get; set; } = 0;

        public bool IsBanned { get; set; } = false;

        public DateTime? DateOfBirth { get; set; }

        public string? Bio { get; set; }

        // Navigation properties
        public Rank? Rank { get; set; }

        public Media? ProfilePhoto { get; set; }

        public UserInventory UserInventory { get; set; } = null!;

        public UserFlairSelection? UserFlairSelection { get; set; }
    }
}
