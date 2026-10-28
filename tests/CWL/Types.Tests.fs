/// The types of CWL in ports and schemas: nested arrays, unions, Any, stream types, port schemas, the array shorthand,
/// File and Directory, Dirent, enum symbols.
module Tests.Types

open ARCtrl.CWL
open TestingUtils
open DynamicObj
open TestingUtils.CWL
open YAMLicious

let testNestedArrayDecoding =
    testList "Nested Array Decoding" [
        testCase "Decode nested array (array of arrays)" <| fun _ ->
            let cwlFile =
                Kit.tool
                    [ "baseCommand: echo"
                      "inputs:"
                      "  IndexInput: File[]"
                      "  sampleRecordFiles:"
                      "    type:"
                      "      type: array"
                      "      items:"
                      "        type: array"
                      "        items: File"
                      "outputs: []" ]
            let decoded = Decode.decodeCommandLineTool cwlFile
            let inputs = (ListEntry.values decoded.Inputs.Value)
            
            // Check IndexInput is File[]
            let indexInput = inputs |> Seq.find (fun i -> i.Name = "IndexInput")
            Expect.isTrue (
                match indexInput.Type_.Value with
                | Array arraySchema -> 
                    match arraySchema.Items with
                    | File -> true
                    | _ -> false
                | _ -> false
            ) "IndexInput should be File[]"
            
            // Check sampleRecordFiles is File[][] - nested Array with Array items
            let sampleRecordFiles = inputs |> Seq.find (fun i -> i.Name = "sampleRecordFiles")
            Expect.isTrue (
                match sampleRecordFiles.Type_.Value with
                | Array outerArraySchema ->
                    match outerArraySchema.Items with
                    | Array innerArraySchema ->
                        match innerArraySchema.Items with
                        | File -> true
                        | _ -> false
                    | _ -> false
                | _ -> false
            ) "sampleRecordFiles should be File[][]"
    ]


let testUnionArrayItemDecoding =
    let expectedMixedInputType =
        Array (InputArraySchema(Union (ResizeArray [String; File])))

    let assertMixedInputType (decoded: CWLToolDescription) =
        let inputs = ListEntry.values (Expect.wantSome decoded.Inputs "Union-array-items fixture should decode inputs.")
        let mixedInput = inputs |> Seq.find (fun input -> input.Name = "mixed_input")
        Expect.equal mixedInput.Type_.Value expectedMixedInputType "mixed_input should decode as array of [string, File]."

    testList "Union Array Items" [
        testCase "Decode array items union in flow style" <| fun _ ->
            let flowStyleCwlFile =
                Kit.tool
                    [ "baseCommand: echo"
                      "inputs:"
                      "  mixed_input:"
                      "    type:"
                      "      type: array"
                      "      items: [string, File]"
                      "outputs: {}" ]
            let decoded = Decode.decodeCommandLineTool flowStyleCwlFile
            assertMixedInputType decoded
        testCase "Decode array items union in block style" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.UnionArrayItems.blockStyleCwlFile
            assertMixedInputType decoded
        testCase "array items union roundtrips through encoder" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.UnionArrayItems.blockStyleCwlFile
            let roundTripped, encoded = Kit.rewrite "the round trip" Decode.decodeCommandLineTool Encode.encodeToolDescription decoded
            Expect.stringContains encoded "items: [string, File]" "Encoder should normalize union-valued items to flow style."
            assertMixedInputType roundTripped
    ]


let testToolUnionPorts =
    let cwlFile =
        Kit.tool
            [ "baseCommand: echo"
              "inputs:"
              "  mode_choice:"
              "    type:"
              "      - \"null\""
              "      - type: enum"
              "        symbols: [\"alpha\", \"beta\"]"
              "    label: Mode choice"
              "  optional_dir:"
              "    type: Directory?"
              "  flow_union: [\"null\", File]"
              "  block_union:"
              "    - string"
              "    - int"
              "  array_or_null:"
              "    type:"
              "      - \"null\""
              "      - type: array"
              "        items: [string, File]"
              "outputs:"
              "  result:"
              "    type: [File, Directory]"
              "    outputBinding:"
              "      glob: \"*.out\""
              "  maybe: [\"null\", File]" ]
    let tool = cwlFile

    let assertTypesEqual (expected: CWLType) (actual: CWLType) (message: string) =
        Expect.isTrue (CWLType.typesEqual expected actual) $"{message}: expected %A{expected} but got %A{actual}"

    let check (decoded: CWLToolDescription) =
        let inputs = ListEntry.values (Expect.wantSome decoded.Inputs "Inputs should decode.")
        let typeOf name = (Expect.wantSome (inputs |> Seq.find (fun i -> i.Name = name)).Type_ $"{name} should have a type")
        let optionalOf inner = Union (ResizeArray [ Null; inner ])
        assertTypesEqual (optionalOf (Enum (InputEnumSchema(ResizeArray [ SchemaSaladString.Literal "alpha"; SchemaSaladString.Literal "beta" ])))) (typeOf "mode_choice") "mode_choice"
        assertTypesEqual (optionalOf (CWLType.Directory)) (typeOf "optional_dir") "optional_dir"
        assertTypesEqual (optionalOf (CWLType.File)) (typeOf "flow_union") "flow_union"
        assertTypesEqual (Union (ResizeArray [ String; Int ])) (typeOf "block_union") "block_union"
        assertTypesEqual (optionalOf (Array (InputArraySchema(Union (ResizeArray [ String; CWLType.File ]))))) (typeOf "array_or_null") "array_or_null"
        let flagOf name = (inputs |> Seq.find (fun i -> i.Name = name)).Optional
        for name in [ "mode_choice"; "optional_dir"; "flow_union"; "array_or_null" ] do
            Expect.isTrue (flagOf name) (sprintf "%s is a union of null and one type, so it is optional." name)
        Expect.isFalse (flagOf "block_union") "A union without null is not optional."
        let outputTypeOf name = Expect.wantSome ((ListEntry.values decoded.Outputs) |> Seq.find (fun o -> o.Name = name)).Type_ $"{name} should have a type"
        assertTypesEqual (Union (ResizeArray [ CWLType.File; CWLType.Directory ])) (outputTypeOf "result") "result"
        assertTypesEqual (optionalOf (CWLType.File)) (outputTypeOf "maybe") "maybe"

    testList "Union port types" [
        testCase "unions in object form and in bare list form decode and read back" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool tool
            check decoded
            check (fst (Kit.rewrite "unions" Decode.decodeCommandLineTool Encode.encodeToolDescription decoded))
        testCase "a bare type list with null is an optional union and one without null is not, in a tool and in a workflow, and reads back" <| fun _ ->
            let ports = [ "inputs:"; "  x: [\"null\", File]"; "  y:"; "    type: [\"null\", File]"; "    label: y"; "  z:"; "    - string"; "    - int"; "outputs:"; "  o: [\"null\", File]" ]
            let expect (name: string) (inputs: ResizeArray<CWLInput>) (outputs: ResizeArray<CWLOutput>) =
                for (port, expected, optional) in [ "x", Union (ResizeArray [ Null; CWLType.File ]), true; "y", Union (ResizeArray [ Null; CWLType.File ]), true; "z", Union (ResizeArray [ String; Int ]), false ] do
                    let input = inputs |> Seq.find (fun i -> i.Name = port)
                    Expect.equal input.Type_ (Some expected) (sprintf "%s: the type of %s" name port)
                    Expect.equal input.Optional optional (sprintf "%s: whether %s is optional" name port)
                Expect.equal (outputs |> Seq.find (fun o -> o.Name = "o")).Type_ (Some (Union (ResizeArray [ Null; CWLType.File ]))) (sprintf "%s: the type of the output" name)
            let decodedTool, _ = Kit.roundTrip "the tool" Decode.decodeCommandLineTool Encode.encodeToolDescription (Kit.tool ("baseCommand: echo" :: ports))
            expect "the tool" (ListEntry.values decodedTool.Inputs.Value) (ListEntry.values decodedTool.Outputs)
            let decodedWorkflow, _ = Kit.roundTrip "the workflow" Decode.decodeWorkflow Encode.encodeWorkflowDescription (Kit.workflow (ports @ [ "steps: []" ]))
            expect "the workflow" (ListEntry.values decodedWorkflow.Inputs) (ListEntry.values decodedWorkflow.Outputs)
        testCase "a type list with the null behind the type keeps its order" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs:
  x: [string, "null"]
outputs:
  z: [File, "null"]
"""
            let decoded = Decode.decodeCommandLineTool yaml
            Expect.equal (ListEntry.values decoded.Inputs.Value).[0].Type_ (Some (Union (ResizeArray [ String; Null ]))) "The input type keeps the order of the list."
            Expect.equal (ListEntry.values decoded.Outputs).[0].Type_ (Some (Union (ResizeArray [ CWLType.File; Null ]))) "The output type keeps the order of the list."
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded """x: [string, "null"]""" "The input is written as a list."
            Expect.stringContains encoded """z: [File, "null"]""" "The output is written as a list."
            let roundTripped = Decode.decodeCommandLineTool encoded
            Expect.equal (ListEntry.values roundTripped.Inputs.Value).[0].Type_ (ListEntry.values decoded.Inputs.Value).[0].Type_ "The input type survives the round trip."
            Expect.equal (ListEntry.values roundTripped.Outputs).[0].Type_ (ListEntry.values decoded.Outputs).[0].Type_ "The output type survives the round trip."
        testCase "a type list with the null in front of the type is written as the shorthand" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs:
  x: ["null", string]
outputs:
  z: ["null", File]
"""
            let decoded = Decode.decodeCommandLineTool yaml
            Expect.equal (ListEntry.values decoded.Inputs.Value).[0].Type_ (Some (Union (ResizeArray [ Null; String ]))) "The input type keeps the order of the list."
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded "x: string?" "The input is written as the shorthand."
            Expect.stringContains encoded "z: File?" "The output is written as the shorthand."
            let roundTripped = Decode.decodeCommandLineTool encoded
            Expect.equal (ListEntry.values roundTripped.Inputs.Value).[0].Type_ (ListEntry.values decoded.Inputs.Value).[0].Type_ "The input type survives the round trip."
            Expect.equal (ListEntry.values roundTripped.Outputs).[0].Type_ (ListEntry.values decoded.Outputs).[0].Type_ "The output type survives the round trip."
    ]


let testAnyType =
    let expectedInputTypes =
        [ "anything", Any
          "maybe_anything", Union (ResizeArray [ Null; Any ])
          "anything_list", Array (InputArraySchema(Any))
          "anything_or_text", Union (ResizeArray [ Any; String ])
          "listed", Array (InputArraySchema(Any)) ]
    let expectedOutputTypes =
        [ "anything_out", Any
          "maybe_out", Union (ResizeArray [ Null; Any ])
          "list_out", Array (InputArraySchema(Any)) ]
    let expectTypes (operation: CWLOperationDescription) =
        for (name, expected) in expectedInputTypes do
            let input = (ListEntry.values operation.Inputs) |> Seq.find (fun i -> i.Name = name)
            Expect.equal input.Type_ (Some expected) $"The input {name} should have the type {expected}."
        for (name, expected) in expectedOutputTypes do
            let output = (ListEntry.values operation.Outputs) |> Seq.find (fun o -> o.Name = name)
            Expect.equal output.Type_ (Some expected) $"The output {name} should have the type {expected}."

    testList "Any type" [
        testCase "Any decodes as a type name, in an optional, in an array, in a union and under items" <| fun _ ->
            expectTypes (Decode.decodeOperation TestObjects.CWL.Operation.AnyType.cwlFile)
        testCase "Any is written as the shorthand and reads back" <| fun _ ->
            let encoded = Encode.encodeOperationDescription (Decode.decodeOperation TestObjects.CWL.Operation.AnyType.cwlFile)
            Expect.stringContains encoded "maybe_anything: Any?" "An optional Any is written as the shorthand."
            Expect.stringContains encoded "anything_list: Any[]" "An array of Any is written as the shorthand."
            Expect.stringContains encoded "anything_or_text: [Any, string]" "A union with Any is a list."
            Expect.stringContains encoded "listed: Any[]" "An array schema with Any items is written as the shorthand."
            expectTypes (Decode.decodeOperation encoded)
        testCase "Any equals Any and differs from the other types" <| fun _ ->
            Expect.equal Any Any "Any is equal to itself."
            Expect.equal (hash Any) (hash Any) "Any hashes to the same value each time."
            Expect.notEqual Any String "Any is no string."
            Expect.notEqual (Union (ResizeArray [ Null; Any ])) (Union (ResizeArray [ Null; String ])) "The members of a union count."
        testCase "Any decodes under the items of an array schema that stands under items" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: Operation
inputs:
  nested:
    type:
      type: array
      items:
        type: array
        items: Any
outputs: {}
"""
            let operation = Decode.decodeOperation yaml
            Expect.equal (ListEntry.values operation.Inputs).[0].Type_ (Some (Array (InputArraySchema(Array (InputArraySchema(Any)))))) "An array schema may hold an array schema of Any."
    ]


let testStreamTypes =
    let tool (ports: string) = Kit.tool [ "baseCommand: cat" ] + ports
    let portType (ports: ResizeArray<'port>) (nameOf: 'port -> string) (typeOf: 'port -> CWLType option) (name: string) =
        Expect.wantSome (typeOf (ports |> Seq.find (fun port -> nameOf port = name))) $"{name} should have a type."

    testList "Stream types" [
        testCase "stdout, stderr and stdin decode as the whole type of a port of a command line tool" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.StreamTypes.cwlFile
            Expect.equal (portType (ListEntry.values decoded.Inputs.Value) (fun i -> i.Name) (fun i -> i.Type_) "unsorted") Stdin "The input has the type stdin."
            Expect.equal (portType (ListEntry.values decoded.Outputs) (fun o -> o.Name) (fun o -> o.Type_) "sorted") Stdout "The output has the type stdout."
            Expect.equal (portType (ListEntry.values decoded.Outputs) (fun o -> o.Name) (fun o -> o.Type_) "log") Stderr "The output in object form has the type stderr."
            Expect.isFalse (ListEntry.values decoded.Inputs.Value).[0].Optional "A stream type is no optional type."
        testCase "a stream type in object form decodes as the whole type" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool (tool """inputs:
  lines:
    type: stdin
    label: Lines
outputs:
  out:
    type: stdout""")
            Expect.equal (ListEntry.values decoded.Inputs.Value).[0].Type_ (Some Stdin) "The input has the type stdin."
            Expect.equal (ListEntry.values decoded.Outputs).[0].Type_ (Some Stdout) "The output has the type stdout."
        testCase "stream types are written as type names and read back" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.StreamTypes.cwlFile
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded "unsorted: stdin" "The input is written as the type name."
            Expect.stringContains encoded "sorted: stdout" "The output is written as the type name."
            Expect.stringContains encoded "type: stderr" "The output in object form keeps its object."
            let roundTripped = Decode.decodeCommandLineTool encoded
            Expect.equal (portType (ListEntry.values roundTripped.Inputs.Value) (fun i -> i.Name) (fun i -> i.Type_) "unsorted") Stdin "The input survives the round trip."
            Expect.equal (portType (ListEntry.values roundTripped.Outputs) (fun o -> o.Name) (fun o -> o.Type_) "sorted") Stdout "The stdout output survives the round trip."
            Expect.equal (portType (ListEntry.values roundTripped.Outputs) (fun o -> o.Name) (fun o -> o.Type_) "log") Stderr "The stderr output survives the round trip."
        testCase "the inline command line tool of a workflow admits stream types" <| fun _ ->
            let workflowWithInlineTool =
                Kit.workflow
                    [ "inputs: {}"
                      "outputs:"
                      "  result:"
                      "    type: File"
                      "    outputSource: sort/sorted"
                      "steps:"
                      "  sort:"
                      "    run:"
                      "      class: CommandLineTool"
                      "      baseCommand: sort"
                      "      inputs: {}"
                      "      outputs:"
                      "        sorted: stdout"
                      "    in: {}"
                      "    out: [sorted]" ]
            let decoded = Decode.decodeWorkflow workflowWithInlineTool
            let inlineTool = Expect.wantSome (WorkflowStepRunOps.tryGetTool (ListEntry.values decoded.Steps).[0].Run) "The step runs an inline tool."
            Expect.equal (ListEntry.values inlineTool.Outputs).[0].Type_ (Some Stdout) "The output has the type stdout."
        testCase "a stream type anywhere but as the whole type of a port is a decode error at its place" <| fun _ ->
            let cases =
                [ "an optional stdout", "outputs.x", tool """inputs: {}
outputs:
  x: stdout?"""
                  "an array of stderr", "outputs.x", tool """inputs: {}
outputs:
  x: stderr[]"""
                  "stdout in a type list", "outputs.x[0]", tool """inputs: {}
outputs:
  x: [stdout, "null"]"""
                  "an optional stdout in a type field", "outputs.x.type", tool """inputs: {}
outputs:
  x:
    type: stdout?"""
                  "stdout under items", "outputs.x.type.items", tool """inputs: {}
outputs:
  x:
    type:
      type: array
      items: stdout"""
                  "stdout as the type of a record field", "outputs.x.type.fields.f", tool """inputs: {}
outputs:
  x:
    type:
      type: record
      fields:
        f: stdout"""
                  "stdin as the type of an output", "outputs.x", tool """inputs: {}
outputs:
  x: stdin"""
                  "stdout as the type of an input", "inputs.x", tool """inputs:
  x: stdout
outputs: {}"""
                  "an optional stdin", "inputs.x", tool """inputs:
  x: stdin?
outputs: {}"""
                  "an array of stdin", "inputs.x", tool """inputs:
  x: stdin[]
outputs: {}"""
                  "stdin in a type list", "inputs.x[0]", tool """inputs:
  x: [stdin, string]
outputs: {}""" ]
            for (name, path, yaml) in cases do
                Kit.toolFails name yaml path "is valid only as the whole type of"
        testCase "a stream type with a binding is a decode error at the port" <| fun _ ->
            let cases =
                [ "stdout with an outputBinding", "outputs.x", "The type stdout is valid only for an output without an outputBinding.", tool """inputs: {}
outputs:
  x:
    type: stdout
    outputBinding:
      glob: out.txt"""
                  "stderr with an outputBinding", "outputs.x", "The type stderr is valid only for an output without an outputBinding.", tool """inputs: {}
outputs:
  x:
    type: stderr
    outputBinding:
      glob: err.txt"""
                  "stdin with an inputBinding", "inputs.x", "The type stdin is valid only for an input without an inputBinding.", tool """inputs:
  x:
    type: stdin
    inputBinding:
      position: 1
outputs: {}""" ]
            for (name, path, message, yaml) in cases do
                let actual = Kit.decodeFailsWith name (fun () -> Decode.decodeCommandLineTool yaml) path message
                Expect.equal actual message $"{name}: the error should say what is wrong."
        testCase "a tool that sets stdin and has an input of the type stdin is a decode error at stdin" <| fun _ ->
            let yaml = tool """stdin: input.txt
inputs:
  x: stdin
outputs: {}"""
            Kit.toolFails "Both ways to read the standard input" yaml "stdin" ""
            let withoutInput = tool """stdin: input.txt
inputs: {}
outputs: {}"""
            Expect.equal (Decode.decodeCommandLineTool withoutInput).Stdin (Some (SchemaSaladString.Literal "input.txt")) "The tool field alone is valid."
        testCase "a workflow, an expression tool and an operation admit no stream type" <| fun _ ->
            let cases =
                [ "a workflow output", "outputs.x", """cwlVersion: v1.2
class: Workflow
inputs: {}
steps: {}
outputs:
  x: stdout"""
                  "a workflow input", "inputs.x", """cwlVersion: v1.2
class: Workflow
inputs:
  x: stdin
steps: {}
outputs: {}"""
                  "an expression tool output", "outputs.x", """cwlVersion: v1.2
class: ExpressionTool
inputs: {}
outputs:
  x: stdout
expression: '${ return {}; }'"""
                  "an operation output", "outputs.x", """cwlVersion: v1.2
class: Operation
inputs: {}
outputs:
  x: stderr""" ]
            for (name, path, yaml) in cases do
                Kit.processFails name yaml path "is valid only as the whole type of"
    ]


let testPortSchemaNesting =
    let tool (portLines: string list) = Kit.tool ([ "baseCommand: echo"; "inputs:" ] @ portLines @ [ "outputs: []" ])
    let enumSchema = Enum (InputEnumSchema(ResizeArray [ SchemaSaladString.Literal "fast"; SchemaSaladString.Literal "slow" ]))

    testList "Port schema nesting" [
        testCase "the schema of a port with other fields stands directly under type" <| fun _ ->
            let cwlFile =
                Kit.tool
                    [ "baseCommand: echo"
                      "inputs:"
                      "  mode:"
                      "    type:"
                      "      type: enum"
                      "      symbols: [fast, slow]"
                      "    inputBinding:"
                      "      prefix: --mode"
                      "  samples:"
                      "    label: the samples"
                      "    type:"
                      "      type: array"
                      "      items:"
                      "        type: record"
                      "        fields:"
                      "          name: string"
                      "outputs:"
                      "  summary:"
                      "    doc: the summary"
                      "    type:"
                      "      type: record"
                      "      fields:"
                      "        count: int" ]
            let decoded = Decode.decodeCommandLineTool cwlFile
            let _, encoded = Kit.rewrite "The written text should decode to the same tool." Decode.decodeCommandLineTool Encode.encodeToolDescription decoded
            Expect.stringContains encoded "  mode:\n    type:\n      type: enum\n      symbols: [fast, slow]\n    inputBinding:\n      prefix: --mode\n" "The enum input keeps its schema under type and its binding beside it."
            Expect.stringContains encoded "  summary:\n    type:\n      type: record\n      fields:\n        count:\n          type: int\n    doc: the summary\n" "The record output keeps its schema under type and its doc beside it."
            Expect.stringContains encoded "  samples:\n    type:\n      type: array\n      items:\n        type: record\n" "The array of records keeps its schema under type."
            Expect.isFalse (encoded.Contains "type:\n      type:\n        type:") "No port writes a schema in a second mapping under type."
        testCase "a port with a schema and no other field writes the schema under type" <| fun _ ->
            let hand =
                CWLToolDescription(
                    ListEntry.ofValues (ResizeArray [ CWLOutput("record", Record (InputRecordSchema(fields = ListEntry.ofValues [ InputRecordField("a", CWLType.String) ]))) ]),
                    inputs = ListEntry.ofValues [ CWLInput("mode", enumSchema) ],
                    baseCommand = ResizeArray [ (SchemaSaladString.Literal "echo") ]
                )
            let _, encoded = Kit.rewrite "The hand-built tool should read back as it was built." Decode.decodeCommandLineTool Encode.encodeToolDescription hand
            Expect.stringContains encoded "  mode:\n    type:\n      type: enum\n      symbols: [fast, slow]\n" "The enum input is a port object with the schema under type."
            Expect.stringContains encoded "  record:\n    type:\n      type: record\n      fields:\n        a:\n          type: string\n" "The record output is a port object with the schema under type."
        testCase "a hand-built enum input with a binding reads back equal" <| fun _ ->
            let hand =
                CWLToolDescription(
                    ResizeArray [],
                    inputs = ListEntry.ofValues [ CWLInput("mode", enumSchema, inputBinding = ListEntry.Value (InputBinding.create(prefix = (SchemaSaladString.Literal "--mode")))) ],
                    baseCommand = ResizeArray [ (SchemaSaladString.Literal "echo") ]
                )
            let _, encoded = Kit.rewrite "The written text should read back as the same tool." Decode.decodeCommandLineTool Encode.encodeToolDescription hand
            Expect.stringContains encoded "    type:\n      type: enum\n" "The schema stands under type."
        testCase "a schema under a schema under type is a decode error" <| fun _ ->
            let yaml = tool [ "  mode:"; "    type:"; "      type:"; "        type: enum"; "        symbols: [fast, slow]"; "    inputBinding:"; "      prefix: --mode" ]
            Kit.toolFails "A schema in the type of a schema" yaml "inputs.mode.type.type" "Expected record, enum or array in the type field of a schema"
        testCase "a type list under the type of a schema is a decode error" <| fun _ ->
            let yaml = tool [ "  x:"; "    type:"; "      type: [string, int]" ]
            match Kit.decodeError (fun () -> Decode.decodeCommandLineTool yaml) with
            | Some error -> Expect.equal (DecodeError.pathText error) "inputs.x.type.type" "The error should show the misplaced level."
            | None -> failwith "A type list in the type of a schema must fail."
    ]


let testArrayShorthand =
    let inputOf (name: string) (tool: CWLToolDescription) = (ListEntry.values tool.Inputs.Value) |> Seq.find (fun input -> input.Name = name)

    testList "Array shorthand" [
        testCase "an array with an input binding keeps its binding and the binding of its port" <| fun _ ->
            let withBinding =
                Kit.tool
                    [ "baseCommand: echo"
                      "inputs:"
                      "  files:"
                      "    type:"
                      "      type: array"
                      "      items: File"
                      "      inputBinding:"
                      "        prefix: -I"
                      "    inputBinding:"
                      "      position: 1"
                      "outputs: []" ]
            let decoded = Decode.decodeCommandLineTool withBinding
            let _, encoded = Kit.rewrite "The written text should decode to the same tool." Decode.decodeCommandLineTool Encode.encodeToolDescription decoded
            Expect.stringContains encoded "    type:\n      type: array\n      items: File\n      inputBinding:\n        prefix: -I\n    inputBinding:\n      position: 1\n" "The array stays a schema with its binding, and the port keeps its own."
            Expect.isFalse (encoded.Contains "File[]") "The shorthand would drop the binding of the array."
        testCase "an array with a label, a doc or a name is written as a schema, as a type or in a type list" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.ArraySchemas.withMetadata
            let _, encoded = Kit.rewrite "The written text should decode to the same tool." Decode.decodeCommandLineTool Encode.encodeToolDescription decoded
            Expect.stringContains encoded "  names:\n    - \"null\"\n    - type: array\n      items: string\n      label: names\n      inputBinding:\n        itemSeparator: \",\"\n" "The optional array with a label keeps its schema."
            Expect.stringContains encoded "  counts:\n    type:\n      type: array\n      items: int\n      doc: the counts\n" "The array with a doc keeps its schema."
            Expect.stringContains encoded "  tags:\n    - string\n    - type: array\n      items: string\n      name: Tags\n" "The array with a name keeps its schema in a type list."
            Expect.stringContains encoded "  plain: string[]\n" "A plain array stays the shorthand."
            Expect.stringContains encoded "  maybe: int[]?\n" "A plain optional array stays the shorthand."
        testCase "the schema of an array keeps the fields that the shorthand has no place for" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.ArraySchemas.withMetadata
            match (inputOf "names" decoded).Type_ with
            | Some (Union members) ->
                match members.[1] with
                | Array schema ->
                    Expect.equal schema.Label (Some (SchemaSaladString.Literal "names")) "The label is typed."
                    Expect.isTrue (schema.TryGetPropertyValue "inputBinding" |> Option.isSome) "The binding stays on the schema."
                | other -> failwithf "Expected an array schema, found %A" other
            | other -> failwithf "Expected a type list, found %A" other
        testCase "a hand-built array with a label is not written as the shorthand" <| fun _ ->
            let labelled = InputArraySchema(CWLType.String, label = (SchemaSaladString.Literal "names"))
            let hand =
                CWLToolDescription(
                    ResizeArray [],
                    inputs = ListEntry.ofValues (ResizeArray [ CWLInput("a", Array labelled); CWLInput("b", Union (ResizeArray [ Null; Array labelled ])); CWLInput("c", Array (InputArraySchema(CWLType.String))) ]),
                    baseCommand = ResizeArray [ (SchemaSaladString.Literal "echo") ]
                )
            let _, encoded = Kit.rewrite "The hand-built tool should read back as it was built." Decode.decodeCommandLineTool Encode.encodeToolDescription hand
            Expect.stringContains encoded "  a:\n    type:\n      type: array\n      items: string\n      label: names\n" "The array as the whole type keeps its label."
            Expect.stringContains encoded "  b:\n    - \"null\"\n    - type: array\n      items: string\n      label: names\n" "The optional array keeps its label."
            Expect.stringContains encoded "  c: string[]\n" "The plain array is the shorthand."
        testCase "the text of an additional type needs a schema for an array with a label" <| fun _ ->
            let labelled = Array (InputArraySchema(CWLType.File, label = (SchemaSaladString.Literal "files")))
            Expect.isTrue (Encode.isComplexType labelled) "An array with a label has no shorthand."
            Expect.isTrue (Encode.isComplexType (Union (ResizeArray [ Null; labelled ]))) "An optional array with a label has no shorthand."
            Expect.isFalse (Encode.isComplexType (Array (InputArraySchema(CWLType.File)))) "A plain array has the shorthand."
            Expect.equal ((Encode.encodeCWLTypeShorthand (Array (InputArraySchema(Array (InputArraySchema(CWLType.File))))) |> Encode.writeYaml).Trim()) "File[][]" "A plain array of arrays has the nested shorthand."
    ]


let testFileAndDirectoryTypes =
    let tool (portLines: string list) = Kit.tool ([ "baseCommand: echo"; "inputs:" ] @ portLines @ [ "outputs: []" ])

    testList "File and Directory types" [
        testCase "the format and the secondaryFiles of an optional File stay on the port and the type round trips equal" <| fun _ ->
            let yaml = tool [ "  reads:"; "    type: File?"; "    format: edam:format_2330"; "    secondaryFiles: .bai"; "  dir: Directory?" ]
            let decoded = Decode.decodeCommandLineTool yaml
            let reads = (ListEntry.values decoded.Inputs.Value).[0]
            Expect.equal reads.Type_ (Some (Union (ResizeArray [ Null; CWLType.File ]))) "The type is an optional File."
            Expect.equal reads.Format (Some (InputFormat.Single (SchemaSaladString.Literal "edam:format_2330"))) "The format belongs to the port."
            Expect.isSome reads.SecondaryFiles "The secondaryFiles belong to the port."
            Expect.equal (ListEntry.values decoded.Inputs.Value).[1].Type_ (Some (Union (ResizeArray [ Null; CWLType.Directory ]))) "The type is an optional Directory."
            let _, encoded = Kit.rewrite "The written text should decode to the same tool." Decode.decodeCommandLineTool Encode.encodeToolDescription decoded
            Expect.stringContains encoded "  reads:\n    type: File?\n    secondaryFiles: .bai\n    format: edam:format_2330\n  dir: Directory?\n" "The port keeps its fields and the types are the shorthand."
        testCase "a File and a Directory are equal to themselves and to nothing else" <| fun _ ->
            Expect.equal CWLType.File CWLType.File "A File equals a File."
            Expect.equal CWLType.Directory CWLType.Directory "A Directory equals a Directory."
            Expect.notEqual CWLType.File CWLType.Directory "A File is no Directory."
            Expect.equal (hash CWLType.File) (hash CWLType.File) "Equal types hash alike."
    ]


let testDirentIsNoType =
    testList "Dirent is no type" [
        testCase "a Dirent stays an entry of the listing of an InitialWorkDirRequirement" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
requirements:
  - class: InitialWorkDirRequirement
    listing:
      - entryname: a.txt
        entry: hello
inputs: {}
outputs: {}
"""
            let decoded = Decode.decodeCommandLineTool yaml
            match decoded.Requirements.Value.[0] with
            | InitialWorkDirRequirement listing ->
                Expect.equal listing.[0] (DirentEntry (DirentInstance(SchemaSaladString.Literal "hello", entryname = SchemaSaladString.Literal "a.txt"))) "The entry is a Dirent."
            | other -> failwithf "Expected an InitialWorkDirRequirement, found %A" other
            Kit.rewrite "The written text should decode to the same tool." Decode.decodeCommandLineTool Encode.encodeToolDescription decoded |> ignore
    ]


let testRepeatedEnumSymbols =
    let tool (lines: string list) = Kit.tool ("baseCommand: echo" :: lines)
    // The place of an enum, the document with {0} where the symbols stand.
    let places =
        [ "the type of an input", tool [ "inputs:"; "  a:"; "    type:"; "      type: enum"; "      symbols: {0}"; "outputs: []" ]
          "the type of an output", tool [ "inputs: []"; "outputs:"; "  o:"; "    type:"; "      type: enum"; "      symbols: {0}"; "    outputBinding: {glob: o}" ]
          "a definition that no port uses", tool [ "requirements:"; "  SchemaDefRequirement:"; "    types:"; "      - type: enum"; "        name: E"; "        symbols: {0}"; "inputs: []"; "outputs: []" ]
          "a definition in the hints", tool [ "hints:"; "  SchemaDefRequirement:"; "    types:"; "      - type: enum"; "        name: E"; "        symbols: {0}"; "inputs: []"; "outputs: []" ]
          "the type of a field of a record", tool [ "inputs:"; "  a:"; "    type:"; "      type: record"; "      fields:"; "        f:"; "          type:"; "            type: enum"; "            symbols: {0}"; "outputs: []" ]
          "the items of an array", tool [ "inputs:"; "  a:"; "    type:"; "      type: array"; "      items:"; "        type: enum"; "        symbols: {0}"; "outputs: []" ]
          "a member of a union", tool [ "inputs:"; "  a:"; "    type:"; "      - 'null'"; "      - type: enum"; "        symbols: {0}"; "outputs: []" ]
          "the input of a workflow", Kit.document "Workflow" [ "inputs:"; "  a:"; "    type:"; "      type: enum"; "      symbols: {0}"; "outputs: {}"; "steps: {}" ]
          "the input of an expression tool", Kit.document "ExpressionTool" [ "inputs:"; "  a:"; "    type:"; "      type: enum"; "      symbols: {0}"; "outputs: {}"; "expression: $(1)" ]
          "the input of an operation", Kit.document "Operation" [ "inputs:"; "  a:"; "    type:"; "      type: enum"; "      symbols: {0}"; "outputs: {}" ] ]
    // Spellings of symbols that cwltool reads as the same symbol, with the index of the symbol that repeats.
    let repeated =
        [ "[a, b, a]", 2
          "[a, a]", 1
          "[a, \"#a\"]", 1
          "[x/a, y/a]", 1
          "[x#a, a]", 1
          "[p:a, q:a]", 1
          "[./a, a]", 1
          "[\"http://example.org/a\", a]", 1
          "['a/', 'b/']", 1
          "['', '']", 1
          "[\"a#\", a]", 1
          "[\"x/#\", \"y/#\"]", 1
          "[\"#\", \"#\"]", 1
          "[\"p:a#\", a]", 1
          "[\"a?q#\", a]", 1
          "[\"x/a#\", \"y/a\"]", 1
          "[a, {$import: s.yml}, a]", 2 ]
    // Spellings that cwltool reads as different symbols, or that the decoder cannot compare.
    let distinct =
        [ "[a, A]"; "[x#y#a, a]"; "['', a]"; "[a, b, c]"; "[x/a, x/b]"; "[\"a#\", \"b#\"]"; "[\"http://x.org/a#\", \"http://x.org/b#\"]"; "[\"#\", a]"; "[\"#a\", \"b#\"]"; "[a, {$import: s.yml}]"; "[{$import: s.yml}, {$import: s.yml}]"; "[{$include: t.txt}, {$include: t.txt}]"; "{$import: s.yml}" ]
    testList "Repeated enum symbols" [
        testCase "an enum that repeats a symbol is a decode error at the repeat, whichever way the symbols are spelled" <| fun _ ->
            for (place, template) in places do
                for (symbols, index) in repeated do
                    match Kit.decodeError (fun () -> Decode.decodeCWLProcessingUnit (template.Replace("{0}", symbols))) with
                    | Some error ->
                        Expect.isTrue ((DecodeError.pathText error).EndsWith (sprintf "symbols[%d]" index)) (sprintf "%s %s: the error should show the repeat, found %s" place symbols (DecodeError.pathText error))
                        Expect.stringContains error.Message "repeats the symbol at line" (sprintf "%s %s: the error should say what is wrong" place symbols)
                        Expect.isSome error.Span (sprintf "%s %s: the error should have the place in the text" place symbols)
                    | None -> failwith (sprintf "%s %s must fail" place symbols)
        testCase "symbols that differ, and directives that stand for symbols of files, decode" <| fun _ ->
            for (place, template) in places do
                for symbols in distinct do
                    Decode.decodeCWLProcessingUnit (template.Replace("{0}", symbols)) |> ignore
        testCase "the rule holds in a document of v1.0" <| fun _ ->
            let text = "cwlVersion: v1.0\nclass: CommandLineTool\nbaseCommand: echo\ninputs:\n  a:\n    type:\n      type: enum\n      symbols: [a, a]\noutputs: []\n"
            Kit.processFails "A repeated symbol" text "inputs.a.type.symbols[1]" ""
    ]


let main =
    testList "Types" [
        testNestedArrayDecoding
        testUnionArrayItemDecoding
        testToolUnionPorts
        testAnyType
        testStreamTypes
        testPortSchemaNesting
        testArrayShorthand
        testFileAndDirectoryTypes
        testDirentIsNoType
        testRepeatedEnumSymbols
    ]
