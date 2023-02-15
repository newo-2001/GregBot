using Xunit;
using GregBot.Modules.Parrot;
using FluentAssertions;

namespace GregBot.Modules.Parrot.Tests;
public class GregTests
{
    public class ReplyIsNullWhen
    {
        [Fact]
        public void MessageDoesNotContainGreg()
        {
            Replies.ReplyFor("Fortnite Battlepass").Should().BeNull();
        }
    }

    public class ReplyIsGregWhen
    {
        private const string RESPONSE = "greg";

        [Theory]
        [InlineData("greg")]
        [InlineData("gregory")]
        [InlineData("aggregate")]
        public void MessageContainsTheStringGreg(string message)
        {
            Replies.ReplyFor(message).Should().Be(RESPONSE);
        }

        [Fact]
        public void MessageContainsTheStringGregWithUppercaseLetters()
        {
            Replies.ReplyFor("Greg").Should().Be(RESPONSE);
        }

        [Fact]
        public void MessageContainsTheStringGregInCyrillic()
        {
            Replies.ReplyFor("грег").Should().Be(RESPONSE);
        }

        [Fact]
        public void MessageContainsTheStringGregInCyrillicWithUppercaseLetters()
        {
            Replies.ReplyFor("ГреГ").Should().Be(RESPONSE);
        }
    }
}
