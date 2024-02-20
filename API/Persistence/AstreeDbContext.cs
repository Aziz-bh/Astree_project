using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Persistence
{
    public class AstreeDbContext : DbContext
    {
        public AstreeDbContext(DbContextOptions<AstreeDbContext> options) :
            base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
