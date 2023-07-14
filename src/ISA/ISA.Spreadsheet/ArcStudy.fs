module ISA.Spreadsheet.ArcStudy

open ISA
open FsSpreadsheet

let [<Literal>] obsoloteStudiesLabel = "STUDY METADATA"
let [<Literal>] studiesLabel = "STUDY"

let [<Literal>] metaDataSheetName = "Study"


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
            printfn "Cannot retrieve metadata: Study file does not contain \"%s\" sheet." metaDataSheetName
            None   
        |> Option.defaultValue (ArcStudy.createEmpty(Identifier.createMissingIdentifier()))
    let sheets = 
        doc.GetWorksheets()
        |> List.choose ArcTable.tryFromFsWorksheet
    if sheets.IsEmpty then
        studyMetadata
    else
        {
            studyMetadata with Tables = ResizeArray(sheets)
        }

let toFsWorkbook (study : ArcStudy) =
    let study = {study with Identifier = Identifier.removeMissingIdentifier(study.Identifier)}
    let doc = new FsWorkbook()
    let metaDataSheet = toMetadataSheet study
    doc.AddWorksheet metaDataSheet

    study.Tables
    |> Seq.iter (ArcTable.toFsWorksheet >> doc.AddWorksheet)

    doc