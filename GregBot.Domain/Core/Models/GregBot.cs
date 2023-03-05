using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GregBot.Domain.Core.Configuration;
using GregBot.Domain.Core.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GregBot.Domain.Core.Models;

public class GregBot
{
    private readonly DiscordSocketClient _client = new(
        new DiscordSocketConfig()
        {
            GatewayIntents =
                GatewayIntents.AllUnprivileged |
                GatewayIntents.MessageContent |
                GatewayIntents.GuildMessages
        });

    private readonly DiscordConfiguration _discordConfig;
    private readonly ILogger<GregBot> _logger;

    public EventBus EventBus { get; } = new();
    
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
        _logger.LogDebug("Received message: [{User}] {Message}", author, message.Content);
        
        return EventBus.Post(new MessageEvent(message));
    }

    private Task OnReady()
    {
        var self = _client.CurrentUser;
        _logger.LogInformation("Logged into Discord as {Name}", self.Username);
        return Task.CompletedTask;
    }

    private Task OnDisconnect(Exception ex)
    {
        _logger.LogInformation("Disconnected from Discord");
        return Task.CompletedTask;
    }
}
