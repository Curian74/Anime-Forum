using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WibuBlog.Services;
using WibuBlog.ViewModels.Home;

namespace WibuBlog.Controllers
{
    public class HomeController(PostService postService, PostCategoryService postCategoryService) : Controller
    {
        private readonly PostService _postService = postService;
        private readonly PostCategoryService _postCategoryService = postCategoryService;

        public async Task<IActionResult> Index()
        {
            var restrictedCategories = await _postCategoryService.GetAllCategories("isRestricted", "true" , false);
            var nonrestrictedCategories = await _postCategoryService.GetAllCategories("isRestricted", "false" , false);
            var recentPosts = await _postService.GetPagedPostAsync(1, 5, "", "", "CreatedAt", true);
            var postList = await _postService.GetAllPostAsync("", "", false);

            HomeVM homeVM = new HomeVM
            {
                RestrictedCategories = restrictedCategories,
                RecentPosts = recentPosts,
                Posts = postList,
                NonRestrictedCategories = nonrestrictedCategories
            };

            return View(homeVM);
        }
    }
}
