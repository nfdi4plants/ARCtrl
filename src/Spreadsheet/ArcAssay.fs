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

    let [<Literal>] obsoleteMetadataSheetName = "Assay"
    let [<Literal>] metadataSheetName = "isa_assay"

    let fromRows (rows : seq<SparseRow>) =
        let hasPrefix = 
            rows
            |> Seq.exists (fun row -> row |> Seq.head |> snd |> fun s -> s.StartsWith(assaysPrefix))
        let aPrefix, cPrefix = 
            if hasPrefix then 
                Some assaysPrefix, Some contactsPrefix
            else None, None
        let en = rows.GetEnumerator()
        let rec loop lastRow assays contacts rowNumber =
               
            match lastRow with
            | Some prefix when prefix = assaysLabel || prefix = obsoleteAssaysLabel -> 
                let currentRow, rowNumber, _, assays = Assays.fromRows aPrefix (rowNumber + 1) en       
                loop currentRow assays contacts rowNumber

            | Some prefix when prefix = contactsLabel -> 
                let currentLine, rowNumber, _, contacts = Contacts.fromRows cPrefix (rowNumber + 1) en  
                loop currentLine assays contacts rowNumber
            | _ -> 
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

    let toRows (assay : ArcAssay) =
        seq {          
            yield  SparseRow.fromValues [assaysLabel]
            yield! Assays.toRows (Some assaysPrefix) [assay]

            yield SparseRow.fromValues [contactsLabel]
            yield! Contacts.toRows (Some contactsPrefix) (List.ofSeq assay.Performers)
        }

    let toMetadataSheet (assay : ArcAssay) : FsWorksheet =
        let sheet = FsWorksheet(metadataSheetName)
        assay
        |> toRows
        |> Seq.iteri (fun rowI r -> SparseRow.writeToSheet (rowI + 1) r sheet)    
        sheet

    let fromMetadataSheet (sheet : FsWorksheet) : ArcAssay =
        try
            let rows =        
                sheet.Rows 
                |> Seq.map SparseRow.fromFsRow
            let hasPrefix = 
                rows
                |> Seq.exists (fun row -> row |> Seq.head |> snd |> fun s -> s.StartsWith(assaysPrefix))
            rows
            |> fromRows
        with 
        | err -> failwithf "Failed while parsing metadatasheet: %s" err.Message

    let toMetadataCollection (assay : ArcAssay) =
        assay
        |> toRows
        |> Seq.map (fun row -> SparseRow.getAllValues row)

    let fromMetadataCollection (collection : seq<seq<string option>>) : ArcAssay =
        try
            let rows =        
                collection 
                |> Seq.map SparseRow.fromAllValues   
            rows
            |> fromRows
        with 
        | err -> failwithf "Failed while parsing metadatasheet: %s" err.Message

    let isMetadataSheetName (name : string) =
        name = metadataSheetName || name = obsoleteMetadataSheetName

    let isMetadataSheet (sheet : FsWorksheet) =
        isMetadataSheetName sheet.Name

    let tryGetMetadataSheet (doc : FsWorkbook) =
        doc.GetWorksheets()
        |> Seq.tryFind isMetadataSheet

[<AutoOpen>]
module ArcAssayExtensions =

    type ArcAssay with

        /// Reads an assay from a spreadsheet
        static member fromFsWorkbook (doc : FsWorkbook) : ArcAssay = 
            try
                // Reading the "Assay" metadata sheet. Here metadata 
                let assayMetadata = 
                    match ArcAssay.tryGetMetadataSheet doc with
                    | Option.Some sheet ->
                        ArcAssay.fromMetadataSheet sheet
                    | None -> 
                        printfn "Cannot retrieve metadata: Assay file does not contain \"%s\" or \"%s\" sheet." ArcAssay.metadataSheetName ArcAssay.obsoleteMetadataSheetName
                        ArcAssay.create(Identifier.createMissingIdentifier())
                let sheets = doc.GetWorksheets()
                let annotationTables = 
                    sheets |> Seq.choose ArcTable.tryFromFsWorksheet
                let datamapSheet =
                    sheets |> Seq.tryPick DatamapTable.tryFromFsWorksheet
                if annotationTables |> Seq.isEmpty |> not then
                    assayMetadata.Tables <- ResizeArray annotationTables
                assayMetadata.Datamap <- datamapSheet
                assayMetadata
            with
            | err -> failwithf "Could not parse assay: \n%s" err.Message
            

        /// <summary>
        /// Write an assay to a spreadsheet
        ///
        /// If datamapSheet is true, the datamap will be written to a worksheet inside assay workbook.
        /// </summary>
        /// <param name="assay"></param>
        /// <param name="datamapSheet">Default: true</param>
        static member toFsWorkbook (assay : ArcAssay, ?datamapSheet : bool) =
            let datamapSheet = defaultArg datamapSheet true
            let doc = new FsWorkbook()
            let metadataSheet = ArcAssay.toMetadataSheet (assay)
            doc.AddWorksheet metadataSheet

            if datamapSheet then
                assay.Datamap
                |> Option.iter (DatamapTable.toFsWorksheet >> doc.AddWorksheet)
     
            assay.Tables
            |> Seq.iteri (fun i -> ArcTable.toFsWorksheet (Some i) >> doc.AddWorksheet)

            doc

        /// Write an assay to a spreadsheet
        ///
        /// If datamapSheet is true, the datamap will be written to a worksheet inside assay workbook.
        member this.ToFsWorkbook (?datamapSheet : bool) =
            ArcAssay.toFsWorkbook (this, ?datamapSheet = datamapSheet)