using Application.Common.Pagination;

namespace WibuBlog.ViewModels.Post
{
    public class PostDetailVM
    {
        public Domain.Entities.Post? Post { get; set; }
        public PagedResult<Domain.Entities.Comment>? Comments { get; set; }
        public Guid? UserId {  get; set; }
    }
}
