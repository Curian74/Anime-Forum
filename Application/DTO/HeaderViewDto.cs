using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
	public class HeaderViewDto
	{
		public UserProfileDto User { get; set; }
		public List<Notification> Notifications { get; set; } = new();
	}
}
