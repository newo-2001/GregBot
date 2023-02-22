using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using GregBot.Domain.Interfaces;
using GregBot.Domain.Modules.Wordle;
using GregBot.Domain.Modules.Wordle.Repositories;
using GregBot.Domain.Modules.Wordle.Services;
using Microsoft.Extensions.Options;

namespace GregBot.Modules.Wordle.Services;

public class GuessService : IGuessService
{
    private const StringComparison COMPARISON = StringComparison.InvariantCultureIgnoreCase;
    
    private readonly WordleSolutionProvider _solutionProvider;
    private readonly TimeProvider _timeProvider;
    private readonly WordleConfiguration _wordleConfig;
    private readonly IGuessRepository _guessRepository;

    public GuessService(IOptions<WordleConfiguration> wordleConfig,
        IGuessRepository guessRepository,
        TimeProvider timeProvider,
        WordleSolutionProvider solutionProvider)
    {
        _wordleConfig = wordleConfig.Value;
        _timeProvider = timeProvider;
        _solutionProvider = solutionProvider;
        _guessRepository = guessRepository;
    }

    public int GuessesPerDay => _wordleConfig.GuessesPerDay;

    public async Task<bool> UserHasGuessesRemaining(IUser user) =>
        await GuessesUsedByUser(user) < GuessesPerDay;

    public async Task<int> GuessesUsedByUser(IUser user) => (await FeedbackForUser(user)).Count();

    public Task ResetGuesses() => _guessRepository.Reset();

    public Task<IEnumerable<Guess>> FeedbackForUser(IUser user) => _guessRepository.GetGuessesForUser(user);

    // WARNING: This check results in a race condition
    public bool IsValidGuess(string guess) => guess.Length == Solution.Length;

    public async Task<Guess> Guess(IUser user, string guess)
    {
        var answer = Solution;
        
        var feedback = answer
            .Zip(guess.ToLowerInvariant())
            .Select(pair => pair switch
            {
                var (correct, guessed) when correct.ToString().Equals(guessed.ToString(), COMPARISON) => Feedback.Correct,
                var (_, guessed) when answer.Contains(guessed, COMPARISON) => Feedback.PartiallyCorrect,
                _ => Feedback.Incorrect
            });

        var result = new Guess(guess, feedback);
        await _guessRepository.AddGuess(user, result);
        
        return result;
    }

    private DateTime Now => _timeProvider();
    private string Solution => _solutionProvider(Now);
}