
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Persistence
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
