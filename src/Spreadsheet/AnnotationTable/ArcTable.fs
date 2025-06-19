module ARCtrl.Spreadsheet.ArcTable

open ARCtrl
open ARCtrl.Helper
open FsSpreadsheet

// I think we really should not add FSharpAux for exactly one function.
module Aux =

    module List =

        /// Iterates over elements of the input list and groups adjacent elements.
        /// A new group is started when the specified predicate holds about the element
        /// of the list (and at the beginning of the iteration).
        ///
        /// For example: 
        ///    List.groupWhen isOdd [3;3;2;4;1;2] = [[3]; [3; 2; 4]; [1; 2]]
        let groupWhen f list =
            list
            |> List.fold (
                fun acc e ->
                    match f e, acc with
                    | true  , _         -> [e] :: acc       // true case
                    | false , h :: t    -> (e :: h) :: t    // false case, non-empty acc list
                    | false , _         -> [[e]]            // false case, empty acc list
            ) []
            |> List.map List.rev
            |> List.rev
 

type ColumnOrder =
    | InputClass = 1
    | ProtocolClass = 2
    | ParamsClass = 3
    | OutputClass = 4

let classifyHeaderOrder (header : CompositeHeader) =     
    match header with
    | CompositeHeader.Input             _ -> ColumnOrder.InputClass

    | CompositeHeader.ProtocolType          
    | CompositeHeader.ProtocolDescription
    | CompositeHeader.ProtocolUri
    | CompositeHeader.ProtocolVersion
    | CompositeHeader.ProtocolREF       
    | CompositeHeader.Performer
    | CompositeHeader.Date                -> ColumnOrder.ProtocolClass

    | CompositeHeader.Component         _
    | CompositeHeader.Characteristic    _
    | CompositeHeader.Factor            _
    | CompositeHeader.Parameter         _ 
    | CompositeHeader.Comment           _
    | CompositeHeader.FreeText          _ -> ColumnOrder.ParamsClass

    | CompositeHeader.Output            _ -> ColumnOrder.OutputClass

let classifyColumnOrder (column : CompositeColumn) =
    column.Header
    |> classifyHeaderOrder

[<Literal>]
let annotationTablePrefix = "annotationTable"

let helperColumnStrings = 
    [
        "Term Source REF"
        "Term Accession Number"
        "Unit"
        "Data Format"
        "Data Selector Format"
    ]

let groupColumnsByHeader (stringCellColumns : array<string []>) = 
    stringCellColumns
    |> Array.toList
    |> Aux.List.groupWhen (fun c -> 
        let v = c.[0]
        helperColumnStrings
        |> List.exists (fun s -> v.StartsWith s) 
        |> not
    )
    |> Array.ofList
    |> Array.map Array.ofList

/// Returns the annotation table of the worksheet if it exists, else returns None
let tryAnnotationTable (sheet : FsWorksheet) =
    sheet.Tables
    |> Seq.tryFind (fun t -> t.Name.StartsWith annotationTablePrefix)

/// Groups and parses a collection of single columns into the according ISA composite columns
let composeColumns (stringCellColumns : array<string []>) : CompositeColumn [] =
    stringCellColumns
    |> groupColumnsByHeader
    |> Array.map CompositeColumn.fromStringCellColumns

/// Returns the protocol described by the headers and a function for parsing the values of the matrix to the processes of this protocol
let tryFromFsWorksheet (sheet : FsWorksheet) =
    try
        match tryAnnotationTable sheet with
        | Some (t: FsTable) -> 
            let stringCellColumns = 
                [|
                for c = 1 to t.RangeAddress.LastAddress.ColumnNumber do
                    [|for r = 1 to t.RangeAddress.LastAddress.RowNumber do
                        match sheet.CellCollection.TryGetCell(r,c) with
                        | Some cell -> cell.ValueAsString()
                        | None -> ""
                    |]
                |]              
            let compositeColumns = 
                stringCellColumns
                |> Array.map CompositeColumn.fixDeprecatedIOHeader
                |> composeColumns
            ArcTable.init sheet.Name
            |> ArcTable.addColumns(compositeColumns)
            |> Some
        | None ->
            None
    with
    | err -> failwithf "Could not parse table with name \"%s\":\n%s" sheet.Name err.Message


let toFsWorksheet (index : int option) (table : ArcTable) =
    /// This dictionary is used to add spaces at the end of duplicate headers.
    let stringCount = System.Collections.Generic.Dictionary<string,string>()
    let ws = FsWorksheet(table.Name)

    // Cancel if there are no columns
    if table.ColumnCount = 0 then ws
    else

    let columns = 
        table.Columns
        |> List.ofSeq
        |> List.sortBy classifyColumnOrder
        |> List.collect CompositeColumn.toStringCellColumns
    let tableRowCount =
        let maxRow = columns |> Seq.fold (fun acc c -> max acc c.Length) 0
        if maxRow = 1 then 2
        else maxRow
    let tableColumnCount = columns.Length
    let name =
        match index with
        | Some i -> $"{annotationTablePrefix}{i}"
        | None -> annotationTablePrefix
    let fsTable = ws.Table(name,FsRangeAddress(FsAddress(1,1),FsAddress(tableRowCount,tableColumnCount)))
    columns
    |> List.iteri (fun colI col ->         
        col
        |> List.iteri (fun rowI stringCell -> 
            let value = 
                if rowI = 0 then
                    
                    match Dictionary.tryGet stringCell stringCount with
                    | Some spaces ->
                        stringCount.[stringCell] <- spaces + " "
                        stringCell + " " + spaces
                    | None ->
                        stringCount.Add(stringCell,"")
                        stringCell
                else stringCell
            let address = FsAddress(rowI+1,colI+1)
            fsTable.Cell(address, ws.CellCollection).SetValueAs value
        )  
    )
    ws