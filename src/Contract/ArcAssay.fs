namespace ARCtrl.Contract

open ARCtrl.FileSystem
open ARCtrl.Path
open ARCtrl.Spreadsheet
open ARCtrl
open ARCtrl.Helper
open FsSpreadsheet


[<AutoOpen>]
module AssayContractExtensions = 

    let (|AssayPath|_|) (input) =
        match input with
        | [|AssaysFolderName; anyAssayName; AssayFileName|] -> 
            let path = ARCtrl.Path.combineMany input
            Some path
        | _ -> None

    type ArcAssay with

        member this.ToCreateContract (?WithFolder) =
            let withFolder = defaultArg WithFolder false
            let path = Identifier.Assay.fileNameFromIdentifier this.Identifier
            let c = Contract.createCreate(path, DTOType.ISA_Assay, DTO.Spreadsheet (this |> ArcAssay.toFsWorkbook))
            [|
                if withFolder then 
                    let folderFS = FileSystemTree.createAssayFolder this.Identifier
                    for p in folderFS.ToFilePaths(false) do
                        if p <> path then Contract.createCreate(p, DTOType.PlainText)
                c
            |]


        member this.ToUpdateContract () =
            let path = Identifier.Assay.fileNameFromIdentifier this.Identifier
            let c = Contract.createUpdate(path, DTOType.ISA_Assay, DTO.Spreadsheet (this |> ArcAssay.toFsWorkbook))
            c

        member this.ToDeleteContract () =
            let path = getAssayFolderPath(this.Identifier)
            let c = Contract.createDelete(path)
            c

        static member toDeleteContract (assay: ArcAssay) : Contract =
            assay.ToDeleteContract()

        static member toCreateContract (assay: ArcAssay,?WithFolder) : Contract [] =
            assay.ToCreateContract(?WithFolder = WithFolder)

        static member toUpdateContract (assay: ArcAssay) : Contract =
            assay.ToUpdateContract()

        static member tryFromReadContract (c:Contract) =
            match c with
            | {Operation = READ; DTOType = Some DTOType.ISA_Assay; DTO = Some (DTO.Spreadsheet fsworkbook)} ->
                fsworkbook :?> FsWorkbook
                |> ArcAssay.fromFsWorkbook
                |> Some 
            | _ -> None
