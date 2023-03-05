using GregBot.Domain.Testing.Core.Assertions;
using Xunit;

namespace GregBot.Tests.Unit.Modules.Parrot.Rules;

using Rules = GregBot.Modules.Parrot.Rules;

public class GregTests
{
    private const string REPLY = "greg";
    
    [Theory]
    [InlineData("gtnhorizons")]
    [InlineData("gt:nhorizons")]
    public void GregRule_Returns_Null_When_MessageDoesNotContain_TheWordGtnh(string message)
    {
        Rules.Greg(message).Should().BeNull();
    }

    [Theory]
    [InlineData("horizons new")]
    [InlineData("anew horizons")]
    public void GregRule_Returns_Null_When_MessageDoesNotContain_NewHorizons(string message)
    {
        Rules.Greg(message).Should().BeNull();
    }

    [Theory]
    [InlineData("Gregory edgeworth")]
    [InlineData("greg")]
    [InlineData("aggregate")]
    public void GregRule_Returns_Greg_When_MessageContains_TheStringGreg(string message)
    {
        Rules.Greg(message).Should().HaveContent(REPLY);
    }
    
    [Fact]
    public void GregRule_Returns_Null_When_MessageDoesNotContain_TheStringGreg()
    {
        Rules.Greg("Fortnite Battlepass").Should().BeNull();
    }

    [Theory]
    [InlineData("Грегори")]
    [InlineData("грегори")]
    public void GregRule_Returns_Greg_When_MessageContains_TheStringGregInCyrillic(string message)
    {
        Rules.Greg(message).Should().HaveContent(REPLY);
    }

    [Theory]
    [InlineData("I love gtnh")]
    [InlineData("gtnh for minecraft 1.7.10")]
    [InlineData("The gt:nh modpack")]
    [InlineData("The Gt:nH modpack")]
    public void GregRule_Returns_Greg_When_MessageContains_TheWordGtnh(string message)
    {
        Rules.Greg(message).Should().HaveContent(REPLY);
    }

    [Theory]
    [InlineData("The new horizons NASA space probe")]
    [InlineData("New HorizonS")]
    public void GregRule_Returns_Greg_When_messageContains_TheWordsNewHorizons(string message)
    {
        Rules.Greg(message).Should().HaveContent(REPLY);
    }
}
