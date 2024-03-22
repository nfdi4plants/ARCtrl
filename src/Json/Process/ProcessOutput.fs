namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

/// Functions for handling the ProcessOutput Type
module ProcessOutput =

    module ISAJson =

        let encoder (value : ProcessOutput) = 
            match value with
            | ProcessOutput.Sample s -> 
                Sample.ISAJson.encoder s
            | ProcessOutput.Data d -> 
                Data.ISAJson.encoder d
            | ProcessOutput.Material m -> 
                Material.ISAJson.encoder m

        let decoder: Decoder<ProcessOutput> =
            { new Decoder<ProcessOutput> with
                member this.Decode(s,json) = 
                    match Sample.ISAJson.decoder.Decode(s,json) with
                    | Ok s -> Ok (ProcessOutput.Sample s)
                    | Error _ -> 
                        match Data.ISAJson.decoder.Decode(s,json) with
                        | Ok s -> Ok (ProcessOutput.Data s)
                        | Error _ -> 
                            match Material.ISAJson.decoder.Decode(s,json) with
                            | Ok s -> Ok (ProcessOutput.Material s)
                            | Error e -> Error e
            }


[<AutoOpen>]
module ProcessOutputExtensions =
    
    type ProcessOutput with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString ProcessOutput.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:ProcessOutput) ->
                ProcessOutput.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)
