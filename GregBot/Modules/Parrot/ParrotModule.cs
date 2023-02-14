using GregBot.Domain.Attributes;
using GregBot.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Discord;
using GregBot.Domain.Events;

namespace GregBot.Modules.Parrot;

[Priority(Priority.Low)]
public class ParrotModule : IModule
{
    private readonly ILogger<ParrotModule> _logger;
    private readonly IMessageService _messageService;

    public string Name => "Parrot";

    public ParrotModule(ILogger<ParrotModule> logger, IMessageService messageService)
    {
        _logger = logger;
        _messageService = messageService;
    }

    public void Activate(Domain.GregBot bot)
    {
        bot.OnMessage.Subscribe(OnMessage);
    }

    [Priority(Priority.Medium)]
    private Task OnMessage(MessageEvent @event)
    {
        var message = @event.Message;
        if (_messageService.IsSentBySelf(message))
            return Task.CompletedTask;

        var content = message.Content;
        var reply = Replies.ReplyFor(content);

        if (reply is null)
            return Task.CompletedTask;
        
        _logger.LogDebug("Replied with \'{Reply}\' to the message \'{Message}\'", reply, content);

        var messageReference = new MessageReference(message.Id);
        return message.Channel.SendMessageAsync(reply, messageReference: messageReference);
    }
}
