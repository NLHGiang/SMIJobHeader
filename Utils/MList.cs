namespace SMIJobHeader.Utils;

public class MList<T> : List<T>
{
    public new void Add(T item)
    {
        if (item != null) base.Add(item);
    }

    public new void AddRange(IEnumerable<T> listItem)
    {
        if (listItem != null) base.AddRange(listItem);
    }
}