namespace ARCtrl.Json


open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open System.IO


module FactorValue =
    
    module ROCrate =

        let encoder : FactorValue -> Json= 
            PropertyValue.ROCrate.encoder<FactorValue>

        let decoder : Decoder<FactorValue> =
            PropertyValue.ROCrate.decoder<FactorValue> (FactorValue.createAsPV)

    module ISAJson = 

        let encoder (fv : FactorValue) = 
            [
                // Is this required for ISA-JSON? The FactorValue type has an @id field
                Encode.tryInclude "@id" Encode.string fv.ID 
                Encode.tryInclude "category" Factor.ISAJson.encoder fv.Category
                Encode.tryInclude "value" Value.ISAJson.encoder fv.Value
                Encode.tryInclude "unit" OntologyAnnotation.ISAJson.encoder fv.Unit
            ]
            |> Encode.choose
            |> Encode.object

        let decoder: Decoder<FactorValue> =
            Decode.object (fun get ->
                {
                    ID = get.Optional.Field "@id" Decode.uri
                    Category = get.Optional.Field "category" Factor.ISAJson.decoder
                    Value = get.Optional.Field "value" Value.ISAJson.decoder
                    Unit = get.Optional.Field "unit" OntologyAnnotation.ISAJson.decoder
                }
            )

[<AutoOpen>]
module FactorValueExtensions =
    
    type FactorValue with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString FactorValue.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:FactorValue) ->
                FactorValue.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)