namespace ISADotNet.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ISADotNet
open System.IO
open GEncode


module AssayMaterials = 

    let encoder (options : ConverterOptions) (oa : obj) = 
        [
            tryInclude "samples" (Sample.encoder options) (oa |> tryGetPropertyValue "Samples")
            tryInclude "otherMaterials" (Material.encoder options) (oa |> tryGetPropertyValue "OtherMaterials")
        ]
        |> GEncode.choose
        |> Encode.object
    
    let decoder (options : ConverterOptions) : Decoder<AssayMaterials> =
        Decode.object (fun get ->
            {
                Samples = get.Optional.Field "samples" (Decode.list (Sample.decoder options))
                OtherMaterials = get.Optional.Field "otherMaterials" (Decode.list (Material.decoder options))
            }
        )

module Assay = 
    
    let genID (a:Assay) = 
        match a.ID with
            | Some id -> URI.toString id
            | None -> match a.FileName with
                        | Some n -> n
                        | None -> "#EmptyAssay"

    let encoder (options : ConverterOptions) (oa : obj) = 
        [
            if options.SetID then "@id", GEncode.string (oa :?> Assay |> genID)
                else tryInclude "@id" GEncode.string (oa |> tryGetPropertyValue "ID")
            if options.IncludeType then "@type", GEncode.string "Assay"
            tryInclude "filename" GEncode.string (oa |> tryGetPropertyValue "FileName")
            tryInclude "measurementType" (OntologyAnnotation.encoder options) (oa |> tryGetPropertyValue "MeasurementType")
            tryInclude "technologyType" (OntologyAnnotation.encoder options) (oa |> tryGetPropertyValue "TechnologyType")
            tryInclude "technologyPlatform" GEncode.string (oa |> tryGetPropertyValue "TechnologyPlatform")
            tryInclude "dataFiles" (Data.encoder options) (oa |> tryGetPropertyValue "DataFiles")
            tryInclude "materials" (AssayMaterials.encoder options) (oa |> tryGetPropertyValue "Materials")
            tryInclude "characteristicCategories" (MaterialAttribute.encoder options) (oa |> tryGetPropertyValue "CharacteristicCategories")
            tryInclude "unitCategories" (OntologyAnnotation.encoder options) (oa |> tryGetPropertyValue "UnitCategories")
            tryInclude "processSequence" (Process.encoder options) (oa |> tryGetPropertyValue "ProcessSequence")
            tryInclude "comments" (Comment.encoder options) (oa |> tryGetPropertyValue "Comments")
        ]
        |> GEncode.choose
        |> Encode.object

    let decoder (options : ConverterOptions) : Decoder<Assay> =
        Decode.object (fun get ->
            {
                ID = get.Optional.Field "@id" GDecode.uri
                FileName = get.Optional.Field "filename" Decode.string
                MeasurementType = get.Optional.Field "measurementType" (OntologyAnnotation.decoder options)
                TechnologyType = get.Optional.Field "technologyType" (OntologyAnnotation.decoder options)
                TechnologyPlatform = get.Optional.Field "technologyPlatform" Decode.string
                DataFiles = get.Optional.Field "dataFiles" (Decode.list (Data.decoder options))
                Materials = get.Optional.Field "materials" (AssayMaterials.decoder options)
                CharacteristicCategories = get.Optional.Field "characteristicCategories" (Decode.list (MaterialAttribute.decoder options))
                UnitCategories = get.Optional.Field "unitCategories" (Decode.list (OntologyAnnotation.decoder options))
                ProcessSequence = get.Optional.Field "processSequence" (Decode.list (Process.decoder options))
                Comments = get.Optional.Field "comments" (Decode.list (Comment.decoder options))
            }
        )

    let fromString (s:string) = 
        GDecode.fromString (decoder (ConverterOptions())) s

    let toString (p:Assay) = 
        encoder (ConverterOptions()) p
        |> Encode.toString 2

    /// exports in json-ld format
    let toStringLD (a:Assay) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) a
        |> Encode.toString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (p:Assay) = 
    //    File.WriteAllText(path,toString p)
