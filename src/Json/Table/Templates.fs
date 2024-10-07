namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open ARCtrl.Helper
open Conversion

module Template =

    module Organisation =

        let encoder = (fun (org: Organisation) -> org.ToString()) >> Encode.string 

        let decoder = 
            Decode.string
            |> Decode.andThen (fun textValue ->
                Organisation.ofString textValue
                |> Decode.succeed
            )

    let encoder (template: Template) =
        Encode.object [
            "id", Encode.guid template.Id
            "table", ArcTable.encoder template.Table
            "name", Encode.string template.Name
            "description", Encode.string template.Description
            "organisation", Organisation.encoder template.Organisation
            "version", Encode.string template.Version
            "authors", (template.Authors |> Seq.map Person.encoder |> Encode.seq)
            "endpoint_repositories", (template.EndpointRepositories |> Seq.map OntologyAnnotation.encoder |> Encode.seq)
            "tags", (template.Tags |> Seq.map OntologyAnnotation.encoder |> Encode.seq)
            "last_updated", Encode.dateTime template.LastUpdated
        ]

    let decoder : Decoder<Template> =
        Decode.object(fun get ->
            Template.create(
                get.Required.Field "id" Decode.guid,
                get.Required.Field "table" ArcTable.decoder,
                get.Required.Field "name" Decode.string,
                get.Required.Field "description" Decode.string,
                get.Required.Field "organisation" Organisation.decoder,
                get.Required.Field "version" Decode.string,
                ?authors = get.Optional.Field "authors" (Decode.resizeArray Person.decoder),
                ?repos = get.Optional.Field "endpoint_repositories" (Decode.resizeArray OntologyAnnotation.decoder),
                ?tags = get.Optional.Field "tags" (Decode.resizeArray OntologyAnnotation.decoder),
                lastUpdated = get.Required.Field "last_updated" Decode.datetime
            )
        )

    let encoderCompressed stringTable oaTable cellTable (template: Template) =
        Encode.object [
            "id", Encode.guid template.Id 
            "table", ArcTable.encoderCompressed stringTable oaTable cellTable template.Table
            "name", Encode.string template.Name
            "description", Encode.string template.Description
            "organisation", Organisation.encoder template.Organisation
            "version", Encode.string template.Version
            "authors", (template.Authors |> Seq.map Person.encoder |> Encode.seq)
            "endpoint_repositories", (template.EndpointRepositories |> Seq.map OntologyAnnotation.encoder |> Encode.seq)
            "tags", (template.Tags |> Seq.map OntologyAnnotation.encoder |> Encode.seq)
            "last_updated", Encode.datetime template.LastUpdated
        ]

    let decoderCompressed stringTable oaTable cellTable : Decoder<Template> =
        Decode.object(fun get ->
            Template.create(
                get.Required.Field "id" Decode.guid,
                get.Required.Field "table" (ArcTable.decoderCompressed stringTable oaTable cellTable),
                get.Required.Field "name" Decode.string,
                get.Required.Field "description" Decode.string,
                get.Required.Field "organisation" Organisation.decoder,
                get.Required.Field "version" Decode.string,
                ?authors = get.Optional.Field "authors" (Decode.resizeArray Person.decoder),
                ?repos = get.Optional.Field "endpoint_repositories" (Decode.resizeArray OntologyAnnotation.decoder),
                ?tags = get.Optional.Field "tags" (Decode.resizeArray OntologyAnnotation.decoder),
                #if FABLE_COMPILER_PYTHON
                lastUpdated = get.Required.Field "last_updated" Decode.datetimeLocal // Currently not supported in Thoth.Json.Core for python
                #else
                lastUpdated = get.Required.Field "last_updated" Decode.datetimeUtc
                #endif
            )
        )

module Templates =

    let encoder (templates: Template []) =
        templates 
        |> Array.map (Template.encoder)
        |> Encode.array

    let decoder =
        Decode.array Template.decoder