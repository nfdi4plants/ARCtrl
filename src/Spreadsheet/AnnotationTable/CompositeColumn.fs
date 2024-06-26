﻿module ARCtrl.Spreadsheet.CompositeColumn

open ARCtrl
open ARCtrl.Helper
open FsSpreadsheet

/// Checks if the column header is a deprecated IO Header. If so, fixes it.
///
/// The old format of IO Headers was only the type of IO so, e.g. "Source Name" or "Raw Data File".
///
/// A "Source Name" column will now be mapped to the propper "Input [Source Name]", and all other IO types will be mapped to "Output [<IO Type>]".
let fixDeprecatedIOHeader (stringCellCol : string list) = 
    let header,values = 
        match stringCellCol with
        | header :: values -> header,values
        | _ -> failwith "Can't fix IOHeader Invalid column, neither header nor values given"
    match IOType.ofString (stringCellCol.[0]) with
    | IOType.FreeText _ -> stringCellCol
    | IOType.Source -> 
        let comp = CompositeHeader.Input (IOType.Source)       
        comp.ToString() :: values
    | ioType ->
        let comp = CompositeHeader.Output (ioType)       
        comp.ToString() :: values

let fromStringCellColumns (columns : list<string list>) : CompositeColumn =
    let header, cellParser = 
        columns
        |> List.map (fun c -> c.[0])
        |> CompositeHeader.fromStringCells
    let l = columns.[0].Length
    let cells = 
        [|
        for i = 1 to l - 1 do
            columns
            |> List.map (fun c -> c.[i])
            |> cellParser
        |]                 
    CompositeColumn.create(header,cells)


let fromFsColumns (columns : list<FsColumn>) : CompositeColumn = 
    let stringCellColumns = 
        columns
        |> List.map (fun c -> 
            c.ToDenseColumn()
            c.Cells
            |> Seq.toList
            |> List.map (fun c -> c.ValueAsString())

        )
    fromStringCellColumns stringCellColumns

let toStringCellColumns (column : CompositeColumn) : string list list =
    let hasUnit = column.Cells |> Seq.exists (fun c -> c.isUnitized)
    let isTerm = column.Header.IsTermColumn
    let isData = column.Header.IsDataColumn
    let header = CompositeHeader.toStringCells hasUnit column.Header
    let cells = column.Cells |> Array.map (CompositeCell.toStringCells isTerm hasUnit)
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

let toFsColumns (column : CompositeColumn) : list<FsCell list> =
    let stringCellColumns = toStringCellColumns column
    let fsColumns = 
        stringCellColumns
        |> List.map (fun c -> 
            c
            |> List.map (fun s -> FsCell(s))
        )
    fsColumns