namespace ARCtrl.Json

open ARCtrl.Process

[<AutoOpen>]
module ProtocolParameterExtensions =
    
    type ProtocolParameter with
    
        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString ProtocolParameter.ISAJson.decoder s   
    
        static member toISAJsonString(?spaces) =
            fun (v:ProtocolParameter) ->
                ProtocolParameter.ISAJson.encoder None v
                |> Encode.toJsonString (Encode.defaultSpaces spaces)
                
        member this.ToISAJsonString(?spaces) =
            ProtocolParameter.toISAJsonString(?spaces=spaces) this