using Domain.Common.BaseEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Notification : BaseAuditableEntity
	{
        public string Content { get; set; }
		public virtual User? User { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(Post))]
        public Guid? PostId { get; set; }
		public bool IsDeleted { get; set; }	
	}
}
