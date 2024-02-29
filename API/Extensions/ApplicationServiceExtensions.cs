
using API.Interfaces;
using API.Persistence;
using API.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
// using Business.Interfaces;
// using Business.Services;

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
services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
            return services;
        }
    }
}
