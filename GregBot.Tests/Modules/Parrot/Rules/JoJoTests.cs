using GregBot.Domain.Testing.Assertions;
using Xunit;

namespace GregBot.Tests.Modules.Parrot.Rules;

using Rules = GregBot.Modules.Parrot.Rules;

public class JoJoTests
{
    public class ReplyIsNullWhen
    {
        [Fact]
        public void MessageIsIrrelavent() =>
            Rules.Narancia("jojo").Should().BeNull();

        [Theory]
        [InlineData("288")]
        [InlineData("328")]
        public void MessageContainsANumberThatContainsTheNumber28(string message) =>
            Rules.Narancia(message).Should().BeNull();
    }

    public class ReplyIsAttachmentWhen
    {
        [Theory]
        [InlineData("I am 28 years old")]
        [InlineData("€28,55")]
        [InlineData("28cm")]
        public void MessageContainsTheNumber28(string message) =>
            Rules.Narancia(message).Should().HaveAttachment("28.jpg");
    }
}