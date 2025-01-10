namespace SMIJobHeader.Entities.Interfaces;

public interface IBaseEntity<T>
{
    T Id { get; set; }
}