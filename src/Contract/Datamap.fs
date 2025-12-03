namespace ARCtrl.Contract

open ARCtrl.FileSystem
open ARCtrl.ArcPathHelper
open ARCtrl.Spreadsheet
open ARCtrl
open ARCtrl.Helper
open FsSpreadsheet

[<AutoOpen>]
module DatamapContractExtensions = 

    let (|DatamapPath|_|) (input) =
        match input with
        | [|AssaysFolderName; anyAssayName; DatamapFileName|] -> 
            let path = ARCtrl.ArcPathHelper.combineMany input
            Some path
        | [|StudiesFolderName; anyStudyName; DatamapFileName|] -> 
            let path = ARCtrl.ArcPathHelper.combineMany input
            Some path
        | [|WorkflowsFolderName; anyWorkflowName; DatamapFileName|] ->
            let path = ARCtrl.ArcPathHelper.combineMany input
            Some path
        | [|RunsFolderName; anyRunName; DatamapFileName|] ->
            let path = ARCtrl.ArcPathHelper.combineMany input
            Some path
        | _ -> None

    type Datamap with

        // Assay

        member this.ToCreateContractForAssay (assayIdentifier : string) =
            let path = Identifier.Assay.datamapFileNameFromIdentifier assayIdentifier
            Contract.createCreate(path, DTOType.ISA_Datamap, DTO.Spreadsheet (this |> Datamap.toFsWorkbook))

        member this.ToUpdateContractForAssay (assayIdentifier : string) =
            let path = Identifier.Assay.datamapFileNameFromIdentifier assayIdentifier
            let c = Contract.createUpdate(path, DTOType.ISA_Datamap, DTO.Spreadsheet (this |> Datamap.toFsWorkbook))
            c

        member this.ToDeleteContractForAssay (assayIdentifier : string) =
            let path = Identifier.Assay.datamapFileNameFromIdentifier assayIdentifier
            let c = Contract.createDelete(path)
            c

        static member toDeleteContractForAssay (assayIdentifier : string) =
            fun (datamap : Datamap) -> 
                datamap.ToDeleteContractForAssay(assayIdentifier)

        static member toUpdateContractForAssay (assayIdentifier : string) =
            fun (datamap : Datamap) -> 
                datamap.ToUpdateContractForAssay(assayIdentifier)

        static member tryFromReadContractForAssay (assayIdentifier : string) (c:Contract) =
            let path = Identifier.Assay.datamapFileNameFromIdentifier assayIdentifier
            match c with
            | {Path = p; Operation = READ; DTOType = Some DTOType.ISA_Datamap; DTO = Some (DTO.Spreadsheet fsworkbook)} when p = path ->
                let dm = 
                    fsworkbook :?> FsWorkbook
                    |> Datamap.fromFsWorkbook                
                dm.StaticHash <- dm.GetHashCode()
                dm
                |> Some 
            | _ -> None

        // Study

        member this.ToCreateContractForStudy (studyIdentifier : string) =
            let path = Identifier.Study.datamapFileNameFromIdentifier studyIdentifier
            Contract.createCreate(path, DTOType.ISA_Datamap, DTO.Spreadsheet (this |> Datamap.toFsWorkbook))

        member this.ToUpdateContractForStudy (studyIdentifier : string) =
            let path = Identifier.Study.datamapFileNameFromIdentifier studyIdentifier
            let c = Contract.createUpdate(path, DTOType.ISA_Datamap, DTO.Spreadsheet (this |> Datamap.toFsWorkbook))
            c

        member this.ToDeleteContractForStudy (studyIdentifier : string) =
            let path = Identifier.Study.datamapFileNameFromIdentifier studyIdentifier
            let c = Contract.createDelete(path)
            c

        static member toDeleteContractForStudy (studyIdentifier : string) =
            fun (datamap : Datamap) -> 
                datamap.ToDeleteContractForStudy(studyIdentifier)

        static member toUpdateContractForStudy (studyIdentifier : string) =
            fun (datamap : Datamap) -> 
                datamap.ToUpdateContractForStudy(studyIdentifier)


        static member tryFromReadContractForStudy (studyIdentifier : string) (c:Contract) =
            let path = Identifier.Study.datamapFileNameFromIdentifier studyIdentifier
            match c with
            | {Path = p; Operation = READ; DTOType = Some DTOType.ISA_Datamap; DTO = Some (DTO.Spreadsheet fsworkbook)} when p = path->
                let dm = 
                    fsworkbook :?> FsWorkbook
                    |> Datamap.fromFsWorkbook                
                dm.StaticHash <- dm.GetHashCode()
                Some (dm)
            | _ -> None

        // Workflow

        member this.ToCreateContractForWorkflow (workflowIdentifier : string) =
            let path = Identifier.Workflow.datamapFileNameFromIdentifier workflowIdentifier
            Contract.createCreate(path, DTOType.ISA_Datamap, DTO.Spreadsheet (this |> Datamap.toFsWorkbook))

        member this.ToUpdateContractForWorkflow (workflowIdentifier : string) =
            let path = Identifier.Workflow.datamapFileNameFromIdentifier workflowIdentifier
            let c = Contract.createUpdate(path, DTOType.ISA_Datamap, DTO.Spreadsheet (this |> Datamap.toFsWorkbook))
            c

        member this.ToDeleteContractForWorkflow (workflowIdentifier : string) =
            let path = Identifier.Workflow.datamapFileNameFromIdentifier workflowIdentifier
            let c = Contract.createDelete(path)
            c

        static member toDeleteContractForWorkflow (workflowIdentifier : string) =
            fun (datamap : Datamap) -> 
                datamap.ToDeleteContractForWorkflow(workflowIdentifier)

        static member toUpdateContractForWorkflow (workflowIdentifier : string) =
            fun (datamap : Datamap) -> 
                datamap.ToUpdateContractForWorkflow(workflowIdentifier)

        static member tryFromReadContractForWorkflow (workflowIdentifier : string) (c:Contract) =
            let path = Identifier.Workflow.datamapFileNameFromIdentifier workflowIdentifier
            match c with
            | {Path = p; Operation = READ; DTOType = Some DTOType.ISA_Datamap; DTO = Some (DTO.Spreadsheet fsworkbook)} when p = path->
                let dm = 
                    fsworkbook :?> FsWorkbook
                    |> Datamap.fromFsWorkbook                
                dm.StaticHash <- dm.GetHashCode()
                Some (dm)
            | _ -> None

        // Run

        member this.ToCreateContractForRun (runIdentifier : string) =
            let path = Identifier.Run.datamapFileNameFromIdentifier runIdentifier
            Contract.createCreate(path, DTOType.ISA_Datamap, DTO.Spreadsheet (this |> Datamap.toFsWorkbook))

        member this.ToUpdateContractForRun (runIdentifier : string) =
            let path = Identifier.Run.datamapFileNameFromIdentifier runIdentifier
            let c = Contract.createUpdate(path, DTOType.ISA_Datamap, DTO.Spreadsheet (this |> Datamap.toFsWorkbook))
            c

        member this.ToDeleteContractForRun (runIdentifier : string) =
            let path = Identifier.Run.datamapFileNameFromIdentifier runIdentifier
            let c = Contract.createDelete(path)
            c

        static member toDeleteContractForRun (runIdentifier : string) =
            fun (datamap : Datamap) -> 
                datamap.ToDeleteContractForRun(runIdentifier)

        static member toUpdateContractForRun (runIdentifier : string) =
            fun (datamap : Datamap) -> 
                datamap.ToUpdateContractForRun(runIdentifier)

        static member tryFromReadContractForRun (runIdentifier : string) (c:Contract) =
            let path = Identifier.Run.datamapFileNameFromIdentifier runIdentifier
            match c with
            | {Path = p; Operation = READ; DTOType = Some DTOType.ISA_Datamap; DTO = Some (DTO.Spreadsheet fsworkbook)} when p = path->
                let dm = 
                    fsworkbook :?> FsWorkbook
                    |> Datamap.fromFsWorkbook                
                dm.StaticHash <- dm.GetHashCode()
                Some (dm)
            | _ -> None