module ISA.Spreadsheet.CompositeColumn

open ISA
open FsSpreadsheet


let fromFsColumns (columns : list<FsColumn>) : CompositeColumn =
    let header = 
        columns
        |> List.map (fun c -> c.[1])
        |> CompositeHeader.fromFsCells
    let l = columns.[0].RangeAddress.LastAddress.RowNumber
    let cells = 
        [|
        for i = 2 to l do
            columns
            |> List.map (fun c -> c.[i])
            |> CompositeCell.fromFsCells
        |]                 
    CompositeColumn.create(header,cells)


let toFsColumns (column : CompositeColumn) : FsCell list list =
    let hasUnit = column.Cells |> Seq.exists (fun c -> c.isUnitized)
    let isTerm = column.Header.IsTermColumn
    let header = CompositeHeader.toFsCells hasUnit column.Header
    let cells = column.Cells |> Array.map (CompositeCell.toFsCells isTerm hasUnit)
    if hasUnit then
        [
            [header.[0]; for i = 0 to column.Cells.Length - 1 do cells.[i].[0]]
            [header.[1]; for i = 0 to column.Cells.Length - 1 do cells.[i].[1]]
            [header.[2]; for i = 0 to column.Cells.Length - 1 do cells.[i].[2]]
            [header.[3]; for i = 0 to column.Cells.Length - 1 do cells.[i].[3]]
        ]
    elif isTerm then
        [
            [header.[0]; for i = 0 to column.Cells.Length - 1 do cells.[i].[0]]
            [header.[1]; for i = 0 to column.Cells.Length - 1 do cells.[i].[1]]
            [header.[2]; for i = 0 to column.Cells.Length - 1 do cells.[i].[2]]
        ]
    else
        [
            [header.[0]; for i = 0 to column.Cells.Length - 1 do cells.[i].[0]]
        ]