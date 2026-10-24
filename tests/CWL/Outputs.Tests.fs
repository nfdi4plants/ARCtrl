module Tests.Outputs

open ARCtrl.CWL
open DynamicObj
open YAMLicious
open TestingUtils

let decodeOutput =
    TestObjects.CWL.Outputs.outputsFileContent
    |> Decode.readYaml
    |> Decode.outputsDecoder
    |> ListEntry.values

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
                let expected = File
                let actual = fileItem.Type_.Value
                Expect.equal actual expected ""
            testCase "OutputBinding" <| fun _ ->
                let expected = Some (OutputBinding.create(glob = OutputGlob.Single (SchemaSaladString.Literal "./arc/runs/fsResult1/result.csv")))
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
                let expected = Directory
                let actual = directoryItem.Type_.Value
                Expect.equal actual expected ""
            testCase "OutputBinding" <| fun _ ->
                let expected = Some (OutputBinding.create(glob = OutputGlob.Single (SchemaSaladString.Literal "./arc/runs/fsResult1/example.csv")))
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
                let expected = Directory
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
                let expected = Array (InputArraySchema(File))
                let actual = fileArrayItem.Type_.Value
                Expect.equal actual expected ""
            testCase "OutputBinding" <| fun _ ->
                let expected = Some (OutputBinding.create(glob = OutputGlob.Single (SchemaSaladString.Literal "./arc/runs/fsResult1/example.csv")))
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
                let expected = Array (InputArraySchema(File))
                let actual = fileArrayItem.Type_.Value
                Expect.equal actual expected ""
            testCase "OutputBinding" <| fun _ ->
                let expected = Some (OutputBinding.create(glob = OutputGlob.Single (SchemaSaladString.Literal "./arc/runs/fsResult1/example.csv")))
                let actual = fileArrayItem.OutputBinding
                Expect.equal actual expected ""
        ]
    ]

let testOutputMutationApi =
    testList "Mutation API" [
        testCase "typed setters roundtrip values" <| fun _ ->
            let output = CWLOutput("result")
            output.Type_ <- Some (File)
            output.OutputBinding <- Some (OutputBinding.create(glob = OutputGlob.Single (SchemaSaladString.Literal "results/*.txt")))
            output.OutputSource <- Some (OutputSource.Single "step/out")

            Expect.equal output.Type_ (Some (File)) "Type_ setter should write DynamicObj-backed value."
            Expect.equal output.OutputBinding (Some (OutputBinding.create(glob = OutputGlob.Single (SchemaSaladString.Literal "results/*.txt")))) "OutputBinding setter should write value."
            Expect.equal output.OutputSource (Some (OutputSource.Single "step/out")) "OutputSource setter should write value."

        testCase "typed setters can clear optional values" <| fun _ ->
            let output =
                CWLOutput(
                    "result",
                    type_ = CWLType.String,
                    outputBinding = OutputBinding.create(glob = OutputGlob.Single (SchemaSaladString.Literal "*.txt")),
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
            let binding = OutputBinding.create(glob = OutputGlob.Single (SchemaSaladString.Literal "*.txt"), loadContents = true, loadListing = LoadListingEnum.DeepListing, outputEval = (SchemaSaladString.Literal "$(self[0])"))
            let output =
                CWLOutput(
                    "result",
                    type_ = CWLType.String,
                    outputBinding = binding,
                    outputSource = OutputSource.Single "step/out",
                    label = (SchemaSaladString.Literal "Result"),
                    doc = (DocText.literal "Result docs"),
                    format = (SchemaSaladString.Literal "edam:format_2330"),
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
                |> Decode.readYaml
                |> Decode.outputsDecoder
                |> ListEntry.values
                |> fun outputs -> outputs.[0]

            Expect.equal output.Label (Some (SchemaSaladString.Literal "Result")) "label should decode to a typed field."
            Expect.equal output.Doc (Some (DocText.literal "Result docs")) "doc should decode to a typed field."
            Expect.equal output.Format (Some (SchemaSaladString.Literal "edam:format_2330")) "format should decode to a typed field."
            Expect.equal output.Streamable (Some true) "streamable should decode to a typed field."
            Expect.isSome output.SecondaryFiles "secondaryFiles should decode to a typed field."
            Expect.equal output.OutputBinding.Value.LoadContents (Some true) "outputBinding.loadContents should decode to a typed field."
            Expect.equal output.OutputBinding.Value.LoadListing (Some LoadListingEnum.DeepListing) "outputBinding.loadListing should decode to a typed field."
            Expect.equal output.OutputBinding.Value.OutputEval (Some (SchemaSaladString.Literal "$(self[0])")) "outputBinding.outputEval should decode to a typed field."
            Expect.isEmpty (output |> DynamicObjHelpers.dynamicPropertiesSnapshot) "Known output fields should not be overflow."
    ]

let testOutputGlob =
    let tool (glob: string) =
        String.concat "\n" [ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo"; "inputs: []"; "outputs:"; "  o:"; "    type: File[]"; "    outputBinding:"; "      glob: " + glob; "" ]
    let globOf (decoded: CWLToolDescription) = (ListEntry.values decoded.Outputs).[0].OutputBinding.Value.Glob

    testList "Output glob" [
        testCase "a pattern, a list of patterns and an expression keep the form that they were written in" <| fun _ ->
            let cases =
                [ "\"*.txt\"", OutputGlob.Single (SchemaSaladString.Literal "*.txt"), "glob: \"*.txt\""
                  "[a.txt, b.txt]", OutputGlob.Multiple (ResizeArray [ (SchemaSaladString.Literal "a.txt"); (SchemaSaladString.Literal "b.txt") ]), "glob: [a.txt, b.txt]"
                  "[a.txt]", OutputGlob.Multiple (ResizeArray [ (SchemaSaladString.Literal "a.txt") ]), "glob: [a.txt]"
                  "[]", OutputGlob.Multiple (ResizeArray []), "glob: []"
                  "[a.txt, $(inputs.x)]", OutputGlob.Multiple (ResizeArray [ (SchemaSaladString.Literal "a.txt"); (SchemaSaladString.Literal "$(inputs.x)") ]), "glob: [a.txt, $(inputs.x)]"
                  "$(inputs.x)", OutputGlob.Single (SchemaSaladString.Literal "$(inputs.x)"), "glob: $(inputs.x)" ]
            for text, expected, written in cases do
                let decoded = Decode.decodeCommandLineTool (tool text)
                Expect.equal (globOf decoded) (Some expected) (sprintf "glob: %s should decode as %A" text expected)
                let encoded = Encode.encodeToolDescription decoded
                Expect.stringContains encoded written (sprintf "glob: %s should be written in its form" text)
                Expect.equal (Decode.decodeCommandLineTool encoded) decoded (sprintf "glob: %s should survive a write and a read" text)
        testCase "a glob that is neither a string nor a list of strings is a decode error" <| fun _ ->
            for text, path in [ "{a: b}", "outputs.o.outputBinding.glob"; "[a.txt, {a: b}]", "outputs.o.outputBinding.glob[1]"; "[a.txt, [b.txt]]", "outputs.o.outputBinding.glob[1]" ] do
                match (try Decode.decodeCommandLineTool (tool text) |> ignore; None with YamlDecodeException error -> Some error) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "glob: %s should fail at its place" text)
                    Expect.isSome error.Span (sprintf "glob: %s should have the place in the text" text)
                | None -> failwith (sprintf "glob: %s must fail" text)
        testCase "globs compare by their patterns" <| fun _ ->
            Expect.equal (OutputGlob.Multiple (ResizeArray [ (SchemaSaladString.Literal "a"); (SchemaSaladString.Literal "b") ])) (OutputGlob.Multiple (ResizeArray [ (SchemaSaladString.Literal "a"); (SchemaSaladString.Literal "b") ])) "Lists with equal patterns are equal."
            Expect.notEqual (OutputGlob.Multiple (ResizeArray [ (SchemaSaladString.Literal "a") ])) (OutputGlob.Single (SchemaSaladString.Literal "a")) "A list of one pattern differs from the pattern."
            Expect.equal (OutputBinding.create(glob = OutputGlob.Multiple (ResizeArray [ (SchemaSaladString.Literal "a") ]))) (OutputBinding.create(glob = OutputGlob.Multiple (ResizeArray [ (SchemaSaladString.Literal "a") ]))) "Bindings with equal globs are equal."
    ]

let main =
    testList "Output" [
        testOutput
        testOutputMutationApi
        testOutputGlob
    ]
