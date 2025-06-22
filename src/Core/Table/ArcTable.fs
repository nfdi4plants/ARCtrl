namespace ARCtrl

open Fable.Core
open System.Collections.Generic
open ArcTableAux
open ARCtrl.Helper

[<StringEnum>]
type TableJoinOptions =
/// Add only headers, no values
| Headers
/// Add headers and unit information without main value
| WithUnit
/// Add full columns
| WithValues

[<AttachMembers>]
type ArcTable(name: string, ?headers: ResizeArray<CompositeHeader>, ?columns: ResizeArray<ResizeArray<CompositeCell>>) =

    let headers = defaultArg headers (ResizeArray<CompositeHeader>())
    let columns = defaultArg columns (ResizeArray<ResizeArray<CompositeCell>>())

    let valid = SanityChecks.validateCellColumns headers columns true
    let mutable _values = ArcTableValues.fromCellColumns(columns)
    let mutable _name = name
    let mutable _headers: ResizeArray<CompositeHeader> = headers

    member this.ValueMap
        with internal get() = _values.ValueMap
        and internal set(valueMap) =
            _values.ValueMap <- valueMap

    member this.ColumnRefs
        with internal get() = _values.Columns
        and internal set(internalColumnRefs) =
            _values.Columns <- internalColumnRefs

    member this.Headers
        with get() = _headers
        and set(newHeaders) =
            // SanityChecks.validate newHeaders values true |> ignore // TODO
            _headers <- newHeaders

    member this.Values
        with get() = _values

    member this.Name
        with get() = _name
        and internal set (newName) = _name <- newName

    static member create(name, headers, values) = ArcTable(name, headers, values)

    /// Create ArcTable with empty 'ValueHeader' and 'Values'
    static member init(name: string) =
        ArcTable(name, ResizeArray<CompositeHeader>(), ResizeArray())

    static member fromArcTableValues (name: string, headers: ResizeArray<CompositeHeader>, values: ArcTableValues) =
        let t = ArcTable.init(name)
        t.Headers <- headers
        t.ValueMap <- values.ValueMap
        t.ColumnRefs <- values.Columns
        t.RowCount <- values.RowCount
        t

    static member createFromRows(name,headers : ResizeArray<CompositeHeader>,rows : ResizeArray<ResizeArray<CompositeCell>>) : ArcTable =
        let t = ArcTable(name,headers)
        t.AddRows(rows)
        t

    /// Will return true or false if table is valid.
    ///
    /// Set `raiseException` = `true` to raise exception.
    member this.Validate(?raiseException: bool) =
        let raiseException = defaultArg raiseException true
        SanityChecks.validateArcTableValues this.Headers this.Values raiseException

    /// Will return true or false if table is valid.
    ///
    /// Set `raiseException` = `true` to raise exception.
    static member validate(?raiseException: bool) =
        fun (table:ArcTable) ->
            table.Validate(?raiseException=raiseException)

    member this.ColumnCount
        with get() = this.Headers.Count

    static member columnCount (table:ArcTable) = table.ColumnCount

    member this.RowCount
        with get() = _values.RowCount
        and set (newRowCount) =
            _values.RowCount <- newRowCount

    static member getRowCount (table:ArcTable) = table.RowCount

    static member setRowCount (newRowCount) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.RowCount <- newRowCount
            newTable

    member this.Columns
        with get() = ResizeArray [|for i = 0 to this.ColumnCount - 1 do this.GetColumn(i)|]

    member this.Copy() : ArcTable =
        let nextHeaders = this.Headers |> ResizeArray.map (fun h -> h.Copy())
        let nextValues = _values.Copy()
        ArcTable.fromArcTableValues(this.Name, nextHeaders, nextValues)

    /// Returns a cell at given position if it exists, else returns None.
    member this.TryGetCellAt (column: int,row: int) = ArcTableAux.Unchecked.tryGetCellAt (column,row) _values

    static member tryGetCellAt  (column: int,row: int) =
        fun (table:ArcTable) ->
            table.TryGetCellAt(column, row)

    member this.GetCellAt (column: int,row: int) =
        if column > this.ColumnCount - 1 || column < 0 then
            failwithf "Column index %i is out of bounds for table %s with %i columns." column this.Name this.ColumnCount
        if row > this.RowCount - 1 || row < 0 then
            failwithf "Row index %i is out of bounds for table %s with %i rows." row this.Name this.RowCount
        Unchecked.getCellWithDefault (column,row) _headers _values

    static member getCellAt (column: int,row: int) =
        fun (table:ArcTable) ->
            table.GetCellAt(column, row)

    member this.IterColumns(action: CompositeColumn -> unit) =
        for columnIndex in 0 .. (this.ColumnCount-1) do
            let column = this.GetColumn columnIndex
            action column

    static member iterColumns(action: CompositeColumn -> unit) =
        fun (table:ArcTable) ->
            let copy = table.Copy()
            copy.IterColumns(action)
            copy

    member this.IteriColumns(action: int -> CompositeColumn -> unit) =
        for columnIndex in 0 .. (this.ColumnCount-1) do
            let column = this.GetColumn columnIndex
            action columnIndex column

    static member iteriColumns(action: int -> CompositeColumn -> unit) =
        fun (table:ArcTable) ->
            let copy = table.Copy()
            copy.IteriColumns(action)
            copy

    // - Cell API - //
    /// Update an already existing cell in the table. Fails if cell is outside the column AND row bounds of the table
    member this.UpdateCellAt(columnIndex, rowIndex,c : CompositeCell, ?skipValidation) =
        let skipValidation = defaultArg skipValidation false
        if not(skipValidation) then
            SanityChecks.validateColumnIndex columnIndex this.ColumnCount false
            SanityChecks.validateRowIndex rowIndex this.RowCount false
            c.ValidateAgainstHeader(this.Headers.[columnIndex],raiseException = true) |> ignore
        Unchecked.setCellAt(columnIndex, rowIndex,c) _values

    /// Update an already existing cell in the table. Fails if cell is outside the columnd AND row bounds of the table
    static member updateCellAt(columnIndex: int, rowIndex: int, cell: CompositeCell, ?skipValidation) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.UpdateCellAt(columnIndex,rowIndex,cell, ?skipValidation = skipValidation)
            newTable


    /// Update an already existing cell in the table, or adds a new cell if the row boundary is exceeded. Fails if cell is outside the column bounds of the table
    member this.SetCellAt(columnIndex, rowIndex,c : CompositeCell, ?skipValidation) =
        let skipValidation = defaultArg skipValidation false
        if not(skipValidation) then
            SanityChecks.validateColumnIndex columnIndex this.ColumnCount false
            c.ValidateAgainstHeader(this.Headers.[columnIndex],raiseException = true) |> ignore
        Unchecked.setCellAt(columnIndex, rowIndex,c) _values

    /// Update an already existing cell in the table, or adds a new cell if the row boundary is exceeded. Fails if cell is outside the column bounds of the table
    static member setCellAt(columnIndex: int, rowIndex: int, cell: CompositeCell, ?skipValidation) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.SetCellAt(columnIndex,rowIndex,cell, ?skipValidation = skipValidation)
            newTable


    /// Update cells in a column by a function.
    ///
    /// Inputs of the function are columnIndex, rowIndex, and the current cell.
    member this.UpdateCellsBy(f : int -> int -> CompositeCell -> CompositeCell, ?skipValidation) =
        let skipValidation = defaultArg skipValidation false
        for kv in _values do
            let ci,ri = kv.Key
            let newCell = f ci ri kv.Value
            if not(skipValidation) then newCell.ValidateAgainstHeader(this.Headers[ci],raiseException = true) |> ignore
            Unchecked.setCellAt(ci, ri,newCell) _values

    /// Update cells in a column by a function.
    ///
    /// Inputs of the function are columnIndex, rowIndex, and the current cell.static member updateCellBy(f : int -> int -> CompositeCell -> CompositeCell) =
    static member updateCellsBy(f : int -> int -> CompositeCell -> CompositeCell, ?skipValidation) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.UpdateCellsBy(f, ?skipValidation = skipValidation)

    /// Update cells in a column by a function.
    ///
    /// Inputs of the function are columnIndex, rowIndex, and the current cell.
    member this.UpdateCellBy(columnIndex, rowIndex, f : CompositeCell -> CompositeCell, ?skipValidation) =
        let skipValidation = defaultArg skipValidation false
        if not(skipValidation) then
            SanityChecks.validateColumnIndex columnIndex this.ColumnCount false
            SanityChecks.validateRowIndex rowIndex this.RowCount false
        let newCell = this.GetCellAt(columnIndex, rowIndex) |> f
        if not(skipValidation) then newCell.ValidateAgainstHeader(this.Headers.[columnIndex],raiseException = true) |> ignore
        Unchecked.setCellAt(columnIndex, rowIndex, newCell) _values

    /// Update cells in a column by a function.
    ///
    /// Inputs of the function are columnIndex, rowIndex, and the current cell.static member updateCellBy(f : int -> int -> CompositeCell -> CompositeCell) =
    static member updateCellBy(columnIndex, rowIndex, f : CompositeCell -> CompositeCell, ?skipValidation) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.UpdateCellBy(columnIndex, rowIndex, f, ?skipValidation = skipValidation)

    // - Header API - //
    member this.UpdateHeader (index:int, newHeader: CompositeHeader, ?forceConvertCells: bool) =
        let forceConvertCells = Option.defaultValue false forceConvertCells
        ArcTableAux.SanityChecks.validateColumnIndex index this.ColumnCount false
        /// remove to be replaced header, this is only used to check if any OTHER header is of the same unique type as column.Header
        /// MUST USE "Seq.removeAt" to not remove in mutable object!
        let otherHeaders = this.Headers |> Seq.removeAt index
        ArcTableAux.SanityChecks.validateNoDuplicateUnique newHeader otherHeaders
        let c = { this.GetColumn(index) with Header = newHeader }
        // Test if column is still valid with new header, if so insert header at index
        if c.Validate() then
            let setHeader = this.Headers.[index] <- newHeader
            ()
        // if we force convert cells, we want to convert the existing cells to a valid cell type for the new header
        elif forceConvertCells then
            let convertedCells =
                match newHeader with
                | isTerm when newHeader.IsTermColumn ->
                    c.Cells |> ResizeArray.map (fun c ->
                        // only update cell if it is freetext to not remove some unit and some term cells
                        if c.isFreeText then
                            c.ToTermCell()
                        else
                            c
                    )
                | _ ->
                    c.Cells |> ResizeArray.map (fun c -> c.ToFreeTextCell())
            this.UpdateColumn(index, newHeader, convertedCells)
        else
            failwith "Tried setting header for column with invalid type of cells. Set `forceConvertCells` flag to automatically convert cells into valid CompositeCell type."

    static member updateHeader (index:int, header:CompositeHeader) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.UpdateHeader(index, header)
            newTable

    // - Column API - //
    //[<NamedParams>]
    member this.AddColumn (header:CompositeHeader, ?cells: ResizeArray<CompositeCell>, ?index: int, ?forceReplace: bool) : unit =
        let index =
            defaultArg index this.ColumnCount
        let cells =
            defaultArg cells (ResizeArray())
        let forceReplace = defaultArg forceReplace false
        SanityChecks.validateColumnIndex index this.ColumnCount true
        SanityChecks.validateColumn(CompositeColumn.create(header, cells))
        //
        Unchecked.addColumn header cells index forceReplace false this.Headers _values


    static member addColumn (header: CompositeHeader, ?cells: ResizeArray<CompositeCell>,?index: int ,?forceReplace : bool) : (ArcTable -> ArcTable) =
        fun (table: ArcTable) ->
            let newTable = table.Copy()
            newTable.AddColumn(header, ?cells = cells, ?index = index, ?forceReplace = forceReplace)
            newTable

    /// Adds a new column which fills in the single given value for the length of the table.
    member this.AddColumnFill (header: CompositeHeader, cell: CompositeCell, ?index: int ,?forceReplace : bool) =
        let index =
            defaultArg index this.ColumnCount
        let forceReplace = defaultArg forceReplace false
        SanityChecks.validateColumnIndex index this.ColumnCount true
        SanityChecks.validateColumn(CompositeColumn.create(header, ResizeArray [cell]))
        //
        Unchecked.addColumnFill header cell index forceReplace false this.Headers _values

    static member addColumnFill (header: CompositeHeader, cell: CompositeCell, ?index: int ,?forceReplace : bool) : ArcTable -> ArcTable =
        fun (table: ArcTable) ->
            let newTable = table.Copy()
            newTable.AddColumnFill(header, cell, ?index = index,  ?forceReplace = forceReplace)
            newTable

    // - Column API - //
    /// Replaces the header and cells of a column at given index.
    member this.UpdateColumn (columnIndex:int, header: CompositeHeader, ?cells: ResizeArray<CompositeCell>) =
        SanityChecks.validateColumnIndex columnIndex this.ColumnCount false
        let column = CompositeColumn.create(header, ?cells=cells)
        SanityChecks.validateColumn(column)
        /// remove to be replaced header, this is only used to check if any OTHER header is of the same unique type as column.Header
        /// MUST USE "Seq.removeAt" to not remove in mutable object!
        let otherHeaders = this.Headers |> Seq.removeAt columnIndex
        SanityChecks.validateNoDuplicateUnique column.Header otherHeaders
        // Must remove first, so no leftover rows stay when setting less rows than before.
        Unchecked.removeHeader columnIndex this.Headers
        Unchecked.removeColumnCells columnIndex _values
        // nextHeader
        this.Headers.Insert(columnIndex,column.Header)
        // nextBody
        column.Cells |> ResizeArray.iteri (fun rowIndex v -> Unchecked.setCellAt(columnIndex,rowIndex,v) _values)

    /// Replaces the header and cells of a column at given index.
    static member updateColumn (columnIndex:int, header: CompositeHeader, ?cells: ResizeArray<CompositeCell>) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.UpdateColumn(columnIndex, header, ?cells=cells)
            newTable

    // - Column API - //
    member this.InsertColumn (index: int, header:CompositeHeader, ?cells: ResizeArray<CompositeCell>) =
        this.AddColumn(header, index = index,?cells = cells, forceReplace = false)

    static member insertColumn (index: int, header:CompositeHeader, ?cells: ResizeArray<CompositeCell>) =
        fun (table: ArcTable) ->
            let newTable = table.Copy()
            newTable.InsertColumn(index, header, ?cells = cells)
            newTable

    // - Column API - //
    member this.AppendColumn (header:CompositeHeader, ?cells: ResizeArray<CompositeCell>) =
        this.AddColumn(header, ?cells = cells, index = this.ColumnCount, forceReplace = false)

    static member appendColumn (header:CompositeHeader, ?cells: ResizeArray<CompositeCell>) =
        fun (table: ArcTable) ->
            let newTable = table.Copy()
            newTable.AppendColumn(header, ?cells = cells)
            newTable

    // - Column API - //
    member this.AddColumns (columns: CompositeColumn [], ?index: int, ?forceReplace: bool) : unit =
        let mutable index = defaultArg index this.ColumnCount
        let forceReplace = defaultArg forceReplace false
        SanityChecks.validateColumnIndex index this.ColumnCount true
        SanityChecks.validateNoDuplicateUniqueColumns columns
        columns |> Array.iter (fun x -> SanityChecks.validateColumn x)
        columns
        |> Array.iter (fun col ->
            let prevHeadersCount = this.Headers.Count
            Unchecked.addColumn col.Header col.Cells index forceReplace false this.Headers _values
            // Check if more headers, otherwise `ArcTableAux.insertColumn` replaced a column and we do not need to increase index.
            if this.Headers.Count > prevHeadersCount then index <- index + 1
        )

    static member addColumns (columns: CompositeColumn [],?index: int) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.AddColumns(columns, ?index = index)
            newTable

    // - Column API - //
    member this.RemoveColumn (index:int) =
        ArcTableAux.SanityChecks.validateColumnIndex index this.ColumnCount false
        /// Set ColumnCount here to avoid changing columnCount by changing header count
        let columnCount = this.ColumnCount
        // removeHeader
        Unchecked.removeHeader(index) this.Headers
        // removeCell
        Unchecked.removeColumnCells_withIndexChange (index) columnCount _values

    static member removeColumn (index:int) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.RemoveColumn(index)
            newTable

    // - Column API - //
    member this.RemoveColumns(indexArr:int []) =
        // Sanity check here too, to avoid removing things from mutable to fail in the middle
        Array.iter (fun index -> SanityChecks.validateColumnIndex index this.ColumnCount false) indexArr
        /// go from highest to lowest so no wrong column gets removed after index shift
        let indexArr = indexArr |> Array.sortDescending
        Array.iter (fun index -> this.RemoveColumn index) indexArr

    static member removeColumns(indexArr:int []) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.RemoveColumns(indexArr)
            newTable

    // - Column API - //
    // GetColumnAt?
    member this.GetColumn(columnIndex:int) : CompositeColumn =
        SanityChecks.validateColumnIndex columnIndex this.ColumnCount false
        let h = this.Headers.[columnIndex]
        let cells =
            [|
                for i = 0 to this.RowCount - 1 do
                    match this.TryGetCellAt(columnIndex, i) with
                    | None -> failwithf "Unable to find cell for index: (%i, %i)" columnIndex i
                    | Some c -> c
            |]
            |> ResizeArray
        CompositeColumn.create(h, cells)

    static member getColumn (index:int) =
        fun (table:ArcTable) ->
            table.GetColumn(index)

    member this.TryGetColumnByHeader (header:CompositeHeader) =
        let index = this.Headers |> Seq.tryFindIndex (fun x -> x = header)
        index
        |> Option.map (fun i -> this.GetColumn(i))

    static member tryGetColumnByHeader (header:CompositeHeader) =
        fun (table:ArcTable) ->
            table.TryGetColumnByHeader(header)

    // tryGetColumnByHeaderBy
    member this.TryGetColumnByHeaderBy (headerPredicate:CompositeHeader -> bool) = //better name for header / action
        this.Headers
        |> Seq.tryFindIndex headerPredicate
        |> Option.map (fun i -> this.GetColumn(i))

    static member tryGetColumnByHeaderBy (headerPredicate:CompositeHeader -> bool) =
        fun (table:ArcTable) ->
            table.TryGetColumnByHeaderBy(headerPredicate)

    member this.GetColumnByHeader (header:CompositeHeader) =
        match this.TryGetColumnByHeader(header) with
        | Some c -> c
        | None -> failwithf "Unable to find column with header in table %s: %O" this.Name header

    static member getColumnByHeader (header:CompositeHeader) =
        fun (table:ArcTable) ->
            table.GetColumnByHeader(header)

    member this.TryGetInputColumn() =
        let index = this.Headers |> Seq.tryFindIndex (fun x -> x.isInput)
        index
        |> Option.map (fun i -> this.GetColumn(i))

    static member tryGetInputColumn () =
        fun (table:ArcTable) ->
            table.TryGetInputColumn()

    member this.GetInputColumn() =
        match this.TryGetInputColumn() with
        | Some c -> c
        | None -> failwithf "Unable to find input column in table %s" this.Name

    static member getInputColumn () =
        fun (table:ArcTable) ->
            table.GetInputColumn()

    member this.TryGetOutputColumn() =
        let index = this.Headers |> Seq.tryFindIndex (fun x -> x.isOutput)
        index
        |> Option.map (fun i -> this.GetColumn(i))

    static member tryGetOutputColumn () =
        fun (table:ArcTable) ->
            table.TryGetOutputColumn()

    member this.GetOutputColumn() =
        match this.TryGetOutputColumn() with
        | Some c -> c
        | None -> failwithf "Unable to find output column in table %s" this.Name

    static member getOutputColumn () =
        fun (table:ArcTable) ->
            table.GetOutputColumn()

    member this.MoveColumn(startCol : int, endCol : int) =
        if startCol = endCol then
            ()
        elif startCol < 0 || startCol >= this.ColumnCount then
            failwithf "Cannt move column. Invalid start column index: %i" startCol
        elif endCol < 0 || endCol >= this.ColumnCount then
            failwithf "Cannt move column. Invalid end column index: %i" endCol
        else
            ArcTableAux.Unchecked.moveColumnTo startCol endCol this.Headers _values

    static member moveColumn(startCol : int, endCol : int) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.MoveColumn(startCol, endCol)
            newTable

    // - Row API - //
    member this.AddRow (?cells: ResizeArray<CompositeCell>, ?index: int) : unit =
        let index = defaultArg index this.RowCount
        let cells =
            if cells.IsNone then
                // generate default cells. Uses the same logic as extending missing row values.
                [|
                    for columnIndex in 0 .. this.ColumnCount-1 do
                        let h = this.Headers.[columnIndex]
                        let tryFirstCell = Unchecked.tryGetCellAt(columnIndex,0) _values
                        yield getEmptyCellForHeader h tryFirstCell
                |]
                |> ResizeArray
            else
                cells.Value
        // Sanity checks
        SanityChecks.validateRowIndex index this.RowCount true
        SanityChecks.validateRowLength cells this.ColumnCount
        for columnIndex in 0 .. this.ColumnCount-1 do
            let h = this.Headers.[columnIndex]
            let column = CompositeColumn.create(h,ResizeArray [|cells.[columnIndex]|])
            SanityChecks.validateColumn column
        // Sanity checks - end
        Unchecked.addRow index cells this.Headers _values

    static member addRow (?cells: ResizeArray<CompositeCell>, ?index: int) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.AddRow(?cells=cells,?index=index)
            newTable

    // - Row API - //
    member this.UpdateRow(rowIndex: int, cells: ResizeArray<CompositeCell>) =
        SanityChecks.validateRowIndex rowIndex this.RowCount false
        SanityChecks.validateRowLength cells this.RowCount
        cells
        |> ResizeArray.iteri (fun i cell ->
            let h = this.Headers.[i]
            let column = CompositeColumn.create(h,ResizeArray.singleton cell)
            SanityChecks.validateColumn column
        )
        cells
        |> ResizeArray.iteri (fun columnIndex cell ->
            Unchecked.setCellAt(columnIndex, rowIndex, cell) _values
        )

    static member updateRow(rowIndex: int, cells: ResizeArray<CompositeCell>) =
        fun (table: ArcTable) ->
            let newTable = table.Copy()
            newTable.UpdateRow(rowIndex, cells)
            newTable

    // - Row API - //
    member this.AppendRow (?cells: ResizeArray<CompositeCell>) =
        this.AddRow(?cells=cells,index = this.RowCount)

    static member appendRow (?cells: ResizeArray<CompositeCell>) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.AppendRow(?cells=cells)
            newTable

    // - Row API - //
    member this.InsertRow (index: int, ?cells: ResizeArray<CompositeCell>) =
        this.AddRow(index=index, ?cells=cells)

    static member insertRow (index: int, ?cells: ResizeArray<CompositeCell>) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.AddRow(index=index, ?cells=cells)
            newTable

    // - Row API - //
    member this.AddRows (rows: ResizeArray<ResizeArray<CompositeCell>>, ?index: int) =
        let mutable index = defaultArg index this.RowCount
        // Sanity checks
        SanityChecks.validateRowIndex index this.RowCount true
        rows |> ResizeArray.iter (fun row -> SanityChecks.validateRowLength row this.ColumnCount)
        for row in rows do
            for columnIndex in 0 .. this.ColumnCount-1 do
                let h = this.Headers.[columnIndex]
                let column = CompositeColumn.create(h,ResizeArray.singleton (row.[columnIndex]))
                SanityChecks.validateColumn column
        // Sanity checks - end
        Unchecked.addRows index rows this.Headers _values

    static member addRows (rows: ResizeArray<ResizeArray<CompositeCell>>, ?index: int) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.AddRows(rows,?index=index)
            newTable

    // - Row API - //
    member this.AddRowsEmpty (rowCount: int, ?index: int) =
        let row =
            [|
                for columnIndex in 0 .. this.ColumnCount-1 do
                    let h = this.Headers.[columnIndex]
                    let tryFirstCell = Unchecked.tryGetCellAt(columnIndex,0) _values
                    yield getEmptyCellForHeader h tryFirstCell
            |]
            |> ResizeArray
        let rows = ResizeArray.init rowCount (fun _ -> row)
        this.AddRows(rows,?index=index)

    static member addRowsEmpty (rowCount: int, ?index: int) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.AddRowsEmpty(rowCount, ?index=index)
            newTable

    // - Row API - //
    member this.RemoveRow (index:int) =
        ArcTableAux.SanityChecks.validateRowIndex index this.RowCount false
        // removeCells
        Unchecked.removeRowCells_withIndexChange index _values

    static member removeRow (index:int) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.RemoveRow (index)
            newTable

    // - Row API - //
    member this.RemoveRows (indexArr:int []) =
        // Sanity check here too, to avoid removing things from mutable to fail in the middle
        Array.iter (fun index -> ArcTableAux.SanityChecks.validateRowIndex index this.RowCount false) indexArr
        /// go from highest to lowest so no wrong column gets removed after index shift
        let indexArr = indexArr |> Array.sortDescending
        Array.iter (fun index -> this.RemoveRow index) indexArr

    static member removeRows (indexArr:int []) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.RemoveColumns indexArr
            newTable

    // - Row API - //
    member this.GetRow(rowIndex : int,?SkipValidation) =
        if not(SkipValidation = Some true) then SanityChecks.validateRowIndex rowIndex this.RowCount false
        [|
            for columnIndex = 0 to this.ColumnCount - 1 do
                this.GetCellAt(columnIndex, rowIndex)
        |]
        |> ResizeArray

    static member getRow (index:int) =
        fun (table:ArcTable) ->
            table.GetRow(index)

    /// <summary>
    /// This function can be used to join two arc tables.
    /// </summary>
    /// <param name="index">If not set default to append. -1 will also append.</param>
    /// <param name="table">The table to join to this table.</param>
    /// <param name="joinOptions">Can add only headers, header with unitized cell information, headers with values.</param>
    /// <param name="forceReplace">if set to true will replace unique columns.</param>
    member this.Join(table:ArcTable, ?index: int, ?joinOptions: TableJoinOptions, ?forceReplace: bool) : unit =
        let joinOptions = defaultArg joinOptions TableJoinOptions.Headers
        let forceReplace = defaultArg forceReplace false
        let mutable index = defaultArg index this.ColumnCount
        index <- if index = -1 then this.ColumnCount else index //make -1 default to append to make function usage more fluent.
        SanityChecks.validateColumnIndex index this.ColumnCount true
        let onlyHeaders = joinOptions = TableJoinOptions.Headers
        let columns =
            let pre = table.Columns
            match joinOptions with
            | Headers -> pre |> ResizeArray.map (fun c -> {c with Cells = ResizeArray()})
            // this is the most problematic case. How do we decide which unit we want to propagate? All?
            | WithUnit ->
                pre |> ResizeArray.map (fun c ->
                    let unitsOpt = c.TryGetColumnUnits()
                    match unitsOpt with
                    | Some units ->
                        let toCompositeCell = fun unitOA -> CompositeCell.createUnitized ("", unitOA)
                        let unitCells = units |> ResizeArray.map (fun u -> toCompositeCell u)
                        {c with Cells = unitCells}
                    | None -> {c with Cells = ResizeArray()}
                )
            | WithValues -> pre
        SanityChecks.validateNoDuplicateUniqueColumns columns
        columns |> ResizeArray.iter (fun x -> SanityChecks.validateColumn x)
        columns
        |> ResizeArray.iter (fun col ->
            let prevHeadersCount = this.Headers.Count
            Unchecked.addColumn col.Header col.Cells index forceReplace onlyHeaders this.Headers _values
            // Check if more headers, otherwise `ArcTableAux.insertColumn` replaced a column and we do not need to increase index.
            if this.Headers.Count > prevHeadersCount then index <- index + 1
        )

    static member join(table:ArcTable, ?index: int, ?joinOptions: TableJoinOptions, ?forceReplace: bool) =
        fun (this: ArcTable) ->
            let copy = this.Copy()
            copy.Join(table,?index=index,?joinOptions=joinOptions,?forceReplace=forceReplace)
            copy

    ///
    member this.AddProtocolTypeColumn(?types : ResizeArray<OntologyAnnotation>, ?index : int) =
        let header = CompositeHeader.ProtocolType
        let cells = types |> Option.map (ResizeArray.map CompositeCell.Term)
        this.AddColumn(header, ?cells = cells, ?index = index)

    member this.AddProtocolVersionColumn(?versions : ResizeArray<string>, ?index : int) =
        let header = CompositeHeader.ProtocolVersion
        let cells = versions |> Option.map (ResizeArray.map CompositeCell.FreeText)
        this.AddColumn(header, ?cells = cells, ?index = index)

    member this.AddProtocolUriColumn(?uris : ResizeArray<string>, ?index : int) =
        let header = CompositeHeader.ProtocolUri
        let cells = uris |> Option.map (ResizeArray.map CompositeCell.FreeText)
        this.AddColumn(header, ?cells = cells, ?index = index)

    member this.AddProtocolDescriptionColumn(?descriptions : ResizeArray<string>, ?index : int) =
        let header = CompositeHeader.ProtocolDescription
        let cells = descriptions |> Option.map (ResizeArray.map CompositeCell.FreeText)
        this.AddColumn(header, ?cells = cells, ?index = index)

    member this.AddProtocolNameColumn(?names : ResizeArray<string>, ?index : int) =
        let header = CompositeHeader.ProtocolREF
        let cells = names |> Option.map (ResizeArray.map CompositeCell.FreeText)
        this.AddColumn(header, ?cells = cells, ?index = index)

    /// Get functions for the protocol columns
    member this.GetProtocolTypeColumn() =
        this.GetColumnByHeader(CompositeHeader.ProtocolType)

    member this.GetProtocolVersionColumn() =
        this.GetColumnByHeader(CompositeHeader.ProtocolVersion)

    member this.GetProtocolUriColumn() =
        this.GetColumnByHeader(CompositeHeader.ProtocolUri)

    member this.GetProtocolDescriptionColumn() =
        this.GetColumnByHeader(CompositeHeader.ProtocolDescription)

    member this.GetProtocolNameColumn() =
        this.GetColumnByHeader(CompositeHeader.ProtocolREF)

    member this.TryGetProtocolNameColumn() =
        this.TryGetColumnByHeader(CompositeHeader.ProtocolREF)

    member this.GetComponentColumns() =
        this.Headers
        |> ResizeArray.choose (fun h ->
            if h.isComponent then Some (this.GetColumnByHeader(h))
            else None
        )

    member this.RescanValueMap() =
        // This will rescan the value map of the table, so that it is up to date with the current values.
        // This is useful if you have changed the values of the table and want to update the value map.
        _values.RescanValueMap()

    /// Splits the table rowWise into a collection of tables, so that each new table has only one value for the given column
    static member SplitByColumnValues(columnIndex) =
        fun (table : ArcTable) ->
            let column = table.GetColumn(columnIndex)
            let indexGroups = column.Cells |> ResizeArray.indexed |> ResizeArray.groupBy snd |> ResizeArray.map (fun (g,vs) -> vs |> ResizeArray.map fst)
            indexGroups
            |> ResizeArray.mapi (fun i indexGroup ->
                let headers  = table.Headers
                let rows =
                    indexGroup
                    // Max row index is the last row index of the table, so no validation needed
                    |> ResizeArray.map (fun i -> table.GetRow(i,SkipValidation = true))
                ArcTable.createFromRows(table.Name,headers,rows)
            )

    /// Splits the table rowWise into a collection of tables, so that each new table has only one value for the given column
    static member SplitByColumnValuesByHeader(header : CompositeHeader) =
        fun (table : ArcTable) ->
            let index = table.Headers |> Seq.tryFindIndex (fun x -> x = header)
            match index with
            | Some i -> ArcTable.SplitByColumnValues i table
            | None -> ResizeArray.singleton (table.Copy())

    /// Splits the table rowWise into a collection of tables, so that each new table has only one value for the ProtocolREF column
    static member SplitByProtocolREF =
        fun (table : ArcTable) ->
            ArcTable.SplitByColumnValuesByHeader CompositeHeader.ProtocolREF table


    /// This method is meant to update an ArcTable stored as a protocol in a study or investigation file with the information from an ArcTable actually stored as an annotation table
    static member updateReferenceByAnnotationTable (refTable:ArcTable) (annotationTable:ArcTable) =
        let refTable = refTable.Copy()
        let annotationTable = annotationTable.Copy()
        let nonProtocolColumns =
            refTable.Headers
            |> Seq.indexed
            |> Seq.choose (fun (i,h) -> if h.isProtocolColumn then None else Some i)
            |> Seq.toArray
        refTable.RemoveColumns nonProtocolColumns
        refTable.RowCount <- annotationTable.RowCount
        for c in annotationTable.Columns do
            refTable.AddColumn(c.Header, cells = c.Cells,forceReplace = true)
        refTable

    /// Append the rows of another table to this one
    ///
    /// The headers of the other table will be aligned with the headers of this table
    ///
    /// The name of table 2 will be ignored
    static member append table1 table2 =
        let getList (t : ArcTable) =
            
            [
                for row = 0 to t.RowCount - 1 do
                    t.GetRow(row,SkipValidation = true)
                    |> Seq.mapi (fun i c -> t.Headers.[i], c)
                    |> Seq.toList
            ]
        let thisCells = getList table1
        let otherCells = getList table2
        let alignedheaders,alignedCells = ArcTableAux.Unchecked.alignByHeaders false (thisCells @ otherCells)
        ArcTable.fromArcTableValues(table1.Name,alignedheaders,alignedCells)

    /// Pretty printer
    override this.ToString() =
        let rowCount = this.RowCount
        [
            $"Table: {this.Name}"
            "-------------"
            this.Headers |> Seq.map (fun x -> x.ToString()) |> String.concat "\t|\t"
            if rowCount > 50 then
                for rowI = 0 to 19 do
                    this.GetRow(rowI) |> Seq.map (fun x -> x.ToString()) |> String.concat "\t|\t"
                "..."
                for rowI = rowCount-20 to rowCount-1 do
                    this.GetRow(rowI) |> Seq.map (fun x -> x.ToString()) |> String.concat "\t|\t"
            elif rowCount = 0 then
                "No rows"
            else
                for rowI = 0 to rowCount-1 do
                    this.GetRow(rowI) |> Seq.map (fun x -> x.ToString()) |> String.concat "\t|\t"
        ]
        |> String.concat "\n"

    member this.StructurallyEquals (other: ArcTable) =
        this.GetHashCode() = other.GetHashCode()

    /// <summary>
    /// Use this function to check if this ArcTable and the input ArcTable refer to the same object.
    ///
    /// If true, updating one will update the other due to mutability.
    /// </summary>
    /// <param name="other">The other ArcTable to test for reference.</param>
    member this.ReferenceEquals (other: ArcTable) = System.Object.ReferenceEquals(this,other)

    // custom check
    override this.Equals other =
        match other with
        | :? ArcTable as table ->
            this.StructurallyEquals(table)
        | _ -> false

    // it's good practice to ensure that this behaves using the same fields as Equals does:
    override this.GetHashCode() =
        //let v1,v2 =
        let vHash = _values.GetHashCode()
        [|
            box this.Name
            this.Headers |> HashCodes.boxHashSeq
            vHash
        |]
        |> HashCodes.boxHashArray
        |> fun x -> x :?> int