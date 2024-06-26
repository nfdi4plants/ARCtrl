module TestObjects.Spreadsheet.ArcTable

open FsSpreadsheet
open ARCtrl
open ARCtrl.Spreadsheet

type FsTable with
    member this.IsEmpty(c : FsCellsCollection) =
        this.RangeAddress.Range = "A1:A1"
        && 
            (this.Cell(FsAddress("A1"),c).Value = null)
            ||
            (this.Cell(FsAddress("A1"),c).Value = "")

module Parameter = 

    let temperatureHeader = 
        CompositeHeader.Parameter 
            (OntologyAnnotation("temperature","PATO","PATO:0000146"))
    let temperatureValue = 
        CompositeCell.createUnitized
            ("5",
            OntologyAnnotation("degree celsius","UO","UO:0000027"))

    let temperatureValue2 = 
        CompositeCell.createUnitized
            ("10",
            OntologyAnnotation("degree celsius","UO","UO:0000027"))

    let temperatureHeaderV1 = "Parameter [temperature]"
    let temperatureHeaderV2 = "Unit"
    let temperatureHeaderV3 = "Term Source REF (PATO:0000146)"
    let temperatureHeaderV4 = "Term Accession Number (PATO:0000146)"

    let temperatureValueV1 = "5"
    let temperatureValueV2 = "degree celsius"
    let temperatureValueV3 = "UO"
    let temperatureValueV4 = Helper.Url.createOAUri "UO" "0000027"

    let temperatureValue2V1 = "10"
    let temperatureValue2V2 = "degree celsius"
    let temperatureValue2V3 = "UO"
    let temperatureValue2V4 = Helper.Url.createOAUri "UO" "0000027"

    let appendTemperatureColumn l (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs temperatureHeaderV1
        t.Cell(FsAddress(1, colCount + 2),c).SetValueAs temperatureHeaderV2
        t.Cell(FsAddress(1, colCount + 3),c).SetValueAs temperatureHeaderV3
        t.Cell(FsAddress(1, colCount + 4),c).SetValueAs temperatureHeaderV4
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs temperatureValueV1
            t.Cell(FsAddress(i, colCount + 2),c).SetValueAs temperatureValueV2 
            t.Cell(FsAddress(i, colCount + 3),c).SetValueAs temperatureValueV3
            t.Cell(FsAddress(i, colCount + 4),c).SetValueAs temperatureValueV4

    let appendMixedTemperatureColumn l1 l2 (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs temperatureHeaderV1
        t.Cell(FsAddress(1, colCount + 2),c).SetValueAs temperatureHeaderV2
        t.Cell(FsAddress(1, colCount + 3),c).SetValueAs temperatureHeaderV3
        t.Cell(FsAddress(1, colCount + 4),c).SetValueAs temperatureHeaderV4
        for i = 2 to l1 + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs temperatureValueV1
            t.Cell(FsAddress(i, colCount + 2),c).SetValueAs temperatureValueV2 
            t.Cell(FsAddress(i, colCount + 3),c).SetValueAs temperatureValueV3
            t.Cell(FsAddress(i, colCount + 4),c).SetValueAs temperatureValueV4
        for j = 2 + l1 to l1 + l2 + 1 do
            t.Cell(FsAddress(j, colCount + 1),c).SetValueAs temperatureValue2V1
            t.Cell(FsAddress(j, colCount + 2),c).SetValueAs temperatureValue2V2 
            t.Cell(FsAddress(j, colCount + 3),c).SetValueAs temperatureValue2V3
            t.Cell(FsAddress(j, colCount + 4),c).SetValueAs temperatureValue2V4

    let instrumentHeader = 
        CompositeHeader.Parameter 
            (OntologyAnnotation("instrument model","MS","MS:1000031"))
    let instrumentValue = 
        CompositeCell.createTermFromString
            ("Thermo Fisher Scientific instrument model","MS",Helper.Url.createOAUri "MS" "1000483")


    let instrumentHeaderV1 = "Parameter [instrument model]"
    let instrumentHeaderV2 = "Term Source REF (MS:1000031)"
    let instrumentHeaderV3 = "Term Accession Number (MS:1000031)"

    let instrumentValueV1 = "Thermo Fisher Scientific instrument model"
    let instrumentValueV2 = "MS"
    let instrumentValueV3 = Helper.Url.createOAUri "MS" "1000483"

    let appendInstrumentColumn l (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs instrumentHeaderV1
        t.Cell(FsAddress(1, colCount + 2),c).SetValueAs instrumentHeaderV2
        t.Cell(FsAddress(1, colCount + 3),c).SetValueAs instrumentHeaderV3
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs instrumentValueV1
            t.Cell(FsAddress(i, colCount + 2),c).SetValueAs instrumentValueV2
            t.Cell(FsAddress(i, colCount + 3),c).SetValueAs instrumentValueV3


module Characteristic = 

    let organismHeader = 
        CompositeHeader.Characteristic 
            (OntologyAnnotation("organism","OBI","OBI:0100026"))
    let organismValue = 
        CompositeCell.createTermFromString
            ("Arabidopsis thaliana","NCBITaxon","http://purl.obolibrary.org/obo/NCBITaxon_3702")

    let organismHeaderV1 = "Characteristic [organism]"
    let organismHeaderV2 = "Term Source REF (OBI:0100026)"
    let organismHeaderV3 = "Term Accession Number (OBI:0100026)"

    let organismValueV1 = "Arabidopsis thaliana"
    let organismValueV2 = "NCBITaxon"
    let organismValueV3 = "http://purl.obolibrary.org/obo/NCBITaxon_3702"

    let appendOrganismColumn l (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs organismHeaderV1
        t.Cell(FsAddress(1, colCount + 2),c).SetValueAs organismHeaderV2
        t.Cell(FsAddress(1, colCount + 3),c).SetValueAs organismHeaderV3
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs organismValueV1
            t.Cell(FsAddress(i, colCount + 2),c).SetValueAs organismValueV2
            t.Cell(FsAddress(i, colCount + 3),c).SetValueAs organismValueV3

module Factor = 

    let timeHeader = 
        CompositeHeader.Factor 
            (OntologyAnnotation("time","PATO","PATO:0000165"))
    let timeValue = 
        CompositeCell.createUnitized
            ("10",
            OntologyAnnotation("hour","UO","UO:0000032"))

    let timeHeaderV1 = "Factor [time]"
    let timeHeaderV2 = "Unit"
    let timeHeaderV3 = "Term Source REF (PATO:0000165)"
    let timeHeaderV4 = "Term Accession Number (PATO:0000165)"

    let timeValueV1 = "10"
    let timeValueV2 = "hour"
    let timeValueV3 = "UO"
    let timeValueV4 = Helper.Url.createOAUri "UO" "0000032"


    let appendTimeColumn l (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs timeHeaderV1
        t.Cell(FsAddress(1, colCount + 2),c).SetValueAs timeHeaderV2
        t.Cell(FsAddress(1, colCount + 3),c).SetValueAs timeHeaderV3
        t.Cell(FsAddress(1, colCount + 4),c).SetValueAs timeHeaderV4
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs timeValueV1
            t.Cell(FsAddress(i, colCount + 2),c).SetValueAs timeValueV2 
            t.Cell(FsAddress(i, colCount + 3),c).SetValueAs timeValueV3
            t.Cell(FsAddress(i, colCount + 4),c).SetValueAs timeValueV4

module Input = 

    let sourceValue = 
        CompositeCell.FreeText "MySource"

    let sourceValueV1 = "MySource"

    let dataHeader = 
        CompositeHeader.Input IOType.Data

    let dataValue =
        CompositeCell.Data (Data.create(Name = "MyInputData", Format = "text/csv", SelectorFormat = "https://datatracker.ietf.org/doc/html/rfc7111"))

    let sampleHeader = 
        CompositeHeader.Input IOType.Sample

    let sampleValue = 
        CompositeCell.FreeText "MySample"

    let sampleHeaderV1 = "Input [Sample Name]"
    
    let sampleValueV1 = "MySample"  

    let appendSampleColumn l (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs sampleHeaderV1
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs sampleValueV1

    let dataHeaderV1 = "Input [Data]"
    
    let dataValueV1 = "MyInputData"  

    let dataHeaderV2 = "Data Format"

    let dataValueV2 = "text/csv"

    let dataHeaderV3 = "Data Selector Format"

    let dataValueV3 = "https://datatracker.ietf.org/doc/html/rfc7111"

    let appenddataColumn l (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs dataHeaderV1
        t.Cell(FsAddress(1, colCount + 2),c).SetValueAs dataHeaderV2
        t.Cell(FsAddress(1, colCount + 3),c).SetValueAs dataHeaderV3
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs dataValueV1
            t.Cell(FsAddress(i, colCount + 2),c).SetValueAs dataValueV2
            t.Cell(FsAddress(i, colCount + 3),c).SetValueAs dataValueV3

    let deprecatedSourceHeader = CompositeHeader.Input IOType.Source

    let deprecatedSourceHeaderV1 = "Source Name"

    let appendDeprecatedSourceColumn l (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs deprecatedSourceHeaderV1
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs sourceValueV1


module Output = 
    let simpleDataHeader = 
        CompositeHeader.Output IOType.Data

    let simpleDataValue = 
        CompositeCell.Data (Data.create (Name = "MyData"))

    let fullDataHeader = 
        CompositeHeader.Output IOType.Data

    let fullDataValue =
        CompositeCell.Data (Data.create(Name = "MyData", Format = "text/csv", SelectorFormat = "https://datatracker.ietf.org/doc/html/rfc7111"))


    let rawDataHeaderV1 = "Output [Raw Data File]"
    
    let rawDataValueV1 = "MyRawData"  

    let dataHeaderV1 = "Output [Data]"

    let dataValueV1 = "MyData"

    let dataHeaderV2 = "Data Format"

    let dataValueV2 = "text/csv"

    let dataHeaderV3 = "Data Selector Format"

    let dataValueV3 = "https://datatracker.ietf.org/doc/html/rfc7111"


    let sampleValue = 
        CompositeCell.FreeText "MySample"

    let sampleValueV1 = "MySample"

    let appendSimpleDataColumn l (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs dataHeaderV1
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs dataValueV1

    let appendFullDataColumn l (c : FsCellsCollection) (t : FsTable) =
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs dataHeaderV1
        t.Cell(FsAddress(1, colCount + 2),c).SetValueAs dataHeaderV2
        t.Cell(FsAddress(1, colCount + 3),c).SetValueAs dataHeaderV3
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs dataValueV1
            t.Cell(FsAddress(i, colCount + 2),c).SetValueAs dataValueV2
            t.Cell(FsAddress(i, colCount + 3),c).SetValueAs dataValueV3

    let appendRawDataColumn l (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs rawDataHeaderV1
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs rawDataValueV1

    let deprecatedSampleHeader = CompositeHeader.Output IOType.Sample

    let deprecatedSampleHeaderV1 = "Sample Name"
    
    let appendDeprecatedSampleColumn l (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs deprecatedSampleHeaderV1
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs sampleValueV1

module Comment = 
    
    let simpleCommentHeader = 
        CompositeHeader.Comment "CommentKey"

    let simpleCommentValue = 
        CompositeCell.FreeText "CommentValue"

    let simpleDataHeaderV1 = "Comment [CommentKey]"

    let simpleDataValueV1 = "CommentValue"

    let appendSimpleCommentColumn l (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs simpleDataHeaderV1
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs simpleDataValueV1

    let niceCommentHeader = 
        CompositeHeader.Comment "NiceComment"

    let niceCommentValue =
        CompositeCell.FreeText "NiceCommentValue"

    let niceCommentHeaderV1 = "Comment [NiceComment]"

    let niceCommentValueV1 = "NiceCommentValue"

    let appendNiceCommentColumn l (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs niceCommentHeaderV1
        for i = 2 to l + 1 do  
            t.Cell(FsAddress(i, colCount + 1),c).SetValueAs niceCommentValueV1

module Protocol =
    module REF = 

        let lolHeader = 
            CompositeHeader.ProtocolREF

        let lolValue = CompositeCell.FreeText "LOL"

        let lolHeaderV1 = "Protocol REF"
        let lolValueV1 = "LOL"

        let appendLolColumn l (c : FsCellsCollection) (t : FsTable) = 
            let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
            t.Cell(FsAddress(1, colCount + 1),c).SetValueAs lolHeaderV1
            for i = 2 to l + 1 do                   
                t.Cell(FsAddress(i, colCount + 1),c).SetValueAs lolValueV1

    module Type = 
        
        let collectionHeader = 
            CompositeHeader.ProtocolType
        
        let collectionValue =
            CompositeCell.createTermFromString
                ("sample collection protocol","DPBO",Helper.Url.createOAUri "DPBO" "1000169")

        let collectionHeaderV1 = "Protocol Type"
        let collectionHeaderV2 = "Term Source REF (DPBO:1000161)"
        let collectionHeaderV3 = "Term Accession Number (DPBO:1000161)"

        let collectionValueV1 = "sample collection protocol"
        let collectionValueV2 = "DPBO"
        let collectionValueV3 = Helper.Url.createOAUri "DPBO" "1000169"

        let appendCollectionColumn l (c : FsCellsCollection) (t : FsTable) = 
            let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
            t.Cell(FsAddress(1, colCount + 1),c).SetValueAs collectionHeaderV1
            t.Cell(FsAddress(1, colCount + 2),c).SetValueAs collectionHeaderV2
            t.Cell(FsAddress(1, colCount + 3),c).SetValueAs collectionHeaderV3
            for i = 2 to l + 1 do               
                t.Cell(FsAddress(i, colCount + 1),c).SetValueAs collectionValueV1
                t.Cell(FsAddress(i, colCount + 2),c).SetValueAs collectionValueV2
                t.Cell(FsAddress(i, colCount + 3),c).SetValueAs collectionValueV3

    module Component = 

        let instrumentHeader = 
            CompositeHeader.Component 
                (OntologyAnnotation("instrument model","MS","MS:1000031"))
        let instrumentValue = 
            CompositeCell.createTermFromString
                ("Thermo Fisher Scientific instrument model","MS",Helper.Url.createOAUri "MS" "1000483")


        let instrumentHeaderV1 = "Component [instrument model]"
        let instrumentHeaderV2 = "Term Source REF (MS:1000031)"
        let instrumentHeaderV3 = "Term Accession Number (MS:1000031)"

        let instrumentValueV1 = "Thermo Fisher Scientific instrument model"
        let instrumentValueV2 = "MS"
        let instrumentValueV3 = Helper.Url.createOAUri "MS" "1000483"

        let appendInstrumentColumn l (c : FsCellsCollection) (t : FsTable) = 
            let colCount = if t.IsEmpty(c) then 0 else t.ColumnCount()
            t.Cell(FsAddress(1, colCount + 1),c).SetValueAs instrumentHeaderV1
            t.Cell(FsAddress(1, colCount + 2),c).SetValueAs instrumentHeaderV2
            t.Cell(FsAddress(1, colCount + 3),c).SetValueAs instrumentHeaderV3
            for i = 2 to l + 1 do  
                t.Cell(FsAddress(i, colCount + 1),c).SetValueAs instrumentValueV1
                t.Cell(FsAddress(i, colCount + 2),c).SetValueAs instrumentValueV2
                t.Cell(FsAddress(i, colCount + 3),c).SetValueAs instrumentValueV3


let addSpacesToEnd (cc : FsCellsCollection) (t : FsTable) =
    let count = System.Collections.Generic.Dictionary<string,string>()
    t.GetHeaderRow(cc) |> Seq.iter  (fun c ->
        let k = c.ValueAsString()
        match Dictionary.tryGet k count with
        | Some v -> 
            let newV = v + " "
            c.SetValueAs (k + newV)
            count.[k] <- newV
        | None ->
            count.[k] <- ""
    
    )

let initTable (appendOperations : (FsCellsCollection -> FsTable -> unit) list)= 
    let c = FsCellsCollection()
    let t = FsTable(ArcTable.annotationTablePrefix, FsRangeAddress("A1:A1"))
    appendOperations 
    |> List.iter (fun o -> o c t)
    c,t

let initTableCols (appendOperations : (FsCellsCollection -> FsTable -> unit) list)= 
    let c,t = initTable appendOperations
    t.GetColumns(c)
    |> Seq.toArray

let initWorksheet (name : string) (appendOperations : (FsCellsCollection -> FsTable -> unit) list) = 
    let w = FsWorksheet(name)
    let t  = w.Table(ArcTable.annotationTablePrefix, FsRangeAddress("A1:A1"))
    appendOperations 
    |> List.iter (fun o -> o w.CellCollection t)
    addSpacesToEnd w.CellCollection t
    t.RescanFieldNames(w.CellCollection)
    w


let initWorksheetWithTableName (worksheetName : string) (tableName : string) (appendOperations : (FsCellsCollection -> FsTable -> unit) list) = 
    let w = FsWorksheet(worksheetName)
    let t  = w.Table(tableName, FsRangeAddress("A1:A1"))
    appendOperations 
    |> List.iter (fun o -> o w.CellCollection t)
    addSpacesToEnd w.CellCollection t
    t.RescanFieldNames(w.CellCollection)
    w