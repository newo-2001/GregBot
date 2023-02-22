using Discord;
using FakeItEasy;
using FluentAssertions;
using GregBot.Domain.Modules.Wordle;
using GregBot.Domain.Modules.Wordle.Repositories;
using GregBot.Modules.Wordle.Services;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Times = Moq.Times;

using static GregBot.Domain.Modules.Wordle.Feedback;

namespace GregBot.Tests.Modules.Wordle.Services;

public class GuessServiceTests
{
    private readonly Mock<IGuessRepository> _guessRepositoryMock = new();

    [Theory]
    [InlineData("banana")]
    [InlineData("abc")]
    public void IsValidGuessReturnsFalseWhenGuessDoesNotMatchLengthOfTheAnswer(string guess)
    {
        var guessService = new GuessService(AnyConfiguration, _guessRepositoryMock.Object, Chronostasis, _ => "test");

        guessService.IsValidGuess(guess).Should().BeFalse();
    }

    [Fact]
    public void IsValidGuessReturnsTrueWhenGuessHasTheSameLengthAsTheAnswer()
    {
        var guessService = new GuessService(AnyConfiguration, _guessRepositoryMock.Object, Chronostasis, _ => "test");

        guessService.IsValidGuess("jojo").Should().BeTrue();
    }

    [Fact]
    public async Task GivenTwoGuessesAUserWhoGuessedOnceHasGuessesRemaining()
    {
        var guessService = new GuessService(ConfigWithMaxGuesses(2), _guessRepositoryMock.Object, Chronostasis, AnySolution);
        var user = GivenUserWithGuesses(1);
        
        (await guessService.UserHasGuessesRemaining(user))
            .Should().BeTrue();
    }

    [Fact]
    public async Task GivenTwoGuessesAUserWhoGuessedTwiceHasNoGuessesRemaining()
    {
        var guessService = new GuessService(ConfigWithMaxGuesses(2), _guessRepositoryMock.Object, Chronostasis, AnySolution);
        var user = GivenUserWithGuesses(2);
            
        (await guessService.UserHasGuessesRemaining(user))
            .Should().BeFalse();
    }
    
    [Fact]
    public async Task ResetDelegatesWorkToRepository()
    {
        var guessService = new GuessService(AnyConfiguration, _guessRepositoryMock.Object, Chronostasis, AnySolution);

        await guessService.ResetGuesses();
        
        _guessRepositoryMock.Verify(x => x.Reset(), Times.Once);
    }

    [Fact]
    public void GuessPerDayReturnsTheValueProvidedInTheConfig()
    {
        var guessService = new GuessService(ConfigWithMaxGuesses(4), _guessRepositoryMock.Object, Chronostasis, AnySolution);

        guessService.GuessesPerDay.Should().Be(4);
    }

    [Fact]
    public async Task GuessesUsedByUserReturnsTheAmountOfGuessesInTheRepository()
    {
        var guessService = new GuessService(AnyConfiguration, _guessRepositoryMock.Object, Chronostasis, AnySolution);

        var user = GivenUserWithGuesses(3);

        (await guessService.GuessesUsedByUser(user)).Should().Be(3);
    }

    [Fact]
    public async Task AfterGuessingTheGuessIsAddedToTheRepository()
    {
        var guessService = new GuessService(AnyConfiguration, _guessRepositoryMock.Object, Chronostasis, AnySolution);
        var user = A.Fake<IUser>();
        const string guess = "Iketani senpai";

        await guessService.Guess(user, guess);
        
        _guessRepositoryMock.Verify(x =>
            x.AddGuess(user, It.Is<Guess>(x => x.Text == guess)),
            Times.Once
        );
    }

    [Theory]
    [InlineData("banana", new[] { Incorrect, Incorrect, PartiallyCorrect, Incorrect, Correct, Incorrect })]
    [InlineData("giorno", new[] { Correct, Correct, Correct, Correct, Correct, Correct })]
    public async Task GuessingGivesTheCorrectFeedback(string guess, IEnumerable<Feedback> feedback)
    {
        var guessService = new GuessService(AnyConfiguration, _guessRepositoryMock.Object, Chronostasis, _ => "giorno");
        var user = A.Fake<IUser>();
        
        var expected = new Guess(guess, feedback);

        (await guessService.Guess(user, guess))
            .Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GuessingIsNotCaseSensative()
    {
        const string guess = "Stupei";
        var guessService = new GuessService(AnyConfiguration, _guessRepositoryMock.Object, Chronostasis, _ => "stupei");
        var user = A.Fake<IUser>();

        var expected = new Guess(guess, Enumerable.Repeat(Correct, guess.Length));

        (await guessService.Guess(user, guess))
            .Should().BeEquivalentTo(expected);
    }

    private IUser GivenUserWithGuesses(int numberOfGuesses)
    {
        var user = A.Fake<IUser>();
        var guesses = A.CollectionOfDummy<Guess>(numberOfGuesses)
            .As<IEnumerable<Guess>>();

        _guessRepositoryMock.Setup(x => x.GetGuessesForUser(user))
            .Returns(Task.FromResult(guesses));

        return user;
    }
    
    private static string AnySolution(DateTime _) => string.Empty;
    private static DateTime Chronostasis() => new();

    private static IOptions<WordleConfiguration> AnyConfiguration =>
        Options.Create(A.Dummy<WordleConfiguration>());
    
    private static IOptions<WordleConfiguration> ConfigWithMaxGuesses(int maxGuesses) =>
        Options.Create(new WordleConfiguration { GuessesPerDay = maxGuesses, ChannelId = 0ul });
}