using System.Threading.Tasks;
using Discord;
using GregBot.Domain.Models;

namespace GregBot.Domain.Interfaces;

public interface IMessageService
{
    bool IsSentBySelf(IMessage message);
    Task ReplyTo(IMessage original, SendableMessage reply);
    Task Send(IMessageChannel channel, SendableMessage message);
}