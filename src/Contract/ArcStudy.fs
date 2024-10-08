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

        member this.ToCreateContract (?WithFolder, ?datamapAsFile) =
            let withFolder = defaultArg WithFolder false
            let dataMapAsFile = defaultArg datamapAsFile false
            let path = Identifier.Study.fileNameFromIdentifier this.Identifier
            let dto = DTO.Spreadsheet (ArcStudy.toFsWorkbook(this, datamapSheet = not dataMapAsFile))
            let c = Contract.createCreate(path, DTOType.ISA_Study, dto)
            
            [|
                if withFolder then 
                    let folderFS = FileSystemTree.createStudiesFolder ([|FileSystemTree.createStudyFolder this.Identifier|])
                    for p in folderFS.ToFilePaths(false) do
                        if p <> path && p <> "studies/.gitkeep" then yield Contract.createCreate(p, DTOType.PlainText)
                match this.DataMap with
                    | Some dm -> 
                        dm.StaticHash <- dm.GetHashCode()
                        if dataMapAsFile then
                            yield dm.ToCreateContractForStudy(this.Identifier)
                    | _ -> ()
                yield c
            |]

        member this.ToUpdateContract (?datamapAsFile) =
            let datamapAsFile = defaultArg datamapAsFile false
            let path = Identifier.Study.fileNameFromIdentifier this.Identifier
            let hash = this.GetLightHashCode()
            let datamapHasChanged = 
                match this.DataMap with
                | Some dm -> 
                    let hc = dm.GetHashCode() <> dm.StaticHash
                    dm.StaticHash <- dm.GetHashCode()
                    hc
                | _ -> false
            let createStudyContract() =                 
                let dto = DTO.Spreadsheet (ArcStudy.toFsWorkbook(this, datamapSheet = not datamapAsFile))
                Contract.createUpdate(path, DTOType.ISA_Study, dto)                  
            [|
                if hash <> this.StaticHash || (datamapHasChanged && not datamapAsFile) then
                    createStudyContract()
                if datamapHasChanged && datamapAsFile then
                    this.DataMap.Value.ToUpdateContractForStudy(this.Identifier)

            |]

        member this.ToDeleteContract () =
            let path = getStudyFolderPath(this.Identifier)
            let c = Contract.createDelete(path)
            c

        static member toDeleteContract (study: ArcStudy) : Contract =
            study.ToDeleteContract()

        static member toCreateContract (study: ArcStudy, ?WithFolder) : Contract [] =
            study.ToCreateContract(?WithFolder = WithFolder)

        static member toUpdateContract (study: ArcStudy) : Contract [] =
            study.ToUpdateContract()           

        static member tryFromReadContract (c:Contract) =
            match c with
            | {Operation = READ; DTOType = Some DTOType.ISA_Study; DTO = Some (DTO.Spreadsheet fsworkbook)} ->
                fsworkbook :?> FsWorkbook
                |> ArcStudy.fromFsWorkbook
                |> Some 
            | _ -> None