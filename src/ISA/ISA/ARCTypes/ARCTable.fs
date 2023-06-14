namespace ISA

open Fable.Core

[<AttachMembers>]
type ArcTable = 
    {
        Name : string
        ValueHeaders : CompositeHeader []
        /// Key: Column * Row
        Values : System.Collections.Generic.Dictionary<int*int,CompositeCell>  
    }

module ArcTableAux =

    /// This function is used to init the correct format from ArcTable.ValueHeaders for `insertColumn`.
    let flattenBodyCells (table: ArcTable) =
        seq {
            for KeyValue((columnIndex,rowIndex),v) in table.Values do
                yield (columnIndex,rowIndex),v
        }

    let (|IsUniqueExistingHeader|_|) existingHeaders (input: CompositeHeader) = 
        match input with
        | CompositeHeader.Parameter _
        | CompositeHeader.Factor _
        | CompositeHeader.Characteristic _
        | CompositeHeader.Component _
        | CompositeHeader.FreeText _        -> None
        // Input and Output does not look very clean :/
        | CompositeHeader.Output _          -> Array.tryFindIndex (fun h -> match h with | CompositeHeader.Output _ -> true | _ -> false) existingHeaders
        | CompositeHeader.Input _           -> Array.tryFindIndex (fun h -> match h with | CompositeHeader.Input _ -> true | _ -> false) existingHeaders
        | header                            -> Array.tryFindIndex (fun h -> h = header) existingHeaders
        
    /// Returns the column index of the duplicate unique column in `existingHeaders`.
    let tryFindDuplicateUnique (newHeader: CompositeHeader) (existingHeaders: CompositeHeader []) = 
        match newHeader with
        | IsUniqueExistingHeader existingHeaders index -> Some index
        | _ -> None

    ///
    let insertColumn (header: CompositeHeader) (cells: CompositeCell []) (index: int) (existingHeaders: CompositeHeader []) (existingCells: seq<(int*int)*CompositeCell>) =
        let mutable numberOfNewColumns = 1
        let mutable index = index
        let hasDuplicateUnique = tryFindDuplicateUnique header existingHeaders
        // Example: existingCells contains `Output io` (With io being any IOType) and header is `Output RawDataFile`. This should replace the existing `Output io`.
        // In this case the number of new columns drops to 0 and we insert the index of the existing `Output io` column.
        if hasDuplicateUnique.IsSome then
            numberOfNewColumns <- 0
            index <- hasDuplicateUnique.Value
        // headers are easily added. Just insert at position of index. This will insert without replace.
        let nextHeaders = 
            let ra = ResizeArray(existingHeaders)
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
                    let nextColumnIndex = if columnIndex >= index then columnIndex + numberOfNewColumns else columnIndex
                    yield 
                        (nextColumnIndex,rowIndex),v
            }
        nextHeaders, nextBody

    // We need to calculate the max number of rows between the new columns and the existing columns in the table.
    // `maxRows` will be the number of rows all columns must have after adding the new columns.
    // This behaviour should be intuitive for the user, as Excel handles this case in the same way.
    let extendBodyCells (maxRows:int) (nextHeaders: CompositeHeader []) (cells:seq<(int*int)*CompositeCell>) =
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
                    let empty = if relatedHeader.IsTermColumn then CompositeCell.emptyTerm else CompositeCell.emptyFreeText
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

// I tested it with Fable Repl. Splitting the type will still handle all members with `AttachMembersAttribute`. ~Kevin F
// Fable Repl test snippet:
//```fsharp
//open Fable.Core

//[<AttachMembersAttribute>]
//type MyType = {
//    TestName: string
//}

//type MyType with

//    member this.NamePlus (plus:string) = this.TestName + plus

//let x = {TestName = "Kevin"}

//x.NamePlus(" Frey")
//```

type ArcTable with

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

    member this.AddColumn (header:CompositeHeader, ?cells: CompositeCell [], ?index: int) = 
        let index = Option.defaultValue this.ColumnCount index
        let cells = Option.defaultValue [||] cells
        if index < 0 then failwith "Cannot insert CompositeColumn at index < 0."
        if index > this.ColumnCount then failwith $"Specified index is out of table range! Table contains only {this.ColumnCount} columns."
        CompositeColumn.create(header, cells).validate(true) |> ignore
        // We need to calculate the max number of rows between the new columns and the existing columns in the table.
        // `maxRows` will be the number of rows all columns must have after adding the new columns.
        // This behaviour should be intuitive for the user, as Excel handles this case in the same way.
        let maxRows = System.Math.Max(this.RowCount, cells.Length)
        let nextHeaders, nextBody =
            this 
            |> ArcTableAux.flattenBodyCells
            |> ArcTableAux.insertColumn header cells index this.ValueHeaders
            ||> ArcTableAux.extendBodyCells maxRows
        { this with ValueHeaders = nextHeaders; Values = nextBody }

    // WIP
    member this.AddColumns (columns: CompositeColumn [], ?index: int) = 
        let index = Option.defaultValue this.ColumnCount index
        if index < 0 then failwith "Cannot insert CompositeColumn at index < 0."
        if index > this.ColumnCount then failwith $"Specified index is out of table range! Table contains only {this.ColumnCount} columns."
        columns |> Array.iter (fun x -> x.validate(true) |> ignore)
        let nNewColumn = columns.Length
        // header and rows are added in two separate instances.
        let columnHeaders, columnRows = columns |> Array.map (fun x -> x.Header, x.Cells) |> Array.unzip
        // headers are easily added. Just insert at position of index. This will insert without replace.
        let nextHeaders = 
            let ra = ResizeArray(this.ValueHeaders)
            ra.InsertRange(index, columnHeaders)
            ra.ToArray()
        // We need to calculate the max number of rows between the new columns and the existing columns in the table.
        // `maxRows` will be the number of rows all columns must have after adding the new columns.
        // This behaviour should be intuitive for the user, as Excel handles this case in the same way.
        let maxRows = 
            let maxRows = columnRows |> Array.map (fun x -> x.Length) |> Array.max
            System.Math.Max(this.RowCount, maxRows)
        // Init a sequence in which we will yield all existing and new columns, while increasing column indizes where necessary.
        let nextHeaders, nextBody =
            seq {
                for i, cells in (Array.indexed columnRows) do
                    let columnIndex = i + index
                    for rowIndex,v in (Array.indexed cells) do
                        yield 
                            (columnIndex,rowIndex),v
                for KeyValue((columnIndex,rowIndex),v) in this.Values do
                    let nextColumnIndex = if columnIndex >= index then columnIndex + nNewColumn else columnIndex
                    yield 
                        (nextColumnIndex,rowIndex),v
            }
            |> ArcTableAux.extendBodyCells maxRows nextHeaders
        { this with ValueHeaders = nextHeaders; Values = nextBody }
        
    static member addColumn (header: CompositeHeader) (cells: CompositeCell []) (index: int option) (table:ArcTable) =
        table.AddColumn(header, cells, ?index = index)

    static member addColumns (columns: CompositeColumn []) (index: int option) (table:ArcTable) =
        table.AddColumns(columns, ?index = index)

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
