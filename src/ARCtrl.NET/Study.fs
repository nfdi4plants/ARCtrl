namespace ARCtrl.NET

//open ISADotNet
//open ISADotNet.XLSX
//open System.IO 


//module Study = 

//    let rootFolderName = "studies"

//    let studyFileName = "isa.study.xlsx"

//    let subFolderPaths = 
//        ["resources";"protocol"]

//    module StudyFolder =
        
//        /// Checks if a Study folder exists in the ARC.
//        let exists (arc : string) (identifier : string) =
//            Path.Combine([|arc;rootFolderName;identifier|])
//            |> System.IO.Directory.Exists

//        /// Returns the Study identifiers of the Study Files located in each Study's folder of a given path to an ARC.
//        let findStudyIdentifiers arcDir =
//            let log = Logging.createLogger "findStudyIdentifiersLog"
//            let studiesPath = Path.Combine(arcDir, rootFolderName)
//            try
//                let studyFolders = Directory.GetDirectories studiesPath
//                studyFolders
//                |> Array.collect (
//                    fun sf ->
//                        let studyIdentifier = (DirectoryInfo sf).Name
//                        Directory.GetFiles sf
//                        |> Array.choose (fun s -> 
//                            if s.EndsWith "isa.study.xlsx" then
//                                Some studyIdentifier
//                            elif s.EndsWith "_isa.study.xlsx" then
//                                log.Warn $"The Study File of Study {studyIdentifier} has a deprecated File Name: {s}"
//                                Some studyIdentifier
//                            else 
//                                None
//                        )
//                )
//            with e -> 
//                log.Error e.Message
//                [||]

//    let readFromFolder (arc : string) (folderPath : string) =
//        let sp = Path.Combine(folderPath,studyFileName).Replace(@"\","/")
//        let study = StudyFile.Study.fromFile sp
//        match study.Assays with
//        | Some assays ->
//            let contacts,ps,assays = 
//                assays
//                |> List.fold (fun (contacts,processSequence,assays) a -> 
//                    let c,a = Assay.readByFileName arc a.FileName.Value               
//                    contacts @ c, processSequence @ (a.ProcessSequence |> Option.defaultValue []), assays @ [a]
//                ) (study.Contacts |> Option.defaultValue [],study.ProcessSequence |> Option.defaultValue [],[])
//            let ref = ps |> ProcessSequence.updateByItself
//            let updatedAssays =
//                assays
//                |> List.map (fun a ->
//                    {a with ProcessSequence = a.ProcessSequence |> Option.map (ProcessSequence.updateByRef ref)}
//                )
//            {study with 
//                ProcessSequence = study.ProcessSequence |> Option.map (ProcessSequence.updateByRef ref)
//                Assays = Some updatedAssays
//                Contacts = Option.fromValueWithDefault [] (contacts |> List.distinct)
//            }
//        | None -> 
//            {study with ProcessSequence = study.ProcessSequence |> Option.map ProcessSequence.updateByItself}

//    let readByIdentifier (arc : string) (studyIdentifier : string) =
//        Path.Combine ([|arc;rootFolderName;studyIdentifier|])
//        |> readFromFolder arc

//    /// Writes a study to the given folder. Fails, if the file already exists
//    let writeToFolder (folderPath : string) (study : Study) =

//        let log = Logging.createLogger "StudyWriteLog"
        
//        log.Info("Start Study Write")

//        let studyFilePath = Path.Combine (folderPath,studyFileName)

//        if System.IO.File.Exists(studyFilePath) then
//            log.Error("Study file does already exist.")

//        else 
//            StudyFile.Study.toFile studyFilePath study 

//    /// Writes a study to the given folder. Overwrites it, if the file already exists
//    let overWriteToFolder (folderPath : string) (study : Study) =
           
//        let log = Logging.createLogger "StudyWriteLog"
//        log.Info("Start Study Write")

//        let studyFilePath = Path.Combine (folderPath,studyFileName)

//        if System.IO.File.Exists(studyFilePath) then
//            try 
//                let cache = File.ReadAllBytes(studyFilePath)
//                File.Delete(studyFilePath)
//                try                     
//                    StudyFile.Study.toFile studyFilePath study 
//                with
//                | err -> 
//                    File.WriteAllBytes(studyFilePath,cache)
//                    log.Error($"Study file could not be overwritten: {err.Message}")
//            with 
//            | err -> 
//                log.Error($"Study file could not be overwritten: {err.Message}")
//        else 
//            StudyFile.Study.toFile studyFilePath study 

//    /// Writes a study to the arc. Fails, if the file already exists
//    let write (arc : string) (study : Study) =

//        let log = Logging.createLogger "StudyWriteLog"
        
//        log.Info("Start Study Write")

//        if study.FileName.IsNone then
//            log.Error("Cannot write study to arc, as it has no filename")
//        else 

//            let studyFilePath = Path.Combine ([|arc;rootFolderName;study.FileName.Value|])

//            if System.IO.File.Exists(studyFilePath) then
//                log.Error("Study file does already exist.")

//            else 
//                StudyFile.Study.toFile studyFilePath study 

//    /// Writes a study to the arc. Overwrites it, if the file already exists
//    let overWrite (arc : string) (study : Study) =
//        let log = Logging.createLogger "StudyWriteLog"
        
//        log.Info("Start Study Write")

//        if study.FileName.IsNone then
//            log.Error("Cannot write study to arc, as it has no filename")
//        else 
//            let studyFilePath = Path.Combine ([|arc;rootFolderName;study.FileName.Value|])
//            if System.IO.File.Exists(studyFilePath) then
//                try 
//                    let cache = File.ReadAllBytes(studyFilePath)
//                    File.Delete(studyFilePath)
//                    try                     
//                        StudyFile.Study.toFile studyFilePath study 
//                    with
//                    | err -> 
//                        File.WriteAllBytes(studyFilePath,cache)
//                        log.Error($"Study file could not be overwritten: {err.Message}")
//                with 
//                | err -> 
//                    log.Error($"Study file could not be overwritten: {err.Message}")
//            else 
//                StudyFile.Study.toFile studyFilePath study 

//    let init (arc : string) (study : Study) =
        
//        if study.Identifier.IsNone || study.FileName.IsNone then
//            failwith "Given study does not contain identifier or filename"

//        let studyIdentifier = study.Identifier.Value

//        if StudyFolder.exists arc studyIdentifier then
//            printfn $"Study folder with identifier {studyIdentifier} already exists."
//        else
//            subFolderPaths 
//            |> List.iter (fun n ->
//                let dp = Path.Combine([|arc;rootFolderName;studyIdentifier;n|])
//                let dir = Directory.CreateDirectory(dp)
//                File.Create(Path.Combine(dir.FullName, ".gitkeep")).Close()
//            )

//            let studyFilePath = Path.Combine([|arc;rootFolderName;study.FileName.Value|])

//            StudyFile.Study.toFile studyFilePath study


//    let initFromName  (arc : string) (studyName : string) =
        
//        let studyFileName = Path.Combine(studyName,studyFileName).Replace(@"\","/")

//        let study = Study.create(FileName = studyFileName, Identifier = studyName)

//        init arc study

//    /// Takes the path to an ARC and lists all study identifiers registered in this ARC's investigation file.
//    let list (arcDir : string) =
        
//        let log = Logging.createLogger "StudyListLog"
        
//        log.Info("Start Study List")

//        (* the following part is _not nice_: The functionality for this already exists in Investigation.fs. Unfortunately, Investigation.fs is compiled *AFTER*
//        Study.fs and, thus, we cannot use functions from there. Moving Study.fs after Investigation.fs in compilation order also does not work due to 
//        Investigation.fs using functions from Study.fs.
//        If someone finds a solution for this F#-specific compilation problem, feel free to fix :) *)
//        let investigationFilePath = Path.Combine(arcDir, "isa.investigation.xlsx")
//        log.Trace($"InvestigationFile: {investigationFilePath}")
//        let investigation = Investigation.fromFile investigationFilePath
//        // end of part

//        let studyFileIdentifiers = set (StudyFolder.findStudyIdentifiers arcDir)

//        let studyIdentifiers = 
//            investigation.Studies
//            |> Option.defaultValue []
//            |> List.choose (fun s -> 
//                match s.Identifier with
//                | None | Some "" -> 
//                    log.Warn("Study does not have identifier")
//                    None
//                | Some i -> Some i
//            ) 
//            |> set
            
//        let onlyRegistered = Set.difference studyIdentifiers studyFileIdentifiers
//        let onlyInitialized = Set.difference studyFileIdentifiers studyIdentifiers
//        let combined = Set.union studyIdentifiers studyFileIdentifiers

//        if not onlyRegistered.IsEmpty then
//            log.Warn("The ARC contains following registered studies that have no associated file:")
//            onlyRegistered
//            |> Seq.iter ((sprintf "%s") >> log.Warn) 
//            log.Info($"You can init the study file using \"arc s init\"")

//        if not onlyInitialized.IsEmpty then
//            log.Warn("The ARC contains study files with the following identifiers not registered in the investigation:")
//            onlyInitialized
//            |> Seq.iter ((sprintf "%s") >> log.Warn) 
//            log.Info($"You can register the study using \"arc s register\"")

//        if combined.IsEmpty then
//            log.Error("The ARC does not contain any studies.")

//        combined
//        |> Seq.map (fun identifier ->
//            //log.Debug(sprintf "Study: %s" identifier)
//            sprintf "Study: %s" identifier
//        )