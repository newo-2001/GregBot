using GregBot.Domain.Attributes;
using GregBot.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GregBot.Domain.Models;

public class EventDispatcher<E> : IEventTopic<E> where E : Event
{
    private readonly IDictionary<int, EventHandler<E>> _handlers = new SortedList<int, EventHandler<E>>();

    public void Subscribe(EventHandler<E> handler)
    {
        var priority = handler.Method.GetPriority();

        _handlers.Add(priority, handler);
    }

    public async Task Dispatch(E @event)
    {
        foreach (var (_, handler) in _handlers)
        {
            if (@event.IsCancelled) break;

            await handler(@event);
        }
    }
}
