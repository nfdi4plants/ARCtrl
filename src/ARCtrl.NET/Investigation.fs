namespace ARCtrl.NET

//open ISADotNet
//open ISADotNet.XLSX
//open System.IO

//module Investigation =

//    let investigationFileName = "isa.investigation.xlsx"

//    /// Creates an investigation file in the ARC.
//    let write (arcDir : string) (investigation : ISADotNet.Investigation) =
           
//        let log = Logging.createLogger "InvestigationWriteLog"
        
//        log.Info("Start Investigation Write")

//        if System.IO.File.Exists(Path.Combine(arcDir,investigationFileName)) then
//            log.Error("Investigation file does already exist.")

//        else 
//            let investigationFilePath = Path.Combine(arcDir,investigationFileName)    
//            Investigation.toFile investigationFilePath investigation

//    /// Creates an investigation file in the ARC.
//    let overWrite (arcDir : string) (investigation : ISADotNet.Investigation) =
           
//        let log = Logging.createLogger "InvestigationWriteLog"
//        log.Info("Start Investigation Write")

//        let investigationFilePath = Path.Combine(arcDir,investigationFileName)

//        if System.IO.File.Exists(investigationFilePath) then
//            try 
//                let cache = File.ReadAllBytes(investigationFilePath)
//                File.Delete(investigationFilePath)
//                try                     
//                    Investigation.toFile investigationFilePath investigation
//                with
//                | err -> 
//                    File.WriteAllBytes(investigationFilePath,cache)
//                    log.Error($"Investigation file could not be overwritten: {err.Message}")
//            with 
//            | err -> 
//                log.Error($"Investigation file could not be overwritten: {err.Message}")
//        else 
//            Investigation.toFile investigationFilePath investigation

//    /// Reads an investigation from the ARC.
//    let read (arcDir : string) =
           
//        let log = Logging.createLogger "InvestigationReadLog"
        
//        log.Info("Start Investigation Read")

//        if System.IO.File.Exists(Path.Combine(arcDir,investigationFileName)) then
//            let investigationFilePath = Path.Combine(arcDir,investigationFileName)    
//            Investigation.fromFile investigationFilePath

//        else 
//            log.Error("Investigation file does not exist.")
//            raise (System.SystemException())

//    /// Reads and combines all ISA components of the ARC into the ISA object
//    let fromArcFolder (arcDir : string) =
//        let log = Logging.createLogger "InvestigationFromArcFolderLog"

//        // read investigation from investigation file
//        let ip = Path.Combine(arcDir,investigationFileName).Replace(@"\","/")
//        let i = Investigation.fromFile ip

//        // get study list from study files and assay files
//        let istudies = 
//            i.Studies
//            |> Option.map (List.map (fun study -> 
//                //// read study from file
//                match study.Identifier with
//                | Some id ->
//                    let studyFromFile = Study.readByIdentifier arcDir id
//                    let mergedStudy = API.Update.UpdateByExistingAppendLists.updateRecordType study studyFromFile
//                    // update study assays and contacts with information from assay files
//                    match study.Assays with
//                    | Some assays ->
//                        let scontacts,sassays = 
//                            assays
//                            |> List.fold (fun (cl,al) assay ->
//                                match assay.FileName with
//                                | Some fn ->
//                                    let contactsFromFile,assayFromFile = Assay.readByFileName arcDir assay.FileName.Value
//                                    cl @ contactsFromFile, al @ [assayFromFile]
//                                | None ->
//                                    log.Warn("Study \'" + id + "\' contains Assay without filename.")
//                                    cl, al @ [assay]
//                            ) (mergedStudy.Contacts |> Option.defaultValue [],[])
//                        {mergedStudy with
//                            Contacts = Some (scontacts |> List.distinct)
//                            Assays = Some sassays 
//                        }
//                    | None -> 
//                        mergedStudy
//                | None ->
//                    log.Warn("Investigation file contains study without identifier.")
//                    study
//            ))
        
//        // construct complete process list from studies and assays, then update by itself
//        let iprocesses = 
//            istudies
//            |> Option.map (List.fold (fun pl study ->
//                let sprocesses = study.ProcessSequence |> Option.defaultValue []
//                let aprocesses =
//                    study.Assays
//                    |> Option.map (List.fold (fun spl assay ->
//                        let ap = assay.ProcessSequence |> Option.defaultValue []
//                        spl @ ap
//                    ) [] )
//                    |> Option.defaultValue []
//                pl @ sprocesses @ aprocesses
//            ) [] )
//            |> Option.defaultValue []
//        let ref = iprocesses |> ProcessSequence.updateByItself

//        // update investigation processes
//        let istudies' =
//            istudies
//            |> Option.map (List.map (fun study ->
//                {study with
//                    Assays = study.Assays |> Option.map (List.map (fun a -> {a with ProcessSequence = a.ProcessSequence |> Option.map (ProcessSequence.updateByRef ref)}))
//                    ProcessSequence = study.ProcessSequence |> Option.map (ProcessSequence.updateByRef ref)
//                }
//            ))

//        // fill investigation with information from study files and assay files
//        {i with Studies = istudies'}
//        |> API.Investigation.update

//    /// Registers an assay to the investigation arc registry
//    let registerAssay arcDir studyName (assayName) =

//        let log = Logging.createLogger "RegisterAssayLog"

//        let _, assay = Assay.readByName arcDir assayName

//        let investigation = read arcDir

//        match investigation.Studies with
//        | Some studies -> 
//            match API.Study.tryGetByIdentifier studyName studies with
//            | Some study -> 
//                match study.Assays with
//                | Some assays -> 
//                    match Assay.tryReadByName arcDir assayName with
//                    | Some _ ->
//                        log.Error($"Assay with the identifier {assayName} already exists in the investigation file.")
//                        assays
//                    | None ->
//                        API.Assay.add assays assay
//                | None ->
//                    [assay]
//                |> API.Study.setAssays study
//                |> fun s -> API.Study.updateByIdentifier API.Update.UpdateAll s studies
//            | None ->
//                log.Info($"Study with the identifier {studyName} does not exist yet, creating it now.")
//                let study = Study.create(Identifier = studyName, Assays = [assay])
//                Study.init arcDir study
//                API.Study.add studies study
//        | None ->
//            log.Info($"Study with the identifier {studyName} does not exist yet, creating it now.")
//            let study = Study.create(Identifier = studyName, Assays = [assay])
//            Study.init arcDir study
//            [study]
//        |> API.Investigation.setStudies investigation
//        |> write arcDir

//    /// Update investigation file with information from the different ISA components of the ARC
//    let updateRegistry arcDir =
//        fromArcFolder arcDir
//        |> overWrite arcDir