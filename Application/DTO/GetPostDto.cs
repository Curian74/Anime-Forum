using Domain.Entities;

namespace Application.DTO
{
    public class GetPostDto
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Media Media { get; set; }
        public int Views { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public Guid LastModifiedBy { get; set; }
        public ICollection<Vote> Votes { get; set; }
        public ICollection<Domain.Entities.Comment> Comments { get; set; }
        public ICollection<Report> Reports { get; set; }
    }
}
