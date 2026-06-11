module Tests.YAMLParameterFile

open ARCtrl.CWL
open YAMLicious
open TestingUtils
open DynamicObj

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
        testCase "nested File arrays decode as structured values" <| fun _ ->
            let decoded = DecodeParameters.decodeYAMLParameterFile TestObjects.CWL.YAMLParameterFile.Structured.nestedFileArray
            let reference = Expect.wantExactlyOne decoded "Expected one parameter reference."
            Expect.equal reference.Key "sampleRecordFiles" "Key should decode."
            Expect.equal reference.Type (Some (CWLType.Array { Items = CWLType.Array { Items = CWLType.file(); Label = None; Doc = None; Name = None }; Label = None; Doc = None; Name = None })) "Nested file array type should be inferred."
            match reference.Value with
            | Some (CWLParameterValue.Array outer) ->
                Expect.equal outer.Count 2 "Outer array should contain one entry per sample."
                for item in outer do
                    match item with
                    | CWLParameterValue.Array inner ->
                        let file = Expect.wantExactlyOne inner "Each sample entry should contain one file in this fixture."
                        match file with
                        | CWLParameterValue.File file ->
                            Expect.isSome (DynObj.tryGetTypedPropertyValue<string> "path" file) "File path should be preserved."
                            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "format" file) (Some "edam:format_1930") "File format metadata should be preserved."
                        | other -> failwithf "Expected file value but got %A" other
                    | other -> failwithf "Expected nested array but got %A" other
            | other -> failwithf "Expected structured array value but got %A" other
            Expect.sequenceEqual reference.Values (ResizeArray [|
                "../../assays/RNASeq/dataset/DB_097.fastq.gz"
                "../../assays/RNASeq/dataset/DB_163.fastq.gz"
            |]) "Legacy Values should flatten leaf file paths."
        testCase "array of records decodes without dropping nested fields" <| fun _ ->
            let decoded = DecodeParameters.decodeYAMLParameterFile TestObjects.CWL.YAMLParameterFile.Structured.arrayOfRecords
            let reference = Expect.wantExactlyOne decoded "Expected one parameter reference."
            match reference.Value with
            | Some (CWLParameterValue.Array records) ->
                Expect.equal records.Count 2 "Record array length should be preserved."
                match records.[0] with
                | CWLParameterValue.Record fields ->
                    Expect.isTrue (fields |> Seq.exists (fun f -> f.Name = "name")) "Record field `name` should exist."
                    Expect.isTrue (fields |> Seq.exists (fun f -> f.Name = "reads")) "Record field `reads` should exist."
                | other -> failwithf "Expected record value but got %A" other
            | other -> failwithf "Expected structured array value but got %A" other
        testCase "empty arrays decode as present empty arrays" <| fun _ ->
            let decoded = DecodeParameters.decodeYAMLParameterFile TestObjects.CWL.YAMLParameterFile.Structured.emptyArray
            let reference = Expect.wantExactlyOne decoded "Expected one parameter reference."
            match reference.Value with
            | Some (CWLParameterValue.Array values) -> Expect.isEmpty values "Empty array should be preserved."
            | other -> failwithf "Expected present empty array but got %A" other
        testCase "nested File arrays roundtrip through YAML" <| fun _ ->
            let decoded = DecodeParameters.decodeYAMLParameterFile TestObjects.CWL.YAMLParameterFile.Structured.nestedFileArray
            let encoded = Encode.encodeYAMLParameterFile decoded
            let decodedAgain = DecodeParameters.decodeYAMLParameterFile encoded
            Expect.sequenceEqual decodedAgain decoded "Structured parameter values should roundtrip through YAML."
    ]

let main = 
    testList "YAMLParameterFile" [
        testYAMLParameterFile
    ]
