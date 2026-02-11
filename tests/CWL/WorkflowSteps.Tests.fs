module Tests.WorkflowSteps

open ARCtrl.CWL
open YAMLicious
open YAMLicious.YAMLiciousTypes
open TestingUtils

let decodeWorkflowStep =
    TestObjects.CWL.WorkflowSteps.workflowStepsFileContent
    |> Decode.read
    |> Decode.stepsDecoder

let tryYamlScalarString (y: YAMLElement) =
    match y with
    | YAMLElement.Object [YAMLElement.Value v]
    | YAMLElement.Value v -> Some v.Value
    | _ -> None

let testWorkflowStep =
    testList "Decode" [
        testCase "Length" <| fun _ -> Expect.equal 2 decodeWorkflowStep.Count ""
        testList "IDs" [
            testCase "MzMLToMzlite" <| fun _ ->
                let expected = "MzMLToMzlite"
                let actual = decodeWorkflowStep.[0].Id
                Expect.equal actual expected ""
            testCase "PeptideSpectrumMatching" <| fun _ ->
                let expected = "PeptideSpectrumMatching"
                let actual = decodeWorkflowStep.[1].Id
                Expect.equal actual expected ""
        ]
        testList "Run" [
            testCase "MzMLToMzlite" <| fun _ ->
                let expected = "./runs/MzMLToMzlite/proteomiqon-mzmltomzlite.cwl"
                let actual = decodeWorkflowStep.[0].Run
                match actual with
                | RunString runPath -> Expect.equal runPath expected ""
                | _ -> failwithf "Expected RunString but got %A" actual
            testCase "PeptideSpectrumMatching" <| fun _ ->
                let expected = "./runs/PeptideSpectrumMatching/proteomiqon-peptidespectrummatching.cwl"
                let actual = decodeWorkflowStep.[1].Run
                match actual with
                | RunString runPath -> Expect.equal runPath expected ""
                | _ -> failwithf "Expected RunString but got %A" actual
        ]
        testList "In" [
            testCase "MzMLToMzlite" <| fun _ ->
                let expected =
                    ResizeArray [|
                        {
                            Id = "stageDirectory"
                            Source = Some (ResizeArray [|"stage"|])
                            DefaultValue = None
                            ValueFrom = None
                            LinkMerge = None
                            PickValue = None
                            Doc = None
                            LoadContents = None
                            LoadListing = None
                            Label = None
                        }
                        {
                            Id = "inputDirectory"
                            Source = Some (ResizeArray [|"inputMzML"|])
                            DefaultValue = None
                            ValueFrom = None
                            LinkMerge = None
                            PickValue = None
                            Doc = None
                            LoadContents = None
                            LoadListing = None
                            Label = None
                        }
                    |]
                let actual = decodeWorkflowStep.[0].In
                Seq.iter2 (fun (expected: StepInput) (actual: StepInput) ->
                    Expect.equal actual.Id expected.Id ""
                    Expect.sequenceEqual actual.Source.Value expected.Source.Value ""
                    Expect.equal
                        (actual.DefaultValue |> Option.bind tryYamlScalarString)
                        (expected.DefaultValue |> Option.bind tryYamlScalarString)
                        ""
                    Expect.equal actual.ValueFrom expected.ValueFrom ""
                    Expect.equal actual.LinkMerge expected.LinkMerge ""
                    Expect.equal actual.PickValue expected.PickValue ""
                    Expect.equal actual.Doc expected.Doc ""
                    Expect.equal actual.LoadContents expected.LoadContents ""
                    Expect.equal actual.LoadListing expected.LoadListing ""
                    Expect.equal actual.Label expected.Label ""
                ) expected actual
            testCase "PeptideSpectrumMatching" <| fun _ ->
                let expected =
                    ResizeArray [|
                        {
                            Id = "stageDirectory"
                            Source = Some (ResizeArray [|"stage"|])
                            DefaultValue = None
                            ValueFrom = None
                            LinkMerge = None
                            PickValue = None
                            Doc = None
                            LoadContents = None
                            LoadListing = None
                            Label = None
                        }
                        {
                            Id = "inputDirectory"
                            Source = Some (ResizeArray [|"MzMLToMzlite/dir1"; "MzMLToMzlite/dir2"|])
                            DefaultValue = None
                            ValueFrom = None
                            LinkMerge = Some MergeFlattened
                            PickValue = None
                            Doc = None
                            LoadContents = None
                            LoadListing = None
                            Label = None
                        }
                        {
                            Id = "parallelismLevel"
                            Source = None
                            DefaultValue = Some (YAMLElement.Object [YAMLElement.Value {Value = "8"; Comment = None}])
                            ValueFrom = None
                            LinkMerge = None
                            PickValue = None
                            Doc = None
                            LoadContents = None
                            LoadListing = None
                            Label = None
                        }
                        {
                            Id = "outputDirectory"
                            Source = None
                            DefaultValue = None
                            ValueFrom = Some "output"
                            LinkMerge = None
                            PickValue = None
                            Doc = None
                            LoadContents = None
                            LoadListing = None
                            Label = None
                        }
                    |]
                let actual = decodeWorkflowStep.[1].In
                Seq.iter2 (fun (expected: StepInput) (actual: StepInput) ->
                    Expect.equal actual.Id expected.Id ""
                    if expected.Source.IsSome then
                        Expect.sequenceEqual actual.Source.Value expected.Source.Value ""
                    Expect.equal
                        (actual.DefaultValue |> Option.bind tryYamlScalarString)
                        (expected.DefaultValue |> Option.bind tryYamlScalarString)
                        ""
                    Expect.equal actual.ValueFrom expected.ValueFrom ""
                    Expect.equal actual.LinkMerge expected.LinkMerge ""
                    Expect.equal actual.PickValue expected.PickValue ""
                    Expect.equal actual.Doc expected.Doc ""
                    Expect.equal actual.LoadContents expected.LoadContents ""
                    Expect.equal actual.LoadListing expected.LoadListing ""
                    Expect.equal actual.Label expected.Label ""
                ) expected actual
        ]
        testList "Out" [
            testCase "MzMLToMzlite" <| fun _ ->
                let expected = ResizeArray [| StepOutputString "dir" |]
                let actual = decodeWorkflowStep.[0].Out
                Expect.sequenceEqual actual expected ""
            testCase "PeptideSpectrumMatching" <| fun _ ->
                let expected = ResizeArray [| StepOutputString "dir1"; StepOutputString "dir2" |]
                let actual = decodeWorkflowStep.[1].Out
                Expect.sequenceEqual actual expected ""
        ]
        testCase "invalid linkMerge fails decode" <| fun _ ->
            let invalidSteps = """steps:
  step1:
    run: ./tool.cwl
    in:
      input1:
        source: in1
        linkMerge: invalid_merge
    out: [out]"""
            let decodeInvalid () =
                invalidSteps
                |> Decode.read
                |> Decode.stepsDecoder
                |> ignore
            Expect.throws decodeInvalid "Invalid linkMerge should fail decoding"
        testCase "step output record requires id" <| fun _ ->
            let invalidSteps = """steps:
  step1:
    run: ./tool.cwl
    in: {}
    out:
      - type: string"""
            let decodeInvalid () =
                invalidSteps
                |> Decode.read
                |> Decode.stepsDecoder
                |> ignore
            Expect.throws decodeInvalid "Step output record without id should fail decoding"
    ]

let main = 
    testList "WorkflowStep" [
        testWorkflowStep
    ]
