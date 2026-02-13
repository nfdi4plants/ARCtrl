module Tests.Inputs

open ARCtrl.CWL
open YAMLicious
open TestingUtils

let decodeInput =
    TestObjects.CWL.Inputs.inputsFileContent
    |> Decode.read
    |> Decode.inputsDecoder
    |>fun i ->i.Value

let testInput =
    testList "Decode" [
        testCase "Length" <| fun _ -> Expect.equal 5 decodeInput.Count ""
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
        testList "File optional" [
            let fileItem = decodeInput.[2]
            testCase "Name" <| fun _ -> Expect.equal "argOptional"  fileItem.Name ""
            testCase "Type" <| fun _ -> 
                let expected = Union (ResizeArray [Null; File (FileInstance())])
                Expect.equal fileItem.Type_.Value expected "Should be Union of [Null; File]"
            testCase "Optional" <| fun _ -> Expect.equal (Some true) fileItem.Optional ""
        ]
        testList "File array optional" [
            let fileItem = decodeInput.[3]
            testCase "Name" <| fun _ -> Expect.equal "argOptionalMap"  fileItem.Name ""
            testCase "Type" <| fun _ -> 
                let expected = Union (ResizeArray [Null; Array {Items = File (FileInstance()); Label = None; Doc = None; Name = None}])
                Expect.equal fileItem.Type_.Value expected "Should be Union of [Null; File[]]"
            testCase "Optional" <| fun _ -> Expect.equal (Some true) fileItem.Optional ""
        ]
        testList "String" [
            let stringItem = decodeInput.[4]
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

let testInputMutationApi =
    testList "Mutation API" [
        testCase "typed setters roundtrip values" <| fun _ ->
            let input = CWLInput("arg")
            input.Type_ <- Some CWLType.String
            input.InputBinding <- Some { Prefix = Some "--arg"; Position = Some 1; ItemSeparator = None; Separate = Some true }
            input.Optional <- Some true

            Expect.equal input.Type_ (Some CWLType.String) "Type_ setter should write DynamicObj-backed value."
            Expect.equal input.InputBinding (Some { Prefix = Some "--arg"; Position = Some 1; ItemSeparator = None; Separate = Some true }) "InputBinding setter should write value."
            Expect.equal input.Optional (Some true) "Optional setter should write value."

        testCase "typed setters can clear optional values" <| fun _ ->
            let input = CWLInput("arg", type_ = CWLType.Int, optional = true)
            input.Type_ <- None
            input.Optional <- None
            input.InputBinding <- None

            Expect.isNone input.Type_ "Type_ should be removable."
            Expect.isNone input.Optional "Optional should be removable."
            Expect.isNone input.InputBinding "InputBinding should be removable."
    ]

let testProcessingUnitInputOps =
    testList "ProcessingUnitOps Inputs" [
        testCase "getOrCreateToolInputs creates missing collection" <| fun _ ->
            let tool = CWLToolDescription(outputs = ResizeArray())
            let created = ProcessingUnitOps.getOrCreateToolInputs tool
            created.Add(CWLInput("x"))
            Expect.isSome tool.Inputs "Inputs should be initialized."
            Expect.equal tool.Inputs.Value.Count 1 "Created collection should be stored on tool."

        testCase "getInputs normalizes CommandLineTool option to empty list" <| fun _ ->
            let tool = CWLToolDescription(outputs = ResizeArray())
            let pu = CWLProcessingUnit.CommandLineTool tool
            let inputs = ProcessingUnitOps.getInputs pu
            Expect.equal inputs.Count 0 "Missing inputs should normalize to empty ResizeArray."

        testCase "getOrCreateExpressionToolInputs creates missing collection" <| fun _ ->
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray(), expression = "$(null)")
            let created = ProcessingUnitOps.getOrCreateExpressionToolInputs expressionTool
            created.Add(CWLInput("arg"))
            Expect.isSome expressionTool.Inputs "Inputs should be initialized."
            Expect.equal expressionTool.Inputs.Value.Count 1 "Created collection should be stored on expression tool."
    ]

let main = 
    testList "Input" [
        testInput
        testInputMutationApi
        testProcessingUnitInputOps
    ]
