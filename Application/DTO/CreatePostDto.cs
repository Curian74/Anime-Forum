using Domain.Entities;

namespace Application.DTO
{
    public class CreatePostDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid? UserId { get; set; } 
        public Guid? PostCategoryId { get; set; } 
        public Media? Media { get; set; }
    }
}
