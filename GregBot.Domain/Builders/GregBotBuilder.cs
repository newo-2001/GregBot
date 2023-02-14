using GregBot.Domain.Attributes;
using GregBot.Domain.Configuration;
using GregBot.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace GregBot.Builders;
public class GregBotBuilder
{
    private readonly ILogger<GregBotBuilder> _logger;
    private readonly IDictionary<int, IModule> _modules = new SortedList<int, IModule>();
    private readonly GregBot _bot;

    public GregBotBuilder(GregBot bot, ILogger<GregBotBuilder> logger)
    {
        _bot = bot;
        _logger = logger;
    }

    public GregBotBuilder LoadModule(IModule? module)
    {
        if (module is not null)
        {
            var priority = module.GetType().GetPriority();
            _modules.Add(priority, module);
        }

        return this;
    }
    
    public GregBot Build()
    {
        foreach (var (_, module) in _modules)
        {
            _logger.LogInformation("Loading module {name}...", module.Name);
            module.Activate(_bot);
        }

        return _bot;
    }
}
