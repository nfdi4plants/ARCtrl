module DataModel.Tests

open TestingUtils
open ARCtrl.ISA


let componentCastingTests =

    testList "ComponentCastingTests" [
        testCase "ComposeNameText" (fun () -> 
            
            let v = Value.Name "Test" |> Some
            let u = None
            let expected = "Test"

            let actual = Component.composeName v u

            Expect.equal actual expected "Name was not correctly composed"

        )
        testCase "ComposeNameOntology" (fun () -> 
            
            let v = OntologyAnnotation.fromString ("Test", "OBO", "OBO:123") |> Value.Ontology |> Some 
            let u = None
            let expected = "Test (OBO:123)"

            let actual = Component.composeName v u

            Expect.equal actual expected "Name was not correctly composed"

        )
        testCase "ComposeNameUnit" (fun () -> 
            
            let v = Value.Int 10 |> Some
            let u = OntologyAnnotation.fromString ("degree Celsius", "UO", "UO:123") |> Some
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
        testCase "DecomposeNameOntology" (fun () -> 
            
            let n = "Test (OBO:123)"

            let actualV, actualU = Component.decomposeName n

            let expectedV = OntologyAnnotation.fromString ("Test", "OBO", "OBO:123") |> Value.Ontology

            let expectedU = None

            Expect.equal actualV expectedV "Name was not correctly decomposed"
            Expect.equal actualU expectedU "Unit was not correctly decomposed"
        )
        testCase "DecomposeNameOntologyWithSpaces" (fun () -> 
            
            let n = "Test This Stuff (OBO:123)"

            let actualV, actualU = Component.decomposeName n

            let expectedV = OntologyAnnotation.fromString ("Test This Stuff", "OBO", "OBO:123") |> Value.Ontology

            let expectedU = None

            Expect.equal actualV expectedV "Name was not correctly decomposed"
            Expect.equal actualU expectedU "Unit was not correctly decomposed"


        )
        testCase "DecomposeNameUnit" (fun () -> 
            
            let n = "10 degree Celsius (UO:123)"

            let actualV, actualU = Component.decomposeName n

            let expectedV = Value.Int 10

            let expectedU = OntologyAnnotation.fromString ("degree Celsius", "UO", "UO:123") |> Some

            Expect.equal actualV expectedV "Name was not correctly decomposed"
            Expect.equal actualU expectedU "Unit was not correctly decomposed"

        )
        testCase "FromStringText" (fun () -> 
            
            let v = Value.Name "Text" |> Some

            let expected = Component.make (Some "Text") v None None

            let actual = Component.fromString ("Text")

            Expect.equal actual expected "Component was not correctly composed"
        )
        testCase "FromStringTextWithCategory" (fun () -> 
            
            let v = Value.Name "Text" |> Some

            let header = OntologyAnnotation.fromString ("TestCategory", "CO", "CO:567") |> Some 

            let expected = Component.make (Some "Text") v None header

            let actual = Component.fromString ("Text", "TestCategory", "CO", "CO:567")

            Expect.equal actual expected "Component was not correctly composed"

        )
        testCase "FromStringOntology" (fun () -> 
            
            let v = OntologyAnnotation.fromString ("Test", "OBO", "OBO:123") |> Value.Ontology |> Some 

            let expected = Component.make (Some "Test (OBO:123)") v None None

            let actual = Component.fromString ("Test (OBO:123)")

            Expect.equal actual expected "Component was not correctly composed"

        )
        testCase "FromStringUnit" (fun () -> 
          
            let v = Value.Int 10 |> Some
            let u = OntologyAnnotation.fromString ("degree Celsius", "UO", "UO:123") |> Some
            let expected = Component.make (Some "10 degree Celsius (UO:123)") v u None

            let actual = Component.fromString ("10 degree Celsius (UO:123)")

            Expect.equal actual expected "Component was not correctly composed"

        )
        testCase "FromOptionsText" (fun () -> 
            
            let v = Value.Name "Text" |> Some

            let expected = Component.make (Some "Text") v None None

            let actual = Component.fromOptions v None None

            Expect.equal actual expected "Component was not correctly composed"
        )
        testCase "FromOptionsTextWithCategory" (fun () -> 
            
            let v = Value.Name "Text" |> Some

            let header = OntologyAnnotation.fromString ("TestCategory", "CO", "CO:567") |> Some 

            let expected = Component.make (Some "Text") v None header

            let actual = Component.fromOptions v None header

            Expect.equal actual expected "Component was not correctly composed"

        )
        testCase "FromOptionsOntology" (fun () -> 
            
            let v = OntologyAnnotation.fromString ("Test", "OBO", "OBO:123") |> Value.Ontology |> Some 

            let expected = Component.make (Some "Test (OBO:123)") v None None

            let actual = Component.fromOptions v None None

            Expect.equal actual expected "Component was not correctly composed"

        )
        testCase "FromOptionsUnit" (fun () -> 
          
            let v = Value.Int 10 |> Some
            let u = OntologyAnnotation.fromString ("degree Celsius", "UO", "UO:123") |> Some
            let expected = Component.make (Some "10 degree Celsius (UO:123)") v u None

            let actual = Component.fromOptions v u None

            Expect.equal actual expected "Component was not correctly composed"

        )
    ]

let ontologyAnnotationTests = 
    testList "ontologyAnnotationTests" [
        let short = "EFO:0000721"
        let uri = "http://purl.obolibrary.org/obo/EFO_0000721" 
        let otherParseable = "http://www.ebi.ac.uk/efo/EFO_0000721"
        let other = "Unparseable"
        testList "fromString" [
            

            testCase "FromShort" (fun () ->             
                let oa = OntologyAnnotation.fromString(tan = short)

                Expect.equal oa.TermAccessionString short "TAN incorrect"
                Expect.equal oa.TermAccessionShort short "short TAN incorrect"
                Expect.equal oa.TermAccessionOntobeeUrl uri "short TAN incorrect"
            )
            testCase "FromUri" (fun () ->          
                let oa = OntologyAnnotation.fromString(tan = uri)
                Expect.equal oa.TermAccessionString uri "TAN incorrect"
                Expect.equal oa.TermAccessionShort short "short TAN incorrect"
                Expect.equal oa.TermAccessionOntobeeUrl uri "short TAN incorrect"
            )
            testCase "FromOtherParseable" (fun () ->          
                let oa = OntologyAnnotation.fromString(tan = otherParseable)
                Expect.equal oa.TermAccessionString otherParseable "TAN incorrect"
                Expect.equal oa.TermAccessionShort short "short TAN incorrect"
                Expect.equal oa.TermAccessionOntobeeUrl uri "short TAN incorrect"
            )
            testCase "FromOther" (fun () ->          
                let oa = OntologyAnnotation.fromString(tan = other)
                Expect.equal oa.TermAccessionString other "TAN incorrect"
            )
            testCase "FromOtherWithTSR" (fun () ->          
                let tsr = "ABC"
                let oa = OntologyAnnotation.fromString(tsr = tsr,tan = other)
                Expect.equal oa.TermAccessionString other "TAN incorrect"
                Expect.equal oa.TermSourceREFString tsr "TSR incorrect"
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
        testCase "FromStringText" (fun () -> 
            
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
            let actual = Value.getText (OntologyAnnotation.fromString ("Test", "OBO", "OBO:123") |> Value.Ontology)
            Expect.equal actual expected "Value was not correctly composed"
        )
    ]

let main =
    testList "DataModelTests" [
        ontologyAnnotationTests
        componentCastingTests
        valueTests
    ]