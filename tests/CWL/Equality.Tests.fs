/// The equality and the hash of decoded and of built objects.
module Tests.Equality

open ARCtrl.CWL
open TestingUtils
open DynamicObj
open TestingUtils.CWL
open YAMLicious

/// A workflow with one step. Its step inputs hold a single source, a list of sources with every other field of a step
/// input, and a source with a default value.
let private stepInputWorkflow (sources: string) (linkMerge: string) (pickValue: string) (valueFrom: string) : string =
    Kit.workflow [
        "inputs:"
        "  a: string"
        "  b: string"
        "  c: string"
        "outputs: {}"
        "steps:"
        "  first:"
        "    run: tool.cwl"
        "    in:"
        "      single: a"
        "      list:"
        "        source: " + sources
        "        linkMerge: " + linkMerge
        "        pickValue: " + pickValue
        "        valueFrom: " + valueFrom
        "      withDefault:"
        "        source: b"
        "        default: 1"
        "    out: [done]" ]


let private stepOf (text: string) : WorkflowStep =
    match Decode.decodeCWLProcessingUnit text with
    | Workflow workflow -> (ListEntry.values workflow.Steps).[0]
    | other -> failwith (sprintf "Expected a workflow but got %A" other)


let private stepInputsOf (text: string) : StepInput list = WorkflowStep.getInputValues (stepOf text) |> Seq.toList


let testEqualityOfDecodedObjects =
    testList "Equality of decoded objects" [
        testCase "two decodes of a workflow are equal in the step and in every step input" <| fun _ ->
            let text = stepInputWorkflow "[a, b]" "merge_flattened" "first_non_null" "$(self)"
            let first = Decode.decodeCWLProcessingUnit text
            let second = Decode.decodeCWLProcessingUnit text
            Kit.expectSame "the workflow" (box first) (box second)
            let firstInputs = stepInputsOf text
            let secondInputs = stepInputsOf text
            Expect.equal firstInputs.Length 3 "The step has three inputs."
            List.iter2 (fun (a: StepInput) (b: StepInput) -> Kit.expectSame (sprintf "the step input %s" a.Id) (box a) (box b)) firstInputs secondInputs
            Expect.equal firstInputs.[0].Source (Some (OutputSource.Single (SchemaSaladString.Literal "a"))) "A string is a single source."
            Expect.equal firstInputs.[1].Source (Some (OutputSource.Multiple (literals [ "a"; "b" ]))) "A list of sources keeps its order."
            Kit.expectSame "the step" (box (stepOf text)) (box (stepOf text))

        testCase "a changed source, list order, link merge, pick value or value expression makes a step input unequal" <| fun _ ->
            let baseText = stepInputWorkflow "[a, b]" "merge_flattened" "first_non_null" "$(self)"
            let baseInput = (stepInputsOf baseText).[1]
            let variants =
                [ "another source in the list", stepInputWorkflow "[a, c]" "merge_flattened" "first_non_null" "$(self)"
                  "the sources in another order", stepInputWorkflow "[b, a]" "merge_flattened" "first_non_null" "$(self)"
                  "one more source", stepInputWorkflow "[a, b, c]" "merge_flattened" "first_non_null" "$(self)"
                  "one source less", stepInputWorkflow "[a]" "merge_flattened" "first_non_null" "$(self)"
                  "another link merge", stepInputWorkflow "[a, b]" "merge_nested" "first_non_null" "$(self)"
                  "another pick value", stepInputWorkflow "[a, b]" "merge_flattened" "all_non_null" "$(self)"
                  "another value expression", stepInputWorkflow "[a, b]" "merge_flattened" "first_non_null" "$(self.x)" ]
            for (name, text) in variants do
                Expect.notEqual (stepInputsOf text).[1] baseInput (sprintf "%s: the step inputs should differ." name)
            let single = (stepInputsOf baseText).[0]
            let otherSingle = (stepInputsOf (baseText.Replace("single: a", "single: c"))).[0]
            Expect.notEqual otherSingle single "Another single source should make the step inputs unequal."
            Expect.equal (stepInputsOf baseText).[1] baseInput "A second decode of the same text is equal."

        testCase "outputs with several sources are equal to a second decode and differ when a source changes" <| fun _ ->
            let outputs (sources: string) =
                Kit.workflow [ "inputs: {}"; "outputs:"; "  many:"; "    type: File[]"; "    outputSource: " + sources; "  one:"; "    type: File"; "    outputSource: a/x"; "steps: {}" ]
            let outputsOf (text: string) =
                match Decode.decodeCWLProcessingUnit text with
                | Workflow workflow -> (ListEntry.values workflow.Outputs) |> Seq.toList
                | other -> failwith (sprintf "Expected a workflow but got %A" other)
            let text = outputs "[a/x, b/y]"
            List.iter2 (fun (a: CWLOutput) (b: CWLOutput) -> Kit.expectSame (sprintf "the output %s" a.Name) (box a) (box b)) (outputsOf text) (outputsOf text)
            Kit.expectSame "multiple output sources" (box (OutputSource.Multiple (literals [ "a"; "b" ]))) (box (OutputSource.Multiple (literals [ "a"; "b" ])))
            Kit.expectSame "a single output source" (box (OutputSource.Single (SchemaSaladString.Literal "a"))) (box (OutputSource.Single (SchemaSaladString.Literal "a")))
            Expect.notEqual (OutputSource.Multiple (literals [ "a"; "b" ])) (OutputSource.Multiple (literals [ "b"; "a" ])) "Another order is another list of sources."
            Expect.notEqual (OutputSource.Multiple (literals [ "a"; "b" ])) (OutputSource.Multiple (literals [ "a" ])) "Another number of sources is another list."
            Expect.equal (outputsOf (outputs "[a/x]")).[0].OutputSource (Some (OutputSource.Multiple (literals [ "a/x" ]))) "A list of one source is a list."
            Expect.equal (outputsOf (outputs "a/x")).[0].OutputSource (Some (OutputSource.Single (SchemaSaladString.Literal "a/x"))) "A string is a single source."
            Expect.notEqual (outputsOf (outputs "[a/x]")).[0] (outputsOf (outputs "a/x")).[0] "A list of one source stays different from a single source."
            Expect.notEqual (outputsOf text).[0] (outputsOf (outputs "[a/x, c/z]")).[0] "A changed source makes the outputs unequal."

        testCase "lists of any element type in dynamic properties compare and hash by their elements" <| fun _ ->
            let input (value: obj) =
                let input = CWLInput("x", CWLType.String)
                DynObj.setProperty "ext" value input
                input
            let strings (items: string list) = box (ResizeArray items)
            let objects (items: obj list) = box (ResizeArray<obj> items)
            let cases =
                [ "a list of strings", strings [ "a"; "b" ], strings [ "a"; "b" ], strings [ "a"; "c" ]
                  "a list of strings in another order", strings [ "a"; "b" ], strings [ "a"; "b" ], strings [ "b"; "a" ]
                  "a shorter list of strings", strings [ "a"; "b" ], strings [ "a"; "b" ], strings [ "a" ]
                  "a list of objects", objects [ box "a"; box 1 ], objects [ box "a"; box 1 ], objects [ box "a"; box 2 ]
                  "lists in a list", objects [ strings [ "a" ]; objects [ box "b" ] ], objects [ strings [ "a" ]; objects [ box "b" ] ], objects [ strings [ "a" ]; objects [ box "c" ] ] ]
            for (name, value, same, changed) in cases do
                Kit.expectSame name (box (input value)) (box (input same))
                Expect.notEqual (input value) (input changed) (sprintf "%s: a changed element makes the objects unequal." name)
            Expect.notEqual (input (strings [ "a" ])) (input (box "a")) "A list differs from a string."

        testCase "lists in extension fields compare by their elements" <| fun _ ->
            let tool (nested: string) =
                Kit.tool [
                    "baseCommand: echo"
                    "inputs:"
                    "  x:"
                    "    type: string"
                    "    inputBinding:"
                    "      position: 1"
                    "      ext-binding: [p, q]"
                    "    ext-list: [a, b]"
                    "    ext-nested: " + nested
                    "    ext-map:"
                    "      k: [v, w]"
                    "outputs: {}"
                    "ext-top: [p, q]" ]
            let decode (text: string) = Decode.decodeCommandLineTool text
            let text = tool "[[1, 2], [3]]"
            let first = decode text
            let second = decode text
            Kit.expectSame "the tool" (box first) (box second)
            Kit.expectSame "the input" (box (ListEntry.values first.Inputs.Value).[0]) (box (ListEntry.values second.Inputs.Value).[0])
            Kit.expectSame "the input binding" (box (ListEntry.values first.Inputs.Value).[0].InputBinding.Value) (box (ListEntry.values second.Inputs.Value).[0].InputBinding.Value)
            let changed = decode (tool "[[1, 2], [4]]")
            Expect.notEqual (ListEntry.values changed.Inputs.Value).[0] (ListEntry.values first.Inputs.Value).[0] "A changed element in a nested list makes the inputs unequal."
            Expect.notEqual (ListEntry.values (decode (tool "[[1, 2]]")).Inputs.Value).[0] (ListEntry.values first.Inputs.Value).[0] "A shorter nested list makes the inputs unequal."
            Expect.notEqual (ListEntry.values (decode (text.Replace("ext-list: [a, b]", "ext-list: [b, a]"))).Inputs.Value).[0] (ListEntry.values first.Inputs.Value).[0] "Another order of the elements makes the inputs unequal."
    ]


let testEqualityAndHashOfPorts =
    let inputOf (text: string) = (ListEntry.values (Decode.decodeCommandLineTool text).Inputs.Value).[0]
    let outputOf (text: string) = (ListEntry.values (Decode.decodeCommandLineTool text).Outputs).[0]
    let workflowOutputOf (text: string) = (ListEntry.values (Decode.decodeWorkflow text).Outputs).[0]
    let stepInputOf (text: string) = (WorkflowStep.getInputValues (ListEntry.values (Decode.decodeWorkflow text).Steps).[0]).[0]
    let inputVariants =
        [ "name", "  x:\n", "  y:\n"
          "type", "type: string", "type: int"
          "imported type", "type: string", "type: {$import: t.yml}"
          "binding prefix", "prefix: -a", "prefix: -b"
          "binding position", "position: 1", "position: $(inputs.n)"
          "label", "label: lab", "label: other"
          "secondaryFiles", "secondaryFiles: .idx", "secondaryFiles: .bai"
          "streamable", "streamable: true", "streamable: false"
          "doc", "doc: first", "doc: second"
          "format", "format: edam:format_1", "format: edam:format_2"
          "loadContents", "loadContents: false", "loadContents: true"
          "loadListing", "loadListing: no_listing", "loadListing: deep_listing"
          "default", "default: dflt", "default: other"
          "extension field", "arc:note: n1", "arc:note: n2" ]
    let outputVariants =
        [ "name", "  o:\n", "  p:\n"
          "type", "type: File", "type: Directory"
          "imported type", "type: File", "type: {$import: t.yml}"
          "glob", "glob: \"*.txt\"", "glob: \"*.csv\""
          "label", "label: lab", "label: other"
          "secondaryFiles", "secondaryFiles: .idx", "secondaryFiles: .bai"
          "streamable", "streamable: true", "streamable: false"
          "doc", "doc: first", "doc: second"
          "format", "format: edam:format_1", "format: edam:format_2"
          "extension field", "arc:note: n1", "arc:note: n2" ]
    let stepInputVariants =
        [ "id", "      x:\n", "      y:\n"
          "source", "source: [a, b]", "source: [a, c]"
          "default", "default: dflt", "default: other"
          "valueFrom", "valueFrom: $(self)", "valueFrom: $(self.x)"
          "linkMerge", "linkMerge: merge_flattened", "linkMerge: merge_nested"
          "pickValue", "pickValue: first_non_null", "pickValue: all_non_null"
          "doc", "doc: first", "doc: second"
          "loadContents", "loadContents: false", "loadContents: true"
          "loadListing", "loadListing: no_listing", "loadListing: deep_listing"
          "label", "label: lab", "label: other"
          "extension field", "arc:note: n1", "arc:note: n2" ]
    testList "Equality and hash of ports and files" [
        testCase "an input differs in equality and hash in each of its fields" <| fun _ ->
            Kit.variants "input" TestObjects.CWL.Inputs.AllFields.toolFile inputOf inputVariants
        testCase "an output of a tool differs in equality and hash in each of its fields" <| fun _ ->
            Kit.variants "tool output" TestObjects.CWL.Outputs.AllFields.toolFile outputOf outputVariants
        testCase "the output source of a workflow output and each field of a step input differ in equality and hash" <| fun _ ->
            Kit.variants "workflow output" TestObjects.CWL.Outputs.AllFields.workflowFile workflowOutputOf [ "output source", "outputSource: step/out", "outputSource: step/other" ]
            Kit.variants "step input" TestObjects.CWL.WorkflowSteps.AllFields.workflowFile stepInputOf stepInputVariants
        testCase "a file differs in equality and hash in each of its fields" <| fun _ ->
            let file () =
                let file = FileInstance(location = SchemaSaladString.Literal "l", path = SchemaSaladString.Literal "p", basename = SchemaSaladString.Literal "b", dirname = SchemaSaladString.Literal "d", nameroot = SchemaSaladString.Literal "r", nameext = SchemaSaladString.Literal ".e", checksum = SchemaSaladString.Literal "c", size = FileSize.Bytes 1L, secondaryFiles = { Node = YAMLicious.Reader.readRoot "s" }, format = SchemaSaladString.Literal "f", contents = SchemaSaladString.Literal "t")
                DynObj.setProperty "arc:note" (box "n1") file
                file
            let edits : (string * (FileInstance -> unit)) list =
                [ "location", fun f -> f.Location <- Some (SchemaSaladString.Literal "l2")
                  "path", fun f -> f.Path <- Some (SchemaSaladString.Literal "p2")
                  "basename", fun f -> f.Basename <- Some (SchemaSaladString.Literal "b2")
                  "basename as a directive", fun f -> f.Basename <- Some (SchemaSaladString.Import "b")
                  "dirname", fun f -> f.Dirname <- Some (SchemaSaladString.Literal "d2")
                  "nameroot", fun f -> f.Nameroot <- Some (SchemaSaladString.Literal "r2")
                  "nameext", fun f -> f.Nameext <- Some (SchemaSaladString.Literal ".e2")
                  "checksum", fun f -> f.Checksum <- Some (SchemaSaladString.Literal "c2")
                  "size", fun f -> f.Size <- Some (FileSize.Bytes 2L)
                  "secondaryFiles", fun f -> f.SecondaryFiles <- Some { Node = YAMLicious.Reader.readRoot "s2" }
                  "format", fun f -> f.Format <- Some (SchemaSaladString.Literal "f2")
                  "contents", fun f -> f.Contents <- Some (SchemaSaladString.Literal "t2")
                  "extension field", fun f -> DynObj.setProperty "arc:note" (box "n2") f ]
            Kit.expectSame "a file" (box (file ())) (box (file ()))
            for (what, edit) in edits do
                let changed = file ()
                edit changed
                Expect.notEqual (box changed) (box (file ())) (sprintf "Another %s should make the files unequal." what)
                Expect.notEqual ((box changed).GetHashCode()) ((box (file ())).GetHashCode()) (sprintf "Another %s should change the hash." what)
        testCase "the entries of a list differ in equality and hash when the value or the path differs" <| fun _ ->
            Expect.notEqual ((box (CWLInput("x", CWLType.String))).GetHashCode()) ((box (CWLInput("y", CWLType.Int))).GetHashCode()) "Two small inputs hash differently."
            Expect.notEqual ((box (ListEntry.Value (CWLInput("x", CWLType.String)))).GetHashCode()) ((box (ListEntry.Value (CWLInput("y", CWLType.Int)))).GetHashCode()) "The entries of two inputs hash differently."
            Expect.notEqual ((box (ListEntry.Value (CWLOutput("o", CWLType.File)))).GetHashCode()) ((box (ListEntry.Value (CWLOutput("p", CWLType.File)))).GetHashCode()) "The entries of two outputs hash differently."
            Expect.notEqual ((box (ListEntry.Imported "a.yml" : ListEntry<CWLInput>)).GetHashCode()) ((box (ListEntry.Imported "b.yml" : ListEntry<CWLInput>)).GetHashCode()) "Imports of two files hash differently."
            Expect.notEqual (ListEntry.Value (CWLInput("x", CWLType.String))) (ListEntry.Value (CWLInput("x", CWLType.Int))) "The entries of two types differ."
            Expect.equal ((box (ListEntry.Value (CWLInput("x", CWLType.String)))).GetHashCode()) ((box (ListEntry.Value (CWLInput("x", CWLType.String)))).GetHashCode()) "Equal entries hash alike."
    ]


let testEqualityAndHashOfProcesses =
    let tool (inputLines: string list) (outputLines: string list) = Kit.tool ("baseCommand: echo" :: inputLines @ outputLines)
    let process' (cls: string) (extra: string list) (inputLines: string list) (outputLines: string list) =
        Kit.document cls (extra @ inputLines @ outputLines)
    let inputs = [ "inputs:"; "  x:"; "    type: string"; "    doc: in-doc"; "    arc:note: in-note" ]
    let outputs = [ "outputs:"; "  o:"; "    type: string"; "    doc: out-doc"; "    arc:note: out-note" ]
    let variants =
        [ "input name", "  x:\n", "  y:\n"
          "input type", "    type: string\n    doc: in-doc", "    type: int\n    doc: in-doc"
          "imported input type", "    type: string\n    doc: in-doc", "    type: {$import: t.yml}\n    doc: in-doc"
          "input doc", "doc: in-doc", "doc: other"
          "input extension field", "in-note", "other"
          "output name", "  o:\n", "  p:\n"
          "output type", "    type: string\n    doc: out-doc", "    type: int\n    doc: out-doc"
          "output doc", "doc: out-doc", "doc: other"
          "output extension field", "out-note", "other" ]
    let kinds =
        [ "tool", Decode.decodeCommandLineTool >> box, tool inputs outputs
          "expression tool", Decode.decodeExpressionTool >> box, process' "ExpressionTool" [ "expression: $(null)" ] inputs outputs
          "operation", Decode.decodeOperation >> box, process' "Operation" [] inputs outputs
          "workflow", Decode.decodeWorkflow >> box, process' "Workflow" [ "steps: {}" ] inputs outputs ]
    testList "Equality and hash of processes" [
        testCase "processes that differ in one field of an input or an output are unequal and hash differently" <| fun _ ->
            for (name, decode, text) in kinds do
                Kit.variants name text decode variants
        testCase "a tool that differs in the binding of an input is unequal and hashes differently" <| fun _ ->
            let binding (prefix: string) (position: string) =
                tool [ "inputs:"; "  x:"; "    type: string"; "    inputBinding:"; "      prefix: " + prefix; "      position: " + position ] [ "outputs: {}" ]
            let text = binding "-a" "1"
            Kit.variants "tool" text Decode.decodeCommandLineTool [ "binding prefix", "prefix: -a", "prefix: -b"; "binding position", "position: 1", "position: $(inputs.n)" ]
        testCase "a tool that differs in the path of an imported type or of an imported input is unequal and hashes differently" <| fun _ ->
            let imported = tool [ "inputs:"; "  x:"; "    type: {$import: a.yml}" ] [ "outputs: {}" ]
            Kit.variants "tool" imported Decode.decodeCommandLineTool [ "path of the imported type", "a.yml", "b.yml" ]
            let importedInput = tool [ "inputs:"; "  - {$import: ports-a.yml}" ] [ "outputs: {}" ]
            Kit.variants "tool" importedInput Decode.decodeCommandLineTool [ "path of the imported inputs", "ports-a.yml", "ports-b.yml" ]
    ]


let main =
    testList "Equality" [
        testEqualityOfDecodedObjects
        testEqualityAndHashOfPorts
        testEqualityAndHashOfProcesses
    ]
