using Discord;

namespace GregBot.Domain.Core.Events;

public record MessageEvent(IMessage Message) : Event
{
    public override bool IsCancellable => true;
}