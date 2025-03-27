using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace ServiceLayerUnitTests
{
    [TestFixture]
    public class InventoryServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IGenericRepository<User>> _userRepositoryMock;
        private Mock<IGenericRepository<UserInventory>> _inventoryRepositoryMock;
        private Mock<IGenericRepository<UserFlair>> _flairRepositoryMock;
        private Mock<IGenericRepository<UserFlairSelection>> _flairSelectionRepositoryMock;
        private InventoryService _inventoryService;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userRepositoryMock = new Mock<IGenericRepository<User>>();
            _inventoryRepositoryMock = new Mock<IGenericRepository<UserInventory>>();
            _flairRepositoryMock = new Mock<IGenericRepository<UserFlair>>();
            _flairSelectionRepositoryMock = new Mock<IGenericRepository<UserFlairSelection>>();

            _unitOfWorkMock.Setup(u => u.GetRepository<User>()).Returns(_userRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.GetRepository<UserInventory>()).Returns(_inventoryRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.GetRepository<UserFlair>()).Returns(_flairRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.GetRepository<UserFlairSelection>()).Returns(_flairSelectionRepositoryMock.Object);

            _inventoryService = new InventoryService(_unitOfWorkMock.Object);
        }

        [Test]
        public async Task GetUserInventoryAsync_ShouldReturnExistingInventory()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var inventory = new UserInventory { UserId = userId };
            var user = new User { Id = userId };

            _inventoryRepositoryMock.Setup(r => r.GetSingleWhereAsync(i => i.UserId == userId)).ReturnsAsync(inventory);
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _inventoryService.GetUserInventoryAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(inventory));
        }

        [Test]
        public async Task SetActiveFlairAsync_ShouldReturnFalse_WhenFlairNotOwned()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var flairId = Guid.NewGuid();
            var inventory = new UserInventory { UserId = userId, Flairs = new List<UserFlair>() };

            _inventoryRepositoryMock.Setup(r => r.GetSingleWhereAsync(i => i.UserId == userId)).ReturnsAsync(inventory);

            // Act
            var result = await _inventoryService.SetActiveFlairAsync(userId, flairId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateFlairsAsync_ShouldNotThrow_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null);

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await _inventoryService.UpdateFlairsAsync(userId));
        }
    }
}
