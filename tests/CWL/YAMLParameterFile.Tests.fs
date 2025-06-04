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
            Expect.equal
                {
                    Key = "exampleKey"
                    Values = ResizeArray [| "1234" |]
                    Type = None
                }
                decodeYAMLParameterFile.[0]
                ""
        testCase "exampleKeyString" <| fun _ ->
            Expect.equal
                {
                    Key = "exampleKeyString"
                    Values = ResizeArray [| "abcdefg" |]
                    Type = None
                }
                decodeYAMLParameterFile.[1]
                ""
        testCase "exampleFile" <| fun _ ->
            Expect.equal
                {
                    Key = "exampleFile"
                    Values = ResizeArray [| "../examplePath" |]
                    Type = Some "File"
                }
                decodeYAMLParameterFile.[2]
                ""
        testCase "exampleDir" <| fun _ ->
            Expect.equal
                {
                    Key = "exampleDir"
                    Values = ResizeArray [| "../examplePathDir" |]
                    Type = Some "Directory"
                }
                decodeYAMLParameterFile.[3]
                ""
        testCase "exampleList" <| fun _ ->
            Expect.equal
                {
                    Key = "exampleList"
                    Values = ResizeArray [| "foo.txt"; "bar.dat"; "baz.txt" |]
                    Type = None
                }
                decodeYAMLParameterFile.[4]
                ""
    ]

let main = 
    testList "YAMLParameterFile" [
        testYAMLParameterFile
    ]