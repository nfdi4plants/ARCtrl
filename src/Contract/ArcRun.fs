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

        member this.ToCreateContract (?WithFolder) =
            let withFolder = defaultArg WithFolder false
            let path = Identifier.Run.fileNameFromIdentifier this.Identifier
            let c = Contract.createCreate(path, DTOType.ISA_Run, DTO.Spreadsheet (this |> ArcRun.toFsWorkbook))
            [|
                if withFolder then 
                    let folderFS =  FileSystemTree.createRunsFolder([|FileSystemTree.createRunFolder this.Identifier|])
                    for p in folderFS.ToFilePaths(false) do
                        if p <> path && p <> "runs/.gitkeep" then Contract.createCreate(p, DTOType.PlainText)
                if this.CWLDescription.IsSome then
                    let path = Identifier.Run.cwlFileNameFromIdentifier this.Identifier
                    let dto = CWL.Encode.encodeCWLProcessingUnit this.CWLDescription.Value
                    Contract.createCreate(path, DTOType.CWL, DTO.Text dto)
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
                fsworkbook :?> FsWorkbook
                |> ArcRun.fromFsWorkbook
                |> Some 
            | _ -> None

        static member tryCWLFromReadContract (runIdentifier : string) (c:Contract) =
            let p = Identifier.Run.cwlFileNameFromIdentifier runIdentifier
            match c with
            | {Operation = READ; DTOType = Some DTOType.CWL; DTO = Some (DTO.Text text)} when c.Path = p ->
                CWL.Decode.decodeCWLProcessingUnit text |> Some
            | _ -> None

        static member tryYMLFromReadContract (runIdentifier : string) (c:Contract) =
            let p = Identifier.Run.ymlFileNameFromIdentifier runIdentifier
            match c with
            | {Operation = READ; DTOType = Some DTOType.YAML; DTO = Some (DTO.Text text)} when c.Path = p ->
                CWL.DecodeParameters.decodeYAMLParameterFile text |> Some
            | _ -> None