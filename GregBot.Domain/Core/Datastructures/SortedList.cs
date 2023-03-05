using System;
using System.Collections;
using System.Collections.Generic;

namespace GregBot.Domain.Core.Datastructures;

public class SortedList<T> : ICollection<T>, IReadOnlyList<T>
{
    private readonly List<T> _list = new();
    private readonly IComparer<T> _comparer;

    public SortedList(IComparer<T> comparer)
    {
        _comparer = comparer;
    }
    
    public SortedList(Func<T, T, int> comparer)
        : this(Comparer<T>.Create(new Comparison<T>(comparer))) {}
    
    public SortedList() : this(Comparer<T>.Default) { }

    public void Add(T item)
    {
        var index = _list.BinarySearch(item, _comparer);
        if (index < 0)
            index = ~index;
        
        _list.Insert(index, item);
    }

    public bool Remove(T item)
    {
        var index = _list.BinarySearch(item, _comparer);
        if (index < 0)
            return false;
        
        _list.RemoveAt(index);
        return true;
    }

    public void Clear() => _list.Clear();

    public bool Contains(T item) => _list.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) =>_list.CopyTo(array, arrayIndex);

    public int Count => _list.Count;

    public bool IsReadOnly => ((ICollection<T>) _list).IsReadOnly;

    public T this[int index] => _list[index];
    
    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}