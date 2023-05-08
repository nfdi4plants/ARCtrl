namespace ISADotNet.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ISADotNet
open System.IO
open GEncode

module Value = 

    let encoder (options : ConverterOptions) (value : obj) = 
        match value with
        | :? Value as Value.Float f -> 
            Encode.float f
        | :? Value as Value.Int i -> 
            Encode.int i
        | :? Value as Value.Name s -> 
            Encode.string s
        | :? Value as Value.Ontology s -> 
            OntologyAnnotation.encoder options s
        | _ -> Encode.nil

    let decoder (options : ConverterOptions) : Decoder<Value> =
        fun s json ->
            match Decode.int s json with
            | Ok i -> Ok (Value.Int i)
            | Error _ -> 
                match Decode.float s json with
                | Ok f -> Ok (Value.Float f)
                | Error _ -> 
                    match OntologyAnnotation.decoder options s json with
                    | Ok f -> Ok (Value.Ontology f)
                    | Error _ -> 
                        match Decode.string s json with
                        | Ok s -> Ok (Value.Name s)
                        | Error e -> Error e


    let fromString (s:string) = 
        GDecode.fromString (decoder (ConverterOptions())) s        

    let toString (v:Value) = 
        encoder (ConverterOptions()) v
        |> Encode.toString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (v:Value) = 
    //    File.WriteAllText(path,toString v)

module Factor =  
    
    let genID (f:Factor) = 
        match f.ID with
            | Some id -> URI.toString id
            | None -> match f.Name with
                        | Some n -> "#Factor_" + n
                        | None -> "#EmptyFactor"

    let encoder (options : ConverterOptions) (oa : obj) = 
        [
            if options.SetID then "@id", GEncode.string (oa :?> Factor |> genID)
                else tryInclude "@id" GEncode.string (oa |> tryGetPropertyValue "ID")
            if options.IncludeType then "@type", GEncode.string "Factor"
            tryInclude "factorName" GEncode.string (oa |> tryGetPropertyValue "Name")
            tryInclude "factorType" (OntologyAnnotation.encoder options) (oa |> tryGetPropertyValue "FactorType")
            tryInclude "comments" (Comment.encoder options) (oa |> tryGetPropertyValue "Comments")
        ]
        |> GEncode.choose
        |> Encode.object

    let decoder (options : ConverterOptions) : Decoder<Factor> =
        Decode.object (fun get ->
            {
                ID = get.Optional.Field "@id" GDecode.uri
                Name = get.Optional.Field "factorName" Decode.string
                FactorType = get.Optional.Field "factorType" (OntologyAnnotation.decoder options)
                Comments = get.Optional.Field "comments" (Decode.list (Comment.decoder options))               
            }
        )

    let fromString (s:string) = 
        GDecode.fromString (decoder (ConverterOptions())) s

    let toString (f:Factor) = 
        encoder (ConverterOptions()) f
        |> Encode.toString 2
    
    /// exports in json-ld format
    let toStringLD (f:Factor) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) f
        |> Encode.toString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (f:Factor) = 
    //    File.WriteAllText(path,toString f)


module FactorValue =
    
    let genID (fv:FactorValue) = 
        match fv.ID with
            | Some id -> URI.toString id
            | None -> "#EmptyFactorValue"

    let encoder (options : ConverterOptions) (oa : obj) = 
        [
            if options.SetID then "@id", GEncode.string (oa :?> FactorValue |> genID)
                else tryInclude "@id" GEncode.string (oa |> tryGetPropertyValue "ID")
            if options.IncludeType then "@type", GEncode.string "FactorValue"
            tryInclude "category" (Factor.encoder options) (oa |> tryGetPropertyValue "Category")
            tryInclude "value" (Value.encoder options) (oa |> tryGetPropertyValue "Value")
            tryInclude "unit" (OntologyAnnotation.encoder options) (oa |> tryGetPropertyValue "Unit")
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

    let fromString (s:string) = 
        GDecode.fromString (decoder (ConverterOptions())) s

    let toString (f:FactorValue) = 
        encoder (ConverterOptions()) f
        |> Encode.toString 2
    
    /// exports in json-ld format
    let toStringLD (f:FactorValue) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) f
        |> Encode.toString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (f:FactorValue) = 
    //    File.WriteAllText(path,toString f)
