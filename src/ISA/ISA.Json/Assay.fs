namespace ARCtrl.ISA.Json

open Thoth.Json.Core

open ARCtrl.ISA
open System.IO

module AssayMaterials = 

    let encoder (options : ConverterOptions) (oa : obj) = 
        [
            GEncode.tryInclude "samples" (Sample.encoder options) (oa |> GEncode.tryGetPropertyValue "Samples")
            GEncode.tryInclude "otherMaterials" (Material.encoder options) (oa |> GEncode.tryGetPropertyValue "OtherMaterials")
        ]
        |> GEncode.choose
        |> Encode.object
    
    let allowedFields = ["samples";"otherMaterials"]

    let decoder (options : ConverterOptions) : Decoder<AssayMaterials> =
        GDecode.object allowedFields (fun get ->
            {
                Samples = get.Optional.Field "samples" (Decode.list (Sample.decoder options))
                OtherMaterials = get.Optional.Field "otherMaterials" (Decode.list (Material.decoder options))
            }
        )

module Assay = 
    
    let genID (a:Assay) : string = 
        match a.ID with
        | Some id -> URI.toString id
        | None -> match a.FileName with
                  | Some n -> n
                  | None -> "#EmptyAssay"

    let encoder (options : ConverterOptions) (oa : obj) = 
        [
            if options.SetID then "@id", GEncode.includeString (oa :?> Assay |> genID)
                else GEncode.tryInclude "@id" GEncode.includeString (oa |> GEncode.tryGetPropertyValue "ID")
            if options.IncludeType then "@type", GEncode.includeString "Assay"
            GEncode.tryInclude "filename" GEncode.includeString (oa |> GEncode.tryGetPropertyValue "FileName")
            GEncode.tryInclude "measurementType" (OntologyAnnotation.encoder options) (oa |> GEncode.tryGetPropertyValue "MeasurementType")
            GEncode.tryInclude "technologyType" (OntologyAnnotation.encoder options) (oa |> GEncode.tryGetPropertyValue "TechnologyType")
            GEncode.tryInclude "technologyPlatform" GEncode.includeString (oa |> GEncode.tryGetPropertyValue "TechnologyPlatform")
            GEncode.tryInclude "dataFiles" (Data.encoder options) (oa |> GEncode.tryGetPropertyValue "DataFiles")
            GEncode.tryInclude "materials" (AssayMaterials.encoder options) (oa |> GEncode.tryGetPropertyValue "Materials")
            GEncode.tryInclude "characteristicCategories" (MaterialAttribute.encoder options) (oa |> GEncode.tryGetPropertyValue "CharacteristicCategories")
            GEncode.tryInclude "unitCategories" (OntologyAnnotation.encoder options) (oa |> GEncode.tryGetPropertyValue "UnitCategories")
            GEncode.tryInclude "processSequence" (Process.encoder options) (oa |> GEncode.tryGetPropertyValue "ProcessSequence")
            GEncode.tryInclude "comments" (Comment.encoder options) (oa |> GEncode.tryGetPropertyValue "Comments")
        ]
        |> GEncode.choose
        |> Encode.object

    let allowedFields = ["@id";"filename";"measurementType";"technologyType";"technologyPlatform";"dataFiles";"materials";"characteristicCategories";"unitCategories";"processSequence";"comments";"@type"]

    let decoder (options : ConverterOptions) : Decoder<Assay> =
        GDecode.object allowedFields (fun get ->
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

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s

    let toJsonString (p:Assay) = 
        encoder (ConverterOptions()) p
        |> GEncode.toJsonString 2

    /// exports in json-ld format
    let toStringLD (a:Assay) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) a
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (p:Assay) = 
    //    File.WriteAllText(path,toString p)
