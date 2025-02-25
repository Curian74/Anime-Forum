
using Application.Common.Pagination;

namespace WibuBlog.ViewModels.Home
{
    public class HomeVM
    {
        public PagedResult<Domain.Entities.Post>? RecentPosts { get; set; }
        public List<Domain.Entities.PostCategory>? RestrictedCategories { get; set; }
        public List<Domain.Entities.Post>? Posts { get; set; }
        public List<Domain.Entities.PostCategory>? NonRestrictedCategories { get; set; }
    }
}
