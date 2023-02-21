using System;
using GregBot.Domain.Configuration;
using GregBot.Modules.Parrot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using GregBot.Domain;
using GregBot.Domain.Builders;
using GregBot.Domain.Interfaces;
using GregBot.Domain.Modules.Parrot;
using GregBot.Domain.Modules.Wordle;
using GregBot.Domain.Modules.Wordle.Repositories;
using GregBot.Domain.Modules.Wordle.Services;
using GregBot.Domain.Services;
using GregBot.Modules.Wordle;
using GregBot.Modules.Wordle.Repositories;
using GregBot.Modules.Wordle.Services;
using GregBot.Services;

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
    .AddTransient<GregBotBuilder>()
    .AddTransient<IGregBot, GregBot.Domain.GregBot>()
    
    .AddSingleton<ParrotRuleProvider>(() => Rules.All)
    .AddTransient<ParrotModule>()
    
    .Configure<WordleConfiguration>(config.GetSection(WordleConfiguration.Location))
    .AddSingleton<WordleSolutionProvider>(_ => "kleand")
    .AddTransient<IGuessService, GuessService>()
    .AddSingleton<IGuessRepository, InMemoryGuessRepository>()
    .AddTransient<WordleModule>()
    
    .BuildServiceProvider();

var bot = services.GetRequiredService<GregBotBuilder>()
    .LoadModule(services.GetService<ParrotModule>())
    .LoadModule(services.GetService<WordleModule>())
    .Build();

await bot.Login();
await Task.Delay(-1);