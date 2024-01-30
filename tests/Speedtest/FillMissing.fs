module FillMissing

open ARCtrl.ISA
open ArcTableAux

open System.Collections.Generic


// We need to calculate the max number of rows between the new columns and the existing columns in the table.
// `maxRows` will be the number of rows all columns must have after adding the new columns.
// This behaviour should be intuitive for the user, as Excel handles this case in the same way.
let fillMissingCellsSeq (headers: ResizeArray<CompositeHeader>) (values:Dictionary<int*int,CompositeCell>) =
    let rowCount = getRowCount values
    let columnCount = getColumnCount headers

    let columnKeyGroups = 
        values.Keys // Get all keys, to map over relevant rows afterwards
        |> Seq.groupBy fst // Group by column index
        |> Map.ofSeq

    for columnIndex = 0 to columnCount - 1 do
        let header = headers.[columnIndex]
        match Map.tryFind columnIndex columnKeyGroups with
        // Some values existed in this column. Fill with default cells
        | Some col ->
            let firstCell = Some (values.[Seq.head col])
            let defaultCell = Unchecked.getEmptyCellForHeader header firstCell
            let rowKeys = col |> Seq.map snd |> Set.ofSeq
            for rowIndex = 0 to rowCount - 1 do
                if not <| rowKeys.Contains rowIndex then
                    Unchecked.setCellAt (columnIndex,rowIndex,defaultCell) values
        // No values existed in this column. Fill with default cells
        | None ->
            let defaultCell = Unchecked.getEmptyCellForHeader header None
            for rowIndex = 0 to rowCount - 1 do
                Unchecked.setCellAt (columnIndex,rowIndex,defaultCell) values

let fillMissingCellsArray (headers: ResizeArray<CompositeHeader>) (values:Dictionary<int*int,CompositeCell>) =
    let rowCount = getRowCount values
    let columnCount = getColumnCount headers

    let columnKeyGroups = 
        values.Keys // Get all keys, to map over relevant rows afterwards
        |> Seq.toArray
        |> Array.groupBy fst // Group by column index
        |> Map.ofArray

    for columnIndex = 0 to columnCount - 1 do
        let header = headers.[columnIndex]
        match Map.tryFind columnIndex columnKeyGroups with
        // All values existed in this column. Nothing to do
        | Some col when col.Length = rowCount ->
            ()
        // Some values existed in this column. Fill with default cells
        | Some col ->
            let firstCell = Some (values.[Seq.head col])
            let defaultCell = Unchecked.getEmptyCellForHeader header firstCell
            let rowKeys = Array.map snd col |> Set.ofArray
            for rowIndex = 0 to rowCount - 1 do
                if not <| rowKeys.Contains rowIndex then
                    Unchecked.setCellAt (columnIndex,rowIndex,defaultCell) values
        // No values existed in this column. Fill with default cells
        | None ->
            let defaultCell = Unchecked.getEmptyCellForHeader header None
            for rowIndex = 0 to rowCount - 1 do
                Unchecked.setCellAt (columnIndex,rowIndex,defaultCell) values


let fillMissingOld (headers: ResizeArray<CompositeHeader>) (values:Dictionary<int*int,CompositeCell>) = 
    let rowCount = getRowCount values
    let columnCount = getColumnCount headers
    let maxRows = rowCount
    let lastColumnIndex = columnCount - 1
    /// Get all keys, to map over relevant rows afterwards
    let keys = values.Keys
    // iterate over columns
    for columnIndex in 0 .. lastColumnIndex do
        /// Only get keys for the relevant column
        let colKeys = keys |> Seq.filter (fun (c,_) -> c = columnIndex) |> Set.ofSeq 
        /// Create set of expected keys
        let expectedKeys = Seq.init maxRows (fun i -> columnIndex,i) |> Set.ofSeq 
        /// Get the missing keys
        let missingKeys = Set.difference expectedKeys colKeys 
        // if no missing keys, we are done and skip the rest, if not empty missing keys we ...
        if missingKeys.IsEmpty |> not then
            /// .. first check which empty filler `CompositeCells` we need. 
            ///
            /// We use header to decide between CompositeCell.Term/CompositeCell.Unitized and CompositeCell.FreeText
            let relatedHeader = headers.[columnIndex]
            /// We use the first cell in the column to decide between CompositeCell.Term and CompositeCell.Unitized
            ///
            /// Not sure if we can add a better logic to infer if empty cells should be term or unitized ~Kevin F
            let tryExistingCell = if colKeys.IsEmpty then None else Some values.[colKeys.MinimumElement]
            let empty = Unchecked.getEmptyCellForHeader relatedHeader tryExistingCell
            for missingColumn,missingRow in missingKeys do
                Unchecked.setCellAt (missingColumn,missingRow,empty) values


let table() = 

    let name = "MyTable"
    let headers = ResizeArray [CompositeHeader.Input IOType.Sample;CompositeHeader.FreeText "Freetext1" ; CompositeHeader.FreeText "Freetext2"; CompositeHeader.Output IOType.Sample]
    let values = System.Collections.Generic.Dictionary()
    for i = 0 to 20000 do       
        if i%2 = 0 then
            Unchecked.setCellAt(0,i,(CompositeCell.FreeText $"Source_{i}")) values
            Unchecked.setCellAt(1,i,(CompositeCell.FreeText $"FT1_{i}")) values
            Unchecked.setCellAt(2,i,(CompositeCell.FreeText $"FT2_{i}")) values
            Unchecked.setCellAt(3,i,(CompositeCell.FreeText $"FT3_{i}")) values
            Unchecked.setCellAt(6,i,(CompositeCell.FreeText $"Sample_{i}")) values
        else
            Unchecked.setCellAt(0,i,(CompositeCell.FreeText $"Source_{i}")) values
            Unchecked.setCellAt(3,i,(CompositeCell.FreeText $"FT3_{i}")) values
            Unchecked.setCellAt(4,i,(CompositeCell.FreeText $"FT4_{i}")) values
            Unchecked.setCellAt(5,i,(CompositeCell.FreeText $"FT5_{i}")) values
            Unchecked.setCellAt(6,i,(CompositeCell.FreeText $"Sample_{i}")) values

    ArcTable(name, headers, values)
    
let prepareTables() =
    let t1 = table()
    let t2 = table()
    let t3 = table()
    let t4 = table()
    t1,t2,t3,t4


let firstF (t:ArcTable) =
    fillMissingOld t.Headers t.Values

let oldF (t:ArcTable) =
    fillMissingOld t.Headers t.Values

let newF (t:ArcTable) = 
    Unchecked.fillMissingCells t.Headers t.Values

let newSeqF (t:ArcTable) = 
    fillMissingCellsSeq t.Headers t.Values
