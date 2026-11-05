/// The Schema Salad directives at every field position: a table of directive forms with the outcome at each kind of
/// position, and the tests of the cases of the model that stand for a directive.
module Tests.Directives

open ARCtrl.CWL
open TestingUtils
open TestingUtils.CWL
open YAMLicious
open Tests.Positions

let private everyKind =
    [ Kind.Text; Kind.Expression; Kind.Bool; Kind.Int; Kind.Number; Kind.Enum; Kind.Type; Kind.Object; Kind.Binding
      Kind.Entry; Kind.OutEntry; Kind.MapValue; Kind.WholeList; Kind.WholeStrings; Kind.WholeNumbers; Kind.Class; Kind.DefinitionType ]

let private allBut (kinds: Kind list) = everyKind |> List.filter (fun kind -> not (List.contains kind kinds))

let private kindName (kind: Kind) =
    match kind with
    | Kind.Text -> "text"
    | Kind.Expression -> "expression"
    | Kind.Bool -> "bool"
    | Kind.Int -> "int"
    | Kind.Number -> "number"
    | Kind.Enum -> "enum"
    | Kind.Type -> "type"
    | Kind.Object -> "object"
    | Kind.Binding -> "binding"
    | Kind.Entry -> "list entry"
    | Kind.OutEntry -> "step output"
    | Kind.MapValue -> "map value"
    | Kind.WholeList -> "whole list"
    | Kind.WholeStrings -> "whole list of strings"
    | Kind.WholeNumbers -> "whole list of numbers"
    | Kind.Class -> "class"
    | Kind.DefinitionType -> "definition type"

/// The value text at a position, and the outcome at each kind of position that the row runs at.
let private rows : (string * (Kind list * Outcome) list) list =
    let mapValue = [ Kind.MapValue ], Fails "cannot stand for the value"
    [ "{$import: a.yml}", [ allBut [ Kind.MapValue; Kind.Class; Kind.DefinitionType ], Import "a.yml"; mapValue; [ Kind.Class; Kind.DefinitionType ], Kept ]
      "{$import: \"42\"}", [ allBut [ Kind.MapValue; Kind.Class; Kind.DefinitionType ], Import "42"; mapValue; [ Kind.Class; Kind.DefinitionType ], Kept ]
      "{$include: t.txt}",
      [ [ Kind.Text; Kind.Expression; Kind.Int; Kind.Enum; Kind.Type; Kind.OutEntry ], Include "t.txt"
        [ Kind.Bool; Kind.Number; Kind.Object; Kind.Binding; Kind.Entry; Kind.WholeList; Kind.WholeStrings; Kind.WholeNumbers ], Fails "$include yields the text of a file"
        mapValue
        [ Kind.Class; Kind.DefinitionType ], Kept ]
      "{$mixin: m.yml}",
      [ [ Kind.Text; Kind.Expression; Kind.WholeStrings ], Mixin "m.yml"
        [ Kind.Bool; Kind.Int; Kind.Number; Kind.Enum; Kind.Type; Kind.Object; Kind.Binding; Kind.Entry; Kind.OutEntry; Kind.WholeNumbers ], mixed "{$mixin: m.yml}"
        [ Kind.MapValue ], Keyed "m.yml"
        [ Kind.WholeList ], Fails "$mixin completes an object"
        [ Kind.Class; Kind.DefinitionType ], Kept ]
      "{$mixin: m.yml, extra: 1}",
      [ allBut [ Kind.MapValue; Kind.WholeList; Kind.Class; Kind.DefinitionType ], mixed "{$mixin: m.yml, extra: 1}"
        [ Kind.MapValue ], Keyed "m.yml"
        [ Kind.WholeList ], Fails "$mixin completes an object"
        [ Kind.Class; Kind.DefinitionType ], Kept ]
      "{$import: a.yml, extra: 1}", [ allBut [ Kind.MapValue ], Fails "exactly one field"; mapValue ]
      "{$include: t.txt, extra: 1}", [ allBut [ Kind.MapValue ], Fails "exactly one field"; mapValue ]
      "{$mixin: m.yml, $import: a.yml}", [ allBut [ Kind.MapValue ], Fails "exactly one field"; mapValue ]
      "{$include: t.txt, $import: a.yml}", [ allBut [ Kind.MapValue ], Fails "exactly one field"; mapValue ]
      "{$import: 42}", [ allBut [ Kind.MapValue ], FailsAt (".$import", "to be a string"); mapValue ]
      "{$include: true}", [ allBut [ Kind.MapValue ], FailsAt (".$include", "to be a string"); mapValue ]
      "{$include: }", [ allBut [ Kind.MapValue ], FailsAt (".$include", "to be a string"); mapValue ]
      "{$import: {a: b}}", [ allBut [ Kind.MapValue ], FailsAt (".$import", "to be a string"); mapValue ]
      "{$mixin: [m.yml]}", [ everyKind, FailsAt (".$mixin", "to be a string") ]
      "{$mixin: 42, extra: 1}", [ everyKind, FailsAt (".$mixin", "to be a string") ]
      "{$import: \"\"}", [ allBut [ Kind.MapValue ], FailsAt (".$import", "is empty"); mapValue ]
      "{$mixin: \"\", extra: 1}", [ everyKind, FailsAt (".$mixin", "is empty") ]
      "{a: b}",
      [ [ Kind.Text; Kind.Expression; Kind.Enum; Kind.Class ], Fails "found a mapping"
        [ Kind.Bool ], Fails "Expected a bool"
        [ Kind.Int ], Fails "Expected an int or an expression"
        [ Kind.Number ], Fails "Expected an int"
        [ Kind.WholeStrings ], Fails "Expected a list of strings"
        [ Kind.WholeNumbers ], Fails "Expected a list of numbers"
        [ Kind.DefinitionType ], FailsOn ("requirements[0].types[0]", "a type that is no name") ]
      "[a]", [ [ Kind.Expression ], Fails "Expected a string or a directive" ]
      "42",
      [ [ Kind.Text ], Fails "plain scalar"
        [ Kind.Binding ], Fails "Expected a mapping"
        [ Kind.WholeStrings ], Fails "Expected a list of strings"
        [ Kind.WholeNumbers ], Fails "Expected a list of numbers" ]
      "true", [ [ Kind.Text ], Fails "plain scalar"; [ Kind.Bool ], Value (box true) ]
      "false", [ [ Kind.Bool ], Value (box false) ]
      "\"true\"", [ [ Kind.Bool ], Fails "Expected a bool" ]
      "3", [ [ Kind.Int ], Value (box (BindingPosition.Number 3)) ]
      "\"$(1 + 1)\"", [ [ Kind.Int ], Value (box (BindingPosition.Expression "$(1 + 1)")) ]
      "\"abc\"", [ [ Kind.Int ], Fails "Expected an int or an expression" ]
      "{$mixin: m.yml, loadContents: abc}", [ [ Kind.Binding ], FailsAt (".loadContents", "Expected a bool") ] ]

let private told (outcome: Outcome) =
    match outcome with
    | Import path -> "reads as the import of " + path + ", is written back as it was and reads back equal"
    | Include path -> "reads as the include of " + path + ", is written back as it was and reads back equal"
    | Mixin path -> "reads as the mixin of " + path + ", is written back as it was and reads back equal"
    | Mixed _ -> "stays the mapping as written, is written back as it was and reads back equal"
    | Keyed path -> "reads as the mixin of " + path + " with the key as its id and reads back equal"
    | Raw _
    | Kept -> "keeps the object that holds it as written, is written back as it was and reads back equal"
    | Value _ -> "reads as its plain value and reads back equal"
    | Fails keyword
    | FailsAt (_, keyword)
    | FailsOn (_, keyword) -> "is a decode error that says " + keyword

/// Run the check at every position of the kind and fail with the names of all the positions where it fails.
let private atEvery (kind: Kind) (check: Position -> unit) =
    let positions = ofKind kind
    Expect.isFalse positions.IsEmpty (sprintf "The table should have positions of the kind %s." (kindName kind))
    let failures =
        positions
        |> List.choose (fun position ->
            try
                check position
                None
            with error ->
                Some (position.Name + ": " + error.Message))
    if not failures.IsEmpty then
        failwith (String.concat "\n\n" failures)

let private expectOutcome (form: string) (outcome: Outcome) (position: Position) =
    let name = position.Name + " with " + form
    let decode () = position.Decode (position.Document form)
    match outcome with
    | Fails keyword -> Kit.decodeFails name decode position.Path keyword
    | FailsAt (below, keyword) -> Kit.decodeFails name decode (position.Path + below) keyword
    | FailsOn (path, keyword) -> Kit.decodeFails name decode path keyword
    | expected ->
        let expected =
            match expected, position.Holder with
            | Kept, Some holder -> Raw { Node = Reader.readRoot (holder form) }
            | Kept, None -> failwith (name + ": the position has no holder.")
            | other, _ -> other
        let decoded, written = Kit.roundTrip name position.Decode position.Encode (position.Document form)
        Expect.equal (position.Read decoded) expected (sprintf "%s: the position should hold the value." name)
        match expected with
        | Keyed _
        | Value _ -> ()
        | _ -> Expect.stringContains written form (sprintf "%s: the value should be written back as it was.\n%s" name written)

/// The value text whose decoded object each edit changes, at the positions of the kinds.
let private variantRows : (Kind list * string * (string * string * string) list) list =
    let path = "path", "a.yml", "b.yml"
    let toMixin = "directive", "$import", "$mixin"
    [ [ Kind.Text; Kind.Expression; Kind.Int; Kind.Enum; Kind.Type; Kind.OutEntry; Kind.Class; Kind.DefinitionType ], "{$import: a.yml}", [ path; toMixin; ("directive", "$import", "$include") ]
      [ Kind.Bool; Kind.Number; Kind.Object; Kind.Binding; Kind.Entry; Kind.WholeStrings; Kind.WholeNumbers ], "{$import: a.yml}", [ path; toMixin ]
      [ Kind.WholeList ], "{$import: a.yml}", [ path ]
      [ Kind.MapValue ], "{$mixin: m.yml}", [ ("path", "m.yml", "n.yml") ]
      allBut [ Kind.WholeList ], "{$mixin: m.yml, extra: 1}", [ ("field beside the mixin", "extra: 1", "extra: 2") ] ]

/// The text that the writer gives for a field that is one $import of a.yml as a whole.
let private wholeSpellings : (string * string) list =
    [ "the inputs of a tool", "inputs: {$import: a.yml}"
      "the outputs of a tool", "outputs: {$import: a.yml}"
      "the inputs of a workflow", "inputs: {$import: a.yml}"
      "the outputs of a workflow", "outputs: {$import: a.yml}"
      "the steps of a workflow", "steps: {$import: a.yml}"
      "the inputs of an expression tool", "inputs: {$import: a.yml}"
      "the outputs of an expression tool", "outputs: {$import: a.yml}"
      "the inputs of an operation", "inputs: {$import: a.yml}"
      "the outputs of an operation", "outputs: {$import: a.yml}"
      "the in field of a step", "in: {$import: a.yml}"
      "the out field of a step", "out: {$import: a.yml}"
      "the requirements of a tool", "requirements: {$import: a.yml}"
      "the hints of a tool", "hints: {$import: a.yml}"
      "the requirements of a step", "requirements: {$import: a.yml}"
      "the hints of a step", "hints: {$import: a.yml}"
      "the envDef", "- {$import: a.yml}"
      "the packages", "- {$import: a.yml}"
      "the fields of a record", "fields: {$import: a.yml}"
      "the types of a SchemaDefRequirement", "- {$import: a.yml}"
      "the intent of a tool", "intent: {$import: a.yml}"
      "the intent of a workflow", "intent: {$import: a.yml}"
      "the intent of an expression tool", "intent: {$import: a.yml}"
      "the intent of an operation", "intent: {$import: a.yml}"
      "the expressionLib", "- {$import: a.yml}"
      "the specs of a package", "- {$import: a.yml}"
      "the version of a package", "- {$import: a.yml}"
      "the symbols of an enum", "symbols: {$import: a.yml}"
      "the successCodes as a whole", "successCodes: {$import: a.yml}"
      "the temporaryFailCodes as a whole", "temporaryFailCodes: {$import: a.yml}"
      "the permanentFailCodes as a whole", "permanentFailCodes: {$import: a.yml}" ]

let private tableTests =
    testList "The directive table" [
        for (form, cells) in rows do
            for (kinds, outcome) in cells do
                for kind in kinds do
                    testCase (sprintf "%s %s at every %s position" form (told outcome) (kindName kind)) <| fun _ ->
                        atEvery kind (expectOutcome form outcome)
        for (kinds, form, edits) in variantRows do
            for kind in kinds do
                let what = edits |> List.map (fun (change, _, _) -> change) |> List.distinct |> String.concat " or another "
                testCase (sprintf "another %s in %s makes another object at every %s position" what form (kindName kind)) <| fun _ ->
                    atEvery kind (fun position -> Kit.variants position.Name (position.Document form) position.Decode edits)
        for kind in everyKind do
            if ofKind kind |> List.exists (fun position -> not position.Plain.IsEmpty) then
                testCase (sprintf "the plain values of every %s position read as their literal outcomes" (kindName kind)) <| fun _ ->
                    atEvery kind (fun position ->
                        for (form, outcome) in position.Plain do
                            expectOutcome form outcome position)
        testCase "a field that is one import equals the list of that import and is written as the table gives" <| fun _ ->
            let wholes = [ Kind.WholeList; Kind.WholeStrings; Kind.WholeNumbers ] |> List.collect ofKind
            Expect.equal (wholes |> List.map (fun position -> position.Name) |> List.sort) (wholeSpellings |> List.map fst |> List.sort) "The table should name every whole list."
            for position in wholes do
                let spelling = wholeSpellings |> List.find (fun (name, _) -> name = position.Name) |> snd
                let whole = position.Decode (position.Document "{$import: a.yml}")
                let listed = position.Decode (position.Document "[{$import: a.yml}]")
                Expect.equal whole listed (sprintf "%s: the spelling is layout, so the objects are equal." position.Name)
                let written = position.Encode whole
                Expect.stringContains written spelling (sprintf "%s: the whole field should be written as the table gives.\n%s" position.Name written)
                Expect.stringContains (position.Encode listed) "- {$import: a.yml}" (sprintf "%s: the listed import should be written as an entry." position.Name)
    ]

let private raw (text: string) : RawYaml = { Node = Reader.readRoot text }

let private modelTests =
    let tool (lines: string list) = Kit.tool ("baseCommand: echo" :: lines)
    let toolOf (lines: string list) = Decode.decodeCommandLineTool (tool lines)
    testList "The cases of the model that stand for a directive" [
        testCase "the mixin path of a raw value is the path of the directive, and the empty text for any other node" <| fun _ ->
            Expect.equal (raw "{$mixin: tool.yml}").MixinPath "tool.yml" "A mixin on its own has its path."
            Expect.equal (raw "{$mixin: tool.yml, label: x}").MixinPath "tool.yml" "A mixin beside a field has its path."
            Expect.equal (raw "{label: x}").MixinPath "" "A mapping without a mixin has none."
            Expect.equal (raw "{$import: tool.yml}").MixinPath "" "An import is no mixin."
            Expect.equal (raw "[a]").MixinPath "" "A list has none."
            Expect.equal (raw "x").MixinPath "" "A scalar has none."
        testCase "a bool field that the document leaves out is none" <| fun _ ->
            let decoded = toolOf [ "inputs:"; "  x:"; "    type: File"; "outputs: {}" ]
            Expect.isNone (ListEntry.values decoded.Inputs.Value).[0].LoadContents "A missing field is none."
        testCase "the getters of a workflow, a tool and a processing unit give the objects of the value entries and leave the directives out" <| fun _ ->
            let workflow =
                Decode.decodeWorkflow (
                    Kit.workflow
                        [ "inputs:"; "  - {$import: inputs.yml}"; "  - {id: a, type: string}"; "  - {id: b, $mixin: m.yml}"
                          "outputs:"; "  - {id: o, type: string, outputSource: s/out}"; "  - {$import: outputs.yml}"
                          "steps:"; "  - {$import: steps.yml}"; "  - {id: s, run: t.cwl, in: {}, out: [out]}"; "  - {$import: more-steps.yml}" ])
            Expect.equal (CWLWorkflowDescription.getInputValues workflow |> Seq.map (fun input -> input.Name) |> List.ofSeq) [ "a" ] "Only the input is an object."
            Expect.equal (CWLWorkflowDescription.getOutputValues workflow |> Seq.map (fun output -> output.Name) |> List.ofSeq) [ "o" ] "Only the output is an object."
            Expect.equal (CWLWorkflowDescription.getStepValues workflow |> Seq.map (fun step -> step.Id) |> List.ofSeq) [ Some "s" ] "Only the step is an object."
            CWLWorkflowDescription.getInputValues(workflow).Clear()
            Expect.equal workflow.Inputs.Count 3 "The list is a new one, so clearing it leaves the workflow as it was."
            let unit = CWLProcessingUnit.Workflow workflow
            Expect.equal (CWLProcessingUnit.getInputValues unit |> Seq.map (fun input -> input.Name) |> List.ofSeq) [ "a" ] "The processing unit reads the input only."
            Expect.equal (CWLProcessingUnit.getOutputValues unit |> Seq.map (fun output -> output.Name) |> List.ofSeq) [ "o" ] "The processing unit reads the output only."
            let decoded = toolOf [ "inputs: [{$import: ports.yml}, {id: a, type: string}, {id: x, $mixin: m.yml}]"; "outputs: [{id: o, type: File}, {$import: out.yml}, {$mixin: o.yml}]" ]
            Expect.equal (CWLToolDescription.getInputValuesOrEmpty decoded |> Seq.map (fun input -> input.Name) |> List.ofSeq) [ "a" ] "The tool inputs without the directives."
            Expect.equal (CWLToolDescription.getOutputValues decoded |> Seq.map (fun output -> output.Name) |> List.ofSeq) [ "o" ] "The tool outputs without the directives."
        testCase "the getters of a step, a record, an enum, an output and an envDef give the values and leave the directives out" <| fun _ ->
            let step =
                (ListEntry.values (Decode.decodeWorkflow (Kit.step [ "run: t.cwl"; "in: [{$import: more-in.yml}, {id: a, source: x}, {id: b, $mixin: m.yml}]"; "out: [r, {$import: more-out.yml}, {$include: id.txt}, {id: o, $mixin: m.yml}]" ])).Steps).[0]
            Expect.equal (WorkflowStep.getInputValues step |> Seq.map (fun input -> input.Id) |> List.ofSeq) [ "a" ] "Only the step input is an object."
            Expect.equal (List.ofSeq (WorkflowStep.getOutputValues step)) [ StepOutputString "r" ] "Only the output r is written out."
            Expect.equal (step.Out |> Seq.map StepOutput.tryId |> List.ofSeq) [ Some "r"; None; None; None ] "A directive has no id, and neither has an output with a mixin."
            WorkflowStep.getInputValues(step).Clear()
            Expect.equal step.In.Count 3 "The list is a new one, so clearing it leaves the step as it was."
            let schemaOf (lines: string list) = (ListEntry.values (toolOf ([ "inputs:"; "  a:"; "    type:" ] @ (lines |> List.map (fun line -> "      " + line)) @ [ "outputs: {}" ])).Inputs.Value).[0].Type_.Value
            let record () = match schemaOf [ "type: record"; "fields: [{$import: more.yml}, {name: f, type: string}, {$import: last.yml}]" ] with Record schema -> schema | other -> failwithf "Expected a record, found %A" other
            Expect.equal (InputRecordSchema.getFieldValuesOrEmpty (record ()) |> Seq.map (fun field -> field.Name) |> List.ofSeq) [ "f" ] "Only the field is an object."
            Expect.equal (InputRecordSchema.getFieldValuesOrEmpty (InputRecordSchema())).Count 0 "A record without fields has none."
            let fields = record ()
            (InputRecordSchema.getFieldValuesOrEmpty fields).Clear()
            Expect.equal fields.Fields.Value.Count 3 "The list is a new one."
            Expect.equal (InputRecordSchema.tryGetFieldType "f" (record ())) (Some CWLType.String) "The field is found behind the import."
            Expect.isNone (InputRecordSchema.tryGetFieldType "g" (record ())) "A field of a file is unknown."
            match schemaOf [ "type: enum"; "symbols: [a, {$import: more.yml}, {$include: sym.txt}, b]" ] with
            | Enum schema -> Expect.equal (List.ofSeq (InputEnumSchema.getSymbolValues schema)) [ "a"; "b" ] "Only the plain symbols are strings."
            | other -> failwithf "Expected an enum, found %A" other
            let output = (ListEntry.values (Decode.decodeWorkflow (Kit.workflow [ "inputs: {}"; "outputs:"; "  o:"; "    type: string"; "    outputSource: [a/x, {$import: p.yml}]"; "steps: {}" ])).Outputs).[0]
            Expect.equal (List.ofSeq (output.GetOutputSources())) [ "a/x" ] "Only the text is an id."
            match (toolOf [ "requirements:"; "  - class: EnvVarRequirement"; "    envDef: [{$import: more-env.yml}, {envName: A, envValue: b}]"; "inputs: {}"; "outputs: {}" ]).Requirements.Value.[0] with
            | EnvVarRequirement envs -> Expect.equal (ListEntry.values envs |> Seq.map (fun env -> env.EnvName) |> List.ofSeq) [ SchemaSaladString.Literal "A" ] "The values leave the imports out."
            | other -> failwithf "Expected an EnvVarRequirement, found %A" other
        testCase "the value of a case that stands for a directive is none, and a path or a location that is a directive holds no text" <| fun _ ->
            let mixinNode = raw "{$mixin: b.yml}"
            Expect.equal (SchemaSaladBool.tryValue (SchemaSaladBool.Value false)) (Some false) "A value gives its bool."
            Expect.isNone (SchemaSaladBool.tryValue (SchemaSaladBool.Imported "b.yml")) "An import gives no bool."
            Expect.isNone (SchemaSaladBool.tryValue (SchemaSaladBool.Mixed mixinNode)) "A mixin gives no bool."
            Expect.equal (FileSize.tryBytes (FileSize.Bytes 3L)) (Some 3L) "A size gives its bytes."
            Expect.isNone (FileSize.tryBytes (FileSize.Imported "s.yml")) "An import gives no bytes."
            Expect.equal (ListEntry.tryValue (ListEntry.Value (InputBinding.create()))) (Some (InputBinding.create())) "A binding gives its value."
            Expect.isNone (ListEntry.tryValue (ListEntry.Imported "b.yml" : ListEntry<InputBinding>)) "An import gives no binding."
            Expect.isNone (ListEntry.tryValue (ListEntry.Mixed mixinNode : ListEntry<OutputBinding>)) "A mixin gives no binding."
            Expect.isNone (HintEntry.tryAsRequirement (ImportedHint "hints.yml")) "An imported hint is no requirement."
            let literal (text: string) = SchemaSaladString.Literal text
            let directive = SchemaSaladString.Import "p.yml"
            Expect.equal (FileInstance.pathOrLocation (FileInstance(path = literal "p", location = literal "l"))) "p" "The path comes first."
            Expect.equal (FileInstance.pathOrLocation (FileInstance(location = literal "l"))) "l" "The location stands in for a missing path."
            Expect.equal (FileInstance.pathOrLocation (FileInstance(path = directive, location = literal "l"))) "l" "A path that is a directive has no text, so the location counts."
            Expect.equal (FileInstance.pathOrLocation (FileInstance(path = directive, location = directive))) "" "Two directives hold no text."
            Expect.equal (FileInstance.pathOrLocation (FileInstance())) "" "A File without a path and a location holds no text."
            Expect.equal (DirectoryInstance.pathOrLocation (DirectoryInstance(path = directive, location = literal "d"))) "d" "A Directory reads the same way."
            Expect.equal (DirectoryInstance.pathOrLocation (DirectoryInstance(location = directive))) "" "A directive holds no text in a Directory either."
        testCase "a directive in an enum shows as the directive in the text of the symbol and is no symbol" <| fun _ ->
            Expect.equal (LoadListingEnum.toCwlString (ImportedLoadListing "l.yml")) "$import: l.yml" "The import shows with its directive."
            Expect.equal (LoadListingEnum.toCwlString (IncludedLoadListing "l.txt")) "$include: l.txt" "The include shows with its directive."
            Expect.equal (LoadListingEnum.toCwlString (MixedLoadListing (raw "{$mixin: l.yml, extra: 1}"))) "$mixin: l.yml" "The mixin shows with its path."
            Expect.equal (ImportedScatterMethod "m.yml").AsCwlString "$import: m.yml" "The scatterMethod shows the import."
            Expect.equal (IncludedPickValue "m.txt").AsCwlString "$include: m.txt" "The pickValue shows the include."
            Expect.equal (ImportedLinkMerge "m.yml").AsCwlString "$import: m.yml" "The linkMerge shows the import."
            Expect.isNone (LoadListingEnum.tryParse "$import: l.yml") "The text of a directive is no symbol."
        testCase "the cases of a directive differ from each other and from a value of the same text" <| fun _ ->
            let file (basename: SchemaSaladString) = FileInstance(location = SchemaSaladString.Literal "f.txt", basename = basename)
            let record (field: ListEntry<InputRecordField>) = CWLType.Record (InputRecordSchema(fields = ResizeArray [ field ]))
            let enum (symbol: SchemaSaladString) = Enum (InputEnumSchema(ResizeArray [ symbol ]))
            Kit.expectSame "two imports of a type" (ImportedType "t.yml") (ImportedType "t.yml")
            Kit.expectSame "two includes of a type" (IncludedType "t.txt") (IncludedType "t.txt")
            Kit.expectSame "two imports of a basename" (DirectoryInstance(basename = SchemaSaladString.Import "a")) (DirectoryInstance(basename = SchemaSaladString.Import "a"))
            Kit.expectSame "two mixed items" (MixedItem (raw "{$mixin: a.yml, basename: z}")) (MixedItem (raw "{$mixin: a.yml, basename: z}"))
            Kit.expectSame "two mixed types" (MixedType (raw "{$mixin: t.yml}")) (MixedType (raw "{$mixin: t.yml}"))
            Kit.expectSame "two imported requirements" (ImportedRequirement "r.yml") (ImportedRequirement "r.yml")
            Kit.expectSame "two mixed requirements" (MixedRequirement (raw "{$mixin: r.yml, class: NetworkAccess}")) (MixedRequirement (raw "{$mixin: r.yml, class: NetworkAccess}"))
            Expect.equal (SchemaSaladString.Mixed (raw "{$mixin: notes.md, extra: 1}")).Value "notes.md" "The value of a mixin beside a field is its path."
            // A requirement compares field by field, so a mixin with another field beside it in each field of a DockerRequirement
            // makes another requirement.
            let docker (field: string) (extra: string) =
                (Decode.decodeCommandLineTool (Kit.toolWith [ "requirements:"; "  - {class: DockerRequirement, " + field + ": {$mixin: m.yml, extra: " + extra + "}}" ])).Requirements.Value.[0]
            for field in [ "dockerPull"; "dockerFile"; "dockerImageId"; "dockerLoad"; "dockerImport"; "dockerOutputDirectory" ] do
                Kit.expectSame field (docker field "1") (docker field "1")
                Expect.notEqual (docker field "1") (docker field "2") (sprintf "%s: another field beside the mixin should make another requirement." field)
            for (what, left, right) in
                [ "an import of a type and a reference", box (ImportedType "t.yml"), box (Reference "t.yml")
                  "an include of a type and an import", box (IncludedType "t.txt"), box (ImportedType "t.txt")
                  "an include of a type and a reference", box (IncludedType "t.txt"), box (Reference "t.txt")
                  "a position that is an import and an include", box (BindingPosition.Imported "p.yml"), box (BindingPosition.Included "p.yml")
                  "a position that is an import and an expression", box (BindingPosition.Imported "p.yml"), box (BindingPosition.Expression "p.yml")
                  "an import of a file called deep_listing and the deep listing", box (ImportedLoadListing "deep_listing"), box DeepListing
                  "a size that is an import and a size of zero", box (FileSize.Imported "s.yml"), box (FileSize.Bytes 0L)
                  "a File with an import of a and with the literal a", box (file (SchemaSaladString.Import "a")), box (file (SchemaSaladString.Literal "a"))
                  "a File with an import and with an include", box (file (SchemaSaladString.Import "a")), box (file (SchemaSaladString.Include "a"))
                  "a Directory with an import of a and with the literal a", box (DirectoryInstance(basename = SchemaSaladString.Import "a")), box (DirectoryInstance(basename = SchemaSaladString.Literal "a"))
                  "an imported item and a mixed item", box (ImportedItem "a.yml"), box (MixedItem (raw "{$mixin: a.yml}"))
                  "a raw requirement and a mixed one of the same text", box (RawRequirement (raw "{class: {$import: c.yml}}")), box (MixedRequirement (raw "{class: {$import: c.yml}}"))
                  "a raw run and a mixed one", box (RunRaw (raw "{a: 1}")), box (RunMixed (raw "{a: 1}"))
                  "a raw listing entry and a mixed string entry", box (RawEntry (raw "{a: 1}")), box (StringEntry (SchemaSaladString.Mixed (raw "{a: 1}")))
                  "a run that is an import and a run of the same path", box (RunImport "tool.cwl"), box (RunString "tool.cwl")
                  "a mixin and an import of the same path", box (SchemaSaladString.Mixin "a"), box (SchemaSaladString.Import "a")
                  "a mixin on its own and a mixin beside a field", box (SchemaSaladString.Mixin "Dockerfile"), box (SchemaSaladString.Mixed (raw "{$mixin: Dockerfile, extra: 1}"))
                  "a source that is an import and the text", box (OutputSource.Single (SchemaSaladString.Import "a")), box (OutputSource.Single (SchemaSaladString.Literal "a"))
                  "imports of the fields of two files", box (record (ListEntry.Imported "a.yml")), box (record (ListEntry.Imported "b.yml"))
                  "a symbol that is an import and an include", box (enum (SchemaSaladString.Import "a")), box (enum (SchemaSaladString.Include "a"))
                  "a symbol that is an import and a literal", box (enum (SchemaSaladString.Import "a")), box (enum (SchemaSaladString.Literal "a")) ] do
                Expect.notEqual left right (sprintf "%s should differ." what)
            Expect.notEqual ((file (SchemaSaladString.Import "a")).GetHashCode()) ((file (SchemaSaladString.Literal "a")).GetHashCode()) "The hash tells an import from a literal."
            Expect.isFalse (CWLType.typesEqual (record (ListEntry.Imported "a.yml")) (record (ListEntry.Value (InputRecordField("a", CWLType.String))))) "An import is no field."
            let bool = ofKind Kind.Bool |> List.head
            let boolOf (value: string) = bool.Decode (bool.Document value)
            Expect.notEqual (boolOf "{$import: b.yml}") (boolOf "true") "An import is no true."
            Expect.notEqual (boolOf "{$import: b.yml}") (boolOf "false") "An import is no false."
            let binding = ofKind Kind.Binding |> List.head
            let bindingOf (value: string) = binding.Decode (binding.Document value)
            Expect.notEqual (bindingOf "{$import: b.yml}") (bindingOf "{}") "An import differs from a binding without fields."
            Expect.equal (binding.Read (bindingOf "{prefix: -x}")) (Value (box (InputBinding.create(prefix = SchemaSaladString.Literal "-x")))) "A binding with a field is the value."
            Expect.equal (binding.Read (bindingOf "{}")) (Value (box (InputBinding.create()))) "A binding without fields is the value too."
        testCase "an input and an output differ in equality and in the hash by the directive of their binding" <| fun _ ->
            let mixinNode = raw "{$mixin: b.yml}"
            let input (binding: ListEntry<InputBinding> option) = box (CWLInput("x", CWLType.String, ?inputBinding = binding))
            let output (binding: ListEntry<OutputBinding> option) = box (CWLOutput("o", CWLType.String, ?outputBinding = binding))
            // An object without a binding and an object with an empty binding may hash alike on a target, because the empty binding
            // has no field to hash. The test compares their hashes only when a directive is among the two.
            for objects in
                [ [ input None; input (Some (ListEntry.Value (InputBinding.create()))); input (Some (ListEntry.Imported "b.yml")); input (Some (ListEntry.Imported "c.yml")); input (Some (ListEntry.Mixed mixinNode)) ]
                  [ output None; output (Some (ListEntry.Value (OutputBinding.create()))); output (Some (ListEntry.Imported "b.yml")); output (Some (ListEntry.Imported "c.yml")); output (Some (ListEntry.Mixed mixinNode)) ] ] do
                for (index, left) in List.indexed objects do
                    for (other, right) in List.indexed objects do
                        if index <> other then
                            Expect.notEqual left right (sprintf "The objects %d and %d differ." index other)
                            if index >= 2 || other >= 2 then
                                Expect.notEqual (left.GetHashCode()) (right.GetHashCode()) (sprintf "The objects %d and %d hash differently." index other)
        testCase "the raw and imported cases compare and hash by their nodes and paths" <| fun _ ->
            // Two values of one case from equal nodes are equal and hash alike, and a changed node makes another value.
            let check (name: string) (make: RawYaml -> obj) =
                Kit.expectSame name (make (raw "{class: {$import: c.yml}, a: [1, 2]}")) (make (raw "{class: {$import: c.yml}, a: [1, 2]}"))
                for (change, text) in [ "a scalar", "{class: {$import: c.yml}, a: [1, 3]}"; "a key", "{class: {$import: c.yml}, b: [1, 2]}"; "a quote", "{class: {$import: c.yml}, a: [1, \"2\"]}"; "the directive", "{class: {$import: d.yml}, a: [1, 2]}"; "a field more", "{class: {$import: c.yml}, a: [1, 2], b: 3}" ] do
                    Expect.notEqual (make (raw "{class: {$import: c.yml}, a: [1, 2]}")) (make (raw text)) (sprintf "%s: %s should change the value." name change)
            check "RawProcess" (fun node -> box (RawProcess node))
            check "RunRaw" (fun node -> box (RunRaw node))
            check "RawType" (fun node -> box (CWLType.RawType node))
            check "RawItem" (fun node -> box (RawItem node))
            check "MixedItem" (fun node -> box (MixedItem node))
            check "RawEntry" (fun node -> box (RawEntry node))
            check "RawRequirement" (fun node -> box (RawRequirement node))
            check "RawHint" (fun node -> box (RawHint node))
            Kit.expectSame "two imported items" (ImportedItem "a.yml") (ImportedItem "a.yml")
            Expect.notEqual (ImportedItem "a.yml") (ImportedItem "b.yml") "Another path should make another item."
            Expect.notEqual (ImportedItem "a.yml") (MixedItem (raw "a.yml")) "The cases of an item differ whatever the text."
    ]

let private writerTests =
    let tool (lines: string list) = Kit.tool ("baseCommand: echo" :: lines)
    let toolOf (lines: string list) = Decode.decodeCommandLineTool (tool lines)
    let requirementOf (lines: string list) = (Decode.decodeCommandLineTool (Kit.toolWith ("requirements:" :: lines))).Requirements.Value.[0]
    testList "The directives in the writer and beside the table" [
        testCase "a doc list with a directive item keeps its parts and a doc list of plain strings is one text" <| fun _ ->
            let withDoc (doc: string list) = Kit.toolWith ([ "doc:" ] @ (doc |> List.map (fun line -> "  " + line)))
            let parts = Decode.decodeCommandLineTool (withDoc [ "- first"; "- {$include: notes.md}"; "- last" ])
            Expect.equal parts.Doc (Some (DocText.Parts (ResizeArray [ SchemaSaladString.Literal "first"; SchemaSaladString.Include "notes.md"; SchemaSaladString.Literal "last" ]))) "The items stay apart, in order."
            let written = Kit.toolRoundTrip "parts" (withDoc [ "- first"; "- {$include: notes.md}"; "- last" ])
            Expect.stringContains written "doc:\n  - first\n  - {$include: notes.md}\n  - last\n" "The doc is written as a list."
            let single = Decode.decodeCommandLineTool (withDoc [ "- {$import: other.yml}" ])
            Expect.equal single.Doc (Some (DocText.Parts (ResizeArray [ SchemaSaladString.Import "other.yml" ]))) "A list of one directive stays a list."
            Expect.notEqual single.Doc (Some (DocText.Text (SchemaSaladString.Import "other.yml"))) "A list of one directive differs from the directive."
            Expect.equal (Decode.decodeCommandLineTool (withDoc [ "- \"first \""; "- last" ])).Doc (Some (DocText.literal "first last")) "The items of a list of plain strings join into one text."
            let inPort = toolOf [ "inputs:"; "  x:"; "    type: string"; "    doc: [a, {$include: notes.md}]"; "outputs: {}" ]
            Expect.equal (ListEntry.values inPort.Inputs.Value).[0].Doc (Some (DocText.Parts (ResizeArray [ SchemaSaladString.Literal "a"; SchemaSaladString.Include "notes.md" ]))) "The doc of a port keeps its parts too."
        testCase "a directive replaces the layout of a doc or a label that is edited into text, and text replaces a directive" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool (Kit.toolWith [ "doc: {$include: notes.md}"; "label: {$import: l.yml}" ])
            decoded.Doc <- Some (DocText.literal "now text")
            decoded.Label <- Some (SchemaSaladString.Literal "now a label")
            let again = Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded)
            Expect.equal again.Doc (Some (DocText.literal "now text")) "The edited doc is text."
            Expect.equal again.Label (Some (SchemaSaladString.Literal "now a label")) "The edited label is text."
            let text = Decode.decodeCommandLineTool (Kit.toolWith [ "doc: some text"; "label: a label" ])
            text.Doc <- Some (DocText.Text (SchemaSaladString.Include "notes.md"))
            text.Label <- Some (SchemaSaladString.Import "l.yml")
            let written = Encode.encodeToolDescription text
            Expect.stringContains written "doc: {$include: notes.md}" "The doc is the include."
            Expect.stringContains written "label: {$import: l.yml}" "The label is the import."
            Expect.equal (Decode.decodeCommandLineTool written).Doc text.Doc "The include reads back."
        testCase "the key of a map entry replaces the field of the same name that the mixed value holds" <| fun _ ->
            let written = Kit.toolRoundTrip "replaced" (tool [ "inputs:"; "  x: {id: other, $mixin: mix.yml}"; "outputs: []" ])
            Expect.stringContains written "id: x" "The key is the id."
            Expect.isFalse (written.Contains "other") "The field of the value gives way to the key."
        testCase "the flow text of a type writes the imports of a record and an enum in their places" <| fun _ ->
            let recordType = Record (InputRecordSchema(fields = ResizeArray [ ListEntry.Imported "f.yml"; ListEntry.Value (InputRecordField("ok", CWLType.File)) ]))
            let enumType = Enum (InputEnumSchema(ResizeArray [ SchemaSaladString.Literal "a"; SchemaSaladString.Import "s.yml"; SchemaSaladString.Include "t.txt" ]))
            let recordText = Encode.cwlTypeToYamlString recordType
            let enumText = Encode.cwlTypeToYamlString enumType
            Expect.equal recordText "{type: record, fields: [{$import: f.yml}, {name: ok, type: File}]}" "The import stands in the list."
            Expect.equal enumText "{type: enum, symbols: [a, {$import: s.yml}, {$include: t.txt}]}" "The directives stand in the list."
            Expect.equal (recordText |> Reader.readRoot |> Decode.cwlTypeDecoder') recordType "The flow text should decode to the same record."
            Expect.equal (enumText |> Reader.readRoot |> Decode.cwlTypeDecoder') enumType "The flow text should decode to the same enum."
        testCase "a record with a repeated field name is written as a list and reads back" <| fun _ ->
            let written = Kit.toolRoundTrip "repeated" (tool [ "inputs:"; "  a:"; "    type:"; "      type: record"; "      fields:"; "        - {name: f, type: string}"; "        - {name: f, type: int}"; "outputs: {}" ])
            Expect.equal (written.Split([| "name: f" |], System.StringSplitOptions.None).Length - 1) 2 "Both fields are written."
        testCase "a path that YAML needs quoted is written quoted and reads back" <| fun _ ->
            for path in [ "a b.txt"; "x, y.txt"; "k: v.txt"; "q\"uote.txt"; "#hash.txt"; "[list].txt"; "{map}.txt"; "42" ] do
                for directive in [ SchemaSaladString.Include path; SchemaSaladString.Import path ] do
                    let requirement = DockerRequirement (DockerRequirement.create(dockerFileReference = directive))
                    let written = Encode.encodeRequirement requirement |> Encode.writeYaml
                    Expect.stringContains written "dockerFile: {$" (sprintf "%s: the directive is a flow mapping" path)
                    let again = requirementOf [ "  - " + written.Replace("\n", "\n    ").TrimEnd() ]
                    Expect.equal again requirement (sprintf "%s: the written directive reads back as the same requirement" path)
        testCase "the requirements that are one import are written as a list when a requirement joins the import" <| fun _ ->
            let whole = Decode.decodeCommandLineTool (Kit.toolWith [ "requirements: {$import: reqs.yml}" ])
            whole.Requirements.Value.Add(ShellCommandRequirement)
            let extended = Encode.encodeToolDescription whole
            Expect.stringContains extended "requirements:\n  - {$import: reqs.yml}\n  - class: ShellCommandRequirement" "A list with more than the import is written as a list."
            Expect.equal (Decode.decodeCommandLineTool extended).Requirements.Value.Count 2 "The longer list reads back."
        testCase "a type of an imported file of requirements is named by the path of the file" <| fun _ ->
            let typeOf (reference: string) =
                (ListEntry.values (toolOf [ "requirements:"; "  - {$import: sd.yml}"; "inputs:"; "  a: " + reference; "outputs: {}" ]).Inputs.Value).[0].Type_
            Expect.equal (typeOf "sd.yml#Color") (Some (Reference "sd.yml#Color")) "The path of the import and the name are the reference."
            Expect.equal (typeOf "sd.yml#Color?") (Some (Union (ResizeArray [ Null; Reference "sd.yml#Color" ]))) "The optional spelling reads too."
            Expect.equal (typeOf "\"file:///work/sd.yml#Color\"") (Some (Reference "file:///work/sd.yml#Color")) "An address with a scheme is a reference."
        testCase "a directive in a list of strings of an extension field is a decode error for a mapping" <| fun _ ->
            Kit.decodeFails "A mapping in a list of strings" (fun () -> requirementOf [ "  - class: DockerRequirement"; "    cwltool:dockerRunOptions:"; "      - $include: options.txt" ]) "requirements[0].cwltool:dockerRunOptions[0]" "Expected a string"
        testCase "the File and Directory values of a job file take the same directives" <| fun _ ->
            let references = DecodeParameters.decodeYAMLParameterFile "f:\n  class: File\n  path: {$include: a.txt}\n  format: {$import: f.yml}\n  checksum: {$include: c.txt}\nd:\n  class: Directory\n  location: {$import: b.yml}\n  basename: {$import: n.yml}\n"
            match references.[0].Value with
            | Some (CWLParameterValue.File file) ->
                Expect.equal file.Path (Some (SchemaSaladString.Include "a.txt")) "The path is the include."
                Expect.equal file.Format (Some (SchemaSaladString.Import "f.yml")) "The format is the import."
                Expect.equal file.Checksum (Some (SchemaSaladString.Include "c.txt")) "The checksum is the include."
            | other -> failwith (sprintf "Expected a File, found %A" other)
            match references.[1].Value with
            | Some (CWLParameterValue.Directory directory) ->
                Expect.equal directory.Location (Some (SchemaSaladString.Import "b.yml")) "The location is the import."
                Expect.equal directory.Basename (Some (SchemaSaladString.Import "n.yml")) "The basename is the import."
            | other -> failwith (sprintf "Expected a Directory, found %A" other)
            let written = Encode.encodeYAMLParameterFile references
            Expect.stringContains written "format: {$import: f.yml}" "The import is written as a flow mapping."
            Expect.sequenceEqual (DecodeParameters.decodeYAMLParameterFile written) references "The written file should decode to the same references."
        testCase "a directive binding keeps the rules of the process and the type: stdin and stdout take none, and a workflow and an operation have no binding there" <| fun _ ->
            Kit.toolFails "stdin" (tool [ "inputs:"; "  x:"; "    type: stdin"; "    inputBinding: {$import: b.yml}"; "outputs: {}" ]) "inputs.x" "The type stdin is valid only for an input without an inputBinding."
            Kit.toolFails "stdout" (tool [ "inputs: {}"; "outputs:"; "  o:"; "    type: stdout"; "    outputBinding: {$import: b.yml}" ]) "outputs.o" "The type stdout is valid only for an output without an outputBinding."
            Kit.workflowFails "a workflow output" (Kit.workflow [ "inputs: {}"; "outputs:"; "  o:"; "    type: File"; "    outputSource: s/o"; "    outputBinding: {$import: b.yml}"; "steps: {}" ]) "outputs.o.outputBinding" "An output of a workflow has no outputBinding."
            Kit.decodeFails "an operation input" (fun () -> Decode.decodeOperation (Kit.operation [ "inputs:"; "  x:"; "    type: File"; "    inputBinding: {$mixin: b.yml}"; "outputs: {}" ])) "inputs.x.inputBinding" "An input of an operation has no inputBinding."
        testCase "a class or a type that is a plain name is still read as the typed object" <| fun _ ->
            let decoded = toolOf [ "requirements:"; "  - class: ShellCommandRequirement"; "hints:"; "  - class: ShellCommandRequirement"; "inputs: {x: {type: {type: enum, symbols: [a]}}}"; "outputs: []" ]
            Expect.equal decoded.Requirements.Value.[0] ShellCommandRequirement "The class of a requirement is the requirement."
            Expect.equal decoded.Hints.Value.[0] (KnownHint ShellCommandRequirement) "The class of a hint is the known hint."
            Expect.isTrue (match (ListEntry.values decoded.Inputs.Value).[0].Type_ with Some (Enum _) -> true | _ -> false) "The type of a schema is the enum."
        testCase "an array of the listing keeps the Files and Directories that it shows beside an object with a directive as its class" <| fun _ ->
            let listing = toolOf [ "requirements:"; "  - class: InitialWorkDirRequirement"; "    listing:"; "      - - {class: File, location: a.txt}"; "        - {class: {$include: c.txt}, location: b.txt}"; "        - {class: Directory, location: d}"; "inputs: []"; "outputs: []" ]
            Expect.equal listing.Requirements.Value.[0] (InitialWorkDirRequirement (ResizeArray [ FilesEntry (ResizeArray [ FileItem (FileInstance(location = SchemaSaladString.Literal "a.txt")); RawItem (raw "{class: {$include: c.txt}, location: b.txt}"); DirectoryItem (DirectoryInstance(location = SchemaSaladString.Literal "d")) ]) ])) "The array holds its three items."
        testCase "the step inputs with an import and a repeated id are written as a list and read back" <| fun _ ->
            Kit.workflowRoundTrip "repeated" (Kit.step [ "run: t.cwl"; "in:"; "  - {id: a, source: x}"; "  - {$import: more.yml}"; "  - {id: a, source: y}"; "out: []" ]) |> ignore
        testCase "the out field is a list, so a map of outputs is still an error, and an inline process keeps the directives that stand for outputs" <| fun _ ->
            Kit.workflowFails "a map of outputs" (Kit.step [ "run: t.cwl"; "in: {}"; "out: {r: {$import: p.yml}}" ]) "steps.s.out" "Expected a list of outputs, found a mapping."
            let inlineStep = Kit.step [ "run:"; "  class: CommandLineTool"; "  baseCommand: echo"; "  inputs: {}"; "  outputs:"; "    r: stdout"; "in: {}"; "out: [r, {$import: more.yml}, {$include: id.txt}]" ]
            Expect.equal (ListEntry.values (Decode.decodeWorkflow inlineStep).Steps).[0].Out.Count 3 "The directives stand for outputs of files."
        testCase "the step input in map form takes a mapping as the step input and a string or a list as the source" <| fun _ ->
            let inputs = WorkflowStep.getInputValues (ListEntry.values (Decode.decodeWorkflow (Kit.step [ "run: t.cwl"; "out: []"; "in:"; "  a: x"; "  b: [x, y]"; "  c: {source: x, default: 1}" ])).Steps).[0]
            Expect.equal (inputs |> Seq.map (fun input -> input.Source) |> List.ofSeq) [ Some (sourceOf [ "x" ]); Some (sourceOf [ "x"; "y" ]); Some (sourceOf [ "x" ]) ] "The shorthand and the object form both give the source."
        testCase "an import among the exit codes stays in its place and the codes are written as a block list" <| fun _ ->
            let entries = ofKind Kind.Number |> List.filter (fun position -> position.Name.StartsWith "an entry of the")
            Expect.equal entries.Length 3 "The successCodes, the temporaryFailCodes and the permanentFailCodes each have an entry position."
            for position in entries do
                let field = position.Path.Substring(0, position.Path.IndexOf '[')
                let written = position.Encode (position.Decode (position.Document "{$import: more.yml}"))
                Expect.stringContains written (field + ":\n  - 0\n  - {$import: more.yml}\n  - 3\n") (sprintf "%s: the import should be written as an entry in its place.\n%s" field written)
        testCase "an import keeps its place among the entries of a list in the written text" <| fun _ ->
            let workflowMixed =
                Kit.workflow
                    [ "inputs: [{$import: inputs.yml}, {id: a, type: string}]"; "outputs: [{id: o, type: string, outputSource: s/out}, {$import: outputs.yml}]"
                      "steps: [{$import: steps.yml}, {id: s, run: t.cwl, in: {}, out: [out]}, {$import: more-steps.yml}]" ]
            for (name, written, fragments) in
                [ "the fields of a record", Kit.toolRoundTrip "fields" (tool [ "inputs:"; "  a:"; "    type: {type: record, fields: [{$import: more-fields.yml}, {name: f, type: string}, {$import: last.yml}]}"; "outputs: {}" ]), [ "more-fields.yml"; "name: f"; "last.yml" ]
                  "the inputs of a workflow", Kit.workflowRoundTrip "inputs" workflowMixed, [ "inputs.yml"; "id: a" ]
                  "the outputs of a workflow", Kit.workflowRoundTrip "outputs" workflowMixed, [ "id: o"; "outputs.yml" ]
                  "the steps of a workflow", Kit.workflowRoundTrip "steps" workflowMixed, [ "steps.yml"; "id: s"; "more-steps.yml" ]
                  "the in field of a step", Kit.workflowRoundTrip "in" (Kit.step [ "run: t.cwl"; "in: [{$import: more-in.yml}, {id: a, source: x}]"; "out: []" ]), [ "more-in.yml"; "id: a" ]
                  "the envDef", Kit.toolRoundTrip "envDef" (Kit.toolWith [ "requirements:"; "  - class: EnvVarRequirement"; "    envDef: [{$import: more-env.yml}, {envName: A, envValue: b}, {$import: last-env.yml}]" ]), [ "more-env.yml"; "envName: A"; "last-env.yml" ] ] do
                let places = fragments |> List.map (fun fragment -> written.IndexOf fragment)
                Expect.isFalse (List.contains -1 places) (sprintf "%s: every entry should be written.\n%s" name written)
                Expect.equal places (List.sort places) (sprintf "%s: the entries should keep their order.\n%s" name written)
        testCase "the plain exit codes read as numbers and a quoted number is an error" <| fun _ ->
            Expect.equal (List.ofSeq (ListEntry.values (Decode.decodeCommandLineTool (Kit.toolWith [ "successCodes: [0, 1]" ])).SuccessCodes.Value)) [ 0; 1 ] "The numbers are the values."
            Kit.toolFails "a quoted number" (Kit.toolWith [ "successCodes: [\"0\"]" ]) "successCodes[0]" "Expected an int"
        testCase "the expression of an expression tool keeps its literal layout" <| fun _ ->
            let decoded = Decode.decodeExpressionTool (Kit.expressionTool [ "inputs: {}"; "outputs: {}"; "expression: $(1 + 1)" ])
            Expect.equal decoded.Expression (SchemaSaladString.Literal "$(1 + 1)") "The expression is the literal."
            Expect.stringContains (Encode.encodeExpressionToolDescription decoded) "expression: \"$(1 + 1)\"" "A literal expression is double-quoted."
    ]

let main =
    testList "Directives" [
        tableTests
        modelTests
        writerTests
    ]
