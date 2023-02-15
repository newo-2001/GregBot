using System.Threading.Tasks;
using Discord;

namespace GregBot.Domain.Interfaces;

public interface IMessageService
{
    bool IsSentBySelf(IMessage message);
    Task Reply(string reply, IMessage original);
}