module ISA.Spreadsheet.CompositeColumn

open ISA
open FsSpreadsheet


let fromFsColumns (columns : list<FsColumn>) : CompositeColumn =
    let header = 
        columns
        |> List.map (fun c -> c.[0])
        |> CompositeHeader.fromFsCells
    let l = columns.[0].RangeAddress.LastAddress.RowNumber
    let cells = 
        [|
        for i = 1 to l do
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
            [header.[0]] @ cells.[0]
            [header.[1]] @ cells.[1]
            [header.[2]] @ cells.[2]
            [header.[3]] @ cells.[3]
        ]
    elif isTerm then
        [
            [header.[0]] @ cells.[0]
            [header.[1]] @ cells.[1]
            [header.[2]] @ cells.[2]
        ]
    else
        [
            [header.[0]] @ cells.[0]
        ]