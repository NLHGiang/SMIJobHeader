using MongoDB.Bson;
using MongoDB.Driver;

namespace SMIJobXml.DBAccessor.Interface
{
    public interface IRepository<T>
    {
        T Get(FilterDefinition<T> filter);
        void Update(T data);
        Task UpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> updateDefinition);
        Task Insert(T data);
        void Delete(ObjectId id);
        IEnumerable<T> Filter(SortDefinition<T> sort, int startingFrom = 0, int count = int.MaxValue);
        IEnumerable<T> Filter(FilterDefinition<T> combineFilters, SortDefinition<T> sort, int startingFrom = 0, int count = int.MaxValue);
        IEnumerable<T> Filter(List<FilterDefinition<T>> filters, SortDefinition<T> sort, int startingFrom = 0, int count = int.MaxValue);
        long CountItem(List<FilterDefinition<T>> filters);
    }
}
