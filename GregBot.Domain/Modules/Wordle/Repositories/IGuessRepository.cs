using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;

namespace GregBot.Domain.Modules.Wordle.Repositories;

public interface IGuessRepository
{
    Task<IEnumerable<Guess>> GetGuessesForUser(IUser user);
    Task AddGuess(IUser user, Guess guess);
    Task Clear();
}