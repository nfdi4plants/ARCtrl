module ARCtrl.Helper.ORCID

open ARCtrl.Helper.Regex.ActivePatterns

[<Literal>]
let orcidPattern = @"[0-9]{4}-[0-9]{4}-[0-9]{4}-[0-9]{3}[0-9X]"

let orcidRegex = System.Text.RegularExpressions.Regex(orcidPattern)

let tryGetOrcidNumber (orcid : string) =
    let m = orcidRegex.Match(orcid)
    if m.Success then
        Some m.Value
    else
        None

let orcidPrefix = "http://orcid.org/"

let (|ORCID|_|) input = 
    match input with
    | Regex orcidPattern r -> Some r.Value
    | _ -> None

let tryGetOrcidURL (orcid : string) =
    match orcid with
    | ORCID orcid -> Some $"{orcidPrefix}{orcid}"
    | _ -> None