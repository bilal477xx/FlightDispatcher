using System;
using System.Collections.Generic;

namespace FlightDispatcher.Collections;

public class ListManager<T> : IListManager<T>
{
    protected readonly List<T> _list = new();
    
    public event Action? ListChanged;
    
    public int Count { get => _list.Count; }
    
    public bool Add(T item)
    {
        if (item == null) return false;
        
        _list.Add(item);
        ListChanged?.Invoke();
        return true;
    }

    public bool ChangeAt(int index, T item)
    {
        if (!CheckIndex(index)) return false;
        if (item == null) return false;
        
        _list[index] = item;
        ListChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire ListManager.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>The zero-based index of the first occurrence of item within the entire ListManager, if found; otherwise, -1.</returns>
    public int GetIndexOf(T item)
    {
        return _list.IndexOf(item);
    }

    public bool DeleteAt(int index)
    {
        if (!CheckIndex(index)) return false;
        
        _list.RemoveAt(index);
        ListChanged?.Invoke();
        return true;
    }

    public T GetAt(int index)
    {
        if (!CheckIndex(index)) throw  new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range");
        
        return _list[index];
    }
    
    public IReadOnlyList<T> GetAll() => _list.AsReadOnly();

    public bool CheckIndex(int index)
    {
        if (_list.Count <= index || index < 0) return false;
        return true;
    }

    public void DeleteAll()
    {
        _list.Clear();
        ListChanged?.Invoke();
    }

    public string[] ToStringArray()
    {
        return ToStringList().ToArray();
    }

    public List<string> ToStringList()
    {
        List<string> stringList = new List<string>();
        foreach (T item in _list)
        {
            stringList.Add(item!.ToString()!);
        }
        
        return stringList;
    }
    
}