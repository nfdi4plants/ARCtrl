/// How the writer spells a string that a reader would take for another type: a table of the predicates of
/// ScalarQuoting, and documents with a lookalike in every place where the writer puts a string.
module Tests.Quoting

open ARCtrl.CWL
open DynamicObj
open TestingUtils
open YAMLicious

/// How the writer spells a plain scalar of a raw value (ScalarQuoting.normalizeScalar).
type private Spelling =
    | Stays
    | Quoted
    | Becomes of string

let private Pl = ScalarStyle.Plain
let private Q = ScalarStyle.DoubleQuoted
let private Bl = ScalarStyle.Block(BlockScalarStyle.Literal, ChompingMode.Strip, None)

/// The text; whether the loader of cwltool or a reader of YAML 1.1 reads it, written plain, as another type than the core
/// schema does (readsAsAnotherTypeThanCore); whether it needs quotes to stay a string (readsAsAnotherType); whether it holds
/// a character that the loader reads as a line break (holdsLineBreakCharacter); the spelling of the plain scalar in a
/// raw value; and the style that keepAsText gives a plain scalar and a literal block scalar of the text (Pl plain, Q double
/// quoted, Bl the literal block).
let private rows : (string * bool * bool * bool * Spelling * ScalarStyle * ScalarStyle) list =
    [ "=", true, true, false, Quoted, Q, Bl
      "<<", true, true, false, Quoted, Q, Bl
      "yes", true, true, false, Quoted, Q, Bl
      "No", true, true, false, Quoted, Q, Bl
      "on", true, true, false, Quoted, Q, Bl
      "OFF", true, true, false, Quoted, Q, Bl
      "y", true, true, false, Quoted, Q, Bl
      "1_000", true, true, false, Quoted, Q, Bl
      "1_0", true, true, false, Quoted, Q, Bl
      "0b101", true, true, false, Quoted, Q, Bl
      "0b1", true, true, false, Quoted, Q, Bl
      "0b_", true, true, false, Quoted, Q, Bl
      "0b1111111111111111111111111111111111111111111111111111111111111111111111", true, true, false, Quoted, Q, Bl
      "0x1_F", true, true, false, Quoted, Q, Bl
      "0x_FFFF_FFFF_FFFF_FFFF_FFFF", true, true, false, Quoted, Q, Bl
      "1_0.5", true, true, false, Quoted, Q, Bl
      "1:30", true, true, false, Quoted, Q, Bl
      "._", true, true, false, Quoted, Q, Bl
      "-._", true, true, false, Quoted, Q, Bl
      "+._", true, true, false, Quoted, Q, Bl
      "._e+5", true, true, false, Quoted, Q, Bl
      ".__", true, true, false, Quoted, Q, Bl
      "0o7", true, true, false, Stays, Q, Bl
      "017", true, true, false, Stays, Q, Bl
      "0o777777777777777777777777777777", true, true, false, Stays, Q, Bl
      "0x1F", true, true, false, Stays, Q, Bl
      "1e3", true, true, false, Stays, Q, Bl
      "12", true, true, false, Stays, Q, Bl
      "1.0", true, true, false, Stays, Q, Bl
      ".nan", true, true, false, Stays, Q, Bl
      "-.5e+3", true, true, false, Stays, Q, Bl
      ".5e3", false, true, false, Becomes "0.5e3", Q, Bl
      "+.5E3", false, true, false, Becomes "+0.5E3", Q, Bl
      "true", false, true, false, Stays, Q, Bl
      "null", false, true, false, Stays, Q, Bl
      "~", false, true, false, Stays, Q, Bl
      "", false, true, false, Stays, Q, Bl
      "NaN", false, false, false, Stays, Pl, Bl
      "0X1F", false, false, false, Stays, Pl, Bl
      "2001-12-14", false, false, false, Stays, Pl, Bl
      "abc", false, false, false, Stays, Pl, Bl
      "a-?b", false, false, false, Stays, Pl, Bl
      ":y", false, false, false, Stays, Pl, Bl
      "?x", false, true, false, Stays, Q, Bl
      "a\tb", false, true, false, Stays, Q, Bl
      "yes\n", false, false, false, Stays, Pl, Bl
      "a\u0085b", false, true, true, Stays, Q, Q
      "a\u2028", false, true, true, Stays, Q, Q
      "\u2029a", false, true, true, Stays, Q, Q ]

let private scalarOf (node: YamlNode) =
    match node.Kind with
    | YamlNodeKind.Scalar scalar -> scalar
    | other -> failwithf "Expected a scalar, found %A" other

let private block (text: string) =
    YamlNode.scalar (ScalarStyle.Block(BlockScalarStyle.Literal, ChompingMode.Strip, None)) text

/// The text with its control characters and line break characters as escapes, for the name of a test.
let private shown (text: string) =
    if text = "" then "the empty text"
    else text.Replace("\n", "\\n").Replace("\t", "\\t").Replace("\u0085", "\\N").Replace("\u2028", "\\L").Replace("\u2029", "\\P")

let private tableTests =
    testList "The quoting table" [
        for (index, (text, thanCore, anyType, lineBreak, spelling, keptPlain, keptBlock)) in List.indexed rows do
            testCase (sprintf "the predicates of ScalarQuoting and the spelling of a raw scalar read as the table gives for %s (row %d)" (shown text) index) <| fun _ ->
                Expect.equal (ScalarQuoting.readsAsAnotherTypeThanCore text) thanCore "readsAsAnotherTypeThanCore"
                Expect.equal (ScalarQuoting.readsAsAnotherType text) anyType "readsAsAnotherType"
                Expect.equal (ScalarQuoting.holdsLineBreakCharacter text) lineBreak "holdsLineBreakCharacter"
                let plain = scalarOf (YamlNode.plain text)
                let expected =
                    match spelling with
                    | Stays -> plain
                    | Quoted -> { plain with Style = ScalarStyle.DoubleQuoted }
                    | Becomes value -> { plain with Value = value }
                Expect.equal (ScalarQuoting.normalizeScalar plain) expected "normalizeScalar"
                Expect.equal (scalarOf (ScalarQuoting.keepAsText (YamlNode.plain text))).Style keptPlain "keepAsText of a plain scalar"
                let kept = scalarOf (ScalarQuoting.keepAsText (block text))
                Expect.equal kept.Style keptBlock "keepAsText of a block scalar"
                Expect.equal kept.Value text "keepAsText keeps the text"
    ]

/// A tool with a lookalike in every place of a tool where the writer puts a string, as the source spells it.
let private toolText =
    Kit.lines
        [ "cwlVersion: v1.2"
          "class: CommandLineTool"
          "$namespaces: {ex: http://example.org/}"
          "label: 1_000"
          "doc: \"null\""
          "stdin: \"a\\Nb\""
          "baseCommand: [echo, \"yes\", \"a\\tb\", \"a\\Nb\", \"a\\L\", \"\\Pa\", \"?x\", \"??\", -x, \":y\", a-?b]"
          "requirements:"
          "  EnvVarRequirement:"
          "    envDef:"
          "      \"0b1\": \"1:30\""
          "      B: \"a\\Nb\\nc\""
          "      C: \"a\\nb\""
          "hints:"
          "  SoftwareRequirement:"
          "    packages:"
          "      \"1_0\": {}"
          "  \"=\": {x: 1}"
          "inputs:"
          "  - $import: ins.yml"
          "  - id: \"017\""
          "    type: [\"null\", string, int]"
          "    default: \"0b_\""
          "  - id: r"
          "    type:"
          "      type: record"
          "      fields:"
          "        \"1e3\": string"
          "  - id: q"
          "    type:"
          "      type: record"
          "      fields:"
          "        - {name: \"0o7\", type: string}"
          "  - id: d"
          "    type: Any"
          "    default: {plain: 1_000, list: [yes, 0b101, .5e3, +.5E3, -.5e+3], 1_000: key}"
          "  - id: k"
          "    type: string"
          "    default: \"._\""
          "  - id: u"
          "    type: [string, \"null\"]"
          "outputs:"
          "  \"<<\": {type: File, outputBinding: {glob: x}}"
          "ex:marker: ="
          "ex:quoted: \"true\""
          "ex:plain: true"
          "ex:number string: '12'"
          "ex:number: 12"
          "ex:null: null"
          "ex:tilde: ~"
          "ex:capital bool: True"
          "ex:hex: 0x1F"
          "ex:word: NaN"
          "ex:whole float: 2.0" ]

/// The written text of the tool holds each of these lines or parts of lines.
let private toolWritten =
    [ "label: \"1_000\""
      "doc: \"null\""
      "stdin: \"a\\Nb\""
      "baseCommand: [echo, \"yes\", \"a\tb\", \"a\\Nb\", \"a\\L\", \"\\Pa\", \"?x\", \"??\", -x, :y, a-?b]"
      "envName: \"0b1\""
      "envValue: \"1:30\""
      "envValue: \"a\\Nb\\nc\""
      "envValue: |-\n"
      "package: \"1_0\""
      "class: \"=\""
      "id: \"017\""
      "[\"null\", string, int]"
      "default: \"0b_\""
      "\"1e3\":"
      "\"0o7\":"
      "plain: \"1_000\""
      "[\"yes\", \"0b101\", 0.5e3, +0.5E3, -.5e+3]"
      "\"1_000\": key"
      "default: \"._\""
      "- id: k\n"
      "[string, \"null\"]"
      "\"<<\":"
      "ex:marker: \"=\""
      "ex:quoted: \"true\""
      "ex:plain: true"
      "ex:number string: \"12\""
      "ex:number: 12"
      "ex:null: null"
      "ex:tilde: ~"
      "ex:capital bool: true"
      "ex:hex: 31"
      "ex:word: NaN"
      "ex:whole float: 2.0" ]

/// A workflow with a lookalike in the key of a step, of a step input and of a step with a mixin.
let private workflowText =
    Kit.workflow [ "inputs:"; "  a: string"; "outputs: {}"; "steps:"; "  \"1_0.5\":"; "    run: t.cwl"; "    in: {\"y\": a}"; "    out: [o]"; "  \"<<\":"; "    $mixin: stepmix.yml" ]

let private integrationTests =
    testList "The lookalikes in the written documents" [
        testCase "a tool with a lookalike in every place of a string is written with quotes where a reader needs them and reads back equal" <| fun _ ->
            let decoded, written = Kit.roundTrip "tool" Decode.decodeCommandLineTool Encode.encodeToolDescription toolText
            for part in toolWritten do
                Expect.stringContains written part (sprintf "The written tool should hold %s.\n%s" part written)
            for part in [ "[null"; ", null"; "- null"; "label: 1_000"; "\"0x1F\""; "\"NaN\""; "-x\"" ] do
                Expect.isFalse (written.Contains part) (sprintf "The written tool should not hold %s.\n%s" part written)
            Expect.equal decoded.Label (Some (SchemaSaladString.Literal "1_000")) "The plain label is a string."
            Expect.equal decoded.Stdin (Some (SchemaSaladString.Literal "a\u0085b")) "The escape is the NEL."
            let metadata = Expect.wantSome decoded.Metadata "The extension fields are kept."
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "ex:quoted" metadata) (Some "true") "The quoted true is a string."
            Expect.equal (DynObj.tryGetTypedPropertyValue<bool> "ex:plain" metadata) (Some true) "The plain true is a bool."
            Expect.equal (DynObj.tryGetTypedPropertyValue<bool> "ex:capital bool" metadata) (Some true) "True is a bool of the core schema."
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "ex:word" metadata) (Some "NaN") "NaN is a string of the core schema."
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "ex:marker" metadata) (Some "=") "The marker is a string."
            Expect.isNone (DynObj.tryGetTypedPropertyValue<string> "ex:null" metadata) "A plain null is no string."
        testCase "a workflow with a lookalike in the key of a step, a step input and a step with a mixin is written with quotes and reads back equal" <| fun _ ->
            let _, written = Kit.roundTrip "workflow" Decode.decodeWorkflow Encode.encodeWorkflowDescription workflowText
            for part in [ "id: \"1_0.5\""; "\"y\": a"; "id: \"<<\"" ] do
                Expect.stringContains written part (sprintf "The written workflow should hold %s.\n%s" part written)
        testCase "a job file with a lookalike key and value is written with quotes and reads back equal" <| fun _ ->
            let read (text: string) = List.ofSeq (DecodeParameters.decodeYAMLParameterFile text)
            let write (references: CWLParameterReference list) = Encode.encodeYAMLParameterFile (ResizeArray references)
            let _, written = Kit.roundTrip "job file" read write "\"1_000\": \"yes\"\nb: \"0b_\"\nc: \"017\"\nd: plain\n"
            Expect.equal written "\"1_000\": \"yes\"\nb: \"0b_\"\nc: \"017\"\nd: plain\n" "The keys and the values keep their quotes."
        testCase "the flow text of a type quotes the lookalike symbols, field names and the null type" <| fun _ ->
            let enumType = Enum (InputEnumSchema(ResizeArray [ SchemaSaladString.Literal "true"; SchemaSaladString.Literal "a"; SchemaSaladString.Literal "x,y"; SchemaSaladString.Literal "12"; SchemaSaladString.Literal "1_000" ]))
            let recordType = Record (InputRecordSchema(fields = ListEntry.ofValues [ InputRecordField("null", CWLType.String); InputRecordField("ok", CWLType.File); InputRecordField("=", CWLType.Int) ]))
            for (cwlType, text) in
                [ enumType, "{type: enum, symbols: [\"true\", a, \"x,y\", \"12\", \"1_000\"]}"
                  recordType, "{type: record, fields: [{name: \"null\", type: string}, {name: ok, type: File}, {name: \"=\", type: int}]}"
                  Null, "\"null\""
                  Union (ResizeArray [ Null; CWLType.File ]), "[\"null\", File]" ] do
                Expect.equal (Encode.cwlTypeToYamlString cwlType) text "The flow text of the type"
                Expect.isTrue (CWLType.typesEqual cwlType (text |> Reader.readRoot |> Decode.cwlTypeDecoder')) (sprintf "%s should decode to the same type." text)
            Expect.equal (Encode.encodeCWLTypeShorthand Null |> Encode.writeYaml |> fun text -> text.Trim()) "\"null\"" "The shorthand of the null type is quoted."
        testCase "an expression with a NEL is written in double quotes and an expression without one is a literal block" <| fun _ ->
            let expressionTool (expression: string) =
                Kit.expressionTool [ "requirements: [{class: InlineJavascriptRequirement}]"; "inputs: []"; "outputs: [{id: o, type: int}]"; "expression: \"" + expression + "\"" ]
            for (expression, spelling) in [ "${\\n  return {o: 1};\\N\\n}", "expression: \"${\\n  return {o: 1};\\N\\n}\""; "${\\n  return {o: 1};\\n}", "expression: |-" ] do
                let _, written = Kit.roundTrip expression Decode.decodeExpressionTool Encode.encodeExpressionToolDescription (expressionTool expression)
                Expect.stringContains written spelling (sprintf "%s: the expression should be written as %s.\n%s" expression spelling written)
        testCase "the comment behind the header of a block scalar stays behind the double-quoted scalar that replaces it" <| fun _ ->
            // A literal or folded doc or label whose text holds a NEL, a line separator or a paragraph separator is written in double quotes.
            // A double-quoted scalar has no header, so its comment moves to the end of its line, and it stays there when the text is read again.
            for (what, field, header, lines, text, written) in
                [ "a literal doc", "doc", "|  # keep this", [ "  first \u2028"; "  second" ], "first \u2028\nsecond\n", "\"first \\L\\nsecond\\n\" # keep this"
                  "a folded label", "label", ">-  # note", [ "  folded\u0085"; "  text" ], "folded\u0085 text", "\"folded\\N text\" # note"
                  "a literal label", "label", "|-  # c", [ "  a\u2029b"; "  c" ], "a\u2029b\nc", "\"a\\Pb\\nc\" # c"
                  "a folded doc", "doc", ">  # d", [ "  x\u0085y"; "  z" ], "x\u0085y z\n", "\"x\\Ny z\\n\" # d"
                  "a literal doc that keeps its final lines", "doc", "|+  # k", [ "  a\u2028b"; ""; "  c"; "" ], "a\u2028b\n\nc\n\n", "\"a\\Lb\\n\\nc\\n\\n\" # k" ] do
                let yaml = Kit.tool ([ field + ": " + header ] @ lines @ [ "baseCommand: echo"; "inputs: []"; "outputs: []" ])
                let decoded = Decode.decodeCommandLineTool yaml
                let textOf (tool: CWLToolDescription) =
                    if field = "doc" then tool.Doc |> Option.bind (fun doc -> doc.TryText) else tool.Label |> Option.bind SchemaSaladString.tryLiteral
                Expect.equal (textOf decoded) (Some text) (sprintf "%s: the block scalar holds the text." what)
                let output = Encode.encodeToolDescription decoded
                Expect.stringContains output (field + ": " + written) (sprintf "%s: the comment stands behind the quoted scalar.\n%s" what output)
                let again = Decode.decodeCommandLineTool output
                Expect.equal (textOf again) (Some text) (sprintf "%s: the text reads back." what)
                Expect.equal again decoded (sprintf "%s: the written text should decode to the same tool." what)
                Expect.equal (Encode.encodeToolDescription again) output (sprintf "%s: the comment is still there when the text is written again." what)
            // A plain scalar on several lines that gets quotes keeps its lines.
            let plain = Decode.decodeCommandLineTool (Kit.tool [ "label: ?x"; "  second line"; "baseCommand: echo"; "inputs: []"; "outputs: []" ])
            Expect.equal plain.Label (Some (SchemaSaladString.Literal "?x second line")) "The plain scalar holds both lines."
            let _, plainOutput = Kit.rewrite "The written text should decode to the same tool." Decode.decodeCommandLineTool Encode.encodeToolDescription plain
            Expect.stringContains plainOutput "label: \"?x" "The label is written in double quotes."
    ]

let main =
    testList "Quoting" [
        tableTests
        integrationTests
    ]
