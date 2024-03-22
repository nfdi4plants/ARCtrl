namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

/// Functions for handling the ProcessInput Type
module ProcessInput =

    module ISAJson =

        let encoder (value : ProcessInput) = 
            match value with
            | ProcessInput.Source s-> 
                Source.ISAJson.encoder s
            | ProcessInput.Sample s -> 
                Sample.ISAJson.encoder s
            | ProcessInput.Data d -> 
                Data.ISAJson.encoder d
            | ProcessInput.Material m -> 
                Material.ISAJson.encoder m

        let decoder: Decoder<ProcessInput> =
            { new Decoder<ProcessInput> with
                member this.Decode(s,json) = 
                    match Source.ISAJson.decoder.Decode(s,json) with
                    | Ok s -> Ok (ProcessInput.Source s)
                    | Error _ -> 
                        match Sample.ISAJson.decoder.Decode(s,json) with
                        | Ok s -> Ok (ProcessInput.Sample s)
                        | Error _ -> 
                            match Data.ISAJson.decoder.Decode(s,json) with
                            | Ok s -> Ok (ProcessInput.Data s)
                            | Error _ -> 
                                match Material.ISAJson.decoder.Decode(s,json) with
                                | Ok s -> Ok (ProcessInput.Material s)
                                | Error e -> Error e

            }

[<AutoOpen>]
module ProcessInputExtensions =
    
    type ProcessInput with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString ProcessInput.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:ProcessInput) ->
                ProcessInput.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)