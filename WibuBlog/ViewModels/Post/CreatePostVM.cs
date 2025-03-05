using Application.Common.MessageOperations;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WibuBlog.ViewModels.Post
{
    public class CreatePostVM
    {
        public List<SelectListItem>? CategoryList { get; set; }
        [Required(ErrorMessage = "Please select a category.")]
        public Guid PostCategoryId { get; set; }
        public User? User { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = MessageConstants.ME012)]
        public string? Content { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = MessageConstants.MEN009)]
        [MaxLength(255, ErrorMessage = MessageConstants.ME012)]
        public string? Title { get; set; }
    }
}
