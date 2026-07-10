module Tests.Inputs

open ARCtrl.CWL
open DynamicObj
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
            testCase "InputBinding" <| fun _ -> Expect.equal (Some (InputBinding.create(position = 1, prefix = "--example"))) fileItem.InputBinding ""
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
                let expected = Union (ResizeArray [Null; Array (InputArraySchema(File (FileInstance())))])
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
                let expected = Some (InputBinding.create(position = 2, separate = false))
                let actual = stringItem.InputBinding
                Expect.equal actual expected ""
        ]
    ]

let testInputMutationApi =
    testList "Mutation API" [
        testCase "typed setters roundtrip values" <| fun _ ->
            let input = CWLInput("arg")
            input.Type_ <- Some CWLType.String
            input.InputBinding <- Some (InputBinding.create(prefix = "--arg", position = 1, separate = true))
            input.Optional <- Some true

            Expect.equal input.Type_ (Some CWLType.String) "Type_ setter should write DynamicObj-backed value."
            Expect.equal input.InputBinding (Some (InputBinding.create(prefix = "--arg", position = 1, separate = true))) "InputBinding setter should write value."
            Expect.equal input.Optional (Some true) "Optional setter should write value."

        testCase "typed setters can clear optional values" <| fun _ ->
            let input = CWLInput("arg", type_ = CWLType.Int, optional = true)
            input.Type_ <- None
            input.Optional <- None
            input.InputBinding <- None

            Expect.isNone input.Type_ "Type_ should be removable."
            Expect.isNone input.Optional "Optional should be removable."
            Expect.isNone input.InputBinding "InputBinding should be removable."

        testCase "known fields are typed fields, not dynamic overflow" <| fun _ ->
            let binding = InputBinding.create(prefix = "--arg", position = 1, itemSeparator = ",", separate = true, loadContents = true, valueFrom = "$(self)", shellQuote = false)
            let input =
                CWLInput(
                    "arg",
                    type_ = CWLType.String,
                    inputBinding = binding,
                    optional = true,
                    label = "Argument",
                    doc = "Argument docs",
                    format = "edam:format_2330",
                    loadContents = true,
                    loadListing = LoadListingEnum.ShallowListing,
                    streamable = false
                )

            Expect.sequenceEqual InputBinding.KnownFieldNames (Set [| "loadContents"; "position"; "prefix"; "separate"; "itemSeparator"; "valueFrom"; "shellQuote" |]) "InputBinding known fields should be declared on the type."
            Expect.sequenceEqual CWLInput.KnownFieldNames (Set [| "id"; "type"; "label"; "secondaryFiles"; "streamable"; "doc"; "format"; "loadContents"; "loadListing"; "default"; "inputBinding" |]) "CWLInput known fields should be declared on the type."
            Expect.isEmpty (binding |> DynamicObjHelpers.dynamicPropertiesSnapshot) "InputBinding known fields should not be stored as dynamic properties."
            Expect.isEmpty (input |> DynamicObjHelpers.dynamicPropertiesSnapshot) "CWLInput known fields should not be stored as dynamic properties."

            DynObj.setProperty "arc:note" "keep overflow" input
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:note" input) (Some "keep overflow") "Unknown fields should still use DynamicObj overflow."

        testCase "known DynamicObj keys are not encoded as input overflow" <| fun _ ->
            let input = CWLInput("arg", type_ = CWLType.String)
            DynObj.setProperty "label" "dynamic label" input
            DynObj.setProperty "arc:note" "keep overflow" input

            let _, encoded = Encode.encodeCWLInput input
            let yaml = Encode.writeYaml encoded

            Expect.isFalse (yaml.Contains("label")) "Known input fields should not be encoded from DynamicObj storage."
            Expect.stringContains yaml "arc:note" "Unknown overflow should still be encoded."

        testCase "missing optional inputBinding fields are not encoded as defaults" <| fun _ ->
            let binding = InputBinding.create(prefix = "--arg")

            let yaml =
                binding
                |> Encode.encodeInputBinding
                |> Encode.writeYaml

            Expect.stringContains yaml "prefix" "The present prefix field should be encoded."
            Expect.isFalse (yaml.Contains("position")) "Missing position should not encode as 0."
            Expect.isFalse (yaml.Contains("_position")) "Compiler backing storage should not be encoded."
            Expect.isFalse (yaml.Contains("itemSeparator")) "Missing itemSeparator should not encode as an empty string."

        testCase "spec input fields decode as typed members regardless of order" <| fun _ ->
            let input =
                TestObjects.CWL.Inputs.specInputFieldsDecodeFileContent
                |> Decode.read
                |> Decode.inputsDecoder
                |> Option.get
                |> fun inputs -> inputs.[0]

            Expect.equal input.Label (Some "Sample") "label should decode to a typed field."
            Expect.equal input.Doc (Some "Sample input docs") "doc should decode to a typed field."
            Expect.equal input.Format (Some "edam:format_2330") "format should decode to a typed field."
            Expect.equal input.LoadContents (Some true) "loadContents should decode to a typed field."
            Expect.equal input.LoadListing (Some LoadListingEnum.ShallowListing) "loadListing should decode to a typed field."
            Expect.equal input.Streamable (Some false) "streamable should decode to a typed field."
            Expect.isSome input.DefaultValue "default should decode to a typed field."
            Expect.isSome input.SecondaryFiles "secondaryFiles should decode to a typed field."
            Expect.equal input.InputBinding.Value.ValueFrom (Some "$(self.path)") "inputBinding.valueFrom should decode to a typed field."
            Expect.equal input.InputBinding.Value.LoadContents (Some true) "inputBinding.loadContents should decode to a typed field."
            Expect.equal input.InputBinding.Value.ShellQuote (Some false) "inputBinding.shellQuote should decode to a typed field."
            Expect.isEmpty (input |> DynamicObjHelpers.dynamicPropertiesSnapshot) "Known input fields should not be overflow."
    ]

let testProcessingUnitInputOps =
    testList "CWLProcessingUnit Inputs" [
        testCase "getOrCreateToolInputs creates missing collection" <| fun _ ->
            let tool = CWLToolDescription(outputs = ResizeArray())
            let created = CWLToolDescription.getOrCreateInputs tool
            created.Add(CWLInput("x"))
            Expect.isSome tool.Inputs "Inputs should be initialized."
            Expect.equal tool.Inputs.Value.Count 1 "Created collection should be stored on tool."

        testCase "getInputs normalizes CommandLineTool option to empty list" <| fun _ ->
            let tool = CWLToolDescription(outputs = ResizeArray())
            let pu = CWLProcessingUnit.CommandLineTool tool
            let inputs = CWLProcessingUnit.getInputs pu
            Expect.equal inputs.Count 0 "Missing inputs should normalize to empty ResizeArray."

        testCase "getOrCreateExpressionToolInputs creates missing collection" <| fun _ ->
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray(), expression = "$(null)")
            let created = CWLExpressionToolDescription.getOrCreateInputs expressionTool
            created.Add(CWLInput("arg"))
            Expect.isSome expressionTool.Inputs "Inputs should be initialized."
            Expect.equal expressionTool.Inputs.Value.Count 1 "Created collection should be stored on expression tool."

        testCase "getToolInputsOrEmpty returns existing collection when inputs are present" <| fun _ ->
            let existing = ResizeArray [| CWLInput("existing") |]
            let tool = CWLToolDescription(outputs = ResizeArray(), inputs = existing)
            let actual = CWLToolDescription.getInputsOrEmpty tool
            Expect.isTrue (obj.ReferenceEquals(actual, existing)) "Existing tool input collection should be returned unchanged."

        testCase "getExpressionToolInputsOrEmpty returns existing collection when inputs are present" <| fun _ ->
            let existing = ResizeArray [| CWLInput("existing") |]
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray(), expression = "$(null)", inputs = existing)
            let actual = CWLExpressionToolDescription.getInputsOrEmpty expressionTool
            Expect.isTrue (obj.ReferenceEquals(actual, existing)) "Existing expression tool input collection should be returned unchanged."

        testCase "getOrCreateToolInputs returns existing collection when already initialized" <| fun _ ->
            let existing = ResizeArray [| CWLInput("existing") |]
            let tool = CWLToolDescription(outputs = ResizeArray(), inputs = existing)
            let actual = CWLToolDescription.getOrCreateInputs tool
            Expect.isTrue (obj.ReferenceEquals(actual, existing)) "Existing collection should be reused."

        testCase "getOrCreateExpressionToolInputs returns existing collection when already initialized" <| fun _ ->
            let existing = ResizeArray [| CWLInput("existing") |]
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray(), expression = "$(null)", inputs = existing)
            let actual = CWLExpressionToolDescription.getOrCreateInputs expressionTool
            Expect.isTrue (obj.ReferenceEquals(actual, existing)) "Existing collection should be reused."

        testCase "getInputs returns Operation inputs" <| fun _ ->
            let inputs = ResizeArray [| CWLInput("opIn") |]
            let operation = CWLOperationDescription(inputs = inputs, outputs = ResizeArray())
            let pu = CWLProcessingUnit.Operation operation
            let actual = CWLProcessingUnit.getInputs pu
            Expect.isTrue (obj.ReferenceEquals(actual, inputs)) "Operation inputs should be returned unchanged."
    ]

let testProcessingUnitOutputOps =
    testList "CWLProcessingUnit Outputs" [
        testCase "getOutputs returns CommandLineTool outputs" <| fun _ ->
            let outputs = ResizeArray [| CWLOutput("toolOut") |]
            let pu = CWLProcessingUnit.CommandLineTool (CWLToolDescription(outputs = outputs))
            let actual = CWLProcessingUnit.getOutputs pu
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
            let actual = CWLProcessingUnit.getOutputs pu
            Expect.isTrue (obj.ReferenceEquals(actual, outputs)) "Workflow outputs should be returned unchanged."

        testCase "getOutputs returns ExpressionTool outputs" <| fun _ ->
            let outputs = ResizeArray [| CWLOutput("expOut") |]
            let pu = CWLProcessingUnit.ExpressionTool (CWLExpressionToolDescription(outputs = outputs, expression = "$(null)"))
            let actual = CWLProcessingUnit.getOutputs pu
            Expect.isTrue (obj.ReferenceEquals(actual, outputs)) "ExpressionTool outputs should be returned unchanged."

        testCase "getOutputs returns Operation outputs" <| fun _ ->
            let outputs = ResizeArray [| CWLOutput("opOut") |]
            let operation = CWLOperationDescription(inputs = ResizeArray(), outputs = outputs)
            let pu = CWLProcessingUnit.Operation operation
            let actual = CWLProcessingUnit.getOutputs pu
            Expect.isTrue (obj.ReferenceEquals(actual, outputs)) "Operation outputs should be returned unchanged."
    ]

let testProcessingUnitRequirementOps =
    testList "CWLProcessingUnit Requirements" [
        testCase "getRequirements normalizes missing requirements to empty for all variants" <| fun _ ->
            let toolReqs =
                CWLToolDescription(outputs = ResizeArray())
                |> CWLProcessingUnit.CommandLineTool
                |> CWLProcessingUnit.getRequirements

            let workflowReqs =
                CWLWorkflowDescription(steps = ResizeArray(), inputs = ResizeArray(), outputs = ResizeArray())
                |> CWLProcessingUnit.Workflow
                |> CWLProcessingUnit.getRequirements

            let expressionReqs =
                CWLExpressionToolDescription(outputs = ResizeArray(), expression = "$(null)")
                |> CWLProcessingUnit.ExpressionTool
                |> CWLProcessingUnit.getRequirements

            let operationReqs =
                CWLOperationDescription(inputs = ResizeArray(), outputs = ResizeArray())
                |> CWLProcessingUnit.Operation
                |> CWLProcessingUnit.getRequirements

            Expect.equal toolReqs.Count 0 "Tool requirements should normalize to empty collection."
            Expect.equal workflowReqs.Count 0 "Workflow requirements should normalize to empty collection."
            Expect.equal expressionReqs.Count 0 "ExpressionTool requirements should normalize to empty collection."
            Expect.equal operationReqs.Count 0 "Operation requirements should normalize to empty collection."

        testCase "getRequirements returns existing collection for all variants" <| fun _ ->
            let toolReqs = ResizeArray [| NetworkAccessRequirement (NetworkAccessRequirementValue(true)) |]
            let workflowReqs = ResizeArray [| SubworkflowFeatureRequirement |]
            let expressionReqs = ResizeArray [| Requirement.defaultInlineJavascriptRequirement |]
            let operationReqs = ResizeArray [| Requirement.defaultInlineJavascriptRequirement |]

            let toolActual =
                CWLToolDescription(outputs = ResizeArray(), requirements = toolReqs)
                |> CWLProcessingUnit.CommandLineTool
                |> CWLProcessingUnit.getRequirements

            let workflowActual =
                CWLWorkflowDescription(steps = ResizeArray(), inputs = ResizeArray(), outputs = ResizeArray(), requirements = workflowReqs)
                |> CWLProcessingUnit.Workflow
                |> CWLProcessingUnit.getRequirements

            let expressionActual =
                CWLExpressionToolDescription(outputs = ResizeArray(), expression = "$(null)", requirements = expressionReqs)
                |> CWLProcessingUnit.ExpressionTool
                |> CWLProcessingUnit.getRequirements

            let operationActual =
                CWLOperationDescription(inputs = ResizeArray(), outputs = ResizeArray(), requirements = operationReqs)
                |> CWLProcessingUnit.Operation
                |> CWLProcessingUnit.getRequirements

            Expect.isTrue (obj.ReferenceEquals(toolActual, toolReqs)) "Tool requirements should reuse existing collection."
            Expect.isTrue (obj.ReferenceEquals(workflowActual, workflowReqs)) "Workflow requirements should reuse existing collection."
            Expect.isTrue (obj.ReferenceEquals(expressionActual, expressionReqs)) "ExpressionTool requirements should reuse existing collection."
            Expect.isTrue (obj.ReferenceEquals(operationActual, operationReqs)) "Operation requirements should reuse existing collection."
    ]

let testProcessingUnitHintOps =
    testList "CWLProcessingUnit Hints" [
        testCase "getHints normalizes missing hints to empty for all variants" <| fun _ ->
            let toolHints =
                CWLToolDescription(outputs = ResizeArray())
                |> CWLProcessingUnit.CommandLineTool
                |> CWLProcessingUnit.getHints

            let workflowHints =
                CWLWorkflowDescription(steps = ResizeArray(), inputs = ResizeArray(), outputs = ResizeArray())
                |> CWLProcessingUnit.Workflow
                |> CWLProcessingUnit.getHints

            let expressionHints =
                CWLExpressionToolDescription(outputs = ResizeArray(), expression = "$(null)")
                |> CWLProcessingUnit.ExpressionTool
                |> CWLProcessingUnit.getHints

            let operationHints =
                CWLOperationDescription(inputs = ResizeArray(), outputs = ResizeArray())
                |> CWLProcessingUnit.Operation
                |> CWLProcessingUnit.getHints

            Expect.equal toolHints.Count 0 "Tool hints should normalize to empty collection."
            Expect.equal workflowHints.Count 0 "Workflow hints should normalize to empty collection."
            Expect.equal expressionHints.Count 0 "ExpressionTool hints should normalize to empty collection."
            Expect.equal operationHints.Count 0 "Operation hints should normalize to empty collection."

        testCase "getOrCreate hint helpers initialize missing collections" <| fun _ ->
            let tool = CWLToolDescription(outputs = ResizeArray())
            let workflow = CWLWorkflowDescription(steps = ResizeArray(), inputs = ResizeArray(), outputs = ResizeArray())
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray(), expression = "$(null)")
            let operation = CWLOperationDescription(inputs = ResizeArray(), outputs = ResizeArray())

            let toolHints = CWLToolDescription.getOrCreateHints tool
            let workflowHints = CWLWorkflowDescription.getOrCreateHints workflow
            let expressionHints = CWLExpressionToolDescription.getOrCreateHints expressionTool
            let operationHints = CWLOperationDescription.getOrCreateHints operation

            toolHints.Add(KnownHint Requirement.defaultInlineJavascriptRequirement)
            workflowHints.Add(UnknownHint (HintUnknownValue(Some "acme:Hint", Decode.read "class: acme:Hint")))
            expressionHints.Add(KnownHint (NetworkAccessRequirement (NetworkAccessRequirementValue(true))))
            operationHints.Add(KnownHint Requirement.defaultInlineJavascriptRequirement)

            Expect.equal tool.Hints.Value.Count 1 "Tool hints should be initialized and retained."
            Expect.equal workflow.Hints.Value.Count 1 "Workflow hints should be initialized and retained."
            Expect.equal expressionTool.Hints.Value.Count 1 "ExpressionTool hints should be initialized and retained."
            Expect.equal operation.Hints.Value.Count 1 "Operation hints should be initialized and retained."

        testCase "getKnownHints drops unknown hints" <| fun _ ->
            let hints =
                ResizeArray [|
                    KnownHint Requirement.defaultInlineJavascriptRequirement
                    UnknownHint (HintUnknownValue(Some "acme:Hint", Decode.read "class: acme:Hint"))
                    KnownHint (WorkReuseRequirement (WorkReuseRequirementValue(true)))
                |]

            let tool = CWLToolDescription(outputs = ResizeArray(), hints = hints)
            let actual = tool |> CWLProcessingUnit.CommandLineTool |> CWLProcessingUnit.getKnownHints

            Expect.equal actual.Count 2 "Only known hints should be returned."
    ]

let testProcessingUnitIntentOps =
    testList "CWLProcessingUnit Intent" [
        testCase "getIntent normalizes missing intent to empty for all variants" <| fun _ ->
            let toolIntent =
                CWLToolDescription(outputs = ResizeArray())
                |> CWLProcessingUnit.CommandLineTool
                |> CWLProcessingUnit.getIntent

            let workflowIntent =
                CWLWorkflowDescription(steps = ResizeArray(), inputs = ResizeArray(), outputs = ResizeArray())
                |> CWLProcessingUnit.Workflow
                |> CWLProcessingUnit.getIntent

            let expressionIntent =
                CWLExpressionToolDescription(outputs = ResizeArray(), expression = "$(null)")
                |> CWLProcessingUnit.ExpressionTool
                |> CWLProcessingUnit.getIntent

            let operationIntent =
                CWLOperationDescription(inputs = ResizeArray(), outputs = ResizeArray())
                |> CWLProcessingUnit.Operation
                |> CWLProcessingUnit.getIntent

            Expect.equal toolIntent.Count 0 "Tool intent should normalize to empty collection."
            Expect.equal workflowIntent.Count 0 "Workflow intent should normalize to empty collection."
            Expect.equal expressionIntent.Count 0 "ExpressionTool intent should normalize to empty collection."
            Expect.equal operationIntent.Count 0 "Operation intent should normalize to empty collection."

        testCase "getIntent returns existing collection for all variants" <| fun _ ->
            let toolIntentValues = ResizeArray [|"classification"|]
            let workflowIntentValues = ResizeArray [|"analysis"|]
            let expressionIntentValues = ResizeArray [|"post-processing"|]
            let operationIntentValues = ResizeArray [|"orchestration"|]

            let toolActual =
                CWLToolDescription(outputs = ResizeArray(), intent = toolIntentValues)
                |> CWLProcessingUnit.CommandLineTool
                |> CWLProcessingUnit.getIntent

            let workflowActual =
                CWLWorkflowDescription(steps = ResizeArray(), inputs = ResizeArray(), outputs = ResizeArray(), intent = workflowIntentValues)
                |> CWLProcessingUnit.Workflow
                |> CWLProcessingUnit.getIntent

            let expressionActual =
                CWLExpressionToolDescription(outputs = ResizeArray(), expression = "$(null)", intent = expressionIntentValues)
                |> CWLProcessingUnit.ExpressionTool
                |> CWLProcessingUnit.getIntent

            let operationActual =
                CWLOperationDescription(inputs = ResizeArray(), outputs = ResizeArray(), intent = operationIntentValues)
                |> CWLProcessingUnit.Operation
                |> CWLProcessingUnit.getIntent

            Expect.isTrue (obj.ReferenceEquals(toolActual, toolIntentValues)) "Tool intent should reuse existing collection."
            Expect.isTrue (obj.ReferenceEquals(workflowActual, workflowIntentValues)) "Workflow intent should reuse existing collection."
            Expect.isTrue (obj.ReferenceEquals(expressionActual, expressionIntentValues)) "ExpressionTool intent should reuse existing collection."
            Expect.isTrue (obj.ReferenceEquals(operationActual, operationIntentValues)) "Operation intent should reuse existing collection."

        testCase "getOrCreate intent helpers initialize missing collections" <| fun _ ->
            let tool = CWLToolDescription(outputs = ResizeArray())
            let workflow = CWLWorkflowDescription(steps = ResizeArray(), inputs = ResizeArray(), outputs = ResizeArray())
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray(), expression = "$(null)")
            let operation = CWLOperationDescription(inputs = ResizeArray(), outputs = ResizeArray())

            let toolIntent = CWLToolDescription.getOrCreateIntent tool
            let workflowIntent = CWLWorkflowDescription.getOrCreateIntent workflow
            let expressionIntent = CWLExpressionToolDescription.getOrCreateIntent expressionTool
            let operationIntent = CWLOperationDescription.getOrCreateIntent operation

            toolIntent.Add("classification")
            workflowIntent.Add("analysis")
            expressionIntent.Add("post-processing")
            operationIntent.Add("orchestration")

            Expect.equal tool.Intent.Value.Count 1 "Tool intent should be initialized and retained."
            Expect.equal workflow.Intent.Value.Count 1 "Workflow intent should be initialized and retained."
            Expect.equal expressionTool.Intent.Value.Count 1 "ExpressionTool intent should be initialized and retained."
            Expect.equal operation.Intent.Value.Count 1 "Operation intent should be initialized and retained."
    ]

let main = 
    testList "Input" [
        testInput
        testInputMutationApi
        testProcessingUnitInputOps
        testProcessingUnitOutputOps
        testProcessingUnitRequirementOps
        testProcessingUnitHintOps
        testProcessingUnitIntentOps
    ]
