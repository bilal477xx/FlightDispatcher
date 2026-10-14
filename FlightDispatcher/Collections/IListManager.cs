using System.Collections.Generic;

namespace FlightDispatcher.Collections;

public interface IListManager<T>
{
    int Count { get; }
    
    bool Add(T item);
    bool ChangeAt(int index, T item);
    bool DeleteAt(int index);
    T GetAt(int index);
    IReadOnlyList<T> GetAll();
    bool CheckIndex(int index);
    int GetIndexOf(T item);
    void DeleteAll();
    string[] ToStringArray();
    List<string> ToStringList();
}