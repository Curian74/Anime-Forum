using Domain.Entities;

namespace WibuBlog.ViewModels.Report
{
    public class AddReportVM
    {
        public Guid? UserId { get; set; }
        public Guid PostId { get; set; }
        public string? Reason { get; set; }
    }
}
