namespace ARCtrl.Json


open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open System.IO


module FactorValue =
    
    //module ROCrate =

    //    let genID (fv:FactorValue) : string = 
    //        match fv.ID with
    //        | Some id -> URI.toString id
    //        | None -> "#EmptyFactorValue"

    //    let encoder (fv : FactorValue) = 
    //        [
    //            "@id", Encode.string (fv |> genID)
    //            "@type", (Encode.list [Encode.string "FactorValue"])
    //            "additionalType", Encode.string "FactorValue"
    //            Encode.tryInclude "categoryName" (fun f -> Encode.option Encode.string f.Name) fv.Category
    //            Encode.tryInclude "category" (fun f -> Encode.option (fun (ft: OntologyAnnotation) -> Encode.option Encode.string ft.Name) f.FactorType) fv.Category
    //            Encode.tryInclude "categoryCode" (fun f -> Encode.option (fun (ft: OntologyAnnotation) -> Encode.option Encode.string ft.TermAccessionNumber) f.FactorType) fv.Category
    //            if fv.Value.IsSome then 
    //                "value", Encode.string fv.ValueText
    //            if fv.Value.IsSome && fv.Value.Value.IsAnOntology then
    //                Encode.tryInclude "valueCode" Encode.string (oa.Value.Value.AsOntology()).TermAccessionNumber
    //            if oa.Unit.IsSome then Encode.tryInclude "unit" Encode.string (oa.Unit.Value.Name)
    //            if oa.Unit.IsSome then Encode.tryInclude "unitCode" Encode.string (oa.Unit.Value.TermAccessionNumber)
    //            "@context", ROCrateContext.FactorValue.context_jsonvalue
    //        ]
    //        |> Encode.choose
    //        |> Encode.object

    //    let decoder (options : ConverterOptions) : Decoder<FactorValue> =
    //        Decode.object (fun get ->
    //            {
    //                ID = get.Optional.Field "@id" GDecode.uri
    //                Category = get.Optional.Field "category" (Factor.decoder options)
    //                Value = get.Optional.Field "value" (Value.decoder options)
    //                Unit = get.Optional.Field "unit" (OntologyAnnotation.decoder options)
    //            }
    //        )

    module ISAJson = 

        let encoder (fv : FactorValue) = 
            [
                // Is this required for ISA-JSON? The FactorValue type has an @id field
                Encode.tryInclude "@id" Encode.string fv.ID 
                Encode.tryInclude "category" Factor.ISAJson.encoder fv.Category
                Encode.tryInclude "value" Value.ISAJson.encoder fv.Value
                Encode.tryInclude "unit" OntologyAnnotation.encoder fv.Unit
            ]
            |> Encode.choose
            |> Encode.object

        let decoder: Decoder<FactorValue> =
            Decode.object (fun get ->
                {
                    ID = get.Optional.Field "@id" Decode.uri
                    Category = get.Optional.Field "category" Factor.ISAJson.decoder
                    Value = get.Optional.Field "value" Value.ISAJson.decoder
                    Unit = get.Optional.Field "unit" OntologyAnnotation.decoder
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