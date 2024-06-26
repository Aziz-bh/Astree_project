using System.Text;
using Data.Models;
using Data.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {

            services.AddIdentityCore<User>(opt=>{
                opt.Password.RequireNonAlphanumeric=false;
  
            }).AddRoles<AppRole>().
            AddRoleManager<RoleManager<AppRole>>().AddEntityFrameworkStores<AstreeDbContext>();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
                services.AddAuthorization(opt => {
                opt.AddPolicy("RequireAdminRole",policy=>policy.RequireRole("Admin"));
                 opt.AddPolicy("Moderator",policy=>policy.RequireRole("Admin","Moderator"));
                
                });
                
            return services;
        }

    }
}
