using Calendar.Shared.Entities;
using Calendar.Startup.Seed;
using Microsoft.AspNetCore.Identity;

namespace Calendar.Startup.Infra
{
    internal static class DatabaseSeeder
    {
        public static void Seed(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<User>>();

                    DatabaseSeed.SeedAsync(userManager).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
        }
    }
}
