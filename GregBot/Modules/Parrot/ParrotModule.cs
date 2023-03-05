using System.Collections.Generic;
using System.Threading.Tasks;
using GregBot.Domain.Core.Attributes;
using GregBot.Domain.Core.Events;
using GregBot.Domain.Core.Interfaces;
using GregBot.Domain.Core.Services;
using GregBot.Domain.Modules.Parrot;
using Microsoft.Extensions.Logging;

namespace GregBot.Modules.Parrot;

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

    [SubscribeEvent(EventPriority.HIGH)]
    public Task OnMessage(MessageEvent @event)
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