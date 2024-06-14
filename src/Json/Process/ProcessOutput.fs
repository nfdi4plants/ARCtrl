namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

/// Functions for handling the ProcessOutput Type
module ProcessOutput =

    module ROCrate =
        let encoder (value : ProcessOutput) = 
            match value with
            | ProcessOutput.Sample s -> 
                Sample.ROCrate.encoder s
            | ProcessOutput.Data d ->
                Data.ROCrate.encoder d
            | ProcessOutput.Material m ->
                Material.ROCrate.encoder m

        let decoder: Decoder<ProcessOutput> =
            Decode.oneOf [
                Decode.map ProcessOutput.Sample Sample.ROCrate.decoder
                Decode.map ProcessOutput.Data Data.ROCrate.decoder
                Decode.map ProcessOutput.Material Material.ROCrate.decoder
            ]

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
            Decode.oneOf [
                Decode.map ProcessOutput.Sample Sample.ISAJson.decoder
                Decode.map ProcessOutput.Data Data.ISAJson.decoder
                Decode.map ProcessOutput.Material Material.ISAJson.decoder
            ]


[<AutoOpen>]
module ProcessOutputExtensions =
    
    type ProcessOutput with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString ProcessOutput.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:ProcessOutput) ->
                ProcessOutput.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.toISAJsonString(?spaces) = 
            ProcessOutput.toISAJsonString(?spaces=spaces) this

        static member fromROCrateJsonString (s:string) =
            Decode.fromJsonString ProcessOutput.ROCrate.decoder s

        static member toROCrateJsonString(?spaces) =
            fun (f:ProcessOutput) ->
                ProcessOutput.ROCrate.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.toROCrateJsonString(?spaces) =
            ProcessOutput.toROCrateJsonString(?spaces=spaces) this