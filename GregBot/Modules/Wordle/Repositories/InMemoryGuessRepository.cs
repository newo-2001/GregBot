using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Discord;
using GregBot.Domain.Modules.Wordle;
using GregBot.Domain.Modules.Wordle.Repositories;

namespace GregBot.Modules.Wordle.Repositories;

public class InMemoryGuessRepository : IGuessRepository
{
    private readonly IDictionary<ulong, List<Guess>> _guesses = new Dictionary<ulong, List<Guess>>();

    public Task<IEnumerable<Guess>> GetGuessesForUser(IUser user) =>
        Task.FromResult<IEnumerable<Guess>>(
            _guesses.TryGetValue(user.Id, out var guesses) ? guesses : Array.Empty<Guess>()
        );
    
    public Task AddGuess(IUser user, Guess guess)
    {
        if (_guesses.TryGetValue(user.Id, out var guesses))
        {
            guesses.Add(guess);
        }
        else
        {
            _guesses.Add(user.Id, new List<Guess> { guess });
        }
        
        return Task.CompletedTask;
    }

    public Task Clear()
    {
        _guesses.Clear();
        return Task.CompletedTask;
    }
}