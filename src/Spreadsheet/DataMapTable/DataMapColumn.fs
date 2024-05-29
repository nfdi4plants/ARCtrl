module ARCtrl.Spreadsheet.DataMapColumn

open ARCtrl
open ArcTable
open FsSpreadsheet

let fromFsColumns (columns : list<FsColumn>) : CompositeColumn =
    let header, cellParser = 
        columns
        |> List.map (fun c -> c.[1])
        |> DataMapHeader.fromFsCells
    let l = columns.[0].RangeAddress.LastAddress.RowNumber
    let cells = 
        [|
        for i = 2 to l do
            columns
            |> List.map (fun c -> c.[i])
            |> cellParser
        |]                 
    CompositeColumn.create(header,cells)


let toFsColumns (column : CompositeColumn) : FsCell list list =
    let isTerm = column.Header.IsTermColumn
    let isData = column.Header.IsDataColumn
    let header = DataMapHeader.toFsCells column.Header
    let cells = column.Cells |> Array.map (CompositeCell.toFsCells isTerm false)
    if isTerm then
        [
            [header.[0]; for i = 0 to column.Cells.Length - 1 do cells.[i].[0]]
            [header.[1]; for i = 0 to column.Cells.Length - 1 do cells.[i].[1]]
            [header.[2]; for i = 0 to column.Cells.Length - 1 do cells.[i].[2]]
        ]
    elif isData then
        let hasFormat = column.Cells |> Seq.exists (fun c -> c.AsData.Format.IsSome)
        let hasSelectorFormat = column.Cells |> Seq.exists (fun c -> c.AsData.SelectorFormat.IsSome)

        [
            [header.[0]; for i = 0 to column.Cells.Length - 1 do cells.[i].[0]]
            if hasFormat then 
                [header.[1]; for i = 0 to column.Cells.Length - 1 do cells.[i].[1]]
            if hasSelectorFormat then 
                [header.[2]; for i = 0 to column.Cells.Length - 1 do cells.[i].[2]]
        ]
    else
        [
            [header.[0]; for i = 0 to column.Cells.Length - 1 do cells.[i].[0]]
        ]