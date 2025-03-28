﻿using WibuBlog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WibuBlog.ViewModels.Users;
using Application.Common.MessageOperations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Application.Common.Pagination;
using Domain.Entities;
using System.Security.Claims;
using Application.Services;

namespace WibuBlog.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
    public class UserController(Services.UserService userService, IWebHostEnvironment webHostEnvironment,
        Services.RankService rankService, Services.RoleService roleService) : Controller
    {
        private readonly Services.UserService _userService = userService;
        private readonly Services.RankService _rankService = rankService;
        private readonly Services.RoleService _roleService = roleService;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile(Guid? userId = null)
        {
            var result = await _userService.GetUserProfile(userId);

            if (result is null)
            {
                return NotFound();
            }

            return View(result);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminPolicy")]
        public async Task<IActionResult> UserList(UserQueryVM? query)
        {
            var usersList = await _userService.GetAllAsync("", "", false);
            var ranksList = await _rankService.GetAllAsync("", "", "name", false);

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var filteredUsers = usersList.Where(x => x.Id != Guid.Parse(currentUserId));

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                filteredUsers = filteredUsers.Where(x => x.UserName
                .Contains(query.SearchTerm.Trim()) || x.Email
                .Contains(query.SearchTerm.Trim())).ToList();
            }

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                filteredUsers = filteredUsers.OrderByDescending(x => x.Points);
            }

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                filteredUsers = filteredUsers.OrderByDescending(x => x.Points);
            }

                if (query.SelectedRankId.HasValue)
            {
                filteredUsers = filteredUsers.Where(x => x.RankId == query.SelectedRankId).ToList();
            }

            filteredUsers = filteredUsers.Where(x => x.IsBanned == query.IsBanned).ToList();

            int totalItems = filteredUsers.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);
            int skip = (query.Page - 1) * query.PageSize;

            var pagedUsers = filteredUsers.Skip(skip).Take(query.PageSize).ToList();

            var userListVM = new UserListVM
            {
                UsersList = new PagedResult<User>(pagedUsers, filteredUsers.Count(), query.Page, query.PageSize),
                RanksList = ranksList.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList(),
            };

            var userRoles = new Dictionary<Guid, List<string>>();

            // Load roles for each user
            foreach (var user in pagedUsers)
            {
                var roles = await _roleService.GetUserRoleAsync(user.Id);
                userRoles[user.Id] = roles;
            }

            userListVM.UserRoles = userRoles;

            return View(userListVM);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(UpdateUserVM updateUserVM)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(UserProfile));
            }

            var user = await _userService.UpdateUserAsync(updateUserVM);
            return RedirectToAction(nameof(UserProfile));
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordVM model)
        {
            if (model.NewPassword != model.ConfirmPassword)
            {
                return BadRequest(MessageConstants.ME006);
            }
            var response = await _userService.UpdatePassword(model);
            if (response.Succeeded)
            {
                return Ok(MessageConstants.ME007a);
            }
            return BadRequest(response.Errors[0].Description);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfilePhoto(IFormFile file)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _userService.UpdateProfilePhoto(file, userId);
            return RedirectToAction(nameof(UserProfile));

        }

        [HttpGet("MemberProfile/{userId}")]
		[AllowAnonymous]
		public async Task<IActionResult> MemberProfile(Guid? userId = null)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != null && userId.ToString() == currentUserId )
            {
                return RedirectToAction(nameof(UserProfile));
            }
            var response = await _userService.GetMemberProfile(userId);
            return View(response);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminPolicy")]
        public async Task<IActionResult> ToggleModeratorAsync(Guid userId)
        {
            await _userService.ToggleModeratorRoleAsync(userId);

            return RedirectToAction(nameof(UserList));
        }
    }
}
