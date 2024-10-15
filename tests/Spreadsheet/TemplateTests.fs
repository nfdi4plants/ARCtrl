module TemplateTests


open ARCtrl
open TestingUtils
open ARCtrl.Spreadsheet

open TestObjects.Spreadsheet

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
            let expectedThird = OntologyAnnotation(name = "PRIDE",tsr = "NFDI4PSO", tan = "NFDI4PSO:1000098")
            Expect.equal ers.[2] expectedThird "Third ER should be equal"

            Expect.equal tags.Length 5 "Should be 5 tags"
            let expectedFirst = OntologyAnnotation(name = "Plants",tsr = "", tan = "")
            Expect.equal tags.[0] expectedFirst "First TAG should be equal"
            let expectedLast = OntologyAnnotation(name = "Plant Sample Checklist",tsr = "", tan = "")
            Expect.equal tags.[4] expectedLast "Last TAG should be equal"

            Expect.equal authors.Length 5 "Should be 5 authors"
            let expectedFirst = Person.create(firstName = "Heinrich", lastName = "Weil", midInitials = "L",orcid = "0000-0003-1945-6342",email = "weil@rptu.de")
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
        testCase "TestMetadataFromCollection" (fun _ ->

            let table = ArcTable.init(TestObjects.Spreadsheet.Template.templateTableName)
            let templateInfo, ers,tags,authors = Spreadsheet.Template.fromMetadataCollection (TestObjects.Spreadsheet.Template.templateetadataCollection table)

            let template = ARCtrl.Spreadsheet.Template.fromParts templateInfo ers tags authors table System.DateTime.Now

            Expect.stringEqual template.Name "Plant growth" "Name should be equal"
            Expect.stringEqual template.Version "1.2.0" "Version should be equal"

            Expect.isSome (template.EndpointRepositories.Item 0).TermAccessionNumber (ARCtrl.Helper.Url.createOAUri "DPBO" "1000096")
            Expect.isSome (template.EndpointRepositories.Item 0).TermAccessionNumber (ARCtrl.Helper.Url.createOAUri "NFDI4PSO" "1000097")
            Expect.isSome (template.EndpointRepositories.Item 0).TermAccessionNumber (ARCtrl.Helper.Url.createOAUri "NFDI4PSO" "1000098")
            Expect.isSome (template.EndpointRepositories.Item 0).TermAccessionNumber (ARCtrl.Helper.Url.createOAUri "NFDI4PSO" "0010002")
            Expect.isSome (template.EndpointRepositories.Item 0).TermAccessionNumber (ARCtrl.Helper.Url.createOAUri "DPBO" "0010000")

        )
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

let main = 
    testList "Template" [
        tests_Spreadsheet
    ]
