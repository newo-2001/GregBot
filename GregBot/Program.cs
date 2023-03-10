using System;
using GregBot.Modules.Parrot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using GregBot.Core.Services;
using GregBot.Domain.Core;
using GregBot.Domain.Core.Configuration;
using GregBot.Domain.Core.Interfaces;
using GregBot.Domain.Core.Services;
using GregBot.Domain.Modules.Parrot;
using GregBot.Domain.Modules.Wordle;
using GregBot.Domain.Modules.Wordle.Repositories;
using GregBot.Domain.Modules.Wordle.Services;
using GregBot.Modules.Wordle;
using GregBot.Modules.Wordle.Repositories;
using GregBot.Modules.Wordle.Services;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection()
    .AddLogging(options => options
        .AddConfiguration(config.GetSection("Logging"))
        .AddConsole())
    
    .Configure<DiscordConfiguration>(config.GetSection(DiscordConfiguration.Location))
    .AddTransient<IMessageService, MessageService>()
    .AddSingleton<TimeProvider>(() => DateTime.UtcNow)
    .AddSingleton<GregBot.Domain.Core.Models.GregBot>()
    
    .AddSingleton<ParrotRuleProvider>(() => Rules.All)
    .AddTransient<ParrotModule>()
    
    .Configure<WordleConfiguration>(config.GetSection(WordleConfiguration.Location))
    .AddSingleton<WordleSolutionProvider>(_ => "kleand")
    .AddTransient<IGuessService, GuessService>()
    .AddSingleton<IGuessRepository, InMemoryGuessRepository>()
    .AddTransient<WordleModule>()
    
    .BuildServiceProvider();

await services.ConfigureGregbot()
    .WithModule<ParrotModule>()
    .WithModule<WordleModule>()
    .Start();