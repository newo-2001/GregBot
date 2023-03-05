using GregBot.Domain.Testing.Core.Assertions;
using Xunit;

namespace GregBot.Tests.Unit.Modules.Parrot.Rules;

using Rules = GregBot.Modules.Parrot.Rules;

public class FeedTheMemesTests
{
    [Theory]
    [InlineData("rat rat we are the rat")]
    [InlineData("rats rats we are the rats")]
    [InlineData("RaTs rAts we are the raTs")]
    public void RatRule_Returns_HahaFunnyRatMod_When_MessageContains_TheWordRat(string message)
    {
        Rules.Rat(message).Should().HaveContent("haha funny rat mod");
    }

    [Theory]
    [InlineData("brat")]
    [InlineData("brats")]
    public void RatRule_Returns_Null_When_MessageDoesNotContain_TheWordRat(string message)
    {
        Rules.Rat(message).Should().BeNull();
    }

    [Fact]
    public void NeatRule_Returns_Null_When_MessageDoesNotContain_TheStringNeat()
    {
        Rules.Neat("r/wordington").Should().BeNull();
    }

    [Theory]
    [InlineData("neatty")]
    [InlineData("Neatty")]
    public void NeatRule_Returns_NeatIsAModByVazkii_When_MessageContains_TheStringNeat(string message)
    {
        Rules.Neat(message).Should().HaveContent("neat is a mod by Vazkii");
    }
}