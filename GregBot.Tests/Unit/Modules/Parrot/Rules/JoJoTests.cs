using GregBot.Domain.Testing.Core.Assertions;
using Xunit;

namespace GregBot.Tests.Unit.Modules.Parrot.Rules;

using Rules = GregBot.Modules.Parrot.Rules;

public class JoJoTests
{
    [Fact]
    public void NaranciaRule_Returns_Null_When_MessageDoesNotContain_TheNumber28()
    {
        Rules.Narancia("jojo").Should().BeNull();
    }

    [Theory]
    [InlineData("288")]
    [InlineData("328")]
    public void NaranciaRule_Returns_Null_When_MessageContains_ANumberThatContainsTheNumber28(string message)
    {
        Rules.Narancia(message).Should().BeNull();
    }

    [Theory]
    [InlineData("I am 28 years old")]
    [InlineData("€28,55")]
    [InlineData("28cm")]
    public void NaranciaRule_Returns_Attachment_When_MessageContains_TheNumber28(string message)
    {
        Rules.Narancia(message).Should().HaveAttachment("28.jpg");
    }
}