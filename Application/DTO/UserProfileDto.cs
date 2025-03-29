using System.ComponentModel.DataAnnotations;
using Application.Common.MessageOperations;
using Domain.Entities;
namespace Application.DTO
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = MessageConstants.ME002)]
        [StringLength(50, ErrorMessage = MessageConstants.MEN001)]
        [MinLength(5, ErrorMessage = MessageConstants.MEN002)]
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; }

        public Media? ProfilePhoto { get; set; }

        [MinLength(0, ErrorMessage = MessageConstants.MEO003)]
        public string? Bio { get; set; }
        public int? Points { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public bool? TwoFactorEnabled { get; set; }
        public Rank? Rank { get; set; }
        public IList<string>? Roles { get; set; }
        public List<Domain.Entities.Post>? PostList { get; set; }
    }
}
