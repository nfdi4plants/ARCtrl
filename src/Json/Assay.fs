namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open ARCtrl.Helper
open Conversion

module Assay = 
    
    let encoder (assay:ArcAssay) = 
        Encode.object [ 
            "Identifier", Encode.string assay.Identifier
            Encode.tryInclude "MeasurementType" OntologyAnnotation.encoder assay.MeasurementType
            Encode.tryInclude "TechnologyType" OntologyAnnotation.encoder assay.TechnologyType
            Encode.tryInclude "TechnologyPlatform" OntologyAnnotation.encoder assay.TechnologyPlatform
            Encode.tryIncludeSeq "Tables" ArcTable.encoder assay.Tables
            Encode.tryIncludeSeq "Performers" Person.encoder assay.Performers 
            Encode.tryIncludeSeq "Comments" Comment.encoder assay.Comments 
        ]
  
    let decoder: Decoder<ArcAssay> =
        Decode.object (fun get ->
            ArcAssay.create(
                get.Required.Field("Identifier") Decode.string,
                ?measurementType = get.Optional.Field "MeasurementType" OntologyAnnotation.decoder,
                ?technologyType = get.Optional.Field "TechnologyType" OntologyAnnotation.decoder,
                ?technologyPlatform = get.Optional.Field "TechnologyPlatform" OntologyAnnotation.decoder,
                ?tables = get.Optional.Field "Tables" (Decode.resizeArray ArcTable.decoder),
                ?performers = get.Optional.Field "Performers" (Decode.resizeArray Person.decoder),
                ?comments = get.Optional.Field "Comments" (Decode.resizeArray Comment.decoder)
            ) 
        )

    open StringTable
    open OATable
    open CellTable

    let encoderCompressed (stringTable : StringTableMap) (oaTable : OATableMap) (cellTable : CellTableMap) (assay:ArcAssay) =
        Encode.object [ 
            "Identifier", Encode.string assay.Identifier
            Encode.tryInclude "MeasurementType" OntologyAnnotation.encoder assay.MeasurementType
            Encode.tryInclude "TechnologyType" OntologyAnnotation.encoder assay.TechnologyType
            Encode.tryInclude "TechnologyPlatform" OntologyAnnotation.encoder assay.TechnologyPlatform
            Encode.tryIncludeSeq "Tables" (ArcTable.encoderCompressed stringTable oaTable cellTable) assay.Tables
            Encode.tryIncludeSeq "Performers" Person.encoder assay.Performers 
            Encode.tryIncludeSeq "Performers" Comment.encoder assay.Comments 
        ]

    let decoderCompressed (stringTable : StringTableArray) (oaTable : OATableArray) (cellTable : CellTableArray): Decoder<ArcAssay> =
        Decode.object (fun get ->
            ArcAssay.create(
                get.Required.Field("Identifier") Decode.string,
                ?measurementType = get.Optional.Field "MeasurementType" OntologyAnnotation.decoder,
                ?technologyType = get.Optional.Field "TechnologyType" OntologyAnnotation.decoder,
                ?technologyPlatform = get.Optional.Field "TechnologyPlatform" OntologyAnnotation.decoder,
                ?tables = get.Optional.Field("Tables") (Decode.resizeArray (ArcTable.decoderCompressed stringTable oaTable cellTable)),
                ?performers = get.Optional.Field "Performers" (Decode.resizeArray Person.decoder),
                ?comments = get.Optional.Field "Comments" (Decode.resizeArray Comment.decoder)
            ) 
        )

    module ROCrate =
        
        let genID (a:ArcAssay) : string = 
            match a.Identifier with
            | "" -> "#EmptyAssay"
            | i -> 
                let identifier = i.Replace(" ","_")
                $"#assay/{identifier}"

        let encoder (studyName:string Option) (a : ArcAssay) = 
            let fileName = Identifier.Assay.fileNameFromIdentifier a.Identifier
            let processes = a.GetProcesses()
            let dataFiles = ProcessSequence.getData processes

            [
                "@id", Encode.string (a |> genID)
                "@type", (Encode.list [ Encode.string "Assay"])
                "additionalType", Encode.string "Assay"
                "identifier", Encode.string a.Identifier
                "filename", Encode.string fileName
                Encode.tryInclude "measurementType" OntologyAnnotation.ROCrate.encoderPropertyValue a.MeasurementType
                Encode.tryInclude "technologyType" OntologyAnnotation.ROCrate.encoderDefinedTerm a.TechnologyType
                Encode.tryInclude "technologyPlatform" OntologyAnnotation.ROCrate.encoderDefinedTerm a.TechnologyPlatform
                Encode.tryIncludeSeq "performers" Person.ROCrate.encoder a.Performers
                Encode.tryIncludeList "dataFiles" Data.ROCrate.encoder dataFiles
                Encode.tryIncludeList "processSequence" (Process.ROCrate.encoder studyName (Some a.Identifier)) processes
                Encode.tryIncludeSeq "comments" Comment.ROCrate.encoder a.Comments
                "@context", ROCrateContext.Assay.context_jsonvalue
            ]
            |> Encode.choose
            |> Encode.object

        let decoder : Decoder<ArcAssay> =
            Decode.object (fun get ->               
                let identifier = 
                    get.Optional.Field "identifier" Decode.string
                    |> Option.defaultValue (Identifier.createMissingIdentifier())
                let tables = 
                    get.Optional.Field "processSequence" (Decode.list Process.ROCrate.decoder)
                    |> Option.map (ArcTables.fromProcesses >> (fun a -> a.Tables))
                ArcAssay(
                    identifier,
                    ?measurementType = get.Optional.Field "measurementType" OntologyAnnotation.ROCrate.decoderPropertyValue,
                    ?technologyType = get.Optional.Field "technologyType" OntologyAnnotation.ROCrate.decoderDefinedTerm,
                    ?technologyPlatform = get.Optional.Field "technologyPlatform" OntologyAnnotation.ROCrate.decoderDefinedTerm,
                    ?tables = tables,
                    ?performers = get.Optional.Field "performers" (Decode.resizeArray Person.ROCrate.decoder),
                    ?comments = get.Optional.Field "comments" (Decode.resizeArray Comment.ROCrate.decoder)
                )
            )
            
    module ISAJson = 

        let encoder (a : ArcAssay) = 
            let fileName = Identifier.Assay.fileNameFromIdentifier a.Identifier
            let processes = a.GetProcesses()
            let dataFiles = ProcessSequence.getData processes
            let characteristics = ProcessSequence.getCharacteristics processes
            let units = ProcessSequence.getUnits processes
            [
                "filename", Encode.string fileName
                Encode.tryInclude "measurementType" OntologyAnnotation.ISAJson.encoder a.MeasurementType
                Encode.tryInclude "technologyType" OntologyAnnotation.ISAJson.encoder a.TechnologyType
                Encode.tryInclude "technologyPlatform" Encode.string (a.TechnologyPlatform |> Option.map Conversion.JsonTypes.composeTechnologyPlatform)
                Encode.tryIncludeList "dataFiles" Data.ISAJson.encoder dataFiles
                Encode.tryInclude "materials" AssayMaterials.ISAJson.encoder (Option.fromValueWithDefault [] processes)
                Encode.tryIncludeList "characteristicCategories" MaterialAttribute.ISAJson.encoder characteristics
                Encode.tryIncludeList "unitCategories" OntologyAnnotation.ISAJson.encoder  units
                Encode.tryIncludeList "processSequence" Process.ISAJson.encoder processes
                Encode.tryIncludeSeq "comments" Comment.ISAJson.encoder a.Comments
            ]
            |> Encode.choose
            |> Encode.object

        let allowedFields = ["@id";"filename";"measurementType";"technologyType";"technologyPlatform";"dataFiles";"materials";"characteristicCategories";"unitCategories";"processSequence";"comments";"@type"; "@context"]

        let decoder : Decoder<ArcAssay> =
            Decode.objectNoAdditionalProperties allowedFields (fun get ->               
                let identifier = 
                    get.Optional.Field "filename" Decode.string
                    |> Option.bind Identifier.Assay.tryIdentifierFromFileName
                    |> Option.defaultValue (Identifier.createMissingIdentifier())
                let tables = 
                    get.Optional.Field "processSequence" (Decode.list Process.ISAJson.decoder)
                    |> Option.map (ArcTables.fromProcesses >> (fun a -> a.Tables))
                ArcAssay(
                    identifier,
                    ?measurementType = get.Optional.Field "measurementType" (OntologyAnnotation.ISAJson.decoder),
                    ?technologyType = get.Optional.Field "technologyType" (OntologyAnnotation.ISAJson.decoder),
                    ?technologyPlatform = get.Optional.Field "technologyPlatform" (Decode.map Conversion.JsonTypes.decomposeTechnologyPlatform Decode.string),
                    ?tables = tables,
                    ?comments = get.Optional.Field "comments" (Decode.resizeArray Comment.ISAJson.decoder)
                )
            )


[<AutoOpen>]
module AssayExtensions =

    open System.Collections.Generic

    type ArcAssay with
       
        static member fromJsonString (s:string)  = 
            Decode.fromJsonString Assay.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:ArcAssay) ->
                Assay.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        static member fromCompressedJsonString (s: string) =
            try Decode.fromJsonString (Compression.decode Assay.decoderCompressed) s with
            | e -> failwithf "Error. Unable to parse json string to ArcStudy: %s" e.Message

        static member toCompressedJsonString(?spaces) =
            fun (obj:ArcAssay) ->
                let spaces = defaultArg spaces 0
                Encode.toJsonString spaces (Compression.encode Assay.encoderCompressed obj)

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString Assay.ROCrate.decoder s

        /// exports in json-ld format
        static member toROCrateJsonString(studyName, ?spaces) =
            fun (obj: ArcAssay) ->
                Assay.ROCrate.encoder studyName obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        static member toISAJsonString(?spaces) =
            fun (obj:ArcAssay) ->
                Assay.ISAJson.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Assay.ISAJson.decoder s