using WibuBlog.Interfaces.Api;
using WibuBlog.Common.ApiResponse;
using Microsoft.AspNetCore.Mvc;
using Application.DTO;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using WibuBlog.ViewModels.Authentication;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace WibuBlog.Services
{
    public class AuthenticationServices(IApiServices apiService)
    {
        private readonly IApiServices _apiService = apiService;

        public async Task<ApiResponse<JsonResult>> AuthorizeLogin(LoginVM loginVM)
        {
            var loginDTO = new LoginDto
            {
                Login = loginVM.Login,
                Password = loginVM.Password
            };
            var response = await _apiService.PostAsync<ApiResponse<JsonResult>>($"Auth/Login",loginDTO);
            return response;
        }

        public async Task<IdentityResult> AuthorizeRegister(RegisterVM registerVM) 
        {
            var RegisterDTO = new RegisterDto
            {
                UserName = registerVM.username,
                Email = registerVM.email,
                Password = registerVM.password

            };
            var response = await _apiService.PostAsync<ApiResponse<IdentityResult>>($"Auth/Register", RegisterDTO);
            return response.Value!;
        }

    }
}
