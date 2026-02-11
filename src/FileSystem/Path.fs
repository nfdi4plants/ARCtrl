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

let private hasLeadingSeparator (path: string) =
    path.Length > 0 && (path.[0] = PathSeperator || path.[0] = PathSeperatorWindows)

let private isUncPath (path: string) =
    path.StartsWith("//") || path.StartsWith(@"\\")

let private tryGetDrivePrefix (path: string) =
    if
        path.Length > 2
        && System.Char.IsLetter(path.[0])
        && path.[1] = ':'
        && (path.[2] = PathSeperator || path.[2] = PathSeperatorWindows)
    then
        Some (path.Substring(0, 2))
    else
        None

let private splitWithPrefix (path: string) : string option * string [] =
    let trimmed = path.Trim()
    match tryGetDrivePrefix trimmed with
    | Some drivePrefix ->
        let remainder = trimmed.Substring(2)
        Some drivePrefix, split remainder
    | None when isUncPath trimmed ->
        Some "//", split trimmed
    | None when hasLeadingSeparator trimmed ->
        Some "/", split trimmed
    | None ->
        None, split trimmed

let private normalizeRootPrefix (prefix: string) =
    if prefix = "//" then "//"
    elif prefix.EndsWith(":") then $"{prefix}/"
    else "/"

let private buildPathFromPrefixAndSegments (prefixOpt: string option) (segments: string []) : string =
    if segments.Length = 0 then
        match prefixOpt with
        | Some prefix -> normalizeRootPrefix prefix
        | None -> ""
    else
        let combined = String.concat (string PathSeperator) segments
        match prefixOpt with
        | Some "//" -> $"//{combined}"
        | Some prefix when prefix.EndsWith(":") -> $"{prefix}/{combined}"
        | Some "/" -> $"/{combined}"
        | Some prefix -> $"{prefix}/{combined}"
        | None -> combined

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
    let trimmedPath = path.Trim()
    let prefixOpt, pathSegments = splitWithPrefix trimmedPath
    let normalizedSegments =
        pathSegments
        |> normalizeSegments
    if normalizedSegments.Length = 0 then
        match prefixOpt with
        | Some prefix -> normalizeRootPrefix prefix
        | None -> trimmedPath
    else
        buildPathFromPrefixAndSegments prefixOpt normalizedSegments

let normalizePathKey (path: string) : string =
    let normalized = normalize path
    if normalized = "" then path.Trim() else normalized

/// Resolve a path (possibly relative) against the directory of a file path.
let resolvePathFromFile (filePath: string) (path: string) : string =
    let trimmedPath = path.Trim()
    let pathPrefixOpt, pathSegments = splitWithPrefix trimmedPath

    if pathPrefixOpt.IsSome then
        normalize trimmedPath
    else
        let filePrefixOpt, filePathSegments = splitWithPrefix filePath
        let fileDirectorySegments =
            if filePathSegments.Length <= 1 then [||]
            else filePathSegments[0..filePathSegments.Length - 2]

        Array.append fileDirectorySegments pathSegments
        |> normalizeSegments
        |> fun resolvedSegments ->
            if resolvedSegments.Length = 0 then
                match filePrefixOpt with
                | Some prefix -> normalizeRootPrefix prefix
                | None -> trimmedPath
            else
                buildPathFromPrefixAndSegments filePrefixOpt resolvedSegments

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
