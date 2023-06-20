module ISA.Spreadsheet.ArcTable

open ISA
open FSharpAux
open FsSpreadsheet

let annotationTablePrefix = "annotationTable"

let groupColumnsByHeader (columns : list<FsColumn>) = 
    columns
    |> List.groupWhen (fun c ->         
        ISA.Regex.tryParseTermAnnotation c.[0].Value 
        |> Option.isSome
        ||
        c.[0].Value = "Unit"
    )

/// Returns the protocol described by the headers and a function for parsing the values of the matrix to the processes of this protocol
let tryFromFsWorksheet (sheet : FsWorksheet) =
    let annotationTable =
        sheet.Tables
        |> Seq.tryFind (fun t -> t.Name.StartsWith annotationTablePrefix)
    match annotationTable with
    | Some t -> 
        let compositeColumns = 
            t.Columns(sheet.CellCollection)
            |> Seq.toList
            |> groupColumnsByHeader
            |> List.map CompositeColumn.fromFsColumns
            |> List.toArray
        ArcTable.init sheet.Name
        |> ArcTable.addColumns compositeColumns 
        |> Some
    | None ->
        None

let toFsWorksheet (table : ArcTable) =
    let ws = FsWorksheet(table.Name)
    let columns = 
        table.Columns
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