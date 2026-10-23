namespace ARCtrl.Contract

open ARCtrl
open ARCtrl.FileSystem
open ARCtrl.Spreadsheet
open ARCtrl.ArcPathHelper
open ARCtrl.Helper
open FsSpreadsheet


[<AutoOpen>]
module WorkflowContractExtensions = 

    let (|WorkflowPath|_|) (input) =
        match input with
        | [|WorkflowsFolderName; anyWorkflowName; WorkflowFileName|] -> 
            let path = combineMany input
            Some path
        | _ -> None

    let (|WorkflowCWLPath|_|) (input) =
        match input with
        | [|WorkflowsFolderName; anyWorkflowName; WorkflowCWLFileName|] -> 
            let path = combineMany input
            Some path
        | _ -> None

    type ArcWorkflow with

        member this.ToCreateContract (?WithFolder) =
            let withFolder = defaultArg WithFolder false
            let path = Identifier.Workflow.fileNameFromIdentifier this.Identifier
            let c = Contract.createCreate(path, DTOType.ISA_Workflow, DTO.Spreadsheet (this |> ArcWorkflow.toFsWorkbook))
            
            [|
                if withFolder then 
                    let folderFS =  FileSystemTree.createWorkflowsFolder([|FileSystemTree.createWorkflowFolder this.Identifier|])
                    for p in folderFS.ToFilePaths(false) do
                        if p <> path && p <> "workflows/.gitkeep" then Contract.createCreate(p, DTOType.PlainText)
                if this.CWLDescription.IsSome then
                    let path = Identifier.Workflow.cwlFileNameFromIdentifier this.Identifier
                    let dto = CWL.Encode.encodeProcessingUnitInGraph this.CWLGraph this.CWLDescription.Value
                    Contract.createCreate(path, DTOType.CWL, DTO.Text dto)
                c
            |]


        member this.ToUpdateContract () =
            let path = Identifier.Workflow.fileNameFromIdentifier this.Identifier
            let c = Contract.createUpdate(path, DTOType.ISA_Workflow, DTO.Spreadsheet (this |> ArcWorkflow.toFsWorkbook))
            c

        member this.ToDeleteContract () =
            let path = getWorkflowFolderPath(this.Identifier)
            let c = Contract.createDelete(path)
            c

        static member toDeleteContract (workflow: ArcWorkflow) : Contract =
            workflow.ToDeleteContract()

        static member toCreateContract (workflow: ArcWorkflow,?WithFolder) : Contract [] =
            workflow.ToCreateContract(?WithFolder = WithFolder)

        static member toUpdateContract (workflow: ArcWorkflow) : Contract =
            workflow.ToUpdateContract()

        static member tryFromReadContract (c:Contract) =
            match c with
            | {Operation = READ; DTOType = Some DTOType.ISA_Workflow; DTO = Some (DTO.Spreadsheet fsworkbook)} ->
                fsworkbook
                |> ArcWorkflow.fromFsWorkbook
                |> Some 
            | _ -> None

        /// The CWL file of the workflow as a document, which is one process or a packed document.
        static member tryCWLDocumentFromReadContract (workflowIdentifier : string) (c:Contract) =
            let p = Identifier.Workflow.cwlFileNameFromIdentifier workflowIdentifier
            match c with
            | {Operation = READ; DTOType = Some DTOType.CWL; DTO = Some (DTO.Text cwl)} when c.Path = p->
                Some (CWL.Decode.decodeCWLDocument cwl)
            | _ -> None

        /// The process that the CWL file of the workflow starts with. A packed document runs from its process main.
        static member tryCWLFromReadContract (workflowIdentifier : string) (c:Contract) =
            ArcWorkflow.tryCWLDocumentFromReadContract workflowIdentifier c
            |> Option.map (fun document ->
                match CWL.CWLDocument.tryGetMain document with
                | Some processingUnit -> processingUnit
                | None -> failwith $"The packed CWL document {c.Path} has no process with the id main.")
