module DataModel.Tests

open TestingUtils
open ARCtrl
open ARCtrl.Process
open ARCtrl.Helper

/// Tests to test whether the accessors, that handel different cases of registration work properly
let registrationAccessTests = 
    testList "RegistrationAccessTests" [
        let studyOnlyRegistered = "StudyOnlyRegistered"
        let studyOnlyExisting = "StudyOnlyExisting"
        let studyRegisteredAndExisting = "StudyRegisteredAndExisting"

        let assayOnlyRegistered = "AssayOnlyRegistered"
        let assayOnlyExisting = "AssayOnlyExisting"
        let assayRegisteredAndExisting = "AssayRegisteredAndExisting"

        
        let i = ArcInvestigation("MyInvestigation", registeredStudyIdentifiers = ResizeArray [studyOnlyRegistered])

        let s = ArcStudy(studyRegisteredAndExisting, registeredAssayIdentifiers = ResizeArray [assayOnlyRegistered])

        i.AddRegisteredStudy(s)
        i.AddStudy(ArcStudy(studyOnlyExisting))

        s.AddRegisteredAssay(ArcAssay(assayRegisteredAndExisting))
        i.AddAssay(ArcAssay(assayOnlyExisting))

        testCase "RegisteredStudyIdentifiers" (fun () ->
            let actual = i.RegisteredStudyIdentifiers
            let expected = ResizeArray [studyOnlyRegistered; studyRegisteredAndExisting]
            Expect.sequenceEqual actual expected "Study identifiers were not correctly retrieved"
        )
        testCase "RegisteredStudies" (fun () -> 
            let actual = i.RegisteredStudies |> ResizeArray.map (fun s -> s.Identifier)
            let expected = ResizeArray [studyRegisteredAndExisting]
            Expect.sequenceEqual actual expected "Registered studies were not correctly retrieved"
        )
        testCase "VacantStudyIdentifers" (fun () -> 
            let actual = i.VacantStudyIdentifiers
            let expected = ResizeArray [studyOnlyRegistered]
            Expect.sequenceEqual actual expected "Vacant study identifiers were not correctly retrieved"
        )
        testCase "UnregisteredStudies" (fun () -> 
            let actual = i.UnregisteredStudies |> ResizeArray.map (fun s -> s.Identifier)
            let expected = ResizeArray [studyOnlyExisting]
            Expect.sequenceEqual actual expected "Unregistered studies were not correctly retrieved"
        )
        testCase "RegisteredAssayIdentifiers" (fun  () ->
            let actual = s.RegisteredAssayIdentifiers
            let expected = ResizeArray [assayOnlyRegistered; assayRegisteredAndExisting]
            Expect.sequenceEqual actual expected "Assay identifiers were not correctly retrieved"
        )   
        testCase "RegisteredAssays" (fun () -> 
            let actual = s.RegisteredAssays |> ResizeArray.map (fun s -> s.Identifier)
            let expected = ResizeArray [assayRegisteredAndExisting]
            Expect.sequenceEqual actual expected "Registered assays were not correctly retrieved"
        )
        testCase "VacantAssayIdentifers" (fun () -> 
            let actual = s.VacantAssayIdentifiers
            let expected = ResizeArray [assayOnlyRegistered]
            Expect.sequenceEqual actual expected "Vacant assay identifiers were not correctly retrieved"
        )
        testCase "UnregisteredAssays" (fun () -> 
            let actual = i.UnregisteredAssays |> ResizeArray.map (fun s -> s.Identifier)
            let expected = ResizeArray [assayOnlyExisting]
            Expect.sequenceEqual actual expected "Unregistered assays were not correctly retrieved"
        )


    ]

let componentCastingTests =

    testList "ComponentCastingTests" [
        testCase "ComposeNameText" (fun () -> 
            
            let v = Value.Name "Test"
            let u = None
            let expected = "Test"

            let actual = Component.composeName v u

            Expect.equal actual expected "Name was not correctly composed"

        )
        testCase "ComposeNameOntology" (fun () -> 
            
            let v = OntologyAnnotation("Test", "OBO", "OBO:123") |> Value.Ontology
            let u = None
            let expected = "Test (OBO:123)"

            let actual = Component.composeName v u

            Expect.equal actual expected "Name was not correctly composed"

        )
        testCase "ComposeNameUnit" (fun () -> 
            
            let v = Value.Int 10
            let u = OntologyAnnotation("degree Celsius", "UO", "UO:123") |> Some
            let expected = "10 degree Celsius (UO:123)"

            let actual = Component.composeName v u

            Expect.equal actual expected "Name was not correctly composed"

        )
        testCase "DecomposeNameText" (fun () -> 
            
            let n = "Test"

            let actualV, actualU = Component.decomposeName n

            let expectedV = Value.Name "Test"

            let expectedU = None

            Expect.equal actualV expectedV "Name was not correctly decomposed"
            Expect.equal actualU expectedU "Unit was not correctly decomposed"

        )
        testCase "DecomposeNameWithBrackets" (fun () -> 
            
            let n = "Test (This component is very important)"

            let actualV, actualU = Component.decomposeName n

            let expectedV = Value.Name "Test (This component is very important)"

            let expectedU = None

            Expect.equal actualV expectedV "Name was not correctly decomposed"
            Expect.equal actualU expectedU "Unit was not correctly decomposed"

        )
        testCase "DecomposeNameOntology" (fun () -> 
            
            let n = "Test (OBO:123)"

            let actualV, actualU = Component.decomposeName n

            let expectedV = OntologyAnnotation("Test", "OBO", "OBO:123") |> Value.Ontology

            let expectedU = None

            Expect.equal actualV expectedV "Name was not correctly decomposed"
            Expect.equal actualU expectedU "Unit was not correctly decomposed"
        )
        testCase "DecomposeNameOntologyEmpty" (fun () -> 
            
            let n = "Test ()"

            let actualV, actualU = Component.decomposeName n

            let expectedV = OntologyAnnotation("Test") |> Value.Ontology

            let expectedU = None

            Expect.equal actualV expectedV "Name was not correctly decomposed"
            Expect.equal actualU expectedU "Unit was not correctly decomposed"
        )
        testCase "DecomposeNameOntologyWithSpaces" (fun () -> 
            
            let n = "Test This Stuff (OBO:123)"

            let actualV, actualU = Component.decomposeName n

            let expectedV = OntologyAnnotation("Test This Stuff", "OBO", "OBO:123") |> Value.Ontology

            let expectedU = None

            Expect.equal actualV expectedV "Name was not correctly decomposed"
            Expect.equal actualU expectedU "Unit was not correctly decomposed"


        )
        testCase "DecomposeNameUnit" (fun () -> 
            
            let n = "10 degree Celsius (UO:123)"

            let actualV, actualU = Component.decomposeName n

            let expectedV = Value.Int 10

            let expectedU = OntologyAnnotation("degree Celsius", "UO", "UO:123") |> Some

            Expect.equal actualV expectedV "Name was not correctly decomposed"
            Expect.equal actualU expectedU "Unit was not correctly decomposed"

        )
        testCase "FromStringText" (fun () -> 
            
            let v = Value.Name "Text" |> Some

            let expected = Component.make v None None

            let actual = Component.fromISAString ("Text")

            Expect.equal actual expected "Component was not correctly composed"
        )
        testCase "FromStringTextWithCategory" (fun () -> 
            
            let v = Value.Name "Text" |> Some

            let header = OntologyAnnotation("TestCategory", "CO", "CO:567") |> Some 

            let expected = Component.make v None header

            let actual = Component.fromISAString ("Text", "TestCategory", "CO", "CO:567")

            Expect.equal actual expected "Component was not correctly composed"

        )
        testCase "FromStringOntology" (fun () -> 
            
            let v = OntologyAnnotation("Test", "OBO", "OBO:123") |> Value.Ontology |> Some 

            let expected = Component.make v None None

            let actual = Component.fromISAString("Test (OBO:123)")

            Expect.equal actual expected "Component was not correctly composed"

        )
        testCase "FromStringUnit" (fun () -> 
          
            let v = Value.Int 10 |> Some
            let u = OntologyAnnotation("degree Celsius", "UO", "UO:123") |> Some
            let expected = Component.make  v u None

            let actual = Component.fromISAString("10 degree Celsius (UO:123)")

            Expect.equal actual expected "Component was not correctly composed"

        )
    ]

let ontologyAnnotationTests = 
    testList "ontologyAnnotationTests" [
        let short = "EFO:0000721"
        let uri = "http://purl.obolibrary.org/obo/EFO_0000721" 
        let otherParseable = "http://www.ebi.ac.uk/efo/EFO_0000721"
        let other = "Unparseable"
        testList "GetHashCode" [
            testCase "Empty" (fun () ->
                let oa1 = OntologyAnnotation()
                let oa2 = OntologyAnnotation()
                let h1 = oa1.GetHashCode()
                let h2 = oa2.GetHashCode()
                Expect.equal h1 h2 "Hashes should be equal"    
            )
            testCase "Equal" (fun () ->
                let oa1 = OntologyAnnotation("MyOntology",tsr = "EFO",tan = uri)
                let oa2 = OntologyAnnotation("MyOntology",tsr = "EFO",tan = uri)
                let h1 = oa1.GetHashCode()
                let h2 = oa2.GetHashCode()
                Expect.equal h1 h2 "Hashes should be equal"
            )
            testCase "Different" (fun () ->
                let oa1 = OntologyAnnotation("MyOntology",tsr = "EFO",tan = uri)
                let oa2 = OntologyAnnotation("YourOntology",tsr = "NCBI",tan = "http://purl.obolibrary.org/obo/NCBI_0000123")
                let h1 = oa1.GetHashCode()
                let h2 = oa2.GetHashCode()
                Expect.notEqual h1 h2 "Hashes should not be equal"
            )
        ]
        testList "fromString" [
            
            testCase "FromShort" (fun () ->
                let oa = OntologyAnnotation(tan = short)
                Expect.equal oa.TermAccessionNumber.Value short "TAN incorrect"
                Expect.equal oa.TermAccessionShort short "short TAN incorrect"
                Expect.equal oa.TermAccessionOntobeeUrl uri "short TAN incorrect"
            )
            testCase "FromUri" (fun () ->          
                let oa = OntologyAnnotation(tan = uri)
                Expect.equal oa.TermAccessionNumber.Value uri "TAN incorrect"
                Expect.equal oa.TermAccessionShort short "short TAN incorrect"
                Expect.equal oa.TermAccessionOntobeeUrl uri "short TAN incorrect"
            )
            testCase "FromOtherParseable" (fun () ->          
                let oa = OntologyAnnotation(tan = otherParseable)
                Expect.equal oa.TermAccessionNumber.Value otherParseable "TAN incorrect"
                Expect.equal oa.TermAccessionShort short "short TAN incorrect"
                Expect.equal oa.TermAccessionOntobeeUrl uri "short TAN incorrect"
            )
            testCase "FromOther" (fun () ->          
                let oa = OntologyAnnotation(tan = other)
                Expect.equal oa.TermAccessionNumber.Value other "TAN incorrect"
            )
            testCase "FromOtherWithTSR" (fun () ->          
                let tsr = "ABC"
                let oa = OntologyAnnotation(tsr = tsr,tan = other)
                Expect.equal oa.TermAccessionNumber.Value other "TAN incorrect"
                Expect.equal oa.TermSourceREF.Value tsr "TSR incorrect"
            )
        ]
    ]

let valueTests = 

    testList "ValueTests" [
        testCase "FromStringInt" (fun () -> 
            
            let expected = Value.Int 10
            let actual = Value.fromString "10"
            Expect.equal actual expected "Value was not correctly composed"
        )
        testCase "FromStringFloat" (fun () -> 
            
            let expected = Value.Float 10.0
            let actual = Value.fromString "10.0"
            Expect.equal actual expected "Value was not correctly composed"
        )

        // Was changed to make table parsing more straightforward in cases, where only the name of an ontology is given. Here in the case of read -> write, an ontology should be again returned
        // As the Value type is not used outside of term triplets, we can always assume that the Value is an ontology or at least a number 
        ptestCase "FromStringText" (fun () -> 
            
            let expected = Value.Name "Test"
            let actual = Value.fromString "Test"
            Expect.equal actual expected "Value was not correctly composed"
        )
        testCase "ToStringInt" (fun () -> 
            
            let expected = "10"
            let actual = Value.getText (Value.Int 10)
            Expect.equal actual expected "Value was not correctly composed"
        )
        testCase "ToStringFloat" (fun () -> 
            
            let expected = "10"
            let actual = Value.getText (Value.Float 10.0)
            Expect.equal actual expected "Value was not correctly composed"
        )
        testCase "ToStringText" (fun () -> 
            
            let expected = "Test"
            let actual = Value.getText (Value.Name "Test")
            Expect.equal actual expected "Value was not correctly composed"
        )
        testCase "ToStringOntology" (fun () -> 
            
            let expected = "Test"
            let actual = Value.getText (OntologyAnnotation("Test", "OBO", "OBO:123") |> Value.Ontology)
            Expect.equal actual expected "Value was not correctly composed"
        )
    ]

let main =
    testList "DataModelTests" [
        ontologyAnnotationTests
        componentCastingTests
        valueTests
    ]