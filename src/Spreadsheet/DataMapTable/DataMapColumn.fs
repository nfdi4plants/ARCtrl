module ARCtrl.Spreadsheet.DatamapColumn

open ARCtrl
open ArcTable
open FsSpreadsheet
open DatamapHeader.ActivePattern

let setFromFsColumns (dc : ResizeArray<DataContext>) (columns : list<FsColumn>) : ResizeArray<DataContext> =
    let cellParser = 
        columns
        |> List.map (fun c -> c.[1])
        |> DatamapHeader.fromFsCells
    for i = 0 to dc.Count - 1 do
        columns
        |> List.map (fun c -> c.[i+2])
        |> cellParser (dc.[i])
        |> ignore
    dc

let toFsColumns (dc : ResizeArray<DataContext>) : FsCell list list =
    let commentKeys = 
        dc 
        |> Seq.collect (fun dc -> dc.Comments |> Seq.map (fun c -> c.Name |> Option.defaultValue ""))
        |> Seq.distinct
        |> Seq.toList
    let headers = 
        DatamapHeader.toFsCells commentKeys
    let createTerm (oa : OntologyAnnotation option) = 
        match oa with
        | Some oa ->
            [
                oa.Name |> Option.defaultValue "" |> FsCell
                oa.TermSourceREF |> Option.defaultValue "" |> FsCell
                oa.TermAccessionNumber |> Option.defaultValue "" |> FsCell
            ]
        | None ->
            [
            FsCell("")
            FsCell("")
            FsCell("")
        ]
    let createText (s : string option) = 
        [
            FsCell(s |> Option.defaultValue "")    
        ]
    let createData (dc : DataContext) =
        [
            FsCell(dc.Name |> Option.defaultValue "")
            FsCell(dc.Format |> Option.defaultValue "")
            FsCell(dc.SelectorFormat |> Option.defaultValue "")        
        ]
    let createRow (dc : DataContext) = 
        [
            yield! (createData dc)
            yield! (createTerm dc.Explication)
            yield! (createTerm dc.Unit)
            yield! (createTerm dc.ObjectType)
            yield! (createText dc.Description)
            yield! (createText dc.GeneratedBy)
            yield! (createText dc.Label)
            yield! (
                commentKeys
                |> List.map (fun key -> 
                    dc.Comments 
                    |> Seq.tryFind (fun c -> 
                        Option.defaultValue "" c.Name = key) 
                    |> Option.bind (fun c -> c.Value)
                    |> Option.defaultValue ""
                    |> FsCell
                )
            )  
        ]
    [
        headers
        for dc in dc do
            createRow dc
    ]
    |> List.transpose