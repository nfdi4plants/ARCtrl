namespace ARCtrl.Json

open Thoth.Json.Core
open ARCtrl.Process

[<AutoOpen>]
module MaterialTypeExtensions =
    
    type MaterialType with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString MaterialType.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:MaterialType) ->
                MaterialType.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToISAJsonString(?spaces) =
            MaterialType.toISAJsonString(?spaces=spaces) this