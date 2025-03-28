using Domain.Entities;
using Domain.Interfaces;
using System.Linq.Expressions;

namespace Application.Services
{
    public class RankService(IUnitOfWork unitOfWork)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IGenericRepository<User> _userRepository = unitOfWork.GetRepository<User>();
        private readonly IGenericRepository<Rank> _rankRepository = unitOfWork.GetRepository<Rank>();

        public async Task<(IEnumerable<Rank> Items, int TotalCount)> GetAllAsync(
            Expression<Func<Rank, bool>>? filter = null,
            Func<IQueryable<Rank>, IOrderedQueryable<Rank>>? orderBy = null)
        {
            return await _rankRepository.GetAllAsync(filter, orderBy);
        }

        public async Task UpdateUserRankAsync(Guid? userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return;

            var (allRanks, _) = await _rankRepository.GetAllAsync();
            if (allRanks == null || !allRanks.Any()) return;

            var bestRank = allRanks
                .Where(r => r.PointsRequired <= user.Points)
                .OrderByDescending(r => r.PointsRequired)
                .FirstOrDefault();

            if (bestRank != null && user.RankId != bestRank.Id)
            {
                user.RankId = bestRank.Id;
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
