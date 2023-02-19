using GregBot.Domain.Models;
using Gregbot.Domain.Testing.Assertions;
using Xunit;

namespace GregBot.Modules.Parrot.Tests.Rules;

using Rules = GregBot.Modules.Parrot.Rules;

public class GregTests
{
    private static SendableMessage? Sending(string message) => Rules.Greg(message);
    
    public class ReplyIsNullWhen
    {

        [Fact]
        public void MessageIsIrrelevant() =>
            Sending("Fortnite Battlepass").Should().BeNull();

        [Fact]
        public void MessageContainsTheStringGtnh() =>
            Sending("gtnhorizons").Should().BeNull();

        [Fact]
        public void MessageContainsTheStringGtnhWithColon() =>
            Sending("gt:nhorizons").Should().BeNull();

        [Fact]
        public void MessageContainsTheStringNewHorizons() =>
            Sending("anew horizons").Should().BeNull();

        [Fact]
        public void MessageContainsTheWordsNewHorizonsOutOfOrder() =>
            Sending("horizons new").Should().BeNull();
    }

    public class ReplyIsGregWhen
    {
        private const string REPLY = "greg";

        [Theory]
        [InlineData("greg")]
        [InlineData("gregory edgeworth")]
        [InlineData("aggregate")]
        public void MessageContainsTheStringGreg(string message) =>
            Sending(message).Should().HaveContent(REPLY);

        [Fact]
        public void MessageContainsTheStringGregWithUppercaseLetters() =>
            Sending("Greg").Should().HaveContent(REPLY);

        [Fact]
        public void MessageContainsTheStringGregInCyrillic() =>
            Sending("грег").Should().HaveContent(REPLY);

        [Fact]
        public void MessageContainsTheStringGregInCyrillicWithUppercaseLetters() =>
            Sending("ГреГ").Should().HaveContent(REPLY);

        [Fact]
        public void MessageContainsTheWordGtnh() =>
            Sending("I love gtnh").Should().HaveContent(REPLY);

        [Fact]
        public void MessageContainsTheWordGtnhWithUppercaseLetters() =>
            Sending("GtNH for minecraft 1.7").Should().HaveContent(REPLY);

        [Fact]
        public void MessageContainsTheWordGtnhWithColon() =>
            Sending("this is gt:nh").Should().HaveContent(REPLY);

        [Fact]
        public void MessageContainsTheWordGtnhWithColonAndUppercaseLetters() =>
            Sending("GT:Nh").Should().HaveContent(REPLY);

        [Fact]
        public void MessageContainsTheWordsNewHorizons() =>
            Sending("The new horizons NASA space probe").Should().HaveContent(REPLY);

        [Fact]
        public void MessageContainsTheWordsNewHorizonsWithUppercaseLetters() =>
            Sending("New HorizonS").Should().HaveContent(REPLY);
    }
}
