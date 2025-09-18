module Tests.Encode.WorkflowNegative

open Fable.Pyxpecto
open ARCtrl.CWL
open TestObjects.CWL

// Negative test: if outputSource lines are manually removed from encoded YAML, decoding should yield outputs without outputSource and our assertion should detect it.
let main =
    testList "Workflow negative tests" [
        testCase "missing outputSource detected" <| fun _ ->
            let original = Workflow.workflowFile
            let decoded = Decode.decodeWorkflow original
            let encoded = Encode.encodeWorkflowDescription decoded
            // Simulate user editing and deleting outputSource lines
            let modified =
                encoded.Split('\n')
                |> Array.filter (fun l -> not (l.TrimStart().StartsWith("outputSource:")))
                |> String.concat "\n"
            let decodedModified = Decode.decodeWorkflow modified
            let missing = decodedModified.Outputs |> Seq.filter (fun o -> o.OutputSource.IsNone) |> Seq.toList
            Expect.isTrue (missing.Length > 0) "At least one output should be missing outputSource after manual removal"
    ]
