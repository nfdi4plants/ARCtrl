module FillMissing

open ARCtrl.ISA
open ArcTableAux

open System.Collections.Generic

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
    for i = 0 to 10000 do       
        if i%2 = 0 then
            Unchecked.setCellAt(0,i,(CompositeCell.FreeText $"Source_{i}")) values
            Unchecked.setCellAt(2,i,(CompositeCell.FreeText $"FT2_{i}")) values
        else
            
            Unchecked.setCellAt(1,i,(CompositeCell.FreeText $"FT1_{i}")) values
            Unchecked.setCellAt(3,i,(CompositeCell.FreeText $"Sample_{i}")) values

    ArcTable(name, headers, values)
    
let prepareTables() =
    let t1 = table()
    let t2 = table()
    t1,t2

let oldF (t:ArcTable) =
    fillMissingOld t.Headers t.Values

let newF (t:ArcTable) = 
    Unchecked.fillMissingCells t.Headers t.Values

