namespace ARCtrl.ISA.Spreadsheet

open ARCtrl.ISA
open FsSpreadsheet


module ArcStudy = 

    let [<Literal>] obsoleteStudiesLabel = "STUDY METADATA"
    let [<Literal>] studiesLabel = "STUDY"

    let [<Literal>] obsoleteMetaDataSheetName = "Study"
    let [<Literal>] metaDataSheetName = "isa_study"

    let toMetadataSheet (study : ArcStudy) (assays : ArcAssay list option) : FsWorksheet =
        //let toRows (study:ArcStudy) assays =
        //    seq {          
        //        yield  SparseRow.fromValues [studiesLabel]
        //        yield! Studies.StudyInfo.toRows study
        //    }
        let sheet = FsWorksheet(metaDataSheetName)
        Studies.toRows study assays
        |> Seq.append [SparseRow.fromValues [studiesLabel]]
        |> Seq.iteri (fun rowI r -> SparseRow.writeToSheet (rowI + 1) r sheet)    
        sheet

    let fromMetadataSheet (sheet : FsWorksheet) : ArcStudy*ArcAssay list =
        let fromRows (rows: seq<SparseRow>) =
            let en = rows.GetEnumerator()
            en.MoveNext() |> ignore  
            let _,_,_,study = Studies.fromRows 2 en
            study
        sheet.Rows 
        |> Seq.map SparseRow.fromFsRow
        |> fromRows
        |> Option.defaultValue (ArcStudy.create(Identifier.createMissingIdentifier()),[])

[<AutoOpen>]
module Extensions =

    type ArcStudy with
    
        /// Reads an assay from a spreadsheet
        static member fromFsWorkbook (doc:FsWorkbook) = 
            // Reading the "Assay" metadata sheet. Here metadata 
            let studyMetadata,assays = 
        
                match doc.TryGetWorksheetByName ArcStudy.metaDataSheetName with 
                | Option.Some sheet ->
                    ArcStudy.fromMetadataSheet sheet
                | None ->  
                    match doc.TryGetWorksheetByName ArcStudy.obsoleteMetaDataSheetName with 
                    | Option.Some sheet ->
                        ArcStudy.fromMetadataSheet sheet
                    | None -> 
                        printfn "Cannot retrieve metadata: Study file does not contain \"%s\" or \"%s\" sheet." ArcStudy.metaDataSheetName ArcStudy.obsoleteMetaDataSheetName
                        ArcStudy.create(Identifier.createMissingIdentifier()),[]

            let sheets = 
                doc.GetWorksheets()
                |> Seq.choose ArcTable.tryFromFsWorksheet
            if sheets |> Seq.isEmpty |> not then
                let updatedTables = 
                        ArcTables.updateReferenceTablesBySheets(
                            (ArcTables studyMetadata.Tables),
                            (ArcTables (ResizeArray sheets)),
                            keepUnusedRefTables =  true
                            )
                studyMetadata.Tables <- updatedTables.Tables
            studyMetadata
            ,assays

        static member toFsWorkbook (study : ArcStudy,?assays : ArcAssay list) =
            let doc = new FsWorkbook()
            let metaDataSheet = ArcStudy.toMetadataSheet study assays
            doc.AddWorksheet metaDataSheet

            study.Tables
            |> Seq.iter (ArcTable.toFsWorksheet >> doc.AddWorksheet)

            doc