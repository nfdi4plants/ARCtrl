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

// Function to check if a string contains only valid characters
let tryCheckValidCharacters (identifier: string) =
    match identifier with
    | Regex ValidIdentifierPattern _ -> true
    | _ -> false

// Function to check if a string contains only valid characters
let checkValidCharacters (identifier: string) =
    match tryCheckValidCharacters identifier with
    | true -> ()
    | false -> failwith $"New identifier \"{identifier}\"contains forbidden characters! Allowed characters are: letters, digits, underscore (_), dash (-) and whitespace ( )."

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
        | _ -> failwith $"Cannot parse identifier from FileName `{fileName}`"

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
        ARCtrl.Path.combineMany [|ARCtrl.Path.AssaysFolderName; identifier; ARCtrl.Path.AssayFileName|]

    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `assays/assayIdentifier/isa.assay.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct assay identifier</param>
    let tryFileNameFromIdentifier (identifier: string) : string option =        
        if tryCheckValidCharacters (identifier) then
            ARCtrl.Path.combineMany [|ARCtrl.Path.AssaysFolderName; identifier; ARCtrl.Path.AssayFileName|]
            |> Some
        else None


    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `assays/assayIdentifier/isa.datamap.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct assay identifier</param>
    let datamapFileNameFromIdentifier (identifier: string) : string =        
        checkValidCharacters (identifier)
        ARCtrl.Path.combineMany [|ARCtrl.Path.AssaysFolderName; identifier; ARCtrl.Path.DataMapFileName|]

    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `assays/assayIdentifier/isa.datamap.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct assay identifier</param>
    let tryDatamapFileNameFromIdentifier (identifier: string) : string option =        
        if tryCheckValidCharacters (identifier) then
            ARCtrl.Path.combineMany [|ARCtrl.Path.AssaysFolderName; identifier; ARCtrl.Path.DataMapFileName|]
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
        | _ -> failwith $"Cannot parse identifier from FileName `{fileName}`"

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
        ARCtrl.Path.combineMany [|ARCtrl.Path.StudiesFolderName; identifier; ARCtrl.Path.StudyFileName|]

    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `studies/studyIdentifier/isa.study.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct study identifier</param>
    let tryFileNameFromIdentifier (identifier: string) : string option =
        if tryCheckValidCharacters (identifier) then
            ARCtrl.Path.combineMany [|ARCtrl.Path.StudiesFolderName; identifier; ARCtrl.Path.StudyFileName|]
            |> Some
        else None

    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `studies/studyIdentifier/isa.investigation.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct study identifier</param>
    let datamapFileNameFromIdentifier (identifier: string) : string =
        checkValidCharacters (identifier)
        ARCtrl.Path.combineMany [|ARCtrl.Path.StudiesFolderName; identifier; ARCtrl.Path.DataMapFileName|]

    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `studies/studyIdentifier/isa.investigation.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct study identifier</param>
    let tryDatamapFileNameFromIdentifier (identifier: string) : string option =
        if tryCheckValidCharacters (identifier) then
            ARCtrl.Path.combineMany [|ARCtrl.Path.StudiesFolderName; identifier; ARCtrl.Path.DataMapFileName|]
            |> Some
        else None