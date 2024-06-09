
using API.Interfaces;
using Data.Persistence;
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
            
            services.AddScoped<IAutomobileService, AutomobileService>();
            services.AddScoped<IPropertyService, PropertyService>();
            services.AddScoped<ChatService>();
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
                          .AllowCredentials());
});
services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
services.AddSingleton<IEmailService, EmailService>(provider =>
    new EmailService("linkzlinkz04@gmail.com", "zixg obnz xohb fizv"));

            return services;
        }
    }
}
