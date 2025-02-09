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
            var wrapper = DeserializeExtensions.Deserialize<BaseApiResponse<Post>>(jsonResponse);

            return View("Index", wrapper!.Value);
        }
    }
}
