using System.Threading.Tasks;
using Discord;
using GregBot.Domain.Configuration;
using GregBot.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace GregBot.Services;

public class MessageService : IMessageService
{
    private readonly DiscordConfiguration _discordConfiguration;
    
    public MessageService(IOptions<DiscordConfiguration> discordConfig)
    {
        _discordConfiguration = discordConfig.Value;
    }

    public bool IsSentBySelf(IMessage message) =>
        message.Author.Id.Equals(_discordConfiguration.ClientId);

    public Task Reply(string reply, IMessage originalMessage)
    {
        var messageReference = new MessageReference(originalMessage.Id);
        var channel = originalMessage.Channel;
        return channel.SendMessageAsync(reply, messageReference: messageReference);
    }
}