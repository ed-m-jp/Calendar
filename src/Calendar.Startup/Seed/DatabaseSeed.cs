using Calendar.Shared.Entities;
using Microsoft.AspNetCore.Identity;

namespace Calendar.Startup.Seed
{
    public static class DatabaseSeed
    {
        public static async Task SeedAsync(UserManager<User> userManager)
        {
            if (userManager.Users.Any()) // Check if any users are in DB
                return;

            var user = new User
            {
                UserName = "Edward"
            };

            await userManager.CreateAsync(user, "Password1");
        }
    }
}
