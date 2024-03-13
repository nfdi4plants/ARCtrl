namespace ARCtrl.ISA.Json


open Thoth.Json.Core

open ARCtrl.ISA
open System.IO

module Value = 

    let encoder (options : ConverterOptions) (value : Value) = 
        match value with
        | Value.Float f -> 
            Encode.float f
        | Value.Int i -> 
            Encode.int i
        | Value.Name s -> 
            Encode.string s
        | Value.Ontology s -> 
            OntologyAnnotation.encoder options s
        | _ -> Encode.nil

    let decoder (options : ConverterOptions) : Decoder<Value> =
        {
        
        new Decoder<Value> with
            member this.Decode (s,json) = 
                match Decode.int.Decode(s,json) with
                | Ok i -> Ok (Value.Int i)
                | Error _ -> 
                    match Decode.float.Decode(s,json) with
                    | Ok f -> Ok (Value.Float f)
                    | Error _ -> 
                        match OntologyAnnotation.decoder(options).Decode(s,json) with
                        | Ok f -> Ok (Value.Ontology f)
                        | Error _ -> 
                            match Decode.string.Decode(s,json) with
                            | Ok s -> Ok (Value.Name s)
                            | Error e -> Error e}



    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s   
    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s     

    let toJsonString (v:Value) = 
        encoder (ConverterOptions()) v
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (v:Value) = 
    //    File.WriteAllText(path,toString v)

module Factor =  
    
    let genID (f:Factor) : string = 
        match f.ID with
        | Some id -> URI.toString id 
        | None -> match f.Name with
                  | Some n -> "#Factor_" + n.Replace(" ","_")
                  | None -> "#EmptyFactor"

    let encoder (options : ConverterOptions) (oa : Factor) = 
        [
            if options.SetID then 
                "@id", Encode.string (oa |> genID)
            else 
                GEncode.tryInclude "@id" Encode.string (oa.ID)
            if options.IsJsonLD then 
                "@type", (Encode.list [Encode.string "Factor"])
            GEncode.tryInclude "factorName" Encode.string (oa.Name)
            if options.IsJsonLD then
                if oa.FactorType.IsSome then
                    GEncode.tryInclude "annotationValue" Encode.string (oa.Name)
                    GEncode.tryInclude "termSource" Encode.string (oa.FactorType.Value.TermSourceREF)
                    GEncode.tryInclude "termAccession" Encode.string (oa.FactorType.Value.TermAccessionNumber)
            else
                GEncode.tryInclude "factorType" (OntologyAnnotation.encoder options) (oa.FactorType)
            GEncode.tryIncludeArray "comments" (Comment.encoder options) (oa.Comments)
            if options.IsJsonLD then
                "@context", ROCrateContext.Factor.context_jsonvalue
        ]
        |> GEncode.choose
        |> Encode.object

    let decoder (options : ConverterOptions) : Decoder<Factor> =
        Decode.object (fun get ->
            {
                ID = get.Optional.Field "@id" GDecode.uri
                Name = get.Optional.Field "factorName" Decode.string
                FactorType = get.Optional.Field "factorType" (OntologyAnnotation.decoder options)
                Comments = get.Optional.Field "comments" (Decode.array (Comment.decoder options))               
            }
        )

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s
    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s

    let toJsonString (f:Factor) = 
        encoder (ConverterOptions()) f
        |> GEncode.toJsonString 2
    
    /// exports in json-ld format
    let toJsonldString (f:Factor) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) f
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (a:Factor) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) a
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (f:Factor) = 
    //    File.WriteAllText(path,toString f)


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
                GEncode.tryInclude "@id" Encode.string (oa.ID)
            if options.IsJsonLD then 
                "@type", (Encode.list [Encode.string "FactorValue"])
            if options.IsJsonLD then
                if oa.Category.IsSome then
                    GEncode.tryInclude "categoryName" Encode.string (oa.Category.Value.Name)
                if oa.Category.IsSome && oa.Category.Value.FactorType.IsSome then
                    GEncode.tryInclude "category" Encode.string (oa.Category.Value.FactorType.Value.Name)
                if oa.Category.IsSome && oa.Category.Value.FactorType.IsSome then
                    GEncode.tryInclude "categoryCode" Encode.string (oa.Category.Value.FactorType.Value.TermAccessionNumber)
                if oa.Value.IsSome then "value", Encode.string (oa.ValueText)
                if oa.Value.IsSome && oa.Value.Value.IsAnOntology then
                    GEncode.tryInclude "valueCode" Encode.string (oa.Value.Value.AsOntology()).TermAccessionNumber
                if oa.Unit.IsSome then GEncode.tryInclude "unit" Encode.string (oa.Unit.Value.Name)
                if oa.Unit.IsSome then GEncode.tryInclude "unitCode" Encode.string (oa.Unit.Value.TermAccessionNumber)
            else
                GEncode.tryInclude "category" (Factor.encoder options) (oa.Category)
                GEncode.tryInclude "value" (Value.encoder options) (oa.Value)
                GEncode.tryInclude "unit" (OntologyAnnotation.encoder options) (oa.Unit)
            if options.IsJsonLD then
                "@context", ROCrateContext.FactorValue.context_jsonvalue
        ]
        |> GEncode.choose
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
        |> GEncode.toJsonString 2
    
    /// exports in json-ld format
    let toJsonldString (f:FactorValue) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) f
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (a:FactorValue) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) a
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (f:FactorValue) = 
    //    File.WriteAllText(path,toString f)