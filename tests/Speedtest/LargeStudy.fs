module LargeStudy

open ARCtrl
open ARCtrl.Spreadsheet
open FsSpreadsheet

let createStudy (n:int) =
      
    let RowCount = n
    
    let s = ArcStudy.init("Large Study")
    let table = s.InitTable("Large Table")

    table.AddColumn(CompositeHeader.Input IOType.Source,[|
      for i in 0 .. (RowCount-1) do
        CompositeCell.FreeText $"Input {i}"
    |], SkipFillMissing = true)
    table.AddColumn(CompositeHeader.Output IOType.Sample,[|
      for i in 0 .. (RowCount-1) do
        CompositeCell.FreeText $"Output {i}"
    |], SkipFillMissing = true)
    table.AddColumn(CompositeHeader.Component <| OntologyAnnotation.fromString("instrument model", "MS", "MS:1"),[|
      for _ in 0 .. (RowCount-1) do
        CompositeCell.createTermFromString("SCIEX instrument model", "MS", "MS:2")
    |], SkipFillMissing = true)
    table.AddColumn(CompositeHeader.Factor <| OntologyAnnotation.fromString("temperatures", "UO", "UO:1"),[|
      for i in 0 .. (RowCount-1) do
        let t = i/1000 |> string 
        CompositeCell.createUnitizedFromString(t, "degree Celsius", "UO", "UO:2")
    |], SkipFillMissing = true)
    table.AddColumn(CompositeHeader.ProtocolREF,[|
      for i in 0 .. (RowCount-1) do
        CompositeCell.FreeText "My Awesome Protocol"
    |], SkipFillMissing = true)

    s
    

let toWorkbook (study:ArcStudy) =
    ArcStudy.toFsWorkbook(study)

let fromWorkbook (workbook:FsWorkbook) =
    ArcStudy.fromFsWorkbook(workbook)