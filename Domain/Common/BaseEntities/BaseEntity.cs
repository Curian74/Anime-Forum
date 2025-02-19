using System.ComponentModel.DataAnnotations;

namespace Domain.Common.BaseEntities
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
