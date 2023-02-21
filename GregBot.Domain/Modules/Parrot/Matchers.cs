using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GregBot.Domain.Extensions;
using GregBot.Domain.Models;

namespace GregBot.Domain.Modules.Parrot;

public delegate bool Matcher(string message);

public static class Matchers
{
    public static Matcher MatchString(string str) => message => message.Contains(str);

    public static Matcher MatchWord(string word) => message =>
        message.Split(' ').Contains(word);

    public static Matcher MatchWords(IEnumerable<string> words) => message =>
        message.Split(' ').ContainsSequentially(words);

    public static Matcher MatchRegex(Regex regex) => regex.IsMatch;

    public static Matcher AsLowerCase(this Matcher matcher) => message =>
        matcher(message.ToLowerInvariant());

    public static ParrotRule Reply(this Matcher matcher, SendableMessage reply) => message =>
        matcher(message) ? reply : null;

    public static Matcher Or(this Matcher self, Matcher other) => message =>
        self(message) || other(message);

    public static Matcher And(this Matcher self, Matcher other) => message =>
        self(message) && other(message);

    public static Matcher Any(IEnumerable<Matcher> matchers) => message =>
        matchers.Any(matcher => matcher(message));

    public static Matcher All(IEnumerable<Matcher> matchers) => message =>
        matchers.All(matcher => matcher(message));
}