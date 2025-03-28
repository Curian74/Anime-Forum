
using Domain.Entities;

namespace Application.DTO
{
    public class WebStatsDto
    {
        public int TotalUserCount { get; set; }
        public int NewUserCount { get; set; }
        public int DailyActiveUserCount { get; set; }
        public int WeeklyActiveUserCount { get; set; }
        public int MonthlyActiveUserCount { get; set; }
        public float ActiveAccountPercentage { get; set; } 
        public float RetentionRate { get; set; }
        public int TotalPostCount { get; set; }
        public int TotalCommentCount { get; set; }
        public int NewPostCount { get; set; }
        public float AveragePostsPerDay { get; set; }
        public float PostEngagementRate { get; set; }
        public IEnumerable<Domain.Entities.Post> TopPosts { get; set; } = [];
        public IEnumerable<User> TopUsers { get; set; } = [];
        public IEnumerable<PostCategory> TopCategories { get; set; } = [];
    }
}
