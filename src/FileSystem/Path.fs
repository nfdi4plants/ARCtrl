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

/// Normalize path segments by removing "." and resolving ".." where possible.
let normalizeSegments (segments: string []) : string [] =
    let resolved = ResizeArray<string>()
    for rawSegment in segments do
        let segment = rawSegment.Trim()
        if segment = "" || segment = "." then
            ()
        elif segment = ".." then
            if resolved.Count > 0 && resolved.[resolved.Count - 1] <> ".." then
                resolved.RemoveAt(resolved.Count - 1)
            else
                resolved.Add(segment)
        else
            resolved.Add(segment)
    resolved.ToArray()

/// Normalize a path by resolving "." and ".." segments.
/// Note: If normalization yields no segments, the trimmed original path is returned
/// to preserve relative markers used by existing callers (e.g. "." or "./").
let normalize (path: string) : string =
    let normalizedSegments =
        path
        |> split
        |> normalizeSegments
    if normalizedSegments.Length = 0 then
        path.Trim()
    else
        combineMany normalizedSegments

let normalizePathKey (path: string) : string =
    let normalized = normalize path
    if normalized = "" then path.Trim() else normalized

/// Resolve a path (possibly relative) against the directory of a file path.
let resolvePathFromFile (filePath: string) (path: string) : string =
    let fileDirectorySegments =
        let filePathSegments = split filePath
        if filePathSegments.Length <= 1 then [||]
        else filePathSegments[0..filePathSegments.Length - 2]
    Array.append fileDirectorySegments (split path)
    |> normalizeSegments
    |> fun resolvedSegments ->
        if resolvedSegments.Length = 0 then
            path.Trim()
        else
            combineMany resolvedSegments

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
