using Discord;
using FluentAssertions;
using GregBot.Domain.Modules.Wordle;
using GregBot.Domain.Modules.Wordle.Repositories;
using GregBot.Domain.Modules.Wordle.Services;
using GregBot.Modules.Wordle.Services;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using Xunit;
using Times = Moq.Times;

using static GregBot.Domain.Modules.Wordle.Feedback;

namespace GregBot.Tests.Unit.Modules.Wordle.Services;

public class GuessServiceTests
{
    private readonly AutoMocker _mocker = new();
    
    [Theory]
    [InlineData("banana")]
    [InlineData("dio")]
    public void GuessIsInvalid_When_ItDoesNotHaveTheSameLengthAsTheAnswer(string guess)
    {
        _mocker.Use<WordleSolutionProvider>(_ => "jojo");
        
        var message = ConfigureMessageInTheWordleChannel();
        message.Setup(x => x.Content).Returns(guess);
        
        var guessService = _mocker.CreateInstance<GuessService>();
        
        guessService.IsValidGuess(message.Object).Should().BeFalse();
    }

    [Fact]
    public void GuessIsInvalid_When_ItIsNotSentInTheWordleChannel()
    {
        const int wordleChannelId = 5;
        var wordleConfig = new WordleConfiguration { ChannelId = wordleChannelId };
        
        _mocker.Use<WordleSolutionProvider>(_ => "jojo");
        _mocker.Use(Options.Create(wordleConfig));
        
        var guessService = _mocker.CreateInstance<GuessService>();
        var message = new Mock<IMessage>();
        var channel = new Mock<IMessageChannel>();
        
        channel.Setup(x => x.Id).Returns(wordleChannelId + 1);
        message.Setup(x => x.Channel).Returns(channel.Object);
        message.Setup(x => x.Content).Returns("test");
        
        guessService.IsValidGuess(message.Object).Should().BeFalse();
    }

    [Fact]
    public void GuessIsValid_When_ItHasTheSameLengthAsTheAnswerAndIsSentInTheWordleChannel()
    {
        _mocker.Use<WordleSolutionProvider>(_ => "jojo");

        var message = ConfigureMessageInTheWordleChannel();
        var guessService = _mocker.CreateInstance<GuessService>();
        
        message.Setup(x => x.Content).Returns("test");
        
        guessService.IsValidGuess(message.Object).Should().BeTrue();
    }

    [Fact]
    public async Task UserHasGuessesRemaining_When_TheyHaveFewerGuessesThanTheDailyLimit()
    {
        ConfigureGuessesPerDay(2);
        
        var guessService = _mocker.CreateInstance<GuessService>();
        var user = GivenUserWithGuesses(1);
        
        (await guessService.UserHasGuessesRemaining(user))
            .Should().BeTrue();
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    public async Task UserHasNoGuessesRemaining_When_TheyDontHaveFewerThanTheDailyLimit(int amount)
    {
        ConfigureGuessesPerDay(2);
        
        var guessService = _mocker.CreateInstance<GuessService>();
        var user = GivenUserWithGuesses(amount);
            
        (await guessService.UserHasGuessesRemaining(user))
            .Should().BeFalse();
    }
    
    [Fact]
    public async Task TheRepositoryIsCleared_When_GuessesAreReset()
    {
        var guessService = _mocker.CreateInstance<GuessService>();
        var guessRepository = _mocker.GetMock<IGuessRepository>();
        
        await guessService.ResetGuesses();
        
        guessRepository.Verify(x => x.Clear(), Times.Once);
    }

    [Fact]
    public async Task GuessesUsedByUser_Returns_TheNumberOfGuessesInTheRepositoryForThatUser()
    {
        var guessService = _mocker.CreateInstance<GuessService>();
        var user = GivenUserWithGuesses(3);

        (await guessService.GuessesUsedByUser(user)).Should().Be(3);
    }

    [Fact]
    public async Task TheGuessIsAddedToTheRepository_After_Guessing()
    {
        const string guess = "Iketani senpai";
        
        _mocker.Use<WordleSolutionProvider>(_ => "Fujiwara Bunta");
        
        var guessService = _mocker.CreateInstance<GuessService>();
        var guessRepository = _mocker.GetMock<IGuessRepository>();
        var user = Mock.Of<IUser>();

        await guessService.Guess(user, guess);
        
        guessRepository.Verify(x =>
            x.AddGuess(user, It.Is<Guess>(x => x.Text == guess)),
            Times.Once
        );
    }

    [Theory]
    [InlineData("banana", new[] { Incorrect, Incorrect, PartiallyCorrect, Incorrect, Correct, Incorrect })]
    [InlineData("giorno", new[] { Correct, Correct, Correct, Correct, Correct, Correct })]
    public async Task Guessing_Returns_TheCorrectFeedback(string guess, IEnumerable<Feedback> feedback)
    {
        _mocker.Use<WordleSolutionProvider>(_ => "giorno");
        
        var expected = new Guess(guess, feedback);
        
        var guessService = _mocker.CreateInstance<GuessService>();
        var user = Mock.Of<IUser>();
        
        (await guessService.Guess(user, guess))
            .Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Guessing_IsNot_CaseSensative()
    {
        const string guess = "Stupei";
        var expected = new Guess(guess, Enumerable.Repeat(Correct, guess.Length));
        
        _mocker.Use<WordleSolutionProvider>(_ => "stupei");
        
        var guessService = _mocker.CreateInstance<GuessService>();
        var user = Mock.Of<IUser>();

        (await guessService.Guess(user, guess))
            .Should().BeEquivalentTo(expected);
    }

    private Mock<IMessage> ConfigureMessageInTheWordleChannel()
    {
        const int wordleChannelId = 5;

        var wordleConfig = new WordleConfiguration { ChannelId = wordleChannelId };
        var channel = new Mock<IMessageChannel>();
        var message = new Mock<IMessage>();

        _mocker.Use(Options.Create(wordleConfig));
        channel.Setup(x => x.Id).Returns(wordleChannelId);
        message.Setup(x => x.Channel).Returns(channel.Object);

        return message;
    }

    private IUser GivenUserWithGuesses(int numberOfGuesses)
    {
        var guesses = Enumerable.Range(0, numberOfGuesses).Select(x =>
            new Guess(x.ToString(), Enumerable.Empty<Feedback>())
        );

        var user = Mock.Of<IUser>();
        _mocker.GetMock<IGuessRepository>()
            .Setup(x => x.GetGuessesForUser(user))
            .Returns(Task.FromResult(guesses));

        return user;
    }

    private void ConfigureGuessesPerDay(int amount)
    {
        var wordleConfig = new WordleConfiguration
        {
            GuessesPerDay = amount,
            ChannelId = 0
        };
            
        _mocker.Use(Options.Create(wordleConfig));
    }
}