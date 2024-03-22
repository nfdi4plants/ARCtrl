namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open System.IO

module MaterialAttributeValue =

    module ISAJson =
      
        let encoder (oa : MaterialAttributeValue) = 
            [
                Encode.tryInclude "@id" Encode.string oa.ID
                Encode.tryInclude "category" MaterialAttribute.ISAJson.encoder oa.Category
                Encode.tryInclude "value" Value.ISAJson.encoder oa.Value
                Encode.tryInclude "unit" OntologyAnnotation.encoder oa.Unit
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

        //let genID (m:MaterialAttributeValue) : string = 
        //    match m.ID with
        //    | Some id -> URI.toString id
        //    | None -> "#EmptyMaterialAttributeValue" 

        //let encoder (options : ConverterOptions) (oa : MaterialAttributeValue) = 
        //    [
        //        if options.SetID then 
        //            "@id", Encode.string (oa |> genID)
        //        else 
        //            Encode.tryInclude "@id" Encode.string (oa.ID)
        //        if options.IsJsonLD then 
        //            "@type", (Encode.list [Encode.string "MaterialAttributeValue"])
        //            "additionalType", Encode.string "MaterialAttributeValue"
        //        if options.IsJsonLD then
        //            if oa.Category.IsSome && oa.Category.Value.CharacteristicType.IsSome then
        //                Encode.tryInclude "category" Encode.string (oa.Category.Value.CharacteristicType.Value.Name)
        //            if oa.Category.IsSome && oa.Category.Value.CharacteristicType.IsSome then
        //                Encode.tryInclude "categoryCode" Encode.string (oa.Category.Value.CharacteristicType.Value.TermAccessionNumber)
        //            if oa.Value.IsSome then "value", Encode.string (oa.ValueText)
        //            if oa.Value.IsSome && oa.Value.Value.IsAnOntology then
        //                Encode.tryInclude "valueCode" Encode.string (oa.Value.Value.AsOntology()).TermAccessionNumber
        //            if oa.Unit.IsSome then Encode.tryInclude "unit" Encode.string (oa.Unit.Value.Name)
        //            if oa.Unit.IsSome then Encode.tryInclude "unitCode" Encode.string (oa.Unit.Value.TermAccessionNumber)
        //        else
        //            Encode.tryInclude "category" (MaterialAttribute.encoder options) (oa.Category)
        //            Encode.tryInclude "value" (Value.encoder options) (oa.Value)
        //            Encode.tryInclude "unit" (OntologyAnnotation.encoder options) (oa.Unit)
        //        if options.IsJsonLD then 
        //            "@context", ROCrateContext.MaterialAttributeValue.context_jsonvalue
        //    ]
        //    |> Encode.choose
        //    |> Encode.object

        //let decoder (options : ConverterOptions) : Decoder<MaterialAttributeValue> =
        //    Decode.object (fun get ->
        //        {
        //            ID = get.Optional.Field "@id" GDecode.uri
        //            Category = get.Optional.Field "category" (MaterialAttribute.decoder options)
        //            Value = get.Optional.Field "value" (Value.decoder options)
        //            Unit = get.Optional.Field "unit" (OntologyAnnotation.decoder options)
        //        }
        //    )

[<AutoOpen>]
module MaterialAttributeValueExtensions =
    
    type MaterialAttributeValue with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString MaterialAttributeValue.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:MaterialAttributeValue) ->
                MaterialAttributeValue.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)