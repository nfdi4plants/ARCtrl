namespace ARCtrl.ISA.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ARCtrl.ISA
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
            if options.SetID then "@id",  GEncode.toJsonString (osr :?> OntologySourceReference |> genID)
            if options.IncludeType then "@type",  GEncode.toJsonString "OntologySourceReference"
            GEncode.tryInclude "description"  GEncode.toJsonString (osr |> GEncode.tryGetPropertyValue "Description")
            GEncode.tryInclude "file"  GEncode.toJsonString (osr |> GEncode.tryGetPropertyValue "File")
            GEncode.tryInclude "name"  GEncode.toJsonString (osr |> GEncode.tryGetPropertyValue "Name")
            GEncode.tryInclude "version"  GEncode.toJsonString (osr |> GEncode.tryGetPropertyValue "Version")
            GEncode.tryInclude "comments" (Comment.encoder options) (osr |> GEncode.tryGetPropertyValue "Comments")
            if options.IncludeContext then ("@context",Newtonsoft.Json.Linq.JObject.Parse(ROCrateContext.OntologySourceReference.context).GetValue("@context"))
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
        |> Encode.toString 2

    /// exports in json-ld format
    let toJsonldString (oa:OntologySourceReference) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) oa
        |> Encode.toString 2
    let toJsonldStringWithContext (a:OntologySourceReference) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true,IncludeContext=true)) a
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
                  | Some ta -> URI.toString ta
                  | None -> match o.TermSourceREF with
                            | Some r -> "#" + r.Replace(" ","_")
                            | None -> match o.TryNameText with
                                        | Some n -> "#UserTerm_" + n .Replace(" ","_")
                                        | None -> "#DummyOntologyAnnotation"

    let encoder (options : ConverterOptions) (oa : obj) = 
        [
            if options.SetID then "@id", GEncode.toJsonString (oa :?> OntologyAnnotation |> genID)
                else GEncode.tryInclude "@id" GEncode.toJsonString (oa |> GEncode.tryGetPropertyValue "ID")
            if options.IncludeType then "@type", GEncode.toJsonString "OntologyAnnotation"
            GEncode.tryInclude "annotationValue" (AnnotationValue.encoder options) (oa |> GEncode.tryGetPropertyValue "Name")
            GEncode.tryInclude "termSource" GEncode.toJsonString (oa |> GEncode.tryGetPropertyValue "TermSourceREF")
            GEncode.tryInclude "termAccession" GEncode.toJsonString (oa |> GEncode.tryGetPropertyValue "TermAccessionNumber")
            GEncode.tryInclude "comments" (Comment.encoder options) (oa |> GEncode.tryGetPropertyValue "Comments")
            if options.IncludeContext then ("@context",Newtonsoft.Json.Linq.JObject.Parse(ROCrateContext.OntologyAnnotation.context).GetValue("@context"))
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

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s        

    let toJsonString (oa:OntologyAnnotation) = 
        encoder (ConverterOptions()) oa
        |> Encode.toString 2
    
    /// exports in json-ld format
    let toJsonldString (oa:OntologyAnnotation) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) oa
        |> Encode.toString 2
    let toJsonldStringWithContext (a:OntologyAnnotation) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true,IncludeContext=true)) a
        |> Encode.toString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (oa:OntologyAnnotation) = 
    //    File.WriteAllText(path,toString oa)
