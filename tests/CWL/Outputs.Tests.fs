module Tests.Outputs

open ARCtrl.CWL
open YAMLicious
open TestingUtils

let decodeOutput =
    TestObjects.CWL.Outputs.outputsFileContent
    |> Decode.read
    |> Decode.outputsDecoder

let testOutput =
    testList "Decode" [
        testCase "Length" <| fun _ ->
            let expected = 5
            let actual = decodeOutput.Count
            Expect.equal actual expected ""
        testList "File" [
            let fileItem = decodeOutput.[0]
            testCase "Name" <| fun _ ->
                let expected = "output"
                let actual = fileItem.Name
                Expect.equal actual expected ""
            testCase "Type" <| fun _ ->
                let expected = File (FileInstance())
                let actual = fileItem.Type_.Value
                Expect.equal actual expected ""
            testCase "OutputBinding" <| fun _ ->
                let expected = Some {Glob = Some "./arc/runs/fsResult1/result.csv"}
                let actual = fileItem.OutputBinding
                Expect.equal actual expected ""
        ]
        testList "Directory" [
            let directoryItem = decodeOutput.[1]
            testCase "Name" <| fun _ ->
                let expected = "example1"
                let actual = directoryItem.Name
                Expect.equal actual expected ""
            testCase "Type" <| fun _ ->
                let expected = Directory (DirectoryInstance())
                let actual = directoryItem.Type_.Value
                Expect.equal actual expected ""
            testCase "OutputBinding" <| fun _ ->
                let expected = Some {Glob = Some "./arc/runs/fsResult1/example.csv"}
                let actual = directoryItem.OutputBinding
                Expect.equal actual expected ""
        ]
        testList "Directory 2" [
            let directoryItem = decodeOutput.[2]
            testCase "Name" <| fun _ ->
                let expected = "example2"
                let actual = directoryItem.Name
                Expect.equal actual expected ""
            testCase "Type" <| fun _ ->
                let expected = Directory (DirectoryInstance())
                let actual = directoryItem.Type_.Value
                Expect.equal actual expected ""
            testCase "OutputBinding" <| fun _ ->
                let expected = None
                let actual = directoryItem.OutputBinding
                Expect.equal actual expected ""
        ]
        testList "File Array" [
            let fileArrayItem = decodeOutput.[3]
            testCase "Name" <| fun _ ->
                let expected = "exampleArray1"
                let actual = fileArrayItem.Name
                Expect.equal actual expected ""
            testCase "Type" <| fun _ ->
                let expected = Array { Items = File (FileInstance()); Label = None; Doc = None; Name = None }
                let actual = fileArrayItem.Type_.Value
                Expect.equal actual expected ""
            testCase "OutputBinding" <| fun _ ->
                let expected = Some {Glob = Some "./arc/runs/fsResult1/example.csv"}
                let actual = fileArrayItem.OutputBinding
                Expect.equal actual expected ""
        ]
        testList "File Array 2" [
            let fileArrayItem = decodeOutput.[4]
            testCase "Name" <| fun _ ->
                let expected = "exampleArray2"
                let actual = fileArrayItem.Name
                Expect.equal actual expected ""
            testCase "Type" <| fun _ ->
                let expected = Array { Items = File (FileInstance()); Label = None; Doc = None; Name = None }
                let actual = fileArrayItem.Type_.Value
                Expect.equal actual expected ""
            testCase "OutputBinding" <| fun _ ->
                let expected = Some {Glob = Some "./arc/runs/fsResult1/example.csv"}
                let actual = fileArrayItem.OutputBinding
                Expect.equal actual expected ""
        ]
    ]

let testOutputMutationApi =
    testList "Mutation API" [
        testCase "typed setters roundtrip values" <| fun _ ->
            let output = CWLOutput("result")
            output.Type_ <- Some (File (FileInstance()))
            output.OutputBinding <- Some { Glob = Some "results/*.txt" }
            output.OutputSource <- Some (OutputSource.Single "step/out")

            Expect.equal output.Type_ (Some (File (FileInstance()))) "Type_ setter should write DynamicObj-backed value."
            Expect.equal output.OutputBinding (Some { Glob = Some "results/*.txt" }) "OutputBinding setter should write value."
            Expect.equal output.OutputSource (Some (OutputSource.Single "step/out")) "OutputSource setter should write value."

        testCase "typed setters can clear optional values" <| fun _ ->
            let output =
                CWLOutput(
                    "result",
                    type_ = CWLType.String,
                    outputBinding = { Glob = Some "*.txt" },
                    outputSource = OutputSource.Single "step/out"
                )
            output.Type_ <- None
            output.OutputBinding <- None
            output.OutputSource <- None

            Expect.isNone output.Type_ "Type_ should be removable."
            Expect.isNone output.OutputBinding "OutputBinding should be removable."
            Expect.isNone output.OutputSource "OutputSource should be removable."
    ]

let main = 
    testList "Output" [
        testOutput
        testOutputMutationApi
    ]
