module ARCtrl.ArcPathHelper

open System

let [<Literal>] PathSeperator = '/'
let [<Literal>] PathSeperatorWindows = '\\'
let seperators = [|PathSeperator; PathSeperatorWindows|]

// Files
let [<Literal>] DatamapFileName = "isa.datamap.xlsx"
let [<Literal>] AssayFileName = "isa.assay.xlsx"
let [<Literal>] StudyFileName = "isa.study.xlsx"
let [<Literal>] WorkflowFileName = "isa.workflow.xlsx"
let [<Literal>] WorkflowCWLFileName = "workflow.cwl"
let [<Literal>] RunFileName = "isa.run.xlsx"
let [<Literal>] RunCWLFileName = "run.cwl"
let [<Literal>] RunYMLFileName = "run.yml"
let [<Literal>] InvestigationFileName = "isa.investigation.xlsx"
let [<Literal>] GitKeepFileName = ".gitkeep"
let [<Literal>] READMEFileName = "README.md"
let [<Literal>] ValidationPackagesYamlFileName = "validation_packages.yml"
let [<Literal>] LICENSEFileName = "LICENSE"
let alternativeLICENSEFileNames = ["LICENSE.txt"; "LICENSE.md"; "LICENSE.rst"]



// Folder
let [<Literal>] ARCConfigFolderName = ".arc"
let [<Literal>] AssaysFolderName = "assays"
let [<Literal>] StudiesFolderName = "studies"
let [<Literal>] WorkflowsFolderName = "workflows"
let [<Literal>] RunsFolderName = "runs"
let [<Literal>] AssayProtocolsFolderName = "protocols"
let [<Literal>] AssayDatasetFolderName = "dataset"
let [<Literal>] StudiesProtocolsFolderName = "protocols"
let [<Literal>] StudiesResourcesFolderName = "resources"



//let assaySubFolderNames = [|assayDatasetFolderName;assayProtocolsFolderName|]

let split(path: string) =
    path.Split(seperators, enum<StringSplitOptions>(3))
    |> Array.filter (fun p -> p <> "" && p <> ".")

let combine (path1 : string) (path2 : string) : string =
    let path1_trimmed = path1.TrimEnd(seperators)
    let path2_trimmed = path2.TrimStart(seperators)
    let combined = path1_trimmed + string PathSeperator + path2_trimmed
    combined // should we trim any excessive path seperators?

let combineMany (paths : string []) : string =
    paths
    |> Array.mapi (fun i p ->
        if i = 0 then p.TrimEnd(seperators)
        elif i = (paths.Length-1) then p.TrimStart(seperators)
        else
            p.Trim(seperators)
    )
    |> String.concat(string PathSeperator)

let getFileName (path: string) : string =
    split path |> Array.last

/// <summary>
/// Checks if `path` points to a file with the name `fileName`
/// </summary>
/// <param name="fileName">The name of the file the path should point to.</param>
/// <param name="path">The path to a file.</param>
let isFile (fileName: string) (path: string) = (getFileName path) = fileName


let getAssayFolderPath (assayIdentifier: string) =
    combine AssaysFolderName assayIdentifier

let getStudyFolderPath (studyIdentifier: string) =
    combine StudiesFolderName studyIdentifier

let getWorkflowFolderPath (workflowIdentifier: string) =
    combine WorkflowsFolderName workflowIdentifier

let getRunFolderPath (runIdentifier: string) =
    combine RunsFolderName runIdentifier