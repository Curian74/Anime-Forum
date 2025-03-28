using System.Linq.Expressions;
using Application.DTO;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace ServiceLayerUnitTests
{
    [TestFixture]
    public class VoteServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IGenericRepository<Vote>> _voteRepositoryMock;
        private Mock<IGenericRepository<Post>> _postRepositoryMock;
        private Mock<IGenericRepository<User>> _userRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<InventoryService> _inventoryServiceMock;
        private Mock<RankService> _rankServiceMock;
        private VoteService _voteService;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _voteRepositoryMock = new Mock<IGenericRepository<Vote>>();
            _postRepositoryMock = new Mock<IGenericRepository<Post>>();
            _userRepositoryMock = new Mock<IGenericRepository<User>>();
            _mapperMock = new Mock<IMapper>();

            _unitOfWorkMock.Setup(u => u.GetRepository<Vote>()).Returns(_voteRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.GetRepository<Post>()).Returns(_postRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.GetRepository<User>()).Returns(_userRepositoryMock.Object);

            _inventoryServiceMock = new Mock<InventoryService>(_unitOfWorkMock.Object);
            _rankServiceMock = new Mock<RankService>(_unitOfWorkMock.Object);

            _voteService = new VoteService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _inventoryServiceMock.Object,
                _rankServiceMock.Object
            );
        }

        [Test]
        public async Task GetTotalPostVotesAsync_ShouldReturnCorrectVoteCount()
        {
            var postId = Guid.NewGuid();
            var votes = new List<Vote> { new() { IsUpvote = true }, new() { IsUpvote = false }, new() { IsUpvote = true } };
            var post = new Post { Id = postId, Votes = votes, TotalVotes = 0 };

            _postRepositoryMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(post);

            var result = await _voteService.GetTotalPostVotesAsync(postId);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void GetTotalPostVotesAsync_ShouldThrowException_WhenPostNotFound()
        {
            var postId = Guid.NewGuid();
            _postRepositoryMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync((Post?)null);

            Assert.ThrowsAsync<ArgumentNullException>(() => _voteService.GetTotalPostVotesAsync(postId));
        }

        [Test]
        public async Task GetCurrentUserVoteAsync_ShouldReturnVote_WhenVoteExists()
        {
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var vote = new Vote { PostId = postId, UserId = userId, IsUpvote = true };

            _voteRepositoryMock.Setup(r => r.GetSingleWhereAsync(It.IsAny<Expression<Func<Vote, bool>>>()))
                .ReturnsAsync(vote);

            var result = await _voteService.GetCurrentUserVoteAsync(postId, userId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsUpvote, Is.True);
        }

        [Test]
        public async Task GetCurrentUserVoteAsync_ShouldReturnNull_WhenVoteNotFound()
        {
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _voteRepositoryMock.Setup(r => r.GetSingleWhereAsync(It.IsAny<Expression<Func<Vote, bool>>>()))
                .ReturnsAsync((Vote?)null);

            var result = await _voteService.GetCurrentUserVoteAsync(postId, userId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task ToggleVoteAsync_ShouldCreateVote_WhenNoExistingVote()
        {
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };
            var authorId = Guid.NewGuid();

            var voteDto = new VoteDto { PostId = postId, IsUpvote = true };
            var post = new Post { Id = postId, TotalVotes = 0, UserId = userId, User = user };
            var newVote = new Vote { PostId = postId, UserId = userId, IsUpvote = true };

            _postRepositoryMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(post);
            _voteRepositoryMock.Setup(r => r.GetSingleWhereAsync(It.IsAny<Expression<Func<Vote, bool>>>()))
                .ReturnsAsync((Vote?)null);
            _mapperMock.Setup(m => m.Map<Vote>(voteDto)).Returns(newVote);
            _userRepositoryMock.Setup(r => r.GetByIdAsync(authorId)).ReturnsAsync(new User { Id = authorId });

            await _voteService.ToggleVoteAsync(voteDto, userId);

            _voteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Vote>()), Times.Once);
        }
    }
}
