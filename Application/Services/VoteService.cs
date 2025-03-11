using Application.DTO;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class VoteService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IGenericRepository<Vote> _voteRepository = unitOfWork.GetRepository<Vote>();
        private readonly IGenericRepository<Post> _postRepository = unitOfWork.GetRepository<Post>();
        private readonly IMapper _mapper = mapper;

        public async Task<int> GetTotalPostVotesAsync(Guid postId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            
            if (post == null)
            {
                throw new ArgumentNullException(nameof(post), "Post not found.");
            }

            var votes = post.Votes;

            int totalVotes = 0;

            foreach (var v in votes)
            {
                if (v.IsUpvote) totalVotes++;
                else totalVotes--;
            }

            if (post.TotalVotes != totalVotes)
            {
                post.TotalVotes = totalVotes;
                await _unitOfWork.SaveChangesAsync();
            }

            return totalVotes;
        }

        public async Task<Vote?> GetCurrentUserVoteAsync(Guid postId, Guid userId)
        {
            return await _voteRepository.GetSingleWhereAsync(v => v.PostId == postId && v.UserId == userId);
        }

        public async Task<int> ToggleVoteAsync(VoteDto dto, Guid userId)
        {
            var post = await _postRepository.GetByIdAsync(dto.PostId);
            if (post == null)
            {
                throw new ArgumentNullException(nameof(post), "Post not found.");
            }

            var existingVote = await _voteRepository.GetSingleWhereAsync(v => v.PostId == dto.PostId && v.UserId == userId);
            var user = post.User ?? throw new InvalidOperationException("Post author not found.");

            if (existingVote != null)
            {
                if (existingVote.IsUpvote == dto.IsUpvote)
                {
                    // Duped vote — remove the vote
                    await _voteRepository.DeleteAsync(existingVote.Id);

                    var decrement = existingVote.IsUpvote ? -1 : 1;
                    post.TotalVotes += decrement;
                    user.Points += decrement;
                }
                else
                {
                    // Switch vote
                    existingVote.IsUpvote = dto.IsUpvote;

                    var change = dto.IsUpvote ? 2 : -2; // +1 to -1, or -1 to +1
                    post.TotalVotes += change;
                    user.Points += change;
                }
            }
            else
            {
                // New vote
                var vote = _mapper.Map<Vote>(dto);
                vote.UserId = userId;

                await _voteRepository.AddAsync(vote);

                var increment = dto.IsUpvote ? 1 : -1;
                post.TotalVotes += increment;
                user.Points += increment;
            }

            return await _unitOfWork.SaveChangesAsync();
        }
    }
}
