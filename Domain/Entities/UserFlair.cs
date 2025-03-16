using Domain.Common.BaseEntities;

namespace Domain.Entities
{
    public class UserFlair : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;

        public string ColorHex { get; set; } = string.Empty; // Example: FF0000 (Red)

        public int PointsRequired { get; set; }
    }
}
