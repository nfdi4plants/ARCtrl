module Tests.Template

open ARCtrl
open ARCtrl.Json
open TestingUtils


let private fableReplaceLineEndings(str: string) =
    str.Replace("\r\n","\n").Replace("\n\r","\n")

let tests_Organisation = testList "Organisation" [
    testList "encode" [
        testCase "DataPLANT" <| fun _ ->
            let o = Organisation.DataPLANT
            let actual = Template.Organisation.encoder o |> Encode.toJsonString 4
            let expected = "\"DataPLANT\""
            Expect.equal actual expected ""
        testCase "Other" <| fun _ ->
            let o = Organisation.Other "My Custom Org"
            let actual = Template.Organisation.encoder o |> Encode.toJsonString 4
            let expected = "\"My Custom Org\""
            // replace line endings to normalize for all editors/environments.
            Expect.equal (fableReplaceLineEndings actual) (fableReplaceLineEndings expected) ""
    ]
    testList "decode" [
        testCase "DataPLANT" <| fun _ ->
            let json = "\"DataPLANT\""
            let actual = Decode.fromJsonString Template.Organisation.decoder json
            let expected =  Organisation.DataPLANT
            Expect.equal actual expected ""
        testCase "Other" <| fun _ ->
            let json = "\"My Custom Org\""
            let actual = Decode.fromJsonString Template.Organisation.decoder json
            let expected =  Other "My Custom Org"
            Expect.equal actual expected ""
    ]
    testList "roundabout" [
        testCase "DataPLANT" <| fun _ ->
            let o = Organisation.DataPLANT
            let json = Template.Organisation.encoder o |> Encode.toJsonString 4
            let actual = Decode.fromJsonString Template.Organisation.decoder json
            let expected = o
            Expect.equal actual expected ""
        testCase "Other" <| fun _ ->
            let o = Organisation.Other "My Custom Org"
            let json = Template.Organisation.encoder o |> Encode.toJsonString 4
            let actual = Decode.fromJsonString Template.Organisation.decoder json
            let expected = o
            Expect.equal actual expected ""
    ]
]

let tests_Template = 
    testList "roundabout" [
        testCase "complete" <| fun _ ->
            let table = ArcTable.init("My Table")
            table.AddColumn(CompositeHeader.Input IOType.Source, [|for i in 0 .. 9 do yield CompositeCell.createFreeText($"Source {i}")|])
            table.AddColumn(CompositeHeader.Output IOType.RawDataFile, [|for i in 0 .. 9 do yield CompositeCell.createFreeText($"Output {i}")|])
            let o = Template.init("MyTemplate")
            o.Table <- table
            o.Authors <- ResizeArray [|ARCtrl.Person.create(firstName="John", lastName="Doe"); ARCtrl.Person.create(firstName="Jane", lastName="Doe");|]
            o.EndpointRepositories <- ResizeArray [|ARCtrl.OntologyAnnotation("Test"); ARCtrl.OntologyAnnotation("Testing second")|]
            let json = Template.toJsonString(4) o
            let actual = Template.fromJsonString json
            let expected = o
            Expect.equal actual.Id expected.Id "id"
            Expect.sequenceEqual actual.Authors expected.Authors "Authors"
            Expect.sequenceEqual actual.EndpointRepositories expected.EndpointRepositories "EndpointRepositories"
            Expect.dateTimeEqual actual.LastUpdated expected.LastUpdated "LastUpdated"
            Expect.equal actual.Name expected.Name "Name"
            Expect.equal actual.Organisation expected.Organisation "Organisation"
            Expect.equal actual.SemVer expected.SemVer "SemVer"
            //printfn "ACTUAL: %A" actualValue.Table
            //printfn "EXPECTED: %A" expected.Table
                
            Expect.equal actual.Table.Name expected.Table.Name "Name should be equal"
            Expect.sequenceEqual actual.Table.Headers expected.Table.Headers "Headers should be equal"
            Expect.sequenceEqual actual.Table.Values expected.Table.Values "Headers should be equal"

            Expect.equal actual.Table.RowCount expected.Table.RowCount "RowCount should be equal"
            Expect.equal actual.Table.ColumnCount expected.Table.ColumnCount "ColumnCount should be equal"

            Expect.equal actual.Table expected.Table "Table"
            Expect.equal actual expected "template"
    ]


let main = testList "Template" [
    tests_Organisation
    tests_Template
]