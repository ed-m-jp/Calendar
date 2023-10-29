using Calendar.shared.Entities;

namespace Calendar.DataAccess.Queries
{
    internal static class EventQueries
    {
        internal static IQueryable<Event> GetEventForUserId(this IQueryable<Event> events, string userId)
            => events.Where(e => e.UserId == userId);

        internal static IQueryable<Event> GetEventsBetweenDates(this IQueryable<Event> events,DateTime startTime, DateTime endTime)
            => events.Where(e => e.StartTime.Date <= endTime.Date && e.EndTime.Date >= startTime.Date);

        internal static IQueryable<Event> GetEventsOnDate(this IQueryable<Event> events, DateTime date)
            => events.Where(e => e.StartTime.Date <= date.Date && e.EndTime.Date >= date.Date);
    }
}
