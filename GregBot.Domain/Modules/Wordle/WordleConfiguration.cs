using System;

namespace GregBot.Domain.Modules.Wordle;

public class WordleConfiguration
{
    public static string Location => "Wordle";
    public required ulong ChannelId { get; init; }
    public int GuessesPerDay { get; init; } = 6;
    public string Name { get; init; } = "Wordle";
    public DateTime Epoch { get; init; } = DateTime.UtcNow.Date;
}