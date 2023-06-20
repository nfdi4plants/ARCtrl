namespace ISA

open Fable.Core
open System.Collections.Generic
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

    /// Will return true or false if table is valid. 
    ///
    /// Set `raiseException` = `true` to raise exception.
    member this.Validate(?raiseException: bool) = 
        let mutable isValid: bool = true
        for columnIndex in 0 .. (this.ColumnCount - 1) do
            let column : CompositeColumn = this.GetColumn(columnIndex)
            isValid <- column.validate(?raiseException=raiseException)
        isValid

    /// Will return true or false if table is valid. 
    ///
    /// Set `raiseException` = `true` to raise exception.
    static member validate(?raiseException: bool) =
        fun (table:ArcTable) ->
            table.Validate(?raiseException=raiseException)


    /// Returns a cell at given position if it exists, else returns None.
    member this.TryGetCellAt (column: int,row: int) = ArcTableAux.Unchecked.tryGetCellAt (column,row) this.Values
    
    static member tryGetCellAt  (column: int,row: int) =
        fun (table:ArcTable) ->
            table.TryGetCellAt(column, row)


    // TODO: And then directly a design question. Is a column with rows containing both CompositeCell.Term and CompositeCell.Unitized allowed?
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


    member this.SetColumn (columnIndex:int, column:CompositeColumn) =
        SanityChecks.validateColumnIndex columnIndex this.ColumnCount false
        SanityChecks.validateColumn(column)
        /// remove to be replaced header, this is only used to check if any OTHER header is of the same unique type as column.Header
        /// MUST USE "Seq.removeAt" to not remove in mutable object!
        let otherHeaders = this.Headers |> Seq.removeAt columnIndex
        SanityChecks.validateNoDuplicateUnique column.Header otherHeaders
        // Must remove first, so no leftover rows stay when setting less rows than before.
        Unchecked.removeHeader columnIndex this.Headers
        Unchecked.removeColumnCells columnIndex this.Values
        let nextHeader = 
            this.Headers.Insert(columnIndex,column.Header)
        let nextBody =
            column.Cells |> Array.iteri (fun rowIndex v -> Unchecked.setCellAt(columnIndex,rowIndex,v) this.Values)
        Unchecked.fillMissingCells this.Headers this.Values
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
        Unchecked.addColumn header cells index forceReplace this.Headers this.Values
        Unchecked.fillMissingCells this.Headers this.Values

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


    member this.AppendColumn (header:CompositeHeader, ?cells: CompositeCell []) =
        this.AddColumn(header, ?cells = cells, index = this.ColumnCount, forceReplace = false)

    static member appendColumn (header:CompositeHeader, ?cells: CompositeCell []) =
        fun (table: ArcTable) ->
            let newTable = table.Copy()
            newTable.AppendColumn(header, ?cells = cells)
            newTable


    member this.AddRow (?cells: CompositeCell [], ?index: int) : unit = 
        let index = defaultArg index this.RowCount
        let cells = 
            if cells.IsNone then
                // generate default cells. Uses the same logic as extending missing row values.
                [|
                    for columnIndex in 0 .. this.ColumnCount-1 do
                        let h = this.Headers.[columnIndex]
                        let tryFirstCell = Unchecked.tryGetCellAt(columnIndex,0) this.Values
                        yield Unchecked.getEmptyCellForHeader h tryFirstCell
                |]
            else 
                cells.Value
        // Sanity checks
        SanityChecks.validateRowIndex index this.RowCount true
        SanityChecks.validateRowLength cells this.ColumnCount
        for columnIndex in 0 .. this.ColumnCount-1 do
            let h = this.Headers.[columnIndex]
            let column = CompositeColumn.create(h,[|cells.[columnIndex]|])
            SanityChecks.validateColumn column
        // Sanity checks - end
        Unchecked.addRow index cells this.Headers this.Values

    static member addRow (?cells: CompositeCell [], ?index: int) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.AddRow(?cells=cells,?index=index)
            newTable


    member this.AppendRow (?cells: CompositeCell []) =
        this.AddRow(?cells=cells,index = this.RowCount)

    static member appendRow (?cells: CompositeCell []) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.AppendRow(?cells=cells)
            newTable


    member this.InsertRow (index: int, ?cells: CompositeCell []) =
        this.AddRow(index=index, ?cells=cells)

    static member insertRow (index: int, ?cells: CompositeCell []) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.AddRow(index=index, ?cells=cells)
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
            Unchecked.addColumn col.Header col.Cells index forceReplace this.Headers this.Values
            // Check if more headers, otherwise `ArcTableAux.insertColumn` replaced a column and we do not need to increase index.
            if this.Headers.Count > prevHeadersCount then index <- index + 1
        )
        Unchecked.fillMissingCells this.Headers this.Values

    static member addColumns (columns: CompositeColumn [],?index: int) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.AddColumns(columns, ?index = index)
            newTable


    member this.AddRows (rows: CompositeCell [] [], ?index: int) =
        let mutable index = defaultArg index this.RowCount
        // Sanity checks
        SanityChecks.validateRowIndex index this.RowCount true
        rows |> Array.iter (fun row -> SanityChecks.validateRowLength row this.ColumnCount)
        for row in rows do
            for columnIndex in 0 .. this.ColumnCount-1 do
                let h = this.Headers.[columnIndex]
                let column = CompositeColumn.create(h,[|row.[columnIndex]|])
                SanityChecks.validateColumn column
        // Sanity checks - end
        rows
        |> Array.iter (fun row ->
            Unchecked.addRow index row this.Headers this.Values
            index <- index + 1
        )

    static member addRows (rows: CompositeCell [] [], ?index: int) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.AddRows(rows,?index=index)
            newTable


    member this.AddRowsEmpty (rowCount: int, ?index: int) =
        let row = [|
            for columnIndex in 0 .. this.ColumnCount-1 do
                let h = this.Headers.[columnIndex]
                let tryFirstCell = Unchecked.tryGetCellAt(columnIndex,0) this.Values
                yield Unchecked.getEmptyCellForHeader h tryFirstCell
        |]
        let rows = Array.init rowCount (fun _ -> row)
        this.AddRows(rows,?index=index)

    static member addRowsEmpty (rowCount: int, ?index: int) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.AddRowsEmpty(rowCount, ?index=index)
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
        /// go from highest to lowest so no wrong column gets removed after index shift
        let indexArr = indexArr |> Array.sortDescending
        Array.iter (fun index -> this.RemoveColumn index) indexArr

    static member removeColumns(indexArr:int []) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.RemoveColumns(indexArr)
            newTable


    member this.RemoveRow (index:int) =
        ArcTableAux.SanityChecks.validateRowIndex index this.RowCount false
        let removeCells = Unchecked.removeRowCells_withIndexChange index this.ColumnCount this.RowCount this.Values
        ()

    static member removeRow (index:int) =
        fun (table:ArcTable) ->
            let newTable = table.Copy()
            newTable.RemoveRow (index)
            newTable


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


    member this.GetRow(rowIndex : int) =
        SanityChecks.validateRowIndex rowIndex this.RowCount false
        [|
            for columnIndex = 0 to this.ColumnCount - 1 do 
                this.TryGetCellAt(columnIndex, rowIndex).Value
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


    static member insertParameterValue (t : ArcTable) (p : ProcessParameterValue) : ArcTable = 
        raise (System.NotImplementedException())

    static member getParameterValues (t : ArcTable) : ProcessParameterValue [] = 
        raise (System.NotImplementedException())

    // no 
    static member addProcess = 
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