
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace API.Persistence
{
    public class AstreeDbContext : IdentityDbContext<User,AppRole,int,IdentityUserClaim<int>,AppUserRole,IdentityUserLogin<int>,IdentityRoleClaim<int>,IdentityUserToken<int>>
    {
        public AstreeDbContext(DbContextOptions<AstreeDbContext> options) :
            base(options)
        {
        }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
             builder.Entity<User>().HasMany(ur=>ur.UserRoles).WithOne(u=>u.User).HasForeignKey(ur=>ur.UserId).IsRequired();
             builder.Entity<AppRole>().HasMany(ur=>ur.UserRoles).WithOne(u=>u.Role).HasForeignKey(ur=>ur.RoleId).IsRequired();

        builder.Entity<Complaint>()
            .HasOne(c => c.User) 
            .WithMany(u => u.Complaints) 
            .HasForeignKey(c => c.UserId); 
        }


    }
}
