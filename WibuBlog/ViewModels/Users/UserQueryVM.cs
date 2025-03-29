namespace WibuBlog.ViewModels.Users
{
    public class UserQueryVM
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; } = string.Empty;
        public Guid? SelectedRankId { get; set; }
        public bool IsInactive { get; set; } = true;
        public bool? IsBanned { get; set; } = false;
        public string? SortBy { get; set; } = string.Empty;
    }
}
