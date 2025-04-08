namespace ARCtrl.Spreadsheet

open ARCtrl
open ARCtrl.Helper
open FsSpreadsheet

open Run

module ArcRun =

    let [<Literal>] metadataSheetName = "isa_run"

    let fromRows (rows : seq<SparseRow>) = 

        let en = rows.GetEnumerator()

        let rec loop lastRow run performers rowNumber =
               
            match lastRow with
            | Some prefix when prefix = runLabel -> 
                let currentRow, rowNumber, _, run = Run.fromRows (rowNumber + 1) en       
                loop currentRow (Some run) performers rowNumber

            | Some prefix when prefix = performersLabel -> 
                let currentLine, rowNumber, _, performers = Contacts.fromRows (Some performersLabelPrefix) (rowNumber + 1) en  
                loop currentLine run performers rowNumber
            | _ -> 
                match run, performers with
                | None, performers -> ArcRun.create(Identifier.createMissingIdentifier(), performers = ResizeArray performers)
                | Some run, performers ->
                    run.Performers <- ResizeArray performers
                    run
        
        if en.MoveNext () then
            let currentLine = en.Current |> SparseRow.tryGetValueAt 0
            loop currentLine None [] 1
            
        else
            failwith "empty run metadata sheet"
   
    let toRows (run : ArcRun) =

        seq {
            yield  SparseRow.fromValues [runLabel]
            yield! toRows run

            yield  SparseRow.fromValues [performersLabel]
            yield! Contacts.toRows (Some performersLabelPrefix) (List.ofSeq run.Performers)
        }

    let toMetadataSheet (run : ArcRun) : FsWorksheet =
        let sheet = FsWorksheet(metadataSheetName)
        run
        |> toRows
        |> Seq.iteri (fun rowI r -> SparseRow.writeToSheet (rowI + 1) r sheet)    
        sheet

    let fromMetadataSheet (sheet : FsWorksheet) : ArcRun =
        try
            let rows =        
                sheet.Rows 
                |> Seq.map SparseRow.fromFsRow
            //let hasPrefix = 
            //    rows
            //    |> Seq.exists (fun row -> row |> Seq.head |> snd |> fun s -> s.StartsWith(assaysPrefix))
            rows
            |> fromRows
        with 
        | err -> failwithf "Failed while parsing metadatasheet: %s" err.Message

    let toMetadataCollection (run : ArcRun) =
        run
        |> toRows
        |> Seq.map (fun row -> SparseRow.getAllValues row)

    let fromMetadataCollection (collection : seq<seq<string option>>) : ArcRun =
        try
            let rows =        
                collection 
                |> Seq.map SparseRow.fromAllValues   
            rows
            |> fromRows
        with 
        | err -> failwithf "Failed while parsing metadatasheet: %s" err.Message

    let isMetadataSheetName (name : string) =
        name = metadataSheetName

    let isMetadataSheet (sheet : FsWorksheet) =
        isMetadataSheetName sheet.Name

    let tryGetMetadataSheet (doc : FsWorkbook) =
        doc.GetWorksheets()
        |> Seq.tryFind isMetadataSheet

[<AutoOpen>]
module ArcRunExtensions =

    type ArcRun with

        /// Reads an run from a spreadsheet
        static member fromFsWorkbook (doc : FsWorkbook) : ArcRun = 
            try
                // Reading the "Run" metadata sheet. Here metadata 
                let runMetadata = 
                    match ArcRun.tryGetMetadataSheet doc with
                    | Option.Some sheet ->
                        ArcRun.fromMetadataSheet sheet
                    | None -> 
                        printfn "Cannot retrieve metadata: Run file does not contain \"%s\" sheet." ArcRun.metadataSheetName
                        ArcRun.create(Identifier.createMissingIdentifier())
                let sheets = doc.GetWorksheets()
                let annotationTables = 
                    sheets |> Seq.choose ArcTable.tryFromFsWorksheet
                let datamapSheet =
                    sheets |> Seq.tryPick DataMapTable.tryFromFsWorksheet
                runMetadata.DataMap <- datamapSheet
                if annotationTables |> Seq.isEmpty |> not then
                    runMetadata.Tables <- ResizeArray annotationTables
                runMetadata
            with
            | err -> failwithf "Could not parse assay: \n%s" err.Message
            

        /// <summary>
        /// Write a run to a spreadsheet
        /// </summary>
        /// <param name="run"></param>
        static member toFsWorkbook (run : ArcRun) =
            let doc = new FsWorkbook()
            let metadataSheet = ArcRun.toMetadataSheet (run)
            doc.AddWorksheet metadataSheet

            run.Tables
            |> Seq.iteri (fun i -> ArcTable.toFsWorksheet (Some i) >> doc.AddWorksheet)


            doc

        /// Write a run to a spreadsheet
        member this.ToFsWorkbook () =
            ArcRun.toFsWorkbook (this)