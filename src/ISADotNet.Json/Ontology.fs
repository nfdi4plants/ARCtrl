namespace ISADotNet.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ISADotNet
open System.IO
open GEncode

module AnnotationValue =

    let encoder (options : ConverterOptions) (value : obj) = 
        match value with
        | :? AnnotationValue as AnnotationValue.Float f -> 
            Encode.float f
        | :? AnnotationValue as AnnotationValue.Int i -> 
            Encode.int i
        | :? AnnotationValue as AnnotationValue.Text s -> 
            Encode.string s

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

//module OntologySourceReference =

//    let fromString (s:string) = 
//        JsonSerializer.Deserialize<OntologySourceReference>(s,JsonExtensions.options)

//    let toString (oa:OntologySourceReference) = 
//        JsonSerializer.Serialize<OntologySourceReference>(oa,JsonExtensions.options)

//    let fromFile (path : string) = 
//        File.ReadAllText path 
//        |> fromString

//    let toFile (path : string) (oa:OntologySourceReference) = 
//        File.WriteAllText(path,toString oa)

module OntologyAnnotation =  

    let encoder (options : ConverterOptions) (oa : obj) = 
        [
            tryInclude "@id" GEncode.string (oa |> tryGetPropertyValue "ID")
            tryInclude "name" (AnnotationValue.encoder options) (oa |> tryGetPropertyValue "Name")
            tryInclude "termSource" GEncode.string (oa |> tryGetPropertyValue "TermSourceREF")
            tryInclude "termAccession" GEncode.string (oa |> tryGetPropertyValue "TermAccessionNumber")
            //tryInclude "comments" GEncode.string (oa |> tryGetPropertyValue "Comments")
        ]
        |> GEncode.choose
        |> Encode.object

    let decoder (options : ConverterOptions) : Decoder<OntologyAnnotation> =
        Decode.object (fun get ->
            {
                ID = get.Optional.Field "@id" GDecode.uri
                Name = get.Optional.Field "name" (AnnotationValue.decoder options)
                TermSourceREF = get.Optional.Field "termSource" Decode.string
                TermAccessionNumber = get.Optional.Field "termAccession" Decode.string
                Comments = None // get.Optional.Field "comments" (Comment.decoder options)

            }
        )

    let fromString (s:string) = 
        GDecode.fromString (decoder (ConverterOptions())) s        

    let toString (oa:OntologyAnnotation) = 
        encoder (ConverterOptions()) oa
        |> Encode.toString 2

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (oa:OntologyAnnotation) = 
        File.WriteAllText(path,toString oa)