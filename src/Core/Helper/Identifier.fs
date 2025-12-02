/// This module contains helper functions to handle study/assay/investigation identifiers in an unsafe, forced way.
module ARCtrl.Helper.Identifier

open Regex.ActivePatterns

/// This pattern should never be used as standalone pattern!
let [<Literal>] internal InnerValidCharactersPattern = @"[a-zA-Z0-9_\- ]+"

// Define a regular expression pattern for valid characters
let [<Literal>] ValidIdentifierPattern = @"^" + InnerValidCharactersPattern + @"$"

// Define a regular expression pattern for valid characters
let [<Literal>] ValidAssayFileNamePattern = @"^(assays(\/|\\))?(?<identifier>" + InnerValidCharactersPattern + @")((\/|\\)isa.assay.xlsx)?$"

// Define a regular expression pattern for valid characters
let [<Literal>] ValidStudyFileNamePattern = @"^(studies(\/|\\))?(?<identifier>" + InnerValidCharactersPattern + @")((\/|\\)isa.study.xlsx)?$"

let [<Literal>] ValidWorkflowFileNamePattern = @"^(workflows(\/|\\))?(?<identifier>" + InnerValidCharactersPattern + @")((\/|\\)isa.workflow.xlsx)?$"

let [<Literal>] ValidRunFileNamePattern = @"^(runs(\/|\\))?(?<identifier>" + InnerValidCharactersPattern + @")((\/|\\)isa.run.xlsx)?$"

// Function to check if a string contains only valid characters
let tryCheckValidCharacters (identifier: string) =
    match identifier with
    | Regex ValidIdentifierPattern _ -> true
    | _ -> false

// Function to check if a string contains only valid characters
let checkValidCharacters (identifier: string) =
    match tryCheckValidCharacters identifier with
    | true -> ()
    | false -> failwith $"New identifier \"{identifier}\" contains forbidden characters! Allowed characters are: letters, digits, underscore (_), dash (-) and whitespace ( )."

let [<Literal>] MISSING_IDENTIFIER = "MISSING_IDENTIFIER_"

let createMissingIdentifier() =
    MISSING_IDENTIFIER + System.Guid.NewGuid().ToString()

let isMissingIdentifier (str: string) =
    str.StartsWith(MISSING_IDENTIFIER)

let removeMissingIdentifier (str: string) =
    if str.StartsWith(MISSING_IDENTIFIER) then "" else str

/// Assay only contains "FileName" in isa.assay.xlsx. To unify naming in our model, on read-in we transform fileName to identifier and reverse for writing.
[<RequireQualifiedAccess>]
module Assay =
    

    /// <summary>
    /// On read-in the FileName can be any combination of "assays" (assay folder name), assayIdentifier and "isa.assay.xlsx" (the actual file name).
    ///
    /// This functions extracts assayIdentifier from any of these combinations and returns it.
    /// </summary>
    /// <param name="fileName">FileName as written in isa.assay.xlsx metadata sheet</param>
    let identifierFromFileName (fileName: string) : string =
        match fileName with
        | Regex ValidAssayFileNamePattern m -> 
            let identifier = m.Groups.["identifier"].Value
            identifier
        | _ -> failwith $"Cannot parse assay identifier from FileName `{fileName}`"

    /// <summary>
    /// On read-in the FileName can be any combination of "assays" (assay folder name), assayIdentifier and "isa.assay.xlsx" (the actual file name).
    ///
    /// This functions extracts assayIdentifier from any of these combinations and returns it.
    /// </summary>
    /// <param name="fileName">FileName as written in isa.assay.xlsx metadata sheet</param>
    let tryIdentifierFromFileName (fileName: string) : string option =
        match fileName with
        | Regex ValidAssayFileNamePattern m -> 
            let identifier = m.Groups.["identifier"].Value
            Some identifier
        | _ -> None

    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `assays/assayIdentifier/isa.assay.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct assay identifier</param>
    let fileNameFromIdentifier (identifier: string) : string =        
        checkValidCharacters (identifier)
        ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.AssaysFolderName; identifier; ARCtrl.ArcPathHelper.AssayFileName|]

    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `assays/assayIdentifier/isa.assay.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct assay identifier</param>
    let tryFileNameFromIdentifier (identifier: string) : string option =        
        if tryCheckValidCharacters (identifier) then
            ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.AssaysFolderName; identifier; ARCtrl.ArcPathHelper.AssayFileName|]
            |> Some
        else None


    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `assays/assayIdentifier/isa.datamap.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct assay identifier</param>
    let datamapFileNameFromIdentifier (identifier: string) : string =        
        checkValidCharacters (identifier)
        ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.AssaysFolderName; identifier; ARCtrl.ArcPathHelper.DatamapFileName|]

    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `assays/assayIdentifier/isa.datamap.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct assay identifier</param>
    let tryDatamapFileNameFromIdentifier (identifier: string) : string option =        
        if tryCheckValidCharacters (identifier) then
            ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.AssaysFolderName; identifier; ARCtrl.ArcPathHelper.DatamapFileName|]
            |> Some
        else None

/// Assay only contains "FileName" in isa.assay.xlsx. To unify naming in our model, on read-in we transform fileName to identifier and reverse for writing.
[<RequireQualifiedAccess>]
module Study =
    
    /// <summary>
    /// On read-in the FileName can be any combination of "studies" (study folder name), studyIdentifier and "isa.study.xlsx" (the actual file name).
    ///
    /// This functions extracts studyIdentifier from any of these combinations and returns it.
    /// </summary>
    /// <param name="fileName">FileName as written in isa.study.xlsx metadata sheet</param>
    let identifierFromFileName (fileName: string) : string =
        match fileName with
        | Regex ValidStudyFileNamePattern m -> 
            let identifier = m.Groups.["identifier"].Value
            identifier
        | _ -> failwith $"Cannot parse study identifier from FileName `{fileName}`"

    /// <summary>
    /// On read-in the FileName can be any combination of "studies" (study folder name), studyIdentifier and "isa.study.xlsx" (the actual file name).
    ///
    /// This functions extracts studyIdentifier from any of these combinations and returns it.
    /// </summary>
    /// <param name="fileName">FileName as written in isa.study.xlsx metadata sheet</param>
    let tryIdentifierFromFileName (fileName: string) : string option =
        match fileName with
        | Regex ValidStudyFileNamePattern m -> 
            let identifier = m.Groups.["identifier"].Value
            Some identifier
        | _ -> None


    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `studies/studyIdentifier/isa.study.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct study identifier</param>
    let fileNameFromIdentifier (identifier: string) : string =
        checkValidCharacters (identifier)
        ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.StudiesFolderName; identifier; ARCtrl.ArcPathHelper.StudyFileName|]

    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `studies/studyIdentifier/isa.study.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct study identifier</param>
    let tryFileNameFromIdentifier (identifier: string) : string option =
        if tryCheckValidCharacters (identifier) then
            ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.StudiesFolderName; identifier; ARCtrl.ArcPathHelper.StudyFileName|]
            |> Some
        else None

    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `studies/studyIdentifier/isa.investigation.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct study identifier</param>
    let datamapFileNameFromIdentifier (identifier: string) : string =
        checkValidCharacters (identifier)
        ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.StudiesFolderName; identifier; ARCtrl.ArcPathHelper.DatamapFileName|]

    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `studies/studyIdentifier/isa.investigation.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct study identifier</param>
    let tryDatamapFileNameFromIdentifier (identifier: string) : string option =
        if tryCheckValidCharacters (identifier) then
            ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.StudiesFolderName; identifier; ARCtrl.ArcPathHelper.DatamapFileName|]
            |> Some
        else None


/// Workflow only contains "FileName" in isa.workflow.xlsx. To unify naming in our model, on read-in we transform fileName to identifier and reverse for writing.
[<RequireQualifiedAccess>]
module Workflow =

    /// <summary>
    /// On read-in the FileName can be any combination of "workflows" (workflow folder name), workflowIdentifier and "isa.workflow.xlsx" (the actual file name).
    ///
    /// This functions extracts workflowIdentifier from any of these combinations and returns it.
    /// </summary>
    /// <param name="fileName">FileName as written in isa.workflow.xlsx metadata sheet</param>
    let identifierFromFileName (fileName: string) : string =
        match fileName with
        | Regex ValidWorkflowFileNamePattern m -> 
            let identifier = m.Groups.["identifier"].Value
            identifier
        | _ -> failwith $"Cannot parse workflow identifier from FileName `{fileName}`"

    /// <summary>
    /// On read-in the FileName can be any combination of "workflows" (workflow folder name), workflowIdentifier and "isa.workflow.xlsx" (the actual file name).
    ///
    /// This functions extracts workflowIdentifier from any of these combinations and returns it.
    /// </summary>
    /// <param name="fileName">FileName as written in isa.workflow.xlsx metadata sheet</param>
    let tryIdentifierFromFileName (fileName: string) : string option =
        match fileName with
        | Regex ValidWorkflowFileNamePattern m -> 
            let identifier = m.Groups.["identifier"].Value
            Some identifier
        | _ -> None

    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `workflows/workflowIdentifier/isa.workflow.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct workflow identifier</param>
    let fileNameFromIdentifier (identifier: string) : string =
        checkValidCharacters (identifier)
        ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.WorkflowsFolderName; identifier; ARCtrl.ArcPathHelper.WorkflowFileName|]

    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `workflows/workflowIdentifier/isa.workflow.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct workflow identifier</param>
    let tryFileNameFromIdentifier (identifier: string) : string option =
        if tryCheckValidCharacters (identifier) then
            ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.WorkflowsFolderName; identifier; ARCtrl.ArcPathHelper.WorkflowFileName|]
            |> Some
        else None

    /// <summary>
    /// On writing a cwl file we unify our output to a relative path to ARC root. So: `workflows/workflowIdentifier/workflow.cwl`.
    /// </summary>
    /// <param name="identifier">Any correct workflow identifier</param>
    let cwlFileNameFromIdentifier (identifier: string) : string =
        checkValidCharacters (identifier)
        ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.WorkflowsFolderName; identifier; ARCtrl.ArcPathHelper.WorkflowCWLFileName|]

    /// <summary>
    /// On writing a cwl file we unify our output to a relative path to ARC root. So: `workflows/workflowIdentifier/workflow.cwl`.
    /// </summary>
    /// <param name="identifier">Any correct workflow identifier</param>
    let tryCwlFileNameFromIdentifier (identifier: string) : string option =
        if tryCheckValidCharacters (identifier) then
            ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.WorkflowsFolderName; identifier; ARCtrl.ArcPathHelper.WorkflowCWLFileName|]
            |> Some
        else None

    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `workflows/workflowIdentifier/isa.datamap.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct workflow identifier</param>
    let datamapFileNameFromIdentifier (identifier: string) : string =
        checkValidCharacters (identifier)
        ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.WorkflowsFolderName; identifier; ARCtrl.ArcPathHelper.DatamapFileName|]

    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `workflows/workflowIdentifier/isa.datamap.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct workflow identifier</param>
    let tryDatamapFileNameFromIdentifier (identifier: string) : string option =
        if tryCheckValidCharacters (identifier) then
            ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.WorkflowsFolderName; identifier; ARCtrl.ArcPathHelper.DatamapFileName|]
            |> Some
        else None

[<RequireQualifiedAccess>]
module Run =

    /// <summary>
    /// On read-in the FileName can be any combination of "runs" (run folder name), runIdentifier and "isa.run.xlsx" (the actual file name).
    ///
    /// This functions extracts runIdentifier from any of these combinations and returns it.
    /// </summary>
    /// <param name="fileName">FileName as written in isa.run.xlsx metadata sheet</param>
    let identifierFromFileName (fileName: string) : string =
        match fileName with
        | Regex ValidRunFileNamePattern m -> 
            let identifier = m.Groups.["identifier"].Value
            identifier
        | _ -> failwith $"Cannot parse run identifier from FileName `{fileName}`"

    /// <summary>
    /// On read-in the FileName can be any combination of "runs" (run folder name), runIdentifier and "isa.run.xlsx" (the actual file name).
    ///
    /// This functions extracts runIdentifier from any of these combinations and returns it.
    /// </summary>
    /// <param name="fileName">FileName as written in isa.run.xlsx metadata sheet</param>
    let tryIdentifierFromFileName (fileName: string) : string option =
        match fileName with
        | Regex ValidRunFileNamePattern m -> 
            let identifier = m.Groups.["identifier"].Value
            Some identifier
        | _ -> None

    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `runs/runIdentifier/isa.run.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct run identifier</param>
    let fileNameFromIdentifier (identifier: string) : string =
        checkValidCharacters (identifier)
        ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.RunsFolderName; identifier; ARCtrl.ArcPathHelper.RunFileName|]

    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `runs/runIdentifier/isa.run.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct run identifier</param>
    let tryFileNameFromIdentifier (identifier: string) : string option =
        if tryCheckValidCharacters (identifier) then
            ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.RunsFolderName; identifier; ARCtrl.ArcPathHelper.RunFileName|]
            |> Some
        else None

    /// <summary>
    /// On writing a cwl file we unify our output to a relative path to ARC root. So: `runs/runIdentifier/run.cwl`.
    /// </summary>
    /// <param name="identifier">Any correct run identifier</param>
    let cwlFileNameFromIdentifier (identifier: string) : string =
        checkValidCharacters (identifier)
        ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.RunsFolderName; identifier; ARCtrl.ArcPathHelper.RunCWLFileName|]

    /// <summary>
    /// On writing a cwl file we unify our output to a relative path to ARC root. So: `runs/runIdentifier/run.cwl`.
    /// </summary>
    /// <param name="identifier">Any correct run identifier</param>
    let tryCwlFileNameFromIdentifier (identifier: string) : string option =
        if tryCheckValidCharacters (identifier) then
            ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.RunsFolderName; identifier; ARCtrl.ArcPathHelper.RunCWLFileName|]
            |> Some
        else None

    /// <summary>
    /// On writing a yml file we unify our output to a relative path to ARC root. So: `runs/runIdentifier/run.yml`.
    /// </summary>
    /// <param name="identifier">Any correct run identifier</param>
    let ymlFileNameFromIdentifier (identifier: string) : string =
        checkValidCharacters (identifier)
        ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.RunsFolderName; identifier; ARCtrl.ArcPathHelper.RunYMLFileName|]

    /// <summary>
    /// On writing a yml file we unify our output to a relative path to ARC root. So: `runs/runIdentifier/run.yml`.
    /// </summary>
    /// <param name="identifier">Any correct run identifier</param>
    let tryYmlFileNameFromIdentifier (identifier: string) : string option =
        if tryCheckValidCharacters (identifier) then
            ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.RunsFolderName; identifier; ARCtrl.ArcPathHelper.RunYMLFileName|]
            |> Some
        else None


    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `runs/runIdentifier/isa.datamap.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct run identifier</param>
    let datamapFileNameFromIdentifier (identifier: string) : string =
        checkValidCharacters (identifier)
        ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.RunsFolderName; identifier; ARCtrl.ArcPathHelper.DatamapFileName|]

    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `runs/runIdentifier/isa.datamap.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct run identifier</param>
    let tryDatamapFileNameFromIdentifier (identifier: string) : string option =
        if tryCheckValidCharacters (identifier) then
            ARCtrl.ArcPathHelper.combineMany [|ARCtrl.ArcPathHelper.RunsFolderName; identifier; ARCtrl.ArcPathHelper.DatamapFileName|]
            |> Some
        else None
