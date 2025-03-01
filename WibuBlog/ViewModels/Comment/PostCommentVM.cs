using System.ComponentModel.DataAnnotations;

namespace WibuBlog.ViewModels.Comment
{
    public class PostCommentVM
    {
        [Required]
        public required string Content { get; set; }
        [Required]
        public required Guid PostId { get; set; }
        [Required]
        public required Guid UserId { get; set; }
    }
}
