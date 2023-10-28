using Calendar.DataAccess.Repositories;
using Calendar.shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Calendar.DataAccess.Tests
{
    [TestClass]
    public class EventRepositoryTests
    {
        private AppDbContext _dbContext;
        private readonly Mock<ILogger<EventRepository>> _mockLogger;
        private EventRepository _eventRepository;

        public EventRepositoryTests()
        {
            _mockLogger = new Mock<ILogger<EventRepository>>();
        }

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new AppDbContext(options);
            _eventRepository = new EventRepository(_dbContext, _mockLogger.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _dbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task AddAsync_ReturnSuccess()
        {
            // Arrange
            var entity = new Event
            {
                Title = "title",
                Description = "description",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMinutes(30),
                UserId = Guid.NewGuid().ToString(),
            };

            // Act
            var result = await _eventRepository.AddAsync(entity);
            var getentityResult = await _eventRepository.GetByIdAsync(entity.Id);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.IsTrue(getentityResult.IsOk);
            Assert.AreEqual(entity.Id, getentityResult.Entity!.Id);
            Assert.AreEqual(entity.Title, getentityResult.Entity!.Title);
            Assert.AreEqual(entity.Description, getentityResult.Entity!.Description);
            Assert.AreEqual(entity.StartTime, getentityResult.Entity!.StartTime);
            Assert.AreEqual(entity.EndTime, getentityResult.Entity!.EndTime);
            Assert.AreEqual(entity.UserId, getentityResult.Entity!.UserId);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsEntity_WhenEntityExists()
        {
            // Arrange
            var entity = new Event
            {
                Title = "title",
                Description = "description",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMinutes(30),
                UserId = Guid.NewGuid().ToString(),
            };

            _dbContext.Events.Add(entity);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _eventRepository.GetByIdAsync(entity.Id);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(entity.Id, result.Entity!.Id);
            Assert.AreEqual(entity.Title, result.Entity!.Title);
            Assert.AreEqual(entity.Description, result.Entity!.Description);
            Assert.AreEqual(entity.StartTime, result.Entity!.StartTime);
            Assert.AreEqual(entity.EndTime, result.Entity!.EndTime);
            Assert.AreEqual(entity.UserId, result.Entity!.UserId);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsNotFound_WhenEntityDoNotExists()
        {
            // Arrange
            // Act
            var result = await _eventRepository.GetByIdAsync(1);

            // Assert
            Assert.IsTrue(result.IsNotFound);
        }

        [TestMethod]
        public async Task UpdateAsync_ReturnSuccess_WhenEntityExists()
        {
            // Arrange
            var entity = new Event
            {
                Title = "title",
                Description = "description",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMinutes(30),
                UserId = Guid.NewGuid().ToString(),
            };

            _dbContext.Events.Add(entity);
            await _dbContext.SaveChangesAsync();

            entity.Description = "upated description";

            // Act
            var result = await _eventRepository.UpdateAsync(entity);
            var updatedResult = await _eventRepository.GetByIdAsync(entity.Id);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.IsTrue(updatedResult.IsOk);
            Assert.AreEqual(entity.Id, updatedResult.Entity!.Id);
            Assert.AreEqual(entity.Title, updatedResult.Entity!.Title);
            Assert.AreEqual(entity.Description, updatedResult.Entity!.Description);
            Assert.AreEqual(entity.StartTime, updatedResult.Entity!.StartTime);
            Assert.AreEqual(entity.EndTime, updatedResult.Entity!.EndTime);
            Assert.AreEqual(entity.UserId, updatedResult.Entity!.UserId);
        }

        [TestMethod]
        public async Task UpdateAsync_ReturnNotFound_WhenEntityDoNotExists()
        {
            // Arrange
            var entity = new Event
            {
                Title = "title",
                Description = "description",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMinutes(30),
                UserId = Guid.NewGuid().ToString(),
            };

            // Act
            var result = await _eventRepository.UpdateAsync(entity);

            // Assert
            Assert.IsTrue(result.IsNotFound);
        }

        [TestMethod]
        public async Task DeleteAsync_ReturnsSuccess_WhenEntityExists()
        {
            // Arrange
            var entity = new Event
            {
                Title = "title",
                Description = "description",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMinutes(30),
                UserId = Guid.NewGuid().ToString(),
            };

            _dbContext.Events.Add(entity);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _eventRepository.DeleteAsync(entity.Id);
            var deletedEntityResult = await _eventRepository.GetByIdAsync(entity.Id);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.IsTrue(deletedEntityResult.IsNotFound);
        }

        [TestMethod]
        public async Task DeleteAsync_ReturnsNotFound_WhenEntityDoNotExists()
        {
            // Arrange
            var entity = new Event
            {
                Title = "title",
                Description = "description",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMinutes(30),
                UserId = Guid.NewGuid().ToString(),
            };

            // Act
            var result = await _eventRepository.DeleteAsync(entity.Id);

            // Assert
            Assert.IsTrue(result.IsNotFound);
        }

        [TestMethod]
        public async Task GetEventsForUserBetweenDatesAsync_ReturnsEventsForGivenCriteria()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var startTime = DateTime.Now;
            var endTime = DateTime.Now.AddHours(1);
            var event1 = new Event
            {
                Title = "title1",
                Description = "description",
                UserId = userId,
                StartTime = startTime,
                EndTime = endTime
            };
            var event2 = new Event
            {
                Title = "title2",
                Description = "description",
                UserId = userId,
                StartTime = startTime.AddMinutes(15),
                EndTime = endTime.AddMinutes(-15)
            };

            _dbContext.Events.AddRange(event1, event2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _eventRepository.GetEventsForUserBetweenDatesAsync(userId, startTime, endTime);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(2, result.Entity!.Count);
        }

        [TestMethod]
        public async Task GetEventsForUserBetweenDatesAsync_ReturnsEmptyWhenNoEventsMatchCriteria()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var startTime = DateTime.Now;
            var endTime = DateTime.Now.AddMinutes(30);

            // Act
            var result = await _eventRepository.GetEventsForUserBetweenDatesAsync(userId, startTime, endTime);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(0, result.Entity!.Count);
        }

        [TestMethod]
        public async Task GetEventsForUserOnDateAsync_ReturnsEventsForGivenCriteria()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var date = DateTime.Today;
            var event1 = new Event
            {
                Title = "title1",
                Description = "description",
                UserId = userId,
                StartTime = date,
                EndTime = date.AddHours(2)
            };
            var event2 = new Event
            {
                Title = "title2",
                Description = "description",
                UserId = Guid.NewGuid().ToString(), // different user.
                StartTime = date,
                EndTime = date.AddHours(3)
            };

            _dbContext.Events.AddRange(event1, event2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _eventRepository.GetEventsForUserOnDateAsync(userId, date);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(1, result.Entity!.Count);
        }

        [TestMethod]
        public async Task GetEventsForUserOnDateAsync_ReturnsEmptyWhenNoEventsMatchCriteria()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var date = DateTime.Today;

            // Act
            var result = await _eventRepository.GetEventsForUserOnDateAsync(userId, date);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(0, result.Entity!.Count);
        }
    }
}
