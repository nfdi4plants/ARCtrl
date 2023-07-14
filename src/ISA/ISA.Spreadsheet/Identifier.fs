module ISA.Spreadsheet.Identifier

let [<Literal>] MISSING_IDENTIFIER = "MISSING_IDENTIFIER_"

let createMissingIdentifier() =
    MISSING_IDENTIFIER + System.Guid.NewGuid().ToString()

let removeMissingIdentifier (str: string) =
    if str.StartsWith(MISSING_IDENTIFIER) then "" else str