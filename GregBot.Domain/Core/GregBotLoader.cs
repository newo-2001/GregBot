using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GregBot.Domain.Core.Attributes;
using GregBot.Domain.Core.Events;
using GregBot.Domain.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GregBot.Domain.Core;

using GregBot = Models.GregBot;

public class GregBotLoader
{
    private readonly ILogger<GregBotLoader> _logger;
    private readonly IServiceProvider _services;
    private readonly GregBot _bot;
    
    public GregBotLoader(IServiceProvider services)
    {
        _services = services;
        _logger = services.GetRequiredService<ILogger<GregBotLoader>>();
        _bot = services.GetRequiredService<GregBot>();
        _logger = services.GetRequiredService<ILogger<GregBotLoader>>();
    }

    public GregBotLoader WithModule<T>() where T : IModule
    {
        var module = _services.GetRequiredService<T>();
        _logger.LogInformation("Loading '{Name}' module", module.Name);

        var listeners = typeof(T).GetMethods()
            .Where(x => x.IsEventListener())
            .ToList();

        if (!listeners.Any())
        {
            _logger.LogWarning("Module '{Name}' did not register any event listeners", module.Name);
        }
        
        foreach (var listener in listeners)
        {
            RegisterListener(module, listener);
        }
        
        return this;
    }

    private void RegisterListener<T>(T module, MethodInfo listener) where T : IModule
    {
        if (listener.ReturnType != typeof(Task))
        {
            IgnoreWarning(listener, "event listeners must have the 'Task' return type");
            return;
        }
       
        var parameters = listener.GetParameters();
        if (parameters.Length != 1)
        {
            IgnoreWarning(listener, "event listeners must have exactly one argument");
            return;
        }
        
        var eventType = parameters.Single().ParameterType;
        if (!eventType.IsAssignableTo(typeof(Event)))
        {
            IgnoreWarning(listener, "method argument is not an event");
            return;
        }

        var handlerType = typeof(EventListener<>).MakeGenericType(eventType);
        var handler = listener.CreateDelegate(handlerType, module);
        
        typeof(EventBus)
            .GetMethod("Subscribe")!
            .MakeGenericMethod(eventType)
            .Invoke(_bot.EventBus, new object[] { handler });
        
        _logger.LogInformation("Registered '{Name}' event listener for '{Module}' module", listener.Name, module.Name);
    }

    private void IgnoreWarning(MemberInfo listener, string reason)
    {
        _logger.LogWarning("Ignoring Event listener '{Name}' in class '{Class}', {Reason}", listener.Name, listener.Module, reason);
    }

    public async Task Start()
    {
        await _bot.Login();
        await Task.Delay(-1);
    }
}

public static class GregBotServiceProviderExtensions
{
    public static GregBotLoader ConfigureGregbot(this IServiceProvider services) => new(services);
}
