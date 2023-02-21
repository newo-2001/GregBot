using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;

namespace GregBot.Domain.Modules.Wordle.Services;

public interface IGuessService
{
    int GuessesPerDay { get; }
    Task<int> GuessesUsedByUser(IUser user);
    Task<bool> UserHasGuessesRemaining(IUser user);
    Task<IEnumerable<Guess>> FeedbackForUser(IUser user);
    Task<Guess> Guess(IUser user, string guess);
    bool IsValidGuess(string guess);
    Task ResetGuesses();
}