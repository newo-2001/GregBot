using GregBot.Builders;
using GregBot.Domain.Configuration;
using GregBot.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection()
    .Configure<DiscordConfiguration>(config.GetSection(DiscordConfiguration.Location))
    .AddLogging(options => options
        .AddConfiguration(config.GetSection("Logging"))
        .AddConsole())
    .AddScoped<ParrotModule>()
    .AddScoped<GregBotBuilder>()
    .AddScoped<GregBot.GregBot>()
    .BuildServiceProvider();

var bot = services.GetRequiredService<GregBotBuilder>()
    .LoadModule(services.GetService<ParrotModule>())
    .Build();

await bot.Login();
await Task.Delay(-1);
