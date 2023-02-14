namespace GregBot.Domain.Configuration;
public class DiscordConfiguration
{
    public static string Location => "Discord";

    public required string ClientId { get; init; }
    public required string Token { get; init; }
    public required string GuildId { get; init; }
}
