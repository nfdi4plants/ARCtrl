namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open ARCtrl.Helper
open Conversion

module Workflow =

    let rec encoder (workflow: ArcWorkflow) =
        [
            "Identifier", Encode.string workflow.Identifier |> Some
            Encode.tryInclude "WorkflowType" OntologyAnnotation.encoder workflow.WorkflowType
            Encode.tryInclude "Title" Encode.string workflow.Title
            Encode.tryInclude "URI" Encode.string workflow.URI
            Encode.tryInclude "Description" Encode.string workflow.Description
            Encode.tryInclude "Version" Encode.string workflow.Version
            Encode.tryInclude "DataMap" DataMap.encoder workflow.DataMap
            Encode.tryIncludeSeq "SubWorkflowIdentifiers" Encode.string workflow.SubWorkflowIdentifiers
            Encode.tryIncludeSeq "Parameters" ProtocolParameter.encoder workflow.Parameters
            Encode.tryIncludeSeq "Components" Component.encoder workflow.Components
            Encode.tryIncludeSeq "Contacts" Person.encoder workflow.Contacts
            Encode.tryIncludeSeq "Comments" Comment.encoder workflow.Comments
        ]
        |> Encode.choose
        |> Encode.object

    let decoder: Decoder<ArcWorkflow> =
        Decode.object (fun get ->
            ArcWorkflow.create(
                get.Required.Field("Identifier") Decode.string,
                ?title = get.Optional.Field "Title" Decode.string,
                ?description = get.Optional.Field "Description" Decode.string,
                ?workflowType = get.Optional.Field "WorkflowType" OntologyAnnotation.decoder,
                ?uri = get.Optional.Field "URI" Decode.string,
                ?version = get.Optional.Field "Version" Decode.string,
                ?subWorkflowIdentifiers = get.Optional.Field "SubWorkflowIdentifiers" (Decode.resizeArray Decode.string),
                ?parameters = get.Optional.Field "Parameters" (Decode.resizeArray ProtocolParameter.decoder),
                ?components = get.Optional.Field "Components" (Decode.resizeArray Component.decoder),
                ?datamap = get.Optional.Field "DataMap" DataMap.decoder,
                ?contacts = get.Optional.Field "Contacts" (Decode.resizeArray Person.decoder),
                ?comments = get.Optional.Field "Comments" (Decode.resizeArray Comment.decoder)
            )
        )

    open StringTable
    open OATable
    open CellTable

    let encoderCompressed (stringTable : StringTableMap) (oaTable : OATableMap) (cellTable : CellTableMap) (workflow:ArcWorkflow) =
        [ 
            "Identifier", Encode.string workflow.Identifier |> Some
            Encode.tryInclude "WorkflowType" OntologyAnnotation.encoder workflow.WorkflowType
            Encode.tryInclude "Title" Encode.string workflow.Title
            Encode.tryInclude "URI" Encode.string workflow.URI
            Encode.tryInclude "Description" Encode.string workflow.Description
            Encode.tryInclude "Version" Encode.string workflow.Version
            Encode.tryInclude "DataMap" (DataMap.encoderCompressed stringTable oaTable cellTable) workflow.DataMap
            Encode.tryIncludeSeq "SubWorkflowIdentifiers" Encode.string workflow.SubWorkflowIdentifiers
            Encode.tryIncludeSeq "Parameters" ProtocolParameter.encoder workflow.Parameters
            Encode.tryIncludeSeq "Components" Component.encoder workflow.Components
            Encode.tryIncludeSeq "Contacts" Person.encoder workflow.Contacts
            Encode.tryIncludeSeq "Comments" Comment.encoder workflow.Comments
        ]
        |> Encode.choose
        |> Encode.object

    let decoderCompressed (stringTable : StringTableArray) (oaTable : OATableArray) (cellTable : CellTableArray): Decoder<ArcWorkflow> =
        Decode.object (fun get ->
            ArcWorkflow.create(
                get.Required.Field("Identifier") Decode.string,
                ?title = get.Optional.Field "Title" Decode.string,
                ?description = get.Optional.Field "Description" Decode.string,
                ?workflowType = get.Optional.Field "WorkflowType" OntologyAnnotation.decoder,
                ?uri = get.Optional.Field "URI" Decode.string,
                ?version = get.Optional.Field "Version" Decode.string,
                ?subWorkflowIdentifiers = get.Optional.Field "SubWorkflowIdentifiers" (Decode.resizeArray Decode.string),
                ?parameters = get.Optional.Field "Parameters" (Decode.resizeArray ProtocolParameter.decoder),
                ?components = get.Optional.Field "Components" (Decode.resizeArray Component.decoder),
                ?datamap = get.Optional.Field "DataMap" (DataMap.decoderCompressed stringTable oaTable cellTable),
                ?contacts = get.Optional.Field "Contacts" (Decode.resizeArray Person.decoder),
                ?comments = get.Optional.Field "Comments" (Decode.resizeArray Comment.decoder)
            )
        )
