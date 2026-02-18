module Tests.ReferenceParsing

open ARCtrl.CWL
open ARCtrl.WorkflowGraph
open TestingUtils

let tests_parseSourceReference =
    testList "parseSourceReference" [
        testCase "step and port" <| fun () ->
            let parsed = ReferenceParsing.parseSourceReference "MzMLToMzlite/dir"
            Expect.equal parsed.StepId (Some "MzMLToMzlite") ""
            Expect.equal parsed.PortId "dir" ""

        testCase "bare input name" <| fun () ->
            let parsed = ReferenceParsing.parseSourceReference "inputMzML"
            Expect.isNone parsed.StepId ""
            Expect.equal parsed.PortId "inputMzML" ""

        testCase "hash-prefixed reference" <| fun () ->
            let parsed = ReferenceParsing.parseSourceReference "#step/out"
            Expect.equal parsed.StepId (Some "step") ""
            Expect.equal parsed.PortId "out" ""

        testCase "empty string" <| fun () ->
            let parsed = ReferenceParsing.parseSourceReference ""
            Expect.isNone parsed.StepId ""
            Expect.equal parsed.PortId "" ""

        testCase "first slash split only" <| fun () ->
            let parsed = ReferenceParsing.parseSourceReference "a/b/c"
            Expect.equal parsed.StepId (Some "a") ""
            Expect.equal parsed.PortId "b/c" ""
    ]

let tests_extractStepOutputId =
    testList "extractStepOutputId" [
        testCase "StepOutputString" <| fun () ->
            let actual =
                StepOutputString "dir"
                |> ReferenceParsing.extractStepOutputId
            Expect.equal actual "dir" ""

        testCase "StepOutputRecord" <| fun () ->
            let actual =
                StepOutputRecord (StepOutputParameter.create "dir")
                |> ReferenceParsing.extractStepOutputId
            Expect.equal actual "dir" ""
    ]

let main =
    testList "ReferenceParsing" [
        tests_parseSourceReference
        tests_extractStepOutputId
    ]
