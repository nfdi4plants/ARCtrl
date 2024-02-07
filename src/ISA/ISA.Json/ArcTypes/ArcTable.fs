namespace ARCtrl.ISA.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ARCtrl.ISA
open System.Collections.Generic

module ArcTable =
    let encoder (table: ArcTable) =
        let keyEncoder : Encoder<int*int> = Encode.tuple2 Encode.int Encode.int
        let valueEncoder = CompositeCell.encoder
        Encode.object [
        "name", Encode.string table.Name
        if table.Headers.Count <> 0 then
            "header", Encode.list [
                for h in table.Headers do yield CompositeHeader.encoder h
            ]
        if table.Values.Count <> 0 then
            "values", Encode.map keyEncoder valueEncoder ([for KeyValue(k,v) in table.Values do yield k, v] |> Map)
        ] 

    let decoder : Decoder<ArcTable> =
        Decode.object(fun get ->
            let decodedHeader = get.Optional.Field "header" (Decode.list CompositeHeader.decoder) |> Option.defaultValue List.empty |> ResizeArray 
            let keyDecoder : Decoder<int*int> = Decode.tuple2 Decode.int Decode.int
            let valueDecoder = CompositeCell.decoder
            let decodedValues = get.Optional.Field "values" (Decode.map' keyDecoder valueDecoder) |> Option.defaultValue Map.empty |> System.Collections.Generic.Dictionary
            ArcTable.create(
                get.Required.Field "name" Decode.string,
                decodedHeader,
                decodedValues
            )
        )

    let compressedColumnEncoder (columnIndex : int) (rowCount : int) (cellTable : CellTableMap) (table: ArcTable) =
        //if header.IsIOType then
            Encode.array [|for r = 0 to rowCount - 1 do CellTable.encodeCell cellTable table.Values[columnIndex,r]|]
        //else
            
    let compressedColumnDecoder (columnIndex : int)(cellTable : CellTableArray) (table: ArcTable) (column : JsonValue)  =
        match Decode.array (CellTable.decodeCell cellTable) "" column with
        | Ok a -> a |> Array.iteri (fun r cell -> table.Values.Add((columnIndex,r),cell))
        | Error err -> failwith "Could not parse col"

    let compressedEncoder (stringTable : StringTableMap) (oaTable : OATableMap) (cellTable : CellTableMap) (table: ArcTable) =
        Encode.object [
            "n", StringTable.encodeString stringTable table.Name
            if table.Headers.Count <> 0 then
                "h", Encode.list [
                    for h in table.Headers do yield CompositeHeader.encoder h
                ]
            if table.Values.Count <> 0 then
                let rowCount = table.RowCount
                let columns = [|for c = 0 to table.ColumnCount - 1 do compressedColumnEncoder c rowCount cellTable table|]
                "c", Encode.array columns
        ] 

    let compressedDecoder (stringTable : StringTableArray) (oaTable : OATableArray) (cellTable : CellTableArray)  : Decoder<ArcTable> =
        Decode.object(fun get ->
            let decodedHeader = get.Optional.Field "h" (Decode.list CompositeHeader.decoder) |> Option.defaultValue List.empty |> ResizeArray 
            //let decodedValues = get.Optional.Field "c" (Decode.map' keyDecoder valueDecoder) |> Option.defaultValue Map.empty |> System.Collections.Generic.Dictionary
            let table = 
                ArcTable.create(
                    get.Required.Field "n" (StringTable.decodeString stringTable),
                    decodedHeader,
                    Dictionary()
                )
            let columns = get.Optional.Field "c" (Decode.array Decode.value)
            columns
            |> Option.iter (Array.iteri (fun c col -> compressedColumnDecoder c cellTable table col))
            table 

        )

[<AutoOpen>]
module ArcTableExtensions =

    type ArcTable with
        static member fromJsonString (jsonString: string) : ArcTable = 
            match Decode.fromString ArcTable.decoder jsonString with
            | Ok r -> r
            | Error e -> failwithf "Error. Unable to parse json string to ArcTable: %s" e

        member this.ToJsonString(?spaces) : string =
            let spaces = defaultArg spaces 0
            Encode.toString spaces (ArcTable.encoder this)

        static member toJsonString(a:ArcTable) = a.ToJsonString()

        static member fromCompressedJsonString (jsonString: string) : ArcTable = 
            let decoder = 
                Decode.object(fun get ->
                    let stringTable = get.Required.Field "stringTable" (StringTable.decoder)
                    let oaTable = get.Required.Field "oaTable" (OATable.decoder stringTable)
                    let cellTable = get.Required.Field "cellTable" (CellTable.decoder stringTable oaTable)
                    get.Required.Field "table" (ArcTable.compressedDecoder stringTable oaTable cellTable)
                )
            match Decode.fromString decoder jsonString with
            | Ok r -> r
            | Error e -> failwithf "Error. Unable to parse json string to ArcTable: %s" e

        member this.ToCompressedJsonString(?spaces) : string =
            let spaces = defaultArg spaces 0
            let stringTable = Dictionary()
            let oaTable = Dictionary()
            let cellTable = Dictionary()
            let arcTable = ArcTable.compressedEncoder stringTable oaTable cellTable this
            let jObject = 
                Encode.object [
                    "cellTable", CellTable.arrayFromMap cellTable |> CellTable.encoder stringTable oaTable
                    "oaTable", OATable.arrayFromMap oaTable |> OATable.encoder stringTable
                    "stringTable", StringTable.arrayFromMap stringTable |> StringTable.encoder
                    "table", arcTable
                ] 
            Encode.toString spaces jObject

        static member toCompressedJsonString(a:ArcTable) = a.ToCompressedJsonString()