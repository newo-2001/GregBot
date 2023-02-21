using GregBot.Domain.Modules.Wordle;

namespace Gregbot.Domain.Testing.Modules.Wordle.Data;

public static class Guesses
{
    public static readonly Guess Speedwagon = new("Robert E. O. Speedwagon", new[]
    {
        Feedback.Correct, Feedback.Incorrect, Feedback.PartiallyCorrect
    });

    public static readonly Guess Dio = new("Dio Brando", Array.Empty<Feedback>());
}