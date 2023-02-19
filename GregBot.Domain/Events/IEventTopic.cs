using System.Threading.Tasks;
using GregBot.Domain.Models;

namespace GregBot.Domain.Events;

public interface IEventTopic<out T> where T : Event
{
    void Subscribe(EventHandler<T> handler);
}

public delegate Task EventHandler<in T>(T @event) where T : Event;
