using MongoDB.Bson;
using MongoDB.Driver;
using SMIJobXml.DBAccessor.Interface.Repositories;
using SMIJobXml.Entities.Interfaces;

namespace SMIJobXml.DBAccessor
{
    public class MongoRepository<TEntity, T> : IMongoRepository<TEntity, T>
    where TEntity : class, IBaseEntity<T>
    {
        private readonly IMongoCollection<TEntity> _moderCollection;

        public MongoRepository(IMongoDatabase database)
        {
            _moderCollection = database.GetCollection<TEntity>(typeof(TEntity).Name.ToLower());
        }

        public long CountItem(List<FilterDefinition<TEntity>> filters)
        {
            throw new NotImplementedException();
        }

        public long CountItem(List<FilterDefinition<TEntity>> filters, int startingFrom = 0, int count = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> Filter(SortDefinition<TEntity> sort, int startingFrom = 0, int count = int.MaxValue)
        {
            return _moderCollection.Find(new BsonDocument()).Sort(sort).Skip(startingFrom).Limit(count).ToList();
        }



        public IEnumerable<TEntity> Filter(FilterDefinition<TEntity> combineFilters, SortDefinition<TEntity> sort, int startingFrom = 0, int count = int.MaxValue)
        {
            if (sort == null)
            {
                return _moderCollection.Find(combineFilters).Skip(startingFrom).Limit(count).ToList();
            }
            else
            {
                return _moderCollection.Find(combineFilters).Sort(sort).Skip(startingFrom).Limit(count).ToList();
            }
        }

        public IEnumerable<TEntity> Filter(List<FilterDefinition<TEntity>> filters, SortDefinition<TEntity> sort, int startingFrom = 0, int count = int.MaxValue)
        {
            if (filters == null || filters.Count == 0)
            {
                return Filter(sort, startingFrom, count);
            }
            FilterDefinition<TEntity> combineFilters = Builders<TEntity>.Filter.And(filters);
            return Filter(combineFilters, sort, startingFrom, count);
        }
        public async Task<TEntity> Get(FilterDefinition<TEntity> filter)
        {
            return await _moderCollection.Find(filter).FirstOrDefaultAsync();
        }
        public Task<TEntity> GetSort(FilterDefinition<TEntity> filter, SortDefinition<TEntity> sort)
        {
            return _moderCollection.Find(filter).Sort(sort).FirstOrDefaultAsync();
        }

        public async Task Insert(TEntity data)
        {
            await this._moderCollection.InsertOneAsync(data);
        }

        public async Task InsertMany(IEnumerable<TEntity>? data)
        {
            await this._moderCollection.InsertManyAsync(data);
        }

        public async Task UpdateAsync(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update, UpdateOptions options = null)
        {
            await _moderCollection.UpdateOneAsync(filter, update, options);
        }


        public async Task Update(FilterDefinition<TEntity> combineFilters, TEntity data)
        {
            await this._moderCollection.DeleteOneAsync(combineFilters);
            await this._moderCollection.InsertOneAsync(data);
        }

        public bool Exist(FilterDefinition<TEntity> filter)
        {
            return this._moderCollection.Find(filter).Any();
        }
    }
}
