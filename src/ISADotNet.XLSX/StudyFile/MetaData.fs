namespace ISADotNet.XLSX.StudyFile

open FsSpreadsheet.ExcelIO
open ISADotNet
open ISADotNet.XLSX

/// Functions for reading and writing the additional information stored in the study metadata sheet
module MetaData =

    let obsoloteStudiesLabel = "STUDY METADATA"
    let studiesLabel = "STUDY"

    /// Write Study Metadata to excel rows
    let toRows (study:Study) =
        seq {          
            yield  SparseRow.fromValues [studiesLabel]
            yield! ISADotNet.XLSX.Study.StudyInfo.toRows study
        }
        
    /// Read Study Metadata from excel rows
    let fromRows (rows: seq<SparseRow>) =
        let en = rows.GetEnumerator()
        en.MoveNext() |> ignore  
        let _,_,_,study = Study.fromRows 2 en
        study
        // Upper version will eventually become obsolete. Then the version below might be used
        //let _,_,_,studyInfo = Study.StudyInfo.fromRows 2 en
        //Study.fromParts studyInfo [] [] [] [] [] []

    let toDSLSheet s =
        toRows s
        |> Seq.map SparseRow.toDSLRow

    /// Diesen Block durch JS ersetzen ----> 

    open DocumentFormat.OpenXml.Packaging
    open DocumentFormat.OpenXml.Spreadsheet

    /// Creates a new row from the given values.
    let ofSparseValues rowIndex (vals : 'T option seq) =
        let spans = Row.Spans.fromBoundaries 1u (Seq.length vals |> uint)
        vals
        |> Seq.mapi (fun i value -> 
            value
            |> Option.map (Cell.fromValue None (i + 1 |> uint) rowIndex)
        )
        |> Seq.choose id
        |> Row.create rowIndex spans 

    /// Append an study metadata sheet with the given sheetname to an existing study file excel spreadsheet
    let init sheetName (study : Study option) (doc: DocumentFormat.OpenXml.Packaging.SpreadsheetDocument) = 

        let sheet = SheetData.empty()

        let study = Option.defaultValue Study.empty study

        toRows study
        |> Seq.mapi (fun i row -> 
            row
            |> SparseRow.getAllValues
            |> ofSparseValues (i+1 |> uint)
        )
        |> Seq.fold (fun s r -> 
            SheetData.appendRow r s
        ) sheet
        |> ignore

        doc
        |> Spreadsheet.getWorkbookPart
        |> WorkbookPart.appendSheet sheetName sheet
        |> ignore 

        doc


    /// Try get study from metadatasheet with given sheetName
    let tryGetStudy sheetName (doc : SpreadsheetDocument) = 
        match Spreadsheet.tryGetSheetBySheetName sheetName doc with
        | Some sheet -> 
            sheet
            |> SheetData.getRows
            |> Seq.map (Row.getIndexedValues None >> Seq.map (fun (i,v) -> (int i) - 1, v))
            |> fromRows           
        | None -> failwithf "Metadata sheetname %s could not be found" sheetName

    /// Replaces study metadata from metadatasheet with given sheetName
    let overwriteWithStudyInfo sheetName study (doc : SpreadsheetDocument) = 

        let workBookPart = Spreadsheet.getWorkbookPart doc
        let newSheet = SheetData.empty()
        
        match Spreadsheet.tryGetSheetBySheetName sheetName doc with
        | Some sheet ->                  
            toRows study
            |> Seq.mapi (fun i row -> 
                row
                |> SparseRow.getAllValues
                |> ofSparseValues (i+1 |> uint)
            )
            |> Seq.fold (fun s r -> 
                SheetData.appendRow r s
            ) newSheet
            |> fun s -> WorkbookPart.replaceSheetDataByName sheetName s workBookPart
        | None -> failwithf "Metadata sheetname %s could not be found" sheetName
        |> ignore

        doc.Save() 

    /// ---->  Bis hier
