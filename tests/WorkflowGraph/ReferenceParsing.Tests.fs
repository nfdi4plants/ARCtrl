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

let tests_relativeId =
    testList "relativeId" [
        testCase "an id behind the id of its holder gives the rest" <| fun () ->
            Expect.equal (ReferenceParsing.relativeId (Some "#main") "#main/msg") "msg" "The workflow input msg."
            Expect.equal (ReferenceParsing.relativeId (Some "#main") "#main/first/out") "first/out" "The output of a step is kept as step and port."
            Expect.equal (ReferenceParsing.relativeId (Some "#main/first") "#main/first/msg") "msg" "The input of a step against the id of the step."
        testCase "the holder id can be written without the hash" <| fun () ->
            Expect.equal (ReferenceParsing.relativeId (Some "main") "#main/msg") "msg" ""
        testCase "an id without the hash is relative already" <| fun () ->
            Expect.equal (ReferenceParsing.relativeId (Some "main") "main/out") "main/out" "A step named like the workflow keeps its name."
            Expect.equal (ReferenceParsing.relativeId (Some "#main") "msg") "msg" ""
        testCase "an id behind another process stays" <| fun () ->
            Expect.equal (ReferenceParsing.relativeId (Some "#main") "#shout.cwl/msg") "#shout.cwl/msg" ""
            Expect.equal (ReferenceParsing.relativeId (Some "#main") "#main") "#main" "The holder itself has nothing behind it."
        testCase "without a holder the id stays" <| fun () ->
            Expect.equal (ReferenceParsing.relativeId None "#main/msg") "#main/msg" ""
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
        tests_relativeId
        tests_extractStepOutputId
    ]
