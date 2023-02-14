using Xunit;
using GregBot.Modules.Parrot;
using FluentAssertions;

namespace GregBot.Tests.Parrot;
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
        private readonly string _response = "greg";

        [Theory]
        [InlineData("greg")]
        [InlineData("gregory")]
        [InlineData("aggregate")]
        public void MessageContainsTheStringGreg(string message)
        {
            Replies.ReplyFor(message).Should().Be(_response);
        }

        [Fact]
        public void MessageContainsTheStringGregWithUppercaseLetters()
        {
            Replies.ReplyFor("Greg").Should().Be(_response);
        }

        [Fact]
        public void MessageContainsTheStringGregInCyrillic()
        {
            Replies.ReplyFor("грег").Should().Be(_response);
        }

        [Fact]
        public void MessageContainsTheStringGregInCyrillicWithUppercaseLetters()
        {
            Replies.ReplyFor("ГреГ").Should().Be(_response);
        }
    }
}
