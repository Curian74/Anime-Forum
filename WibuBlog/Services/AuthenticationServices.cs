using WibuBlog.Interfaces.Api;
using Application.DTO;
using WibuBlog.ViewModels.Authentication;
using Microsoft.Extensions.Options;
using Infrastructure.Configurations;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using WibuBlog.Common.ApiResponse;

namespace WibuBlog.Services
{
    public class AuthenticationServices(IApiServices apiService, IHttpContextAccessor httpContextAccessor, IOptions<AuthTokenOptions> authTokenOptions)
    {
        private readonly IApiServices _apiService = apiService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly AuthTokenOptions _authTokenOptions = authTokenOptions.Value;
        private readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7186/api/Auth") };

        public async Task<bool> AuthorizeLogin(LoginVM loginVM)
        {
            var loginDTO = new LoginDto
            {
                Login = loginVM.Login,
                Password = loginVM.Password
            };

            var response = await _httpClient.PostAsJsonAsync($"Auth/Login", loginDTO);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                var cookie = cookies.FirstOrDefault();
                if (!string.IsNullOrEmpty(cookie))
                {
                    // Parse cookie name and value
                    var cookieParts = cookie.Split(';')[0].Split('=');
                    if (cookieParts.Length == 2)
                    {
                        //var cookieName = cookieParts[0];
                        var cookieValue = cookieParts[1];

                        // Attach cookie to the response so it reaches the browser
                        _httpContextAccessor.HttpContext?.Response.Cookies.Append(_authTokenOptions.Name, cookieValue, new CookieOptions
                        {
                            HttpOnly = _authTokenOptions.HttpOnly,
                            Secure = _authTokenOptions.Secure,
                            SameSite = _authTokenOptions.SameSite,
                            Expires = DateTimeOffset.UtcNow.AddHours(_authTokenOptions.Expires)
                        });
                    }
                }
            }

            return true;
        }

        public async Task<IdentityResult> AuthorizeRegister(RegisterVM registerVM)
        {
            var RegisterDTO = new RegisterDto
            {
                UserName = registerVM.username,
                Email = registerVM.email,
                Password = registerVM.password

            };
            var response = await _apiService.PostAsync<ApiResponse<IdentityResult>>($"User/Register", RegisterDTO);
            return response.Value!;
        }

        public async Task<bool> AuthorizeLogout()
        {
            var authToken = _httpContextAccessor.HttpContext?.Request.Cookies[_authTokenOptions.Name];

            if (!string.IsNullOrEmpty(authToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            }

            var response = await _httpClient.PostAsync("Auth/Logout", null);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(_authTokenOptions.Name);
            return true;
        }
    }
}
