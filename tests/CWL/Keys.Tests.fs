/// Repeated keys and the YAML features that Schema Salad v1.2 prohibits: one table of the places where a key repeats, the
/// pairs of keys that the loader of cwltool takes for one key, the decoders of a fragment, and one table of the prohibited
/// features (anchor, alias, explicit tag, %YAML and %TAG directive) at every place that reads YAML.
module Tests.Keys

open ARCtrl.CWL
open TestingUtils
open YAMLicious

/// A command line tool with the base command echo and the lines, without ports.
let private tool (lines: string list) = Kit.tool ("baseCommand: echo" :: lines)

/// A command line tool with the lines in front of empty ports. The lines start at line 4.
let private toolText (lines: string list) = tool (lines @ [ "inputs: []"; "outputs: []" ])

let private decodeTool (text: string) = fun () -> Decode.decodeCommandLineTool text |> ignore

/// A tool whose input x has the value as its streamable field, so that a $mixin stands at line 7.
let private belowMixin (value: string) = decodeTool (tool [ "inputs:"; "  x:"; "    type: File"; "    streamable: " + value; "outputs: []" ])

/// A workflow whose step s runs an inline tool with the line, which stands at line 12, column 7.
let private inlineRun (line: string) =
    fun () ->
        Decode.decodeWorkflow (Kit.workflow [ "inputs: []"; "outputs: []"; "steps:"; "  s:"; "    run:"; "      class: CommandLineTool"; "      baseCommand: echo"; "      inputs: []"; "      outputs: []"; "      " + line; "    in: []"; "    out: []" ])
        |> ignore

let private jobFile (text: string) = fun () -> DecodeParameters.decodeYAMLParameterFile text |> ignore
let private processTree (tree: YamlNode) = fun () -> Decode.decodeCWLProcessingUnitElement tree |> ignore
let private jobTree (tree: YamlNode) = fun () -> DecodeParameters.cwlparameterReferenceArrayDecoder tree |> ignore
let private typeText (text: string) = fun () -> Decode.cwlTypeFromAdditionalType text |> ignore
let private withPorts (cls: string) (lines: string list) = fun () -> Decode.decodeCWLProcessingUnit (Kit.document cls lines) |> ignore

/// The name of the row, the decode that fails, the path of the error and the keyword of its message.
let private repeatedKeys : (string * (unit -> unit) * string * string) list =
    let input (lines: string list) = decodeTool (tool ([ "inputs:"; "  x:"; "    type: string" ] @ lines @ [ "outputs: []" ]))
    [ "a field of a tool", decodeTool (Kit.toolWith [ "label: first"; "label: second" ]), "label", "key label occurs twice"
      "a parameter of a job file", jobFile "a: 1\na: 2\n", "a", "key a occurs twice"
      "a raw value in flow form", input [ "    default: {a: 1, a: 2}" ], "inputs.x.default.a", "key a occurs twice"
      "a raw value in block form", input [ "    default:"; "      a: 1"; "      a: 2" ], "inputs.x.default.a", "key a occurs twice"
      "a mapping in a list of a raw value", input [ "    default:"; "      b:"; "        - {k: 1}"; "        - {k: 1, k: 2}" ], "inputs.x.default.b[1].k", "key k occurs twice"
      "a plain and a quoted key in a list of a raw hint", decodeTool (tool [ "hints:"; "  - class: acme:Hint"; "    list:"; "      - {k: 1, \"k\": 2}"; "inputs: []"; "outputs: []" ]), "hints[0].list[0].k", "key k occurs twice"
      "a secondary file of an output", decodeTool (tool [ "inputs: []"; "outputs:"; "  o:"; "    type: File"; "    secondaryFiles:"; "      - {k: 1, k: 2}" ]), "outputs.o.secondaryFiles[0].k", "key k occurs twice"
      "a field of the root", decodeTool (tool [ "baseCommand: ls"; "inputs: []"; "outputs: []" ]), "baseCommand", "key baseCommand occurs twice"
      "the inputs in map form", withPorts "CommandLineTool" [ "baseCommand: echo"; "inputs: {x: string, x: int}"; "outputs: []" ], "inputs.x", "key x occurs twice"
      "the outputs in map form", withPorts "CommandLineTool" [ "baseCommand: echo"; "inputs: []"; "outputs: {x: stdout, x: stderr}" ], "outputs.x", "key x occurs twice"
      "the steps in map form", withPorts "Workflow" [ "inputs: []"; "outputs: []"; "steps: {s: {run: t.cwl, in: [], out: []}, s: {run: t.cwl, in: [], out: []}}" ], "steps.s", "key s occurs twice"
      "the step inputs in map form", withPorts "Workflow" [ "inputs: []"; "outputs: []"; "steps:"; "  s:"; "    run: t.cwl"; "    in: {a: x, a: y}"; "    out: []" ], "steps.s.in.a", "key a occurs twice"
      "a mapping with a $mixin", belowMixin "{$mixin: t.yml, a: 1, a: 2}", "inputs.x.streamable.a", "key a occurs twice" ]

/// The name of the row, the decode that fails, the path of the error, the keyword of its message and the line and the
/// column where the error starts. A tree has no directives, because Reader.readRoot drops the prelude of the document.
let private prohibitedFeatures : (string * (unit -> unit) * string * string * (int * int)) list =
    let inDocument (lines: string list) = decodeTool (toolText lines)
    let yamlDirective = "%YAML 1.2\n---\n"
    [ "an anchor on a scalar", inDocument [ "doc: &d greeting" ], "doc", "anchor &d", (4, 6)
      "an anchor on a mapping", inDocument [ "ex:meta: &m {a: 1}" ], "ex:meta", "anchor &m", (4, 10)
      "an anchor on a sequence", inDocument [ "arguments: &c [echo]" ], "arguments", "anchor &c", (4, 12)
      "an anchor on an item of a list", inDocument [ "arguments: [&i a]" ], "arguments[0]", "anchor &i", (4, 13)
      "an anchor on a key", inDocument [ "ex:x: {&k a: 1}" ], "ex:x.a", "anchor &k", (4, 8)
      "an anchor below a later field", inDocument [ "label: x"; "ex:y: {b: &z 1}" ], "ex:y.b", "anchor &z", (5, 11)
      "an anchor in front of its alias", inDocument [ "doc: &d greeting"; "label: *d" ], "doc", "anchor &d", (4, 6)
      "an anchor in a default and its alias in another", decodeTool (Kit.tool [ "baseCommand: echo"; "inputs:"; "  x:"; "    type: string"; "    default: &a value"; "  y:"; "    type: string"; "    default: *a"; "outputs: {}" ]), "inputs.x.default", "anchor &a", (7, 14)
      "a core tag on a string", inDocument [ "doc: !!str greeting" ], "doc", "tag !!str", (4, 6)
      "a core tag on an int", inDocument [ "ex:n: !!int 5" ], "ex:n", "tag !!int", (4, 7)
      "a core tag on a quoted string", inDocument [ "doc: !!str \"greeting\"" ], "doc", "tag !!str", (4, 6)
      "a local tag", inDocument [ "ex:n: !custom {a: 1}" ], "ex:n", "tag !custom", (4, 7)
      "a verbatim tag", inDocument [ "ex:n: !<tag:x,2000:y> a" ], "ex:n", "tag !<tag:x,2000:y>", (4, 7)
      "the non-specific tag", inDocument [ "doc: ! greeting" ], "doc", "tag ! is", (4, 6)
      "a tag on a key", inDocument [ "ex:x: {!!str a: 1}" ], "ex:x.a", "tag !!str", (4, 8)
      "an alias without an anchor", inDocument [ "doc: *nothere" ], "", "*nothere", (4, 6)
      "a %YAML directive", decodeTool (yamlDirective + toolText []), "", "directive %YAML 1.2", (1, 1)
      "a %TAG directive behind a comment", decodeTool ("# before\n%TAG ! tag:x,2000:\n---\n" + toolText []), "", "directive %TAG ! tag:x,2000:", (2, 1)
      "an anchor in an inline run", inlineRun "doc: &d x", "steps.s.run.doc", "anchor &d", (12, 12)
      "a tag in an inline run", inlineRun "doc: !!str x", "steps.s.run.doc", "tag !!str", (12, 12)
      "an alias without an anchor in an inline run", inlineRun "doc: *nothere", "", "*nothere", (12, 12)
      "an anchor below a mixin", belowMixin "{$mixin: t.yml, a: &anc 1, b: *anc}", "inputs.x.streamable.a", "anchor &anc", (7, 36)
      "a tag below a mixin", belowMixin "{$mixin: t.yml, a: !!int 1}", "inputs.x.streamable.a", "tag !!int", (7, 36)
      "a %YAML directive in front of a mixin", decodeTool (yamlDirective + tool [ "inputs:"; "  x:"; "    type: File"; "    streamable: {$mixin: t.yml}"; "outputs: []" ]), "", "directive %YAML 1.2", (1, 1)
      "an anchor in a job file", jobFile "x: &v a\ny: b\n", "x", "anchor &v", (1, 4)
      "an anchor in front of its alias in a job file", jobFile "x: &v a\ny: *v\n", "x", "anchor &v", (1, 4)
      "an anchor in a list of a job file", jobFile "x: [a, &v b]\n", "x[1]", "anchor &v", (1, 8)
      "an anchor in a record of a job file", jobFile "x:\n  y: &m {z: 1}\n", "x.y", "anchor &m", (2, 6)
      "a tag in a job file", jobFile "x: !!str 5\n", "x", "tag !!str", (1, 4)
      "a tag of a File in a job file", jobFile "x: !custom {class: File, path: a.txt}\n", "x", "tag !custom", (1, 4)
      "an alias without an anchor in a job file", jobFile "x: *nothere\n", "", "*nothere", (1, 4)
      "a %YAML directive in a job file", jobFile "%YAML 1.2\n---\nx: 1\n", "", "directive %YAML 1.2", (1, 1)
      "a %TAG directive in a job file", jobFile "%TAG ! tag:x,2000:\n---\nx: 1\n", "", "directive %TAG ! tag:x,2000:", (1, 1)
      "an anchor in the tree of a process", processTree (Reader.readRoot (toolText [ "doc: &d greeting" ])), "doc", "anchor &d", (4, 6)
      "a tag in the tree of a process", processTree (Reader.readRoot (toolText [ "doc: !!str greeting" ])), "doc", "tag !!str", (4, 6)
      "an alias in the tree of a process without its anchor", processTree (Kit.withoutAnchors (Reader.readRoot (toolText [ "doc: &d greeting"; "label: *d" ]))), "label", "alias *d", (5, 8)
      "an anchor in the tree of a job file", jobTree (Reader.readRoot "x: &v a\n"), "x", "anchor &v", (1, 4)
      "a tag in the tree of a job file", jobTree (Reader.readRoot "x: !!str 5\n"), "x", "tag !!str", (1, 4)
      "an alias in the tree of a job file without its anchor", jobTree (Kit.withoutAnchors (Reader.readRoot "x: &v a\ny: *v\n")), "y", "alias *v", (2, 4)
      "an anchor in the text of an RO-Crate type", typeText "&t File", "", "anchor &t", (1, 1)
      "an anchor in a schema in the text of an RO-Crate type", typeText "{type: array, items: &i File}", "items", "anchor &i", (1, 22)
      "a tag in the text of an RO-Crate type", typeText "!!str File", "", "tag !!str", (1, 1)
      "an alias without an anchor in the text of an RO-Crate type", typeText "*t", "", "*t", (1, 1)
      "a %YAML directive in the text of an RO-Crate type", typeText (yamlDirective + "File"), "", "directive %YAML 1.2", (1, 1) ]

let private testRepeatedKeys =
    testList "The repeated key table" [
        for (name, decode, path, keyword) in repeatedKeys do
            testCase (sprintf "a key that occurs twice in %s is a decode error at %s" name path) <| fun _ ->
                Kit.decodeFails name decode path keyword
        testCase "the same key in two mappings of a raw value is no repeated key" <| fun _ ->
            let text = tool [ "inputs:"; "  x:"; "    type: string"; "    default: {a: {a: 1}, b: {a: 2}}"; "outputs: []" ]
            Expect.isNone (Kit.decodeError (decodeTool text)) "The same key in different mappings is no repeated key."
    ]

let private testProhibitedFeatures =
    testList "The prohibited feature table" [
        for (name, decode, path, keyword, start) in prohibitedFeatures do
            testCase (sprintf "%s is a decode error at '%s' that names %s" name path keyword) <| fun _ ->
                Kit.decodeFails name decode path keyword
                let error = (Kit.decodeError decode).Value
                Expect.equal (error.Span |> Option.map (fun span -> span.Start.Line, span.Start.Column)) (Some start) (sprintf "%s: the error should stand at the feature" name)
    ]

let private testBesideTheTables =
    testList "The keys and the features beside the tables" [
        testCase "keys that the loader of cwltool takes for one key are a repeated key, and keys of another type are different keys" <| fun _ ->
            let tool (keys: string) = Kit.tool [ "baseCommand: echo"; "inputs:"; "  x:"; "    type: Any"; "    default: " + keys; "outputs: []" ]
            // The loader compares keys as Python does: 1 equals 0x1, 1.0 and true, and every null is one key.
            // The message names both spellings when they differ, and the key once when they are equal.
            for (keys, first, second) in
                [ "{null: a, ~: b}", "null", "~"; "{null: a, Null: b}", "null", "Null"; "{true: a, True: b}", "true", "True"; "{1: a, 0x1: b}", "1", "0x1"
                  "{1: a, 01: b}", "1", "01"; "{1: a, +1: b}", "1", "+1"; "{1: a, 1.0: b}", "1", "1.0"; "{true: a, 1: b}", "true", "1"
                  "{false: a, 0: b}", "false", "0"; "{1.5: a, 15e-1: b}", "1.5", "15e-1"; "{.inf: a, .Inf: b}", ".inf", ".Inf"
                  "{.nan: a, .NaN: b}", ".nan", ".NaN"; "{8: a, 0o10: b}", "8", "0o10"
                  "{a: 1, \"a\": 2}", "a", "a"; "{\"1\": a, '1': b}", "1", "1"
                  "{12345678901234567890: a, 12345678901234567890: b}", "12345678901234567890", "12345678901234567890" ] do
                match Kit.decodeError (fun () -> Decode.decodeCommandLineTool (tool keys)) with
                | Some error ->
                    Expect.isTrue ((DecodeError.pathText error).StartsWith "inputs.x.default.") (sprintf "%s: the error should show the second key, found %s." keys (DecodeError.pathText error))
                    if first = second then
                        Expect.stringContains error.Message ("The key " + first + " occurs twice, at line ") (sprintf "%s: the error should say that the key occurs twice." keys)
                    else
                        Expect.stringContains error.Message ("The key " + second + " at line ") (sprintf "%s: the error should name the second key and its place." keys)
                        Expect.stringContains error.Message (" repeats the key " + first + " at line ") (sprintf "%s: the error should name the first key and its place." keys)
                        Expect.isFalse (error.Message.Contains "occurs twice") (sprintf "%s: the error should not use the wording for equal keys." keys)
                | None -> failwith (sprintf "The keys %s are one key for cwltool and must fail." keys)
            // A quoted key is a text, so it differs from the plain key of another type.
            for keys in
                [ "{1: a, \"1\": b}"; "{\"1\": a, 1: b}"; "{true: a, \"true\": b}"; "{null: a, \"null\": b}"; "{~: a, \"~\": b}"; "{1: a, 2: b}"; "{1: a, 1.5: b}"
                  "{.inf: a, -.inf: b}"; "{1: a, a: b}"; "{.nan: a, \".nan\": b}" ] do
                Kit.toolRoundTrip keys (tool keys) |> ignore
            let written = Encode.encodeToolDescription (Decode.decodeCommandLineTool (tool "{1: a, \"1\": b, null: c, \"null\": d}"))
            for expected in [ "1: a"; "\"1\": b"; "null: c"; "\"null\": d" ] do
                Expect.stringContains written expected (sprintf "The key keeps its spelling: %s\n%s" expected written)
        testCase "a decoder of a fragment fails at a key that repeats a key of the mapping it reads, with the error of the whole text" <| fun _ ->
            // A tree that goes straight to a decoder of a fragment has not passed the check of a document, so the decoder checks
            // the keys of each mapping that it reads entry by entry. The error equals the error of the text entry point.
            let places : (string * (YamlNode -> unit) * (string -> string -> string)) list =
                [ "inputs", (Decode.inputArrayDecoder >> ignore), (fun a b -> a + ": string\n" + b + ": int\n")
                  "outputs", (Decode.outputArrayDecoder >> ignore), (fun a b -> a + ": stdout\n" + b + ": stderr\n")
                  "steps", (Decode.stepsDecoder >> ignore), (fun a b -> "steps:\n  " + a + ": {run: t.cwl, in: [], out: []}\n  " + b + ": {run: t.cwl, in: [], out: []}\n")
                  "step inputs", (Decode.inputStepDecoder >> ignore), (fun a b -> a + ": x\n" + b + ": y\n")
                  "hints in map form", (Decode.hintArrayDecoder >> ignore), (fun a b -> a + ": {}\n" + b + ": {}\n")
                  "the fields of a hint in map form", (Decode.hintArrayDecoder >> ignore), (fun a b -> "acme:Hint: {" + a + ": 1, " + b + ": 2}\n")
                  "the fields of a hint of an unknown class", (Decode.hintArrayDecoder >> ignore), (fun a b -> "- {class: acme:Hint, " + a + ": 1, " + b + ": 2}\n")
                  "requirements in map form", (Decode.requirementArrayDecoder >> ignore), (fun a b -> a + ": {}\n" + b + ": {}\n")
                  "envDef in map form", (Decode.requirementArrayDecoder >> ignore), (fun a b -> "- class: EnvVarRequirement\n  envDef: {" + a + ": x, " + b + ": y}\n")
                  "a record of a job file", (DecodeParameters.decodeParameterValue >> ignore), (fun a b -> "{" + a + ": 1, " + b + ": 2}\n") ]
            // An extension field holds a mapping with a key that is no text as it was written, so these places take text keys.
            let extensionPlaces : (string * (YamlNode -> unit) * (string -> string -> string)) list =
                [ "a mapping in an extension field", (Decode.inputArrayDecoder >> ignore), (fun a b -> "x: {type: string, acme:f: {" + a + ": 1, " + b + ": 2}}\n")
                  "a mapping in a list of an extension field", (Decode.inputArrayDecoder >> ignore), (fun a b -> "x: {type: string, acme:f: [{a: 1}, {g: {" + a + ": 1, " + b + ": 2}}]}\n") ]
            // Equal texts in any quoting and plain keys that the loader of cwltool takes for one key. A hint in map form drops
            // its class field, and a repeated class key fails there too.
            let textPairs = [ "x", "x"; "x", "\"x\""; "'x'", "\"x\""; "class", "class" ]
            let otherPairs = [ "1", "0x1"; "true", "1"; "null", "~" ]
            for (somePlaces, keyPairs) in [ places, textPairs @ otherPairs; extensionPlaces, textPairs ] do
                for (place, decode, text) in somePlaces do
                    for (first, second) in keyPairs do
                        let yaml = text first second
                        let expected = Kit.decodeError (fun () -> Decode.readYaml yaml)
                        match expected with
                        | Some error ->
                            Expect.isTrue (error.Message.Contains "occurs twice" || error.Message.Contains "repeats the key") (sprintf "%s, %s and %s: the whole text fails at the repeated key, found %s" place first second error.Message)
                        | None -> failwith (sprintf "%s, %s and %s: the whole text must fail." place first second)
                        Expect.equal (Kit.decodeError (fun () -> decode (Reader.readRoot yaml))) expected (sprintf "%s, %s and %s: the decoder should fail with the error of the whole text." place first second)
            let apart = "x: {type: string, acme:f: {a: 1}}\ny: {type: string, acme:f: {a: 2}}\n"
            Expect.equal (Decode.inputArrayDecoder (Reader.readRoot apart)).Count 2 "The same key in two mappings is no repeated key."
        testCase "the characters of the features are legal where they are no features" <| fun _ ->
            let accepted (name: string) (lines: string list) =
                match Kit.decodeError (fun () -> Decode.decodeCommandLineTool (tool (lines @ [ "inputs: []"; "outputs: []" ]))) with
                | Some error -> failwith (sprintf "%s must decode, found %s" name (DecodeError.text error))
                | None -> ()
            accepted "quoted characters" [ "doc: \"!x &x *x %YAML 1.2\""; "label: '&x'"; "ex:q: \"*x\"" ]
            accepted "plain scalars with the characters inside" [ "doc: a&b"; "label: a*b"; "ex:p: a!b %c" ]
            accepted "a block scalar with lines that look like features" [ "doc: |"; "  &x"; "  *y"; "  !z"; "  %YAML 1.2"; "label: x" ]
            accepted "a document with a shebang and the markers of one document" [ "label: x" ]
            Expect.equal (Decode.decodeCommandLineTool ("#!/usr/bin/env cwl-runner\n---\n" + tool [ "inputs: []"; "outputs: []"; "..." ])).CWLVersion "v1.2" "A shebang and the markers of a single document are read."
            accepted "the keys of the directives of Schema Salad" [ "$namespaces: {ex: http://example.org/}"; "$schemas: [s.rdf]"; "ex:r: {$import: r.yml}" ]
        testCase "a << key is an ordinary key, plain or quoted, and its entry is an extension field that round trips" <| fun _ ->
            for key in [ "<<"; "\"<<\"" ] do
                let decoded = Decode.decodeCommandLineTool (tool [ "$namespaces: {ex: http://example.org/}"; key + ": {a: 1}"; "ex:marker: <<"; "inputs: []"; "outputs: []" ])
                let _, written = Kit.rewrite key Decode.decodeCommandLineTool Encode.encodeToolDescription decoded
                Expect.stringContains written "\"<<\":" (sprintf "%s: the key is written in quotes.\n%s" key written)
                Expect.stringContains written "ex:marker: \"<<\"" (sprintf "%s: the value is a string written in quotes." key)
            let mapKey = Decode.decodeCommandLineTool (tool [ "inputs:"; "  <<: string"; "outputs: []" ])
            Expect.equal (ListEntry.values mapKey.Inputs.Value).[0].Name "<<" "A << key of an id map is the name of the entry."
        testCase "a << key is an ordinary key of the job file, and a quoted one is too" <| fun _ ->
            for key in [ "<<"; "\"<<\"" ] do
                let references = DecodeParameters.decodeYAMLParameterFile (key + ": {a: 1}\ny: <<\n")
                Expect.equal references.[0].Key "<<" (sprintf "%s: the key stays." key)
                Expect.equal references.[0].Value (Some (CWLParameterValue.Record (ResizeArray [ CWLParameterRecordField("a", CWLParameterValue.Int 1L) ]))) (sprintf "%s: the value is a record." key)
                Expect.equal references.[1].Value (Some (CWLParameterValue.String "<<")) (sprintf "%s: the value << is a text." key)
                let written = Encode.encodeYAMLParameterFile references
                Expect.stringContains written "\"<<\":" (sprintf "%s: the key is written in quotes.\n%s" key written)
                Expect.sequenceEqual (DecodeParameters.decodeYAMLParameterFile written) references (sprintf "%s: the written file should decode to the same references." key)
        testCase "a reserved directive is read, and a tree without a prohibited feature decodes as its text" <| fun _ ->
            Expect.equal (Decode.decodeCommandLineTool ("%FOO bar\n---\n" + toolText [])).CWLVersion "v1.2" "A reserved directive is read."
            Expect.equal (Decode.decodeCWLProcessingUnitElement (Reader.readRoot (toolText []))) (Decode.decodeCWLProcessingUnit (toolText [])) "A tree without a prohibited feature decodes as its text."
    ]

let main =
    testList "Keys" [
        testRepeatedKeys
        testProhibitedFeatures
        testBesideTheTables
    ]
