using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Common.DbEventStore.MongoDB
{
    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> _collection;
        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;
        private readonly ILogger<MongoRepository<T>> _logger;

        public MongoRepository(
            IMongoDatabase database,
            string collectionName,
            ILogger<MongoRepository<T>> logger
        )
        {
            _collection = database.GetCollection<T>(collectionName);
            _logger = logger;
        }

        private FilterDefinition<T> BuildTenantFilter(Guid tenantId)
        {
            return Builders<T>.Filter.Eq(e => e.TenantId, tenantId);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(FilterDefinition<T>.Empty).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Guid tenantId)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.TenantId, tenantId);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Guid tenantId, Expression<Func<T, bool>> filter)
        {
            var tenantFilter = Builders<T>.Filter.Eq(e => e.TenantId, tenantId);
            var combinedFilter = Builders<T>.Filter.And(tenantFilter, filter);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<T> GetAsync(Guid tenantId, Guid id)
        {
            var tenantFilter = Builders<T>.Filter.Eq(e => e.TenantId, tenantId);
            var idFilter = Builders<T>.Filter.Eq(e => e.Id, id);
            var combinedFilter = Builders<T>.Filter.And(tenantFilter, idFilter);

            return await _collection.Find(combinedFilter).FirstOrDefaultAsync();
        }

        public async Task<T> GetAsync(Guid tenantId, Expression<Func<T, bool>> filter)
        {
            var tenantFilter = Builders<T>.Filter.Eq(e => e.TenantId, tenantId);
            var combinedFilter = Builders<T>.Filter.And(tenantFilter, filter);

            return await _collection.Find(combinedFilter).FirstOrDefaultAsync();
        }

        public async Task<T> FirstOrDefaultAsync()
        {
            return await _collection.Find(FilterDefinition<T>.Empty).FirstOrDefaultAsync();
        }

        public async Task<T> FirstOrDefaultAsync(Guid tenantId)
        {
            var tenantFilter = Builders<T>.Filter.Eq(e => e.TenantId, tenantId);
            return await _collection.Find(tenantFilter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), $"{nameof(entity)} cannot be null.");
            }

            await _collection.InsertOneAsync(entity);
        }

        public async Task CreateManyAsync(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities), $"{nameof(entities)} cannot be null.");
            }

            await _collection.InsertManyAsync(entities);
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), $"{nameof(entity)} cannot be null.");
            }

            FilterDefinition<T> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task RemoveAsync(Guid tenantId, Guid id)
        {
            if (id == Guid.Empty || tenantId == Guid.Empty)
            {
                throw new ArgumentNullException(id == Guid.Empty ? nameof(id) : nameof(tenantId), "Id and tenantId cannot be empty.");

            }

            var tenantFilter = BuildTenantFilter(tenantId);
            var idFilter = Builders<T>.Filter.Eq(entity => entity.Id, id);
            var combinedFilter = Builders<T>.Filter.And(tenantFilter, idFilter);

            await _collection.DeleteOneAsync(combinedFilter);
        }
    }

}
