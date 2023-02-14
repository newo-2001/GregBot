﻿using Discord;
using Discord.WebSocket;
using GregBot.Domain.Configuration;
using GregBot.Domain.Interfaces;
using GregBot.Domain.Models;
using GregBot.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace GregBot;

public class GregBot
{
    private readonly DiscordSocketClient _client = new DiscordSocketClient(
        new DiscordSocketConfig()
        {
            GatewayIntents =
                GatewayIntents.AllUnprivileged |
                GatewayIntents.MessageContent |
                GatewayIntents.GuildMessages
        });

    private readonly DiscordConfiguration _discordConfig;
    private readonly ILogger<GregBot> _logger;

    private readonly EventDispatcher<MessageEvent> _messageDispatcher = new();

    public IEventTopic<MessageEvent> OnMessage => _messageDispatcher;

    public GregBot(IOptions<DiscordConfiguration> discordConfig, ILogger<GregBot> logger)
    {
        _discordConfig = discordConfig.Value;
        _logger = logger;

        _client.Connected += OnReady;
        _client.Disconnected += OnDisconnect;

        _client.MessageReceived += DispatchMessageEvent;
    }

    public async Task Login()
    {
        await _client.LoginAsync(TokenType.Bot, _discordConfig.Token);
        await _client.StartAsync();
    }

    public Task Logout() => _client.LogoutAsync();

    private Task DispatchMessageEvent(SocketMessage message)
    {
        var author = message.Author.Username;
        _logger.LogDebug("Received message: [{user}] {message}", author, message.Content);
        
        var @event = new MessageEvent(message);
        return _messageDispatcher.Dispatch(@event);
    }

    private Task OnReady()
    {
        var self = _client.CurrentUser;
        _logger.LogInformation("Logged into Discord as {name}.", self.Username);
        return Task.CompletedTask;
    }

    private Task OnDisconnect(Exception ex)
    {
        _logger.LogInformation("Disconnected from Discord.");
        return Task.CompletedTask;
    }
}
