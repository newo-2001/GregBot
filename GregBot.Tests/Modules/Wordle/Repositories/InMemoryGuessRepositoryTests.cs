using Discord;
using FakeItEasy;
using FluentAssertions;
using GregBot.Domain.Extensions;
using GregBot.Domain.Modules.Wordle;
using GregBot.Domain.Modules.Wordle.Repositories;
using GregBot.Modules.Wordle.Repositories;
using Xunit;

namespace GregBot.Tests.Modules.Wordle.Repositories;

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
        var guesses = UniqueGuessesWithoutFeedback(3).ToList();

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
        var guesses = A.CollectionOfDummy<Guess>(1);
        var users = UniqueUsers(2).ToList();
        
        await GivenGuessesForUser(users.First(), guesses);

        (await _guesses.GetGuessesForUser(users.Last()))
            .Should().BeEmpty();
    }

    private static IEnumerable<Guess> UniqueGuessesWithoutFeedback(int amount) =>
        Enumerable.Range(0, amount).Select(i => new Guess(i.ToString(), Array.Empty<Feedback>()));

    private static IEnumerable<IUser> UniqueUsers(int amount)
    {
        var users = A.CollectionOfFake<IUser>(amount);

        foreach (var (user, i) in users.Indexed())
        {
            A.CallTo(() => user.Id).Returns((ulong) i);
        }

        return users;
    }

    private async Task GivenGuessesForUser(IUser user, IEnumerable<Guess> guesses)
    {
        foreach (var guess in guesses)
        {
            await _guesses.AddGuess(user, guess);
        }
    }
}