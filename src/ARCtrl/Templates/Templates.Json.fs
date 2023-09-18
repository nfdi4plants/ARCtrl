module ARCtrl.Templates.Json

open ARCtrl.Templates
open ARCtrl.ISA
open System
#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

module Organisation =
    let encode = Encode.Auto.generateEncoder<Organisation>()

    let decode = Decode.Auto.generateDecoder<Organisation>()

module CompositeCell =

    let encode = Encode.Auto.generateEncoder<CompositeCell>()

    let decode = Decode.Auto.generateDecoder<CompositeCell>()

module CompositeHeader =

    let encode = Encode.Auto.generateEncoder<CompositeHeader>()

    let decode = Decode.Auto.generateDecoder<CompositeHeader>()

module ArcTable =

    let encode (table: ArcTable) =
        let keyEncoder : Encoder<int*int> = Encode.tuple2 Encode.int Encode.int
        let valueEncoder = CompositeCell.encode
        Encode.object [
            "name", Encode.string table.Name
            "header", Encode.list [
                for h in table.Headers do yield CompositeHeader.encode h
            ]
            "values", Encode.map keyEncoder valueEncoder ([for KeyValue(k,v) in table.Values do yield k, v] |> Map)
        ] 

    let decode : Decoder<ArcTable> =
        Decode.object(fun get ->
            let decodedHeader = get.Required.Field "header" (Decode.list CompositeHeader.decode) |> ResizeArray
            let keyDecoder : Decoder<int*int> = Decode.tuple2 Decode.int Decode.int
            let valueDecoder = CompositeCell.decode
            let decodedValues = get.Required.Field "values" (Decode.map' keyDecoder valueDecoder) |> System.Collections.Generic.Dictionary
            ArcTable.create(
                get.Required.Field "name" Decode.string,
                decodedHeader,
                decodedValues
            )
        )

module Json =

    open ARCtrl.ISA.Json

    let encode (template: Template) =
        let personEncoder = ARCtrl.ISA.Json.Person.encoder (ConverterOptions())
        let oaEncoder = ARCtrl.ISA.Json.OntologyAnnotation.encoder (ConverterOptions())
        Encode.object [
            "id", Encode.guid template.Id
            "table", ArcTable.encode template.Table
            "name", Encode.string template.Name
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
                get.Required.Field "table" ArcTable.decode,
                get.Required.Field "name" Decode.string,
                get.Required.Field "organisation" Organisation.decode,
                get.Required.Field "version" Decode.string,
                get.Required.Field "authors" (Decode.array personDecoder),
                get.Required.Field "endpoint_repositories" (Decode.array oaDecoder),
                get.Required.Field "tags" (Decode.array oaDecoder),
                get.Required.Field "last_updated" Decode.datetimeUtc
            )
        )