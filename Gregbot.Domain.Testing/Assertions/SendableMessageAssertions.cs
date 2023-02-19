using Discord;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using GregBot.Domain.Models;

namespace Gregbot.Domain.Testing.Assertions;

public static class SendableMessageExtensions
{
    public static SendableMessageAssertions Should(this SendableMessage? message) => new(message);
}

public class SendableMessageAssertions : ReferenceTypeAssertions<SendableMessage, SendableMessageAssertions>
{
    public SendableMessageAssertions(SendableMessage? subject) : base(subject!) { }

    protected override string Identifier => "sendable message";

    public AndConstraint<SendableMessageAssertions> HaveContent()
    {
        Execute.Assertion
            .Given(() => Subject)
            .ForCondition(message => message.Content is not null)
            .FailWith("Expected message to have content but was null");
        return new AndConstraint<SendableMessageAssertions>(this);
    }

    public AndConstraint<SendableMessageAssertions> HaveNoContent()
    {
        Execute.Assertion
            .Given(() => Subject)
            .ForCondition(message => message.Content is null)
            .FailWith("Expected message to be have no content but was '{0}'", Subject.Content);
        return new AndConstraint<SendableMessageAssertions>(this);
    }

    public AndConstraint<SendableMessageAssertions> HaveContent(string content)
    {
        Execute.Assertion
            .Given(() => Subject)
            .ForCondition(message => message?.Content?.Equals(content) ?? false)
            .FailWith("Expected message to be '{0}' but was '{1}'", content, Subject?.Content);
        return new AndConstraint<SendableMessageAssertions>(this);
    }

    public AndConstraint<SendableMessageAssertions> HaveAttachment()
    {
        Execute.Assertion
            .Given(() => Subject)
            .ForCondition(message => message.Attachments?.Any() ?? false)
            .FailWith("Expected message to have an attachment but it didn't");
        return new AndConstraint<SendableMessageAssertions>(this);
    }

    public AndConstraint<SendableMessageAssertions> HaveNoAttachment()
    {
        Execute.Assertion
            .Given(() => Subject)
            .ForCondition(message => message is null || !message.Attachments!.Any())
            .FailWith("Expected message to have no attachments but it had '{0}'",
                Subject.Attachments?.Select(x => x.FileName));
        return new AndConstraint<SendableMessageAssertions>(this);
    }

    public AndConstraint<SendableMessageAssertions> HaveAttachment(string fileName)
    {
        Execute.Assertion
            .Given(() => Subject)
            .ForCondition(message => message.Attachments?.Any(x => x.FileName.Equals(fileName)) ?? false)
            .FailWith("Expected message to have attachment '{0}', but it didn't and instead had {1}",
                fileName, Subject.Attachments?.Select(x => x.FileName));
        return new AndConstraint<SendableMessageAssertions>(this);
    }
}