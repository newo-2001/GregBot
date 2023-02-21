using Gregbot.Domain.Testing.Assertions;
using Xunit;

namespace GregBot.Tests.Modules.Parrot.Rules;

using Rules = GregBot.Modules.Parrot.Rules;

public class OsuTests
{
    public class ReplyIsNullWhen
    {
        [Fact]
        public void MessageDoesNotContainTheStringUnited() =>
            Rules.United("Joe Biden moment").Should().BeNull();

        [Fact]
        public void MessageContainsTheStringUnited() =>
            Rules.United("Cunited").Should().BeNull();

        [Fact]
        public void MessageDoesNotContainTheString727() =>
            Rules.Wysi("Literally anything").Should().BeNull();

        [Fact]
        public void MessageDoesNotContainTheStringWysi() =>
            Rules.Wyfsi.Invoke("When you see it").Should().BeNull();
    }

    public class ReplyIsWeKickedAKidWhen
    {
        private const string REPLY = "we kicked a kid";

        [Fact]
        public void MessageContainsTheWordUnited() =>
            Rules.United("united we stand").Should().HaveContent(REPLY);

        [Fact]
        public void MessageContainsTheWordUnitedWithUppercaseLetters() =>
            Rules.United("we are United").Should().HaveContent(REPLY);
    }

    public class ReplyIsWysiWhen
    {
        private const string REPLY = "wysi";

        [Theory]
        [InlineData("727")]
        [InlineData("7727")]
        public void MessageContainsTheString727(string message) =>
            Rules.Wysi(message).Should().HaveContent(REPLY);
    }

    public class ReplyIsWyfsiWhen
    {
        private const string REPLY = "wyfsi";
        
        [Theory]
        [InlineData("wysi")]
        [InlineData("wysit")]
        public void MessageContainsTheStringWysi(string message) =>
            Rules.Wyfsi(message).Should().HaveContent(REPLY);

        [Fact]
        public void MessageContainsTheStringWysiWithUppercaseLetters() =>
            Rules.Wyfsi("WySi").Should().HaveContent(REPLY);
    }
}