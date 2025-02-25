using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WibuBlog.Models;
using WibuBlog.Services;
using WibuBlog.ViewModels.Home;

namespace WibuBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly PostServices _postService;
        private readonly PostCategoryServices _postCategoryServices;

        public HomeController(PostServices postService, PostCategoryServices postCategoryServices)
        {
            _postService = postService;
            _postCategoryServices = postCategoryServices;
        }

        public async Task<IActionResult> Index()
        {
            var restrictedCategories = await _postCategoryServices.GetAllCategories("isRestricted", "true" , false);
            var nonrestrictedCategories = await _postCategoryServices.GetAllCategories("isRestricted", "false" , false);
            var recentPosts = await _postService.GetPagedPostAsync(1, 5, "CreatedAt", true);
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
