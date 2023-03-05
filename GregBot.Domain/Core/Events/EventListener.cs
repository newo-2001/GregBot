using System.Threading.Tasks;

namespace GregBot.Domain.Core.Events;

public delegate Task EventListener<in T>(T @event) where T : Event;