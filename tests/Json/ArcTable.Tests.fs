module Tests.ArcTable

open ARCtrl
open ARCtrl.Json
open TestingUtils


module Helper =
    let create_empty() = ArcTable.init("New Table")
    let create_filled() = ArcTable.create(
        "New Table", 
        ResizeArray([
            CompositeHeader.Input IOType.Source; 
            CompositeHeader.Component (OntologyAnnotation("instrument model", "MS", "MS:424242")); 
            CompositeHeader.Output IOType.Sample
        ]),
        System.Collections.Generic.Dictionary(Map [
            (0,0), CompositeCell.FreeText "Input 1"
            (0,1), CompositeCell.FreeText "Input 2"
            (1,0), CompositeCell.Term (OntologyAnnotation("SCIEX instrument model"))
            (1,1), CompositeCell.Term (OntologyAnnotation("SCIEX instrument model"))
            (2,0), CompositeCell.FreeText "Output 1"
            (2,1), CompositeCell.FreeText "Output 2"
        ])
    )

open Helper

let private tests_core = 
    let json = """{"name":"New Table","header":[{"headertype":"Input","values":["Source Name"]},{"headertype":"Component","values":[{"annotationValue":"instrument model","termSource":"MS","termAccession":"MS:424242"}]},{"headertype":"Output","values":["Sample Name"]}],"values":[[[0,0],{"celltype":"FreeText","values":["Input 1"]}],[[0,1],{"celltype":"FreeText","values":["Input 2"]}],[[1,0],{"celltype":"Term","values":[{"annotationValue":"SCIEX instrument model"}]}],[[1,1],{"celltype":"Term","values":[{"annotationValue":"SCIEX instrument model"}]}],[[2,0],{"celltype":"FreeText","values":["Output 1"]}],[[2,1],{"celltype":"FreeText","values":["Output 2"]}]]}"""
    createBaseJsonTests
        "core"
        create_filled
        ArcTable.toJsonString
        ArcTable.fromJsonString
        None
        None

let private tests_coreEmpty = 
    let json = """{"name":"New Table"}""" 
    createBaseJsonTests
        "core"
        create_empty
        ArcTable.toJsonString
        ArcTable.fromJsonString
        None
        None

let private tests_compressedEmpty = 
    let json = """{"cellTable":[],"oaTable":[],"stringTable":["New Table"],"table":{"n":0}}""" 
    createBaseJsonTests
        "core"
        create_empty
        ArcTable.toCompressedJsonString
        ArcTable.fromCompressedJsonString
        None
        None
        
let private tests_compressedFilled = 
    let json = """{"cellTable":[{"t":2,"v":[1]},{"t":2,"v":[3]},{"t":4,"v":[0]},{"t":2,"v":[5]},{"t":2,"v":[6]}],"oaTable":[{"a":7}],"stringTable":["New Table","Input 1","FreeText","Input 2","Term","Output 1","Output 2","SCIEX instrument model"],"table":{"n":0,"h":[{"headertype":"Input","values":["Source Name"]},{"headertype":"Component","values":[{"annotationValue":"instrument model","termSource":"MS","termAccession":"MS:424242"}]},{"headertype":"Output","values":["Sample Name"]}],"c":[[0,1],[2,2],[3,4]]}}"""
    createBaseJsonTests
        "core"
        create_filled
        ArcTable.toCompressedJsonString
        ArcTable.fromCompressedJsonString
        None
        None

let private tests_compressedExtended = testList "compressed_extended" [
        testCase "rangeColumnSize" <| fun _ ->
            // testTable column should be saved as range column, this should make it smaller than the IO column even though it has more cells
            let testTable = ArcTable.init("Test")
            testTable.AddColumn (CompositeHeader.FreeText "ABC", [|for i = 0 to 200 do yield CompositeCell.FreeText "TestValue"|])
            let referenceTable = ArcTable.init("Test")
            referenceTable.AddColumn (CompositeHeader.Input IOType.Source, [|for i = 0 to 100 do yield CompositeCell.FreeText "TestValue"|])
            let compressedTest = testTable.ToCompressedJsonString()
            let compressedReference = referenceTable.ToCompressedJsonString()
            Expect.isTrue (compressedTest.Length < compressedReference.Length) "range column should be smaller than IO column"
            let decompressedTest = ArcTable.fromCompressedJsonString compressedTest
            Expect.arcTableEqual decompressedTest testTable "decompressed table should be equal to original table"
        testCase "rangeColumnCorrectness" <| fun _ ->
            let testTable = ArcTable.init("Test")
            testTable.AddColumn (CompositeHeader.FreeText "ABC")
            for i = 0 to 100 do testTable.AddRow ([|CompositeCell.FreeText "TestValue1"|])
            for i = 101 to 200 do testTable.AddRow ([|CompositeCell.FreeText "TestValue2"|])
            testTable.AddRow ([|CompositeCell.FreeText "TestValue3"|])
            let encoded = testTable.ToCompressedJsonString()
            let decoded = ArcTable.fromCompressedJsonString encoded
            Expect.arcTableEqual decoded testTable "decompressed table should be equal to original table"
        testCase "SeparateOntologyAnnotations" <| fun _ ->
            let table = ArcTable.init "My Table"

            table.AddColumn(
                CompositeHeader.Component (OntologyAnnotation.create("instrument model", "MS", "MS:424242")),
                [|for i in 0 .. 2 do CompositeCell.createTermFromString $"Term"|]
            )

            let encoded = table.ToCompressedJsonString()
            let decoded = ArcTable.fromCompressedJsonString encoded

            Expect.equal (table.Values.[(0,1)]) (decoded.Values.[(0,1)]) "Should be the same cell"

            let cell = decoded.Values.[(0,0)]

            match cell with
            | CompositeCell.Term oa -> 
              oa.Name <- Some "New Name"
            | _ -> failwith "Expected a term"

            let otherCell = decoded.Values.[(0,1)]

            Expect.equal (table.Values.[(0,1)]) (decoded.Values.[(0,1)]) "Should still be the same cell"
            Expect.equal (decoded.Values.[(0,1)]) (decoded.Values.[(0,2)]) "Should also be the same cell"
            Expect.notEqual cell otherCell "Should not be the same cell"
    ]


let private tests_dataColumns = 
    testList "dataColumns" [
        testCase "basic WriteRead" <| fun _ ->
            let testTable = ArcTable.init("Test") 
            testTable.AddColumn (CompositeHeader.Input IOType.Data, [|CompositeCell.Data (Data(name="MyInputDataFile.csv"))|])
            testTable.AddColumn (CompositeHeader.Output IOType.Data, [|CompositeCell.Data (Data(name="MyData.csv#row=1",format="text/csv",selectorFormat = "MySelector"))|])

            let encoded = testTable.ToJsonString()
            let decoded = ArcTable.fromJsonString encoded
            Expect.arcTableEqual decoded testTable "decompressed table should be equal to original table"
    ]
 
let private tests_commentColumns = 
    testList "commentColumns" [
        testCase "basic WriteRead" <| fun _ ->
            let testTable = ArcTable.init("Test") 
            testTable.AddColumn (CompositeHeader.Input IOType.Source, [|CompositeCell.FreeText "Source"|])
            testTable.AddColumn (CompositeHeader.Comment "CommentKey", [|CompositeCell.FreeText "CommentValue"|])
            testTable.AddColumn (CompositeHeader.Output IOType.Sample, [|CompositeCell.FreeText "Sample"|])
            let encoded = testTable.ToJsonString()
            let decoded = ArcTable.fromJsonString encoded
            Expect.arcTableEqual decoded testTable "decoded table should be equal to original table"
        testCase "compressed WriteRead" <| fun _ ->
            let testTable = ArcTable.init("Test") 
            testTable.AddColumn (CompositeHeader.Input IOType.Source, [|CompositeCell.FreeText "Source"|])
            testTable.AddColumn (CompositeHeader.Comment "CommentKey", [|CompositeCell.FreeText "CommentValue"|])
            testTable.AddColumn (CompositeHeader.Output IOType.Sample, [|CompositeCell.FreeText "Sample"|])
            let encoded = testTable.ToCompressedJsonString()
            let decoded = ArcTable.fromCompressedJsonString encoded
            Expect.arcTableEqual decoded testTable "decoded table should be equal to original table"
    ] 


let main = testList "ArcTable" [
    tests_compressedExtended
    tests_core
    tests_coreEmpty
    tests_compressedEmpty
    tests_compressedFilled
    tests_dataColumns
    tests_commentColumns
]