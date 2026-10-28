/// The layout of the written text: the reader and the writer of YAMLicious under CWL, the doc and label layout, and the
/// one writer for the entries of a list.
module Tests.Layout

open ARCtrl.CWL
open TestingUtils
open DynamicObj
open TestingUtils.CWL
open YAMLicious

/// The text of an optional doc, and none for a doc with a directive.
let docTextOf (doc: DocText option) : string option =
    doc |> Option.bind (fun value -> value.TryText)


/// The text of an optional label, and none for a directive.
let labelTextOf (label: SchemaSaladString option) : string option =
    label |> Option.bind SchemaSaladString.tryLiteral


let testYamlReaderAndWriter =
    let minimalTool = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs: {}
outputs: {}
"""
    testList "YAMLicious Reader and Writer" [
        testCase "a keep-chomped block at the end of a section is not extended by the section separator" <| fun _ ->
            let withKeptBlockBeforeSection =
                Kit.tool
                    [ "baseCommand: echo"
                      "inputs:"
                      "  x:"
                      "    type: string"
                      "    inputBinding:"
                      "      valueFrom: |+"
                      "        $(self)"
                      ""
                      "outputs: {}" ]
            let yaml = withKeptBlockBeforeSection
            let valueFrom (tool: CWLToolDescription) =
                tool.Inputs |> Option.map ListEntry.values |> Option.bind Seq.tryHead |> Option.bind (fun i -> i.InputBinding) |> Option.bind ListEntry.tryValue |> Option.bind (fun b -> b.ValueFrom)
            let decoded = Decode.decodeCommandLineTool yaml
            Expect.equal (valueFrom decoded) (Some (SchemaSaladString.Literal "$(self)\n\n")) "The keep header should keep both line breaks."
            let roundTripped = Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded)
            Expect.equal (valueFrom roundTripped) (Some (SchemaSaladString.Literal "$(self)\n\n")) "The blank line between sections must not become part of the scalar."

        testCase "a keep-chomped block at the end of the document ends with a line break" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs: {}
outputs: {}
arguments:
  - valueFrom: |+
      $(self)

"""
            let rec scalarsOf (node: YamlNode) =
                match node.Kind with
                | YamlNodeKind.Scalar scalar -> [ scalar.Value ]
                | YamlNodeKind.Mapping collection -> collection.Items |> List.collect (fun entry -> scalarsOf entry.Value)
                | YamlNodeKind.Sequence collection -> collection.Items |> List.collect (fun item -> scalarsOf item.Node)
                | YamlNodeKind.Alias _ -> []
            let argumentScalars (tool: CWLToolDescription) = scalarsOf (Expect.wantSome tool.Arguments "Arguments should decode.").Node
            let decoded = Decode.decodeCommandLineTool yaml
            Expect.isTrue (List.contains "$(self)\n\n" (argumentScalars decoded)) "The keep header should keep both line breaks."
            let encoded = Encode.encodeToolDescription decoded
            Expect.isTrue (encoded.EndsWith "\n") "The last blank line of the scalar needs a line break behind it, or other YAML readers drop it."
            Expect.isTrue (List.contains "$(self)\n\n" (argumentScalars (Decode.decodeCommandLineTool encoded))) "The scalar should read back unchanged."

        testCase "empty values in extension fields are written back as they were" <| fun _ ->
            let withEmptyExtensionValues =
                Kit.tool [ "baseCommand: echo"; "inputs: {}"; "outputs: {}"; "arc:note:"; "arc:list:"; "  - a"; "  -"; "  - b"; "arc:flow: {}" ]
            let yaml = withEmptyExtensionValues
            let encoded = Encode.encodeToolDescription (Decode.decodeCommandLineTool yaml)
            let lines = encoded.Replace("\r", "").Split('\n') |> Array.map (fun line -> line.TrimEnd())
            Expect.isTrue (Array.contains "arc:note:" lines) "A key without a value stays without a value."
            let listLines = lines |> Array.skipWhile (fun line -> line <> "arc:list:") |> Array.skip 1 |> Array.take 3
            Expect.sequenceEqual listLines [ "  - a"; "  -"; "  - b" ] "An empty item is written as a dash and not as an empty mapping."
            Expect.isTrue (Array.contains "arc:flow: {}" lines) "An empty flow mapping stays one."

        testCase "a package in the map form without a value has no specs" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
requirements:
  - class: SoftwareRequirement
    packages:
      samtools:
      bwa: {}
inputs: {}
outputs: {}
"""
            let decoded = Decode.decodeCommandLineTool yaml
            let packages =
                decoded.Requirements
                |> Option.get
                |> Seq.pick (function SoftwareRequirement packages -> Some packages | _ -> None)
            Expect.sequenceEqual (ListEntry.values packages |> Seq.map (fun p -> p.Package)) [ SchemaSaladString.Literal "samtools"; SchemaSaladString.Literal "bwa" ] "Both packages should decode."
            Expect.isTrue (ListEntry.values packages |> Seq.forall (fun p -> p.Specs.IsNone)) "Neither package has specs."

        testCase "quoted booleans and integers in typed fields fail like schema validation" <| fun _ ->
            let tool (lines: string list) = Kit.tool ("baseCommand: echo" :: lines)
            let inputBinding field = tool [ "inputs:"; "  x:"; "    type: string"; "    inputBinding:"; "      " + field; "outputs: {}" ]
            let inputPort field = tool [ "inputs:"; "  x:"; "    type: string"; "    " + field; "outputs: {}" ]
            let outputPort field = tool [ "inputs: {}"; "outputs:"; "  o:"; "    type: File"; "    " + field ]
            let outputBinding field = tool [ "inputs: {}"; "outputs:"; "  o:"; "    type: File"; "    outputBinding:"; "      glob: a"; "      " + field ]
            let dirent field = tool [ "requirements:"; "  - class: InitialWorkDirRequirement"; "    listing:"; "      - entryname: a"; "        entry: b"; "        " + field; "inputs: {}"; "outputs: {}" ]
            let inplaceUpdate field = tool [ "requirements:"; "  - class: InplaceUpdateRequirement"; "    " + field; "inputs: {}"; "outputs: {}" ]
            let exitCodes field = tool [ field; "inputs: {}"; "outputs: {}" ]
            let workflowStep field =
                Kit.workflow [ "inputs:"; "  a:"; "    type: File"; "outputs: {}"; "steps:"; "  s:"; "    run: other.cwl"; "    in:"; "      x:"; "        source: a"; "        " + field; "    out: []" ]
            let decodeTool (yaml: string) = Decode.decodeCommandLineTool yaml |> ignore
            let decodeWorkflow (yaml: string) = Decode.decodeWorkflow yaml |> ignore
            let decodeParameters (yaml: string) = DecodeParameters.decodeYAMLParameterFile yaml |> ignore
            let fileWithSize = """f:
  class: File
  path: a.txt
  size: 42
"""
            let fileWithQuotedSize = """f:
  class: File
  path: a.txt
  size: "42"
"""
            let cases =
                [ "inputBinding position", decodeTool, inputBinding "position: 1", inputBinding "position: \"1\""
                  "inputBinding separate", decodeTool, inputBinding "separate: false", inputBinding "separate: \"false\""
                  "inputBinding loadContents", decodeTool, inputBinding "loadContents: true", inputBinding "loadContents: \"true\""
                  "inputBinding shellQuote", decodeTool, inputBinding "shellQuote: false", inputBinding "shellQuote: \"false\""
                  "input streamable", decodeTool, inputPort "streamable: true", inputPort "streamable: \"true\""
                  "output streamable", decodeTool, outputPort "streamable: true", outputPort "streamable: \"true\""
                  "outputBinding loadContents", decodeTool, outputBinding "loadContents: true", outputBinding "loadContents: \"true\""
                  "Dirent writable", decodeTool, dirent "writable: true", dirent "writable: \"true\""
                  "inplaceUpdate", decodeTool, inplaceUpdate "inplaceUpdate: true", inplaceUpdate "inplaceUpdate: \"true\""
                  "successCodes", decodeTool, exitCodes "successCodes: [0, 1]", exitCodes "successCodes: [\"0\", \"1\"]"
                  "step input loadContents", decodeWorkflow, workflowStep "loadContents: true", workflowStep "loadContents: \"true\""
                  "File size", decodeParameters, fileWithSize, fileWithQuotedSize ]
            for (name, decode, plain, quoted) in cases do
                decode plain
                Expect.throws (fun () -> decode quoted) (sprintf "A quoted value for %s is a string and must fail." name)
            // The core schema also reads these plain spellings as a bool and an int.
            decodeTool (inputBinding "shellQuote: False")
            decodeTool (inputBinding "position: 0x2")

        testCase "$namespaces and $schemas are kept as mapping and list through decode and encode" <| fun _ ->
            let withNamespacesAndSchemas =
                Kit.tool
                    [ "baseCommand: echo"
                      "$namespaces:"
                      "  cwltool: \"http://commonwl.org/cwltool#\""
                      "  arc: \"https://github.com/nfdi4plants/ARC_ontology\""
                      "$schemas:"
                      "  - \"https://example.org/a.owl\""
                      "  - \"https://example.org/b.yml\""
                      "inputs: {}"
                      "outputs: {}" ]
            let yaml = withNamespacesAndSchemas
            let check (tool: CWLToolDescription) =
                let metadata = Expect.wantSome tool.Metadata "The extension fields should be kept."
                let namespaces = Expect.wantSome (DynObj.tryGetTypedPropertyValue<DynamicObj> "$namespaces" metadata) "$namespaces should be a mapping."
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "cwltool" namespaces) (Some "http://commonwl.org/cwltool#") "The cwltool prefix should be kept."
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc" namespaces) (Some "https://github.com/nfdi4plants/ARC_ontology") "The arc prefix should be kept."
                Expect.isSome (DynObj.tryGetTypedPropertyValue<obj> "$schemas" metadata) "$schemas should be kept."
            let decoded = Decode.decodeCommandLineTool yaml
            check decoded
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded "$namespaces:" "The namespaces should be written as a mapping key."
            Expect.stringContains encoded "cwltool: http://commonwl.org/cwltool#" "The prefixes should be written as nested keys."
            Expect.stringContains encoded "$schemas:" "The schemas should be written as a key."
            Expect.stringContains encoded "https://example.org/a.owl" "The first schema should be written."
            Expect.stringContains encoded "https://example.org/b.yml" "The second schema should be written."
            check (Decode.decodeCommandLineTool encoded)
            Expect.equal (Encode.encodeToolDescription (Decode.decodeCommandLineTool encoded)) encoded "A second pass should write the same text."

        testCase "a text with two documents is a parse error" <| fun _ ->
            let yaml = minimalTool + "---\nfoo: 1\n"
            let parseError =
                try
                    Decode.decodeCommandLineTool yaml |> ignore
                    None
                with YamlParseException error -> Some error
            Expect.isSome parseError "A CWL file holds one document."

        testCase "a parameter file without content has no references" <| fun _ ->
            let onlyAComment = """# only a comment
"""
            let onlyADocumentMarker = """---
"""
            for text in [ ""; onlyAComment; onlyADocumentMarker ] do
                let references = DecodeParameters.decodeYAMLParameterFile text
                Expect.equal references.Count 0 (sprintf "The text %A holds no parameter." text)

        testCase "an empty value where CWL requires a value is a decode error with the full path" <| fun _ ->
            let tool (lines: string list) = Kit.tool ("baseCommand: echo" :: lines)
            let requirement (lines: string list) = tool ([ "requirements:" ] @ lines @ [ "inputs: {}"; "outputs: {}" ])
            let workflow (lines: string list) = Kit.workflowWithSteps ("  s:" :: lines)
            let steps (lines: string list) = Kit.workflowWithSteps ([ "  - id: s"; "    run: t.cwl" ] @ lines)
            let emptyCwlVersion = """cwlVersion:
class: CommandLineTool
baseCommand: echo
inputs: {}
outputs: {}
"""
            let emptyClass = """cwlVersion: v1.2
class:
inputs: {}
outputs: {}
"""
            let emptyBaseCommandItem = """cwlVersion: v1.2
class: CommandLineTool
baseCommand:
  - echo
  -
inputs: {}
outputs: {}
"""
            let emptyExpression = """cwlVersion: v1.2
class: ExpressionTool
inputs: {}
outputs: {}
expression:
"""
            let emptyStepRun = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  - id: s
    run:
    in: {}
    out: []
"""
            // the path that the error must show, and a document that holds an empty value there
            let cases =
                [ "cwlVersion", emptyCwlVersion
                  "class", emptyClass
                  "baseCommand[1]", emptyBaseCommandItem
                  // ports in map form and in sequence form
                  "inputs.x", tool [ "inputs:"; "  x:"; "outputs: {}" ]
                  "outputs.o", tool [ "inputs: {}"; "outputs:"; "  o:" ]
                  "inputs.x.type", tool [ "inputs:"; "  - id: x"; "    type:"; "outputs: {}" ]
                  "inputs.x.type[1]", tool [ "inputs:"; "  - id: x"; "    type:"; "      - string"; "      -"; "outputs: {}" ]
                  "outputs.o.type", tool [ "inputs: {}"; "outputs:"; "  - id: o"; "    type:" ]
                  "inputs.a.type", Kit.workflow [ "inputs:"; "  - id: a"; "    type:"; "outputs: {}"; "steps: {}" ]
                  "outputs.o.type", Kit.workflow [ "inputs: {}"; "outputs:"; "  - id: o"; "    type:"; "steps: {}" ]
                  "inputs.x.type.symbols[1]", tool [ "inputs:"; "  x:"; "    type:"; "      type: enum"; "      symbols:"; "        - a"; "        -"; "outputs: {}" ]
                  // requirements
                  "requirements[0].envDef[0].envName", requirement [ "  - class: EnvVarRequirement"; "    envDef:"; "      - envName:"; "        envValue: a" ]
                  "requirements[0].envDef[0].envValue", requirement [ "  - class: EnvVarRequirement"; "    envDef:"; "      - envName: A"; "        envValue:" ]
                  "requirements[0].envDef[0].envValue", requirement [ "  - class: EnvVarRequirement"; "    envDef:"; "      - envName: A"; "        envValue: null" ]
                  "requirements[0].envDef.A", requirement [ "  - class: EnvVarRequirement"; "    envDef:"; "      A:" ]
                  "requirements.EnvVarRequirement.envDef.A", tool [ "requirements:"; "  EnvVarRequirement:"; "    envDef:"; "      A:"; "inputs: {}"; "outputs: {}" ]
                  "requirements[0].packages[0].package", requirement [ "  - class: SoftwareRequirement"; "    packages:"; "      - package:" ]
                  "requirements[0].packages[0].specs[0]", requirement [ "  - class: SoftwareRequirement"; "    packages:"; "      - package: a"; "        specs:"; "          -" ]
                  "requirements[0].packages.samtools.specs[0]", requirement [ "  - class: SoftwareRequirement"; "    packages:"; "      samtools:"; "        specs:"; "          -" ]
                  "requirements[0].listing", requirement [ "  - class: InitialWorkDirRequirement"; "    listing:" ]
                  "requirements[0].listing[0].entry", requirement [ "  - class: InitialWorkDirRequirement"; "    listing:"; "      - entryname: a"; "        entry:" ]
                  "requirements[0].timelimit", requirement [ "  - class: ToolTimeLimit"; "    timelimit:" ]
                  "requirements[0].class", requirement [ "  - class:" ]
                  "expression", emptyExpression
                  // steps in map form and in sequence form
                  "steps.s.run", workflow [ "    run:"; "    in: {}"; "    out: []" ]
                  "steps.s.in.x", workflow [ "    run: t.cwl"; "    in:"; "      x:"; "    out: []" ]
                  "steps.s.out", workflow [ "    run: t.cwl"; "    in: {}"; "    out:" ]
                  "steps.s.out[1]", workflow [ "    run: t.cwl"; "    in: {}"; "    out:"; "      - a"; "      -" ]
                  "steps.s.run", emptyStepRun
                  "steps.s.out", steps [ "    in: {}"; "    out:" ]
                  "steps.s.in.x", steps [ "    in:"; "      x:"; "    out: []" ]
                  "steps.s.in.x.source[1]", steps [ "    in:"; "      - id: x"; "        source:"; "          - a"; "          -"; "    out: []" ]
                  "steps.s.out[0].id", steps [ "    in: {}"; "    out:"; "      - id:" ] ]
            for (path, yaml) in cases do
                Kit.processFails (sprintf "An empty value at %s in\n%s" path yaml) yaml path "Expected a value, found"
            // a quoted null is a string, and the empty list of out is not an empty value
            let quoted = requirement [ "  - class: EnvVarRequirement"; "    envDef:"; "      - envName: A"; "        envValue: \"null\"" ]
            Expect.isSome (Decode.decodeCommandLineTool quoted).Requirements "A quoted null is the string null."
            let decoded = Decode.decodeWorkflow (workflow [ "    run: t.cwl"; "    in: {}"; "    out: []" ])
            Expect.equal (ListEntry.values decoded.Steps).[0].Out.Count 0 "out: [] has no outputs."

        testCase "encoded CWL has compact list items, inline scalar lists and line feeds only" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: [dotnet, fsi, script.fsx]
requirements:
  - class: InlineJavascriptRequirement
  - class: NetworkAccess
    networkAccess: true
inputs: {}
outputs: {}
"""
            let encoded = Encode.encodeToolDescription (Decode.decodeCommandLineTool yaml)
            Expect.stringContains encoded "baseCommand: [dotnet, fsi, script.fsx]" "A list of single-line scalars stays inline."
            Expect.stringContains encoded "  - class: InlineJavascriptRequirement" "A list item that holds a mapping starts on the line of its dash."
            Expect.stringContains encoded "  - class: NetworkAccess" "Every item is written this way."
            Expect.isFalse (encoded.Contains "\r") "The writer ends every line with a line feed."

        testCase "raw YAML kept in a CWL object compares and hashes by value" <| fun _ ->
            let withDefaultsAndExtensionField =
                Kit.tool
                    [ "baseCommand: echo"
                      "inputs:"
                      "  x:"
                      "    type: File"
                      "    secondaryFiles: [.bai, ^.idx]"
                      "    default: {class: File, path: a.txt}"
                      "  y:"
                      "    type: string"
                      "    default: one"
                      "outputs: {}"
                      "arc:note:" ]
            let yaml = withDefaultsAndExtensionField
            let first = Decode.decodeCommandLineTool yaml
            let second = Decode.decodeCommandLineTool yaml
            Kit.expectSame "two decodes of the same port" (box (ListEntry.values first.Inputs.Value).[0]) (box (ListEntry.values second.Inputs.Value).[0])
            Expect.isTrue (DynamicObjHelpers.dynamicPropertiesEqual first.Metadata.Value second.Metadata.Value) "A preserved null compares by its node."
            Expect.equal (DynamicObjHelpers.hashDynamicProperties first.Metadata.Value) (DynamicObjHelpers.hashDynamicProperties second.Metadata.Value) "Equal extension fields have the same hash."
            Expect.equal first second "Two decodes of the same text are equal tools."
            let changed = Decode.decodeCommandLineTool (yaml.Replace("default: one", "default: two"))
            Expect.notEqual (ListEntry.values first.Inputs.Value).[1] (ListEntry.values changed.Inputs.Value).[1] "A different default value is a different port."
            Expect.notEqual first changed "A different default value is a different tool."

        testCase "raw YAML with comments is equal after a roundtrip" <| fun _ ->
            let withComments =
                Kit.tool
                    [ "baseCommand: echo"
                      "hints:"
                      "  - class: acme:Hint # trailing comment"
                      "    # leading comment"
                      "    list:"
                      "      - a # first"
                      "      - b"
                      "inputs:"
                      "  x:"
                      "    type: File"
                      "    secondaryFiles:"
                      "      # about files"
                      "      - .bai # index"
                      "    default:"
                      "      class: File # the class"
                      "      path: a.txt"
                      "outputs: {}" ]
            let yaml = withComments
            let decoded = Decode.decodeCommandLineTool yaml
            let roundTripped, _ = Kit.rewrite "the data of the raw values" Decode.decodeCommandLineTool Encode.encodeToolDescription decoded
            Kit.expectSame "the tool read back" (box roundTripped) (box decoded)

        testCase "an expression keeps its line feeds in front of a metadata section" <| fun _ ->
            let expressionTool (header: string) (blankLines: string) (metadata: string) =
                Kit.expressionTool [ "inputs: {}"; "outputs: {}"; "expression: " + header; "  ${"; "    return {};"; "  }" ] + blankLines + metadata
            // the header, the blank lines behind the body and the value that they give
            let cases =
                [ "|-", "", "${\n  return {};\n}"
                  "|", "", "${\n  return {};\n}\n"
                  "|+", "\n", "${\n  return {};\n}\n\n"
                  "|+", "\n\n", "${\n  return {};\n}\n\n\n" ]
            for (header, blankLines, expected) in cases do
                for metadata in [ "s:author: me\n"; "" ] do
                    let decoded = Decode.decodeExpressionTool (expressionTool header blankLines metadata)
                    Expect.equal decoded.Expression (SchemaSaladString.Literal expected) (sprintf "The header %s should give this value." header)
                    let roundTripped, encoded = Kit.rewrite "the round trip" Decode.decodeExpressionTool Encode.encodeExpressionToolDescription decoded
                    Expect.equal roundTripped.Expression (SchemaSaladString.Literal expected) (sprintf "The expression with %s and %d blank lines should survive a write and a read, written as:\n%s" header blankLines.Length encoded)
                    Expect.isTrue (encoded.EndsWith "\n") "The text ends with a line feed."
                    if metadata <> "" then
                        let author = Expect.wantSome (DynObj.tryGetTypedPropertyValue<string> "s:author" roundTripped.Metadata.Value) "The metadata should survive."
                        Expect.equal author "me" "The metadata value should be kept."
                        // The line feeds between the closing brace and the next key are the end of the body line and one blank
                        // line, or the line feeds that the value keeps when there are more of them.
                        let bodyEnd = encoded.IndexOf "  }" + 3
                        let between = encoded.Substring(bodyEnd, encoded.IndexOf "s:author" - bodyEnd)
                        let keptLineFeeds = expected.Length - expected.TrimEnd('\n').Length
                        Expect.isTrue (between |> Seq.forall (fun c -> c = '\n')) "Only line feeds stand between the sections."
                        Expect.equal between.Length (max 2 keptLineFeeds) "One blank line separates the sections, and a kept blank line is that separator."

        testCase "raw YAML values keep the comments and the blank lines of their source" <| fun _ ->
            let withCommentsAndBlankLines =
                Kit.tool
                    [ "baseCommand: echo"
                      "hints:"
                      "  - class: acme:Hint # hint comment"
                      "    # leading comment of a field"
                      "    list:"
                      "      - a # item comment"
                      "inputs:"
                      "  x:"
                      "    type: File"
                      "    secondaryFiles:"
                      "      # about files"
                      "      - .bai # index"
                      "    default:"
                      "      class: File # class comment"
                      "      path: a.txt"
                      "arguments:"
                      "  # inside arguments"
                      "  - valueFrom: x # trailing"
                      ""
                      "  - z"
                      "outputs: {}" ]
            let yaml = withCommentsAndBlankLines
            let encoded = Encode.encodeToolDescription (Decode.decodeCommandLineTool yaml)
            for comment in [ "# hint comment"; "# leading comment of a field"; "# item comment"; "# about files"; "# index"; "# class comment"; "# inside arguments"; "# trailing" ] do
                Expect.stringContains encoded comment (sprintf "The comment %s belongs to a raw value and should be written back." comment)
            Expect.stringContains encoded "# trailing\n\n  - z" "The blank line between the two arguments should be written back."

        testCase "a raw value of depth 100000 in flow and in block form decodes, compares, hashes, keeps its last comment and writes without a stack overflow" <| fun _ ->
            let depth = 100000
            let tool (value: string) = Kit.tool [ "baseCommand: echo"; "outputs: []"; "inputs:"; "  x:"; "    type: string"; "    default:" + value ]
            // The block value ends in a comment, which moves when the writer puts the outputs behind the inputs.
            for (value, comment) in [ " " + String.replicate depth "[" + "x" + String.replicate depth "]", false; "\n      " + String.replicate depth "- " + "x\n      # tail", true ] do
                let first = Decode.decodeCommandLineTool (tool value)
                let second = Decode.decodeCommandLineTool (tool value)
                Kit.expectSame "two decodes of the same text" (box first) (box second)
                let _, written = Kit.roundTrip "depth" Decode.decodeCommandLineTool Encode.encodeToolDescription (tool value)
                if comment then
                    Expect.stringContains written "# tail\n" "The comment should be written."
                    Expect.equal (written.Split("# tail").Length) 2 "The comment should be written once."

        testCase "an extension field keeps the keys of a mapping with their types, and a field of an object needs a text as its name" <| fun _ ->
            let tool (lines: string list) =
                Kit.lines ([ "cwlVersion: v1.2"; "$namespaces: {ex: \"http://example.org/\"}"; "class: CommandLineTool"; "baseCommand: echo"; "inputs: []"; "outputs: []" ] @ lines)
            let decoded = Decode.decodeCommandLineTool (tool [ "ex:meta:"; "  1: a"; "  \"1\": b"; "  null: e"; "  \"null\": f"; "ex:flags:"; "  true: c"; "  \"true\": d"; "ex:list:"; "  - {1: a, \"1\": b}" ])
            let _, written = Kit.rewrite "the round trip" Decode.decodeCommandLineTool Encode.encodeToolDescription decoded
            for expected in [ "1: a"; "\"1\": b"; "true: c"; "\"true\": d"; "null: e"; "\"null\": f" ] do
                Expect.stringContains written expected (sprintf "The key keeps its spelling: %s\n%s" expected written)
            Expect.notEqual decoded (Decode.decodeCommandLineTool (tool [ "ex:meta:"; "  \"1\": a"; "  \"1x\": b"; "  null: e"; "  \"null\": f"; "ex:flags:"; "  true: c"; "  \"true\": d"; "ex:list:"; "  - {1: a, \"1\": b}" ])) "Another key makes another tool."
            for (name, key, tag) in [ "an int", "1", "!!int"; "a bool", "true", "!!bool"; "a null", "~", "!!null"; "a float", "1.5", "!!float" ] do
                match Kit.decodeError (fun () -> Decode.decodeCommandLineTool (tool [ key + ": x" ])) with
                | Some error ->
                    Expect.stringContains error.Message "Expected a string as a field name, found" (sprintf "%s as the name of a field: the error should say what is wrong." name)
                    Expect.stringContains error.Message ("which resolves to " + tag) (sprintf "%s as the name of a field: the error should name the type of the key." name)
                    Expect.isSome error.Span (sprintf "%s as the name of a field: the error should have the place in the text." name)
                | None -> failwith (sprintf "%s as the name of a field must fail." name)
            Expect.isSome (Kit.decodeError (fun () -> Decode.decodeCommandLineTool (tool [ "ex:x: 1"; "1: x" ]))) "A key of another type fails also behind a valid field."
        testCase "a raw hint that starts with a comment or a blank line keeps it in the hint and is written behind a dash with a line break" <| fun _ ->
            let tool (hints: string list) = Kit.tool ([ "baseCommand: echo"; "inputs: []"; "outputs: []"; "hints:" ] @ hints)
            let commentLines (text: string) = text.Split('\n') |> Array.filter (fun line -> line.TrimStart().StartsWith "#") |> Array.length
            let cases =
                [ "array form, comment in front of the first field",
                  tool [ "  -"; "    # about the hint"; "    class: acme:Hint"; "    a: 1" ],
                  "hints:\n  -\n    # about the hint\n    class: acme:Hint\n    a: 1\n"
                  "array form, blank line and comment in front of the first field",
                  tool [ "  - class: acme:Other"; "  -"; ""; "    # about the hint"; "    class: acme:Hint"; "    a: 1" ],
                  "  - class: acme:Other\n  -\n\n    # about the hint\n    class: acme:Hint\n    a: 1\n"
                  "map form with the class in the payload",
                  tool [ "  acme:Hint:"; "    # about the hint"; "    class: acme:Hint"; "    a: 1" ],
                  "hints:\n  -\n    # about the hint\n    class: acme:Hint\n    a: 1\n"
                  "map form without the class in the payload",
                  tool [ "  acme:Hint:"; "    # about the hint"; "    a: 1" ],
                  "hints:\n  - class: acme:Hint\n    # about the hint\n    a: 1\n" ]
            for (name, yaml, expected) in cases do
                let decoded = Decode.decodeCommandLineTool yaml
                let _, encoded = Kit.rewrite (sprintf "%s: the written text should decode to the same tool." name) Decode.decodeCommandLineTool Encode.encodeToolDescription decoded
                Expect.stringContains encoded expected (sprintf "%s: the comment should stay in front of the first field." name)
                Expect.equal (commentLines encoded) (commentLines yaml) (sprintf "%s: the comment should be written once." name)

        testCase "a comment at the end of a raw value moves in front of its last entry when the value is no longer the last node of the document" <| fun _ ->
            let stepInputDefaultInSequenceForm =
                Kit.workflow
                    [ "inputs: []"
                      "outputs: []"
                      "steps:"
                      "  - id: s"
                      "    run: t.cwl"
                      "    out: []"
                      "    in:"
                      "      - id: x"
                      "        default:"
                      "          - 1"
                      "          - 2"
                      "          # tail" ]
            let stepInputDefaultInMapForm =
                Kit.workflow
                    [ "inputs: []"
                      "outputs: []"
                      "steps:"
                      "  s:"
                      "    run: t.cwl"
                      "    out: []"
                      "    in:"
                      "      x:"
                      "        default:"
                      "          k: v"
                      "          # tail" ]
            let header = Kit.tool [ "baseCommand: echo" ]
            let cases =
                [ // the name, the document and the text that the written document holds
                  "hints in array form, with blank lines at the end of the document",
                  header + "inputs: []\noutputs: []\nhints:\n  - class: acme:Unknown\n    foo: bar\n    # baz: qux\n\n\n",
                  "  - class: acme:Unknown\n    # baz: qux\n    foo: bar\n\nbaseCommand: [echo]\n"
                  "hints in map form",
                  header + "inputs: []\noutputs: []\nhints:\n  acme:Unknown:\n    foo: bar\n    # baz: qux\n",
                  "  - class: acme:Unknown\n    # baz: qux\n    foo: bar\n\nbaseCommand: [echo]\n"
                  "arguments in front of extension metadata",
                  header + "inputs: []\noutputs: []\ns:author: me\narguments:\n  - valueFrom: x\n    # tail\n",
                  "  -\n    # tail\n    valueFrom: x\ns:author: me\n"
                  "input default as a mapping",
                  header + "outputs: []\ninputs:\n  x:\n    type: string\n    default:\n      a: 1\n      # tail\n",
                  "      # tail\n      a: 1\n\noutputs: {}\n"
                  "input default as a sequence",
                  header + "outputs: []\ninputs:\n  x:\n    type: string\n    default:\n      - a\n      - b\n      # tail\n",
                  "      - a\n      # tail\n      - b\n\noutputs: {}\n"
                  "input default in front of a field of the same input",
                  header + "outputs: []\ninputs:\n  x:\n    type: string\n    inputBinding:\n      prefix: -x\n    default:\n      a: 1\n      # tail\n",
                  "      # tail\n      a: 1\n    inputBinding:\n"
                  "nested collections that each end with a comment",
                  header + "outputs: []\ninputs:\n  x:\n    type: string\n    default:\n      a:\n        b:\n          - 1\n          # deep\n        # middle\n      # outer\n",
                  "      # outer\n      a:\n        # middle\n        b:\n          # deep\n          - 1\n\noutputs: {}\n"
                  "secondaryFiles of an input",
                  header + "outputs: []\ninputs:\n  x:\n    type: File\n    secondaryFiles:\n      - pattern: .bai\n        # tail\n",
                  "      -\n        # tail\n        pattern: .bai\n\noutputs: {}\n"
                  "default of a step input in map form",
                  stepInputDefaultInMapForm,
                  "          # tail\n          k: v\n    out: []\n"
                  "default of a step input in sequence form",
                  stepInputDefaultInSequenceForm,
                  "          - 1\n          # tail\n          - 2\n    out: []\n"
                  "a kept block scalar before the comment",
                  header + "outputs: []\ninputs:\n  x:\n    type: string\n    default:\n      a: |+\n        abc\n\n      # tail\n",
                  "      # tail\n      a: |+\n        abc\n\noutputs: {}\n" ]
            let commentLines (text: string) = text.Split('\n') |> Array.filter (fun line -> line.TrimStart().StartsWith "#") |> Array.length
            for (name, yaml, expected) in cases do
                let decoded = Decode.decodeCWLProcessingUnit yaml
                let _, encoded = Kit.rewrite (sprintf "%s: the written text should decode to the same document." name) Decode.decodeCWLProcessingUnit Encode.encodeProcessingUnit decoded
                Expect.stringContains encoded expected (sprintf "%s: the comment should stand in front of the last entry." name)
                Expect.equal (commentLines encoded) (commentLines yaml) (sprintf "%s: every comment should be written once." name)

        testCase "a map-form hint without a payload is the hint with an empty payload" <| fun _ ->
            let tool (hints: string list) = Kit.tool ([ "baseCommand: echo"; "inputs: []"; "outputs: []"; "hints:" ] @ hints)
            for hint in [ "acme:Hint"; "ShellCommandRequirement"; "WorkReuse" ] do
                let withoutPayload = Decode.decodeCommandLineTool (tool [ "  " + hint + ":" ])
                let emptyPayload = Decode.decodeCommandLineTool (tool [ "  " + hint + ": {}" ])
                Expect.equal withoutPayload emptyPayload (sprintf "%s without a payload should equal %s with {}." hint hint)
                let _, encoded = Kit.rewrite (sprintf "%s: a round trip should keep the hint." hint) Decode.decodeCommandLineTool Encode.encodeToolDescription withoutPayload
                Expect.isFalse (encoded.Contains "value:") (sprintf "%s: the hint has no value field.\n%s" hint encoded)
            let encoded = Encode.encodeToolDescription (Decode.decodeCommandLineTool (tool [ "  acme:Hint:" ]))
            Expect.stringContains encoded "class: acme:Hint" "The unknown hint should keep its class."

        testCase "a raw block scalar that keeps its line feeds is written back with them in front of a section" <| fun _ ->
            let tool (scalar: string) = Kit.tool [ "baseCommand: echo"; "inputs:"; "  x:"; "    type: string" ] + "    default: " + scalar + "outputs: []\n"
            let cases =
                [ // the source, the text that follows default: in the written document
                  "|+\n", "|+\noutputs: {}\n"
                  "|+\n      abc\n", "|+\n      abc\noutputs: {}\n"
                  "|+\n      abc\n\n", "|+\n      abc\n\noutputs: {}\n"
                  "|+\n      abc\n\n\n", "|+\n      abc\n\n\noutputs: {}\n"
                  "|\n      abc\n", "|\n      abc\n\noutputs: {}\n"
                  "|\n", "|\n\noutputs: {}\n"
                  "|-\n      abc\n", "|-\n      abc\n\noutputs: {}\n"
                  "|-\n", "|-\n\noutputs: {}\n" ]
            for (source, expected) in cases do
                let decoded = Decode.decodeCommandLineTool (tool source)
                let _, encoded = Kit.rewrite (sprintf "The scalar %s should read back as the same tool." (source.Replace("\n", "\\n"))) Decode.decodeCommandLineTool Encode.encodeToolDescription decoded
                Expect.stringContains encoded expected (sprintf "The scalar %s should be written back with its line feeds." (source.Replace("\n", "\\n")))

        testCase "kept block scalars inside a raw value stay in front of the next section and of a comment" <| fun _ ->
            let header = Kit.tool [ "baseCommand: echo" ]
            let cases =
                [ // the document and the text that follows the raw value in the written document
                  "a kept scalar, then a comment at the end of the document",
                  header + "outputs: []\ninputs:\n  x:\n    type: string\n    default:\n      a: |+\n        abc\n\n      # tail\n",
                  "      # tail\n      a: |+\n        abc\n\noutputs: {}\n"
                  "kept scalars at the end of nested mappings",
                  header + "inputs:\n  x:\n    type: string\n    default:\n      a: |+\n        abc\n\n      b:\n        c: |+\n          q\n\noutputs: []\n",
                  "          q\n\noutputs: {}\n"
                  "a kept scalar as the last item of a sequence",
                  header + "inputs:\n  x:\n    type: string\n    default:\n      - a\n      - |+\n        abc\n\noutputs: []\n",
                  "        abc\n\noutputs: {}\n" ]
            for (name, yaml, expected) in cases do
                let decoded = Decode.decodeCommandLineTool yaml
                let _, encoded = Kit.rewrite (sprintf "%s: the written text should read back as the same tool." name) Decode.decodeCommandLineTool Encode.encodeToolDescription decoded
                Expect.stringContains encoded expected (sprintf "%s: the lines of the scalar should come first." name)

        testCase "a record field keeps its type and its other properties apart, in map and array form, for input and output records" <| fun _ ->
            // the lines of the type of the field, and the type that the decoder must find (None where the text is the check)
            let types : (string * string list * CWLType option) list =
                [ "File", [ "type: File" ], Some (CWLType.File)
                  "Directory", [ "type: Directory" ], Some (CWLType.Directory)
                  "string", [ "type: string" ], Some CWLType.String
                  "int", [ "type: int" ], Some CWLType.Int
                  "long", [ "type: long" ], Some CWLType.Long
                  "float", [ "type: float" ], Some CWLType.Float
                  "double", [ "type: double" ], Some CWLType.Double
                  "boolean", [ "type: boolean" ], Some CWLType.Boolean
                  "optional File", [ "type: File?" ], Some (Union (ResizeArray [ Null; CWLType.File ]))
                  "File array", [ "type: File[]" ], Some (Array (InputArraySchema(CWLType.File)))
                  "union list", [ "type:"; "  - \"null\""; "  - File" ], Some (Union (ResizeArray [ Null; CWLType.File ]))
                  "union of two types", [ "type:"; "  - string"; "  - int" ], Some (Union (ResizeArray [ CWLType.String; CWLType.Int ]))
                  "array schema", [ "type:"; "  type: array"; "  items: File" ], Some (Array (InputArraySchema(CWLType.File)))
                  "nested array schema", [ "type:"; "  type: array"; "  items:"; "    type: array"; "    items: string" ], None
                  "array of records", [ "type:"; "  type: array"; "  items:"; "    type: record"; "    fields:"; "      g:"; "        type: File"; "        inputBinding:"; "          prefix: --g" ], None
                  "enum", [ "type:"; "  type: enum"; "  symbols: [a, b]" ], None
                  "nested record", [ "type:"; "  type: record"; "  fields:"; "    g:"; "      type: File"; "      inputBinding:"; "        prefix: --g"; "    h: string" ], None ]
            let indent (count: int) (lines: string list) = lines |> List.map (fun line -> System.String(' ', count) + line)
            for isOutput in [ false; true ] do
                let binding = if isOutput then "outputBinding" else "inputBinding"
                let bindingLines = if isOutput then [ "outputBinding:"; "  glob: x" ] else [ "inputBinding:"; "  prefix: --f" ]
                let propertySets : (string * string list * string list) list =
                    [ "no other property", [], []
                      "a binding", bindingLines, [ binding ]
                      "all properties", [ "doc: the field"; "label: The field"; "secondaryFiles: .bai"; "streamable: true" ] @ bindingLines, [ "doc"; "label"; "secondaryFiles"; "streamable"; binding ] ]
                for isArrayForm in [ false; true ] do
                    for (typeName, typeLines, expectedType) in types do
                        for (propertyName, propertyLines, propertyKeys) in propertySets do
                            let field = (if isArrayForm then [ "        - name: f" ] else [ "        f:" ]) @ indent 10 (typeLines @ propertyLines)
                            let record = [ "      type: record"; "      fields:" ]
                            let lines =
                                if isOutput then [ "inputs:"; "  x: string"; "outputs:"; "  o:"; "    type:" ] @ record @ field
                                else [ "inputs:"; "  r:"; "    type:" ] @ record @ field @ [ "outputs: []" ]
                            let yaml = Kit.tool ("baseCommand: echo" :: lines)
                            let name = sprintf "%s, %s form, %s, %s" typeName (if isArrayForm then "array" else "map") propertyName (if isOutput then "output" else "input")
                            let decoded = Decode.decodeCommandLineTool yaml
                            let port = if isOutput then (ListEntry.values decoded.Outputs).[0].Type_ else (ListEntry.values decoded.Inputs.Value).[0].Type_
                            let recordField =
                                match port with
                                | Some (Record schema) -> (InputRecordSchema.getFieldValuesOrEmpty schema).[0]
                                | other -> failwithf "%s: expected a record, found %A" name other
                            Expect.equal recordField.Name "f" (sprintf "%s: the field name" name)
                            match expectedType with
                            | Some expected -> Expect.isTrue (CWLType.typesEqual expected recordField.Type) (sprintf "%s: the type of the field is %A, found %A" name expected recordField.Type)
                            | None -> ()
                            let fieldKeys = recordField.GetProperties(false) |> Seq.map (fun property -> property.Key) |> Seq.toList
                            for key in propertyKeys do
                                if key <> "doc" && key <> "label" then
                                    Expect.isTrue (List.contains key fieldKeys) (sprintf "%s: the property %s stays on the field" name key)
                            let _, encoded = Kit.rewrite "the round trip" Decode.decodeCommandLineTool Encode.encodeToolDescription decoded
                            for key in propertyKeys do
                                Expect.equal (encoded.Split(key + ":").Length - 1) (yaml.Split(key + ":").Length - 1) (sprintf "%s: %s is written once\n%s" name key encoded)

        testCase "a record field in map form can be the type itself or a type list" <| fun _ ->
            let tool (fields: string list) =
                Kit.tool ([ "baseCommand: echo"; "inputs:"; "  r:"; "    type:"; "      type: record"; "      fields:" ] @ fields @ [ "outputs: []" ])
            let yaml =
                tool [ "        a: File"
                       "        b: string?"
                       "        c: [\"null\", File]"
                       "        d:"
                       "          type: File"
                       "          doc: object form" ]
            let decoded = Decode.decodeCommandLineTool yaml
            let fields = match (ListEntry.values decoded.Inputs.Value).[0].Type_ with | Some (Record schema) -> (InputRecordSchema.getFieldValuesOrEmpty schema) | other -> failwithf "Expected a record, found %A" other
            Expect.sequenceEqual (fields |> Seq.map (fun field -> field.Name)) [ "a"; "b"; "c"; "d" ] "The fields keep their order."
            Expect.isTrue (CWLType.typesEqual (CWLType.File) fields.[0].Type) "A field can be given as the type."
            Expect.isTrue (CWLType.typesEqual (Union (ResizeArray [ Null; CWLType.String ])) fields.[1].Type) "A field can be given as an optional type."
            Expect.isTrue (CWLType.typesEqual (Union (ResizeArray [ Null; CWLType.File ])) fields.[2].Type) "A field can be given as a type list."
            Expect.equal fields.[3].Doc (Some (DocText.literal "object form")) "A field object keeps its doc."
            Kit.rewrite "The written text should decode to the same tool." Decode.decodeCommandLineTool Encode.encodeToolDescription decoded |> ignore

        testCase "a union of null and records keeps the order of the fields and the binding of each field" <| fun _ ->
            let withRecordMembers =
                Kit.tool
                    [ "baseCommand: [run]"
                      "inputs:"
                      "  choice:"
                      "    type:"
                      "      - \"null\""
                      "      - type: record"
                      "        name: with_file"
                      "        fields:"
                      "          source:"
                      "            type: File"
                      "            inputBinding:"
                      "              prefix: --source"
                      "          title:"
                      "            type: string"
                      "            inputBinding:"
                      "              prefix: --title"
                      "      - type: record"
                      "        name: with_flags"
                      "        fields:"
                      "          verbose:"
                      "            type: boolean"
                      "            inputBinding:"
                      "              prefix: --verbose"
                      "          quiet:"
                      "            type: boolean"
                      "            inputBinding:"
                      "              prefix: --quiet"
                      "outputs: []" ]
            let yaml = withRecordMembers
            let decoded = Decode.decodeCommandLineTool yaml
            let recordFields (index: int) =
                match (ListEntry.values decoded.Inputs.Value).[0].Type_ with
                | Some (Union members) ->
                    match members.[index] with
                    | Record schema -> (InputRecordSchema.getFieldValuesOrEmpty schema)
                    | other -> failwithf "Expected a record, found %A" other
                | other -> failwithf "Expected a union, found %A" other
            let withFile = recordFields 1
            let withFlags = recordFields 2
            Expect.sequenceEqual (withFile |> Seq.map (fun field -> field.Name)) [ "source"; "title" ] "The fields of the first record keep their order."
            Expect.sequenceEqual (withFlags |> Seq.map (fun field -> field.Name)) [ "verbose"; "quiet" ] "The fields of the second record keep their order."
            Expect.equal withFile.[1].Type CWLType.String "title is a string."
            Expect.equal withFlags.[0].Type CWLType.Boolean "verbose is a boolean."
            Expect.equal withFlags.[1].Type CWLType.Boolean "quiet is a boolean."
            Expect.equal withFile.[0].Type CWLType.File "source is a File."
            for field in Seq.append withFile withFlags do
                Expect.isTrue (field.GetProperties(false) |> Seq.exists (fun property -> property.Key = "inputBinding")) (sprintf "%s keeps its inputBinding." field.Name)
            let _, encoded = Kit.rewrite "The written text should decode to the same tool." Decode.decodeCommandLineTool Encode.encodeToolDescription decoded
            Expect.stringContains encoded "        source:\n          type: File\n          inputBinding:\n            prefix: --source\n        title:\n          type: string\n          inputBinding:\n            prefix: --title\n" "A File field is the type File and its binding."
            for prefix in [ "--source"; "--title"; "--verbose"; "--quiet" ] do
                Expect.equal (encoded.Split("prefix: " + prefix).Length) 2 (sprintf "The binding %s is written once." prefix)

        testCase "an array of arrays, array items and the members of a type list are written as schemas and lists in a CWL document" <| fun _ ->
            let tool (portLines: string list) = Kit.tool ([ "baseCommand: echo"; "inputs:" ] @ portLines @ [ "outputs: []" ])
            let cases =
                [ // the name, the lines of the input x and the text that the written document holds for it
                  "array of arrays",
                  [ "  x:"; "    type:"; "      type: array"; "      items:"; "        type: array"; "        items: File" ],
                  "  x:\n    type:\n      type: array\n      items:\n        type: array\n        items: File\n"
                  "optional items",
                  [ "  x:"; "    type:"; "      type: array"; "      items: [\"null\", File]" ],
                  "      items: [\"null\", File]\n"
                  "union with an array schema as a member",
                  [ "  x: [string, {type: array, items: File}]" ],
                  "  x: [string, \"File[]\"]\n"
                  "type list members with the shorthand",
                  [ "  x: [string, \"File[]\"]"; "  z: [int, \"File?\"]" ],
                  "  x: [string, \"File[]\"]\n  z: [int, File?]\n"
                  "optional array of arrays",
                  [ "  x:"; "    type:"; "      - \"null\""; "      - type: array"; "        items:"; "          type: array"; "          items: string" ],
                  "  x:\n    - \"null\"\n    - type: array\n      items:\n        type: array\n        items: string\n"
                  "array of arrays as the type of a record field",
                  [ "  x:"; "    type:"; "      type: record"; "      fields:"; "        f:"; "          type:"; "            type: array"; "            items:"; "              type: array"; "              items: string" ],
                  "        f:\n          type:\n            type: array\n            items:\n              type: array\n              items: string\n"
                  "arrays of one level keep the shorthand",
                  [ "  x: File[]"; "  w: File[]?"; "  z: string?" ],
                  "  x: File[]\n  w: File[]?\n  z: string?\n" ]
            for (name, portLines, expected) in cases do
                let decoded = Decode.decodeCommandLineTool (tool portLines)
                let _, encoded = Kit.rewrite (sprintf "%s: the written text should decode to the same tool." name) Decode.decodeCommandLineTool Encode.encodeToolDescription decoded
                Expect.stringContains encoded expected (sprintf "%s: the type should be written as CWL reads it." name)
                Expect.isFalse (encoded.Contains "[][]") (sprintf "%s: CWL reads one [] after a name." name)
                Expect.isFalse (encoded.Contains "items: File[]" || encoded.Contains "items: File?") (sprintf "%s: CWL reads no shorthand in items." name)

        testCase "a nested array and any shorthand under items are decode errors at the field" <| fun _ ->
            let tool (portLines: string list) = Kit.tool ([ "baseCommand: echo"; "inputs:" ] @ portLines @ [ "outputs: []" ])
            let nested = "Expected one [] behind a type name"
            let underItems = "Expected a type name or a schema under items"
            let cases =
                [ "a nested array as a port type", "inputs.x", nested, [ "  x: File[][]" ]
                  "a nested optional array as a port type", "inputs.x", nested, [ "  x: File[][]?" ]
                  "a nested array in the type field of a port", "inputs.x.type", nested, [ "  x:"; "    type: File[][]" ]
                  "a nested array as a member of a type list", "inputs.x[1]", nested, [ "  x: [string, \"File[][]\"]" ]
                  "a nested array as the type of a record field", "inputs.r.type.fields.f", nested, [ "  r:"; "    type:"; "      type: record"; "      fields:"; "        f: File[][]" ]
                  "a nested array in the type field of a record field", "inputs.r.type.fields.f.type", nested, [ "  r:"; "    type:"; "      type: record"; "      fields:"; "        f:"; "          type: File[][]" ]
                  "an array as the items of an array", "inputs.x.type.items", underItems, [ "  x:"; "    type:"; "      type: array"; "      items: File[]" ]
                  "an optional type as the items of an array", "inputs.x.type.items", underItems, [ "  x:"; "    type:"; "      type: array"; "      items: File?" ]
                  "an optional member in the items of an array", "inputs.x.type.items[1]", underItems, [ "  x:"; "    type:"; "      type: array"; "      items: [string, \"File?\"]" ] ]
            for (name, path, message, portLines) in cases do
                Kit.toolFails name (tool portLines) path message
            // one level of shorthand is read as a type, as a member of a type list and as the type of a record field
            let read (portLines: string list) = (ListEntry.values (Decode.decodeCommandLineTool (tool portLines)).Inputs.Value)
            let arrayOf (inner: CWLType) = CWLType.Array (InputArraySchema(inner))
            let optionalOf (inner: CWLType) = CWLType.Union (ResizeArray [ CWLType.Null; inner ])
            let members = read [ "  x: [string, \"File[]\"]"; "  y: [int, \"File?\"]"; "  z: [\"null\", \"string[]\"]" ]
            Expect.equal members.[0].Type_ (Some (CWLType.Union (ResizeArray [ CWLType.String; arrayOf (CWLType.File) ]))) "A member may be an array shorthand."
            Expect.equal members.[1].Type_ (Some (CWLType.Union (ResizeArray [ CWLType.Int; optionalOf (CWLType.File) ]))) "A member may be an optional shorthand."
            Expect.equal members.[2].Type_ (Some (optionalOf (arrayOf CWLType.String))) "A list of null and an array shorthand is an optional array."
            let fieldTypes = read [ "  r:"; "    type:"; "      type: record"; "      fields:"; "        f: File[]?"; "        g:"; "          type: string[]" ]
            match fieldTypes.[0].Type_ with
            | Some (CWLType.Record schema) ->
                Expect.equal (InputRecordSchema.getFieldValuesOrEmpty schema).[0].Type (optionalOf (arrayOf (CWLType.File))) "A record field reads File[]? as a type."
                Expect.equal (InputRecordSchema.getFieldValuesOrEmpty schema).[1].Type (arrayOf CWLType.String) "A record field reads string[] in its type field."
            | other -> failwith (sprintf "Expected a record but got %A" other)
            let items = read [ "  x:"; "    type:"; "      type: array"; "      items: [string, File]" ]
            Expect.equal items.[0].Type_ (Some (arrayOf (CWLType.Union (ResizeArray [ CWLType.String; CWLType.File ])))) "A name under items needs no shorthand."

        testCase "the shorthand text of a type nests arrays and the type of a CWL document does not" <| fun _ ->
            let nested = Array (InputArraySchema(Array (InputArraySchema(CWLType.File))))
            Expect.equal ((Encode.encodeCWLTypeShorthand nested |> Encode.writeYaml).Trim()) "File[][]" "The shorthand text nests the arrays."
            Expect.equal ((Encode.encodeCWLType nested |> Encode.writeYaml).Trim()) "type: array\nitems:\n  type: array\n  items: File" "A CWL document writes the schema."
            Expect.equal ((Encode.encodeCWLType (Array (InputArraySchema(CWLType.File))) |> Encode.writeYaml).Trim()) "File[]" "An array of one level is the shorthand."

        testCase "a process without inputs writes the empty inputs that CWL requires and reads back equal" <| fun _ ->
            let tool (inputs: string) = Kit.tool [ "baseCommand: echo" ] + inputs + "outputs: []\n"
            let expression (inputs: string) =
                Kit.expressionTool [] + inputs + "outputs: []\nexpression: \"${ return {}; }\"\nrequirements:\n  - class: InlineJavascriptRequirement\n"
            let operation (inputs: string) = Kit.operation [] + inputs + "outputs: []\n"
            let workflow (inputs: string) (steps: string) = Kit.workflow [] + inputs + "outputs: []\n" + steps
            let step (toolInputs: string) (inOut: string) =
                "steps:\n  s:\n    run:\n      class: CommandLineTool\n      baseCommand: echo\n" + toolInputs + "      outputs: []\n" + inOut
            let cases =
                [ "a tool with an empty list", tool "inputs: []\n"
                  "a tool with an empty map", tool "inputs: {}\n"
                  "an expression tool with an empty list", expression "inputs: []\n"
                  "an operation with an empty list", operation "inputs: []\n"
                  "a workflow with an empty list", workflow "inputs: []\n" "steps: []\n"
                  "a workflow with an inline tool with empty inputs", workflow "inputs: {}\n" (step "      inputs: {}\n" "    in: {}\n    out: []\n")
                  "a workflow with empty lists of step inputs and step outputs", workflow "inputs: {}\n" (step "      inputs: []\n" "    in: []\n    out: []\n") ]
            for (name, yaml) in cases do
                let decoded = Decode.decodeCWLProcessingUnit yaml
                let _, encoded = Kit.rewrite (sprintf "%s: the written text should decode to the same document" name) Decode.decodeCWLProcessingUnit Encode.encodeProcessingUnit decoded
                Expect.stringContains encoded "inputs: {}\n" (sprintf "%s: the inputs are written\n%s" name encoded)
                Expect.stringContains encoded "outputs: {}\n" (sprintf "%s: the outputs are written\n%s" name encoded)
            let workflowText = Encode.encodeProcessingUnit (Decode.decodeCWLProcessingUnit (workflow "inputs: {}\n" (step "      inputs: []\n" "    in: {}\n    out: []\n")))
            Expect.stringContains workflowText "      inputs: {}\n" "The inline tool of a step writes its inputs too."
            Expect.stringContains workflowText "    in: {}\n    out: []\n" "The step writes in and out."

        testCase "a document without inputs is a decode error at the place of the process for every class" <| fun _ ->
            let toolInStepMapForm =
                Kit.workflow
                    [ "inputs: {}"
                      "outputs: []"
                      "steps:"
                      "  s:"
                      "    run:"
                      "      class: CommandLineTool"
                      "      baseCommand: echo"
                      "      outputs: []"
                      "    in: {}"
                      "    out: []" ]
            let step = toolInStepMapForm
            let toolInStepListForm =
                Kit.workflow
                    [ "inputs: {}"
                      "outputs: []"
                      "steps:"
                      "  - id: s"
                      "    run:"
                      "      class: CommandLineTool"
                      "      baseCommand: echo"
                      "      outputs: []"
                      "    in: {}"
                      "    out: []" ]
            let stepList = toolInStepListForm
            let toolWithoutInputs = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
outputs: []
"""
            let expressionToolWithoutInputs = """cwlVersion: v1.2
class: ExpressionTool
outputs: []
expression: "${ return {}; }"
"""
            let operationWithoutInputs = """cwlVersion: v1.2
class: Operation
outputs: []
"""
            let workflowWithoutInputs = """cwlVersion: v1.2
class: Workflow
outputs: []
steps: []
"""
            // the document, the process that needs inputs, the path of the error and the line where the process starts
            let documents =
                [ "a tool", toolWithoutInputs, "a command line tool", "", 1
                  "an expression tool", expressionToolWithoutInputs, "an expression tool", "", 1
                  "an operation", operationWithoutInputs, "an operation", "", 1
                  "a workflow", workflowWithoutInputs, "a workflow", "", 1
                  "a tool inside a step in map form", step, "a command line tool", "steps.s.run", 8
                  "a tool inside a step in list form", stepList, "a command line tool", "steps.s.run", 8 ]
            for (name, yaml, what, path, line) in documents do
                match Kit.decodeError (fun () -> Decode.decodeCWLProcessingUnit yaml) with
                | Some error ->
                    Expect.equal error.Message ("Expected an inputs field, which CWL requires for " + what + ".") (sprintf "%s: the message should say what is missing." name)
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s: the path should show the place of the process." name)
                    match error.Span with
                    | Some span -> Expect.equal span.Start.Line line (sprintf "%s: the span should start where the process starts." name)
                    | None -> failwith (sprintf "%s: the error should have the place in the text" name)
                | None -> failwith (sprintf "%s without inputs should fail." name)

        testCase "a doc keeps its final line feeds through a write and a read" <| fun _ ->
            let tool (doc: string) = Kit.tool [ "baseCommand: echo" ] + "doc: " + doc + "inputs: []\noutputs: []\n"
            let expression (doc: string) =
                Kit.expressionTool [] + "doc: " + doc + "inputs: []\noutputs: []\nexpression: \"${ return {}; }\"\nrequirements:\n  - class: InlineJavascriptRequirement\n"
            // the YAML of the doc and the text that the decoder finds
            let docs =
                [ "|\n  first line\n  second line\n", "first line\nsecond line\n"
                  "|-\n  first line\n  second line\n", "first line\nsecond line"
                  "|+\n  first line\n  second line\n\n", "first line\nsecond line\n\n"
                  "|\n\n  first line\n\n  second line\n\n", "\nfirst line\n\nsecond line\n"
                  "one line\n", "one line" ]
            for (yaml, expected) in docs do
                for (kind, make) in [ "tool", tool; "expression tool", expression ] do
                    let name = sprintf "%s with the doc %s" kind (yaml.Replace("\n", "\\n"))
                    let decoded = Decode.decodeCWLProcessingUnit (make yaml)
                    let docOf unit =
                        match unit with
                        | CommandLineTool tool -> tool.Doc
                        | ExpressionTool expressionTool -> expressionTool.Doc
                        | _ -> None
                    Expect.equal (docOf decoded) (Some (DocText.literal expected)) (sprintf "%s: the doc is found" name)
                    let again, encoded = Kit.rewrite "the round trip" Decode.decodeCWLProcessingUnit Encode.encodeProcessingUnit decoded
                    Expect.equal (docOf again) (Some (DocText.literal expected)) (sprintf "%s: the doc is the same after a round trip\n%s" name encoded)
                    Expect.equal again decoded (sprintf "%s: the document is the same after a round trip" name)
                    Expect.equal (Encode.encodeProcessingUnit again) encoded (sprintf "%s: a second pass should write the same text" name)

        testCase "requirements and hints with lists are equal to a second decode of the same text and unequal after a change" <| fun _ ->
            let head = Kit.tool [ "baseCommand: echo"; "inputs: []"; "outputs: []" ]
            let software (version: string) =
                "  - class: SoftwareRequirement\n    packages:\n      - package: tool\n        version: [ \"" + version + "\" ]\n        specs: [ https://example.org/tool ]\n"
            let cases =
                [ "software packages with a version and specs", software "1.0", software "2.0"
                  "environment variables",
                  "  - class: EnvVarRequirement\n    envDef:\n      - envName: A\n        envValue: one\n",
                  "  - class: EnvVarRequirement\n    envDef:\n      - envName: A\n        envValue: two\n"
                  "files to create",
                  "  - class: InitialWorkDirRequirement\n    listing:\n      - entryname: a\n        entry: one\n",
                  "  - class: InitialWorkDirRequirement\n    listing:\n      - entryname: a\n        entry: two\n"
                  "schema definitions",
                  "  - class: SchemaDefRequirement\n    types:\n      - name: t\n        type: enum\n        symbols: [a]\n",
                  "  - class: SchemaDefRequirement\n    types:\n      - name: t\n        type: enum\n        symbols: [b]\n"
                  "a library of expressions",
                  "  - class: InlineJavascriptRequirement\n    expressionLib:\n      - \"var a = 1;\"\n",
                  "  - class: InlineJavascriptRequirement\n    expressionLib:\n      - \"var a = 2;\"\n"
                  "container options",
                  "  - class: DockerRequirement\n    dockerPull: image\n    cwltool:dockerRunOptions: [ --one ]\n",
                  "  - class: DockerRequirement\n    dockerPull: image\n    cwltool:dockerRunOptions: [ --two ]\n" ]
            for (name, requirement, changed) in cases do
                for section in [ "requirements"; "hints" ] do
                    let yaml = head + section + ":\n" + requirement
                    let first = Decode.decodeCommandLineTool yaml
                    let second = Decode.decodeCommandLineTool yaml
                    Kit.expectSame (sprintf "%s in %s: two decodes of the same text" name section) (box first) (box second)
                    Kit.rewrite (sprintf "%s in %s: the written text decodes to the same tool" name section) Decode.decodeCommandLineTool Encode.encodeToolDescription first |> ignore
                    let other = Decode.decodeCommandLineTool (head + section + ":\n" + changed)
                    Expect.notEqual first other (sprintf "%s in %s: a changed list makes the tools unequal" name section)

        testCase "requirement values compare their lists by their elements" <| fun _ ->
            let package (version: string) = SoftwarePackage(SchemaSaladString.Literal "tool", literals [ version ], literals [ "https://example.org/tool" ])
            Expect.equal (SoftwareRequirement (ListEntry.ofValues [ package "1.0" ])) (SoftwareRequirement (ListEntry.ofValues [ package "1.0" ])) "Equal lists of equal packages are equal."
            Expect.notEqual (SoftwareRequirement (ListEntry.ofValues [ package "1.0" ])) (SoftwareRequirement (ListEntry.ofValues [ package "2.0" ])) "A different version is a different requirement."
            Expect.notEqual (SoftwareRequirement (ListEntry.ofValues [ package "1.0" ])) (SoftwareRequirement (ListEntry.ofValues [ package "1.0"; package "1.0" ])) "A different number of packages is a different requirement."
            Expect.notEqual (SoftwareRequirement (ListEntry.ofValues [ package "1.0" ])) (EnvVarRequirement (ResizeArray [])) "Another kind of requirement is a different requirement."
            Expect.equal (hash (SoftwareRequirement (ListEntry.ofValues [ package "1.0" ]))) (hash (SoftwareRequirement (ListEntry.ofValues [ package "1.0" ]))) "Equal requirements have the same hash."
            Expect.equal (SoftwarePackage(SchemaSaladString.Literal "tool")) (SoftwarePackage(SchemaSaladString.Literal "tool")) "Packages without a version are equal."
            Expect.notEqual (SoftwarePackage(SchemaSaladString.Literal "tool")) (SoftwarePackage(SchemaSaladString.Literal "tool", literals [ "1.0" ])) "A missing version differs from a version."
    ]


/// A piece of a workflow document. A spelled piece is a doc or a label that a test writes in different layouts.
type private Piece =
    | Fixed of string
    | Spelled of indent: string * key: string * parts: string list


/// A workflow with a doc or a label at every place that has one. Each text has two or three parts, one for each line
/// of its multi-line layouts. The lines of the pieces are in the order and the form that the encoder writes.
let private layoutPieces =
    [ Fixed "cwlVersion: v1.2"
      Fixed "class: Workflow"
      Spelled("", "label", [ "first part of the workflow name"; "second part of the workflow name" ])
      Spelled("", "doc", [ "first line of the workflow text"; "second line of the workflow text"; "third line of the workflow text" ])
      Fixed ""
      Fixed "inputs:"
      Fixed "  sample:"
      Fixed "    type: string"
      Spelled("    ", "label", [ "first part of the input name"; "second part of the input name" ])
      Spelled("    ", "doc", [ "first line of the input text"; "second line of the input text" ])
      Fixed "  options:"
      Fixed "    type:"
      Fixed "      type: record"
      Fixed "      fields:"
      Fixed "        size:"
      Fixed "          type: int"
      Spelled("          ", "doc", [ "first line of the field text"; "second line of the field text" ])
      Spelled("      ", "label", [ "first part of the record name"; "second part of the record name" ])
      Spelled("      ", "doc", [ "first line of the record text"; "second line of the record text" ])
      Fixed "  choice:"
      Fixed "    type:"
      Fixed "      type: enum"
      Fixed "      symbols: [a, b]"
      Spelled("      ", "doc", [ "first line of the enum text"; "second line of the enum text" ])
      Fixed "  list:"
      Fixed "    type:"
      Fixed "      type: array"
      Fixed "      items:"
      Fixed "        type: enum"
      Fixed "        symbols: [x, z]"
      Spelled("      ", "label", [ "first part of the array name"; "second part of the array name" ])
      Fixed ""
      Fixed "steps:"
      Fixed "  work:"
      Fixed "    run:"
      Fixed "      cwlVersion: v1.2"
      Fixed "      class: CommandLineTool"
      Spelled("      ", "doc", [ "first line of the tool text"; "second line of the tool text" ])
      Fixed "      baseCommand: [echo]"
      Fixed "      inputs: {}"
      Fixed "      outputs:"
      Fixed "        done: string"
      Fixed "    in:"
      Fixed "      value:"
      Fixed "        source:"
      Fixed "          - sample"
      Fixed "          - options"
      Spelled("        ", "doc", [ "first line of the step input text"; "second line of the step input text" ])
      Fixed "    out: [done]"
      Spelled("    ", "label", [ "first part of the step name"; "second part of the step name" ])
      Spelled("    ", "doc", [ "first line of the step text"; "second line of the step text" ])
      Fixed ""
      Fixed "outputs:"
      Fixed "  result:"
      Fixed "    type: string"
      Spelled("    ", "label", [ "first part of the output name"; "second part of the output name" ])
      Spelled("    ", "doc", [ "first line of the output text"; "second line of the output text" ])
      Fixed "    outputSource: work/done" ]


/// The ways to write a text of several parts. Every way reads as the parts joined by single spaces.
let private layoutSpellings : (string * (string -> string -> string list -> string list)) list =
    let quoted (opening: string) (closing: string) (lineEnd: string) indent key (parts: string list) =
        let last = List.length parts - 1
        parts
        |> List.mapi (fun index part ->
            (if index = 0 then indent + key + ": " + opening else indent + "  ")
            + part
            + (if index = last then closing else lineEnd))
    [ "on one line", (fun indent key parts -> [ indent + key + ": " + String.concat " " parts ])
      "as plain lines",
      (fun indent key parts -> (indent + key + ": " + List.head parts) :: (List.tail parts |> List.map (fun part -> indent + "  " + part)))
      "as single-quoted lines", quoted "'" "'" ""
      "as double-quoted lines", quoted "\"" "\"" ""
      "as double-quoted lines with escaped breaks", quoted "\"" "\"" " \\"
      "as folded lines", (fun indent key parts -> (indent + key + ": >-") :: (parts |> List.map (fun part -> indent + "  " + part))) ]


let private layoutWorkflow (spell: string -> string -> string list -> string list) : string =
    layoutPieces
    |> List.collect (fun piece ->
        match piece with
        | Fixed line -> [ line ]
        | Spelled(indent, key, parts) -> spell indent key parts)
    |> String.concat "\n"
    |> fun text -> text + "\n"


let private layoutWorkflowOf (text: string) : CWLWorkflowDescription =
    match Decode.decodeCWLProcessingUnit text with
    | Workflow workflow -> workflow
    | other -> failwith (sprintf "Expected a workflow but got %A" other)


/// The objects of a decoded layout workflow that carry a doc or a label, each one with its own equality.
let private layoutObjects (workflow: CWLWorkflowDescription) : (string * obj) list =
    let inputs = (ListEntry.values workflow.Inputs) |> Seq.toList
    let schemas =
        inputs
        |> List.choose (fun input ->
            match input.Type_ with
            | Some (CWLType.Record schema) -> Some ("record schema", box schema)
            | Some (CWLType.Enum schema) -> Some ("enum schema", box schema)
            | Some (CWLType.Array schema) -> Some ("array schema", box schema)
            | _ -> None)
    let fields =
        inputs
        |> List.collect (fun input ->
            match input.Type_ with
            | Some (CWLType.Record schema) -> schema.Fields |> Option.map (Seq.map (fun field -> "record field", box field) >> Seq.toList) |> Option.defaultValue []
            | _ -> [])
    let step = (ListEntry.values workflow.Steps).[0]
    [ "workflow", box workflow ]
    @ (inputs |> List.map (fun input -> "input", box input))
    @ schemas
    @ fields
    @ ((ListEntry.values workflow.Outputs) |> Seq.map (fun output -> "output", box output) |> Seq.toList)
    @ [ "step", box step ]
    @ (WorkflowStep.getInputValues step |> Seq.map (fun input -> "step input", box input) |> Seq.toList)
    @ (WorkflowStepRunOps.tryGetTool step.Run |> Option.map (fun tool -> "tool", box tool) |> Option.toList)


/// The ways to write a doc or a label of a tool and of its first input, as a function of the indent and the key.
let private editSources : (string * (string -> string -> string list)) list =
    [ "a folded doc", (fun indent key -> [ indent + key + ": >"; indent + "  first line of the text"; indent + "  second line of the text" ])
      "a literal doc", (fun indent key -> [ indent + key + ": |"; indent + "  first line of the text"; indent + "  second line of the text" ])
      "a plain doc over two lines", (fun indent key -> [ indent + key + ": first part of the text"; indent + "  second part of the text" ])
      "a single-quoted doc over two lines", (fun indent key -> [ indent + key + ": 'first part of the text"; indent + "  second part of the text'" ])
      "a double-quoted doc with an escaped break", (fun indent key -> [ indent + key + ": \"first part of the text \\"; indent + "  second part of the text\"" ])
      "a doc on one line", (fun indent key -> [ indent + key + ": one line of the text" ]) ]


let private editTool (key: string) (lines: string -> string -> string list) : string =
    Kit.tool (lines "" key @ [ "baseCommand: [echo]"; "inputs:"; "  x:"; "    type: string" ] @ lines "    " key @ [ "outputs: {}" ])


let testDocAndLabelLayout =
    testList "Doc and label layout" [
        testCase "a document comes back with the layout of every doc and label as it was written" <| fun _ ->
            for (name, spell) in layoutSpellings do
                let text = layoutWorkflow spell
                let workflow = layoutWorkflowOf text
                let encoded = Encode.encodeProcessingUnit (Workflow workflow)
                Expect.equal encoded text (sprintf "Texts %s should come back as they were written." name)
                Expect.equal (Encode.encodeProcessingUnit (Workflow (layoutWorkflowOf encoded))) encoded (sprintf "Texts %s: a second pass should write the same text." name)
                Expect.equal workflow.Label (Some (SchemaSaladString.Literal "first part of the workflow name second part of the workflow name")) (sprintf "Texts %s: the label is the parts joined by spaces." name)
                Expect.equal workflow.Doc (Some (DocText.literal "first line of the workflow text second line of the workflow text third line of the workflow text")) (sprintf "Texts %s: the doc is the parts joined by spaces." name)
                Expect.equal (ListEntry.values workflow.Inputs).[0].Doc (Some (DocText.literal "first line of the input text second line of the input text")) (sprintf "Texts %s: the doc of an input is the parts joined by spaces." name)

        testCase "a doc or a label that is not edited comes back in the spelling of its source" <| fun _ ->
            let cases =
                [ "a folded doc over three lines",
                  "doc: >\n  first line of the text\n  second line of the text\n  third line of the text\n",
                  (fun (tool: CWLToolDescription) -> docTextOf tool.Doc),
                  "first line of the text second line of the text third line of the text\n"
                  "a plain label over three lines",
                  "label: first part of the name\n  second part of the name\n  third part of the name\n",
                  (fun (tool: CWLToolDescription) -> labelTextOf tool.Label),
                  "first part of the name second part of the name third part of the name"
                  "a double-quoted doc with an escaped break",
                  "doc: \"first part of the text \\\n  second part of the text\"\n",
                  (fun (tool: CWLToolDescription) -> docTextOf tool.Doc),
                  "first part of the text second part of the text"
                  "a double-quoted label on one line",
                  "label: \"just one line of the name\"\n",
                  (fun (tool: CWLToolDescription) -> labelTextOf tool.Label),
                  "just one line of the name" ]
            for (name, lines, read, expected) in cases do
                let yaml = "cwlVersion: v1.2\nclass: CommandLineTool\n" + lines + "baseCommand: [echo]\ninputs: {}\noutputs: {}\n"
                let decoded = Decode.decodeCommandLineTool yaml
                Expect.equal (read decoded) (Some expected) (sprintf "%s: the text is found." name)
                let encoded = Encode.encodeToolDescription decoded
                Expect.stringContains encoded lines (sprintf "%s: the lines come back as written.\n%s" name encoded)

        testCase "the label and the doc of an expression tool and of an operation come back as written" <| fun _ ->
            let texts = "label: first part of the name\n  second part of the name\ndoc: >\n  first line of the text\n  second line of the text\n"
            let documents =
                [ "an expression tool",
                  "cwlVersion: v1.2\nclass: ExpressionTool\n" + texts + "inputs: {}\noutputs: {}\nexpression: \"${ return {}; }\"\nrequirements:\n  - class: InlineJavascriptRequirement\n"
                  "an operation", "cwlVersion: v1.2\nclass: Operation\n" + texts + "inputs: {}\noutputs: {}\n" ]
            for (name, yaml) in documents do
                let encoded = Encode.encodeProcessingUnit (Decode.decodeCWLProcessingUnit yaml)
                Expect.stringContains encoded texts (sprintf "%s: the label and the doc come back as written.\n%s" name encoded)

        testCase "documents that differ only in the layout of their texts are equal and have the same hash" <| fun _ ->
            let decoded = layoutSpellings |> List.map (fun (name, spell) -> name, layoutWorkflowOf (layoutWorkflow spell))
            for (name, workflow) in decoded do
                for (otherName, other) in decoded do
                    let first = layoutObjects workflow
                    let second = layoutObjects other
                    Expect.equal first.Length second.Length "The same objects should carry a doc or a label."
                    List.iter2
                        (fun (kind: string, a: obj) (_: string, b: obj) ->
                            Kit.expectSame (sprintf "Texts %s and %s: the %s objects" name otherName kind) a b)
                        first
                        second
            let text = layoutWorkflow (snd (List.last layoutSpellings))
            let changed = layoutWorkflowOf (text.Replace("first line of the tool text", "first line of the other text"))
            let original = layoutWorkflowOf text
            Expect.notEqual (List.zip (layoutObjects original) (layoutObjects changed) |> List.filter (fun ((_, a), (_, b)) -> a <> b) |> List.length) 0 "A changed text should still make an object unequal."

        testCase "an edited doc or label is written for the new text and reads back exactly" <| fun _ ->
            let edits =
                [ ""; "short"; "true"; "null"; "42"; " leading blank"; "trailing blank "; "key: value # comment"; "- item"
                  "two\nlines"; "two\nlines\n"; "\ntext after a blank line"; "tab\there"; "café über"; "line one\r\nline two" ]
            for (name, lines) in editSources do
                for edit in edits do
                    // The doc of a tool reads its line breaks as line feeds. The other texts keep the characters that they have.
                    let toolDoc = edit.Replace("\r\n", "\n")
                    let docTool = Decode.decodeCommandLineTool (editTool "doc" lines)
                    docTool.Doc <- Some (DocText.literal edit)
                    (ListEntry.values docTool.Inputs.Value).[0].Doc <- Some (DocText.literal edit)
                    let encodedDoc = Encode.encodeToolDescription docTool
                    let againDoc = Decode.decodeCommandLineTool encodedDoc
                    Expect.equal againDoc.Doc (Some (DocText.literal toolDoc)) (sprintf "%s edited to %A: the doc of the tool reads back.\n%s" name edit encodedDoc)
                    Expect.equal (ListEntry.values againDoc.Inputs.Value).[0].Doc (Some (DocText.literal edit)) (sprintf "%s edited to %A: the doc of the input reads back.\n%s" name edit encodedDoc)
                    Expect.isFalse (encodedDoc.Contains "of the text") (sprintf "%s edited to %A: the old text is gone." name edit)
                    let labelTool = Decode.decodeCommandLineTool (editTool "label" lines)
                    labelTool.Label <- Some (SchemaSaladString.Literal edit)
                    (ListEntry.values labelTool.Inputs.Value).[0].Label <- Some (SchemaSaladString.Literal edit)
                    let encodedLabel = Encode.encodeToolDescription labelTool
                    let againLabel = Decode.decodeCommandLineTool encodedLabel
                    Expect.equal againLabel.Label (Some (SchemaSaladString.Literal edit)) (sprintf "%s edited to %A: the label of the tool reads back.\n%s" name edit encodedLabel)
                    Expect.equal (ListEntry.values againLabel.Inputs.Value).[0].Label (Some (SchemaSaladString.Literal edit)) (sprintf "%s edited to %A: the label of the input reads back.\n%s" name edit encodedLabel)

        testCase "a doc or a label that is a plain bool, int or float is a decode error at the field and a string spelling is not" <| fun _ ->
            let tool (lines: string list) = Kit.tool ("baseCommand: echo" :: lines)
            for key in [ "doc"; "label" ] do
                for text in [ "true"; "42"; "1.5"; ".inf"; "0x1F" ] do
                    let cases =
                        [ key, tool [ key + ": " + text; "inputs: {}"; "outputs: {}" ]
                          "inputs.x." + key, tool [ "inputs:"; "  x:"; "    type: string"; "    " + key + ": " + text; "outputs: {}" ]
                          "outputs.o." + key, tool [ "inputs: {}"; "outputs:"; "  o:"; "    type: File"; "    " + key + ": " + text ] ]
                    for (path, yaml) in cases do
                        Kit.toolFails (sprintf "A plain %s as %s in\n%s" text key yaml) yaml path "Expected a string"
                for text in [ "\"42\""; "'true'"; "|\n  42" ] do
                    let decoded = Decode.decodeCommandLineTool (tool [ key + ": " + text; "inputs: {}"; "outputs: {}" ])
                    let isSet = if key = "doc" then decoded.Doc.IsSome else decoded.Label.IsSome
                    Expect.isTrue isSet (sprintf "The spelling %s is a string." (text.Replace("\n", "\\n")))
            let step = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  s:
    run: t.cwl
    in: {}
    out: []
    label: 42
"""
            match Kit.decodeError (fun () -> Decode.decodeWorkflow step) with
            | Some error -> Expect.equal (DecodeError.pathText error) "steps.s.label" "The error should show the label of the step."
            | None -> failwith "A plain int as the label of a step must fail."

        testCase "an edit of a plain string doc or label to a bool, an int or null reads back as the same string and keeps the comment" <| fun _ ->
            let tool (field: string) = Kit.tool [ "baseCommand: echo" ] + field + "inputs: {}\noutputs: {}\n"
            let sources =
                [ "a plain label with a comment", "label: abc # keep me\n", "# keep me"
                  "a quoted label with a comment", "label: \"a name\" # keep me\n", "# keep me"
                  "a literal doc with a header comment", "doc: | # keep me\n  some text\n", "# keep me" ]
            for (name, field, kept) in sources do
                for edit in [ "true"; "42"; "1.5"; "null" ] do
                    let decoded = Decode.decodeCommandLineTool (tool field)
                    if field.StartsWith "doc" then decoded.Doc <- Some (DocText.literal edit) else decoded.Label <- Some (SchemaSaladString.Literal edit)
                    let encoded = Encode.encodeToolDescription decoded
                    let again = Decode.decodeCommandLineTool encoded
                    Expect.equal (if field.StartsWith "doc" then docTextOf again.Doc else labelTextOf again.Label) (Some edit) (sprintf "%s edited to %s: the text reads back as the string.\n%s" name edit encoded)
                    Expect.stringContains encoded kept (sprintf "%s edited to %s: %s stays.\n%s" name edit kept encoded)

        testCase "a doc that is a list of strings reads as their concatenation, comes back unchanged and is a string after an edit" <| fun _ ->
            let tool (field: string) = Kit.tool [ "baseCommand: [echo]" ] + field + "inputs:\n  x:\n    type: string\n    doc: [one, two]\noutputs: {}\n"
            let lists =
                [ "a flow list", "doc: [first part, second part]\n"
                  "a block list", "doc:\n  - first part\n  - second part\n"
                  "a block list with a comment", "doc:\n  - first part # note\n  - second part\n" ]
            for (name, lines) in lists do
                let decoded = Decode.decodeCommandLineTool (tool lines)
                Expect.equal decoded.Doc (Some (DocText.literal "first partsecond part")) (sprintf "%s: the doc is the items joined without a separator." name)
                Expect.equal (ListEntry.values decoded.Inputs.Value).[0].Doc (Some (DocText.literal "onetwo")) (sprintf "%s: the doc of an input is a list too." name)
                let encoded = Encode.encodeToolDescription decoded
                Expect.stringContains encoded lines (sprintf "%s: the list comes back as written.\n%s" name encoded)
                Expect.stringContains encoded "    doc: [one, two]\n" (sprintf "%s: the list of the input comes back as written.\n%s" name encoded)
                Expect.equal (Encode.encodeToolDescription (Decode.decodeCommandLineTool encoded)) encoded (sprintf "%s: a second pass should write the same text." name)
                // the same text as a string and the list are equal, and the same text set again keeps the list
                Expect.equal decoded (Decode.decodeCommandLineTool (tool "doc: first partsecond part\n")) (sprintf "%s: a list and its concatenation are equal." name)
                decoded.Doc <- Some (DocText.literal "first partsecond part")
                Expect.stringContains (Encode.encodeToolDescription decoded) lines (sprintf "%s: the same text keeps the list." name)
                // an edit writes a string
                decoded.Doc <- Some (DocText.literal "one text")
                (ListEntry.values decoded.Inputs.Value).[0].Doc <- Some (DocText.literal "another text")
                let edited = Encode.encodeToolDescription decoded
                Expect.stringContains edited "doc: one text\n" (sprintf "%s: an edited doc is a string.\n%s" name edited)
                Expect.stringContains edited "doc: another text\n" (sprintf "%s: an edited doc of an input is a string.\n%s" name edited)
                Expect.isFalse (edited.Contains "first part" || edited.Contains "[one, two]") (sprintf "%s: the list is gone.\n%s" name edited)
                Expect.equal (Decode.decodeCommandLineTool edited).Doc (Some (DocText.literal "one text")) (sprintf "%s: the edited doc reads back." name)

        testCase "the label is a string only and the items of a doc list are strings" <| fun _ ->
            let tool (field: string) = Kit.tool [ "baseCommand: [echo]" ] + field + "inputs: {}\noutputs: {}\n"
            for (field, path, message) in [ "label: [a, b]\n", "label", "Expected a string"; "doc: [a, 42]\n", "doc[1]", "Expected a string, found the plain scalar 42"; "doc: [a, true]\n", "doc[1]", "a bool"; "doc: [a, {b: c}]\n", "doc[1]", "Expected a string"; "doc:\n  - a\n  -\n", "doc[1]", "Expected a value, found" ] do
                Kit.toolFails field (tool field) path message
            Expect.equal (Decode.decodeCommandLineTool (tool "doc: [\"42\", 'true']\n")).Doc (Some (DocText.literal "42true")) "Quoted items are strings."

        testCase "an edit that keeps the length of every line keeps the lines and a longer edit writes the new text" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
doc: >
  first line of the text
  second line of the text
baseCommand: [echo]
inputs: {}
outputs: {}
"""
            let tool = Decode.decodeCommandLineTool yaml
            tool.Doc <- Some (DocText.literal "other line of the text second line of the text\n")
            let sameLengths = Encode.encodeToolDescription tool
            Expect.stringContains sameLengths "doc: >\n  other line of the text\n  second line of the text\n" "The lines of the author should stay for a text with lines of the same lengths."
            tool.Doc <- Some (DocText.literal "first line of the text, now a longer one second line of the text\n")
            let longer = Encode.encodeToolDescription tool
            Expect.equal (Decode.decodeCommandLineTool longer).Doc tool.Doc "The longer text should read back exactly."
            Expect.isFalse (longer.Contains "first line of the text\n") "The old line should be gone."

        testCase "a tool that is built from the texts of a decoded one writes them in the plain spelling" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
label: first part of the name
  second part of the name
doc: >
  first line of the text
  second line of the text
baseCommand: [echo]
inputs: {}
outputs: {}
"""
            let decoded = Decode.decodeCommandLineTool yaml
            let rebuilt =
                CWLToolDescription(
                    ListEntry.ofValues (ResizeArray<CWLOutput>()),
                    baseCommand = ResizeArray [ (SchemaSaladString.Literal "echo") ],
                    inputs = ListEntry.ofValues (ResizeArray<CWLInput>()),
                    ?label = decoded.Label,
                    ?doc = decoded.Doc
                )
            let encoded = Encode.encodeToolDescription rebuilt
            Expect.stringContains encoded "label: first part of the name second part of the name\n" "The label has no layout of the author."
            Expect.stringContains encoded "doc: |\n  first line of the text second line of the text\n" "The doc has no layout of the author."
            Expect.equal (Decode.decodeCommandLineTool encoded) decoded "The tools are equal whatever their layouts."

        testCase "the nodes of the texts stay out of the dynamic properties and out of the written document" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool (editTool "doc" (snd editSources.[0]))
            Expect.isNone decoded.Metadata "The nodes are not metadata."
            Expect.equal (decoded.GetProperties(false) |> Seq.length) 0 "The nodes are not dynamic properties."
            Expect.equal ((ListEntry.values decoded.Inputs.Value).[0].GetProperties(false) |> Seq.length) 0 "The nodes are not dynamic properties of an input."
            let encoded = Encode.encodeToolDescription decoded
            Expect.isFalse (encoded.ToLowerInvariant().Contains "sourcenodes") "The nodes are not written."
    ]


let testOneWriterForTheEntriesOfAList =
    let tool (lines: string list) = Kit.tool ("baseCommand: echo" :: lines)
    testList "One writer for the entries of a list" [
        testCase "the flow text of a record writes a mixed field as it was written" <| fun _ ->
            let mixed = { Node = YAMLicious.Reader.readRoot "{$mixin: f.yml, name: x}" }
            let recordType = Record (InputRecordSchema(fields = ResizeArray [ ListEntry.Mixed mixed; ListEntry.Imported "g.yml" ]))
            Expect.equal (Encode.cwlTypeToYamlString recordType) "{type: record, fields: [{$mixin: f.yml, name: x}, {$import: g.yml}]}" "The mixin and the import stand in the list."
        testCase "the success codes, the packages and the environment definitions write an import and a mixin in their places" <| fun _ ->
            let text =
                tool [ "successCodes: [0, {$import: codes.yml}, {$mixin: code.yml}]"
                       "requirements:"
                       "  - {class: SoftwareRequirement, packages: [{package: git}, {$import: packages.yml}, {$mixin: package.yml, package: curl}]}"
                       "  - {class: EnvVarRequirement, envDef: [{envName: A, envValue: a}, {$import: env.yml}, {$mixin: e.yml, envName: B}]}"
                       "inputs: []"
                       "outputs: []" ]
            let decoded = Decode.decodeCommandLineTool text
            let _, written = Kit.rewrite "the round trip" Decode.decodeCommandLineTool Encode.encodeToolDescription decoded
            for expected in [ "{$import: codes.yml}"; "{$mixin: code.yml}"; "{$import: packages.yml}"; "{$mixin: package.yml, package: curl}"; "{$import: env.yml}"; "{$mixin: e.yml, envName: B}" ] do
                Expect.stringContains written expected (sprintf "%s should be written back as it was." expected)
        testCase "the inputs, the outputs and the steps write an import and a mixin in their places in a list with an import" <| fun _ ->
            let text = tool [ "inputs: [{$import: ports.yml}, {id: a, type: string}, {$mixin: port.yml, id: b}]"; "outputs: [{id: o, type: string, outputBinding: {glob: o}}, {$import: outs.yml}]" ]
            let decoded = Decode.decodeCommandLineTool text
            let _, written = Kit.rewrite "the round trip" Decode.decodeCommandLineTool Encode.encodeToolDescription decoded
            for expected in [ "{$import: ports.yml}"; "{$mixin: port.yml, id: b}"; "{$import: outs.yml}" ] do
                Expect.stringContains written expected (sprintf "%s should be written back as it was." expected)
    ]


let main =
    testList "Layout" [
        testYamlReaderAndWriter
        testDocAndLabelLayout
        testOneWriterForTheEntriesOfAList
    ]
