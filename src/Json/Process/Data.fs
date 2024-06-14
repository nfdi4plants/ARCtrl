namespace ARCtrl.Json

open Thoth.Json.Core

open StringTable

open ARCtrl
open ARCtrl.Process

module Data = 

    // let mutable _id : URI option = id
    // let mutable _name : string option = name
    // let mutable _dataType : DataFile option = dataType
    // let mutable _format : string option = format
    // let mutable _selectorFormat : URI option = selectorFormat
    // let mutable _comments : Comment list option = comments
    let encoder (d : Data) = 
        [
            Encode.tryInclude "@id" Encode.string d.ID
            Encode.tryInclude "name" Encode.string d.Name
            Encode.tryInclude "dataType" DataFile.ISAJson.encoder d.DataType
            Encode.tryInclude "format" Encode.string d.Format
            Encode.tryInclude "selectorFormat" Encode.string d.SelectorFormat
            Encode.tryIncludeSeq "comments" Comment.encoder d.Comments
        ]
        |> Encode.choose
        |> Encode.object


    let decoder : Decoder<Data> =
        Decode.object (fun get ->
            Data(
                ?id = get.Optional.Field "@id" Decode.uri,
                ?name = get.Optional.Field "name" Decode.string,
                ?dataType = get.Optional.Field "dataType" DataFile.ISAJson.decoder,
                ?format = get.Optional.Field "format" Decode.string,
                ?selectorFormat = get.Optional.Field "selectorFormat" Decode.uri,
                ?comments = get.Optional.Field "comments" (Decode.resizeArray Comment.decoder)
            )
        )

    let compressedEncoder (stringTable : StringTableMap) (d : Data) = 
        [
            Encode.tryInclude "i" (StringTable.encodeString stringTable) d.ID
            Encode.tryInclude "n" (StringTable.encodeString stringTable) d.Name
            Encode.tryInclude "d" DataFile.ISAJson.encoder d.DataType
            Encode.tryInclude "f" (StringTable.encodeString stringTable) d.Format
            Encode.tryInclude "s" (StringTable.encodeString stringTable) d.SelectorFormat
            Encode.tryIncludeSeq "c" Comment.encoder d.Comments
        ]
        |> Encode.choose
        |> Encode.object


    let compressedDecoder (stringTable : StringTableArray) : Decoder<Data> =
        Decode.object (fun get ->
            Data(
                ?id = get.Optional.Field "i" (StringTable.decodeString stringTable),
                ?name = get.Optional.Field "n" (StringTable.decodeString stringTable),
                ?dataType = get.Optional.Field "d" DataFile.ISAJson.decoder,
                ?format = get.Optional.Field "f" (StringTable.decodeString stringTable),
                ?selectorFormat = get.Optional.Field "s" (StringTable.decodeString stringTable),
                ?comments = get.Optional.Field "c" (Decode.resizeArray Comment.decoder)
            )
        )

    module ROCrate =

        let genID (d:Data) : string = 
            match d.ID with
            | Some id -> URI.toString id
            | None -> match d.Name with
                      | Some n -> n.Replace(" ","_")
                      | None -> "#EmptyData"
    
        let encoder (oa : Data) = 
            [
                "@id", Encode.string (oa |> genID)           
                "@type", (Encode.list [Encode.string "Data"])
                Encode.tryInclude "name" Encode.string (oa.Name)
                Encode.tryInclude "type" DataFile.ROCrate.encoder oa.DataType
                Encode.tryInclude "encodingFormat" Encode.string oa.Format
                Encode.tryInclude "usageInfo" Encode.string oa.SelectorFormat
                Encode.tryIncludeSeq "comments" Comment.ROCrate.encoder oa.Comments
                "@context", ROCrateContext.Data.context_jsonvalue
            ]
            |> Encode.choose
            |> Encode.object

        let decoder : Decoder<Data> =
            Decode.object (fun get ->
                Data(
                    ?id = get.Optional.Field "@id" Decode.uri,
                    ?name = get.Optional.Field "name" Decode.string,
                    ?dataType = get.Optional.Field "type" DataFile.ROCrate.decoder,
                    ?format = get.Optional.Field "encodingFormat" Decode.string,
                    ?selectorFormat = get.Optional.Field "usageInfo" Decode.uri,
                    ?comments = get.Optional.Field "comments" (Decode.resizeArray Comment.ROCrate.decoder)
                )
            
            )


    module ISAJson =
    
        let encoder (oa : Data) = 
            [
                Encode.tryInclude "@id" Encode.string oa.ID
                Encode.tryInclude "name" Encode.string oa.Name
                Encode.tryInclude "type" DataFile.ISAJson.encoder oa.DataType
                Encode.tryIncludeSeq "comments" Comment.ISAJson.encoder oa.Comments
            ]
            |> Encode.choose
            |> Encode.object

        let allowedFields = ["@id";"name";"type";"comments";"@type"; "@context"]

        let decoder: Decoder<Data> =
            Decode.objectNoAdditionalProperties allowedFields (fun get ->
                Data(
                    ?id = get.Optional.Field "@id" Decode.uri,
                    ?name = get.Optional.Field "name" Decode.string,
                    ?dataType = get.Optional.Field "type" DataFile.ISAJson.decoder,
                    ?comments = get.Optional.Field "comments" (Decode.resizeArray Comment.ISAJson.decoder)
                )
            )

[<AutoOpen>]
module DataExtensions =
    
    type Data with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Data.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:Data) ->
                Data.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.toISAJsonString(?spaces) = 
            Data.toISAJsonString(?spaces=spaces) this

        static member fromROCrateJsonString (s:string) =
            Decode.fromJsonString Data.ROCrate.decoder s

        static member toROCrateJsonString(?spaces) =
            fun (f:Data) ->
                Data.ROCrate.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.toROCrateJsonString(?spaces) =
            Data.toROCrateJsonString(?spaces=spaces) this