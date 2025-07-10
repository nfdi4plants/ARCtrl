module PerformanceReport

open ARCtrl
open ARCtrl.Helper
open ARCtrl.Json
open ARCtrl.Spreadsheet
open Fable.Core

#if FABLE_COMPILER_JAVASCRIPT
open Node.Api
#endif

#if FABLE_COMPILER_PYTHON
open Fable.Python.Builtins
#endif

let writeFile (path : string) (content : string) =
    #if FABLE_COMPILER_JAVASCRIPT
    fs.writeFileSync(path,content)
    #endif
    #if FABLE_COMPILER_PYTHON
    let file = builtins.``open``(path, OpenTextMode.Write)
    file.write(content) |> ignore
    #endif
    #if !FABLE_COMPILER
    System.IO.File.WriteAllText(path,content)
    #endif

    //fs.writeFile(path,content)

// Create an assay with 6 columns and 10000 rows, where 3 columns are fixed and 3 columns are variable
let prepareAssay() =
    let a = ArcAssay.init "MyAssay"
    let t = a.InitTable("MyTable")
    t.AddColumn(CompositeHeader.Input IOType.Source)
    t.AddColumn(CompositeHeader.Parameter (OntologyAnnotation("MyParameter1")))
    t.AddColumn(CompositeHeader.Parameter (OntologyAnnotation("MyParameter")))
    t.AddColumn(CompositeHeader.Parameter (OntologyAnnotation("MyParameter")))
    t.AddColumn(CompositeHeader.Characteristic (OntologyAnnotation("MyCharacteristic")))
    t.AddColumn(CompositeHeader.Output IOType.Sample)
    let rowCount = 10000
    for i = 0 to rowCount - 1 do
        let cells =             
            [|
                CompositeCell.FreeText $"Source{i}"
                CompositeCell.FreeText $"Parameter1_value"
                CompositeCell.FreeText $"Parameter2_value"
                CompositeCell.FreeText $"Parameter3_value{i - i % 10}"
                CompositeCell.FreeText $"Characteristic_value"
                CompositeCell.FreeText $"Sample{i}"
            |]
        for j = 0 to cells.Length - 1 do
            ArcTableAux.Unchecked.setCellAt (j,i,cells.[j]) a.Tables.[0].Values
    a

type PerformanceTest =
    {
        Name : string
        Description : string
        Prepare : unit -> unit
        PrepareAgain : bool
        Test : unit -> unit
        Times : int []
    }

    member this.Run() =
        printfn  "%A: Preparing test: %s" System.DateTime.Now this.Name
        printfn "\tC%A: Completeted in %ims" System.DateTime.Now (TestingUtils.Stopwatch.measure(this.Prepare) |> int)
        printfn "%A: Running test: %s" System.DateTime.Now this.Name
        let times =
            [|
                for i = 0 to 10 do
                    if this.PrepareAgain then
                        printfn "\t%A: Preparing test again: %s" System.DateTime.Now this.Name
                        this.Prepare()
                    let time = TestingUtils.Stopwatch.measure(this.Test) |> int
                    printfn "\t%A: Completeted in %ims" System.DateTime.Now time
                    time
            |]
        printfn ""
        {this with Times = times}

    static member create name description prepare prepareAgain test = {Name = name; Description = description; PrepareAgain = prepareAgain; Prepare = prepare; Test = test; Times = [||]}

type PerformanceReport =
    {
        CPU : string
        Lang : string
        Tests : PerformanceTest list
    }

    static member create cpu lang tests = {CPU = cpu; Lang = lang; Tests = tests}

    member this.RunTests() =
        {
            this with Tests = this.Tests |> List.map (fun t -> t.Run())
        }

    member this.ToMarkdown() = 
        let header = $"| Name | Description | CPU | {this.Lang} Time (ms) |"
        let separator = $"| --- | --- | --- | --- |"
        let formatTime (times: int []) =
            if times.Length = 0 then "N/A"
            else
                let mean = Array.average (Array.map float times) |> int
                let stdev = 
                    let meanFloat = float mean
                    sqrt (Array.average (Array.map (fun x -> (float x - meanFloat) ** 2.0) times))
                    |> int
                $"{mean} ± {stdev}"
        let tests = this.Tests |> List.map (fun t -> $"| {t.Name} | {t.Description} | {this.CPU} | {formatTime t.Times} |")
        String.concat "\n" [header; separator; tests |> String.concat "\n"]


let table_GetHashCode = 
    let testTable = ArcTable.init("Test")
    let fillTable () =
        // Prepare the table with 1 column and 10000 rows
        let values = ResizeArray.init 10000 (fun i -> CompositeCell.createFreeText (string i))
        testTable.AddColumn(CompositeHeader.FreeText "Header", values)
    PerformanceTest.create
        "Table_GetHashCode"
        "From a table with 1 column and 10000 rows, retrieve the Hash Code"
        fillTable
        false // Do not prepare again, as the table is used and not altered
        (fun _ ->            
            testTable.GetHashCode()
            |> ignore
        )   
    
                       
//    // Commented this test out, as the behaviour is different in dotnet and js, but both implementations are very close together performance-wise     
//    //testCase "performance" (fun () ->
//        //    // Test, that for most cases (because of performance), setter should be used
//        //    let f1 = fun () ->
//        //        let table = ArcTable.init("Table")
//        //        for i = 0 to 10 do
//        //            table.Headers.Insert(i,CompositeHeader.FreeText $"Header_{i}")
//        //            for j = 0 to 5000 do
//        //                ArcTableAux.Unchecked.setCellAt(i,j,CompositeCell.createFreeText $"Cell_{i}_{j}") table.Values
//        //    let f2 = fun () ->
//        //        let table = ArcTable.init("Table")
//        //        for i = 0 to 10 do
//        //            table.Headers.Insert(i,CompositeHeader.FreeText $"Header_{i}")
//        //            for j = 0 to 5000 do
//        //                ArcTableAux.Unchecked.addCellAt(i,j,CompositeCell.createFreeText $"Cell_{i}_{j}") table.Values
//        //    Expect.isFasterThan f1 f2 "SetCell Implementation should be faster than reference"

let table_AddColumnsWithDistinctValues =
    let initTable () = ArcTable("MyTable")
    let mutable columns = ResizeArray()
    let prepareColumns () =
        columns.Add (
            CompositeColumn.create(header = CompositeHeader.Input IOType.Sample, cells = ResizeArray.init 10000 (fun i -> CompositeCell.FreeText $"Source_{i}"))
        )
        columns.Add (
            CompositeColumn.create(header = CompositeHeader.FreeText "Freetext1", cells = ResizeArray.init 10000 (fun i -> CompositeCell.FreeText $"FT1_{i}"))
        )
        columns.Add (
            CompositeColumn.create(header = CompositeHeader.FreeText "Freetext2", cells = ResizeArray.init 10000 (fun i -> CompositeCell.FreeText $"FT2_{i}"))
        )
        columns.Add (
            CompositeColumn.create(header = CompositeHeader.Output IOType.Sample, cells = ResizeArray.init 10000 (fun i -> CompositeCell.FreeText $"Sample_{i}"))
        )

    PerformanceTest.create
        "Table_AddColumnsWithDistinctValues"
        "Add 4 columns with 10000 distinct values each."
        prepareColumns
        false // Do not prepare again, as the rows are used and not altered
        (fun _ ->
            let table = initTable()
            table.AddColumns(columns)
            |> ignore
        )

let table_AddColumnsWithIdenticalValues =
    let initTable () = ArcTable("MyTable")
    let mutable columns = ResizeArray()
    let prepareColumns () =
        columns.Add (
            CompositeColumn.create(header = CompositeHeader.Input IOType.Sample, cells = ResizeArray.init 10000 (fun _ -> CompositeCell.FreeText "Source_5000"))
        )
        columns.Add (
            CompositeColumn.create(header = CompositeHeader.FreeText "Freetext1", cells = ResizeArray.init 10000 (fun _ -> CompositeCell.FreeText "FT1_5000"))
        )
        columns.Add (
            CompositeColumn.create(header = CompositeHeader.FreeText "Freetext2", cells = ResizeArray.init 10000 (fun _ -> CompositeCell.FreeText "FT2_5000"))
        )
        columns.Add (
            CompositeColumn.create(header = CompositeHeader.Output IOType.Sample, cells = ResizeArray.init 10000 (fun _ -> CompositeCell.FreeText "Sample_5000"))
        )

    PerformanceTest.create
        "Table_AddColumnsWithIdenticalValues"
        "Add 4 columns with 10000 identical values each."
        prepareColumns
        false // Do not prepare again, as the rows are used and not altered
        (fun _ ->
            let table = initTable()
            table.AddColumns(columns)
            |> ignore
        )


let table_AddDistinctRows =
    let initTable () =
        ArcTable("MyTable", ResizeArray [CompositeHeader.Input IOType.Sample; CompositeHeader.FreeText "Freetext1"; CompositeHeader.FreeText "Freetext2"; CompositeHeader.Output IOType.Sample])
    let mutable rows = ResizeArray()
    let prepareRows () =
        rows <-
         ResizeArray.init 10000 (fun i -> 
            ResizeArray [|CompositeCell.FreeText $"Source_{i}"; CompositeCell.FreeText $"FT1_{i}"; CompositeCell.FreeText $"FT2_{i}"; CompositeCell.FreeText $"Sample_{i}"; |])
    PerformanceTest.create
        "Table_AddDistinctRows"
        "Add 10000 distinct rows to a table with 4 columns."
        prepareRows
        false // Do not prepare again, as the rows are used and not altered
        (fun _ ->
            let table = initTable()
            table.AddRows(rows)
            |> ignore
        )


let table_AddIdenticalRows =
    let initTable () =
        ArcTable("MyTable", ResizeArray [CompositeHeader.Input IOType.Sample; CompositeHeader.FreeText "Freetext1"; CompositeHeader.FreeText "Freetext2"; CompositeHeader.Output IOType.Sample])
    let mutable rows = ResizeArray()
    let prepareRows () =
        rows <-
         ResizeArray.init 10000 (fun i -> 
            ResizeArray [|CompositeCell.FreeText $"Source_{5000}"; CompositeCell.FreeText $"FT1_{5000}"; CompositeCell.FreeText $"FT2_{5000}"; CompositeCell.FreeText $"Sample_{5000}"; |])
    PerformanceTest.create
        "Table_AddIdenticalRows"
        "Add 10000 identical rows to a table with 4 columns."
        prepareRows
        false // Do not prepare again, as the rows are used and not altered
        (fun _ ->
            let table = initTable()
            table.AddRows(rows)
            |> ignore
        )

let table_fillMissingCells =
    let headers = ResizeArray [CompositeHeader.Input IOType.Sample;CompositeHeader.FreeText "Freetext1" ; CompositeHeader.FreeText "Freetext2"; CompositeHeader.FreeText "Freetext3"; CompositeHeader.FreeText "Freetext4"; CompositeHeader.FreeText "Freetext5"; CompositeHeader.Output IOType.Sample]
    let mutable values = ArcTableAux.ArcTableValues.init()
    
    let prepareValues () =
        values <- ArcTableAux.ArcTableValues.init()
        for i = 0 to 20000 do       
        if i%2 = 0 then
            ArcTableAux.Unchecked.setCellAt(0,i,(CompositeCell.FreeText $"Source_{i}")) values
            ArcTableAux.Unchecked.setCellAt(1,i,(CompositeCell.FreeText $"FT1_{i}")) values
            ArcTableAux.Unchecked.setCellAt(2,i,(CompositeCell.FreeText $"FT2_{i}")) values
            ArcTableAux.Unchecked.setCellAt(3,i,(CompositeCell.FreeText $"FT3_{i}")) values
            ArcTableAux.Unchecked.setCellAt(6,i,(CompositeCell.FreeText $"Sample_{i}")) values
        else
            ArcTableAux.Unchecked.setCellAt(0,i,(CompositeCell.FreeText $"Source_{i}")) values
            ArcTableAux.Unchecked.setCellAt(3,i,(CompositeCell.FreeText $"FT3_{i}")) values
            ArcTableAux.Unchecked.setCellAt(4,i,(CompositeCell.FreeText $"FT4_{i}")) values
            ArcTableAux.Unchecked.setCellAt(5,i,(CompositeCell.FreeText $"FT5_{i}")) values
            ArcTableAux.Unchecked.setCellAt(6,i,(CompositeCell.FreeText $"Sample_{i}")) values

    PerformanceTest.create 
        "Table_fillMissingCells"
        "For a table 6 columns and 20000 rows, where each row has one missing value, fill those values with default values."
        prepareValues
        true // Prepare again, as the dictionary is mutably altered
        (fun () -> ArcTableAux.Unchecked.fillMissingCells headers values |> ignore)
   

let table_toJson =
    let mutable t = ArcTable.init("Default")
    let prepare() =
        t <- TestObjects.Spreadsheet.Study.LargeFile.createTable()
    PerformanceTest.create
        "Table_ToJson"
        "Serialize a table with 5 columns and 10000 rows to json, with 3 fixed and 2 variable columns."
        prepare
        false // Do not prepare again, as the table is used and not altered
        (fun _ -> t.ToJsonString() |> ignore)

let table_toCompressedJson =
    let mutable t = ArcTable.init("Default")
    let prepare() =
        t <- TestObjects.Spreadsheet.Study.LargeFile.createTable()
    PerformanceTest.create
        "Table_ToCompressedJson"
        "Serialize a table with 5 columns and 10000 rows to compressed json, with 3 fixed and 2 variable columns."
        prepare
        false // Do not prepare again, as the table is used and not altered
        (fun _ -> t.ToCompressedJsonString() |> ignore)       
    
let assay_toJson =
    let mutable a = ArcAssay.init("MyAssay")
    let prepare() =
        a <- prepareAssay()
    PerformanceTest.create
        "Assay_toJson"
        "Parse an assay with one table with 10000 rows and 6 columns to json, with 3 fixed and 3 variable columns."
        prepare
        false // Do not prepare again, as the assay is used and not altered
        (fun _ -> ArcAssay.toJsonString() a |> ignore)

let assay_fromJson =
    let mutable json = ""
    let prepare() =
        let a = prepareAssay()
        json <- a.ToJsonString()
    PerformanceTest.create
        "Assay_fromJson"
        "Parse an assay with one table with 10000 rows and 6 columns from json, with 3 fixed and 3 variable columns."
        prepare
        false // Do not prepare again, as the assay is used and not altered
        (fun _ -> ArcAssay.fromJsonString json |> ignore)
        
let assay_toISAJson =
    let mutable a = ArcAssay.init("MyAssay")
    let prepare() =
        a <- prepareAssay()
    PerformanceTest.create
        "Assay_toISAJson"
        "Parse an assay with one table with 10000 rows and 6 columns to json, with 3 fixed and 3 variable columns"
        prepare
        false // Do not prepare again, as the assay is used and not altered
        (fun _ -> ArcAssay.toISAJsonString() a |> ignore)

let assay_fromISAJson =
    let mutable json = ""
    let prepare() =
        let a = prepareAssay()
        json <- ArcAssay.toISAJsonString() a
    PerformanceTest.create
        "Assay_fromISAJson"
        "Parse an assay with one table with 10000 rows and 6 columns from json, with 3 fixed and 3 variable columns"
        prepare
        false // Do not prepare again, as the assay is used and not altered
        (fun _ -> ArcAssay.fromISAJsonString json |> ignore)

let study_fromWorkbook =
    let mutable fswb = new FsSpreadsheet.FsWorkbook()
    let prepareWorkbook ()  =
        let table = TestObjects.Spreadsheet.Study.LargeFile.createTable()
        fswb <- TestObjects.Spreadsheet.Study.LargeFile.createWorkbook (Some table)
    PerformanceTest.create
        "Study_FromWorkbook"
        "Parse a workbook with one study with 10000 rows and 6 columns to an ArcStudy"
        prepareWorkbook
        false // Do not prepare again, as the workbook is used and not altered
        (fun _ -> ArcStudy.fromFsWorkbook fswb |> ignore)
   
let investigation_toWorkbook_ManyStudies =
    let inv = ArcInvestigation.init("MyInvestigation")
    let prepareInvestigation () =
        for i = 0 to 1500 do 
            let s = ArcStudy.init($"Study{i}")
            inv.AddRegisteredStudy(s)
    PerformanceTest.create
        "Investigation_ToWorkbook_ManyStudies"
        "Parse an investigation with 1500 studies to a workbook"
        prepareInvestigation
        false // Do not prepare again, as the investigation is used and not altered
        (fun _ -> ArcInvestigation.toFsWorkbook inv |> ignore)

let investigation_fromWorkbook_ManyStudies =
    let mutable fswb = new FsSpreadsheet.FsWorkbook()
    let prepareWorkbook () =
        let inv = ArcInvestigation.init("MyInvestigation")
        for i = 0 to 1500 do 
            let s = ArcStudy.init($"Study{i}")
            inv.AddRegisteredStudy(s)
        fswb <- ArcInvestigation.toFsWorkbook inv
    PerformanceTest.create
        "Investigation_FromWorkbook_ManyStudies"
        "Parse a workbook with 1500 studies to an ArcInvestigation"
        prepareWorkbook
        false // Do not prepare again, as the workbook is used and not altered
        (fun _ -> ArcInvestigation.fromFsWorkbook fswb |> ignore)

let arc_toROCrate =
    let arc = ARC("MyARC", title = "My ARC Title", description = "My Description")
    let prepareARC () =
        let a = prepareAssay()
        arc.AddAssay(a)
    PerformanceTest.create
        "ARC_ToROCrate"
        "Parse an ARC with one assay with 10000 rows and 6 columns to a RO-Crate metadata file."
        prepareARC
        false // Do not prepare again, as the ARC is used and not altered
        (fun _ -> arc.ToROCrateJsonString() |> ignore)


let arc_fromROCrate =
    let mutable json = ""
    let prepareARC () =
        let a = prepareAssay()
        let arc = ARC("MyARC", title = "My ARC Title", description = "My Description")
        arc.AddAssay(a)
        json <- arc.ToROCrateJsonString()
    PerformanceTest.create
        "ARC_FromROCrate"
        "Parse an ARC with one assay with 10000 rows and 6 columns from a RO-Crate metadata file."
        prepareARC
        false // Do not prepare again, as the ARC is used and not altered
        (fun _ -> ARC.fromROCrateJsonString json |> ignore)
        
let allPerformanceTests = 
    [
        table_GetHashCode
        table_AddDistinctRows
        table_AddIdenticalRows
        table_AddColumnsWithDistinctValues
        table_AddColumnsWithIdenticalValues
        table_fillMissingCells
        table_toJson
        table_toCompressedJson
        assay_toJson
        assay_fromJson
        assay_toISAJson
        assay_fromISAJson
        study_fromWorkbook
        investigation_toWorkbook_ManyStudies
        investigation_fromWorkbook_ManyStudies
        arc_toROCrate
        //arc_fromROCrate
    ]


let createMarkdownPerformanceReport lang cpu =
    let report = PerformanceReport.create cpu lang allPerformanceTests
    report.RunTests().ToMarkdown()

let lang = 
    #if FABLE_COMPILER_JAVASCRIPT
    "JavaScript"
    #endif 
    #if FABLE_COMPILER_PYTHON
    "Python"
    #endif
    #if !FABLE_COMPILER
    "FSharp"
    #endif

let runReport cpu =
    let report = createMarkdownPerformanceReport lang cpu
    let outFile = $"tests/Speedtest/PerformanceReport/PerformanceReport_{lang}.md"
    writeFile outFile report
    printfn "%s" report
    0
