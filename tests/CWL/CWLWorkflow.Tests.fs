module Tests.CWLWorkflow

open ARCtrl.CWL
open DynamicObj
open TestingUtils
open TestingUtils.CWL
open YAMLicious

let decodeCWLWorkflowDescription: CWLWorkflowDescription =
    TestObjects.CWL.Workflow.workflowFile
    |> Decode.decodeWorkflow

let tryYamlScalarString (y: RawYaml) =
    match y.Node.Kind with
    | YamlNodeKind.Scalar scalar -> Some scalar.Value
    | _ -> None

let tryYamlScalarSequence (y: RawYaml) =
    match y.Node.Kind with
    | YamlNodeKind.Sequence collection ->
        collection.Items
        |> List.choose (fun item ->
            match item.Node.Kind with
            | YamlNodeKind.Scalar scalar -> Some scalar.Value
            | _ -> None)
        |> ResizeArray
        |> Some
    | _ -> None

let renderYamlElement (element: YamlNode) =
    Encode.writeYaml element

let renderYamlPair pair =
    YAMLicious.Encode.object [ pair ]
    |> Encode.writeYaml

let mkStepInput id (source: ResizeArray<string> option) linkMerge =
    StepInput.create(id, ?source = (source |> Option.map (fun texts -> literals texts)), ?linkMerge = linkMerge)

/// The error that a decode raises, and none when it succeeds.
let decodeError (decode: unit -> unit) =
    try
        decode ()
        None
    with YamlDecodeException error -> Some error

let testImportedRun =
    let workflow (runLines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: Workflow"; "inputs: {}"; "outputs: {}"; "steps:"; "  s:" ] @ (runLines |> List.map (fun line -> "    " + line)) @ [ "    in: {}"; "    out: []"; "" ])
    testList "Imported run" [
        testCase "the run of a step may be an import, which is not the run of the same path" <| fun _ ->
            let imported = Decode.decodeWorkflow (workflow [ "run: {$import: tool.cwl}" ])
            Expect.equal (ListEntry.values imported.Steps).[0].Run (RunImport "tool.cwl") "The run is the import."
            let plain = Decode.decodeWorkflow (workflow [ "run: tool.cwl" ])
            Expect.equal (ListEntry.values plain.Steps).[0].Run (RunString "tool.cwl") "The run is the path."
            Expect.notEqual (ListEntry.values imported.Steps).[0].Run (ListEntry.values plain.Steps).[0].Run "The two spellings differ."
            Expect.notEqual imported plain "The workflows differ."
        testCase "an imported run is written as the directive and reads back equal" <| fun _ ->
            let imported = Decode.decodeWorkflow (workflow [ "run: {$import: tools/tool.cwl}" ])
            let written = Encode.encodeWorkflowDescription imported
            Expect.stringContains written "run: {$import: tools/tool.cwl}" "The run is a flow mapping."
            Expect.equal (Decode.decodeWorkflow written) imported "The written text should decode to the same workflow."
        testCase "an include or a directive with another field cannot stand for a run" <| fun _ ->
            for (name, runLines, message) in
                [ "an include", [ "run: {$include: tool.cwl}" ], "An $include yields the text of a file, but the run field of a step takes an object."
                  "an import with another field", [ "run: {$import: tool.cwl, extra: 1}" ], "A directive has exactly one field" ] do
                match decodeError (fun () -> Decode.decodeWorkflow (workflow runLines) |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) "steps.s.run" (sprintf "%s: the error should show the run" name)
                    Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
                    Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
                | None -> failwith (sprintf "%s must fail" name)
    ]

let testImportedEntries =
    let workflow (inputs: string list) (outputs: string list) (steps: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: Workflow" ] @ inputs @ outputs @ steps @ [ "" ])
    let mixed =
        workflow
            [ "inputs:"; "  - {$import: inputs.yml}"; "  - id: a"; "    type: string" ]
            [ "outputs:"; "  - id: o"; "    type: string"; "    outputSource: s/out"; "  - {$import: outputs.yml}" ]
            [ "steps:"; "  - {$import: steps.yml}"; "  - id: s"; "    run: t.cwl"; "    in: {}"; "    out: [out]"; "  - {$import: more-steps.yml}" ]
    let tool (cls: string) (extra: string list) (inputs: string list) (outputs: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: " + cls ] @ extra @ inputs @ outputs @ [ "" ])
    testList "Imported entries" [
        testCase "an import stands in its place among the inputs, outputs and steps, is written back there and reads back equal" <| fun _ ->
            let decoded = Decode.decodeWorkflow mixed
            Expect.equal (List.ofSeq decoded.Inputs |> List.map (ListEntry.map (fun input -> input.Name))) [ ListEntry.Imported "inputs.yml"; ListEntry.Value "a" ] "The import comes first, then the input."
            Expect.equal (List.ofSeq decoded.Outputs |> List.map (ListEntry.map (fun output -> output.Name))) [ ListEntry.Value "o"; ListEntry.Imported "outputs.yml" ] "The output comes first, then the import."
            Expect.equal (List.ofSeq decoded.Steps |> List.map (ListEntry.map (fun step -> step.Id))) [ ListEntry.Imported "steps.yml"; ListEntry.Value "s"; ListEntry.Imported "more-steps.yml" ] "The imports stand around the step."
            let written = Encode.encodeWorkflowDescription decoded
            for position in [ "- {$import: inputs.yml}"; "- {$import: outputs.yml}"; "- {$import: steps.yml}"; "- {$import: more-steps.yml}" ] do
                Expect.stringContains written position (sprintf "%s is written as an entry.\n%s" position written)
            Expect.isTrue (written.IndexOf "inputs.yml" < written.IndexOf "id: a") "The import of the inputs stays in front of the input."
            Expect.isTrue (written.IndexOf "id: s" < written.IndexOf "more-steps.yml") "The import of more steps stays behind the step."
            let reread = Decode.decodeWorkflow written
            Expect.equal reread decoded "The written text should decode to the same workflow."
            Expect.equal (Encode.encodeWorkflowDescription reread) written "Writing again should give the same text."
        testCase "the getters give the objects of the value entries and leave the imports out" <| fun _ ->
            let decoded = Decode.decodeWorkflow mixed
            Expect.equal (CWLWorkflowDescription.getInputValues decoded |> Seq.map (fun input -> input.Name) |> List.ofSeq) [ "a" ] "Only the input is an object."
            Expect.equal (CWLWorkflowDescription.getOutputValues decoded |> Seq.map (fun output -> output.Name) |> List.ofSeq) [ "o" ] "Only the output is an object."
            Expect.equal (CWLWorkflowDescription.getStepValues decoded |> Seq.map (fun step -> step.Id) |> List.ofSeq) [ "s" ] "Only the step is an object."
            let inputs = CWLWorkflowDescription.getInputValues decoded
            inputs.Clear()
            Expect.equal decoded.Inputs.Count 2 "The list is a new one, so clearing it leaves the workflow as it was."
        testCase "a field that is one directive keeps that spelling, and the list of that one import is equal" <| fun _ ->
            let whole = workflow [ "inputs: {$import: p.yml}" ] [ "outputs: {$import: o.yml}" ] [ "steps: {$import: s.yml}" ]
            let listed = workflow [ "inputs: [{$import: p.yml}]" ] [ "outputs:"; "  - {$import: o.yml}" ] [ "steps: [{$import: s.yml}]" ]
            let decodedWhole = Decode.decodeWorkflow whole
            let decodedListed = Decode.decodeWorkflow listed
            Expect.equal (List.ofSeq decodedWhole.Inputs) [ ListEntry.Imported "p.yml" ] "The inputs are the import."
            Expect.equal (List.ofSeq decodedWhole.Outputs) [ ListEntry.Imported "o.yml" ] "The outputs are the import."
            Expect.equal (List.ofSeq decodedWhole.Steps) [ ListEntry.Imported "s.yml" ] "The steps are the import."
            Expect.equal decodedWhole decodedListed "The spelling is layout, so the workflows are equal."
            let writtenWhole = Encode.encodeWorkflowDescription decodedWhole
            for field in [ "inputs: {$import: p.yml}"; "outputs: {$import: o.yml}"; "steps: {$import: s.yml}" ] do
                Expect.stringContains writtenWhole field (sprintf "%s keeps its spelling.\n%s" field writtenWhole)
            Expect.equal (Decode.decodeWorkflow writtenWhole) decodedWhole "The written text should decode to the same workflow."
            let writtenListed = Encode.encodeWorkflowDescription decodedListed
            Expect.isFalse (writtenListed.Contains "inputs: {$import") "The listed spelling is written as a list."
            Expect.stringContains writtenListed "- {$import: p.yml}" "The listed import is an entry."
        testCase "tools, expression tools and operations take imports among their inputs and outputs" <| fun _ ->
            let inputs = [ "inputs:"; "  - {$import: ports.yml}"; "  - id: a"; "    type: string" ]
            let outputs = [ "outputs:"; "  - id: o"; "    type: File"; "  - {$import: out.yml}" ]
            let wholeInputs = [ "inputs: {$import: ports.yml}" ]
            let wholeOutputs = [ "outputs: {$import: out.yml}" ]
            let check (name: string) (decode: string -> 'T) (encode: 'T -> string) (text: string) (written: string list) =
                let decoded = decode text
                let encoded = encode decoded
                for expected in written do
                    Expect.stringContains encoded expected (sprintf "%s: %s is written.\n%s" name expected encoded)
                Expect.equal (decode encoded) decoded (sprintf "%s: the written text should decode to the same process." name)
                Expect.equal (encode (decode encoded)) encoded (sprintf "%s: writing again should give the same text." name)
            let both = [ "- {$import: ports.yml}"; "- {$import: out.yml}" ]
            let whole = [ "inputs: {$import: ports.yml}"; "outputs: {$import: out.yml}" ]
            check "tool" Decode.decodeCommandLineTool Encode.encodeToolDescription (tool "CommandLineTool" [ "baseCommand: echo" ] inputs outputs) both
            check "tool, whole fields" Decode.decodeCommandLineTool Encode.encodeToolDescription (tool "CommandLineTool" [ "baseCommand: echo" ] wholeInputs wholeOutputs) whole
            check "expression tool" Decode.decodeExpressionTool Encode.encodeExpressionToolDescription (tool "ExpressionTool" [ "expression: $(null)" ] inputs outputs) both
            check "expression tool, whole fields" Decode.decodeExpressionTool Encode.encodeExpressionToolDescription (tool "ExpressionTool" [ "expression: $(null)" ] wholeInputs wholeOutputs) whole
            check "operation" Decode.decodeOperation Encode.encodeOperationDescription (tool "Operation" [] inputs outputs) both
            check "operation, whole fields" Decode.decodeOperation Encode.encodeOperationDescription (tool "Operation" [] wholeInputs wholeOutputs) whole
            let decoded = Decode.decodeCommandLineTool (tool "CommandLineTool" [ "baseCommand: echo" ] inputs outputs)
            Expect.equal (CWLToolDescription.getInputValuesOrEmpty decoded |> Seq.map (fun input -> input.Name) |> List.ofSeq) [ "a" ] "The tool inputs without the import."
            Expect.equal (CWLToolDescription.getOutputValues decoded |> Seq.map (fun output -> output.Name) |> List.ofSeq) [ "o" ] "The tool outputs without the import."
        testCase "an import among the outputs of an inline process leaves the out field of its step unchecked" <| fun _ ->
            let inlineStep (outputLines: string list) =
                workflow
                    [ "inputs: {}" ]
                    [ "outputs: {}" ]
                    ([ "steps:"; "  s:"; "    run:"; "      class: CommandLineTool"; "      baseCommand: echo"; "      inputs: {}" ] @ (outputLines |> List.map (fun line -> "      " + line)) @ [ "    in: {}"; "    out: [r]" ])
            let withImport = Decode.decodeWorkflow (inlineStep [ "outputs:"; "  - {$import: out.yml}" ])
            Expect.equal (ListEntry.values withImport.Steps).[0].Out.Count 1 "The step lists the output that the file may hold."
            match decodeError (fun () -> Decode.decodeWorkflow (inlineStep [ "outputs:"; "  s: string" ]) |> ignore) with
            | Some error -> Expect.stringContains error.Message "has no output of that name" "Without the import the output is unknown."
            | None -> failwith "An output that the process lacks must fail."
        testCase "a directive cannot stand for the value of a map entry of the inputs, outputs or steps" <| fun _ ->
            let cases =
                [ "an input", workflow [ "inputs:"; "  a: {$import: port.yml}" ] [ "outputs: {}" ] [ "steps: {}" ], "inputs.a", "an entry of the inputs"
                  "an output", workflow [ "inputs: {}" ] [ "outputs:"; "  o: {$import: port.yml}" ] [ "steps: {}" ], "outputs.o", "an entry of the outputs"
                  "a step", workflow [ "inputs: {}" ] [ "outputs: {}" ] [ "steps:"; "  s: {$import: step.yml}" ], "steps.s", "an entry of the steps" ]
            for (name, text, path, what) in cases do
                match decodeError (fun () -> Decode.decodeWorkflow text |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the entry" name)
                    Expect.stringContains error.Message ("A directive cannot stand for the value of " + what) (sprintf "%s: the error should say what is wrong" name)
                    Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
                | None -> failwith (sprintf "%s must fail" name)
        testCase "an include or a directive with another field cannot stand for an entry" <| fun _ ->
            let cases =
                [ "an include among the inputs", workflow [ "inputs:"; "  - {$include: p.yml}" ] [ "outputs: {}" ] [ "steps: {}" ], "inputs[0]", "An $include yields the text of a file, but an entry of the inputs takes an object."
                  "an include among the outputs", workflow [ "inputs: {}" ] [ "outputs:"; "  - {$include: p.yml}" ] [ "steps: {}" ], "outputs[0]", "An $include yields the text of a file, but an entry of the outputs takes an object."
                  "an include among the steps", workflow [ "inputs: {}" ] [ "outputs: {}" ] [ "steps:"; "  - {$include: p.yml}" ], "steps[0]", "An $include yields the text of a file, but an entry of the steps takes an object."
                  "an import with another field among the inputs", workflow [ "inputs:"; "  - {$import: p.yml, id: a}" ] [ "outputs: {}" ] [ "steps: {}" ], "inputs[0]", "A directive has exactly one field"
                  "an include as the whole outputs", workflow [ "inputs: {}" ] [ "outputs: {$include: o.yml}" ] [ "steps: {}" ], "outputs", "An $include yields the text of a file, but the outputs field takes an object." ]
            for (name, text, path, message) in cases do
                match decodeError (fun () -> Decode.decodeWorkflow text |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the entry" name)
                    Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
                    Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
                | None -> failwith (sprintf "%s must fail" name)
        testCase "the processing unit gives the objects of the value entries" <| fun _ ->
            let unit = CWLProcessingUnit.Workflow (Decode.decodeWorkflow mixed)
            Expect.equal (CWLProcessingUnit.getInputValues unit |> Seq.map (fun input -> input.Name) |> List.ofSeq) [ "a" ] "The processing unit reads the input only."
            Expect.equal (CWLProcessingUnit.getOutputValues unit |> Seq.map (fun output -> output.Name) |> List.ofSeq) [ "o" ] "The processing unit reads the output only."
    ]

let testImportedStepPorts =
    let workflow (stepLines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: Workflow"; "inputs: {}"; "outputs: {}"; "steps:"; "  s:" ] @ (stepLines |> List.map (fun line -> "    " + line)) @ [ "" ])
    let referenced (portLines: string list) = workflow ([ "run: t.cwl" ] @ portLines)
    let stepOf (text: string) = (ListEntry.values (Decode.decodeWorkflow text).Steps).[0]
    let mixed =
        referenced [ "in:"; "  - {$import: more-in.yml}"; "  - id: a"; "    source: x"; "out:"; "  - r"; "  - {$import: more-out.yml}"; "  - {$include: id.txt}" ]
    let failsAt (name: string) (text: string) (path: string) (message: string) =
        match decodeError (fun () -> Decode.decodeWorkflow text |> ignore) with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the entry" name)
            Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        | None -> failwith (sprintf "%s must fail" name)
    testList "Imported step inputs and outputs" [
        testCase "an import stands in its place among the step inputs and outputs, is written back there and reads back equal" <| fun _ ->
            let decoded = Decode.decodeWorkflow mixed
            let step = (ListEntry.values decoded.Steps).[0]
            Expect.equal (List.ofSeq step.In |> List.map (ListEntry.map (fun input -> input.Id))) [ ListEntry.Imported "more-in.yml"; ListEntry.Value "a" ] "The import comes first, then the step input."
            Expect.equal (List.ofSeq step.Out) [ StepOutputString "r"; StepOutputImport "more-out.yml"; StepOutputInclude "id.txt" ] "The outputs keep their order and their kinds."
            let written = Encode.encodeWorkflowDescription decoded
            for position in [ "- {$import: more-in.yml}"; "- {$import: more-out.yml}"; "- {$include: id.txt}" ] do
                Expect.stringContains written position (sprintf "%s is written as an entry.\n%s" position written)
            Expect.isTrue (written.IndexOf "more-in.yml" < written.IndexOf "id: a") "The import stays in front of the step input."
            let reread = Decode.decodeWorkflow written
            Expect.equal reread decoded "The written text should decode to the same workflow."
            Expect.equal (Encode.encodeWorkflowDescription reread) written "Writing again should give the same text."
        testCase "the getters give the step inputs and outputs that the document writes out and leave the directives out" <| fun _ ->
            let step = stepOf mixed
            Expect.equal (WorkflowStep.getInputValues step |> Seq.map (fun input -> input.Id) |> List.ofSeq) [ "a" ] "Only the step input is an object."
            Expect.equal (List.ofSeq (WorkflowStep.getOutputValues step)) [ StepOutputString "r" ] "Only the output r is written out."
            Expect.equal (step.Out |> Seq.map StepOutput.tryId |> List.ofSeq) [ Some "r"; None; None ] "A directive has no id."
            let inputs = WorkflowStep.getInputValues step
            inputs.Clear()
            Expect.equal step.In.Count 2 "The list is a new one, so clearing it leaves the step as it was."
        testCase "a field that is one directive keeps that spelling, and the list of that one import is equal" <| fun _ ->
            let whole = referenced [ "in: {$import: in.yml}"; "out: {$import: out.yml}" ]
            let listed = referenced [ "in: [{$import: in.yml}]"; "out:"; "  - {$import: out.yml}" ]
            let decodedWhole = Decode.decodeWorkflow whole
            let decodedListed = Decode.decodeWorkflow listed
            Expect.equal (List.ofSeq (stepOf whole).In) [ ListEntry.Imported "in.yml" ] "The step inputs are the import, and not an input with the id $import."
            Expect.equal (List.ofSeq (stepOf whole).Out) [ StepOutputImport "out.yml" ] "The outputs are the import."
            Expect.equal decodedWhole decodedListed "The spelling is layout, so the workflows are equal."
            let writtenWhole = Encode.encodeWorkflowDescription decodedWhole
            for field in [ "in: {$import: in.yml}"; "out: {$import: out.yml}" ] do
                Expect.stringContains writtenWhole field (sprintf "%s keeps its spelling.\n%s" field writtenWhole)
            Expect.equal (Decode.decodeWorkflow writtenWhole) decodedWhole "The written text should decode to the same workflow."
            let writtenListed = Encode.encodeWorkflowDescription decodedListed
            Expect.isFalse (writtenListed.Contains "in: {$import") "The listed spelling is written as a list."
            Expect.stringContains writtenListed "- {$import: in.yml}" "The listed import is an entry."
        testCase "a list with an import and a repeated id is written as a list and reads back" <| fun _ ->
            let text = referenced [ "in:"; "  - {id: a, source: x}"; "  - {$import: more.yml}"; "  - {id: a, source: y}"; "out: []" ]
            let decoded = Decode.decodeWorkflow text
            Expect.equal (Decode.decodeWorkflow (Encode.encodeWorkflowDescription decoded)) decoded "The written text should decode to the same workflow."
        testCase "a directive cannot stand for the value of a map entry of the step inputs" <| fun _ ->
            for (name, line) in [ "an import", "in: {a: {$import: p.yml}}"; "an include", "in: {a: {$include: p.yml}}" ] do
                failsAt name (referenced [ line; "out: []" ]) "steps.s.in.a" "A directive cannot stand for the value of an entry of the in field"
        testCase "an include or a directive with another field cannot stand for a step input" <| fun _ ->
            let cases =
                [ "an include among the step inputs", referenced [ "in: [{$include: p.yml}]"; "out: []" ], "steps.s.in[0]", "An $include yields the text of a file, but an entry of the in field takes an object."
                  "an import with another field among the step inputs", referenced [ "in: [{$import: p.yml, id: a}]"; "out: []" ], "steps.s.in[0]", "A directive has exactly one field"
                  "an include as the whole in field", referenced [ "in: {$include: p.yml}"; "out: []" ], "steps.s.in", "An $include yields the text of a file, but the in field of a step takes an object."
                  "an include as the whole out field", referenced [ "in: {}"; "out: {$include: p.yml}" ], "steps.s.out", "An $include yields the text of a file, but the out field of a step takes an object."
                  "an import with another field among the outputs", referenced [ "in: {}"; "out: [{$import: p.yml, id: r}]" ], "steps.s.out[0]", "A directive has exactly one field" ]
            for (name, text, path, message) in cases do
                failsAt name text path message
        testCase "the out field is a list, so a map of outputs is still an error" <| fun _ ->
            failsAt "a map of outputs" (referenced [ "in: {}"; "out: {r: {$import: p.yml}}" ]) "steps.s.out" "Expected a list of outputs, found a mapping."
        testCase "the out field of a step with an inline process is checked for the entries that name an output" <| fun _ ->
            let inlineStep (outLine: string) =
                workflow [ "run:"; "  class: CommandLineTool"; "  baseCommand: echo"; "  inputs: {}"; "  outputs:"; "    r: stdout"; "in: {}"; outLine ]
            Expect.equal (stepOf (inlineStep "out: [r, {$import: more.yml}, {$include: id.txt}]")).Out.Count 3 "The directives stand for outputs of files."
            failsAt "a name beside an import" (inlineStep "out: [other, {$import: more.yml}]") "steps.s.out[0]" "has no output of that name"
    ]

let testStringLists =
    let workflowOf (stepLines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: Workflow"; "requirements:"; "  ScatterFeatureRequirement: {}"; "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    run: t.cwl"; "    out: []" ] @ (stepLines |> List.map (fun line -> "    " + line)) @ [ "" ])
    let outputsOf (outputLine: string) =
        String.concat "\n" [ "cwlVersion: v1.2"; "class: Workflow"; "inputs: {}"; "outputs:"; "  o:"; "    type: string"; "    outputSource: " + outputLine; "steps: {}"; "" ]
    let document (header: string list) (intentLine: string) =
        String.concat "\n" ([ "cwlVersion: v1.2" ] @ header @ [ intentLine; "" ])
    let processes =
        [ "a tool", [ "class: CommandLineTool"; "baseCommand: echo"; "inputs: {}"; "outputs: {}" ]
          "a workflow", [ "class: Workflow"; "inputs: {}"; "outputs: {}"; "steps: {}" ]
          "an expression tool", [ "class: ExpressionTool"; "inputs: {}"; "outputs: {}"; "expression: $(1)" ]
          "an operation", [ "class: Operation"; "inputs: {}"; "outputs: {}" ] ]
    let stepOf (text: string) = (ListEntry.values (Decode.decodeWorkflow text).Steps).[0]
    let intentOf (text: string) =
        match Decode.decodeCWLDocument text with
        | CWLDocument.Process (CommandLineTool tool) -> tool.Intent
        | CWLDocument.Process (Workflow workflow) -> workflow.Intent
        | CWLDocument.Process (ExpressionTool tool) -> tool.Intent
        | CWLDocument.Process (Operation operation) -> operation.Intent
        | other -> failwith (sprintf "Expected a process but got %A" other)
    let rewritten (text: string) =
        match Decode.decodeCWLDocument text with
        | CWLDocument.Process processingUnit -> Encode.encodeCWLDocument (CWLDocument.Process processingUnit)
        | other -> failwith (sprintf "Expected a process but got %A" other)
    // A field that holds a string or a list of strings: the document with {0} where the value stands, and how to read the entries.
    let fields : (string * string * (string -> ResizeArray<SchemaSaladString> option)) list =
        [ "the scatter", workflowOf [ "in: {}"; "scatter: {0}" ], (fun text -> (stepOf text).Scatter)
          "the source of a step input", workflowOf [ "in:"; "  a:"; "    source: {0}" ], (fun text -> (WorkflowStep.getInputValues (stepOf text)).[0].Source)
          "the source of a step input in a list", workflowOf [ "in:"; "  - id: a"; "    source: {0}" ], (fun text -> (WorkflowStep.getInputValues (stepOf text)).[0].Source)
          "the outputSource", (outputsOf "{0}"), (fun text -> match (ListEntry.values (Decode.decodeWorkflow text).Outputs).[0].OutputSource with Some source -> Some (source.AsValues()) | None -> None) ]
    testList "Directives in the lists of strings" [
        testCase "a directive stands for the string or the list of a file in the scatter, the source and the outputSource, whole or in a list" <| fun _ ->
            for (name, template, read) in fields do
                for (spelling, expected) in
                    [ "[{$import: p.yml}]", SchemaSaladString.Import "p.yml"
                      "{$import: p.yml}", SchemaSaladString.Import "p.yml"
                      "[{$include: p.txt}]", SchemaSaladString.Include "p.txt"
                      "{$include: p.txt}", SchemaSaladString.Include "p.txt"
                      "[{$mixin: p.yml}]", SchemaSaladString.Mixin "p.yml"
                      "{$mixin: p.yml}", SchemaSaladString.Mixin "p.yml" ] do
                    let text = template.Replace("{0}", spelling)
                    Expect.equal (read text |> Option.map List.ofSeq) (Some [ expected ]) (sprintf "%s %s: the entry is the directive." name spelling)
                    let written = rewritten text
                    Expect.stringContains written (spelling.Trim('[', ']')) (sprintf "%s %s: the directive is written as a flow mapping.\n%s" name spelling written)
                    Expect.equal (read written |> Option.map List.ofSeq) (Some [ expected ]) (sprintf "%s %s: the written directive is the entry." name spelling)
                    Expect.equal (rewritten written) written (sprintf "%s %s: writing again should give the same text." name spelling)
        testCase "a directive stands among the strings of a list and a mixin beside a field is kept as it was written" <| fun _ ->
            for (name, template, read) in fields do
                let text = template.Replace("{0}", "[a, {$import: b.yml}, {$mixin: c.yml, extra: 1}]")
                let entries = read text |> Option.defaultValue (ResizeArray())
                Expect.equal entries.Count 3 (sprintf "%s: every entry stays." name)
                Expect.equal entries.[0] (SchemaSaladString.Literal "a") (sprintf "%s: the first entry is text." name)
                Expect.equal entries.[1] (SchemaSaladString.Import "b.yml") (sprintf "%s: the second entry is the import." name)
                match entries.[2] with
                | SchemaSaladString.Mixed _ -> Expect.equal entries.[2].Value "c.yml" (sprintf "%s: the third entry is the mixin with its field." name)
                | other -> failwith (sprintf "%s: expected a mixin beside a field, found %A" name other)
                let written = rewritten text
                Expect.stringContains written "{$mixin: c.yml, extra: 1}" (sprintf "%s: the mixin keeps its field.\n%s" name written)
                Expect.equal (read written |> Option.map List.ofSeq) (read text |> Option.map List.ofSeq) (sprintf "%s: the written text should give the same entries." name)
        testCase "a mixin beside a field may stand for the whole of the scatter" <| fun _ ->
            let text = workflowOf [ "in: {}"; "scatter: {$mixin: p.yml, extra: 1}" ]
            Expect.equal (stepOf text).Scatter.Value.Count 1 "The mixin is the one entry."
            Expect.stringContains (rewritten text) "scatter: {$mixin: p.yml, extra: 1}" "The scatter is written as it was."
        testCase "a mapping that is no directive and an include with another field are decode errors in these fields" <| fun _ ->
            let cases =
                [ "a mapping as the scatter", workflowOf [ "in: {}"; "scatter: {a: b}" ], "steps.s.scatter", "Expected a string or a list of strings, found a mapping."
                  "a mapping as the source", workflowOf [ "in:"; "  a:"; "    source: {a: b}" ], "steps.s.in.a.source", "Expected a string or a list of strings, found a mapping."
                  "a mapping as the source in a list", workflowOf [ "in:"; "  - id: a"; "    source: {a: b}" ], "steps.s.in.a.source", "Expected a string or a list of strings, found a mapping."
                  "a mapping as the outputSource", outputsOf "{a: b}", "outputs.o.outputSource", "Expected a string or a list of strings, found a mapping."
                  "an import beside a field in the scatter", workflowOf [ "in: {}"; "scatter: {$import: p.yml, extra: 1}" ], "steps.s.scatter", "A directive has exactly one field"
                  "a number as the path in the source", workflowOf [ "in:"; "  a:"; "    source: [{$include: 42}]" ], "steps.s.in.a.source[0].$include", "Expected the path of the $include directive to be a string" ]
            for (name, text, path, message) in cases do
                match decodeError (fun () -> Decode.decodeCWLDocument text |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the field" name)
                    Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
                    Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
                | None -> failwith (sprintf "%s must fail" name)
        testCase "the step input in map form still takes a mapping as the step input and a string or a list as the source" <| fun _ ->
            let text = workflowOf [ "in:"; "  a: x"; "  b: [x, y]"; "  c: {source: x, default: 1}" ]
            let inputs = WorkflowStep.getInputValues (stepOf text)
            Expect.equal (inputs |> Seq.map (fun input -> input.Source |> Option.map (Seq.choose SchemaSaladString.tryLiteral >> List.ofSeq)) |> List.ofSeq) [ Some [ "x" ]; Some [ "x"; "y" ]; Some [ "x" ] ] "The shorthand and the object form both give the source."
            let directives = workflowOf [ "in:"; "  a: [x, {$import: y.yml}]" ]
            Expect.equal ((WorkflowStep.getInputValues (stepOf directives)).[0].Source |> Option.map List.ofSeq) (Some [ SchemaSaladString.Literal "x"; SchemaSaladString.Import "y.yml" ]) "The shorthand list takes a directive."
            match decodeError (fun () -> Decode.decodeWorkflow (workflowOf [ "in:"; "  a: {$import: y.yml}" ]) |> ignore) with
            | Some error -> Expect.stringContains error.Message "A directive cannot stand for the value of an entry of the in field" "A directive cannot stand for the value of a map entry."
            | None -> failwith "A directive as the value of a map entry must fail."
        testCase "the sources of a step input and of an output give the strings that the document writes out and leave the directives out" <| fun _ ->
            let output = (ListEntry.values (Decode.decodeWorkflow (outputsOf "[a/x, {$import: p.yml}]")).Outputs).[0]
            Expect.equal (List.ofSeq (output.GetOutputSources())) [ "a/x" ] "Only the text is an id."
            Expect.equal (List.ofSeq (output.OutputSource.Value.AsValues())) [ SchemaSaladString.Literal "a/x"; SchemaSaladString.Import "p.yml" ] "The entries keep the directive."
            Expect.notEqual (OutputSource.Single (SchemaSaladString.Import "a")) (OutputSource.Single (SchemaSaladString.Literal "a")) "An import differs from the text."
        testCase "the intent takes directives in a list and as a whole, keeps the spelling of the whole field and reads back equal" <| fun _ ->
            for (name, header) in processes do
                let text (spelling: string) = document header ("intent: " + spelling)
                let whole = text "{$import: p.yml}"
                let listed = text "[{$import: p.yml}]"
                Expect.equal (intentOf whole |> Option.map List.ofSeq) (Some [ SchemaSaladString.Import "p.yml" ]) (sprintf "%s: the intent is the import." name)
                Expect.equal (Decode.decodeCWLDocument whole) (Decode.decodeCWLDocument listed) (sprintf "%s: the spelling is layout, so the processes are equal." name)
                Expect.stringContains (rewritten whole) "intent: {$import: p.yml}" (sprintf "%s: the whole field keeps its spelling." name)
                Expect.isFalse ((rewritten listed).Contains "intent: {$import") (sprintf "%s: the listed spelling is written as a list." name)
                Expect.stringContains (rewritten listed) "- {$import: p.yml}" (sprintf "%s: the listed import is an entry." name)
                Expect.stringContains (rewritten (text "{$mixin: p.yml}")) "intent: {$mixin: p.yml}" (sprintf "%s: a mixin as the whole field keeps its spelling." name)
                Expect.stringContains (rewritten (text "{$mixin: p.yml, extra: 1}")) "intent: {$mixin: p.yml, extra: 1}" (sprintf "%s: a mixin beside a field keeps its spelling." name)
                let mixed = text "[a, {$include: b.txt}, {$import: c.yml}, {$mixin: d.yml}]"
                Expect.equal (intentOf mixed |> Option.map List.ofSeq) (Some [ SchemaSaladString.Literal "a"; SchemaSaladString.Include "b.txt"; SchemaSaladString.Import "c.yml"; SchemaSaladString.Mixin "d.yml" ]) (sprintf "%s: the entries keep their kinds." name)
                Expect.equal (Decode.decodeCWLDocument (rewritten mixed)) (Decode.decodeCWLDocument mixed) (sprintf "%s: the written text should decode to the same process." name)
        testCase "the intent is a list, so a text, a mapping and a whole include are decode errors" <| fun _ ->
            for (name, header) in processes do
                for (what, spelling, message) in
                    [ "a text", "http://example.org/i", "Expected a list of strings, found the text"
                      "a mapping", "{a: b}", "Expected a list of strings, found a mapping."
                      "an include", "{$include: p.txt}", "An $include yields the text of a file, but the intent field takes a list of strings." ] do
                    match decodeError (fun () -> Decode.decodeCWLDocument (document header ("intent: " + spelling)) |> ignore) with
                    | Some error ->
                        Expect.equal (DecodeError.pathText error) "intent" (sprintf "%s, %s: the error should show the field" name what)
                        Expect.stringContains error.Message message (sprintf "%s, %s: the error should say what is wrong" name what)
                        Expect.isSome error.Span (sprintf "%s, %s: the error should have the place in the text" name what)
                    | None -> failwith (sprintf "%s, %s must fail" name what)
    ]

let testCWLWorkflowDescriptionDecode =
    testList "Decode" [
        testCase "CWLVersion" <| fun _ ->
            let expected = "v1.2"
            let actual = decodeCWLWorkflowDescription.CWLVersion
            Expect.equal actual expected ""
        testCase "MultipleInputFeatureRequirement" <| fun _ ->
            let requirementsItem = decodeCWLWorkflowDescription.Requirements
            let expected = MultipleInputFeatureRequirement
            let actual = requirementsItem.Value.[0]
            Expect.equal actual expected ""
        testCase "inputs" <| fun _ ->
            let expected = ResizeArray [|
                CWLInput("cores", CWLType.Int)
                CWLInput("db", CWLType.File)
                CWLInput ("stage", CWLType.Directory)
                CWLInput ("outputMzML", CWLType.Directory)
                CWLInput ("outputPSM", CWLType.Directory)
                CWLInput ("inputMzML", CWLType.Directory)
                CWLInput ("paramsMzML", CWLType.File)
                CWLInput ("paramsPSM", CWLType.File)
                // Note: sampleRecord has complex type and will be checked separately
            |]
            let actual = (ListEntry.values decodeCWLWorkflowDescription.Inputs)
            // Check the simple inputs (first 8)
            for i = 0 to 7 do
                Expect.equal actual.[i].Name expected.[i].Name ""
                Expect.equal actual.[i].InputBinding expected.[i].InputBinding ""
                Expect.equal actual.[i].Type_ expected.[i].Type_ ""
            // Check that sampleRecord exists
            Expect.equal actual.Count 9 "Should have 9 inputs including sampleRecord"
            Expect.equal actual.[8].Name "sampleRecord" ""
        testCase "sampleRecord complex type" <| fun _ ->
            let sampleRecordInput = (ListEntry.values decodeCWLWorkflowDescription.Inputs).[8]
            Expect.equal sampleRecordInput.Name "sampleRecord" ""
            Expect.isSome sampleRecordInput.Type_ "sampleRecord should have a type"
            match sampleRecordInput.Type_ with
            | Some (Array arraySchema) ->
                match arraySchema.Items with
                | Record recordSchema ->
                    Expect.isSome recordSchema.Fields "record should have fields"
                    Expect.equal recordSchema.Fields.Value.Count 2 "Should have 2 fields: readsOfOneSample and sampleName"
                | _ -> failwithf "Expected Record for array items but got: %A" arraySchema.Items
            | _ -> failwithf "Expected Array for sampleRecord type but got: %A" sampleRecordInput.Type_
        testList "steps" [
            let workflowSteps = (ListEntry.values decodeCWLWorkflowDescription.Steps)
            testList "IDs" [
                testCase "MzMLToMzlite" <| fun _ ->
                    let expected = "MzMLToMzlite"
                    let actual = workflowSteps.[0].Id
                    Expect.equal actual expected ""
                testCase "PeptideSpectrumMatching" <| fun _ ->
                    let expected = "PeptideSpectrumMatching"
                    let actual = workflowSteps.[1].Id
                    Expect.equal actual expected ""
            ]
            testList "Run" [
                testCase "MzMLToMzlite" <| fun _ ->
                    let expected = "./runs/MzMLToMzlite/proteomiqon-mzmltomzlite.cwl"
                    let actual = workflowSteps.[0].Run
                    match actual with
                    | RunString runPath -> Expect.equal runPath expected ""
                    | _ -> failwithf "Expected RunString but got %A" actual
                testCase "PeptideSpectrumMatching" <| fun _ ->
                    let expected = "./runs/PeptideSpectrumMatching/proteomiqon-peptidespectrummatching.cwl"
                    let actual = workflowSteps.[1].Run
                    match actual with
                    | RunString runPath -> Expect.equal runPath expected ""
                    | _ -> failwithf "Expected RunString but got %A" actual
            ]
            testList "In" [
                testCase "MzMLToMzlite" <| fun _ ->
                    let expected = ResizeArray [|
                        mkStepInput "stageDirectory" (Some (ResizeArray [|"stage"|])) None
                        mkStepInput "inputDirectory" (Some (ResizeArray [|"inputMzML"|])) None
                        mkStepInput "params" (Some (ResizeArray [|"paramsMzML"|])) None
                        mkStepInput "outputDirectory" (Some (ResizeArray [|"outputMzML"|])) None
                        mkStepInput "parallelismLevel" (Some (ResizeArray [|"cores"|])) None
                    |]
                    let actual = WorkflowStep.getInputValues workflowSteps.[0]
                    Seq.iter2 (fun (expected: StepInput) (actual: StepInput) ->
                        Expect.equal actual.Id expected.Id ""
                        Expect.sequenceEqual actual.Source.Value expected.Source.Value ""
                        Expect.equal
                            (actual.DefaultValue |> Option.bind tryYamlScalarString)
                            (expected.DefaultValue |> Option.bind tryYamlScalarString)
                            ""
                        Expect.equal actual.ValueFrom expected.ValueFrom ""
                        Expect.equal actual.LinkMerge expected.LinkMerge ""
                        Expect.equal actual.PickValue expected.PickValue ""
                        Expect.equal actual.Doc expected.Doc ""
                        Expect.equal actual.LoadContents expected.LoadContents ""
                        Expect.equal actual.LoadListing expected.LoadListing ""
                        Expect.equal actual.Label expected.Label ""
                    ) expected actual
                testCase "PeptideSpectrumMatching" <| fun _ ->
                    let expected = ResizeArray [|
                        mkStepInput "stageDirectory" (Some (ResizeArray [|"stage"|])) None
                        mkStepInput "inputDirectory" (Some (ResizeArray [|"MzMLToMzlite/dir1"; "MzMLToMzlite/dir2"|])) (Some MergeFlattened)
                        mkStepInput "database" (Some (ResizeArray [|"db"|])) None
                        mkStepInput "params" (Some (ResizeArray [|"paramsPSM"|])) None
                        mkStepInput "outputDirectory" (Some (ResizeArray [|"outputPSM"|])) None
                        mkStepInput "parallelismLevel" (Some (ResizeArray [|"cores"|])) None
                    |]
                    let actual = WorkflowStep.getInputValues workflowSteps.[1]
                    Seq.iter2 (fun (expected: StepInput) (actual: StepInput) ->
                        Expect.equal actual.Id expected.Id ""
                        Expect.sequenceEqual actual.Source.Value expected.Source.Value ""
                        Expect.equal
                            (actual.DefaultValue |> Option.bind tryYamlScalarString)
                            (expected.DefaultValue |> Option.bind tryYamlScalarString)
                            ""
                        Expect.equal actual.ValueFrom expected.ValueFrom ""
                        Expect.equal actual.LinkMerge expected.LinkMerge ""
                        Expect.equal actual.PickValue expected.PickValue ""
                        Expect.equal actual.Doc expected.Doc ""
                        Expect.equal actual.LoadContents expected.LoadContents ""
                        Expect.equal actual.LoadListing expected.LoadListing ""
                        Expect.equal actual.Label expected.Label ""
                    ) expected actual
            ]
            testList "Out" [
                testCase "MzMLToMzlite" <| fun _ ->
                    let expected = ResizeArray [| StepOutputString "dir" |]
                    let actual = workflowSteps.[0].Out
                    Expect.sequenceEqual actual expected ""
                testCase "PeptideSpectrumMatching" <| fun _ ->
                    let expected = ResizeArray [| StepOutputString "dir" |]
                    let actual = workflowSteps.[1].Out
                    Expect.sequenceEqual actual expected ""
            ]
        ]
        testCase "outputs" <| fun _ ->
            let expected = ResizeArray [|
                CWLOutput("mzlite", CWLType.Directory, outputSource = OutputSource.Single (SchemaSaladString.Literal "MzMLToMzlite/dir"))
                CWLOutput("psm", CWLType.Directory, outputSource = OutputSource.Single (SchemaSaladString.Literal "PeptideSpectrumMatching/dir"))
            |]
            let actual = (ListEntry.values decodeCWLWorkflowDescription.Outputs)
            for i = 0 to actual.Count - 1 do
                Expect.equal actual.[i].Name expected.[i].Name ""
                Expect.equal actual.[i].OutputBinding expected.[i].OutputBinding ""
                Expect.equal actual.[i].Type_ expected.[i].Type_ ""
                Expect.equal actual.[i].OutputSource expected.[i].OutputSource ""
        testCase "workflow intent decodes as typed field, not metadata overflow" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithIntentFile
            let intent = Expect.wantSome decoded.Intent "Intent should decode on Workflow."
            Expect.sequenceEqual intent (literals [ "primary-analysis"; "quality-control" ]) ""
            Expect.isNone decoded.Metadata "Intent should not be captured as overflow metadata."
        testCase "extended step fields decode" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithExtendedStepFile
            let step = (ListEntry.values decoded.Steps).[0]
            Expect.equal step.Label (Some (SchemaSaladString.Literal "Example step")) ""
            Expect.equal step.Doc (Some (DocText.literal "Step docs")) ""
            Expect.sequenceEqual step.Scatter.Value (literals [ "input1" ]) ""
            Expect.equal step.ScatterMethod (Some DotProduct) ""
            Expect.equal (WorkflowStep.getInputValues step).[0].LoadContents (Some (SchemaSaladBool.Value true)) ""
            Expect.equal (WorkflowStep.getInputValues step).[0].LoadListing (Some LoadListingEnum.DeepListing) ""
            Expect.equal (WorkflowStep.getInputValues step).[0].Label (Some (SchemaSaladString.Literal "Input label")) ""
            Expect.equal (WorkflowStep.getInputValues step).[0].LinkMerge (Some MergeNested) ""
            let expectedOut = ResizeArray [| StepOutputRecord (StepOutputParameter.create "out") |]
            Expect.sequenceEqual step.Out expectedOut ""
        testCase "inline run commandline tool decode" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithInlineRunCommandLineToolFile
            match (ListEntry.values decoded.Steps).[0].Run with
            | RunCommandLineTool toolObj ->
                match toolObj with
                | :? CWLToolDescription as tool ->
                    Expect.equal tool.CWLVersion "v1.2" ""
                | _ ->
                    failwithf "Expected CWLToolDescription payload but got %A" toolObj
            | other ->
                failwithf "Expected RunCommandLineTool but got %A" other
        testCase "an inline run takes the version of the enclosing process and ignores its own cwlVersion" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.InlineRunVersions.workflowFile
            let step (id: string) = (ListEntry.values decoded.Steps) |> Seq.find (fun s -> s.Id = id)
            let tool (run: WorkflowStepRun) = Expect.wantSome (WorkflowStepRunOps.tryGetTool run) "Expected an inline tool"
            Expect.equal (tool (step "plain").Run).CWLVersion "v1.2" "A run without a version takes the version of the workflow."
            Expect.equal (tool (step "own_version").Run).CWLVersion "v1.2" "A run that names v1.0 uses the version of the workflow."
            let nested = Expect.wantSome (WorkflowStepRunOps.tryGetWorkflow (step "nested").Run) "Expected an inline workflow"
            Expect.equal nested.CWLVersion "v1.2" "An inline workflow without a version takes the version of the enclosing one."
            Expect.equal (tool (ListEntry.values nested.Steps).[0].Run).CWLVersion "v1.2" "The process in it takes that version."
        testCase "a process without a cwlVersion is a decode error when no process encloses it" <| fun _ ->
            let yaml = """class: CommandLineTool
baseCommand: echo
inputs: {}
outputs: {}"""
            match (try Decode.decodeCommandLineTool yaml |> ignore; None with YamlDecodeException error -> Some error) with
            | Some error ->
                Expect.stringContains error.Message "cwlVersion" "The error names the missing field."
                Expect.isSome error.Span "The error has the place in the text."
            | None -> failwith "A process at the root needs a cwlVersion."
        testCase "the cwlVersion at the root is a decode error at the field when it has no value or holds a list or a mapping" <| fun _ ->
            let tool (field: string) = "cwlVersion:" + field + "\nclass: CommandLineTool\nbaseCommand: echo\ninputs: {}\noutputs: {}"
            let graph (field: string) = "cwlVersion:" + field + "\n$graph:\n  - id: main\n    class: CommandLineTool\n    baseCommand: echo\n    inputs: {}\n    outputs: {}"
            let cases =
                [ "a list", " [v1.2]", None
                  "a mapping", " {version: v1.2}", None
                  "no value", "", Some "Expected a value, found" ]
            for (name, field, message) in cases do
                for (kind, decode) in [ "a process", (fun () -> Decode.decodeCWLDocument (tool field) |> ignore); "a packed document", (fun () -> Decode.decodeCWLDocument (graph field) |> ignore) ] do
                    match decodeError decode with
                    | Some error ->
                        Expect.equal (DecodeError.pathText error) "cwlVersion" $"{name} in {kind}: the error should show the field."
                        message |> Option.iter (fun text -> Expect.stringContains error.Message text $"{name} in {kind}: the error should say that the value is missing.")
                        Expect.isSome error.Span $"{name} in {kind}: the error should have the place in the text."
                    | None -> failwith $"{name} in {kind} must fail"
        testCase "the cwlVersion at the root is read as written and every document decodes under the rules of v1.2" <| fun _ ->
            // A NetworkAccess requirement exists from v1.1, so the documents of v1.0 and v1.3 show that the version selects no rule.
            for version in [ "v1.0"; "v1.1"; "v1.2"; "v1.2.0-dev2"; "v1.3"; "v1.3.0-dev1"; "banana" ] do
                let tool = "cwlVersion: " + version + "\nclass: CommandLineTool\nrequirements:\n  - class: NetworkAccess\n    networkAccess: true\nbaseCommand: echo\ninputs: {}\noutputs: {}"
                let graph = "cwlVersion: " + version + "\n$graph:\n  - id: main\n    class: CommandLineTool\n    requirements:\n      - class: NetworkAccess\n        networkAccess: true\n    baseCommand: echo\n    inputs: {}\n    outputs: {}"
                let decodedTool = Decode.decodeCommandLineTool tool
                Expect.equal decodedTool.CWLVersion version $"A process decodes with {version}."
                Expect.isSome decodedTool.Requirements $"A process of {version} keeps its NetworkAccess requirement."
                Expect.stringContains (Encode.encodeToolDescription decodedTool) ("cwlVersion: " + version) $"A process writes {version} back."
                match Decode.decodeCWLDocument graph with
                | CWLDocument.Graph decoded ->
                    Expect.equal decoded.CWLVersion version $"A packed document decodes with {version}."
                    Expect.stringContains (Encode.encodeCWLDocument (CWLDocument.Graph decoded)) ("cwlVersion: " + version) $"A packed document writes {version} back."
                | other -> failwith $"Expected a packed document but got %A{other}"
        testCase "a number at the root is read as the text of its spelling and written back" <| fun _ ->
            let tool = "cwlVersion: 1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs: {}\noutputs: {}"
            let decoded = Decode.decodeCommandLineTool tool
            Expect.equal decoded.CWLVersion "1.2" "The number is read as its text."
            Expect.equal (Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded)).CWLVersion "1.2" "The text reads back as the same version."
        testCase "the cwlVersion of an inline run changes nothing whatever it holds" <| fun _ ->
            let head = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  s:
    run:
      class: CommandLineTool
      cwlVersion:"""
            let tail = """
      baseCommand: echo
      inputs: {}
      outputs: {}
    in: {}
    out: []"""
            for (name, field) in [ "no value", ""; "an older version", " v1.0"; "a draft", " draft-3"; "the version itself", " v1.2"; "a newer version", " v1.3"; "a text that is no version", " banana" ] do
                let decoded = Decode.decodeWorkflow (head + field + tail)
                let tool = Expect.wantSome (WorkflowStepRunOps.tryGetTool (ListEntry.values decoded.Steps).[0].Run) "The step runs an inline tool."
                Expect.equal tool.CWLVersion "v1.2" $"{name}: the inline run takes the version of the workflow."
        testCase "step input array form decode" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithInputArrayStepFile
            let stepInput = (WorkflowStep.getInputValues (ListEntry.values decoded.Steps).[0]).[0]
            Expect.equal stepInput.Id "in1" ""
            Expect.sequenceEqual stepInput.Source.Value (literals [ "input1" ]) ""
            Expect.equal stepInput.Label (Some (SchemaSaladString.Literal "Input in array syntax")) ""
        testCase "workflow with no steps decodes to empty" <| fun _ ->
            let yaml = TestObjects.CWL.Workflow.workflowWithNoStepsFile
            let decoded = Decode.decodeWorkflow yaml
            Expect.equal (ListEntry.values decoded.Steps).Count 0 ""
        testCase "sequence-form workflow ports with comments decode" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithSequencePortsAndCommentsFile
            Expect.equal (ListEntry.values decoded.Inputs).Count 2 "Sequence-form inputs should decode."
            Expect.equal (ListEntry.values decoded.Outputs).Count 2 "Sequence-form outputs should decode."
            Expect.equal (ListEntry.values decoded.Steps).Count 2 "Sequence-form steps should decode after comments."
            Expect.isTrue ((ListEntry.values decoded.Inputs) |> Seq.exists (fun input -> input.Name = "input_data")) "input_data should decode."
            Expect.isTrue ((ListEntry.values decoded.Outputs) |> Seq.exists (fun output -> output.Name = "result_data")) "result_data output should decode."
            Expect.equal decoded.Doc (Some (DocText.literal "Workflow doc line.\n# This line is doc text, not a YAML comment.\n")) "Block scalar comment-looking text should be preserved."
        testCase "sequence-form workflow fields with blank separator lines decode" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithBlankSeparatedSequenceFieldsFile
            Expect.equal (ListEntry.values decoded.Inputs).Count 2 "Blank lines before and between sequence-form inputs should decode."
            Expect.equal (ListEntry.values decoded.Outputs).Count 1 "Blank lines before sequence-form outputs should decode."
            Expect.equal (ListEntry.values decoded.Steps).Count 1 "Blank lines before sequence-form steps should decode."
            Expect.equal (ListEntry.values decoded.Steps).[0].In.Count 2 "Blank lines before and between nested step inputs should decode."
            Expect.isTrue ((ListEntry.values decoded.Inputs) |> Seq.exists (fun input -> input.Name = "trial_id")) "trial_id should decode."
            Expect.equal (ListEntry.values decoded.Steps).[0].Id "collect" "The step following blank separators should stay mapped under steps."
        testCase "comment-separated map-form workflow inputs decode" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithLowerIndentedCommentMapInputsFile
            Expect.equal (ListEntry.values decoded.Inputs).Count 2 "Comment-separated map-form inputs should decode."
            Expect.isTrue ((ListEntry.values decoded.Inputs) |> Seq.exists (fun input -> input.Name = "first")) "first input should decode."
            Expect.isTrue ((ListEntry.values decoded.Inputs) |> Seq.exists (fun input -> input.Name = "second")) "second input should decode."
        testCase "comment before sequence-form workflow inputs decodes" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithLowerIndentedCommentBeforeSequenceInputFile
            Expect.equal (ListEntry.values decoded.Inputs).Count 1 "Comment before sequence-form inputs should decode."
            Expect.equal (ListEntry.values decoded.Inputs).[0].Name "first" "first input should decode."
        testCase "multiline valueFrom expression decodes as one string" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithMultilineValueFromFile
            let step = (ListEntry.values decoded.Steps) |> Seq.find (fun step -> step.Id = "expr_step")
            let stepInput = WorkflowStep.getInputValues step |> Seq.find (fun input -> input.Id = "reads")
            let valueFrom = (Expect.wantSome stepInput.ValueFrom "valueFrom should decode.").Value
            Expect.stringContains valueFrom "${" "valueFrom should include the expression opener."
            Expect.stringContains valueFrom "return reads;" "valueFrom should include the expression body."
            Expect.stringContains valueFrom "}" "valueFrom should include the expression closer."
        testCase "optional array shorthand input types decode" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithOptionalArrayInputTypesFile
            let assertOptionalArray name expectedItem =
                let input = (ListEntry.values decoded.Inputs) |> Seq.find (fun input -> input.Name = name)
                match input.Type_ with
                | Some (Union types) ->
                    Expect.isTrue (types |> Seq.exists ((=) Null)) $"{name} should include null in the union."
                    let arrayItems =
                        types
                        |> Seq.choose (function | Array arraySchema -> Some arraySchema.Items | _ -> None)
                        |> Seq.toList
                    Expect.equal arrayItems [expectedItem] $"{name} should include exactly one array type."
                | other -> failwithf "Expected optional array type for %s, got %A" name other
            assertOptionalArray "optional_files" (File)
            assertOptionalArray "optional_dirs" (Directory)
        testCase "inline enum and record schema union types decode" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithInlineSchemaUnionTypesFile
            let mode = (ListEntry.values decoded.Inputs) |> Seq.find (fun input -> input.Name = "mode")
            match mode.Type_ with
            | Some (Union types) ->
                Expect.isTrue (types |> Seq.exists ((=) Null)) "mode should include null in the union."
                match types |> Seq.choose (function | Enum schema -> Some schema | _ -> None) |> Seq.toList with
                | [enumSchema] ->
                    Expect.equal enumSchema.Name (Some "Mode") "Inline enum name should decode."
                    Expect.sequenceEqual (InputEnumSchema.getSymbolValues enumSchema) (ResizeArray [| "fast"; "stringent" |]) "Inline enum symbols should decode."
                | other -> failwithf "Expected one inline enum schema, got %A" other
            | other -> failwithf "Expected mode to decode as a union, got %A" other
            let recordPayload = (ListEntry.values decoded.Inputs) |> Seq.find (fun input -> input.Name = "record_payload")
            match recordPayload.Type_ with
            | Some (Union types) ->
                Expect.isTrue (types |> Seq.exists ((=) Null)) "record_payload should include null in the union."
                match types |> Seq.choose (function | Record schema -> Some schema | _ -> None) |> Seq.toList with
                | [recordSchema] ->
                    Expect.equal recordSchema.Name (Some "Payload") "Inline record name should decode."
                    let fields = Expect.wantSome recordSchema.Fields "Inline record fields should decode." |> ListEntry.values
                    let sampleId = fields |> Seq.find (fun field -> field.Name = "sample_id")
                    Expect.equal sampleId.Type String "sample_id type should decode."
                    let files = fields |> Seq.find (fun field -> field.Name = "files")
                    match files.Type with
                    | Array arraySchema -> Expect.equal arraySchema.Items (File) "files should decode as File array."
                    | other -> failwithf "Expected files to decode as an array, got %A" other
                | other -> failwithf "Expected one inline record schema, got %A" other
            | other -> failwithf "Expected record_payload to decode as a union, got %A" other
            Expect.isNone (DynObj.tryGetTypedPropertyValue<obj> "symbols" decoded) "Nested enum fields should not become workflow overflow."
            Expect.isNone (DynObj.tryGetTypedPropertyValue<obj> "fields" decoded) "Nested record fields should not become workflow overflow."
        testCase "decodeWorkflowWithWarnings skips malformed unnamed entries" <| fun _ ->
            let result = Decode.decodeWorkflowWithWarnings TestObjects.CWL.Workflow.workflowWithUnnamedMalformedEntriesFile
            Expect.equal (ListEntry.values result.Value.Inputs).Count 1 "Only valid input should decode."
            Expect.equal (ListEntry.values result.Value.Outputs).Count 1 "Only valid output should decode."
            Expect.equal (ListEntry.values result.Value.Steps).Count 1 "Only valid step should decode."
            Expect.equal (ListEntry.values result.Value.Steps).[0].In.Count 1 "Only valid step input should decode."
            Expect.equal result.Warnings.Count 4 "Each malformed unnamed entry should produce one warning."
            Expect.sequenceEqual
                (result.Warnings |> Seq.map (fun w -> w.Path) |> Seq.sort |> ResizeArray)
                (ResizeArray [| "inputs[1]"; "outputs[1]"; "steps[0]"; "steps[1].in[1]" |] |> Seq.sort |> ResizeArray)
                "Warning paths should identify skipped entries."
        testCase "the warning of a skipped entry holds the YAML that the entry was written in" <| fun _ ->
            let result = Decode.decodeWorkflowWithWarnings TestObjects.CWL.Workflow.workflowWithUnnamedMalformedEntriesFile
            let rawOf (path: string) = (result.Warnings |> Seq.find (fun w -> w.Path = path)).Raw
            Expect.equal (rawOf "inputs[1]") (Some "not_a_cwl_port: true") "A skipped mapping shows its entries."
            Expect.equal (rawOf "outputs[1]") (Some "[not, a, cwl, output]") "A skipped flow sequence keeps its flow style."
            Expect.equal (rawOf "steps[0]") (Some "\"not a workflow step\"") "A skipped scalar keeps its quotes."
            Expect.equal (rawOf "steps[1].in[1]") (Some "not_a_step_input: true") "A skipped step input shows its entries."
            let withComment = """cwlVersion: v1.2
class: Workflow
inputs:
- not_a_port: true # about it
  other: [1, 2]
outputs: {}
steps: {}
"""
            let commented = Decode.decodeWorkflowWithWarnings withComment
            Expect.equal commented.Warnings.[0].Raw (Some "not_a_port: true # about it\nother: [1, 2]") "A skipped entry keeps its comment and its flow sequence."
        testCase "malformed named input still fails" <| fun _ ->
            Expect.throws
                (fun _ -> Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithMalformedNamedInputFile |> ignore)
                "Named CWL inputs with missing type should fail."
        testCase "malformed known field on named input fails" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: Workflow
inputs:
- id: named
  type: string
  label: [not, a, string]
outputs: {}
steps: {}"""
            Expect.throws
                (fun _ -> Decode.decodeWorkflow yaml |> ignore)
                "Named CWL inputs with malformed known fields should fail."
        testCase "malformed id-bearing sequence entries fail instead of becoming warnings" <| fun _ ->
            let malformedInputId = """cwlVersion: v1.2
class: Workflow
inputs:
- id: [not, a, string]
  type: string
outputs: {}
steps: {}"""
            let malformedOutputId = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
- id: [not, a, string]
  type: string
steps: {}"""
            let malformedStepId = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
- id: [not, a, string]
  run: ./tool.cwl
  in: []
  out: []"""
            let malformedStepInputId = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
- id: step1
  run: ./tool.cwl
  in:
  - id: [not, a, string]
    source: input
  out: []"""
            Expect.throws
                (fun _ -> Decode.decodeWorkflowWithWarnings malformedInputId |> ignore)
                "Malformed input ids should fail rather than become skipped unnamed inputs."
            Expect.throws
                (fun _ -> Decode.decodeWorkflowWithWarnings malformedOutputId |> ignore)
                "Malformed output ids should fail rather than become skipped unnamed outputs."
            Expect.throws
                (fun _ -> Decode.decodeWorkflowWithWarnings malformedStepId |> ignore)
                "Malformed step ids should fail rather than become skipped unnamed steps."
            Expect.throws
                (fun _ -> Decode.decodeWorkflowWithWarnings malformedStepInputId |> ignore)
                "Malformed step input ids should fail rather than become skipped unnamed step inputs."
        testCase "unknown fields on named ports and steps are preserved as dynamic properties" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithUnknownPortFieldsFile
            let input = (ListEntry.values decoded.Inputs) |> Seq.find (fun input -> input.Name = "sample")
            let output = (ListEntry.values decoded.Outputs) |> Seq.find (fun output -> output.Name = "result")
            let step = (ListEntry.values decoded.Steps) |> Seq.find (fun step -> step.Id = "step1")
            let stepInput = WorkflowStep.getInputValues step |> Seq.find (fun input -> input.Id = "sample")
            let inputBinding = Expect.wantSome input.InputBinding "Input binding should decode."
            Expect.equal
                (DynObj.tryGetTypedPropertyValue<string> "arc:note" input)
                (Some "keep input note")
                "Unknown input fields should be preserved."
            Expect.equal input.Type_ (Some CWLType.File) "The sample input should decode as File."
            Expect.isNone
                (DynObj.tryGetTypedPropertyValue<string> "arc:binding note" input)
                "Unknown input binding fields should not be flattened onto the input."
            Expect.equal
                (DynObj.tryGetTypedPropertyValue<string> "arc:binding note" inputBinding)
                (Some "keep input binding note")
                "Unknown input binding fields should be preserved on the binding."
            Expect.equal
                (DynObj.tryGetTypedPropertyValue<string> "arc:note" output)
                (Some "keep output note")
                "Unknown output fields should be preserved."
            Expect.equal output.Type_ (Some CWLType.File) "The result output should decode as File."
            Expect.equal
                (DynObj.tryGetTypedPropertyValue<string> "arc:step note" step)
                (Some "keep step note")
                "Unknown workflow step fields should be preserved."
            Expect.isNone
                (DynObj.tryGetTypedPropertyValue<string> "arc:step input note" step)
                "Unknown step input fields should not be flattened onto the step."
            Expect.equal
                (DynObj.tryGetTypedPropertyValue<string> "arc:step input note" stepInput)
                (Some "keep step input note")
                "Unknown step input fields should be preserved on the step input."
        testCase "steps array form decode with when/pickValue/doc/default" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithStepsArrayFile
            let step = (ListEntry.values decoded.Steps).[0]
            let stepInput = (WorkflowStep.getInputValues step).[0]
            Expect.equal step.Id "step1" ""
            Expect.equal step.When_ (Some (SchemaSaladString.Literal "$(inputs.input1 != null)")) ""
            Expect.equal stepInput.PickValue (Some FirstNonNull) ""
            Expect.equal stepInput.Doc (Some (DocText.literal "Input docs")) ""
            let defaultValue =
                stepInput.DefaultValue
                |> Option.bind tryYamlScalarString
            Expect.equal defaultValue None "Default is structured object and should not collapse to scalar"
        testCase "inline run workflow decode" <| fun _ ->
            let yaml = TestObjects.CWL.Workflow.workflowWithInlineRunWorkflowFile
            let decoded = Decode.decodeWorkflow yaml
            let runValue = (ListEntry.values decoded.Steps).[0].Run
            let isRunWorkflow =
                match runValue with
                | RunWorkflow _ -> true
                | _ -> false
            Expect.isTrue isRunWorkflow $"Expected RunWorkflow but got %A{runValue}"
            let workflow = Expect.wantSome (WorkflowStepRunOps.tryGetWorkflow runValue) "Expected inline workflow payload"
            Expect.equal (ListEntry.values workflow.Steps).Count 1 ""
            Expect.equal (ListEntry.values workflow.Steps).[0].Id "inner" ""
        testCase "workflow outputSource array form decodes and roundtrips" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithOutputSourceArrayFile
            let encoded = Encode.encodeWorkflowDescription decoded
            let roundTripped = Decode.decodeWorkflow encoded
            Expect.stringContains encoded "outputSource:" "Array-form outputSource should be emitted."
            let mergedOutput =
                (ListEntry.values roundTripped.Outputs)
                |> Seq.find (fun output -> output.Name = "merged")
            let outputSources = mergedOutput.GetOutputSources()
            Expect.sequenceEqual outputSources (ResizeArray [| "step1/out"; "step2/out" |]) "outputSource entries should survive decode/encode/decode."
        testCase "workflow step run Operation class decodes and roundtrips" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithInlineRunOperationFile
            let encoded = Encode.encodeWorkflowDescription decoded
            let roundTripped = Decode.decodeWorkflow encoded
            Expect.stringContains encoded "class: Operation" "Operation run should roundtrip with class marker."
            Expect.equal (ListEntry.values roundTripped.Steps).[0].Id "op" "Operation-backed step should survive roundtrip."
        testCase "invalid pickValue fails decode" <| fun _ ->
            let invalidPickValue = TestObjects.CWL.Workflow.workflowWithInvalidPickValueFile
            Expect.throws (fun _ -> Decode.decodeWorkflow invalidPickValue |> ignore) "Invalid pickValue should fail"
        testCase "invalid scatterMethod fails decode" <| fun _ ->
            let invalidScatter = TestObjects.CWL.Workflow.workflowWithInvalidScatterMethodFile
            Expect.throws (fun _ -> Decode.decodeWorkflow invalidScatter |> ignore) "Invalid scatterMethod should fail"
        testCase "inline ExpressionTool run decode succeeds" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithUnsupportedInlineRunClassFile
            let runValue = (ListEntry.values decoded.Steps).[0].Run
            let isRunExpressionTool =
                match runValue with
                | RunExpressionTool _ -> true
                | _ -> false
            Expect.isTrue isRunExpressionTool $"Expected RunExpressionTool but got %A{runValue}"
            let expressionTool =
                Expect.wantSome
                    (WorkflowStepRunOps.tryGetExpressionTool runValue)
                    "Should extract ExpressionTool payload"
            Expect.equal expressionTool.Expression (SchemaSaladString.Literal "$(null)") ""
        testCase "inline ExpressionTool chain decodes all steps" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithInlineExpressionToolChainFile
            Expect.equal (ListEntry.values decoded.Steps).Count 3 "Should have 3 steps"
            for i in 0..2 do
                let step = (ListEntry.values decoded.Steps).[i]
                let expressionTool =
                    Expect.wantSome
                        (WorkflowStepRunOps.tryGetExpressionTool step.Run)
                        $"Step {i} should be ExpressionTool"
                Expect.isTrue (expressionTool.Expression.Value.Length > 0) $"Step {i} expression should be non-empty"
        testCase "loadContents on inline ExpressionTool input decoded" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithLoadContentsExpressionToolFile
            let step = (ListEntry.values decoded.Steps).[0]
            let expressionTool =
                Expect.wantSome (WorkflowStepRunOps.tryGetExpressionTool step.Run) "Step run should be inline ExpressionTool"
            let hasLoadContentsOnStepInput =
                WorkflowStep.getInputValues step |> Seq.exists (fun si -> si.LoadContents = Some (SchemaSaladBool.Value true))
            let hasValueFromOnStepInput =
                WorkflowStep.getInputValues step |> Seq.exists (fun si -> si.ValueFrom.IsSome)
            Expect.isTrue hasLoadContentsOnStepInput "Step input should have loadContents"
            Expect.isTrue hasValueFromOnStepInput "Step input should have valueFrom"
            let expressionToolInputs = expressionTool.Inputs |> Option.defaultValue (ResizeArray())
            Expect.equal expressionToolInputs.Count 1 "Inline expression tool should have one input"
        testCase "mixed workflow with CommandLineTool and ExpressionTool steps" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithMixedToolAndExpressionStepFile
            Expect.equal (ListEntry.values decoded.Steps).Count 2 ""
            match (ListEntry.values decoded.Steps).[0].Run with
            | RunString _ -> ()
            | other -> Expect.isTrue false $"Step 0 should be RunString but got %A{other}"
            let isExpressionToolStep =
                match (ListEntry.values decoded.Steps).[1].Run with
                | RunExpressionTool _ -> true
                | _ -> false
            Expect.isTrue isExpressionToolStep "Step 1 should be RunExpressionTool"
        testCase "chained ExpressionTool array pipeline type verification" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithInlineExpressionToolChainFile
            let step1 = Expect.wantSome (WorkflowStepRunOps.tryGetExpressionTool (ListEntry.values decoded.Steps).[0].Run) "Step 1 should be ExpressionTool"
            let step2 = Expect.wantSome (WorkflowStepRunOps.tryGetExpressionTool (ListEntry.values decoded.Steps).[1].Run) "Step 2 should be ExpressionTool"
            let step3 = Expect.wantSome (WorkflowStepRunOps.tryGetExpressionTool (ListEntry.values decoded.Steps).[2].Run) "Step 3 should be ExpressionTool"

            Expect.equal (ListEntry.values step1.Inputs.Value).[0].Type_ (Some CWLType.Int) "step1 input should be int"
            match (ListEntry.values step1.Outputs).[0].Type_ with
            | Some (Array arraySchema) ->
                Expect.equal arraySchema.Items CWLType.Int "step1 output should be int[]"
            | other ->
                Expect.isTrue false $"Expected int[] but got %A{other}"

            // Asserted via `if`/bool rather than a `match` with a unit success branch: Fable's
            // Python backend drops empty branches from a match in statement position, which would
            // leave `Expect.isTrue false` unconditional and make this assertion always fail.
            let step2InputType = (ListEntry.values step2.Inputs.Value).[0].Type_
            let isArray = match step2InputType with Some (Array _) -> true | _ -> false
            Expect.isTrue isArray $"step2 input should be int[] but got %A{step2InputType}"

            Expect.equal (ListEntry.values step3.Outputs).[0].Type_ (Some CWLType.Int) "step3 output should be int"
        testCase "step-level loadContents with valueFrom on ExpressionTool" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithLoadContentsExpressionToolFile
            let step = (ListEntry.values decoded.Steps).[0]
            let stepInputHasLoadContents =
                WorkflowStep.getInputValues step |> Seq.exists (fun si -> si.LoadContents = Some (SchemaSaladBool.Value true))
            let stepInputHasValueFrom =
                WorkflowStep.getInputValues step |> Seq.exists (fun si -> si.ValueFrom.IsSome)
            Expect.isTrue stepInputHasLoadContents "Step input should have loadContents"
            Expect.isTrue stepInputHasValueFrom "Step input should have valueFrom"
        testCase "advanced workflow fixture decodes and preserves full content" <| fun _ ->
            let original = TestObjects.CWL.Workflow.advancedWorkflowFile
            let processingUnit = Decode.decodeCWLProcessingUnit original
            let workflow =
                match processingUnit with
                | Workflow workflow -> workflow
                | other -> failwithf "Expected Workflow but got %A" other

            Expect.equal workflow.CWLVersion "v1.2" ""
            Expect.equal workflow.Inputs.Count 24 "All workflow inputs should decode."
            Expect.equal workflow.Steps.Count 11 "All workflow steps should decode."
            Expect.equal workflow.Outputs.Count 1 "Workflow output should decode."
            Expect.equal workflow.Id (Some "CWL_advanced_layout_workflow") "Workflow id should be preserved."

            let hints = Expect.wantSome workflow.Hints "Workflow hints should decode."
            Expect.equal hints.Count 3 "All workflow hints should decode."
            match hints.[0] with
            | KnownHint (DockerRequirement dockerRequirement) ->
                Expect.equal dockerRequirement.DockerImageId (Some (SchemaSaladString.Literal "devcontainer")) "Docker hint should keep dockerImageId."
                Expect.equal dockerRequirement.DockerFile (Some (SchemaSaladString.Include "./Dockerfile")) "Docker hint should keep dockerFile include."
                Expect.sequenceEqual
                    (dockerRequirement.DockerRunOptions |> Option.defaultValue (ResizeArray()))
                    (ResizeArray [| "--gpus=all" |])
                    "Docker hint should keep cwltool docker run options."
            | other ->
                failwithf "Expected first hint to decode as DockerRequirement but got %A" other

            match hints.[1] with
            | UnknownHint unknownHint ->
                Expect.equal unknownHint.Class (Some "cwltool:CUDARequirement") "Unknown CUDA hint should be preserved."
            | other ->
                failwithf "Expected second hint to decode as UnknownHint but got %A" other

            let requirements = Expect.wantSome workflow.Requirements "Workflow requirements should decode."
            Expect.equal requirements.Count 4 "All workflow requirements should decode."

            let encoded = Encode.encodeWorkflowDescription workflow
            let roundTripped = Decode.decodeWorkflow encoded

            let encodeInputs (inputs: ResizeArray<CWLInput>) =
                inputs
                |> Seq.map (Encode.encodeCWLInput >> renderYamlPair)
                |> ResizeArray

            let encodeOutputs (outputs: ResizeArray<CWLOutput>) =
                outputs
                |> Seq.map (Encode.encodeCWLOutput >> renderYamlPair)
                |> ResizeArray

            let encodeSteps (steps: ResizeArray<WorkflowStep>) =
                steps
                |> Seq.map (Encode.encodeWorkflowStep false >> renderYamlPair)
                |> ResizeArray

            let encodeRequirements (requirements: ResizeArray<Requirement>) =
                requirements
                |> Seq.map (Encode.encodeRequirement >> renderYamlElement)
                |> ResizeArray

            let encodeHints (hints: ResizeArray<HintEntry>) =
                hints
                |> Seq.map (Encode.encodeHintEntry >> renderYamlElement)
                |> ResizeArray

            Expect.equal roundTripped.CWLVersion workflow.CWLVersion "Workflow version should roundtrip."
            Expect.equal roundTripped.Id workflow.Id "Workflow id should roundtrip."
            Expect.sequenceEqual (encodeHints (Expect.wantSome roundTripped.Hints "Roundtripped workflow hints should exist.")) (encodeHints hints) "Workflow hints should roundtrip."
            Expect.sequenceEqual (encodeRequirements (Expect.wantSome roundTripped.Requirements "Roundtripped workflow requirements should exist.")) (encodeRequirements requirements) "Workflow requirements should roundtrip."
            Expect.sequenceEqual (encodeInputs (ListEntry.values roundTripped.Inputs)) (encodeInputs (ListEntry.values workflow.Inputs)) "Workflow inputs should roundtrip."
            Expect.sequenceEqual (encodeSteps (ListEntry.values roundTripped.Steps)) (encodeSteps (ListEntry.values workflow.Steps)) "Workflow steps should roundtrip."
            Expect.sequenceEqual (encodeOutputs (ListEntry.values roundTripped.Outputs)) (encodeOutputs (ListEntry.values workflow.Outputs)) "Workflow outputs should roundtrip."
    ]

let testCWLWorkflowDescriptionEncode =
    testList "Encode" [
        testList "Workflow negative tests" [
            testCase "missing outputSource detected" <| fun _ ->
                let original = TestObjects.CWL.Workflow.workflowFile
                let decoded = Decode.decodeWorkflow original
                let encoded = Encode.encodeWorkflowDescription decoded
                // Simulate user editing and deleting outputSource lines
                let modified =
                    encoded.Split('\n')
                    |> Array.filter (fun l -> not (l.TrimStart().StartsWith("outputSource:")))
                    |> String.concat "\n"
                let decodedModified = Decode.decodeWorkflow modified
                let missing = (ListEntry.values decodedModified.Outputs) |> Seq.filter (fun o -> o.OutputSource.IsNone) |> Seq.toList
                Expect.isTrue (missing.Length > 0) "At least one output should be missing outputSource after manual removal"
        ]
        testList "Process elements" [
            testCase "the process element encoders write the cwlVersion only when asked to" <| fun _ ->
                let elements (includeCwlVersion: bool) =
                    [ Encode.encodeToolDescriptionElement includeCwlVersion (CWLToolDescription(outputs = ResizeArray()))
                      Encode.encodeWorkflowDescriptionElement false includeCwlVersion (CWLWorkflowDescription(steps = ResizeArray(), inputs = ResizeArray(), outputs = ResizeArray()))
                      Encode.encodeExpressionToolDescriptionElement includeCwlVersion (CWLExpressionToolDescription(outputs = ResizeArray(), expression = SchemaSaladString.Literal "$(null)"))
                      Encode.encodeOperationDescriptionElement includeCwlVersion (CWLOperationDescription(inputs = ResizeArray(), outputs = ResizeArray())) ]
                    |> List.map Encode.writeYaml
                for text in elements true do
                    Expect.stringContains text "cwlVersion: v1.2" "The element writes its version."
                for text in elements false do
                    Expect.isFalse (text.Contains "cwlVersion") "The element takes the version of its document."
                    Expect.stringContains text "class:" "The element still writes its class."
        ]
        testList "Workflow Encode RoundTrip" [
            testCase "workflow encode/decode deterministic & outputSource preserved" <| fun _ ->
                let original = TestObjects.CWL.Workflow.workflowFile
                let (encoded1, d1, d2) = assertDeterministic Encode.encodeWorkflowDescription Decode.decodeWorkflow "Workflow" original
                // Verify outputs have outputSource in both decoded versions
                assertAllOutputsHaveSource d1
                assertAllOutputsHaveSource d2
            testCase "workflow encode/decode preserves step array extended fields" <| fun _ ->
                let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithStepsArrayFile
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded
                let step = (ListEntry.values roundTripped.Steps).[0]
                let stepInput = (WorkflowStep.getInputValues step).[0]
                Expect.equal step.When_ (Some (SchemaSaladString.Literal "$(inputs.input1 != null)")) ""
                Expect.equal stepInput.PickValue (Some FirstNonNull) ""
                Expect.equal stepInput.Doc (Some (DocText.literal "Input docs")) ""
                let defaultEntry =
                    Expect.wantSome stepInput.DefaultValue "Step input default should remain present as structured YAML"
                match defaultEntry.Node.Kind with
                | YamlNodeKind.Mapping _
                | YamlNodeKind.Sequence _ ->
                    Expect.isTrue true ""
                | _ ->
                    Expect.isTrue false (sprintf "Expected structured default, got %s" (YamlNode.show defaultEntry.Node))
            testCase "workflow hints are encoded and preserved" <| fun _ ->
                let original = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowFile
                original.Hints <- Some (ResizeArray [KnownHint StepInputExpressionRequirement])
                let encoded = Encode.encodeWorkflowDescription original
                let decoded = Decode.decodeWorkflow encoded
                Expect.stringContains encoded "hints:" "Workflow hints should be present in encoded output"
                let hints = Expect.wantSome decoded.Hints "Workflow hints should survive roundtrip"
                Expect.equal hints.[0] (KnownHint StepInputExpressionRequirement) ""
            testCase "workflow intent is encoded and preserved" <| fun _ ->
                let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithIntentFile
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded
                Expect.stringContains encoded "intent:" "Workflow intent should be present in encoded output"
                let intent = Expect.wantSome roundTripped.Intent "Workflow intent should survive roundtrip"
                Expect.sequenceEqual intent (literals [ "primary-analysis"; "quality-control" ]) ""
            testCase "workflow direct overflow and nested port overflow roundtrip" <| fun _ ->
                let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithUnknownPortFieldsFile
                DynObj.setProperty "arc:direct note" "workflow note" decoded
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded
                let metadata = Expect.wantSome roundTripped.Metadata "Decoded direct overflow should be available as metadata."
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:direct note" metadata) (Some "workflow note") "Workflow direct DynamicObj overflow should encode."

                let input = (ListEntry.values roundTripped.Inputs) |> Seq.find (fun input -> input.Name = "sample")
                let output = (ListEntry.values roundTripped.Outputs) |> Seq.find (fun output -> output.Name = "result")
                let step = (ListEntry.values roundTripped.Steps) |> Seq.find (fun step -> step.Id = "step1")
                let stepInput = WorkflowStep.getInputValues step |> Seq.find (fun input -> input.Id = "sample")
                let inputBinding = Expect.wantSome input.InputBinding "Input binding should survive roundtrip."

                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:note" input) (Some "keep input note") "Input overflow should roundtrip."
                Expect.equal input.Type_ (Some CWLType.File) "The input type should roundtrip."
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:binding note" inputBinding) (Some "keep input binding note") "Input binding overflow should roundtrip."

                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:note" output) (Some "keep output note") "Output overflow should roundtrip."
                Expect.equal output.Type_ (Some CWLType.File) "The output type should roundtrip."

                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:step note" step) (Some "keep step note") "Workflow step overflow should roundtrip."
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:step input note" stepInput) (Some "keep step input note") "Step input overflow should roundtrip."
            testCase "workflow decimal overflow roundtrips" <| fun _ ->
                let yaml = """cwlVersion: v1.2
class: Workflow
inputs:
  sample:
    type: string
    arc:threshold: 2.5
outputs: {}
steps: {}"""
                let decoded = Decode.decodeWorkflow yaml
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded
                let input = (ListEntry.values roundTripped.Inputs) |> Seq.find (fun input -> input.Name = "sample")
                Expect.equal (DynObj.tryGetTypedPropertyValue<float> "arc:threshold" input) (Some 2.5) "Decimal overflow should roundtrip as a float."
            testCase "workflow singleton sequence overflow roundtrips as a sequence" <| fun _ ->
                let yaml = """cwlVersion: v1.2
class: Workflow
inputs:
  sample:
    type: string
    arc:list: [only]
outputs: {}
steps: {}"""
                let decoded = Decode.decodeWorkflow yaml
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded
                let input = (ListEntry.values roundTripped.Inputs) |> Seq.find (fun input -> input.Name = "sample")
                let items = Expect.wantSome (DynamicObjHelpers.tryGetTypedPropertyValueAsResizeArray<obj> "arc:list" input) "Singleton overflow should roundtrip as a collection."
                Expect.equal items.Count 1 "Singleton overflow sequence should keep its item count."
                Expect.equal (items.[0] :?> string) "only" "Singleton overflow sequence should keep its value."
            testList "PickValueMethod roundtrip" [
                for (pickValueMethod, cwlString) in [
                    FirstNonNull, "first_non_null"
                    TheOnlyNonNull, "the_only_non_null"
                    AllNonNull, "all_non_null"
                ] do
                    testCase cwlString <| fun _ ->
                        let yaml = TestObjects.CWL.Workflow.workflowWithPickValueMethodFile cwlString
                        let decoded = Decode.decodeWorkflow yaml
                        Expect.equal (WorkflowStep.getInputValues (ListEntry.values decoded.Steps).[0]).[0].PickValue (Some pickValueMethod) ""
                        let encoded = Encode.encodeWorkflowDescription decoded
                        Expect.stringContains encoded $"pickValue: {cwlString}" ""
                        let roundTripped = Decode.decodeWorkflow encoded
                        Expect.equal (WorkflowStep.getInputValues (ListEntry.values roundTripped.Steps).[0]).[0].PickValue (Some pickValueMethod) ""
            ]
            testList "ScatterMethod roundtrip" [
                for (scatterMethod, cwlString) in [
                    DotProduct, "dotproduct"
                    NestedCrossProduct, "nested_crossproduct"
                    FlatCrossProduct, "flat_crossproduct"
                ] do
                    testCase cwlString <| fun _ ->
                        let yaml = TestObjects.CWL.Workflow.workflowWithScatterMethodFile cwlString
                        let decoded = Decode.decodeWorkflow yaml
                        Expect.equal (ListEntry.values decoded.Steps).[0].ScatterMethod (Some scatterMethod) ""
                        let encoded = Encode.encodeWorkflowDescription decoded
                        Expect.stringContains encoded $"scatterMethod: {cwlString}" ""
                        let roundTripped = Decode.decodeWorkflow encoded
                        Expect.equal (ListEntry.values roundTripped.Steps).[0].ScatterMethod (Some scatterMethod) ""
            ]
            testCase "structured default with array value roundtrips" <| fun _ ->
                let yaml = TestObjects.CWL.Workflow.workflowWithStructuredArrayDefaultFile
                let decoded = Decode.decodeWorkflow yaml
                let decodedDefault =
                    (WorkflowStep.getInputValues (ListEntry.values decoded.Steps).[0]).[0].DefaultValue
                    |> Option.bind tryYamlScalarSequence
                    |> Option.defaultValue (ResizeArray())
                Expect.sequenceEqual decodedDefault (ResizeArray [|"1"; "2"; "3"|]) ""
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded
                let roundTrippedDefault =
                    (WorkflowStep.getInputValues (ListEntry.values roundTripped.Steps).[0]).[0].DefaultValue
                    |> Option.bind tryYamlScalarSequence
                    |> Option.defaultValue (ResizeArray())
                Expect.sequenceEqual roundTrippedDefault (ResizeArray [|"1"; "2"; "3"|]) ""
            testCase "when expression with embedded quotes roundtrips" <| fun _ ->
                let expression = "$(inputs.name == 'test')"
                let yaml = TestObjects.CWL.Workflow.workflowWithWhenExpressionFile expression
                let decoded = Decode.decodeWorkflow yaml
                Expect.equal (ListEntry.values decoded.Steps).[0].When_ (Some (SchemaSaladString.Literal expression)) ""
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded
                Expect.equal (ListEntry.values roundTripped.Steps).[0].When_ (Some (SchemaSaladString.Literal expression)) ""
            testCase "scatter + scatterMethod together roundtrip" <| fun _ ->
                let yaml = TestObjects.CWL.Workflow.workflowWithScatterAndScatterMethodFile
                let decoded = Decode.decodeWorkflow yaml
                Expect.sequenceEqual (ListEntry.values decoded.Steps).[0].Scatter.Value (literals [ "in1"; "in2" ]) ""
                Expect.equal (ListEntry.values decoded.Steps).[0].ScatterMethod (Some FlatCrossProduct) ""
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded
                Expect.sequenceEqual (ListEntry.values roundTripped.Steps).[0].Scatter.Value (literals [ "in1"; "in2" ]) ""
                Expect.equal (ListEntry.values roundTripped.Steps).[0].ScatterMethod (Some FlatCrossProduct) ""
            testCase "step-level requirements and hints roundtrip" <| fun _ ->
                let yaml = TestObjects.CWL.Workflow.workflowWithStepLevelRequirementsAndHintsFile
                let decoded = Decode.decodeWorkflow yaml
                let decodedStep = (ListEntry.values decoded.Steps).[0]
                let decodedHints = Expect.wantSome decodedStep.Hints "Step hints should decode"
                let decodedRequirements = Expect.wantSome decodedStep.Requirements "Step requirements should decode"
                Expect.equal decodedHints.[0] (KnownHint StepInputExpressionRequirement) ""
                Expect.equal decodedRequirements.[0] (NetworkAccessRequirement (NetworkAccessRequirementValue(true))) ""
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded
                let roundTrippedStep = (ListEntry.values roundTripped.Steps).[0]
                let roundTrippedHints = Expect.wantSome roundTrippedStep.Hints "Step hints should survive roundtrip"
                let roundTrippedRequirements = Expect.wantSome roundTrippedStep.Requirements "Step requirements should survive roundtrip"
                Expect.equal roundTrippedHints.[0] (KnownHint StepInputExpressionRequirement) ""
                Expect.equal roundTrippedRequirements.[0] (NetworkAccessRequirement (NetworkAccessRequirementValue(true))) ""
            testCase "inline ExpressionTool roundtrip preserves expression" <| fun _ ->
                let original = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithMixedToolAndExpressionStepFile
                let encoded = Encode.encodeWorkflowDescription original
                let roundTripped = Decode.decodeWorkflow encoded
                let expressionTool =
                    Expect.wantSome
                        (WorkflowStepRunOps.tryGetExpressionTool (ListEntry.values roundTripped.Steps).[1].Run)
                        "Should roundtrip ExpressionTool run"
                Expect.equal expressionTool.Expression (SchemaSaladString.Literal "$({'out': inputs.y})") "Expression preserved"
            testCase "inline ExpressionTool chain roundtrip" <| fun _ ->
                let original = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithInlineExpressionToolChainFile
                let encoded = Encode.encodeWorkflowDescription original
                let roundTripped = Decode.decodeWorkflow encoded
                Expect.equal (ListEntry.values roundTripped.Steps).Count 3 ""
                for i in 0..2 do
                    let originalTool =
                        Expect.wantSome
                            (WorkflowStepRunOps.tryGetExpressionTool (ListEntry.values original.Steps).[i].Run)
                            $"Original step {i} should be ExpressionTool"
                    let roundTrippedTool =
                        Expect.wantSome
                            (WorkflowStepRunOps.tryGetExpressionTool (ListEntry.values roundTripped.Steps).[i].Run)
                            $"Roundtripped step {i} should be ExpressionTool"
                    Expect.equal roundTrippedTool.Expression originalTool.Expression $"Step {i} expression should be preserved"
                    let originalInputs = originalTool.Inputs |> Option.defaultValue (ResizeArray())
                    let roundTrippedInputs = roundTrippedTool.Inputs |> Option.defaultValue (ResizeArray())
                    Expect.equal roundTrippedInputs.Count originalInputs.Count $"Step {i} input count should match"
                    Expect.equal roundTrippedTool.Outputs.Count originalTool.Outputs.Count $"Step {i} output count should match"
            testCase "inline ExpressionTool multiline expression in workflow step roundtrips" <| fun _ ->
                let yaml = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  out:
    type: string
    outputSource: expr/out
steps:
  expr:
    run:
      cwlVersion: v1.2
      class: ExpressionTool
      requirements:
        - class: InlineJavascriptRequirement
      inputs: {}
      outputs:
        out: string
      expression: |
        ${ return (function() {
          var name = "arc";
          return {"out": name};
        })(); }
    in: {}
    out: [out]"""

                let decoded = Decode.decodeWorkflow yaml
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded

                Expect.stringContains encoded "expression: |" "Expression should encode as block scalar in nested workflow runs."

                let expressionTool =
                    Expect.wantSome
                        (WorkflowStepRunOps.tryGetExpressionTool (ListEntry.values roundTripped.Steps).[0].Run)
                        "Should roundtrip multiline inline ExpressionTool run"

                Expect.stringContains expressionTool.Expression.Value "return" "Expression should decode after roundtrip."
                Expect.stringContains expressionTool.Expression.Value "var name = \"arc\"" "Variable assignment should survive roundtrip."
                Expect.stringContains expressionTool.Expression.Value "{\"out\": name}" "Return object should survive roundtrip."
        ]
    ]

let testWorkflowUnionPortRoundTrip =
    let optionalOf (inner: CWLType) = Union (ResizeArray [ Null; inner ])

    let assertTypesEqual (expected: CWLType) (actual: CWLType) (message: string) =
        Expect.isTrue (CWLType.typesEqual expected actual) $"{message}: expected %A{expected} but got %A{actual}"

    let inputTypeOf (workflow: CWLWorkflowDescription) (name: string) =
        let input = CWLWorkflowDescription.getInputValues workflow |> Seq.find (fun i -> i.Name = name)
        Expect.wantSome input.Type_ $"Input {name} should have a type."

    let outputTypeOf (workflow: CWLWorkflowDescription) (name: string) =
        let output = CWLWorkflowDescription.getOutputValues workflow |> Seq.find (fun o -> o.Name = name)
        Expect.wantSome output.Type_ $"Output {name} should have a type."

    let enumOf (symbols: string list) = Enum (InputEnumSchema(ResizeArray (List.map SchemaSaladString.Literal symbols)))

    testList "Union port types roundtrip" [
        testCase "unions of null with an enum, a record or an array decode, encode and decode again" <| fun _ ->
            let check (workflow: CWLWorkflowDescription) =
                assertTypesEqual (optionalOf (enumOf [ "alpha"; "beta"; "gamma"; "delta" ])) (inputTypeOf workflow "mode_choice") "mode_choice"
                assertTypesEqual (optionalOf (CWLType.Directory)) (inputTypeOf workflow "annotation_directory") "annotation_directory"
                assertTypesEqual (optionalOf (Array (InputArraySchema(CWLType.File)))) (inputTypeOf workflow "sample_files") "sample_files"
                assertTypesEqual (Union (ResizeArray [ Int; Float ])) (inputTypeOf workflow "threshold") "threshold"
                match inputTypeOf workflow "reference_dbs" with
                | Union members ->
                    Expect.equal members.Count 2 "reference_dbs should be a two member union."
                    Expect.equal members.[0] Null "The first member should be null."
                    match members.[1] with
                    | Record schema ->
                        let fields = Expect.wantSome schema.Fields "The record should keep its fields." |> ListEntry.values
                        Expect.sequenceEqual (fields |> Seq.map (fun f -> f.Name)) [ "data_folder"; "db_file" ] "Record fields should keep their order."
                    | other -> failwithf "Expected a record member but got %A" other
                | other -> failwithf "Expected a union but got %A" other
                let flagOf name = (CWLWorkflowDescription.getInputValues workflow |> Seq.find (fun i -> i.Name = name)).Optional
                for name in [ "mode_choice"; "annotation_directory"; "sample_files"; "reference_dbs" ] do
                    Expect.isTrue (flagOf name) (sprintf "%s is a union of null and one type, so it is optional." name)
                Expect.isFalse (flagOf "threshold") "A union without null is not optional."
                assertTypesEqual (optionalOf (CWLType.File)) (outputTypeOf workflow "chosen") "chosen"
                assertTypesEqual (Union (ResizeArray [ CWLType.File; CWLType.Directory ])) (outputTypeOf workflow "summary") "summary"
                let expressionTool = Expect.wantSome (WorkflowStepRunOps.tryGetExpressionTool (ListEntry.values workflow.Steps).[0].Run) "The inline ExpressionTool should decode."
                let toolInput = (Expect.wantSome expressionTool.Inputs "The inline tool should keep its inputs.") |> ListEntry.values |> Seq.find (fun i -> i.Name = "mode_string")
                assertTypesEqual (optionalOf (enumOf [ "alpha"; "delta"; "beta"; "gamma" ])) (Expect.wantSome toolInput.Type_ "Inline input type") "inline mode_string"
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.UnionPorts.typeListWorkflow
            check decoded
            let encoded = Encode.encodeWorkflowDescription decoded
            check (Decode.decodeWorkflow encoded)

        testCase "the encoder writes the null type as a quoted string" <| fun _ ->
            let encoded = Encode.encodeWorkflowDescription (Decode.decodeWorkflow TestObjects.CWL.Workflow.UnionPorts.typeListWorkflow)
            Expect.stringContains encoded "\"null\"" "A plain null would read as a YAML null, so the null type must be quoted."
            Expect.isFalse (encoded.Contains("[null") || encoded.Contains("- null") || encoded.Contains(", null")) "No union may contain an unquoted null."

        testCase "array form ports with unions roundtrip" <| fun _ ->
            let check (workflow: CWLWorkflowDescription) =
                assertTypesEqual CWLType.String (inputTypeOf workflow "NAME_PATTERN") "NAME_PATTERN"
                assertTypesEqual (optionalOf (CWLType.File)) (inputTypeOf workflow "INDEX_FILES") "INDEX_FILES"
                assertTypesEqual (optionalOf (Array (InputArraySchema(Float)))) (inputTypeOf workflow "LIMITS") "LIMITS"
                assertTypesEqual (Union (ResizeArray [ String; Int; Boolean ])) (inputTypeOf workflow "CHOICE") "CHOICE"
                assertTypesEqual (optionalOf (CWLType.File)) (inputTypeOf workflow "MAYBE_LIST") "MAYBE_LIST"
                assertTypesEqual (CWLType.File) (outputTypeOf workflow "RESULT_LIST") "RESULT_LIST"
                assertTypesEqual (optionalOf (CWLType.File)) (outputTypeOf workflow "OPTIONAL_RESULT") "OPTIONAL_RESULT"
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.UnionPorts.arrayFormWorkflow
            check decoded
            check (Decode.decodeWorkflow (Encode.encodeWorkflowDescription decoded))

        testCase "a port written as a bare type list decodes as a union" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: Workflow
inputs:
  flow_style: [null, File]
  block_style:
    - string
    - int
steps: []
outputs:
  result: ["null", File]
"""
            let workflow = Decode.decodeWorkflow yaml
            assertTypesEqual (optionalOf (CWLType.File)) (inputTypeOf workflow "flow_style") "flow_style"
            assertTypesEqual (Union (ResizeArray [ String; Int ])) (inputTypeOf workflow "block_style") "block_style"
            let flagOf name = (CWLWorkflowDescription.getInputValues workflow |> Seq.find (fun i -> i.Name = name)).Optional
            Expect.isTrue (flagOf "flow_style") "A bare list of null and a file is optional."
            Expect.isFalse (flagOf "block_style") "A bare list of two types without null is not optional."
            assertTypesEqual (optionalOf (CWLType.File)) (outputTypeOf workflow "result") "result"
    ]

let testPackedDocuments =
    let fixture = TestObjects.CWL.Workflow.PackedDocument.graphFile
    let graphOf (yaml: string) =
        match Decode.decodeCWLDocument yaml with
        | CWLDocument.Graph graph -> graph
        | other -> failwith $"Expected a packed document but got %A{other}"
    let workflowOf (processingUnit: CWLProcessingUnit) =
        match processingUnit with
        | Workflow workflow -> workflow
        | other -> failwith $"Expected a workflow but got %A{other}"
    let toolOf (processingUnit: CWLProcessingUnit) =
        match processingUnit with
        | CommandLineTool tool -> tool
        | other -> failwith $"Expected a command line tool but got %A{other}"
    testList "Packed documents" [
        testCase "a packed document decodes as a graph of its processes that use the version of the root" <| fun _ ->
            let graph = graphOf fixture
            Expect.equal graph.CWLVersion "v1.2" "The graph has the version of the root."
            Expect.equal graph.Processes.Count 2 "The graph holds two processes."
            let main = workflowOf graph.Processes.[0]
            Expect.equal main.Id (Some "main") "The workflow keeps its id."
            Expect.equal main.CWLVersion "v1.2" "The workflow uses the version of the root."
            let shout = toolOf graph.Processes.[1]
            Expect.equal shout.Id (Some "shout") "The tool keeps its id."
            Expect.equal shout.CWLVersion "v1.2" "The tool uses the version of the root."
        testCase "a step keeps the reference to a process of the graph as its run string" <| fun _ ->
            let main = workflowOf (graphOf fixture).Processes.[0]
            Expect.equal (ListEntry.values main.Steps).[0].Run (RunString "#shout") "The run field stays the reference."
        testCase "the other keys of the root stay on the graph" <| fun _ ->
            let graph = graphOf fixture
            let namespaces = Expect.wantSome (DynObj.tryGetTypedPropertyValue<DynamicObj> "$namespaces" graph) "The graph keeps $namespaces."
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "ex" namespaces) (Some "https://example.org/ns#") "The namespace keeps its IRI."
            Expect.isNone (DynObj.tryGetTypedPropertyValue<obj> "$graph" graph) "The processes are no dynamic property."
        testCase "a graph is written with the version of the root only and reads back" <| fun _ ->
            let graph = graphOf fixture
            let encoded = Encode.encodeCWLDocument (CWLDocument.Graph graph)
            Expect.stringContains encoded "$graph:" "The processes stand under $graph."
            Expect.stringContains encoded "$namespaces:" "The other keys of the root are written."
            Expect.equal (encoded.Split('\n') |> Array.filter (fun line -> line.Contains "cwlVersion") |> Array.length) 1 "Only the root writes a cwlVersion."
            let roundTripped = graphOf encoded
            Expect.equal roundTripped.CWLVersion "v1.2" "The version survives the round trip."
            Expect.equal roundTripped.Processes.Count 2 "Both processes survive the round trip."
            Expect.equal (ListEntry.values (workflowOf roundTripped.Processes.[0]).Steps).[0].Run (RunString "#shout") "The reference survives the round trip."
            Expect.equal (toolOf roundTripped.Processes.[1]).CWLVersion "v1.2" "The tool uses the version of the root again."
            Expect.isSome (DynObj.tryGetTypedPropertyValue<DynamicObj> "$namespaces" roundTripped) "$namespaces survives the round trip."
            Expect.equal (Encode.encodeCWLDocument (CWLDocument.Graph roundTripped)) encoded "A second pass writes the same text."
        testCase "a graph compares by its version, its processes and the other keys of its root" <| fun _ ->
            let namespaced = TestObjects.CWL.Workflow.PackedDocument.namespacedFile
            Expect.equal (graphOf namespaced) (graphOf namespaced) "Two reads of one text are equal."
            Expect.equal (Decode.decodeCWLDocument namespaced) (Decode.decodeCWLDocument namespaced) "The documents are equal too."
            Expect.equal (hash (graphOf namespaced)) (hash (graphOf namespaced)) "Equal graphs hash alike."
            Expect.notEqual (graphOf namespaced) (graphOf (namespaced.Replace("baseCommand: wc", "baseCommand: cat"))) "Another process makes another graph."
            Expect.notEqual (graphOf namespaced) (graphOf (namespaced.Replace("ex:note: kept at the root", "ex:note: changed"))) "Another key of the root makes another graph."
            Expect.notEqual (graphOf namespaced) (graphOf (namespaced.Replace("cwlVersion: v1.2", "cwlVersion: v1.1"))) "Another version makes another graph."
        testCase "a step that runs a process of the graph keeps the reference in the graph and writes the resolved process inline in a plain file" <| fun _ ->
            let graph = graphOf fixture
            let main = workflowOf graph.Processes.[0]
            let shout = toolOf graph.Processes.[1]
            (ListEntry.values main.Steps).[0].ResolvedRun <- { Run = Some (WorkflowStepRunOps.fromTool shout) }
            Expect.equal (graphOf (Encode.encodeCWLGraph graph)) (graphOf fixture) "In the graph the step runs the process by its id."
            let plain = Encode.encodeProcessingUnit (Workflow main)
            Expect.isFalse (plain.Contains "#shout") "The plain file has no reference to the graph."
            match Decode.decodeCWLDocument plain with
            | CWLDocument.Process (Workflow written) ->
                Expect.equal (WorkflowStepRunOps.tryGetTool (ListEntry.values written.Steps).[0].Run |> Option.bind (fun tool -> tool.BaseCommand) |> Option.map Seq.toList) (Some [ (SchemaSaladString.Literal "echo") ]) "The plain file holds the tool inside the step."
            | other -> failwith $"Expected a plain workflow but got %A{other}"
        testCase "a step that runs a process of the graph without a resolved process cannot be written as a plain file" <| fun _ ->
            let main = workflowOf (graphOf fixture).Processes.[0]
            let error = Expect.wantSome (try Encode.encodeProcessingUnit (Workflow main) |> ignore; None with :? System.ArgumentException as error -> Some error) "The plain file cannot refer to the graph."
            Expect.stringContains error.Message "The step shout" "The error names the step."
            Expect.stringContains error.Message "#shout" "The error names the reference."
        testCase "a step that runs a file keeps the reference in a plain file even with a resolved process" <| fun _ ->
            let step = WorkflowStep.fromRunPath(id = "s", in_ = ResizeArray(), out_ = ResizeArray(), runPath = "./tool.cwl")
            step.ResolvedRun <- { Run = Some (WorkflowStepRunOps.fromTool (CWLToolDescription(outputs = ResizeArray(), baseCommand = ResizeArray [ (SchemaSaladString.Literal "echo") ]))) }
            let workflow = CWLWorkflowDescription(steps = ListEntry.ofValues [ step ], inputs = ResizeArray(), outputs = ResizeArray())
            let plain = Encode.encodeProcessingUnit (Workflow workflow)
            Expect.stringContains plain "run: ./tool.cwl" "The file reference stays."
            Expect.isFalse (plain.Contains "baseCommand") "The tool is not inlined."
        testCase "a process is the process main of a graph when the graph finds that object under the id main" <| fun _ ->
            let namespaced = TestObjects.CWL.Workflow.PackedDocument.namespacedFile
            let graph = graphOf namespaced
            let main = workflowOf graph.Processes.[0]
            Expect.isTrue (CWLGraph.isMain (Workflow main) graph) "A new union value around the object of main is the process main."
            Expect.isFalse (CWLGraph.isMain graph.Processes.[1] graph) "Another process of the graph is not."
            Expect.isFalse (CWLGraph.isMain (Workflow (workflowOf (graphOf namespaced).Processes.[0])) graph) "A process with the same content in another graph is another object."
            main.Id <- Some "renamed"
            Expect.isFalse (CWLGraph.isMain (Workflow main) graph) "The graph reads the id on each call, so a process that was renamed is not the process main."
        testCase "the process main of a graph is written with its graph and any other process is written alone" <| fun _ ->
            let namespaced = TestObjects.CWL.Workflow.PackedDocument.namespacedFile
            let graph = graphOf namespaced
            let packed = Encode.encodeProcessingUnitInGraph (Some graph) graph.Processes.[0]
            Expect.equal packed (Encode.encodeCWLGraph graph) "The process main stands for its graph."
            Expect.equal (graphOf packed) (graphOf namespaced) "The other processes and the keys of the root are written as they were read."
            // A plain file holds the processes that its steps run inline, so the steps need their resolved processes.
            let resolved (graph: CWLGraph) =
                for processingUnit in graph.Processes do
                    match processingUnit with
                    | Workflow workflow ->
                        for step in (ListEntry.values workflow.Steps) do
                            match step.Run with
                            | RunString reference when reference.StartsWith "#" ->
                                CWLGraph.tryFindProcess reference graph
                                |> Option.iter (fun target ->
                                    let run =
                                        match target with
                                        | CommandLineTool tool -> WorkflowStepRunOps.fromTool tool
                                        | other -> failwith $"Expected a tool but got %A{other}"
                                    step.ResolvedRun <- { Run = Some run })
                            | _ -> ()
                    | _ -> ()
                graph
            let renamed (id: string option) =
                let fresh = resolved (graphOf namespaced)
                (workflowOf fresh.Processes.[0]).Id <- id
                fresh, fresh.Processes.[0]
            let withoutMain = resolved (graphOf (namespaced.Replace("- id: main", "- id: first")))
            let renamedGraph, renamedMain = renamed (Some "renamed")
            let unnamedGraph, unnamedMain = renamed None
            let cases =
                [ "a new process with the id main", graph, CommandLineTool (CWLToolDescription(outputs = ResizeArray(), baseCommand = ResizeArray [ (SchemaSaladString.Literal "date") ], id = "main"))
                  "another process of the graph", graph, graph.Processes.[1]
                  "the process main after a rename", renamedGraph, renamedMain
                  "the process main after its id was removed", unnamedGraph, unnamedMain
                  "a process of a graph that has no process main", withoutMain, withoutMain.Processes.[0] ]
            for (name, owner, processingUnit) in cases do
                let written = Encode.encodeProcessingUnitInGraph (Some owner) processingUnit
                Expect.equal written (Encode.encodeProcessingUnit processingUnit) $"{name}: the file holds the process alone."
                match Decode.decodeCWLDocument written with
                | CWLDocument.Process _ -> ()
                | other -> failwith $"{name}: expected a plain process but got %A{other}"
            let withoutId = CWLToolDescription(outputs = ResizeArray(), baseCommand = ResizeArray [ (SchemaSaladString.Literal "date") ])
            Encode.encodeProcessingUnitInGraph (Some graph) (CommandLineTool withoutId) |> ignore
            Expect.isNone withoutId.Id "Writing does not give a process an id."
        testCase "a graph with a cycle of run references is a decode error at the run field that closes the cycle" <| fun _ ->
            let cases =
                [ "two workflows that run each other", TestObjects.CWL.Workflow.PackedDocument.cyclicFile, "$graph[1].steps.back.run", "#main, #sub, #main"
                  "a workflow that runs itself", TestObjects.CWL.Workflow.PackedDocument.cyclicFile.Replace("run: \"#sub\"", "run: \"#main\""), "$graph[0].steps.s.run", "#main, #main"
                  "a step of an inline workflow that runs main", TestObjects.CWL.Workflow.PackedDocument.cyclicThroughInlineRunFile, "$graph[0].steps.s.run.steps.t.run", "#main, #main" ]
            for (name, yaml, path, cycle) in cases do
                match decodeError (fun () -> Decode.decodeCWLDocument yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path $"{name}: the error should show the run field."
                    Expect.stringContains error.Message ("cycle of run references (" + cycle + ")") $"{name}: the error should name the cycle."
                    Expect.isSome error.Span $"{name}: the error should have the place in the text."
                | None -> failwith $"{name} must fail"
            let shared = graphOf TestObjects.CWL.Workflow.PackedDocument.sharedProcessFile
            Expect.equal shared.Processes.Count 2 "Two runs of one process are no cycle."
        testCase "a run that names no process of the graph is a decode error at the run field" <| fun _ ->
            let yaml = fixture.Replace("run: \"#shout\"", "run: \"#absent\"")
            match decodeError (fun () -> Decode.decodeCWLDocument yaml |> ignore) with
            | Some error ->
                Expect.equal (DecodeError.pathText error) "$graph[0].steps.shout.run" "The error shows the run field of the step."
                Expect.stringContains error.Message "#absent" "The error names the reference."
                Expect.isSome error.Span "The error has the place in the text."
            | None -> failwith "A reference to a missing process must fail."
        testCase "ids with and without the leading # name the same process" <| fun _ ->
            let withHash = fixture.Replace("- id: main", "- id: \"#main\"").Replace("- id: shout", "- id: \"#shout\"")
            let graph = graphOf withHash
            Expect.equal (workflowOf graph.Processes.[0]).Id (Some "#main") "The id keeps the # as it is written."
            Expect.isSome (CWLGraph.tryFindProcess "main" graph) "main is found without the #."
            Expect.isSome (CWLGraph.tryFindProcess "#shout" graph) "shout is found with the #."
            Expect.isNone (CWLGraph.tryFindProcess "absent" graph) "A missing id finds nothing."
            let plain = graphOf fixture
            Expect.isSome (CWLGraph.tryFindProcess "#main" plain) "An id written without # is found with the #."
            Expect.isSome (CWLGraph.tryFindProcess "shout" plain) "An id written without # is found without it."
        testCase "a process that names another cwlVersion than the root uses the version of the root" <| fun _ ->
            let graph = graphOf TestObjects.CWL.Workflow.PackedDocument.versionsFile
            Expect.equal (workflowOf graph.Processes.[0]).CWLVersion "v1.2" "The same version is accepted."
            Expect.equal (toolOf graph.Processes.[1]).CWLVersion "v1.2" "The version of the root wins over v1.0."
        testCase "the cwlVersion of a process of a packed document changes nothing whatever it holds" <| fun _ ->
            let head = """cwlVersion: v1.2
$graph:
  - id: main
    class: CommandLineTool
    cwlVersion:"""
            let tail = """
    baseCommand: echo
    inputs: {}
    outputs: {}"""
            for (name, field) in [ "no value", ""; "an older version", " v1.0"; "a draft", " draft-3"; "the version itself", " v1.2"; "a newer version", " v1.3"; "a text that is no version", " banana" ] do
                Expect.equal (toolOf (graphOf (head + field + tail)).Processes.[0]).CWLVersion "v1.2" $"{name}: the process uses the version of the root."
        testCase "a process of a packed document without an id or with an empty id decodes, and the id is written as it was" <| fun _ ->
            let yaml = """cwlVersion: v1.2
$graph:
  - id: main
    class: CommandLineTool
    baseCommand: echo
    inputs: {}
    outputs: {}
  - class: CommandLineTool
    baseCommand: cat
    inputs: {}
    outputs: {}
  - id: ""
    class: CommandLineTool
    baseCommand: ls
    inputs: {}
    outputs: {}"""
            let graph = graphOf yaml
            Expect.equal (graph.Processes |> Seq.map CWLProcessingUnit.tryGetId |> List.ofSeq) [ Some "main"; None; Some "" ] "The second process has no id and the third has the empty one."
            let written = Encode.encodeCWLDocument (CWLDocument.Graph graph)
            Expect.equal (graphOf written) graph "The written text should decode to the same graph."
            Expect.equal (CWLProcessingUnit.tryGetId (graphOf written).Processes.[1]) None "The process without an id stays without one."
        testCase "a packed document with a class at the root is a decode error at the root" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: Workflow
$graph:
  - id: main
    class: CommandLineTool
    baseCommand: echo
    inputs: {}
    outputs: {}"""
            match decodeError (fun () -> Decode.decodeCWLDocument yaml |> ignore) with
            | Some error ->
                Expect.equal (DecodeError.pathText error) "" "The error stands at the root."
                Expect.stringContains error.Message "no class field" "The error names the field."
                Expect.isSome error.Span "The error has the place in the text."
            | None -> failwith "A class at the root of a packed document must fail."
        testCase "two processes of a packed document with one id decode with a warning at the second and are written back" <| fun _ ->
            let cases =
                [ "the same id", TestObjects.CWL.Workflow.PackedDocument.duplicateIdsFile
                  "the same id with and without the #", TestObjects.CWL.Workflow.PackedDocument.duplicateIdsWithHashFile ]
            for (name, yaml) in cases do
                let decoded = Decode.decodeCWLDocumentWithWarnings yaml
                let graph = match decoded.Value with CWLDocument.Graph graph -> graph | other -> failwith $"Expected a packed document but got %A{other}"
                Expect.equal graph.Processes.Count 2 $"{name}: both processes stay in the graph."
                Expect.equal decoded.Warnings.Count 1 $"{name}: the repeated id is one warning."
                Expect.equal decoded.Warnings.[0].Path "$graph[1]" $"{name}: the warning should show the second process."
                Expect.stringContains decoded.Warnings.[0].Message "The id main" $"{name}: the warning should name the id."
                Expect.isSome decoded.Warnings.[0].Raw $"{name}: the warning should show the process."
                Expect.equal (CWLGraph.tryFindMain graph |> Option.map (fun processingUnit -> processingUnit = graph.Processes.[0])) (Some true) $"{name}: a runner starts with the first process."
                Expect.equal (CWLGraph.tryFindProcess "main" graph |> Option.map (fun processingUnit -> processingUnit = graph.Processes.[1])) (Some true) $"{name}: a run of the form #main reaches the last process."
                Expect.equal (graphOf (Encode.encodeCWLDocument decoded.Value)) graph $"{name}: the written text should decode to the same graph."
        testCase "a $graph that is no list is a decode error that names the kind of the value" <| fun _ ->
            let mapping = """cwlVersion: v1.2
$graph:
  id: main"""
            let text = """cwlVersion: v1.2
$graph: main"""
            let cases = [ "a mapping", mapping, "found a mapping."; "a text", text, "found the text \"main\"." ]
            for (name, yaml, message) in cases do
                match decodeError (fun () -> Decode.decodeCWLDocument yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) "$graph" $"{name}: the error should show the field."
                    Expect.stringContains error.Message message $"{name}: the error should say what it found."
                | None -> failwith $"{name} must fail"
        testCase "a type is named through the id of another process of the graph" <| fun _ ->
            let graph = graphOf TestObjects.CWL.Workflow.PackedDocument.typesFile
            let place = toolOf graph.Processes.[1]
            Expect.equal (ListEntry.values place.Inputs.Value).[0].Type_ (Some (Reference "#main/Point")) "The tool names the type of the workflow."
            let main = workflowOf graph.Processes.[0]
            Expect.equal (ListEntry.values main.Inputs).[0].Type_ (Some (Reference "#main/Point")) "The workflow names its own type the same way."
        testCase "a document without a graph decodes as a single process" <| fun _ ->
            let yaml = TestObjects.CWL.CommandLineTool.StreamTypes.cwlFile
            match Decode.decodeCWLDocument yaml with
            | CWLDocument.Process (CommandLineTool tool) -> Expect.equal tool.CWLVersion "v1.2" "The process keeps its version."
            | other -> failwith $"Expected a command line tool but got %A{other}"
            Expect.equal (Encode.encodeCWLDocument (Decode.decodeCWLDocument yaml)) (Encode.encodeProcessingUnit (Decode.decodeCWLProcessingUnit yaml)) "A process is written as before."
        testCase "a packed document is a decode error at the root when it is read as one process" <| fun _ ->
            match decodeError (fun () -> Decode.decodeCWLProcessingUnit fixture |> ignore) with
            | Some error ->
                Expect.equal (DecodeError.pathText error) "" "The error stands at the root."
                Expect.stringContains error.Message "decodeCWLDocument" "The error names the function that reads the document."
                Expect.isSome error.Span "The error has the place in the text."
            | None -> failwith "A packed document has no process at its root."
        testCase "a reference by #id in a document without a graph stays a run string" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  s:
    run: "#tool"
    in: {}
    out: []"""
            Expect.equal (ListEntry.values (Decode.decodeWorkflow yaml).Steps).[0].Run (RunString "#tool") "Nothing names a process, so the reference is kept."
    ]

let testReachabilityInPackedDocuments =
    let tool = [ "  - id: tool"; "    class: CommandLineTool"; "    baseCommand: echo"; "    inputs: []"; "    outputs: []" ]
    let mainTool = [ "  - id: main"; "    class: CommandLineTool"; "    baseCommand: echo"; "    inputs: []"; "    outputs: []" ]
    let workflow (id: string) (stepLines: string list) =
        [ "  - id: " + id; "    class: Workflow"; "    requirements:"; "      SubworkflowFeatureRequirement: {}"; "    inputs: []"; "    outputs: []"; "    steps:"; "      s:" ] @ (stepLines |> List.map (fun line -> "        " + line))
    let graph (members: string list list) = String.concat "\n" ([ "cwlVersion: v1.2"; "$graph:" ] @ List.concat members @ [ "" ])
    let runTool = [ "run: \"#tool\""; "in: {}"; "out: [nothing]" ]
    let failsAt (name: string) (yaml: string) (path: string) (message: string) =
        match decodeError (fun () -> Decode.decodeCWLDocument yaml |> ignore) with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the step" name)
            Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        | None -> failwith (sprintf "%s must fail" name)
    let expectReadsBack (name: string) (yaml: string) =
        let decoded = Decode.decodeCWLDocument yaml
        let written = Encode.encodeCWLDocument decoded
        Expect.equal (Decode.decodeCWLDocument written) decoded (sprintf "%s: the written text should decode to the same document." name)
    testList "Reachability of the checks in a packed document" [
        testCase "a step of a process that the process main does not reach is not checked against the process that it runs" <| fun _ ->
            let unreachable = graph [ mainTool; tool; workflow "other" runTool ]
            expectReadsBack "unreachable" unreachable
            let reached = graph [ workflow "main" [ "run: \"#other\""; "in: {}"; "out: []" ]; tool; workflow "other" runTool ]
            failsAt "reached through another process" reached "$graph[2].steps.s.out[0]" "has no output of that name"
            let direct = graph [ workflow "main" runTool; tool ]
            failsAt "reached by main" direct "$graph[0].steps.s.out[0]" "has no output of that name"
        testCase "a step that runs a process of the document by an id that nothing defines is an error where the process main reaches it" <| fun _ ->
            let missing = [ "run: \"#nothing\""; "in: {}"; "out: []" ]
            expectReadsBack "unreachable" (graph [ mainTool; workflow "other" missing ])
            failsAt "reached" (graph [ workflow "main" missing ]) "$graph[0].steps.s.run" "which is no process of the $graph"
        testCase "processes that run each other are an error where the process main reaches them and a document otherwise" <| fun _ ->
            let toB = [ "run: \"#b\""; "in: {}"; "out: []" ]
            let toA = [ "run: \"#a\""; "in: {}"; "out: []" ]
            expectReadsBack "unreachable" (graph [ mainTool; workflow "a" toB; workflow "b" toA ])
            failsAt "reached" (graph [ workflow "main" toB; workflow "a" toB; workflow "b" toA ]) "$graph[1].steps.s.run" "closes a cycle of run references"
        testCase "the process that cwltool runs is main, or the only process of the document, and a document with several processes and no main has none" <| fun _ ->
            expectReadsBack "no main" (graph [ tool; workflow "other" runTool ])
            failsAt "the only process" (graph [ workflow "other" [ "run:"; "  class: CommandLineTool"; "  baseCommand: echo"; "  inputs: []"; "  outputs: []"; "in: {}"; "out: [nothing]" ] ]) "$graph[0].steps.s.out[0]" "has no output of that name"
            failsAt "the inline process of main" (graph [ workflow "main" [ "run:"; "  class: CommandLineTool"; "  baseCommand: echo"; "  inputs: []"; "  outputs: []"; "in: {}"; "out: [nothing]" ] ]) "$graph[0].steps.s.out[0]" "has no output of that name"
            expectReadsBack "unreachable inline process" (graph [ mainTool; workflow "other" [ "run:"; "  class: CommandLineTool"; "  baseCommand: echo"; "  inputs: []"; "  outputs: []"; "in: {}"; "out: [nothing]" ] ])
        testCase "a run of the form #id reaches the last process with the id and a runner starts with the first process main" <| fun _ ->
            let toolWith (output: string) = [ "  - id: tool"; "    class: CommandLineTool"; "    baseCommand: echo"; "    inputs: []"; "    outputs:"; "      " + output + ": stdout" ]
            let runsTool (out: string) = workflow "main" [ "run: \"#tool\""; "in: {}"; "out: [" + out + "]" ]
            failsAt "the first of two tools" (graph [ toolWith "a"; toolWith "b"; runsTool "a" ]) "$graph[2].steps.s.out[0]" "has no output of that name"
            expectReadsBack "the last of two tools" (graph [ toolWith "a"; toolWith "b"; runsTool "b" ])
            let badWorkflow = workflow "main" runTool
            failsAt "the first of two process mains" (graph [ badWorkflow; tool; mainTool ]) "$graph[0].steps.s.out[0]" "has no output of that name"
            expectReadsBack "the last of two process mains" (graph [ mainTool; tool; badWorkflow ])
        testCase "a repeated id is a warning that says which entry a reference finds" <| fun _ ->
            let decoded = Decode.decodeCWLDocumentWithWarnings (graph [ mainTool; tool; tool ])
            Expect.equal decoded.Warnings.Count 1 "The repeated id is one warning."
            Expect.stringContains decoded.Warnings.[0].Message "A reference to the id finds the last entry." "The warning says which entry stands for the id."
    ]

let main =
    testList "CWLWorkflowDescription" [
        testCWLWorkflowDescriptionDecode
        testCWLWorkflowDescriptionEncode
        testWorkflowUnionPortRoundTrip
        testPackedDocuments
        testReachabilityInPackedDocuments
        testImportedRun
        testImportedEntries
        testImportedStepPorts
        testStringLists
    ]
