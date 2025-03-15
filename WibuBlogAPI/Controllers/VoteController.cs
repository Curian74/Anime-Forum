using Application.DTO;
using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WibuBlogAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VoteController(VoteService voteService) : ControllerBase
    {
        private readonly VoteService _voteService = voteService;

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTotalPostVotes(Guid postId)
        {
            try
            {
                var totalPostVotes = await _voteService.GetTotalPostVotesAsync(postId);
                return new JsonResult(Ok(totalPostVotes));
            }
            catch (ArgumentNullException e)
            {
                return new JsonResult(BadRequest(e.Message));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentUserVote(Guid postId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return new JsonResult(NotFound());
            }

            var currentUserVote = await _voteService.GetCurrentUserVoteAsync(postId, Guid.Parse(userIdClaim));

            return new JsonResult(Ok(currentUserVote));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleVote([FromBody]VoteDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return new JsonResult(BadRequest());
            }

            try
            {
                var result = await _voteService.ToggleVoteAsync(dto, Guid.Parse(userIdClaim));
                return new JsonResult(Ok(result));
            }
            catch (ArgumentNullException e)
            {
                return new JsonResult(BadRequest(e.Message));
            }
        }
    }
}
