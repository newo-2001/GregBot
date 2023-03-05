using System.Collections.Generic;
using Discord;

namespace GregBot.Domain.Core.Models;

public record SendableMessage(string? Content = null,
    IEnumerable<FileAttachment>? Attachments = null,
    IEnumerable<ISticker>? Stickers = null,
    IEnumerable<Embed>? Embeds = null,
    MessageReference? ReplyTo = null,
    bool Tts = false)
{    
    public SendableMessage(string content) : this()
    {
        Content = content;
    }

    public SendableMessage(FileAttachment attachment) : this()
    {
        Attachments = new[] { attachment };
    }

    public override string ToString() => Content ?? "";
}