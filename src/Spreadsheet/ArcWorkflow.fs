namespace ARCtrl.Spreadsheet

open ARCtrl
open ARCtrl.Helper
open FsSpreadsheet

open Workflow
open System.Collections.Generic

module ArcWorkflow =

    let [<Literal>] metadataSheetName = "isa_workflow"
        
    let fromRows (rows : seq<SparseRow>) = 

        let en = rows.GetEnumerator()

        let rec loop lastRow workflow contacts rowNumber =
               
            match lastRow with
            | Some prefix when prefix = workflowLabel -> 
                let currentRow, rowNumber, _, workflow = Workflow.fromRows (rowNumber + 1) en       
                loop currentRow (Some workflow) contacts rowNumber

            | Some prefix when prefix = contactsLabel -> 
                let currentLine, rowNumber, _, contacts = Contacts.fromRows (Some contactsLabelPrefix) (rowNumber + 1) en  
                loop currentLine workflow contacts rowNumber
            | _ -> 
                match workflow, contacts with
                | None, contacts -> ArcWorkflow.create(Identifier.createMissingIdentifier(), contacts = ResizeArray contacts)
                | Some workflow, contacts ->
                    workflow.Contacts <- ResizeArray contacts
                    workflow
        
        if en.MoveNext () then
            let currentLine = en.Current |> SparseRow.tryGetValueAt 0
            loop currentLine None [] 1
            
        else
            failwith "empty workflow metadata sheet"
   
    let toRows (workflow : ArcWorkflow) =

        seq {
            yield  SparseRow.fromValues [workflowLabel]
            yield! toRows workflow

            yield  SparseRow.fromValues [contactsLabel]
            yield! Contacts.toRows (Some contactsLabelPrefix) (List.ofSeq workflow.Contacts)
        }

    let toMetadataSheet (workflow : ArcWorkflow) : FsWorksheet =
        let sheet = FsWorksheet(metadataSheetName)
        workflow
        |> toRows
        |> Seq.iteri (fun rowI r -> SparseRow.writeToSheet (rowI + 1) r sheet)    
        sheet

    let fromMetadataSheet (sheet : FsWorksheet) : ArcWorkflow =
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

    let toMetadataCollection (workflow : ArcWorkflow) =
        workflow
        |> toRows
        |> Seq.map (fun row -> SparseRow.getAllValues row)

    let fromMetadataCollection (collection : seq<seq<string option>>) : ArcWorkflow =
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
module ArcWorkflowExtensions =

    type ArcWorkflow with

        /// Reads an workflow from a spreadsheet
        static member fromFsWorkbook (doc : FsWorkbook) : ArcWorkflow = 
            try
                // Reading the "Workflow" metadata sheet. Here metadata 
                let workflowMetadata = 
                    match ArcWorkflow.tryGetMetadataSheet doc with
                    | Option.Some sheet ->
                        ArcWorkflow.fromMetadataSheet sheet
                    | None -> 
                        printfn "Cannot retrieve metadata: Workflow file does not contain \"%s\" sheet." ArcWorkflow.metadataSheetName
                        ArcWorkflow.create(Identifier.createMissingIdentifier())
                let sheets = doc.GetWorksheets()
                let datamapSheet =
                    sheets |> Seq.tryPick DatamapTable.tryFromFsWorksheet
                workflowMetadata.Datamap <- datamapSheet
                workflowMetadata
            with
            | err -> failwithf "Could not parse assay: \n%s" err.Message
            

        /// <summary>
        /// Write a workflow to a spreadsheet
        /// </summary>
        /// <param name="workflow"></param>
        static member toFsWorkbook (workflow : ArcWorkflow) =
            let doc = new FsWorkbook()
            let metadataSheet = ArcWorkflow.toMetadataSheet (workflow)
            doc.AddWorksheet metadataSheet

            doc

        /// Write a workflow to a spreadsheet
        member this.ToFsWorkbook () =
            ArcWorkflow.toFsWorkbook (this)