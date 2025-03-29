using Application.Services;
using Domain.Entities;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace WibuBlogAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class RankController : Controller
    {
        private readonly RankService _rankService;

        public RankController(RankService rankService)
        {
            _rankService = rankService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            string? filterBy = null,
            string? searchTerm = null,
            string? orderBy = null,
            bool descending = false)
        {
            Expression<Func<Rank, bool>>? filter = ExpressionBuilder.BuildFilterExpression<Rank>(filterBy, searchTerm);
            Func<IQueryable<Rank>, IOrderedQueryable<Rank>>? orderExpression = ExpressionBuilder.BuildOrderExpression<Rank>(orderBy, descending);

            var result = await _rankService.GetAllAsync(filter, orderExpression);

            return new JsonResult(Ok(result.Items));
        }
    }
}
