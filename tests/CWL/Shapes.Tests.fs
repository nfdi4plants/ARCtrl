/// The nodes of the wrong shape in a CWL document: one table of documents with the path of the error and a keyword of
/// its message, and the documents of the right shape beside them.
module Tests.Shapes

open ARCtrl.CWL
open TestingUtils
open YAMLicious

let private classes =
    [ "CommandLineTool", [ "baseCommand: echo" ]
      "ExpressionTool", [ "expression: $(null)" ]
      "Operation", []
      "Workflow", [ "steps: []" ] ]

/// A tool with the lines of its inputs.
let private ports (portLines: string list) = Kit.document "CommandLineTool" ([ "baseCommand: echo"; "inputs:" ] @ portLines @ [ "outputs: []" ])
/// The input r, a record with the lines of its fields.
let private record (fieldLines: string list) = [ "  r:"; "    type:"; "      type: record"; "      fields:" ] @ fieldLines
/// The input x, an array with the lines below its type.
let private items (lines: string list) = [ "  x:"; "    type:"; "      type: array" ] @ lines
let private stepWith (field: string) = Kit.document "Workflow" [ "inputs: []"; "outputs: []"; "steps:"; "  s:"; "    run: t.cwl"; "    " + field; "    out: []" ]
let private unnamedStep (lines: string list) = Kit.document "Workflow" ([ "inputs: []"; "outputs: []"; "steps:"; "  - label: unnamed" ] @ (lines |> List.map (fun line -> "    " + line)))
/// A process whose class is a directive, as a document and as the run of a step.
let private unknownDocument (lines: string list) = Kit.document "{$import: c.yml}" lines
let private unknownRun (lines: string list) =
    Kit.workflow ([ "inputs: []"; "outputs: []"; "steps:"; "  s:"; "    run:"; "      class: {$import: c.yml}" ] @ (lines |> List.map (fun line -> "      " + line)) @ [ "    in: []"; "    out: []" ])

let private badEntries =
    [ "a number", "5", "", "Expected a mapping"
      "a word", "x", "", "Expected a mapping"
      "a bool", "true", "", "Expected a mapping"
      "a list", "[a]", "", "Expected a mapping"
      "a null", "~", "", "Expected a mapping"
      "an empty mapping", "{}", "", "has no field id"
      "a mapping without an id", "{type: string}", "", "has no field id"
      "a mapping with a null id", "{id: ~, type: string}", ".id", "found the null" ]

/// The fields that every process has, wrong in a process of unknown class: the name, the lines, the path and the keyword.
let private unknownClassFields =
    [ "a list as the label", [ "label: [1, 2]"; "inputs: []"; "outputs: []" ], "label", "Expected a string or a directive"
      "a number as the doc", [ "doc: 5"; "inputs: []"; "outputs: []" ], "doc", "plain scalar"
      "a list as the id", [ "id: [a]"; "inputs: []"; "outputs: []" ], "id", "found a sequence"
      "a name that is no type in the inputs", [ "inputs: {x: not a type}"; "outputs: []" ], "inputs.x", "Invalid CWL type"
      "a name that is no type in the outputs", [ "inputs: []"; "outputs: {x: not a type}" ], "outputs.x", "Invalid CWL type"
      "no inputs", [ "outputs: []" ], "", "Expected an inputs field"
      "no outputs", [ "inputs: []" ], "", "has no field outputs"
      "a number as the requirements", [ "requirements: 5"; "inputs: []"; "outputs: []" ], "requirements", "list or a mapping of requirements"
      "a number as the hints", [ "hints: 5"; "inputs: []"; "outputs: []" ], "hints", "list or a mapping of hints"
      "a number as the intent", [ "intent: 5"; "inputs: []"; "outputs: []" ], "intent", "Expected a list of strings"
      "a number as the inputs", [ "inputs: 5"; "outputs: []" ], "inputs", "list or a mapping of inputs"
      "a number as the outputs", [ "inputs: []"; "outputs: 5" ], "outputs", "list or a mapping of outputs"
      "an input of the type stdout", [ "inputs: {x: stdout}"; "outputs: []" ], "inputs.x", "whole type of an output"
      "an input of the type stderr", [ "inputs: [{id: x, type: stderr}]"; "outputs: []" ], "inputs.x.type", "whole type of an output"
      "an output of the type stdin", [ "inputs: []"; "outputs: {o: stdin}" ], "outputs.o", "whole type of an input"
      "an output of the type stdin in a list", [ "inputs: []"; "outputs: [{id: o, type: stdin}]" ], "outputs.o.type", "whole type of an input" ]

/// The name of the row, the document, the path of the error and a keyword of its message.
let private rows : (string * string * string * string) list =
    [ // A type name that is no type, and a schema that stands where a name belongs.
      "a name that is no type as the type of a port", ports [ "  x: foo bar" ], "inputs.x", "Invalid CWL type"
      "a name that is no type with the shorthand", ports [ "  x: foo bar[]?" ], "inputs.x", "Invalid CWL type"
      "a name that is no type in the type field of a port", ports [ "  x:"; "    type: foo bar" ], "inputs.x.type", "Invalid CWL type"
      "a name that is no type in a type list", ports [ "  x: [string, foo bar]" ], "inputs.x[1]", "Invalid CWL type"
      "a name that is no type in the type list of the type field", ports [ "  x:"; "    type: [string, foo bar]" ], "inputs.x.type[1]", "Invalid CWL type"
      "a name that is no type as the items of an array", ports (items [ "      items: foo bar" ]), "inputs.x.type.items", "Invalid CWL type"
      "a name that is no type in the items of an array", ports (items [ "      items: [string, foo bar]" ]), "inputs.x.type.items[1]", "Invalid CWL type"
      "a name that is no type as a record field in map form", ports (record [ "        f: foo bar" ]), "inputs.r.type.fields.f", "Invalid CWL type"
      "a name that is no type in the type field of a record field", ports (record [ "        f:"; "          type: foo bar" ]), "inputs.r.type.fields.f.type", "Invalid CWL type"
      "a name that is no type as the type of an output", Kit.document "CommandLineTool" [ "baseCommand: echo"; "inputs: []"; "outputs:"; "  o: foo bar" ], "outputs.o", "Invalid CWL type"
      "a bare array in a type list", ports [ "  x: [\"null\", array]" ], "inputs.x[1]", "a schema belongs under type"
      "a bare record as the type of a port", ports [ "  x: record" ], "inputs.x", "a schema belongs under type"
      "a bare array as a record field in map form", ports (record [ "        f: array" ]), "inputs.r.type.fields.f", "a schema belongs under type"
      "a bare array in the type list of a record field", ports (record [ "        f:"; "          type: [\"null\", array]" ]), "inputs.r.type.fields.f.type[1]", "a schema belongs under type"
      "items that are a mapping without a type", ports (items [ "      items: {doc: a}" ]), "inputs.x.type.items", "a mapping without a type field"
      "an array schema in place of a field object in map form", ports (record [ "        f:"; "          type: array"; "          items: string" ]), "inputs.r.type.fields.f.type", "a schema belongs under type"
      "a record schema in place of a field object in map form", ports (record [ "        f:"; "          type: record"; "          fields:"; "            g: string" ]), "inputs.r.type.fields.f.type", "a schema belongs under type"
      "an enum schema in place of a field object in map form", ports (record [ "        f:"; "          type: enum"; "          symbols: [a, b]" ]), "inputs.r.type.fields.f.type", "a schema belongs under type"
      "an array schema in place of a field object in a list", ports (record [ "        - name: f"; "          type: array"; "          items: string" ]), "inputs.r.type.fields[0].f.type", "a schema belongs under type"
      // A type name in the type field of a schema.
      "File in the type field of the items", ports (items [ "      items: {type: File}" ]), "inputs.x.type.items.type", "in the type field of a schema"
      "File? in the type field of the items", ports (items [ "      items: {type: \"File?\"}" ]), "inputs.x.type.items.type", "in the type field of a schema"
      "Directory? in a type list of the items", ports (items [ "      items: [string, {type: \"Directory?\"}]" ]), "inputs.x.type.items[1].type", "in the type field of a schema"
      "File with a format in the type field of the items", ports (items [ "      items: {type: File, format: \"http://edamontology.org/format_2330\"}" ]), "inputs.x.type.items.type", "in the type field of a schema"
      "File? with a size in the type field of a port", ports [ "  x:"; "    type:"; "      type: \"File?\""; "      size: 5" ], "inputs.x.type.type", "in the type field of a schema"
      "File with a format in a type list", ports [ "  x: [\"null\", {type: File, format: \"http://edamontology.org/format_2330\"}]" ], "inputs.x[1].type", "in the type field of a schema"
      "a stream type in the type field of the items", ports (items [ "      items: {type: stdout}" ]), "inputs.x.type.items.type", "in the type field of a schema"
      "a name that is no type in the type field of the items", ports (items [ "      items: {type: foo}" ]), "inputs.x.type.items.type", "in the type field of a schema"
      // The fields of a record.
      for fields in [ "abc"; "42"; "true"; "\"x\"" ] do
          "the fields " + fields, ports [ "  r:"; "    type:"; "      type: record"; "      fields: " + fields ], "inputs.r.type.fields", "Expected a list of fields or a mapping of fields"
      // The ports, the steps and the step inputs.
      for (cls, extra) in classes do
          for value in [ "5"; "true"; "abc"; "\"abc\""; "1.5" ] do
              cls + " with the inputs " + value, Kit.document cls (extra @ [ "inputs: " + value; "outputs: []" ]), "inputs", "Expected a list or a mapping of inputs"
              cls + " with the outputs " + value, Kit.document cls (extra @ [ "inputs: []"; "outputs: " + value ]), "outputs", "Expected a list or a mapping of outputs"
          for (what, entry, below, keyword) in badEntries do
              cls + " with an input that is " + what, Kit.document cls (extra @ [ "inputs: [" + entry + "]"; "outputs: []" ]), "inputs[0]" + below, keyword
              cls + " with an output that is " + what, Kit.document cls (extra @ [ "inputs: []"; "outputs: [" + entry + "]" ]), "outputs[0]" + below, keyword
          for spelling in [ "null"; "Null"; "NULL"; "~" ] do
              cls + " with the type " + spelling, Kit.document cls (extra @ [ "inputs: [{id: x, type: " + spelling + "}]"; "outputs: []" ]), "inputs.x.type", "Write it in quotes"
              cls + " with the map entry " + spelling, Kit.document cls (extra @ [ "inputs: {x: " + spelling + "}"; "outputs: []" ]), "inputs.x", "Write it in quotes"
              cls + " with the first type of a list " + spelling, Kit.document cls (extra @ [ "inputs: [{id: x, type: [" + spelling + ", string]}]"; "outputs: []" ]), "inputs.x.type[0]", "Write it in quotes"
              cls + " with the last type of a list " + spelling, Kit.document cls (extra @ [ "inputs: [{id: x, type: [string, " + spelling + "]}]"; "outputs: []" ]), "inputs.x.type[1]", "Write it in quotes"
              cls + " with the map entry list " + spelling, Kit.document cls (extra @ [ "inputs: {x: [" + spelling + ", string]}"; "outputs: []" ]), "inputs.x[0]", "Write it in quotes"
              cls + " with the output " + spelling, Kit.document cls (extra @ [ "inputs: []"; "outputs: {o: " + spelling + "}" ]), "outputs.o", "Write it in quotes"
              cls + " with the items " + spelling, Kit.document cls (extra @ [ "inputs: [{id: x, type: {type: array, items: " + spelling + "}}]"; "outputs: []" ]), "inputs.x.type.items", "Write it in quotes"
      for value in [ "5"; "true"; "abc"; "\"abc\""; "1.5" ] do
          "the steps " + value, Kit.document "Workflow" [ "inputs: []"; "outputs: []"; "steps: " + value ], "steps", "Expected a list or a mapping of steps"
          "the step inputs " + value, stepWith ("in: " + value), "steps.s.in", "Expected a list or a mapping of step inputs"
      for (what, entry, below, keyword) in badEntries do
          "a step input that is " + what, stepWith ("in: [" + entry + "]"), "steps.s.in[0]" + below, keyword
          if what <> "an empty mapping" && what <> "a mapping without an id" then
              "a step that is " + what, Kit.document "Workflow" [ "inputs: []"; "outputs: []"; "steps: [" + entry + "]" ], "steps[0]" + below, keyword
      "a step without an id and without fields", Kit.document "Workflow" [ "inputs: []"; "outputs: []"; "steps: [{}]" ], "steps[0]", "has no field run"
      "a step without an id and without run", unnamedStep [ "in: []"; "out: []" ], "steps[0]", "has no field run"
      "a step without an id and without in", unnamedStep [ "run: t.cwl"; "out: []" ], "steps[0]", "has no field in"
      "a step without an id and without out", unnamedStep [ "run: t.cwl"; "in: []" ], "steps[0]", "has no field out"
      "a number behind a valid input", Kit.document "CommandLineTool" [ "baseCommand: echo"; "inputs: [{id: a, type: string}, 5]"; "outputs: []" ], "inputs[1]", "Expected a mapping"
      "a number behind a valid output", Kit.document "CommandLineTool" [ "baseCommand: echo"; "inputs: []"; "outputs: [{id: a, type: string}, 5]" ], "outputs[1]", "Expected a mapping"
      "a number behind a valid step", Kit.document "Workflow" [ "inputs: []"; "outputs: []"; "steps: [{id: s, run: t.cwl, in: [], out: []}, 5]" ], "steps[1]", "Expected a mapping"
      "a number behind a valid step input", stepWith "in: [{id: a, source: x}, 5]", "steps.s.in[1]", "Expected a mapping"
      // The fields that every process has, in a process of unknown class.
      for (name, lines, path, keyword) in unknownClassFields do
          name + " of a process of unknown class", unknownDocument lines, path, keyword
          name + " of a run of unknown class", unknownRun lines, "steps.s.run" + (if path = "" then "" else "." + path), keyword ]

let private accepted =
    testList "The nodes of the right shape beside the table" [
        testCase "the fields of a record are a list or a mapping, and a schema under the type of a field is the field type" <| fun _ ->
            for fields in [ "[]"; "{}"; "[{name: f, type: string}]"; "{f: string}" ] do
                Expect.isSome (ListEntry.values (Decode.decodeCommandLineTool (ports [ "  r:"; "    type:"; "      type: record"; "      fields: " + fields ])).Inputs.Value).[0].Type_ (sprintf "fields: %s is a list or a mapping." fields)
            Decode.decodeCommandLineTool (ports (record [ "        f:"; "          type:"; "            type: array"; "            items: string" ])) |> ignore
        testCase "a step without an id that has the fields run, in and out decodes" <| fun _ ->
            Decode.decodeCWLProcessingUnit (unnamedStep [ "run: t.cwl"; "in: []"; "out: []" ]) |> ignore
        testCase "the quoted null is the null type in every class" <| fun _ ->
            let typeOf (cls: string) (extra: string list) (line: string) =
                (CWLProcessingUnit.getInputValues (Decode.decodeCWLProcessingUnit (Kit.document cls (extra @ [ line; "outputs: []" ])))).[0].Type_
            for (cls, extra) in classes do
                Expect.equal (typeOf cls extra "inputs: {x: \"null\"}") (Some CWLType.Null) (cls + ": the quoted null is the null type.")
                Expect.equal (typeOf cls extra "inputs: [{id: x, type: 'null'}]") (Some CWLType.Null) (cls + ": the single quoted null is the null type.")
                Expect.equal (typeOf cls extra "inputs: [{id: x, type: [\"null\", string]}]") (Some (CWLType.Union (ResizeArray [ CWLType.Null; CWLType.String ]))) (cls + ": the quoted null is a member of the union.")
        testCase "a process of unknown class may have the stream types of a command line tool and the fields of any one class" <| fun _ ->
            // The file may hold a command line tool, so an input may have the type stdin and an output stdout or stderr.
            for lines in [ [ "inputs: {i: stdin}"; "outputs: {o: stdout, e: stderr}" ]; [ "inputs: {i: {type: string, inputBinding: {position: 1}}}"; "outputs: {o: {type: string, outputBinding: {glob: o}}}"; "steps: []"; "baseCommand: echo"; "expression: x" ] ] do
                Expect.equal (Decode.decodeCWLProcessingUnit (unknownDocument lines)) (RawProcess { Node = Reader.readRoot (unknownDocument lines) }) "The process stays as written."
            Decode.decodeCWLProcessingUnit (unknownRun [ "inputs: {i: stdin}"; "outputs: {o: stdout, e: stderr}" ]) |> ignore
    ]

let main =
    testList "Shapes" [
        testList "The shape table" [
            for (name, document, path, keyword) in rows do
                testCase (sprintf "%s is a decode error at %s that says %s" name (if path = "" then "the process" else path) keyword) <| fun _ ->
                    Kit.processFails name document path keyword
        ]
        accepted
    ]
