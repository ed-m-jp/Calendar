using Calendar.DataAccess.Infra;
using Calendar.shared.Entities;

namespace Calendar.DataAccess.Interfaces
{
    public interface IEventRepository
    {

        Task<RepositoryActionResult<Event>> GetByIdAsync(int entityId);

        Task<RepositoryActionResult> AddAsync(Event entity);

        Task<RepositoryActionResult> UpdateAsync(Event entity);

        Task<RepositoryActionResult> DeleteAsync(int entityId);

        Task<RepositoryActionResult<IReadOnlyList<Event>>> GetEventsForUserOnDateAsync(string userId, DateTime date);

        Task<RepositoryActionResult<IReadOnlyList<Event>>> GetEventsForUserBetweenDatesAsync(string userId, DateTime startTime, DateTime endTime);
    }
}