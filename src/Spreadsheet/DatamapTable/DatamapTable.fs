module ARCtrl.Spreadsheet.DatamapTable

open ARCtrl
open ArcTable
open FsSpreadsheet

[<Literal>]
let datamapTablePrefix = "datamapTable"

let helperColumnStrings = 
    [
        "Term Source REF"
        "Term Accession Number"
        "Data Format"
        "Data Selector Format"
    ]

let groupColumnsByHeader (columns : list<FsColumn>) = 
    columns
    |> Aux.List.groupWhen (fun c -> 
        let v = c.[1].ValueAsString()
        helperColumnStrings
        |> List.exists (fun s -> v.StartsWith s) 
        |> not
    )

/// Returns the annotation table of the worksheet if it exists, else returns None
let tryDatamapTable (sheet : FsWorksheet) =
    sheet.Tables
    |> Seq.tryFind (fun t -> t.Name.StartsWith datamapTablePrefix)

/// Groups and parses a collection of single columns into the according ISA composite columns
let composeColumns (columns : seq<FsColumn>) : ResizeArray<DataContext> =
    let l = (columns |> Seq.item 0).MaxRowIndex - 1
    let dc = ResizeArray([| for i = 0 to l - 1 do yield DataContext()|])
    columns
    |> Seq.toList
    |> groupColumnsByHeader
    |> List.iter (DatamapColumn.setFromFsColumns dc >> ignore)
    dc

/// Returns the protocol described by the headers and a function for parsing the values of the matrix to the processes of this protocol
let tryFromFsWorksheet (sheet : FsWorksheet) =
    try
        match tryDatamapTable sheet with
        | Some (t: FsTable) -> 
            let dataContexts = 
                t.GetColumns(sheet.CellCollection)
                |> composeColumns
            Datamap(dataContexts)
            |> Some
        | None ->
            None
    with
    | err -> failwithf "Could not parse datamap table with name \"%s\":\n%s" sheet.Name err.Message

let toFsWorksheet (table : Datamap) =
    /// This dictionary is used to add spaces at the end of duplicate headers.
    let stringCount = System.Collections.Generic.Dictionary<string,string>()
    let ws = FsWorksheet("isa_datamap")

    // Cancel if there are no columns
    if table.DataContexts.Count = 0 then ws
    else

    let columns = 
        DatamapColumn.toFsColumns table.DataContexts
    let maxRow = columns.Head.Length
    let maxCol = columns.Length
    let fsTable = ws.Table("datamapTable",FsRangeAddress(FsAddress(1,1),FsAddress(maxRow,maxCol)))
    columns
    |> List.iteri (fun colI col ->         
        col
        |> List.iteri (fun rowI cell -> 
            let value = 
                let v = cell.ValueAsString()
                if rowI = 0 then
                    
                    match Dictionary.tryGet v stringCount with
                    | Some spaces ->
                        stringCount.[v] <- spaces + " "
                        v + " " + spaces
                    | None ->
                        stringCount.Add(cell.ValueAsString(),"")
                        v
                else v
            let address = FsAddress(rowI+1,colI+1)
            fsTable.Cell(address, ws.CellCollection).SetValueAs value
        )  
    )
    ws.RescanRows()
    ws