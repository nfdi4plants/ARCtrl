namespace ISA

open Fable.Core

module rec ArcTableAux =

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

    /// This function is used to init the correct format from ArcTable.ValueHeaders for `insertColumn`.
    let flattenBodyCells (values: System.Collections.Generic.Dictionary<int*int,CompositeCell>) =
        seq {
            for KeyValue((columnIndex,rowIndex),v) in values do
                yield (columnIndex,rowIndex),v
        }

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
                
    ///
    let insertColumn (header: CompositeHeader) (cells: CompositeCell []) (index: int) (forceReplace: bool) (existingHeaders: CompositeHeader []) (existingCells: seq<(int*int)*CompositeCell>) =
        let mutable numberOfNewColumns = 1
        let mutable index = index
        let hasDuplicateUnique = tryFindDuplicateUnique header existingHeaders
        // implement fail if unique column should be added but exists already
        if not forceReplace && hasDuplicateUnique.IsSome then failwith $"Invalid new column `{header}`. Table already contains header of the same type on index `{hasDuplicateUnique.Value}`"
        // Example: existingCells contains `Output io` (With io being any IOType) and header is `Output RawDataFile`. This should replace the existing `Output io`.
        // In this case the number of new columns drops to 0 and we insert the index of the existing `Output io` column.
        if hasDuplicateUnique.IsSome then
            numberOfNewColumns <- 0
            index <- hasDuplicateUnique.Value
        // headers are easily added. Just insert at position of index. This will insert without replace.
        let nextHeaders = 
            let ra = ResizeArray(existingHeaders)
            // If we need to replace, set at index
            if hasDuplicateUnique.IsSome then 
                ra.[index] <- header
            // if not we just insert at index
            else
                ra.Insert(index, header)
            ra.ToArray()
        // Init a sequence in which we will yield all existing and new columns, while increasing column indizes where necessary.
        let nextBody =
            seq {
                for rowIndex,v in (Array.indexed cells) do
                    let columnIndex = index
                    yield 
                        (columnIndex,rowIndex),v
                for (columnIndex,rowIndex),v in existingCells do
                    // Do not yield to be replaced duplicate unique column
                    if hasDuplicateUnique.IsSome && columnIndex = hasDuplicateUnique.Value then
                        ()
                    else
                        let nextColumnIndex = if columnIndex >= index then columnIndex + numberOfNewColumns else columnIndex
                        yield 
                            (nextColumnIndex,rowIndex),v
            }
        nextHeaders, nextBody

    // We need to calculate the max number of rows between the new columns and the existing columns in the table.
    // `maxRows` will be the number of rows all columns must have after adding the new columns.
    // This behaviour should be intuitive for the user, as Excel handles this case in the same way.
    let extendBodyCells (nextHeaders: CompositeHeader []) (cells:seq<(int*int)*CompositeCell>) =
        let maxRows = 
            let getMax() = cells |> Seq.map fst |> Seq.groupBy fst |> Seq.map (fun (_,r) -> Seq.length r) |> Seq.max
            if Seq.isEmpty cells then 0 else getMax()
        cells
        |> Seq.groupBy(fun ((columnIndex,_),_) -> columnIndex )
            // check how many, if any, rows must be added and add them with empty `CompositeCells`
            |> Seq.collect (fun (columnKey, v) ->
                let nExistingRows = Seq.length v
                let diff = maxRows - nExistingRows
                // diff is the count of rows with empty CompositeCells we need to add.
                if diff > 0 then
                    let ra = ResizeArray(v)
                    let relatedHeader = nextHeaders.[columnKey]
                    // check which empty filler `CompositeCells` we need.
                    let empty = 
                        // Not sure if we can add a better logic to infer if empty cells should be term or unitized ~Kevin F
                        match relatedHeader.IsTermColumn, snd ra.[0] with
                        | true, CompositeCell.Term _ -> CompositeCell.emptyTerm
                        | true, CompositeCell.Unitized _ -> CompositeCell.emptyUnitized
                        | false, _ -> CompositeCell.emptyFreeText
                        | _, _ -> failwith "[extendBodyCells] This should never happen, IsTermColumn header must be paired with either term or unitized cell."
                    // Create an array of `diff`-many filler `CompositeCells` with the same columnIndex and the correct missing rowIndizes.
                    let fillerCells = Array.init diff (fun i -> 
                        let nextRowIndex = i + nExistingRows // nExistingRows -> length(!) we keep length as we would need to add 1 anyways to work with the `i` zero based index;  i -> [0,diff];  
                        (columnKey, nextRowIndex), empty
                    )
                    ra.AddRange(fillerCells)
                    ra.ToArray()
                else
                    v 
                    |> Array.ofSeq
            )
            |> Map.ofSeq
            |> fun m -> nextHeaders, System.Collections.Generic.Dictionary<int*int,CompositeCell>(m)

[<AttachMembers>]
type ArcTable = 
    {
        Name : string
        ValueHeaders : CompositeHeader []
        /// Key: Column * Row
        Values : System.Collections.Generic.Dictionary<int*int,CompositeCell>  
    }

    /// Create ArcTable with empty 'ValueHeader' and 'Values' 
    static member init(name: string) = {
        Name = name
        ValueHeaders = [||]
        Values = System.Collections.Generic.Dictionary<int*int,CompositeCell>()
    }

    member this.ColumnCount 
        with get() = this.ValueHeaders.Length
    member this.RowCount 
        with get() =
            if this.Values.Count = 0 then 0 else
                this.Values.Keys |> Seq.groupBy fst |> Seq.map (fun (_,r) -> Seq.length r) |> Seq.max

    member this.AddColumn (header:CompositeHeader, ?cells: CompositeCell [], ?index: int, ?forceReplace: bool) = 
        let index = Option.defaultValue this.ColumnCount index
        let cells = Option.defaultValue [||] cells
        let forceReplace = Option.defaultValue false forceReplace
        // sanity checks
        ArcTableAux.SanityChecks.validateIndex index this.ColumnCount
        ArcTableAux.SanityChecks.validateColumn(CompositeColumn.create(header, cells))
        // 
        let nextHeaders, nextBody =
            this.Values 
            |> ArcTableAux.flattenBodyCells
            |> ArcTableAux.insertColumn header cells index forceReplace this.ValueHeaders
            ||> ArcTableAux.extendBodyCells
        { this with ValueHeaders = nextHeaders; Values = nextBody }

    static member addColumn (header: CompositeHeader) (cells: CompositeCell []) (index: int option) (table:ArcTable) =
        table.AddColumn(header, cells, ?index = index)

    member this.AddColumns (columns: CompositeColumn [], ?index: int, ?forceReplace: bool) = 
        let mutable index = Option.defaultValue this.ColumnCount index
        let forceReplace = Option.defaultValue false forceReplace
        // sanity checks
        ArcTableAux.SanityChecks.validateIndex index this.ColumnCount
        ArcTableAux.SanityChecks.validateNoDuplicateUniqueColumns columns
        columns |> Array.iter (fun x -> ArcTableAux.SanityChecks.validateColumn x)
        //
        let rec recInsertColumn i headers body = 
            let c = columns.[i]
            let nextHeaders, nextBody = ArcTableAux.insertColumn c.Header c.Cells index forceReplace headers body
            // Check if more headers, otherwise `ArcTableAux.insertColumn` replaced a column and we do not need to increase index.
            if nextHeaders.Length > headers.Length then index <- index + 1
            if i >= (columns.Length-1) then nextHeaders, nextBody else recInsertColumn (i+1) nextHeaders nextBody
        let nextHeaders, nextBody =
            let tableFlattened = this.Values |> ArcTableAux.flattenBodyCells
            recInsertColumn 0 this.ValueHeaders tableFlattened
            ||> ArcTableAux.extendBodyCells
        { this with ValueHeaders = nextHeaders; Values = nextBody }
        
    static member addColumns (columns: CompositeColumn []) (index: int option) (table:ArcTable) =
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
        let otherHeaders = this.ValueHeaders |> Array.removeAt index
        ArcTableAux.SanityChecks.validateNoDuplicateUnique column.Header otherHeaders
        let nextHeader = 
            // create clone of array
            let nc = this.ValueHeaders |> Array.copy
            nc.[index] <- column.Header
            nc
        let nextBody =
            // create clone of dictionary
            let nb = System.Collections.Generic.Dictionary(this.Values)
            column.Cells |> Array.iteri (fun i v -> nb.[(index,i)] <- v)
            nb
        {
            this with
                ValueHeaders = nextHeader
                Values = nextBody
        }

    static member setColumn (index:int) (column:CompositeColumn) (table:ArcTable) = table.SetColumn(index, column)
        
    member this.SetHeader (index:int, newHeader: CompositeHeader, ?forceConvertCells: bool) =
        let forceConvertCells = Option.defaultValue false forceConvertCells
        ArcTableAux.SanityChecks.validateIndex index this.ColumnCount
        /// remove to be replaced header, this is only used to check if any OTHER header is of the same unique type as column.Header
        let otherHeaders = this.ValueHeaders |> Array.removeAt index
        ArcTableAux.SanityChecks.validateNoDuplicateUnique newHeader otherHeaders
        // Test if column is still valid with new header
        let c = { this.GetColumn(index) with Header = newHeader }
        if c.validate() then
            let nextHeaders = 
                // create clone of array
                let h = this.ValueHeaders |> Array.copy
                h.[index] <- newHeader
                h
            { this with ValueHeaders = nextHeaders }
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
