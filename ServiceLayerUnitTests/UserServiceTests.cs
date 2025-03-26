using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Application.Services;
using Domain.Entities;
using System.Linq.Expressions;
using Application.DTO;
using Domain.Interfaces;

namespace ServiceLayerUnitTests
{
    [TestFixture]
    public class UserServiceTests
    {
        private UserService _userService;
        private RankService _rankService;

        private Mock<UserManager<User>> _userManagerMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IGenericRepository<User>> _userRepositoryMock;
        private Mock<IGenericRepository<Post>> _postRepositoryMock;
        private Mock<IGenericRepository<Rank>> _rankRepositoryMock; 
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void Setup()
        {
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userRepositoryMock = new Mock<IGenericRepository<User>>();
            _postRepositoryMock = new Mock<IGenericRepository<Post>>();
            _rankRepositoryMock = new Mock<IGenericRepository<Rank>>();
            _mapperMock = new Mock<IMapper>();

            // Mock repositories
            _unitOfWorkMock.Setup(uow => uow.GetRepository<User>()).Returns(_userRepositoryMock.Object);
            _unitOfWorkMock.Setup(uow => uow.GetRepository<Post>()).Returns(_postRepositoryMock.Object);
            _unitOfWorkMock.Setup(uow => uow.GetRepository<Rank>()).Returns(_rankRepositoryMock.Object);

            _rankService = new RankService(_unitOfWorkMock.Object);

            _userService = new UserService(
                _userManagerMock.Object,
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _rankService
            );
        }

        [Test]
        public async Task FindByLoginAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var loginDto = new LoginDto { Login = "testuser" };
            var expectedUser = new User { UserName = "testuser", Email = "test@example.com" };

            _userManagerMock.Setup(um => um.FindByNameAsync(loginDto.Login)).ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.FindByLoginAsync(loginDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserName, Is.EqualTo(expectedUser.UserName));
        }

        [Test]
        public async Task GetUserByEmail_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var email = "test@example.com";
            var expectedUser = new User { Email = email };

            _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.GetUserByEmail(email);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo(email));
        }

        [Test]
        public async Task Register_ShouldReturnSuccess_WhenUserIsCreated()
        {
            // Arrange
            var registerDto = new RegisterDto { UserName = "newuser", Email = "new@example.com", Password = "P@ssword1" };
            var user = new User { UserName = registerDto.UserName, Email = registerDto.Email };

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), "Member"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.Register(registerDto);

            // Assert
            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task Login_ShouldReturnFalse_WhenUserNotFound()
        {
            // Arrange
            var loginDto = new LoginDto { Login = "unknown", Password = "password" };

            _userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

            // Act
            var result = await _userService.Login(loginDto);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetProfileDetails_ShouldReturnProfileDto_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, UserName = "testuser", Email = "test@example.com" };
            var userProfileDto = new UserProfileDto { UserName = "testuser", Email = "test@example.com" };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<UserProfileDto>(user)).Returns(userProfileDto);

            _rankRepositoryMock.Setup(r => r.GetAllWhereAsync(It.IsAny<Expression<Func<Rank, bool>>>()))
                 .ReturnsAsync(new List<Rank>());

            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(["Member"]);

            // Act
            var result = await _userService.GetProfileDetails(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.UserName, Is.EqualTo("testuser"));
                Assert.That(result.Email, Is.EqualTo("test@example.com"));
                Assert.That(result.Roles, Contains.Item("Member"));
            });
        }

        [Test]
        public async Task UpdateUserAsync_ShouldSaveChanges_WhenUserIsUpdated()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updateUserDto = new UpdateUserDto { Id = userId, Bio = "UpdatedBio" };
            var user = new User { Id = userId, Bio = "OldBio" };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map(updateUserDto, user));
            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _userService.UpdateUserAsync(updateUserDto);

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public async Task UpdatePasswordAsync_ShouldReturnSuccess_WhenPasswordChanges()
        {
            // Arrange
            var updatePasswordDto = new UpdatePasswordDTO
            {
                UserId = Guid.NewGuid(),
                OldPassword = "OldPass123",
                NewPassword = "NewPass456"
            };

            var user = new User { Id = updatePasswordDto.UserId };

            _userManagerMock.Setup(um => um.FindByIdAsync(updatePasswordDto.UserId.ToString()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(um => um.ChangePasswordAsync(user, updatePasswordDto.OldPassword, updatePasswordDto.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.UpdatePasswordAsync(updatePasswordDto);

            // Assert
            Assert.That(result.Succeeded, Is.True);
        }
    }
}