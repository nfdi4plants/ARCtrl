module ISA.Spreadsheet.CompositeHeader

open ISA
open FsSpreadsheet

module ActivePattern = 

    open ISA.Regex.ActivePatterns

    let mergeTerms tsr1 tan1 tsr2 tan2 =
        if tsr1 <> tsr2 then failwithf "TermSourceRef %s and %s do not match" tsr1 tsr2
        if tan1 <> tan2 then failwithf "TermAccessionNumber %s and %s do not match" tan1 tan2
        {|TermSourceRef = tsr1; TermAccessionNumber = tan1|}

    let (|Term|_|) (categoryParser : string -> string option) (f : OntologyAnnotation -> CompositeHeader) (cells : FsCell list) =
        let (|AC|_|) s =
            categoryParser s
        let cellValues = cells |> List.map (fun c -> c.Value)
        match cellValues with
        | [AC name] -> 
            let ont = OntologyAnnotation.fromString(name)
            f ont
            |> Some
        | [AC name; TSRColumnHeader term1; TANColumnHeader term2] 
        //| [AC name; TermAccessionNumber term1; TermSourceREF term2] 
        //| [AC name; Unit; TermAccessionNumber term1; TermSourceREF term2] 
        | [AC name; UnitColumnHeader; TSRColumnHeader term1; TANColumnHeader term2] ->
            let term = mergeTerms term1.TermSourceREF term1.TermSourceREF term2.LocalTAN term2.LocalTAN
            let ont = OntologyAnnotation.fromString(name, term.TermSourceRef, term.TermAccessionNumber)
            f ont
            |> Some
        | _ -> None

    let (|Parameter|_|) (cells : FsCell list) =
        match cells with
        | Term Regex.tryParseParameterColumnHeader CompositeHeader.Parameter r ->
            Some r
        | _ -> None

    let (|Factor|_|) (cells : FsCell list) =
        match cells with
        | Term Regex.tryParseFactorColumnHeader CompositeHeader.Factor r ->
            Some r
        | _ -> None

    let (|Characteristic|_|) (cells : FsCell list) =
        match cells with
        | Term Regex.tryParseCharacteristicColumnHeader CompositeHeader.Characteristic r ->
            Some r
        | _ -> None
    
    let (|Input|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.Value)
        match cellValues with
        | [InputColumnHeader ioType] -> 
            IOType.ofString ioType
            |> CompositeHeader.Input
            |> Some
        | _ -> None

    let (|Output|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.Value)
        match cellValues with
        | [OutputColumnHeader ioType] -> 
            IOType.ofString ioType
            |> CompositeHeader.Output
            |> Some
        | _ -> None

    let (|FreeText|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.Value)
        match cellValues with
        | [text] -> 
            CompositeHeader.FreeText text
            |> Some
        | _ -> None

open ActivePattern

let fromFsCells (cells : list<FsCell>) : CompositeHeader =
    match cells with
    | Parameter p -> p
    | Factor f -> f
    | Characteristic c -> c
    | Input i -> i
    | Output o -> o
    | FreeText ft -> ft
    | _ -> raise (System.NotImplementedException("parseCompositeHeader"))

let toFsCells (hasUnit : bool) (header : CompositeHeader) : list<FsCell> = 
    if header.IsSingleColumn then
        [FsCell(header.ToString())]
    elif header.IsTermColumn then
        [
            FsCell(header.ToString())
            if hasUnit then FsCell("Unit")
            FsCell($"Term Source REF ({header.GetColumnAccessionShort})")
            FsCell($"Term Accession Number ({header.GetColumnAccessionShort})")
        ]
    else 
        failwithf "header %O is neither single nor term column" header