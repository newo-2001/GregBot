namespace GregBot.Modules.Parrot

open System.Text.RegularExpressions

module Matchers =
    type Matcher = string -> bool
    
    let Combine combinator left right message =
         combinator (left message) (right message)

    let (<&>) = Combine (&&)
    let (<|>) = Combine (||)
    
    let Any: Matcher seq -> Matcher = Seq.reduce (<|>)
    let All: Matcher seq -> Matcher = Seq.reduce (<&>)

    let StringMatcher (str: string) (message: string) =
        message.Contains str

    let WordMatcher (word: string) (message: string) =
        Array.contains word (message.Split(' '))

    let WordsMatcher: string seq -> Matcher =
        Seq.map WordMatcher
        >> All

    let ExactMatcher (word: string) = word.Equals

    let RegexMatcher (regex: Regex): Matcher = regex.IsMatch

    let LowerCase matcher: Matcher =
        fun msg -> msg.ToLowerInvariant()
        >> matcher