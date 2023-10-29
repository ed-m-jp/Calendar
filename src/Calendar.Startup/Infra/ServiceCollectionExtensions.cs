using Calendar.DataAccess;
using Calendar.DataAccess.Interfaces;
using Calendar.DataAccess.Repositories;
using Calendar.Services.Services;
using Calendar.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Calendar.Shared.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Calendar.Startup.Infra
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            var dbProvider = configuration.GetValue<string>("DatabaseProvider");

            if (dbProvider == "SqlServer")
            {
                services.AddDbContext<AppDbContext>(
                    options => options.UseSqlite(configuration.GetConnectionString("DefaultConnection"),
                        builder => builder.MigrationsAssembly("Calendar.DataAccess")));
            }
            else
            {
                services.AddDbContext<AppDbContext>(
                    options => options.UseInMemoryDatabase("inMemoryDatabase"));
            }

            return services;
        }

        internal static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Set password requirements.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            });

            return services;
        }

        internal static IServiceCollection ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtConfig = configuration.GetSection("jwtConfig");
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig["validIssuer"],
                    ValidAudiences = jwtConfig.GetSection("validAudiences").Get<List<string>>(),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["secret"]!))
                };
            });

            return services;
        }

        internal static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IEventRepository, EventRepository>();

            return services;
        }

        internal static IServiceCollection AddServices(this IServiceCollection services)
        {
            // app services
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();

            // build in services
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
