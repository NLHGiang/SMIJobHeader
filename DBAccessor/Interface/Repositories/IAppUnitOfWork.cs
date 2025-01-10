using SMIJobHeader.Entities.Interfaces;

namespace SMIJobHeader.DBAccessor.Interface.Repositories;

public interface IAppUnitOfWork
{
    IMongoRepository<TEntity, T> GetRepository<TEntity, T>() where TEntity : class, IBaseEntity<T>;
}