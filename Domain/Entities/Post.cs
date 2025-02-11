namespace Domain.Entities
{
    public class Post
    {
        public Guid id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

    }
}
