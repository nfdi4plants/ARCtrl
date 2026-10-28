/// The plain and quoted scalars of YAML in the scalar fields of CWL: a table of spellings with the literal outcome in each
/// kind of field, where each row runs at every place of the kind.
module Tests.Scalars

open ARCtrl.CWL
open TestingUtils
open TestingUtils.CWL
open Tests.Positions

/// The kind of scalar that a field takes.
[<RequireQualifiedAccess>]
type private Field =
    /// A string, such as a label or an entry of the baseCommand.
    | Text
    /// The class of an object or of a process.
    | Class
    /// A text with $( or ${, as the expression of an expression tool.
    | Expression
    /// The enableReuse of WorkReuse and the networkAccess of NetworkAccess.
    | BoolOrExpression
    | Bool
    /// A bool that the object may leave out, so null counts as missing.
    | OptionalBool
    /// The listing of InitialWorkDirRequirement as a whole: a list or an expression.
    | WorkDir
    /// The position of an input binding: an int of 32 bits or an expression.
    | Position
    /// An entry of the exit codes: an int of 32 bits.
    | Code
    /// The size of a File: a long.
    | Size
    /// The timelimit of ToolTimeLimit: a long or an expression.
    | TimeLimit
    /// A field of ResourceRequirement: a long, a float or an expression.
    | Resource
    /// The required field of a secondary file: a bool or an expression, and null counts as missing.
    | Required
    /// The pattern of a secondary file: a string or an expression.
    | Pattern
    /// An entry of the secondaryFiles: a string or an object.
    | SecondaryFile
    /// The secondaryFiles as a whole: a string, an object or a list, and null counts as missing.
    | SecondaryFiles
    /// The value of a parameter in a job file, which the core schema types.
    | JobValue
    /// A field that the model keeps as YAML: an extension field and the default of an input.
    | Kept
    /// The loadListing of an object.
    | LoadListing
    /// The linkMerge, the pickValue and the scatterMethod of a step.
    | Method

/// What a place holds after the decoder read a spelling.
type private Cell =
    /// The value, and the written text reads back equal.
    | Holds of obj
    /// The value, the written text holds the field with the spelling as it was written, and it reads back equal.
    | HoldsAsWritten of obj
    /// The field counts as missing, and the written text reads back equal.
    | Missing
    /// The document decodes, the written text holds the field with the spelling as it was written and reads back equal.
    | AsWritten
    /// The document decodes and the written text reads back equal. The model keeps the field as YAML.
    | Passes
    /// A decode error at the place whose message holds the keyword.
    | Fails of string
    /// A decode error at the field below the place, such as .$import, whose message holds the keyword.
    | FailsAt of string * string

/// A field that takes a scalar: its name, the path of its errors, the document with the spelling in place, and the reader
/// of the decoded value, which gives none for a missing field.
type private Place =
    { Name: string
      Path: string
      Document: string -> string
      Decode: string -> obj
      Encode: obj -> string
      Read: obj -> obj option }

let private ofPosition (position: Position) : Place =
    { Name = position.Name
      Path = position.Path
      Document = position.Document
      Decode = position.Decode
      Encode = position.Encode
      Read =
        fun decoded ->
            try
                match position.Read decoded with
                | Value value -> Some value
                | other -> failwithf "Expected a plain value, found %A" other
            with error when error.Message = "The position holds no value." ->
                None }

let private named (names: string list) : Place list =
    names |> List.map (fun name -> positions |> List.find (fun position -> position.Name = name) |> ofPosition)

let private toolPlace (name: string) (path: string) (document: string -> string) (read: CWLToolDescription -> obj option) : Place =
    { Name = name
      Path = path
      Document = document
      Decode = fun text -> box (Decode.decodeCommandLineTool text)
      Encode = fun decoded -> Encode.encodeToolDescription (unbox decoded)
      Read = fun decoded -> read (unbox decoded) }

let private workflowPlace (name: string) (path: string) (document: string -> string) (read: CWLWorkflowDescription -> obj option) : Place =
    { Name = name
      Path = path
      Document = document
      Decode = fun text -> box (Decode.decodeWorkflow text)
      Encode = fun decoded -> Encode.encodeWorkflowDescription (unbox decoded)
      Read = fun decoded -> read (unbox decoded) }

/// The text of a literal as a plain value, and any other string as it is.
let private ofText (text: SchemaSaladString) : obj =
    match text with
    | SchemaSaladString.Literal literal -> box literal
    | other -> box other

let private requirementOf (tool: CWLToolDescription) =
    match tool.Requirements, tool.Hints with
    | Some requirements, _ when requirements.Count > 0 -> requirements.[0]
    | _, Some hints ->
        match hints.[0] with
        | KnownHint requirement -> requirement
        | other -> failwithf "Expected a known hint, found %A" other
    | _ -> failwith "The tool holds no requirement."

let private boolOrExpression (tool: CWLToolDescription) : obj option =
    match requirementOf tool with
    | WorkReuseRequirement value -> Some (box value.EnableReuse)
    | NetworkAccessRequirement value -> Some (box value.NetworkAccess)
    | WorkReuseExpressionRequirement text
    | NetworkAccessExpressionRequirement text -> Some (ofText text)
    | other -> failwithf "Expected WorkReuse or NetworkAccess, found %A" other

let private timeLimit (tool: CWLToolDescription) : obj option =
    match requirementOf tool with
    | ToolTimeLimitRequirement (ToolTimeLimitSeconds seconds) -> Some (box seconds)
    | ToolTimeLimitRequirement (ToolTimeLimitExpression text) -> Some (ofText text)
    | other -> failwithf "Expected a ToolTimeLimit, found %A" other

let private resource (field: string) (tool: CWLToolDescription) : obj option =
    match requirementOf tool with
    | ResourceRequirement resources ->
        resources.KnownFieldValues
        |> List.pick (fun (name, held) -> if name = field then Some held else None)
    | other -> failwithf "Expected a ResourceRequirement, found %A" other

let private lines (prefix: string list) (body: string list) = prefix @ (body |> List.map (fun line -> "      " + line))

/// A tool whose input x of type File has the secondary files of the lines.
let private secondaryOfInput (body: string list) =
    Kit.tool ([ "baseCommand: echo"; "inputs:"; "  x:"; "    type: File" ] @ lines [ "    secondaryFiles:" ] body @ [ "outputs: []" ])

/// A tool whose output o of type File has the secondary files of the lines.
let private secondaryOfOutput (body: string list) =
    Kit.tool ([ "baseCommand: echo"; "inputs: []"; "outputs:"; "  o:"; "    type: File"; "    outputBinding: {glob: o}" ] @ lines [ "    secondaryFiles:" ] body)

let private secondary (name: string) (path: string) (document: string -> string) =
    toolPlace name path document (fun _ -> None)

let private resourceFields = [ "coresMin"; "coresMax"; "ramMin"; "ramMax"; "tmpdirMin"; "tmpdirMax"; "outdirMin"; "outdirMax" ]

let private jobPlace : Place =
    { Name = "the value of a parameter in a job file"
      Path = "x"
      Document = fun value -> "x: " + value + "\n"
      Decode = fun text -> box (List.ofSeq (DecodeParameters.decodeYAMLParameterFile text))
      Encode = fun decoded -> Encode.encodeYAMLParameterFile (ResizeArray (unbox<CWLParameterReference list> decoded))
      Read = fun decoded -> (List.head (unbox<CWLParameterReference list> decoded)).Value |> Option.map box }

let private places (field: Field) : Place list =
    match field with
    | Field.Text ->
        (ofKind Kind.Text |> List.filter (fun position -> not (position.Name.StartsWith "the listing" || position.Name = "an entry of the listing")) |> List.map ofPosition)
        @ [ toolPlace "the id of a tool" "id" (fun v -> Kit.tool [ "id: " + v; "baseCommand: echo"; "inputs: {}"; "outputs: {}" ]) (fun t -> t.Id |> Option.map box)
            workflowPlace "the run of a step" "steps.s.run" (fun v -> Kit.step [ "run: " + v; "in: {}"; "out: []" ]) (fun w -> match (ListEntry.values w.Steps).[0].Run with RunString path -> Some (box path) | other -> failwithf "Expected a path, found %A" other)
            workflowPlace "an entry of the out field of a step" "steps.s.out[0]" (fun v -> Kit.step [ "run: t.cwl"; "in: {}"; "out: [" + v + "]" ]) (fun w -> match (ListEntry.values w.Steps).[0].Out.[0] with StepOutputString id -> Some (box id) | other -> failwithf "Expected an id, found %A" other)
            workflowPlace "the source shorthand of a step input" "steps.s.in.x" (fun v -> Kit.step [ "run: t.cwl"; "in: {x: " + v + "}"; "out: []" ]) (fun w -> match (WorkflowStep.getInputValues (ListEntry.values w.Steps).[0]).[0].Source with Some (OutputSource.Single text) -> Some (ofText text) | other -> failwithf "Expected one source, found %A" other) ]
    | Field.Class -> named [ "the class of a requirement"; "the class of a hint"; "the class of a process"; "the class of the process that a step runs" ]
    | Field.Expression -> named [ "the expression of an expression tool"; "the when of a step"; "the outputEval of an outputBinding" ]
    | Field.BoolOrExpression ->
        [ for cls, field in [ "WorkReuse", "enableReuse"; "NetworkAccess", "networkAccess" ] do
            for list in [ "requirements"; "hints" ] do
                toolPlace (sprintf "the %s of %s in the %s" field cls list) (list + "[0]." + field) (fun v -> Kit.toolWith [ list + ":"; "  - class: " + cls; "    " + field + ": " + v ]) boolOrExpression ]
    | Field.Bool -> ofKind Kind.Bool |> List.map ofPosition
    | Field.OptionalBool -> ofKind Kind.Bool |> List.filter (fun position -> position.Name <> "the inplaceUpdate of the requirement") |> List.map ofPosition
    | Field.WorkDir -> named [ "the listing as a whole" ]
    | Field.Position -> ofKind Kind.Int |> List.map ofPosition
    | Field.Code -> named [ "an entry of the successCodes"; "an entry of the temporaryFailCodes"; "an entry of the permanentFailCodes" ]
    | Field.Size -> named [ "the size of a File in the listing" ]
    | Field.TimeLimit ->
        [ toolPlace "the timelimit of a requirement" "requirements[0].timelimit" (fun v -> Kit.toolWith [ "requirements:"; "  - class: ToolTimeLimit"; "    timelimit: " + v ]) timeLimit
          toolPlace "the timelimit of a hint in map form" "hints.ToolTimeLimit.timelimit" (fun v -> Kit.toolWith [ "hints:"; "  ToolTimeLimit:"; "    timelimit: " + v ]) timeLimit ]
    | Field.Resource ->
        [ for field in resourceFields do
            toolPlace ("the " + field + " of a requirement") ("requirements[0]." + field) (fun v -> Kit.toolWith [ "requirements:"; "  - class: ResourceRequirement"; "    " + field + ": " + v ]) (resource field)
          toolPlace "the ramMin of a hint" "hints[0].ramMin" (fun v -> Kit.toolWith [ "hints:"; "  - class: ResourceRequirement"; "    ramMin: " + v ]) (resource "ramMin")
          toolPlace "the ramMin of a hint in map form" "hints.ResourceRequirement.ramMin" (fun v -> Kit.toolWith [ "hints:"; "  ResourceRequirement:"; "    ramMin: " + v ]) (resource "ramMin")
          toolPlace "the ramMin of a requirement in map form" "requirements.ResourceRequirement.ramMin" (fun v -> Kit.toolWith [ "requirements:"; "  ResourceRequirement:"; "    ramMin: " + v ]) (resource "ramMin") ]
    | Field.Required ->
        [ secondary "the required of a secondary file of an input" "inputs.x.secondaryFiles[0].required" (fun v -> secondaryOfInput [ "- pattern: .idx"; "  required: " + v ])
          secondary "the required of the one secondary file of an input" "inputs.x.secondaryFiles.required" (fun v -> secondaryOfInput [ "pattern: .idx"; "required: " + v ])
          secondary "the required of the second secondary file of an input" "inputs.x.secondaryFiles[1].required" (fun v -> secondaryOfInput [ "- .bai"; "- pattern: .idx"; "  required: " + v ])
          secondary "the required of a secondary file of an output" "outputs.o.secondaryFiles[0].required" (fun v -> secondaryOfOutput [ "- pattern: .idx"; "  required: " + v ]) ]
    | Field.Pattern ->
        [ secondary "the pattern of a secondary file of an input" "inputs.x.secondaryFiles[0].pattern" (fun v -> secondaryOfInput [ "- pattern: " + v ])
          secondary "the pattern of the one secondary file of an input" "inputs.x.secondaryFiles.pattern" (fun v -> secondaryOfInput [ "{pattern: " + v + "}" ])
          secondary "the pattern of the second secondary file of an input" "inputs.x.secondaryFiles[1].pattern" (fun v -> secondaryOfInput [ "- pattern: .bai"; "- pattern: " + v ])
          secondary "the pattern of a secondary file of an output" "outputs.o.secondaryFiles[0].pattern" (fun v -> secondaryOfOutput [ "- pattern: " + v ]) ]
    | Field.SecondaryFile ->
        [ secondary "an entry of the secondary files of an input" "inputs.x.secondaryFiles[0]" (fun v -> secondaryOfInput [ "- " + v ])
          secondary "an entry of the secondary files of an output" "outputs.o.secondaryFiles[0]" (fun v -> secondaryOfOutput [ "- " + v ]) ]
    | Field.SecondaryFiles -> [ secondary "the secondary files of an input" "inputs.x.secondaryFiles" (fun v -> secondaryOfInput [ v ]) ]
    | Field.JobValue -> [ jobPlace ]
    | Field.Kept ->
        [ toolPlace "an extension field" "ex:big" (fun v -> Kit.toolWith [ "$namespaces: {ex: http://example.org/}"; "ex:big: " + v ]) (fun _ -> None)
          toolPlace "the default of an input of type Any" "inputs.x.default" (fun v -> Kit.tool [ "baseCommand: echo"; "inputs:"; "  x:"; "    type: Any"; "    default: " + v; "outputs: []" ]) (fun _ -> None) ]
    | Field.LoadListing -> ofKind Kind.Enum |> List.filter (fun position -> position.Name.StartsWith "the loadListing") |> List.map ofPosition
    | Field.Method -> ofKind Kind.Enum |> List.filter (fun position -> not (position.Name.StartsWith "the loadListing")) |> List.map ofPosition

let private fieldName (field: Field) =
    match field with
    | Field.Text -> "string"
    | Field.Class -> "class"
    | Field.Expression -> "expression"
    | Field.BoolOrExpression -> "bool or expression"
    | Field.Bool -> "bool"
    | Field.OptionalBool -> "optional bool"
    | Field.WorkDir -> "whole listing"
    | Field.Position -> "binding position"
    | Field.Code -> "exit code"
    | Field.Size -> "size"
    | Field.TimeLimit -> "time limit"
    | Field.Resource -> "resource"
    | Field.Required -> "required of a secondary file"
    | Field.Pattern -> "pattern of a secondary file"
    | Field.SecondaryFile -> "secondary file entry"
    | Field.SecondaryFiles -> "whole secondaryFiles"
    | Field.JobValue -> "job file value"
    | Field.Kept -> "kept"
    | Field.LoadListing -> "loadListing"
    | Field.Method -> "linkMerge, pickValue and scatterMethod"

let private told (cell: Cell) =
    match cell with
    | Holds value
    | HoldsAsWritten value -> sprintf "reads as %A and reads back equal" value
    | Missing -> "counts as missing"
    | AsWritten -> "is written back as it was"
    | Passes -> "decodes and reads back equal"
    | Fails keyword
    | FailsAt (_, keyword) -> "is a decode error that says " + keyword

let private beyondFloat = "1" + String.replicate 400 "0"
let private hexBeyondFloat = "0x1" + String.replicate 300 "0"

/// The name of a spelling in the name of a test.
let private spellingName (text: string) =
    if text = beyondFloat then "1 and 400 zeros"
    elif text = hexBeyondFloat then "0x1 and 300 zeros"
    else text

let private T = Field.Text
let private C = Field.Class
let private E = Field.Expression
let private BE = Field.BoolOrExpression
let private B = Field.Bool
let private P = Field.Position
let private Cd = Field.Code
let private Sz = Field.Size
let private TL = Field.TimeLimit
let private R = Field.Resource
let private Rq = Field.Required
let private Pt = Field.Pattern
let private SF = Field.SecondaryFile
let private SFs = Field.SecondaryFiles
let private Job = Field.JobValue
let private Kp = Field.Kept
let private L = Field.LoadListing
let private OB = Field.OptionalBool
let private W = Field.WorkDir
let private M = Field.Method

/// The failures of the plain scalars that the core schema reads as no string, in the fields that take no number.
let private noNumber = [ T, Fails "plain scalar"; C, Fails "Expected a string"; E, Fails "Expected an expression"; BE, Fails "Expected a bool or an expression"; B, Fails "Expected a bool"; Rq, Fails "Expected a bool or an expression"; Pt, Fails "as a pattern"; SF, Fails "as a secondary file"; SFs, Fails "as a secondary file"; L, Fails "Expected one of"; M, Fails "Invalid" ]

/// The failures of a text without an expression in the fields that take no string.
let private noText = [ E, Fails "Expected an expression"; BE, Fails "Expected a bool or an expression"; B, Fails "Expected a bool"; P, Fails "Expected an int or an expression"; Cd, Fails "Expected an int"; Sz, Fails "Expected an int64"; TL, Fails "Expected an int or an expression"; R, Fails "Expected a number or an expression"; Rq, Fails "Expected a bool or an expression"; W, Fails "for the listing" ]

/// The spelling, and the outcome in each kind of field that the row runs at.
let private rows : (string * (Field * Cell) list) list =
    [ "5", noNumber @ [ P, Holds (box (BindingPosition.Number 5)); Cd, Holds (box 5); Sz, Holds (box 5L); TL, Holds (box 5L); R, Holds (box 5L); Job, Holds (box (CWLParameterValue.Int 5L)); Kp, AsWritten ]
      "-1", [ P, Holds (box (BindingPosition.Number -1)); Cd, Holds (box -1) ]
      "1.5", noNumber @ [ P, Fails "Expected an int or an expression"; Cd, Fails "Expected an int"; Sz, Fails "Expected an int64"; TL, Fails "Expected an int or an expression"; R, Holds (box 1.5); Job, Holds (box (CWLParameterValue.Float 1.5)) ]
      "4.0", [ R, Holds (box 4.0); Job, Holds (box (CWLParameterValue.Float 4.0)) ]
      ".5e3", [ T, Fails "plain scalar"; C, Fails "Expected a string"; E, Fails "Expected an expression"; R, Holds (box 500.0) ]
      "+.5e3", [ T, Fails "plain scalar"; C, Fails "Expected a string"; E, Fails "Expected an expression"; R, Holds (box 500.0) ]
      ".inf", noNumber @ [ P, Fails "Expected an int or an expression"; Cd, Fails "Expected an int"; Sz, Fails "Expected an int64"; TL, Fails "Expected an int or an expression"; R, Holds (box infinity); Job, Holds (box (CWLParameterValue.Float infinity)) ]
      ".nan", [ T, Fails "plain scalar"; E, Fails "Expected an expression"; P, Fails "Expected an int or an expression"; Cd, Fails "Expected an int"; TL, Fails "Expected an int or an expression"; Pt, Fails "as a pattern" ]
      "true", noNumber |> List.filter (fun (field, _) -> field <> BE && field <> B && field <> Rq) |> List.append [ BE, Holds (box true); B, Holds (box true); Rq, Passes; P, Fails "Expected an int or an expression"; Cd, Fails "Expected an int"; Sz, Fails "Expected an int64"; TL, Fails "Expected an int or an expression"; R, Fails "Expected a number or an expression"; Job, Holds (box (CWLParameterValue.Boolean true)) ]
      "false", [ BE, Holds (box false); B, Holds (box false); Rq, Passes; Cd, Fails "Expected an int"; TL, Fails "Expected an int or an expression"; R, Fails "Expected a number or an expression"; Job, Holds (box (CWLParameterValue.Boolean false)) ]
      "null", [ OB, Missing; P, Missing; Sz, Missing; R, Missing; Rq, Passes; Pt, Fails "as a pattern"; SF, Fails "as a secondary file"; SFs, Passes; Job, Holds (box CWLParameterValue.Null); Cd, Fails "Expected an int"; TL, Fails "Expected" ]
      "~", [ OB, Missing; Rq, Passes; Pt, Fails "as a pattern"; SFs, Passes; Job, Holds (box CWLParameterValue.Null) ]
      "\"5\"", noText @ [ T, Holds (box "5"); Pt, Passes; SF, Passes; SFs, Passes; Job, Holds (box (CWLParameterValue.String "5")); L, Fails "Expected one of"; M, Fails "Invalid" ]
      "'5'", [ T, Holds (box "5"); TL, Fails "Expected an int or an expression"; R, Fails "Expected a number or an expression"; SF, Passes; SFs, Passes ]
      "\"true\"", [ T, Holds (box "true"); E, Fails "Expected an expression"; P, Fails "Expected an int or an expression"; W, Fails "for the listing"; B, Fails "Expected a bool"; BE, Fails "Expected a bool or an expression"; TL, Fails "Expected an int or an expression"; R, Fails "Expected a number or an expression"; Rq, Fails "Expected a bool or an expression"; Pt, Passes; Job, Holds (box (CWLParameterValue.String "true")) ]
      "abc", noText @ [ T, Holds (box "abc"); Pt, Passes; SF, Passes; SFs, Passes; Job, Holds (box (CWLParameterValue.String "abc")); L, Fails "Expected one of"; M, Fails "Invalid" ]
      "''", noText @ [ T, Holds (box ""); Pt, Passes; SF, Passes; SFs, Passes; Job, Holds (box (CWLParameterValue.String "")) ]
      "$(x)",
      [ T, Holds (box "$(x)"); W, Holds (box "$(x)"); E, Holds (box "$(x)"); BE, Holds (box "$(x)"); B, Fails "Expected a bool"; P, HoldsAsWritten (box (BindingPosition.Expression "$(x)")); Cd, Fails "Expected an int"; Sz, Fails "Expected an int64"
        TL, Holds (box "$(x)"); R, Holds (box (SchemaSaladString.Literal "$(x)")); Rq, Passes; Pt, Passes; SF, Passes; SFs, Passes; Job, Holds (box (CWLParameterValue.String "$(x)")) ]
      "'${return 1;}'", [ E, Holds (box "${return 1;}"); BE, Holds (box "${return 1;}"); P, Holds (box (BindingPosition.Expression "${return 1;}")); TL, Holds (box "${return 1;}"); R, Holds (box (SchemaSaladString.Literal "${return 1;}")); Rq, Passes; Pt, Passes; SF, Passes ]
      "'size: $(1)'", [ E, Holds (box "size: $(1)"); TL, Holds (box "size: $(1)"); R, Holds (box (SchemaSaladString.Literal "size: $(1)")) ]
      // Ints beyond 64 bits and the ends of the ranges of 32 and 64 bits.
      "18446744073709551616",
      noNumber @ [ P, Fails "64-bit range"; Cd, Fails "32-bit range"; Sz, Fails "64-bit range"; TL, Fails "64-bit range"; R, Holds (box 18446744073709551616.0); Job, Fails "beyond 64 bits"; Kp, AsWritten ]
      "100000000000000000000000000000", [ T, Fails "plain scalar"; P, Fails "64-bit range"; Cd, Fails "32-bit range"; Sz, Fails "64-bit range"; TL, Fails "64-bit range"; R, Holds (box 1e29); SF, Fails "as a secondary file"; Job, Fails "beyond 64 bits"; Kp, AsWritten ]
      "-100000000000000000000000000000", [ R, Holds (box -1e29); Kp, AsWritten ]
      "9223372036854775807", [ P, Fails "32-bit range"; Cd, Fails "32-bit range"; Sz, Holds (box 9223372036854775807L); TL, Holds (box 9223372036854775807L); R, Holds (box 9223372036854775807L); Job, Holds (box (CWLParameterValue.Int 9223372036854775807L)) ]
      "0x7FFFFFFFFFFFFFFF", [ R, Holds (box 9223372036854775807L) ]
      "9223372036854775808", [ Sz, Fails "64-bit range"; TL, Fails "64-bit range"; R, Holds (box 9223372036854775808.0); Job, Fails "beyond 64 bits" ]
      "+9223372036854775808", [ Sz, Fails "64-bit range"; TL, Fails "64-bit range"; R, Holds (box 9223372036854775808.0); Job, Fails "beyond 64 bits" ]
      "-9223372036854775808",
      [ T, Fails "plain scalar"; P, Fails "32-bit range"; Cd, Fails "32-bit range"; Sz, Holds (box -9223372036854775808L); TL, Holds (box -9223372036854775808L); R, HoldsAsWritten (box -9223372036854775808L); Job, HoldsAsWritten (box (CWLParameterValue.Int -9223372036854775808L)) ]
      "-9223372036854775809", [ P, Fails "64-bit range"; Cd, Fails "32-bit range"; Sz, Fails "64-bit range"; TL, Fails "64-bit range"; R, Holds (box -9223372036854775808.0); Job, Fails "beyond 64 bits" ]
      // The prefixed spellings take no sign in the core schema, so these are strings.
      "-0x8000000000000000", [ T, Holds (box "-0x8000000000000000"); Sz, Fails "Expected an int64"; TL, Fails "Expected an int or an expression"; R, Fails "Expected a number or an expression"; Job, Holds (box (CWLParameterValue.String "-0x8000000000000000")) ]
      "-0o1000000000000000000000", [ Sz, Fails "Expected an int64"; TL, Fails "Expected an int or an expression"; R, Fails "Expected a number or an expression"; Job, Holds (box (CWLParameterValue.String "-0o1000000000000000000000")) ]
      // An int beyond 64 bits is the nearest float in every base of the core schema.
      "0x8000000000000000", [ P, Fails "64-bit range"; Cd, Fails "32-bit range"; Sz, Fails "64-bit range"; TL, Fails "64-bit range"; R, Holds (box 9223372036854775808.0); Job, Fails "beyond 64 bits"; Kp, AsWritten ]
      "0x400000000000000000", [ R, Holds (box 1180591620717411303424.0) ]
      "0x10000000000000800", [ R, Holds (box 18446744073709551616.0) ]
      "0x10000000000000801", [ R, Holds (box 18446744073709555712.0) ]
      "0x10000000000001800", [ R, Holds (box 18446744073709559808.0) ]
      "0o7777777777777777777777777", [ R, Holds (box 37778931862957161709568.0) ]
      beyondFloat, [ R, Fails "beyond the largest float" ]
      hexBeyondFloat, [ R, Fails "beyond the largest float" ]
      "2147483647", [ P, Holds (box (BindingPosition.Number 2147483647)); Cd, Holds (box 2147483647) ]
      "2147483648", [ P, Fails "32-bit range"; Cd, Fails "32-bit range" ]
      "-2147483648", [ P, Holds (box (BindingPosition.Number -2147483648)); Cd, Holds (box -2147483648) ]
      "-2147483649", [ P, Fails "32-bit range"; Cd, Fails "32-bit range" ]
      // The ints of the core schema in the other bases, and spellings that only YAML 1.1 reads as ints.
      "0x1F", [ T, Fails "plain scalar"; C, Fails "Expected a string"; E, Fails "Expected an expression"; P, Holds (box (BindingPosition.Number 31)); Cd, Holds (box 31); Sz, Holds (box 31L); TL, Holds (box 31L); R, Holds (box 31L); Job, Holds (box (CWLParameterValue.Int 31L)) ]
      "0o17", [ T, Fails "plain scalar"; C, Fails "Expected a string"; E, Fails "Expected an expression"; P, Holds (box (BindingPosition.Number 15)); Cd, Holds (box 15); Sz, Holds (box 15L); TL, Holds (box 15L); R, Holds (box 15L); Job, Holds (box (CWLParameterValue.Int 15L)) ]
      "017", [ T, Fails "plain scalar"; C, Fails "Expected a string"; E, Fails "Expected an expression"; P, Holds (box (BindingPosition.Number 17)); Cd, Holds (box 17); Sz, Holds (box 17L); TL, Holds (box 17L) ]
      "1_000", [ T, Holds (box "1_000"); P, Fails "Expected an int or an expression"; Cd, Fails "Expected an int"; Sz, Fails "Expected an int64"; TL, Fails "Expected an int or an expression"; R, Fails "Expected a number or an expression"; Pt, Passes; Job, Holds (box (CWLParameterValue.String "1_000")) ]
      "0b101", [ T, Holds (box "0b101"); Cd, Fails "Expected an int" ]
      "-0x10", [ Cd, Fails "Expected an int" ]
      "0b_", [ Cd, Fails "Expected an int" ]
      "yes", [ T, Holds (box "yes"); B, Fails "Expected a bool"; Rq, Fails "Expected a bool or an expression" ]
      "0X1F", [ T, Holds (box "0X1F") ]
      "1_0.5", [ T, Holds (box "1_0.5") ]
      "2001-12-14", [ T, Holds (box "2001-12-14"); Pt, Passes ]
      "1:30", [ T, Holds (box "1:30") ]
      "=", [ T, Holds (box "="); Job, Holds (box (CWLParameterValue.String "=")) ]
      "<<", [ T, Holds (box "<<"); Job, Holds (box (CWLParameterValue.String "<<")) ]
      "._", [ T, Holds (box "._") ]
      // Nodes that are no scalar, and directives, where a scalar stands.
      "0", [ Rq, Fails "Expected a bool or an expression" ]
      "1", [ Rq, Fails "Expected a bool or an expression" ]
      "[1]", [ Rq, Fails "Expected a bool or an expression" ]
      "[true]", [ Rq, Fails "Expected a bool or an expression" ]
      "[]", [ Rq, Fails "Expected a bool or an expression"; SFs, Passes ]
      "[a]", [ Pt, Fails "as a pattern"; SF, Fails "as a secondary file" ]
      "{a: b}", [ Rq, Fails "Expected a bool or an expression"; Pt, Fails "as a pattern" ]
      "{}", [ SF, Fails "Expected a pattern"; SFs, Fails "Expected a pattern" ]
      "{required: true}", [ SF, Fails "Expected a pattern" ]
      "[.bai, \"^.idx\"]", [ SFs, Passes ]
      "{$import: s.yml}", [ Rq, Passes; Pt, Passes; SF, Passes; SFs, Passes ]
      "{$include: s.txt}", [ Rq, Passes; Pt, Passes; SF, Passes; SFs, Passes ]
      "{$mixin: s.yml}", [ Rq, Passes; Pt, Passes; SF, Passes; SFs, Passes ]
      "{$mixin: s.yml, extra: 1}", [ Rq, Passes; Pt, Passes ]
      "{$mixin: s.yml, required: 1}", [ SF, FailsAt (".required", "Expected a bool or an expression") ]
      "{$import: e.yml}", [ BE, Holds (box (SchemaSaladString.Import "e.yml")); TL, Holds (box (SchemaSaladString.Import "e.yml")); R, Holds (box (SchemaSaladString.Import "e.yml")) ]
      "{$include: e.txt}", [ BE, Holds (box (SchemaSaladString.Include "e.txt")); TL, Holds (box (SchemaSaladString.Include "e.txt")); R, Holds (box (SchemaSaladString.Include "e.txt")) ]
      "{$import: 42}", [ Rq, FailsAt (".$import", "to be a string"); Pt, FailsAt (".$import", "to be a string") ] ]

let private expectCell (text: string) (cell: Cell) (place: Place) =
    let name = place.Name + " with " + spellingName text
    let decode () = place.Decode (place.Document text)
    match cell with
    | Fails keyword -> Kit.decodeFails name decode place.Path keyword
    | FailsAt (below, keyword) -> Kit.decodeFails name decode (place.Path + below) keyword
    | _ ->
        let decoded, written = Kit.roundTrip name place.Decode place.Encode (place.Document text)
        let key = place.Path.Substring(place.Path.LastIndexOf '.' + 1)
        match cell with
        | Holds value -> Expect.equal (place.Read decoded) (Some value) (sprintf "%s: the field should hold the value." name)
        | HoldsAsWritten value ->
            Expect.equal (place.Read decoded) (Some value) (sprintf "%s: the field should hold the value." name)
            Expect.stringContains written (key + ": " + text) (sprintf "%s: the value should be written as it was.\n%s" name written)
        | Missing -> Expect.isNone (place.Read decoded) (sprintf "%s: the field should be missing." name)
        | AsWritten -> Expect.stringContains written (key + ": " + text) (sprintf "%s: the value should be written as it was.\n%s" name written)
        | _ -> ()

/// Run the check at every place of the field and fail with the names of all the places where it fails.
let private atEvery (field: Field) (check: Place -> unit) =
    let all = places field
    Expect.isFalse all.IsEmpty (sprintf "The table should have places of the kind %s." (fieldName field))
    let failures =
        all
        |> List.choose (fun place ->
            try
                check place
                None
            with error ->
                Some (place.Name + ": " + error.Message))
    if not failures.IsEmpty then
        failwith (String.concat "\n\n" failures)

let main =
    testList "Scalars" [
        testList "The scalar table" [
            for (text, cells) in rows do
                for (field, cell) in cells do
                    testCase (sprintf "%s %s in every %s field" (spellingName text) (told cell) (fieldName field)) <| fun _ ->
                        atEvery field (expectCell text cell)
        ]
        testCase "a block scalar is a pattern of a secondary file" <| fun _ ->
            Decode.decodeCommandLineTool (secondaryOfInput [ "- pattern: |"; "    .bai" ]) |> ignore
    ]
