namespace ARCtrl.Contract

open ARCtrl.FileSystem
open ARCtrl.ArcPathHelper
open ARCtrl.Spreadsheet
open ARCtrl
open ARCtrl.Helper
open FsSpreadsheet


[<AutoOpen>]
module StudyContractExtensions = 

    let (|StudyPath|_|) (input) =
        match input with
        | [|StudiesFolderName; anyStudyName; StudyFileName|] -> 
            let path = ARCtrl.ArcPathHelper.combineMany input
            Some path
        | _ -> None     

    type ArcStudy with

        member this.ToCreateContract (?WithFolder) =
            let withFolder = defaultArg WithFolder false
            let path = Identifier.Study.fileNameFromIdentifier this.Identifier
            let c = Contract.createCreate(path, DTOType.ISA_Study, DTO.Spreadsheet (this |> ArcStudy.toFsWorkbook))
            
            [|
                if withFolder then 
                    let folderFS = FileSystemTree.createStudiesFolder ([|FileSystemTree.createStudyFolder this.Identifier|])
                    for p in folderFS.ToFilePaths(false) do
                        if p <> path && p <> "studies/.gitkeep" then Contract.createCreate(p, DTOType.PlainText)
                c
            |]

        member this.ToUpdateContract () =
            let path = Identifier.Study.fileNameFromIdentifier this.Identifier
            let c = Contract.createUpdate(path, DTOType.ISA_Study, DTO.Spreadsheet (this |> ArcStudy.toFsWorkbook))
            c

        member this.ToDeleteContract () =
            let path = getStudyFolderPath(this.Identifier)
            let c = Contract.createDelete(path)
            c

        static member toDeleteContract (study: ArcStudy) : Contract =
            study.ToDeleteContract()

        static member toCreateContract (study: ArcStudy, ?WithFolder) : Contract [] =
            study.ToCreateContract(?WithFolder = WithFolder)

        static member toUpdateContract (study: ArcStudy) : Contract =
            study.ToUpdateContract()           

        static member tryFromReadContract (c:Contract) =
            match c with
            | {Operation = READ; DTOType = Some DTOType.ISA_Study; DTO = Some (DTO.Spreadsheet fsworkbook)} ->
                fsworkbook :?> FsWorkbook
                |> ArcStudy.fromFsWorkbook
                |> Some 
            | _ -> None