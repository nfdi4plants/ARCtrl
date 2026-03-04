namespace ARCtrl.Json

open ARCtrl.Process

[<AutoOpen>]
module ProtocolExtensions =
    
    type Protocol with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Protocol.ISAJson.decoder s   

        static member toISAJsonString(?spaces, ?useIDReferencing) =
            let useIDReferencing = Option.defaultValue false useIDReferencing
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (f:Protocol) ->
                Protocol.ISAJson.encoder None None None idMap f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)
        
        member this.ToISAJsonString(?spaces) =
            Protocol.toISAJsonString(?spaces=spaces) this
