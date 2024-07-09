module ARCtrl.Spreadsheet.DataMap

open ARCtrl
open ArcTable
open FsSpreadsheet


/// Reads an datamap from a spreadsheet
let fromFsWorkbook (doc:FsWorkbook) = 
    try
        let dataMapTable = 
            doc.GetWorksheets()
            |> Seq.tryPick DataMapTable.tryFromFsWorksheet
        match dataMapTable with
        | Some table -> table
        | None -> failwith "No DataMapTable found in workbook"
    with
    | err -> failwithf "Could not parse datamap: \n%s" err.Message
            
let toFsWorkbook (dataMap : DataMap) =
    let doc = new FsWorkbook()

    DataMapTable.toFsWorksheet dataMap
    |> doc.AddWorksheet
    doc