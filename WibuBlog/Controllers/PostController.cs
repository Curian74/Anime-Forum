using Microsoft.AspNetCore.Mvc;
using WibuBlog.Services;
using WibuBlog.ViewModels.Post;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using WibuBlog.Helpers;

namespace WibuBlog.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
    public class PostController(PostServices postService) : Controller
    {
        private readonly PostServices _postService = postService;

        [AllowAnonymous]
        public async Task<IActionResult> Index(int? page = 1, int? pageSize = 5)
        {
            var value = await _postService.GetPagedPostAsync(page, pageSize, "", "", "", false);
            return View("Index", value);
        }

        [AllowAnonymous]
        public async Task<IActionResult> NewPosts([FromQuery] QueryObject queryObject)
        {
            var value = await _postService.GetPagedPostAsync(queryObject.Page, queryObject.Size,
                queryObject.FilterBy, queryObject.SearchTerm, queryObject.OrderBy, queryObject.Descending);

            return View(value);
        }

        
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddPostVM addPostVM)
        {
            if (!ModelState.IsValid)
            {
                return View(addPostVM);
            }

            try
            {
                await _postService.AddNewPostAsync(addPostVM);
                return RedirectToAction(nameof(Index));
            }

            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "*De message loi vao day.*");
                return View(addPostVM);
            }

        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var post = await _postService.GetPostByIdAsync(id);

            if (post is null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Guid id, Post post)
        {
            if (!ModelState.IsValid)
            {
                return View(post);
            }

            try
            {
                var data = await _postService.UpdatePostAsync(id, post);

                return RedirectToAction(nameof(Update), new { id });
            }

            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "*De message loi vao day.*");
                return View(post);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            _ = await _postService.DeletePostAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
