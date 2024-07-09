namespace ARCtrl.Spreadsheet

open ARCtrl
open ARCtrl.Helper
open FsSpreadsheet

module ArcAssay =

    let [<Literal>] obsoleteAssaysLabel = "ASSAY METADATA"
    let [<Literal>] assaysLabel = "ASSAY"
    let [<Literal>] contactsLabel = "ASSAY PERFORMERS"

    let [<Literal>] assaysPrefix = "Assay"
    let [<Literal>] contactsPrefix = "Assay Person"

    let [<Literal>] obsoleteMetaDataSheetName = "Assay"
    let [<Literal>] metaDataSheetName = "isa_assay"

    let toMetadataSheet (assay : ArcAssay) : FsWorksheet =
        let toRows (assay:ArcAssay) =
            seq {          
                yield  SparseRow.fromValues [assaysLabel]
                yield! Assays.toRows (Some assaysPrefix) [assay]

                yield SparseRow.fromValues [contactsLabel]
                yield! Contacts.toRows (Some contactsPrefix) (List.ofSeq assay.Performers)
            }
        let sheet = FsWorksheet(metaDataSheetName)
        assay
        |> toRows
        |> Seq.iteri (fun rowI r -> SparseRow.writeToSheet (rowI + 1) r sheet)    
        sheet

    let fromMetadataSheet (sheet : FsWorksheet) : ArcAssay =
        try
            let fromRows (usePrefixes : bool) (rows: seq<SparseRow>) =
                let aPrefix,cPrefix = 
                    if usePrefixes then 
                        Some assaysPrefix,Some contactsPrefix
                    else None,None
                let en = rows.GetEnumerator()
                let rec loop lastLine assays contacts lineNumber =
               
                    match lastLine with

                    | Some k when k = assaysLabel || k = obsoleteAssaysLabel -> 
                        let currentLine,lineNumber,_,assays = Assays.fromRows aPrefix (lineNumber + 1) en       
                        loop currentLine assays contacts lineNumber

                    | Some k when k = contactsLabel -> 
                        let currentLine,lineNumber,_,contacts = Contacts.fromRows cPrefix (lineNumber + 1) en  
                        loop currentLine assays contacts lineNumber
                    | k -> 
                        match assays, contacts with
                        | [], [] -> ArcAssay.create(Identifier.createMissingIdentifier())
                        | assays, contacts ->
                            assays
                            |> Seq.tryHead 
                            |> Option.defaultValue (ArcAssay.create(Identifier.createMissingIdentifier()))
                            |> ArcAssay.setPerformers (ResizeArray contacts)
        
                if en.MoveNext () then
                    let currentLine = en.Current |> SparseRow.tryGetValueAt 0
                    loop currentLine [] [] 1
            
                else
                    failwith "empty assay metadata sheet"
            let rows =        
                sheet.Rows 
                |> Seq.map SparseRow.fromFsRow
            let hasPrefix = 
                rows
                |> Seq.exists (fun row -> row |> Seq.head |> snd |> fun s -> s.StartsWith(assaysPrefix))
            rows
            |> fromRows hasPrefix
        with 
        | err -> failwithf "Failed while parsing metadatasheet: %s" err.Message


[<AutoOpen>]
module ArcAssayExtensions =

    type ArcAssay with

        /// Reads an assay from a spreadsheet
        static member fromFsWorkbook (doc:FsWorkbook) : ArcAssay = 
            try
                // Reading the "Assay" metadata sheet. Here metadata 
                let assayMetaData = 
        
                    match doc.TryGetWorksheetByName ArcAssay.metaDataSheetName with 
                    | Option.Some sheet ->
                        ArcAssay.fromMetadataSheet sheet
                    | None -> 
                        match doc.TryGetWorksheetByName ArcAssay.obsoleteMetaDataSheetName with 
                        | Option.Some sheet ->
                            ArcAssay.fromMetadataSheet sheet
                        | None -> 
                            printfn "Cannot retrieve metadata: Assay file does not contain \"%s\" or \"%s\" sheet." ArcAssay.metaDataSheetName ArcAssay.obsoleteMetaDataSheetName
                            ArcAssay.create(Identifier.createMissingIdentifier())
                let sheets = doc.GetWorksheets()
                let annotationTables = 
                    sheets |> Seq.choose ArcTable.tryFromFsWorksheet
                let datamapSheet =
                    sheets |> Seq.tryPick DataMapTable.tryFromFsWorksheet
                if annotationTables |> Seq.isEmpty |> not then
                    assayMetaData.Tables <- ResizeArray annotationTables
                assayMetaData.DataMap <- datamapSheet
                assayMetaData
            with
            | err -> failwithf "Could not parse assay: \n%s" err.Message
            

        /// <summary>
        /// Write an assay to a spreadsheet
        ///
        /// If datamapSheet is true, the datamap will be written to a worksheet inside assay workbook.
        /// </summary>
        /// <param name="assay"></param>
        /// <param name="datamapSheet">Default: true</param>
        static member toFsWorkbook (assay : ArcAssay, ?datamapSheet: bool) =
            let datamapSheet = defaultArg datamapSheet true
            let doc = new FsWorkbook()
            let metaDataSheet = ArcAssay.toMetadataSheet (assay)
            doc.AddWorksheet metaDataSheet

            if datamapSheet then
                assay.DataMap
                |> Option.iter (DataMapTable.toFsWorksheet >> doc.AddWorksheet)
     
            assay.Tables
            |> Seq.iter (ArcTable.toFsWorksheet >> doc.AddWorksheet)

            doc