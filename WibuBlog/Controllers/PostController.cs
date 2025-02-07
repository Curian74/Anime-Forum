using Domain.Common.BaseEntities;
using Domain.Common.Extensions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;

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

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("api"); //Lay client api tu program

            var response = await client.GetAsync("Post/Get"); //endpoint build tu baseAddress

            if(!response.IsSuccessStatusCode)
            {
                return BadRequest("Ngu");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            //Goi extension custom
            var wrapper = DeserializeExtensions.Deserialize<ApiResponse<Post>>(jsonResponse);

            return View("Index", wrapper!.Value);
        }
    }
}
