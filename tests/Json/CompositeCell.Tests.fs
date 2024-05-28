module Tests.CompositeCell

open TestingUtils
open ARCtrl
open ARCtrl.Json

let tests_extended = testList "extended" [
    let cell_freetext = CompositeCell.FreeText "Hello World"
    let cell_term = CompositeCell.Term (OntologyAnnotation("My Name", "MY", "MY:1"))
    let cell_term_empty = CompositeCell.emptyTerm
    let cell_unitized = CompositeCell.Unitized ("42", OntologyAnnotation("My Name", "MY", "MY:1"))
    let cell_unitized_empty = CompositeCell.emptyUnitized
    let cell_freetext_jsonString = sprintf """{"%s":"FreeText","values":["Hello World"]}""" CompositeCell.CellType
    let cell_term_jsonString = sprintf """{"%s":"Term","values":[{"annotationValue":"My Name","termSource":"MY","termAccession":"MY:1"}]}""" CompositeCell.CellType
    let cell_term_empty_jsonString = sprintf """{"%s":"Term","values":[{}]}""" CompositeCell.CellType
    let cell_unitized_jsonString = sprintf """{"%s":"Unitized","values":["42",{"annotationValue":"My Name","termSource":"MY","termAccession":"MY:1"}]}""" CompositeCell.CellType
    let cell_unitized_empty_jsonString = sprintf """{"%s":"Unitized","values":["",{}]}""" CompositeCell.CellType
    let cell_data_empty_jsonString = sprintf """{"%s":"Data","values":[{}]}""" CompositeCell.CellType
    let cell_data_jsonString = sprintf """{"%s":"Data","values":[{"@id":"MyID","name":"MyName","dataType":"Raw Data File","format":"text/csv","selectorFormat":"MySelector","comments":[{"name":"MyKey","value":"MyValue"}]}]}""" CompositeCell.CellType

    testList "encoder (toJsonString)" [
        testCase "FreeText" <| fun _ -> 
            let actual = CompositeCell.encoder cell_freetext |> Encode.toJsonString  0
            let expected = cell_freetext_jsonString
            Expect.equal actual expected ""
        testCase "Term" <| fun _ -> 
            let actual = CompositeCell.encoder cell_term |> Encode.toJsonString  0
            let expected = cell_term_jsonString
            Expect.equal actual expected ""
        testCase "Term empty" <| fun _ -> 
            let actual = CompositeCell.encoder cell_term_empty |> Encode.toJsonString  0
            let expected = cell_term_empty_jsonString
            Expect.equal actual expected ""
        testCase "Unitized" <| fun _ -> 
            let actual = CompositeCell.encoder cell_unitized |> Encode.toJsonString  0
            let expected = cell_unitized_jsonString
            Expect.equal actual expected ""
        testCase "Unitized empty" <| fun _ -> 
            let actual = CompositeCell.encoder cell_unitized_empty |> Encode.toJsonString  0
            let expected = cell_unitized_empty_jsonString
            Expect.equal actual expected ""
        testCase "Data" <| fun _ -> 
            let actual = CompositeCell.encoder (CompositeCell.Data(Data("MyID","MyName",Process.DataFile.RawDataFile,"text/csv","MySelector",ResizeArray [Comment.create("MyKey","MyValue")])) ) |> Encode.toJsonString  0
            let expected = cell_data_jsonString
            Expect.equal actual expected ""
        testCase "Data empty" <| fun _ ->
            let actual = CompositeCell.encoder (CompositeCell.Data(Data())) |> Encode.toJsonString  0
            let expected = cell_data_empty_jsonString
            Expect.equal actual expected ""
    ]
    testList "decoder (fromJsonString)" [ 
        testCase "FreeText" <| fun _ -> 
            let actual = Decode.fromJsonString CompositeCell.decoder cell_freetext_jsonString
            let expected = cell_freetext
            Expect.equal actual expected ""
        testCase "Term" <| fun _ -> 
            let actual = Decode.fromJsonString CompositeCell.decoder cell_term_jsonString
            let expected = cell_term
            Expect.equal actual expected ""
        testCase "Term empty" <| fun _ -> 
            let actual = Decode.fromJsonString CompositeCell.decoder cell_term_empty_jsonString
            let expected = cell_term_empty
            Expect.equal actual expected ""
        testCase "Unitized" <| fun _ -> 
            let actual = Decode.fromJsonString CompositeCell.decoder cell_unitized_jsonString
            let expected = cell_unitized
            Expect.equal actual expected ""
        testCase "Unitized empty" <| fun _ -> 
            let actual = Decode.fromJsonString CompositeCell.decoder cell_unitized_empty_jsonString
            let expected = cell_unitized_empty
            Expect.equal actual expected ""
        testCase "Data" <| fun _ ->
            let actual = Decode.fromJsonString CompositeCell.decoder cell_data_jsonString
            let expected = CompositeCell.Data(Data("MyID","MyName",Process.DataFile.RawDataFile,"text/csv","MySelector",ResizeArray [Comment.create("MyKey","MyValue")]))
            Expect.equal actual expected ""
        testCase "Data empty" <| fun _ ->
            let actual = Decode.fromJsonString CompositeCell.decoder cell_data_empty_jsonString
            let expected = CompositeCell.Data(Data())
            Expect.equal actual expected ""
    ]
]

let main = testList "CompositeCell" [
    tests_extended
]