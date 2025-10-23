namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open ARCtrl.Helper
open Conversion

module Run =

    let rec encoder (run: ArcRun) =
        [
            "Identifier", Encode.string run.Identifier |> Some
            Encode.tryInclude "Title" Encode.string run.Title
            Encode.tryInclude "Description" Encode.string run.Description
            Encode.tryInclude "MeasurementType" OntologyAnnotation.encoder run.MeasurementType
            Encode.tryInclude "TechnologyType" OntologyAnnotation.encoder run.TechnologyType
            Encode.tryInclude "TechnologyPlatform" OntologyAnnotation.encoder run.TechnologyPlatform
            Encode.tryInclude "DataMap" DataMap.encoder run.DataMap
            Encode.tryIncludeSeq "WorkflowIdentifiers" Encode.string run.WorkflowIdentifiers
            Encode.tryIncludeSeq "Tables" ArcTable.encoder run.Tables
            Encode.tryIncludeSeq "Performers" Person.encoder run.Performers
            Encode.tryIncludeSeq "Comments" Comment.encoder run.Comments
        ]
        |> Encode.choose
        |> Encode.object

    let decoder: Decoder<ArcRun> =
        Decode.object (fun get ->
            ArcRun.create(
                get.Required.Field("Identifier") Decode.string,
                ?title = get.Optional.Field "Title" Decode.string,
                ?description = get.Optional.Field "Description" Decode.string,
                ?measurementType = get.Optional.Field "MeasurementType" OntologyAnnotation.decoder,
                ?technologyType = get.Optional.Field "TechnologyType" OntologyAnnotation.decoder,
                ?technologyPlatform = get.Optional.Field "TechnologyPlatform" OntologyAnnotation.decoder,
                ?workflowIdentifiers = get.Optional.Field "WorkflowIdentifiers" (Decode.resizeArray Decode.string),
                ?tables = get.Optional.Field "Tables" (Decode.resizeArray ArcTable.decoder),
                ?datamap = get.Optional.Field "DataMap" DataMap.decoder,
                ?performers = get.Optional.Field "Performers" (Decode.resizeArray Person.decoder),
                ?comments = get.Optional.Field "Comments" (Decode.resizeArray Comment.decoder)
            ) 
        )

    open StringTable
    open OATable
    open CellTable

    let encoderCompressed (stringTable : StringTableMap) (oaTable : OATableMap) (cellTable : CellTableMap) (run:ArcRun) =
        [
            "Identifier", Encode.string run.Identifier |> Some
            Encode.tryInclude "Title" Encode.string run.Title
            Encode.tryInclude "Description" Encode.string run.Description
            Encode.tryInclude "MeasurementType" OntologyAnnotation.encoder run.MeasurementType
            Encode.tryInclude "TechnologyType" OntologyAnnotation.encoder run.TechnologyType
            Encode.tryInclude "TechnologyPlatform" OntologyAnnotation.encoder run.TechnologyPlatform
            Encode.tryInclude "DataMap" (DataMap.encoderCompressed stringTable oaTable cellTable) run.DataMap
            Encode.tryIncludeSeq "WorkflowIdentifiers" Encode.string run.WorkflowIdentifiers
            Encode.tryIncludeSeq "Tables" (ArcTable.encoderCompressed stringTable oaTable cellTable) run.Tables
            Encode.tryIncludeSeq "Performers" Person.encoder run.Performers
            Encode.tryIncludeSeq "Comments" Comment.encoder run.Comments
        ]
        |> Encode.choose
        |> Encode.object

    let decoderCompressed (stringTable : StringTableArray) (oaTable : OATableArray) (cellTable : CellTableArray): Decoder<ArcRun> =
        Decode.object (fun get ->
            ArcRun.create(
                get.Required.Field("Identifier") Decode.string,
                ?title = get.Optional.Field "Title" Decode.string,
                ?description = get.Optional.Field "Description" Decode.string,
                ?measurementType = get.Optional.Field "MeasurementType" OntologyAnnotation.decoder,
                ?technologyType = get.Optional.Field "TechnologyType" OntologyAnnotation.decoder,
                ?technologyPlatform = get.Optional.Field "TechnologyPlatform" OntologyAnnotation.decoder,
                ?workflowIdentifiers = get.Optional.Field "WorkflowIdentifiers" (Decode.resizeArray Decode.string),
                ?tables = get.Optional.Field "Tables" (Decode.resizeArray (ArcTable.decoderCompressed stringTable oaTable cellTable)),
                ?datamap = get.Optional.Field "DataMap" (DataMap.decoderCompressed stringTable oaTable cellTable),
                ?performers = get.Optional.Field "Performers" (Decode.resizeArray Person.decoder),
                ?comments = get.Optional.Field "Comments" (Decode.resizeArray Comment.decoder)
            ) 
        )
