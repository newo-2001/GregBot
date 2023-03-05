using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GregBot.Domain.Core.Events;

public class EventBus
{
    private readonly IDictionary<Type, object> _listenerLists = new Dictionary<Type, object>();

    public async Task Post<T>(T @event) where T : Event
    {
        if (!_listenerLists.TryGetValue(typeof(T), out var list))
            return;

        foreach (var listener in (IEnumerable<EventListener<T>>) list)
        {
            await listener(@event);

            if (@event.IsCancelled) break;
        }
    }

    public void Subscribe<T>(EventListener<T> listener) where T : Event
    {
        if (_listenerLists.TryGetValue(typeof(T), out var list))
        {
            ((ListenerList<T>) list).AddListener(listener);
            return;
        }

        var newList = new ListenerList<T>();
        newList.AddListener(listener);
        _listenerLists.Add(typeof(T), newList);
    }
}