namespace ARCtrl.Contract

open ARCtrl
open ARCtrl.FileSystem
open ARCtrl.Spreadsheet
open ARCtrl.ArcPathHelper
open ARCtrl.Helper
open FsSpreadsheet


[<AutoOpen>]
module RunContractExtensions = 

    let (|RunPath|_|) (input) =
        match input with
        | [|RunsFolderName; anyRunName; RunFileName|] -> 
            let path = combineMany input
            Some path
        | _ -> None

    let (|RunCWLPath|_|) (input) =
        match input with
        | [|RunsFolderName; anyRunName; RunCWLFileName|] -> 
            let path = combineMany input
            Some path
        | _ -> None

    let (|RunYMLPath|_|) (input) =
        match input with
        | [|RunsFolderName; anyRunName; RunYMLFileName|] -> 
            let path = combineMany input
            Some path
        | _ -> None

    type ArcRun with

        /// The contract that carries the CWL file of the run, or none for a run without a description. The operation is
        /// CREATE for a file that is new and UPDATE for a file that changed.
        member this.TryToCWLContract (operation: Operation) : Contract option =
            this.CWLDescription
            |> Option.map (fun description ->
                let path = Identifier.Run.cwlFileNameFromIdentifier this.Identifier
                let dto = CWL.Encode.encodeProcessingUnitInGraph this.CWLGraph description
                Contract.create(operation, path, DTOType.CWL, DTO.Text dto))

        /// The contract that carries the parameter file run.yml of the run, or none for a run without parameter
        /// references. The operation is CREATE for a file that is new and UPDATE for a file that changed.
        member this.TryToYMLContract (operation: Operation) : Contract option =
            if this.CWLInput.Count = 0 then
                None
            else
                let path = Identifier.Run.ymlFileNameFromIdentifier this.Identifier
                let dto = CWL.Encode.encodeYAMLParameterFile this.CWLInput
                Some (Contract.create(operation, path, DTOType.YAML, DTO.Text dto))

        member this.ToCreateContract (?WithFolder) =
            let withFolder = defaultArg WithFolder false
            let path = Identifier.Run.fileNameFromIdentifier this.Identifier
            let c = Contract.createCreate(path, DTOType.ISA_Run, DTO.Spreadsheet (this |> ArcRun.toFsWorkbook))
            [|
                if withFolder then 
                    let folderFS =  FileSystemTree.createRunsFolder([|FileSystemTree.createRunFolder this.Identifier|])
                    for p in folderFS.ToFilePaths(false) do
                        if p <> path && p <> "runs/.gitkeep" then Contract.createCreate(p, DTOType.PlainText)
                match this.TryToCWLContract Operation.CREATE with
                | Some contract -> contract
                | None -> ()
                match this.TryToYMLContract Operation.CREATE with
                | Some contract -> contract
                | None -> ()
                c
            |]


        member this.ToUpdateContract () =
            let path = Identifier.Run.fileNameFromIdentifier this.Identifier
            let c = Contract.createUpdate(path, DTOType.ISA_Run, DTO.Spreadsheet (this |> ArcRun.toFsWorkbook))
            c

        member this.ToDeleteContract () =
            let path = getRunFolderPath(this.Identifier)
            let c = Contract.createDelete(path)
            c

        static member toDeleteContract (run: ArcRun) : Contract =
            run.ToDeleteContract()

        static member toCreateContract (run: ArcRun,?WithFolder) : Contract [] =
            run.ToCreateContract(?WithFolder = WithFolder)

        static member toUpdateContract (run: ArcRun) : Contract =
            run.ToUpdateContract()

        static member tryFromReadContract (c:Contract) =
            match c with
            | {Operation = READ; DTOType = Some DTOType.ISA_Run; DTO = Some (DTO.Spreadsheet fsworkbook)} ->
                fsworkbook
                |> ArcRun.fromFsWorkbook
                |> Some 
            | _ -> None

        /// The CWL file of the run as a document, which is one process or a packed document.
        static member tryCWLDocumentFromReadContract (runIdentifier : string) (c:Contract) =
            let p = Identifier.Run.cwlFileNameFromIdentifier runIdentifier
            match c with
            | {Operation = READ; DTOType = Some DTOType.CWL; DTO = Some (DTO.Text text)} when c.Path = p ->
                Some (decodeCWLFile c.Path text)
            | _ -> None

        /// The process that the CWL file of the run starts with. A packed document runs from its process main.
        static member tryCWLFromReadContract (runIdentifier : string) (c:Contract) =
            ArcRun.tryCWLDocumentFromReadContract runIdentifier c
            |> Option.map (fun document ->
                match CWL.CWLDocument.tryGetMain document with
                | Some processingUnit -> processingUnit
                | None -> failwith $"The packed CWL document {c.Path} has no process with the id main.")

        static member tryYMLFromReadContract (runIdentifier : string) (c:Contract) =
            let p = Identifier.Run.ymlFileNameFromIdentifier runIdentifier
            match c with
            | {Operation = READ; DTOType = Some DTOType.YAML; DTO = Some (DTO.Text text)} when c.Path = p ->
                decodeParameterFile c.Path text |> Some
            | _ -> None
