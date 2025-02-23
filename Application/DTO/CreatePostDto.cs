using Domain.Entities;

namespace Application.DTO
{
    public class CreatePostDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public Media? Media { get; set; }
    }
}
