namespace ARC

open FileSystem
open ISA

open Contract
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

    static member updateISA (isa : ISA.Investigation) (arc : ARC) : ARC =
        raise (System.NotImplementedException())

    static member updateCWL (cwl : CWL.CWL) (arc : ARC) : ARC =
        raise (System.NotImplementedException())

    static member updateFileSystem (fileSystem : FileSystem.FileSystem) (arc : ARC) : ARC =
        raise (System.NotImplementedException())

    static member addFile (path : string) (arc : ARC) : ARC =
        FileSystem.addFile |> ignore
        ARC.updateFileSystem |> ignore
        raise (System.NotImplementedException())

    static member addFolder (path : string) (arc : ARC) : ARC =
        FileSystem.addFolder |> ignore
        ARC.updateFileSystem |> ignore
        raise (System.NotImplementedException())

    static member addFolders (paths : string array) (arc : ARC) : ARC =
        paths
        |> Array.fold (fun arc path -> ARC.addFolder path arc) arc |> ignore
        raise (System.NotImplementedException())

    /// Add folder to ARC.FileSystem and add .gitkeep file to the folder
    static member addEmptyFolder (path : string) (arc : ARC) : ARC =
        FileSystem.addFolder  |> ignore   
        FileSystem.addFile (Path.combine path Path.gitKeepFileName) |> ignore
        ARC.updateFileSystem |> ignore
        raise (System.NotImplementedException())

    static member addEmptyFolders (paths : string array) (arc : ARC) : ARC =
        paths
        |> Array.fold (fun arc path -> ARC.addEmptyFolder path arc) arc |> ignore
        raise (System.NotImplementedException())


    /// Add assay folder to the ARC.FileSystem and update the ARC.ISA with the new assay metadata
    static member addAssay (assay : ISA.Assay) (studyIdentifier : string) (arc : ARC) : ARC = 
  
        // - Contracts - //
        // create spreadsheet assays/AssayName/isa.assay.xlsx  
        // create text assays/AssayName/dataset/.gitkeep 
        // create text assays/AssayName/dataset/Readme.md
        // create text assays/AssayName/protocols/.gitkeep 
        // update spreadsheet isa.investigation.xlsx

        // - ISA - //
        let assayFolderPath = Path.combineAssayFolderPath assay
        let assaySubFolderPaths = Path.combineAssaySubfolderPaths assay
        let assayReadmeFilePath = Path.combine assayFolderPath Path.assayReadmeFileName
        let updatedInvestigation = ArcInvestigation.addAssay assay studyIdentifier arc.ISA

        // - FileSystem - //
        // Create assay root folder in ARC.FileSystem
        ARC.addFolder assayFolderPath arc 
        // Create assay subfolders in ARC.FileSystem
        |> ARC.addEmptyFolders assaySubFolderPaths 
        // Create assay readme file in ARC.FileSystem
        |> ARC.addFile assayReadmeFilePath 
        // Update ARC.ISA with the updated investigation
        |> ARC.updateISA updatedInvestigation 

    // to-do: we need a function that generates only create contracts from a ARC data model. 
    // reason: contracts are initially designed to sync disk with in-memory model while working on the arc.
    // but we need a way to create an arc programmatically and then write it to disk.

    // to-do: function that returns read contracts based on a list of paths.
    // the list of paths is used to create a filesystem tree
    static member createReadContracts (paths : string array) : Contract array =
        let fileTree = FileSystemTree.fromFilePaths paths
        raise (System.NotImplementedException())