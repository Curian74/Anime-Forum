using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Comment
{
    public class PostCommentDto
    {
        public required Guid UserId { get; set; }
        public required Guid PostId { get; set; }
        [Required]
        public required string Content { get; set; }
    }
}
