using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using GregBot.Domain.Configuration;
using GregBot.Domain.Interfaces;
using GregBot.Domain.Models;
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

    public Task ReplyTo(IMessage original, SendableMessage reply)
    {
        var messageReference = new MessageReference(original.Id);
        var newReply = reply with { ReplyTo = messageReference };
        var channel = original.Channel;
        
        return Send(channel, newReply);
    }

    public Task Send(IMessageChannel channel, SendableMessage message)
    {
        switch (message.Attachments?.Count())
        {
            case null:
            case 0:
                if (message.Content is null)
                    throw new ArgumentException("Can't send empty message");
                return SendWithoutAttachments(channel, message);
            case 1:
                return SendWithSingleAttachment(channel, message);
            default:
                return SendWithMultipleAttachments(channel, message);
        }
    }
    
    private static Task SendWithoutAttachments(IMessageChannel channel, SendableMessage message) =>
        channel.SendMessageAsync(
            text: message.Content,
            isTTS: message.Tts, embeds: message.Embeds?.ToArray(),
            stickers: message.Stickers?.ToArray(), messageReference: message.ReplyTo);

    private static Task SendWithSingleAttachment(IMessageChannel channel, SendableMessage message) =>
        channel.SendFileAsync(
            text: message.Content, attachment: message.Attachments!.First(),
            isTTS: message.Tts, embeds: message.Embeds?.ToArray(),
            stickers: message.Stickers?.ToArray(), messageReference: message.ReplyTo);
    
    private static Task SendWithMultipleAttachments(IMessageChannel channel, SendableMessage message) =>
       channel.SendFilesAsync(
           text: message.Content, attachments: message.Attachments!.ToArray(),
           isTTS: message.Tts, embeds: message.Embeds?.ToArray(),
           stickers: message.Stickers?.ToArray(), messageReference: message.ReplyTo); 
}