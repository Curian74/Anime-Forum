using Microsoft.AspNetCore.Http;

namespace Infrastructure.Configurations
{
    public class AuthTokenOptions
    {
        public string Name { get; set; }
        public int Expires { get; set; } // Expiration time in hours
        public bool Secure { get; set; }
        public bool HttpOnly { get; set; }
        public SameSiteMode SameSite { get; set; }
        public bool IsEssential { get; set; }
    }
}
