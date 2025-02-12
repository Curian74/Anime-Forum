using Application.Common.Pagination;
using Domain.Entities;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Common.ApiResponse;

namespace WibuBlog.Controllers
{
    public class PostController(IHttpClientFactory httpClientFactory) : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var client = _httpClientFactory.CreateClient("api"); //Lay client api tu program

            //Goi api(chi can truyen vao url sau api/)
            var response = await client.GetAsync($"Post/GetPaged?page={page}&size={pageSize}");

            if(!response.IsSuccessStatusCode)
            {
                return BadRequest(response);
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            //Goi extension custom
            var data = DeserializeExtensions.Deserialize<ApiResponse<PagedResult<Post>>>(jsonResponse);

            return View("Index", data.Value);
        }
    }
}
