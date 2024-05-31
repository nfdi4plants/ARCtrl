module ARCtrl.ArcTableAux

open ARCtrl
open ARCtrl.Helper
open System.Collections.Generic
open Fable.Core

let getColumnCount (headers:ResizeArray<CompositeHeader>) = 
    headers.Count

let getRowCount (values:Dictionary<int*int,CompositeCell>) = 
    if values.Count = 0 then 0 else
        values.Keys |> Seq.maxBy snd |> snd |> (+) 1

let boxHashValues colCount (values:Dictionary<int*int,CompositeCell>) =
    let mutable hash = 0
    let rowCount = getRowCount values
    for col = 0 to colCount - 1 do
        for row = 0 to rowCount - 1 do
            hash <- 0x9e3779b9 + values.[col,row].GetHashCode() + (hash <<< 6) + (hash >>> 2)
    hash    
    |> box

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

    let validateColumn (column:CompositeColumn) = column.Validate(true) |> ignore

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

    let inline validateRowLength (newCells: seq<CompositeCell>) (columnCount: int) =
        let newCellsCount = newCells |> Seq.length
        match columnCount with
        | 0 ->
            failwith $"Table contains no columns! Cannot add row to empty table!"
        | unequal when newCellsCount <> columnCount ->
            failwith $"Cannot add a new row with {newCellsCount} cells, as the table has {columnCount} columns."
        | _ ->
            ()

    let validate (headers: ResizeArray<CompositeHeader>) (values: Dictionary<int*int,CompositeCell>) (raiseException: bool) =
        let mutable isValid = true
        let mutable en = values.GetEnumerator()
        while isValid && en.MoveNext() do
            let (ci,_),cell = en.Current.Key,en.Current.Value
            let header = headers.[ci]
            let headerIsData = header.IsDataColumn
            let headerIsFreetext = (not header.IsTermColumn) && (not header.IsDataColumn)
            let cellIsNotFreetext = not cell.isFreeText
            let cellIsNotData = not cell.isData
            if headerIsData && (cellIsNotData && cellIsNotFreetext) then 
                (if raiseException then failwith else printfn "%s") $"Invalid combination of header `{header}` and cell `{cell}`. Data header should contain either Data or Freetext cells."
                isValid <- false
            if headerIsFreetext && cellIsNotFreetext then 
                (if raiseException then failwith else printfn "%s") $"Invalid combination of header `{header}` and cell `{cell}`. Freetext header should not contain non-freetext cells."
                isValid <- false
        isValid

module Unchecked =
        
    let tryGetCellAt (column: int,row: int) (cells:System.Collections.Generic.Dictionary<int*int,CompositeCell>) = 
        Dictionary.tryFind (column, row) cells

    /// Add or update a cell in the dictionary.
    let setCellAt(columnIndex, rowIndex,c : CompositeCell) (cells:Dictionary<int*int,CompositeCell>) = 
        cells.[(columnIndex,rowIndex)] <- c

    /// Add a cell to the dictionary. If a cell already exists at the given position, it fails.
    let addCellAt(columnIndex, rowIndex,c : CompositeCell) (cells:Dictionary<int*int,CompositeCell>) = 
        cells.Add((columnIndex,rowIndex),c)

    let moveCellTo (fromCol:int,fromRow:int,toCol:int,toRow:int) (cells:Dictionary<int*int,CompositeCell>) =
        match Dictionary.tryFind (fromCol, fromRow) cells with
        | Some c ->
            // Remove value. This is necessary in the following scenario:
            //
            // "AddColumn.Existing Table.add less rows, insert at".
            //
            // Assume a table with 5 rows, insert column with 2 cells. All 5 rows at `index` position are shifted +1, but only row 0 and 1 are replaced with new values.
            // Without explicit removing, row 2..4 would stay as is.
            // EDIT: First remove then set cell to otherwise a `amount` = 0 would just remove the cell!
            cells.Remove((fromCol,fromRow)) |> ignore
            setCellAt(toCol,toRow,c) cells
            |> ignore
        | None -> ()

    let removeHeader (index:int) (headers:ResizeArray<CompositeHeader>) = headers.RemoveAt (index)

    /// Remove cells of one Column, change index of cells with higher index to index - 1
    let removeColumnCells (index:int) (cells:Dictionary<int*int,CompositeCell>) = 
        for KeyValue((c,r),_) in cells do
            // Remove cells of column
            if c = index then
                cells.Remove((c,r))
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
                    cells.Remove((col,row))
                    |> ignore
                // move to left if "column index > index"
                elif col > index then
                    moveCellTo(col,row,col-1,row) cells
                else
                    ()

    let removeRowCells (rowIndex:int) (cells:Dictionary<int*int,CompositeCell>) = 
        for KeyValue((c,r),_) in cells do
            // Remove cells of column
            if r = rowIndex then
                cells.Remove((c,r))
                |> ignore
            else
                ()

    /// Remove cells of one Row, change index of cells with higher index to index - 1
    let removeRowCells_withIndexChange (rowIndex:int) (columnCount:int) (rowCount:int) (cells:Dictionary<int*int,CompositeCell>) = 
        // Cannot loop over collection and change keys of existing.
        // Therefore we need to run over values between columncount and rowcount.
        for row = rowIndex to (rowCount-1) do
            for col = 0 to (columnCount-1) do
                if row = rowIndex then
                    cells.Remove((col,row))
                    |> ignore
                // move to top if "row index > index"
                elif row > rowIndex then
                    moveCellTo(col,row,col,row-1) cells
                else
                    ()

    /// Get an empty cell fitting for related column header.
    ///
    /// `columCellOption` is used to decide between `CompositeCell.Term` or `CompositeCell.Unitized`. `columCellOption` can be any other cell in the same column, preferably the first one.
    let getEmptyCellForHeader (header:CompositeHeader) (columCellOption: CompositeCell option) =
        match header.IsTermColumn with
        | false                                 -> CompositeCell.emptyFreeText
        | true ->
            match columCellOption with
            | Some (CompositeCell.Term _) 
            | None                              -> CompositeCell.emptyTerm
            | Some (CompositeCell.Unitized _)   -> CompositeCell.emptyUnitized
            | _                                 -> failwith "[extendBodyCells] This should never happen, IsTermColumn header must be paired with either term or unitized cell."

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newHeader"></param>
    /// <param name="newCells"></param>
    /// <param name="index"></param>
    /// <param name="forceReplace"></param>
    /// <param name="onlyHeaders">If set to true, no values will be added</param>
    /// <param name="headers"></param>
    /// <param name="values"></param>
    let addColumn (newHeader: CompositeHeader) (newCells: CompositeCell []) (index: int) (forceReplace: bool) (onlyHeaders: bool) (headers: ResizeArray<CompositeHeader>) (values:Dictionary<int*int,CompositeCell>) =
        let mutable numberOfNewColumns = 1
        let mutable index = index
        /// If this isSome and the function does not raise exception we are executing a forceReplace.
        let hasDuplicateUnique = tryFindDuplicateUnique newHeader headers
        // implement fail if unique column should be added but exists already
        if not forceReplace && hasDuplicateUnique.IsSome then failwith $"Invalid new column `{newHeader}`. Table already contains header of the same type on index `{hasDuplicateUnique.Value}`"
        // Example: existingCells contains `Output io` (With io being any IOType) and header is `Output RawDataFile`. This should replace the existing `Output io`.
        // In this case the number of new columns drops to 0 and we insert the index of the existing `Output io` column.
        if hasDuplicateUnique.IsSome then
            numberOfNewColumns <- 0
            index <- hasDuplicateUnique.Value
        /// This ensures nothing gets messed up during mutable insert, for example inser header first and change ColumCount in the process
        let startColCount, startRowCount = getColumnCount headers, getRowCount values
        // headers are easily added. Just insert at position of index. This will insert without replace.
        let setNewHeader() = 
            // if duplication found and we want to forceReplace we remove related header
            if hasDuplicateUnique.IsSome then
                removeHeader(index) headers
            headers.Insert(index, newHeader)
        /// For all columns with index >= we need to increase column index by `numberOfNewColumns`.
        /// We do this by moving all these columns one to the right with mutable dictionary set logic (cells.[key] <- newValue), 
        /// Therefore we need to start with the last column to not overwrite any values we still need to shift
        let increaseColumnIndices() =
            /// Get last column index
            let lastColumnIndex = System.Math.Max(startColCount - 1, 0) // If there are no columns. We get negative last column index. In this case just return 0.
            // start with last column index and go down to `index`
            for columnIndex = lastColumnIndex downto index do
                for rowIndex in 0 .. startRowCount do
                    moveCellTo(columnIndex,rowIndex,columnIndex+numberOfNewColumns,rowIndex) values
        /// Then we can set the new column at `index`
        let setNewCells() =
            // Not sure if this is intended? If we for example `forceReplace` a single column table with `Input`and 5 rows with a new column of `Input` ..
            // ..and only 2 rows, then table RowCount will decrease from 5 to 2.
            // Related Test: `All.ArcTable.addColumn.Existing Table.add less rows, replace input, force replace
            if hasDuplicateUnique.IsSome then
                removeColumnCells(index) values
            let f = 
                if index >= startColCount then 
                    fun (colIndex,rowIndex,cell) (values : Dictionary<int*int,CompositeCell>) -> 
                        values.Add((colIndex,rowIndex),cell) |> ignore
                else 
                   setCellAt
            newCells 
            |> Array.iteri (fun rowIndex cell ->
                let columnIndex = index
                f (columnIndex,rowIndex,cell) values
            )
        setNewHeader()
        // Only do this if column is inserted and not appended AND we do not execute forceReplace!
        if index < startColCount && hasDuplicateUnique.IsNone then
            increaseColumnIndices()
        // 
        if not onlyHeaders then 
            setNewCells()
        ()

    // We need to calculate the max number of rows between the new columns and the existing columns in the table.
    // `maxRows` will be the number of rows all columns must have after adding the new columns.
    // This behaviour should be intuitive for the user, as Excel handles this case in the same way.
    let fillMissingCells (headers: ResizeArray<CompositeHeader>) (values:Dictionary<int*int,CompositeCell>) =
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
                let defaultCell = getEmptyCellForHeader header firstCell
                let rowKeys = Array.map snd col |> Set.ofArray
                for rowIndex = 0 to rowCount - 1 do
                    if not <| rowKeys.Contains rowIndex then
                        addCellAt (columnIndex,rowIndex,defaultCell) values
            // No values existed in this column. Fill with default cells
            | None ->
                let defaultCell = getEmptyCellForHeader header None
                for rowIndex = 0 to rowCount - 1 do
                    addCellAt (columnIndex,rowIndex,defaultCell) values


    /// Increases the table size to the given new row count and fills the new rows with the last value of the column
    let extendToRowCount rowCount (headers: ResizeArray<CompositeHeader>) (values:Dictionary<int*int,CompositeCell>) =
        let columnCount = getColumnCount headers
        let previousRowCount = getRowCount values
        // iterate over columns
        for columnIndex = 0 to columnCount - 1 do
            let lastValue = values[columnIndex,previousRowCount-1]
            for rowIndex = previousRowCount - 1 to rowCount - 1 do
                setCellAt (columnIndex,rowIndex,lastValue) values

    let addRow (index:int) (newCells:CompositeCell []) (headers: ResizeArray<CompositeHeader>) (values:Dictionary<int*int,CompositeCell>) =
        /// Store start rowCount here, so it does not get changed midway through
        let rowCount = getRowCount values
        let columnCount = getColumnCount headers
        let increaseRowIndices =  
            // Only do this if column is inserted and not appended!
            if index < rowCount then
                /// Get last row index
                let lastRowIndex = System.Math.Max(rowCount - 1, 0) // If there are no rows. We get negative last column index. In this case just return 0.
                // start with last row index and go down to `index`
                for rowIndex = lastRowIndex downto index do
                    for columnIndex in 0 .. (columnCount-1) do
                        moveCellTo(columnIndex,rowIndex,columnIndex,rowIndex+1) values
        /// Then we can set the new row at `index`
        let setNewCells =
            newCells |> Array.iteri (fun columnIndex cell ->
                let rowIndex = index
                setCellAt (columnIndex,rowIndex,cell) values
            )
        ()

    let addRows (index:int) (newRows:CompositeCell [][]) (headers: ResizeArray<CompositeHeader>) (values:Dictionary<int*int,CompositeCell>) =
        /// Store start rowCount here, so it does not get changed midway through
        let rowCount = getRowCount values
        let columnCount = getColumnCount headers
        let numNewRows = newRows.Length
        let increaseRowIndices =  
            // Only do this if column is inserted and not appended!
            if index < rowCount then
                /// Get last row index
                let lastRowIndex = System.Math.Max(rowCount - 1, 0) // If there are no rows. We get negative last column index. In this case just return 0.
                // start with last row index and go down to `index`
                for rowIndex = lastRowIndex downto index do
                    for columnIndex in 0 .. (columnCount-1) do
                        moveCellTo(columnIndex,rowIndex,columnIndex,rowIndex+numNewRows) values
        let mutable currentRowIndex = index
        for row in newRows do
            /// Then we can set the new row at `index`
            let setNewCells =
                row |> Array.iteri (fun columnIndex cell ->
                    setCellAt (columnIndex,currentRowIndex,cell) values
                )
            currentRowIndex <- currentRowIndex + 1

            
    /// Returns true, if two composite headers share the same main header string
    let compositeHeaderMainColumnEqual (ch1 : CompositeHeader) (ch2 : CompositeHeader) = 
        ch1.ToString() = ch2.ToString()

    /// Moves a column from one position to another
    ///
    /// This function moves the column from `fromCol` to `toCol` and shifts all columns in between accordingly
    let moveColumnTo (rowCount : int) (fromCol:int) (toCol:int) (headers : ResizeArray<CompositeHeader>) (values:Dictionary<int*int,CompositeCell>) =       
        // Shift describes the moving of all the cells that are between fromCol and toCol
        let shift, shiftStart, shiftEnd = 
            if fromCol < toCol then
                -1, fromCol + 1, toCol               
            else
                1, fromCol - 1, toCol

        // Remember the header of interest (at fromCol)
        let header = headers.[fromCol]
        // Shift all headers  between fromCol and toCol
        if shiftStart < shiftEnd then           
            for c = shiftStart to shiftEnd do
                headers.[c + shift] <- headers.[c]
        else 
            for c = shiftStart downto shiftEnd do
                headers.[c + shift] <- headers.[c]
        // Set the column of interest to the new position
        headers.[toCol] <- header

        // Iterate rowWise
        for r = 0 to rowCount - 1 do
            // Remember the cell of interest (at fromCol)
            let cell = values.[fromCol,r]
            // Shift all cells between fromCol and toCol
            if shiftStart < shiftEnd then
                for c = shiftStart to shiftEnd do
                    values.[(c + shift,r)] <- values.[(c,r)]
            else
                for c = shiftStart downto shiftEnd do
                    values.[(c + shift,r)] <- values.[(c,r)]
            // Set the cell of interest to the new position
            values.[(toCol,r)] <- cell

    /// From a list of rows consisting of headers and values, creates a list of combined headers and the values as a sparse matrix
    ///
    /// The values cant be directly taken as they are, as there is no guarantee that the headers are aligned
    ///
    /// This function aligns the headers and values by the main header string
    ///
    /// If keepOrder is true, the order of values per row is kept intact, otherwise the values are allowed to be reordered
    let alignByHeaders (keepOrder : bool) (rows : ((CompositeHeader * CompositeCell) list) list) = 
        let headers : ResizeArray<CompositeHeader> = ResizeArray()
        let values : Dictionary<int*int,CompositeCell> = Dictionary()
        let getFirstElem (rows : ('T list) list) : 'T =
            List.pick (fun l -> if List.isEmpty l then None else List.head l |> Some) rows
        let rec loop colI (rows : ((CompositeHeader * CompositeCell) list) list) =
            if List.exists (List.isEmpty >> not) rows |> not then 
                headers,values
            else 
                
                let firstElem = rows |> getFirstElem |> fst
                headers.Add firstElem
                let rows = 
                    rows
                    |> List.mapi (fun rowI l ->
                        if keepOrder then                           
                            match l with
                            | [] -> []
                            | (h,c)::t ->
                                if compositeHeaderMainColumnEqual h firstElem then
                                    values.Add((colI,rowI),c)
                                    t
                                else
                                    l
                                
                        else
                            let firstMatch,newL = 
                                l
                                |> List.tryPickAndRemove (fun (h,c) ->
                                    if compositeHeaderMainColumnEqual h firstElem then Some c 
                                    else None
                                )
                            match firstMatch with
                            | Some m -> 
                                values.Add((colI,rowI),m)
                                newL
                            | None -> newL
                    )
                loop (colI+1) rows
        loop 0 rows