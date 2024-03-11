namespace ARCtrl.ISA.Json

open Thoth.Json.Core

open ARCtrl.ISA
open System.IO

module AnnotationValue =

    /// <summary>
    /// This decodes from integer, float or string to string only!
    /// </summary>
    /// <param name="options"></param>
    /// <param name="s"></param>
    /// <param name="json"></param>
    let decoder (options : ConverterOptions) : Decoder<string> =
        { new Decoder<string> with
            member this.Decode (s,json) =
                match Decode.int.Decode(s,json) with
                | Ok i -> Ok <| string i
                | Error _ -> 
                    match Decode.float.Decode(s,json) with
                    | Ok f -> Ok <| string f
                    | Error _ -> 
                        match Decode.string.Decode(s,json) with
                        | Ok s -> Ok <| s
                        | Error e -> Error e       
        }
            

module OntologySourceReference = 
    
    let genID (o:OntologySourceReference) = 
        match o.File with
        | Some f -> f
        | None -> 
            match o.Name with
            | Some n -> "#OntologySourceRef_" + n.Replace(" ","_")
            | None -> "#DummyOntologySourceRef"

    let encoder (options : ConverterOptions) (osr : OntologySourceReference) = 
        [
            if options.SetID then 
                "@id", Encode.string (osr |> genID)
            if options.IsJsonLD then 
                "@type", Encode.string "OntologySourceReference"
            GEncode.tryInclude "description" Encode.string (osr.Description)
            GEncode.tryInclude "file" Encode.string (osr.File)
            GEncode.tryInclude "name" Encode.string (osr.Name)
            GEncode.tryInclude "version" Encode.string (osr.Version)
            GEncode.tryIncludeArray "comments" (Comment.encoder options) (osr.Comments)
            if options.IsJsonLD then
                "@context", ROCrateContext.OntologySourceReference.context_jsonvalue
        ]
        |> GEncode.choose
        |> Encode.object

    let decoder (options : ConverterOptions) : Decoder<OntologySourceReference> =
        Decode.object (fun get ->
            {
                Description = get.Optional.Field "description" GDecode.uri
                File = get.Optional.Field "file" Decode.string
                Name = get.Optional.Field "name" Decode.string
                Version = get.Optional.Field "version" Decode.string
                Comments = get.Optional.Field "comments" (Decode.array (Comment.decoder options))               
            }
        )

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s        

    let toJsonString (oa:OntologySourceReference) = 
        encoder (ConverterOptions()) oa
        |> GEncode.toJsonString 2

    /// exports in json-ld format
    let toJsonldString (oa:OntologySourceReference) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) oa
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (a:OntologySourceReference) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) a
        |> GEncode.toJsonString 2

    // let fromFile (path : string) = 
    //     File.ReadAllText path 
    //     |> fromString

    //let toFile (path : string) (osr:OntologySourceReference) = 
    //    File.WriteAllText(path,toString osr)

module OntologyAnnotation =  
    
    let genID (o:OntologyAnnotation) : string = 
        match o.ID with
        | Some id -> URI.toString id
        | None -> match o.TermAccessionNumber with
                  | Some ta -> URI.toString ta
                  | None -> match o.TermSourceREF with
                            | Some r -> "#" + r.Replace(" ","_")
                            | None -> match o.Name with
                                        | Some n -> "#UserTerm_" + n .Replace(" ","_")
                                        | None -> "#DummyOntologyAnnotation"

    let encoder (options : ConverterOptions) (oa : OntologyAnnotation) = 
        [
            if options.SetID then 
                "@id", Encode.string (oa |> genID)
            else 
                GEncode.tryInclude "@id" Encode.string (oa.ID)
            if options.IsJsonLD then 
                "@type", Encode.string "OntologyAnnotation"
            GEncode.tryInclude "annotationValue" Encode.string (oa.Name)
            GEncode.tryInclude "termSource" Encode.string (oa.TermSourceREF)
            GEncode.tryInclude "termAccession" Encode.string (oa.TermAccessionNumber)
            GEncode.tryIncludeArray "comments" (Comment.encoder options) (oa.Comments)
            if options.IsJsonLD then
                "@context", ROCrateContext.OntologyAnnotation.context_jsonvalue
        ]
        |> GEncode.choose
        |> Encode.object


    let decoder (options : ConverterOptions) : Decoder<OntologyAnnotation> =
        Decode.object (fun get ->
            OntologyAnnotation.create(
                ?Id = get.Optional.Field "@id" GDecode.uri,
                ?Name = get.Optional.Field "annotationValue" (AnnotationValue.decoder options),
                ?TermSourceREF = get.Optional.Field "termSource" Decode.string,
                ?TermAccessionNumber = get.Optional.Field "termAccession" Decode.string,
                ?Comments = get.Optional.Field "comments" (Decode.array (Comment.decoder options))               
            )
        )


    let compressedEncoder (stringTable : StringTableMap) (options : ConverterOptions) (oa : OntologyAnnotation) = 
        [
            if options.SetID then "@id", Encode.string (oa |> genID)
                else GEncode.tryInclude "@id" Encode.string (oa.ID)
            if options.IsJsonLD then "@type", Encode.string "OntologyAnnotation"
            GEncode.tryInclude "a" (StringTable.encodeString stringTable) (oa.Name)
            GEncode.tryInclude "ts" (StringTable.encodeString stringTable) (oa.TermSourceREF)
            GEncode.tryInclude "ta" (StringTable.encodeString stringTable) (oa.TermAccessionNumber)
            GEncode.tryIncludeArray "comments" (Comment.encoder options) (oa.Comments)
        ]
        |> GEncode.choose
        |> Encode.object


    let compressedDecoder (stringTable : StringTableArray) (options : ConverterOptions) : Decoder<OntologyAnnotation> =
        Decode.object (fun get ->
            OntologyAnnotation.create(
                ?Id = get.Optional.Field "@id" GDecode.uri,
                ?Name = get.Optional.Field "a" (StringTable.decodeString stringTable),
                ?TermSourceREF = get.Optional.Field "ts" (StringTable.decodeString stringTable),
                ?TermAccessionNumber = get.Optional.Field "ta" (StringTable.decodeString stringTable),
                ?Comments = get.Optional.Field "comments" (Decode.array (Comment.decoder options))               
            )
        )

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s        

    let toJsonString (oa:OntologyAnnotation) = 
        encoder (ConverterOptions()) oa
        |> GEncode.toJsonString 2
    
    /// exports in json-ld format
    let toJsonldString (oa:OntologyAnnotation) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) oa
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (a:OntologyAnnotation) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) a
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (oa:OntologyAnnotation) = 
    //    File.WriteAllText(path,toString oa)