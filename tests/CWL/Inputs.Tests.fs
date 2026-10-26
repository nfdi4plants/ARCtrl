module Tests.Inputs

open ARCtrl.CWL
open DynamicObj
open YAMLicious
open TestingUtils
open TestingUtils.CWL

let decodeInput =
    TestObjects.CWL.Inputs.inputsFileContent
    |> Decode.readYaml
    |> Decode.inputsDecoder
    |> Option.map ListEntry.values
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
            testCase "InputBinding" <| fun _ -> Expect.equal (Some (ListEntry.Value (InputBinding.create(position = BindingPosition.Number 1, prefix = (SchemaSaladString.Literal "--example"))))) fileItem.InputBinding ""
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
                let expected = Some (ListEntry.Value (InputBinding.create(position = BindingPosition.Number 2, separate = SchemaSaladBool.Value false)))
                let actual = stringItem.InputBinding
                Expect.equal actual expected ""
        ]
    ]

let testInputMutationApi =
    testList "Mutation API" [
        testCase "typed setters roundtrip values" <| fun _ ->
            let input = CWLInput("arg")
            input.Type_ <- Some CWLType.String
            input.InputBinding <- Some (ListEntry.Value (InputBinding.create(prefix = (SchemaSaladString.Literal "--arg"), position = BindingPosition.Number 1, separate = SchemaSaladBool.Value true)))

            Expect.equal input.Type_ (Some CWLType.String) "Type_ setter should write DynamicObj-backed value."
            Expect.equal input.InputBinding (Some (ListEntry.Value (InputBinding.create(prefix = (SchemaSaladString.Literal "--arg"), position = BindingPosition.Number 1, separate = SchemaSaladBool.Value true)))) "InputBinding setter should write value."
            Expect.isFalse input.Optional "A string is no optional type."

        testCase "typed setters can clear optional values" <| fun _ ->
            let input = CWLInput("arg", type_ = CWLType.makeOptional CWLType.Int)
            input.Type_ <- None
            input.InputBinding <- None

            Expect.isNone input.Type_ "Type_ should be removable."
            Expect.isFalse input.Optional "An input without a type is not optional."
            Expect.isNone input.InputBinding "InputBinding should be removable."

        testCase "known fields are typed fields, not dynamic overflow" <| fun _ ->
            let binding = InputBinding.create(prefix = (SchemaSaladString.Literal "--arg"), position = BindingPosition.Number 1, itemSeparator = (SchemaSaladString.Literal ","), separate = SchemaSaladBool.Value true, loadContents = SchemaSaladBool.Value true, valueFrom = (SchemaSaladString.Literal "$(self)"), shellQuote = SchemaSaladBool.Value false)
            let input =
                CWLInput(
                    "arg",
                    type_ = CWLType.makeOptional CWLType.String,
                    inputBinding = ListEntry.Value binding,
                    label = (SchemaSaladString.Literal "Argument"),
                    doc = (DocText.literal "Argument docs"),
                    format = InputFormat.Single (SchemaSaladString.Literal "edam:format_2330"),
                    loadContents = SchemaSaladBool.Value true,
                    loadListing = LoadListingEnum.ShallowListing,
                    streamable = SchemaSaladBool.Value false
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
            let binding = InputBinding.create(prefix = (SchemaSaladString.Literal "--arg"))

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
                |> Option.map ListEntry.values
                |> Option.get
                |> fun inputs -> inputs.[0]

            Expect.equal input.Label (Some (SchemaSaladString.Literal "Sample")) "label should decode to a typed field."
            Expect.equal input.Doc (Some (DocText.literal "Sample input docs")) "doc should decode to a typed field."
            Expect.equal input.Format (Some (InputFormat.Single (SchemaSaladString.Literal "edam:format_2330"))) "format should decode to a typed field."
            Expect.equal input.LoadContents (Some (SchemaSaladBool.Value true)) "loadContents should decode to a typed field."
            Expect.equal input.LoadListing (Some LoadListingEnum.ShallowListing) "loadListing should decode to a typed field."
            Expect.equal input.Streamable (Some (SchemaSaladBool.Value false)) "streamable should decode to a typed field."
            Expect.isSome input.DefaultValue "default should decode to a typed field."
            Expect.isSome input.SecondaryFiles "secondaryFiles should decode to a typed field."
            Expect.equal (inputBindingOf input).ValueFrom (Some (SchemaSaladString.Literal "$(self.path)")) "inputBinding.valueFrom should decode to a typed field."
            Expect.equal (inputBindingOf input).LoadContents (Some (SchemaSaladBool.Value true)) "inputBinding.loadContents should decode to a typed field."
            Expect.equal (inputBindingOf input).ShellQuote (Some (SchemaSaladBool.Value false)) "inputBinding.shellQuote should decode to a typed field."
            Expect.isEmpty (input |> DynamicObjHelpers.dynamicPropertiesSnapshot) "Known input fields should not be overflow."
    ]

let testInputFormat =
    let decodedTool () = Decode.decodeCommandLineTool TestObjects.CWL.Inputs.formatFormsFileContent
    let decodedInputs () = (ListEntry.values (decodedTool ()).Inputs.Value)
    let formatOf (inputs: ResizeArray<CWLInput>) (name: string) =
        (inputs |> Seq.find (fun input -> input.Name = name)).Format

    testList "Input format" [
        testCase "a name, a list of names, a list of one name and the empty list decode into their own forms" <| fun _ ->
            let inputs = decodedInputs ()
            Expect.equal (formatOf inputs "single") (Some (InputFormat.Single (SchemaSaladString.Literal "edam:format_2330"))) "A name is a single format."
            Expect.equal (formatOf inputs "several") (Some (InputFormat.Multiple (ResizeArray [ (SchemaSaladString.Literal "edam:format_2330"); (SchemaSaladString.Literal "edam:format_1930") ]))) "A list keeps its names in order."
            Expect.equal (formatOf inputs "one_in_list") (Some (InputFormat.Multiple (ResizeArray [ (SchemaSaladString.Literal "edam:format_2330") ]))) "A list of one name is a list."
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
            let roundTripped = (ListEntry.values (Decode.decodeCommandLineTool encoded).Inputs.Value)
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
            let several = InputFormat.Multiple (ResizeArray [ (SchemaSaladString.Literal "a"); (SchemaSaladString.Literal "b") ])
            Expect.equal several (InputFormat.Multiple (ResizeArray [ (SchemaSaladString.Literal "a"); (SchemaSaladString.Literal "b") ])) "Lists with the same names are equal."
            Expect.equal (hash several) (hash (InputFormat.Multiple (ResizeArray [ (SchemaSaladString.Literal "a"); (SchemaSaladString.Literal "b") ]))) "Equal lists have the same hash."
            Expect.notEqual several (InputFormat.Multiple (ResizeArray [ (SchemaSaladString.Literal "b"); (SchemaSaladString.Literal "a") ])) "The order of the names counts."
            let values = (InputFormat.Single (SchemaSaladString.Literal "a")).AsValues()
            Expect.sequenceEqual values (ResizeArray [ SchemaSaladString.Literal "a" ]) "A name reads as a list of one name."
    ]

let testProcessingUnitInputOps =
    testList "CWLProcessingUnit Inputs" [
        testCase "getOrCreateToolInputs creates missing collection" <| fun _ ->
            let tool = CWLToolDescription(outputs = ResizeArray())
            let created = CWLToolDescription.getOrCreateInputs tool
            created.Add(ListEntry.Value (CWLInput("x")))
            Expect.isSome tool.Inputs "Inputs should be initialized."
            Expect.equal tool.Inputs.Value.Count 1 "Created collection should be stored on tool."

        testCase "getInputValues normalizes CommandLineTool option to empty list" <| fun _ ->
            let tool = CWLToolDescription(outputs = ResizeArray())
            let pu = CWLProcessingUnit.CommandLineTool tool
            let inputs = CWLProcessingUnit.getInputValues pu
            Expect.equal inputs.Count 0 "Missing inputs should normalize to empty ResizeArray."

        testCase "getOrCreateExpressionToolInputs creates missing collection" <| fun _ ->
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray(), expression = SchemaSaladString.Literal "$(null)")
            let created = CWLExpressionToolDescription.getOrCreateInputs expressionTool
            created.Add(ListEntry.Value (CWLInput("arg")))
            Expect.isSome expressionTool.Inputs "Inputs should be initialized."
            Expect.equal (ListEntry.values expressionTool.Inputs.Value).Count 1 "Created collection should be stored on expression tool."

        testCase "getInputValuesOrEmpty returns the inputs of the tool when they are present" <| fun _ ->
            let existing = ResizeArray [| CWLInput("existing") |]
            let tool = CWLToolDescription(outputs = ResizeArray(), inputs = ListEntry.ofValues existing)
            let actual = CWLToolDescription.getInputValuesOrEmpty tool
            Expect.sequenceEqual actual existing "The inputs of the tool should be returned."
            actual.Add(CWLInput("other"))
            Expect.equal tool.Inputs.Value.Count 1 "The list is a new one, so adding to it leaves the tool as it was."

        testCase "getInputValuesOrEmpty of an expression tool returns the inputs of the expression tool when they are present" <| fun _ ->
            let existing = ResizeArray [| CWLInput("existing") |]
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray(), expression = SchemaSaladString.Literal "$(null)", inputs = ListEntry.ofValues existing)
            let actual = CWLExpressionToolDescription.getInputValuesOrEmpty expressionTool
            Expect.sequenceEqual actual existing "The inputs of the expression tool should be returned."

        testCase "getOrCreateToolInputs returns existing collection when already initialized" <| fun _ ->
            let existing = ListEntry.ofValues [ CWLInput("existing") ]
            let tool = CWLToolDescription(outputs = ResizeArray(), inputs = existing)
            let actual = CWLToolDescription.getOrCreateInputs tool
            Expect.isTrue (obj.ReferenceEquals(actual, existing)) "Existing collection should be reused."

        testCase "getOrCreateExpressionToolInputs returns existing collection when already initialized" <| fun _ ->
            let existing = ListEntry.ofValues [ CWLInput("existing") ]
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray(), expression = SchemaSaladString.Literal "$(null)", inputs = existing)
            let actual = CWLExpressionToolDescription.getOrCreateInputs expressionTool
            Expect.isTrue (obj.ReferenceEquals(actual, existing)) "Existing collection should be reused."

        testCase "getInputValues returns Operation inputs" <| fun _ ->
            let inputs = ResizeArray [| CWLInput("opIn") |]
            let operation = CWLOperationDescription(inputs = ListEntry.ofValues inputs, outputs = ResizeArray())
            let pu = CWLProcessingUnit.Operation operation
            let actual = CWLProcessingUnit.getInputValues pu
            Expect.sequenceEqual actual inputs "Operation inputs should be returned."
    ]

let testProcessingUnitOutputOps =
    testList "CWLProcessingUnit Outputs" [
        testCase "getOutputValues returns CommandLineTool outputs" <| fun _ ->
            let outputs = ResizeArray [| CWLOutput("toolOut") |]
            let pu = CWLProcessingUnit.CommandLineTool (CWLToolDescription(outputs = ListEntry.ofValues outputs))
            let actual = CWLProcessingUnit.getOutputValues pu
            Expect.sequenceEqual actual outputs "CommandLineTool outputs should be returned."

        testCase "getOutputValues returns Workflow outputs" <| fun _ ->
            let outputs = ResizeArray [| CWLOutput("wfOut") |]
            let workflow =
                CWLWorkflowDescription(
                    steps = ResizeArray(),
                    inputs = ResizeArray(),
                    outputs = ListEntry.ofValues outputs
                )
            let pu = CWLProcessingUnit.Workflow workflow
            let actual = CWLProcessingUnit.getOutputValues pu
            Expect.sequenceEqual actual outputs "Workflow outputs should be returned."

        testCase "getOutputValues returns ExpressionTool outputs" <| fun _ ->
            let outputs = ResizeArray [| CWLOutput("expOut") |]
            let pu = CWLProcessingUnit.ExpressionTool (CWLExpressionToolDescription(outputs = ListEntry.ofValues outputs, expression = SchemaSaladString.Literal "$(null)"))
            let actual = CWLProcessingUnit.getOutputValues pu
            Expect.sequenceEqual actual outputs "ExpressionTool outputs should be returned."

        testCase "getOutputValues returns Operation outputs" <| fun _ ->
            let outputs = ResizeArray [| CWLOutput("opOut") |]
            let operation = CWLOperationDescription(inputs = ResizeArray(), outputs = ListEntry.ofValues outputs)
            let pu = CWLProcessingUnit.Operation operation
            let actual = CWLProcessingUnit.getOutputValues pu
            Expect.sequenceEqual actual outputs "Operation outputs should be returned."
    ]

let testProcessingUnitRequirementOps =
    testList "CWLProcessingUnit Requirements" [
        testCase "the value getters return a new list, so adding to it changes nothing in the process" <| fun _ ->
            let tool = CWLToolDescription(outputs = ListEntry.ofValues [ CWLOutput("o") ], inputs = ListEntry.ofValues [ CWLInput("i") ])
            let expressionTool = CWLExpressionToolDescription(outputs = ListEntry.ofValues [ CWLOutput("o") ], expression = SchemaSaladString.Literal "$(null)", inputs = ListEntry.ofValues [ CWLInput("i") ])
            let operation = CWLOperationDescription(inputs = ListEntry.ofValues [ CWLInput("i") ], outputs = ListEntry.ofValues [ CWLOutput("o") ])
            let workflow = CWLWorkflowDescription(steps = ResizeArray(), inputs = ListEntry.ofValues [ CWLInput("i") ], outputs = ListEntry.ofValues [ CWLOutput("o") ])
            (CWLToolDescription.getInputValuesOrEmpty tool).Add(CWLInput("added"))
            (CWLToolDescription.getOutputValues tool).Add(CWLOutput("added"))
            (CWLExpressionToolDescription.getInputValuesOrEmpty expressionTool).Add(CWLInput("added"))
            (CWLExpressionToolDescription.getOutputValues expressionTool).Add(CWLOutput("added"))
            (CWLOperationDescription.getInputValues operation).Add(CWLInput("added"))
            (CWLOperationDescription.getOutputValues operation).Add(CWLOutput("added"))
            (CWLWorkflowDescription.getInputValues workflow).Add(CWLInput("added"))
            (CWLWorkflowDescription.getOutputValues workflow).Add(CWLOutput("added"))
            (CWLWorkflowDescription.getStepValues workflow).Add(WorkflowStep.fromRunPath("s", ResizeArray(), ResizeArray(), "t.cwl"))
            Expect.equal tool.Inputs.Value.Count 1 "The inputs of the tool are as they were."
            Expect.equal tool.Outputs.Count 1 "The outputs of the tool are as they were."
            Expect.equal expressionTool.Inputs.Value.Count 1 "The inputs of the expression tool are as they were."
            Expect.equal expressionTool.Outputs.Count 1 "The outputs of the expression tool are as they were."
            Expect.equal operation.Inputs.Count 1 "The inputs of the operation are as they were."
            Expect.equal operation.Outputs.Count 1 "The outputs of the operation are as they were."
            Expect.equal workflow.Inputs.Count 1 "The inputs of the workflow are as they were."
            Expect.equal workflow.Outputs.Count 1 "The outputs of the workflow are as they were."
            Expect.equal workflow.Steps.Count 0 "The steps of the workflow are as they were."

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
            let toolIntentValues = literals [ "classification" ]
            let workflowIntentValues = literals [ "analysis" ]
            let expressionIntentValues = literals [ "post-processing" ]
            let operationIntentValues = literals [ "orchestration" ]

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

            toolIntent.Add(SchemaSaladString.Literal "classification")
            workflowIntent.Add(SchemaSaladString.Literal "analysis")
            expressionIntent.Add(SchemaSaladString.Literal "post-processing")
            operationIntent.Add(SchemaSaladString.Literal "orchestration")

            Expect.equal tool.Intent.Value.Count 1 "Tool intent should be initialized and retained."
            Expect.equal workflow.Intent.Value.Count 1 "Workflow intent should be initialized and retained."
            Expect.equal expressionTool.Intent.Value.Count 1 "ExpressionTool intent should be initialized and retained."
            Expect.equal operation.Intent.Value.Count 1 "Operation intent should be initialized and retained."
    ]

let testInputOptional =
    let tool (inputs: CWLInput list) =
        CWLToolDescription(ResizeArray [], inputs = ListEntry.ofValues inputs, baseCommand = ResizeArray [ (SchemaSaladString.Literal "echo") ])

    testList "Optional input" [
        testCase "an optional int is written as int? and reads back equal" <| fun _ ->
            let hand = tool [ CWLInput("x", CWLType.makeOptional CWLType.Int) ]
            Expect.equal (ListEntry.values hand.Inputs.Value).[0].Type_ (Some (Union (ResizeArray [ Null; CWLType.Int ]))) "The optional int is the union of null and int."
            Expect.isTrue (ListEntry.values hand.Inputs.Value).[0].Optional "The input is optional."
            let encoded = Encode.encodeToolDescription hand
            Expect.stringContains encoded "  x: int?\n" "The optional int is the shorthand."
            Expect.equal (Decode.decodeCommandLineTool encoded) hand "The written text should read back as the same tool."
        testCase "the input decoded from the shorthand equals the input built with makeOptional" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs:\n  x: int?\noutputs: []\n"
            Expect.equal (ListEntry.values decoded.Inputs.Value).[0] (CWLInput("x", CWLType.makeOptional CWLType.Int)) "Both inputs have the union of null and int."
            Expect.equal (ListEntry.values decoded.Inputs.Value).[0] (CWLInput("x", Union (ResizeArray [ Null; CWLType.Int ]))) "A union with null is optional without the parameter."
        testCase "makeOptional adds null to a type that has none and changes no other type" <| fun _ ->
            Expect.equal (CWLType.makeOptional (Union (ResizeArray [ CWLType.String; CWLType.Int ]))) (Union (ResizeArray [ Null; CWLType.String; CWLType.Int ])) "The members of a union stay the members of one union."
            Expect.equal (CWLType.makeOptional (Union (ResizeArray [ CWLType.String; Null ]))) (Union (ResizeArray [ CWLType.String; Null ])) "A union with null stays as it is."
            Expect.equal (CWLType.makeOptional Null) Null "The type null admits null already."
            Expect.isFalse (CWLInput("e")).Optional "An input without a type is not optional."
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
            Expect.isTrue (ListEntry.values decoded.Inputs.Value).[0].Optional "A decoded union that contains null is optional."
        testCase "inputs of equal types are equal whether the type was given or came from makeOptional" <| fun _ ->
            Expect.equal (hash (CWLInput("x", CWLType.makeOptional CWLType.Int))) (hash (CWLInput("x", Union (ResizeArray [ Null; CWLType.Int ])))) "Equal inputs hash alike."
            Expect.notEqual (CWLInput("x", CWLType.Int)) (CWLInput("x", CWLType.makeOptional CWLType.Int)) "An optional int differs from an int."
    ]

let testBindingPosition =
    let tool (position: string) =
        String.concat "\n" [ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo"; "inputs:"; "  x:"; "    type: string"; "    inputBinding:"; "      position: " + position; "outputs: []"; "" ]
    let positionOf (decoded: CWLToolDescription) = (inputBindingOf (ListEntry.values decoded.Inputs.Value).[0]).Position

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
