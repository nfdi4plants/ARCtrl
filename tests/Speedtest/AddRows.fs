module AddRows

open ARCtrl
open ARCtrl.Helper
open ArcTableAux


let addRowsOld (this:ArcTable) (rows:ResizeArray<ResizeArray<CompositeCell>>) (index:int option) = 

    let mutable index = defaultArg index this.RowCount
    // Sanity checks
    SanityChecks.validateRowIndex index this.RowCount true
    rows |> ResizeArray.iter (fun row -> SanityChecks.validateRowLength row this.ColumnCount)
    for row in rows do
        for columnIndex in 0 .. this.ColumnCount-1 do
            let h = this.Headers.[columnIndex]
            let column = CompositeColumn.create(h,ResizeArray [|row.[columnIndex]|])
            SanityChecks.validateColumn column
    rows
    |> ResizeArray.iter (fun row ->
        Unchecked.addRow index row this.Headers this.Values
        index <- index + 1
    )


let table() = ArcTable("MyTable",ResizeArray [CompositeHeader.Input IOType.Sample;CompositeHeader.FreeText "Freetext1" ; CompositeHeader.FreeText "Freetext2"; CompositeHeader.Output IOType.Sample])
let rows = 
        ResizeArray.init 10000 (fun i -> 
        ResizeArray [|CompositeCell.FreeText $"Source_{i}"; CompositeCell.FreeText $"FT1_{i}"; CompositeCell.FreeText $"FT2_{i}"; CompositeCell.FreeText $"Sample_{i}"; |])
   
let prepareTables() =
    let t1 = table()
    let t2 = table()
    t1,t2

let oldF (t) =
    addRowsOld t rows None
let newF (t:ArcTable) = 
    t.AddRows(rows)

