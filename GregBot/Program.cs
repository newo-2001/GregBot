using GregBot.Builders;
using GregBot.Domain.Configuration;
using GregBot.Modules.Parrot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System.Threading.Tasks;
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
    .AddSingleton<ParrotModule>()
    .AddTransient<GregBotBuilder>()
    .AddTransient<GregBot.Domain.GregBot>()
    .BuildServiceProvider();

var bot = services.GetRequiredService<GregBotBuilder>()
    .LoadModule(services.GetService<ParrotModule>())
    .Build();

await bot.Login();
await Task.Delay(-1);
