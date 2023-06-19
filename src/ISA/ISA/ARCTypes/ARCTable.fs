namespace ISA

open Fable.Core
open System.Collections.Generic
//open FSharpAux

module ArcTableAux =

    // Taken from FSharpAux.Core
    /// .Net Dictionary
    module Dictionary = 
    
        open System
        open System.Collections.Generic

        /// <summary>Returns the dictionary with the binding added to the given dictionary.
        /// If a binding with the given key already exists in the input dictionary, the existing binding is replaced by the new binding in the result dictionary.</summary>
        /// <param name="key">The input key.</param>
        /// <returns>The dictionary with change in place.</returns>
        let addOrUpdateInPlace key value (table:IDictionary<_,_>) =
            match table.ContainsKey(key) with
            | true  -> table.[key] <- value
            | false -> table.Add(key,value)
            table

        /// <summary>Lookup an element in the dictionary, returning a <c>Some</c> value if the element is in the domain 
        /// of the dictionary and <c>None</c> if not.</summary>
        /// <param name="key">The input key.</param>
        /// <returns>The mapped value, or None if the key is not in the dictionary.</returns>
        let tryFind key (table:IDictionary<_,_>) =
            match table.ContainsKey(key) with
            | true -> Some table.[key]
            | false -> None

    let getColumnCount (headers:ResizeArray<CompositeHeader>) = 
        headers.Count

    let getRowCount (values:Dictionary<int*int,CompositeCell>) = 
        if values.Count = 0 then 0 else
            values.Keys |> Seq.maxBy snd |> snd |> (+) 1

    // TODO: Move to CompositeHeader?
    let (|IsUniqueExistingHeader|_|) existingHeaders (input: CompositeHeader) = 
        match input with
        | CompositeHeader.Parameter _
        | CompositeHeader.Factor _
        | CompositeHeader.Characteristic _
        | CompositeHeader.Component _
        | CompositeHeader.FreeText _        -> None
        // Input and Output does not look very clean :/
        | CompositeHeader.Output _          -> Seq.tryFindIndex (fun h -> match h with | CompositeHeader.Output _ -> true | _ -> false) existingHeaders
        | CompositeHeader.Input _           -> Seq.tryFindIndex (fun h -> match h with | CompositeHeader.Input _ -> true | _ -> false) existingHeaders
        | header                            -> Seq.tryFindIndex (fun h -> h = header) existingHeaders
        
    // TODO: Move to CompositeHeader?
    /// Returns the column index of the duplicate unique column in `existingHeaders`.
    let tryFindDuplicateUnique (newHeader: CompositeHeader) (existingHeaders: seq<CompositeHeader>) = 
        match newHeader with
        | IsUniqueExistingHeader existingHeaders index -> Some index
        | _ -> None

    let inline failWithDuplicateUnique () = 
        failwith "Found duplicate unique columns in `columns`."

    /// Returns the column index of the duplicate unique column in `existingHeaders`.
    let tryFindDuplicateUniqueInArray (existingHeaders: seq<CompositeHeader>) = 
        let rec loop i (duplicateList: {|Index1: int; Index2: int; HeaderType: CompositeHeader|} list) (headerList: CompositeHeader list) =
            match headerList with
            | _ :: [] | [] -> duplicateList
            | header :: tail -> 
                let hasDuplicate = tryFindDuplicateUnique header tail
                let nextDuplicateList = if hasDuplicate.IsSome then {|Index1 = i; Index2 = hasDuplicate.Value; HeaderType = header|}::duplicateList else duplicateList
                loop (i+1) nextDuplicateList tail
        existingHeaders
        |> Seq.filter (fun x -> not x.IsTermColumn)
        |> List.ofSeq
        |> loop 0 []

    module SanityChecks =

        /// Checks if given column index is valid for given number of columns.
        ///
        /// if `allowAppend` = true => `0 < index <= columnCount`
        /// 
        /// if `allowAppend` = false => `0 < index < columnCount`
        let validateColumnIndex (index:int) (columnCount:int) (allowAppend:bool) =
            let eval x y = if allowAppend then x > y else x >= y
            if index < 0 then failwith "Cannot insert CompositeColumn at index < 0."
            if eval index columnCount then failwith $"Specified index is out of table range! Table contains only {columnCount} columns."

        /// Checks if given index is valid for given number of rows.
        ///
        /// if `allowAppend` = true => `0 < index <= rowCount`
        /// 
        /// if `allowAppend` = false => `0 < index < rowCount`
        let validateRowIndex (index:int) (rowCount:int) (allowAppend:bool) =
            let eval x y = if allowAppend then x > y else x >= y
            if index < 0 then failwith "Cannot insert CompositeColumn at index < 0."
            if eval index rowCount then failwith $"Specified index is out of table range! Table contains only {rowCount} rows."

        let validateColumn (column:CompositeColumn) = column.validate(true) |> ignore

        let inline validateNoDuplicateUniqueColumns (columns:seq<CompositeColumn>) =
            let duplicates = columns |> Seq.map (fun x -> x.Header) |> tryFindDuplicateUniqueInArray
            if not <| List.isEmpty duplicates then
                let baseMsg = "Found duplicate unique columns in `columns`."
                let sb = System.Text.StringBuilder()
                sb.AppendLine(baseMsg) |> ignore
                duplicates |> List.iter (fun (x: {| HeaderType: CompositeHeader; Index1: int; Index2: int |}) -> 
                    sb.AppendLine($"Duplicate `{x.HeaderType}` at index {x.Index1} and {x.Index2}.")
                    |> ignore
                )
                let msg = sb.ToString()
                failwith msg

        let inline validateNoDuplicateUnique (header: CompositeHeader) (columns:seq<CompositeHeader>) =
            match tryFindDuplicateUnique header columns with
            | None -> ()
            | Some i -> failwith $"Invalid input. Tried setting unique header `{header}`, but header of same type already exists at index {i}."

    module Unchecked =
        
        let tryGetCellAt (column: int,row: int) (cells:Dictionary<int*int,CompositeCell>) = Dictionary.tryFind (column, row) cells
        let setCellAt(columnIndex, rowIndex,c : CompositeCell) (cells:Dictionary<int*int,CompositeCell>) = Dictionary.addOrUpdateInPlace (columnIndex,rowIndex) c cells |> ignore
        let moveCellTo (fromCol:int,fromRow:int,toCol:int,toRow:int) (cells:Dictionary<int*int,CompositeCell>) =
            match Dictionary.tryFind (fromCol, fromRow) cells with
            | Some c ->
                // Dictionary.addOrUpdateInPlace (column+amount,row) c this.Values |> ignore // This is the same as `SetCellAt`
                // Remove value. This is necessary in the following scenario:
                //
                // "AddColumn.Existing Table.add less rows, insert at".
                //
                // Assume a table with 5 rows, insert column with 2 cells. All 5 rows at `index` position are shifted +1, but only row 0 and 1 are replaced with new values.
                // Without explicit removing, row 2..4 would stay as is.
                // EDIT: First remove then set cell to otherwise a `amount` = 0 would just remove the cell!
                cells.Remove(fromCol,fromRow)
                |> ignore
                setCellAt(toCol,toRow,c) cells
                |> ignore
            | None -> ()
        let removeHeader (index:int) (headers:ResizeArray<CompositeHeader>) = headers.RemoveAt (index)
        /// Remove cells of one Column, change index of cells with higher index to index - 1
        let removeColumnCells (index:int) (cells:Dictionary<int*int,CompositeCell>) = 
            for KeyValue((c,r),_) in cells do
                // Remove cells of column
                if c = index then
                    cells.Remove(c,r)
                    |> ignore
                else
                    ()
        /// Remove cells of one Column, change index of cells with higher index to index - 1
        let removeColumnCells_withIndexChange (index:int) (columnCount:int) (rowCount:int) (cells:Dictionary<int*int,CompositeCell>) = 
            // Cannot loop over collection and change keys of existing.
            // Therefore we need to run over values between columncount and rowcount.
            for col = index to (columnCount-1) do
                for row = 0 to (rowCount-1) do
                    if col = index then
                        cells.Remove(col,row)
                        |> ignore
                    // move to left if "column index > index"
                    elif col > index then
                        moveCellTo(col,row,col-1,row) cells
                    else
                        ()

open ArcTableAux

[<AttachMembers>]
type ArcTable = 
    {
        Name : string
        Headers : ResizeArray<CompositeHeader>
        /// Key: Column * Row
        Values : System.Collections.Generic.Dictionary<int*int,CompositeCell>  
    }

    static member create(name, headers, values) =
        {
            Name = name
            Headers = headers
            Values = values

        }

    /// Create ArcTable with empty 'ValueHeader' and 'Values' 
    static member init(name: string) = {
        Name = name
        Headers = ResizeArray<CompositeHeader>()
        Values = System.Collections.Generic.Dictionary<int*int,CompositeCell>()
    }

    /// Returns a cell at given position if it exists, else returns None.
    member this.TryGetCellAt (column: int,row: int) = ArcTableAux.Unchecked.tryGetCellAt (column,row) this.Values
    
    // 
    member this.SetCellAt(columnIndex, rowIndex,c : CompositeCell) =
        SanityChecks.validateColumnIndex columnIndex this.ColumnCount false
        SanityChecks.validateRowIndex rowIndex this.RowCount false
        SanityChecks.validateColumn <| CompositeColumn.create(this.Headers.[columnIndex],[|c|])
        Unchecked.setCellAt(columnIndex, rowIndex,c) this.Values

    static member setCellAt(columnIndex: int, rowIndex: int, cell: CompositeCell) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.SetCellAt(columnIndex,rowIndex,cell)
            newTable
     
    // TODO: No error checking if new position is valid for body cells. If we want to give a streamlined API this subfunction should be placed in ArcTableAux and not as member without sanity checks. 
    // TODO: This is just SetHeader?
    //member this.OverwriteHeader(header: CompositeHeader, index : int) =
    //    ArcTableAux.SanityChecks.validateColumnIndex index this.ColumnCount false
    //    this.Headers.[index] <- header

    // TODO: This is just SetColumn?
    //member this.OverwriteColumn(header: CompositeHeader, cells : CompositeCell [], index : int) =
    //    ArcTableAux.SanityChecks.validateColumnIndex index this.ColumnCount false
    //    ArcTableAux.SanityChecks.validateColumn <| CompositeColumn.create(header, cells)
    //    this.Headers.[index] <- header
    //    cells
    //    |> Array.iteri (fun i c ->  Unchecked.setCellAt (index,i,c) this.Values)
          
    ///
    static member private insertRawColumn (newHeader: CompositeHeader) (newCells: CompositeCell []) (index: int) (forceReplace: bool) (table:ArcTable) =
        let mutable numberOfNewColumns = 1
        let mutable index = index
        /// If this isSome and the function does not raise exception we are executing a forceReplace.
        let hasDuplicateUnique = ArcTableAux.tryFindDuplicateUnique newHeader table.Headers
        // implement fail if unique column should be added but exists already
        if not forceReplace && hasDuplicateUnique.IsSome then failwith $"Invalid new column `{newHeader}`. Table already contains header of the same type on index `{hasDuplicateUnique.Value}`"
        // Example: existingCells contains `Output io` (With io being any IOType) and header is `Output RawDataFile`. This should replace the existing `Output io`.
        // In this case the number of new columns drops to 0 and we insert the index of the existing `Output io` column.
        if hasDuplicateUnique.IsSome then
            numberOfNewColumns <- 0
            index <- hasDuplicateUnique.Value
        /// This ensures nothing gets messed up during mutable insert, for example inser header first and change ColumCount in the process
        let startColCount, startRowCount = table.ColumnCount, table.RowCount
        // headers are easily added. Just insert at position of index. This will insert without replace.
        let setNewHeader = 
            // if duplication found and we want to forceReplace we remove related header
            if hasDuplicateUnique.IsSome then
                Unchecked.removeHeader(index) table.Headers
            table.Headers.Insert(index, newHeader)
        /// For all columns with index >= we need to increase column index by `numberOfNewColumns`.
        /// We do this by moving all these columns one to the right with mutable dictionary set logic (cells.[key] <- newValue), 
        /// Therefore we need to start with the last column to not overwrite any values we still need to shift
        let increaseColumnIndices =
            // Only do this if column is inserted and not appended AND we do not execute forceReplace!
            if index < startColCount && hasDuplicateUnique.IsNone then
                /// Get last column index
                let lastColumnIndex = System.Math.Max(startColCount - 1, 0) // If there are no columns. We get negative last column index. In this case just return 0.
                // start with last column index and go down to `index`
                for columnIndex = lastColumnIndex downto index do
                    for rowIndex in 0 .. startRowCount do
                        Unchecked.moveCellTo(columnIndex,rowIndex,columnIndex+numberOfNewColumns,rowIndex) table.Values
        /// Then we can set the new column at `index`
        let setNewCells =
            // Not sure if this is intended? If we for example `forceReplace` a single column table with `Input`and 5 rows with a new column of `Input` ..
            // ..and only 2 rows, then table RowCount will decrease from 5 to 2.
            // Related Test: `All.ArcTable.addColumn.Existing Table.add less rows, replace input, force replace
            if hasDuplicateUnique.IsSome then
                Unchecked.removeColumnCells(index) table.Values
            newCells |> Array.iteri (fun rowIndex cell ->
                let columnIndex = index
                Unchecked.setCellAt (columnIndex,rowIndex,cell) table.Values
            )
        ()

    // We need to calculate the max number of rows between the new columns and the existing columns in the table.
    // `maxRows` will be the number of rows all columns must have after adding the new columns.
    // This behaviour should be intuitive for the user, as Excel handles this case in the same way.
    static member private extendBodyCells (table:ArcTable) =
        let maxRows = table.RowCount
        let lastColumnIndex = table.ColumnCount - 1
        /// Get all keys, to map over relevant rows afterwards
        let keys = table.Values.Keys
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
                let relatedHeader = table.Headers.[columnIndex]
                let empty = 
                    match relatedHeader.IsTermColumn with
                    | false                                 -> CompositeCell.emptyFreeText
                    | true ->
                        /// We use the first cell in the column to decide between CompositeCell.Term and CompositeCell.Unitized
                        ///
                        /// Not sure if we can add a better logic to infer if empty cells should be term or unitized ~Kevin F
                        let tryExistingCell = if colKeys.IsEmpty then None else Some table.Values.[colKeys.MinimumElement]
                        match tryExistingCell with
                        | Some (CompositeCell.Term _) 
                        | None                              -> CompositeCell.emptyTerm
                        | Some (CompositeCell.Unitized _)   -> CompositeCell.emptyUnitized
                        | _                                 -> failwith "[extendBodyCells] This should never happen, IsTermColumn header must be paired with either term or unitized cell."
                for missingColumn,missingRow in missingKeys do
                    Unchecked.setCellAt (missingColumn,missingRow,empty) table.Values

    //member this.AppendColumn(header: CompositeHeader,cells: CompositeCell [],?forceReplace: bool) =
    //    let forceReplace = defaultArg forceReplace false
    //    match ArcTableAux.tryFindDuplicateUnique header this.Headers with
    //    | Some i when forceReplace -> this.OverwriteColumn(header,cells,i)
    //    | Some _ -> ArcTableAux.failWithDuplicateUnique()
    //    | None -> 
    //        cells
    //        |> Array.iteri (fun i c -> this.SetCellAt(this.ColumnCount,i,c))
    //        this.Headers.Add(header)

    //member this.AppendEmptyColumn(header: CompositeHeader,?forceReplace: bool) =
    //    this.AppendColumn(header, [||], ?forceReplace = forceReplace)
        

    member this.ColumnCount 
        with get() = ArcTableAux.getColumnCount this.Headers

    member this.RowCount 
        with get() = ArcTableAux.getRowCount this.Values

    member this.Copy() : ArcTable = 
        ArcTable.create(
            this.Name,
            ResizeArray(this.Headers), 
            Dictionary(this.Values)
        )

    member this.AddColumn (header:CompositeHeader, ?cells: CompositeCell [], ?index: int, ?forceReplace: bool) : unit = 
        let index = defaultArg index this.ColumnCount
        let cells = defaultArg cells [||]
        let forceReplace = defaultArg forceReplace false 
        SanityChecks.validateColumnIndex index this.ColumnCount true
        SanityChecks.validateColumn(CompositeColumn.create(header, cells))
        // 
        ArcTable.insertRawColumn header cells index forceReplace this
        ArcTable.extendBodyCells this
    //member this.AddColumn(header: CompositeHeader,?cells: CompositeCell [], ?index,?forceReplace: bool) =
    //    match cells,index with
    //    | Some c, Some i -> this.InsertColumn(header,c,i,?forceReplace = forceReplace)
    //    | Some c, None -> this.AppendColumn(header,c,?forceReplace = forceReplace)
    //    | None, Some i -> this.InsertEmptyColumn(header,i,?forceReplace = forceReplace)
    //    | None,None -> this.AppendEmptyColumn(header,?forceReplace = forceReplace)
    //    ArcTable.extendBodyCells this

    static member addColumn (header: CompositeHeader,cells: CompositeCell [],?Index: int ,?ForceReplace : bool) : (ArcTable -> ArcTable) =
        fun (table: ArcTable) ->
            let newTable = table.Copy()
            newTable.AddColumn(header, cells, ?index = Index)
            newTable

    member this.InsertColumn (header:CompositeHeader, index: int, ?cells: CompositeCell []) =
        this.AddColumn(header, index = index,?cells = cells, forceReplace = false)

    static member insertColumn (header:CompositeHeader, index: int, ?cells: CompositeCell []) =
        fun (table: ArcTable) ->
            let newTable = table.Copy()
            newTable.InsertColumn(header, index, ?cells = cells)
            newTable
    //// TODO: I think this function should not have a forceReplace option. With a specific usecase of a given `index` it seems unintuitive to allow forceReplace an another index.
    //member this.InsertColumn(header: CompositeHeader,cells: CompositeCell [],index: int,?forceReplace: bool) =
    //    let forceReplace = defaultArg forceReplace false
    //    ArcTableAux.SanityChecks.validateIndex index this.ColumnCount
    //    match ArcTableAux.tryFindDuplicateUnique header this.Headers with
    //    | Some i when forceReplace -> this.OverwriteColumn(header,cells,i)
    //    | Some _ -> ArcTableAux.failWithDuplicateUnique()
    //    | None -> 
    //        for rowI = 0 to this.RowCount - 1 do
                  // THIS DOES NOT WORK!? I thought so too, but after tests failed i checked and for-loops down must use `downto` or `(this.ColumnCount - 1) .. -1 .. index`
    //            for colI = this.ColumnCount - 1 to index do
    //                this.MoveCellAtHorizontally(colI,rowI,1)
    //        cells
    //        |> Array.iteri (fun rowI c ->               
    //            this.SetCellAt(index,rowI,c)           
    //        )
    //        this.Headers.Insert(index,header)
           
    //member this.InsertEmptyColumn(header: CompositeHeader,index: int,?forceReplace: bool) =
    //    this.InsertColumn(header, [||],index ,?forceReplace = forceReplace)

    member this.AppendColumn (header:CompositeHeader, ?cells: CompositeCell []) =
        this.AddColumn(header, ?cells = cells, index = this.ColumnCount, forceReplace = false)

    static member appendColumn (header:CompositeHeader, ?cells: CompositeCell []) =
        fun (table: ArcTable) ->
            let newTable = table.Copy()
            newTable.AppendColumn(header, ?cells = cells)
            newTable

    member this.AddColumns (columns: CompositeColumn [], ?index: int, ?forceReplace: bool) : unit = 
        let mutable index = defaultArg index this.ColumnCount
        let forceReplace = defaultArg forceReplace false
        SanityChecks.validateColumnIndex index this.ColumnCount true
        SanityChecks.validateNoDuplicateUniqueColumns columns
        columns |> Array.iter (fun x -> SanityChecks.validateColumn x)
        columns
        |> Array.iter (fun col -> 
            let prevHeadersCount = this.Headers.Count
            ArcTable.insertRawColumn col.Header col.Cells index forceReplace this
            // Check if more headers, otherwise `ArcTableAux.insertColumn` replaced a column and we do not need to increase index.
            if this.Headers.Count > prevHeadersCount then index <- index + 1
        )
        ArcTable.extendBodyCells this

    static member addColumns (columns: CompositeColumn [],?index: int) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.AddColumns(columns, ?index = index)
            newTable

    member this.RemoveColumn (index:int) =
        ArcTableAux.SanityChecks.validateColumnIndex index this.ColumnCount false
        /// Set ColumnCount here to avoid changing columnCount by changing header count
        let columnCount = this.ColumnCount
        let removeHeader = Unchecked.removeHeader(index) this.Headers
        let removeCells = Unchecked.removeColumnCells_withIndexChange(index) columnCount this.RowCount this.Values
        ()

    static member removeColumn (index:int) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.RemoveColumn(index)
            newTable

    member this.RemoveColumns(indexArr:int []) =
        // Sanity check here too, to avoid removing things from mutable to fail in the middle
        Array.iter (fun index -> SanityChecks.validateColumnIndex index this.ColumnCount false) indexArr
        Array.iter (fun index -> this.RemoveColumn index) indexArr

    static member removeColumns(indexArr:int []) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.RemoveColumns(indexArr)
            newTable

    member this.GetRow(rowIndex : int) =
        SanityChecks.validateRowIndex rowIndex this.RowCount false
        [|
            for i = 0 to this.ColumnCount - 1 do 
                this.TryGetCellAt(i, rowIndex).Value
        |]       
        
    static member getRow (index:int) = 
        fun (table:ArcTable) ->
            table.GetRow(index)
        
    member this.GetColumn(columnIndex:int) =
        SanityChecks.validateColumnIndex columnIndex this.ColumnCount false
        let h = this.Headers.[columnIndex]
        let cells = [|
            for i = 0 to this.RowCount - 1 do 
                this.TryGetCellAt(columnIndex, i).Value
        |]
        CompositeColumn.create(h, cells)

    static member getColumn (index:int) = 
        fun (table:ArcTable) ->
            table.GetColumn(index)

    member this.SetColumn (columnIndex:int, column:CompositeColumn) =
        SanityChecks.validateColumnIndex columnIndex this.ColumnCount false
        SanityChecks.validateColumn(column)
        /// remove to be replaced header, this is only used to check if any OTHER header is of the same unique type as column.Header
        /// MUST USE "Seq.removeAt" to not remove in mutable object!
        let otherHeaders = this.Headers |> Seq.removeAt columnIndex
        SanityChecks.validateNoDuplicateUnique column.Header otherHeaders
        Unchecked.removeHeader columnIndex this.Headers
        Unchecked.removeColumnCells columnIndex this.Values
        let nextHeader = 
            this.Headers.Insert(columnIndex,column.Header)
        let nextBody =
            column.Cells |> Array.iteri (fun rowIndex v -> Unchecked.setCellAt(columnIndex,rowIndex,v) this.Values)
        ArcTable.extendBodyCells(this)
        ()

    static member setColumn (columnIndex:int, column:CompositeColumn) = 
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.SetColumn(columnIndex, column)
            newTable
        
    member this.SetHeader (index:int, newHeader: CompositeHeader, ?forceConvertCells: bool) =
        let forceConvertCells = Option.defaultValue false forceConvertCells
        ArcTableAux.SanityChecks.validateColumnIndex index this.ColumnCount false
        /// remove to be replaced header, this is only used to check if any OTHER header is of the same unique type as column.Header
        /// MUST USE "Seq.removeAt" to not remove in mutable object!
        let otherHeaders = this.Headers |> Seq.removeAt index
        ArcTableAux.SanityChecks.validateNoDuplicateUnique newHeader otherHeaders
        let c = { this.GetColumn(index) with Header = newHeader }
        // Test if column is still valid with new header, if so insert header at index
        if c.validate() then
            let setHeader = this.Headers.[index] <- newHeader
            ()
        // if we force convert cells, we want to convert the existing cells to a valid cell type for the new header
        elif forceConvertCells then
            let convertedCells =
                match newHeader with
                | isTerm when newHeader.IsTermColumn -> c.Cells |> Array.map (fun c -> c.ToTermCell())
                | _ -> c.Cells |> Array.map (fun c -> c.ToFreeTextCell())
            let newColumn = CompositeColumn.create(newHeader,convertedCells)
            this.SetColumn(index, newColumn)
        else
            failwith "Tried setting header for column with invalid type of cells. Set `forceConvertCells` flag to automatically convert cells into valid CompositeCell type."

    static member setHeader (index:int, header:CompositeHeader) = 
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.SetHeader(index, header)
            newTable

    static member insertParameterValue (t : ArcTable) (p : ProcessParameterValue) : ArcTable = 
        raise (System.NotImplementedException())

    static member getParameterValues (t : ArcTable) : ProcessParameterValue [] = 
        raise (System.NotImplementedException())

    // no 
    static member addProcess = 
        raise (System.NotImplementedException())

    static member addRow input output values = //yes
        raise (System.NotImplementedException())

    static member getProtocols (t : ArcTable) : Protocol [] = 
        raise (System.NotImplementedException())

    static member getProcesses (t : ArcTable) : Process [] = 
        raise (System.NotImplementedException())

    static member fromProcesses (ps : Process array) : ArcTable = 
        raise (System.NotImplementedException())

    override this.ToString() =
        [
            $"Table: {this.Name}"
            "-------------"
            this.Headers |> Seq.map (fun x -> x.ToString()) |> String.concat "\t|\t"
            for rowI = 0 to this.RowCount-1 do
                this.GetRow(rowI) |> Seq.map (fun x -> x.ToString()) |> String.concat "\t|\t"
        ]
        |> String.concat "\n"