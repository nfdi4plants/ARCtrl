module DataModel.Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif
open ISADotNet


let main =

    testList "ComponentCastingTests" [
        testCase "ComposeNameText" (fun () -> 
            
            let v = Value.Name "Test" |> Some
            let u = None
            let expected = "Test"

            let actual = Component.composeName v u

            Expect.equal actual expected "Name was not correctly composed"

        )
        testCase "ComposeNameOntology" (fun () -> 
            
            let v = OntologyAnnotation.fromString "Test" "OBO" "OBO:123" |> Value.Ontology |> Some 
            let u = None
            let expected = "Test (OBO:123)"

            let actual = Component.composeName v u

            Expect.equal actual expected "Name was not correctly composed"

        )
        testCase "ComposeNameUnit" (fun () -> 
            
            let v = Value.Int 10 |> Some
            let u = OntologyAnnotation.fromString "degree Celsius" "UO" "UO:123" |> Some
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

            let expectedV = OntologyAnnotation.fromString "Test" "OBO" "OBO:123" |> Value.Ontology

            let expectedU = None

            Expect.equal actualV expectedV "Name was not correctly decomposed"
            Expect.equal actualU expectedU "Unit was not correctly decomposed"
        )
        testCase "DecomposeNameOntologyWithSpaces" (fun () -> 
            
            let n = "Test This Stuff (OBO:123)"

            let actualV, actualU = Component.decomposeName n

            let expectedV = OntologyAnnotation.fromString "Test This Stuff" "OBO" "OBO:123" |> Value.Ontology

            let expectedU = None

            Expect.equal actualV expectedV "Name was not correctly decomposed"
            Expect.equal actualU expectedU "Unit was not correctly decomposed"


        )
        testCase "DecomposeNameUnit" (fun () -> 
            
            let n = "10 degree Celsius (UO:123)"

            let actualV, actualU = Component.decomposeName n

            let expectedV = Value.Int 10

            let expectedU = OntologyAnnotation.fromString "degree Celsius" "UO" "UO:123" |> Some

            Expect.equal actualV expectedV "Name was not correctly decomposed"
            Expect.equal actualU expectedU "Unit was not correctly decomposed"

        )
        testCase "FromStringText" (fun () -> 
            
            let v = Value.Name "Text" |> Some

            let expected = Component.make (Some "Text") v None None

            let actual = Component.fromString "Text" "" "" ""

            Expect.equal actual expected "Component was not correctly composed"
        )
        testCase "FromStringTextWithCategory" (fun () -> 
            
            let v = Value.Name "Text" |> Some

            let header = OntologyAnnotation.fromString "TestCategory" "CO" "CO:567" |> Some 

            let expected = Component.make (Some "Text") v None header

            let actual = Component.fromString "Text" "TestCategory" "CO" "CO:567"

            Expect.equal actual expected "Component was not correctly composed"

        )
        testCase "FromStringOntology" (fun () -> 
            
            let v = OntologyAnnotation.fromString "Test" "OBO" "OBO:123" |> Value.Ontology |> Some 

            let expected = Component.make (Some "Test (OBO:123)") v None None

            let actual = Component.fromString "Test (OBO:123)" "" "" ""

            Expect.equal actual expected "Component was not correctly composed"

        )
        testCase "FromStringUnit" (fun () -> 
          
            let v = Value.Int 10 |> Some
            let u = OntologyAnnotation.fromString "degree Celsius" "UO" "UO:123" |> Some
            let expected = Component.make (Some "10 degree Celsius (UO:123)") v u None

            let actual = Component.fromString "10 degree Celsius (UO:123)" "" "" ""

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

            let header = OntologyAnnotation.fromString "TestCategory" "CO" "CO:567" |> Some 

            let expected = Component.make (Some "Text") v None header

            let actual = Component.fromOptions v None header

            Expect.equal actual expected "Component was not correctly composed"

        )
        testCase "FromOptionsOntology" (fun () -> 
            
            let v = OntologyAnnotation.fromString "Test" "OBO" "OBO:123" |> Value.Ontology |> Some 

            let expected = Component.make (Some "Test (OBO:123)") v None None

            let actual = Component.fromOptions v None None

            Expect.equal actual expected "Component was not correctly composed"

        )
        testCase "FromOptionsUnit" (fun () -> 
          
            let v = Value.Int 10 |> Some
            let u = OntologyAnnotation.fromString "degree Celsius" "UO" "UO:123" |> Some
            let expected = Component.make (Some "10 degree Celsius (UO:123)") v u None

            let actual = Component.fromOptions v u None

            Expect.equal actual expected "Component was not correctly composed"

        )
    ]