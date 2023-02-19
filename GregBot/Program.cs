using GregBot.Domain.Configuration;
using GregBot.Modules.Parrot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using GregBot.Domain;
using GregBot.Domain.Builders;
using GregBot.Domain.Interfaces;
using GregBot.Services;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection()
    .Configure<DiscordConfiguration>(config.GetSection(DiscordConfiguration.Location))
    .AddLogging(options => options
        .AddConfiguration(config.GetSection("Logging"))
        .AddConsole())
    .AddSingleton<IMessageService, MessageService>()
    .AddSingleton<ParrotRuleProvider>(() => Rules.All)
    .AddSingleton<ParrotModule>()
    .AddTransient<GregBotBuilder>()
    .AddTransient<IGregBot, GregBot.Domain.GregBot>()
    .BuildServiceProvider();

var bot = services.GetRequiredService<GregBotBuilder>()
    .LoadModule(services.GetService<ParrotModule>())
    .Build();

await bot.Login();
await Task.Delay(-1);