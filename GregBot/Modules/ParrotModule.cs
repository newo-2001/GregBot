using GregBot.Domain.Attributes;
using GregBot.Domain.Interfaces;
using GregBot.Events;
using Microsoft.Extensions.Logging;

namespace GregBot.Modules;

[Priority(Priority.Low)]
public class ParrotModule : IModule
{
    private readonly ILogger<ParrotModule> _logger;

    public string Name => "Parrot";

    public ParrotModule(ILogger<ParrotModule> logger)
    {
        _logger = logger;
    }

    public void Activate(GregBot bot)
    {
        bot.OnMessage.Subscribe(OnMessage);
    }
    
    [Priority(Priority.Medium)]
    private Task OnMessage(MessageEvent @event)
    {
        _logger.LogTrace("Parrot module received: {msg}", @event.Message.Content);

        return Task.CompletedTask;
    }
}
