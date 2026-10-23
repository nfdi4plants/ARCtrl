module Tests.Inputs

open ARCtrl.CWL
open DynamicObj
open YAMLicious
open TestingUtils

let decodeInput =
    TestObjects.CWL.Inputs.inputsFileContent
    |> Decode.readYaml
    |> Decode.inputsDecoder
    |>fun i ->i.Value

let testInput =
    testList "Decode" [
        testCase "Length" <| fun _ -> Expect.equal 5 decodeInput.Count ""
        testList "Directory" [
            let directoryItem = decodeInput.[0]
            testCase "Name" <| fun _ -> Expect.equal "arcDirectory"  directoryItem.Name ""
            testCase "Type" <| fun _ -> Expect.equal (Directory)  directoryItem.Type_.Value ""
        ]
        testList "File" [
            let fileItem = decodeInput.[1]
            testCase "Name" <| fun _ -> Expect.equal "firstArg"  fileItem.Name ""
            testCase "Type" <| fun _ -> Expect.equal (File)  fileItem.Type_.Value ""
            testCase "InputBinding" <| fun _ -> Expect.equal (Some (InputBinding.create(position = BindingPosition.Number 1, prefix = "--example"))) fileItem.InputBinding ""
        ]
        testList "File optional" [
            let fileItem = decodeInput.[2]
            testCase "Name" <| fun _ -> Expect.equal "argOptional"  fileItem.Name ""
            testCase "Type" <| fun _ -> 
                let expected = Union (ResizeArray [Null; File])
                Expect.equal fileItem.Type_.Value expected "Should be Union of [Null; File]"
            testCase "Optional" <| fun _ -> Expect.isTrue fileItem.Optional ""
        ]
        testList "File array optional" [
            let fileItem = decodeInput.[3]
            testCase "Name" <| fun _ -> Expect.equal "argOptionalMap"  fileItem.Name ""
            testCase "Type" <| fun _ -> 
                let expected = Union (ResizeArray [Null; Array (InputArraySchema(File))])
                Expect.equal fileItem.Type_.Value expected "Should be Union of [Null; File[]]"
            testCase "Optional" <| fun _ -> Expect.isTrue fileItem.Optional ""
        ]
        testList "String" [
            let stringItem = decodeInput.[4]
            testCase "Name" <| fun _ -> Expect.equal "secondArg"  stringItem.Name ""
            testCase "Type" <| fun _ ->
                let expected = String
                let actual = stringItem.Type_.Value
                Expect.equal actual expected ""
            testCase "InputBinding" <| fun _ ->
                let expected = Some (InputBinding.create(position = BindingPosition.Number 2, separate = false))
                let actual = stringItem.InputBinding
                Expect.equal actual expected ""
        ]
    ]

let testInputMutationApi =
    testList "Mutation API" [
        testCase "typed setters roundtrip values" <| fun _ ->
            let input = CWLInput("arg")
            input.Type_ <- Some CWLType.String
            input.InputBinding <- Some (InputBinding.create(prefix = "--arg", position = BindingPosition.Number 1, separate = true))

            Expect.equal input.Type_ (Some CWLType.String) "Type_ setter should write DynamicObj-backed value."
            Expect.equal input.InputBinding (Some (InputBinding.create(prefix = "--arg", position = BindingPosition.Number 1, separate = true))) "InputBinding setter should write value."
            Expect.isFalse input.Optional "A string is no optional type."

        testCase "typed setters can clear optional values" <| fun _ ->
            let input = CWLInput("arg", type_ = CWLType.Int, optional = true)
            input.Type_ <- None
            input.InputBinding <- None

            Expect.isNone input.Type_ "Type_ should be removable."
            Expect.isFalse input.Optional "An input without a type is not optional."
            Expect.isNone input.InputBinding "InputBinding should be removable."

        testCase "known fields are typed fields, not dynamic overflow" <| fun _ ->
            let binding = InputBinding.create(prefix = "--arg", position = BindingPosition.Number 1, itemSeparator = ",", separate = true, loadContents = true, valueFrom = "$(self)", shellQuote = false)
            let input =
                CWLInput(
                    "arg",
                    type_ = CWLType.String,
                    inputBinding = binding,
                    optional = true,
                    label = "Argument",
                    doc = "Argument docs",
                    format = InputFormat.Single "edam:format_2330",
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
                |> Decode.readYaml
                |> Decode.inputsDecoder
                |> Option.get
                |> fun inputs -> inputs.[0]

            Expect.equal input.Label (Some "Sample") "label should decode to a typed field."
            Expect.equal input.Doc (Some "Sample input docs") "doc should decode to a typed field."
            Expect.equal input.Format (Some (InputFormat.Single "edam:format_2330")) "format should decode to a typed field."
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

let testInputFormat =
    let decodedTool () = Decode.decodeCommandLineTool TestObjects.CWL.Inputs.formatFormsFileContent
    let decodedInputs () = (decodedTool ()).Inputs.Value
    let formatOf (inputs: ResizeArray<CWLInput>) (name: string) =
        (inputs |> Seq.find (fun input -> input.Name = name)).Format

    testList "Input format" [
        testCase "a name, a list of names, a list of one name and the empty list decode into their own forms" <| fun _ ->
            let inputs = decodedInputs ()
            Expect.equal (formatOf inputs "single") (Some (InputFormat.Single "edam:format_2330")) "A name is a single format."
            Expect.equal (formatOf inputs "several") (Some (InputFormat.Multiple (ResizeArray [ "edam:format_2330"; "edam:format_1930" ]))) "A list keeps its names in order."
            Expect.equal (formatOf inputs "one_in_list") (Some (InputFormat.Multiple (ResizeArray [ "edam:format_2330" ]))) "A list of one name is a list."
            Expect.equal (formatOf inputs "empty") (Some (InputFormat.Multiple (ResizeArray()))) "The empty list stays a list."
            Expect.notEqual (formatOf inputs "single") (formatOf inputs "one_in_list") "A name differs from a list of that name."

        testCase "each form of the format is written as it was read" <| fun _ ->
            let inputs = decodedInputs ()
            let encoded = Encode.encodeToolDescription (decodedTool ())
            Expect.stringContains encoded "edam: http://edamontology.org/" "The namespace of the prefix is written too."
            Expect.stringContains encoded "format: edam:format_2330\n" "A name is written as a scalar."
            Expect.stringContains encoded "format: [edam:format_2330, edam:format_1930]\n" "A list is written as a list."
            Expect.stringContains encoded "format: [edam:format_2330]\n" "A list of one name stays a list."
            Expect.stringContains encoded "format: []\n" "The empty list stays an empty list."
            let roundTripped = (Decode.decodeCommandLineTool encoded).Inputs.Value
            for input in inputs do
                Expect.equal (formatOf roundTripped input.Name) input.Format $"The format of {input.Name} survives the round trip."

        testCase "a mapping as the format of an input is a decode error at format" <| fun _ ->
            let yaml = """inputs:
  x:
    type: File
    format:
      iri: edam:format_2330"""
            match (try Decode.readYaml yaml |> Decode.inputsDecoder |> ignore; None with YamlDecodeException error -> Some error) with
            | Some error ->
                Expect.equal (DecodeError.pathText error) "inputs.x.format" "The error shows the format field."
                Expect.isSome error.Span "The error has the place in the text."
            | None -> failwith "A mapping as the format of an input must fail."

        testCase "a list as the format of an output is a decode error at format" <| fun _ ->
            let yaml = """outputs:
  result:
    type: File
    format: [edam:format_2330, edam:format_1930]"""
            match (try Decode.readYaml yaml |> Decode.outputsDecoder |> ignore; None with YamlDecodeException error -> Some error) with
            | Some error ->
                Expect.equal (DecodeError.pathText error) "outputs.result.format" "The error shows the format field."
                Expect.isSome error.Span "The error has the place in the text."
            | None -> failwith "A list as the format of an output must fail."

        testCase "formats compare and hash by their names" <| fun _ ->
            let several = InputFormat.Multiple (ResizeArray [ "a"; "b" ])
            Expect.equal several (InputFormat.Multiple (ResizeArray [ "a"; "b" ])) "Lists with the same names are equal."
            Expect.equal (hash several) (hash (InputFormat.Multiple (ResizeArray [ "a"; "b" ]))) "Equal lists have the same hash."
            Expect.notEqual several (InputFormat.Multiple (ResizeArray [ "b"; "a" ])) "The order of the names counts."
            let values = (InputFormat.Single "a").AsValues()
            Expect.sequenceEqual values (ResizeArray [ "a" ]) "A name reads as a list of one name."
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
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray(), expression = SchemaSaladString.Literal "$(null)")
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
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray(), expression = SchemaSaladString.Literal "$(null)", inputs = existing)
            let actual = CWLExpressionToolDescription.getInputsOrEmpty expressionTool
            Expect.isTrue (obj.ReferenceEquals(actual, existing)) "Existing expression tool input collection should be returned unchanged."

        testCase "getOrCreateToolInputs returns existing collection when already initialized" <| fun _ ->
            let existing = ResizeArray [| CWLInput("existing") |]
            let tool = CWLToolDescription(outputs = ResizeArray(), inputs = existing)
            let actual = CWLToolDescription.getOrCreateInputs tool
            Expect.isTrue (obj.ReferenceEquals(actual, existing)) "Existing collection should be reused."

        testCase "getOrCreateExpressionToolInputs returns existing collection when already initialized" <| fun _ ->
            let existing = ResizeArray [| CWLInput("existing") |]
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray(), expression = SchemaSaladString.Literal "$(null)", inputs = existing)
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
            let pu = CWLProcessingUnit.ExpressionTool (CWLExpressionToolDescription(outputs = outputs, expression = SchemaSaladString.Literal "$(null)"))
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
                CWLExpressionToolDescription(outputs = ResizeArray(), expression = SchemaSaladString.Literal "$(null)")
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
                CWLExpressionToolDescription(outputs = ResizeArray(), expression = SchemaSaladString.Literal "$(null)", requirements = expressionReqs)
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
                CWLExpressionToolDescription(outputs = ResizeArray(), expression = SchemaSaladString.Literal "$(null)")
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
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray(), expression = SchemaSaladString.Literal "$(null)")
            let operation = CWLOperationDescription(inputs = ResizeArray(), outputs = ResizeArray())

            let toolHints = CWLToolDescription.getOrCreateHints tool
            let workflowHints = CWLWorkflowDescription.getOrCreateHints workflow
            let expressionHints = CWLExpressionToolDescription.getOrCreateHints expressionTool
            let operationHints = CWLOperationDescription.getOrCreateHints operation

            toolHints.Add(KnownHint Requirement.defaultInlineJavascriptRequirement)
            workflowHints.Add(UnknownHint (HintUnknownValue(Some "acme:Hint", { Node = Decode.readYaml "class: acme:Hint" })))
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
                    UnknownHint (HintUnknownValue(Some "acme:Hint", { Node = Decode.readYaml "class: acme:Hint" }))
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
                CWLExpressionToolDescription(outputs = ResizeArray(), expression = SchemaSaladString.Literal "$(null)")
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
                CWLExpressionToolDescription(outputs = ResizeArray(), expression = SchemaSaladString.Literal "$(null)", intent = expressionIntentValues)
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
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray(), expression = SchemaSaladString.Literal "$(null)")
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

let testInputOptional =
    let tool (inputs: CWLInput list) =
        CWLToolDescription(ResizeArray [], inputs = ResizeArray inputs, baseCommand = ResizeArray [ "echo" ])

    testList "Optional input" [
        testCase "optional = true with an int is written as int? and reads back equal" <| fun _ ->
            let hand = tool [ CWLInput("x", CWLType.Int, optional = true) ]
            Expect.equal hand.Inputs.Value.[0].Type_ (Some (Union (ResizeArray [ Null; CWLType.Int ]))) "The optional int is the union of null and int."
            Expect.isTrue hand.Inputs.Value.[0].Optional "The input is optional."
            let encoded = Encode.encodeToolDescription hand
            Expect.stringContains encoded "  x: int?\n" "The optional int is the shorthand."
            Expect.equal (Decode.decodeCommandLineTool encoded) hand "The written text should read back as the same tool."
        testCase "the input decoded from the shorthand equals the input built with optional = true" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs:\n  x: int?\noutputs: []\n"
            Expect.equal decoded.Inputs.Value.[0] (CWLInput("x", CWLType.Int, optional = true)) "Both inputs have the union of null and int."
            Expect.equal decoded.Inputs.Value.[0] (CWLInput("x", Union (ResizeArray [ Null; CWLType.Int ]))) "A union with null is optional without the parameter."
        testCase "the parameter adds null to a type that has none and changes no other type" <| fun _ ->
            Expect.equal (CWLInput("a", Union (ResizeArray [ CWLType.String; CWLType.Int ]), optional = true)).Type_ (Some (Union (ResizeArray [ Null; CWLType.String; CWLType.Int ]))) "The members of a union stay the members of one union."
            Expect.equal (CWLInput("b", Union (ResizeArray [ CWLType.String; Null ]), optional = true)).Type_ (Some (Union (ResizeArray [ CWLType.String; Null ]))) "A union with null stays as it is."
            Expect.equal (CWLInput("c", CWLType.Int, optional = false)).Type_ (Some CWLType.Int) "The parameter false changes nothing."
            Expect.equal (CWLInput("d", Null, optional = true)).Type_ (Some Null) "The type null admits null already."
            Expect.isNone (CWLInput("e", optional = true)).Type_ "An input without a type has nothing to add null to."
            Expect.isFalse (CWLInput("e", optional = true)).Optional "An input without a type is not optional."
        testCase "an input is optional when its type is a union that contains null, wherever the null stands" <| fun _ ->
            let input = CWLInput("x", CWLType.String)
            Expect.isFalse input.Optional "A string is not optional."
            input.Type_ <- Some (Union (ResizeArray [ CWLType.String; CWLType.Int ]))
            Expect.isFalse input.Optional "A union without null is not optional."
            input.Type_ <- Some (Union (ResizeArray [ CWLType.String; Null ]))
            Expect.isTrue input.Optional "The null behind a type makes the input optional."
            input.Type_ <- Some (Union (ResizeArray [ CWLType.String; CWLType.Int; Null ]))
            Expect.isTrue input.Optional "The null behind two types makes the input optional."
            input.Type_ <- Some Null
            Expect.isFalse input.Optional "The bare type null is no union."
            let decoded = Decode.decodeCommandLineTool "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs:\n  x: [string, int, \"null\"]\noutputs: []\n"
            Expect.isTrue decoded.Inputs.Value.[0].Optional "A decoded union that contains null is optional."
        testCase "inputs of equal types are equal whether the type was given or came from the parameter" <| fun _ ->
            Expect.equal (hash (CWLInput("x", CWLType.Int, optional = true))) (hash (CWLInput("x", Union (ResizeArray [ Null; CWLType.Int ])))) "Equal inputs hash alike."
            Expect.notEqual (CWLInput("x", CWLType.Int)) (CWLInput("x", CWLType.Int, optional = true)) "An optional int differs from an int."
    ]

let testBindingPosition =
    let tool (position: string) =
        String.concat "\n" [ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo"; "inputs:"; "  x:"; "    type: string"; "    inputBinding:"; "      position: " + position; "outputs: []"; "" ]
    let positionOf (decoded: CWLToolDescription) = decoded.Inputs.Value.[0].InputBinding.Value.Position

    testList "Binding position" [
        testCase "an int is a number and an expression is an expression, and both are written as they were" <| fun _ ->
            for text, expected in [ "3", BindingPosition.Number 3; "-1", BindingPosition.Number -1; "0x2", BindingPosition.Number 2; "$(1 + 1)", BindingPosition.Expression "$(1 + 1)"; "${return 3;}", BindingPosition.Expression "${return 3;}" ] do
                let decoded = Decode.decodeCommandLineTool (tool text)
                Expect.equal (positionOf decoded) (Some expected) (sprintf "position: %s should decode as %A" text expected)
                let encoded = Encode.encodeToolDescription decoded
                Expect.equal (positionOf (Decode.decodeCommandLineTool encoded)) (Some expected) (sprintf "position: %s should survive a write and a read" text)
            let encoded = Encode.encodeToolDescription (Decode.decodeCommandLineTool (tool "$(1 + 1)"))
            Expect.stringContains encoded "      position: $(1 + 1)\n" "The expression is written as a plain scalar."
        testCase "a position that is neither an int nor an expression is a decode error at the position" <| fun _ ->
            for text in [ "abc"; "\"2\""; "1.5"; "true" ] do
                match (try Decode.decodeCommandLineTool (tool text) |> ignore; None with YamlDecodeException error -> Some error) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) "inputs.x.inputBinding.position" (sprintf "position: %s should fail at the field" text)
                    Expect.stringContains error.Message "Expected an int or an expression" (sprintf "position: %s should say what is wrong" text)
                    Expect.isSome error.Span (sprintf "position: %s should have the place in the text" text)
                | None -> failwith (sprintf "position: %s must fail" text)
        testCase "positions compare by their value" <| fun _ ->
            Expect.equal (BindingPosition.Number 1) (BindingPosition.Number 1) "Equal numbers are equal."
            Expect.notEqual (BindingPosition.Number 1) (BindingPosition.Expression "$(1)") "A number differs from an expression."
            Expect.equal (InputBinding.create(position = BindingPosition.Expression "$(1)")) (InputBinding.create(position = BindingPosition.Expression "$(1)")) "Bindings with equal positions are equal."
    ]

let main =
    testList "Input" [
        testInput
        testInputMutationApi
        testInputFormat
        testBindingPosition
        testInputOptional
        testProcessingUnitInputOps
        testProcessingUnitOutputOps
        testProcessingUnitRequirementOps
        testProcessingUnitHintOps
        testProcessingUnitIntentOps
    ]
