using Microsoft.AspNetCore.Identity;

namespace Data.Models
{
    public class AppUserRole: IdentityUserRole<int>
    {
        public User User { get; set; }
        public AppRole Role { get; set; }
    }
}