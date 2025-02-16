﻿using Microsoft.AspNetCore.Mvc;
using WibuBlog.Services;
using WibuBlog.ViewModels.Post;
using Domain.Entities;

namespace WibuBlog.Controllers
{
    public class PostController(PostServices postService) : Controller
    {
        private readonly PostServices _postService = postService;

        public async Task<IActionResult> Index(int? page = 1, int? pageSize = 5)
        {
            var value = await _postService.GetPagedPostAsync(page, pageSize);
            return View("Index", value);
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
                var isSuccess = await _postService.UpdatePostAsync(id, post);

                if (!isSuccess)
                {
                    ModelState.AddModelError("", "Failed to update post.");
                    return View(post);
                }
            }

            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "*De message loi vao day.*");
                return View(post);
            }

            return RedirectToAction(nameof(Update), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            _ = await _postService.DeletePostAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
