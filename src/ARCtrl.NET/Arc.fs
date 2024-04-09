namespace ARCtrl.NET

open ARCtrl
open Contract

[<AutoOpen>]
module ARCExtensions = 

    type ARC with

        member this.Write(arcPath) =
            this.GetWriteContracts()
            |> Array.iter (Contract.fulfillWriteContract arcPath)

        static member load (arcPath : string) =
            let paths = Path.getAllFilePaths arcPath
            let arc = ARC.fromFilePaths paths

            let contracts = arc.GetReadContracts()

            let fulFilledContracts = 
                contracts 
                |> Array.map (fulfillReadContract arcPath)

            arc.SetISAFromContracts(fulFilledContracts)
            arc

    ///// Initializes the ARC-specific folder structure.
    //let initFolders (arcPath) =        
    //    subFolderPaths
    //    |> List.iter (fun n ->
    //        let dp = Path.Combine(arcPath,n)
    //        let dir = Directory.CreateDirectory(dp)
    //        File.Create(Path.Combine(dir.FullName, ".gitkeep")).Close()
    //    )

        /// Initializes the ARC-specific git repository.
        static member initGit(workDir,?repositoryAddress : string,?branch : string) =

            let log = Logging.createLogger "ArcInitGitLog"

            log.Trace("Init Git repository")

            let branch = branch |> Option.defaultValue "main"

            try

                GitHelper.executeGitCommand workDir $"init -b {branch}"

                log.Trace("Add remote repository")
                match repositoryAddress with
                | None -> ()
                | Some remote ->
                    GitHelper.executeGitCommand workDir $"remote add origin {remote}"
                    //GitHelper.executeGitCommand workDir $"branch -u origin/{branch} {branch}"

            with 
            | e -> 

                log.Error($"Git could not be set up. Please try installing Git cli and run `arc git init`.\n\t{e}")

    ///// Initializes the ARC-specific folder structure, investigation file and git repository.
    //let init (workDir : string) (identifier : string) (repositoryAddress : string option) (branch : string option) =

    //    let log = Logging.createLogger "ArcInitLog"
        
    //    log.Info("Start Arc Init")

    //    log.Trace("Create Directory")

    //    Directory.CreateDirectory workDir |> ignore

    //    log.Trace("Initiate folder structure")
        
    //    initFolders workDir

    //    log.Trace("Initiate investigation file")

    //    let inv = ISADotNet.Investigation.create(Identifier=identifier)
    //    Investigation.write workDir inv

    //    initGit workDir repositoryAddress branch
