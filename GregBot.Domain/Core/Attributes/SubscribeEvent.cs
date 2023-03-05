using System;
using System.Reflection;
using GregBot.Domain.Core.Events;

namespace GregBot.Domain.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class SubscribeEvent : Attribute
{
    public int Priority { get; }

    public SubscribeEvent() : this(EventPriority.NORMAL) { }
    
    public SubscribeEvent(int priority)
    {
        Priority = priority;
    }
}

public static class SubscribeEventMethodInfoExtensions
{
    public static bool IsEventListener(this MethodInfo method) =>
        method.GetCustomAttribute<SubscribeEvent>() != null;

    public static int GetPriority(this MethodInfo method) =>
        method.GetCustomAttribute<SubscribeEvent>()?.Priority
            ?? throw new InvalidOperationException("Method does not have a priority");
}
