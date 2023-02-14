using System;
using System.Reflection;

namespace GregBot.Domain.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class Priority : Attribute
{
    public const int High = 100;
    public const int Medium = 0;
    public const int Low = -100;

    public int Value { get; }

    public Priority(int priority)
    {
        Value = priority;
    }
}

public static class PriorityExtensions
{
    public static int GetPriority(this MemberInfo info) =>
        info.GetCustomAttribute<Priority>()?.Value ?? Priority.Medium;
}
