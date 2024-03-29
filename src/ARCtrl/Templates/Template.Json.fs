﻿module ARCtrl.Template.Json

open ARCtrl.Template
open ARCtrl.ISA
open System
open ARCtrl

open Thoth.Json.Core
open ARCtrl.ISA.Json

//https://thoth-org.github.io/Thoth.Json/documentation/auto/extra-coders.html#ready-to-use-extra-coders

module Organisation =

    let encode = (fun (org: Organisation) -> org.ToString()) >> Encode.string 

    let decode = 
        Decode.string
        |> Decode.andThen (fun textValue ->
            Organisation.ofString textValue
            |> Decode.succeed
        )

module Template =

    

    let encode (template: Template) =
        let personEncoder = ARCtrl.ISA.Json.Person.encoder (ConverterOptions())
        let oaEncoder = ARCtrl.ISA.Json.OntologyAnnotation.encoder (ConverterOptions())
        Encode.object [
            "id", Encode.guid template.Id
            "table", ArcTable.encoder template.Table
            "name", Encode.string template.Name
            "description", Encode.string template.Description
            "organisation", Organisation.encode template.Organisation
            "version", Encode.string template.Version
            "authors", Encode.list [
                for a in template.Authors do yield personEncoder a
            ]
            "endpoint_repositories", Encode.list [
                for oa in template.EndpointRepositories do yield oaEncoder oa
            ]
            "tags", Encode.list [
                for oa in template.Tags do yield oaEncoder oa
            ]
            "last_updated", Encode.datetime template.LastUpdated
        ]

    let decode : Decoder<Template> =
        let personDecoder = ARCtrl.ISA.Json.Person.decoder (ConverterOptions())
        let oaDecoder = ARCtrl.ISA.Json.OntologyAnnotation.decoder (ConverterOptions())
        Decode.object(fun get ->
            Template.create(
                get.Required.Field "id" Decode.guid,
                get.Required.Field "table" ArcTable.decoder,
                get.Required.Field "name" Decode.string,
                get.Required.Field "description" Decode.string,
                get.Required.Field "organisation" Organisation.decode,
                get.Required.Field "version" Decode.string,
                get.Required.Field "authors" (Decode.array personDecoder),
                get.Required.Field "endpoint_repositories" (Decode.array oaDecoder),
                get.Required.Field "tags" (Decode.array oaDecoder),
                #if FABLE_COMPILER_PYTHON
                get.Required.Field "last_updated" Decode.datetimeLocal // Currently not supported in Thoth.Json.Core for python
                #else
                get.Required.Field "last_updated" Decode.datetimeUtc
                #endif
            )
        )

    let fromJsonString (jsonString: string) =
        try GDecode.fromJsonString decode jsonString with
        | exn     -> failwithf "Error. Given json string cannot be parsed to Template: %A" exn

    let toJsonString (spaces: int) (template:Template) =
        GEncode.toJsonString spaces (encode template)

module Templates =

    let encode (templateList: (string*Template) []) =
        templateList
        |> Array.toList
        |> List.map (fun (p: string, t: Template) -> p , Template.encode t)
        |> Encode.object

    let decode =
        Decode.dict Template.decode
        

    let fromJsonString (jsonString: string) =
        try GDecode.fromJsonString decode jsonString with
        | exn         -> failwithf "Error. Given json string cannot be parsed to Templates map: %A" exn

    let toJsonString (spaces: int) (templateList: (string*Template) []) =
        GEncode.toJsonString spaces (encode templateList)


[<AutoOpen>]
module Extension =

    type Template with
        member this.ToJson(?spaces: int) =
            let spaces = defaultArg spaces 0
            Template.toJsonString(spaces)