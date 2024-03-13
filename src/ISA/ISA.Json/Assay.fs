namespace ARCtrl.ISA.Json

open Thoth.Json.Core

open ARCtrl.ISA
open System.IO

module AssayMaterials = 

    let encoder (options : ConverterOptions) (oa : AssayMaterials) = 
        [
            GEncode.tryIncludeList "samples" (Sample.encoder options) (oa.Samples)
            GEncode.tryIncludeList "otherMaterials" (Material.encoder options) (oa.OtherMaterials)
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
                  | Some n -> n.Replace(" ","_")//.Remove(0,1 + (max (n.LastIndexOf('/')) (n.LastIndexOf('\\'))))
                  | None -> "#EmptyAssay"

    let encoder (options : ConverterOptions) (studyName:string Option) (oa : Assay) = 
        let assayName =
                try 
                    match (oa).FileName with
                    | Some fn -> Some (Identifier.Assay.identifierFromFileName fn)
                    | None -> None
                with 
                    | Failure(msg) -> None
        [
            if options.SetID then 
                "@id", Encode.string (oa |> genID)
            else 
                GEncode.tryInclude "@id" Encode.string (oa.ID)
            if options.IsJsonLD then 
                "@type", (Encode.list [ Encode.string "Assay"])
            GEncode.tryInclude "filename" Encode.string (oa.FileName)
            GEncode.tryInclude "measurementType" (OntologyAnnotation.encoder options) (oa.MeasurementType)
            GEncode.tryInclude "technologyType" (OntologyAnnotation.encoder options) (oa.TechnologyType)
            GEncode.tryInclude "technologyPlatform" Encode.string (oa.TechnologyPlatform)
            GEncode.tryIncludeList "dataFiles" (Data.encoder options) (oa.DataFiles)
            if not options.IsJsonLD then
                GEncode.tryInclude "materials" (AssayMaterials.encoder options) oa.Materials
                GEncode.tryIncludeList "characteristicCategories" (MaterialAttribute.encoder options) (oa.CharacteristicCategories)
                GEncode.tryIncludeList "unitCategories" (OntologyAnnotation.encoder options) (oa.UnitCategories)
            GEncode.tryIncludeList "processSequence" (Process.encoder options studyName assayName) (oa.ProcessSequence)
            GEncode.tryIncludeList "comments" (Comment.encoder options) (oa.Comments)
            if options.IsJsonLD then
                "@context", ROCrateContext.Assay.context_jsonvalue
        ]
        |> GEncode.choose
        |> Encode.object

    let allowedFields = ["@id";"filename";"measurementType";"technologyType";"technologyPlatform";"dataFiles";"materials";"characteristicCategories";"unitCategories";"processSequence";"comments";"@type"; "@context"]

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
    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s

    let toJsonString (p:Assay) = 
        encoder (ConverterOptions()) None p
        |> GEncode.toJsonString 2

    /// exports in json-ld format
    let toJsonldString (a:Assay) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) None a
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (a:Assay) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) None a
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (p:Assay) = 
    //    File.WriteAllText(path,toString p)