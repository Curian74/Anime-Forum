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
    public class AuthController(UserServices userServices, JwtHelper jwtHelper, IOptions<AuthTokenOptions> authTokenOptions) : ControllerBase
    {
        private readonly UserServices _userServices = userServices;
        private readonly JwtHelper _jwtHelper = jwtHelper;
        private readonly AuthTokenOptions _authTokenOptions = authTokenOptions.Value;

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (IsLoggedIn())
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
                Expires = DateTime.Now.AddHours(_authTokenOptions.Expires),
                Secure = _authTokenOptions.Secure,
                HttpOnly = _authTokenOptions.HttpOnly,
                SameSite = _authTokenOptions.SameSite,
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

            var result = await _userServices.Register(dto);

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

            if (authToken != null && _jwtHelper.IsValidToken(authToken))
            {
                return true;
            }

            return false;
        }
    }
}
