using System.Collections.Generic;
using System.Text.RegularExpressions;
using Discord;
using GregBot.Domain.Models;
using GregBot.Domain.Modules.Parrot;
using static GregBot.Domain.Modules.Parrot.Matchers;

namespace GregBot.Modules.Parrot;

public static partial class Rules
{
    private static readonly SendableMessage Narancia28 = new(new FileAttachment("Resources/28.jpg"));
    
    public static readonly ParrotRule Greg = Any(
        new[]
        {
            MatchString("greg"),
            MatchString("грег"),
            MatchWord("gtnh"),
            MatchWord("gt:nh"),
            MatchWords(new[] { "new", "horizons" })
        }
    ).AsLowerCase().Reply(new SendableMessage("greg"));

    public static readonly ParrotRule United = MatchWord("united")
        .AsLowerCase().Reply(new SendableMessage("we kicked a kid"));

    public static readonly ParrotRule Wysi = MatchString("727")
        .AsLowerCase().Reply(new SendableMessage("wysi"));

    public static readonly ParrotRule Wyfsi = MatchString("wysi")
        .AsLowerCase().Reply(new SendableMessage("wyfsi"));

    public static readonly ParrotRule Neat = MatchString("neat")
        .AsLowerCase().Reply(new SendableMessage("neat is a mod by Vazkii"));

    public static readonly ParrotRule Rat = MatchWord("rat")
        .Or(MatchWord("rats")).AsLowerCase()
        .Reply(new SendableMessage("haha funny rat mod"));

    public static readonly ParrotRule Narancia = MatchRegex(Number28())
        .Reply(Narancia28);

    public static readonly IEnumerable<ParrotRule> All = new[]
    {
        Greg, United, Wysi, Wyfsi, Neat, Rat, Narancia
    };

    [GeneratedRegex("(^|.*[^\\d])28([^\\d].*|$)")]
    private static partial Regex Number28();
}