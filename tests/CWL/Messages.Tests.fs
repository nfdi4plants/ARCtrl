/// The wording of the decode errors: one row per message template, with the whole message written out. Every other test
/// of a failure checks the path, one keyword and the place in the text only.
module Tests.Messages

open ARCtrl.CWL
open TestingUtils

let private tool (body: string list) = fun () -> Decode.decodeCommandLineTool (Kit.toolWith body) |> ignore
let private requirement (cls: string) (field: string) (value: string) = tool [ "requirements:"; "  - class: " + cls; "    " + field + ": " + value ]
let private input (body: string list) = fun () -> Decode.decodeCommandLineTool (Kit.tool ([ "baseCommand: echo"; "inputs:"; "  x:" ] @ (body |> List.map (fun line -> "    " + line)) @ [ "outputs: []" ])) |> ignore
/// A tool with the lines of its inputs.
let private ports (portLines: string list) = fun () -> Decode.decodeCommandLineTool (Kit.tool ([ "baseCommand: echo"; "inputs:" ] @ portLines @ [ "outputs: []" ])) |> ignore
let private processOf (text: string) = fun () -> Decode.decodeCWLProcessingUnit text |> ignore
let private bigDecimal = "100000000000000000000000000000"
let private beyondFloat = "1" + String.replicate 400 "0"

/// The name of the row, the decode that fails, the path of the error and the message.
let private rows : (string * (unit -> unit) * string * string) list =
    [ "a plain int in a string field", tool [ "label: 5" ], "label", "Expected a string, found the plain scalar 5, which the core schema reads as an int. Write it in quotes to make it a string."
      "a text without an expression in a resource field", requirement "ResourceRequirement" "ramMin" "abc", "requirements[0].ramMin", "Expected a number or an expression, found the text \"abc\". An expression holds $( or ${."
      "a plain float in a time limit", requirement "ToolTimeLimit" "timelimit" "1.5", "requirements[0].timelimit", "Expected an int or an expression, found the plain scalar 1.5, which the core schema reads as a float. An expression holds $( or ${."
      "a plain int beyond 64 bits in a time limit", requirement "ToolTimeLimit" "timelimit" bigDecimal, "requirements[0].timelimit", "Expected an int or an expression, found the plain scalar " + bigDecimal + ", which the core schema reads as an int beyond the 64-bit range. An expression holds $( or ${."
      "a plain int where a bool or an expression stands", requirement "NetworkAccess" "networkAccess" "5", "requirements[0].networkAccess", "Expected a bool or an expression, found the plain scalar 5, which the core schema reads as an int. An expression holds $( or ${."
      "a plain bool in a resource field", requirement "ResourceRequirement" "coresMin" "true", "requirements[0].coresMin", "Expected a number or an expression, found the plain scalar true, which the core schema reads as a bool. An expression holds $( or ${."
      "an int beyond the largest float in a resource field", requirement "ResourceRequirement" "coresMin" beyondFloat, "requirements[0].coresMin", "The int " + beyondFloat + " is beyond the largest float."
      "an int beyond 32 bits as an exit code", tool [ "successCodes: [2147483648]" ], "successCodes[0]", "Expected an int, found the plain scalar 2147483648, which is an int beyond the 32-bit range."
      "an int beyond 64 bits as the size of a File", requirement "InitialWorkDirRequirement" "listing" ("[{class: File, location: f.txt, size: " + bigDecimal + "}]"), "requirements[0].listing[0].size", "Expected an int64, found the plain scalar " + bigDecimal + ", which is an int beyond the 64-bit range."
      "a quoted number as the position of a binding", input [ "type: string"; "inputBinding: {position: \"1\"}" ], "inputs.x.inputBinding.position", "Expected an int or an expression, found the text \"1\"."
      "an int beyond 64 bits as the position of a binding", input [ "type: string"; "inputBinding: {position: " + bigDecimal + "}" ], "inputs.x.inputBinding.position", "Expected an int or an expression, found the plain scalar " + bigDecimal + ", which the core schema reads as an int beyond the 64-bit range."
      "null as the pattern of a secondary file", input [ "type: File"; "secondaryFiles: [{pattern: null}]" ], "inputs.x.secondaryFiles[0].pattern", "Expected a string or an expression as a pattern, found null."
      "a plain int as a secondary file", input [ "type: File"; "secondaryFiles: [5]" ], "inputs.x.secondaryFiles[0]", "Expected a string or an object as a secondary file, found the plain scalar 5, which the core schema reads as an int."
      "a secondary file without a pattern", input [ "type: File"; "secondaryFiles: [{required: true}]" ], "inputs.x.secondaryFiles[0]", "Expected a pattern in an object as a secondary file, found an object without one."
      "a plain int as the required of a secondary file", input [ "type: File"; "secondaryFiles: [{pattern: .idx, required: 1}]" ], "inputs.x.secondaryFiles[0].required", "Expected a bool or an expression, found the plain scalar 1, which the core schema reads as an int. An expression holds $( or ${."
      "a plain int as a loadListing", input [ "type: Directory"; "loadListing: 5" ], "inputs.x.loadListing", "Expected one of no_listing, shallow_listing and deep_listing, found the plain scalar 5, which the core schema reads as an int."
      "a plain int as a linkMerge", (fun () -> Decode.decodeWorkflow (Kit.step [ "run: t.cwl"; "in: {x: {source: [a, b], linkMerge: 5}}"; "out: []" ]) |> ignore), "steps.s.in.x.linkMerge", "Invalid linkMerge value: 5"
      "an int beyond 64 bits in a job file", (fun () -> DecodeParameters.decodeYAMLParameterFile ("x: " + bigDecimal + "\n") |> ignore), "x", "The int " + bigDecimal + " in a job file is beyond 64 bits, and the model holds an int as a long."
      "a name that is no type", ports [ "  x: foo bar" ], "inputs.x", "Invalid CWL type: foo bar"
      "a bare record as a type", ports [ "  x: record" ], "inputs.x", "Expected a type name, found \"record\". A schema is a mapping, so a schema belongs under type."
      "a mapping without a type field as a type", ports [ "  x:"; "    type:"; "      type: array"; "      items: {doc: a}" ], "inputs.x.type.items", "Expected a type name or a schema, found a mapping without a type field."
      "a type name in the type field of a schema", ports [ "  x:"; "    type:"; "      type: array"; "      items: {type: File}" ], "inputs.x.type.items.type", "Expected record, enum or array in the type field of a schema, found the text \"File\"."
      "a text as the fields of a record", ports [ "  r:"; "    type:"; "      type: record"; "      fields: abc" ], "inputs.r.type.fields", "Expected a list of fields or a mapping of fields, found the text \"abc\"."
      "a plain int as the inputs", processOf (Kit.document "CommandLineTool" [ "baseCommand: echo"; "inputs: 5"; "outputs: []" ]), "inputs", "Expected a list or a mapping of inputs, found the plain scalar 5, which the core schema reads as an int."
      "a plain int as an entry of the inputs", processOf (Kit.document "CommandLineTool" [ "baseCommand: echo"; "inputs: [5]"; "outputs: []" ]), "inputs[0]", "Expected a mapping, found the plain scalar \"5\"."
      "an entry of the inputs without an id", processOf (Kit.document "CommandLineTool" [ "baseCommand: echo"; "inputs: [{type: string}]"; "outputs: []" ]), "inputs[0]", "The mapping has no field id."
      "a null id of an input", processOf (Kit.document "CommandLineTool" [ "baseCommand: echo"; "inputs: [{id: ~, type: string}]"; "outputs: []" ]), "inputs[0].id", "Expected a value, found the null \"~\"."
      "a process without inputs", processOf (Kit.document "{$import: c.yml}" [ "outputs: []" ]), "", "Expected an inputs field, which CWL requires for a process."
      "a plain null as a type", ports [ "  x: null" ], "inputs.x", "Expected a string, found null. Write it in quotes to make it a string."
      "the type stdout of an input", ports [ "  x: stdout" ], "inputs.x", "The type stdout is valid only as the whole type of an output of a command line tool, without an outputBinding."
      "a list as a label", tool [ "label: [1, 2]" ], "label", "Expected a string or a directive, found a list."
      "a list as the id", tool [ "id: [a]" ], "id", "Expected a string, found a sequence."
      "a directive as the value of a map entry", ports [ "  x: {$import: a.yml}" ], "inputs.x", "A directive cannot stand for the value of an entry of the inputs, because Schema Salad adds the key as a field of the loaded object. Write the field as a list."
      "an include as an entry of the fields of a record", ports [ "  r:"; "    type:"; "      type: record"; "      fields: [{$include: t.txt}]" ], "inputs.r.type.fields[0]", "An $include yields the text of a file, but an entry of the fields field of a record takes an object. Write an $import for a file that holds the object."
      "a mixin as the inputs", processOf (Kit.document "CommandLineTool" [ "baseCommand: echo"; "inputs: {$mixin: m.yml}"; "outputs: []" ]), "inputs", "A $mixin completes an object with the fields beside it, and the inputs field is no object. Write an $import for a file that holds it."
      "an include as the intent", tool [ "intent: {$include: t.txt}" ], "intent", "An $include yields the text of a file, but the intent field takes a list of strings. Write the $include as an entry of the list."
      "a directive with a second field", tool [ "label: {$import: a.yml, extra: 1}" ], "label", "A directive has exactly one field, but this mapping has the fields $import, extra."
      "a number as the path of a directive", tool [ "label: {$import: 42}" ], "label.$import", "Expected the path of the $import directive to be a string, found the plain scalar 42, which the core schema reads as an int. Write it in quotes to make it a string."
      "an empty path of a directive", tool [ "label: {$import: \"\"}" ], "label.$import", "The path of the $import directive is empty."
      "an include as a bool", input [ "type: File"; "streamable: {$include: t.txt}" ], "inputs.x.streamable", "An $include yields the text of a file, but the streamable field takes a boolean. Write an $import for a file that holds the boolean."
      "an include as an input binding", input [ "type: string"; "inputBinding: {$include: f.txt}" ], "inputs.x.inputBinding", "An $include yields the text of a file, but the inputBinding field takes an object. Write an $import for a file that holds the object."
      "a text as the listing", requirement "InitialWorkDirRequirement" "listing" "abc", "requirements[0].listing", "Expected a list of entries or an expression for the listing, found the text \"abc\". An expression holds $( or ${."
      "an invalid pickValue", (fun () -> Decode.decodeWorkflow (Kit.step [ "run: t.cwl"; "in: {x: {source: [a, b], pickValue: foo}}"; "out: []" ]) |> ignore), "steps.s.in.x.pickValue", "Invalid pickValue value: foo"
      "an invalid scatterMethod", (fun () -> Decode.decodeWorkflow (Kit.step [ "run: t.cwl"; "in: {x: a}"; "out: []"; "scatter: x"; "scatterMethod: foo" ]) |> ignore), "steps.s.scatterMethod", "Invalid scatterMethod value: foo"
      "an optional stdin as the type of an input", ports [ "  x: stdin?" ], "inputs.x", "The type stdin is valid only as the whole type of an input of a command line tool, without an inputBinding."
      "a key that occurs twice", tool [ "label: a"; "label: b" ], "label", "The key label occurs twice, at line 6, column 1 and at line 7, column 1."
      "a key that repeats a key of another spelling", input [ "type: Any"; "default: {null: a, ~: b}" ], "inputs.x.default.~", "The key ~ at line 7, column 24 repeats the key null at line 7, column 15."
      "an anchor", tool [ "doc: &d greeting" ], "doc", "The anchor &d is not allowed. Schema Salad v1.2 prohibits anchors and aliases in a document."
      "an alias in a tree without its anchor", (fun () -> Decode.decodeCWLProcessingUnitElement (Kit.withoutAnchors (YAMLicious.Reader.readRoot (Kit.toolWith [ "doc: &d greeting"; "label: *d" ]))) |> ignore), "label", "The alias *d is not allowed. Schema Salad v1.2 prohibits anchors and aliases in a document."
      "an alias without an anchor", tool [ "doc: *nothere" ], "", "The alias *nothere names no anchor that stands earlier in the document."
      "an explicit tag", tool [ "doc: !!str greeting" ], "doc", "The tag !!str is not allowed. Schema Salad v1.2 prohibits explicit node tags in a document."
      "a %YAML directive", (fun () -> Decode.decodeCommandLineTool ("%YAML 1.2\n---\n" + Kit.toolWith []) |> ignore), "", "The directive %YAML 1.2 is not allowed. Schema Salad v1.2 prohibits %YAML and %TAG directives in a document."
      "a %TAG directive", (fun () -> Decode.decodeCommandLineTool ("%TAG ! tag:x,2000:\n---\n" + Kit.toolWith []) |> ignore), "", "The directive %TAG ! tag:x,2000: is not allowed. Schema Salad v1.2 prohibits %YAML and %TAG directives in a document." ]

let main =
    testList "Messages" [
        testCase "every message template reads as written" <| fun _ ->
            let failures =
                rows
                |> List.choose (fun (name, decode, path, message) ->
                    try
                        Expect.equal (Kit.decodeFailsWith name decode path "") message (sprintf "%s: the message" name)
                        None
                    with error ->
                        Some error.Message)
            if not failures.IsEmpty then
                failwith (String.concat "\n\n" failures)
    ]
