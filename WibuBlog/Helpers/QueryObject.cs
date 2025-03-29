namespace WibuBlog.Helpers
{
    public class QueryObject
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public string FilterBy { get; set; } = string.Empty;
        public string SearchTerm { get; set; } = string.Empty;
        public string? TagFilter { get; set; }
        public string OrderBy {  get; set; } = string.Empty;
        public bool Descending { get; set; } = false;
        public Guid PostCategoryId { get; set; }

    }
}
