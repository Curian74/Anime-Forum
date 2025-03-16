using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WibuBlogAPI.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class MediaController(MediaService mediaService) : ControllerBase
	{
		private readonly MediaService _mediaService = mediaService;

		[HttpPost]
		public async Task<IActionResult> Add([FromBody] Media media)
		{
			Media result = await _mediaService.AddAsync(media);
			return new JsonResult(result);
		}
	}
}
