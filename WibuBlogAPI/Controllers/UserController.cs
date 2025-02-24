using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTO;
using Infrastructure.Extensions;
using Microsoft.Extensions.Options;
using Infrastructure.Configurations;
using Domain.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WibuBlogAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController(UserServices userServices, TicketServices ticketServices, JwtHelper jwtHelper, IOptions<AuthTokenOptions> authTokenOptions) : ControllerBase
    {
        private readonly UserServices _userServices = userServices;
        private readonly TicketServices _ticketServices = ticketServices;
        private readonly JwtHelper _jwtHelper = jwtHelper;
        private readonly AuthTokenOptions _authTokenOptions = authTokenOptions.Value;

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            Request.Cookies.TryGetValue(_authTokenOptions.Name, out string? authToken);

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
                Expires = DateTime.UtcNow.AddHours(_authTokenOptions.Expires),
                Secure = _authTokenOptions.Secure,
                HttpOnly = _authTokenOptions.HttpOnly,
                SameSite = _authTokenOptions.SameSite,
            };

            Response.Cookies.Append(_authTokenOptions.Name, token, cookieOptions);

            return new JsonResult(Ok("Login approved"));
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            Request.Cookies.TryGetValue(_authTokenOptions.Name, out string? authToken);

            if (authToken == null || !_jwtHelper.IsValidToken(authToken))
            {
                return new JsonResult(BadRequest("User is not logged in"));
            }

            Response.Cookies.Delete(_authTokenOptions.Name);

            return new JsonResult(Ok("Logged out"));
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            Request.Cookies.TryGetValue(_authTokenOptions.Name, out string? authToken);

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
            Request.Cookies.TryGetValue(_authTokenOptions.Name, out string? authToken);

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
        [HttpGet]
        public async Task<IActionResult> GetUserTickets()
        {
            Request.Cookies.TryGetValue(_authTokenOptions.Name, out string? authToken);
            if (authToken == null || !_jwtHelper.IsValidToken(authToken))
            {
                return new JsonResult(BadRequest("User is not logged in"));
            }
            var userId = (Guid)JwtHelper.ExtractUserIdFromToken(authToken);
            var tickets = await _ticketServices.GetUserTickets(userId);
            return new JsonResult(Ok(tickets));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket(CreateTicketDto dto)
        {
            Request.Cookies.TryGetValue(_authTokenOptions.Name, out string? authToken);
            if (authToken == null || !_jwtHelper.IsValidToken(authToken))
            {
                return new JsonResult(BadRequest("User is not logged in"));
            }
            var userId = (Guid)JwtHelper.ExtractUserIdFromToken(authToken);
            dto.UserId = userId;
            var result = await _ticketServices.CreateTicket(dto);
            return new JsonResult(Ok(result));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTicket(UpdateTicketDto dto)
        {
            Request.Cookies.TryGetValue(_authTokenOptions.Name, out string? authToken);
            if (authToken == null || !_jwtHelper.IsValidToken(authToken))
            {
                return new JsonResult(BadRequest("User is not logged in"));
            }
            var userId = (Guid)JwtHelper.ExtractUserIdFromToken(authToken);
            var result = await _ticketServices.UpdateTicket(dto, userId);
            if (!result)
            {
                return new JsonResult(NotFound("Ticket not found or unauthorized"));
            }
            return new JsonResult(Ok("Ticket updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(Guid id)
        {
            Request.Cookies.TryGetValue(_authTokenOptions.Name, out string? authToken);
            if (authToken == null || !_jwtHelper.IsValidToken(authToken))
            {
                return new JsonResult(BadRequest("User is not logged in"));
            }
            var userId = (Guid)JwtHelper.ExtractUserIdFromToken(authToken);
            var result = await _ticketServices.DeleteTicket(id, userId);
            if (!result)
            {
                return new JsonResult(NotFound("Ticket not found or unauthorized"));
            }
            return new JsonResult(Ok("Ticket deleted successfully"));
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
                var result = await _userServices.GetProfileDetails(userId);
                if(result == null)
                {
                     return new JsonResult(NotFound());
                }
                return new JsonResult(Ok(result));
        }
    }
}
