namespace GregBot.Modules.Parrot

open Matchers

module Replies =
    let ReplyFor message =
        message
        |> Seq.reduce (Combine Option.orElse) Rules.All
        |> Option.defaultValue null
