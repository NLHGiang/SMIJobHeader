using MongoDB.Driver;
using SMIJobHeader.Entities.Interfaces;

namespace SMIJobHeader.DBAccessor.Interface.Repositories;

public interface IMongoRepository<TEntity, T> where TEntity : class, IBaseEntity<T>
{
    Task<TEntity> Get(FilterDefinition<TEntity> filter);
    Task UpdateAsync(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update, UpdateOptions options = null);
    Task Update(FilterDefinition<TEntity> combineFilters, TEntity data);
    Task Insert(TEntity data);
    Task InsertMany(IEnumerable<TEntity>? data);
    IEnumerable<TEntity> Filter(SortDefinition<TEntity> sort, int startingFrom = 0, int count = int.MaxValue);

    IEnumerable<TEntity> Filter(FilterDefinition<TEntity> combineFilters, SortDefinition<TEntity> sort,
        int startingFrom = 0, int count = int.MaxValue);

    long CountItem(List<FilterDefinition<TEntity>> filters);
    long CountItem(List<FilterDefinition<TEntity>> filters, int startingFrom = 0, int count = int.MaxValue);

    IEnumerable<TEntity> Filter(List<FilterDefinition<TEntity>> filters, SortDefinition<TEntity> sort,
        int startingFrom = 0, int count = int.MaxValue);

    Task<TEntity> GetSort(FilterDefinition<TEntity> filter, SortDefinition<TEntity> sort);

    bool Exist(FilterDefinition<TEntity> filter);
}