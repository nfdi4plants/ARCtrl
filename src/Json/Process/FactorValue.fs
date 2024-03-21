namespace ARCtrl.Json


open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open System.IO


module FactorValue =
    
    let genID (fv:FactorValue) : string = 
        match fv.ID with
        | Some id -> URI.toString id
        | None -> "#EmptyFactorValue"

    let encoder (options : ConverterOptions) (oa : FactorValue) = 
        [
            if options.SetID then 
                "@id", Encode.string (oa |> genID)
            else 
                Encode.tryInclude "@id" Encode.string (oa.ID)
            if options.IsJsonLD then 
                "@type", (Encode.list [Encode.string "FactorValue"])
                "additionalType", Encode.string "FactorValue"
            if options.IsJsonLD then
                if oa.Category.IsSome then
                    Encode.tryInclude "categoryName" Encode.string (oa.Category.Value.Name)
                if oa.Category.IsSome && oa.Category.Value.FactorType.IsSome then
                    Encode.tryInclude "category" Encode.string (oa.Category.Value.FactorType.Value.Name)
                if oa.Category.IsSome && oa.Category.Value.FactorType.IsSome then
                    Encode.tryInclude "categoryCode" Encode.string (oa.Category.Value.FactorType.Value.TermAccessionNumber)
                if oa.Value.IsSome then "value", Encode.string (oa.ValueText)
                if oa.Value.IsSome && oa.Value.Value.IsAnOntology then
                    Encode.tryInclude "valueCode" Encode.string (oa.Value.Value.AsOntology()).TermAccessionNumber
                if oa.Unit.IsSome then Encode.tryInclude "unit" Encode.string (oa.Unit.Value.Name)
                if oa.Unit.IsSome then Encode.tryInclude "unitCode" Encode.string (oa.Unit.Value.TermAccessionNumber)
            else
                Encode.tryInclude "category" (Factor.encoder options) (oa.Category)
                Encode.tryInclude "value" (Value.encoder options) (oa.Value)
                Encode.tryInclude "unit" (OntologyAnnotation.encoder options) (oa.Unit)
            if options.IsJsonLD then
                "@context", ROCrateContext.FactorValue.context_jsonvalue
        ]
        |> Encode.choose
        |> Encode.object

    let decoder (options : ConverterOptions) : Decoder<FactorValue> =
        Decode.object (fun get ->
            {
                ID = get.Optional.Field "@id" GDecode.uri
                Category = get.Optional.Field "category" (Factor.decoder options)
                Value = get.Optional.Field "value" (Value.decoder options)
                Unit = get.Optional.Field "unit" (OntologyAnnotation.decoder options)
            }
        )

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s
    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s

    let toJsonString (f:FactorValue) = 
        encoder (ConverterOptions()) f
        |> Encode.toJsonString 2
    
    /// exports in json-ld format
    let toJsonldString (f:FactorValue) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) f
        |> Encode.toJsonString 2

    let toJsonldStringWithContext (a:FactorValue) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) a
        |> Encode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (f:FactorValue) = 
    //    File.WriteAllText(path,toString f)