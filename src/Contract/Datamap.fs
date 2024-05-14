namespace ARCtrl.Contract

open ARCtrl.FileSystem
open ARCtrl.Path
open ARCtrl.Spreadsheet
open ARCtrl
open ARCtrl.Helper
open FsSpreadsheet

[<AutoOpen>]
module DatamapContractExtensions = 

    let (|DatamapPath|_|) (input) =
        match input with
        | [|AssaysFolderName; anyAssayName; DataMapFileName|] -> 
            let path = ARCtrl.Path.combineMany input
            Some path
        | [|StudiesFolderName; anyStudyName; DataMapFileName|] -> 
            let path = ARCtrl.Path.combineMany input
            Some path
        | _ -> None

    type DataMap with

        member this.ToCreateContractForAssay (assayIdentifier : string) =
            let path = Identifier.Assay.datamapFileNameFromIdentifier assayIdentifier
            Contract.createCreate(path, DTOType.ISA_Datamap, DTO.Spreadsheet (this |> DataMap.toFsWorkbook))

        member this.ToUpdateContractForAssay (assayIdentifier : string) =
            let path = Identifier.Assay.datamapFileNameFromIdentifier assayIdentifier
            let c = Contract.createUpdate(path, DTOType.ISA_Datamap, DTO.Spreadsheet (this |> DataMap.toFsWorkbook))
            c

        member this.ToDeleteContractForAssay (assayIdentifier : string) =
            let path = Identifier.Assay.datamapFileNameFromIdentifier assayIdentifier
            let c = Contract.createDelete(path)
            c

        static member toDeleteContractForAssay (assayIdentifier : string) =
            fun (dataMap : DataMap) -> 
                dataMap.ToDeleteContractForAssay(assayIdentifier)

        static member toUpdateContractForAssay (assayIdentifier : string) =
            fun (dataMap : DataMap) -> 
                dataMap.ToUpdateContractForAssay(assayIdentifier)

        static member tryFromReadContractForAssay (assayIdentifier : string) (c:Contract) =
            let path = Identifier.Assay.datamapFileNameFromIdentifier assayIdentifier
            match c with
            | {Path = p; Operation = READ; DTOType = Some DTOType.ISA_Datamap; DTO = Some (DTO.Spreadsheet fsworkbook)} when p = path->
                fsworkbook :?> FsWorkbook
                |> DataMap.fromFsWorkbook
                |> Some 
            | _ -> None


        member this.ToCreateContractForStudy (studyIdentifier : string) =
            let path = Identifier.Study.datamapFileNameFromIdentifier studyIdentifier
            Contract.createCreate(path, DTOType.ISA_Datamap, DTO.Spreadsheet (this |> DataMap.toFsWorkbook))

        member this.ToUpdateContractForStudy (studyIdentifier : string) =
            let path = Identifier.Study.datamapFileNameFromIdentifier studyIdentifier
            let c = Contract.createUpdate(path, DTOType.ISA_Datamap, DTO.Spreadsheet (this |> DataMap.toFsWorkbook))
            c

        member this.ToDeleteContractForStudy (studyIdentifier : string) =
            let path = Identifier.Study.datamapFileNameFromIdentifier studyIdentifier
            let c = Contract.createDelete(path)
            c

        static member toDeleteContractForStudy (studyIdentifier : string) =
            fun (dataMap : DataMap) -> 
                dataMap.ToDeleteContractForStudy(studyIdentifier)

        static member toUpdateContractForStudy (studyIdentifier : string) =
            fun (dataMap : DataMap) -> 
                dataMap.ToUpdateContractForStudy(studyIdentifier)


        static member tryFromReadContractForStudy (studyIdentifier : string) (c:Contract) =
            let path = Identifier.Study.datamapFileNameFromIdentifier studyIdentifier
            match c with
            | {Path = p; Operation = READ; DTOType = Some DTOType.ISA_Datamap; DTO = Some (DTO.Spreadsheet fsworkbook)} when p = path->
                fsworkbook :?> FsWorkbook
                |> DataMap.fromFsWorkbook
                |> Some 
            | _ -> None
