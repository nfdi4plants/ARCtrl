/// The fields of Schema Salad in a CWL document: $schemas, $namespaces, a directive as the class of a process or of a
/// map entry, and the fields of a Dirent beside a class.
module Tests.SchemaSalad

open ARCtrl.CWL
open TestingUtils
open DynamicObj
open TestingUtils.CWL
open YAMLicious

let testSchemasField =
    let tool (schemas: string) = Kit.tool [ "baseCommand: echo"; "inputs: []"; "outputs: []"; "$schemas:" + schemas ]
    testList "The $schemas field" [
        testCase "a path or a list of paths is kept as an extension field and written back" <| fun _ ->
            for schemas in [ " []"; " schema.rdf"; " [schema.rdf]"; " [a.rdf, b.rdf]"; " \"2\"" ] do
                let decoded = Decode.decodeCommandLineTool (tool schemas)
                Expect.isSome decoded.Metadata (sprintf "%s: the field is an extension field." schemas)
                Kit.rewrite (sprintf "%s: the written text should decode to the same tool." schemas) Decode.decodeCommandLineTool Encode.encodeToolDescription decoded |> ignore
        testCase "a directive, a mapping, a nested list, a number and null are decode errors, as cwltool refuses them" <| fun _ ->
            let cases =
                [ "an import in a list", " [{$import: a.yml}]", "$schemas[0]", "Expected the path of a schema file as a string, found a mapping."
                  "an include in a list", " [{$include: a.txt}]", "$schemas[0]", "Expected the path of a schema file as a string, found a mapping."
                  "an import as the whole field", " {$import: a.yml}", "$schemas", "Expected the path of a schema file or a list of paths, found a mapping."
                  "a mapping", " {a: b}", "$schemas", "Expected the path of a schema file or a list of paths, found a mapping."
                  "a nested list", " [[a]]", "$schemas[0]", "Expected the path of a schema file as a string, found a list."
                  "a number in a list", " [1]", "$schemas[0]", "Expected a string, found the plain scalar 1"
                  "null", "", "$schemas", "Expected a string, found null" ]
            for (name, schemas, path, message) in cases do
                Kit.processFails name (tool schemas) path message
    ]


let testNamespacesField =
    let tool (lines: string list) = Kit.tool ("baseCommand: echo" :: lines)
    testList "The $namespaces field" [
        testCase "a $namespaces that is no mapping is a decode error at the field, wherever an object of the document holds it" <| fun _ ->
            let decodeTool (text: string) = Decode.decodeCommandLineTool text |> ignore
            let message = "The field $namespaces takes a mapping of prefixes to IRIs"
            Kit.decodeFails "a list at the root" (fun () -> decodeTool (tool [ "inputs: []"; "outputs: []"; "$namespaces: [ex]" ])) "$namespaces" (message + ", found a list.")
            Kit.decodeFails "a text at the root" (fun () -> decodeTool (tool [ "inputs: []"; "outputs: []"; "$namespaces: x" ])) "$namespaces" (message + ", found the text")
            Kit.decodeFails "a number at the root" (fun () -> decodeTool (tool [ "inputs: []"; "outputs: []"; "$namespaces: 5" ])) "$namespaces" message
            Kit.decodeFails "null at the root" (fun () -> decodeTool (tool [ "inputs: []"; "outputs: []"; "$namespaces:" ])) "$namespaces" message
            Kit.decodeFails "a list of mappings" (fun () -> decodeTool (tool [ "inputs: []"; "outputs: []"; "$namespaces: [{ex: 'http://example.org/'}]" ])) "$namespaces" message
            Kit.decodeFails "an empty list" (fun () -> decodeTool (tool [ "inputs: []"; "outputs: []"; "$namespaces: []" ])) "$namespaces" message
            Kit.decodeFails "an input" (fun () -> decodeTool (tool [ "inputs:"; "  x:"; "    type: string"; "    $namespaces: [ex]"; "outputs: []" ])) "inputs.x.$namespaces" message
            Kit.decodeFails "a requirement" (fun () -> decodeTool (tool [ "requirements:"; "  - class: ShellCommandRequirement"; "    $namespaces: [ex]"; "inputs: []"; "outputs: []" ])) "requirements[0].$namespaces" message
            Kit.decodeFails "a default value" (fun () -> decodeTool (tool [ "inputs:"; "  x:"; "    type: Any"; "    default:"; "      a:"; "        - $namespaces: x"; "outputs: []" ])) "inputs.x.default.a[0].$namespaces" message
            Kit.decodeFails "an unknown hint" (fun () -> decodeTool (tool [ "hints:"; "  - class: ex:Hint"; "    payload:"; "      $namespaces: [ex]"; "inputs: []"; "outputs: []" ])) "hints[0].payload.$namespaces" message
            Kit.decodeFails "an inline run" (fun () ->
                Decode.decodeWorkflow (Kit.workflow [ "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    run:"; "      $namespaces: [ex]"; "      class: CommandLineTool"; "      baseCommand: echo"; "      inputs: []"; "      outputs: []"; "    in: {}"; "    out: []" ]) |> ignore) "steps.s.run.$namespaces" message
            Kit.decodeFails "a job file" (fun () -> DecodeParameters.decodeYAMLParameterFile "x: a\n$namespaces: [ex]\n") "$namespaces" message
        testCase "a $namespaces that is a mapping or a directive decodes, whatever its values are" <| fun _ ->
            for value in [ "{ex: 'http://example.org/'}"; "{}"; "{ex: 1}"; "{ex: true}"; "{ex: }"; "{ex: [a]}"; "{ex: {a: b}}"; "{1: 'http://example.org/'}"; "{$import: a.yml}"; "{$include: a.yml}"; "{$mixin: a.yml}" ] do
                let text = tool [ "inputs: []"; "outputs: []"; "$namespaces: " + value ]
                let decoded = Decode.decodeCommandLineTool text
                Kit.rewrite value Decode.decodeCommandLineTool Encode.encodeToolDescription decoded |> ignore
            Decode.decodeCommandLineTool (tool [ "inputs:"; "  x:"; "    type: string"; "    $namespaces: {ex: 'http://example.org/'}"; "outputs: []" ]) |> ignore
            DecodeParameters.decodeYAMLParameterFile "x: a\n$namespaces: {ex: 'http://example.org/'}\n" |> ignore
        testCase "a $namespaces beside a $mixin or below it is no error, because the fields drop out when the file holds a scalar" <| fun _ ->
            let withStreamable (value: string) = tool [ "inputs:"; "  x:"; "    type: File"; "    streamable: " + value; "outputs: []" ]
            for value in [ "{$mixin: t.yml, $namespaces: 5}"; "{$mixin: t.yml, $namespaces: [ex]}"; "{$mixin: t.yml, $namespaces:}"; "{$mixin: t.yml, a: {$namespaces: 5}}"; "{$mixin: t.yml, a: {b: [{$namespaces: [ex]}]}}" ] do
                let decoded = Decode.decodeCommandLineTool (withStreamable value)
                match (ListEntry.values decoded.Inputs.Value).[0].Streamable with
                | Some (SchemaSaladBool.Mixed raw) -> Expect.equal raw.MixinPath "t.yml" (sprintf "%s: the mixin keeps its path." value)
                | other -> failwith (sprintf "%s: expected a mixin, found %A" value other)
                let _, written = Kit.rewrite value Decode.decodeCommandLineTool Encode.encodeToolDescription decoded
                Expect.stringContains written value (sprintf "%s: the mapping is written as it was." value)
            let label = Decode.decodeCommandLineTool (tool [ "label: {$mixin: l.yml, $namespaces: 5}"; "inputs: []"; "outputs: []" ])
            Expect.equal label.Label (Some (SchemaSaladString.Mixed { Node = Reader.readRoot "{$mixin: l.yml, $namespaces: 5}" })) "A string with a mixin keeps the mapping."
        testCase "a $namespaces outside of a mixin stays an error" <| fun _ ->
            let withStreamable (value: string) = tool [ "inputs:"; "  x:"; "    type: File"; "    streamable: " + value; "outputs: []" ]
            Kit.toolFails "a binding without a mixin" (tool [ "inputs:"; "  x:"; "    type: string"; "    inputBinding: {prefix: -x, $namespaces: 5}"; "outputs: []" ]) "inputs.x.inputBinding.$namespaces" "The field $namespaces takes a mapping"
            Kit.toolFails "a field next to the mixin" (tool [ "inputs:"; "  x:"; "    type: File"; "    streamable: {$mixin: t.yml}"; "    ex:note: {$namespaces: 5}"; "outputs: []" ]) "inputs.x.ex:note.$namespaces" "The field $namespaces takes a mapping"
            Kit.toolFails "an import beside the field" (withStreamable "{$import: t.yml, $namespaces: 5}") "inputs.x.streamable.$namespaces" "The field $namespaces takes a mapping"
    ]


let testProcessWithDirectiveClass =
    let forms = [ "{$import: s.yml}"; "{$include: s.txt}"; "{$mixin: s.yml}"; "{$mixin: s.yml, extra: 1}" ]
    let documentText (classValue: string) = Kit.document classValue [ "baseCommand: echo"; "inputs: []"; "outputs: []" ]
    testList "A process whose class is a directive" [
        testCase "a document of one process stays as written, reads back equal and is written back as it was" <| fun _ ->
            for form in forms do
                let text = documentText form
                let decoded = Decode.decodeCWLProcessingUnit text
                Expect.equal decoded (RawProcess { Node = Reader.readRoot text }) (sprintf "%s: the process should stay as written." form)
                let _, written = Kit.rewrite form Decode.decodeCWLProcessingUnit Encode.encodeProcessingUnit decoded
                Expect.equal written text (sprintf "%s: the document should be written back as it was." form)
        testCase "the comments and blank lines around the directive stay in the written document" <| fun _ ->
            let text = "# leading\ncwlVersion: v1.2\n\n# before the class\nclass: {$import: s.yml} # trailing\n\nbaseCommand: echo\ninputs: []\noutputs: []\n"
            Expect.equal (Encode.encodeProcessingUnit (Decode.decodeCWLProcessingUnit text)) text "The layout of the document should stay."
        testCase "the model shows no inputs, outputs, requirements, hints or intent of a process of unknown class" <| fun _ ->
            let processingUnit = Decode.decodeCWLProcessingUnit (documentText "{$import: s.yml}")
            Expect.equal (CWLProcessingUnit.getInputValues processingUnit).Count 0 "No inputs."
            Expect.equal (CWLProcessingUnit.getOutputValues processingUnit).Count 0 "No outputs."
            Expect.equal (CWLProcessingUnit.getRequirements processingUnit).Count 0 "No requirements."
            Expect.equal (CWLProcessingUnit.getHints processingUnit).Count 0 "No hints."
            Expect.equal (CWLProcessingUnit.getKnownHints processingUnit).Count 0 "No known hints."
            Expect.equal (CWLProcessingUnit.getIntent processingUnit).Count 0 "No intent."
        testCase "two processes of unknown class are equal when their documents are, and the directive decides" <| fun _ ->
            let read (form: string) = Decode.decodeCWLProcessingUnit (documentText form)
            Expect.equal (read "{$import: s.yml}") (read "{$import: s.yml}") "The same directive gives the same document."
            Expect.notEqual (read "{$import: s.yml}") (read "{$import: t.yml}") "Two paths differ."
            Expect.notEqual (read "{$import: s.yml}") (read "{$include: s.yml}") "An import and an include differ."
            let one = Decode.decodeCWLProcessingUnit (documentText "{$import: s.yml}")
            let two = Decode.decodeCWLProcessingUnit (documentText "{$import: s.yml}")
            Expect.isFalse (CWLProcessingUnit.isSameObject one two) "Two decoded processes are two objects."
            Expect.isTrue (CWLProcessingUnit.isSameObject one one) "A process is the same object as itself."
        testCase "the cwlVersion of the document is still required and is read as written" <| fun _ ->
            let versioned (header: string) = header + "class: {$import: s.yml}\nbaseCommand: echo\ninputs: []\noutputs: []\n"
            Kit.processFails "a missing version" (versioned "") "" "cwlVersion"
            for version in [ "v1.0"; "v1.1"; "v1.2"; "v1.3"; "v1.5" ] do
                Expect.isTrue (match Decode.decodeCWLProcessingUnit (versioned ("cwlVersion: " + version + "\n")) with RawProcess _ -> true | _ -> false) (sprintf "%s: the document should be read." version)
        testCase "the cwlVersion of an inline run whose class is a directive changes nothing" <| fun _ ->
            let withVersion (version: string) =
                Kit.workflow [ "inputs: []"; "outputs: []"; "steps:"; "  s:"; "    run:"; "      cwlVersion: " + version; "      class: {$import: s.yml}"; "      baseCommand: echo"; "      inputs: []"; "      outputs: []"; "    in: []"; "    out: []" ]
            for version in [ "v1.1"; "v1.5" ] do
                let workflow = Decode.decodeWorkflow (withVersion version)
                Expect.isTrue (match (ListEntry.values workflow.Steps).[0].Run with RunRaw _ -> true | _ -> false) (sprintf "%s: the run stays as written." version)
        testCase "a malformed directive as the class of a process is a decode error at the class" <| fun _ ->
            let fails (name: string) (classValue: string) (path: string) (message: string) =
                Kit.processFails name (documentText classValue) path message
            fails "a path that is no string" "{$import: 42}" "class.$import" "Expected the path of the $import directive to be a string"
            fails "a second field in a directive" "{$import: s.yml, extra: 1}" "class" "A directive has exactly one field"
            fails "a mapping that is no directive" "{a: b}" "class" "Expected a string, found a mapping."
            fails "a class that is a list" "[CommandLineTool]" "class" "Expected a string"
    ]


let testClassFieldOfMapEntries =
    let toolWith (field: string) (entry: string) = Kit.tool [ "baseCommand: echo"; field + ":"; entry; "inputs: []"; "outputs: []" ]
    // Schema Salad gives the object the key of the entry as its class and drops the class field of the value unread.
    let classValues =
        [ "the key", "class: ShellCommandRequirement"
          "another requirement", "class: DockerRequirement"
          "an unknown class", "class: Bogus"
          "a number", "class: 5"
          "a list", "class: [ShellCommandRequirement]"
          "a mapping that is no directive", "class: {a: b}"
          "an import", "class: {$import: s.yml}"
          "an include", "class: {$include: s.txt}"
          "a mixin", "class: {$mixin: s.yml}"
          "a mixin beside a field", "class: {$mixin: s.yml, extra: 1}"
          "a directive that is malformed", "class: {$import: 42}" ]
    testList "The class field of a map-form requirement or hint" [
        testCase "a requirement is the requirement of its key whatever its class field holds" <| fun _ ->
            let plain = Decode.decodeCommandLineTool (toolWith "requirements" "  ShellCommandRequirement: {}")
            for (name, classField) in classValues do
                let tool = Decode.decodeCommandLineTool (toolWith "requirements" ("  ShellCommandRequirement:\n    " + classField))
                Expect.equal tool plain (sprintf "%s: the class field should change nothing." name)
                Expect.equal tool.Requirements.Value.[0] ShellCommandRequirement (sprintf "%s: the requirement should be the one of the key." name)
        testCase "a hint is the hint of its key whatever its class field holds" <| fun _ ->
            let plain = Decode.decodeCommandLineTool (toolWith "hints" "  ShellCommandRequirement: {}")
            for (name, classField) in classValues do
                let tool = Decode.decodeCommandLineTool (toolWith "hints" ("  ShellCommandRequirement:\n    " + classField))
                Expect.equal tool plain (sprintf "%s: the class field should change nothing." name)
                Expect.equal tool.Hints.Value.[0] (KnownHint ShellCommandRequirement) (sprintf "%s: the hint should be the one of the key." name)
        testCase "a hint of an unknown class keeps the key as its class and its other fields" <| fun _ ->
            for (name, classField) in classValues do
                let tool = Decode.decodeCommandLineTool (toolWith "hints" ("  acme:Hint:\n    " + classField + "\n    a: 1"))
                match tool.Hints.Value.[0] with
                | UnknownHint value ->
                    Expect.equal value.Class (Some "acme:Hint") (sprintf "%s: the class should be the key." name)
                    Expect.equal value.Raw (Reader.readRoot "class: acme:Hint\na: 1\n" |> fun node -> { Node = node }) (sprintf "%s: the hint should hold the key as its class and the other fields." name)
                | other -> failwith (sprintf "%s: expected an unknown hint, found %A" name other)
        testCase "the written hint and requirement have the class of the key and no class field of the value" <| fun _ ->
            for (name, classField) in classValues do
                for field in [ "requirements"; "hints" ] do
                    let tool = Decode.decodeCommandLineTool (toolWith field ("  ShellCommandRequirement:\n    " + classField))
                    let _, written = Kit.rewrite "the round trip" Decode.decodeCommandLineTool Encode.encodeToolDescription tool
                    Expect.stringContains written "class: ShellCommandRequirement" (sprintf "%s in %s: the written text should hold the class of the key." name field)
                    Expect.isFalse (written.Contains "$import" || written.Contains "$include" || written.Contains "$mixin" || written.Contains "Bogus") (sprintf "%s in %s: the field of the value is gone." name field)
        testCase "a class field that holds the key stays where it stands in a hint" <| fun _ ->
            let tool = Decode.decodeCommandLineTool (toolWith "hints" "  acme:Hint:\n    a: 1\n    class: acme:Hint\n    b: 2")
            Expect.stringContains (Encode.encodeToolDescription tool) "  - a: 1\n    class: acme:Hint\n    b: 2\n" "The hint is written as it stands."
    ]


let testDirentFieldsBesideAClass =
    let listing (entries: string list) =
        Kit.tool ([ "baseCommand: echo"; "requirements:"; "  - class: InitialWorkDirRequirement"; "    listing:" ] @ (entries |> List.map (fun entry -> "      " + entry)) @ [ "inputs: []"; "outputs: []" ])
    let classes = [ "{$include: c.txt}"; "{$import: c.yml}"; "{$mixin: c.yml}"; "{$mixin: c.yml, extra: 1}" ]
    testList "The fields of a Dirent beside a class" [
        testCase "the fields entry, entryname and writable are decode errors beside a class that is a directive, in an entry and in an array" <| fun _ ->
            for cls in classes do
                for field in [ "entry: foo"; "entryname: x.txt"; "writable: true" ] do
                    let name = field.Split(':').[0]
                    Kit.toolFails (sprintf "%s with %s" cls field) (listing [ "- {class: " + cls + ", location: a.txt, " + field + "}" ]) ("requirements[0].listing[0]." + name) "belong to a Dirent"
                    Kit.toolFails (sprintf "%s with %s in an array" cls field) (listing [ "- [{class: " + cls + ", location: a.txt, " + field + "}]" ]) ("requirements[0].listing[0][0]." + name) "belong to a Dirent"
        testCase "a class beside the fields of a Dirent is a decode error at the class" <| fun _ ->
            for cls in [ "Dirent"; "Foo" ] do
                Kit.toolFails ("the class " + cls) (listing [ "- {class: " + cls + ", entry: foo, entryname: x.txt}" ]) "requirements[0].listing[0].class" ("A Dirent has no class field, found the class " + cls)
        testCase "a Dirent without a class and an object with a directive as its class and no field of a Dirent still decode" <| fun _ ->
            let tool = Decode.decodeCommandLineTool (listing [ "- {entry: foo, entryname: x.txt}"; "- {class: {$include: c.txt}, location: a.txt}"; "- [{class: {$include: c.txt}, location: a.txt}]" ])
            match tool.Requirements.Value.[0] with
            | InitialWorkDirRequirement entries ->
                Expect.isTrue (match entries.[0] with DirentEntry _ -> true | _ -> false) "The first entry is the Dirent."
                Expect.isTrue (match entries.[1] with RawEntry _ -> true | _ -> false) "The second entry is the raw object."
                Expect.isTrue (match entries.[2] with FilesEntry items -> (match items.[0] with RawItem _ -> true | _ -> false) | _ -> false) "The third entry is the array with a raw item."
            | other -> failwith (sprintf "Expected an InitialWorkDirRequirement, found %A" other)
    ]


let main =
    testList "Schema Salad fields" [
        testSchemasField
        testNamespacesField
        testProcessWithDirectiveClass
        testClassFieldOfMapEntries
        testDirentFieldsBesideAClass
    ]
