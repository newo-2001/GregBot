using Discord;
using FluentAssertions;
using GregBot.Domain.Modules.Wordle;
using GregBot.Domain.Modules.Wordle.Repositories;
using GregBot.Modules.Wordle.Repositories;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace GregBot.Tests.Unit.Modules.Wordle.Repositories;

public class InMemoryGuessRepositoryTests
{
    private readonly AutoMocker _mocker = new();
    private IGuessRepository _guessRepository = null!;
    
    [Fact]
    public async Task UserHasNoGuessesRemaining_After_ClearingTheRepository()
    {
        const int numberOfGuesses = 2;
        
        _guessRepository = _mocker.CreateInstance<InMemoryGuessRepository>();
        var user = await GivenUserWithGuesses(numberOfGuesses);

        (await _guessRepository.GetGuessesForUser(user))
            .Should().HaveCount(numberOfGuesses);

        await _guessRepository.Clear();
        
        (await _guessRepository.GetGuessesForUser(user))
            .Should().BeEmpty();
    }

    [Fact]
    public async Task RetrievingGuessesForUserWithGuesses_Returns_TheirGuesses()
    {
        _guessRepository = _mocker.CreateInstance<InMemoryGuessRepository>();

        var guesses = new[] { "jotaro", "kujo" }
            .Select(GivenGuessWithoutFeedback).ToList();

        var user = await GivenUserWithGuesses(guesses);

        (await _guessRepository.GetGuessesForUser(user))
            .Should().Equal(guesses);
    }

    [Fact]
    public async Task UsersInitiallyHave_NoGuesses()
    {
        var user = Mock.Of<IUser>();
        _guessRepository = _mocker.CreateInstance<InMemoryGuessRepository>();

        (await _guessRepository.GetGuessesForUser(user))
            .Should().BeEmpty();
    }

    [Fact]
    public async Task GuessesForDifferentUsers_Are_Independent()
    {
        _guessRepository = _mocker.CreateInstance<InMemoryGuessRepository>();
        
        var aliceGuesses = new[] { "josuke", "higashikata" }
            .Select(GivenGuessWithoutFeedback).ToList();
        var alice = await GivenUserWithGuesses(aliceGuesses);
        
        var bobGuesses = new List<Guess> { GivenGuessWithoutFeedback("speedwagon") };
        var bob = await GivenUserWithGuesses(bobGuesses);

        (await _guessRepository.GetGuessesForUser(alice))
            .Should().Equal(aliceGuesses);

        (await _guessRepository.GetGuessesForUser(bob))
            .Should().Equal(bobGuesses);
    }

    private static Guess GivenGuessWithoutFeedback(string guess) =>
        new(guess, Enumerable.Empty<Feedback>());

    private Task<IUser> GivenUserWithGuesses(int numberOfGuesses)
    {
        var guesses = Enumerable.Range(0, numberOfGuesses)
            .Select(x => GivenGuessWithoutFeedback(x.ToString()));

        return GivenUserWithGuesses(guesses);
    }

    private ulong _userCount;
    private async Task<IUser> GivenUserWithGuesses(IEnumerable<Guess> guesses)
    {
        var user = new Mock<IUser>();

        user.Setup(x => x.Id).Returns(_userCount++);

        foreach (var guess in guesses)
        {
            await _guessRepository.AddGuess(user.Object, guess);
        }
        
        return user.Object; 
    }
}