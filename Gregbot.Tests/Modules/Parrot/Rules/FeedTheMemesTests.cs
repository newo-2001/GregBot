using Gregbot.Domain.Testing.Assertions;
using Xunit;

namespace GregBot.Tests.Modules.Parrot.Rules;

using Rules = GregBot.Modules.Parrot.Rules;

public class FeedTheMemesTests
{
    public class ReplyIsNullWhen
    {
        [Fact]
        public void MessageContainsTheStringRat() =>
            Rules.Rat("brat").Should().BeNull();

        [Fact]
        public void MessageContainsTheStringRats() =>
            Rules.Rat("brats").Should().BeNull();

        [Fact]
        public void MessageDoesNotContainTheStringNeat() =>
            Rules.Neat("r/wordington").Should().BeNull();
    }

    public class ReplyIsHahaFunnyRatModWhen
    {
        private const string REPLY = "haha funny rat mod";
        
        [Fact]
        public void MessageContainsTheWordRats() =>
            Rules.Rat("rats rats we are the rats").Should().HaveContent(REPLY);

        [Fact]
        public void MessageContainsTheWordRatsWithUppercaseLetters() =>
            Rules.Rat("RatS RaTS we are the rAts").Should().HaveContent(REPLY);

        [Fact]
        public void MessageContainsTheWordRat() =>
            Rules.Rat("haha funny rat mod").Should().HaveContent(REPLY);

        [Fact]
        public void MessageContainsTheWordRatWithUppercaseLetters() =>
            Rules.Rat("haha funny RaT mod").Should().HaveContent(REPLY);
    }

    public class ReplyIsNeatIsAModByVazkiiWhen
    {
        private const string REPLY = "neat is a mod by Vazkii";

        [Fact]
        public void MessageContainsTheStringNeat() =>
            Rules.Neat("neatty").Should().HaveContent(REPLY);

        [Fact]
        public void MessageContainsTheStringNeatWithUppercaseLetters() =>
            Rules.Neat("Neatty").Should().HaveContent(REPLY);
    }
}