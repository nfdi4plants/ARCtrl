module ISA.Spreadsheet.Identifier

let [<Literal>] MISSING_IDENTIFIER = "MISSING_IDENTIFIER_"

let createMissingIdentifier() =
    MISSING_IDENTIFIER + System.Guid.NewGuid().ToString()

let removeMissingIdentifier (str: string) =
    if str.StartsWith(MISSING_IDENTIFIER) then "" else str

/// Assay only contains "FileName" in isa.assay.xlsx. To unify naming in our model, on read-in we transform fileName to identifier and reverse for writing.
[<RequireQualifiedAccess>]
module Assay =
    
    open System.Text.RegularExpressions

    /// <summary>
    /// On read-in the FileName can be any combination of "assays" (assay folder name), assayIdentifier and "isa.assay.xlsx" (the actual file name).
    ///
    /// This functions extracts assayIdentifier from any of these combinations and returns it.
    /// </summary>
    /// <param name="fileName">FileName as written in isa.assay.xlsx metadata sheet</param>
    let identifierFromFileName (fileName: string) : string =
        let regex = Regex(ISA.Identifier.ValidFileNamePattern)
        let m = regex.Match(fileName)
        match m.Success with
        | false -> failwith $"Cannot parse identifier from FileName `{fileName}`"
        | true ->
            let identifier = m.Groups.["identifier"].Value
            identifier

    /// <summary>
    /// On writing a xlsx file we unify our output to a relative path to ARC root. So: `assays/assayIdentifier/isa.assay.xlsx`.
    /// </summary>
    /// <param name="identifier">Any correct assay identifier</param>
    let fileNameFromIdentifier (identifier: string) : string =
        ISA.Identifier.checkValidCharacters (identifier)
        FileSystem.Path.combineMany [|ARCtrl.Path.AssaysFolderName; identifier; ARCtrl.Path.AssayFileName|]