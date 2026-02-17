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

        testCase "getToolInputsOrEmpty returns existing collection when inputs are present" <| fun _ ->
            let existing = ResizeArray [| CWLInput("existing") |]
            let tool = CWLToolDescription(outputs = ResizeArray(), inputs = existing)
            let actual = ProcessingUnitOps.getToolInputsOrEmpty tool
            Expect.isTrue (obj.ReferenceEquals(actual, existing)) "Existing tool input collection should be returned unchanged."

        testCase "getExpressionToolInputsOrEmpty returns existing collection when inputs are present" <| fun _ ->
            let existing = ResizeArray [| CWLInput("existing") |]
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray(), expression = "$(null)", inputs = existing)
            let actual = ProcessingUnitOps.getExpressionToolInputsOrEmpty expressionTool
            Expect.isTrue (obj.ReferenceEquals(actual, existing)) "Existing expression tool input collection should be returned unchanged."

        testCase "getOrCreateToolInputs returns existing collection when already initialized" <| fun _ ->
            let existing = ResizeArray [| CWLInput("existing") |]
            let tool = CWLToolDescription(outputs = ResizeArray(), inputs = existing)
            let actual = ProcessingUnitOps.getOrCreateToolInputs tool
            Expect.isTrue (obj.ReferenceEquals(actual, existing)) "Existing collection should be reused."

        testCase "getOrCreateExpressionToolInputs returns existing collection when already initialized" <| fun _ ->
            let existing = ResizeArray [| CWLInput("existing") |]
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray(), expression = "$(null)", inputs = existing)
            let actual = ProcessingUnitOps.getOrCreateExpressionToolInputs expressionTool
            Expect.isTrue (obj.ReferenceEquals(actual, existing)) "Existing collection should be reused."
    ]

let testProcessingUnitOutputOps =
    testList "ProcessingUnitOps Outputs" [
        testCase "getOutputs returns CommandLineTool outputs" <| fun _ ->
            let outputs = ResizeArray [| CWLOutput("toolOut") |]
            let pu = CWLProcessingUnit.CommandLineTool (CWLToolDescription(outputs = outputs))
            let actual = ProcessingUnitOps.getOutputs pu
            Expect.isTrue (obj.ReferenceEquals(actual, outputs)) "CommandLineTool outputs should be returned unchanged."

        testCase "getOutputs returns Workflow outputs" <| fun _ ->
            let outputs = ResizeArray [| CWLOutput("wfOut") |]
            let workflow =
                CWLWorkflowDescription(
                    steps = ResizeArray(),
                    inputs = ResizeArray(),
                    outputs = outputs
                )
            let pu = CWLProcessingUnit.Workflow workflow
            let actual = ProcessingUnitOps.getOutputs pu
            Expect.isTrue (obj.ReferenceEquals(actual, outputs)) "Workflow outputs should be returned unchanged."

        testCase "getOutputs returns ExpressionTool outputs" <| fun _ ->
            let outputs = ResizeArray [| CWLOutput("expOut") |]
            let pu = CWLProcessingUnit.ExpressionTool (CWLExpressionToolDescription(outputs = outputs, expression = "$(null)"))
            let actual = ProcessingUnitOps.getOutputs pu
            Expect.isTrue (obj.ReferenceEquals(actual, outputs)) "ExpressionTool outputs should be returned unchanged."
    ]

let testProcessingUnitRequirementOps =
    testList "ProcessingUnitOps Requirements" [
        testCase "getRequirements normalizes missing requirements to empty for all variants" <| fun _ ->
            let toolReqs =
                CWLToolDescription(outputs = ResizeArray())
                |> CWLProcessingUnit.CommandLineTool
                |> ProcessingUnitOps.getRequirements

            let workflowReqs =
                CWLWorkflowDescription(steps = ResizeArray(), inputs = ResizeArray(), outputs = ResizeArray())
                |> CWLProcessingUnit.Workflow
                |> ProcessingUnitOps.getRequirements

            let expressionReqs =
                CWLExpressionToolDescription(outputs = ResizeArray(), expression = "$(null)")
                |> CWLProcessingUnit.ExpressionTool
                |> ProcessingUnitOps.getRequirements

            Expect.equal toolReqs.Count 0 "Tool requirements should normalize to empty collection."
            Expect.equal workflowReqs.Count 0 "Workflow requirements should normalize to empty collection."
            Expect.equal expressionReqs.Count 0 "ExpressionTool requirements should normalize to empty collection."

        testCase "getRequirements returns existing collection for all variants" <| fun _ ->
            let toolReqs = ResizeArray [| NetworkAccessRequirement { NetworkAccess = true } |]
            let workflowReqs = ResizeArray [| SubworkflowFeatureRequirement |]
            let expressionReqs = ResizeArray [| RequirementDefaults.inlineJavascriptRequirement |]

            let toolActual =
                CWLToolDescription(outputs = ResizeArray(), requirements = toolReqs)
                |> CWLProcessingUnit.CommandLineTool
                |> ProcessingUnitOps.getRequirements

            let workflowActual =
                CWLWorkflowDescription(steps = ResizeArray(), inputs = ResizeArray(), outputs = ResizeArray(), requirements = workflowReqs)
                |> CWLProcessingUnit.Workflow
                |> ProcessingUnitOps.getRequirements

            let expressionActual =
                CWLExpressionToolDescription(outputs = ResizeArray(), expression = "$(null)", requirements = expressionReqs)
                |> CWLProcessingUnit.ExpressionTool
                |> ProcessingUnitOps.getRequirements

            Expect.isTrue (obj.ReferenceEquals(toolActual, toolReqs)) "Tool requirements should reuse existing collection."
            Expect.isTrue (obj.ReferenceEquals(workflowActual, workflowReqs)) "Workflow requirements should reuse existing collection."
            Expect.isTrue (obj.ReferenceEquals(expressionActual, expressionReqs)) "ExpressionTool requirements should reuse existing collection."
    ]

let testProcessingUnitHintOps =
    testList "ProcessingUnitOps Hints" [
        testCase "getHints normalizes missing hints to empty for all variants" <| fun _ ->
            let toolHints =
                CWLToolDescription(outputs = ResizeArray())
                |> CWLProcessingUnit.CommandLineTool
                |> ProcessingUnitOps.getHints

            let workflowHints =
                CWLWorkflowDescription(steps = ResizeArray(), inputs = ResizeArray(), outputs = ResizeArray())
                |> CWLProcessingUnit.Workflow
                |> ProcessingUnitOps.getHints

            let expressionHints =
                CWLExpressionToolDescription(outputs = ResizeArray(), expression = "$(null)")
                |> CWLProcessingUnit.ExpressionTool
                |> ProcessingUnitOps.getHints

            Expect.equal toolHints.Count 0 "Tool hints should normalize to empty collection."
            Expect.equal workflowHints.Count 0 "Workflow hints should normalize to empty collection."
            Expect.equal expressionHints.Count 0 "ExpressionTool hints should normalize to empty collection."

        testCase "getOrCreate hint helpers initialize missing collections" <| fun _ ->
            let tool = CWLToolDescription(outputs = ResizeArray())
            let workflow = CWLWorkflowDescription(steps = ResizeArray(), inputs = ResizeArray(), outputs = ResizeArray())
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray(), expression = "$(null)")

            let toolHints = ProcessingUnitOps.getOrCreateToolHints tool
            let workflowHints = ProcessingUnitOps.getOrCreateWorkflowHints workflow
            let expressionHints = ProcessingUnitOps.getOrCreateExpressionToolHints expressionTool

            toolHints.Add(KnownHint RequirementDefaults.inlineJavascriptRequirement)
            workflowHints.Add(UnknownHint { Class = Some "acme:Hint"; Raw = Decode.read "class: acme:Hint" })
            expressionHints.Add(KnownHint (NetworkAccessRequirement { NetworkAccess = true }))

            Expect.equal tool.Hints.Value.Count 1 "Tool hints should be initialized and retained."
            Expect.equal workflow.Hints.Value.Count 1 "Workflow hints should be initialized and retained."
            Expect.equal expressionTool.Hints.Value.Count 1 "ExpressionTool hints should be initialized and retained."

        testCase "getKnownHints drops unknown hints" <| fun _ ->
            let hints =
                ResizeArray [|
                    KnownHint RequirementDefaults.inlineJavascriptRequirement
                    UnknownHint { Class = Some "acme:Hint"; Raw = Decode.read "class: acme:Hint" }
                    KnownHint (WorkReuseRequirement { EnableReuse = true })
                |]

            let tool = CWLToolDescription(outputs = ResizeArray(), hints = hints)
            let actual = tool |> CWLProcessingUnit.CommandLineTool |> ProcessingUnitOps.getKnownHints

            Expect.equal actual.Count 2 "Only known hints should be returned."
    ]

let main = 
    testList "Input" [
        testInput
        testInputMutationApi
        testProcessingUnitInputOps
        testProcessingUnitOutputOps
        testProcessingUnitRequirementOps
        testProcessingUnitHintOps
    ]
