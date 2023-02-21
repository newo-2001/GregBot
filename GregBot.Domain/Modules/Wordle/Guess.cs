using System.Collections.Generic;

namespace GregBot.Domain.Modules.Wordle;

public record Guess(string Text, IEnumerable<Feedback> Feedback)
{
    public override string ToString() => string.Join(string.Empty, Feedback);
}