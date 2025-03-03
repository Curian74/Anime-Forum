namespace Domain.Common.BaseEntities
{
    public class BaseAuditableEntity : BaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; } = DateTime.Now;
        public Guid? LastModifiedBy { get; set; }
    }
}
