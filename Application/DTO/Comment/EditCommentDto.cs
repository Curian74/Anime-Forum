using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Comment
{
    public class EditCommentDto
    {
        [Required]
        public string? Content { get; set; }

    }
}
