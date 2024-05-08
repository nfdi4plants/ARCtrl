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

    let (|Term|_|) (categoryParser : string -> string option) (f : OntologyAnnotation -> CompositeHeader) (cells : FsCell list) : (CompositeHeader*(FsCell list -> CompositeCell)) option =
        let (|AC|_|) s =
            categoryParser s
        let cellValues = cells |> List.map (fun c -> c.ValueAsString())
        match cellValues with
        | [AC name] -> 
            let ont = OntologyAnnotation.create(name)
            (f ont, CompositeCell.termFromFsCells None None)
            |> Some
        | [AC name; TSRColumnHeader term1; TANColumnHeader term2] ->
            let term = mergeIDInfo term1.IDSpace term1.LocalID term2.IDSpace term2.LocalID
            let ont = OntologyAnnotation.create(name, term.TermSourceRef, term.TermAccessionNumber)
            (f ont, CompositeCell.termFromFsCells (Some 1) (Some 2))
            |> Some
        | [AC name; TANColumnHeader term2; TSRColumnHeader term1] ->
            let term = mergeIDInfo term1.IDSpace term1.LocalID term2.IDSpace term2.LocalID
            let ont = OntologyAnnotation.create(name, term.TermSourceRef, term.TermAccessionNumber)
            (f ont, CompositeCell.termFromFsCells (Some 2) (Some 1))
            |> Some
        | [AC name; UnitColumnHeader _; TSRColumnHeader term1; TANColumnHeader term2] ->
            let term = mergeIDInfo term1.IDSpace term1.LocalID term2.IDSpace term2.LocalID
            let ont = OntologyAnnotation.create(name, term.TermSourceRef, term.TermAccessionNumber)
            (f ont, CompositeCell.unitizedFromFsCells 1 (Some 2) (Some 3))
            |> Some
        | [AC name; UnitColumnHeader _; TANColumnHeader term2; TSRColumnHeader term1] ->
            let term = mergeIDInfo term1.IDSpace term1.LocalID term2.IDSpace term2.LocalID
            let ont = OntologyAnnotation.create(name, term.TermSourceRef, term.TermAccessionNumber)
            (f ont, CompositeCell.unitizedFromFsCells 1 (Some 3) (Some 2))
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
    
    let (|Component|_|) (cells : FsCell list) =
        match cells with
        | Term Regex.tryParseComponentColumnHeader CompositeHeader.Component r ->
            Some r
        | _ -> None

    let (|Input|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.ValueAsString())
        match cellValues with
        | InputColumnHeader ioType :: cols -> 
            match IOType.ofString ioType with
            | IOType.Data ->
                let format = List.tryFindIndex ((=) "Data Format") cols
                let selectorFormat = List.tryFindIndex ((=) "Data Selector Format") cols
                (CompositeHeader.Input (IOType.Data), CompositeCell.dataFromFsCells format selectorFormat)
                |> Some
            | ioType ->
                (CompositeHeader.Input ioType, CompositeCell.freeTextFromFsCells)
                |> Some
        | _ -> None

    let (|Output|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.ValueAsString())
        match cellValues with
        | OutputColumnHeader ioType :: cols -> 
            match IOType.ofString ioType with
            | IOType.Data ->
                let format = List.tryFindIndex ((=) "Data Format") cols
                let selectorFormat = List.tryFindIndex ((=) "Data Selector Format") cols
                (CompositeHeader.Output (IOType.Data), CompositeCell.dataFromFsCells format selectorFormat)
                |> Some
            | ioType ->
                (CompositeHeader.Output ioType, CompositeCell.freeTextFromFsCells)
                |> Some
        | _ -> None

    let (|ProtocolType|_|) (cells : FsCell list) =
        let parser s = if s = "Protocol Type" then Some s else None
        let header _ = CompositeHeader.ProtocolType
        match cells with
        | Term parser header r -> Some r
        | _ -> None

    let (|ProtocolHeader|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.ValueAsString())
        match cellValues with
        | ["Protocol REF"] -> Some (CompositeHeader.ProtocolREF, CompositeCell.freeTextFromFsCells)
        | ["Protocol Description"] -> Some (CompositeHeader.ProtocolDescription, CompositeCell.freeTextFromFsCells)
        | ["Protocol Uri"] -> Some (CompositeHeader.ProtocolUri, CompositeCell.freeTextFromFsCells)
        | ["Protocol Version"] -> Some (CompositeHeader.ProtocolVersion, CompositeCell.freeTextFromFsCells)
        | ["Performer"] -> Some (CompositeHeader.Performer, CompositeCell.freeTextFromFsCells)
        | ["Date"] -> Some (CompositeHeader.Date, CompositeCell.freeTextFromFsCells)       
        | _ -> None

    let (|FreeText|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.ValueAsString())
        match cellValues with
        | [text] -> 
            (CompositeHeader.FreeText text, CompositeCell.freeTextFromFsCells)
            |> Some
        | _ -> None

open ActivePattern

let fromFsCells (cells : list<FsCell>) : CompositeHeader*(FsCell list -> CompositeCell) =
    match cells with
    | Parameter p -> p
    | Factor f -> f
    | Characteristic c -> c
    | Component c -> c
    | Input i -> i
    | Output o -> o
    | ProtocolType pt -> pt
    | ProtocolHeader ph -> ph 
    | FreeText ft -> ft
    | _ -> failwithf "Could not parse header group %O" cells
  

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