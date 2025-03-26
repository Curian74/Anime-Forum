using Domain.Common.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public enum NotiType { Profile, Post }
    public class Notification : BaseAuditableEntity
	{
        public NotiType NotiType { get; set; }
        public string Content { get; set; }
		public virtual User? User { get; set; }

		public bool IsDeleted { get; set; }	
	}
}
