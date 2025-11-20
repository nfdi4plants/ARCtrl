module Tests.YAMLParameterFile

open ARCtrl.CWL
open YAMLicious
open TestingUtils

let decodeYAMLParameterFile =
    TestObjects.CWL.YAMLParameterFile.yamlParameterFileContent
    |> Decode.read
    |> DecodeParameters.cwlparameterReferenceArrayDecoder

let testYAMLParameterFile =
    testList "Decode" [
        testCase "Length" <| fun _ -> Expect.equal 5  decodeYAMLParameterFile.Count ""
        testCase "exampleKeyInt" <| fun _ ->
            let expected =
                CWLParameterReference(
                    key = "exampleKey",
                    values = ResizeArray [| "1234" |]
                )
            Expect.equal expected.Key decodeYAMLParameterFile.[0].Key ""
            Expect.sequenceEqual expected.Values decodeYAMLParameterFile.[0].Values ""
            Expect.equal expected.Type decodeYAMLParameterFile.[0].Type ""
        testCase "exampleKeyString" <| fun _ ->
            let expected =             
                CWLParameterReference(
                    key = "exampleKeyString",
                    values = ResizeArray [| "abcdefg" |]
                )
            Expect.equal expected.Key decodeYAMLParameterFile.[1].Key ""
            Expect.sequenceEqual expected.Values decodeYAMLParameterFile.[1].Values ""
            Expect.equal expected.Type decodeYAMLParameterFile.[1].Type ""
        testCase "exampleFile" <| fun _ ->
            let expected =
                CWLParameterReference(
                    key = "exampleFile",
                    values = ResizeArray [| "../examplePath" |],
                    type_ = (CWLType.file())
                )
            Expect.equal expected.Key decodeYAMLParameterFile.[2].Key ""
            Expect.sequenceEqual expected.Values decodeYAMLParameterFile.[2].Values ""
            Expect.equal expected.Type decodeYAMLParameterFile.[2].Type ""
        testCase "exampleDir" <| fun _ ->
            let expected =
                CWLParameterReference(
                    key = "exampleDir",
                    values = ResizeArray [| "../examplePathDir" |],
                    type_ = (CWLType.directory())
                )
            Expect.equal expected.Key decodeYAMLParameterFile.[3].Key ""
            Expect.sequenceEqual expected.Values decodeYAMLParameterFile.[3].Values ""
            Expect.equal expected.Type decodeYAMLParameterFile.[3].Type ""
        testCase "exampleList" <| fun _ ->
            let expected =
                CWLParameterReference(
                    key = "exampleList",
                    values = ResizeArray [| "foo.txt"; "bar.dat"; "baz.txt" |]
                )
            Expect.equal expected.Key decodeYAMLParameterFile.[4].Key ""
            Expect.sequenceEqual expected.Values decodeYAMLParameterFile.[4].Values ""
            Expect.equal expected.Type decodeYAMLParameterFile.[4].Type ""
    ]

let main = 
    testList "YAMLParameterFile" [
        testYAMLParameterFile
    ]