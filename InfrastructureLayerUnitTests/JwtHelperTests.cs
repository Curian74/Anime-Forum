using Domain.Entities;
using Infrastructure.Configurations;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InfrastructureLayerUnitTests
{
    [TestFixture]
    public class JwtHelperTests
    {
        private JwtHelper _jwtHelper;
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IOptions<AuthTokenOptions>> _authTokenOptionsMock;

        private const string SecretKey = "ThisIsASecretKeyForTestingPurposes123!";
        private const string Issuer = "testIssuer";
        private const string Audience = "testAudience";

        [SetUp]
        public void Setup()
        {
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            _configurationMock = new Mock<IConfiguration>();
            _authTokenOptionsMock = new Mock<IOptions<AuthTokenOptions>>();

            // Mock JWT settings
            _configurationMock.Setup(c => c["Jwt:Secret"]).Returns(SecretKey);
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns(Issuer);
            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns(Audience);

            _authTokenOptionsMock.Setup(a => a.Value).Returns(new AuthTokenOptions
            {
                Expires = 2 // 2 hours expiration
            });

            _jwtHelper = new JwtHelper(_configurationMock.Object, _userManagerMock.Object, _authTokenOptionsMock.Object);
        }

        [Test]
        public async Task GenerateJwtToken_ShouldReturnValidToken()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), UserName = "testUser", Email = "test@example.com" };
            _userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(["User", "Admin"]);

            // Act
            var token = await _jwtHelper.GenerateJwtToken(user);

            // Assert
            Assert.That(token, Is.Not.Null.And.Not.Empty);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            Assert.That(jwtToken, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(jwtToken.Issuer, Is.EqualTo(Issuer));
                Assert.That(jwtToken.Audiences, Contains.Item(Audience));
                Assert.That(jwtToken.ValidTo, Is.GreaterThan(DateTime.UtcNow));
            });
        }

        [Test]
        public void ExtractClaimsFromToken_ShouldReturnCorrectClaims()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var username = "testUser";
            var email = "test@example.com";

            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Name, username),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = GenerateTestToken(claims);

            // Act
            var extractedClaims = _jwtHelper.ExtractClaimsFromToken(token);

            // Assert
            Assert.That(extractedClaims, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(extractedClaims[JwtRegisteredClaimNames.Sub], Is.EqualTo(userId));
                Assert.That(extractedClaims[JwtRegisteredClaimNames.Name], Is.EqualTo(username));
                Assert.That(extractedClaims[JwtRegisteredClaimNames.Email], Is.EqualTo(email));
            });
        }

        [Test]
        public void ExtractUserIdFromToken_ShouldReturnCorrectUserId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var claims = new List<Claim> { new(JwtRegisteredClaimNames.Sub, userId.ToString()) };
            var token = GenerateTestToken(claims);

            // Act
            var extractedUserId = JwtHelper.ExtractUserIdFromToken(token);

            // Assert
            Assert.That(extractedUserId, Is.EqualTo(userId));
        }

        [Test]
        public void IsValidToken_ShouldReturnTrueForValidToken()
        {
            // Arrange
            var token = GenerateTestToken(new List<Claim>());

            // Act
            var isValid = _jwtHelper.IsValidToken(token);

            // Assert
            Assert.That(isValid, Is.True);
        }

        [Test]
        public void IsValidToken_ShouldReturnFalseForInvalidToken()
        {
            // Arrange
            var invalidToken = "invalid.token.string";

            // Act
            var isValid = _jwtHelper.IsValidToken(invalidToken);

            // Assert
            Assert.That(isValid, Is.False);
        }

        private string GenerateTestToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = Issuer,
                Audience = Audience,
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}