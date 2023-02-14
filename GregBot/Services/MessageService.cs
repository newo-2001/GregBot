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
}