namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open System.IO

module AnnotationValue =

    /// <summary>
    /// This decodes from integer, float or string to string only!
    /// </summary>
    /// <param name="options"></param>
    /// <param name="s"></param>
    /// <param name="json"></param>
    let decoder : Decoder<string> =
        Decode.oneOf [
            Decode.map string Decode.int
            Decode.map string Decode.float
            Decode.string
        ]
            
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
            Encode.tryInclude "description" Encode.string (osr.Description)
            Encode.tryInclude "file" Encode.string (osr.File)
            Encode.tryInclude "name" Encode.string (osr.Name)
            Encode.tryInclude "version" Encode.string (osr.Version)
            Encode.tryIncludeSeq "comments" (Comment.encoder options) (osr.Comments)
            if options.IsJsonLD then
                "@context", ROCrateContext.OntologySourceReference.context_jsonvalue
        ]
        |> Encode.choose
        |> Encode.object

    let decoder (options : ConverterOptions) : Decoder<OntologySourceReference> =
        Decode.object (fun get ->
            OntologySourceReference(
                ?description = get.Optional.Field "description" Decode.uri,
                ?file = get.Optional.Field "file" Decode.string,
                ?name = get.Optional.Field "name" Decode.string,
                ?version = get.Optional.Field "version" Decode.string,
                ?comments = get.Optional.Field "comments" (Decode.resizeArray (Comment.decoder options))               
            )
        )

    let fromJsonString (s:string) = 
        Decode.fromJsonString (decoder (ConverterOptions())) s  

    let fromJsonldString (s:string) = 
        Decode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s      

    let toJsonString (oa:OntologySourceReference) = 
        encoder (ConverterOptions()) oa
        |> Encode.toJsonString 2

    /// exports in json-ld format
    let toJsonldString (oa:OntologySourceReference) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) oa
        |> Encode.toJsonString 2

    let toJsonldStringWithContext (a:OntologySourceReference) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) a
        |> Encode.toJsonString 2

        // let fromFile (path : string) = 
        //     File.ReadAllText path 
        //     |> fromString

        //let toFile (path : string) (osr:OntologySourceReference) = 
        //    File.WriteAllText(path,toString osr)

module OntologyAnnotation =  
    
    let genID (o:OntologyAnnotation) : string = 
        match o.TermAccessionNumber with
        | Some ta -> URI.toString ta
        | None -> match o.TermSourceREF with
                  | Some r -> "#" + r.Replace(" ","_")
                  | None -> match o.Name with
                            | Some n -> "#UserTerm_" + n .Replace(" ","_")
                            | None -> "#DummyOntologyAnnotation"

    let encoder (options : ConverterOptions) (oa : OntologyAnnotation) = 
        let commentEncoder = if options.IsJsonLD then Comment.encoderDisambiguatingDescription else Comment.encoder options
        [
            if options.SetID then 
                "@id", Encode.string (oa |> genID)
            if options.IsJsonLD then 
                "@type", Encode.string "OntologyAnnotation"
            Encode.tryInclude "annotationValue" Encode.string (oa.Name)
            Encode.tryInclude "termSource" Encode.string (oa.TermSourceREF)
            Encode.tryInclude "termAccession" Encode.string (oa.TermAccessionNumber)
            Encode.tryIncludeSeq "comments" commentEncoder (oa.Comments)
            if options.IsJsonLD then
                "@context", ROCrateContext.OntologyAnnotation.context_jsonvalue
        ]
        |> Encode.choose
        |> Encode.object


    let decoder (options : ConverterOptions) : Decoder<OntologyAnnotation> =
        Decode.object (fun get ->
            OntologyAnnotation.create(
                ?name = get.Optional.Field "annotationValue" (AnnotationValue.decoder options),
                ?tsr = get.Optional.Field "termSource" Decode.string,
                ?tan = get.Optional.Field "termAccession" Decode.string,
                ?comments = get.Optional.Field "comments" (Decode.resizeArray (Comment.decoder options))               
            )
        )


    let compressedEncoder (stringTable : StringTableMap) (options : ConverterOptions) (oa : OntologyAnnotation) = 
        [
            if options.SetID then "@id", Encode.string (oa |> genID)
            if options.IsJsonLD then "@type", Encode.string "OntologyAnnotation"
            Encode.tryInclude "a" (StringTable.encodeString stringTable) (oa.Name)
            Encode.tryInclude "ts" (StringTable.encodeString stringTable) (oa.TermSourceREF)
            Encode.tryInclude "ta" (StringTable.encodeString stringTable) (oa.TermAccessionNumber)
            Encode.tryIncludeSeq "comments" (Comment.encoder options) (oa.Comments)
        ]
        |> Encode.choose
        |> Encode.object


    let compressedDecoder (stringTable : StringTableArray) (options : ConverterOptions) : Decoder<OntologyAnnotation> =
        Decode.object (fun get ->
            OntologyAnnotation(
                ?name = get.Optional.Field "a" (StringTable.decodeString stringTable),
                ?tsr = get.Optional.Field "ts" (StringTable.decodeString stringTable),
                ?tan = get.Optional.Field "ta" (StringTable.decodeString stringTable),
                ?comments = get.Optional.Field "comments" (Decode.resizeArray (Comment.decoder options))               
            )
        )

    let fromJsonString (s:string) = 
        Decode.fromJsonString (decoder (ConverterOptions())) s   
        
    let fromJsonldString (s:string) = 
        Decode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s  

    let toJsonString (oa:OntologyAnnotation) = 
        encoder (ConverterOptions()) oa
        |> Encode.toJsonString 2
    
    /// exports in json-ld format
    let toJsonldString (oa:OntologyAnnotation) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) oa
        |> Encode.toJsonString 2

    let toJsonldStringWithContext (a:OntologyAnnotation) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) a
        |> Encode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (oa:OntologyAnnotation) = 
    //    File.WriteAllText(path,toString oa)