namespace GregBot.Domain.Core.Configuration;
public class DiscordConfiguration
{
    public static string Location => "Discord";

    public required ulong ClientId { get; init; }
    public required string Token { get; init; }
    public required ulong GuildId { get; init; }
}
