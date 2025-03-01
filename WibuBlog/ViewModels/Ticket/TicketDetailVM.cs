namespace WibuBlog.ViewModels.Ticket
{
    public class TicketDetailVM
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string? Content { get; set; }
        public string Tag { get; set; } = null!;
        public bool? IsApproved { get; set; }
    }
}
