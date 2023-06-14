module ISA.Spreadsheet.ArcAssay

open ISA
open FsSpreadsheet

let toMetadataSheet (assay : ARCAssay) : FsWorksheet =
    raise (System.NotImplementedException())

let fromMetadataSheet (sheet : FsWorksheet) : ARCAssay =
    raise (System.NotImplementedException())

/// Reads an assay from a spreadsheet
let fromFsWorkbook (doc:FsWorkbook) = 
    // Reading the "Assay" metadata sheet. Here metadata 
    let assayMetaData = 
        
        match doc.TryGetWorksheetByName "Assay" with 
        | Option.Some sheet ->
            fromMetadataSheet sheet
        | None -> 
            printfn "Cannot retrieve metadata: Assay file does not contain \"Assay\" sheet."
            ARCAssay.create()     
    let sheets = 
        doc.GetWorksheets()
        |> List.choose ArcTable.tryFromFsWorksheet
    if sheets.IsEmpty then
        assayMetaData
    else {
        assayMetaData with Sheets = Some sheets   
    }

let toFsWorkbook (assay : ARCAssay) =
    let doc = new FsWorkbook()
    let metaDataSheet = toMetadataSheet assay
    doc.AddWorksheet metaDataSheet

    assay.Sheets
    |> Option.defaultValue []
    |> List.iter (ArcTable.toFsWorksheet >> doc.AddWorksheet)

    doc