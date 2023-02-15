namespace GregBot.Modules.Parrot

open System.Threading.Tasks
open GregBot.Domain.Attributes
open GregBot.Domain.Events
open GregBot.Domain
open GregBot.Domain.Interfaces
open GregBot.Modules.Parrot.Matchers
open Microsoft.Extensions.Logging

[<Priority(Priority.Low)>]
type ParrotModule(logger: ILogger<ParrotModule>, messageService: IMessageService) =
    let ReplyFor = Seq.reduce (Combine Option.orElse) Rules.All
    
    [<Priority(Priority.High)>]
    let OnMessage (event: MessageEvent) =
        let message = event.Message
        
        if messageService.IsSentBySelf message
        then Task.CompletedTask else
            let content = message.Content
            
            match ReplyFor content with
            | None -> Task.CompletedTask
            | Some(reply) ->
                logger.LogDebug("Replied with \'{Reply}\' to the message \'{Message}\'", reply, content)
                messageService.Reply(reply, message)
    
    member _.Name = "Parrot"
    member _.Activate (bot: GregBot) =
        bot.OnMessage.Subscribe(OnMessage)
    
    interface IModule with        
        member this.Name = this.Name
        member this.Activate bot = this.Activate bot
    