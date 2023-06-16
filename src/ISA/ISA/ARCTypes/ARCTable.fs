namespace ISA

open Fable.Core

module rec ArcTableAux =

    let getColumnCount (headers:ResizeArray<CompositeHeader>) = 
        headers.Count
    let getRowCount (values:System.Collections.Generic.Dictionary<int*int,CompositeCell>) = 
        if values.Count = 0 then 0 else
            values.Keys |> Seq.maxBy snd |> snd |> (+) 1

    module SanityChecks =
        let validateIndex (index:int) (columnCount:int) =
            if index < 0 then failwith "Cannot insert CompositeColumn at index < 0."
            if index > columnCount then failwith $"Specified index is out of table range! Table contains only {columnCount} columns."

        let validateColumn (column:CompositeColumn) = column.validate(true) |> ignore

        let inline validateNoDuplicateUniqueColumns (columns:seq<CompositeColumn>) =
            let duplicates = columns |> Seq.map (fun x -> x.Header) |> ArcTableAux.tryFindDuplicateUniqueInArray
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
               

[<AttachMembers>]
type ArcTable = 
    {
        Name : string
        ValueHeaders : ResizeArray<CompositeHeader>
        /// Key: Column * Row
        Values : System.Collections.Generic.Dictionary<int*int,CompositeCell>  
    }

    /// Create ArcTable with empty 'ValueHeader' and 'Values' 
    static member init(name: string) = {
        Name = name
        ValueHeaders = ResizeArray<CompositeHeader>()
        Values = System.Collections.Generic.Dictionary<int*int,CompositeCell>()
    }

    ///
    static member private insertRawColumn (newHeader: CompositeHeader) (newCells: CompositeCell []) (index: int) (forceReplace: bool) (table:ArcTable) =
        let mutable numberOfNewColumns = 1
        let mutable index = index
        let hasDuplicateUnique = ArcTableAux.tryFindDuplicateUnique newHeader table.ValueHeaders
        // implement fail if unique column should be added but exists already
        if not forceReplace && hasDuplicateUnique.IsSome then failwith $"Invalid new column `{newHeader}`. Table already contains header of the same type on index `{hasDuplicateUnique.Value}`"
        // Example: existingCells contains `Output io` (With io being any IOType) and header is `Output RawDataFile`. This should replace the existing `Output io`.
        // In this case the number of new columns drops to 0 and we insert the index of the existing `Output io` column.
        if hasDuplicateUnique.IsSome then
            numberOfNewColumns <- 0
            index <- hasDuplicateUnique.Value
        // headers are easily added. Just insert at position of index. This will insert without replace.
        let insertNewHeader = 
            // If we need to replace, set at index
            if hasDuplicateUnique.IsSome then 
                table.ValueHeaders.[index] <- newHeader
            // if not we just insert at index
            else
                table.ValueHeaders.Insert(index, newHeader)
        /// For all columns with index >= we need to increase column index by `numberOfNewColumns`.
        /// We do this by moving all these columns one to the right with mutable dictionary set logic (cells.[key] <- newValue), 
        /// Therefore we need to start with the last column to not overwrite any values we still need to shift
        let increaseColIndexForExisting =
            /// Get last column index
            let lastColumnIndex = table.ColumnCount - 1
            /// Get all keys, to map over relevant rows afterwards
            let keys = table.Values.Keys
            // start with last column index and go down to `index`
            for nextColumnIndex in lastColumnIndex .. index do
                keys 
                |> Seq.filter (fun (c,_) -> c = nextColumnIndex) // only get keys for the relevant column
                |> Seq.iter (fun (c_index,r_index)-> // iterate over all keys for the column
                    let v = table.Values.[(c_index,r_index)] // get the value
                    /// Remove value. This is necessary in the following scenario:
                    ///
                    /// "AddColumn.Existing Table.add less rows, insert at".
                    ///
                    /// Assume a table with 5 rows, insert column with 2 cells. All 5 rows at `index` position are shifted +1, but only row 0 and 1 are replaced with new values. 
                    /// Without explicit removing, row 2..4 would stay as is. 
                    let rmv_v = table.Values.Remove(c_index,r_index) 
                    //table.Values.[(c_index + numberOfNewColumns,r_index)] <- v // set the same value for the same row but (c_index + `numberOfNewColumns`)
                    ()
                )
        /// Then we can set the new column at `index`
        let insertNewColumn =
            for rowIndex,v in (Array.indexed newCells) do
                let columnIndex = index
                table.Values.[(columnIndex,rowIndex)] <- v
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
                let relatedHeader = table.ValueHeaders.[columnIndex]
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
                    table.Values.[(missingColumn,missingRow)] <- empty

    member this.ColumnCount 
        with get() = ArcTableAux.getColumnCount (this.ValueHeaders)
    member this.RowCount 
        with get() = ArcTableAux.getRowCount (this.Values)

    member this.AddColumn (header:CompositeHeader, ?cells: CompositeCell [], ?index: int, ?forceReplace: bool) : unit = 
        let index = Option.defaultValue this.ColumnCount index
        let cells = Option.defaultValue [||] cells
        let forceReplace = Option.defaultValue false forceReplace
        // sanity checks
        ArcTableAux.SanityChecks.validateIndex index this.ColumnCount
        ArcTableAux.SanityChecks.validateColumn(CompositeColumn.create(header, cells))
        // 
        ArcTable.insertRawColumn header cells index forceReplace this
        //ArcTable.extendBodyCells this

    static member addColumn (header: CompositeHeader) (cells: CompositeCell []) (index: int option) (table:ArcTable) =
        table.AddColumn(header, cells, ?index = index)

    member this.AddColumns (columns: CompositeColumn [], ?index: int, ?forceReplace: bool) : unit = 
        let mutable index = Option.defaultValue this.ColumnCount index
        let forceReplace = Option.defaultValue false forceReplace
        // sanity checks
        ArcTableAux.SanityChecks.validateIndex index this.ColumnCount
        ArcTableAux.SanityChecks.validateNoDuplicateUniqueColumns columns
        columns |> Array.iter (fun x -> ArcTableAux.SanityChecks.validateColumn x)
        //
        let rec recInsertColumn i = 
            let prevHeadersCount = this.ValueHeaders.Count
            let c = columns.[i]
            ArcTable.insertRawColumn c.Header c.Cells index forceReplace this
            // Check if more headers, otherwise `ArcTableAux.insertColumn` replaced a column and we do not need to increase index.
            if this.ValueHeaders.Count > prevHeadersCount then index <- index + 1
            if i < (columns.Length-1) then recInsertColumn (i+1)

        recInsertColumn 0
        ArcTable.extendBodyCells this
        
    static member addColumns (columns: CompositeColumn [],?index: int) =
        fun (table:ArcTable) ->
            table.AddColumns(columns, ?index = index)

    member this.GetColumn(index:int) =
        ArcTableAux.SanityChecks.validateIndex index this.ColumnCount
        let h = this.ValueHeaders.[index]
        let cells = 
            this.Values |> Seq.choose (fun x -> 
                match x.Key with
                | col, i when col = index -> Some (i, x.Value)
                | _ -> None
            )
            |> Seq.sortBy fst
            |> Seq.map snd
            |> Array.ofSeq
        CompositeColumn.create(h, cells)

    static member getColumn (index:int) (table:ArcTable) = table.GetColumn(index)

    member this.SetColumn (index:int, column:CompositeColumn) =
        ArcTableAux.SanityChecks.validateIndex index this.ColumnCount
        ArcTableAux.SanityChecks.validateColumn(column)
        /// remove to be replaced header, this is only used to check if any OTHER header is of the same unique type as column.Header
        /// MUST USE "Seq.removeAt" to not remove in mutable object!
        let otherHeaders = this.ValueHeaders |> Seq.removeAt index
        ArcTableAux.SanityChecks.validateNoDuplicateUnique column.Header otherHeaders
        let nextHeader = 
            this.ValueHeaders.[index] <- column.Header
        let nextBody =
            column.Cells |> Array.iteri (fun i v -> this.Values.[(index,i)] <- v)
        ()
        //{
        //    this with
        //        ValueHeaders = nextHeader
        //        Values = nextBody
        //}

    static member setColumn (index:int) (column:CompositeColumn) (table:ArcTable) = table.SetColumn(index, column)
        
    member this.SetHeader (index:int, newHeader: CompositeHeader, ?forceConvertCells: bool) =
        let forceConvertCells = Option.defaultValue false forceConvertCells
        ArcTableAux.SanityChecks.validateIndex index this.ColumnCount
        /// remove to be replaced header, this is only used to check if any OTHER header is of the same unique type as column.Header
        /// MUST USE "Seq.removeAt" to not remove in mutable object!
        let otherHeaders = this.ValueHeaders |> Seq.removeAt index
        ArcTableAux.SanityChecks.validateNoDuplicateUnique newHeader otherHeaders
        // Test if column is still valid with new header
        let c = { this.GetColumn(index) with Header = newHeader }
        if c.validate() then
            let setHeader = this.ValueHeaders.[index] <- newHeader
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

    static member setHeader (index:int) (header:CompositeHeader) (table:ArcTable) = table.SetHeader(index, header)

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
