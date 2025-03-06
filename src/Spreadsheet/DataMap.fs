module ARCtrl.Spreadsheet.DataMap

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
            |> Seq.tryPick DataMapTable.tryFromFsWorksheet
        match dataMapTable with
        | Some table -> table
        | None -> 
            if worksheets |> Seq.forall sheetIsEmpty then
                DataMap.init()
            else
                failwith "No DataMapTable was found in any of the sheets of the workbook"
    with
    | err -> failwithf "Could not parse datamap: \n%s" err.Message
            
let toFsWorkbook (dataMap : DataMap) =
    let doc = new FsWorkbook()

    DataMapTable.toFsWorksheet dataMap
    |> doc.AddWorksheet
    doc