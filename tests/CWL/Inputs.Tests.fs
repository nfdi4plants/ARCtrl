module Tests.Inputs

open ARCtrl.CWL
open ARCtrl.CWL.CWLTypes
open ARCtrl.CWL.Inputs
open YAMLicious
open TestingUtils

let decodeInput =
    TestObjects.CWL.Inputs.inputs
    |> Decode.read
    |> Decode.inputsDecoder
    |>fun i ->i.Value

let testInput =
    testList "inputs with Directory, File and string" [
        testCase "Length" <| fun _ -> Expect.isTrue (3 = decodeInput.Length) "Length of inputs is not 3"
        testList "Directory" [
            let directoryItem = decodeInput.[0]
            testCase "Name" <| fun _ -> Expect.isTrue ("arcDirectory" = directoryItem.Name) "Name of input is not 'arcDirectory'"
            testCase "Type" <| fun _ -> Expect.isTrue ((Directory (DirectoryInstance())) = directoryItem.Type.Value) "Type of input is not Directory"
        ]
        testList "File" [
            let fileItem = decodeInput.[1]
            testCase "Name" <| fun _ -> Expect.isTrue ("firstArg" = fileItem.Name) "Name of input is not 'firstArg'"
            testCase "Type" <| fun _ -> Expect.isTrue ((File (FileInstance())) = fileItem.Type.Value) "Type of input is not File"
            testCase "InputBinding" <| fun _ -> Expect.isTrue (Some {Position = Some 1; Prefix = Some "--example"; ItemSeparator = None; Separate = None} = fileItem.InputBinding) "InputBinding of input is not Some Pattern"
        ]
        testList "String" [
            let stringItem = decodeInput.[2]
            testCase "Name" <| fun _ -> Expect.isTrue ("secondArg" = stringItem.Name) "Name of input is not 'secondArg'"
            testCase "Type" <| fun _ ->
                let expected = String
                let actual = stringItem.Type.Value
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
            testCase "InputBinding" <| fun _ ->
                let expected = Some {Position = Some 2; Prefix = None; ItemSeparator = None; Separate = Some false}
                let actual = stringItem.InputBinding
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
    ]