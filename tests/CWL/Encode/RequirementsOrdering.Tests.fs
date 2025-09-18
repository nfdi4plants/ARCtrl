module Tests.Encode.RequirementsOrdering

open Fable.Pyxpecto
open ARCtrl.CWL
open TestObjects.CWL
open EncodeTestUtils

let private extractRequirementsOrder (text:string) =
    text.Split('\n')
    |> Array.filter (fun l -> l.TrimStart().StartsWith("- class:"))
    |> Array.map (fun l -> l.Trim())

let main =
    testList "Requirements ordering" [
        testCase "requirements order stable" <| fun _ ->
            let original = CommandLineTool.cwlFile
            let (encoded1, _, _) = assertDeterministic Encode.encodeToolDescription Decode.decodeCommandLineTool "CommandLineTool" original
            let order1 = extractRequirementsOrder encoded1
            // Do another cycle explicitly
            let decoded2 = Decode.decodeCommandLineTool encoded1
            let encoded2 = Encode.encodeToolDescription decoded2
            let order2 = extractRequirementsOrder encoded2
            Expect.equal order2 order1 "Requirement entries order must remain stable across encode cycles"
    ]
