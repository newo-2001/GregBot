using System.Threading.Tasks;
using GregBot.Domain.Models;

namespace GregBot.Domain.Interfaces;

public interface IEventTopic<E> where E : Event
{
    void Subscribe(EventHandler<E> handler);
}

public delegate Task EventHandler<E>(E @event) where E : Event;
