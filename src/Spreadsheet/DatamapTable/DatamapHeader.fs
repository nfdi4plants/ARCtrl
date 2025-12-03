module ARCtrl.Spreadsheet.DatamapHeader

open ARCtrl
open ARCtrl.Helper
open FsSpreadsheet

module ActivePattern = 

    open Regex.ActivePatterns

    let ontologyAnnotationFromFsCells (tsrCol : int option) (tanCol : int option) (cells : list<FsCell>) : OntologyAnnotation =
        let cellValues = cells |> List.map (fun c -> c.ValueAsString())
        let tsr = Option.map (fun i -> cellValues.[i]) tsrCol
        let tan = Option.map (fun i -> cellValues.[i]) tanCol
        OntologyAnnotation(cellValues.[0],?tsr = tsr, ?tan = tan)

    let freeTextFromFsCells (cells : list<FsCell>) : string =
        let cellValues = cells |> List.map (fun c -> c.ValueAsString())
        cellValues.[0]

    let dataFromFsCells (format : int option) (selectorFormat : int option) (cells : list<FsCell>) : Data =
        let cellValues = cells |> List.map (fun c -> c.ValueAsString())
        let format = Option.map (fun i -> cellValues.[i]) format
        let selectorFormat = Option.map (fun i -> cellValues.[i]) selectorFormat
        Data(name = cellValues.[0],?format = format, ?selectorFormat = selectorFormat)

    let (|Term|_|) (categoryString : string) (cells : FsCell list) : ((FsCell list -> OntologyAnnotation)) option =
        let (|AC|_|) s =
            if s = categoryString then Some 1 else None
        let (|TSRColumnHeaderRaw|_|) (s : string) =
            if s.StartsWith("Term Source REF") then Some s else None
        let (|TANColumnHeaderRaw|_|) (s : string) =
            if s.StartsWith("Term Accession Number") then Some s else None
        let cellValues = cells |> List.map (fun c -> c.ValueAsString())
        match cellValues with
        | [AC header] -> 
            (ontologyAnnotationFromFsCells None None)
            |> Some
        | [AC header; TSRColumnHeaderRaw _; TANColumnHeaderRaw _] ->
            (ontologyAnnotationFromFsCells (Some 1) (Some 2))
            |> Some
        | [AC header; TANColumnHeaderRaw _; TSRColumnHeaderRaw _] ->
            (ontologyAnnotationFromFsCells (Some 2) (Some 1))
            |> Some
        | _ -> None

    let (|Explication|_|) (cells : FsCell list) =
        match cells with
        | Term DatamapAux.explicationShortHand r ->
            (fun (dc : DataContext) (cells : FsCell list) -> 
                dc.Explication <- Some (r cells)
                dc
            )
            |> Some
        | _ -> None

    let (|Unit|_|) (cells : FsCell list) =
        match cells with
        | Term DatamapAux.unitShortHand r ->
            (fun (dc : DataContext) (cells : FsCell list) -> 
                dc.Unit <- Some (r cells)
                dc
            )
            |> Some
        | _ -> None

    let (|ObjectType|_|) (cells : FsCell list) =
        match cells with
        | Term DatamapAux.objectTypeShortHand r ->
            (fun (dc : DataContext) (cells : FsCell list) -> 
                dc.ObjectType <- Some (r cells)
                dc
            )
            |> Some
        | _ -> None

    let (|Description|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.ValueAsString())
        match cellValues with
        | [DatamapAux.descriptionShortHand] -> 
            (fun (dc : DataContext) (cells : FsCell list) -> 
                dc.Description <- freeTextFromFsCells cells |> Option.fromValueWithDefault ""
                dc
            )
            |> Some
        | _ -> None

    let (|GeneratedBy|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.ValueAsString())
        match cellValues with
        | [DatamapAux.generatedByShortHand] -> 
            (fun (dc : DataContext) (cells : FsCell list) -> 
                dc.GeneratedBy <- freeTextFromFsCells cells |> Option.fromValueWithDefault ""
                dc
            )
            |> Some
        | _ -> None

    let (|Label|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.ValueAsString())
        match cellValues with
        | [DatamapAux.labelShortHand] -> 
            (fun (dc : DataContext) (cells : FsCell list) -> 
                dc.Label <- freeTextFromFsCells cells |> Option.fromValueWithDefault ""
                dc
            )
            |> Some
        | _ -> None

    let (|Data|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.ValueAsString())
        match cellValues with
        | DatamapAux.dataShortHand :: cols -> 
            (fun (dc : DataContext) (cells : FsCell list) ->
                let format = cols |> List.tryFindIndex (fun s -> s.StartsWith("Data Format"))  |> Option.map ((+) 1)
                let selectorFormat = cols |> List.tryFindIndex (fun s -> s.StartsWith("Data Selector Format"))  |> Option.map ((+) 1)
                let d = dataFromFsCells format selectorFormat cells
                dc.FilePath <- d.FilePath
                dc.Selector <- d.Selector
                dc.Format <- d.Format
                dc.SelectorFormat <- d.SelectorFormat
                dc
            )
            |> Some
               
        | _ -> None

    let (|Comment|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.ValueAsString())
        match cellValues with
        | [Comment key] -> 
            (fun (dc : DataContext) (cells : FsCell list) -> 
                let cellValues = cells |> List.map (fun c -> c.ValueAsString())
                let comment = cellValues.[0]
                dc.Comments.Add(Comment.create(key,comment))
                dc
            )
            |> Some   
        | _ -> None

    let (|Freetext|_|) (cells : FsCell list) =
        let cellValues = cells |> List.map (fun c -> c.ValueAsString())
        match cellValues with
        | [key] -> 
            (fun (dc : DataContext) (cells : FsCell list) -> 
                let cellValues = cells |> List.map (fun c -> c.ValueAsString())
                let comment = cellValues.[0]
                dc.Comments.Add(Comment.create(key,comment))
                dc
            )
            |> Some   
        | _ -> None

open ActivePattern

let fromFsCells (cells : FsCell list) : DataContext -> FsCell list -> DataContext =
    match cells with
    | Explication r 
    | Unit r 
    | ObjectType r 
    | Description r 
    | GeneratedBy r
    | Label r
    | Data r 
    | Comment r 
    | Freetext r -> 
        fun (dc : DataContext) (cells : FsCell list) -> r dc cells
    | _ -> failwithf "Could not parse data map column: %s" (cells |> List.map (fun c -> c.ValueAsString()) |> String.concat ", ")

let toFsCells (commentKeys : string list) : list<FsCell> = 
    [
        yield! [
            FsCell("Data")
            FsCell("Data Format")
            FsCell("Data Selector Format")
        ]  
        yield! [
            FsCell(DatamapAux.explicationShortHand)
            FsCell("Term Source REF")
            FsCell("Term Accession Number")
        ]
        yield![
            FsCell(DatamapAux.unitShortHand)
            FsCell("Term Source REF")
            FsCell("Term Accession Number")
        ]
        yield![
            FsCell(DatamapAux.objectTypeShortHand)
            FsCell("Term Source REF")
            FsCell("Term Accession Number")
        ]
        yield! [
            FsCell(DatamapAux.descriptionShortHand)
        ]
        yield! [
            FsCell(DatamapAux.generatedByShortHand)
        ]
        yield! [
            FsCell(DatamapAux.labelShortHand)
        ]
        for ck in commentKeys do
            yield FsCell($"Comment [{ck}]")
    ]
