namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

module ProcessParameterValue =
    
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

    //let genID (p:ProcessParameterValue) = 
    //    match (p.Value,p.Category) with
    //    | (Some v, Some c) -> 
    //        "#Param_" 
    //        + (ProtocolParameter.getNameText c).Replace(" ","_") 
    //        + "_" 
    //        + (Value.getText v).Replace(" ","_")
    //    | _ -> "#EmptyParameterValue"

    //let encoder (options : ConverterOptions) (oa : ProcessParameterValue) = 
    //    [
    //        if options.SetID then 
    //            "@id", Encode.string (oa |> genID)
    //        if options.IsJsonLD then 
    //            "@type", (Encode.list [Encode.string "ProcessParameterValue"])
    //            "additionalType", Encode.string "ProcessParameterValue"
    //        if options.IsJsonLD then
    //            if oa.Category.IsSome && oa.Category.Value.ParameterName.IsSome then
    //                Encode.tryInclude "category" Encode.string (oa.Category.Value.ParameterName.Value.Name)
    //            if oa.Category.IsSome && oa.Category.Value.ParameterName.IsSome then
    //                Encode.tryInclude "categoryCode" Encode.string (oa.Category.Value.ParameterName.Value.TermAccessionNumber)
    //            if oa.Value.IsSome then
    //                "value", 
    //                    (match oa.Value.Value with
    //                        | Value.Float f -> 
    //                            Encode.float f
    //                        | Value.Int i -> 
    //                            Encode.int i
    //                        | Value.Name s -> 
    //                            Encode.string s
    //                        | Value.Ontology s -> 
    //                            Encode.string oa.ValueText
    //                    )
    //            if oa.Value.IsSome && oa.Value.Value.IsAnOntology then
    //                Encode.tryInclude "valueCode" Encode.string (oa.Value.Value.AsOntology()).TermAccessionNumber
    //            if oa.Unit.IsSome then Encode.tryInclude "unit" Encode.string (oa.Unit.Value.Name)
    //            if oa.Unit.IsSome then Encode.tryInclude "unitCode" Encode.string (oa.Unit.Value.TermAccessionNumber)
    //        else
    //            Encode.tryInclude "category" (ProtocolParameter.encoder options) (oa.Category)
    //            Encode.tryInclude "value" (Value.encoder options) (oa.Value)
    //            Encode.tryInclude "unit" (OntologyAnnotation.encoder options) (oa.Unit)
    //        if options.IsJsonLD then
    //            "@context", ROCrateContext.ProcessParameterValue.context_jsonvalue
    //    ]
    //    |> Encode.choose
    //    |> Encode.object

    //let decoder (options : ConverterOptions) : Decoder<ProcessParameterValue> =
    //    if not options.IsJsonLD then
    //        Decode.object (fun get ->
    //            {
    //                Category = get.Optional.Field "category" (ProtocolParameter.decoder options)
    //                Value = get.Optional.Field "value" (Value.decoder options)
    //                Unit = get.Optional.Field "unit" (OntologyAnnotation.decoder options)
    //            }
    //        )
    //    else
    //        Decode.object (fun get ->
    //            let categoryName = get.Optional.Field "category" (Decode.string)
    //            let categoryCode = get.Optional.Field "categoryCode" (Decode.string)
    //            let category =
    //                match categoryName,categoryCode with
    //                | None,None -> None
    //                | _ -> Some (ProtocolParameter.make None (Some (OntologyAnnotation.make None categoryName None categoryCode None)))
    //            let valueName = get.Optional.Field "value" (Value.decoder options)
    //            let valueCode = get.Optional.Field "valueCode" (Decode.string)
    //            let value =
    //                match valueName,valueCode with
    //                | Some (Value.Name name), Some code ->
    //                    let oa = OntologyAnnotation.make None (Some name) None (Some (URI.fromString code)) None
    //                    let vo = Value.Ontology(oa)
    //                    Some vo
    //                | None, Some code ->
    //                    let oa = OntologyAnnotation.make None None None (Some (URI.fromString code)) None
    //                    let vo = Value.Ontology(oa)
    //                    Some vo
    //                | Some (Value.Name name), None -> valueName
    //                | Some (Value.Float name), None -> valueName
    //                | Some (Value.Int name), None -> valueName
    //                | _ -> None
    //            let unitName = get.Optional.Field "unit" (Decode.string)
    //            let unitCode = get.Optional.Field "unitCode" (Decode.string)
    //            let unit = 
    //                match unitName,unitCode with
    //                | None,None -> None
    //                | _ -> Some (OntologyAnnotation.make None unitName None unitCode None)
    //            {
    //                Category = category
    //                Value = value
    //                Unit = unit
    //            }
    //        )

[<AutoOpen>]
module ProcessParameterValueExtensions =
    
    type ProcessParameterValue with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString ProcessParameterValue.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:ProcessParameterValue) ->
                ProcessParameterValue.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)