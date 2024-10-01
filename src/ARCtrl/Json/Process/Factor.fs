namespace ARCtrl.Json

open ARCtrl.Process

[<AutoOpen>]
module FactorExtensions =
    
    type Factor with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Factor.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:Factor) ->
                Factor.ISAJson.encoder None f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToISAJsonString(?spaces) =
            Factor.toISAJsonString(?spaces=spaces) this