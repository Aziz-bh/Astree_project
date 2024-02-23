using API.Interfaces;
using API.Services;
using Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection
        AddApplicationServices(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            services.AddScoped<ITokenService, TokenService>();
            services
                .AddDbContext<AstreeDbContext>(options =>
                {
                    options
                        .UseSqlServer(config
                            .GetConnectionString("DefaultConnection"));
                });

                services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("https://localhost:44470")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()); // Added AllowCredentials
});
            return services;
        }
    }
}
