using System.ComponentModel;

namespace GregBot.Domain.Modules.Wordle;

public enum Feedback
{
    Correct, PartiallyCorrect, Incorrect
}

public static class FeedbackExtensions
{
    public static string ToString(this Feedback feedback) => feedback switch
    {
        Feedback.Correct => "🟩",
        Feedback.PartiallyCorrect => "🟨",
        Feedback.Incorrect => "🟥",
        _ => throw new InvalidEnumArgumentException("Invalid Feedback value")
    };
}