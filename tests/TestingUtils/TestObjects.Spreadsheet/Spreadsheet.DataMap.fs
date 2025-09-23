module TestObjects.Spreadsheet.DataMap

open ARCtrl
open FsSpreadsheet
open ARCtrl.Spreadsheet

type FsTable with
    member this.IsEmpty(c : FsCellsCollection) =
        this.RangeAddress.Range = "A1:A1"
        && 
            (this.Cell(FsAddress.fromString("A1"),c).Value = null)
            ||
            (this.Cell(FsAddress.fromString("A1"),c).Value = "")

module Data = 

    let dataValue = 
        Data.create(Name = "MyDataFile.csv#col=1",Format = "text/csv", SelectorFormat = "https://datatracker.ietf.org/doc/html/rfc7111")

    let dataHeaderV1 = "Data"
    let dataHeaderV2 = "Data Format"
    let dataHeaderV3 = "Data Selector Format"

    let dataValueV1 = "MyDataFile.csv#col=1"
    let dataValueV2 = "text/csv"
    let dataValueV3 = "https://datatracker.ietf.org/doc/html/rfc7111"

    let appendDataColumn l (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs dataHeaderV1
        t.Cell(FsAddress(1, colCount + 2),c).SetValueAs dataHeaderV2
        t.Cell(FsAddress(1, colCount + 3),c).SetValueAs dataHeaderV3
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs dataValueV1
            t.Cell(FsAddress(i, colCount + 2),c).SetValueAs dataValueV2 
            t.Cell(FsAddress(i, colCount + 3),c).SetValueAs dataValueV3

module Explication =
    
    let pValueValue = 
        OntologyAnnotation("p-value","NCIT","http://purl.obolibrary.org/obo/NCIT_C44185")

    let explicationHeaderV1 = "Explication"
    let explicationHeaderV2 = "Term Source REF"
    let explicationHeaderV3 = "Term Accession Number"

    let pValueValueV1 = "p-value"
    let pValueValueV2 = "NCIT"
    let pValueValueV3 = "http://purl.obolibrary.org/obo/NCIT_C44185"
        
    let appendPValueColumn l (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs explicationHeaderV1
        t.Cell(FsAddress(1, colCount + 2),c).SetValueAs explicationHeaderV2
        t.Cell(FsAddress(1, colCount + 3),c).SetValueAs explicationHeaderV3
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs pValueValueV1
            t.Cell(FsAddress(i, colCount + 2),c).SetValueAs pValueValueV2 
            t.Cell(FsAddress(i, colCount + 3),c).SetValueAs pValueValueV3

    // Arithmetic Mean, http://purl.obolibrary.org/obo/NCIT_C53319
    let meanValue = 
        OntologyAnnotation("Arithmetic Mean","NCIT","http://purl.obolibrary.org/obo/NCIT_C53319")

    let meanValueV1 = "Arithmetic Mean"
    let meanValueV2 = "NCIT"
    let meanValueV3 = "http://purl.obolibrary.org/obo/NCIT_C53319"

    let appendMeanColumn l (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs explicationHeaderV1
        t.Cell(FsAddress(1, colCount + 2),c).SetValueAs explicationHeaderV2
        t.Cell(FsAddress(1, colCount + 3),c).SetValueAs explicationHeaderV3
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs meanValueV1
            t.Cell(FsAddress(i, colCount + 2),c).SetValueAs meanValueV2 
            t.Cell(FsAddress(i, colCount + 3),c).SetValueAs meanValueV3     

module Unit = 
    
    let ppmValue = 
        OntologyAnnotation("parts per million","UO","http://purl.obolibrary.org/obo/UO_0000169")

    let unitHeaderV1 = "Unit"
    let unitHeaderV2 = "Term Source REF"
    let unitHeaderV3 = "Term Accession Number"

    let ppmValueV1 = "parts per million"
    let ppmValueV2 = "UO"
    let ppmValueV3 = "http://purl.obolibrary.org/obo/UO_0000169"

    let appendPPMColumn l (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs unitHeaderV1
        t.Cell(FsAddress(1, colCount + 2),c).SetValueAs unitHeaderV2
        t.Cell(FsAddress(1, colCount + 3),c).SetValueAs unitHeaderV3
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs ppmValueV1
            t.Cell(FsAddress(i, colCount + 2),c).SetValueAs ppmValueV2 
            t.Cell(FsAddress(i, colCount + 3),c).SetValueAs ppmValueV3

module ObjectType =

    let floatValue =
        OntologyAnnotation("float","NCIT","http://purl.obolibrary.org/obo/NCIT_C42645")

    let objectTypeHeaderV1 = "Object Type"
    let objectTypeHeaderV2 = "Term Source REF"
    let objectTypeHeaderV3 = "Term Accession Number"

    let floatValueV1 = "float"
    let floatValueV2 = "NCIT"
    let floatValueV3 = "http://purl.obolibrary.org/obo/NCIT_C42645"
    
    let appendFloatColumn l (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs objectTypeHeaderV1
        t.Cell(FsAddress(1, colCount + 2),c).SetValueAs objectTypeHeaderV2
        t.Cell(FsAddress(1, colCount + 3),c).SetValueAs objectTypeHeaderV3
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs floatValueV1
            t.Cell(FsAddress(i, colCount + 2),c).SetValueAs floatValueV2 
            t.Cell(FsAddress(i, colCount + 3),c).SetValueAs floatValueV3

module Description =
    
    let descriptionValue = "This is a description"

    let descriptionHeaderV1 = "Description"

    let descriptionValueV1 = "This is a description"

    let appendDescriptionColumn l (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs descriptionHeaderV1
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs descriptionValueV1

module GeneratedBy =
    
    let generatedByValue = "MyTool.exe"

    let generatedByHeaderV1 = "Generated By"

    let generatedByValueV1 = "MyTool.exe"

    let appendGeneratedByColumn l (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs generatedByHeaderV1
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs generatedByValueV1

module Label =
    
    let labelValue = "avg"

    let labelHeaderV1 = "Label"

    let labelValueV1 = "avg"

    let appendLabelColumn l (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs labelHeaderV1
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs labelValueV1

module Comment =

    let commentHeader = "Kommend"

    let commentValue = "This is a comment"

    let commentHeaderV1 = "Comment [Kommend]"

    let commentValueV1 = "This is a comment"

    let appendCommentColumn l (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs commentHeaderV1
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs commentValueV1

let initWorksheet (name : string) (appendOperations : (FsCellsCollection -> FsTable -> unit) list) = 
    let w = FsWorksheet(name)
    let t  = w.Table(DataMapTable.datamapTablePrefix, FsRangeAddress.fromString("A1:A1"))
    appendOperations 
    |> List.iter (fun o -> o w.CellCollection t)
    ArcTable.addSpacesToEnd w.CellCollection t
    t.RescanFieldNames(w.CellCollection)
    w