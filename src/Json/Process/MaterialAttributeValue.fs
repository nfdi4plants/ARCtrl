namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open System.IO

module MaterialAttributeValue =

    module ROCrate =

        let encoder : MaterialAttributeValue -> Json= 
            PropertyValue.ROCrate.encoder<MaterialAttributeValue>

        let decoder : Decoder<MaterialAttributeValue> =
            PropertyValue.ROCrate.decoder<MaterialAttributeValue> (MaterialAttributeValue.createAsPV)

    module ISAJson =
      
        let encoder (oa : MaterialAttributeValue) = 
            [
                Encode.tryInclude "@id" Encode.string oa.ID
                Encode.tryInclude "category" MaterialAttribute.ISAJson.encoder oa.Category
                Encode.tryInclude "value" Value.ISAJson.encoder oa.Value
                Encode.tryInclude "unit" OntologyAnnotation.ISAJson.encoder oa.Unit
            ]
            |> Encode.choose
            |> Encode.object

        let decoder : Decoder<MaterialAttributeValue> =
            Decode.object (fun get ->
                {
                    ID = get.Optional.Field "@id" Decode.uri
                    Category = get.Optional.Field "category" MaterialAttribute.ISAJson.decoder
                    Value = get.Optional.Field "value" Value.ISAJson.decoder
                    Unit = get.Optional.Field "unit" OntologyAnnotation.ISAJson.decoder
                }
            )

[<AutoOpen>]
module MaterialAttributeValueExtensions =
    
    type MaterialAttributeValue with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString MaterialAttributeValue.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:MaterialAttributeValue) ->
                MaterialAttributeValue.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString MaterialAttributeValue.ROCrate.decoder s

        static member toROCrateJsonString(?spaces) =
            fun (f:MaterialAttributeValue) ->
                MaterialAttributeValue.ROCrate.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)