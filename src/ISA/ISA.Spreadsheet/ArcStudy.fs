module ISA.Spreadsheet.ArcStudy

open ISA
open FsSpreadsheet

let [<Literal>] obsoleteStudiesLabel = "STUDY METADATA"
let [<Literal>] studiesLabel = "STUDY"

let [<Literal>] obsoleteMetaDataSheetName = "Study"
let [<Literal>] metaDataSheetName = "isa_study"


let toMetadataSheet (study : ArcStudy) : FsWorksheet =
    let toRows (study:ArcStudy) =
        seq {          
            yield  SparseRow.fromValues [studiesLabel]
            yield! Studies.StudyInfo.toRows study
        }
    let sheet = FsWorksheet(metaDataSheetName)
    study
    |> toRows
    |> Seq.iteri (fun rowI r -> SparseRow.writeToSheet (rowI + 1) r sheet)    
    sheet

let fromMetadataSheet (sheet : FsWorksheet) : ArcStudy option =
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
            match doc.TryGetWorksheetByName obsoleteMetaDataSheetName with 
            | Option.Some sheet ->
                fromMetadataSheet sheet
            | None -> 
                printfn "Cannot retrieve metadata: Study file does not contain \"%s\" or \"%s\" sheet." metaDataSheetName obsoleteMetaDataSheetName
                None   
              
        |> Option.defaultValue (ArcStudy.init(Identifier.createMissingIdentifier()))
    let sheets = 
        doc.GetWorksheets()
        |> List.choose ArcTable.tryFromFsWorksheet
    if sheets.IsEmpty then
        studyMetadata
    else
        studyMetadata.Tables <- ResizeArray(sheets)
        studyMetadata

let toFsWorkbook (study : ArcStudy) =
    let doc = new FsWorkbook()
    let metaDataSheet = toMetadataSheet study
    doc.AddWorksheet metaDataSheet

    study.Tables
    |> Seq.iter (ArcTable.toFsWorksheet >> doc.AddWorksheet)

    doc