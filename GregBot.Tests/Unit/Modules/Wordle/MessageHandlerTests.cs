using Discord;
using GregBot.Domain.Core.Events;
using GregBot.Domain.Core.Models;
using GregBot.Domain.Core.Services;
using GregBot.Domain.Modules.Wordle.Services;
using GregBot.Modules.Wordle;
using Moq;
using Moq.AutoMock;
using Xunit;
using Times = Moq.Times;

namespace GregBot.Tests.Unit.Modules.Wordle;

public class MessageHandlerTests
{
    private readonly AutoMocker _mocker = new();
    
    [Fact]
    public async Task WhenUserHasNoGuessesRemaining_AnAppropriateResponseIsSent()
    {
        const string response = "You don't have any guesses remaining today.";

        var message = Mock.Of<IMessage>();
        var module = _mocker.CreateInstance<WordleModule>();
        
        var guessService = _mocker.GetMock<IGuessService>();
        var messageService = _mocker.GetMock<IMessageService>();
        
        guessService.Setup(x => x.IsValidGuess(message)).Returns(true);
        guessService.Setup(x => x.UserHasGuessesRemaining(It.IsAny<IUser>()))
            .Returns(Task.FromResult(false));

        await module.OnMessage(new MessageEvent(message));
        
        messageService.Verify(x =>
            x.ReplyTo(message, It.Is<SendableMessage>(x => x.Content == response)),
            Times.Once
        );
    }

    [Fact]
    public async Task AnInvalidGuess_IsIngored()
    {
        var message = Mock.Of<IMessage>();
        var module = _mocker.CreateInstance<WordleModule>();
        
        var messageService = _mocker.GetMock<IMessageService>();
        var guessService = _mocker.GetMock<IGuessService>();

        guessService.Setup(x => x.IsValidGuess(message)).Returns(false);

        await module.OnMessage(new MessageEvent(message));
        
        messageService.VerifyNoOtherCalls();
    }
}