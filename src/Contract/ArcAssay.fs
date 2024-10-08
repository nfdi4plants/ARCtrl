namespace ARCtrl.Contract

open ARCtrl
open ARCtrl.FileSystem
open ARCtrl.Spreadsheet
open ARCtrl.ArcPathHelper
open ARCtrl.Helper
open FsSpreadsheet


[<AutoOpen>]
module AssayContractExtensions = 

    let (|AssayPath|_|) (input) =
        match input with
        | [|AssaysFolderName; anyAssayName; AssayFileName|] -> 
            let path = combineMany input
            Some path
        | _ -> None

    type ArcAssay with

        member this.ToCreateContract (?WithFolder, ?datamapAsFile) =
            let withFolder = defaultArg WithFolder false
            let dataMapAsFile = defaultArg datamapAsFile false
            let path = Identifier.Assay.fileNameFromIdentifier this.Identifier
            let dto = DTO.Spreadsheet (ArcAssay.toFsWorkbook(this, datamapSheet = not dataMapAsFile))
            let c = Contract.createCreate(path, DTOType.ISA_Assay, dto)
            [|
                if withFolder then 
                    let folderFS =  FileSystemTree.createAssaysFolder([|FileSystemTree.createAssayFolder this.Identifier|])
                    for p in folderFS.ToFilePaths(false) do
                        if p <> path && p <> "assays/.gitkeep" then
                            yield Contract.createCreate(p, DTOType.PlainText)              
                match this.DataMap with
                    | Some dm -> 
                        dm.StaticHash <- dm.GetHashCode()
                        if dataMapAsFile then
                            yield dm.ToCreateContractForAssay(this.Identifier)
                    | _ -> ()
                yield c              
            |]


        member this.ToUpdateContract (?datamapAsFile) =
            let dataMapAsFile = defaultArg datamapAsFile false
            let path = Identifier.Assay.fileNameFromIdentifier this.Identifier
            let dto = DTO.Spreadsheet (ArcAssay.toFsWorkbook(this, datamapSheet = not dataMapAsFile))
            let c = Contract.createUpdate(path, DTOType.ISA_Assay, dto)
            [|
                match this.DataMap with
                    | Some dm -> 
                        dm.StaticHash <- dm.GetHashCode()
                        if dataMapAsFile then
                            yield dm.ToUpdateContractForAssay(this.Identifier)
                    | _ -> ()
                yield c
            |]

        member this.ToDeleteContract () =
            let path = getAssayFolderPath(this.Identifier)
            let c = Contract.createDelete(path)
            c

        static member toDeleteContract (assay: ArcAssay) : Contract =
            assay.ToDeleteContract()

        static member toCreateContract (assay: ArcAssay,?WithFolder) : Contract [] =
            assay.ToCreateContract(?WithFolder = WithFolder)

        static member toUpdateContract (assay: ArcAssay) : Contract [] =
            assay.ToUpdateContract()

        static member tryFromReadContract (c:Contract) =
            match c with
            | {Operation = READ; DTOType = Some DTOType.ISA_Assay; DTO = Some (DTO.Spreadsheet fsworkbook)} ->
                fsworkbook :?> FsWorkbook
                |> ArcAssay.fromFsWorkbook
                |> Some 
            | _ -> None
