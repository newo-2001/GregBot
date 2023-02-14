using Discord;
using GregBot.Domain.Events;

namespace GregBot.Events;

public record MessageEvent(IMessage Message) : Event;