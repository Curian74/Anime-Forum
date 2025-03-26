using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace ServiceLayerUnitTests
{
    [TestFixture]
    public class MediaServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IGenericRepository<Media>> _mediaRepositoryMock;
        private MediaService _mediaService;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mediaRepositoryMock = new Mock<IGenericRepository<Media>>();

            _unitOfWorkMock.Setup(u => u.GetRepository<Media>()).Returns(_mediaRepositoryMock.Object);

            _mediaService = new MediaService(_unitOfWorkMock.Object);
        }

        [Test]
        public async Task AddAsync_ShouldAddMedia_AndSaveChanges()
        {
            // Arrange
            var media = new Media { Id = Guid.NewGuid(), Url = "https://example.com/media.jpg" };

            // Act
            var result = await _mediaService.AddAsync(media);

            // Assert
            _mediaRepositoryMock.Verify(r => r.AddAsync(media), Times.Once, "Media was not added.");
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once, "Changes were not saved.");
            Assert.That(result, Is.EqualTo(media), "Returned media does not match the input.");
        }
    }
}
