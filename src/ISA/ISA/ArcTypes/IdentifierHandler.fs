/// This module contains helper functions to handle study/assay/investigation identifiers in an unsafe, forced way.
module ISA.IdentifierHandler 

open System.Text.RegularExpressions

/// This pattern should never be used as standalone pattern!
let [<Literal>] internal InnerValidCharactersPattern = @"[a-zA-Z0-9_\- ]+"

// Define a regular expression pattern for valid characters
let [<Literal>] ValidIdentifierPattern = @"^" + InnerValidCharactersPattern + @"$"

// Define a regular expression pattern for valid characters
let [<Literal>] ValidFileNamePattern = @"^(assays(\/|\\))?(?<identifier>" + InnerValidCharactersPattern + @")((\/|\\)isa.assay.xlsx)?$"

// Function to check if a string contains only valid characters
let checkValidCharacters (identifier: string) =
    let regex = new Regex(ValidIdentifierPattern)
    let isValid = regex.IsMatch(identifier)
    if not isValid then failwith "New identifier contains forbidden characters! Allowed characters are: letters, digits, underscore (_), dash (-) and whitespace ( )."

let setAssayIdentifier (newIdentifier: string) (assay: ArcAssay) =
    checkValidCharacters newIdentifier
    assay.Identifier <- newIdentifier
    assay

let setStudyIdentifier (newIdentifier: string) (study: ArcStudy) =
    checkValidCharacters newIdentifier
    study.Identifier <- newIdentifier
    study

let setInvestigationIdentifier (newIdentifier: string) (investigation: ArcInvestigation) =
    checkValidCharacters newIdentifier
    investigation.Identifier <- newIdentifier
    investigation



