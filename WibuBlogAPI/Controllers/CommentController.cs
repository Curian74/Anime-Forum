using Application.DTO;
using Application.DTO.Comment;
using Application.Services;
using Domain.Entities;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace WibuBlogAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommentController(CommentService commentService) : ControllerBase
    {
        private readonly CommentService _commentService = commentService;

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

            var result = await _commentService.GetAllAsync(filter, orderExpression);

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

            var result = await _commentService.GetPagedAsync(page, size, filter, orderExpression);

            return new JsonResult(Ok(result));
        }

        [HttpPost]
        public async Task<IActionResult> PostComment([FromBody] PostCommentDto dto)
        {
            try
            {
                await _commentService.PostCommentAsync(dto);
            }
            catch (ValidationException ex)
            {
                return new JsonResult(BadRequest($"{ex.GetType().Name}: {ex.Message}"));
            }

            catch(Exception ex)
            {
                return new JsonResult(BadRequest(ex.Message));
            }

            return new JsonResult(Ok());
        }

        [HttpPut("{commentId}")]
        public async Task<IActionResult> Update(Guid commentId, [FromBody] EditCommentDto dto)
        {
            try
            {
                await _commentService.UpdateCommentAsync(commentId, dto);
            }
            catch (KeyNotFoundException ex)
            {
                return new JsonResult(NotFound($"{ex.GetType().Name}: {ex.Message}"));
            }

            return new JsonResult(Accepted(dto));

        }

        [AllowAnonymous]
        [HttpPut("{commentId}")]
        public async Task<IActionResult> Delete(Guid commentId)
        {
            try
            {
                await _commentService.DeleteCommentAsync(commentId);
            }
            catch (KeyNotFoundException ex)
            {
                return new JsonResult(NotFound($"{ex.GetType().Name}: {ex.Message}"));
            }

            return new JsonResult(NoContent());

        }
    }
}
