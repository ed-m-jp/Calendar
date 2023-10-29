using Calendar.DataAccess.Infra;
using Calendar.DataAccess.Interfaces;
using Calendar.DataAccess.Queries;
using Calendar.shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Calendar.DataAccess.Repositories
{
    public class EventRepository : RepositoryBase<Event>, IEventRepository
    {
        public EventRepository([NotNull] AppDbContext dbContext, ILogger<EventRepository> logger) 
            : base(dbContext, logger) {}

        public async Task<RepositoryActionResult<IReadOnlyList<Event>>> GetEventsForUserBetweenDatesAsync(
            string userId, DateTime startTime, DateTime endTime)
        {
            try
            {
                var events = await DbContext.Events
                    .AsNoTracking()
                    .GetEventForUserId(userId)
                    .GetEventsBetweenDates(startTime, endTime)
                    .ToListAsync();

                return RepositoryActionResult<IReadOnlyList<Event>>.Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"GetEventsForUserBetweenDateAsync failed for [{userId}], start date [{startTime}] and end date [{endTime}] with exception [{ex}]");
                return RepositoryActionResult<IReadOnlyList<Event>>.Error(ex, ex.Message);
            }
        }

        public async Task<RepositoryActionResult<IReadOnlyList<Event>>> GetEventsForUserOnDateAsync(
            string userId, DateTime date)
        {
            try
            {
                var events = await DbContext.Events
                    .AsNoTracking()
                    .GetEventForUserId(userId)
                    .GetEventsOnDate(date)
                    .ToListAsync();

                return RepositoryActionResult<IReadOnlyList<Event>>.Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"GetEventsForUserAndDateAsync failed for [{userId}] and date [{date}] with exception [{ex}]");
                return RepositoryActionResult<IReadOnlyList<Event>>.Error(ex, ex.Message);
            }
        }
    }
}
