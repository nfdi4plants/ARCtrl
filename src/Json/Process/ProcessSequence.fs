namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

module ProcessSequence = 

    let fromISAJsonString (s:string) = 
        Decode.fromJsonString (Decode.list Process.ISAJson.decoder) s  

    let toISAJsonString (spaces) =
        fun (f:Process list) ->
            f
            |> List.map Process.ISAJson.encoder
            |> Encode.list
            |> Encode.toJsonString (Encode.defaultSpaces spaces)