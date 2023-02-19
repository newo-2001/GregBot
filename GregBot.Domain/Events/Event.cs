namespace GregBot.Domain.Events;

public record Event
{
    public bool IsCancelled { get; private set; } = false;
    public void Cancel() => IsCancelled = true;
}