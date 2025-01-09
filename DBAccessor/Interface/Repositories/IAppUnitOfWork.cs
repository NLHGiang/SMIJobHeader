using SMIJobXml.Entities.Interfaces;
namespace SMIJobXml.DBAccessor.Interface.Repositories
{
    public interface IAppUnitOfWork
    {
        IMongoRepository<TEntity, T> GetRepository<TEntity, T>() where TEntity : class, IBaseEntity<T>;
    }
}
