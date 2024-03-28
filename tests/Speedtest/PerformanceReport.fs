module PerformanceReport

open ARCtrl
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

type PerformanceTest =
    {
        Name : string
        Description : string
        Test : unit -> unit
        Time : int option
    }

    member this.Run() =
        printfn "%A: Running test: %s" System.DateTime.Now this.Name
        let time = TestingUtils.Stopwatch.measure(this.Test) |> int
        printfn "\tC%A: Completeted in %ims" System.DateTime.Now time
        {this with Time = Some time}

    static member create name description test = {Name = name; Description = description; Test = test; Time = None}

type PerformanceReport =
    {
        CPU : string
        Lang : string
        Tests : PerformanceTest list
    }

    static member create cpu lang tests = {CPU = cpu; Lang = lang; Tests = tests}

    member this.RunTests() =
        {this with Tests = this.Tests |> List.map (fun t -> t.Run())}

    member this.ToMarkdown() = 
        let header = $"| Name | Description | CPU | {this.Lang} Time (ms) |"
        let separator = $"| --- | --- | --- | --- |"
        let tests = this.Tests |> List.map (fun t -> $"| {t.Name} | {t.Description} | {this.CPU} | {t.Time.Value} |")
        String.concat "\n" [header; separator; tests |> String.concat "\n"]


let table_GetHashCode = 
    let testTable = ArcTable.init("Test")
    let values = Array.init 10000 (fun i -> CompositeCell.createFreeText (string i))
    testTable.AddColumn(CompositeHeader.FreeText "Header", values)
    PerformanceTest.create
        "Table_GetHashCode"
        "From a table with 1 column and 10000 rows, retrieve the Hash Code"
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


let table_AddRows =

    let table = ArcTable("MyTable",ResizeArray [CompositeHeader.Input IOType.Sample;CompositeHeader.FreeText "Freetext1" ; CompositeHeader.FreeText "Freetext2"; CompositeHeader.Output IOType.Sample], System.Collections.Generic.Dictionary())
    let rows = 
         Array.init 10000 (fun i -> 
            [|CompositeCell.FreeText $"Source_{i}"; CompositeCell.FreeText $"FT1_{i}"; CompositeCell.FreeText $"FT2_{i}"; CompositeCell.FreeText $"Sample_{i}"; |])
    PerformanceTest.create
        "Table_AddRows"
        "Add 10000 rows to a table with 4 columns."
        (fun _ ->            
            table.AddRows(rows)
            |> ignore
        )

let table_fillMissingCells =
    let headers = ResizeArray [CompositeHeader.Input IOType.Sample;CompositeHeader.FreeText "Freetext1" ; CompositeHeader.FreeText "Freetext2"; CompositeHeader.Output IOType.Sample]
    let values = System.Collections.Generic.Dictionary()
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
        (fun () -> ArcTableAux.Unchecked.fillMissingCells headers values |> ignore)
       
let table_toJson =
    let t = TestObjects.Spreadsheet.Study.LargeFile.table
    PerformanceTest.create
        "Table_ToJson"
        "Serialize a table with 5 columns and 10000 rows to json."
        (fun _ -> t.ToJsonString() |> ignore)

let table_toCompressedJson =
    let t = TestObjects.Spreadsheet.Study.LargeFile.table
    PerformanceTest.create
        "Table_ToCompressedJson"
        "Serialize a table with 5 columns and 10000 rows to compressed json."
        (fun _ -> t.ToCompressedJsonString() |> ignore)       
    
let assay_toJson =
    let a = ArcAssay.init("MyAssay")
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
            t.Values.[(j,i)] <- cells.[j]
    PerformanceTest.create
        "Assay_toJson"
        "Parse an assay with one table with 10000 rows and 6 columns to json"
        (fun _ -> ArcAssay.toJsonString() a |> ignore)

let study_fromWorkbook =
    let fswb = TestObjects.Spreadsheet.Study.LargeFile.Workbook
    PerformanceTest.create
        "Study_FromWorkbook"
        "Parse a workbook with one study with 10000 rows and 6 columns to an ArcStudy"
        (fun _ -> ArcStudy.fromFsWorkbook fswb |> ignore)
   
let investigation_toWorkbook_ManyStudies =
    let inv = ArcInvestigation.init("MyInvestigation")
    for i = 0 to 1500 do 
        let s = ArcStudy.init($"Study{i}")
        inv.AddRegisteredStudy(s)
    PerformanceTest.create
        "Investigation_ToWorkbook_ManyStudies"
        "Parse an investigation with 1500 studies to a workbook"
        (fun _ -> ArcInvestigation.toFsWorkbook inv |> ignore)


let allPerformanceTests = 
    [
        table_GetHashCode
        table_AddRows
        table_fillMissingCells
        table_toJson
        table_toCompressedJson
        assay_toJson
        study_fromWorkbook
        investigation_toWorkbook_ManyStudies
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
