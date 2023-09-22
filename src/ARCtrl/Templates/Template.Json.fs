module ARCtrl.Template.Json

open ARCtrl.Template
open ARCtrl.ISA
open System
#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
open ARCtrl
#endif

//https://thoth-org.github.io/Thoth.Json/documentation/auto/extra-coders.html#ready-to-use-extra-coders

module Organisation =

    let encode = (fun (org: Organisation) -> org.ToString()) >> Encode.string 

    let decode = 
        Decode.string
        |> Decode.andThen (fun textValue ->
            Organisation.ofString textValue
            |> Decode.succeed
        )

//module CompositeCell =

//    let encode = Encode.Auto.generateEncoder<CompositeCell>()

//    let decode = Decode.Auto.generateDecoder<CompositeCell>()

//module CompositeHeader =

//    let encode = Encode.Auto.generateEncoder<CompositeHeader>()

//    let decode = Decode.Auto.generateDecoder<CompositeHeader>()

//module ArcTable =

//    let encode (table: ArcTable) =
//        let keyEncoder : Encoder<int*int> = Encode.tuple2 Encode.int Encode.int
//        let valueEncoder = CompositeCell.encode
//        Encode.object [
//            "name", Encode.string table.Name
//            "header", Encode.list [
//                for h in table.Headers do yield CompositeHeader.encode h
//            ]
//            "values", Encode.map keyEncoder valueEncoder ([for KeyValue(k,v) in table.Values do yield k, v] |> Map)
//        ] 

//    let decode : Decoder<ArcTable> =
//        Decode.object(fun get ->
//            let decodedHeader = get.Required.Field "header" (Decode.list CompositeHeader.decode) |> ResizeArray
//            let keyDecoder : Decoder<int*int> = Decode.tuple2 Decode.int Decode.int
//            let valueDecoder = CompositeCell.decode
//            let decodedValues = get.Required.Field "values" (Decode.map' keyDecoder valueDecoder) |> System.Collections.Generic.Dictionary
//            ArcTable.create(
//                get.Required.Field "name" Decode.string,
//                decodedHeader,
//                decodedValues
//            )
//        )

module Template =

    open ARCtrl.ISA.Json

    let encode (template: Template) =
        let personEncoder = ARCtrl.ISA.Json.Person.encoder (ConverterOptions())
        let oaEncoder = ARCtrl.ISA.Json.OntologyAnnotation.encoder (ConverterOptions())
        Encode.object [
            "id", Encode.guid template.Id
            "table", template.Table.GetProcesses() |> List.map (Process.encoder (ConverterOptions())) |> Encode.list 
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
                get.Required.Field "table" (Decode.list (Process.decoder (ConverterOptions()) ) |> Decode.map (fun ps -> ArcTables.fromProcesses ps |> fun arctbls -> arctbls.[0])),
                get.Required.Field "name" Decode.string,
                get.Required.Field "description" Decode.string,
                get.Required.Field "organisation" Organisation.decode,
                get.Required.Field "version" Decode.string,
                get.Required.Field "authors" (Decode.array personDecoder),
                get.Required.Field "endpoint_repositories" (Decode.array oaDecoder),
                get.Required.Field "tags" (Decode.array oaDecoder),
                get.Required.Field "last_updated" Decode.datetimeUtc
            )
        )

    let decodeFromString (jsonString: string) =
        match Decode.fromString decode jsonString with
        | Ok template   -> template
        | Error exn     -> failwithf "Error. Given json string cannot be parsed to Template: %A" exn

    let encodeToString (spaces: int) (template:Template) =
        Encode.toString spaces (encode template)

module Templates =

    let encode (templateList: (string*Template) []) =
        templateList
        |> Array.toList
        |> List.map (fun (p: string, t: Template) -> p , Template.encode t)
        |> Encode.object

    let decode =
        let d = Decode.dict Template.decode
        Decode.fromString d

    let decodeFromString (jsonString: string) =
        match decode jsonString with
        | Ok templateMap    -> templateMap
        | Error exn         -> failwithf "Error. Given json string cannot be parsed to Templates map: %A" exn

    let encodeToString (spaces: int) (templateList: (string*Template) []) =
        Encode.toString spaces (encode templateList)


[<AutoOpen>]
module Extension =

    type Template with
        member this.ToJson(?spaces: int) =
            let spaces = defaultArg spaces 0
            Template.encodeToString(spaces)