module Tests.Outputs

open ARCtrl.CWL
open DynamicObj
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
                let expected = Some (OutputBinding.create(glob = "./arc/runs/fsResult1/result.csv"))
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
                let expected = Some (OutputBinding.create(glob = "./arc/runs/fsResult1/example.csv"))
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
                let expected = Array (InputArraySchema(File (FileInstance())))
                let actual = fileArrayItem.Type_.Value
                Expect.equal actual expected ""
            testCase "OutputBinding" <| fun _ ->
                let expected = Some (OutputBinding.create(glob = "./arc/runs/fsResult1/example.csv"))
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
                let expected = Array (InputArraySchema(File (FileInstance())))
                let actual = fileArrayItem.Type_.Value
                Expect.equal actual expected ""
            testCase "OutputBinding" <| fun _ ->
                let expected = Some (OutputBinding.create(glob = "./arc/runs/fsResult1/example.csv"))
                let actual = fileArrayItem.OutputBinding
                Expect.equal actual expected ""
        ]
    ]

let testOutputMutationApi =
    testList "Mutation API" [
        testCase "typed setters roundtrip values" <| fun _ ->
            let output = CWLOutput("result")
            output.Type_ <- Some (File (FileInstance()))
            output.OutputBinding <- Some (OutputBinding.create(glob = "results/*.txt"))
            output.OutputSource <- Some (OutputSource.Single "step/out")

            Expect.equal output.Type_ (Some (File (FileInstance()))) "Type_ setter should write DynamicObj-backed value."
            Expect.equal output.OutputBinding (Some (OutputBinding.create(glob = "results/*.txt"))) "OutputBinding setter should write value."
            Expect.equal output.OutputSource (Some (OutputSource.Single "step/out")) "OutputSource setter should write value."

        testCase "typed setters can clear optional values" <| fun _ ->
            let output =
                CWLOutput(
                    "result",
                    type_ = CWLType.String,
                    outputBinding = OutputBinding.create(glob = "*.txt"),
                    outputSource = OutputSource.Single "step/out"
                )
            output.Type_ <- None
            output.OutputBinding <- None
            output.OutputSource <- None

            Expect.isNone output.Type_ "Type_ should be removable."
            Expect.isNone output.OutputBinding "OutputBinding should be removable."
            Expect.isNone output.OutputSource "OutputSource should be removable."

        testCase "constructor normalizes empty outputSource collections" <| fun _ ->
            let output = CWLOutput("result", outputSource = OutputSource.Multiple (ResizeArray()))
            Expect.isNone output.OutputSource "Empty outputSource collections should normalize to None."

        testCase "known fields are typed fields, not dynamic overflow" <| fun _ ->
            let binding = OutputBinding.create(glob = "*.txt", loadContents = true, loadListing = LoadListingEnum.DeepListing, outputEval = "$(self[0])")
            let output =
                CWLOutput(
                    "result",
                    type_ = CWLType.String,
                    outputBinding = binding,
                    outputSource = OutputSource.Single "step/out",
                    label = "Result",
                    doc = "Result docs",
                    format = "edam:format_2330",
                    streamable = true
                )

            Expect.sequenceEqual OutputBinding.KnownFieldNames (Set [| "loadContents"; "loadListing"; "glob"; "outputEval" |]) "OutputBinding known fields should be declared on the type."
            Expect.sequenceEqual CWLOutput.KnownFieldNames (Set [| "id"; "type"; "label"; "secondaryFiles"; "streamable"; "doc"; "format"; "outputBinding"; "outputSource" |]) "CWLOutput known fields should be declared on the type."
            Expect.isEmpty (binding |> DynamicObjHelpers.dynamicPropertiesSnapshot) "OutputBinding known fields should not be stored as dynamic properties."
            Expect.isEmpty (output |> DynamicObjHelpers.dynamicPropertiesSnapshot) "CWLOutput known fields should not be stored as dynamic properties."

            DynObj.setProperty "arc:note" "keep overflow" output
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:note" output) (Some "keep overflow") "Unknown fields should still use DynamicObj overflow."

        testCase "spec output fields decode as typed members regardless of order" <| fun _ ->
            let output =
                TestObjects.CWL.Outputs.specOutputFieldsDecodeFileContent
                |> Decode.read
                |> Decode.outputsDecoder
                |> fun outputs -> outputs.[0]

            Expect.equal output.Label (Some "Result") "label should decode to a typed field."
            Expect.equal output.Doc (Some "Result docs") "doc should decode to a typed field."
            Expect.equal output.Format (Some "edam:format_2330") "format should decode to a typed field."
            Expect.equal output.Streamable (Some true) "streamable should decode to a typed field."
            Expect.isSome output.SecondaryFiles "secondaryFiles should decode to a typed field."
            Expect.equal output.OutputBinding.Value.LoadContents (Some true) "outputBinding.loadContents should decode to a typed field."
            Expect.equal output.OutputBinding.Value.LoadListing (Some LoadListingEnum.DeepListing) "outputBinding.loadListing should decode to a typed field."
            Expect.equal output.OutputBinding.Value.OutputEval (Some "$(self[0])") "outputBinding.outputEval should decode to a typed field."
            Expect.isEmpty (output |> DynamicObjHelpers.dynamicPropertiesSnapshot) "Known output fields should not be overflow."
    ]

let main = 
    testList "Output" [
        testOutput
        testOutputMutationApi
    ]
