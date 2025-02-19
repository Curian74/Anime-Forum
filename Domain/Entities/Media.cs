using Domain.Common.BaseEntities;

namespace Domain.Entities
{
    public class Media : BaseAuditableEntity
    {
        public string Url { get; set; } = string.Empty;
    }
}
