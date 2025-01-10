using MongoDB.Driver;
using SMIJobHeader.DBAccessor.Interface.Repositories;
using SMIJobHeader.Entities.Interfaces;

namespace SMIJobHeader.DBAccessor;

public class AppUnitOfWork : IAppUnitOfWork
{
    private readonly IMongoDatabase _database;
    private readonly Dictionary<Type, object> _repositories;

    public AppUnitOfWork(IMongoDatabase database)
    {
        _database = database;
        _repositories = new Dictionary<Type, object>();
    }

    public IMongoRepository<TEntity, T> GetRepository<TEntity, T>() where TEntity : class, IBaseEntity<T>
    {
        return GetOrAddRepository(typeof(TEntity), () => new MongoRepository<TEntity, T>(_database));
    }

    private IMongoRepository<TEntity, T> GetOrAddRepository<TEntity, T>(Type entityType,
        Func<IMongoRepository<TEntity, T>> factory)
        where TEntity : class, IBaseEntity<T>
    {
        if (_repositories.TryGetValue(entityType, out var repository))
            return (IMongoRepository<TEntity, T>)repository;

        var newRepository = factory();
        _repositories[entityType] = newRepository;
        return newRepository;
    }
}