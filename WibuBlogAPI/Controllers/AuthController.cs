using Application.DTO;
using Application.Services;
using Infrastructure.Configurations;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WibuBlogAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController(UserService userService, JwtHelper jwtHelper, IOptions<AuthTokenOptions> authTokenOptions) : ControllerBase
    {
        private readonly UserService _userService = userService;
        private readonly JwtHelper _jwtHelper = jwtHelper;
        private readonly AuthTokenOptions _authTokenOptions = authTokenOptions.Value;

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (IsLoggedIn())
            {
                return new JsonResult(BadRequest("User is already logged in"));
            }

            var result = await _userService.Login(dto);

            if (result == false)
            {
                return new JsonResult(Challenge("Invalid credentials"));
            }

            var user = await _userService.FindByLoginAsync(dto);

            var token = await _jwtHelper.GenerateJwtToken(user);

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddHours(_authTokenOptions.Expires),
                Secure = _authTokenOptions.Secure,
                HttpOnly = _authTokenOptions.HttpOnly,
                SameSite = _authTokenOptions.SameSite,
                IsEssential = _authTokenOptions.IsEssential
            };

            Response.Cookies.Append(_authTokenOptions.Name, token, cookieOptions);

            return new JsonResult(Ok("Login approved"));
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (IsLoggedIn())
            {
                return new JsonResult(BadRequest("User is already logged in"));
            }

            var result = await _userService.Register(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);

        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete(_authTokenOptions.Name);

            return new JsonResult(Ok("Logged out"));
        }

        private bool IsLoggedIn()
        {
            Request.Cookies.TryGetValue(_authTokenOptions.Name, out string? authToken);

            if (string.IsNullOrEmpty(authToken) && Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var bearerToken = authHeader.FirstOrDefault();
                if (!string.IsNullOrEmpty(bearerToken) && bearerToken.StartsWith("Bearer "))
                {
                    authToken = bearerToken["Bearer ".Length..]; // Strip "Bearer " prefix
                }
            }

            return !string.IsNullOrEmpty(authToken) && _jwtHelper.IsValidToken(authToken);
        }
    }
}
