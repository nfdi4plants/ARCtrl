module ISA.Spreadsheet.ArcTable

open ISA
open FSharpAux
open FsSpreadsheet

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
    | CompositeHeader.FreeText          _ -> ColumnOrder.ParamsClass

    | CompositeHeader.Output            _ -> ColumnOrder.OutputClass

let classifyColumnOrder (column : CompositeColumn) =
    column.Header
    |> classifyHeaderOrder

[<Literal>]
let annotationTablePrefix = "annotationTable"

let groupColumnsByHeader (columns : list<FsColumn>) = 
    columns
    |> List.groupWhen (fun c ->         
        ISA.Regex.tryParseTermAnnotation c.[1].Value 
        |> Option.isNone
        &&
        c.[1].Value <> "Unit"

    )

/// Returns the annotation table of the worksheet if it exists, else returns None
let tryAnnotationTable (sheet : FsWorksheet) =
    sheet.Tables
    |> Seq.tryFind (fun t -> t.Name.StartsWith annotationTablePrefix)

/// Groups and parses a collection of single columns into the according ISA composite columns
let composeColumns (columns : seq<FsColumn>) : CompositeColumn [] =
    columns
    |> Seq.toList
    |> groupColumnsByHeader
    |> List.map CompositeColumn.fromFsColumns
    |> List.toArray

/// Returns the protocol described by the headers and a function for parsing the values of the matrix to the processes of this protocol
let tryFromFsWorksheet (sheet : FsWorksheet) =
    match tryAnnotationTable sheet with
    | Some t -> 
        let compositeColumns = 
            t.Columns(sheet.CellCollection)
            |> composeColumns
        ArcTable.init sheet.Name
        |> ArcTable.addColumns compositeColumns 
        |> Some
    | None ->
        None

let toFsWorksheet (table : ArcTable) =
    let ws = FsWorksheet(table.Name)
    let columns = 
        table.Columns
        |> List.sortBy classifyColumnOrder
        |> List.collect CompositeColumn.toFsColumns
    let maxRow = columns.Head.Length
    let maxCol = columns.Length
    let fsTable = ws.Table("annotationTable",FsRangeAddress(FsAddress(1,1),FsAddress(maxRow,maxCol)))
    columns
    |> List.iteri (fun colI col ->         
        col
        |> List.iteri (fun rowI cell -> 
            let address = FsAddress(rowI+1,colI+1)
            fsTable.Cell(address, ws.CellCollection).SetValueAs cell.Value
        )  
    )
    ws