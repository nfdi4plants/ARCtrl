namespace ISA.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ISA
open System.IO

module AnnotationValue =

    let encoder (options : ConverterOptions) (value : obj) = 
        match value with
        | :? AnnotationValue as AnnotationValue.Float f -> 
            Encode.float f
        | :? AnnotationValue as AnnotationValue.Int i -> 
            Encode.int i
        | :? AnnotationValue as AnnotationValue.Text s -> 
            Encode.string s
        | _ -> Encode.nil

    let decoder (options : ConverterOptions) : Decoder<AnnotationValue> =
        fun s json ->
            match Decode.int s json with
            | Ok i -> Ok (AnnotationValue.Int i)
            | Error _ -> 
                match Decode.float s json with
                | Ok f -> Ok (AnnotationValue.Float f)
                | Error _ -> 
                    match Decode.string s json with
                    | Ok s -> Ok (AnnotationValue.Text s)
                    | Error e -> Error e


module OntologySourceReference = 
    
    let genID (o:OntologySourceReference) = 
        match o.File with
        | Some f -> f
        | None -> match o.Name with
                  | Some n -> "#OntologySourceRef_" + n.Replace(" ","_")
                  | None -> "#DummyOntologySourceRef"

    let encoder (options : ConverterOptions) (osr : obj) = 
        [
            if options.SetID then "@id", GEncode.string (osr :?> OntologySourceReference |> genID)
            if options.IncludeType then "@type", GEncode.string "OntologySourceReference"
            GEncode.tryInclude "description" GEncode.string (osr |> GEncode.tryGetPropertyValue "Description")
            GEncode.tryInclude "file" GEncode.string (osr |> GEncode.tryGetPropertyValue "File")
            GEncode.tryInclude "name" GEncode.string (osr |> GEncode.tryGetPropertyValue "Name")
            GEncode.tryInclude "version" GEncode.string (osr |> GEncode.tryGetPropertyValue "Version")
            GEncode.tryInclude "comments" (Comment.encoder options) (osr |> GEncode.tryGetPropertyValue "Comments")
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
                Comments = get.Optional.Field "comments" (Decode.list (Comment.decoder options))               
            }
        )

    let fromString (s:string) = 
        GDecode.fromString (decoder (ConverterOptions())) s        

    let toString (oa:OntologySourceReference) = 
        encoder (ConverterOptions()) oa
        |> Encode.toString 2

    /// exports in json-ld format
    let toStringLD (oa:OntologySourceReference) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) oa
        |> Encode.toString 2

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
                  | Some ta -> ta
                  | None -> match o.TermSourceREF with
                            | Some r -> "#" + r.Replace(" ","_")
                            | None -> "#DummyOntologyAnnotation"

    let encoder (options : ConverterOptions) (oa : obj) = 
        [
            if options.SetID then "@id", GEncode.string (oa :?> OntologyAnnotation |> genID)
                else GEncode.tryInclude "@id" GEncode.string (oa |> GEncode.tryGetPropertyValue "ID")
            if options.IncludeType then "@type", GEncode.string "OntologyAnnotation"
            GEncode.tryInclude "annotationValue" (AnnotationValue.encoder options) (oa |> GEncode.tryGetPropertyValue "Name")
            GEncode.tryInclude "termSource" GEncode.string (oa |> GEncode.tryGetPropertyValue "TermSourceREF")
            GEncode.tryInclude "termAccession" GEncode.string (oa |> GEncode.tryGetPropertyValue "TermAccessionNumber")
            GEncode.tryInclude "comments" (Comment.encoder options) (oa |> GEncode.tryGetPropertyValue "Comments")
        ]
        |> GEncode.choose
        |> Encode.object

    let localIDDecoder : Decoder<string> =
        fun s json ->
            match Decode.string s json with
            | Ok (Regex.ActivePatterns.TermAnnotation tan) -> 
                Ok (tan.TermSourceREF)
            | _ -> Ok ""
            //| Ok s -> Error (DecoderError(s,ErrorReason.FailMessage "Could not parse local ID from string"))
            //| Error e -> Error e


    let decoder (options : ConverterOptions) : Decoder<OntologyAnnotation> =
        Decode.object (fun get ->
            {
                ID = get.Optional.Field "@id" GDecode.uri
                Name = get.Optional.Field "annotationValue" (AnnotationValue.decoder options)
                TermSourceREF = get.Optional.Field "termSource" Decode.string
                //LocalID = try get.Optional.Field "termAccession" localIDDecoder with | _ -> None
                LocalID = get.Optional.Field "termAccession" localIDDecoder |> Option.bind (fun s -> if s = "" then None else Some s)
                TermAccessionNumber = get.Optional.Field "termAccession" Decode.string
                Comments = get.Optional.Field "comments" (Decode.list (Comment.decoder options))               
            }
        )

    let fromString (s:string) = 
        GDecode.fromString (decoder (ConverterOptions())) s        

    let toString (oa:OntologyAnnotation) = 
        encoder (ConverterOptions()) oa
        |> Encode.toString 2
    
    /// exports in json-ld format
    let toStringLD (oa:OntologyAnnotation) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) oa
        |> Encode.toString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (oa:OntologyAnnotation) = 
    //    File.WriteAllText(path,toString oa)
