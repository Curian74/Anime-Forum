using Microsoft.EntityFrameworkCore;

namespace WibuBlog.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }
    }
}
