namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open System.Collections.Generic

module ArcTable =

    let encoder (table: ArcTable) =
        let keyEncoder : Encoder<int*int> = Encode.tuple2 Encode.int Encode.int
        let columnEncoder (col : ArcTableAux.ColumnValueRefs) : IEncodable =
            match col with
            | ArcTableAux.ColumnValueRefs.Constant hash -> Encode.int hash
            | ArcTableAux.ColumnValueRefs.Sparse cells -> Encode.intDictionary Encode.int cells

        Encode.object [
            "name", Encode.string table.Name
            if table.Headers.Count <> 0 then
                "headers", Encode.list [
                    for h in table.Headers do yield CompositeHeader.encoder h
                ]
            if table.Values.RowCount <> 0 then
                "cells", Encode.intDictionary CompositeCell.encoder table.Values.ValueMap
                "columns", Encode.intDictionary columnEncoder table.Values.Columns
                "rowCount", Encode.int table.RowCount
        ] 

    let decoderV2Deprecated : Decoder<ArcTable> =
        Decode.object(fun get ->
            let decodedHeader = get.Optional.Field "header" (Decode.list CompositeHeader.decoder) |> Option.defaultValue List.empty |> ResizeArray 
            let keyDecoder : Decoder<int*int> = Decode.tuple2 Decode.int Decode.int
            let valueDecoder = CompositeCell.decoder
            let decodedValues = get.Optional.Field "values" (Decode.map' keyDecoder valueDecoder) |> Option.defaultValue Map.empty |> System.Collections.Generic.Dictionary
            let t = 
                ArcTable.create(
                    get.Required.Field "name" Decode.string,
                    decodedHeader,
                    ResizeArray()
                )
            for KeyValue((c,r),v) in decodedValues do
                t.SetCellAt(c, r, v)
            t
        )

    let decoder : Decoder<ArcTable> =

        let columnDecoder : Decoder<ArcTableAux.ColumnValueRefs> =
            Decode.oneOf [
                Decode.int |> Decode.map ArcTableAux.ColumnValueRefs.Constant
                Decode.dictionary Decode.int Decode.int |> Decode.map ArcTableAux.ColumnValueRefs.Sparse
            ]
        let decoder : Decoder<ArcTable> =
            Decode.object(fun get ->               
                let decodedHeader = get.Optional.Field "headers" (Decode.resizeArray CompositeHeader.decoder) |> Option.defaultValue (ResizeArray())
                let decodedCells = get.Optional.Field "cells" (Decode.dictionary Decode.int CompositeCell.decoder) |> Option.defaultValue (Dictionary())
                let decodedColumns = get.Optional.Field "columns" (Decode.dictionary Decode.int columnDecoder) |> Option.defaultValue (Dictionary())
                let rowCount = get.Optional.Field "rowCount" Decode.int |> Option.defaultValue 0

                let values = ArcTableAux.ArcTableValues(decodedColumns, decodedCells, rowCount)
                
                ArcTable.fromArcTableValues(
                    get.Required.Field "name" Decode.string,
                    decodedHeader,
                    values
                )
            )
        {new Decoder<ArcTable> with
            member this.Decode (helper, column) =
                if helper.hasProperty "values" column then
                    // This is the old format, we need to decode it with the old decoder
                    decoderV2Deprecated.Decode(helper, column)
                else
                    // This is the new format, we can use the new decoder
                    decoder.Decode(helper, column)              
        }
        

    open CellTable

    let decoderCompressedColumn (cellTable : CellTableArray) (table: ArcTable) (columnIndex : int)  =
        {new Decoder<unit> with
            member this.Decode (helper,column) =
                match (Decode.array (CellTable.decodeCell cellTable)).Decode(helper,column) with
                | Ok a ->             
                    a |> Array.iteri (fun r cell -> ArcTableAux.Unchecked.setCellAt(columnIndex,r,cell) table.Values)
                    Ok(())
                | Error err -> 
                    let rangeDecoder = 
                        Decode.object (fun get -> 
                            let from = get.Required.Field "f" Decode.int
                            let to_ = get.Required.Field "t" Decode.int
                            let value = get.Required.Field "v" (CellTable.decodeCell cellTable)
                            for i = from to to_ do
                                ArcTableAux.Unchecked.setCellAt(columnIndex,i,value) table.Values
                        )
                    match (Decode.array (rangeDecoder)).Decode(helper,column) with
                    | Ok _ -> Ok ()
                    | Error err -> Error err
        } 
            
    let arrayi (decoderi: int -> Decoder<'value>) : Decoder<'value array> =
        { new Decoder<'value array> with
            member _.Decode(helpers, value) =
                if helpers.isArray value then
                    let mutable i = -1
                    let tokens = helpers.asArray value
                    let arr = Array.zeroCreate tokens.Length

                    (Ok arr, tokens)
                    ||> Array.fold (fun acc value ->
                        i <- i + 1

                        match acc with
                        | Error _ -> acc
                        | Ok acc ->
                            match (decoderi i).Decode(helpers, value) with
                            | Error er ->
                                Error(
                                    er
                                    |> Decode.Helpers.prependPath (
                                        ".[" + (i.ToString()) + "]"
                                    )
                                )
                            | Ok value ->
                                acc.[i] <- value
                                Ok acc
                    )
                else
                    ("", BadPrimitive("an array", value)) |> Error
        }
        
    open OATable
    open StringTable

    let encoderCompressed (stringTable : StringTableMap) (oaTable : OATableMap) (cellTable : CellTableMap) (table: ArcTable) =
        let keyEncoder : Encoder<int*int> = Encode.tuple2 Encode.int Encode.int
        let columnEncoder (col : ArcTableAux.ColumnValueRefs) : IEncodable =
            match col with
            | ArcTableAux.ColumnValueRefs.Constant hash -> Encode.int hash
            | ArcTableAux.ColumnValueRefs.Sparse cells -> Encode.intDictionary Encode.int cells
        Encode.object [
            "n", StringTable.encodeString stringTable table.Name
            if table.Headers.Count <> 0 then
                "h", Encode.list [
                    for h in table.Headers do yield CompositeHeader.encoder h
                ]
            if table.Values.RowCount <> 0 then
                "ce", Encode.intDictionary (CellTable.encodeCell cellTable) table.Values.ValueMap
                "co", Encode.intDictionary columnEncoder table.Values.Columns
                "r", Encode.int table.RowCount
        ] 

    let decoderCompressedV2Deprecated (stringTable : StringTableArray) (oaTable : OATableArray) (cellTable : CellTableArray)  : Decoder<ArcTable> =
        Decode.object(fun get ->
            let decodedHeader = get.Optional.Field "h" (Decode.list CompositeHeader.decoder) |> Option.defaultValue List.empty |> ResizeArray 
            //let decodedValues = get.Optional.Field "c" (Decode.map' keyDecoder valueDecoder) |> Option.defaultValue Map.empty |> System.Collections.Generic.Dictionary
            let table = 
                ArcTable.create(
                    get.Required.Field "n" (StringTable.decodeString stringTable),
                    decodedHeader,
                    ResizeArray()
                )
                
            // Columns
            get.Optional.Field "c" (arrayi (decoderCompressedColumn cellTable table)) |> ignore
            
            table 

        )

    let decoderCompressed (stringTable : StringTableArray) (oaTable : OATableArray) (cellTable : CellTableArray)  : Decoder<ArcTable> =

        let columnDecoder : Decoder<ArcTableAux.ColumnValueRefs> =
            Decode.oneOf [
                Decode.int |> Decode.map ArcTableAux.ColumnValueRefs.Constant
                Decode.dictionary Decode.int Decode.int |> Decode.map ArcTableAux.ColumnValueRefs.Sparse
            ]
        let decoder : Decoder<ArcTable> =
            Decode.object(fun get ->               
                let decodedHeader = get.Optional.Field "h" (Decode.resizeArray CompositeHeader.decoder) |> Option.defaultValue (ResizeArray())
                let decodedCells = get.Optional.Field "ce" (Decode.dictionary Decode.int (CellTable.decodeCell cellTable)) |> Option.defaultValue (Dictionary())
                let decodedColumns = get.Optional.Field "co" (Decode.dictionary Decode.int columnDecoder) |> Option.defaultValue (Dictionary())
                let rowCount = get.Optional.Field "r" Decode.int |> Option.defaultValue 0

                let values = ArcTableAux.ArcTableValues(decodedColumns, decodedCells, rowCount)
                
                ArcTable.fromArcTableValues(
                    get.Required.Field "n" (StringTable.decodeString stringTable),
                    decodedHeader,
                    values
                )
            )
        {new Decoder<ArcTable> with
            member this.Decode (helper, column) =
                if helper.hasProperty "c" column then
                    // This is the old format, we need to decode it with the old decoder
                    (decoderCompressedV2Deprecated stringTable oaTable cellTable).Decode(helper, column)
                else
                    // This is the new format, we can use the new decoder
                    decoder.Decode(helper, column)              
        }