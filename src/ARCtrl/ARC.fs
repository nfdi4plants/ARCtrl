namespace rec ARCtrl

open ARCtrl.ValidationPackages
open ARCtrl.FileSystem
open ARCtrl.Contract
open ARCtrl
open ARCtrl.Helper
open ARCtrl.Spreadsheet
open FsSpreadsheet
open Fable.Core
open ARCtrl.ArcPathHelper
open CrossAsync

module ARCAux =

    // No idea where to move this
    let getArcAssaysFromContracts (contracts: Contract []) = 
        contracts 
        |> Array.choose ArcAssay.tryFromReadContract

    // No idea where to move this
    let getArcStudiesFromContracts (contracts: Contract []) =
        contracts 
        |> Array.choose ArcStudy.tryFromReadContract

    let getArcWorkflowsFromContracts (contracts: Contract []) =
        contracts
        |> Array.choose ArcWorkflow.tryFromReadContract

    let getArcRunsFromContracts (contracts : Contract []) =
        contracts
        |> Array.choose ArcRun.tryFromReadContract
        
    let getAssayDataMapFromContracts (assayIdentifier) (contracts: Contract []) = 
        contracts 
        |> Array.tryPick (DataMap.tryFromReadContractForAssay assayIdentifier)

    let getStudyDataMapFromContracts (studyIdentifier) (contracts: Contract []) = 
        contracts 
        |> Array.tryPick (DataMap.tryFromReadContractForStudy studyIdentifier)

    let getWorkflowDataMapFromContracts (workflowIdentifier) (contracts: Contract []) = 
        contracts 
        |> Array.tryPick (DataMap.tryFromReadContractForWorkflow workflowIdentifier)

    let getRunDataMapFromContracts (runIdentifier) (contracts: Contract []) = 
        contracts 
        |> Array.tryPick (DataMap.tryFromReadContractForRun runIdentifier)


    let getArcInvestigationFromContracts (contracts: Contract []) =
        match contracts |> Array.choose ArcInvestigation.tryFromReadContract with
        | [|inv|] -> inv
        | invs -> 
            failwithf "Could not find investigation in contracts. Expected exactly one investigation, but found %d." (invs |> Array.length)

    let getLicenseFromContracts (contracts: Contract []) =
        contracts 
        |> Array.tryPick License.tryFromReadContract

    // Do not cast `ArcInvestigation` to `ARC` or you will encounter recursion issues
    // -> "The initialization of an object or value resulted in an object or value being accessed recursively before it was fully initialized."
    let updateFSByARC (isa : ArcInvestigation) (license: License option) (fs : FileSystem) = 

        let assaysFolder = 
            isa.Assays |> Seq.toArray
            |> Array.map (fun a -> FileSystemTree.createAssayFolder(a.Identifier,a.DataMap.IsSome))
            |> FileSystemTree.createAssaysFolder
        let studiesFolder = 
            isa.Studies |> Seq.toArray
            |> Array.map (fun s -> FileSystemTree.createStudyFolder(s.Identifier,s.DataMap.IsSome))
            |> FileSystemTree.createStudiesFolder
        let workflowsFolder =
            isa.Workflows |> Seq.toArray
            |> Array.map (fun w -> FileSystemTree.createWorkflowFolder(w.Identifier,w.DataMap.IsSome))
            |> FileSystemTree.createWorkflowsFolder
        let runsFolder =
            isa.Runs |> Seq.toArray
            |> Array.map (fun r -> FileSystemTree.createRunFolder(r.Identifier,r.DataMap.IsSome))
            |> FileSystemTree.createRunsFolder
        let investigation = FileSystemTree.createInvestigationFile()
        let tree = 
            FileSystemTree.createRootFolder [|
                investigation;
                assaysFolder;
                studiesFolder;
                workflowsFolder;
                runsFolder
                if license.IsSome then
                    File license.Value.Path
            |]
            |> FileSystem.create
        fs.Union(tree)

    let updateFSByCWL (cwl : unit option) (fs : FileSystem) =       
        let workflows = FileSystemTree.createWorkflowsFolder [||]
        let runs = FileSystemTree.createRunsFolder [||]       
        let tree = 
            FileSystemTree.createRootFolder [|workflows;runs|]
            |> FileSystem.create
        fs.Union(tree)

[<AttachMembers>]
type ARC(identifier : string, ?title : string, ?description : string, ?submissionDate : string, ?publicReleaseDate : string, ?ontologySourceReferences, ?publications, ?contacts, ?assays : ResizeArray<ArcAssay>, ?studies : ResizeArray<ArcStudy>, ?workflows : ResizeArray<ArcWorkflow>, ?runs : ResizeArray<ArcRun>, ?registeredStudyIdentifiers : ResizeArray<string>, ?comments : ResizeArray<Comment>, ?remarks, ?cwl : unit, ?fs : FileSystem.FileSystem, ?license) as this =

    inherit ArcInvestigation(identifier, ?title = title, ?description = description, ?submissionDate = submissionDate, ?publicReleaseDate = publicReleaseDate, ?ontologySourceReferences = ontologySourceReferences, ?publications = publications, ?contacts = contacts, ?assays = assays, ?studies = studies, ?workflows = workflows, ?runs = runs, ?registeredStudyIdentifiers = registeredStudyIdentifiers, ?comments = comments, ?remarks = remarks) 

    let mutable _cwl = cwl
    let mutable _license: License option = license
    let mutable _fs = 
        fs
        |> Option.defaultValue (FileSystem.create(FileSystemTree.Folder ("",[||])))
        |> ARCAux.updateFSByARC this license
        //|> ARCAux.updateFSByCWL cwl

    //member this.CWL 
    //    with get() = cwl

    member this.FileSystem 
        with get() = _fs
        and set(fs) = _fs <- fs

    member this.License
        with get() : License option = _license
        and set(license) = _license <- license

    static member fromArcInvestigation (isa : ArcInvestigation, ?cwl : unit, ?fs : FileSystem, ?license: License) = 
        ARC(identifier = isa.Identifier, ?title = isa.Title, ?description = isa.Description, ?submissionDate = isa.SubmissionDate, ?publicReleaseDate = isa.PublicReleaseDate, ontologySourceReferences = isa.OntologySourceReferences, publications = isa.Publications, contacts = isa.Contacts, assays = isa.Assays, studies = isa.Studies, workflows = isa.Workflows, runs = isa.Runs, registeredStudyIdentifiers = isa.RegisteredStudyIdentifiers, comments = isa.Comments, remarks = isa.Remarks, ?cwl = cwl, ?fs = fs, ?license = license) 

    member this.TryWriteAsync(arcPath) =
        this.GetWriteContracts()
        |> fullFillContractBatchAsync arcPath

    member this.TryUpdateAsync(arcPath) =
        this.GetUpdateContracts()
        |> fullFillContractBatchAsync arcPath

    member this.SetLicenseFulltext (fulltext : string, ?path : string) : unit =
        match this.License with
        | Some license -> 
            license.Type <- LicenseContentType.Fulltext
            license.Content <- fulltext
            if path.IsSome then license.Path <- path.Value
        | None -> 
            let license = License(LicenseContentType.Fulltext, fulltext)
            this.License <- Some license
            if path.IsSome then license.Path <- path.Value

    static member tryLoadAsync (arcPath : string) =
        crossAsync {

            let! paths = FileSystemHelper.getAllFilePathsAsync arcPath
            let arc = ARC.fromFilePaths (paths |> Seq.toArray)

            let contracts = arc.GetReadContracts()
      
            let! fulFilledContracts = 
                contracts 
                |> fullFillContractBatchAsync arcPath

            match fulFilledContracts with
            | Ok c -> 
                arc.SetISAFromContracts(c)
                return Ok arc
            | Error e -> return Error e
        }      

    member this.GetAssayRemoveContracts(assayIdentifier: string) =
        if this.AssayIdentifiers |> Seq.contains assayIdentifier |> not then
            failwith "ARC does not contain assay with given name"
        let assay = this.GetAssay(assayIdentifier)
        let studies = assay.StudiesRegisteredIn
        base.RemoveAssay(assayIdentifier)
        let paths = this.FileSystem.Tree.ToFilePaths()
        let assayFolderPath = getAssayFolderPath(assayIdentifier)
        let filteredPaths = paths |> Array.filter (fun p -> p.StartsWith(assayFolderPath) |> not)
        this.SetFilePaths(filteredPaths)      
        [|
            assay.ToDeleteContract()
            this.ToUpdateContract()
            for s in studies do
                s.ToUpdateContract()
        |]

    member this.TryRemoveAssayAsync(arcPath : string, assayIdentifier: string) =
        this.GetAssayRemoveContracts(assayIdentifier)
        |> fullFillContractBatchAsync arcPath

    member this.GetAssayRenameContracts(oldAssayIdentifier: string, newAssayIdentifier: string) =
        if this.AssayIdentifiers |> Seq.contains oldAssayIdentifier |> not then
            failwith "ARC does not contain assay with given name"

        base.RenameAssay(oldAssayIdentifier,newAssayIdentifier)
        let paths = this.FileSystem.Tree.ToFilePaths()
        let oldAssayFolderPath = getAssayFolderPath(oldAssayIdentifier)
        let newAssayFolderPath = getAssayFolderPath(newAssayIdentifier)
        let renamedPaths = paths |> Array.map (fun p -> p.Replace(oldAssayFolderPath,newAssayFolderPath))
        this.SetFilePaths(renamedPaths)
        [|
            yield Contract.createRename(oldAssayFolderPath,newAssayFolderPath)
            yield! this.GetUpdateContracts()
        |]

    member this.TryRenameAssayAsync(arcPath : string, oldAssayIdentifier: string, newAssayIdentifier: string) =
        this.GetAssayRenameContracts(oldAssayIdentifier,newAssayIdentifier)
        |> fullFillContractBatchAsync arcPath

    member this.GetStudyRemoveContracts(studyIdentifier: string) =
        base.RemoveStudy(studyIdentifier)
        let paths = this.FileSystem.Tree.ToFilePaths()
        let studyFolderPath = getStudyFolderPath(studyIdentifier)
        let filteredPaths = paths |> Array.filter (fun p -> p.StartsWith(studyFolderPath) |> not)
        this.SetFilePaths(filteredPaths)
        [|
            Contract.createDelete(studyFolderPath) // isa.GetStudy(studyIdentifier).ToDeleteContract()
            this.ToUpdateContract()
        |]

    member this.TryRemoveStudyAsync(arcPath : string, studyIdentifier: string) =
        this.GetStudyRemoveContracts(studyIdentifier)
        |> fullFillContractBatchAsync arcPath

    member this.GetStudyRenameContracts(oldStudyIdentifier: string, newStudyIdentifier: string) =
        if this.StudyIdentifiers |> Seq.contains oldStudyIdentifier |> not then
            failwith "ARC does not contain study with given name"
        base.RenameStudy(oldStudyIdentifier,newStudyIdentifier)
        let paths = this.FileSystem.Tree.ToFilePaths()
        let oldStudyFolderPath = getStudyFolderPath(oldStudyIdentifier)
        let newStudyFolderPath = getStudyFolderPath(newStudyIdentifier)
        let renamedPaths = paths |> Array.map (fun p -> p.Replace(oldStudyFolderPath,newStudyFolderPath))
        this.SetFilePaths(renamedPaths)
        [|
            yield Contract.createRename(oldStudyFolderPath,newStudyFolderPath)
            yield! this.GetUpdateContracts()
        |]

    member this.TryRenameStudyAsync(arcPath : string, oldStudyIdentifier: string, newStudyIdentifier: string) =
        this.GetStudyRenameContracts(oldStudyIdentifier,newStudyIdentifier)
        |> fullFillContractBatchAsync arcPath


    member this.WriteAsync(arcPath) =
        crossAsync {
            let! result = this.TryWriteAsync(arcPath)
            match result with
            | Ok _ -> ()
            | Error errors ->
                let appended = errors |> Array.map (fun e -> e.ToString()) |> String.concat "\n"
                failwithf "Could not write ARC, failed with the following errors %s" appended
        }

    member this.UpdateAsync(arcPath) =
        crossAsync {
            let! result = this.TryUpdateAsync(arcPath)
            match result with
            | Ok _ -> ()
            | Error errors ->
                let appended = errors |> Array.map (fun e -> e.ToString()) |> String.concat "\n"
                failwithf "Could not update ARC, failed with the following errors %s" appended
        }

    member this.RemoveAssayAsync(arcPath, assayIdentifier) =
        crossAsync {
            let! result = this.TryRemoveAssayAsync(arcPath, assayIdentifier)
            match result with
            | Ok _ -> ()
            | Error errors ->
                let appended = errors |> Array.map (fun e -> e.ToString()) |> String.concat "\n"
                failwithf "Could not remove assay, failed with the following errors %s" appended
        }

    member this.RenameAssayAsync(arcPath, oldAssayIdentifier, newAssayIdentifier) =
        crossAsync {
            let! result = this.TryRenameAssayAsync(arcPath, oldAssayIdentifier, newAssayIdentifier)
            match result with
            | Ok _ -> ()
            | Error errors ->
                let appended = errors |> Array.map (fun e -> e.ToString()) |> String.concat "\n"
                failwithf "Could not rename assay, failed with the following errors %s" appended
        }

    member this.RemoveStudyAsync(arcPath, studyIdentifier) =
        crossAsync {
            let! result = this.TryRemoveStudyAsync(arcPath, studyIdentifier)
            match result with
            | Ok _ -> ()
            | Error errors ->
                let appended = errors |> Array.map (fun e -> e.ToString()) |> String.concat "\n"
                failwithf "Could not remove study, failed with the following errors %s" appended
        }

    member this.RenameStudyAsync(arcPath, oldStudyIdentifier, newStudyIdentifier) =
        crossAsync {
            let! result = this.TryRenameStudyAsync(arcPath, oldStudyIdentifier, newStudyIdentifier)
            match result with
            | Ok _ -> ()
            | Error errors ->
                let appended = errors |> Array.map (fun e -> e.ToString()) |> String.concat "\n"
                failwithf "Could not rename study, failed with the following errors %s" appended
        }

    static member loadAsync (arcPath) =
        crossAsync {
            let! result = ARC.tryLoadAsync arcPath
            match result with
            | Ok arc -> return arc
            | Error errors ->
                let appended = errors |> Array.map (fun e -> e.ToString()) |> String.concat "\n"
                failwithf "Could not load ARC, failed with the following errors %s" appended
                return (ARC(identifier = Helper.Identifier.createMissingIdentifier()))
        }
    
    #if FABLE_COMPILER_PYTHON || !FABLE_COMPILER
    member this.Write(arcPath) =
        Async.RunSynchronously (this.WriteAsync(arcPath))

    member this.Update(arcPath) =
        Async.RunSynchronously (this.UpdateAsync(arcPath))

    member this.RemoveAssay(arcPath, assayIdentifier) =
        Async.RunSynchronously (this.RemoveAssayAsync(arcPath, assayIdentifier))

    member this.RenameAssay(arcPath, oldAssayIdentifier, newAssayIdentifier) =
        Async.RunSynchronously (this.RenameAssayAsync(arcPath, oldAssayIdentifier, newAssayIdentifier))

    member this.RemoveStudy(arcPath, studyIdentifier) =
        Async.RunSynchronously (this.RemoveStudyAsync(arcPath, studyIdentifier))

    member this.RenameStudy(arcPath, oldStudyIdentifier, newStudyIdentifier) =
        Async.RunSynchronously (this.RenameStudyAsync(arcPath, oldStudyIdentifier, newStudyIdentifier))

    static member load (arcPath) =
        Async.RunSynchronously (ARC.loadAsync arcPath)
    #endif


    member this.MakeDataFilesAbsolute() =
        let filesPaths = this.FileSystem.Tree.ToFilePaths() |> set
        let checkExistenceFromRoot = fun p -> filesPaths |> Set.contains p
        let updateColumnOption (dataNameFunction : Data -> string) (col : CompositeColumn option) =
            match col with
            | Some col when col.Header.IsDataColumn -> 
                col.Cells |> ResizeArray.iter (fun c ->
                    if c.AsData.FilePath.IsSome then               
                        let newFilePath = dataNameFunction c.AsData
                        c.AsData.FilePath <- Some newFilePath
                )
            | _ -> ()
        let updateTable (dataNameFunction : Data -> string) (t : ArcTable) =
            t.TryGetInputColumn() |> updateColumnOption dataNameFunction
            t.TryGetOutputColumn() |> updateColumnOption dataNameFunction
        let updateDataMap (dataNameFunction : Data -> string) (dm : DataMap) =
            dm.DataContexts |> Seq.iter (fun c ->
                if c.FilePath.IsSome then
                    let newFilePath = dataNameFunction c
                    c.FilePath <- Some newFilePath
            )
        
        this.Studies |> Seq.iter (fun s ->
            let f (d : Data) = d.GetAbsolutePathForStudy(s.Identifier,checkExistenceFromRoot)
            s.Tables |> Seq.iter (updateTable f)
            if s.DataMap.IsSome then
                updateDataMap f s.DataMap.Value
        )
        this.Assays |> Seq.iter (fun a ->
            let f (d : Data) = d.GetAbsolutePathForAssay(a.Identifier,checkExistenceFromRoot)
            a.Tables |> Seq.iter (updateTable f)
            if a.DataMap.IsSome then
                updateDataMap f a.DataMap.Value
        )


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
        ARC(identifier = Identifier.createMissingIdentifier(), fs=fs)

    member this.SetFilePaths (filePaths : string array) =
        let tree = FileSystemTree.fromFilePaths filePaths
        _fs <- {_fs with Tree = tree}

    //// Maybe add forceReplace flag?
    //member this.SetFSFromFilePaths (filePaths : string array) = 
    //    let newFS : FileSystem = FileSystem.fromFilePaths filePaths
    //    fs <- Some newFS

    // to-do: function that returns read contracts based on a list of paths.
    member this.GetReadContracts () : Contract [] =
        _fs.Tree.ToFilePaths()
        |> Array.choose Contract.ARC.tryISAReadContractFromPath 

    /// <summary>
    /// This function creates the ARC-model from fullfilled READ contracts. The necessary READ contracts can be created with `ARC.getReadContracts`.
    /// </summary>
    /// <param name="cArr">The fullfilled READ contracts.</param>
    /// <param name="enableLogging">If this flag is set true, the function will print any missing/found assays/studies to the console. *Default* = false</param>
    member this.SetISAFromContracts (contracts: Contract []) =
        /// get investigation from xlsx
        let investigation = ARCAux.getArcInvestigationFromContracts contracts
        IdentifierSetters.setInvestigationIdentifier investigation.Identifier this |> ignore
        this.Title <- investigation.Title
        this.Description <- investigation.Description
        this.SubmissionDate <- investigation.SubmissionDate
        this.PublicReleaseDate <- investigation.PublicReleaseDate
        this.OntologySourceReferences <- investigation.OntologySourceReferences
        this.Publications <- investigation.Publications
        this.Contacts <- investigation.Contacts
        this.Comments <- investigation.Comments
        this.Remarks <- investigation.Remarks
        this.RegisteredStudyIdentifiers <- investigation.RegisteredStudyIdentifiers

        /// get studies from xlsx
        let studies = ARCAux.getArcStudiesFromContracts contracts |> Array.map fst
        /// get assays from xlsx
        let assays = ARCAux.getArcAssaysFromContracts contracts
        /// get workflows from xlsx
        let workflows = ARCAux.getArcWorkflowsFromContracts contracts
        /// get runs from xlsx
        let runs = ARCAux.getArcRunsFromContracts contracts

        let license = ARCAux.getLicenseFromContracts contracts 

        // Remove Assay metadata objects read from investigation file from investigation object, if no assosiated assay file exists
        this.AssayIdentifiers
        |> Array.iter (fun ai -> 
            if assays |> Array.exists (fun a -> a.Identifier = ai) |> not then
                this.DeleteAssay(ai)      
        )

        // Remove Study metadata objects read from investigation file from investigation object, if no assosiated study file exists
        this.StudyIdentifiers
        |> Array.iter (fun si -> 
            if studies |> Array.exists (fun s -> s.Identifier = si) |> not then
                this.DeleteStudy(si)
        )

        studies |> Array.iter (fun study ->            
            /// Try find registered study in parsed READ contracts
            let registeredStudyOpt = this.Studies |> Seq.tryFind (fun s -> s.Identifier = study.Identifier)
            match registeredStudyOpt with
            | Some registeredStudy -> 
                registeredStudy.UpdateReferenceByStudyFile(study,true)
            | None -> 
                this.AddStudy(study)
            let datamap = ARCAux.getStudyDataMapFromContracts study.Identifier contracts
            if study.DataMap.IsNone then
                study.DataMap <- datamap
            study.StaticHash <- study.GetLightHashCode()
        )
        assays |> Array.iter (fun assay ->
            /// Try find registered study in parsed READ contracts
            let registeredAssayOpt = this.Assays |> Seq.tryFind (fun a -> a.Identifier = assay.Identifier)
            match registeredAssayOpt with
            | Some registeredAssay -> // This study element is parsed from FsWorkbook and has no regsitered assays, yet
                registeredAssay.UpdateReferenceByAssayFile(assay,true)
            | None -> 
                this.AddAssay(assay)
            let assay = this.Assays |> Seq.find (fun a -> a.Identifier = assay.Identifier)
            let updatedTables = 
                assay.StudiesRegisteredIn
                |> Array.fold (fun tables study -> 
                    ArcTables.updateReferenceTablesBySheets(ArcTables(study.Tables),tables,false)
                ) (ArcTables(assay.Tables))
            let datamap = ARCAux.getAssayDataMapFromContracts assay.Identifier contracts
            if assay.DataMap.IsNone then
                assay.DataMap <- datamap
            assay.Tables <- updatedTables.Tables
        )
        workflows |> Array.iter (fun workflow ->
            let datamap = ARCAux.getWorkflowDataMapFromContracts workflow.Identifier contracts
            if workflow.DataMap.IsNone then
                workflow.DataMap <- datamap
            this.AddWorkflow(workflow)
            workflow.StaticHash <- workflow.GetLightHashCode()
        )
        runs |> Array.iter (fun run ->
            let datamap = ARCAux.getRunDataMapFromContracts run.Identifier contracts
            if run.DataMap.IsNone then
                run.DataMap <- datamap
            this.AddRun(run)
            run.StaticHash <- run.GetLightHashCode()
        )
        this.Assays |> Seq.iter (fun a -> a.StaticHash <- a.GetLightHashCode())
        this.Studies |> Seq.iter (fun s -> s.StaticHash <- s.GetLightHashCode())
        this.License <- license 
        this.StaticHash <- this.GetLightHashCode()

    member this.UpdateFileSystem() =   
        let newFS = 
            ARCAux.updateFSByARC this _license _fs
            //|> ARCAux.updateFSByCWL _cwl
        _fs <- newFS        

    /// <summary>
    /// This function returns the all write Contracts for the current state of the ARC. ISA contracts do contain the object data as spreadsheets, while the other contracts only contain the path.
    ///
    /// If datamapFile is set to true, a write contract for the datamap file will be included for each study and assay.
    /// </summary>
    /// <param name="datamapFile">Default: false.</param>
    member this.GetWriteContracts (?skipUpdateFS : bool) =
        if not (defaultArg skipUpdateFS false) then
            this.UpdateFileSystem()
        //let datamapFile = defaultArg datamapFile false
        /// Map containing the fileName and the types for DTOTypes and objects.
        let filemap = System.Collections.Generic.Dictionary<string, DTOType*DTO>()
        
        let investigationConverter = ArcInvestigation.toFsWorkbook
        filemap.Add (InvestigationFileName, (DTOType.ISA_Investigation, investigationConverter this |> box |> DTO.Spreadsheet))
        this.StaticHash <- this.GetLightHashCode()
        this.Studies
        |> Seq.iter (fun s ->
            s.StaticHash <- s.GetLightHashCode()
            filemap.Add (
                Identifier.Study.fileNameFromIdentifier s.Identifier,
                (DTOType.ISA_Study, ArcStudy.toFsWorkbook s |> box |> DTO.Spreadsheet)
            )
            if s.DataMap.IsSome (*&& datamapFile*) then 
                let dm = s.DataMap.Value
                dm.StaticHash <- dm.GetHashCode()
                filemap.Add (
                    Identifier.Study.datamapFileNameFromIdentifier s.Identifier,
                    (DTOType.ISA_Datamap, Spreadsheet.DataMap.toFsWorkbook dm |> box |> DTO.Spreadsheet)
                )
                
        )
        this.Assays
        |> Seq.iter (fun a ->
            a.StaticHash <- a.GetLightHashCode()
            filemap.Add (
                Identifier.Assay.fileNameFromIdentifier a.Identifier,
                (DTOType.ISA_Assay, ArcAssay.toFsWorkbook a |> box |> DTO.Spreadsheet))     
            if a.DataMap.IsSome (*&& datamapFile*) then 
                let dm = a.DataMap.Value
                dm.StaticHash <- dm.GetHashCode()
                filemap.Add (
                    Identifier.Assay.datamapFileNameFromIdentifier a.Identifier,
                    (DTOType.ISA_Datamap, Spreadsheet.DataMap.toFsWorkbook dm |> box |> DTO.Spreadsheet)
                )
        )
        this.Workflows
        |> Seq.iter (fun w ->
            w.StaticHash <- w.GetLightHashCode()
            filemap.Add (
                Identifier.Workflow.fileNameFromIdentifier w.Identifier,
                (DTOType.ISA_Workflow, ArcWorkflow.toFsWorkbook w |> box |> DTO.Spreadsheet)
            )
            if w.DataMap.IsSome (*&& datamapFile*) then 
                let dm = w.DataMap.Value
                dm.StaticHash <- dm.GetHashCode()
                filemap.Add (
                    Identifier.Workflow.datamapFileNameFromIdentifier w.Identifier,
                    (DTOType.ISA_Datamap, Spreadsheet.DataMap.toFsWorkbook dm |> box |> DTO.Spreadsheet)
                )
        )
        this.Runs
        |> Seq.iter (fun r ->
            r.StaticHash <- r.GetLightHashCode()
            filemap.Add (
                Identifier.Run.fileNameFromIdentifier r.Identifier,
                (DTOType.ISA_Run, ArcRun.toFsWorkbook r |> box |> DTO.Spreadsheet)
            )
            if r.DataMap.IsSome (*&& datamapFile*) then 
                let dm = r.DataMap.Value
                dm.StaticHash <- dm.GetHashCode()
                filemap.Add (
                    Identifier.Run.datamapFileNameFromIdentifier r.Identifier,
                    (DTOType.ISA_Datamap, Spreadsheet.DataMap.toFsWorkbook dm |> box |> DTO.Spreadsheet)
                )
        )

        match this.License with
        | Some l ->
            match l.Type with
            | LicenseContentType.Fulltext ->
                l.StaticHash <- l.GetHashCode()
                filemap.Add (l.Path, (DTOType.PlainText, DTO.Text l.Content))
        | None ->
            ()

        // Iterates over filesystem and creates a write contract for every file. If possible, include DTO.
        // "No idea why we do this, i think this is done to also write .gitkeep files and folders? Then this should be part of getWriteContracts on the class objects.
        // Will keep it for now but this is not DRY" ~Kevin Frey
        _fs.Tree.ToFilePaths(true)
        |> Array.map (fun fp ->
            match Dictionary.tryGet fp filemap with
            | Some (dto,wb) -> Contract.createCreate(fp,dto,wb)
            | None -> Contract.createCreate(fp, DTOType.PlainText)
        )
    /// <summary>
    /// This function returns the all update Contracts for the current state of the ARC. Only update contracts for those ISA objects that have been changed will be returned.
    /// If an ISA object was added to the ARC, instead a write contract for the complete object folder will be returned. 
    /// 
    /// An obect is considered changed if its hash code has changed compared with the StaticHash. An object is considered added if its StaticHash code is 0.
    /// 
    /// ISA contracts do contain the object data as spreadsheets, while the other contracts only contain the path.
    /// </summary>  
    member this.GetUpdateContracts (?skipUpdateFS : bool) =
        // Map containing the DTOTypes and objects for the ISA objects.
        if this.StaticHash = 0 then
            this.StaticHash <- this.GetLightHashCode()
            this.GetWriteContracts(?skipUpdateFS = skipUpdateFS) // <- why this?
        else
            [|
                // Get Investigation contract
                let hash = this.GetLightHashCode()
                // Currently catched by match case
                //if this.StaticHash = 0 then       
                //    yield this.ToCreateContract(isLight)
                if this.StaticHash <> hash then 
                    yield this.ToUpdateContract()
                this.StaticHash <- hash

                // Get Study contracts
                for s in this.Studies do
                    let hash = s.GetLightHashCode()
                    if s.StaticHash = 0 then
                        yield! s.ToCreateContract(WithFolder = true)
                    elif s.StaticHash <> hash then 
                        yield s.ToUpdateContract()
                    s.StaticHash <- hash

                    match s.DataMap with
                    | Some dm when dm.StaticHash = 0 -> 
                        yield dm.ToCreateContractForStudy(s.Identifier)
                        dm.StaticHash <- dm.GetHashCode()
                    | Some dm when dm.StaticHash <> dm.GetHashCode() ->
                        yield dm.ToUpdateContractForStudy(s.Identifier)
                        dm.StaticHash <- dm.GetHashCode()
                    | _ -> ()
                
                // Get Assay contracts
                for a in this.Assays do
                    let hash = a.GetLightHashCode()
                    if a.StaticHash = 0 then 
                        yield! a.ToCreateContract(WithFolder = true)
                    elif a.StaticHash <> hash then 
                        yield a.ToUpdateContract()
                    a.StaticHash <- hash

                    match a.DataMap with
                    | Some dm when dm.StaticHash = 0 -> 
                        yield dm.ToCreateContractForAssay(a.Identifier)
                        dm.StaticHash <- dm.GetHashCode()
                    | Some dm when dm.StaticHash <> dm.GetHashCode() ->
                        yield dm.ToUpdateContractForAssay(a.Identifier)
                        dm.StaticHash <- dm.GetHashCode()
                    | _ -> ()

                for w in this.Workflows do
                    let hash = w.GetLightHashCode()
                    if w.StaticHash = 0 then 
                        yield! w.ToCreateContract(WithFolder = true)
                    elif w.StaticHash <> hash then 
                        yield w.ToUpdateContract()
                    w.StaticHash <- hash
                    match w.DataMap with
                    | Some dm when dm.StaticHash = 0 -> 
                        yield dm.ToCreateContractForWorkflow(w.Identifier)
                        dm.StaticHash <- dm.GetHashCode()
                    | Some dm when dm.StaticHash <> dm.GetHashCode() ->
                        yield dm.ToUpdateContractForWorkflow(w.Identifier)
                        dm.StaticHash <- dm.GetHashCode()
                    | _ -> ()

                for r in this.Runs do
                    let hash = r.GetLightHashCode()
                    if r.StaticHash = 0 then 
                        yield! r.ToCreateContract(WithFolder = true)
                    elif r.StaticHash <> hash then 
                        yield r.ToUpdateContract()
                    r.StaticHash <- hash
                    match r.DataMap with
                    | Some dm when dm.StaticHash = 0 -> 
                        yield dm.ToCreateContractForRun(r.Identifier)
                        dm.StaticHash <- dm.GetHashCode()
                    | Some dm when dm.StaticHash <> dm.GetHashCode() ->
                        yield dm.ToUpdateContractForRun(r.Identifier)
                        dm.StaticHash <- dm.GetHashCode()
                    | _ -> ()

                match this.License with
                | Some l ->
                    let hash = l.GetHashCode()
                    if l.StaticHash = 0 then
                        yield l.ToCreateContract()
                    elif l.StaticHash <> hash then
                        yield l.ToUpdateContract()
                    l.StaticHash <- hash
                | None ->
                    ()
            |]           

    member this.GetGitInitContracts(?branch : string,?repositoryAddress : string,?defaultGitignore : bool, ?defaultGitattributes) = 
        let defaultGitignore = defaultArg defaultGitignore false
        let defaultGitattributes = defaultArg defaultGitattributes false
        [|
            Contract.Git.Init.createInitContract(?branch = branch)
            if defaultGitignore then Contract.Git.gitignoreContract
            if defaultGitattributes then Contract.Git.gitattributesContract
            if repositoryAddress.IsSome then Contract.Git.Init.createAddRemoteContract repositoryAddress.Value
        |]

    static member getCloneContract(remoteUrl : string,?merge : bool ,?branch : string,?token : string*string,?nolfs : bool) =
        Contract.Git.Clone.createCloneContract(remoteUrl,?merge = merge,?branch = branch,?token = token,?nolfs = nolfs)

    member this.Copy() =
        let nextAssays = ResizeArray()
        let nextStudies = ResizeArray()
        let nextWorkflows = ResizeArray()
        let nextRuns = ResizeArray()
        for assay in this.Assays do
            let copy = assay.Copy()
            nextAssays.Add(copy)
        for study in this.Studies do
            let copy = study.Copy()
            nextStudies.Add(copy)
        for workflow in this.Workflows do
            let copy = workflow.Copy()
            nextWorkflows.Add(copy)
        for run in this.Runs do
            let copy = run.Copy()
            nextRuns.Add(copy)
        let nextComments = this.Comments |> ResizeArray.map (fun c -> c.Copy())
        let nextRemarks = this.Remarks |> ResizeArray.map (fun c -> c.Copy())
        let nextContacts = this.Contacts |> ResizeArray.map (fun c -> c.Copy())
        let nextPublications = this.Publications |> ResizeArray.map (fun c -> c.Copy())
        let nextOntologySourceReferences = this.OntologySourceReferences |> ResizeArray.map (fun c -> c.Copy())
        let nextStudyIdentifiers = ResizeArray(this.RegisteredStudyIdentifiers)
        let nextLicense = this.License |> Option.map _.Copy()
        let fsCopy = _fs.Copy()
        ARC(
            this.Identifier,
            ?title = this.Title,
            ?description = this.Description,
            ?submissionDate = this.SubmissionDate,
            ?publicReleaseDate = this.PublicReleaseDate,
            studies = nextStudies,
            assays = nextAssays,
            workflows = nextWorkflows,
            runs = nextRuns,
            registeredStudyIdentifiers = nextStudyIdentifiers,
            ontologySourceReferences = nextOntologySourceReferences,
            publications = nextPublications,
            contacts = nextContacts,
            comments = nextComments,
            remarks = nextRemarks,
            ?cwl = _cwl,
            ?license = nextLicense,
            fs = fsCopy
        )

    /// <summary>
    /// Returns the FileSystemTree of the ARC with only the registered files and folders included.
    /// </summary>
    /// <param name="IgnoreHidden">Wether or not to ignore hidden files and folders starting with '.'. If true, no hidden files are included in the result. (default: true)</param>
    member this.GetRegisteredPayload(?IgnoreHidden:bool) =

        let copy = this.Copy()

        let registeredStudies =     
            copy.Studies.ToArray()
        
        let registeredAssays =
            registeredStudies
            |> Array.map (fun s -> s.RegisteredAssays.ToArray()) // to-do: s.RegisteredAssays
            |> Array.concat

        let includeRootFiles : Set<string> = 
            set [
                InvestigationFileName
                READMEFileName
                LICENSEFileName
            ]

        let includeStudyFiles = 
            registeredStudies
            |> Array.map (fun s -> 
                let studyFoldername = $"{StudiesFolderName}/{s.Identifier}"

                set [
                    yield $"{studyFoldername}/{StudyFileName}"
                    yield $"{studyFoldername}/{READMEFileName}"

                    //just allow any constructed path from cell values. there may be occasions where this includes wrong files, but its good enough for now.
                    for table in s.Tables do
                        for kv in table.Values do
                            let textValue = kv.Value.ToFreeTextCell().AsFreeText
                            yield textValue // from arc root
                            yield $"{studyFoldername}/{StudiesResourcesFolderName}/{textValue}" // from study root > resources
                            yield $"{studyFoldername}/{StudiesProtocolsFolderName}/{textValue}" // from study root > protocols
                ]
            )
            |> Set.unionMany

        let includeAssayFiles = 
            registeredAssays
            |> Array.map (fun a -> 
                let assayFoldername = $"{AssaysFolderName}/{a.Identifier}"

                set [
                    yield $"{assayFoldername}/{AssayFileName}"
                    yield $"{assayFoldername}/{READMEFileName}"

                    //just allow any constructed path from cell values. there may be occasions where this includes wrong files, but its good enough for now.
                    for table in a.Tables do
                        for kv in table.Values do
                            let textValue = kv.Value.ToFreeTextCell().AsFreeText
                            yield textValue // from arc root
                            yield $"{assayFoldername}/{AssayDatasetFolderName}/{textValue}" // from assay root > dataset
                            yield $"{assayFoldername}/{AssayProtocolsFolderName}/{textValue}" // from assay root > protocols
                ]
            )
            |> Set.unionMany

        let includeFiles = Set.unionMany [includeRootFiles; includeStudyFiles; includeAssayFiles]

        let ignoreHidden = defaultArg IgnoreHidden true
        let fsCopy = _fs.Copy() // not sure if needed, but let's be safe

        fsCopy.Tree
        |> FileSystemTree.toFilePaths()
        |> Array.filter (fun p -> 
            p.StartsWith(WorkflowsFolderName) 
            || p.StartsWith(RunsFolderName) 
            || includeFiles.Contains(p)
        )
        |> FileSystemTree.fromFilePaths
        |> fun tree -> if ignoreHidden then tree |> FileSystemTree.filterFiles (fun n -> not (n.StartsWith("."))) else Some tree
        |> Option.bind (fun tree -> if ignoreHidden then tree |> FileSystemTree.filterFolders (fun n -> not (n.StartsWith("."))) else Some tree)
        |> Option.defaultValue (FileSystemTree.fromFilePaths [||])

    /// <summary>
    /// Returns the FileSystemTree of the ARC with only and folders included that are considered additional payload.
    /// </summary>
    /// <param name="IgnoreHidden">Wether or not to ignore hidden files and folders starting with '.'. If true, no hidden files are included in the result. (default: true)</param>
    member this.GetAdditionalPayload(?IgnoreHidden:bool) =
        let ignoreHidden = defaultArg IgnoreHidden true
        let registeredPayload = 
            this.GetRegisteredPayload()
            |> FileSystemTree.toFilePaths()
            |> set

        _fs.Copy().Tree
        |> FileSystemTree.toFilePaths()
        |> Array.filter (fun p -> not (registeredPayload.Contains(p)))
        |> FileSystemTree.fromFilePaths
        |> fun tree -> if ignoreHidden then tree |> FileSystemTree.filterFiles (fun n -> not (n.StartsWith("."))) else Some tree
        |> Option.bind (fun tree -> if ignoreHidden then tree |> FileSystemTree.filterFolders (fun n -> not (n.StartsWith("."))) else Some tree)
        |> Option.defaultValue (FileSystemTree.fromFilePaths [||])
        
    static member DefaultContracts = Map<string,Contract> [|
        ARCtrl.Contract.Git.gitignoreFileName, ARCtrl.Contract.Git.gitignoreContract
        ARCtrl.Contract.Git.gitattributesFileName, ARCtrl.Contract.Git.gitattributesContract
        ARCtrl.ArcPathHelper.LICENSEFileName, ARCtrl.Contract.License.deaultLicenseContract // not sure if we want this?
    |]

    static member fromDeprecatedROCrateJsonString (s:string) =
        try
            let s = s.Replace("bio:additionalProperty","sdo:additionalProperty")
            let isa = ARCtrl.Json.Decode.fromJsonString ARCtrl.Json.ARC.ROCrate.decoderDeprecated s
            ARC.fromArcInvestigation(isa = isa)
        with
        | ex -> 
            failwithf "Could not parse deprecated ARC-RO-Crate metadata: \n%s" ex.Message

    static member fromROCrateJsonString (s:string) =
        try 
            let isa, files, license = ARCtrl.Json.Decode.fromJsonString ARCtrl.Json.ARC.ROCrate.decoder s
            let fileSystem = Array.ofSeq files |> FileSystem.fromFilePaths

            ARC.fromArcInvestigation(isa = isa, fs = fileSystem, ?license = license)
        with
        | ex -> 
            failwithf "Could not parse ARC-RO-Crate metadata: \n%s" ex.Message

    member this.ToROCrateJsonString(?spaces) =
        this.MakeDataFilesAbsolute()
        ARCtrl.Json.ARC.ROCrate.encoder(this, ?license = _license, fs = _fs)
        |> ARCtrl.Json.Encode.toJsonString (ARCtrl.Json.Encode.defaultSpaces spaces)

        /// exports in json-ld format
    static member toROCrateJsonString(?spaces) =
        fun (obj:ARC) ->
            obj.ToROCrateJsonString(?spaces = spaces)

    member this.GetLicenseWriteContract() =
        this.License |> Option.defaultWith License.GetDefaultLicense |> _.ToCreateContract()

    /// <summary>
    /// Returns the write contract for the input ValidationPackagesConfig object.
    ///
    /// This will mutate the ARC file system to include the `.arc/validation_packages.yml` file if it is not already included.
    /// </summary>
    /// <param name="vpc">the config to write</param>
    member this.GetValidationPackagesConfigWriteContract(vpc: ValidationPackagesConfig) =
        let paths = this.FileSystem.Tree.ToFilePaths()
        if not (Array.contains ValidationPackagesConfigHelper.ConfigFilePath paths) then
            Array.append [|ValidationPackagesConfigHelper.ConfigFilePath|] paths
            |> this.SetFilePaths
        ValidationPackagesConfig.toCreateContract vpc

    /// <summary>
    /// Returns the delete contract for `.arc/validation_packages.yml`
    ///
    /// If the ARC file system includes the `.arc/validation_packages.yml` file, it is removed.
    /// </summary>
    /// <param name="vpc"></param>
    member this.GetValidationPackagesConfigDeleteContract(vpc) =
        let paths = this.FileSystem.Tree.ToFilePaths()
        if (Array.contains ValidationPackagesConfigHelper.ConfigFilePath paths) then
            paths
            |> Array.filter (fun p -> not (p = ValidationPackagesConfigHelper.ConfigFilePath))
            |> this.SetFilePaths
        ValidationPackagesConfig.toDeleteContract vpc

    /// <summary>
    /// Returns the read contract for `.arc/validation_packages.yml`
    /// </summary>
    member this.GetValidationPackagesConfigReadContract() =
        ValidationPackagesConfigHelper.ReadContract

    /// <summary>
    /// Returns the ValidationPackagesConfig object from the given read contract if possible, otherwise returns None/null.
    /// </summary>
    /// <param name="contract">the input contract that contains the config in it's DTO</param>
    member this.GetValidationPackagesConfigFromReadContract(contract) =
        ValidationPackagesConfig.tryFromReadContract contract


    member this.ToFilePaths(?removeRoot: bool, ?skipUpdateFS : bool) =
        if not (defaultArg skipUpdateFS false) then
            this.UpdateFileSystem()
        this.FileSystem.Tree.ToFilePaths(?removeRoot = removeRoot)

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
