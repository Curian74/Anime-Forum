using Application.DTO.Comment;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using System.Linq.Expressions;

namespace ServiceLayerUnitTests
{
    [TestFixture]
    public class CommentServiceTests
    {
        private CommentService _commentService;

        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IGenericRepository<Comment>> _commentRepositoryMock;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _commentRepositoryMock = new Mock<IGenericRepository<Comment>>();
            _mapperMock = new Mock<IMapper>();

            // Mock repository
            _unitOfWorkMock.Setup(uow => uow.GetRepository<Comment>()).Returns(_commentRepositoryMock.Object);

            _commentService = new CommentService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnListOfComments()
        {
            // Arrange
            var comments = new List<Comment> { new Comment { Id = Guid.NewGuid(), Content = "Test Comment" } };
            _commentRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<Comment, bool>>>(), It.IsAny<Func<IQueryable<Comment>, IOrderedQueryable<Comment>>>()))
                                  .ReturnsAsync((comments, comments.Count));

            // Act
            var result = await _commentService.GetAllAsync();

            // Assert
            Assert.That(result.Items, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Items.Count, Is.EqualTo(1));
                Assert.That(result.Items.First().Content, Is.EqualTo("Test Comment"));
            });
        }

        [Test]
        public async Task GetPagedAsync_ShouldReturnPagedComments()
        {
            // Arrange
            var comments = new List<Comment> { new Comment { Id = Guid.NewGuid(), Content = "Test Comment" } };
            _commentRepositoryMock.Setup(repo => repo.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Comment, bool>>>(), It.IsAny<Func<IQueryable<Comment>, IOrderedQueryable<Comment>>>()))
                                  .ReturnsAsync((comments, comments.Count));

            // Act
            var result = await _commentService.GetPagedAsync(1, 10);

            // Assert
            Assert.That(result.Items, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Items.Count, Is.EqualTo(1));
                Assert.That(result.Items.First().Content, Is.EqualTo("Test Comment"));
                Assert.That(result.TotalPages, Is.EqualTo(1));
                Assert.That(result.TotalCount, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task PostCommentAsync_ShouldAddCommentAndReturnSaveResult()
        {
            // Arrange
            var dto = new PostCommentDto { PostId = Guid.NewGuid(), UserId = Guid.NewGuid(), Content = "New Comment" };
            var comment = new Comment { Id = Guid.NewGuid(), Content = dto.Content };

            _mapperMock.Setup(mapper => mapper.Map<Comment>(dto)).Returns(comment);
            _commentRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _commentService.PostCommentAsync(dto);

            // Assert
            Assert.That(result, Is.EqualTo(1));
            _commentRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Comment>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateCommentAsync_ShouldUpdateCommentAndReturnSaveResult()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var dto = new EditCommentDto { Content = "Updated Content" };
            var existingComment = new Comment { Id = commentId, Content = "Old Content" };

            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(commentId)).ReturnsAsync(existingComment);
            _mapperMock.Setup(mapper => mapper.Map(dto, existingComment));
            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _commentService.UpdateCommentAsync(commentId, dto);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(1));
                Assert.That(existingComment.Content, Is.EqualTo("Old Content"));
            });
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void UpdateCommentAsync_ShouldThrowException_WhenCommentNotFound()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var dto = new EditCommentDto { Content = "Updated Content" };

            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(commentId)).ReturnsAsync((Comment?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _commentService.UpdateCommentAsync(commentId, dto));
            Assert.That(ex!.Message, Is.EqualTo("Could not find requested post."));
        }

        [Test]
        public async Task DeleteCommentAsync_ShouldMarkCommentAsHiddenAndReturnSaveResult()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var existingComment = new Comment { Id = commentId, Content = "To be deleted", IsHidden = false };

            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(commentId)).ReturnsAsync(existingComment);
            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _commentService.DeleteCommentAsync(commentId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(1));
                Assert.That(existingComment.IsHidden, Is.True);
            });
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeleteCommentAsync_ShouldThrowException_WhenCommentNotFound()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(commentId)).ReturnsAsync((Comment?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _commentService.DeleteCommentAsync(commentId));
            Assert.That(ex!.Message, Is.EqualTo("Could not find requested post."));
        }
    }
}
