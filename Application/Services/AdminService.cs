using Application.DTO;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class AdminService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IGenericRepository<User> _userRepository = unitOfWork.GetRepository<User>();
        private readonly IGenericRepository<Post> _postRepository = unitOfWork.GetRepository<Post>();
        private readonly IGenericRepository<Comment> _commentRepository = unitOfWork.GetRepository<Comment>();
        private readonly IGenericRepository<Vote> _voteRepository = unitOfWork.GetRepository<Vote>();
        private readonly IGenericRepository<PostCategory> _categoryRepository = unitOfWork.GetRepository<PostCategory>();
        private readonly IGenericRepository<Ticket> _ticketRepository = unitOfWork.GetRepository<Ticket>();
        private readonly IMapper _mapper = mapper;

        public async Task<(IEnumerable<User> Items, int TotalCount)> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<int?> UpdateUserAsync(Guid userId, UserProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                       ?? throw new KeyNotFoundException("Could not find requested user.");

            _mapper.Map(dto, user);

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int?> DeleteUserAsync(Guid userId)
        {
            _ = await _userRepository.GetByIdAsync(userId)
                       ?? throw new KeyNotFoundException("Could not find requested user.");

            await _userRepository.DeleteAsync(userId);

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<WebStatsDto> GetStats(int days)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);

            var totalUserCount = await _userRepository.CountAsync();
            var newUsersCount = await _userRepository.CountAsync(u => u.CreatedAt >= startDate);
            
            var dailyActiveUsers = await _userRepository.CountAsync(u => u.LastActive >= DateTime.UtcNow.AddDays(-1));
            var weeklyActiveUsers = await _userRepository.CountAsync(u => u.LastActive >= DateTime.UtcNow.AddDays(-7));
            var monthlyActiveUsers = await _userRepository.CountAsync(u => u.LastActive >= DateTime.UtcNow.AddDays(-30));
            var activeAccountPercentage = (float)dailyActiveUsers / totalUserCount * 100;

            var lastMonthActiveUsers = await _userRepository.CountAsync(
                u => u.LastActive >= DateTime.UtcNow.AddMonths(-2)                          
                && u.LastActive < DateTime.UtcNow.AddMonths(-1)
            );
            var retentionRate = lastMonthActiveUsers == 0 ? 0 : (float)monthlyActiveUsers / lastMonthActiveUsers * 100;

            var totalPosts = await _postRepository.CountAsync();
            var newPosts = await _postRepository.CountAsync(p => p.CreatedAt >= startDate);
            var averagePostsPerDay = days > 0 ? (float)totalPosts / days : 0;

            var totalComments = await _commentRepository.CountAsync();
            var totalVotes = await _voteRepository.CountAsync();
            var engagementRate = totalPosts == 0 ? 0 : (float)(totalVotes + totalComments) / totalPosts;

            var allPosts = await _postRepository.GetAllWhereAsync(p => p.CreatedAt >= startDate);
            var topPosts = allPosts
                .OrderByDescending(p => p.Votes.Count)
                .Take(10)
                .ToList();

            var allUsers = await _userRepository.GetAllAsync();
            var topUsers = allUsers.Items
                .OrderByDescending(u => u.Points)
                .Take(10)
                .ToList();

            var allCategories = await _categoryRepository.GetAllAsync();
            var topCategories = allCategories.Items
                .OrderByDescending(c => c.Posts.Count())
                .Take(5)
                .ToList();

            var webStats = new WebStatsDto
            {
                TotalUserCount = totalUserCount,
                NewUserCount = newUsersCount,
                DailyActiveUserCount = dailyActiveUsers,
                WeeklyActiveUserCount = weeklyActiveUsers,
                MonthlyActiveUserCount = monthlyActiveUsers,
                ActiveAccountPercentage = activeAccountPercentage,
                RetentionRate = retentionRate,
                TotalPostCount = totalPosts,
                NewPostCount = newPosts,
                AveragePostsPerDay = averagePostsPerDay,
                TotalCommentCount = totalComments,
                PostEngagementRate = engagementRate,
                TopPosts = topPosts,
                TopUsers = topUsers,
                TopCategories = topCategories
            };

            return webStats;
        }
    }
}
