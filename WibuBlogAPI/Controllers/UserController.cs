using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTO;
using Infrastructure.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WibuBlogAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController(UserServices userServices, JwtHelper jwtHelper) : ControllerBase
    {
        private readonly UserServices _userServices = userServices;
        private readonly JwtHelper _jwtHelper = jwtHelper;

        private readonly string authTokenName = "AnimeForumAuthToken";

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            Request.Cookies.TryGetValue(authTokenName, out string? authToken);

            if (authToken != null || _jwtHelper.IsValidToken(authToken))
            {
                return new JsonResult(BadRequest("User is already logged in"));
            }

            var result = await _userServices.Login(dto);

            if (result == false)
            {
                return new JsonResult(Challenge("Invalid credentials"));
            }

            var user = await _userServices.FindByLoginAsync(dto);

            var token = await _jwtHelper.GenerateJwtToken(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            };

            Response.Cookies.Append(authTokenName, token, cookieOptions);

            return new JsonResult(Ok("Login approved"));
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            Request.Cookies.TryGetValue(authTokenName, out string? authToken);

            if (authToken == null || !_jwtHelper.IsValidToken(authToken))
            {
                return new JsonResult(BadRequest("User is not logged in"));
            }

            Response.Cookies.Delete(authTokenName);

            return new JsonResult(Ok("Logged out"));
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            Request.Cookies.TryGetValue(authTokenName, out string? authToken);

            if (authToken != null || _jwtHelper.IsValidToken(authToken))
            {
                return new JsonResult(BadRequest("User is already logged in"));
            }

            var result = await _userServices.Register(dto);

            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAccountDetails()
        {
            Request.Cookies.TryGetValue(authTokenName, out string? authToken);

            if (authToken == null || !_jwtHelper.IsValidToken(authToken))
            {
                return new JsonResult(BadRequest("User is not logged in"));
            }

            var id = (Guid)JwtHelper.ExtractUserIdFromToken(authToken);

            var result = await _userServices.GetProfileDetails(id);

            if (result == null)
            {
                return new JsonResult(NotFound());
            }

            return new JsonResult(result);
        }
    }
}
