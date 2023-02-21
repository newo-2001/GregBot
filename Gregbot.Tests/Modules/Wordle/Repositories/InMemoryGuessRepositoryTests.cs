using Discord;
using FakeItEasy;
using FluentAssertions;
using GregBot.Domain.Modules.Wordle;
using GregBot.Domain.Modules.Wordle.Repositories;
using GregBot.Modules.Wordle.Repositories;
using Xunit;

using static Gregbot.Domain.Testing.Modules.Wordle.Data.Guesses;

namespace Gregbot.Tests.Modules.Wordle.Repositories;

public class InMemoryGuessRepositoryTests
{
    private readonly IGuessRepository _guesses = new InMemoryGuessRepository();

    [Fact]
    public async Task GivenGuessesAfterCallingResetTheGuessesAreEmpty()
    {
        var user = A.Fake<IUser>();
        var guesses = A.CollectionOfDummy<Guess>(3);
        await GivenGuessesForUser(user, guesses);
            
        await _guesses.Reset();

        (await _guesses.GetGuessesForUser(user))
            .Should().BeEmpty();
    }

    [Fact]
    public async Task GivenGuessesWhenRetrievingGuessesTheyAreReturned()
    {
        var user = A.Fake<IUser>();
        var guesses = new[] { Speedwagon, Dio };

        await GivenGuessesForUser(user, guesses);
        
        (await _guesses.GetGuessesForUser(user))
            .Should().Equal(guesses);
    }

    [Fact]
    public async Task WithoutAddingGuessesRetrievingGuessReturnsNoGuesses()
    {
        var user = A.Fake<IUser>();

        (await _guesses.GetGuessesForUser(user))
            .Should().BeEmpty();
    }

    [Fact]
    public async Task GivenGuessesForAUserRetrievingGuessesForAnotherUserReturnsNoGuesses()
    {
        var self = A.Fake<IUser>();
        var other = A.Fake<IUser>();
        
        A.CallTo(() => self.Id).Returns(1ul);
        A.CallTo(() => other.Id).Returns(2ul);

        await GivenGuessesForUser(self, new[] { Dio });

        (await _guesses.GetGuessesForUser(other))
            .Should().BeEmpty();
    }

    private async Task GivenGuessesForUser(IUser user, IEnumerable<Guess> guesses)
    {
        foreach (var guess in guesses)
        {
            await _guesses.AddGuess(user, guess);
        }
    }
}