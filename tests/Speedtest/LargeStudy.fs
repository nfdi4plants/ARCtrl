module LargeStudy

open ARCtrl
open ARCtrl.Spreadsheet
open FsSpreadsheet

let createStudy (n:int) =
      
    let RowCount = n
    
    let s = ArcStudy.init("Large Study")
    let table = s.InitTable("Large Table")

    table.AddColumn(CompositeHeader.Input IOType.Source,ResizeArray [|
      for i in 0 .. (RowCount-1) do
        CompositeCell.FreeText $"Input {i}"
    |], skipFillMissing = true)
    table.AddColumn(CompositeHeader.Output IOType.Sample,ResizeArray [|
      for i in 0 .. (RowCount-1) do
        CompositeCell.FreeText $"Output {i}"
    |], skipFillMissing = true)
    table.AddColumn(CompositeHeader.Component <| OntologyAnnotation("instrument model", "MS", "MS:1"),ResizeArray [|
      for _ in 0 .. (RowCount-1) do
        CompositeCell.createTermFromString("SCIEX instrument model", "MS", "MS:2")
    |], skipFillMissing = true)
    table.AddColumn(CompositeHeader.Factor <| OntologyAnnotation("temperatures", "UO", "UO:1"),ResizeArray [|
      for i in 0 .. (RowCount-1) do
        let t = i/1000 |> string 
        CompositeCell.createUnitizedFromString(t, "degree Celsius", "UO", "UO:2")
    |], skipFillMissing = true)
    table.AddColumn(CompositeHeader.ProtocolREF,ResizeArray [|
      for i in 0 .. (RowCount-1) do
        CompositeCell.FreeText "My Awesome Protocol"
    |], skipFillMissing = true)

    s
    

let toWorkbook (study:ArcStudy) =
    ArcStudy.toFsWorkbook(study)

let fromWorkbook (workbook:FsWorkbook) =
    ArcStudy.fromFsWorkbook(workbook)