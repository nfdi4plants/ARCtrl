module ARCtrl.Spreadsheet.DataMapHeader

open ARCtrl
open ARCtrl.Helper
open FsSpreadsheet

module ActivePattern = 

    open Regex.ActivePatterns

    let (|Term|_|) (categoryString : string) (categoryHeader : CompositeHeader) (cells : FsCell list) : (CompositeHeader*(FsCell list -> CompositeCell)) option =
        let (|AC|_|) s =
            if s = categoryString then Some categoryHeader else None
        let (|TSRColumnHeaderRaw|_|) (s : string) =
            if s.StartsWith("Term Source REF") then Some s else None
        let (|TANColumnHeaderRaw|_|) (s : string) =
            if s.StartsWith("Term Accession Number") then Some s else None
        let cellValues = cells |> List.map (fun c -> c.ValueAsString())
        match cellValues with
        | [AC header] -> 
            (header, CompositeCell.termFromFsCells None None)
            |> Some
        | [AC header; TSRColumnHeaderRaw _; TANColumnHeaderRaw _] ->
            (header, CompositeCell.termFromFsCells (Some 1) (Some 2))
            |> Some
        | [AC header; TANColumnHeaderRaw _; TSRColumnHeaderRaw _] ->
            (header, CompositeCell.termFromFsCells (Some 2) (Some 1))
            |> Some
        | _ -> None

    let (|Explication|_|) (cells : FsCell list) =
        match cells with
        | Term DataMapAux.explicationShortHand DataMapAux.explicationHeader r ->
            Some r
        | _ -> None

    let (|Unit|_|) (cells : FsCell list) =
        match cells with
        | Term DataMapAux.unitShortHand DataMapAux.unitHeader r ->
            Some r
        | _ -> None

    let (|ObjectType|_|) (cells : FsCell list) =
        match cells with
        | Term DataMapAux.objectTypeShortHand DataMapAux.objectTypeHeader r ->
            Some r
        | _ -> None

    let (|Description|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.ValueAsString())
        match cellValues with
        | [DataMapAux.descriptionShortHand] -> Some(DataMapAux.descriptionHeader, CompositeCell.freeTextFromFsCells)
        | _ -> None

    let (|GeneratedBy|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.ValueAsString())
        match cellValues with
        | [DataMapAux.generatedByShortHand] -> Some(DataMapAux.generatedByHeader, CompositeCell.freeTextFromFsCells)
        | _ -> None

    let (|Data|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.ValueAsString())
        match cellValues with
        | DataMapAux.dataShortHand :: cols -> 
            
            let format = cols |> List.tryFindIndex (fun s -> s.StartsWith("Data Format"))  |> Option.map ((+) 1)
            let selectorFormat = cols |> List.tryFindIndex (fun s -> s.StartsWith("Data Selector Format"))  |> Option.map ((+) 1)
            (CompositeHeader.Input (IOType.Data), CompositeCell.dataFromFsCells format selectorFormat)
            |> Some

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
    | Data d -> d
    | Explication e -> e
    | Unit u -> u
    | ObjectType ot -> ot
    | Description d -> d
    | GeneratedBy gb -> gb
    | FreeText ft -> ft
    | _ -> failwithf "Could not parse header group %O" cells

let toFsCells (header : CompositeHeader) : list<FsCell> = 
    match header with
    | CompositeHeader.Input IOType.Data ->      
        [
            FsCell("Data")
            FsCell("Data Format")
            FsCell("Data Selector Format")
        ]                 
    | h when h = DataMapAux.explicationHeader -> 
        [
            FsCell(DataMapAux.explicationShortHand)
            FsCell("Term Source REF")
            FsCell("Term Accession Number")
        ]
    | h when h = DataMapAux.unitHeader -> 
        [
            FsCell(DataMapAux.unitShortHand)
            FsCell("Term Source REF")
            FsCell("Term Accession Number")
        ]
    | h when h = DataMapAux.objectTypeHeader -> 
        [
            FsCell(DataMapAux.objectTypeShortHand)
            FsCell("Term Source REF")
            FsCell("Term Accession Number")
        ]
    | CompositeHeader.FreeText text -> 
        [FsCell(text)]
    | _ -> failwithf "Could not parse DataMap header %O." header
