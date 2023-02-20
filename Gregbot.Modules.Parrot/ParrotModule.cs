using GregBot.Domain;
using GregBot.Domain.Attributes;
using GregBot.Domain.Events;
using GregBot.Domain.Interfaces;
using GregBot.Domain.Services;
using Microsoft.Extensions.Logging;

namespace GregBot.Modules.Parrot;

[Priority(Priority.Low)]
public class ParrotModule : IModule
{
    private readonly ILogger<ParrotModule> _logger;
    private readonly IEnumerable<ParrotRule> _rules;
    private readonly IMessageService _messageService;

    public ParrotModule(ParrotRuleProvider ruleProvider, ILogger<ParrotModule> logger, IMessageService messageService)
    {
        _rules = ruleProvider();
        _logger = logger;
        _messageService = messageService;
    }
    
    public string Name => "Parrot";
    public void Activate(IGregBot bot)
    {
        bot.OnMessage.Subscribe(OnMessage);
    }

    [Priority(Priority.High)]
    private Task OnMessage(MessageEvent @event)
    {
        var message = @event.Message;

        if (_messageService.IsSentBySelf(message))
            return Task.CompletedTask;

        var content = message.Content;
        foreach (var rule in _rules)
        {
            var reply = rule(content);
            if (reply is null) continue;
            
            _logger.LogDebug("Replied with '{Reply}' to '{Message}'", reply, content);
            return _messageService.ReplyTo(message, reply);
        }

        return Task.CompletedTask;
    }
}