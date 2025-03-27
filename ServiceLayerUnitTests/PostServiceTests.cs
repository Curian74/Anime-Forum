using Moq;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Application.Services;
using Application.Common.Mappings;
using Microsoft.Extensions.DependencyInjection;
using Application.DTO;
using System.Linq.Expressions;
using Application.DTO.Post;

namespace ServiceLayerUnitTests
{
    [TestFixture]
    public class PostServiceTests
    {
        private PostService _postService;

        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IGenericRepository<Post>> _postRepositoryMock;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddAutoMapper(typeof(MappingProfile));
            var serviceProvider = serviceCollection.BuildServiceProvider();

            _mapper = serviceProvider.GetRequiredService<IMapper>();

            _unitOfWorkMock = new();
            _postRepositoryMock = new();

            _unitOfWorkMock.Setup(x => x.GetRepository<Post>()).Returns(_postRepositoryMock.Object);

            _postService = new PostService(_unitOfWorkMock.Object, _mapper);
        }

        [Test]
        public async Task GetAllPosts_ShouldReturnAllPosts()
        {
            // Arrange
            var posts = new List<Post>
            {
                new Post { Id = Guid.NewGuid(), Title = "TestPost1" },
                new Post { Id = Guid.NewGuid(), Title = "TestPost2" },
                new Post { Id = Guid.NewGuid(), Title = "TestPost3" }
            };

            _postRepositoryMock
                .Setup(repo => repo.GetAllAsync(null, null))
                .ReturnsAsync((posts, posts.Count));

            // Act
            var (result, count) = await _postService.GetAllAsync();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Count, Is.EqualTo(posts.Count));
                Assert.That(count, Is.EqualTo(posts.Count));
                Assert.That(result.ElementAt(0).Title, Is.EqualTo("TestPost1"));
            });

        }

        [Test]
        public async Task GetPostById_ShouldReturnPost()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var post = new Post { Id = postId, Title = "TestPost" };
            _postRepositoryMock
                .Setup(repo => repo.GetByIdAsync(postId))
                .ReturnsAsync(post);

            // Act
            var result = await _postService.GetByIdAsync(postId);

            // Assert
            Assert.That(result.Id, Is.EqualTo(postId));
        }

        [Test]
        public async Task CreatePostAsync_ShouldAddPostAndSaveChanges()
        {
            // Arrange
            var createPostDto = new CreatePostDto { Title = "New Post", Content = "Post Content" };
            var expectedPost = _mapper.Map<Post>(createPostDto);
            var expectedSaveResult = 1;

            _postRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Post>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync())
                .ReturnsAsync(expectedSaveResult);

            // Act
            var result = await _postService.CreatePostAsync(createPostDto);

            // Assert
            Assert.That(expectedSaveResult,Is.EqualTo(result));

            _postRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Post>(p =>
                p.Title == createPostDto.Title &&
                p.Content == createPostDto.Content)), Times.Once);

            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task UpdatePostAsync_ShouldUpdatePostAndSaveChanges()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var existingPost = new Post { Id = postId, Title = "Old Title", Content = "Old Content" };
            var updateDto = new CreatePostDto { Title = "Updated Title", Content = "Updated Content" };
            var expectedSaveResult = 1;

            // Mock repository to return the existing post
            _postRepositoryMock
                .Setup(repo => repo.GetByIdAsync(postId))
                .ReturnsAsync(existingPost);

            // Mock SaveChangesAsync to return 1 (indicating one record updated)
            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync())
                .ReturnsAsync(expectedSaveResult);

            // Act
            var result = await _postService.UpdatePostAsync(postId, updateDto);

            // Assert
            Assert.That(expectedSaveResult, Is.EqualTo(result));

            _postRepositoryMock.Verify(repo => repo.GetByIdAsync(postId), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);

            Assert.Multiple(() =>
            {
                Assert.That(updateDto.Title, Is.EqualTo(existingPost.Title));
                Assert.That(updateDto.Content, Is.EqualTo(existingPost.Content));
            });
        }

        [Test]
        public void UpdatePostAsync_ShouldThrowKeyNotFoundException_WhenPostDoesNotExist()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var updateDto = new CreatePostDto { Title = "Updated Title", Content = "Updated Content" };

            _postRepositoryMock
                .Setup(repo => repo.GetByIdAsync(postId))
                .ReturnsAsync((Post)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<KeyNotFoundException>(() => _postService.UpdatePostAsync(postId, updateDto));

            Assert.That(exception.Message, Is.EqualTo("Could not find requested post."));
            _postRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task DeletePostAsync_ShouldDeletePostAndSaveChanges()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var existingPost = new Post { Id = postId, Title = "To be deleted" };
            var expectedSaveResult = 1;

            _postRepositoryMock
                .Setup(repo => repo.GetByIdAsync(postId))
                .ReturnsAsync(existingPost);

            _postRepositoryMock
                .Setup(repo => repo.DeleteAsync(postId))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync())
                .ReturnsAsync(expectedSaveResult);

            // Act
            var result = await _postService.DeletePostAsync(postId);

            // Assert
            Assert.That(expectedSaveResult, Is.EqualTo(result));
            _postRepositoryMock.Verify(repo => repo.DeleteAsync(postId), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeletePostAsync_ShouldThrowKeyNotFoundException_WhenPostDoesNotExist()
        {
            // Arrange
            var postId = Guid.NewGuid();

            _postRepositoryMock
                .Setup(repo => repo.GetByIdAsync(postId))
                .ReturnsAsync((Post)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<KeyNotFoundException>(() => _postService.DeletePostAsync(postId));

            Assert.That(exception.Message, Is.EqualTo("Could not find requested post."));
            _postRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task DeletePostWhereAsync_ShouldDeleteMatchingPostsAndSaveChanges()
        {
            // Arrange
            Expression<Func<Post, bool>> filter = p => p.Title.Contains("delete");
            var expectedSaveResult = 1;

            _postRepositoryMock
                .Setup(repo => repo.DeleteWhereAsync(filter))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync())
                .ReturnsAsync(expectedSaveResult);

            // Act
            var result = await _postService.DeletePostWhereAsync(filter);

            // Assert
            Assert.That(result, Is.EqualTo(expectedSaveResult));
            _postRepositoryMock.Verify(repo => repo.DeleteWhereAsync(filter), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task DeactivatePostAsync_ShouldUpdateIsHiddenAndSaveChanges()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var existingPost = new Post { Id = postId, Title = "Active Post", IsHidden = false };
            var deactivateDto = new DeactivatePostDto { IsHidden = true };
            var expectedSaveResult = 1;

            _postRepositoryMock
                .Setup(repo => repo.GetByIdAsync(postId))
                .ReturnsAsync(existingPost);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync())
                .ReturnsAsync(expectedSaveResult);

            // Act
            var result = await _postService.DeactivatePostAsync(postId, deactivateDto);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(expectedSaveResult));
                Assert.That(existingPost.IsHidden, Is.True);
            });

            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeactivatePostAsync_ShouldThrowKeyNotFoundException_WhenPostDoesNotExist()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var deactivateDto = new DeactivatePostDto { IsHidden = true };

            _postRepositoryMock
                .Setup(repo => repo.GetByIdAsync(postId))
                .ReturnsAsync((Post)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<KeyNotFoundException>(() => _postService.DeactivatePostAsync(postId, deactivateDto));

            Assert.That(exception.Message, Is.EqualTo("Could not find requested post."));
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }


        [TearDown]
        public void TearDown()
        {
        }
    }
}
