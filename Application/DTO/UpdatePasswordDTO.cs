﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
	public class UpdatePasswordDTO
	{
		public Guid UserId { get; set; }
		public string OldPassword { get; set; }
		public string NewPassword { get; set; }
		public string ConfirmPassword { get; set; }
	}
}
