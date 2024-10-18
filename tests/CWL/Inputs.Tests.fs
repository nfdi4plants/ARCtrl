module Tests.Inputs

open ARCtrl.CWL
open ARCtrl.CWL.CWLTypes
open ARCtrl.CWL.Inputs
open YAMLicious
open TestingUtils

let decodeInput =
    TestObjects.CWL.Inputs.inputsFileContent
    |> Decode.read
    |> Decode.inputsDecoder
    |>fun i ->i.Value

let testInput =
    testList "Decode" [
        testCase "Length" <| fun _ -> Expect.equal 3 decodeInput.Count ""
        testList "Directory" [
            let directoryItem = decodeInput.[0]
            testCase "Name" <| fun _ -> Expect.equal "arcDirectory"  directoryItem.Name ""
            testCase "Type" <| fun _ -> Expect.equal (Directory (DirectoryInstance()))  directoryItem.Type_.Value ""
        ]
        testList "File" [
            let fileItem = decodeInput.[1]
            testCase "Name" <| fun _ -> Expect.equal "firstArg"  fileItem.Name ""
            testCase "Type" <| fun _ -> Expect.equal (File (FileInstance()))  fileItem.Type_.Value ""
            testCase "InputBinding" <| fun _ -> Expect.equal (Some {Position = Some 1; Prefix = Some "--example"; ItemSeparator = None; Separate = None}) fileItem.InputBinding ""
        ]
        testList "String" [
            let stringItem = decodeInput.[2]
            testCase "Name" <| fun _ -> Expect.equal "secondArg"  stringItem.Name ""
            testCase "Type" <| fun _ ->
                let expected = String
                let actual = stringItem.Type_.Value
                Expect.equal actual expected ""
            testCase "InputBinding" <| fun _ ->
                let expected = Some {Position = Some 2; Prefix = None; ItemSeparator = None; Separate = Some false}
                let actual = stringItem.InputBinding
                Expect.equal actual expected ""
        ]
    ]

let main = 
    testList "Input" [
        testInput
    ]