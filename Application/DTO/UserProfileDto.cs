﻿using System.ComponentModel.DataAnnotations;
using Application.Common.MessageOperations;
namespace Application.DTO
{
    public class UserProfileDto
    {

        [Required(ErrorMessage = MessageConstants.ME002)]
        [StringLength(50, ErrorMessage = MessageConstants.MEN001)]
        [MinLength(5, ErrorMessage = MessageConstants.MEN002)]
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public bool? TwoFactorEnabled { get; set; }
        public IList<string>? Roles { get; set; }
    }
}
