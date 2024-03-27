module ARCtrl.Template.Tests

open Thoth.Json.Core


open ARCtrl.Json
open ARCtrl

open TestingUtils


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
            let expectedFirst = Person.create(firstName = "Heinrich", lastName = "Weil", MidInitials = "L",ORCID = "0000-0003-1945-6342",Email = "weil@rptu.de")
            Expect.equal authors.[0] expectedFirst "First AUTHOR should be equal"
            let expectedFourth = Person.create(firstName = "Martin", lastName = "Kuhl")
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

let private tests_filters = testList "filters" [
    let create_TestTemplate() =
        let guid = System.Guid(String.init 32 (fun _ -> "d"))
        Template.make 
            guid 
            (ArcTable.init("TestTable")) 
            "My Template" 
            "My Template is great"
            DataPLANT 
            "1.0.3" 
            [|Person.create(FirstName="John", LastName="Doe")|] 
            [|OntologyAnnotation.fromString "PRIDE";|]
            [|OntologyAnnotation.fromString "Protein"; OntologyAnnotation.fromString "DNA";|] 
            (System.DateTime(2023,09,19))
    // this testList is representative to filterByEndpointRepositories
    testList "filterByTags" [
        testCase "OR, contains all" <| fun _ -> 
            let templates = [|create_TestTemplate()|]
            let queryTags = [|OntologyAnnotation.fromString "DNA"|]
            let actual = Templates.filterByTags(queryTags) templates
            let expected = 1
            Expect.equal actual.Length expected ""
        testCase "OR, contains different" <| fun _ -> 
            let templates = [|create_TestTemplate()|]
            let queryTags = [|OntologyAnnotation.fromString "DNA"; OntologyAnnotation.fromString "RNA"|]
            let actual = Templates.filterByTags(queryTags) templates
            let expected = 1
            Expect.equal actual.Length expected ""
        testCase "AND, contains some" <| fun _ -> 
            let templates = [|create_TestTemplate()|]
            let queryTags = [|OntologyAnnotation.fromString "DNA"|]
            let actual = Templates.filterByTags(queryTags, true) templates
            let expected = 1
            Expect.equal actual.Length expected ""
        testCase "AND, contains all" <| fun _ -> 
            let templates = [|create_TestTemplate()|]
            let queryTags = [|OntologyAnnotation.fromString "DNA"; OntologyAnnotation.fromString "Protein";|]
            let actual = Templates.filterByTags(queryTags, true) templates
            let expected = 1
            Expect.equal actual.Length expected ""
        testCase "AND, contains different" <| fun _ -> 
            let templates = [|create_TestTemplate()|]
            let queryTags = [|OntologyAnnotation.fromString "DNA"; OntologyAnnotation.fromString "RNA"|]
            let actual = Templates.filterByTags(queryTags, true) templates
            let expected = 0
            Expect.equal actual.Length expected ""
    ]
    testList "filterByOntologyAnnotations" [
        testCase "OR, contains tag" <| fun _ -> 
            let templates = [|create_TestTemplate()|]
            let queryTags = [|OntologyAnnotation.fromString "DNA"|]
            let actual = Templates.filterByOntologyAnnotation(queryTags) templates
            let expected = 1
            Expect.equal actual.Length expected ""
        testCase "OR, contains er" <| fun _ -> 
            let templates = [|create_TestTemplate()|]
            let queryTags = [|OntologyAnnotation.fromString "PRIDE"|]
            let actual = Templates.filterByOntologyAnnotation(queryTags) templates
            let expected = 1
            Expect.equal actual.Length expected ""
        testCase "OR, contains combined" <| fun _ -> 
            let templates = [|create_TestTemplate()|]
            let queryTags = [|OntologyAnnotation.fromString "PRIDE"; OntologyAnnotation.fromString "DNA"|]
            let actual = Templates.filterByOntologyAnnotation(queryTags) templates
            let expected = 1
            Expect.equal actual.Length expected ""
        testCase "OR, contains different" <| fun _ -> 
            let templates = [|create_TestTemplate()|]
            let queryTags = [|OntologyAnnotation.fromString "DNA"; OntologyAnnotation.fromString "RNA"|]
            let actual = Templates.filterByOntologyAnnotation(queryTags) templates
            let expected = 1
            Expect.equal actual.Length expected ""
        testCase "AND, contains tag" <| fun _ -> 
            let templates = [|create_TestTemplate()|]
            let queryTags = [|OntologyAnnotation.fromString "DNA"|]
            let actual = Templates.filterByOntologyAnnotation(queryTags, true) templates
            let expected = 1
            Expect.equal actual.Length expected ""
        testCase "AND, contains er" <| fun _ -> 
            let templates = [|create_TestTemplate()|]
            let queryTags = [|OntologyAnnotation.fromString "PRIDE"|]
            let actual = Templates.filterByOntologyAnnotation(queryTags, true) templates
            let expected = 1
            Expect.equal actual.Length expected ""
        testCase "AND, contains combined" <| fun _ -> 
            let templates = [|create_TestTemplate()|]
            let queryTags = [|OntologyAnnotation.fromString "PRIDE"; OntologyAnnotation.fromString "DNA"|]
            let actual = Templates.filterByOntologyAnnotation(queryTags, true) templates
            let expected = 1
            Expect.equal actual.Length expected ""
        testCase "AND, contains different" <| fun _ -> 
            let templates = [|create_TestTemplate()|]
            let queryTags = [|OntologyAnnotation.fromString "DNA"; OntologyAnnotation.fromString "RNA"|]
            let actual = Templates.filterByOntologyAnnotation(queryTags, true) templates
            let expected = 0
            Expect.equal actual.Length expected ""
    ]
]

let main = testList "Templates" [
    tests_json
    tests_equality
    tests_Web
    tests_filters
]