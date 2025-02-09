using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTO;
using System.ComponentModel.DataAnnotations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WibuBlogAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PostController(PostServices postServices) : ControllerBase
    {
        private readonly PostServices _postServices = postServices;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _postServices.GetAllAsync();

            return new JsonResult(Ok(result));
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(int page, int size = 10)
        {
            var result = await _postServices.GetPagedAsync(page, size);

            return new JsonResult(Ok(result));
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> Get(int postId)
        {
            var result = await _postServices.GetByIdAsync(postId);

            if (result == null)
            {
                return new JsonResult(NotFound());
            }

            return new JsonResult(Ok(result));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PostDto dto)
        {
            try 
            { 
                await _postServices.CreatePostAsync(dto); 
            } 
            catch (ValidationException ex)
            {
                return new JsonResult(BadRequest($"{ex.GetType().Name}: {ex.Message}"));
            }

            return new JsonResult(Created());
        }

        [HttpPut("{postId}")]
        public async Task<IActionResult> Update(int postId, [FromBody] PostDto dto)
        {
            try
            {
                await _postServices.UpdatePostAsync(postId, dto);
            }
            catch (KeyNotFoundException ex)
            {
                return new JsonResult(NotFound($"{ex.GetType().Name}: {ex.Message}"));
            }

            return new JsonResult(Accepted(dto));

        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> Delete(int postId)
        {
            try
            {
                await _postServices.DeletePostAsync(postId);
            }
            catch (KeyNotFoundException ex)
            {
                return new JsonResult(NotFound($"{ex.GetType().Name}: {ex.Message}"));
            }

            return new JsonResult(NoContent());
        }
    }
}
