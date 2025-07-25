module ARCtrl.Spreadsheet.CompositeColumn

open ARCtrl
open ARCtrl.Helper
open FsSpreadsheet

/// Checks if the column header is a deprecated IO Header. If so, fixes it.
///
/// The old format of IO Headers was only the type of IO so, e.g. "Source Name" or "Raw Data File".
///
/// A "Source Name" column will now be mapped to the propper "Input [Source Name]", and all other IO types will be mapped to "Output [<IO Type>]".
let fixDeprecatedIOHeader (stringCellCol : string []) = 
    if stringCellCol.Length = 0 then 
        failwith "Can't fix IOHeader Invalid column, neither header nor values given"
    let values = stringCellCol |> Array.skip 1
    match IOType.ofString (stringCellCol.[0]) with
    | IOType.FreeText _ -> stringCellCol
    | IOType.Source -> 
        let comp = CompositeHeader.Input (IOType.Source)
        stringCellCol.[0] <- comp.ToString()
        stringCellCol
    | ioType ->
        let comp = CompositeHeader.Output (ioType)
        stringCellCol.[0] <- comp.ToString()
        stringCellCol

let fromStringCellColumns (columns : array<string []>) : CompositeColumn =
    let header, cellParser = 
        columns
        |> Array.map (fun c -> c.[0])
        |> CompositeHeader.fromStringCells
    let l = columns.[0].Length
    let cells = 
        ResizeArray [|
            for i = 1 to l - 1 do
                columns
                |> Array.map (fun c -> c.[i])
                |> cellParser
        |]                 
    CompositeColumn.create(header,cells)

let stringCellColumnsOfFsColumns (columns : FsColumn []) : string [][] =
    columns
    |> Array.map (fun c ->
        c.ToDenseColumn()
        c.Cells
        |> Seq.toArray
        |> Array.map (fun cell -> cell.ValueAsString())
    )


let fromFsColumns (columns : FsColumn []) : CompositeColumn = 
    let stringCellColumns = 
        columns
        |> Array.map (fun c -> 
            c.ToDenseColumn()
            c.Cells
            |> Seq.toArray
            |> Array.map (fun c -> c.ValueAsString())
        )
    fromStringCellColumns stringCellColumns

let toStringCellColumns (column : CompositeColumn) : string list list =
    let hasUnit = column.Cells |> Seq.exists (fun c -> c.isUnitized)
    let isTerm = column.Header.IsTermColumn
    let isData = column.Header.IsDataColumn && column.Cells |> Seq.exists (fun c -> c.isData)
    let header = CompositeHeader.toStringCells hasUnit column.Header
    let compositeCells = column.Cells |> Seq.toArray
    let cells = column.Cells |> Seq.toArray |> Array.map (CompositeCell.toStringCells isTerm hasUnit)
    let getCellOrDefault (ri: int) (ci: int) (cells: string [] []) = cells.[ri] |> Array.tryItem ci |> Option.defaultValue ""
    if hasUnit then
        [
            [header.[0]; for i = 0 to compositeCells.Length - 1 do getCellOrDefault i 0 cells]
            [header.[1]; for i = 0 to compositeCells.Length - 1 do getCellOrDefault i 1 cells]
            [header.[2]; for i = 0 to compositeCells.Length - 1 do getCellOrDefault i 2 cells]
            [header.[3]; for i = 0 to compositeCells.Length - 1 do getCellOrDefault i 3 cells]
        ]
    elif isTerm then
        [
            [header.[0]; for i = 0 to compositeCells.Length - 1 do getCellOrDefault i 0 cells]
            [header.[1]; for i = 0 to compositeCells.Length - 1 do getCellOrDefault i 1 cells]
            [header.[2]; for i = 0 to compositeCells.Length - 1 do getCellOrDefault i 2 cells]
        ]
    elif isData then
        [
            [header.[0]; for i = 0 to compositeCells.Length - 1 do getCellOrDefault i 0 cells]
            [header.[1]; for i = 0 to compositeCells.Length - 1 do getCellOrDefault i 1 cells]
            [header.[2]; for i = 0 to compositeCells.Length - 1 do getCellOrDefault i 2 cells]
        ]
    else
        [
            [header.[0]; for i = 0 to compositeCells.Length - 1 do cells.[i].[0]]
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


module ColumnValueRefs =
    open System.Collections.Generic
    open ArcTableAux


    let fromStringCellColumns (valueMap : Dictionary<int, CompositeCell>) (columns : string [][]) : CompositeHeader*ColumnValueRefs =
        // This hashmap is only for internal performance improvements and is supposed to check whether a new cell value has to be created
        // Maps from the hash of the row strings to the cell hash
        let hashMap = Dictionary<int, int>()
        // This represents a potential sparse column,
        // Only filled if constant is set to false
        // Maps from row index to cell hash
        let cells = Dictionary<int, int>()
        // This is true, as long as there are no distinct values in the column
        let mutable constant = true
        let header, cellParser = 
            columns
            |> Array.map (fun c -> c.[0])
            |> CompositeHeader.fromStringCells
        let l = columns.[0].Length
        // If the column consists only of the header, we can return an empty sparse column
        if l = 1 then
            header, ColumnValueRefs.Sparse cells
        else 
            for i = 1 to l - 1 do
                let strings =
                    columns
                    |> Array.map (fun c -> c.[i])
                let hash =
                    strings
                    |> Seq.map (fun s -> s.GetHashCode())
                    |> Seq.reduce HashCodes.mergeHashes
                match IntDictionary.tryFind hash hashMap with
                | Some cellHash when constant->
                    ()
                | Some cellHash ->
                    // If the hash is already in the hashmap, we can use it
                    cells.[i - 1] <- cellHash
                | None when i = 1 ->
                    let cell = cellParser strings
                    let cellHash = ArcTableAux.ensureCellHashInValueMap cell valueMap 
                    hashMap.[hash] <- cellHash
                    cells.[i - 1] <- cellHash
                | None ->
                    if constant then
                        // If the column was constant until now, we can set it to unconstant now
                        constant <- false
                        // We also need to fill the cells up until this point
                        for j = 0 to i - 1 do
                            let cellHash = cells.[0]
                            cells.[j] <- cellHash
                    let cell = cellParser strings
                    let cellHash = ArcTableAux.ensureCellHashInValueMap cell valueMap
                    hashMap.[hash] <- cellHash
                    cells.[i - 1] <- cellHash                  
            if constant then
                header, ColumnValueRefs.Constant cells.[0]           
            else
                header, ColumnValueRefs.Sparse cells