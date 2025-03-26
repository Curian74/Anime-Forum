using Application.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using WibuBlog.Services;

namespace WibuBlog.Views.Shared.ViewComponents
{
	public class HeaderViewComponent(UserService userService) : ViewComponent
	{
		private readonly UserService _userService = userService;

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var user = await _userService.GetUserProfile();
			var notifications = await _userService.GetUserNotifications();

			var model = new HeaderViewDto
			{
				User = user,
				Notifications = notifications.Notifications
			};

			return View(model);
		}
	}
}
