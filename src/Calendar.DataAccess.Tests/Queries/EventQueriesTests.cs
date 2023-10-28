using Calendar.shared.Entities;
using Calendar.DataAccess.Queries;

namespace Calendar.DataAccess.Tests.Queries
{
    [TestClass]
    public class EventQueriesTests
    {
        [TestMethod]
        public void GetEventForUserId_GivenUserId_ReturnAllEventsForThatId()
        {
            var userId = Guid.NewGuid().ToString();
            var eventsForTest = new List<Event>
            {
                new Event
                {
                    Title = "title1",
                    Description = "description",
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddMinutes(30),
                    UserId = userId,
                },
                new Event
                {
                    Title = "title2",
                    Description = "description",
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddMinutes(30),
                    UserId = userId,
                },
                new Event
                {
                    Title = "title3",
                    Description = "description",
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddMinutes(30),
                    UserId = userId,
                },
                new Event
                {
                    Title = "title3",
                    Description = "description",
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddMinutes(30),
                    UserId = Guid.NewGuid().ToString(), // Another user.
                },
            };

            // Act 
            var result = eventsForTest.AsQueryable().GetEventForUserId(userId);

            // Assert
            Assert.AreEqual(result.Count(), 3);
        }

        [TestMethod]
        public void GetEventsBetweenDates_GivenDates_ReturnAllEventsBetweenDates()
        {
            var userId = Guid.NewGuid().ToString();
            var startDate = DateTime.Today;
            var endDate = startDate.AddDays(1);

            var eventsForTest = new List<Event>
            {
                new Event
                {
                    Title = "title1",
                    Description = "description",
                    StartTime = startDate.AddHours(2),
                    EndTime = endDate,
                    UserId = userId,
                },
                new Event
                {
                    Title = "title2",
                    Description = "description",
                    StartTime = startDate.AddHours(10),
                    EndTime = endDate.AddHours(-2),
                    UserId = userId,
                },
                new Event
                {
                    Title = "title3",
                    Description = "description",
                    StartTime = startDate.AddHours(8),
                    EndTime = endDate.AddHours(-10),
                    UserId = userId,
                },
                new Event
                {
                    Title = "title3",
                    Description = "description",
                    StartTime = endDate.AddDays(2), // out of date range
                    EndTime = endDate.AddDays(5),
                    UserId = userId,
                },
            };

            // Act 
            var result = eventsForTest.AsQueryable().GetEventsBetweenDates(startDate, endDate);

            // Assert
            Assert.AreEqual(result.Count(), 3);
        }

        [TestMethod]
        public void GetEventsOnDate_GivenDate_ReturnAllEventsForThatDate()
        {
            var userId = Guid.NewGuid().ToString();
            var date = DateTime.Today;

            var eventsForTest = new List<Event>
            {
                new Event
                {
                    Title = "title1",
                    Description = "description",
                    StartTime = date,
                    EndTime = date.AddHours(2),
                    UserId = userId,
                },
                new Event
                {
                    Title = "title2",
                    Description = "description",
                    StartTime = date.AddHours(10),
                    EndTime = date.AddHours(12),
                    UserId = userId,
                },
                new Event
                {
                    Title = "title3",
                    Description = "description",
                    StartTime = date.AddHours(8),
                    EndTime = date.AddHours(15),
                    UserId = userId,
                },
                new Event
                {
                    Title = "title3",
                    Description = "description",
                    StartTime = date.AddDays(2), // different date
                    EndTime = date.AddDays(2).AddHours(1),
                    UserId = userId,
                },
            };

            // Act 
            var result = eventsForTest.AsQueryable().GetEventsOnDate(date);

            // Assert
            Assert.AreEqual(result.Count(), 3);
        }
    }
}