using Calendar.DataAccess.Infra;
using Calendar.shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Calendar.DataAccess.Repositories
{
    public abstract class RepositoryBase<T> where T : EntityBase
    {
        protected readonly AppDbContext DbContext;
        protected readonly ILogger _logger;

        protected RepositoryBase(AppDbContext dbContext, ILogger logger)
        {
            DbContext = dbContext;
            _logger = logger;
        }

        public async Task<RepositoryActionResult<T>> GetByIdAsync(int entityId)
        {
            try
            {
                var result = await DbContext.Set<T>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == entityId);

                if (result != null)
                    return RepositoryActionResult<T>.Ok(result);
                else
                    return RepositoryActionResult<T>.NotFound(entityId);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Failed to fetch entity of type [{typeof(T)}] and for entity id : [{entityId}].");
                return RepositoryActionResult<T>.Error(ex, ex.Message);
            }
        }

        public async Task<RepositoryActionResult> AddAsync(T entity)
        {
            try
            {
                DbContext.Set<T>().Add(entity);

                return await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Failed to add new entity of type [{typeof(T)}].");
                return RepositoryActionResult.Error(ex, ex.Message);
            }
        }

        public async Task<RepositoryActionResult> UpdateAsync(T entity)
        {
            try
            {
                // you can only update when one already exists for this id
                var entityInDb = await DbContext.Set<T>().FindAsync(entity.Id);

                if (entityInDb == null)
                    return RepositoryActionResult.NotFound(entity.Id);

                // change the original entity status to detached; otherwise, we get an error on attach
                // as the entity is already in the dbSet
                DbContext.Entry(entityInDb).State = EntityState.Detached;

                // attach the updated entity
                DbContext.Set<T>().Attach(entity);

                // set the updated entity state to modified, so it gets updated.
                DbContext.Entry(entity).State = EntityState.Modified;

                return await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string entityJson = JsonSerializer.Serialize(entity);
                _logger.LogCritical($"Failed to update entity of type [{typeof(T)}] for entity id : [{entity.Id}] and values : [{entityJson}].");
                return RepositoryActionResult.Error(ex, ex.Message);
            }
        }

        public async Task<RepositoryActionResult> DeleteAsync(int entityId)
        {
            try
            {
                var todelete = await DbContext.Set<T>().FindAsync(entityId);

                if (todelete != null)
                {
                    DbContext.Set<T>().Remove(todelete);
                    return await SaveChangesAsync();
                }
                else
                    return RepositoryActionResult.NotFound(entityId);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Failed to delete entity of type [{typeof(T)}] for entity id : [{entityId}].");
                return RepositoryActionResult.Error(ex, ex.Message);
            }
        }

        private async Task<RepositoryActionResult> SaveChangesAsync()
        {
            var result = await DbContext.SaveChangesAsync();
            
            if (result == 0)
                return RepositoryActionResult.Error("Save to database Failed.");
            
            return RepositoryActionResult.Ok();
        }
    }
}
