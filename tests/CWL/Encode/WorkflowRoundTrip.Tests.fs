module Tests.Encode.WorkflowRoundTrip

open Fable.Pyxpecto
open ARCtrl.CWL
open TestObjects.CWL
open EncodeTestUtils

let main =
    testList "Workflow Encode RoundTrip" [
        testCase "workflow encode/decode deterministic & outputSource preserved" <| fun _ ->
            let original = Workflow.workflowFile
            let (encoded1, d1, d2) = assertDeterministic Encode.encodeWorkflowDescription Decode.decodeWorkflow "Workflow" original
            // Verify outputs have outputSource in both decoded versions
            assertAllOutputsHaveSource d1
            assertAllOutputsHaveSource d2
    ]
