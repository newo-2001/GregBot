using Discord;
using GregBot.Domain.Core.Events;
using GregBot.Domain.Core.Models;
using GregBot.Domain.Core.Services;
using GregBot.Domain.Modules.Parrot;
using GregBot.Modules.Parrot;
using Moq;
using Moq.AutoMock;
using Xunit;
using Times = Moq.Times;

namespace GregBot.Tests.Unit.Modules.Parrot;

public class MessageHandlerTests
{
    private readonly AutoMocker _mocker = new();
    
    [Fact]
    public async Task NoReplyIsSent_When_MessageWasSentBySelf()
    {
        var message = Mock.Of<IMessage>();
        var module = _mocker.CreateInstance<ParrotModule>();
        var messageService = _mocker.GetMock<IMessageService>();
        
        messageService.Setup(x => x.IsSentBySelf(message)).Returns(true);

        await module.OnMessage(new MessageEvent(message));
        
        AssertNoReplyWasSent();
    }

    [Fact]
    public async Task CorrectReplyIsSent_When_RuleMatches()
    {
        var reply = new SendableMessage("bruh");
        _mocker.Use<ParrotRuleProvider>(() => new ParrotRule[] { _ => reply });

        var message = Mock.Of<IMessage>();
        var module = _mocker.CreateInstance<ParrotModule>();
        var messageService = _mocker.GetMock<IMessageService>();
        
        await module.OnMessage(new MessageEvent(message));
        
        messageService.Verify(x => x.ReplyTo(message, reply), Times.Once);
    }

    [Fact]
    public async Task NoReplyIsSent_When_NoRuleMatches()
    {
        _mocker.Use<ParrotRuleProvider>(() => new ParrotRule[] { _ => null });
        
        var message = Mock.Of<IMessage>();
        var module = _mocker.CreateInstance<ParrotModule>();

        await module.OnMessage(new MessageEvent(message));
        
        AssertNoReplyWasSent();
    }

    private void AssertNoReplyWasSent()
    {
        var messageService = _mocker.GetMock<IMessageService>();
        
        messageService.Verify(x => 
            x.ReplyTo(It.IsAny<IMessage>(), It.IsAny<SendableMessage>()
        ), Times.Never); 
    }
}