using System.Threading.Tasks;
using Discord;
using GregBot.Domain.Core.Models;

namespace GregBot.Domain.Core.Services;

public interface IMessageService
{
    bool IsSentBySelf(IMessage message);
    Task ReplyTo(IMessage original, SendableMessage reply);
    Task Send(IMessageChannel channel, SendableMessage message);
}