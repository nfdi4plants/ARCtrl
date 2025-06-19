module ARCtrl.ArcTableAux

open ARCtrl
open ARCtrl.Helper
open System.Collections.Generic
open Fable.Core

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


[<Erase>]
type ColumnValueRefs =
    | Constant of int
    | Sparse of Dictionary<int,int>

    member this.RowCount =
        match this with
        | Constant _ -> None
        | Sparse values ->
            values |> Seq.maxBy (fun kv -> kv.Key) |> Some

    member this.Copy() =
        match this with
        | Constant valueHash -> Constant valueHash
        | Sparse values ->
            let d = Dictionary<int, int>()
            values |> Seq.iter (fun v -> d.Add(v.Key, v.Value))
            Sparse d

    member this.AsSparse(rowCount : int) =
        match this with
        | Constant valueHash ->
            let d = Dictionary<int, int>()
            for i in 0 .. rowCount - 1 do
                d.Add(i, valueHash)
            Sparse d
        | Sparse values -> Sparse values

    static member fromCellColumn (column : ResizeArray<CompositeCell>, previousRowCount : int, valueMap: Dictionary<int, CompositeCell>) =
        let distinctValues = column |> ResizeArray.distinct
        let columnValues = distinctValues |> ResizeArray.map(fun cell ->
            let hash = cell.GetHashCode()
            if valueMap.ContainsKey hash |> not then
                valueMap.Add(hash, cell)
            hash
        )
        match columnValues.Count with
        | 1 when column.Count >= previousRowCount -> Constant(columnValues.[0])
        | 1 ->
            let d = Dictionary<int, int>()
            for i in 0 .. column.Count - 1 do
                d.Add(i, columnValues.[0])
            Sparse d
        | _ ->
            let d = Dictionary<int, int>()
            columnValues |> Seq.iteri (fun i v -> d.Add(i, v))            
            Sparse d

    member this.ToCellColumn(valueMap: Dictionary<int, CompositeCell>, rowCount : int, header : CompositeHeader) =
        match this with
        | Constant valueHash ->
            ResizeArray.create rowCount (valueMap.[valueHash])
        | Sparse values ->
            let defaultCell =
                if values.Count = 0 then
                    getEmptyCellForHeader header None
                else
                    let i = values.Keys |> Seq.max
                    let c = valueMap.[i]
                    getEmptyCellForHeader header (Some c)
            ResizeArray.init rowCount (fun i ->
                if values.ContainsKey i then
                    valueMap.[values.[i]]
                else
                    defaultCell
            )

//let __DEFAULT_TERM_HASH__ = CompositeCell.emptyTerm.GetHashCode()
//let __DEFAULT_DATA_HASH__ = CompositeCell.emptyData.GetHashCode()
//let __DEFAULT_FREETEXT_HASH__ = CompositeCell.emptyFreeText.GetHashCode()
//let __DEFAULT_UNITIZED_HASH__ = CompositeCell.emptyUnitized.GetHashCode()

//let initValueMap() =
//    let dict = Dictionary<int, CompositeCell>()
//    let emptyTerm = CompositeCell.emptyTerm
//    let emptyData = CompositeCell.emptyData
//    let emptyFreeText = CompositeCell.emptyFreeText
//    let emptyUnitized = CompositeCell.emptyUnitized
//    dict.Add(__DEFAULT_TERM_HASH__, emptyTerm)
//    dict.Add(__DEFAULT_DATA_HASH__, emptyData)
//    dict.Add(__DEFAULT_FREETEXT_HASH__, emptyFreeText)
//    dict.Add(__DEFAULT_UNITIZED_HASH__, emptyUnitized)
//    dict

/// <summary>
/// Helper type to hold the values of an ArcTable.
///
/// Composed of a list of `ColumnValueRefs` which reference the actual values in a dictionary.
/// </summary>
[<AttachMembers>]
type ArcTableValues (cols : Dictionary<int,ColumnValueRefs>, valueMap: Dictionary<int, CompositeCell>, rowCount : int) =

    let mutable _columns = cols
    let mutable _valueMap = valueMap
    let mutable _rowCount = rowCount

    member this.Columns
        with get() = _columns
        and internal set(columns) =
            _columns <- columns

    member this.ColumnCount
        with get() =
            if Seq.isEmpty _columns then 0
            else _columns.Keys |> Seq.max |> (+) 1

    member this.ValueMap
        with get() = _valueMap
        and internal set(valueMap) =
            _valueMap <- valueMap

    member this.RowCount
        with get() = _rowCount
        and internal set(rowCount) =
            _rowCount <- rowCount

    static member fromCellColumns (columns: ResizeArray<ResizeArray<CompositeCell>>) =       
        let rowCount =
            if columns.Count = 0 then 0
            else columns |> ResizeArray.map (fun c -> c.Count) |> Seq.max
        let valueMap = Dictionary<int, CompositeCell>() //initValueMap()
        let parsedColumns = Dictionary<int, ColumnValueRefs>()     
        columns |> Seq.iteri (fun i column ->
            let columnValueRefs = ColumnValueRefs.fromCellColumn(column, rowCount, valueMap)
            parsedColumns.Add(i, columnValueRefs)
        )      
        ArcTableValues(parsedColumns, valueMap, rowCount)

    static member init () =
        let valueMap = Dictionary<int, CompositeCell>() //initValueMap()
        let parsedColumns = Dictionary<int, ColumnValueRefs>()
        ArcTableValues(parsedColumns, valueMap, 0)

    member this.ToCellColumns (headers : ResizeArray<CompositeHeader>) =
        
        ResizeArray.init headers.Count (fun i ->
            let header = headers.[i]
            let column = _columns.[i]
            column.ToCellColumn(_valueMap, _rowCount, header)
        )

    member this.Copy() =
        let nextValueMap = Dictionary<int, CompositeCell>()
        _valueMap |> Seq.iter (fun kv -> nextValueMap.Add(kv.Key, kv.Value.Copy()))
        let nextColumns = Dictionary<int, ColumnValueRefs>()
        _columns |> Seq.iter (fun kv -> nextColumns.Add(kv.Key, kv.Value.Copy()))
        ArcTableValues(nextColumns, nextValueMap, _rowCount)

    override this.GetHashCode() =
        let mutable hash = 0
        for column in _columns do         
            match column.Value with
            | ColumnValueRefs.Constant valueHash ->
                for i = 0 to rowCount - 1 do
                    hash <- 0x9e3779b9 + i + (hash <<< 6) + (hash >>> 2)
                    hash <- 0x9e3779b9 + valueHash + (hash <<< 6) + (hash >>> 2)
            | ColumnValueRefs.Sparse values ->
                for kv in values do
                    hash <- 0x9e3779b9 + kv.Key + (hash <<< 6) + (hash >>> 2)
                    hash <- 0x9e3779b9 + kv.Value + (hash <<< 6) + (hash >>> 2)
        hash

    override this.Equals(other : obj) =
        match other with
        | :? ArcTableValues as other ->
            this.GetHashCode() = other.GetHashCode()
        | _ -> false

    interface IEnumerable<KeyValuePair<int*int,CompositeCell>> with
        member this.GetEnumerator() : IEnumerator<KeyValuePair<int*int,CompositeCell>> =
            let s = 
                seq {
                    for colI in 0 .. this.ColumnCount - 1 do
                        match IntDictionary.tryFind colI _columns with
                        | None -> ()
                        | Some col ->
                            match col with
                            | ColumnValueRefs.Constant valueHash ->
                                for rowI = 0 to _rowCount - 1 do
                                    
                                    yield KeyValuePair((colI, rowI), _valueMap.[valueHash])
                            | ColumnValueRefs.Sparse values ->
                                for rowI = 0 to _rowCount - 1 do
                                    if values.ContainsKey rowI then
                                        yield KeyValuePair((colI, rowI), _valueMap.[values.[rowI]])
                }
            s.GetEnumerator()

        member this.GetEnumerator(): System.Collections.IEnumerator = 
            (this :> IEnumerable<KeyValuePair<int*int,CompositeCell>>).GetEnumerator()

    member this.Item(crIndices : (int*int)) =
        let col, row = crIndices
        if row > this.RowCount - 1 then
            failwithf "Row index %d is out of bounds for ArcTableValues with row count %d." row this.RowCount
        if col > this.ColumnCount - 1 then
            failwithf "Column index %d is out of bounds for ArcTableValues with column count %d." col this.ColumnCount
        match IntDictionary.tryFind col _columns with
        | None -> failwithf "Column %d does not exist in ArcTableValues." col
        | Some colValueRefs ->
            match colValueRefs with
            | ColumnValueRefs.Constant valueHash ->
                _valueMap.[valueHash]
            | ColumnValueRefs.Sparse values ->
                if values.ContainsKey row then
                    _valueMap.[values.[row]]
                else
                    failwithf "Row value for index %d does not exist in column %d of ArcTableValues." row col

//let boxHashValues colCount (values:ArcTableValues) =
//    let mutable hash = 0
//    let rowCount = getRowCount values
//    for col = 0 to colCount - 1 do
//        for row = 0 to rowCount - 1 do
//            hash <- 0x9e3779b9 + values.[col,row].GetHashCode() + (hash <<< 6) + (hash >>> 2)
//    hash
//    |> box

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

    let validateCellColumns (headers: ResizeArray<CompositeHeader>) (columns: ResizeArray<ResizeArray<CompositeCell>>) (raiseException: bool) =
        let mutable isValid = true
        let mutable en = headers.GetEnumerator()
        let mutable colIndex = 0
        if headers.Count <> columns.Count then
            (if raiseException then failwith else printfn "%s") $"Invalid table. Number of headers ({headers.Count}) does not match number of columns ({columns.Count})."
            isValid <- false
        while isValid && en.MoveNext() do
            let header = en.Current
            let mutable colEn = columns.[colIndex].GetEnumerator()
            colIndex <- colIndex + 1
            while isValid && colEn.MoveNext() do
                let cell = colEn.Current
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

    let validateArcTableValues (headers: ResizeArray<CompositeHeader>) (values: ArcTableValues) (raiseException: bool) =
        printfn "fix this function for performance reasons"
        validateCellColumns headers (values.ToCellColumns(headers)) raiseException

module Unchecked =

    let ensureCellHashInValueMap (value: CompositeCell) (valueMap: Dictionary<int, CompositeCell>) =
        let hash = value.GetHashCode()
        if valueMap.ContainsKey hash then
            hash
        else
            // If value is not in the map, add it.
            valueMap.Add(hash, value)
            hash

    let tryGetCellAt (column: int,row: int) (values:ArcTableValues) : CompositeCell option =
        IntDictionary.tryFind column values.Columns
        |> Option.bind (fun col ->
            match col with
            | ColumnValueRefs.Constant valueHash ->
                Some values.ValueMap.[valueHash]
            | ColumnValueRefs.Sparse vals ->
                if vals.ContainsKey row then
                    Some (values.ValueMap.[vals.[row]])
                else
                    None
        )

    /// Add or update a cell in the dictionary.
    let setCellAt(columnIndex, rowIndex,c : CompositeCell) (values:ArcTableValues) =
        if rowIndex + 1 > values.RowCount then values.RowCount <- rowIndex + 1
        match IntDictionary.tryFind columnIndex values.Columns with
        | None ->           
            // If the column does not exist, we create a new one.
            let newColumn = Dictionary<int, int>()
            let hash = ensureCellHashInValueMap c values.ValueMap
            newColumn.Add(rowIndex, hash)
            values.Columns.Add(columnIndex, ColumnValueRefs.Sparse newColumn)
        | Some col  ->
            match col with
            | Constant valueHash ->              
                let hash = ensureCellHashInValueMap c values.ValueMap
                if hash = valueHash then
                    ()
                else
                    let d = Dictionary<int, int>()
                    for i in 0 .. values.RowCount - 1 do
                        if i = rowIndex then
                            d.Add(i, hash)
                        else
                            d.Add(i, valueHash) // Keep the old value for other rows
            | Sparse vals ->
                let hash = ensureCellHashInValueMap c values.ValueMap
                IntDictionary.addOrUpdate rowIndex hash vals      

    let removeCellAt (columnIndex, rowIndex) (values:ArcTableValues) =
        match IntDictionary.tryFind columnIndex values.Columns with
        | None -> ()
        | Some col ->
            match col with
            | Constant valueHash ->
                let d = Dictionary<int, int>()
                for i in 0 .. values.RowCount - 1 do
                    if i = rowIndex then
                        ()
                    else
                        d.Add(i, valueHash) // Keep the old value for other rows
            | Sparse vals ->
                if vals.ContainsKey rowIndex then
                    vals.Remove(rowIndex) |> ignore
                    // If the column is empty after removing the cell, we remove it.
                    if vals.Count = 0 then
                        values.Columns.Remove(columnIndex) |> ignore

    let moveCellTo (fromCol:int,fromRow:int,toCol:int,toRow:int) (values:ArcTableValues) =
        match tryGetCellAt (fromCol, fromRow) values with
        | Some cell ->
            // Remove the cell from the old position
            removeCellAt (fromCol, fromRow) values
            // Add the cell to the new position
            setCellAt (toCol, toRow, cell) values
        | None -> 
            // If there is no cell at the old position, we do nothing.
            ()

    let removeHeader (index:int) (headers:ResizeArray<CompositeHeader>) = headers.RemoveAt (index)

    /// Remove cells of one Column, change index of cells with higher index to index - 1
    let removeColumnCells (colIndex:int) (values:ArcTableValues) =
        values.Columns.Remove(colIndex) |> ignore

    /// Remove cells of one Column, change index of cells with higher index to index - 1
    let removeColumnCells_withIndexChange (colIndex:int) (columnCount : int) (values:ArcTableValues) =
        // Cannot loop over collection and change keys of existing.
        // Therefore we need to run over values between columncount and rowcount.
        
        if colIndex < columnCount - 1 then
            let cols = Dictionary<int, ColumnValueRefs>()
            values.Columns
            |> Seq.iter (fun kv ->
                let cI = kv.Key
                if cI > colIndex then
                    cols.Add(cI - 1, kv.Value)
                elif cI < colIndex then
                    cols.Add(cI, kv.Value)
            )
            values.Columns <- cols
        else
            removeColumnCells colIndex values |> ignore

    let removeRowCells (rowIndex:int) (columnCount : int) (values:ArcTableValues) =
        for c = 0 to columnCount - 1 do
            removeCellAt (c, rowIndex) values
        

    /// Remove cells of one Row, change index of cells with higher index to index - 1
    let removeRowCells_withIndexChange (rowIndex:int) (values:ArcTableValues) =
        if rowIndex = 0 && values.RowCount = 1 then
            values.RowCount <- 0
            values.Columns <- Dictionary()
        else
            values.RowCount <- values.RowCount - 1
            values.Columns
            |> Seq.iter (fun kv ->
                match kv.Value with
                | Constant _ -> ()
                | Sparse vals ->
                    if rowIndex < values.RowCount - 1 then
                        let col = Dictionary<int, int>()
                        vals
                        |> Seq.iter (fun kv ->
                            let rI = kv.Key
                            if rI > rowIndex then
                                col.Add(rI - 1, kv.Value)
                            elif rI < rowIndex then
                                col.Add(rI, kv.Value)
                        )
                        values.Columns.[kv.Key] <- ColumnValueRefs.Sparse col
                    else vals.Remove(rowIndex) |> ignore
            )
  
    let moveColumnCellsTo (fromCol:int) (toCol:int) (values:ArcTableValues) =
        match IntDictionary.tryFind fromCol values.Columns with
        | None -> ()
        | Some col ->
            removeColumnCells fromCol values
            IntDictionary.addOrUpdate toCol col values.Columns                   

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
    let addRefColumn (newHeader: CompositeHeader) (newCol: ColumnValueRefs) (rowCount : int) (index: int) (forceReplace: bool) (onlyHeaders: bool) (headers: ResizeArray<CompositeHeader>) (values:ArcTableValues) =
        let mutable numberOfNewColumns = 1
        let mutable index = index
        /// If this isSome and the function does not raise exception we are executing a forceReplace.
        let hasDuplicateUnique = tryFindDuplicateUnique newHeader headers
        // implement fail if unique column should be added but exists already
        if not forceReplace && hasDuplicateUnique.IsSome then failwith $"Invalid new column `{newHeader}`. Table already contains header of the same type on index `{hasDuplicateUnique.Value}`"
        
        if rowCount > values.RowCount then
            values.RowCount <- rowCount
            for kv in values.Columns do
                values.Columns.[kv.Key] <- kv.Value.AsSparse(values.RowCount)
        // In case we replace an existing unique column, we should adjust the new rowCount. For other constant rows we
        if rowCount < values.RowCount && hasDuplicateUnique.IsSome && forceReplace && values.Columns.Count = 1 then
            values.RowCount <- rowCount
        // Example: existingCells contains `Output io` (With io being any IOType) and header is `Output RawDataFile`. This should replace the existing `Output io`.
        // In this case the number of new columns drops to 0 and we insert the index of the existing `Output io` column.
        if hasDuplicateUnique.IsSome then
            numberOfNewColumns <- 0
            index <- hasDuplicateUnique.Value
        /// This ensures nothing gets messed up during mutable insert, for example inser header first and change ColumCount in the process
        let startColCount, startRowCount = headers.Count, values.RowCount
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
                moveColumnCellsTo (columnIndex) (columnIndex + numberOfNewColumns) values
        /// Then we can set the new column at `index`
        let setNewCells() =
            // Not sure if this is intended? If we for example `forceReplace` a single column table with `Input`and 5 rows with a new column of `Input` ..
            // ..and only 2 rows, then table RowCount will decrease from 5 to 2.
            // Related Test: `All.ArcTable.addColumn.Existing Table.add less rows, replace input, force replace
            //if hasDuplicateUnique.IsSome then
            //    removeColumnCells(index) values
            IntDictionary.addOrUpdate index newCol values.Columns
                
        setNewHeader()
        // Only do this if column is inserted and not appended AND we do not execute forceReplace!
        if index < startColCount && hasDuplicateUnique.IsNone then
            increaseColumnIndices()
        //
        if not onlyHeaders then
            setNewCells()
        ()

    let addColumn (newHeader: CompositeHeader) (newCells: ResizeArray<CompositeCell>) (index: int) (forceReplace: bool) (onlyHeaders: bool) (headers: ResizeArray<CompositeHeader>) (values:ArcTableValues) =
        // Create a new column with the new header and cells
        let newCol = ColumnValueRefs.fromCellColumn(newCells,  values.RowCount, values.ValueMap)
        addRefColumn newHeader newCol newCells.Count index forceReplace onlyHeaders headers values


    // We need to calculate the max number of rows between the new columns and the existing columns in the table.
    // `maxRows` will be the number of rows all columns must have after adding the new columns.
    // This behaviour should be intuitive for the user, as Excel handles this case in the same way.
    let fillMissingCells (headers: ResizeArray<CompositeHeader>) (values:ArcTableValues) =
        failwith "FillMissingCells not implemented"


    let addRow (index:int) (newCells:ResizeArray<CompositeCell>) (headers: ResizeArray<CompositeHeader>) (values:ArcTableValues) =
        /// Store start rowCount here, so it does not get changed midway through
        let rowCount = values.RowCount
        let columnCount = values.ColumnCount
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
            newCells |> ResizeArray.iteri (fun columnIndex cell ->
                let rowIndex = index
                setCellAt (columnIndex,rowIndex,cell) values
            )
        ()

    let addRows (index:int) (newRows:ResizeArray<ResizeArray<CompositeCell>>) (headers: ResizeArray<CompositeHeader>) (values:ArcTableValues) =
        /// Store start rowCount here, so it does not get changed midway through
        let rowCount = values.RowCount
        let columnCount = values.ColumnCount
        let numNewRows = newRows.Count
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
                row |> ResizeArray.iteri (fun columnIndex cell ->
                    setCellAt (columnIndex,currentRowIndex,cell) values
                )
            currentRowIndex <- currentRowIndex + 1


    /// Returns true, if two composite headers share the same main header string
    let compositeHeaderMainColumnEqual (ch1 : CompositeHeader) (ch2 : CompositeHeader) =
        ch1.ToString() = ch2.ToString()

    /// Moves a column from one position to another
    ///
    /// This function moves the column from `fromCol` to `toCol` and shifts all columns in between accordingly
    let moveColumnTo (fromCol:int) (toCol:int) (headers : ResizeArray<CompositeHeader>) (values:ArcTableValues) =
        // Shift describes the moving of all the cells that are between fromCol and toCol
        let shift, shiftStart, shiftEnd =
            if fromCol < toCol then
                -1, fromCol + 1, toCol
            else
                1, fromCol - 1, toCol

        // Remember the header of interest (at fromCol)
        let header = headers.[fromCol]
        // Shift all headers  between fromCol and toCol
        for c in [shiftStart .. (-shift) .. shiftEnd] do
            headers.[c + shift] <- headers.[c]
        // Set the column of interest to the new position
        headers.[toCol] <- header
     
        // Remember the cell of interest (at fromCol)
        let col = values.Columns.[fromCol]
        // Shift all cells between fromCol and toCol
        for c in [shiftStart .. (-shift) .. shiftEnd] do
            values.Columns.[(c + shift)] <- values.Columns.[c]
        // Set the cell of interest to the new position
        values.Columns.[(toCol)] <- col

    /// From a list of rows consisting of headers and values, creates a list of combined headers and the values as a sparse matrix
    ///
    /// The values cant be directly taken as they are, as there is no guarantee that the headers are aligned
    ///
    /// This function aligns the headers and values by the main header string
    ///
    /// If keepOrder is true, the order of values per row is kept intact, otherwise the values are allowed to be reordered
    let alignByHeaders (keepOrder : bool) (rows : ((CompositeHeader * CompositeCell) list) list) =
        let headers : ResizeArray<CompositeHeader> = ResizeArray()
        let values : ArcTableValues = ArcTableValues.init()
        values.RowCount <- rows.Length
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
                                    setCellAt (colI,rowI,c) values
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
                                setCellAt (colI,rowI,m) values
                                newL
                            | None -> newL
                    )
                loop (colI+1) rows
        loop 0 rows