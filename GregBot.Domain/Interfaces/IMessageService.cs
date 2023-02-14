using Discord;

namespace GregBot.Domain.Interfaces;

public interface IMessageService
{
    bool IsSentBySelf(IMessage message);
}