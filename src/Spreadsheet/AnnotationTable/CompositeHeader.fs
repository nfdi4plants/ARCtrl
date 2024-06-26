module ARCtrl.Spreadsheet.CompositeHeader

open ARCtrl
open ARCtrl.Helper
open FsSpreadsheet

module ActivePattern = 

    open Regex.ActivePatterns

    let mergeIDInfo idSpace1 localID1 idSpace2 localID2 =
        if idSpace1 <> idSpace2 then failwithf "TermSourceRef %s and %s do not match" idSpace1 idSpace2
        if localID1 <> localID2 then failwithf "LocalID %s and %s do not match" localID1 localID2
        {|TermSourceRef = idSpace1; TermAccessionNumber = $"{idSpace1}:{localID1}"|}

    let (|Term|_|) (categoryParser : string -> string option) (f : OntologyAnnotation -> CompositeHeader) (cellValues : string []) : (CompositeHeader*(string [] -> CompositeCell)) option =
        let (|AC|_|) s =
            categoryParser s
        match cellValues with
        | [|AC name|] -> 
            let ont = OntologyAnnotation.create(name)
            (f ont, CompositeCell.termFromStringCells None None)
            |> Some
        | [|AC name; TSRColumnHeader term1; TANColumnHeader term2|] ->
            let term = mergeIDInfo term1.IDSpace term1.LocalID term2.IDSpace term2.LocalID
            let ont = OntologyAnnotation.create(name, term.TermSourceRef, term.TermAccessionNumber)
            (f ont, CompositeCell.termFromStringCells (Some 1) (Some 2))
            |> Some
        | [|AC name; TANColumnHeader term2; TSRColumnHeader term1|] ->
            let term = mergeIDInfo term1.IDSpace term1.LocalID term2.IDSpace term2.LocalID
            let ont = OntologyAnnotation.create(name, term.TermSourceRef, term.TermAccessionNumber)
            (f ont, CompositeCell.termFromStringCells (Some 2) (Some 1))
            |> Some
        | [|AC name; UnitColumnHeader _; TSRColumnHeader term1; TANColumnHeader term2|] ->
            let term = mergeIDInfo term1.IDSpace term1.LocalID term2.IDSpace term2.LocalID
            let ont = OntologyAnnotation.create(name, term.TermSourceRef, term.TermAccessionNumber)
            (f ont, CompositeCell.unitizedFromStringCells 1 (Some 2) (Some 3))
            |> Some
        | [|AC name; UnitColumnHeader _; TANColumnHeader term2; TSRColumnHeader term1|] ->
            let term = mergeIDInfo term1.IDSpace term1.LocalID term2.IDSpace term2.LocalID
            let ont = OntologyAnnotation.create(name, term.TermSourceRef, term.TermAccessionNumber)
            (f ont, CompositeCell.unitizedFromStringCells 1 (Some 3) (Some 2))
            |> Some
        | _ -> None

    let (|Parameter|_|) (cellValues : string []) =
        match cellValues with
        | Term Regex.tryParseParameterColumnHeader CompositeHeader.Parameter r ->
            Some r
        | _ -> None

    let (|Factor|_|) (cellValues : string []) =
        match cellValues with
        | Term Regex.tryParseFactorColumnHeader CompositeHeader.Factor r ->
            Some r
        | _ -> None

    let (|Characteristic|_|) (cellValues : string []) =
        match cellValues with
        | Term Regex.tryParseCharacteristicColumnHeader CompositeHeader.Characteristic r ->
            Some r
        | _ -> None
    
    let (|Component|_|) (cellValues : string []) =
        match cellValues with
        | Term Regex.tryParseComponentColumnHeader CompositeHeader.Component r ->
            Some r
        | _ -> None

    let (|Input|_|) (cellValues : string []) =
        if cellValues.Length = 0 then None
        else
            match cellValues.[0] with
            | InputColumnHeader ioType -> 
                let cols = cellValues |> Array.skip 1
                match IOType.ofString ioType with
                | IOType.Data ->
                    let format = cols |> Array.tryFindIndex (fun s -> s.StartsWith("Data Format"))  |> Option.map ((+) 1)
                    let selectorFormat = cols |> Array.tryFindIndex (fun s -> s.StartsWith("Data Selector Format"))  |> Option.map ((+) 1)
                    (CompositeHeader.Input (IOType.Data), CompositeCell.dataFromStringCells format selectorFormat)
                    |> Some
                | ioType ->
                    (CompositeHeader.Input ioType, CompositeCell.freeTextFromStringCells)
                    |> Some
            | _ -> None

    let (|Output|_|) (cellValues : string []) =
        if cellValues.Length = 0 then None
        else
            match cellValues.[0] with
            | OutputColumnHeader ioType -> 
                let cols = cellValues |> Array.skip 1
                match IOType.ofString ioType with
                | IOType.Data ->
                    let format = cols |> Array.tryFindIndex (fun s -> s.StartsWith("Data Format"))  |> Option.map ((+) 1)
                    let selectorFormat = cols |> Array.tryFindIndex (fun s -> s.StartsWith("Data Selector Format"))  |> Option.map ((+) 1)
                    (CompositeHeader.Output (IOType.Data), CompositeCell.dataFromStringCells format selectorFormat)
                    |> Some
                | ioType ->
                    (CompositeHeader.Output ioType, CompositeCell.freeTextFromStringCells)
                    |> Some
            | _ -> None

    let (|Comment|_|) (cellValues : string []) =
        match cellValues with
        | [|Comment key|] -> Some (CompositeHeader.Comment key, CompositeCell.freeTextFromStringCells)
        | _ -> None

    let (|ProtocolType|_|) (cellValues : string []) =
        let parser s = if s = "Protocol Type" then Some s else None
        let header _ = CompositeHeader.ProtocolType
        match cellValues with
        | Term parser header r -> Some r
        | _ -> None

    let (|ProtocolHeader|_|) (cellValues : string []) =
        match cellValues with
        | [|"Protocol REF"|] -> Some (CompositeHeader.ProtocolREF, CompositeCell.freeTextFromStringCells)
        | [|"Protocol Description"|] -> Some (CompositeHeader.ProtocolDescription, CompositeCell.freeTextFromStringCells)
        | [|"Protocol Uri"|] -> Some (CompositeHeader.ProtocolUri, CompositeCell.freeTextFromStringCells)
        | [|"Protocol Version"|] -> Some (CompositeHeader.ProtocolVersion, CompositeCell.freeTextFromStringCells)
        | [|"Performer"|] -> Some (CompositeHeader.Performer, CompositeCell.freeTextFromStringCells)
        | [|"Date"|] -> Some (CompositeHeader.Date, CompositeCell.freeTextFromStringCells)       
        | _ -> None

    let (|FreeText|_|) (cellValues : string []) =
        match cellValues with
        | [|text|] -> 
            (CompositeHeader.FreeText text, CompositeCell.freeTextFromStringCells)
            |> Some
        | _ -> None

open ActivePattern

let fromStringCells (cellValues : string []) : CompositeHeader*(string [] -> CompositeCell) =
    match cellValues with
    | Parameter p -> p
    | Factor f -> f
    | Characteristic c -> c
    | Component c -> c
    | Input i -> i
    | Output o -> o
    | ProtocolType pt -> pt
    | ProtocolHeader ph -> ph 
    | Comment c -> c
    | FreeText ft -> ft
    | _ -> failwithf "Could not parse header group %O" cellValues
  

let toStringCells (hasUnit : bool) (header : CompositeHeader) : string [] = 
    if header.IsDataColumn then
        [|header.ToString(); "Data Format";  "Data Selector Format"|]
    elif header.IsSingleColumn then
        [|header.ToString()|]
    elif header.IsTermColumn then
        [|
            header.ToString()
            if hasUnit then "Unit"
            $"Term Source REF ({header.GetColumnAccessionShort})"
            $"Term Accession Number ({header.GetColumnAccessionShort})"
        |]
    else 
        failwithf "header %O is neither single nor term column" header