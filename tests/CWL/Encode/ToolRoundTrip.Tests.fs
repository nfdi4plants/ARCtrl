module Tests.Encode.ToolRoundTrip

open Fable.Pyxpecto
open ARCtrl.CWL
open TestObjects.CWL
open EncodeTestUtils

let main =
    testList "Tool Encode RoundTrip" [
        testCase "tool encode/decode deterministic & extended requirements" <| fun _ ->
            let original = CommandLineTool.cwlFile
            let (encoded1, _, _) = assertDeterministic Encode.encodeToolDescription Decode.decodeCommandLineTool "CommandLineTool" original
            assertRequirementsExtended encoded1
    ]
