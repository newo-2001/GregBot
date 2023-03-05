using System;
using System.Threading.Tasks;
using Discord;
using GregBot.Domain.Core.Attributes;
using GregBot.Domain.Core.Events;
using GregBot.Domain.Core.Interfaces;
using GregBot.Domain.Core.Models;
using GregBot.Domain.Core.Services;
using GregBot.Domain.Modules.Wordle;
using GregBot.Domain.Modules.Wordle.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GregBot.Modules.Wordle;

public class WordleModule : IModule
{
    private readonly TimeProvider _timeProvider;
    private readonly WordleConfiguration _wordleConfig;
    private readonly IGuessService _guessService;
    private readonly IMessageService _messageService;
    private readonly ILogger<WordleModule> _logger;

    public WordleModule(IOptions<WordleConfiguration> wordleConfig,
        ILogger<WordleModule> logger,
        IGuessService guessService,
        IMessageService messageService,
        TimeProvider timeProvider)
    {
        _wordleConfig = wordleConfig.Value;
        _messageService = messageService;
        _guessService = guessService;
        _timeProvider = timeProvider;
        _logger = logger;
    }
    
    public string Name => "Wordle";

    private int DaysSinceEpoch => (Now.Date - _wordleConfig.Epoch).Days + 1;
    
    [SubscribeEvent(EventPriority.HIGH)]
    public async Task OnMessage(MessageEvent @event)
    {
        var message = @event.Message;
        var content = message.Content;
        
        if (!_guessService.IsValidGuess(@event.Message)) return;

        var user = message.Author;
        if (!await _guessService.UserHasGuessesRemaining(user))
        {
            await _messageService.ReplyTo(message, new SendableMessage("You don't have any guesses remaining today."));
            return;
        }

        await Guess(message);
    }

    private async Task Guess(IMessage message)
    {
        var content = message.Content;
        var user = message.Author;
        
        var guess = await _guessService.Guess(user, content);
        var usedGuesses = await _guessService.GuessesUsedByUser(user);
        var maxGuesses = _guessService.GuessesPerDay;

        if (await _guessService.UserHasGuessesRemaining(user))
        {
            await _messageService.ReplyTo(message, new SendableMessage($"{usedGuesses}/{maxGuesses} {guess}"));
        }
        else
        {
            var summary = string.Join('\n', await _guessService.FeedbackForUser(user));
            var reply = new SendableMessage($"{_wordleConfig.Name} {DaysSinceEpoch}\t{usedGuesses}/{maxGuesses}\n{summary}");
            await _messageService.ReplyTo(message, reply);
        }
    }

    private DateTime Now => _timeProvider();
}