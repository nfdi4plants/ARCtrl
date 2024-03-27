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
            Encode.tryIncludeSeq "authors" Person.encoder template.Authors 
            Encode.tryIncludeSeq "endpoint_repositories" OntologyAnnotation.encoder template.EndpointRepositories 
            Encode.tryIncludeSeq "tags" OntologyAnnotation.encoder template.Tags 
            "last_updated", Encode.datetime template.LastUpdated
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
                #if FABLE_COMPILER_PYTHON
                ?lastUpdated = get.Required.Field "last_updated" Decode.datetimeLocal // Currently not supported in Thoth.Json.Core for python
                #else
                lastUpdated = get.Required.Field "last_updated" Decode.datetimeUtc
                #endif
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
            Encode.tryIncludeSeq "authors" Person.encoder template.Authors 
            Encode.tryIncludeSeq "endpoint_repositories" OntologyAnnotation.encoder template.EndpointRepositories 
            Encode.tryIncludeSeq "tags" OntologyAnnotation.encoder template.Tags 
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
                ?lastUpdated = get.Required.Field "last_updated" Decode.datetimeLocal // Currently not supported in Thoth.Json.Core for python
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
        Decode.dict Template.decoder
        
    let fromJsonString (jsonString: string) =
        try Decode.fromJsonString decoder jsonString with
        | exn         -> failwithf "Error. Given json string cannot be parsed to Templates map: %A" exn

    let toJsonString (spaces: int) (templates: Template []) =
        Encode.toJsonString spaces (encoder templates)

[<AutoOpen>]
module TemplateExtension =

    type Template with

        static member fromJsonString (jsonString: string) =
            try Decode.fromJsonString Template.decoder jsonString with
            | exn     -> failwithf "Error. Given json string cannot be parsed to Template: %A" exn

        static member toJsonString(?spaces: int) =
            fun (template:Template) ->
                Encode.toJsonString (Encode.defaultSpaces spaces) (Template.encoder template)

        static member fromCompressedJsonString (s: string) =
            try Decode.fromJsonString (Compression.decode Template.decoderCompressed) s with
            | e -> failwithf "Error. Unable to parse json string to ArcStudy: %s" e.Message

        static member toCompressedJsonString(?spaces) =
            fun (obj:Template) ->
                let spaces = defaultArg spaces 0
                Encode.toJsonString spaces (Compression.encode Template.encoderCompressed obj)
