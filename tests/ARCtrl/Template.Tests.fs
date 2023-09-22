module ARCtrl.Template.Tests

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

open ARCtrl.Template.Json
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

let tests_Spreadsheet =  testList "Template_Spreadsheet" [
        testList "metadata" [
            testCase "simple" <| fun _ ->
                let templateInfo,ers,tags,authors = Spreadsheet.Template.fromMetadataSheet TestObjects.Spreadsheet.Template.templateMetadata

                Expect.equal templateInfo.Id "f12e98ee-a4e7-4ada-ba56-1e13cce1a44b" "Id"
                Expect.equal templateInfo.Name "Plant growth" "Name"
                Expect.equal templateInfo.Version "1.2.0" "Version"
                Expect.equal templateInfo.Description "Template to describe a plant growth study as well as sample collection and handling." "Description"
                Expect.equal templateInfo.Organisation "DataPLANT" "Organisation"
                Expect.equal templateInfo.Table TestObjects.Spreadsheet.Template.templateTableName "Table"

                Expect.equal ers.Length 5 "Should be 5 endpoint repositories"
                let expectedThird = OntologyAnnotation.fromString(termName = "PRIDE",tsr = "NFDI4PSO", tan = "NFDI4PSO:1000098")
                Expect.equal ers.[2] expectedThird "Third ER should be equal"

                Expect.equal tags.Length 5 "Should be 5 tags"
                let expectedFirst = OntologyAnnotation.fromString(termName = "Plants",tsr = "", tan = "")
                Expect.equal tags.[0] expectedFirst "First TAG should be equal"
                let expectedLast = OntologyAnnotation.fromString(termName = "Plant Sample Checklist",tsr = "", tan = "")
                Expect.equal tags.[4] expectedLast "Last TAG should be equal"

                Expect.equal authors.Length 5 "Should be 5 authors"
                let expectedFirst = Person.create(FirstName = "Heinrich", LastName = "Weil", MidInitials = "L",ORCID = "0000-0003-1945-6342",Email = "weil@rptu.de")
                Expect.equal authors.[0] expectedFirst "First AUTHOR should be equal"
                let expectedFourth = Person.create(FirstName = "Martin", LastName = "Kuhl")
                Expect.equal authors.[3] expectedFourth "Fourth AUTHOR should be equal"
            testCase "complete roundabout" <| fun _ ->
                let templateInfo,ers,tags,authors = Spreadsheet.Template.fromMetadataSheet TestObjects.Spreadsheet.Template.templateMetadata

                let table = ArcTable.init(TestObjects.Spreadsheet.Template.templateTableName)
                let template = Spreadsheet.Template.fromParts templateInfo ers tags authors table System.DateTime.Now
                let sheet = Spreadsheet.Template.toMetadataSheet template

                Expect.workSheetEqual sheet TestObjects.Spreadsheet.Template.templateMetadata "Metadata sheet should be equal"

            testCase "deprecated roundabout" <| fun _ ->
                let templateInfo,ers,tags,authors = Spreadsheet.Template.fromMetadataSheet TestObjects.Spreadsheet.Template.templateMetadata_deprecatedKeys

                let table = ArcTable.init(TestObjects.Spreadsheet.Template.templateTableName)
                let template = Spreadsheet.Template.fromParts templateInfo ers tags authors table System.DateTime.Now
                let sheet = Spreadsheet.Template.toMetadataSheet template

                Expect.workSheetEqual sheet TestObjects.Spreadsheet.Template.templateMetadata "Metadata sheet should be equal"
        ]
        testList "fullFile" [
            testCase "simple" <| fun _ ->
                let template = Spreadsheet.Template.fromFsWorkbook TestObjects.Spreadsheet.Template.template
    
                Expect.equal template.Id (System.Guid "f12e98ee-a4e7-4ada-ba56-1e13cce1a44b") "Id"
                Expect.equal template.Name "Plant growth" "Name"
                Expect.equal template.Version "1.2.0" "Version"
                Expect.equal template.Description "Template to describe a plant growth study as well as sample collection and handling." "Description"
                Expect.equal template.Organisation (DataPLANT) "Organisation"

                let expectedTable = Spreadsheet.ArcTable.tryFromFsWorksheet TestObjects.Spreadsheet.Template.templateTable |> Option.get

                Expect.arcTableEqual template.Table expectedTable "Table"
            testCase "tableByTableName" <| fun _ ->
                let template = Spreadsheet.Template.fromFsWorkbook TestObjects.Spreadsheet.Template.template_matchingXLSXTableName
    
                Expect.equal template.Id (System.Guid "f12e98ee-a4e7-4ada-ba56-1e13cce1a44b") "Id"
                Expect.equal template.Name "Plant growth" "Name"
                Expect.equal template.Version "1.2.0" "Version"
                Expect.equal template.Description "Template to describe a plant growth study as well as sample collection and handling." "Description"
                Expect.equal template.Organisation (DataPLANT) "Organisation"

                let expectedTable = Spreadsheet.ArcTable.tryFromFsWorksheet TestObjects.Spreadsheet.Template.templateTableMatchingByTableName |> Option.get

                Expect.arcTableEqual template.Table expectedTable "Table"
            testCase "deprecatedSheetName" <| fun _ ->
                let template = Spreadsheet.Template.fromFsWorkbook TestObjects.Spreadsheet.Template.template_deprecatedMetadataSheetName
    
                Expect.equal template.Id (System.Guid "f12e98ee-a4e7-4ada-ba56-1e13cce1a44b") "Id"
                Expect.equal template.Name "Plant growth" "Name"
                Expect.equal template.Version "1.2.0" "Version"
                Expect.equal template.Description "Template to describe a plant growth study as well as sample collection and handling." "Description"
                Expect.equal template.Organisation (DataPLANT) "Organisation"

                let expectedTable = Spreadsheet.ArcTable.tryFromFsWorksheet TestObjects.Spreadsheet.Template.templateTable |> Option.get

                Expect.arcTableEqual template.Table expectedTable "Table"
            testCase "template_wrongTemplateTableName" <| fun _ ->
                let tryReadNonExistingTable = 
                    fun () ->
                        let template = Spreadsheet.Template.fromFsWorkbook TestObjects.Spreadsheet.Template.template_wrongTemplateTableName
                        ()
                Expect.throws tryReadNonExistingTable "Should fail, as neither xlsx table name nor sheet name match"
            
            testCase "roundabout" <| fun _ ->
                let template = 
                    Spreadsheet.Template.fromFsWorkbook TestObjects.Spreadsheet.Template.template
                    |> Spreadsheet.Template.toFsWorkbook
                    |> Spreadsheet.Template.fromFsWorkbook

                Expect.equal template.Id (System.Guid "f12e98ee-a4e7-4ada-ba56-1e13cce1a44b") "Id"
                Expect.equal template.Name "Plant growth" "Name"
                Expect.equal template.Version "1.2.0" "Version"
                Expect.equal template.Description "Template to describe a plant growth study as well as sample collection and handling." "Description"
                Expect.equal template.Organisation (DataPLANT) "Organisation"

                let expectedTable = Spreadsheet.ArcTable.tryFromFsWorksheet TestObjects.Spreadsheet.Template.templateTable |> Option.get

                Expect.arcTableEqual template.Table expectedTable "Table"

        ]
    ]

let private tests_json = testList "Json" [
    tests_Organisation
    //tests_CompositeCell
    //tests_CompositeHeader
    //tests_ArcTable
    tests_Template
    tests_Spreadsheet
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
            "My Template is great"
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

let private tests_Web = testList "Web" [
    testCaseAsync "getTemplates" <| async {
        let! templatesMap = ARCtrl.Template.Web.getTemplates(None)
        Expect.isTrue (templatesMap.Count > 0) "Count > 0"
    }
]

let main = testList "Templates" [
    tests_json
    tests_equality
    tests_Web
]