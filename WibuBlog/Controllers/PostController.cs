using Application.Common.Pagination;
using Domain.Common.BaseEntities;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace WibuBlog.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;

        public PostController(ApplicationDbContext dbContext, IHttpClientFactory httpClientFactory)
        {
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
        }

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
            var data = DeserializeExtensions.Deserialize<BaseApiResponse<PagedResult<Post>>>(jsonResponse);

            return View("Index", data.Value);
        }
    }
}
