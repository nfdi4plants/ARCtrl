namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl

module ArcAssay = 

    /// exports in json-ld format
    let toJsonldString (a:ArcAssay) = 
        Assay.encoder (ConverterOptions(SetID=true,IsJsonLD=true)) None (a.ToAssay())
        |> Encode.toJsonString 2

    let toJsonldStringWithContext (a:ArcAssay) = 
        Assay.encoder (ConverterOptions(SetID=true,IsJsonLD=true)) None (a.ToAssay())
        |> Encode.toJsonString 2

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (Assay.decoder (ConverterOptions())) s
        |> ArcAssay.fromAssay

    let toJsonString (a:ArcAssay) = 
        Assay.encoder (ConverterOptions()) None (a.ToAssay())
        |> Encode.toJsonString 2

    let toArcJsonString (a:ArcAssay) : string =
        let spaces = 0
        Encode.toJsonString spaces (encoder a)

    let fromArcJsonString (jsonString: string) =
        try GDecode.fromJsonString decoder jsonString with
        | e -> failwithf "Error. Unable to parse json string to ArcAssay: %s" e.Message

[<AutoOpen>]
module ArcAssayExtensions =

    open System.Collections.Generic

    type ArcAssay with
        static member fromArcJsonString (jsonString: string) : ArcAssay = 
            try GDecode.fromJsonString ArcAssay.decoder jsonString with
            | e -> failwithf "Error. Unable to parse json string to ArcAssay: %s" e.Message

        member this.ToArcJsonString(?spaces) : string =
            let spaces = defaultArg spaces 0
            Encode.toJsonString spaces (ArcAssay.encoder this)

        static member toArcJsonString (a:ArcAssay) = a.ToArcJsonString()

        static member fromCompressedJsonString (jsonString: string) : ArcAssay = 
            let decoder = 
                Decode.object(fun get ->
                    let stringTable = get.Required.Field "stringTable" (StringTable.decoder)
                    let oaTable = get.Required.Field "oaTable" (OATable.decoder stringTable)
                    let cellTable = get.Required.Field "cellTable" (CellTable.decoder stringTable oaTable)
                    get.Required.Field "assay" (ArcAssay.compressedDecoder stringTable oaTable cellTable)
                )
            try GDecode.fromJsonString decoder jsonString with
            | e -> failwithf "Error. Unable to parse json string to ArcAssay: %s" e.Message

        member this.ToCompressedJsonString(?spaces) : string =
            let spaces = defaultArg spaces 0
            let stringTable = Dictionary()
            let oaTable = Dictionary()
            let cellTable = Dictionary()
            let arcAssay = ArcAssay.compressedEncoder stringTable oaTable cellTable this
            let jObject = 
                Encode.object [
                    "cellTable", CellTable.arrayFromMap cellTable |> CellTable.encoder stringTable oaTable
                    "oaTable", OATable.arrayFromMap oaTable |> OATable.encoder stringTable
                    "stringTable", StringTable.arrayFromMap stringTable |> StringTable.encoder
                    "assay", arcAssay
                ] 
            Encode.toJsonString spaces jObject

        static member toCompressedJsonString (a:ArcAssay) = a.ToCompressedJsonString()