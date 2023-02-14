namespace GregBot.Modules.Parrot

module Rules =
    open Matchers

    type Rule = string -> string option

    let RuleFor reply matcher message =
        if matcher message then Some reply else None

    let Greg =
        let matcher =
            Any [
                StringMatcher "greg"
                StringMatcher "грег"
                WordMatcher "gtnh";
                WordMatcher "gt:nh";
                WordsMatcher ["new"; "horizons"]
            ] |> LowerCase

        RuleFor "greg" matcher

    let United = RuleFor "united" (WordsMatcher ["we"; "kicked"; "a"; "kid"] |> LowerCase)
    let Wysi = RuleFor "727" (StringMatcher "727" |> LowerCase)
    let Wyfsi = RuleFor "wyfsi" (WordMatcher "wysi" |> LowerCase)
    let Neat = RuleFor "neat is a mod by Vazkii" (StringMatcher "neat" |> LowerCase)
    let Rat = RuleFor "haha funny rat mod" ((WordMatcher "rat" <|> WordMatcher "rats") |> LowerCase)

    let All = [Greg; United; Wysi; Wyfsi; Neat; Rat]

