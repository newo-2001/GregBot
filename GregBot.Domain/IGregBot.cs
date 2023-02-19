using System.Threading.Tasks;
using GregBot.Domain.Events;

namespace GregBot.Domain;

public interface IGregBot
{
    public IEventTopic<MessageEvent> OnMessage { get; }

    public Task Login();
    public Task Logout();
}