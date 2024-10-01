namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open ARCtrl.Helper
open Conversion

module Assay = 
    
    let encoder (assay:ArcAssay) = 
        [ 
            "Identifier", Encode.string assay.Identifier |> Some
            Encode.tryInclude "MeasurementType" OntologyAnnotation.encoder assay.MeasurementType
            Encode.tryInclude "TechnologyType" OntologyAnnotation.encoder assay.TechnologyType
            Encode.tryInclude "TechnologyPlatform" OntologyAnnotation.encoder assay.TechnologyPlatform
            Encode.tryInclude "DataMap" DataMap.encoder assay.DataMap
            Encode.tryIncludeSeq "Tables" ArcTable.encoder assay.Tables
            Encode.tryIncludeSeq "Performers" Person.encoder assay.Performers 
            Encode.tryIncludeSeq "Comments" Comment.encoder assay.Comments 
        ]
        |> Encode.choose
        |> Encode.object
  
    let decoder: Decoder<ArcAssay> =
        Decode.object (fun get ->
            ArcAssay.create(
                get.Required.Field("Identifier") Decode.string,
                ?measurementType = get.Optional.Field "MeasurementType" OntologyAnnotation.decoder,
                ?technologyType = get.Optional.Field "TechnologyType" OntologyAnnotation.decoder,
                ?technologyPlatform = get.Optional.Field "TechnologyPlatform" OntologyAnnotation.decoder,
                ?tables = get.Optional.Field "Tables" (Decode.resizeArray ArcTable.decoder),
                ?datamap = get.Optional.Field "DataMap" DataMap.decoder,
                ?performers = get.Optional.Field "Performers" (Decode.resizeArray Person.decoder),
                ?comments = get.Optional.Field "Comments" (Decode.resizeArray Comment.decoder)
            ) 
        )

    open StringTable
    open OATable
    open CellTable

    let encoderCompressed (stringTable : StringTableMap) (oaTable : OATableMap) (cellTable : CellTableMap) (assay:ArcAssay) =
        [ 
            "Identifier", Encode.string assay.Identifier |> Some
            Encode.tryInclude "MeasurementType" OntologyAnnotation.encoder assay.MeasurementType
            Encode.tryInclude "TechnologyType" OntologyAnnotation.encoder assay.TechnologyType
            Encode.tryInclude "TechnologyPlatform" OntologyAnnotation.encoder assay.TechnologyPlatform
            Encode.tryIncludeSeq "Tables" (ArcTable.encoderCompressed stringTable oaTable cellTable) assay.Tables
            Encode.tryInclude "DataMap" (DataMap.encoderCompressed stringTable oaTable cellTable) assay.DataMap
            Encode.tryIncludeSeq "Performers" Person.encoder assay.Performers 
            Encode.tryIncludeSeq "Comments" Comment.encoder assay.Comments 
        ]
        |> Encode.choose
        |> Encode.object

    let decoderCompressed (stringTable : StringTableArray) (oaTable : OATableArray) (cellTable : CellTableArray): Decoder<ArcAssay> =
        Decode.object (fun get ->
            ArcAssay.create(
                get.Required.Field "Identifier" Decode.string,
                ?measurementType = get.Optional.Field "MeasurementType" OntologyAnnotation.decoder,
                ?technologyType = get.Optional.Field "TechnologyType" OntologyAnnotation.decoder,
                ?technologyPlatform = get.Optional.Field "TechnologyPlatform" OntologyAnnotation.decoder,
                ?tables = get.Optional.Field "Tables" (Decode.resizeArray (ArcTable.decoderCompressed stringTable oaTable cellTable)),
                ?datamap = get.Optional.Field "DataMap" (DataMap.decoderCompressed stringTable oaTable cellTable),
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
                "@id", Encode.string (a |> genID) |> Some
                "@type", (Encode.list [ Encode.string "Assay"]) |> Some
                "additionalType", Encode.string "Assay" |> Some
                "identifier", Encode.string a.Identifier |> Some
                "filename", Encode.string fileName |> Some
                Encode.tryInclude "measurementType" OntologyAnnotation.ROCrate.encoderPropertyValue a.MeasurementType
                Encode.tryInclude "technologyType" OntologyAnnotation.ROCrate.encoderDefinedTerm a.TechnologyType
                Encode.tryInclude "technologyPlatform" OntologyAnnotation.ROCrate.encoderDefinedTerm a.TechnologyPlatform
                Encode.tryIncludeSeq "performers" Person.ROCrate.encoder a.Performers
                Encode.tryIncludeList "dataFiles" Data.ROCrate.encoder dataFiles
                Encode.tryIncludeList "processSequence" (Process.ROCrate.encoder studyName (Some a.Identifier)) processes
                Encode.tryIncludeSeq "comments" Comment.ROCrate.encoder a.Comments
                "@context", ROCrateContext.Assay.context_jsonvalue |> Some
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

        let encoder (studyName:string Option) idMap (a : ArcAssay) = 
            let f (a : ArcAssay) =
                let fileName = Identifier.Assay.fileNameFromIdentifier a.Identifier
                let processes = a.GetProcesses()
                let encodedUnits = 
                    ProcessSequence.getUnits processes
                    |> Encode.tryIncludeList "unitCategories" (OntologyAnnotation.ISAJson.encoder idMap)
                let encodedCharacteristics = 
                    ProcessSequence.getCharacteristics processes
                    |> Encode.tryIncludeList "characteristicCategories" (MaterialAttribute.ISAJson.encoder idMap) 
                let encodedMaterials = 
                    Encode.tryInclude "materials" (AssayMaterials.ISAJson.encoder idMap) (Option.fromValueWithDefault [] processes)
                let encocedDataFiles = 
                    ProcessSequence.getData processes
                    |> Encode.tryIncludeList "dataFiles" (Data.ISAJson.encoder idMap) 
                let units = ProcessSequence.getUnits processes
                [
                    "filename", Encode.string fileName |> Some
                    Encode.tryInclude "@id" Encode.string (ROCrate.genID a |> Some)
                    Encode.tryInclude "measurementType" (OntologyAnnotation.ISAJson.encoder idMap) a.MeasurementType
                    Encode.tryInclude "technologyType" (OntologyAnnotation.ISAJson.encoder idMap) a.TechnologyType
                    Encode.tryInclude "technologyPlatform" Encode.string (a.TechnologyPlatform |> Option.map Conversion.JsonTypes.composeTechnologyPlatform)
                    encocedDataFiles
                    encodedMaterials
                    encodedCharacteristics
                    encodedUnits
                    Encode.tryIncludeList "processSequence" (Process.ISAJson.encoder studyName (Some a.Identifier) idMap) processes
                    Encode.tryIncludeSeq "comments" (Comment.ISAJson.encoder idMap) a.Comments
                ]
                |> Encode.choose
                |> Encode.object
            match idMap with
            | None -> f a
            | Some idMap -> IDTable.encode ROCrate.genID f a idMap

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