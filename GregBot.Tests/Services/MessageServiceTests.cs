using System.Linq.Expressions;
using Discord;
using FakeItEasy;
using GregBot.Domain.Configuration;
using GregBot.Domain.Models;
using GregBot.Domain.Services;
using GregBot.Services;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Times = Moq.Times;

namespace GregBot.Tests.Services;

using MessageVerification = Expression<Func<IMessageChannel, Task<IUserMessage>>>;

public class MessageServiceTests
{
    private static readonly IOptions<DiscordConfiguration> DiscordConfig = A.Fake<IOptions<DiscordConfiguration>>();
    
    private readonly IMessageService _messageService = new MessageService(DiscordConfig);
    private readonly Mock<IMessageChannel> _channelMock = new();
    
    [Fact]
    public async Task SendingAMessageWithoutAttachmentsSendsTheMessageToDiscord()
    {
        var message = new SendableMessage("test");
        
        await _messageService.Send(_channelMock.Object, message);

        _channelMock.Verify(x =>
            x.SendMessageAsync(
                "test", false, null, null, null, null, null, null, null, MessageFlags.None
            ), Times.Once
        );
    }

    [Fact]
    public async Task SendingAMessageWithASingleAttachmentSendsTheMessageToDiscord()
    {
        var attachment = GivenAttachment("test.jpg");
        var message = new SendableMessage(Content: "test", Attachments: new [] { attachment });

        await _messageService.Send(_channelMock.Object, message);
        
        _channelMock.Verify(x =>
            x.SendFileAsync(
                attachment, "test", false, null, null, null, null, null, null, null, MessageFlags.None
            ), Times.Once
        );
    }

    [Fact]
    public async Task SendingAMessageWithMultipleAttachmentSendsTheMessageToDiscord()
    {
        var attachments = new[] { GivenAttachment("test.jpg"), GivenAttachment("test.png") };
        var message = new SendableMessage(Content: "test", Attachments: attachments);

        await _messageService.Send(_channelMock.Object, message);
        
        _channelMock.Verify(x =>
            x.SendFilesAsync(
                attachments, "test", false, null, null, null, null, null, null, null, MessageFlags.None
            ), Times.Once
        );
    }

    private static FileAttachment GivenAttachment(string name) => new(Stream.Null, name);
}