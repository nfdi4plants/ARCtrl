namespace ARCtrl

open FileSystem
open Contract
open ISA

open Fable.Core

[<AttachMembers>]
type ARC =
    {
       ISA : ISA.Investigation
       CWL : CWL.CWL
       FileSystem : FileSystem.FileSystem 
    }    

    static member create(
        isa: ISA.Investigation,
        cwl: CWL.CWL,
        fs: FileSystem.FileSystem 
    ) =
        {
           ISA = isa
           CWL = cwl
           FileSystem = fs
        }  

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

    // to-do: function that returns read contracts based on a list of paths.
    // the list of paths is used to create a filesystem tree
    static member createReadContracts (filePaths : string array) : Contract array =
        let xlsxReadContractFromPath (path: string) = Contract.createRead(path, DTOType.Spreadsheet)
        let fs = FileSystem.fromFilePaths filePaths
        let xlsxFileNames = [|Path.AssayFileName; Path.StudyFileName; Path.InvestigationFileName|]
        let xlsxFiles = fs.Tree.Filter(fun p -> xlsxFileNames |> Array.contains p)
        match xlsxFiles with
        | Some xlsxPaths -> xlsxPaths.ToFilePaths() |> Array.map xlsxReadContractFromPath
        | None -> [||]

    [<NamedParams(fromIndex=1)>]
    static member createFromReadContracts (cArr: Contract [], ?enableLogging: bool) =
        let enableLogging = defaultArg enableLogging false
        /// filter to only keep *fullfilled* READ contracts of type spreadsheet.
        let filteredContracts = cArr |> Array.choose (fun c ->
            match c with
            | {Operation = READ; DTOType = Some DTOType.Spreadsheet; DTO = Some (DTO.Spreadsheet fsworkbook); Path = p} ->
                Some (p, fsworkbook)
            | _ -> None
        )
        /// get investigation from xlsx
        let investigation = 
            filteredContracts 
            |> Array.find (fun c -> fst c |> FileSystem.Path.isFile Path.InvestigationFileName)
            |> snd 
            |> ISA.Spreadsheet.ArcInvestigation.fromFsWorkbook
        /// get studies from xlsx
        let studies =
            filteredContracts 
            |> Array.filter (fun c -> fst c |> FileSystem.Path.isFile Path.StudyFileName)
            |> Array.map (snd >> ISA.Spreadsheet.ArcStudy.fromFsWorkbook)
        /// get assays from xlsx
        let assays =
            filteredContracts 
            |> Array.filter (fun c -> fst c |> FileSystem.Path.isFile Path.AssayFileName)
            |> Array.map (snd >> ISA.Spreadsheet.ArcAssay.fromFsWorkbook)
        /// Create a investigation copy to check for registered studies/assays
        let copy = investigation.Copy()
        /// Get all registered studies
        let registeredStudies = copy.StudyIdentifiers
        registeredStudies |> Seq.iter (fun studyRegisteredIdent ->
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
        investigation