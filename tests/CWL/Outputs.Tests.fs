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
                let expected = Array (File (FileInstance()))
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
                let expected = Array (File (FileInstance()))
                let actual = fileArrayItem.Type_.Value
                Expect.equal actual expected ""
            testCase "OutputBinding" <| fun _ ->
                let expected = Some {Glob = Some "./arc/runs/fsResult1/example.csv"}
                let actual = fileArrayItem.OutputBinding
                Expect.equal actual expected ""
        ]
    ]

let main = 
    testList "Output" [
        testOutput
    ]