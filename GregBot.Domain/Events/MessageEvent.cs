using Discord;
using GregBot.Domain.Models;

namespace GregBot.Domain.Events;

public record MessageEvent(IMessage Message) : Event;