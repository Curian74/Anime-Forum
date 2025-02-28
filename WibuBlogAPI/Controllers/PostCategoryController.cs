using Application.Services;
using Domain.Entities;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace WibuBlogAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PostCategoryController(PostCategoryService postCategoryService) : ControllerBase
    {
        private readonly PostCategoryService _postCategoryService = postCategoryService;

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            string? filterBy = null,
            string? searchTerm = null,
            string? orderBy = null,
            bool descending = false)
        {
            Expression<Func<PostCategory, bool>>? filter = ExpressionBuilder.BuildFilterExpression<PostCategory>(filterBy, searchTerm);
            Func<IQueryable<PostCategory>, IOrderedQueryable<PostCategory>>? orderExpression = ExpressionBuilder.BuildOrderExpression<PostCategory>(orderBy, descending);

            var result = await _postCategoryService.GetAllAsync(filter, orderExpression);

            return new JsonResult(Ok(result.Items));
        }
    }
}
