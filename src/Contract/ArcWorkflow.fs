namespace ARCtrl.Contract

open ARCtrl
open ARCtrl.FileSystem
open ARCtrl.Spreadsheet
open ARCtrl.ArcPathHelper
open ARCtrl.Helper
open FsSpreadsheet


/// Reading the CWL files and the parameter file of an ARC. The readers of the spreadsheets of an ARC throw when a file
/// does not parse, and they name the kind of file in the message. A CWL file or a parameter file that does not parse
/// throws the error of the decoder as it is, with the path of the file added to the message. The place of the error
/// in the document stays in the error.
[<AutoOpen>]
module CWLFileReading =

    let private readingFile (path: string) (read: unit -> 'T) : 'T =
        let withFile (message: string) = $"{message} (file {path})"
        try
            read ()
        with
        | YAMLicious.YamlDecodeException error ->
            raise (YAMLicious.YamlDecodeException { error with Message = withFile error.Message })
        | YAMLicious.YamlParseException error ->
            raise (YAMLicious.YamlParseException { error with Message = withFile error.Message })

    /// Decodes the text of the CWL file at the path.
    let decodeCWLFile (path: string) (text: string) : CWL.CWLProcessingUnit =
        readingFile path (fun () -> CWL.Decode.decodeCWLProcessingUnit text)

    /// Decodes the text of the parameter file at the path.
    let decodeParameterFile (path: string) (text: string) : ResizeArray<CWL.CWLParameterReference> =
        readingFile path (fun () -> CWL.DecodeParameters.decodeYAMLParameterFile text)

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

        /// The contract that carries the CWL file of the workflow, or none for a workflow without a description. The
        /// operation is CREATE for a file that is new and UPDATE for a file that changed.
        member this.TryToCWLContract (operation: Operation) : Contract option =
            this.CWLDescription
            |> Option.map (fun description ->
                let path = Identifier.Workflow.cwlFileNameFromIdentifier this.Identifier
                let dto = CWL.Encode.encodeProcessingUnit description
                Contract.create(operation, path, DTOType.CWL, DTO.Text dto))

        member this.ToCreateContract (?WithFolder) =
            let withFolder = defaultArg WithFolder false
            let path = Identifier.Workflow.fileNameFromIdentifier this.Identifier
            let c = Contract.createCreate(path, DTOType.ISA_Workflow, DTO.Spreadsheet (this |> ArcWorkflow.toFsWorkbook))
            
            [|
                if withFolder then 
                    let folderFS =  FileSystemTree.createWorkflowsFolder([|FileSystemTree.createWorkflowFolder this.Identifier|])
                    for p in folderFS.ToFilePaths(false) do
                        if p <> path && p <> "workflows/.gitkeep" then Contract.createCreate(p, DTOType.PlainText)
                match this.TryToCWLContract Operation.CREATE with
                | Some contract -> contract
                | None -> ()
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

        static member tryCWLFromReadContract (workflowIdentifier : string) (c:Contract) =
            let p = Identifier.Workflow.cwlFileNameFromIdentifier workflowIdentifier
            match c with
            | {Operation = READ; DTOType = Some DTOType.CWL; DTO = Some (DTO.Text cwl)} when c.Path = p->
                Some (decodeCWLFile c.Path cwl)
            | _ -> None
