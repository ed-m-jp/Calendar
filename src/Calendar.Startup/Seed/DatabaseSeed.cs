using Calendar.DataAccess;
using Calendar.shared.Entities;
using Calendar.Shared.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Calendar.Startup.Seed
{
    public static class DatabaseSeed
    {
        public static async Task SeedAsync(UserManager<User> userManager, AppDbContext dbContext)
        {
            if (userManager.Users.Any()) // Check if any users are in DB
                return;

            var user = new User
            {
                UserName = "Edward"
            };

            await userManager.CreateAsync(user, "Password1");

            var now = DateTime.Now;

            var eventToSeed = new List<Event>
            {
                new Event()
                {
                    Title = "Lunch with Bob",
                    Description = "Reservation for 2 person at sancho pizza restaurant.",
                    AllDay = false,
                    StartTime = new DateTime(now.Year, now.Month, 1, 11, 00, 00),
                    EndTime = new DateTime(now.Year, now.Month, 1, 15, 00, 00),
                    UserId = user.Id,
                },
                new Event()
                {
                    Title = "Dentist",
                    Description = "Yearly check.",
                    AllDay = false,
                    StartTime = new DateTime(now.Year, now.Month, 13, 13, 00, 00),
                    EndTime = new DateTime(now.Year, now.Month, 13, 14, 00, 00),
                    UserId = user.Id,
                },
                new Event()
                {
                    Title = "Piano lesson",
                    Description = "",
                    AllDay = false,
                    StartTime = new DateTime(now.Year, now.Month, 20, 13, 00, 00),
                    EndTime = new DateTime(now.Year, now.Month, 20, 14, 30, 00),
                    UserId = user.Id,
                },
                new Event()
                {
                    Title = "Sales at gameshop",
                    Description = "Sales up to 70% all the day!",
                    AllDay = true,
                    StartTime = new DateTime(now.Year, now.Month, 27, 00, 00, 00),
                    EndTime = new DateTime(now.Year, now.Month, 28, 00, 00, 00),
                    UserId = user.Id,
                },
            };

            dbContext.Events.AddRange(eventToSeed);
            await dbContext.SaveChangesAsync();
        }
    }
}
