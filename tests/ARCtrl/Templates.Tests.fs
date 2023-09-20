module ARCtrl.Templates.Tests

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

open ARCtrl.Templates.Json
open ARCtrl.ISA

open TestingUtils

let private fableReplaceLineEndings(str: string) =
    str.Replace("\r\n","\n").Replace("\n\r","\n")

let private tests_Organisation = testList "Organisation" [
    testList "encode" [
        testCase "DataPLANT" <| fun _ ->
            let o = Organisation.DataPLANT
            let actual = Organisation.encode o |> Encode.toString 4
            let expected = "\"DataPLANT\""
            Expect.equal actual expected ""
        testCase "Other" <| fun _ ->
            let o = Organisation.Other "My Custom Org"
            let actual = Organisation.encode o |> Encode.toString 4
            let expected = "\"My Custom Org\""
            // replace line endings to normalize for all editors/environments.
            Expect.equal (fableReplaceLineEndings actual) (fableReplaceLineEndings expected) ""
    ]
    testList "decode" [
        testCase "DataPLANT" <| fun _ ->
            let json = "\"DataPLANT\""
            let actual = Decode.fromString Organisation.decode json
            let expected =  Ok (Organisation.DataPLANT)
            Expect.equal actual expected ""
        testCase "Other" <| fun _ ->
            let json = "\"My Custom Org\""
            let actual = Decode.fromString Organisation.decode json
            let expected =  Ok (Other "My Custom Org")
            Expect.equal actual expected ""
    ]
    testList "roundabout" [
        testCase "DataPLANT" <| fun _ ->
            let o = Organisation.DataPLANT
            let json = Organisation.encode o |> Encode.toString 4
            let actual = Decode.fromString Organisation.decode json
            let expected = Ok (o)
            Expect.equal actual expected ""
        testCase "Other" <| fun _ ->
            let o = Organisation.Other "My Custom Org"
            let json = Organisation.encode o |> Encode.toString 4
            let actual = Decode.fromString Organisation.decode json
            let expected = Ok (o)
            Expect.equal actual expected ""
    ]
]

//let private tests_CompositeCell = testList "CompositeCell" [
//        testList "roundabout" [
//            testCase "Freetext" <| fun _ ->
//                let o = CompositeCell.createFreeText "My Freetext"
//                let json = CompositeCell.encode o |> Encode.toString 4
//                let actual = Decode.fromString CompositeCell.decode json
//                let expected = Ok o
//                Expect.equal actual expected ""
//            testCase "Term" <| fun _ ->
//                let o = CompositeCell.createTermFromString "My Term"
//                let json = CompositeCell.encode o |> Encode.toString 4
//                let actual = Decode.fromString CompositeCell.decode json
//                let expected = Ok o
//                Expect.equal actual expected ""
//            testCase "Unitized" <| fun _ ->
//                let o = CompositeCell.createUnitizedFromString ("12","My Term")
//                let json = CompositeCell.encode o |> Encode.toString 4
//                let actual = Decode.fromString CompositeCell.decode json
//                let expected = Ok o
//                Expect.equal actual expected ""
//        ]
//    ]

//let private tests_CompositeHeader = testList "CompositeHeader" [
//        testList "roundabout" [
//            testCase "Input/Output" <| fun _ ->
//                let o = CompositeHeader.Input IOType.Source
//                let json = CompositeHeader.encode o |> Encode.toString 4
//                let actual = Decode.fromString CompositeHeader.decode json
//                let expected = Ok o
//                Expect.equal actual expected ""
//            testCase "Single" <| fun _ ->
//                let o = CompositeHeader.ProtocolREF
//                let json = CompositeHeader.encode o |> Encode.toString 4
//                let actual = Decode.fromString CompositeHeader.decode json
//                let expected = Ok o
//                Expect.equal actual expected ""
//            testCase "Term" <| fun _ ->
//                let o = CompositeHeader.Characteristic (OntologyAnnotation.fromString "My Chara")
//                let json = CompositeHeader.encode o |> Encode.toString 4
//                let actual = Decode.fromString CompositeHeader.decode json
//                let expected = Ok o
//                Expect.equal actual expected ""
//        ]
//    ]

//let tests_ArcTable = testList "ArcTable" [  
//        testList "roundabout" [
//            testCase "complete" <| fun _ ->
//                let o = ArcTable.init("My Table")
//                o.AddColumn(CompositeHeader.Input IOType.Source, [|for i in 0 .. 9 do yield CompositeCell.createFreeText($"Source {i}")|])
//                o.AddColumn(CompositeHeader.Output IOType.RawDataFile, [|for i in 0 .. 9 do yield CompositeCell.createFreeText($"Output {i}")|])
//                let json = Encode.toString 4 (ArcTable.encode o)
//                let actual = Decode.fromString ArcTable.decode json
//                let expected = Ok o
//                Expect.equal actual expected ""
//        ]
//    ]

let tests_Template = testList "Template" [
        testList "roundabout" [
            testCase "complete" <| fun _ ->
                let table = ArcTable.init("My Table")
                table.AddColumn(CompositeHeader.Input IOType.Source, [|for i in 0 .. 9 do yield CompositeCell.createFreeText($"Source {i}")|])
                table.AddColumn(CompositeHeader.Output IOType.RawDataFile, [|for i in 0 .. 9 do yield CompositeCell.createFreeText($"Output {i}")|])
                let o = Template.init("MyTemplate")
                o.Table <- table
                o.Authors <- [|ARCtrl.ISA.Person.create(FirstName="John", LastName="Doe"); ARCtrl.ISA.Person.create(FirstName="Jane", LastName="Doe");|]
                o.EndpointRepositories <- [|ARCtrl.ISA.OntologyAnnotation.fromString "Test"; ARCtrl.ISA.OntologyAnnotation.fromString "Testing second"|]
                let json = Encode.toString 4 (Template.encode o)
                let actual = Decode.fromString Template.decode json
                let expected = o
                let actualValue = Expect.wantOk actual "Ok"
                Expect.equal actualValue.Id expected.Id "id"
                Expect.equal actualValue.Authors expected.Authors "Authors"
                Expect.equal actualValue.EndpointRepositories expected.EndpointRepositories "EndpointRepositories"
                Expect.equal actualValue.LastUpdated expected.LastUpdated "LastUpdated"
                Expect.equal actualValue.Name expected.Name "Name"
                Expect.equal actualValue.Organisation expected.Organisation "Organisation"
                Expect.equal actualValue.SemVer expected.SemVer "SemVer"
                //printfn "ACTUAL: %A" actualValue.Table
                //printfn "EXPECTED: %A" expected.Table
                
                Expect.equal actualValue.Table.Name expected.Table.Name "Name should be equal"
                Expect.sequenceEqual actualValue.Table.Headers expected.Table.Headers "Headers should be equal"
                Expect.sequenceEqual actualValue.Table.Values expected.Table.Values "Headers should be equal"

                Expect.equal actualValue.Table.RowCount expected.Table.RowCount "RowCount should be equal"
                Expect.equal actualValue.Table.ColumnCount expected.Table.ColumnCount "ColumnCount should be equal"

                Expect.equal actualValue.Table expected.Table "Table"
                Expect.equal actualValue expected "template"
        ]
    ]

let private tests_json = testList "Json" [
    tests_Organisation
    //tests_CompositeCell
    //tests_CompositeHeader
    //tests_ArcTable
    tests_Template
]

let private tests_equality = testList "equality" [
    let create_TestTemplate() =
        let table = ArcTable.init("My Table")
        table.AddColumn(CompositeHeader.Input IOType.Source, [|for i in 0 .. 9 do yield CompositeCell.createFreeText($"Source {i}")|])
        table.AddColumn(CompositeHeader.Output IOType.RawDataFile, [|for i in 0 .. 9 do yield CompositeCell.createFreeText($"Output {i}")|])
        let guid = System.Guid(String.init 32 (fun _ -> "d"))
        Template.make 
            guid 
            table 
            "My Template" 
            DataPLANT 
            "1.0.3" 
            [|Person.create(FirstName="John", LastName="Doe")|] 
            [|OntologyAnnotation.fromString "My oa rep"|] 
            [|OntologyAnnotation.fromString "My oa tag"|]
            (System.DateTime(2023,09,19))

    testList "override equality" [
        testCase "equal" <| fun _ ->
            let template1 = create_TestTemplate()
            let template2 = create_TestTemplate()
            Expect.equal template1 template2 "equal"
        testCase "not equal" <| fun _ ->
            let template1 = create_TestTemplate()
            let template2 = create_TestTemplate()
            template2.Name <- "New Name"
            Expect.notEqual template1 template2 "not equal"
    ]
    testList "structural equality" [
        testCase "equal" <| fun _ ->
            let template1 = create_TestTemplate()
            let template2 = create_TestTemplate()
            let equals = template1.StructurallyEquals(template2)
            Expect.isTrue equals "equal"
        testCase "not equal" <| fun _ ->
            let template1 = create_TestTemplate()
            let template2 = create_TestTemplate()
            template2.Name <- "New Name"
            let equals = template1.StructurallyEquals(template2)
            Expect.isFalse equals "not equal"
    ]
    testList "reference equality" [
        testCase "not same object" <| fun _ ->
            let template1 = create_TestTemplate()
            let template2 = create_TestTemplate()
            let equals = template1.ReferenceEquals(template2)
            Expect.isFalse equals ""
        testCase "same object" <| fun _ ->
            let template1 = create_TestTemplate()
            let equals = template1.ReferenceEquals(template1)
            Expect.isTrue equals ""
    ]
]

let main = testList "Templates" [
    tests_json
    tests_equality
]