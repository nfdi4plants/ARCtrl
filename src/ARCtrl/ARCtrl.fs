﻿namespace ARCtrl

open ARCtrl.FileSystem
open ARCtrl.Contract
open ARCtrl.ISA
open FsSpreadsheet
open Fable.Core

module ARCAux =

    // No idea where to move this
    let getArcAssaysFromContracts (contracts: Contract []) = 
        contracts 
        |> Array.choose ARCtrl.Contract.ArcAssay.tryFromContract
        |> Array.map (fun x -> x :?> FsWorkbook |> ISA.Spreadsheet.ArcAssay.fromFsWorkbook)

    // No idea where to move this
    let getArcStudiesFromContracts (contracts: Contract []) =
        contracts 
        |> Array.choose Contract.ArcStudy.tryFromContract
        |> Array.map (fun x -> x :?> FsWorkbook |> ISA.Spreadsheet.ArcStudy.fromFsWorkbook)

    let getArcInvestigationFromContracts (contracts: Contract []) =
        contracts 
        |> Array.choose Contract.ArcInvestigation.tryFromContract
        |> Array.exactlyOne 
        |> fun x -> x :?> FsWorkbook |> ISA.Spreadsheet.ArcInvestigation.fromFsWorkbook

    let updateFSByISA (isa : ArcInvestigation option) (fs : FileSystem) = 
        let (studyNames,assayNames) = 
            match isa with
            | Some inv ->         
                inv.Studies
                |> Seq.fold (fun (studyNames,assayNames) s ->
                    Array.append studyNames [|s.Identifier|],
                    Array.append assayNames (s.Assays |> Seq.map (fun a -> a.Identifier) |> Array.ofSeq)
            
                ) ([||],[||])
            | None -> ([||],[||])
        let assays = FileSystemTree.createAssaysFolder (assayNames |> Array.map FileSystemTree.createAssayFolder)
        let studies = FileSystemTree.createStudiesFolder (studyNames |> Array.map FileSystemTree.createStudyFolder)
        let investigation = FileSystemTree.createInvestigationFile()
        let tree = 
            FileSystemTree.createRootFolder [|investigation;assays;studies|]
            |> FileSystem.create
        fs.Union(tree)    

    let updateFSByCWL (cwl : CWL.CWL option) (fs : FileSystem) =       
        let workflows = FileSystemTree.createWorkflowsFolder [||]
        let runs = FileSystemTree.createRunsFolder [||]       
        let tree = 
            FileSystemTree.createRootFolder [|workflows;runs|]
            |> FileSystem.create
        fs.Union(tree)    

[<AttachMembers>]
type ARC(?isa : ISA.ArcInvestigation, ?cwl : CWL.CWL, ?fs : FileSystem.FileSystem) =

    let mutable cwl = cwl
    let mutable fs = 
        fs
        |> Option.defaultValue (FileSystem.create(FileSystemTree.Folder ("",[||])))
        |> ARCAux.updateFSByISA isa
        |> ARCAux.updateFSByCWL cwl

    member val ISA : ArcInvestigation option = isa with get, set

    member this.CWL 
        with get() = cwl

    member this.FileSystem 
        with get() = fs

    //static member updateISA (isa : ISA.Investigation) (arc : ARC) : ARC =
    //    raise (System.NotImplementedException())

    //static member updateCWL (cwl : CWL.CWL) (arc : ARC) : ARC =
    //    raise (System.NotImplementedException())

    //static member updateFileSystem (fileSystem : FileSystem.FileSystem) (arc : ARC) : ARC =
    //    raise (System.NotImplementedException())

    //static member addFile (path : string) (arc : ARC) : ARC =
    //    FileSystem.addFile |> ignore
    //    ARC.updateFileSystem |> ignore
    //    raise (System.NotImplementedException())

    //static member addFolder (path : string) (arc : ARC) : ARC =
    //    FileSystem.addFolder |> ignore
    //    ARC.updateFileSystem |> ignore
    //    raise (System.NotImplementedException())

    //static member addFolders (paths : string array) (arc : ARC) : ARC =
    //    paths
    //    |> Array.fold (fun arc path -> ARC.addFolder path arc) arc |> ignore
    //    raise (System.NotImplementedException())

    ///// Add folder to ARC.FileSystem and add .gitkeep file to the folder
    //static member addEmptyFolder (path : string) (arc : ARC) : ARC =
    //    FileSystem.addFolder  |> ignore   
    //    FileSystem.addFile (Path.combine path Path.gitKeepFileName) |> ignore
    //    ARC.updateFileSystem |> ignore
    //    raise (System.NotImplementedException())

    //static member addEmptyFolders (paths : string array) (arc : ARC) : ARC =
    //    paths
    //    |> Array.fold (fun arc path -> ARC.addEmptyFolder path arc) arc |> ignore
    //    raise (System.NotImplementedException())


    /// Add assay folder to the ARC.FileSystem and update the ARC.ISA with the new assay metadata
    //static member addAssay (assay : ISA.ArcAssay) (studyIdentifier : string) (arc : ARC) : ARC = 
  
        // - Contracts - //
        // create spreadsheet assays/AssayName/isa.assay.xlsx  
        // create text assays/AssayName/dataset/.gitkeep 
        // create text assays/AssayName/dataset/Readme.md
        // create text assays/AssayName/protocols/.gitkeep 
        // update spreadsheet isa.investigation.xlsx

        // - ISA - //
        //let assayFolderPath = Path.combineAssayFolderPath assay
        //let assaySubFolderPaths = Path.combineAssaySubfolderPaths assay
        //let assayReadmeFilePath = Path.combine assayFolderPath Path.assayReadmeFileName
        //let updatedInvestigation = ArcInvestigation.addAssay assay studyIdentifier arc.ISA
        //arc
        // - FileSystem - //
        //// Create assay root folder in ARC.FileSystem
        //ARC.addFolder assayFolderPath arc 
        //// Create assay subfolders in ARC.FileSystem
        //|> ARC.addEmptyFolders assaySubFolderPaths 
        // Create assay readme file in ARC.FileSystem
        //ARC.addFile assayReadmeFilePath 
        //// Update ARC.ISA with the updated investigation
        //|> ARC.updateISA updatedInvestigation 

    // to-do: we need a function that generates only create contracts from a ARC data model. 
    // reason: contracts are initially designed to sync disk with in-memory model while working on the arc.
    // but we need a way to create an arc programmatically and then write it to disk.

    static member fromFilePaths (filePaths : string array) : ARC = 
        let fs : FileSystem = FileSystem.fromFilePaths filePaths
        ARC(fs=fs)

    //// Maybe add forceReplace flag?
    //member this.SetFSFromFilePaths (filePaths : string array) = 
    //    let newFS : FileSystem = FileSystem.fromFilePaths filePaths
    //    fs <- Some newFS

    // to-do: function that returns read contracts based on a list of paths.
    member this.GetReadContracts () =
        fs.Tree.ToFilePaths() |> Array.choose Contract.ARC.tryISAReadContractFromPath 

    /// <summary>
    /// This function creates the ARC-model from fullfilled READ contracts. The necessary READ contracts can be created with `ARC.getReadContracts`.
    /// </summary>
    /// <param name="cArr">The fullfilled READ contracts.</param>
    /// <param name="enableLogging">If this flag is set true, the function will print any missing/found assays/studies to the console. *Default* = false</param>
    member this.SetISAFromContracts (contracts: Contract [], ?enableLogging: bool) =
        let enableLogging = defaultArg enableLogging false
        /// get investigation from xlsx
        let investigation = ARCAux.getArcInvestigationFromContracts contracts
        /// get studies from xlsx
        let studies = ARCAux.getArcStudiesFromContracts contracts
        /// get assays from xlsx
        let assays = ARCAux.getArcAssaysFromContracts contracts
        /// Necessary, else: System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
        let copy = investigation.Copy()
        copy.StudyIdentifiers |> Seq.iter (fun studyRegisteredIdent ->
            /// Try find registered study in parsed READ contracts
            let studyOpt = studies |> Array.tryFind (fun s -> s.Identifier = studyRegisteredIdent)
            match studyOpt with
            | Some study -> // This study element is parsed from FsWorkbook and has no regsitered assays, yet
                if enableLogging then printfn "Found study: %s" studyRegisteredIdent
                let registeredAssays = copy.GetStudy(studyRegisteredIdent).AssayIdentifiers
                registeredAssays |> Seq.iter (fun assayRegisteredIdent ->
                    /// Try find registered assay in parsed READ contracts
                    let assayOpt = assays |> Array.tryFind (fun a -> a.Identifier = assayRegisteredIdent)
                    match assayOpt with
                    | Some assay -> 
                        if enableLogging then printfn "Found assay: %s - %s" studyRegisteredIdent assayRegisteredIdent 
                        study.AddAssay(assay)
                    | None -> 
                        if enableLogging then printfn "Unable to find registered assay '%s' in fullfilled READ contracts!" assayRegisteredIdent
                )
                investigation.SetStudy(studyRegisteredIdent, study)
            | None -> 
                if enableLogging then printfn "Unable to find registered study '%s' in fullfilled READ contracts!" studyRegisteredIdent
        )
        this.ISA <- Some investigation


    member this.UpdateFileSystem() =   
        let newFS = 
            ARCAux.updateFSByISA isa fs
            |> ARCAux.updateFSByCWL cwl
        fs <- newFS        


    /// <summary>
    /// This function returns the all write Contracts for the current state of the ARC. ISA contracts do contain the object data as spreadsheets, while the other contracts only contain the path.
    /// </summary>  
    member this.GetWriteContracts () =

        /// Map containing the DTOTypes and objects for the ISA objects.
        let workbooks = System.Collections.Generic.Dictionary<string, DTOType*FsWorkbook>()
        match this.ISA with
        | Some inv -> 
            workbooks.Add (Path.InvestigationFileName, (DTOType.ISA_Investigation, ISA.Spreadsheet.ArcInvestigation.toFsWorkbook inv))
            inv.Studies
            |> Seq.iter (fun s ->
                workbooks.Add (
                    Identifier.Study.fileNameFromIdentifier s.Identifier,
                    (DTOType.ISA_Study, ISA.Spreadsheet.ArcStudy.toFsWorkbook s))
                s.Assays
                |> Seq.iter (fun a ->
                    workbooks.Add (
                        Identifier.Assay.fileNameFromIdentifier a.Identifier,
                        (DTOType.ISA_Assay, ISA.Spreadsheet.ArcAssay.toFsWorkbook a))                
                )
            )
        | None -> 
            workbooks.Add (Path.InvestigationFileName, (DTOType.ISA_Investigation, ISA.Spreadsheet.ArcInvestigation.toFsWorkbook (ArcInvestigation.create(Identifier.MISSING_IDENTIFIER))))
            printfn "ARC contains no ISA part."

        // Iterates over filesystem and creates a write contract for every file. If possible, include DTO.       
        fs.Tree.ToFilePaths(true)
        |> Array.map (fun fp ->
            match Dictionary.tryGet fp workbooks with
            | Some (dto,wb) -> Contract.createCreate(fp,dto,DTO.Spreadsheet wb)
            | None -> Contract.createCreate(fp, DTOType.PlainText)
           
        )
        


//-Pseudo code-//
//// Option 1
//let fs, readcontracts = ARC.FSFromFilePaths filepaths
//let isa = ARC.ISAFromContracts fullfilled_readcontracts
//let cwl = ARC.CWLFromXXX XXX
//ARC.create(fs, isa, cwl)

//// Option 2
//let arc = ARC.fromFilePaths // returned ARC
//let contracts = arc.getREADContracts // retunred READ
//arc.updateFromContracts fullfilled_contracts //returned ARC
