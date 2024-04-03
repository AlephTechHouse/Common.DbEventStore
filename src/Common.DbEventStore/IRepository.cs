using System.Linq.Expressions;

namespace Common.DbEventStore
{
    public interface IRepository<T> where T : IEntity
    {
        // Define the methods that the repository should implement.
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(Guid tenantId);
        Task<IEnumerable<T>> GetAllAsync(Guid tenantId, Expression<Func<T, bool>> filter);
        Task<T> GetAsync(Guid tenantId, Guid id);
        Task<T> GetAsync(Guid tenantId, Expression<Func<T, bool>> filter);
        Task<T> FirstOrDefaultAsync();
        Task<T> FirstOrDefaultAsync(Guid tenantId);
        Task CreateAsync(T entity);
        Task CreateManyAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task RemoveAsync(Guid tenantId, Guid id);
    }
}