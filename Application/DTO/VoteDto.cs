namespace Application.DTO
{
    public class VoteDto
    {
        public Guid PostId { get; set; }
        public bool IsUpvote { get; set; }
    }
}
