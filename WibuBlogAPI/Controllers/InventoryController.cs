using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WibuBlogAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InventoryController(InventoryService inventoryService) : ControllerBase
    {
        private readonly InventoryService _inventoryService = inventoryService;

        [HttpGet]
        public async Task<IActionResult> GetUserInventory()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var inventory = await _inventoryService.GetUserInventoryAsync(userId);

            return new JsonResult(Ok(inventory));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetActiveFlair(Guid? userId)
        {
            if (userId == null)
            {
                userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }

            var activeFlair = await _inventoryService.GetActiveFlairAsync(userId);

            if (activeFlair == null)
            {
                return new JsonResult(NotFound());
            }

            return new JsonResult(Ok(activeFlair));
        }

        [HttpPost]
        public async Task<IActionResult> SetActiveFlair(Guid flairId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var success = await _inventoryService.SetActiveFlairAsync(userId, flairId);

            if (!success)
            {
                return new JsonResult(BadRequest());
            }

            return new JsonResult(Ok());
        }
    }
}
