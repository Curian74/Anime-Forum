using Application.Services;
using Domain.Entities;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace WibuBlogAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommentController(CommentSerivces commentSerivces) : ControllerBase
    {
        private readonly CommentSerivces _commentSerivces = commentSerivces;

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            string? filterBy = null,
            string? searchTerm = null,
            string? orderBy = null,
            bool descending = false)
        {
            Expression<Func<Comment, bool>>? filter = ExpressionBuilder.BuildFilterExpression<Comment>(filterBy, searchTerm);
            Func<IQueryable<Comment>, IOrderedQueryable<Comment>>? orderExpression = ExpressionBuilder.BuildOrderExpression<Comment>(orderBy, descending);

            var result = await _commentSerivces.GetAllAsync(filter, orderExpression);

            return new JsonResult(Ok(result.Items));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetPaged(
            int page = 1,
            int size = 10,
            string? filterBy = null,
            string? searchTerm = null,
            string? orderBy = null,
            bool descending = false)
        {
            Expression<Func<Comment, bool>>? filter = ExpressionBuilder.BuildFilterExpression<Comment>(filterBy, searchTerm);
            Func<IQueryable<Comment>, IOrderedQueryable<Comment>>? orderExpression = ExpressionBuilder.BuildOrderExpression<Comment>(orderBy, descending);

            var result = await _commentSerivces.GetPagedAsync(page, size, filter, orderExpression);

            return new JsonResult(Ok(result));
        }
    }
}
