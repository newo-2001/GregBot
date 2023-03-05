using System.Collections;
using System.Collections.Generic;
using GregBot.Domain.Core.Attributes;
using GregBot.Domain.Core.Datastructures;

namespace GregBot.Domain.Core.Events;

public class ListenerList<T> : IEnumerable<EventListener<T>> where T : Event
{
    private readonly SortedList<EventListener<T>> _listeners;

    public ListenerList()
    {
        int Compare(EventListener<T> x, EventListener<T> y) =>
            x.Method.GetPriority() - y.Method.GetPriority();
        
        _listeners = new SortedList<EventListener<T>>(Compare);
    }

    public void AddListener(EventListener<T> listener) => _listeners.Add(listener);
    
    public IEnumerator<EventListener<T>> GetEnumerator() => _listeners.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}