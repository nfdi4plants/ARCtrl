namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

module ProcessParameterValue =
    
    module ROCrate =

        let encoder : ProcessParameterValue -> Json= 
            PropertyValue.ROCrate.encoder<ProcessParameterValue>

        let decoder : Decoder<ProcessParameterValue> =
            PropertyValue.ROCrate.decoder<ProcessParameterValue> (ProcessParameterValue.createAsPV)

    module ISAJson =

        let encoder (oa : ProcessParameterValue) = 
            [
                Encode.tryInclude "category" ProtocolParameter.ISAJson.encoder oa.Category
                Encode.tryInclude "value" Value.ISAJson.encoder oa.Value
                Encode.tryInclude "unit" OntologyAnnotation.ISAJson.encoder oa.Unit
            ]
            |> Encode.choose
            |> Encode.object

        let decoder : Decoder<ProcessParameterValue> =
            Decode.object (fun get ->
                {
                    Category = get.Optional.Field "category" ProtocolParameter.ISAJson.decoder
                    Value = get.Optional.Field "value" Value.ISAJson.decoder
                    Unit = get.Optional.Field "unit" OntologyAnnotation.ISAJson.decoder
                }
            )

[<AutoOpen>]
module ProcessParameterValueExtensions =
    
    type ProcessParameterValue with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString ProcessParameterValue.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:ProcessParameterValue) ->
                ProcessParameterValue.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        static member fromROCrateJsonString (s:string) =
            Decode.fromJsonString ProcessParameterValue.ROCrate.decoder s

        static member toROCrateJsonString(?spaces) =
            fun (f:ProcessParameterValue) ->
                ProcessParameterValue.ROCrate.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)