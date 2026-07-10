module Tests.YAMLParameterFile

open ARCtrl.CWL
open DynamicObj
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
        testCase "known fields are typed fields, not dynamic overflow" <| fun _ ->
            let reference =
                CWLParameterReference(
                    key = "input",
                    values = ResizeArray [| "value" |],
                    type_ = CWLType.String
                )
            Expect.sequenceEqual CWLParameterReference.KnownFieldNames (Set [| "class"; "path"; "location"; "type"; "value" |]) ""
            Expect.isEmpty ((reference :> DynamicObj) |> DynamicObjHelpers.dynamicPropertiesSnapshot) "Known fields should not be stored as dynamic properties."
            DynObj.setProperty "arc:note" "keep me" reference
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:note" reference) (Some "keep me") "Unknown fields should stay in dynamic overflow."
        testCase "file object parameter fields decode without field order assumptions and keep overflow" <| fun _ ->
            let yaml = """exampleFile:
  path: ../examplePath
  arc:file note: keep file note
  class: File"""
            let decoded = DecodeParameters.decodeYAMLParameterFile yaml
            let reference = decoded.[0]
            Expect.equal reference.Key "exampleFile" ""
            Expect.sequenceEqual reference.Values (ResizeArray [| "../examplePath" |]) "Path should decode even when it appears before class."
            Expect.equal reference.Type (Some (CWLType.file())) "Class should decode to File type regardless of field order."
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:file note" reference) (Some "keep file note") "Unknown parameter object fields should be preserved on the parameter reference."
            Expect.isNone (DynObj.tryGetTypedPropertyValue<string> "class" reference) "Known class field should not be overflow."
            Expect.isNone (DynObj.tryGetTypedPropertyValue<string> "path" reference) "Known path field should not be overflow."
    ]

let main = 
    testList "YAMLParameterFile" [
        testYAMLParameterFile
    ]
