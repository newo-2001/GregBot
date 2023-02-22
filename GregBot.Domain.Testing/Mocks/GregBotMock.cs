using GregBot.Domain;
using GregBot.Domain.Events;

namespace GregBot.Domain.Testing.Mocks;

public abstract class GregBotMock : IGregBot
{
    public EventDispatcher<MessageEvent> MessageEventDispatcher { get; } = new();
    public IEventTopic<MessageEvent> OnMessage => MessageEventDispatcher;

    public abstract Task Login();
    public abstract Task Logout();
}