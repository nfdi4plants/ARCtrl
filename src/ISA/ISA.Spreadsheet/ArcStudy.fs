module ISA.Spreadsheet.ArcStudy

open ISA
open FsSpreadsheet

let obsoloteStudiesLabel = "STUDY METADATA"
let studiesLabel = "STUDY"

let metaDataSheetName = "Study"


let toMetadataSheet (study : ARCStudy) : FsWorksheet =
    let toRows (study:ARCStudy) =
        seq {          
            yield  SparseRow.fromValues [studiesLabel]
            yield! Studies.StudyInfo.toRows study
        }
    let sheet = FsWorksheet(metaDataSheetName)
    study
    |> toRows
    |> Seq.iteri (fun rowI r -> SparseRow.writeToSheet rowI r sheet)    
    sheet

let fromMetadataSheet (sheet : FsWorksheet) : ARCStudy =
    let fromRows (rows: seq<SparseRow>) =
        let en = rows.GetEnumerator()
        en.MoveNext() |> ignore  
        let _,_,_,study = Studies.fromRows 2 en
        study
    sheet.Rows 
    |> Seq.map SparseRow.fromFsRow
    |> fromRows

/// Reads an assay from a spreadsheet
let fromFsWorkbook (doc:FsWorkbook) = 
    // Reading the "Assay" metadata sheet. Here metadata 
    let studyMetadata = 
        
        match doc.TryGetWorksheetByName metaDataSheetName with 
        | Option.Some sheet ->
            fromMetadataSheet sheet
        | None -> 
            printfn "Cannot retrieve metadata: Study file does not contain \"%s\" sheet." metaDataSheetName
            ARCStudy.create()     
    let sheets = 
        doc.GetWorksheets()
        |> List.choose ArcTable.tryFromFsWorksheet
    if sheets.IsEmpty then
        studyMetadata
    else {
        studyMetadata with Sheets = Some sheets   
    }

let toFsWorkbook (study : ARCStudy) =
    let doc = new FsWorkbook()
    let metaDataSheet = toMetadataSheet study
    doc.AddWorksheet metaDataSheet

    study.Sheets
    |> Option.defaultValue []
    |> List.iter (ArcTable.toFsWorksheet >> doc.AddWorksheet)

    doc