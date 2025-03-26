using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTO;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using NUnit.Framework;

namespace ServiceLayerUnitTests
{
    [TestFixture]
    public class AdminServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IGenericRepository<User>> _userRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private AdminService _adminService;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userRepositoryMock = new Mock<IGenericRepository<User>>();
            _mapperMock = new Mock<IMapper>();

            _unitOfWorkMock.Setup(u => u.GetRepository<User>()).Returns(_userRepositoryMock.Object);

            _adminService = new AdminService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task GetAllUsersAsync_ShouldReturnUsers()
        {
            // Arrange
            var users = new List<User> { new() { Id = Guid.NewGuid(), UserName = "JohnDoe" } };
            _userRepositoryMock.Setup(r => r.GetAllAsync(null, null)).ReturnsAsync((users, users.Count));

            // Act
            var result = await _adminService.GetAllUsersAsync();

            // Assert
            Assert.That(result.Items, Is.EqualTo(users));
            Assert.That(result.TotalCount, Is.EqualTo(users.Count));
        }

        [Test]
        public async Task GetUserByIdAsync_WhenUserExists_ShouldReturnUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, UserName = "JohnDoe" };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _adminService.GetUserByIdAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(user));
        }

        [Test]
        public async Task GetUserByIdAsync_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act
            var result = await _adminService.GetUserByIdAsync(userId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UpdateUserAsync_WhenUserExists_ShouldUpdateAndReturnAffectedRows()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, UserName = "JohnDoe" };
            var dto = new UserProfileDto { /* Populate with test data */ };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map(dto, user));
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _adminService.UpdateUserAsync(userId, dto);

            // Assert
            Assert.That(result, Is.EqualTo(1));
            _mapperMock.Verify(m => m.Map(dto, user), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void UpdateUserAsync_WhenUserDoesNotExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dto = new UserProfileDto();

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _adminService.UpdateUserAsync(userId, dto));
        }

        [Test]
        public async Task DeleteUserAsync_WhenUserExists_ShouldDeleteAndReturnAffectedRows()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, UserName = "JohnDoe" };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.DeleteAsync(userId)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _adminService.DeleteUserAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(1));
            _userRepositoryMock.Verify(r => r.DeleteAsync(userId), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeleteUserAsync_WhenUserDoesNotExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _adminService.DeleteUserAsync(userId));
        }
    }
}
