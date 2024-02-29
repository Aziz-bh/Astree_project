
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
             builder.Entity<User>().HasMany(ur=>ur.UserRoles).WithOne(u=>u.User).HasForeignKey(ur=>ur.UserId).IsRequired();
             builder.Entity<AppRole>().HasMany(ur=>ur.UserRoles).WithOne(u=>u.Role).HasForeignKey(ur=>ur.RoleId).IsRequired();
            // builder.Entity<AppUserRole>(userRole =>
            // {
            //     userRole.HasKey(ur => new {ur.UserId, ur.RoleId});
            //     userRole.HasOne(ur => ur.Role)
            //         .WithMany(r => r.UserRoles)
            //         .HasForeignKey(ur => ur.RoleId)
            //         .IsRequired();
            //     userRole.HasOne(ur => ur.User)
            //         .WithMany(r => r.UserRoles)
            //         .HasForeignKey(ur => ur.UserId)
            //         .IsRequired();
            // });
        }


    }
}
