using Discord;
using FakeItEasy;
using GregBot.Domain.Events;
using GregBot.Domain.Models;
using GregBot.Domain.Modules.Parrot;
using GregBot.Domain.Services;
using Gregbot.Domain.Testing.Mocks;
using GregBot.Modules.Parrot;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Times = Moq.Times;

namespace GregBot.Tests.Modules.Parrot;

public class MessageHandlerTests
{
    private readonly Mock<ILogger<ParrotModule>> _loggerMock = new();
    private readonly Mock<IMessageService> _messageServiceMock = new();
    private readonly Mock<GregBotMock> _botMock = new();
    private readonly List<ParrotRule> _rules = new();

    public MessageHandlerTests()
    {
        var parrot = new ParrotModule(() => _rules, _loggerMock.Object, _messageServiceMock.Object);
        parrot.Load(_botMock.Object);
    }

    [Fact]
    public async Task NoReplyIsSentWhenMessageWasSentBySelf()
    {
        var message = A.Fake<IMessage>();
        
        _rules.Add(_ => new SendableMessage());
        _messageServiceMock.Setup(x => x.IsSentBySelf(message)).Returns(true);

        await ReceiveMessage(message);
        
        AssertNoReplyWasSent();
    }

    [Fact]
    public async Task CorrectReplyIsSentWhenRuleMatches()
    {
        var reply = new SendableMessage("bruh");
        var message = A.Fake<IMessage>();

        _rules.Add(_ => reply);
        await ReceiveMessage(message);
        
        _messageServiceMock.Verify(x => x.ReplyTo(message, reply), Times.Once);
    }

    [Fact]
    public async Task NoReplyIsSentWhenNoRuleMatches()
    {
        var message = A.Fake<IMessage>();
        
        _rules.Add(_ => null);
        
        await ReceiveMessage(message);
        
        AssertNoReplyWasSent();
    }

    private Task ReceiveMessage(IMessage message) =>
        _botMock.Object.MessageEventDispatcher.Dispatch(new MessageEvent(message));
    
    private void AssertNoReplyWasSent() =>
        _messageServiceMock.Verify(x =>
            x.ReplyTo(It.IsAny<IMessage>(), It.IsAny<SendableMessage>()
        ), Times.Never);
}