using System.Linq.Expressions;
using Discord;
using FluentAssertions;
using GregBot.Core.Services;
using GregBot.Domain.Core.Configuration;
using GregBot.Domain.Core.Models;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using Xunit;
using Times = Moq.Times;

namespace GregBot.Tests.Unit.Core.Services;

using MessageVerification = Expression<Func<IMessageChannel, Task<IUserMessage>>>;

public class MessageServiceTests
{
    private readonly AutoMocker _mocker = new();
    
    [Fact]
    public async Task Sending_AMessageWithoutAttachments_SendsTheMessageToDiscord()
    {
        var message = new SendableMessage("test");

        var messageService = _mocker.CreateInstance<MessageService>();
        var channel = new Mock<IMessageChannel>();

        await messageService.Send(channel.Object, message);

        channel.Verify(x =>
            x.SendMessageAsync(
                "test", false, null, null, null, null, null, null, null, MessageFlags.None
            ), Times.Once
        );
    }

    [Fact]
    public async Task Sending_AMessageWithASingleAttachment_SendsTheMessageToDiscord()
    {
        var attachment = GivenAttachment("test.jpg");
        var message = new SendableMessage(Content: "test", Attachments: new [] { attachment });

        var messageService = _mocker.CreateInstance<MessageService>();
        var channel = new Mock<IMessageChannel>();

        await messageService.Send(channel.Object, message);
        
        channel.Verify(x =>
            x.SendFileAsync(
                attachment, "test", false, null, null, null, null, null, null, null, MessageFlags.None
            ), Times.Once
        );
    }

    [Fact]
    public async Task Sending_AMessageWithMultipleAttachment_SendsTheMessageToDiscord()
    {
        var attachments = new[] { GivenAttachment("test.jpg"), GivenAttachment("test.png") };
        var message = new SendableMessage(Content: "test", Attachments: attachments);

        var messageService = _mocker.CreateInstance<MessageService>();
        var channel = new Mock<IMessageChannel>();

        await messageService.Send(channel.Object, message);
        
        channel.Verify(x =>
            x.SendFilesAsync(
                attachments, "test", false, null, null, null, null, null, null, null, MessageFlags.None
            ), Times.Once
        );
    }

    [Fact]
    public void AMessageSentByGregBot_Is_SentBySelf()
    {
        const ulong gregBotId = 4;
        ConfigureGregBotId(gregBotId);
        
        var messageService = _mocker.CreateInstance<MessageService>();
        var message = GivenMessageSentBy(GivenUserWithId(gregBotId));

        messageService.IsSentBySelf(message).Should().BeTrue();
    }

    [Fact]
    public void AMessageNotSentByGregBot_Is_NotSentBySelf()
    {
         const ulong gregBotId = 2;
         ConfigureGregBotId(gregBotId);
         
         var messageService = _mocker.CreateInstance<MessageService>();
         var message = GivenMessageSentBy(GivenUserWithId(gregBotId + 1));
 
         messageService.IsSentBySelf(message).Should().BeFalse();
    }

    private static FileAttachment GivenAttachment(string name) => new(Stream.Null, name);

    private void ConfigureGregBotId(ulong id)
    {
        var discordConfig = new DiscordConfiguration
        {
            ClientId = id,
            GuildId = 0,
            Token = string.Empty
        };
        
        _mocker.Use(Options.Create(discordConfig));
    }
    
    private static IMessage GivenMessageSentBy(IUser user)
    {
        var message = new Mock<IMessage>();
        message.Setup(x => x.Author).Returns(user);

        return message.Object;
    }

    private static IUser GivenUserWithId(ulong id)
    {
        var user = new Mock<IUser>();
        user.Setup(x => x.Id).Returns(id);

        return user.Object;
    }
}