/// The field positions of CWL documents for the table tests. A position has a name, the path of its errors, the kind of
/// value that it takes, the document with a given value text in its place, and the reader of the decoded value.
module Tests.Positions

open ARCtrl.CWL
open TestingUtils.CWL
open YAMLicious

/// The kind of value that a position takes. A table row names the kinds that it runs at.
[<RequireQualifiedAccess>]
type Kind =
    /// A string, which a directive can stand for.
    | Text
    /// A field that CWL types as an expression, alone or beside a number or a boolean.
    | Expression
    | Bool
    /// The position of an input binding, an int or an expression.
    | Int
    /// The size of a File and an entry of the exit codes.
    | Number
    | Enum
    | Type
    /// An object of its own: the run of a step or an item of an array in the listing.
    | Object
    /// An input or an output binding, whose fields beside a $mixin the decoder checks.
    | Binding
    /// An entry of a list of objects.
    | Entry
    /// An entry of the out field of a step, a string or an object.
    | OutEntry
    /// The value of an entry of an id map, whose key Schema Salad adds as a field of the object.
    | MapValue
    /// A list of objects as a whole.
    | WholeList
    /// A list of strings as a whole.
    | WholeStrings
    /// A list of numbers as a whole.
    | WholeNumbers
    /// The class or the type field of an object.
    | Class
    /// The type field of an entry of the SchemaDefRequirement, a class position whose entry fails as a whole when the type
    /// is a mapping without a directive.
    | DefinitionType

/// What a position holds after the decoder read the document, or the decode error that a table expects there.
type Outcome =
    /// An $import of the file at the path.
    | Import of string
    /// An $include of the file at the path.
    | Include of string
    /// A $mixin on its own, as a directive of a string.
    | Mixin of string
    /// A mapping with a $mixin that stays as it was written.
    | Mixed of RawYaml
    /// A $mixin in the value of a map entry, with the key as the id field beside it, and the path of the mixin.
    | Keyed of string
    /// An object whose class or type is a directive, which stays as it was written, with its node.
    | Raw of RawYaml
    /// The expected outcome of a table that the object of the position, which holds the value, stays as it was written:
    /// Raw of the text that the Holder of the position gives for the value.
    | Kept
    /// A value without a directive.
    | Value of obj
    /// A decode error at the position whose message holds the keyword.
    | Fails of string
    /// A decode error at the field below the position, such as .$import, whose message holds the keyword.
    | FailsAt of string * string
    /// A decode error at the given path outside the position, whose message holds the keyword.
    | FailsOn of string * string

/// The mapping of the text, kept as it was written.
let mixed (text: string) : Outcome = Mixed { Node = Reader.readRoot text }

type Position =
    { Name: string
      /// The path of the field in the errors.
      Path: string
      Kind: Kind
      /// The text of the whole document with the value text at the position.
      Document: string -> string
      Decode: string -> obj
      Encode: obj -> string
      /// What the decoded document holds at the position.
      Read: obj -> Outcome
      /// The text of the object that holds the value, for a position whose object stays as written.
      Holder: (string -> string) option
      /// The plain values of the position, each with its literal outcome.
      Plain: (string * Outcome) list }

let private positionIn (decode: string -> 'D) (encode: 'D -> string) (kind: Kind) (name: string) (path: string) (document: string -> string) (read: 'D -> Outcome) : Position =
    { Name = name
      Path = path
      Kind = kind
      Document = document
      Decode = fun text -> box (decode text)
      Encode = fun decoded -> encode (unbox<'D> decoded)
      Read = fun decoded -> read (unbox<'D> decoded)
      Holder = None
      Plain = [] }

/// The position whose value stands in the object that the holder gives, and whose document holds that object.
let private holding (holder: string -> string) (position: Position) : Position =
    { position with Document = holder >> position.Document; Holder = Some holder }

/// The position with its plain values and their literal outcomes.
let private plain (values: (string * Outcome) list) (position: Position) : Position = { position with Plain = values }

let private tool kind name path document read = positionIn Decode.decodeCommandLineTool Encode.encodeToolDescription kind name path document read
let private workflow kind name path document read = positionIn Decode.decodeWorkflow Encode.encodeWorkflowDescription kind name path document read
let private expressionTool kind name path document read = positionIn Decode.decodeExpressionTool Encode.encodeExpressionToolDescription kind name path document read
let private operation kind name path document read = positionIn Decode.decodeOperation Encode.encodeOperationDescription kind name path document read

let private given (read: 'T -> Outcome) (value: 'T option) : Outcome =
    match value with
    | Some value -> read value
    | None -> failwith "The position holds no value."

let private ofString (value: SchemaSaladString) : Outcome =
    match value with
    | SchemaSaladString.Import path -> Import path
    | SchemaSaladString.Include path -> Include path
    | SchemaSaladString.Mixin path -> Mixin path
    | SchemaSaladString.Mixed raw -> Mixed raw
    | SchemaSaladString.Literal text -> Value (box text)

let private ofDoc (doc: DocText) : Outcome =
    match doc with
    | DocText.Text text -> ofString text
    | parts -> Value (box parts)

let private ofEntry (entry: ListEntry<'T>) : Outcome =
    match entry with
    | ListEntry.Imported path -> Import path
    | ListEntry.Mixed raw -> Mixed raw
    | ListEntry.Value value -> Value (box value)

let private ofBool (value: SchemaSaladBool) : Outcome =
    match value with
    | SchemaSaladBool.Imported path -> Import path
    | SchemaSaladBool.Mixed raw -> Mixed raw
    | SchemaSaladBool.Value flag -> Value (box flag)

let private ofSize (value: FileSize) : Outcome =
    match value with
    | FileSize.Imported path -> Import path
    | FileSize.Mixed raw -> Mixed raw
    | FileSize.Bytes count -> Value (box count)

let private ofPosition (value: BindingPosition) : Outcome =
    match value with
    | BindingPosition.Imported path -> Import path
    | BindingPosition.Included path -> Include path
    | BindingPosition.Mixed raw -> Mixed raw
    | other -> Value (box other)

let private ofLoadListing (value: LoadListingEnum) : Outcome =
    match value with
    | ImportedLoadListing path -> Import path
    | IncludedLoadListing path -> Include path
    | MixedLoadListing raw -> Mixed raw
    | other -> Value (box other)

let private ofLinkMerge (value: LinkMergeMethod) : Outcome =
    match value with
    | ImportedLinkMerge path -> Import path
    | IncludedLinkMerge path -> Include path
    | MixedLinkMerge raw -> Mixed raw
    | other -> Value (box other)

let private ofPickValue (value: PickValueMethod) : Outcome =
    match value with
    | ImportedPickValue path -> Import path
    | IncludedPickValue path -> Include path
    | MixedPickValue raw -> Mixed raw
    | other -> Value (box other)

let private ofScatterMethod (value: ScatterMethod) : Outcome =
    match value with
    | ImportedScatterMethod path -> Import path
    | IncludedScatterMethod path -> Include path
    | MixedScatterMethod raw -> Mixed raw
    | other -> Value (box other)

let private ofType (value: CWLType) : Outcome =
    match value with
    | ImportedType path -> Import path
    | IncludedType path -> Include path
    | MixedType raw -> Mixed raw
    | RawType raw -> Raw raw
    | other -> Value (box other)

let private ofRun (value: WorkflowStepRun) : Outcome =
    match value with
    | RunImport path -> Import path
    | RunMixed raw -> Mixed raw
    | RunRaw raw -> Raw raw
    | other -> Value (box other)

let private ofItem (value: FileOrDirectory) : Outcome =
    match value with
    | ImportedItem path -> Import path
    | MixedItem raw -> Mixed raw
    | RawItem raw -> Raw raw
    | other -> Value (box other)

let private ofListingEntry (value: InitialWorkDirEntry) : Outcome =
    match value with
    | StringEntry text -> ofString text
    | RawEntry raw -> Raw raw
    | other -> Value (box other)

let private ofRequirement (value: Requirement) : Outcome =
    match value with
    | ImportedRequirement path -> Import path
    | MixedRequirement raw -> Mixed raw
    | RawRequirement raw -> Raw raw
    | other -> Value (box other)

let private ofHint (value: HintEntry) : Outcome =
    match value with
    | ImportedHint path -> Import path
    | MixedHint raw -> Mixed raw
    | RawHint raw -> Raw raw
    | other -> Value (box other)

let private ofSchemaDef (value: SchemaDefEntry) : Outcome =
    match value with
    | SchemaDefEntry.Imported path -> Import path
    | SchemaDefEntry.Mixed raw -> Mixed raw
    | SchemaDefEntry.Raw raw -> Raw raw
    | other -> Value (box other)

let private ofStepOutput (value: StepOutput) : Outcome =
    match value with
    | StepOutputImport path -> Import path
    | StepOutputInclude path -> Include path
    | StepOutputMixed raw -> Mixed raw
    | other -> Value (box other)

let private ofProcess (value: CWLProcessingUnit) : Outcome =
    match value with
    | RawProcess raw -> Raw raw
    | other -> Value (box other)

/// The one entry of a list, as a directive that stands for the whole list reads.
let private ofOne (read: 'T -> Outcome) (items: ResizeArray<'T>) : Outcome =
    if items.Count = 1 then read items.[0] else Value (box items)

/// A mixed entry that holds the key of its map entry as the id field.
let private keyed (idField: string) (key: string) (outcome: Outcome) : Outcome =
    let scalarOf (node: YamlNode) = match node.Kind with YamlNodeKind.Scalar scalar -> Some scalar.Value | _ -> None
    match outcome with
    | Mixed raw ->
        match raw.Node.Kind with
        | YamlNodeKind.Mapping collection when collection.Items |> List.exists (fun entry -> scalarOf entry.Key = Some idField && scalarOf entry.Value = Some key) -> Keyed raw.MixinPath
        | _ -> outcome
    | other -> other

let private indent (spaces: int) (lines: string list) = lines |> List.map (fun line -> String.replicate spaces " " + line)
let private input (t: CWLToolDescription) = (ListEntry.values t.Inputs.Value).[0]
let private output (t: CWLToolDescription) = (ListEntry.values t.Outputs).[0]
let private schema (t: CWLToolDescription) = (input t).Type_.Value
let private record (t: CWLToolDescription) = match schema t with Record r -> r | other -> failwithf "Expected a record, found %A" other
let private requirement (t: CWLToolDescription) = t.Requirements.Value.[0]
let private listing (t: CWLToolDescription) = match requirement t with InitialWorkDirRequirement entries -> entries | other -> failwithf "Expected an InitialWorkDirRequirement, found %A" other
let private items (t: CWLToolDescription) = match (listing t).[0] with FilesEntry items -> items | other -> failwithf "Expected an array of the listing, found %A" other
let private file (t: CWLToolDescription) = match (listing t).[0] with FileEntry file -> file | other -> failwithf "Expected a File, found %A" other
let private directory (t: CWLToolDescription) = match (listing t).[0] with DirectoryEntry directory -> directory | other -> failwithf "Expected a Directory, found %A" other
let private docker (t: CWLToolDescription) = match requirement t with DockerRequirement docker -> docker | other -> failwithf "Expected a DockerRequirement, found %A" other
let private envs (t: CWLToolDescription) = match requirement t with EnvVarRequirement envs -> envs | other -> failwithf "Expected an EnvVarRequirement, found %A" other
let private packages (t: CWLToolDescription) = match t.Hints.Value.[0] with KnownHint (SoftwareRequirement packages) -> packages | other -> failwithf "Expected a SoftwareRequirement, found %A" other
let private package (t: CWLToolDescription) = (ListEntry.values (packages t)).[0]
let private step (w: CWLWorkflowDescription) = (ListEntry.values w.Steps).[0]
let private stepInput (w: CWLWorkflowDescription) = (WorkflowStep.getInputValues (step w)).[0]
let private sourceOf (source: OutputSource) = source.AsValues()

/// A tool with the input x of the type and the lines below it.
let private port (portType: string) (lines: string list) = Kit.tool ([ "baseCommand: echo"; "inputs:"; "  x:"; "    type: " + portType ] @ indent 4 lines @ [ "outputs: {}" ])
/// A tool with the output o of the type and the lines below it.
let private outPort (portType: string) (lines: string list) = Kit.tool ([ "baseCommand: echo"; "inputs: {}"; "outputs:"; "  o:"; "    type: " + portType ] @ indent 4 lines)
/// A tool with the input a whose type is the schema of the lines.
let private schemaPort (lines: string list) = Kit.tool ([ "baseCommand: echo"; "inputs:"; "  a:"; "    type:" ] @ indent 6 lines @ [ "outputs: {}" ])
/// A tool with the input a of the type text.
let private typed (typeText: string) = Kit.tool [ "baseCommand: echo"; "inputs:"; "  a:"; "    type: " + typeText; "outputs: {}" ]
let private requirements (lines: string list) = Kit.toolWith ("requirements:" :: lines)
let private listed (lines: string list) = requirements ([ "  - class: InitialWorkDirRequirement"; "    listing:" ] @ indent 6 lines)
let private software (lines: string list) = Kit.toolWith ([ "hints:"; "  - class: SoftwareRequirement" ] @ indent 4 lines)
/// A workflow with the step s, which runs t.cwl, and the lines of the step.
let private stepWith (lines: string list) = Kit.step ([ "run: t.cwl"; "in: {}"; "out: []" ] @ lines)
/// A workflow whose step s has the step input a with two sources and the lines below it.
let private stepInputWith (lines: string list) = Kit.workflowWithSteps ([ "  s:"; "    run: t.cwl"; "    in:"; "      a:"; "        source: [x, y]" ] @ indent 8 lines @ [ "    out: []" ])
let private scattered (lines: string list) = Kit.workflow ([ "requirements:"; "  ScatterFeatureRequirement: {}"; "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    run: t.cwl"; "    out: []" ] @ indent 4 lines)
let private workflowOutput (value: string) = Kit.workflow [ "inputs: {}"; "outputs:"; "  o:"; "    type: string"; "    outputSource: " + value; "steps: {}" ]

let private fileField (field: string) (read: FileInstance -> SchemaSaladString option) =
    let location = if field = "location" then [] else [ "  location: f.txt" ]
    tool Kind.Text ("the " + field + " of a File") ("requirements[0].listing[0]." + field) (fun v -> listed ([ "- class: File" ] @ location @ [ "  " + field + ": " + v ])) (fun t -> given ofString (read (file t)))
    |> plain [ "text", Value (box "text") ]

let private directoryField (field: string) (read: DirectoryInstance -> SchemaSaladString option) =
    let location = if field = "location" then [] else [ "  location: d" ]
    tool Kind.Text ("the " + field + " of a Directory") ("requirements[0].listing[0]." + field) (fun v -> listed ([ "- class: Directory" ] @ location @ [ "  " + field + ": " + v ])) (fun t -> given ofString (read (directory t)))
    |> plain [ "text", Value (box "text") ]

let private bindingField (kind: Kind) (field: string) (portType: string) (read: InputBinding -> Outcome) =
    tool kind ("the " + field + " of an inputBinding") ("inputs.x.inputBinding." + field) (fun v -> port portType [ "inputBinding: {" + field + ": " + v + "}" ]) (fun t -> read (inputBindingOf (input t)))

let private dockerField (field: string) (read: DockerRequirement -> SchemaSaladString option) =
    tool Kind.Text ("the " + field) ("requirements[0]." + field) (fun v -> requirements [ "  - {class: DockerRequirement, " + field + ": " + v + "}" ]) (fun t -> given ofString (read (docker t)))

let private codeEntry (field: string) (read: CWLToolDescription -> ResizeArray<ListEntry<int>> option) =
    tool Kind.Number ("an entry of the " + field) (field + "[1]") (fun v -> Kit.toolWith [ field + ": [0, " + v + ", 3]" ]) (fun t -> ofEntry (read t).Value.[1])

let private codes (field: string) (read: CWLToolDescription -> ResizeArray<ListEntry<int>> option) =
    tool Kind.WholeNumbers ("the " + field + " as a whole") field (fun v -> Kit.toolWith [ field + ": " + v ]) (fun t -> given (ofOne ofEntry) (read t))

let private arrayItemField (cls: string) (field: string) (read: FileOrDirectory -> SchemaSaladString option) =
    tool Kind.Text ("the " + field + " of a " + cls + " in an array of the listing") ("requirements[0].listing[0][0]." + field) (fun v -> listed [ "- - class: " + cls; "    " + field + ": " + v ]) (fun t -> given ofString (read (items t).[0]))

let private intent (name: string) (document: string -> string) (decode: string -> 'D) (encode: 'D -> string) (read: 'D -> ResizeArray<SchemaSaladString> option) =
    positionIn decode encode Kind.WholeStrings ("the intent of " + name) "intent" (fun v -> document ("intent: " + v)) (fun d -> given (ofOne ofString) (read d))
    |> plain [ "http://example.org/i", Fails "found the text" ]

let positions : Position list =
    [ tool Kind.Text "the doc of a tool" "doc" (fun v -> Kit.toolWith [ "doc: " + v ]) (fun t -> given ofDoc t.Doc)
      tool Kind.Text "the label of a tool" "label" (fun v -> Kit.toolWith [ "label: " + v ]) (fun t -> given ofString t.Label)
      tool Kind.Text "the stdin of a tool" "stdin" (fun v -> Kit.toolWith [ "stdin: " + v ]) (fun t -> given ofString t.Stdin)
      tool Kind.Text "the stdout of a tool" "stdout" (fun v -> Kit.toolWith [ "stdout: " + v ]) (fun t -> given ofString t.Stdout)
      tool Kind.Text "the stderr of a tool" "stderr" (fun v -> Kit.toolWith [ "stderr: " + v ]) (fun t -> given ofString t.Stderr)
      tool Kind.Text "the baseCommand as a whole" "baseCommand" (fun v -> Kit.tool [ "baseCommand: " + v; "inputs: {}"; "outputs: {}" ]) (fun t -> given (ofOne ofString) t.BaseCommand)
      tool Kind.Text "an entry of the baseCommand" "baseCommand[1]" (fun v -> Kit.tool [ "baseCommand: [echo, " + v + "]"; "inputs: {}"; "outputs: {}" ]) (fun t -> ofString t.BaseCommand.Value.[1])
      workflow Kind.Text "the doc of a workflow" "doc" (fun v -> Kit.workflow [ "inputs: {}"; "outputs: {}"; "steps: {}"; "doc: " + v ]) (fun w -> given ofDoc w.Doc)
      workflow Kind.Text "the label of a workflow" "label" (fun v -> Kit.workflow [ "inputs: {}"; "outputs: {}"; "steps: {}"; "label: " + v ]) (fun w -> given ofString w.Label)
      expressionTool Kind.Text "the doc of an expression tool" "doc" (fun v -> Kit.expressionTool [ "inputs: {}"; "outputs: {}"; "expression: $(1)"; "doc: " + v ]) (fun e -> given ofDoc e.Doc)
      operation Kind.Text "the label of an operation" "label" (fun v -> Kit.operation [ "inputs: {}"; "outputs: {}"; "label: " + v ]) (fun o -> given ofString o.Label)
      tool Kind.Text "the doc of an input" "inputs.x.doc" (fun v -> port "File" [ "doc: " + v ]) (fun t -> given ofDoc (input t).Doc)
      tool Kind.Text "the label of an input" "inputs.x.label" (fun v -> port "File" [ "label: " + v ]) (fun t -> given ofString (input t).Label)
      tool Kind.Text "the format of an input" "inputs.x.format" (fun v -> port "File" [ "format: " + v ]) (fun t -> match (input t).Format with Some (InputFormat.Single format) -> ofString format | other -> Value (box other))
      tool Kind.Text "an entry of the format of an input" "inputs.x.format[0]" (fun v -> port "File" [ "format: [" + v + "]" ]) (fun t -> match (input t).Format with Some (InputFormat.Multiple formats) -> ofString formats.[0] | other -> Value (box other))
      tool Kind.Text "the doc of an output" "outputs.o.doc" (fun v -> outPort "File" [ "doc: " + v ]) (fun t -> given ofDoc (output t).Doc)
      tool Kind.Text "the label of an output" "outputs.o.label" (fun v -> outPort "File" [ "label: " + v ]) (fun t -> given ofString (output t).Label)
      tool Kind.Text "the format of an output" "outputs.o.format" (fun v -> outPort "File" [ "format: " + v ]) (fun t -> given ofString (output t).Format)
      workflow Kind.Text "the doc of a step" "steps.s.doc" (fun v -> stepWith [ "doc: " + v ]) (fun w -> given ofDoc (step w).Doc)
      workflow Kind.Text "the label of a step" "steps.s.label" (fun v -> stepWith [ "label: " + v ]) (fun w -> given ofString (step w).Label)
      workflow Kind.Text "the doc of a step input" "steps.s.in.a.doc" (fun v -> stepInputWith [ "doc: " + v ]) (fun w -> given ofDoc (stepInput w).Doc)
      workflow Kind.Text "the label of a step input" "steps.s.in.a.label" (fun v -> stepInputWith [ "label: " + v ]) (fun w -> given ofString (stepInput w).Label)
      workflow Kind.Text "the valueFrom of a step input" "steps.s.in.a.valueFrom" (fun v -> stepInputWith [ "valueFrom: " + v ]) (fun w -> given ofString (stepInput w).ValueFrom)
      tool Kind.Text "the doc of a record field" "inputs.a.type.fields.f.doc" (fun v -> schemaPort [ "type: record"; "fields:"; "  f:"; "    type: string"; "    doc: " + v ]) (fun t -> given ofDoc (InputRecordSchema.getFieldValuesOrEmpty (record t)).[0].Doc)
      tool Kind.Text "the label of a record" "inputs.a.type.label" (fun v -> schemaPort [ "type: record"; "fields: {}"; "label: " + v ]) (fun t -> given ofString (record t).Label)
      tool Kind.Text "the doc of an enum" "inputs.a.type.doc" (fun v -> schemaPort [ "type: enum"; "symbols: [a]"; "doc: " + v ]) (fun t -> match schema t with Enum e -> given ofDoc e.Doc | other -> Value (box other))
      tool Kind.Text "the label of an array" "inputs.a.type.label" (fun v -> schemaPort [ "type: array"; "items: string"; "label: " + v ]) (fun t -> match schema t with Array a -> given ofString a.Label | other -> Value (box other))
      tool Kind.Text "an entry of the symbols of an enum" "inputs.a.type.symbols[1]" (fun v -> schemaPort [ "type: enum"; "symbols: [a, " + v + ", b]" ]) (fun t -> match schema t with Enum e -> ofString e.Symbols.[1] | other -> Value (box other))
      bindingField Kind.Text "prefix" "string" (fun binding -> given ofString binding.Prefix)
      bindingField Kind.Text "itemSeparator" "string" (fun binding -> given ofString binding.ItemSeparator)
      bindingField Kind.Text "valueFrom" "string" (fun binding -> given ofString binding.ValueFrom)
      tool Kind.Text "the glob of an outputBinding" "outputs.o.outputBinding.glob" (fun v -> outPort "File" [ "outputBinding: {glob: " + v + "}" ]) (fun t -> match (outputBindingOf (output t)).Glob with Some (OutputGlob.Single glob) -> ofString glob | other -> Value (box other))
      tool Kind.Text "an entry of the glob of an outputBinding" "outputs.o.outputBinding.glob[1]" (fun v -> outPort "File" [ "outputBinding: {glob: [a, " + v + "]}" ]) (fun t -> match (outputBindingOf (output t)).Glob with Some (OutputGlob.Multiple globs) -> ofString globs.[1] | other -> Value (box other))
      dockerField "dockerPull" (fun docker -> docker.DockerPull)
      dockerField "dockerImageId" (fun docker -> docker.DockerImageId)
      dockerField "dockerLoad" (fun docker -> docker.DockerLoad)
      dockerField "dockerImport" (fun docker -> docker.DockerImport)
      dockerField "dockerOutputDirectory" (fun docker -> docker.DockerOutputDirectory)
      dockerField "dockerFile" (fun docker -> docker.DockerFile)
      tool Kind.Text "the listing as a whole" "requirements[0].listing" (fun v -> requirements [ "  - {class: InitialWorkDirRequirement, listing: " + v + "}" ]) (fun t -> match requirement t with InitialWorkDirExpressionRequirement text -> ofString text | other -> Value (box other))
      tool Kind.Text "an entry of the listing" "requirements[0].listing[0]" (fun v -> listed [ "- " + v ]) (fun t -> ofListingEntry (listing t).[0])
      tool Kind.Text "the envName" "requirements[0].envDef[0].envName" (fun v -> requirements [ "  - class: EnvVarRequirement"; "    envDef:"; "      - envName: " + v; "        envValue: w" ]) (fun t -> ofString (ListEntry.values (envs t)).[0].EnvName)
      tool Kind.Text "the envValue" "requirements[0].envDef[0].envValue" (fun v -> requirements [ "  - class: EnvVarRequirement"; "    envDef:"; "      - envName: A"; "        envValue: " + v ]) (fun t -> ofString (ListEntry.values (envs t)).[0].EnvValue)
      tool Kind.Text "the package" "hints[0].packages[0].package" (fun v -> software [ "packages:"; "  - package: " + v ]) (fun t -> ofString (package t).Package)
      tool Kind.Text "an entry of the version of a package" "hints[0].packages[0].version[1]" (fun v -> software [ "packages:"; "  - package: git"; "    version: [\"1.0\", " + v + "]" ]) (fun t -> ofString (package t).Version.Value.[1])
      tool Kind.Text "an entry of the specs of a package" "hints[0].packages[0].specs[1]" (fun v -> software [ "packages:"; "  - package: git"; "    specs: [a, " + v + "]" ]) (fun t -> ofString (package t).Specs.Value.[1])
      tool Kind.Text "an entry of the specs of a package in map form" "hints[0].packages.git[1]" (fun v -> software [ "packages:"; "  git: [a, " + v + "]" ]) (fun t -> ofString (package t).Specs.Value.[1])
      tool Kind.Text "an entry of the version of a package in map form" "hints[0].packages.curl.version[0]" (fun v -> software [ "packages:"; "  curl: {version: [" + v + "]}" ]) (fun t -> ofString (package t).Version.Value.[0])
      tool Kind.Text "an entry of the expressionLib" "requirements[0].expressionLib[1]" (fun v -> requirements [ "  - class: InlineJavascriptRequirement"; "    expressionLib: [a, " + v + "]" ]) (fun t -> match requirement t with InlineJavascriptRequirement lib -> ofString lib.ExpressionLib.Value.[1] | other -> Value (box other))
      fileField "location" (fun f -> f.Location)
      fileField "path" (fun f -> f.Path)
      fileField "basename" (fun f -> f.Basename)
      fileField "dirname" (fun f -> f.Dirname)
      fileField "nameroot" (fun f -> f.Nameroot)
      fileField "nameext" (fun f -> f.Nameext)
      fileField "checksum" (fun f -> f.Checksum)
      fileField "contents" (fun f -> f.Contents)
      fileField "format" (fun f -> f.Format)
      directoryField "location" (fun d -> d.Location)
      directoryField "path" (fun d -> d.Path)
      directoryField "basename" (fun d -> d.Basename)
      arrayItemField "File" "location" (function FileItem f -> f.Location | _ -> None)
      arrayItemField "File" "path" (function FileItem f -> f.Path | _ -> None)
      arrayItemField "Directory" "location" (function DirectoryItem d -> d.Location | _ -> None)
      arrayItemField "Directory" "path" (function DirectoryItem d -> d.Path | _ -> None)
      workflow Kind.Text "the scatter as a whole" "steps.s.scatter" (fun v -> scattered [ "in: {}"; "scatter: " + v ]) (fun w -> given (ofOne ofString) (step w).Scatter)
      workflow Kind.Text "an entry of the scatter" "steps.s.scatter[1]" (fun v -> scattered [ "in: {}"; "scatter: [a, " + v + "]" ]) (fun w -> ofString (step w).Scatter.Value.[1])
      workflow Kind.Text "the source of a step input" "steps.s.in.a.source" (fun v -> scattered [ "in:"; "  a:"; "    source: " + v ]) (fun w -> given (sourceOf >> ofOne ofString) (stepInput w).Source)
      workflow Kind.Text "an entry of the source of a step input" "steps.s.in.a.source[1]" (fun v -> scattered [ "in:"; "  a:"; "    source: [x, " + v + "]" ]) (fun w -> ofString ((stepInput w).Source.Value.AsValues()).[1])
      workflow Kind.Text "the source of a step input in a list" "steps.s.in.a.source" (fun v -> scattered [ "in:"; "  - id: a"; "    source: " + v ]) (fun w -> given (sourceOf >> ofOne ofString) (stepInput w).Source)
      workflow Kind.Text "an entry of the source shorthand of a step input" "steps.s.in.a[1]" (fun v -> scattered [ "in:"; "  a: [x, " + v + "]" ]) (fun w -> ofString ((stepInput w).Source.Value.AsValues()).[1])
      workflow Kind.Text "the outputSource" "outputs.o.outputSource" workflowOutput (fun w -> given (sourceOf >> ofOne ofString) (ListEntry.values w.Outputs).[0].OutputSource)
      workflow Kind.Text "an entry of the outputSource" "outputs.o.outputSource[1]" (fun v -> workflowOutput ("[a/x, " + v + "]")) (fun w -> ofString ((ListEntry.values w.Outputs).[0].OutputSource.Value.AsValues()).[1])
      tool Kind.Text "an entry of the intent" "intent[1]" (fun v -> Kit.toolWith [ "intent: [a, " + v + "]" ]) (fun t -> ofString t.Intent.Value.[1])

      expressionTool Kind.Expression "the expression of an expression tool" "expression" (fun v -> Kit.expressionTool [ "inputs: {}"; "outputs: {}"; "expression: " + v ]) (fun e -> ofString e.Expression)
      workflow Kind.Expression "the when of a step" "steps.s.when" (fun v -> stepWith [ "when: " + v ]) (fun w -> given ofString (step w).When_)
      tool Kind.Expression "the outputEval of an outputBinding" "outputs.o.outputBinding.outputEval" (fun v -> outPort "File" [ "outputBinding: {glob: a, outputEval: " + v + "}" ]) (fun t -> given ofString (outputBindingOf (output t)).OutputEval)
      tool Kind.Expression "the enableReuse of WorkReuse" "requirements[0].enableReuse" (fun v -> requirements [ "  - class: WorkReuse"; "    enableReuse: " + v ]) (fun t -> match requirement t with WorkReuseExpressionRequirement text -> ofString text | other -> Value (box other))
      tool Kind.Expression "the networkAccess of NetworkAccess" "requirements[0].networkAccess" (fun v -> requirements [ "  - class: NetworkAccess"; "    networkAccess: " + v ]) (fun t -> match requirement t with NetworkAccessExpressionRequirement text -> ofString text | other -> Value (box other))
      tool Kind.Expression "the timelimit of ToolTimeLimit" "requirements[0].timelimit" (fun v -> requirements [ "  - class: ToolTimeLimit"; "    timelimit: " + v ]) (fun t -> match requirement t with ToolTimeLimitRequirement (ToolTimeLimitExpression text) -> ofString text | other -> Value (box other))
      tool Kind.Expression "the ramMin of a ResourceRequirement" "requirements[0].ramMin" (fun v -> requirements [ "  - class: ResourceRequirement"; "    ramMin: " + v ]) (fun t -> match requirement t with ResourceRequirement resource -> given ofString (resource.TryGetExpression "ramMin") | other -> Value (box other))

      bindingField Kind.Bool "loadContents" "File" (fun binding -> given ofBool binding.LoadContents)
      bindingField Kind.Bool "separate" "string" (fun binding -> given ofBool binding.Separate)
      bindingField Kind.Bool "shellQuote" "string" (fun binding -> given ofBool binding.ShellQuote)
      tool Kind.Bool "the loadContents of an input" "inputs.x.loadContents" (fun v -> port "File" [ "loadContents: " + v ]) (fun t -> given ofBool (input t).LoadContents)
      tool Kind.Bool "the streamable of an input" "inputs.x.streamable" (fun v -> port "File" [ "streamable: " + v ]) (fun t -> given ofBool (input t).Streamable)
      tool Kind.Bool "the loadContents of an outputBinding" "outputs.o.outputBinding.loadContents" (fun v -> outPort "File" [ "outputBinding: {glob: o.txt, loadContents: " + v + "}" ]) (fun t -> given ofBool (outputBindingOf (output t)).LoadContents)
      tool Kind.Bool "the streamable of an output" "outputs.o.streamable" (fun v -> outPort "File" [ "streamable: " + v ]) (fun t -> given ofBool (output t).Streamable)
      tool Kind.Bool "the inplaceUpdate of the requirement" "requirements[0].inplaceUpdate" (fun v -> requirements [ "  - class: InplaceUpdateRequirement"; "    inplaceUpdate: " + v ]) (fun t -> match requirement t with InplaceUpdateRequirement value -> ofBool value.InplaceUpdate | other -> Value (box other))
      tool Kind.Bool "the writable of a Dirent" "requirements[0].listing[0].writable" (fun v -> listed [ "- entryname: a.txt"; "  entry: x"; "  writable: " + v ]) (fun t -> match (listing t).[0] with DirentEntry dirent -> given ofBool dirent.Writable | other -> Value (box other))
      workflow Kind.Bool "the streamable of a workflow input" "inputs.x.streamable" (fun v -> Kit.workflow [ "inputs:"; "  x:"; "    type: File"; "    streamable: " + v; "outputs: {}"; "steps: {}" ]) (fun w -> given ofBool (ListEntry.values w.Inputs).[0].Streamable)
      workflow Kind.Bool "the loadContents of a step input" "steps.s.in.a.loadContents" (fun v -> stepInputWith [ "loadContents: " + v ]) (fun w -> given ofBool (stepInput w).LoadContents)

      bindingField Kind.Int "position" "string" (fun binding -> given ofPosition binding.Position)
      tool Kind.Number "the size of a File in the listing" "requirements[0].listing[0].size" (fun v -> listed [ "- class: File"; "  location: f.txt"; "  size: " + v ]) (fun t -> given ofSize (file t).Size)
      |> plain [ "42", Value (box 42L); "\"42\"", Fails "Expected an int" ]
      codeEntry "successCodes" (fun t -> t.SuccessCodes)
      codeEntry "temporaryFailCodes" (fun t -> t.TemporaryFailCodes)
      codeEntry "permanentFailCodes" (fun t -> t.PermanentFailCodes)
      codes "successCodes" (fun t -> t.SuccessCodes)
      codes "temporaryFailCodes" (fun t -> t.TemporaryFailCodes)
      codes "permanentFailCodes" (fun t -> t.PermanentFailCodes)

      tool Kind.Enum "the loadListing of an input" "inputs.x.loadListing" (fun v -> port "Directory" [ "loadListing: " + v ]) (fun t -> given ofLoadListing (input t).LoadListing)
      |> plain [ "deep_listing", Value (box DeepListing); "foo", Fails "Expected one of" ]
      tool Kind.Enum "the loadListing of an outputBinding" "outputs.o.outputBinding.loadListing" (fun v -> outPort "Directory" [ "outputBinding: {glob: o, loadListing: " + v + "}" ]) (fun t -> given ofLoadListing (outputBindingOf (output t)).LoadListing)
      |> plain [ "deep_listing", Value (box DeepListing); "foo", Fails "Expected one of" ]
      tool Kind.Enum "the loadListing of the requirement" "requirements[0].loadListing" (fun v -> requirements [ "  - class: LoadListingRequirement"; "    loadListing: " + v ]) (fun t -> match requirement t with LoadListingRequirement value -> ofLoadListing value.LoadListing | other -> Value (box other))
      |> plain [ "deep_listing", Value (box DeepListing); "foo", Fails "Expected one of" ]
      workflow Kind.Enum "the loadListing of a step input" "steps.s.in.a.loadListing" (fun v -> stepInputWith [ "loadListing: " + v ]) (fun w -> given ofLoadListing (stepInput w).LoadListing)
      |> plain [ "deep_listing", Value (box DeepListing); "foo", Fails "Expected one of" ]
      workflow Kind.Enum "the linkMerge of a step input" "steps.s.in.a.linkMerge" (fun v -> stepInputWith [ "linkMerge: " + v ]) (fun w -> given ofLinkMerge (stepInput w).LinkMerge)
      |> plain [ "merge_flattened", Value (box MergeFlattened); "merge_all", Fails "Invalid linkMerge value" ]
      workflow Kind.Enum "the pickValue of a step input" "steps.s.in.a.pickValue" (fun v -> stepInputWith [ "pickValue: " + v ]) (fun w -> given ofPickValue (stepInput w).PickValue)
      |> plain [ "all_non_null", Value (box AllNonNull); "any", Fails "Invalid pickValue value" ]
      workflow Kind.Enum "the scatterMethod of a step" "steps.s.scatterMethod" (fun v -> scattered [ "in: {a: x}"; "scatter: a"; "scatterMethod: " + v ]) (fun w -> given ofScatterMethod (step w).ScatterMethod)
      |> plain [ "flat_crossproduct", Value (box FlatCrossProduct); "cross", Fails "Invalid scatterMethod value" ]

      tool Kind.Type "the type of a port" "inputs.a.type" typed (fun t -> ofType (schema t))
      tool Kind.Type "a member of a type list" "inputs.a.type[1]" (fun v -> typed ("[\"null\", " + v + "]")) (fun t -> match schema t with Union members -> ofType members.[1] | other -> Value (box other))
      tool Kind.Type "the items of an array" "inputs.a.type.items" (fun v -> schemaPort [ "type: array"; "items: " + v ]) (fun t -> match schema t with Array a -> ofType a.Items | other -> Value (box other))
      tool Kind.Type "a member of the items of an array" "inputs.a.type.items[1]" (fun v -> schemaPort [ "type: array"; "items: [string, " + v + "]" ]) (fun t -> match schema t with Array a -> (match a.Items with Union members -> ofType members.[1] | other -> Value (box other)) | other -> Value (box other))
      tool Kind.Type "the type of a record field" "inputs.a.type.fields.f.type" (fun v -> schemaPort [ "type: record"; "fields:"; "  f:"; "    type: " + v ]) (fun t -> ofType (InputRecordSchema.getFieldValuesOrEmpty (record t)).[0].Type)

      tool Kind.Binding "the inputBinding of a tool input" "inputs.x.inputBinding" (fun v -> port "File" [ "inputBinding: " + v ]) (fun t -> given ofEntry (input t).InputBinding)
      tool Kind.Binding "the inputBinding of a tool input in a list" "inputs.x.inputBinding" (fun v -> Kit.tool [ "baseCommand: echo"; "inputs:"; "  - id: x"; "    type: File"; "    inputBinding: " + v; "outputs: {}" ]) (fun t -> given ofEntry (input t).InputBinding)
      workflow Kind.Binding "the inputBinding of a workflow input" "inputs.x.inputBinding" (fun v -> Kit.workflow [ "inputs:"; "  x:"; "    type: File"; "    inputBinding: " + v; "outputs: {}"; "steps: {}" ]) (fun w -> given ofEntry (ListEntry.values w.Inputs).[0].InputBinding)
      expressionTool Kind.Binding "the inputBinding of an expression tool input" "inputs.x.inputBinding" (fun v -> Kit.expressionTool [ "expression: $(1)"; "inputs:"; "  x:"; "    type: File"; "    inputBinding: " + v; "outputs: {}" ]) (fun e -> given ofEntry (ListEntry.values e.Inputs.Value).[0].InputBinding)
      tool Kind.Binding "the outputBinding of a tool output" "outputs.o.outputBinding" (fun v -> outPort "File" [ "outputBinding: " + v ]) (fun t -> given ofEntry (output t).OutputBinding)
      tool Kind.Binding "the outputBinding of a tool output in a list" "outputs.o.outputBinding" (fun v -> Kit.tool [ "baseCommand: echo"; "inputs: {}"; "outputs:"; "  - id: o"; "    type: File"; "    outputBinding: " + v ]) (fun t -> given ofEntry (output t).OutputBinding)

      workflow Kind.Object "the run of a step" "steps.s.run" (fun v -> Kit.step [ "run: " + v; "in: {}"; "out: []" ]) (fun w -> ofRun (step w).Run)
      tool Kind.Object "an item of an array in the listing" "requirements[0].listing[0][1]" (fun v -> listed [ "- - {class: File, location: a.txt}"; "  - " + v ]) (fun t -> ofItem (items t).[1])

      tool Kind.Entry "an entry of the inputs of a tool" "inputs[1]" (fun v -> Kit.tool [ "baseCommand: echo"; "inputs: [{id: a, type: string}, " + v + "]"; "outputs: []" ]) (fun t -> ofEntry t.Inputs.Value.[1])
      tool Kind.Entry "an entry of the outputs of a tool" "outputs[0]" (fun v -> Kit.tool [ "baseCommand: echo"; "inputs: []"; "outputs: [" + v + ", {id: o, type: File}]" ]) (fun t -> ofEntry t.Outputs.[0])
      workflow Kind.Entry "an entry of the inputs of a workflow" "inputs[0]" (fun v -> Kit.workflow [ "inputs: [" + v + ", {id: a, type: string}]"; "outputs: []"; "steps: []" ]) (fun w -> ofEntry w.Inputs.[0])
      workflow Kind.Entry "an entry of the outputs of a workflow" "outputs[1]" (fun v -> Kit.workflow [ "inputs: []"; "outputs: [{id: o, type: string, outputSource: s/out}, " + v + "]"; "steps: []" ]) (fun w -> ofEntry w.Outputs.[1])
      workflow Kind.Entry "an entry of the steps" "steps[1]" (fun v -> Kit.workflow [ "inputs: []"; "outputs: []"; "steps: [{id: s, run: t.cwl, in: {}, out: [out]}, " + v + "]" ]) (fun w -> ofEntry w.Steps.[1])
      expressionTool Kind.Entry "an entry of the inputs of an expression tool" "inputs[0]" (fun v -> Kit.expressionTool [ "expression: $(null)"; "inputs: [" + v + "]"; "outputs: []" ]) (fun e -> ofEntry e.Inputs.Value.[0])
      expressionTool Kind.Entry "an entry of the outputs of an expression tool" "outputs[0]" (fun v -> Kit.expressionTool [ "expression: $(null)"; "inputs: []"; "outputs: [" + v + "]" ]) (fun e -> ofEntry e.Outputs.[0])
      operation Kind.Entry "an entry of the inputs of an operation" "inputs[0]" (fun v -> Kit.operation [ "inputs: [" + v + "]"; "outputs: []" ]) (fun o -> ofEntry o.Inputs.[0])
      operation Kind.Entry "an entry of the outputs of an operation" "outputs[0]" (fun v -> Kit.operation [ "inputs: []"; "outputs: [" + v + "]" ]) (fun o -> ofEntry o.Outputs.[0])
      workflow Kind.Entry "an entry of the in field of a step" "steps.s.in[0]" (fun v -> Kit.step [ "run: t.cwl"; "in: [" + v + ", {id: a, source: x}]"; "out: []" ]) (fun w -> ofEntry (step w).In.[0])
      tool Kind.Entry "an entry of the requirements" "requirements[0]" (fun v -> Kit.toolWith [ "requirements: [" + v + ", {class: InlineJavascriptRequirement}]" ]) (fun t -> ofRequirement t.Requirements.Value.[0])
      tool Kind.Entry "an entry of the hints" "hints[0]" (fun v -> Kit.toolWith [ "hints: [" + v + ", {class: acme:Thing}]" ]) (fun t -> ofHint t.Hints.Value.[0])
      tool Kind.Entry "an entry of the envDef" "requirements[0].envDef[0]" (fun v -> requirements [ "  - class: EnvVarRequirement"; "    envDef: [" + v + ", {envName: A, envValue: b}]" ]) (fun t -> ofEntry (envs t).[0])
      tool Kind.Entry "an entry of the packages" "hints[0].packages[0]" (fun v -> software [ "packages: [" + v + ", {package: git}]" ]) (fun t -> ofEntry (packages t).[0])
      tool Kind.Entry "an entry of the fields of a record" "inputs.a.type.fields[1]" (fun v -> schemaPort [ "type: record"; "fields: [{name: f, type: string}, " + v + "]" ]) (fun t -> ofEntry (record t).Fields.Value.[1])
      tool Kind.Entry "an entry of the types of a SchemaDefRequirement" "requirements[0].types[0]" (fun v -> requirements [ "  - class: SchemaDefRequirement"; "    types: [" + v + "]" ]) (fun t -> match requirement t with SchemaDefRequirement entries -> ofSchemaDef entries.[0] | other -> Value (box other))
      workflow Kind.OutEntry "an entry of the out field of a step" "steps.s.out[1]" (fun v -> Kit.step [ "run: t.cwl"; "in: {}"; "out: [r, " + v + "]" ]) (fun w -> ofStepOutput (step w).Out.[1])

      tool Kind.MapValue "an input of a tool in map form" "inputs.x" (fun v -> Kit.tool [ "baseCommand: echo"; "inputs:"; "  x: " + v; "outputs: {}" ]) (fun t -> keyed "id" "x" (ofEntry t.Inputs.Value.[0]))
      tool Kind.MapValue "an output of a tool in map form" "outputs.o" (fun v -> Kit.tool [ "baseCommand: echo"; "inputs: {}"; "outputs:"; "  o: " + v ]) (fun t -> keyed "id" "o" (ofEntry t.Outputs.[0]))
      workflow Kind.MapValue "an input of a workflow in map form" "inputs.a" (fun v -> Kit.workflow [ "inputs:"; "  a: " + v; "outputs: {}"; "steps: {}" ]) (fun w -> keyed "id" "a" (ofEntry w.Inputs.[0]))
      workflow Kind.MapValue "an output of a workflow in map form" "outputs.o" (fun v -> Kit.workflow [ "inputs: {}"; "outputs:"; "  o: " + v; "steps: {}" ]) (fun w -> keyed "id" "o" (ofEntry w.Outputs.[0]))
      workflow Kind.MapValue "a step in map form" "steps.s" (fun v -> Kit.workflowWithSteps [ "  s: " + v ]) (fun w -> keyed "id" "s" (ofEntry w.Steps.[0]))
      workflow Kind.MapValue "a step input in map form" "steps.s.in.a" (fun v -> Kit.step [ "run: t.cwl"; "in:"; "  a: " + v; "out: []" ]) (fun w -> keyed "id" "a" (ofEntry (step w).In.[0]))
      tool Kind.MapValue "a field of a record in map form" "inputs.a.type.fields.f" (fun v -> schemaPort [ "type: record"; "fields:"; "  f: " + v ]) (fun t -> keyed "name" "f" (ofEntry (record t).Fields.Value.[0]))
      tool Kind.MapValue "an envDef in map form" "requirements[0].envDef.A" (fun v -> requirements [ "  - class: EnvVarRequirement"; "    envDef:"; "      A: " + v ]) (fun t -> keyed "envName" "A" (ofEntry (envs t).[0]))
      tool Kind.MapValue "a package in map form" "hints[0].packages.git" (fun v -> software [ "packages:"; "  git: " + v ]) (fun t -> keyed "package" "git" (ofEntry (packages t).[0]))
      tool Kind.MapValue "a requirement in map form" "requirements.NetworkAccess" (fun v -> Kit.toolWith [ "requirements:"; "  NetworkAccess: " + v ]) (fun t -> keyed "class" "NetworkAccess" (ofRequirement t.Requirements.Value.[0]))
      tool Kind.MapValue "a hint in map form" "hints.NetworkAccess" (fun v -> Kit.toolWith [ "hints:"; "  NetworkAccess: " + v ]) (fun t -> keyed "class" "NetworkAccess" (ofHint t.Hints.Value.[0]))

      tool Kind.WholeList "the inputs of a tool" "inputs" (fun v -> Kit.tool [ "baseCommand: echo"; "inputs: " + v; "outputs: []" ]) (fun t -> given (ofOne ofEntry) t.Inputs)
      tool Kind.WholeList "the outputs of a tool" "outputs" (fun v -> Kit.tool [ "baseCommand: echo"; "inputs: []"; "outputs: " + v ]) (fun t -> ofOne ofEntry t.Outputs)
      workflow Kind.WholeList "the inputs of a workflow" "inputs" (fun v -> Kit.workflow [ "inputs: " + v; "outputs: []"; "steps: []" ]) (fun w -> ofOne ofEntry w.Inputs)
      workflow Kind.WholeList "the outputs of a workflow" "outputs" (fun v -> Kit.workflow [ "inputs: []"; "outputs: " + v; "steps: []" ]) (fun w -> ofOne ofEntry w.Outputs)
      workflow Kind.WholeList "the steps of a workflow" "steps" (fun v -> Kit.workflow [ "inputs: []"; "outputs: []"; "steps: " + v ]) (fun w -> ofOne ofEntry w.Steps)
      expressionTool Kind.WholeList "the inputs of an expression tool" "inputs" (fun v -> Kit.expressionTool [ "expression: $(null)"; "inputs: " + v; "outputs: []" ]) (fun e -> given (ofOne ofEntry) e.Inputs)
      expressionTool Kind.WholeList "the outputs of an expression tool" "outputs" (fun v -> Kit.expressionTool [ "expression: $(null)"; "inputs: []"; "outputs: " + v ]) (fun e -> ofOne ofEntry e.Outputs)
      operation Kind.WholeList "the inputs of an operation" "inputs" (fun v -> Kit.operation [ "inputs: " + v; "outputs: []" ]) (fun o -> ofOne ofEntry o.Inputs)
      operation Kind.WholeList "the outputs of an operation" "outputs" (fun v -> Kit.operation [ "inputs: []"; "outputs: " + v ]) (fun o -> ofOne ofEntry o.Outputs)
      workflow Kind.WholeList "the in field of a step" "steps.s.in" (fun v -> Kit.step [ "run: t.cwl"; "in: " + v; "out: []" ]) (fun w -> ofOne ofEntry (step w).In)
      workflow Kind.WholeList "the out field of a step" "steps.s.out" (fun v -> Kit.step [ "run: t.cwl"; "in: {}"; "out: " + v ]) (fun w -> ofOne ofStepOutput (step w).Out)
      tool Kind.WholeList "the requirements of a tool" "requirements" (fun v -> Kit.toolWith [ "requirements: " + v ]) (fun t -> given (ofOne ofRequirement) t.Requirements)
      tool Kind.WholeList "the hints of a tool" "hints" (fun v -> Kit.toolWith [ "hints: " + v ]) (fun t -> given (ofOne ofHint) t.Hints)
      workflow Kind.WholeList "the requirements of a step" "steps.s.requirements" (fun v -> stepWith [ "requirements: " + v ]) (fun w -> given (ofOne ofRequirement) (step w).Requirements)
      workflow Kind.WholeList "the hints of a step" "steps.s.hints" (fun v -> stepWith [ "hints: " + v ]) (fun w -> given (ofOne ofHint) (step w).Hints)
      tool Kind.WholeList "the envDef" "requirements[0].envDef" (fun v -> requirements [ "  - class: EnvVarRequirement"; "    envDef: " + v ]) (fun t -> ofOne ofEntry (envs t))
      tool Kind.WholeList "the packages" "hints[0].packages" (fun v -> software [ "packages: " + v ]) (fun t -> ofOne ofEntry (packages t))
      tool Kind.WholeList "the fields of a record" "inputs.a.type.fields" (fun v -> schemaPort [ "type: record"; "fields: " + v ]) (fun t -> given (ofOne ofEntry) (record t).Fields)
      tool Kind.WholeList "the types of a SchemaDefRequirement" "requirements[0].types" (fun v -> requirements [ "  - class: SchemaDefRequirement"; "    types: " + v ]) (fun t -> match requirement t with SchemaDefRequirement entries -> ofOne ofSchemaDef entries | other -> Value (box other))

      intent "a tool" (fun line -> Kit.toolWith [ line ]) Decode.decodeCommandLineTool Encode.encodeToolDescription (fun t -> t.Intent)
      intent "a workflow" (fun line -> Kit.workflow [ "inputs: {}"; "outputs: {}"; "steps: {}"; line ]) Decode.decodeWorkflow Encode.encodeWorkflowDescription (fun w -> w.Intent)
      intent "an expression tool" (fun line -> Kit.expressionTool [ "inputs: {}"; "outputs: {}"; "expression: $(1)"; line ]) Decode.decodeExpressionTool Encode.encodeExpressionToolDescription (fun e -> e.Intent)
      intent "an operation" (fun line -> Kit.operation [ "inputs: {}"; "outputs: {}"; line ]) Decode.decodeOperation Encode.encodeOperationDescription (fun o -> o.Intent)
      tool Kind.WholeStrings "the expressionLib" "requirements[0].expressionLib" (fun v -> requirements [ "  - class: InlineJavascriptRequirement"; "    expressionLib: " + v ]) (fun t -> match requirement t with InlineJavascriptRequirement lib -> given (ofOne ofString) lib.ExpressionLib | other -> Value (box other))
      tool Kind.WholeStrings "the specs of a package" "hints[0].packages[0].specs" (fun v -> software [ "packages:"; "  - package: git"; "    specs: " + v ]) (fun t -> given (ofOne ofString) (package t).Specs)
      tool Kind.WholeStrings "the version of a package" "hints[0].packages[0].version" (fun v -> software [ "packages:"; "  - package: git"; "    version: " + v ]) (fun t -> given (ofOne ofString) (package t).Version)
      |> plain [ "v1", Fails "Expected a list of strings" ]
      tool Kind.WholeStrings "the symbols of an enum" "inputs.a.type.symbols" (fun v -> schemaPort [ "type: enum"; "symbols: " + v ]) (fun t -> match schema t with Enum e -> ofOne ofString e.Symbols | other -> Value (box other))

      tool Kind.Class "the class of a requirement" "requirements[0].class" (fun entry -> Kit.toolWith [ "requirements:"; "  - " + entry ]) (fun t -> ofRequirement t.Requirements.Value.[0])
      |> holding (fun v -> "{class: " + v + ", dockerPull: alpine}")
      tool Kind.Class "the class of a hint" "hints[0].class" (fun entry -> Kit.toolWith [ "hints:"; "  - " + entry ]) (fun t -> ofHint t.Hints.Value.[0])
      |> holding (fun v -> "{class: " + v + ", dockerPull: alpine}")
      tool Kind.DefinitionType "the type of an entry of the SchemaDefRequirement" "requirements[0].types[0].type" (fun entry -> requirements [ "  - class: SchemaDefRequirement"; "    types:"; "      - " + entry ]) (fun t -> match requirement t with SchemaDefRequirement entries -> ofSchemaDef entries.[0] | other -> Value (box other))
      |> holding (fun v -> "{type: " + v + ", name: R, fields: []}")
      tool Kind.Class "the type of the schema of an input" "inputs.x.type.type" (fun schema -> Kit.tool [ "baseCommand: echo"; "inputs: {x: {type: " + schema + "}}"; "outputs: []" ]) (fun t -> ofType (input t).Type_.Value)
      |> holding (fun v -> "{type: " + v + ", symbols: [a]}")
      tool Kind.Class "the type of the schema of an output" "outputs.o.type.type" (fun schema -> Kit.tool [ "baseCommand: echo"; "inputs: []"; "outputs: {o: {type: " + schema + ", outputBinding: {glob: o}}}" ]) (fun t -> ofType (output t).Type_.Value)
      |> holding (fun v -> "{type: " + v + ", fields: {f: string}}")
      tool Kind.Class "the type of the schema in the items of an array" "inputs.x.type.items.type" (fun schema -> Kit.tool [ "baseCommand: echo"; "inputs: {x: {type: {type: array, items: " + schema + "}}}"; "outputs: []" ]) (fun t -> match (input t).Type_.Value with Array a -> ofType a.Items | other -> Value (box other))
      |> holding (fun v -> "{type: " + v + ", symbols: [a]}")
      tool Kind.Class "the type of the schema in a union" "inputs.x.type[1].type" (fun schema -> Kit.tool [ "baseCommand: echo"; "inputs: {x: {type: [\"null\", " + schema + "]}}"; "outputs: []" ]) (fun t -> match (input t).Type_.Value with Union members -> ofType members.[1] | other -> Value (box other))
      |> holding (fun v -> "{type: " + v + ", symbols: [a]}")
      tool Kind.Class "the type of the schema of a record field" "inputs.x.type.fields.f.type.type" (fun schema -> Kit.tool [ "baseCommand: echo"; "inputs: {x: {type: {type: record, fields: {f: {type: " + schema + "}}}}}"; "outputs: []" ]) (fun t -> match (input t).Type_.Value with Record r -> ofType (InputRecordSchema.getFieldValuesOrEmpty r).[0].Type | other -> Value (box other))
      |> holding (fun v -> "{type: " + v + ", symbols: [a]}")
      tool Kind.Class "the class of a File in the listing" "requirements[0].listing[0].class" (fun entry -> listed [ "- " + entry ]) (fun t -> ofListingEntry (listing t).[0])
      |> holding (fun v -> "{class: " + v + ", location: f.txt}")
      tool Kind.Class "the class of a File in an array of the listing" "requirements[0].listing[0][1].class" (fun item -> listed [ "- [{class: File, location: a.txt}, " + item + "]" ]) (fun t -> ofItem (items t).[1])
      |> holding (fun v -> "{class: " + v + ", location: b.txt}")
      workflow Kind.Class "the class of the process that a step runs" "steps.s.run.class" (fun run -> Kit.step [ "run: " + run; "in: []"; "out: []" ]) (fun w -> ofRun (step w).Run)
      |> holding (fun v -> "{class: " + v + ", baseCommand: echo, inputs: [], outputs: []}")
      positionIn Decode.decodeCWLProcessingUnit Encode.encodeProcessingUnit Kind.Class "the class of a process" "class" id ofProcess
      |> holding (fun v -> Kit.document v [ "baseCommand: echo"; "inputs: []"; "outputs: []" ]) ]

/// The positions of the kind.
let ofKind (kind: Kind) : Position list = positions |> List.filter (fun position -> position.Kind = kind)
