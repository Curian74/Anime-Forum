using Domain.Common.BaseEntities;

namespace Domain.Entities
{
    public class Rank : BaseAuditableEntity
    {
        public string Name {  get; set; } = string.Empty;
        
        public int PointsRequired { get; set; }
    }
}
