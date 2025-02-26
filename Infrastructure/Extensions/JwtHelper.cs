using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Interfaces;
using Microsoft.Extensions.Options;
using Infrastructure.Configurations;

namespace Infrastructure.Extensions
{
    public class JwtHelper(IConfigurationManager configurationManager, UserManager<User> userManager, IOptions<AuthTokenOptions> authTokenOptions)
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly AuthTokenOptions _authTokenOptions = authTokenOptions.Value;

        private readonly string _secretKey = configurationManager.GetValue("Jwt:Secret");
        private readonly string _issuer = configurationManager.GetValue("Jwt:Issuer");
        private readonly string _audience = configurationManager.GetValue("Jwt:Audience");

        // Gen jwt token voi user claims
        public async Task<string> GenerateJwtToken(User user)
        {
            var authClaims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // subject: userid
            new Claim(JwtRegisteredClaimNames.Name, user.UserName), // name: username
            new Claim(JwtRegisteredClaimNames.Email, user.Email), // email: ...
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // random token id
        };

            // add claims tat ca cac role cua user
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddHours(_authTokenOptions.Expires),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Issuer = _issuer,
                Audience = _audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        // Lay tat ca claims tu jwt, xuat ra dang Dictionary
        public Dictionary<string, object> ExtractClaimsFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Invalid token");
            }

            var handler = new JwtSecurityTokenHandler();

            if (handler.ReadToken(token) is not JwtSecurityToken jwtToken)
            {
                throw new ArgumentException("Invalid JWT format");
            }

            var claims = jwtToken.Claims.ToDictionary(c => c.Type, c => (object)c.Value);

            return claims;
        }

        // Lay ra userid (Guid) tu jwt
        public static Guid? ExtractUserIdFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();

            if (handler.ReadToken(token) is not JwtSecurityToken jwtToken)
            {
                return null;
            }

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return null;
            }

            return userId;
        }

        // Check jwt legit (dung format, chua het han)
        public bool IsValidToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                var isTrue = validatedToken is JwtSecurityToken jwt && DateTime.UtcNow < jwt.ValidTo ;
                return isTrue;
            }
            catch
            {
                return false;
            }
        }
    }
}
