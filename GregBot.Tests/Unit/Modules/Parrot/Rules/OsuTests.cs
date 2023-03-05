using GregBot.Domain.Testing.Core.Assertions;
using Xunit;

namespace GregBot.Tests.Unit.Modules.Parrot.Rules;

using Rules = GregBot.Modules.Parrot.Rules;

public class OsuTests
{
    [Fact]
    public void UnitedRule_Returns_Null_When_MessageDoesNotContain_TheStringUnited()
    {
        Rules.United("Cunited").Should().BeNull();
    }

    [Theory]
    [InlineData("united we stand")]
    [InlineData("The United States of America)")]
    public void UnitedRule_Returns_WeKickedAKid_When_MessageContains_TheWordUnited(string message)
    {
        Rules.United(message).Should().HaveContent("we kicked a kid");
    }

    [Fact]
    public void WysiRule_Returns_Null_When_MessageDoesNotContain_TheString727()
    {
        Rules.Wysi("Cookiezi").Should().BeNull();
    }

    [Fact]
    public void WyfsiRule_Returns_Null_When_MessageDoesNotContain_TheStringWysi()
    {
        Rules.Wyfsi("When you see it").Should().BeNull();
    }

    [Theory]
    [InlineData("727")]
    [InlineData("7727")]
    public void WysiRule_Returns_Wysi_When_MessageContains_TheString727(string message)
    {
        Rules.Wysi(message).Should().HaveContent("wysi");
    }

    [Theory]
    [InlineData("wysi")]
    [InlineData("WysIt")]
    public void WyfsiRule_Returns_Wyfsi_When_MessageContains_TheStringWysi(string message)
    {
        Rules.Wyfsi(message).Should().HaveContent("wyfsi");
    }
}