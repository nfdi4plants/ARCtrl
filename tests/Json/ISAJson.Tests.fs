module Tests.ISAJson

open TestingUtils
open ARCtrl

let private tests_arcAssay = 
    testList "ARCAssay" [

        let identifier = "MyIdentifier"
        let measurementType = OntologyAnnotation("Measurement Type")
        let technologyType = OntologyAnnotation("Technology Type")
        let technologyPlatformName = "Technology Platform"
        let technologyPlatformTSR = "ABC"
        let technologyPlatformTAN = "ABC:123"
        let technologyPlatform = OntologyAnnotation(technologyPlatformName,technologyPlatformTSR,technologyPlatformTAN)
        let technologyPlatformExpectedString = $"{technologyPlatformName} ({technologyPlatformTAN})"
        let t1 = singleRowSingleParam.Copy()
        let t2 = 
            singleRowDataInputWithCharacteristic.Copy() |> fun t -> ArcTable.create(tableName2, t.Headers, t.Values)
        let tables = ResizeArray[t1;t2] |> ArcTables
        let comments = [|Comment.create("Comment 1")|]
        let fullArcAssay =  ArcAssay.create(identifier, measurementType, technologyType, technologyPlatform, tables.Tables, Comments = comments)

        testCase "Identifier Set" (fun () ->
            let identifier = "MyIdentifier"
            let arcAssay = ArcAssay.create(identifier)
            let assay = arcAssay.ToAssay()
            Expect.isSome assay.FileName "Assay should have fileName" 
            let expectedFileName = Identifier.Assay.fileNameFromIdentifier identifier
            Expect.equal assay.FileName.Value expectedFileName "Assay fileName should match"
            let resultArcAssay = ArcAssay.fromAssay assay
            Expect.equal resultArcAssay.Identifier identifier "ArcAssay identifier should match"            
        )
        testCase "No Identifier Set" (fun () ->
            let identifier = Identifier.createMissingIdentifier()
            let arcAssay = ArcAssay.create(identifier)
            let assay = arcAssay.ToAssay()
            Expect.isNone assay.FileName "Assay should not have fileName" 
            let resultArcAssay = ArcAssay.fromAssay assay
            Expect.isTrue (Identifier.isMissingIdentifier resultArcAssay.Identifier) "ArcAssay identifier should be missing"
        )
        testCase "FullAssay ToAssay" (fun () ->
            let arcAssay = fullArcAssay.Copy()
            let assay = arcAssay.ToAssay()

            Expect.isSome assay.FileName "Assay should have fileName"
            let expectedFileName = Identifier.Assay.fileNameFromIdentifier identifier
            Expect.equal assay.FileName.Value expectedFileName "Assay fileName should match"
        
            Expect.isSome assay.MeasurementType "Assay should have measurementType"
            Expect.equal assay.MeasurementType.Value measurementType "Assay measurementType should match"
            
            Expect.isSome assay.TechnologyType "Assay should have technologyType"
            Expect.equal assay.TechnologyType.Value technologyType "Assay technologyType should match"

            Expect.isSome assay.TechnologyPlatform "Assay should have technologyPlatform"
            Expect.equal assay.TechnologyPlatform.Value technologyPlatformExpectedString "Assay technologyPlatform should match"

            Expect.isSome assay.ProcessSequence "Assay should have processes"
            Expect.equal assay.ProcessSequence.Value.Length 2 "Should have 2 processes"

            Expect.isSome assay.CharacteristicCategories "Assay should have characteristicCategories"
            Expect.equal assay.CharacteristicCategories.Value.Length 1 "Should have 1 characteristicCategory"

            Expect.isSome assay.DataFiles "Assay should have dataFiles"
            Expect.equal assay.DataFiles.Value.Length 2 "Should have 2 dataFiles"

            Expect.isSome assay.Comments "Assay should have comments"
            Expect.equal assay.Comments.Value.Length 1 "Should have 1 comment"                      
        )
        testCase "FullAssay ToAndFromAssay" (fun () ->
            let arcAssay = fullArcAssay.Copy()
            let assay = arcAssay.ToAssay()

            let resultingArcAssay = ArcAssay.fromAssay assay
            Expect.equal resultingArcAssay.Identifier arcAssay.Identifier "ArcAssay identifier should match"
            Expect.equal resultingArcAssay.MeasurementType arcAssay.MeasurementType "ArcAssay measurementType should match"
            Expect.equal resultingArcAssay.TechnologyType arcAssay.TechnologyType "ArcAssay technologyType should match"
            Expect.equal resultingArcAssay.TechnologyPlatform arcAssay.TechnologyPlatform "ArcAssay technologyPlatform should match"
            let expectedTables = [t1;t2] 
                
            Expect.sequenceEqual resultingArcAssay.Tables expectedTables "ArcAssay tables should match"
            Expect.sequenceEqual resultingArcAssay.Comments arcAssay.Comments "ArcAssay comments should match"
        )
    ]

let private tests_arcStudy = 
    testList "ARCStudy" [
        testCase "Identifier Set" (fun () ->
            let identifier = "MyIdentifier"       
            let arcStudy = ArcStudy.create(identifier)
            let study = arcStudy.ToStudy(ResizeArray())
            Expect.isSome study.Identifier "Study should have identifier" 
            Expect.equal study.Identifier.Value identifier "Study identifier should match"
            let resultArcStudy, resultArcAssays = ArcStudy.fromStudy study
            Expect.equal resultArcStudy.Identifier identifier "ArcStudy identifier should match"
            Expect.isEmpty resultArcAssays "ArcAssays should match"
        )
        testCase "No Identifier Set" (fun () ->
            let identifier = Identifier.createMissingIdentifier()
            let arcStudy = ArcStudy.create(identifier)
            let study = arcStudy.ToStudy(ResizeArray())
            Expect.isNone study.Identifier "Study should not have identifier" 
            let resultArcStudy, resultArcAssays = ArcStudy.fromStudy study
            Expect.isTrue (Identifier.isMissingIdentifier resultArcStudy.Identifier) "ArcStudy identifier should be missing"
            Expect.isEmpty resultArcAssays "ArcAssays should match"
        )
    ]



let private tests_arcInvestigation = 
    testList "ARCInvestigation" [
        testCase "Identifier Set" (fun () ->
            let identifier = "MyIdentifier"
            let arcInvestigation = ArcInvestigation.create(identifier)
            let investigation = arcInvestigation.ToInvestigation()
            Expect.isSome investigation.Identifier "Investigation should have identifier" 
            Expect.equal investigation.Identifier.Value identifier "Investigation identifier should match"
            let resultArcInvestigation = ArcInvestigation.fromInvestigation investigation
            Expect.equal resultArcInvestigation.Identifier identifier "ArcInvestigation identifier should match"
        )
        testCase "No Identifier Set" (fun () ->
            let identifier = Identifier.createMissingIdentifier()
            let arcInvestigation = ArcInvestigation.create(identifier)
            let investigation = arcInvestigation.ToInvestigation()
            Expect.isNone investigation.Identifier "Investigation should not have identifier" 
            let resultArcInvestigation = ArcInvestigation.fromInvestigation investigation
            Expect.isTrue (Identifier.isMissingIdentifier resultArcInvestigation.Identifier) "ArcInvestigation identifier should be missing"
        )
    ]