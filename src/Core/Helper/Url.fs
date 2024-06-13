module ARCtrl.Helper.Url

open System.Collections.Generic

[<LiteralAttribute>]
let OntobeeOboPurl = @"http://purl.obolibrary.org/obo/"

let OntobeeParser (tsr : string) (localTan : string) =
    $"{OntobeeOboPurl}{tsr}_{localTan}"

[<LiteralAttribute>]
let BioregistryUrl = @"https://bioregistry.io/"

let BioregistryParser (tsr : string) (localTan : string) =
    $"{BioregistryUrl}{tsr}:{localTan}"

[<LiteralAttribute>]
let OntobeeDPBOUrl = @"http://purl.org/nfdi4plants/ontology/dpbo/"

let OntobeeDPBOParser (tsr : string) (localTan : string) =
    $"{OntobeeDPBOUrl}{tsr}_{localTan}"

[<LiteralAttribute>]
let MSUrl = @"https://www.ebi.ac.uk/ols4/ontologies/ms/classes/http%253A%252F%252Fpurl.obolibrary.org%252Fobo%252F"

let MSParser (tsr : string) (localTan : string) =
    $"{MSUrl}{tsr}_{localTan}"

[<LiteralAttribute>]
let POUrl = @"https://www.ebi.ac.uk/ols4/ontologies/po/classes/http%253A%252F%252Fpurl.obolibrary.org%252Fobo%252F"

let POParser (tsr : string) (localTan : string) =
    $"{POUrl}{tsr}_{localTan}"

[<LiteralAttribute>]
let ROUrl = @"https://www.ebi.ac.uk/ols4/ontologies/ro/classes/http%253A%252F%252Fpurl.obolibrary.org%252Fobo%252F"

let ROParser (tsr : string) (localTan : string) =
    $"{ROUrl}{tsr}_{localTan}"


/// ENVO, PSI-MS (Prefix MS), CHEBI, GO, OBI, PATO, PECO, PO (purls broken), RO (purls broken), TO, UO, PSI-MOD (prefix MOD), EFO, NCIT, OMP should match to bioregistry parser
let uriParserCollection = 
    [
        "DPBO", OntobeeDPBOParser
        "MS", MSParser
        "PO", POParser
        "RO", ROParser
        "ENVO", BioregistryParser
        "CHEBI", BioregistryParser
        "GO", BioregistryParser
        "OBI", BioregistryParser
        "PATO", BioregistryParser
        "PECO", BioregistryParser
        "TO", BioregistryParser
        "UO", BioregistryParser
        "EFO", BioregistryParser
        "NCIT", BioregistryParser
        "OMP", BioregistryParser
    ]
    |> Dictionary.ofSeq


let createOAUri (tsr : string) (localTan : string) =
    match uriParserCollection.TryGetValue tsr with
    | true, parser -> parser tsr localTan
    | false, _ -> OntobeeParser tsr localTan