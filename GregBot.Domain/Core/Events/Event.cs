using System;

namespace GregBot.Domain.Core.Events;

public abstract record Event
{
    public abstract bool IsCancellable { get; }
    public bool IsCancelled { get; private set; }

    public void Cancel()
    {
        if (!IsCancellable)
        {
            throw new InvalidOperationException("This event is not cancellable");
        }

        IsCancelled = true;
    }
}