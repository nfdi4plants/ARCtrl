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
                    let dto = CWL.Encode.encodeProcessingUnit this.CWLDescription.Value
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
                fsworkbook :?> FsWorkbook
                |> ArcWorkflow.fromFsWorkbook
                |> Some 
            | _ -> None

        static member tryCWLFromReadContract (workflowIdentifier : string) (c:Contract) =
            let p = Identifier.Workflow.cwlFileNameFromIdentifier workflowIdentifier
            match c with
            | {Operation = READ; DTOType = Some DTOType.CWL; DTO = Some (DTO.Text cwl)} when c.Path = p->
                cwl
                |> CWL.Decode.decodeCWLProcessingUnit
                |> Some 
            | _ -> None
