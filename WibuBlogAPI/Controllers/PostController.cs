using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTO;
using System.ComponentModel.DataAnnotations;
using Domain.Entities;
using System.Linq.Expressions;
using Infrastructure.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WibuBlogAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PostController(PostServices postServices) : ControllerBase
    {
        private readonly PostServices _postServices = postServices;

        [HttpGet]
        public async Task<IActionResult> GetAll(
            string? filterBy = null,
            string? searchTerm = null,
            string? orderBy = null,
            bool descending = false)
        {
            Expression<Func<Post, bool>>? filter = ExpressionBuilder.BuildFilterExpression<Post>(filterBy, searchTerm);
            Func<IQueryable<Post>, IOrderedQueryable<Post>>? orderExpression = ExpressionBuilder.BuildOrderExpression<Post>(orderBy, descending);

            var result = await _postServices.GetAllAsync(filter, orderExpression);

            return new JsonResult(Ok(result.Items));
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            int page = 1,
            int size = 10,
            string? filterBy = null,
            string? searchTerm = null,
            string? orderBy = null,
            bool descending = false)
        {
            Expression<Func<Post, bool>>? filter = ExpressionBuilder.BuildFilterExpression<Post>(filterBy, searchTerm);
            Func<IQueryable<Post>, IOrderedQueryable<Post>>? orderExpression = ExpressionBuilder.BuildOrderExpression<Post>(orderBy, descending);

            var result = await _postServices.GetPagedAsync(page, size, filter, orderExpression);

            return new JsonResult(Ok(result));
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> Get(Guid postId)
        {
            var result = await _postServices.GetByIdAsync(postId);

            if (result == null)
            {
                return new JsonResult(NotFound());
            }

            return new JsonResult(Ok(result));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePostDto dto)
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
        public async Task<IActionResult> Update(Guid postId, [FromBody] PostDto dto)
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
        public async Task<IActionResult> Delete(Guid postId)
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

        [HttpDelete]
        public async Task<IActionResult> DeleteWhere(
            string filterBy,
            string searchTerm)
        {
            Expression<Func<Post, bool>>? filter = ExpressionBuilder.BuildFilterExpression<Post>(filterBy, searchTerm);

            await _postServices.DeletePostWhereAsync(filter);

            return new JsonResult(NoContent());
        }
    }
}
