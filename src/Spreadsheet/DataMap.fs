module ARCtrl.Spreadsheet.Datamap

open ARCtrl
open ArcTable
open FsSpreadsheet

/// Reads an datamap from a spreadsheet
let fromFsWorkbook (doc : FsWorkbook) = 
    try
        let worksheets = doc.GetWorksheets()
        let sheetIsEmpty (sheet : FsWorksheet) = sheet.CellCollection.Count = 0
        let dataMapTable = 
            worksheets
            |> Seq.tryPick DatamapTable.tryFromFsWorksheet
        match dataMapTable with
        | Some table -> table
        | None -> 
            if worksheets |> Seq.forall sheetIsEmpty then
                Datamap.init()
            else
                failwith "No DatamapTable was found in any of the sheets of the workbook"
    with
    | err -> failwithf "Could not parse datamap: \n%s" err.Message
            
let toFsWorkbook (dataMap : Datamap) =
    let doc = new FsWorkbook()

    DatamapTable.toFsWorksheet dataMap
    |> doc.AddWorksheet
    doc