namespace Application.DTO
{
    public class CreateTicketDto
    {
        public Guid UserId { get; set; }
        public string Content { get; set; } = null!;
        public string Tag { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

}
