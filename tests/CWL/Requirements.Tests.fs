module Tests.Requirements

open ARCtrl.CWL
open YAMLicious
open YAMLicious
open DynamicObj
open TestingUtils
open TestingUtils.CWL

open Fable.Pyxpecto
open TestObjects.CWL

let decodeRequirements (cwl: string) =
    cwl
    |> Decode.readYaml
    |> Decode.requirementsDecoder
    |> fun r -> r.Value

let decodeHints (cwl: string) =
    cwl
    |> Decode.readYaml
    |> Decode.hintsDecoder
    |> fun h -> h.Value

let findRequirement reqs predicate =
    reqs
    |> Seq.tryFind predicate
    |> Option.defaultWith (fun () -> failwith "Required requirement not found")

/// The error that a decode raises, and none when it succeeds.
let decodeError (decode: unit -> unit) =
    try
        decode ()
        None
    with YamlDecodeException error -> Some error

/// The type that an entry of a SchemaDefRequirement defines.
let definedOf (entry: SchemaDefEntry) : SchemaDefinition =
    match entry with
    | Defined definition -> definition
    | Imported path -> failwith $"Expected a defined type but got the import of {path}"
    | SchemaDefEntry.Mixed _ -> failwith "Expected a defined type but got a mixin"
    | SchemaDefEntry.Raw _ -> failwith "Expected a defined type but got a schema of unknown kind"

/// The name that a definition holds, and none for an unnamed one.
let nameOf (definition: SchemaDefinition) : string option =
    match definition with
    | SchemaDefinition.Record schema -> schema.Name
    | SchemaDefinition.Enum schema -> schema.Name
    | SchemaDefinition.Array schema -> schema.Name

let testRequirementDecode =
    testList "Decode" [
        testList "Length" [
            testCase "Class Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsClassFileContent
                Expect.hasLength reqs 5 "Test expect Requirements features Length is equal 5"
            testCase "Mapping Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsMappingFileContent
                Expect.hasLength reqs 5 "Test expect Requirements features Length is equal 5"
            testCase "Json Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsJSONFileContent
                Expect.hasLength reqs 5 "Test expect Requirements features Length is equal 5"
        ]
        testList "Hints Unknown Passthrough" [
            testCase "Unknown hint class is preserved as UnknownHint" <| fun _ ->
                let yaml = """hints:
  - class: CustomVendorHint
    vendorFlag: true
    nested:
      key: value"""
                let hints = decodeHints yaml
                Expect.equal hints.Count 1 "Expected one hint entry"
                match hints.[0] with
                | UnknownHint unknownHint ->
                    Expect.equal unknownHint.Class (Some "CustomVendorHint") "Unknown hint class should be captured"
                | _ ->
                    failwith "Expected UnknownHint"

            testCase "Known hint still decodes as KnownHint" <| fun _ ->
                let yaml = """hints:
  - class: StepInputExpressionRequirement"""
                let hints = decodeHints yaml
                Expect.equal hints.Count 1 "Expected one hint entry"
                Expect.equal hints.[0] (KnownHint StepInputExpressionRequirement) "Known hint should decode as KnownHint"

            testCase "A malformed known hint is a decode error at the field, in the list form and in the map form" <| fun _ ->
                let cases =
                    [ "list form", "hints[0].dockerPull", "hints:\n  - class: DockerRequirement\n    dockerPull:\n      nested: invalid"
                      "list form with a list", "hints[1].dockerPull", "hints:\n  - class: ShellCommandRequirement\n  - class: DockerRequirement\n    dockerPull: [a, b]"
                      "map form", "hints.DockerRequirement.dockerPull", "hints:\n  DockerRequirement:\n    dockerPull: [a, b]" ]
                for name, path, yaml in cases do
                    match decodeError (fun () -> decodeHints yaml |> ignore) with
                    | Some error ->
                        Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the field" name)
                        Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
                    | None -> failwith (sprintf "%s: a malformed known hint must fail" name)

            testCase "A hint of a known class decodes as the requirement of that class" <| fun _ ->
                let bodies =
                    [ "InlineJavascriptRequirement", [ "expressionLib:"; "  - var a = 1;" ]
                      "DockerRequirement", [ "dockerPull: ubuntu" ]
                      "SoftwareRequirement", [ "packages:"; "  samtools: [\"1.0\"]" ]
                      "EnvVarRequirement", [ "envDef:"; "  A: b" ]
                      "ResourceRequirement", [ "coresMin: 2" ]
                      "WorkReuse", [ "enableReuse: false" ]
                      "NetworkAccess", [ "networkAccess: true" ]
                      "ToolTimeLimit", [ "timelimit: 60" ]
                      "ShellCommandRequirement", []
                      "ScatterFeatureRequirement", [] ]
                for className, body in bodies do
                    let lines (field: string) = field + ":" :: ("  - class: " + className) :: (body |> List.map (fun line -> "    " + line))
                    let asHint = (decodeHints (String.concat "\n" (lines "hints"))).[0]
                    let asRequirement = (decodeRequirements (String.concat "\n" (lines "requirements"))).[0]
                    Expect.equal asHint (KnownHint asRequirement) (sprintf "%s: the hint is the requirement" className)

            testCase "A valid InitialWorkDirRequirement hint with a null item is typed" <| fun _ ->
                let yaml = "hints:\n  - class: InitialWorkDirRequirement\n    listing:\n      - null\n      - [{class: File, location: a.txt}]"
                match (decodeHints yaml).[0] with
                | KnownHint (InitialWorkDirRequirement listing) ->
                    Expect.sequenceEqual listing (ResizeArray [ NullEntry; FilesEntry (ResizeArray [ FileItem (FileInstance(location = SchemaSaladString.Literal "a.txt")) ]) ]) "The listing keeps the null item and the array."
                | other -> failwithf "Expected a known InitialWorkDirRequirement hint, found %A" other

            testCase "A hint without a class is a decode error" <| fun _ ->
                match decodeError (fun () -> decodeHints "hints:\n  - foo: bar" |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) "hints[0]" "The error shows the hint."
                    Expect.stringContains error.Message "class" "The error names the missing field."
                | None -> failwith "A hint without a class must fail."

            testCase "A SchemaDefRequirement hint that a requirement overrides keeps type names that do not resolve" <| fun _ ->
                let tool (hintFields: string) (requirement: bool) =
                    let header = [ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ]
                    let overriding = [ "requirements:"; "  - class: SchemaDefRequirement"; "    types:"; "      - name: A"; "        type: enum"; "        symbols: [x]" ]
                    let hint = [ "hints:"; "  - class: SchemaDefRequirement"; "    types:"; "      - name: B"; "        type: record"; "        fields:"; "          a: " + hintFields ]
                    String.concat "\n" (header @ (if requirement then overriding else []) @ hint @ [ "inputs: {}"; "outputs: {}"; "" ])
                let overridden = Decode.decodeCommandLineTool (tool "Nope" true)
                // Asserted through a bool, because the Python backend drops an empty branch of a match in statement position.
                let isKnownSchemaDef = match overridden.Hints.Value.[0] with | KnownHint (SchemaDefRequirement _) -> true | _ -> false
                Expect.isTrue isKnownSchemaDef "The hint is a known SchemaDefRequirement."
                match decodeError (fun () -> Decode.decodeCommandLineTool (tool "Nope" false) |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) "hints[0].types[0].fields.a" "The error shows the type of the field."
                    Expect.stringContains error.Message "Invalid CWL type: Nope" "The error names the type."
                | None -> failwith "A hint that applies must name types that resolve."

            testCase "Unknown requirement class still fails for requirements" <| fun _ ->
                let yaml = """requirements:
  - class: CustomVendorRequirement
    something: 1"""
                let act () = decodeRequirements yaml |> ignore
                Expect.throws act "Unknown requirement class should fail strict requirements decoding"

            testCase "Unknown hint shape survives decode and encode" <| fun _ ->
                let yaml = """cwlVersion: v1.2
class: CommandLineTool
hints:
  - class: CustomVendorHint
    vendorFlag: true
    nested:
      key: value
baseCommand: echo
inputs: {}
outputs: {}"""
                let decoded = Decode.decodeCommandLineTool yaml
                let encoded = Encode.encodeToolDescription decoded
                Expect.stringContains encoded "CustomVendorHint" "Encoded output should keep unknown hint class"
                Expect.stringContains encoded "vendorFlag" "Encoded output should keep unknown hint payload key"
                Expect.stringContains encoded "nested:" "Encoded output should keep unknown hint nested object"

            testCase "Map-style hints decode known and unknown entries" <| fun _ ->
                let yaml = """hints:
  StepInputExpressionRequirement: {}
  CustomHint:
    flag: true
    nested:
      key: value"""
                let hints = decodeHints yaml
                Expect.equal hints.Count 2 "Expected two hint entries from map-style syntax"
                Expect.equal hints.[0] (KnownHint StepInputExpressionRequirement) "Known map-style hint should decode as KnownHint"
                match hints.[1] with
                | UnknownHint unknownHint ->
                    Expect.equal unknownHint.Class (Some "CustomHint") "Unknown map-style hint class should be preserved"
                    let encoded = Encode.encodeHintEntry hints.[1] |> Encode.writeYaml
                    Expect.stringContains encoded "CustomHint" "Unknown map-style hint should encode with synthetic class"
                    Expect.stringContains encoded "flag" "Unknown map-style hint payload should be preserved"
                | _ ->
                    failwith "Expected UnknownHint for custom map-style hint"
        ]
        testList "InlineJavascriptRequirement" [
            testCase "Decode class-only form" <| fun _ ->
                let yaml = """requirements:
  - class: InlineJavascriptRequirement"""
                let reqs = decodeRequirements yaml
                let requirement = findRequirement reqs (function InlineJavascriptRequirement _ -> true | _ -> false)
                Expect.equal requirement Requirement.defaultInlineJavascriptRequirement "Class-only form should decode to empty payload"

            testCase "Decode expressionLib form" <| fun _ ->
                let yaml = """requirements:
  - class: InlineJavascriptRequirement
    expressionLib:
      - $(function() { return 1; })
      - $(function() { return 2; })"""
                let reqs = decodeRequirements yaml
                let requirement = findRequirement reqs (function InlineJavascriptRequirement _ -> true | _ -> false)
                match requirement with
                | InlineJavascriptRequirement inlineJavascriptRequirement ->
                    let actualExpressionLib = Expect.wantSome inlineJavascriptRequirement.ExpressionLib "expressionLib should be present"
                    let expectedExpressionLib = ResizeArray [| SchemaSaladString.Literal "$(function() { return 1; })"; SchemaSaladString.Literal "$(function() { return 2; })" |]
                    Expect.sequenceEqual actualExpressionLib expectedExpressionLib "expressionLib entries should decode"
                | _ ->
                    failwith "Expected InlineJavascriptRequirement"
            testCase "A scalar expressionLib is a decode error at the field" <| fun _ ->
                let yaml = """requirements:
  - class: InlineJavascriptRequirement
    expressionLib: $(function() { return 42; })"""
                match decodeError (fun () -> decodeRequirements yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) "requirements[0].expressionLib" "The error shows the field."
                    Expect.stringContains error.Message "Expected a list of strings" "The error says what is wrong."
                    Expect.isSome error.Span "The error has the place in the text."
                | None -> failwith "A scalar expressionLib must fail."

            testCase "Encode emits expressionLib when present" <| fun _ ->
                let requirement =
                    InlineJavascriptRequirement (InlineJavascriptRequirementValue(expressionLib = ResizeArray [| SchemaSaladString.Literal "$(function() { return 1; })" |]))
                let encoded = Encode.encodeRequirement requirement |> Encode.writeYaml
                Expect.stringContains encoded "class: InlineJavascriptRequirement" "Encoded output should include class"
                Expect.stringContains encoded "expressionLib" "Encoded output should include expressionLib"

            testCase "Encode omits expressionLib when absent" <| fun _ ->
                let encoded = Encode.encodeRequirement Requirement.defaultInlineJavascriptRequirement |> Encode.writeYaml
                Expect.stringContains encoded "class: InlineJavascriptRequirement" "Encoded output should include class"
                Expect.isFalse (encoded.Contains("expressionLib")) "Encoded output should omit expressionLib when absent"

            testCase "Encode omits expressionLib when empty" <| fun _ ->
                let requirement = InlineJavascriptRequirement (InlineJavascriptRequirementValue(expressionLib = ResizeArray()))
                let encoded = Encode.encodeRequirement requirement |> Encode.writeYaml
                Expect.isFalse (encoded.Contains("expressionLib")) "Encoded output should omit expressionLib for empty arrays"
        ]
        testList "DockerRequirement" [
            testCase "Class Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsClassFileContent
                let dockerItem = findRequirement reqs (function DockerRequirement _ -> true | _ -> false)
                let expected =
                    Requirement.DockerRequirement (
                        DockerRequirement.create(dockerImageId = (SchemaSaladString.Literal "devcontainer"), dockerFileReference = SchemaSaladString.Include "FSharpArcCapsule/Dockerfile")
                    )
                let actual = dockerItem
                Expect.equal actual expected "Mismatch or Wrong requirement type: Type get of Decode Class Syntax for DockerRequirement, can only be DockerRequirement"
            testCase "Mapping Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsMappingFileContent
                let dockerItem = findRequirement reqs (function DockerRequirement _ -> true | _ -> false)
                let expected =
                    Requirement.DockerRequirement (
                        DockerRequirement.create(dockerImageId = (SchemaSaladString.Literal "devcontainer"), dockerFileReference = SchemaSaladString.Include "FSharpArcCapsule/Dockerfile")
                    )
                let actual = dockerItem
                Expect.equal actual expected "Mismatch or Wrong requirement type: Type get of Decode Mapping Syntax for DockerRequirement, can only be DockerRequirement"
            testCase "Json Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsJSONFileContent
                let dockerItem = findRequirement reqs (function DockerRequirement _ -> true | _ -> false)
                let expected =
                    Requirement.DockerRequirement (
                        DockerRequirement.create(dockerImageId = (SchemaSaladString.Literal "devcontainer"), dockerFileReference = SchemaSaladString.Include "FSharpArcCapsule/Dockerfile")
                    )
                let actual = dockerItem
                Expect.equal actual expected "Mismatch or Wrong requirement type: Type get of Decode Json Syntax for DockerRequirement, can only be DockerRequirement"
            testCase "Decode and encode cwltool docker run options extension" <| fun _ ->
                let yaml = """hints:
  - class: DockerRequirement
    dockerImageId: devcontainer
    cwltool:dockerRunOptions:
      - --gpus=all"""
                let hints = decodeHints yaml
                match hints.[0] with
                | KnownHint (DockerRequirement dockerRequirement) ->
                    let options = dockerRequirement.DockerRunOptions |> Option.defaultValue (ResizeArray())
                    Expect.sequenceEqual options (ResizeArray [| "--gpus=all" |]) "DockerRequirement should preserve cwltool docker run options."
                    let encoded = Encode.encodeHintEntry hints.[0] |> Encode.writeYaml
                    Expect.stringContains encoded "cwltool:dockerRunOptions:" "Encoded hint should emit cwltool docker run options."
                    Expect.stringContains encoded "--gpus=all" "Encoded hint should keep cwltool docker run option values."
                | _ ->
                    failwith "Expected DockerRequirement hint"

            testCase "Unknown DockerRequirement payload fields stay on payload and roundtrip" <| fun _ ->
                let yaml = """requirements:
  - class: DockerRequirement
    dockerPull: ubuntu:24.04
    arc:note: keep docker note"""
                let reqs = decodeRequirements yaml
                let requirement = findRequirement reqs (function DockerRequirement _ -> true | _ -> false)
                match requirement with
                | DockerRequirement dockerRequirement ->
                    Expect.equal
                        (DynObj.tryGetTypedPropertyValue<string> "arc:note" dockerRequirement)
                        (Some "keep docker note")
                        "Unknown DockerRequirement fields should be stored on the DockerRequirement payload."
                    let encoded = Encode.encodeRequirement requirement |> Encode.writeYaml
                    let document = "requirements:\n  - " + encoded.Replace("\n", "\n    ")
                    let roundTripped =
                        Decode.readYaml document
                        |> Decode.requirementsDecoder
                        |> Option.get
                        |> fun reqs -> findRequirement reqs (function DockerRequirement _ -> true | _ -> false)
                    match roundTripped with
                    | DockerRequirement roundTrippedDocker ->
                        Expect.equal
                            (DynObj.tryGetTypedPropertyValue<string> "arc:note" roundTrippedDocker)
                            (Some "keep docker note")
                            "Unknown DockerRequirement fields should survive encode/decode."
                    | _ -> failwith "Expected DockerRequirement after roundtrip"
                | _ ->
                    failwith "Expected DockerRequirement"
        ]
        testList "InitialWorkDirRequirement" [
            testCase "Class Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsClassFileContent
                let initialWorkDirItem = findRequirement reqs (function InitialWorkDirRequirement _ -> true | _ -> false)
                let expected =
                    InitialWorkDirRequirement (
                        ResizeArray [|
                            DirentEntry (DirentInstance(SchemaSaladString.Literal "$(inputs.arcDirectory)", entryname = SchemaSaladString.Literal "arc", writable = SchemaSaladBool.Value true))
                            DirentEntry (DirentInstance(SchemaSaladString.Literal "$(inputs.outputDirectory)", writable = SchemaSaladBool.Value true))
                        |]
                    )
                let actual = initialWorkDirItem
                match actual, expected with
                | InitialWorkDirRequirement a, InitialWorkDirRequirement e ->
                    Expect.sequenceEqual a e "InitialWorkDirRequirement mismatch: Type get of Decode Class Syntax for InitialWorkDirRequirement"
                | _ ->
                    failwith "Wrong requirement type: Type of Decode Class Syntax for InitialWorkDirRequirement can only be InitialWorkDirRequirement"
            testCase "Mapping Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsMappingFileContent
                let initialWorkDirItem = findRequirement reqs (function InitialWorkDirRequirement _ -> true | _ -> false)
                let expected =
                    InitialWorkDirRequirement (
                        ResizeArray [|
                            DirentEntry (DirentInstance(SchemaSaladString.Literal "$(inputs.arcDirectory)", entryname = SchemaSaladString.Literal "arc", writable = SchemaSaladBool.Value true))
                            DirentEntry (DirentInstance(SchemaSaladString.Literal "$(inputs.outputDirectory)", writable = SchemaSaladBool.Value true))
                        |]
                    )
                let actual = initialWorkDirItem
                match actual, expected with
                | InitialWorkDirRequirement a, InitialWorkDirRequirement e ->
                    Expect.sequenceEqual a e "InitialWorkDirRequirement mismatch: Type of Decode Mapping Syntax for InitialWorkDirRequirement"
                | _ ->
                    failwith "Wrong requirement type: Type of Decode Mapping Syntax for InitialWorkDirRequirement can only be InitialWorkDirRequirement"
            testCase "Json Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsJSONFileContent
                let initialWorkDirItem = findRequirement reqs (function InitialWorkDirRequirement _ -> true | _ -> false)
                let expected =
                    InitialWorkDirRequirement (
                        ResizeArray [|
                            DirentEntry (DirentInstance(SchemaSaladString.Literal "$(inputs.arcDirectory)", entryname = SchemaSaladString.Literal "arc", writable = SchemaSaladBool.Value true))
                            DirentEntry (DirentInstance(SchemaSaladString.Literal "$(inputs.outputDirectory)", writable = SchemaSaladBool.Value true))
                        |]
                    )
                let actual = initialWorkDirItem
                match actual, expected with
                | InitialWorkDirRequirement a, InitialWorkDirRequirement e ->
                    Expect.sequenceEqual a e "InitialWorkDirRequirement mismatch: Type of Decode Json Syntax for InitialWorkDirRequirement"
                | _ ->
                    failwith "Wrong requirement type: Type of Decode Json Syntax for InitialWorkDirRequirement can only be InitialWorkDirRequirement"
            testCase "String listing entries decode" <| fun _ ->
                let yaml = """requirements:
  - class: InitialWorkDirRequirement
    listing:
      - $(inputs.stageDirectory)
      - $(inputs.outputDirectory)"""
                let reqs = decodeRequirements yaml
                let initialWorkDirItem = findRequirement reqs (function InitialWorkDirRequirement _ -> true | _ -> false)
                match initialWorkDirItem with
                | InitialWorkDirRequirement listing ->
                    let expected =
                        ResizeArray [|
                            StringEntry (SchemaSaladString.Literal "$(inputs.stageDirectory)")
                            StringEntry (SchemaSaladString.Literal "$(inputs.outputDirectory)")
                        |]
                    Expect.sequenceEqual listing expected "String listing entries should decode into StringEntry values."
                | _ ->
                    failwith "Wrong requirement type: expected InitialWorkDirRequirement"
            testCase "Scalar listing form decodes as the expression form of the requirement" <| fun _ ->
                let yaml = """requirements:
  - class: InitialWorkDirRequirement
    listing: $(inputs.stageDirectory)"""
                let reqs = decodeRequirements yaml
                let initialWorkDirItem = findRequirement reqs (function InitialWorkDirExpressionRequirement _ -> true | _ -> false)
                Expect.equal initialWorkDirItem (InitialWorkDirExpressionRequirement (SchemaSaladString.Literal "$(inputs.stageDirectory)")) "Scalar listing form should decode to the expression."
            testCase "Object listing entry without entry field decodes as StringEntry" <| fun _ ->
                let yaml = """requirements:
  - class: InitialWorkDirRequirement
    listing:
      - $include: scripts/dirent.js"""
                let reqs = decodeRequirements yaml
                let initialWorkDirItem = findRequirement reqs (function InitialWorkDirRequirement _ -> true | _ -> false)
                match initialWorkDirItem with
                | InitialWorkDirRequirement listing ->
                    let expected = ResizeArray [| StringEntry (SchemaSaladString.Include "scripts/dirent.js") |]
                    Expect.sequenceEqual listing expected "Object entries without an `entry` field should decode as StringEntry."
                | _ ->
                    failwith "Wrong requirement type: expected InitialWorkDirRequirement"
            testCase "Object listing entry with $import decodes as StringEntry Import" <| fun _ ->
                let yaml = """requirements:
  - class: InitialWorkDirRequirement
    listing:
      - $import: scripts/dirent.js"""
                let reqs = decodeRequirements yaml
                let initialWorkDirItem = findRequirement reqs (function InitialWorkDirRequirement _ -> true | _ -> false)
                match initialWorkDirItem with
                | InitialWorkDirRequirement listing ->
                    let expected = ResizeArray [| StringEntry (SchemaSaladString.Import "scripts/dirent.js") |]
                    Expect.sequenceEqual listing expected "Object entries with $import should decode as StringEntry Import."
                | _ ->
                    failwith "Wrong requirement type: expected InitialWorkDirRequirement"
            testCase "Dirent entry preserves include wrapper when provided as mapping" <| fun _ ->
                let yaml = """requirements:
  - class: InitialWorkDirRequirement
    listing:
      - entryname: script-name.txt
        entry:
          $include: scripts/bootstrap.sh"""
                let reqs = decodeRequirements yaml
                let initialWorkDirItem = findRequirement reqs (function InitialWorkDirRequirement _ -> true | _ -> false)
                match initialWorkDirItem with
                | InitialWorkDirRequirement listing ->
                    let expected =
                        ResizeArray [|
                            DirentEntry (DirentInstance(SchemaSaladString.Include "scripts/bootstrap.sh", entryname = SchemaSaladString.Literal "script-name.txt"))
                        |]
                    Expect.sequenceEqual listing expected "Dirent entry include wrapper should preserve directive kind."
                | _ ->
                    failwith "Wrong requirement type: expected InitialWorkDirRequirement"

            testCase "A File or a Directory with the fields of a Dirent is a decode error, as cwltool rejects it" <| fun _ ->
                let listing (items: string list) = "requirements:\n  - class: InitialWorkDirRequirement\n    listing:\n" + String.concat "\n" (items |> List.map (fun item -> "      " + item))
                let cases =
                    [ "a File with entry", listing [ "- class: File"; "  location: a.txt"; "  entry: $(inputs.arcDirectory)" ], "requirements[0].listing[0].entry", "A File has no field entry."
                      "a File with writable", listing [ "- class: File"; "  location: a.txt"; "  writable: true" ], "requirements[0].listing[0].writable", "A File has no field writable."
                      "a File with entryname", listing [ "- {class: File, location: a.txt, entryname: b}" ], "requirements[0].listing[0].entryname", "A File has no field entryname."
                      "a Directory with entry", listing [ "- class: Directory"; "  location: d"; "  entry: b" ], "requirements[0].listing[0].entry", "A Directory has no field entry."
                      "a File with entry in an array", listing [ "- - class: File"; "    location: a.txt"; "    entry: b" ], "requirements[0].listing[0][0].entry", "A File has no field entry." ]
                for (name, yaml, path, message) in cases do
                    match decodeError (fun () -> decodeRequirements yaml |> ignore) with
                    | Some error ->
                        Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the field" name)
                        Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
                        Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
                    | None -> failwith (sprintf "%s must fail" name)
                let dirent = decodeRequirements (listing [ "- entryname: a.txt"; "  entry: $(1)"; "  writable: true"; "- class: File"; "  location: a.txt" ])
                match findRequirement dirent (function InitialWorkDirRequirement _ -> true | _ -> false) with
                | InitialWorkDirRequirement entries ->
                    Expect.isTrue (match entries.[0], entries.[1] with | DirentEntry _, FileEntry _ -> true | _ -> false) "An object without a class is still a Dirent, and a File without these fields is a File."
                | other -> failwithf "Expected an InitialWorkDirRequirement, found %A" other
        ]
        testList "EnvVarRequirement" [
            testCase "Class Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsClassFileContent
                let envVarItem = findRequirement reqs (function EnvVarRequirement _ -> true | _ -> false)
                let expected = EnvVarRequirement (ListEntry.ofValues [| EnvironmentDef(SchemaSaladString.Literal "DOTNET_NOLOGO", SchemaSaladString.Literal "true"); EnvironmentDef(SchemaSaladString.Literal "TEST", SchemaSaladString.Literal "false") |])
                let actual = envVarItem
                match actual, expected with
                | EnvVarRequirement a, EnvVarRequirement e ->
                    Expect.sequenceEqual a e "EnvVarRequirement mismatch: Type of Decode Class Syntax for EnvVarRequirement"
                | _ ->
                    failwith "Wrong requirement type: Type of Decode Class Syntax for EnvVarRequirement can only be EnvVarRequirement"
            testCase "Mapping Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsMappingFileContent
                let envVarItem = findRequirement reqs (function EnvVarRequirement _ -> true | _ -> false)
                let expected = EnvVarRequirement (ListEntry.ofValues [| EnvironmentDef(SchemaSaladString.Literal "DOTNET_NOLOGO", SchemaSaladString.Literal "true"); EnvironmentDef(SchemaSaladString.Literal "TEST", SchemaSaladString.Literal "false") |])
                let actual = envVarItem
                match actual, expected with
                | EnvVarRequirement a, EnvVarRequirement e ->
                    Expect.sequenceEqual a e "EnvVarRequirement mismatch: Type of Decode Mapping Syntax for EnvVarRequirement"
                | _ ->
                    failwith "Wrong requirement type: Type of Decode Mapping Syntax for EnvVarRequirement can only be EnvVarRequirement"
            testCase "Json Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsJSONFileContent
                let envVarItem = findRequirement reqs (function EnvVarRequirement _ -> true | _ -> false)
                let expected = EnvVarRequirement (ListEntry.ofValues [| EnvironmentDef(SchemaSaladString.Literal "DOTNET_NOLOGO", SchemaSaladString.Literal "true"); EnvironmentDef(SchemaSaladString.Literal "TEST", SchemaSaladString.Literal "false") |])
                let actual = envVarItem
                match actual, expected with
                | EnvVarRequirement a, EnvVarRequirement e ->
                    Expect.sequenceEqual a e "EnvVarRequirement mismatch: Type of Decode Json Syntax for EnvVarRequirement"
                | _ ->
                    failwith "Wrong requirement type: Type of Decode Json Syntax for EnvVarRequirement can only be EnvVarRequirement"
            testCase "Map shorthand decodes to EnvironmentDef list" <| fun _ ->
                let yaml = """requirements:
  - class: EnvVarRequirement
    envDef:
      DOTNET_NOLOGO: "true"
      TEST: "false"
"""
                let reqs = decodeRequirements yaml
                let envVarItem = findRequirement reqs (function EnvVarRequirement _ -> true | _ -> false)
                match envVarItem with
                | EnvVarRequirement envs ->
                    let expected = ResizeArray [| EnvironmentDef(SchemaSaladString.Literal "DOTNET_NOLOGO", SchemaSaladString.Literal "true"); EnvironmentDef(SchemaSaladString.Literal "TEST", SchemaSaladString.Literal "false") |]
                    Expect.sequenceEqual (ListEntry.values envs) expected "EnvVar map shorthand should decode to normalized EnvironmentDef list."
                | _ ->
                    failwith "Wrong requirement type: expected EnvVarRequirement"

            testCase "Map shorthand unquoted boolean is a decode error and a quoted one is a string" <| fun _ ->
                let plain = """requirements:
  - class: EnvVarRequirement
    envDef:
      FLAG: true"""
                Expect.throws (fun _ -> decodeRequirements plain |> ignore) "cwltool rejects a plain bool for the string field envValue."
                let yaml = """requirements:
  - class: EnvVarRequirement
    envDef:
      FLAG: 'true'"""
                let reqs = decodeRequirements yaml
                let envVarItem = findRequirement reqs (function EnvVarRequirement _ -> true | _ -> false)
                match envVarItem with
                | EnvVarRequirement envs ->
                    Expect.equal envs.Count 1 "Expected one map-shorthand environment definition"
                    Expect.equal (ListEntry.values envs).[0].EnvName (SchemaSaladString.Literal "FLAG") "Map key should decode as env name"
                    Expect.equal (ListEntry.values envs).[0].EnvValue (SchemaSaladString.Literal "true") "A quoted YAML boolean should decode to a string value"
                | _ ->
                    failwith "Wrong requirement type: expected EnvVarRequirement"

            testCase "Map shorthand decode then default encode uses array form" <| fun _ ->
                let yaml = """requirements:
  - class: EnvVarRequirement
    envDef:
      DOTNET_NOLOGO: "true"
      TEST: "false"
"""
                let reqs = decodeRequirements yaml
                let envVarItem = findRequirement reqs (function EnvVarRequirement _ -> true | _ -> false)
                let encoded = Encode.encodeRequirement envVarItem |> Encode.writeYaml
                Expect.stringContains encoded "envDef:" "Encoded requirement should include envDef"
                Expect.stringContains encoded "envName: DOTNET_NOLOGO" "Default encoder should emit array-form envDef entries"
                Expect.stringContains encoded "envValue: \"true\"" "Boolean-like strings should remain quoted in default array encoder"
                Expect.stringContains encoded "envValue: \"false\"" "The second value is a string too and must stay quoted"
                Expect.isFalse (encoded.Contains("\\\"")) "The quotes belong to the YAML style, so no escaped quote may appear in the output"

            testCase "Array form roundtrips values that look like another type" <| fun _ ->
                let envs =
                    ResizeArray [|
                        EnvironmentDef(SchemaSaladString.Literal "A", SchemaSaladString.Literal "true")
                        EnvironmentDef(SchemaSaladString.Literal "B", SchemaSaladString.Literal "1.0")
                        EnvironmentDef(SchemaSaladString.Literal "C", SchemaSaladString.Literal "null")
                        EnvironmentDef(SchemaSaladString.Literal "D", SchemaSaladString.Literal "0x1F")
                        EnvironmentDef(SchemaSaladString.Literal "E", SchemaSaladString.Literal "plain")
                        EnvironmentDef(SchemaSaladString.Literal "F", SchemaSaladString.Literal "\"inner quotes\"")
                    |]
                let yaml =
                    YAMLicious.Encode.object [ "requirements", YamlNode.sequence [ YamlNode.item (Encode.encodeRequirement (EnvVarRequirement (ListEntry.ofValues envs))) ] ]
                    |> Encode.writeYaml
                Expect.stringContains yaml "envValue: \"1.0\"" "A string that reads as a float needs quotes."
                Expect.stringContains yaml "envValue: \"null\"" "A string that reads as null needs quotes."
                Expect.stringContains yaml "envValue: \"0x1F\"" "A string that reads as a hex int needs quotes."
                Expect.stringContains yaml "envValue: plain" "A string that cannot be mistaken for another type stays plain."
                let reqs = decodeRequirements yaml
                match findRequirement reqs (function EnvVarRequirement _ -> true | _ -> false) with
                | EnvVarRequirement decoded ->
                    Expect.sequenceEqual (ListEntry.values decoded) envs "Env values must survive the roundtrip unchanged, including literal quote characters."
                | _ ->
                    failwith "Wrong requirement type: expected EnvVarRequirement"
        ]
        testList "SoftwareRequirement" [
            testCase "Class Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsClassFileContent
                let softwareItem = findRequirement reqs (function SoftwareRequirement _ -> true | _ -> false)
                let expected = SoftwareRequirement (ListEntry.ofValues [| SoftwarePackage(SchemaSaladString.Literal "interproscan", version = literals [ "5.21-60" ], specs = literals [ "https://identifiers.org/rrid/RRID:SCR_005829" ]) |])
                let actual = softwareItem
                match actual, expected with
                | SoftwareRequirement actualType, SoftwareRequirement expectedType ->
                    let a = (ListEntry.values actualType).[0]
                    let e = (ListEntry.values expectedType).[0]
                    Expect.equal a.Package e.Package $"SoftwareRequirement.Package mismatch. expected = '{e.Package}', actual = '{a.Package}'"
                    Expect.sequenceEqual a.Specs.Value e.Specs.Value $"SoftwareRequirement.Specs mismatch. expected = {e.Specs.Value}, actual = {a.Specs.Value}"
                    Expect.sequenceEqual a.Version.Value e.Version.Value $"SoftwareRequirement.Version mismatch. expected = {e.Version.Value}, actual = {a.Version.Value}"
                | _ ->
                    failwith "Wrong requirement type: Type of Decode Class Syntax for SoftwareRequirement can only be SoftwareRequirement"
            testCase "Mapping Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsMappingFileContent
                let softwareItem = findRequirement reqs (function SoftwareRequirement _ -> true | _ -> false)
                let expected = SoftwareRequirement (ListEntry.ofValues [| SoftwarePackage(SchemaSaladString.Literal "interproscan", version = literals [ "5.21-60" ], specs = literals [ "https://identifiers.org/rrid/RRID:SCR_005829" ]) |])
                let actual = softwareItem
                match actual, expected with
                | SoftwareRequirement actualType, SoftwareRequirement expectedType ->
                    let a = (ListEntry.values actualType).[0]
                    let e = (ListEntry.values expectedType).[0]
                    Expect.equal a.Package e.Package $"SoftwareRequirement.Package mismatch. expected = '{e.Package}', actual = '{a.Package}'"
                    Expect.sequenceEqual a.Specs.Value e.Specs.Value $"SoftwareRequirement.Specs mismatch. expected = {e.Specs.Value}, actual = {a.Specs.Value}"
                    Expect.sequenceEqual a.Version.Value e.Version.Value $"SoftwareRequirement.Version mismatch. expected = {e.Version.Value}, actual = {a.Version.Value}"
                | _ ->
                    failwith "Wrong requirement type: Type of Decode Mapping Syntax for SoftwareRequirement can only be SoftwareRequirement"
            testCase "Json Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsJSONFileContent
                let softwareItem = findRequirement reqs (function SoftwareRequirement _ -> true | _ -> false)
                let expected = SoftwareRequirement (ListEntry.ofValues [| SoftwarePackage(SchemaSaladString.Literal "interproscan", version = literals [ "5.21-60" ], specs = literals [ "https://identifiers.org/rrid/RRID:SCR_005829" ]) |])
                let actual = softwareItem
                match actual, expected with
                | SoftwareRequirement actualType, SoftwareRequirement expectedType ->
                    let a = (ListEntry.values actualType).[0]
                    let e = (ListEntry.values expectedType).[0]
                    Expect.equal a.Package e.Package $"SoftwareRequirement.Package mismatch. expected = '{e.Package}', actual = '{a.Package}'"
                    Expect.sequenceEqual a.Specs.Value e.Specs.Value $"SoftwareRequirement.Specs mismatch. expected = {e.Specs.Value}, actual = {a.Specs.Value}"
                    Expect.sequenceEqual a.Version.Value e.Version.Value $"SoftwareRequirement.Version mismatch. expected = {e.Version.Value}, actual = {a.Version.Value}"
                | _ ->
                    failwith "Wrong requirement type: Type of Decode Json Syntax for SoftwareRequirement can only be SoftwareRequirement"
            testCase "Map shorthand decodes package specs list" <| fun _ ->
                let yaml = """requirements:
  - class: SoftwareRequirement
    packages:
      blast:
        - https://example.org/blast-spec-1
        - https://example.org/blast-spec-2"""
                let reqs = decodeRequirements yaml
                let softwareItem = findRequirement reqs (function SoftwareRequirement _ -> true | _ -> false)
                match softwareItem with
                | SoftwareRequirement entries ->
                    let packages = ListEntry.values entries
                    Expect.equal packages.Count 1 "One package should decode from shorthand map"
                    Expect.equal packages.[0].Package (SchemaSaladString.Literal "blast") "Package name should decode from map key"
                    Expect.isSome packages.[0].Specs "Specs should decode from map sequence"
                    Expect.equal packages.[0].Specs.Value.Count 2 "Specs shorthand sequence should be preserved"
                | _ ->
                    failwith "Wrong requirement type: expected SoftwareRequirement"
            testCase "Map shorthand decodes package object form" <| fun _ ->
                let yaml = """requirements:
  - class: SoftwareRequirement
    packages:
      interproscan:
        specs:
          - https://identifiers.org/rrid/RRID:SCR_005829
        version:
          - 5.21-60"""
                let reqs = decodeRequirements yaml
                let softwareItem = findRequirement reqs (function SoftwareRequirement _ -> true | _ -> false)
                match softwareItem with
                | SoftwareRequirement entries ->
                    let packages = ListEntry.values entries
                    Expect.equal packages.Count 1 "One package should decode from object map form"
                    let package = packages.[0]
                    Expect.equal package.Package (SchemaSaladString.Literal "interproscan") "Package should decode from map key"
                    Expect.equal package.Specs.Value.Count 1 "Specs should decode from object map"
                    Expect.equal package.Version.Value.Count 1 "Version should decode from object map"
                | _ ->
                    failwith "Wrong requirement type: expected SoftwareRequirement"

            testCase "Map shorthand decode then default encode uses array form" <| fun _ ->
                let yaml = """requirements:
  - class: SoftwareRequirement
    packages:
      blast:
        - https://example.org/blast-spec-1
        - https://example.org/blast-spec-2"""
                let reqs = decodeRequirements yaml
                let softwareItem = findRequirement reqs (function SoftwareRequirement _ -> true | _ -> false)
                let encoded = Encode.encodeRequirement softwareItem |> Encode.writeYaml
                Expect.stringContains encoded "packages:" "Encoded requirement should include packages"
                Expect.stringContains encoded "package: blast" "Default encoder should emit array-form package entries"
                Expect.stringContains encoded "specs:" "Specs should remain present after map-decode array-encode"

            testCase "Map shorthand decodes version-only object form" <| fun _ ->
                let yaml = """requirements:
  - class: SoftwareRequirement
    packages:
      interproscan:
        version:
          - 5.21-60"""
                let reqs = decodeRequirements yaml
                let softwareItem = findRequirement reqs (function SoftwareRequirement _ -> true | _ -> false)
                match softwareItem with
                | SoftwareRequirement entries ->
                    let packages = ListEntry.values entries
                    Expect.equal packages.Count 1 "One package should decode from version-only map form"
                    let package = packages.[0]
                    Expect.equal package.Package (SchemaSaladString.Literal "interproscan") "Package should decode from map key"
                    Expect.isSome package.Version "Version should decode from map object"
                    Expect.isNone package.Specs "Specs should be absent for version-only map object"
                | _ ->
                    failwith "Wrong requirement type: expected SoftwareRequirement"
        ]
        testList "NetworkAccess" [
            testCase "Class Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsClassFileContent
                let networkAccessItem = findRequirement reqs (function NetworkAccessRequirement _ -> true | _ -> false)
                let expected = NetworkAccessRequirement (NetworkAccessRequirementValue(true))
                let actual = networkAccessItem
                Expect.equal actual expected "Type of Decode Classs Syntax for NetworkAccess, Requirement can only be NetworkAccess"
            testCase "Mapping Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsMappingFileContent
                let networkAccessItem = findRequirement reqs (function NetworkAccessRequirement _ -> true | _ -> false)
                let expected = NetworkAccessRequirement (NetworkAccessRequirementValue(true))
                let actual = networkAccessItem
                Expect.equal actual expected "Type of Decode Mapping Syntax for NetworkAccess, Requirement can only be NetworkAccess"
            testCase "Json Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsJSONFileContent
                let networkAccessItem = findRequirement reqs (function NetworkAccessRequirement _ -> true | _ -> false)
                let expected = NetworkAccessRequirement (NetworkAccessRequirementValue(true))
                let actual = networkAccessItem
                Expect.equal actual expected "Type of Decode Json Syntax for NetworkAccess, Requirement can only be NetworkAccess"
        ]
        testList "DockerRequirement Canonical" [
            testCase "Decode canonical docker string and extended fields" <| fun _ ->
                let yaml = """requirements:
  - class: DockerRequirement
    dockerPull: ghcr.io/example/tool:1.0.0
    dockerFile: ./Dockerfile
    dockerImageId: tool-image
    dockerLoad: docker-archive:///tmp/tool.tar
    dockerImport: https://example.org/images/tool.sif
    dockerOutputDirectory: /work/out"""
                let reqs = decodeRequirements yaml
                let dockerItem = findRequirement reqs (function DockerRequirement _ -> true | _ -> false)
                let expected =
                    Requirement.DockerRequirement (
                        DockerRequirement.create(
                            dockerPull = (SchemaSaladString.Literal "ghcr.io/example/tool:1.0.0"),
                            dockerFileReference = SchemaSaladString.Literal "./Dockerfile",
                            dockerImageId = (SchemaSaladString.Literal "tool-image"),
                            dockerLoad = (SchemaSaladString.Literal "docker-archive:///tmp/tool.tar"),
                            dockerImport = (SchemaSaladString.Literal "https://example.org/images/tool.sif"),
                            dockerOutputDirectory = (SchemaSaladString.Literal "/work/out")
                        )
                    )
                Expect.equal dockerItem expected "Canonical docker fields should decode into the typed DockerRequirement model."
            testCase "Decode dockerFile $import wrapper and preserve directive kind" <| fun _ ->
                let yaml = """requirements:
  - class: DockerRequirement
    dockerFile:
      $import: ./Dockerfile"""
                let reqs = decodeRequirements yaml
                let dockerItem = findRequirement reqs (function DockerRequirement _ -> true | _ -> false)
                let expected = Requirement.DockerRequirement (DockerRequirement.create(dockerFileReference = SchemaSaladString.Import "./Dockerfile"))
                Expect.equal dockerItem expected "dockerFile $import wrapper should decode into Import and survive type mapping."
            testCase "Decode dockerFile map with both $include and $import fails" <| fun _ ->
                let yaml = """requirements:
  - class: DockerRequirement
    dockerFile:
      $include: ./Dockerfile.include
      $import: ./Dockerfile.import"""
                let decodeInvalid () =
                    decodeRequirements yaml
                    |> ignore
                Expect.throws decodeInvalid "dockerFile maps that specify both $include and $import should fail decoding."
        ]
        testList "LoadListingRequirement" [
            testCase "Class Syntax" <| fun _ ->
                let yaml = """requirements:
  - class: LoadListingRequirement
    loadListing: deep_listing"""
                let reqs = decodeRequirements yaml
                let requirement = findRequirement reqs (function LoadListingRequirement _ -> true | _ -> false)
                let expected = LoadListingRequirement (LoadListingRequirementValue(DeepListing))
                Expect.equal requirement expected "Class-array syntax should decode LoadListingRequirement payload."
            testCase "Mapping Syntax" <| fun _ ->
                let yaml = """requirements:
  LoadListingRequirement:
    loadListing: shallow_listing"""
                let reqs = decodeRequirements yaml
                let requirement = findRequirement reqs (function LoadListingRequirement _ -> true | _ -> false)
                let expected = LoadListingRequirement (LoadListingRequirementValue(ShallowListing))
                Expect.equal requirement expected "Mapping syntax should decode LoadListingRequirement payload."
            testCase "Json Syntax" <| fun _ ->
                let yaml = """requirements: { LoadListingRequirement: { loadListing: "no_listing" } }"""
                let reqs = decodeRequirements yaml
                let requirement = findRequirement reqs (function LoadListingRequirement _ -> true | _ -> false)
                let expected = LoadListingRequirement (LoadListingRequirementValue(NoListing))
                Expect.equal requirement expected "JSON object syntax should decode LoadListingRequirement payload."
            testCase "Default value when field omitted" <| fun _ ->
                let yaml = """requirements:
  - class: LoadListingRequirement"""
                let reqs = decodeRequirements yaml
                let requirement = findRequirement reqs (function LoadListingRequirement _ -> true | _ -> false)
                let expected = LoadListingRequirement (LoadListingRequirementValue(NoListing))
                Expect.equal requirement expected "Missing loadListing should default to no_listing."
            testCase "Invalid value fails clearly" <| fun _ ->
                let yaml = """requirements:
  - class: LoadListingRequirement
    loadListing: invalid_listing"""
                Expect.throws
                    (fun _ -> decodeRequirements yaml |> ignore)
                    "Invalid loadListing symbols should fail during decode."

            testCase "Case-sensitive value fails clearly" <| fun _ ->
                let yaml = """requirements:
  - class: LoadListingRequirement
    loadListing: No_Listing"""
                Expect.throws
                    (fun _ -> decodeRequirements yaml |> ignore)
                    "Non-canonical case should fail for loadListing values."

            testCase "Encode canonical loadListing symbol" <| fun _ ->
                let requirement = LoadListingRequirement (LoadListingRequirementValue(ShallowListing))
                let encoded = Encode.encodeRequirement requirement |> Encode.writeYaml
                Expect.stringContains encoded "loadListing: shallow_listing" "Enum should encode to canonical CWL symbol."
        ]
        testList "Payloaded requirement defaults and custom values" [
            testCase "WorkReuse defaults to true when payload omitted" <| fun _ ->
                let yaml = """requirements:
  - class: WorkReuse"""
                let workReuse = findRequirement (decodeRequirements yaml) (function WorkReuseRequirement _ -> true | _ -> false)
                Expect.equal workReuse (WorkReuseRequirement (WorkReuseRequirementValue(true))) "WorkReuse without explicit payload should default to true."
            testCase "NetworkAccess and InplaceUpdateRequirement without their field are decode errors and never get a true" <| fun _ ->
                for className, field in [ "NetworkAccess", "networkAccess"; "InplaceUpdateRequirement", "inplaceUpdate" ] do
                    for form in [ "requirements"; "hints" ] do
                        let yaml = form + ":\n  - class: " + className
                        match decodeError (fun () -> (if form = "hints" then decodeHints yaml |> ignore else decodeRequirements yaml |> ignore)) with
                        | Some error ->
                            Expect.equal (DecodeError.pathText error) (form + "[0]") (sprintf "%s in %s: the error shows the requirement" className form)
                            Expect.stringContains error.Message ("no field " + field) (sprintf "%s in %s: the error names the field" className form)
                            Expect.isSome error.Span (sprintf "%s in %s: the error has the place in the text" className form)
                        | None -> failwith (sprintf "%s without %s must fail in %s" className field form)
                let falseNetwork = (decodeRequirements "requirements:\n  - class: NetworkAccess\n    networkAccess: false").[0]
                Expect.stringContains (Encode.encodeRequirement falseNetwork |> Encode.writeYaml) "networkAccess: false" "A false stays false."
            testCase "WorkReuse, NetworkAccess, InplaceUpdate decode explicit false payloads" <| fun _ ->
                let yaml = """requirements:
  - class: WorkReuse
    enableReuse: false
  - class: NetworkAccess
    networkAccess: false
  - class: InplaceUpdateRequirement
    inplaceUpdate: false"""
                let reqs = decodeRequirements yaml
                let workReuse = findRequirement reqs (function WorkReuseRequirement _ -> true | _ -> false)
                let network = findRequirement reqs (function NetworkAccessRequirement _ -> true | _ -> false)
                let inplace = findRequirement reqs (function InplaceUpdateRequirement _ -> true | _ -> false)
                Expect.equal workReuse (WorkReuseRequirement (WorkReuseRequirementValue(false))) "WorkReuse enableReuse=false should decode."
                Expect.equal network (NetworkAccessRequirement (NetworkAccessRequirementValue(false))) "NetworkAccess networkAccess=false should decode."
                Expect.equal inplace (InplaceUpdateRequirement (InplaceUpdateRequirementValue(SchemaSaladBool.Value false))) "InplaceUpdateRequirement inplaceUpdate=false should decode."
            testCase "WorkReuse and NetworkAccess accept expression payloads" <| fun _ ->
                let yaml = """requirements:
  - class: WorkReuse
    enableReuse: $(inputs.enable_reuse)
  - class: NetworkAccess
    networkAccess: $(inputs.enable_network)"""
                let reqs = decodeRequirements yaml
                let encoded =
                    reqs
                    |> Seq.map (fun requirement -> Encode.encodeRequirement requirement |> Encode.writeYaml)
                    |> String.concat "\n"
                Expect.stringContains encoded "$(inputs.enable_reuse)" "WorkReuse expression payload should survive decode/encode."
                Expect.stringContains encoded "$(inputs.enable_network)" "NetworkAccess expression payload should survive decode/encode."
            testCase "A quoted bool of WorkReuse and NetworkAccess is a text without an expression and a decode error" <| fun _ ->
                for (cls, field, text) in [ "WorkReuse", "enableReuse", "\"false\""; "NetworkAccess", "networkAccess", "'true'" ] do
                    let yaml = "requirements:\n  - class: " + cls + "\n    " + field + ": " + text
                    Expect.throws (fun _ -> decodeRequirements yaml |> ignore) (sprintf "The quoted %s of %s holds no expression." field cls)
            testCase "An expression for WorkReuse and NetworkAccess is a string that reads back as the same expression" <| fun _ ->
                let yaml = """requirements:
  - class: WorkReuse
    enableReuse: "$(inputs.reuse)"
  - class: NetworkAccess
    networkAccess: '$(inputs.network)'"""
                let reqs = decodeRequirements yaml
                let workReuse = findRequirement reqs (function WorkReuseExpressionRequirement _ -> true | _ -> false)
                let network = findRequirement reqs (function NetworkAccessExpressionRequirement _ -> true | _ -> false)
                Expect.equal workReuse (WorkReuseExpressionRequirement (SchemaSaladString.Literal "$(inputs.reuse)")) "The expression is the text."
                Expect.equal network (NetworkAccessExpressionRequirement (SchemaSaladString.Literal "$(inputs.network)")) "The expression is the text."
                let encoded = Encode.encodeRequirement workReuse |> Encode.writeYaml
                Expect.equal (findRequirement (decodeRequirements ("requirements:\n  - " + encoded.Replace("\n", "\n    ").TrimEnd())) (function WorkReuseExpressionRequirement _ -> true | _ -> false)) workReuse "The written expression reads back as the same requirement."
            testCase "WorkReuse and NetworkAccess decode every core schema spelling of a bool" <| fun _ ->
                let yaml = """requirements:
  - class: WorkReuse
    enableReuse: False
  - class: NetworkAccess
    networkAccess: TRUE"""
                let reqs = decodeRequirements yaml
                let workReuse = findRequirement reqs (function WorkReuseRequirement _ -> true | _ -> false)
                let network = findRequirement reqs (function NetworkAccessRequirement _ -> true | _ -> false)
                Expect.equal workReuse (WorkReuseRequirement (WorkReuseRequirementValue(false))) "False is a bool in the core schema."
                Expect.equal network (NetworkAccessRequirement (NetworkAccessRequirementValue(true))) "TRUE is a bool in the core schema."
        ]
        testList "ToolTimeLimitRequirement" [
            testCase "Decode numeric timelimit" <| fun _ ->
                let yaml = """requirements:
  - class: ToolTimeLimit
    timelimit: 120"""
                let reqs = decodeRequirements yaml
                let requirement = findRequirement reqs (function ToolTimeLimitRequirement _ -> true | _ -> false)
                let expected = ToolTimeLimitRequirement (ToolTimeLimitSeconds 120L)
                Expect.equal requirement expected "Numeric timelimit should decode to ToolTimeLimitSeconds."
            testCase "Decode long timelimit" <| fun _ ->
                let yaml = """requirements:
  - class: ToolTimeLimit
    timelimit: 922337203685477580"""
                let reqs = decodeRequirements yaml
                let requirement = findRequirement reqs (function ToolTimeLimitRequirement _ -> true | _ -> false)
                let expected = ToolTimeLimitRequirement (ToolTimeLimitSeconds 922337203685477580L)
                Expect.equal requirement expected "Long timelimit should decode to ToolTimeLimitSeconds int64."
            testCase "Decode expression timelimit" <| fun _ ->
                let yaml = """requirements:
  - class: ToolTimeLimit
    timelimit: $(inputs.max_runtime_seconds)"""
                let reqs = decodeRequirements yaml
                let requirement = findRequirement reqs (function ToolTimeLimitRequirement _ -> true | _ -> false)
                let expected = ToolTimeLimitRequirement (ToolTimeLimitExpression (SchemaSaladString.Literal "$(inputs.max_runtime_seconds)"))
                Expect.equal requirement expected "Expression timelimit should decode to ToolTimeLimitExpression."
            testCase "Encode numeric timelimit as integer scalar" <| fun _ ->
                let requirement = ToolTimeLimitRequirement (ToolTimeLimitSeconds 300L)
                let encoded = Encode.encodeRequirement requirement |> Encode.writeYaml
                Expect.stringContains encoded "timelimit: 300" "Numeric timelimit should encode as integer scalar"

            testCase "A float timelimit is a decode error because CWL types it as long or string" <| fun _ ->
                let yaml = """requirements:
  - class: ToolTimeLimit
    timelimit: 3.5"""
                Expect.throws (fun _ -> decodeRequirements yaml |> ignore) "cwltool rejects a float for the long or expression timelimit."
            testCase "A plain bool timelimit is a decode error because CWL types it as long or string" <| fun _ ->
                for text in [ "true"; "false" ] do
                    let yaml = "requirements:\n  - class: ToolTimeLimit\n    timelimit: " + text
                    Expect.throws (fun _ -> decodeRequirements yaml |> ignore) (sprintf "A bool is no number of seconds (%s)." text)
            testCase "A quoted number timelimit is a text without an expression and a decode error, and an expression survives a write and a read" <| fun _ ->
                for text in [ "\"5\""; "\"3.5\"" ] do
                    let yaml = "requirements:\n  - class: ToolTimeLimit\n    timelimit: " + text
                    Expect.throws (fun _ -> decodeRequirements yaml |> ignore) (sprintf "The quoted time limit %s holds no expression." text)
                for text in [ "\"$(5)\""; "'${return 5;}'" ] do
                    let yaml = "requirements:\n  - class: ToolTimeLimit\n    timelimit: " + text
                    let requirement = findRequirement (decodeRequirements yaml) (function ToolTimeLimitRequirement _ -> true | _ -> false)
                    let encoded = Encode.encodeRequirement requirement |> Encode.writeYaml
                    let writtenLine = encoded.Split('\n') |> Array.find (fun line -> line.StartsWith "timelimit:")
                    let again = findRequirement (decodeRequirements ("requirements:\n  - class: ToolTimeLimit\n    " + writtenLine)) (function ToolTimeLimitRequirement _ -> true | _ -> false)
                    Expect.equal again requirement (sprintf "The time limit %s should read back as the same requirement." text)
            testCase "A negative numeric timelimit decodes with a warning, as a requirement and as a hint" <| fun _ ->
                for (field, path) in [ "requirements", "requirements[0].timelimit"; "hints", "hints[0].timelimit" ] do
                    let yaml = "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\n" + field + ":\n  - class: ToolTimeLimit\n    timelimit: -1\ninputs: {}\noutputs: {}\n"
                    let decoded = Decode.decodeCommandLineToolWithWarnings yaml
                    let requirement =
                        match field with
                        | "requirements" -> decoded.Value.Requirements.Value.[0]
                        | _ -> (HintEntry.tryAsRequirement decoded.Value.Hints.Value.[0]).Value
                    Expect.equal requirement (ToolTimeLimitRequirement (ToolTimeLimitSeconds -1L)) $"A negative timelimit in {field} is kept as it is."
                    Expect.equal (decoded.Warnings |> Seq.map (fun warning -> warning.Path) |> List.ofSeq) [ path ] $"The warning for {field} shows the timelimit."
                    Expect.stringContains decoded.Warnings.[0].Message "negative timelimit" "The warning says what is wrong."
                    Expect.isSome decoded.Warnings.[0].Raw "The warning shows the requirement."
                    Expect.equal (Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded.Value)) decoded.Value $"The written {field} should decode to the same tool."
        ]
        testList "ResourceRequirement" [
            testCase "A negative resource value decodes with a warning at the field, as a requirement and as a hint" <| fun _ ->
                for (field, prefix) in [ "requirements", "requirements[0]"; "hints", "hints[0]" ] do
                    for (name, value) in [ "coresMin", "-5"; "coresMax", "-1"; "ramMin", "-5.5"; "ramMax", "-2"; "tmpdirMin", "-3"; "tmpdirMax", "-4"; "outdirMin", "-6"; "outdirMax", "-7" ] do
                        let yaml = "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\n" + field + ":\n  - class: ResourceRequirement\n    " + name + ": " + value + "\ninputs: {}\noutputs: {}\n"
                        let decoded = Decode.decodeCommandLineToolWithWarnings yaml
                        Expect.equal (decoded.Warnings |> Seq.map (fun warning -> warning.Path) |> List.ofSeq) [ prefix + "." + name ] $"The warning for {name} in {field} shows the field."
                        Expect.stringContains decoded.Warnings.[0].Message ("negative " + name) "The warning says what is wrong."
                        Expect.isSome decoded.Warnings.[0].Raw "The warning shows the requirement."
                        Expect.equal (Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded.Value)) decoded.Value $"The written {name} should decode to the same tool."
            testCase "A maximum below its minimum decodes with a warning at the maximum, as a requirement and as a hint" <| fun _ ->
                for (field, prefix) in [ "requirements", "requirements[0]"; "hints", "hints[0]" ] do
                    for (name, minimum, maximum) in [ "cores", "4", "2"; "ram", "4.5", "2"; "tmpdir", "4", "2.5"; "outdir", "1024", "512" ] do
                        let yaml = "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\n" + field + ":\n  - class: ResourceRequirement\n    " + name + "Min: " + minimum + "\n    " + name + "Max: " + maximum + "\ninputs: {}\noutputs: {}\n"
                        let decoded = Decode.decodeCommandLineToolWithWarnings yaml
                        Expect.equal (decoded.Warnings |> Seq.map (fun warning -> warning.Path) |> List.ofSeq) [ prefix + "." + name + "Max" ] $"The warning for {name} in {field} shows the maximum."
                        Expect.stringContains decoded.Warnings.[0].Message (name + "Max below the " + name + "Min") "The warning says what is wrong."
                        Expect.isSome decoded.Warnings.[0].Raw "The warning shows the requirement."
                        Expect.equal (Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded.Value)) decoded.Value $"The written {name} should decode to the same tool."
            testCase "A maximum that equals or exceeds its minimum, an expression and a lone maximum decode without a warning" <| fun _ ->
                let warnings (lines: string) =
                    (Decode.decodeCommandLineToolWithWarnings ("cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\nrequirements:\n  - class: ResourceRequirement\n" + lines + "inputs: {}\noutputs: {}\n")).Warnings.Count
                Expect.equal (warnings "    coresMin: 4\n    coresMax: 4\n") 0 "Equal values are fine."
                Expect.equal (warnings "    coresMin: 2\n    coresMax: 4.5\n") 0 "A larger maximum is fine."
                Expect.equal (warnings "    coresMin: 4\n    coresMax: $(inputs.n)\n") 0 "An expression has no number to compare."
                Expect.equal (warnings "    coresMax: 2\n") 0 "A lone maximum has no minimum."
                Expect.equal (warnings "    ramMin: 4\n    coresMax: 2\n") 0 "A maximum compares with the minimum of its own resource."
            testCase "A resource value that is zero, positive or an expression decodes without a warning" <| fun _ ->
                let yaml = "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\nrequirements:\n  - class: ResourceRequirement\n    coresMin: 0\n    ramMin: 1.5\n    tmpdirMin: $(-5)\ninputs: {}\noutputs: {}\n"
                Expect.equal (Decode.decodeCommandLineToolWithWarnings yaml).Warnings.Count 0 "Only a number below zero is a warning."
            testCase "Decode int long float and expression resource scalars" <| fun _ ->
                let yaml = """requirements:
  - class: ResourceRequirement
    coresMin: 2
    coresMax: 922337203685477580
    ramMin: 4.5
    outdirMin: $(inputs.outdir_min)"""
                let reqs = decodeRequirements yaml
                let requirement = findRequirement reqs (function ResourceRequirement _ -> true | _ -> false)
                match requirement with
                | ResourceRequirement resourceRequirement ->
                    let coresMin = resourceRequirement.CoresMin.Value :?> int64
                    let coresMax = resourceRequirement.CoresMax.Value :?> int64
                    let ramMin = resourceRequirement.RamMin.Value :?> float
                    let outdirMin = resourceRequirement.OutdirMin.Value :?> SchemaSaladString
                    Expect.equal coresMin 2L "coresMin should decode to int64"
                    Expect.equal coresMax 922337203685477580L "coresMax should decode to int64"
                    Expect.equal ramMin 4.5 "ramMin should decode to float"
                    Expect.equal outdirMin (SchemaSaladString.Literal "$(inputs.outdir_min)") "Expression strings should be preserved"
                    Expect.equal (resourceRequirement.TryGetInt64("coresMin")) (Some 2L) "Typed int64 getter should return normalized value"
                    Expect.equal (resourceRequirement.TryGetFloat("ramMin")) (Some 4.5) "Typed float getter should return normalized value"
                    Expect.equal (resourceRequirement.TryGetExpression("outdirMin")) (Some (SchemaSaladString.Literal "$(inputs.outdir_min)")) "Typed expression getter should return normalized value"
                | _ ->
                    failwith "Expected ResourceRequirement"

            testCase "resource scalars and time limits follow the core schema" <| fun _ ->
                let resource = """requirements:
  - class: ResourceRequirement
    coresMin: 0x10
    ramMin: "$(inputs.ram)"
    ramMax: 1024
    tmpdirMin: .inf
    outdirMin: 1e3"""
                match findRequirement (decodeRequirements resource) (function ResourceRequirement _ -> true | _ -> false) with
                | ResourceRequirement resourceRequirement ->
                    Expect.equal (resourceRequirement.TryGetInt64("coresMin")) (Some 16L) "A plain hexadecimal number is an int."
                    Expect.equal (resourceRequirement.TryGetExpression("ramMin")) (Some (SchemaSaladString.Literal "$(inputs.ram)")) "A quoted expression is a string."
                    Expect.equal (resourceRequirement.TryGetInt64("ramMax")) (Some 1024L) "A plain number is an int."
                    Expect.equal (resourceRequirement.TryGetFloat("tmpdirMin")) (Some System.Double.PositiveInfinity) "The core schema spells infinity .inf."
                    Expect.equal (resourceRequirement.TryGetFloat("outdirMin")) (Some 1000.0) "An exponent number is a float."
                | _ ->
                    failwith "Expected ResourceRequirement"
                let limit (value: string) =
                    let yaml = "requirements:\n  - class: ToolTimeLimit\n    timelimit: " + value
                    findRequirement (decodeRequirements yaml) (function ToolTimeLimitRequirement _ -> true | _ -> false)
                Expect.equal (limit "0x3C") (ToolTimeLimitRequirement (ToolTimeLimitSeconds 60L)) "A plain hexadecimal number is the number of seconds."
                Expect.equal (limit "\"$(60)\"") (ToolTimeLimitRequirement (ToolTimeLimitExpression (SchemaSaladString.Literal "$(60)"))) "A quoted expression is a string and stays an expression."
                Expect.throws (fun _ -> limit "\"60\"" |> ignore) "A quoted number holds no expression."

            testCase "the typed getters give none for a number of another kind, and a fractional number is no integer on any target" <| fun _ ->
                let decoded = decodeRequirements "requirements:\n  - class: ResourceRequirement\n    coresMin: 4.5\n    coresMax: 4\n    ramMax: $(inputs.m)\n"
                match findRequirement decoded (function ResourceRequirement _ -> true | _ -> false) with
                | ResourceRequirement resources ->
                    Expect.isNone (resources.TryGetInt64 "coresMin") "4.5 is no integer."
                    Expect.equal (resources.TryGetFloat "coresMin") (Some 4.5) "4.5 is a float."
                    Expect.equal (resources.TryGetInt64 "coresMax") (Some 4L) "4 is an integer."
                    Expect.isNone (resources.TryGetFloat "coresMax") "4 is no float."
                    Expect.isNone (resources.TryGetInt64 "ramMax") "An expression is no integer."
                    Expect.isNone (resources.TryGetFloat "ramMax") "An expression is no float."
                    Expect.equal (resources.TryGetExpression "ramMax") (Some (SchemaSaladString.Literal "$(inputs.m)")) "The expression is the expression."
                    Expect.isNone (resources.TryGetInt64 "tmpdirMin") "A missing field has none."
                | other -> failwithf "Expected a ResourceRequirement, found %A" other
                let built = ResourceRequirementInstance(coresMin = 3.5, coresMax = box 7, ramMin = -0.5, ramMax = 1e30, tmpdirMin = 9L)
                Expect.isNone (built.TryGetInt64 "coresMin") "3.5 is no integer."
                Expect.equal (built.TryGetInt64 "coresMax") (Some 7L) "An int is an integer."
                Expect.isNone (built.TryGetInt64 "ramMin") "-0.5 is no integer."
                Expect.isNone (built.TryGetInt64 "ramMax") "A number beyond 64 bits is no int64."
                Expect.equal (built.TryGetInt64 "tmpdirMin") (Some 9L) "An int64 is an integer."
                Expect.isNone (CWLParameterValue.tryParseInt64 (box 4.5)) "4.5 is no integer in a parameter value either."
                Expect.equal (CWLParameterValue.tryParseInt64 (box 4)) (Some 4L) "4 is an integer."
                Expect.isNone (CWLParameterValue.tryParseInt64 (box nan)) "NaN is no integer."
                Expect.isNone (CWLParameterValue.tryParseInt64 (box infinity)) "Infinity is no integer."

            testCase "a float that holds a whole number is an integer value and a float on every target" <| fun _ ->
                let decoded = decodeRequirements "requirements:\n  - class: ResourceRequirement\n    coresMin: 4.0\n    coresMax: 4.5\n    ramMin: -2.0\n    ramMax: 1e30\n"
                match findRequirement decoded (function ResourceRequirement _ -> true | _ -> false) with
                | ResourceRequirement resources ->
                    Expect.equal (resources.TryGetInt64 "coresMin") (Some 4L) "4.0 is an integer value."
                    Expect.equal (resources.TryGetFloat "coresMin") (Some 4.0) "4.0 is still a float."
                    Expect.isNone (resources.TryGetInt64 "coresMax") "4.5 is no integer."
                    Expect.equal (resources.TryGetInt64 "ramMin") (Some -2L) "-2.0 is an integer value."
                    Expect.isNone (resources.TryGetInt64 "ramMax") "1e30 is beyond 64 bits."
                | other -> failwithf "Expected a ResourceRequirement, found %A" other
                let built = ResourceRequirementInstance(coresMin = 4.0, coresMax = 4.5, ramMin = nan, ramMax = infinity)
                Expect.equal (built.TryGetInt64 "coresMin") (Some 4L) "A built 4.0 is an integer value."
                Expect.isNone (built.TryGetInt64 "coresMax") "A built 4.5 is no integer."
                Expect.isNone (built.TryGetInt64 "ramMin") "NaN is no integer."
                Expect.isNone (built.TryGetInt64 "ramMax") "Infinity is no integer."
                Expect.equal (CWLParameterValue.tryParseInt64 (box 4.0)) (Some 4L) "4.0 is an integer value in a parameter value too."

            testCase "known fields are typed fields, not dynamic overflow" <| fun _ ->
                let resourceRequirement =
                    ResourceRequirementInstance(coresMin = 2L, ramMin = 4.5, outdirMin = SchemaSaladString.Literal "$(inputs.outdir_min)")

                Expect.sequenceEqual
                    ResourceRequirementInstance.KnownFieldNames
                    (Set [| "class"; "coresMin"; "coresMax"; "ramMin"; "ramMax"; "tmpdirMin"; "tmpdirMax"; "outdirMin"; "outdirMax" |])
                    "ResourceRequirement known fields should be declared on the type."
                Expect.isEmpty
                    (resourceRequirement |> DynamicObjHelpers.dynamicPropertiesSnapshot)
                    "ResourceRequirement known fields should not be stored as dynamic properties."
                Expect.equal (resourceRequirement.TryGetInt64("coresMin")) (Some 2L) "Typed int64 getter should read known field."
                Expect.equal (resourceRequirement.TryGetFloat("ramMin")) (Some 4.5) "Typed float getter should read known field."
                Expect.equal (resourceRequirement.TryGetExpression("outdirMin")) (Some (SchemaSaladString.Literal "$(inputs.outdir_min)")) "Typed expression getter should read known field."

                DynObj.setProperty "arc:note" "keep overflow" resourceRequirement
                Expect.equal
                    (DynObj.tryGetTypedPropertyValue<string> "arc:note" resourceRequirement)
                    (Some "keep overflow")
                    "Unknown fields should still use DynamicObj overflow."

            testCase "DynamicObj values with known resource keys are not treated as typed fields" <| fun _ ->
                let resourceRequirement = ResourceRequirementInstance()
                DynObj.setProperty "coresMin" (box 99L) resourceRequirement
                DynObj.setProperty "arc:note" "keep overflow" resourceRequirement

                Expect.equal
                    (resourceRequirement.TryGetInt64("coresMin"))
                    None
                    "Typed getters should not read known resource fields from DynamicObj storage."

                let yaml = Encode.encodeRequirement (ResourceRequirement resourceRequirement) |> Encode.writeYaml
                Expect.isFalse
                    (yaml.Contains("coresMin"))
                    "Encoding should not emit known resource fields from DynamicObj storage."
                Expect.stringContains yaml "arc:note" "Unknown overflow should still be encoded."

            testCase "Resource scalars roundtrip through encode and decode" <| fun _ ->
                let resourceRequirement = ResourceRequirementInstance(coresMin = 2L, ramMin = 4.5)
                let requirement = ResourceRequirement resourceRequirement
                let encodedElement = Encode.encodeRequirement requirement
                let roundtripped =
                    Decode.requirementArrayDecoder (YamlNode.sequence [ YamlNode.item encodedElement ])
                    |> Seq.head

                match requirement, roundtripped with
                | ResourceRequirement original, ResourceRequirement roundtrip ->
                    let originalCoresMin = original.CoresMin.Value :?> int64
                    let roundtripCoresMin = roundtrip.CoresMin.Value :?> int64
                    let originalRamMin = original.RamMin.Value :?> float
                    let roundtripRamMin = roundtrip.RamMin.Value :?> float

                    Expect.equal roundtripCoresMin originalCoresMin "coresMin should roundtrip as int64"
                    Expect.equal roundtripRamMin originalRamMin "ramMin should roundtrip as float"
                | _ ->
                    failwith "Expected ResourceRequirement in both original and roundtrip values"

        ]
        testList "SchemaDefRequirement" [
            testCase "The map shorthand of a schema definition entry is a decode error" <| fun _ ->
                let yaml = """requirements:
  - class: SchemaDefRequirement
    types:
      - Point:
          type: record
          fields:
            x: int"""
                match decodeError (fun () -> decodeRequirements yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) "requirements[0].types[0]" "The error shows the entry."
                    Expect.stringContains error.Message "Expected a type field" "The entry has no type field."
                    Expect.isSome error.Span "The error has the place in the text."
                | None -> failwith "The map shorthand must fail."
            testCase "A schema definition entry that defines no record or enum is a decode error" <| fun _ ->
                let requirement (entry: string) =
                    "requirements:\n  - class: SchemaDefRequirement\n    types:\n" + entry
                let cases =
                    [ "an alias of a builtin type", "found type: string", requirement """      - name: SampleId
        type: string"""
                      "a name of a file type", "found type: File", requirement """      - name: Reads
        type: File"""
                      "a union", "found a type that is no name", requirement """      - name: Either
        type: [string, int]"""
                      "an entry without a type", "Expected a type field", requirement """      - name: Tag
        symbols: [a]""" ]
                for (name, message, yaml) in cases do
                    match decodeError (fun () -> decodeRequirements yaml |> ignore) with
                    | Some error ->
                        Expect.equal (DecodeError.pathText error) "requirements[0].types[0]" $"{name}: the error should show the entry."
                        Expect.stringContains error.Message message $"{name}: the error should say what is wrong."
                        Expect.isSome error.Span $"{name}: the error should have the place in the text."
                    | None -> failwith $"{name} must fail"
            testCase "Decode canonical object style schema definition entries" <| fun _ ->
                let yaml = """requirements:
  - class: SchemaDefRequirement
    types:
      - name: SampleRecord
        type: record
        fields:
          sampleName: string"""
                let reqs = decodeRequirements yaml
                let requirement = findRequirement reqs (function SchemaDefRequirement _ -> true | _ -> false)
                match requirement with
                | SchemaDefRequirement defs ->
                    Expect.equal defs.Count 1 "SchemaDefRequirement should decode one type definition."
                    Expect.equal (nameOf (definedOf defs.[0])) (Some "SampleRecord") "Schema definition name should be preserved."
                    match definedOf defs.[0] with
                    | SchemaDefinition.Record recordSchema ->
                        let fields = Expect.wantSome recordSchema.Fields "Record schema should keep fields." |> ListEntry.values
                        Expect.equal fields.Count 1 "Record schema should contain one field."
                        Expect.equal fields.[0].Name "sampleName" "Field name should decode."
                        Expect.equal fields.[0].Type CWLType.String "Field type should decode."
                    | other ->
                        failwith $"Expected record schema type but got %A{other}"
                | _ ->
                    failwith "Expected SchemaDefRequirement"
        ]
    ]

let testSchemaDefTypeReferences =
    let point = Reference "Point"
    let inputTypeOf (inputs: ResizeArray<CWLInput>) (name: string) =
        (inputs |> Seq.find (fun input -> input.Name = name)).Type_
    let toolHead = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
"""
    let tool (ports: string) = toolHead + ports

    testList "SchemaDefRequirement type references" [
        testCase "a record names the record that is defined before it by its bare name, by #name and as an array" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.Requirements.SchemaDefTypes.toolFile
            let definitions =
                match decoded.Requirements.Value.[0] with
                | SchemaDefRequirement entries -> entries |> Seq.map definedOf |> Seq.toList
                | other -> failwith $"Expected a SchemaDefRequirement but got %A{other}"
            let segment =
                match definitions |> List.find (fun definition -> nameOf definition = Some "Segment") with
                | SchemaDefinition.Record schema -> schema
                | other -> failwith $"Expected a record but got %A{other}"
            let fieldType (name: string) = InputRecordSchema.tryGetFieldType name segment
            Expect.equal (fieldType "from") (Some point) "A bare name is a reference."
            Expect.equal (fieldType "to") (Some (Reference "#Point")) "A name with # is a reference that keeps the #."
            Expect.equal (fieldType "waypoints") (Some (Array (InputArraySchema(point)))) "A name with [] is an array of the reference."
        testCase "ports name the defined types as a type, a type object, an optional type and an array" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.Requirements.SchemaDefTypes.toolFile
            let inputs = (ListEntry.values decoded.Inputs.Value)
            Expect.equal (inputTypeOf inputs "start") (Some point) "A port can name a type."
            Expect.equal (inputTypeOf inputs "anchor") (Some (Reference "#Point")) "A port can name a type with #."
            Expect.equal (inputTypeOf inputs "route") (Some (Reference "Segment")) "A port can name the second type."
            Expect.equal (inputTypeOf inputs "corners") (Some (Array (InputArraySchema(point)))) "A port can name an array of a type."
            Expect.equal (inputTypeOf inputs "spare") (Some (Union (ResizeArray [ Null; point ]))) "A port can name an optional type."
            Expect.equal (ListEntry.values decoded.Outputs).[0].Type_ (Some (Reference "Segment")) "An output can name a type in object form."
        testCase "references are written as their names and read back" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.Requirements.SchemaDefTypes.toolFile
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded "start: Point\n" "A reference is written as its name."
            Expect.stringContains encoded "corners: Point[]\n" "An array of a reference is written with the shorthand."
            Expect.stringContains encoded "spare: Point?\n" "An optional reference is written with the shorthand."
            let roundTripped = Decode.decodeCommandLineTool encoded
            for input in (ListEntry.values decoded.Inputs.Value) do
                Expect.equal (inputTypeOf (ListEntry.values roundTripped.Inputs.Value) input.Name) input.Type_ $"The type of {input.Name} survives the round trip."
            Expect.sequenceEqual roundTripped.Requirements.Value decoded.Requirements.Value "The definitions survive the round trip."
        testCase "the requirements in map syntax define names for the inline runs of the steps" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Requirements.SchemaDefTypes.workflowFile
            Expect.equal (inputTypeOf (ListEntry.values decoded.Inputs) "origin") (Some (Reference "#pipeline/Point")) "The workflow names a type that its map syntax defines."
            let inlineToolOf (index: int) = Expect.wantSome (WorkflowStepRunOps.tryGetTool (ListEntry.values decoded.Steps).[index].Run) "The step runs an inline tool."
            Expect.equal (inputTypeOf (ListEntry.values (inlineToolOf 0).Inputs.Value) "from") (Some point) "The inline tool of move names a type of the workflow."
            Expect.equal (inputTypeOf (ListEntry.values (inlineToolOf 1).Inputs.Value) "by") (Some (Union (ResizeArray [ Null; Reference "Offset" ]))) "The inline tool of shift names a type of its step."
        testCase "a type is named through the id of the enclosing process" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Requirements.SchemaDefTypes.workflowFile
            let inlineTool = Expect.wantSome (WorkflowStepRunOps.tryGetTool (ListEntry.values decoded.Steps).[0].Run) "The step runs an inline tool."
            Expect.equal (inputTypeOf (ListEntry.values inlineTool.Inputs.Value) "again") (Some (Reference "#pipeline/Point")) "The id of the workflow and the name make the reference."
        testCase "the inline runs of nested steps find the type of the workflow by walking up the ids" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Requirements.SchemaDefTypes.nestedWorkflowFile
            let sub = Expect.wantSome (WorkflowStepRunOps.tryGetWorkflow (ListEntry.values decoded.Steps).[0].Run) "The step runs an inline workflow."
            let deep = Expect.wantSome (WorkflowStepRunOps.tryGetTool (ListEntry.values sub.Steps).[0].Run) "The inner step runs an inline tool."
            Expect.equal (inputTypeOf (ListEntry.values deep.Inputs.Value) "p") (Some point) "The tool two steps down names the type of the workflow."
        testCase "a process with an id names its types with the id, absolute or relative" <| fun _ ->
            let yaml = tool """id: t
requirements:
  - class: SchemaDefRequirement
    types:
      - name: Point
        type: record
        fields:
          x: int
inputs:
  a: "#t/Point"
  b: t/Point
  c: "#t/Point[]"
outputs: {}"""
            let inputs = (ListEntry.values (Decode.decodeCommandLineTool yaml).Inputs.Value)
            Expect.equal (inputTypeOf inputs "a") (Some (Reference "#t/Point")) "The absolute id is a reference."
            Expect.equal (inputTypeOf inputs "b") (Some (Reference "t/Point")) "The id behind the root is a reference."
            Expect.equal (inputTypeOf inputs "c") (Some (Array (InputArraySchema(Reference "#t/Point")))) "The shorthand works with an absolute id."
        testCase "a name that starts a type definition is absolute in the document" <| fun _ ->
            let yaml = tool """id: t
requirements:
  - class: SchemaDefRequirement
    types:
      - name: "#Point"
        type: record
        fields:
          x: int
inputs:
  a: "#Point"
  b: Point
outputs: {}"""
            let inputs = (ListEntry.values (Decode.decodeCommandLineTool yaml).Inputs.Value)
            Expect.equal (inputTypeOf inputs "a") (Some (Reference "#Point")) "The absolute name is a reference."
            Expect.equal (inputTypeOf inputs "b") (Some point) "The bare name finds the type at the root of the document."
        testCase "the processes of a packed document name each other's types by id" <| fun _ ->
            let graph = match Decode.decodeCWLDocument TestObjects.CWL.Workflow.PackedDocument.siblingTypesFile with
                        | CWLDocument.Graph graph -> graph
                        | other -> failwith $"Expected a packed document but got %A{other}"
            let other = match graph.Processes.[1] with
                        | CommandLineTool tool -> tool
                        | unit -> failwith $"Expected a command line tool but got %A{unit}"
            Expect.equal (inputTypeOf (ListEntry.values other.Inputs.Value) "absolute") (Some (Reference "#main/Point")) "The absolute id is a reference."
            Expect.equal (inputTypeOf (ListEntry.values other.Inputs.Value) "relative") (Some (Reference "main/Point")) "The id behind the root is a reference."
        testCase "a process of a packed document that no step runs may name the types of its siblings" <| fun _ ->
            let graph = match Decode.decodeCWLDocument TestObjects.CWL.Workflow.PackedDocument.unreachedMemberNamesSiblingTypeFile with
                        | CWLDocument.Graph graph -> graph
                        | other -> failwith $"Expected a packed document but got %A{other}"
            let place = match CWLGraph.tryFindProcess "place" graph with
                        | Some (CommandLineTool tool) -> tool
                        | other -> failwith $"Expected the tool place but got %A{other}"
            Expect.equal (inputTypeOf (ListEntry.values place.Inputs.Value) "at") (Some (Reference "#other/Q")) "A runner may start with place, so the tool may name any type of the document."
        testCase "a process that only a process other than main runs may name the types of its siblings" <| fun _ ->
            let graph = match Decode.decodeCWLDocument TestObjects.CWL.Workflow.PackedDocument.unreachedWorkflowRunsMemberNamesSiblingTypeFile with
                        | CWLDocument.Graph graph -> graph
                        | other -> failwith $"Expected a packed document but got %A{other}"
            let place = match CWLGraph.tryFindProcess "place" graph with
                        | Some (CommandLineTool tool) -> tool
                        | other -> failwith $"Expected the tool place but got %A{other}"
            Expect.equal (inputTypeOf (ListEntry.values place.Inputs.Value) "at") (Some (Reference "#shift/Offset")) "A runner may start with extra, so its steps take the types of the whole document."
        testCase "a process of a packed document names the types in force at the steps that run it" <| fun _ ->
            let cases =
                [ "a step of main", TestObjects.CWL.Workflow.PackedDocument.typesFile
                  "two steps that share the requirement of main", TestObjects.CWL.Workflow.PackedDocument.memberRunByTwoStepsWithSharedTypesFile
                  "a hint of the tool, which the requirement of main overrides", TestObjects.CWL.Workflow.PackedDocument.memberWithOwnHintFile
                  "a step of a workflow that main runs", TestObjects.CWL.Workflow.PackedDocument.nestedMemberFile
                  "a step of an inline workflow", TestObjects.CWL.Workflow.PackedDocument.memberRunFromInlineWorkflowFile ]
            for (name, yaml) in cases do
                let graph = match Decode.decodeCWLDocument yaml with
                            | CWLDocument.Graph graph -> graph
                            | other -> failwith $"{name}: expected a packed document but got %A{other}"
                match CWLGraph.tryFindProcess "place" graph with
                | Some (CommandLineTool place) -> Expect.equal (inputTypeOf (ListEntry.values place.Inputs.Value) "at") (Some (Reference "#main/Point")) $"{name}: the tool names the type of main."
                | other -> failwith $"{name}: expected the tool place but got %A{other}"
        testCase "a bare name finds no type behind the id of its process" <| fun _ ->
            let withId (portType: string) =
                tool ("""id: t
requirements:
  - class: SchemaDefRequirement
    types:
      - name: Point
        type: record
        fields:
          x: int
inputs:
  p: """ + portType + """
outputs: {}""")
            let siblingStep = """cwlVersion: v1.2
class: Workflow
id: wf
inputs: {}
outputs: {}
steps:
  one:
    requirements:
      - class: SchemaDefRequirement
        types:
          - name: Offset
            type: enum
            symbols: [a]
    run: one.cwl
    in: {}
    out: []
  two:
    run:
      class: CommandLineTool
      baseCommand: x
      inputs:
        p: Offset
      outputs: {}
    in: {}
    out: []"""
            let ownDefinition = """cwlVersion: v1.2
class: Workflow
id: wf
inputs: {}
outputs: {}
steps:
  move:
    run:
      class: CommandLineTool
      baseCommand: x
      requirements:
        - class: SchemaDefRequirement
          types:
            - name: Foo
              type: enum
              symbols: [a]
      inputs:
        p: Foo
      outputs: {}
    in: {}
    out: []"""
            let cases =
                [ "a bare name in a process with an id", "inputs.p", "Invalid CWL type: Point", withId "Point", false
                  "a name with # and no id in a process with an id", "inputs.p", "Invalid CWL type: #Point", withId "\"#Point\"", false
                  "a bare name for the type of another step", "steps.two.run.inputs.p", "Invalid CWL type: Offset", siblingStep, false
                  "a bare name for a type that the inline run defines itself", "steps.move.run.inputs.p", "Invalid CWL type: Foo", ownDefinition, false
                  "a bare name in a sibling process of a packed document", "$graph[1].inputs.absolute", "Invalid CWL type: Point", TestObjects.CWL.Workflow.PackedDocument.siblingTypesFile.Replace("\"#main/Point\"", "Point"), true ]
            for (name, path, message, yaml, packed) in cases do
                match decodeError (fun () -> (if packed then Decode.decodeCWLDocument yaml |> ignore else Decode.decodeCWLProcessingUnit yaml |> ignore)) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path $"{name}: the error should show the place of the type."
                    Expect.stringContains error.Message message $"{name}: the error should say what is wrong."
                    Expect.isSome error.Span $"{name}: the error should have the place in the text."
                | None -> failwith $"{name} must fail"
        testCase "the innermost SchemaDefRequirement of a list applies" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.Requirements.SchemaDefOverride.twoRequirementsFile
            Expect.equal (inputTypeOf (ListEntry.values decoded.Inputs.Value) "y") (Some (Reference "B")) "The port names the type of the second requirement."
        testCase "a requirement overrides a hint, and a hint applies where no requirement overrides it" <| fun _ ->
            let named = TestObjects.CWL.Requirements.SchemaDefOverride.requirementAndHintFile.Replace("y: B", "y: A")
            let tool = Decode.decodeCommandLineTool named
            Expect.equal (inputTypeOf (ListEntry.values tool.Inputs.Value) "y") (Some (Reference "A")) "The port names the type of the requirement."
            let inlineToolOf (yaml: string) =
                let workflow = Decode.decodeWorkflow yaml
                Expect.wantSome (WorkflowStepRunOps.tryGetTool (ListEntry.values workflow.Steps).[0].Run) "The step runs an inline tool."
            let optional (name: string) = Some (Union (ResizeArray [ Null; Reference name ]))
            let outer = TestObjects.CWL.Requirements.SchemaDefOverride.outerRequirementInnerHintFile.Replace("p: run/B?", "p: A?")
            Expect.equal (inputTypeOf (ListEntry.values (inlineToolOf outer).Inputs.Value) "p") (optional "A") "The requirement of the workflow reaches the inline tool."
            let inner = TestObjects.CWL.Requirements.SchemaDefOverride.innerHintFile.Replace("p: A?", "p: run/B?")
            Expect.equal (inputTypeOf (ListEntry.values (inlineToolOf inner).Inputs.Value) "p") (optional "run/B") "The hint of the tool applies, because no requirement overrides it."
        testCase "the requirement of a process overrides the one of its step" <| fun _ ->
            let yaml = TestObjects.CWL.Requirements.SchemaDefOverride.runOverridesStepFile.Replace("p: \"#wf/s/Offset?\"", "p: run/Foo?")
            let workflow = Decode.decodeWorkflow yaml
            let tool = Expect.wantSome (WorkflowStepRunOps.tryGetTool (ListEntry.values workflow.Steps).[0].Run) "The step runs an inline tool."
            Expect.equal (inputTypeOf (ListEntry.values tool.Inputs.Value) "p") (Some (Union (ResizeArray [ Null; Reference "run/Foo" ]))) "The tool names the type of its own requirement."
        testCase "a type that another SchemaDefRequirement defines is a decode error where the innermost one applies" <| fun _ ->
            let over = TestObjects.CWL.Requirements.SchemaDefOverride.twoRequirementsFile
            let cases =
                [ "a port names the type of a hint that a requirement overrides", "inputs.y", TestObjects.CWL.Requirements.SchemaDefOverride.requirementAndHintFile
                  "a port names the type of the first of two requirements", "inputs.y", over.Replace("y: B", "y: A")
                  "the second of two requirements names the type of the first", "requirements[1].types[0].fields.a", TestObjects.CWL.Requirements.SchemaDefOverride.secondRequirementNamesFirstFile
                  "the requirement of a step names the type of its workflow", "steps.move.requirements[0].types[0].fields.a", TestObjects.CWL.Requirements.SchemaDefOverride.stepOverridesWorkflowFile
                  "a tool names the type of the hint of its workflow, which its own hint overrides", "steps.s.run.inputs.p", TestObjects.CWL.Requirements.SchemaDefOverride.innerHintFile
                  "a tool names the type of its own hint, which the requirement of its workflow overrides", "steps.s.run.inputs.p", TestObjects.CWL.Requirements.SchemaDefOverride.outerRequirementInnerHintFile
                  "a workflow port names the type of a step", "inputs.o", TestObjects.CWL.Requirements.SchemaDefOverride.workflowPortStepTypeFile
                  "a tool names the type of its step, which its own requirement overrides", "steps.s.run.inputs.p", TestObjects.CWL.Requirements.SchemaDefOverride.runOverridesStepFile
                  "a port names a file that an overridden requirement imports", "inputs.x", TestObjects.CWL.Requirements.SchemaDefOverride.importInOverriddenRequirementFile
                  "a port names a file that an overridden hint imports", "inputs.x", TestObjects.CWL.Requirements.SchemaDefOverride.importInOverriddenHintFile
                  "a process of a packed document names a type of main, and its own requirement overrides the types of main", "$graph[1].inputs.at", TestObjects.CWL.Workflow.PackedDocument.memberWithOwnTypesFile
                  "a process that a step runs names a type that only a sibling defines", "$graph[1].inputs.at", TestObjects.CWL.Workflow.PackedDocument.reachedMemberNamesSiblingTypeFile
                  "a process that two steps run names a type that the requirement of one step overrides", "$graph[1].inputs.at", TestObjects.CWL.Workflow.PackedDocument.memberRunByTwoStepsFile
                  "a process names the type of its own hint, which the requirement of the workflow that runs it overrides", "$graph[1].inputs.at", TestObjects.CWL.Workflow.PackedDocument.memberWithOwnHintFile.Replace("at: \"#main/Point\"", "at: \"#place/Other\"")
                  "a process two workflows down names a type that only a sibling defines", "$graph[2].inputs.at", TestObjects.CWL.Workflow.PackedDocument.nestedMemberNamesSiblingTypeFile
                  "a process that an inline workflow runs names a type that the requirement of the step overrides", "$graph[1].inputs.at", TestObjects.CWL.Workflow.PackedDocument.memberRunFromInlineWorkflowWithStepTypesFile
                  "the process main names a type that only a sibling defines", "$graph[0].inputs.at.type", TestObjects.CWL.Workflow.PackedDocument.mainNamesSiblingTypeFile
                  "a process that main runs names a type that only a sibling defines", "$graph[1].inputs.at", TestObjects.CWL.Workflow.PackedDocument.mainRunsMemberNamesSiblingTypeFile
                  "a process that a workflow of main runs names a type that only a sibling defines", "$graph[2].inputs.at", TestObjects.CWL.Workflow.PackedDocument.mainReachesMemberThroughWorkflowNamesSiblingTypeFile ]
            for (name, path, yaml) in cases do
                match decodeError (fun () -> Decode.decodeCWLDocument yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path $"{name}: the error should show the place of the type."
                    Expect.stringContains error.Message "The innermost SchemaDefRequirement applies" $"{name}: the error should say why the type does not apply."
                    Expect.isSome error.Span $"{name}: the error should have the place in the text."
                | None -> failwith $"{name} must fail"
        testCase "the first SchemaDefRequirement of the workflows around a tool reaches it and the requirement of an inner workflow keeps to its own ports" <| fun _ ->
            let file = TestObjects.CWL.Requirements.SchemaDefNested.nestedWorkflowsFile.Replace("\r\n", "\n")
            let decode (reference: string) = Decode.decodeWorkflow (file.Replace("{0}", reference))
            let innerTool (workflow: CWLWorkflowDescription) =
                let innerWorkflow = Expect.wantSome (WorkflowStepRunOps.tryGetWorkflow (ListEntry.values workflow.Steps).[0].Run) "The step runs an inline workflow."
                Expect.wantSome (WorkflowStepRunOps.tryGetTool (ListEntry.values innerWorkflow.Steps).[0].Run) "The inner step runs an inline tool."
            Expect.equal (inputTypeOf (ListEntry.values (innerTool (decode "#wf/Outer")).Inputs.Value) "p") (Some (Reference "#wf/Outer")) "The tool below both workflows names the type of the outermost one."
            match decodeError (fun () -> decode "#wf/s/run/Inner" |> ignore) with
            | Some error ->
                Expect.equal (DecodeError.pathText error) "steps.s.run.steps.t.run.inputs.p" "The error shows the port of the tool."
                Expect.stringContains error.Message "A nested workflow hands its steps the SchemaDefRequirement that it inherited, the outermost one." "The error says why the type does not apply."
            | None -> failwith "The tool below the nested workflow cannot name the type of the nested workflow."
        testCase "a requirement of a step or a tool replaces the one that a nested workflow hands down" <| fun _ ->
            let file = TestObjects.CWL.Requirements.SchemaDefNested.nestedWorkflowsFile.Replace("\r\n", "\n")
            let withStepTypes = file.Replace("          in:\n            p:\n              default: a\n          out: []\n", "          requirements:\n            - class: SchemaDefRequirement\n              types:\n                - name: Step\n                  type: enum\n                  symbols: [a]\n          in:\n            p:\n              default: a\n          out: []\n")
            Expect.equal (withStepTypes.Contains "name: Step") true "The fixture holds the requirement of the step."
            let innerTool (workflow: CWLWorkflowDescription) =
                let innerWorkflow = Expect.wantSome (WorkflowStepRunOps.tryGetWorkflow (ListEntry.values workflow.Steps).[0].Run) "The step runs an inline workflow."
                Expect.wantSome (WorkflowStepRunOps.tryGetTool (ListEntry.values innerWorkflow.Steps).[0].Run) "The inner step runs an inline tool."
            Expect.equal (inputTypeOf (ListEntry.values (innerTool (Decode.decodeWorkflow (withStepTypes.Replace("{0}", "#wf/s/run/t/Step")))).Inputs.Value) "p") (Some (Reference "#wf/s/run/t/Step")) "The tool names the type of its step."
            match decodeError (fun () -> Decode.decodeWorkflow (withStepTypes.Replace("{0}", "#wf/Outer")) |> ignore) with
            | Some error -> Expect.equal (DecodeError.pathText error) "steps.s.run.steps.t.run.inputs.p" "The requirement of the step replaces the one of the outer workflow."
            | None -> failwith "The requirement of the step replaces the one of the workflow."
            let withToolTypes = file.Replace("            baseCommand: x\n", "            baseCommand: x\n            requirements:\n              - class: SchemaDefRequirement\n                types:\n                  - name: Mine\n                    type: enum\n                    symbols: [a]\n")
            match decodeError (fun () -> Decode.decodeWorkflow (withToolTypes.Replace("{0}", "#wf/Outer")) |> ignore) with
            | Some error -> Expect.equal (DecodeError.pathText error) "steps.s.run.steps.t.run.inputs.p" "The requirement of the tool replaces the one that the nested workflow hands down."
            | None -> failwith "The requirement of the tool replaces the one of the workflow."
        testCase "an array definition of a SchemaDefRequirement decodes, reads back equal and no port can name it" <| fun _ ->
            let yaml = tool "requirements:\n  - class: SchemaDefRequirement\n    types:\n      - name: Ints\n        type: array\n        items: int\n        label: some numbers\n        arc:note: kept\ninputs: {}\noutputs: {}\n"
            let decoded = Decode.decodeCommandLineTool yaml
            match decoded.Requirements.Value.[0] with
            | SchemaDefRequirement types ->
                match definedOf types.[0] with
                | SchemaDefinition.Array schema ->
                    Expect.equal schema.Name (Some "Ints") "The array keeps its name."
                    Expect.equal schema.Items Int "The array keeps its items."
                    Expect.equal schema.Label (Some (SchemaSaladString.Literal "some numbers")) "The array keeps its label."
                    Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:note" schema) (Some "kept") "The array keeps its extension fields."
                | other -> failwith $"Expected an array definition but got %A{other}"
            | other -> failwith $"Expected a SchemaDefRequirement but got %A{other}"
            let written = Encode.encodeToolDescription decoded
            Expect.stringContains written "type: array" "The array definition is written as a schema."
            Expect.equal (Decode.decodeCommandLineTool written) decoded "The written text should decode to the same tool."
            match decodeError (fun () -> Decode.decodeCommandLineTool (yaml.Replace("inputs: {}", "inputs:\n  s: Ints")) |> ignore) with
            | Some error ->
                Expect.equal (DecodeError.pathText error) "inputs.s" "The error shows the port that names the array."
                Expect.stringContains error.Message "Invalid CWL type: Ints" "cwltool knows no type of that name."
            | None -> failwith "A port cannot name an array definition."
        testCase "a named schema in the type of a port is a type of the process that other ports name by its id" <| fun _ ->
            let file = TestObjects.CWL.Requirements.SchemaDefNested.portSchemaFile.Replace("\r\n", "\n")
            let decode (second: string) (output: string) = Decode.decodeCommandLineTool (file.Replace("{0}", second).Replace("{1}", output))
            let decoded = decode "#a/Color" "#a/Color"
            Expect.equal (inputTypeOf (ListEntry.values decoded.Inputs.Value) "b") (Some (Reference "#a/Color")) "The second port names the enum of the first one by its id."
            Expect.equal ((ListEntry.values decoded.Outputs) |> Seq.find (fun output -> output.Name = "o")).Type_ (Some (Reference "#a/Color")) "An output names it too."
            Expect.equal (Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded)) decoded "The written text should decode to the same tool."
            let withId = Decode.decodeCommandLineTool ((file.Replace("{0}", "#t/a/Color").Replace("{1}", "#t/a/Color")).Replace("baseCommand: echo\n", "id: t\nbaseCommand: echo\n"))
            Expect.equal (inputTypeOf (ListEntry.values withId.Inputs.Value) "b") (Some (Reference "#t/a/Color")) "The id of the process leads the id."
            for (name, second) in [ "a bare name", "Color"; "the name below the wrong port", "#b/Color"; "an unknown name", "#a/Shade" ] do
                match decodeError (fun () -> decode second "#a/Color" |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) "inputs.b.type" $"{name}: the error should show the type of the port."
                    Expect.stringContains error.Message "Invalid CWL type" $"{name}: the error should say that the type is invalid."
                | None -> failwith $"{name} must fail"
        testCase "the id of an inline run lies below run, and only that spelling names its types" <| fun _ ->
            let file = TestObjects.CWL.Requirements.SchemaDefIds.inlineRunWithIdFile
            let workflow = Decode.decodeWorkflow file
            let inlineTool = Expect.wantSome (WorkflowStepRunOps.tryGetTool (ListEntry.values workflow.Steps).[0].Run) "The step runs an inline tool."
            Expect.equal (inputTypeOf (ListEntry.values inlineTool.Inputs.Value) "x") (Some (Union (ResizeArray [ Null; Reference "#wf/s/run/inner/Point" ]))) "The id of the tool starts with the id of the step and run."
            let cases =
                [ "a bare name", "Point?"
                  "the id of the tool without the step", "\"#inner/Point?\""
                  "the id of the tool below the step", "\"#wf/s/inner/Point?\"" ]
            for (name, port) in cases do
                match decodeError (fun () -> Decode.decodeWorkflow (file.Replace("x: \"#wf/s/run/inner/Point?\"", "x: " + port)) |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) "steps.s.run.inputs.x" $"{name}: the error should show the place of the type."
                    Expect.stringContains error.Message "Invalid CWL type" $"{name}: the error should say that the type is invalid."
                | None -> failwith $"{name} must fail"
        testCase "named schemas inside a definition get ids below the id of their field" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.Requirements.SchemaDefIds.nestedNamedFile
            let expected =
                [ "color", "#Outer/inner/Color"
                  "shade", "#Outer/list/Shade"
                  "leaf", "#Outer/deep/Mid/leaf/Leaf"
                  "mid", "#Outer/deep/Mid"
                  "pick", "#Outer/pick/Pick"
                  "kind", "#Listed/item/Kind" ]
            for (port, id) in expected do
                Expect.equal (inputTypeOf (ListEntry.values decoded.Inputs.Value) port) (Some (Union (ResizeArray [ Null; Reference id ]))) $"The port {port} names the schema by {id}."
            let withProcessId = Decode.decodeCommandLineTool TestObjects.CWL.Requirements.SchemaDefIds.nestedNamedInProcessWithIdFile
            Expect.equal (inputTypeOf (ListEntry.values withProcessId.Inputs.Value) "c") (Some (Union (ResizeArray [ Null; Reference "#t/Outer/inner/Color" ]))) "The id of the process leads the id."
        testCase "a nested schema is no name of its own and its id needs the id of the process" <| fun _ ->
            let nested = TestObjects.CWL.Requirements.SchemaDefIds.nestedNamedFile
            let cases =
                [ "a bare name of a nested schema", "inputs.color", nested.Replace("color: \"#Outer/inner/Color?\"", "color: Color?")
                  "an id without the id of the process", "inputs.c", TestObjects.CWL.Requirements.SchemaDefIds.nestedNamedInProcessWithIdFile.Replace("\"#t/Outer/inner/Color?\"", "\"#Outer/inner/Color?\"") ]
            for (name, path, yaml) in cases do
                match decodeError (fun () -> Decode.decodeCommandLineTool yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path $"{name}: the error should show the place of the type."
                    Expect.stringContains error.Message "Invalid CWL type" $"{name}: the error should say that the type is invalid."
                | None -> failwith $"{name} must fail"
        testCase "an unnamed definition decodes with its own name, adds no id and reads back equal" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.Requirements.SchemaDefIds.unnamedDefinitionsFile
            let definitions =
                match decoded.Requirements.Value.[0] with
                | SchemaDefRequirement entries -> entries |> Seq.map definedOf |> Seq.toList
                | other -> failwith $"Expected a SchemaDefRequirement but got %A{other}"
            Expect.equal (definitions |> List.map nameOf) [ Some "Point"; None; None ] "Only the first definition has a name."
            Expect.equal (inputTypeOf (ListEntry.values decoded.Inputs.Value) "x") (Some (Union (ResizeArray [ Null; Reference "Point" ]))) "The named record is a type."
            let encoded = Encode.encodeToolDescription decoded
            Expect.isFalse (encoded.Contains "name: null" || encoded.Contains "name: \"\"") "An unnamed schema writes no name."
            Expect.sequenceEqual (Decode.decodeCommandLineTool encoded).Requirements.Value decoded.Requirements.Value "The definitions survive the round trip."
        testCase "a record may name itself" <| fun _ ->
            let yaml = tool """requirements:
  - class: SchemaDefRequirement
    types:
      - name: Node
        type: record
        fields:
          value: int
          next: Node?
inputs:
  head: Node
outputs: {}"""
            let decoded = Decode.decodeCommandLineTool yaml
            let definition =
                match decoded.Requirements.Value.[0] with
                | SchemaDefRequirement entries -> definedOf entries.[0]
                | other -> failwith $"Expected a SchemaDefRequirement but got %A{other}"
            let node =
                match definition with
                | SchemaDefinition.Record schema -> schema
                | other -> failwith $"Expected a record but got %A{other}"
            Expect.equal (InputRecordSchema.tryGetFieldType "next" node) (Some (Union (ResizeArray [ Null; Reference "Node" ]))) "The field names its own record."
            Expect.equal (inputTypeOf (ListEntry.values decoded.Inputs.Value) "head") (Some (Reference "Node")) "The port names the record."
        testCase "a definition that names a type defined further down is a decode error at the type" <| fun _ ->
            let cases =
                [ "a record before the record that it names", "requirements[0].types[0].fields.from", "Invalid CWL type: Point. The SchemaDefRequirement defines this type further down.", tool """requirements:
  - class: SchemaDefRequirement
    types:
      - name: Segment
        type: record
        fields:
          from: Point
      - name: Point
        type: record
        fields:
          x: int
inputs:
  s: Segment
outputs: {}"""
                  "two records that name each other", "requirements[0].types[0].fields.b", "Invalid CWL type: B. The SchemaDefRequirement defines this type further down.", tool """requirements:
  - class: SchemaDefRequirement
    types:
      - name: A
        type: record
        fields:
          b: B?
      - name: B
        type: record
        fields:
          a: A?
inputs:
  x: A
outputs: {}"""
                  "an array of a type further down", "requirements[0].types[0].fields.all", "Invalid CWL type: Point. The SchemaDefRequirement defines this type further down.", tool """requirements:
  - class: SchemaDefRequirement
    types:
      - name: Cloud
        type: record
        fields:
          all: Point[]
      - name: Point
        type: record
        fields:
          x: int
inputs: {}
outputs: {}""" ]
            for (name, path, message, yaml) in cases do
                match decodeError (fun () -> Decode.decodeCommandLineTool yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path $"{name}: the error should show the place of the type."
                    Expect.stringContains error.Message message $"{name}: the error should say that the definition comes later."
                    Expect.isSome error.Span $"{name}: the error should have the place in the text."
                | None -> failwith $"{name} must fail"
        testCase "a type that the hints define can be named" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.Requirements.SchemaDefTypes.hintsFile
            Expect.equal (ListEntry.values decoded.Inputs.Value).[0].Type_ (Some (Reference "Tag")) "The hint defines Tag."
        testCase "an imported file defines the names behind its path" <| fun _ ->
            let yaml = tool """requirements:
  - class: SchemaDefRequirement
    types:
      - $import: shapes.yml
inputs:
  shape: shapes.yml#Circle
outputs: {}"""
            let decoded = Decode.decodeCommandLineTool yaml
            Expect.equal decoded.Requirements.Value.[0] (SchemaDefRequirement (ResizeArray [ Imported "shapes.yml" ])) "The import is an entry of the requirement."
            Expect.equal (ListEntry.values decoded.Inputs.Value).[0].Type_ (Some (Reference "shapes.yml#Circle")) "The name behind the path is a reference."
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded "$import: shapes.yml" "The import is written as a directive."
            Expect.stringContains encoded "shape: shapes.yml#Circle" "The reference is written as it was read."
            let roundTripped = Decode.decodeCommandLineTool encoded
            Expect.sequenceEqual roundTripped.Requirements.Value decoded.Requirements.Value "The import survives the round trip."
        testCase "the path of an import and the path of a name count with their . and .. segments resolved" <| fun _ ->
            let withPaths (imported: string) (name: string) =
                tool ("""requirements:
  - class: SchemaDefRequirement
    types:
      - $import: """ + imported + """
inputs:
  shape: """ + name + """
outputs: {}""")
            let sameFile =
                [ "./shapes.yml", "shapes.yml#Circle"
                  "shapes.yml", "./shapes.yml#Circle"
                  "sub/../shapes.yml", "shapes.yml#Circle"
                  "shapes.yml", "sub/../shapes.yml#Circle"
                  "./sub/./shapes.yml", "sub/shapes.yml#Circle" ]
            for (imported, name) in sameFile do
                let decoded = Decode.decodeCommandLineTool (withPaths imported name)
                Expect.equal (ListEntry.values decoded.Inputs.Value).[0].Type_ (Some (Reference name)) $"{imported} and {name} name one file."
                Expect.equal decoded.Requirements.Value.[0] (SchemaDefRequirement (ResizeArray [ Imported imported ])) $"The import {imported} stays as written."
            for (imported, name) in [ "sub/shapes.yml", "shapes.yml#Circle"; "../shapes.yml", "shapes.yml#Circle" ] do
                match decodeError (fun () -> Decode.decodeCommandLineTool (withPaths imported name) |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) "inputs.shape" $"{imported} and {name} name two files: the error should show the type."
                    Expect.stringContains error.Message "is not imported by a SchemaDefRequirement of the document." $"{imported} and {name}: the error should say that the file is not imported."
                | None -> failwith $"{imported} and {name} must fail"
        testCase "a types field that is an import stands for a list with the import" <| fun _ ->
            let whole = tool """requirements:
  - class: SchemaDefRequirement
    types:
      $import: shapes.yml
inputs:
  shape: shapes.yml#Circle
outputs: {}"""
            let inList = tool """requirements:
  - class: SchemaDefRequirement
    types:
      - $import: shapes.yml
inputs:
  shape: shapes.yml#Circle
outputs: {}"""
            let decoded = Decode.decodeCommandLineTool whole
            Expect.equal decoded.Requirements.Value.[0] (SchemaDefRequirement (ResizeArray [ Imported "shapes.yml" ])) "The field is one import."
            Expect.equal (ListEntry.values decoded.Inputs.Value).[0].Type_ (Some (Reference "shapes.yml#Circle")) "The name behind the path is a reference."
            Expect.sequenceEqual decoded.Requirements.Value (Decode.decodeCommandLineTool inList).Requirements.Value "Both spellings give one model."
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded "- {$import: shapes.yml}" "The encoder writes the list."
            Expect.sequenceEqual (Decode.decodeCommandLineTool encoded).Requirements.Value decoded.Requirements.Value "The import survives the round trip."
        testCase "a types field that is a mapping with more than the import is a decode error" <| fun _ ->
            let yaml = tool """requirements:
  - class: SchemaDefRequirement
    types:
      $import: shapes.yml
      extra: 1
inputs: {}
outputs: {}"""
            match decodeError (fun () -> Decode.decodeCommandLineTool yaml |> ignore) with
            | Some error ->
                Expect.equal (DecodeError.pathText error) "requirements[0].types" "The error shows the types field."
                Expect.isSome error.Span "The error has the place in the text."
            | None -> failwith "A mapping with another key is no import."
        testCase "an unknown name or an unknown path is a decode error at the type" <| fun _ ->
            let cases =
                [ "an undefined name", "inputs.x", "Invalid CWL type: Missing", tool """inputs:
  x: Missing
outputs: {}"""
                  "an undefined name with #", "inputs.x", "Invalid CWL type: #Missing", tool """requirements:
  - class: SchemaDefRequirement
    types:
      - name: Point
        type: enum
        symbols: [a]
inputs:
  x: "#Missing"
outputs: {}"""
                  "an array of an undefined name", "inputs.x", "Invalid CWL type: Missing", tool """inputs:
  x: Missing[]
outputs: {}"""
                  "a name behind a file that is not imported", "inputs.x", "The file other.yml is not imported by a SchemaDefRequirement of the document.", tool """requirements:
  - class: SchemaDefRequirement
    types:
      - $import: shapes.yml
inputs:
  x: other.yml#Circle
outputs: {}"""
                  "a name behind a path when nothing is imported", "outputs.x", "The file shapes.yml is not imported by a SchemaDefRequirement of the document.", tool """inputs: {}
outputs:
  x: shapes.yml#Circle"""
                  "an undefined name in a record field", "inputs.r.type.fields.f", "Invalid CWL type: Missing", tool """inputs:
  r:
    type:
      type: record
      fields:
        f: Missing
outputs: {}""" ]
            for (name, path, message, yaml) in cases do
                match decodeError (fun () -> Decode.decodeCommandLineTool yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path $"{name}: the error should show the place of the type."
                    Expect.stringContains error.Message message $"{name}: the error should say what is wrong."
                    Expect.isSome error.Span $"{name}: the error should have the place in the text."
                | None -> failwith $"{name} must fail"
        testCase "a type that the requirements of a step define is no name for the workflow" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: Workflow
inputs:
  p: Offset
steps:
  move:
    requirements:
      - class: SchemaDefRequirement
        types:
          - name: Offset
            type: enum
            symbols: [a]
    run: tool.cwl
    in: {}
    out: []
outputs: {}"""
            match decodeError (fun () -> Decode.decodeWorkflow yaml |> ignore) with
            | Some error -> Expect.equal (DecodeError.pathText error) "inputs.p" "The workflow does not see the type of its step."
            | None -> failwith "The type of a step is not in force for the workflow."
        testCase "references compare and hash by their names" <| fun _ ->
            Expect.equal (Reference "Point") (Reference "Point") "The same name is the same reference."
            Expect.equal (hash (Reference "Point")) (hash (Reference "Point")) "The same name gives the same hash."
            Expect.notEqual (Reference "Point") (Reference "#Point") "The name counts as it is written."
            Expect.notEqual (Reference "String") String "A reference is no builtin type."
            let tag = SchemaDefinition.Enum (InputEnumSchema(ResizeArray [ SchemaSaladString.Literal "a" ]))
            Expect.notEqual (Defined tag) (Imported "Point") "A definition differs from an import."
        testCase "definitions compare and hash by their schemas" <| fun _ ->
            let enumOf (symbols: string list) = SchemaDefinition.Enum (InputEnumSchema(ResizeArray (List.map SchemaSaladString.Literal symbols)))
            let recordOf (name: string) = SchemaDefinition.Record (InputRecordSchema(fields = ListEntry.ofValues [ InputRecordField("x", Int) ], name = name))
            Expect.equal (enumOf [ "a" ]) (enumOf [ "a" ]) "The same symbols are the same definition."
            Expect.equal (hash (enumOf [ "a" ])) (hash (enumOf [ "a" ])) "The same definition gives the same hash."
            Expect.notEqual (enumOf [ "a" ]) (enumOf [ "b" ]) "Other symbols are another definition."
            Expect.equal (recordOf "Point") (recordOf "Point") "The same record is the same definition."
            Expect.equal (hash (recordOf "Point")) (hash (recordOf "Point")) "The same record gives the same hash."
            Expect.notEqual (recordOf "Point") (recordOf "Other") "Another record name is another definition."
            Expect.notEqual (enumOf [ "a" ]) (recordOf "Point") "An enum differs from a record."
            Expect.notEqual (Defined (enumOf [ "a" ])) (Defined (enumOf [ "b" ])) "The schema counts for the entry."
    ]

let testSchemaDefEncoding =
    let linesOf (requirement: Requirement) =
        (Encode.encodeRequirement requirement |> Encode.writeYaml).TrimEnd('\n').Split('\n')
    let encodeDecodeRequirementFragment (requirement: Requirement) : Requirement =
        let encoded = Encode.encodeRequirement requirement |> Encode.writeYaml
        (decodeRequirements ("requirements:\n  - " + encoded.Replace("\n", "\n    "))).[0]
    let definitionsOf (requirement: Requirement) =
        match requirement with
        | SchemaDefRequirement entries -> entries |> Seq.map definedOf |> Seq.toList
        | other -> failwith $"Expected a SchemaDefRequirement but got %A{other}"

    testList "SchemaDefRequirement encoding" [
        testCase "a record definition is written as the schema with its name beside type" <| fun _ ->
            let yaml = """requirements:
  - class: SchemaDefRequirement
    types:
      - name: Point
        type: record
        fields:
          x: int
          z: int"""
            let expected =
                [| "class: SchemaDefRequirement"
                   "types:"
                   "  - name: Point"
                   "    type: record"
                   "    fields:"
                   "      x:"
                   "        type: int"
                   "      z:"
                   "        type: int" |]
            Expect.sequenceEqual (linesOf (decodeRequirements yaml).[0]) expected "The definition is one mapping with name, type and fields."
        testCase "an enum definition is written the same way" <| fun _ ->
            let yaml = """requirements:
  - class: SchemaDefRequirement
    types:
      - name: Tag
        type: enum
        symbols: [a, b]"""
            let expected =
                [| "class: SchemaDefRequirement"
                   "types:"
                   "  - name: Tag"
                   "    type: enum"
                   "    symbols: [a, b]" |]
            Expect.sequenceEqual (linesOf (decodeRequirements yaml).[0]) expected "The definition is the schema with its name beside type."
        testCase "a definition that was read reads back equal" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.Requirements.SchemaDefTypes.plainRecordFile
            let roundTripped = Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded)
            Expect.sequenceEqual roundTripped.Requirements.Value decoded.Requirements.Value "The plain record survives the round trip."
        testCase "the definitions of the cross-referencing fixture are written as schemas and read back equal" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.Requirements.SchemaDefTypes.toolFile
            let encoded = Encode.encodeToolDescription decoded
            let lines = encoded.Split('\n')
            Expect.isTrue (Array.contains "      - name: Segment" lines) "The definition starts with its name."
            Expect.isTrue (Array.contains "        type: record" lines) "The type stands beside the name."
            Expect.isFalse (lines |> Array.exists (fun line -> line = "          type: record")) "No schema stands nested under type."
            let roundTripped = Decode.decodeCommandLineTool encoded
            Expect.sequenceEqual roundTripped.Requirements.Value decoded.Requirements.Value "The definitions survive the round trip."
        testCase "an extension field of a definition is written once and survives" <| fun _ ->
            let yaml = """requirements:
  - class: SchemaDefRequirement
    types:
      - name: Point
        type: record
        fields: {x: int}
        arc:note: keep"""
            let requirement = (decodeRequirements yaml).[0]
            let lines = linesOf requirement
            Expect.equal (lines |> Array.filter (fun line -> line.Contains "arc:note") |> Array.length) 1 "The key stands once."
            let roundTripped = encodeDecodeRequirementFragment requirement
            match (definitionsOf roundTripped).[0] with
            | SchemaDefinition.Record schema -> Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:note" schema) (Some "keep") "The field stays on the schema."
            | other -> failwith $"Expected a record but got %A{other}"
            Expect.sequenceEqual (linesOf roundTripped) lines "A second pass writes the same text."
    ]

let testDecodeAllRequirementSyntaxes =
    testList "Requirement Decode Syntax Coverage" [
        testCase "Array Syntax" <| fun _ ->
            let r = decodeRequirements TestObjects.CWL.Requirements.requirementsArraySyntax
            Expect.hasLength r 1 "Decode Class Syntax for SubworkflowFeatureRequirement, can only be 'one' element of SubworkflowFeatureRequirement"
            Expect.equal r.[0] SubworkflowFeatureRequirement "Decode Class Syntax for SubworkflowFeatureRequirement, can only be a element of SubworkflowFeatureRequirement"

        testCase "Mapping Syntax" <| fun _ ->
            let r = decodeRequirements TestObjects.CWL.Requirements.requirementsMappingSyntax
            Expect.hasLength r 1 "Decode Mapping Syntax for SubworkflowFeatureRequirement, can only be 'one' element of SubworkflowFeatureRequirement"
            Expect.equal r.[0] SubworkflowFeatureRequirement "Decode Mapping Syntax for SubworkflowFeatureRequirement, can only be a element of SubworkflowFeatureRequirement"

        testCase "Json Syntax" <| fun _ ->
            let r = decodeRequirements TestObjects.CWL.Requirements.requirementsJSONSyntax
            Expect.hasLength r 1 "Decode Json Syntax for SubworkflowFeatureRequirement, can only be 'one' element of SubworkflowFeatureRequirement"
            Expect.equal r.[0] SubworkflowFeatureRequirement "Decode Json Syntax for SubworkflowFeatureRequirement, can only be a element of SubworkflowFeatureRequirement"
    ]

let private extractRequirementsOrder (text:string) =
    text.Split('\n')
    |> Array.filter (fun l -> l.TrimStart().StartsWith("- class:"))
    |> Array.map (fun l -> l.Trim())

let testRequirementEncode =
    testList "Encode" [
        testList "Requirements ordering" [
            testCase "requirements order stable" <| fun _ ->
                let original = CommandLineTool.cwlFile
                let (encoded1, _, _) = assertDeterministic Encode.encodeToolDescription Decode.decodeCommandLineTool "CommandLineTool" original
                let order1 = extractRequirementsOrder encoded1
                // Do another cycle explicitly
                let decoded2 = Decode.decodeCommandLineTool encoded1
                let encoded2 = Encode.encodeToolDescription decoded2
                let order2 = extractRequirementsOrder encoded2
                Expect.equal order2 order1 "Requirement entries order must remain stable across encode cycles"
        ]
        testList "InitialWorkDirRequirement listing" [
            testCase "string listing entries roundtrip through encode/decode" <| fun _ ->
                let listing =
                    ResizeArray [|
                        StringEntry (SchemaSaladString.Literal "$(inputs.stageDirectory)")
                        DirentEntry (DirentInstance(SchemaSaladString.Literal "$(inputs.outputDirectory)", entryname = SchemaSaladString.Literal "outdir", writable = SchemaSaladBool.Value true))
                    |]
                let req = InitialWorkDirRequirement listing
                let yaml = Encode.encodeRequirement req |> Encode.writeYaml
                let indented = yaml.Replace("\n", "\n    ")
                let document = "requirements:\n  - " + indented
                let decoded =
                    Decode.readYaml document
                    |> Decode.requirementsDecoder
                    |> Option.get
                match decoded.[0] with
                | InitialWorkDirRequirement roundtripped ->
                    Expect.sequenceEqual roundtripped listing "InitialWorkDirRequirement listing entries should roundtrip."
                | _ ->
                    failwith "Expected InitialWorkDirRequirement"
            testCase "include/import listing entries and dirent wrappers roundtrip through encode/decode" <| fun _ ->
                let listing =
                    ResizeArray [|
                        StringEntry (SchemaSaladString.Include "scripts/load.js")
                        StringEntry (SchemaSaladString.Import "scripts/manifest.yml")
                        DirentEntry (DirentInstance(SchemaSaladString.Include "scripts/bootstrap.sh", entryname = SchemaSaladString.Import "scripts/name.txt", writable = SchemaSaladBool.Value false))
                    |]
                let req = InitialWorkDirRequirement listing
                let yaml = Encode.encodeRequirement req |> Encode.writeYaml
                Expect.stringContains yaml "$include: scripts/load.js" "String listing include wrapper should encode as map."
                Expect.stringContains yaml "$import: scripts/manifest.yml" "String listing import wrapper should encode as map."
                Expect.stringContains yaml "$include: scripts/bootstrap.sh" "Dirent entry include wrapper should encode as map."
                Expect.stringContains yaml "$import: scripts/name.txt" "Dirent entryname import wrapper should encode as map."
                let indented = yaml.Replace("\n", "\n    ")
                let document = "requirements:\n  - " + indented
                let decoded =
                    Decode.readYaml document
                    |> Decode.requirementsDecoder
                    |> Option.get
                match decoded.[0] with
                | InitialWorkDirRequirement roundtripped ->
                    Expect.sequenceEqual roundtripped listing "Directive wrappers in InitialWorkDirRequirement should roundtrip."
                | _ ->
                    failwith "Expected InitialWorkDirRequirement"

            testCase "File and Directory entries roundtrip through decode/encode/decode" <| fun _ ->
                let yaml = """requirements:
  - class: InitialWorkDirRequirement
    listing:
      - class: File
        path: /tmp/input.txt
      - class: Directory
        path: /tmp/workdir"""
                let firstDecode = decodeRequirements yaml
                let requirement = findRequirement firstDecode (function InitialWorkDirRequirement _ -> true | _ -> false)
                let encoded = Encode.encodeRequirement requirement |> Encode.writeYaml
                Expect.stringContains encoded "class: File" "File entries should encode with explicit class"
                Expect.stringContains encoded "class: Directory" "Directory entries should encode with explicit class"
                let document = "requirements:\n  - " + encoded.Replace("\n", "\n    ")
                let secondDecode =
                    Decode.readYaml document
                    |> Decode.requirementsDecoder
                    |> Option.get
                    |> fun reqs -> findRequirement reqs (function InitialWorkDirRequirement _ -> true | _ -> false)

                match requirement, secondDecode with
                | InitialWorkDirRequirement original, InitialWorkDirRequirement roundtripped ->
                    Expect.equal roundtripped.Count original.Count "Listing item count should roundtrip"
                    Expect.equal roundtripped.[0] original.[0] "File entry should roundtrip"
                    Expect.equal roundtripped.[1] original.[1] "Directory entry should roundtrip"
                | _ ->
                    failwith "Expected InitialWorkDirRequirement in both decode passes"
        ]
        testList "Canonical requirement encoding" [
            testCase "DockerRequirement.create prefers dockerFileReference over dockerFile" <| fun _ ->
                let created =
                    DockerRequirement.create(
                        dockerFile = "./Dockerfile.literal",
                        dockerFileReference = SchemaSaladString.Include "./Dockerfile.include"
                    )
                Expect.equal created.DockerFile (Some (SchemaSaladString.Include "./Dockerfile.include")) "dockerFileReference should take precedence when both inputs are provided."
            testCase "DockerRequirement encodes dockerFile as canonical string and includes extended fields" <| fun _ ->
                let requirement =
                    Requirement.DockerRequirement (
                        DockerRequirement.create(
                            dockerPull = (SchemaSaladString.Literal "ghcr.io/example/tool:1.0.0"),
                            dockerFileReference = SchemaSaladString.Literal "./Dockerfile",
                            dockerImageId = (SchemaSaladString.Literal "tool-image"),
                            dockerLoad = (SchemaSaladString.Literal "docker-archive:///tmp/tool.tar"),
                            dockerImport = (SchemaSaladString.Literal "https://example.org/images/tool.sif"),
                            dockerOutputDirectory = (SchemaSaladString.Literal "/work/out")
                        )
                    )
                let encoded = Encode.encodeRequirement requirement |> Encode.writeYaml
                Expect.stringContains encoded "dockerFile: ./Dockerfile" "dockerFile should be emitted as canonical string."
                Expect.stringContains encoded "dockerLoad: docker-archive:///tmp/tool.tar" "dockerLoad should be encoded when present."
                Expect.stringContains encoded "dockerImport: https://example.org/images/tool.sif" "dockerImport should be encoded when present."
                Expect.stringContains encoded "dockerOutputDirectory: /work/out" "dockerOutputDirectory should be encoded when present."
                Expect.isFalse (encoded.Contains("$include")) "Canonical docker encoding should not emit legacy map-style dockerFile."
            testCase "Legacy include syntax is preserved after decode/encode" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsClassFileContent
                let docker = findRequirement reqs (function DockerRequirement _ -> true | _ -> false)
                let encoded = Encode.encodeRequirement docker |> Encode.writeYaml
                Expect.stringContains encoded "$include: FSharpArcCapsule/Dockerfile" "Legacy include syntax should be preserved when originally provided."
            testCase "DockerRequirement $import syntax roundtrips" <| fun _ ->
                let requirement = Requirement.DockerRequirement (DockerRequirement.create(dockerFileReference = SchemaSaladString.Import "./Dockerfile"))
                let yaml = Encode.encodeRequirement requirement |> Encode.writeYaml
                Expect.stringContains yaml "$import: ./Dockerfile" "dockerFile should preserve $import directive during encode."
                let document = "requirements:\n  - " + yaml.Replace("\n", "\n    ")
                let decoded =
                    Decode.readYaml document
                    |> Decode.requirementsDecoder
                    |> Option.get
                Expect.equal decoded.[0] requirement "dockerFile $import directive should roundtrip."
            testCase "LoadListingRequirement roundtrips" <| fun _ ->
                let requirement = LoadListingRequirement (LoadListingRequirementValue(DeepListing))
                let yaml = Encode.encodeRequirement requirement |> Encode.writeYaml
                let document = "requirements:\n  - " + yaml.Replace("\n", "\n    ")
                let decoded =
                    Decode.readYaml document
                    |> Decode.requirementsDecoder
                    |> Option.get
                Expect.equal decoded.[0] requirement "LoadListingRequirement should roundtrip through encode/decode."
            testCase "WorkReuse/NetworkAccess/InplaceUpdate payloads roundtrip" <| fun _ ->
                let requirements =
                    ResizeArray [|
                        WorkReuseRequirement (WorkReuseRequirementValue(false))
                        NetworkAccessRequirement (NetworkAccessRequirementValue(false))
                        InplaceUpdateRequirement (InplaceUpdateRequirementValue(SchemaSaladBool.Value false))
                    |]
                let encodedLines =
                    requirements
                    |> Seq.map (fun requirement -> "  - " + (Encode.encodeRequirement requirement |> Encode.writeYaml).Replace("\n", "\n    "))
                    |> String.concat "\n"
                let document = "requirements:\n" + encodedLines
                let decoded =
                    Decode.readYaml document
                    |> Decode.requirementsDecoder
                    |> Option.get
                Expect.sequenceEqual decoded requirements "Payload-bearing requirements should roundtrip."
            testCase "ToolTimeLimit expression form roundtrips" <| fun _ ->
                let requirement = ToolTimeLimitRequirement (ToolTimeLimitExpression (SchemaSaladString.Literal "$(inputs.max_runtime_seconds)"))
                let yaml = Encode.encodeRequirement requirement |> Encode.writeYaml
                let document = "requirements:\n  - " + yaml.Replace("\n", "\n    ")
                let decoded =
                    Decode.readYaml document
                    |> Decode.requirementsDecoder
                    |> Option.get
                Expect.equal decoded.[0] requirement "Expression timelimit should roundtrip."
            testCase "SchemaDefRequirement roundtrips with explicit typed representation" <| fun _ ->
                let recordSchema = InputRecordSchema(fields = ListEntry.ofValues [| InputRecordField("sampleName", CWLType.String) |], name = "SampleRecord")
                let requirement =
                    SchemaDefRequirement (ResizeArray [| Defined (SchemaDefinition.Record recordSchema) |])
                let yaml = Encode.encodeRequirement requirement |> Encode.writeYaml
                Expect.stringContains yaml "name: SampleRecord" "SchemaDefRequirement should encode canonical name field."
                Expect.stringContains yaml "type: record" "SchemaDefRequirement should encode canonical type field."
                let document = "requirements:\n  - " + yaml.Replace("\n", "\n    ")
                let decoded =
                    Decode.readYaml document
                    |> Decode.requirementsDecoder
                    |> Option.get
                match decoded.[0] with
                | SchemaDefRequirement definitions ->
                    let expected = ResizeArray [| Defined (SchemaDefinition.Record recordSchema) |]
                    Expect.sequenceEqual definitions expected "SchemaDefRequirement should roundtrip with explicit Name/Type_ entries."
                | _ ->
                    failwith "Expected SchemaDefRequirement"
        ]
    ]
let testInitialWorkDirFileDirectoryEntries =
    testList "InitialWorkDirRequirement File/Directory entries" [
        testCase "Decode mixed listing with Dirent String File Directory" <| fun _ ->
            let yaml = """requirements:
  - class: InitialWorkDirRequirement
    listing:
      - entry: $(inputs.arcDirectory)
        writable: true
      - $(inputs.outputDirectory)
      - class: File
        path: /tmp/in.txt
      - class: Directory
        path: /tmp/outdir"""
            let reqs = decodeRequirements yaml
            let initialWorkDirItem = findRequirement reqs (function InitialWorkDirRequirement _ -> true | _ -> false)
            match initialWorkDirItem with
            | InitialWorkDirRequirement listing ->
                Expect.equal listing.Count 4 "Expected four listing entries"
                Expect.isTrue (match listing.[0] with | DirentEntry _ -> true | _ -> false) "First entry should be DirentEntry"
                Expect.isTrue (match listing.[1] with | StringEntry _ -> true | _ -> false) "Second entry should be StringEntry"
                Expect.isTrue (match listing.[2] with | FileEntry _ -> true | _ -> false) "Third entry should be FileEntry"
                Expect.isTrue (match listing.[3] with | DirectoryEntry _ -> true | _ -> false) "Fourth entry should be DirectoryEntry"
            | _ ->
                failwith "Wrong requirement type: expected InitialWorkDirRequirement"

        testCase "Encode mixed listing with File and Directory entries" <| fun _ ->
            let file = FileInstance(path = SchemaSaladString.Literal "/tmp/input.txt", basename = SchemaSaladString.Literal "input.txt")
            let directory = DirectoryInstance(path = SchemaSaladString.Literal "/tmp/output", basename = SchemaSaladString.Literal "output")
            let requirement =
                InitialWorkDirRequirement (
                    ResizeArray [|
                        StringEntry (SchemaSaladString.Literal "$(inputs.outputDirectory)")
                        FileEntry file
                        DirectoryEntry directory
                    |]
                )
            let encoded = Encode.encodeRequirement requirement |> Encode.writeYaml
            Expect.stringContains encoded "class: InitialWorkDirRequirement" "Encoded output should include requirement class"
            Expect.stringContains encoded "class: File" "Encoded output should include File listing entry"
            Expect.stringContains encoded "class: Directory" "Encoded output should include Directory listing entry"

        testCase "File and Directory listing object fields decode as typed members" <| fun _ ->
            let reqs = decodeRequirements TestObjects.CWL.Requirements.initialWorkDirListingTypedFileContent
            match findRequirement reqs (function InitialWorkDirRequirement _ -> true | _ -> false) with
            | InitialWorkDirRequirement listing ->
                match listing.[0], listing.[1] with
                | FileEntry file, DirectoryEntry directory ->
                    Expect.equal file.Path (Some (SchemaSaladString.Literal "/tmp/in.txt")) "File path should decode as a typed field."
                    Expect.equal file.Basename (Some (SchemaSaladString.Literal "in.txt")) "File basename should decode as a typed field."
                    Expect.equal file.Checksum (Some (SchemaSaladString.Literal "sha1$abc")) "File checksum should decode as a typed field."
                    Expect.equal file.Size (Some (FileSize.Bytes 42L)) "File size should decode as a typed field."
                    Expect.equal directory.Path (Some (SchemaSaladString.Literal "/tmp/out")) "Directory path should decode as a typed field."
                    Expect.equal directory.Basename (Some (SchemaSaladString.Literal "out")) "Directory basename should decode as a typed field."
                    Expect.isSome directory.Listing "Directory listing should decode as a typed field."
                    Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:file note" file) (Some "keep file overflow") "File extension should remain overflow."
                    Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:dir note" directory) (Some "keep dir overflow") "Directory extension should remain overflow."
                    Expect.isNone (DynObj.tryGetTypedPropertyValue<string> "path" file) "Known File fields should not be overflow."
                    Expect.isNone (DynObj.tryGetTypedPropertyValue<string> "path" directory) "Known Directory fields should not be overflow."
                | _ -> failwith "Expected File and Directory entries"
            | _ ->
                failwith "Expected InitialWorkDirRequirement"
    ]

let testInitialWorkDirListingForms =
    let listingOf (lines: string list) =
        "requirements:\n  - class: InitialWorkDirRequirement\n    listing:" + String.concat "" (lines |> List.map (fun line -> "\n      " + line))
    let encodeAndDecodeAgain (requirement: Requirement) =
        let written = Encode.encodeRequirement requirement |> Encode.writeYaml
        written, (decodeRequirements ("requirements:\n" + (written.TrimEnd().Split('\n') |> Array.mapi (fun index line -> (if index = 0 then "  - " else "    ") + line) |> String.concat "\n")))

    testList "InitialWorkDirRequirement listing forms" [
        testCase "a null item, an array of Files and Directories and an empty array decode as entries and round trip" <| fun _ ->
            let yaml =
                listingOf [ "- null"
                            "- entryname: a.txt"
                            "  entry: hello"
                            "- - class: File"
                            "    location: a.txt"
                            "  - class: Directory"
                            "    location: d"
                            "- []" ]
            let requirement = (decodeRequirements yaml).[0]
            match requirement with
            | InitialWorkDirRequirement listing ->
                Expect.equal listing.Count 4 "Expected four entries."
                Expect.equal listing.[0] NullEntry "A null item is a NullEntry."
                Expect.equal listing.[1] (DirentEntry (DirentInstance(SchemaSaladString.Literal "hello", entryname = SchemaSaladString.Literal "a.txt"))) "A Dirent stays a DirentEntry."
                Expect.equal listing.[2] (FilesEntry (ResizeArray [ FileItem (FileInstance(location = SchemaSaladString.Literal "a.txt")); DirectoryItem (DirectoryInstance(location = SchemaSaladString.Literal "d")) ])) "An array holds its File and its Directory."
                Expect.equal listing.[3] (FilesEntry (ResizeArray [])) "An empty array is an entry."
            | other -> failwithf "Expected an InitialWorkDirRequirement, found %A" other
            let written, again = encodeAndDecodeAgain requirement
            Expect.stringContains written "- null" "The null item is written as null."
            Expect.stringContains written "- - class: File" "The array is written as an array."
            Expect.equal again.[0] requirement "The written text should decode to the same requirement."
        testCase "the entries of an array compare by their elements" <| fun _ ->
            let files (location: string) = FilesEntry (ResizeArray [ FileItem (FileInstance(location = SchemaSaladString.Literal location)) ])
            Expect.equal (files "a.txt") (files "a.txt") "Arrays with equal items are equal."
            Expect.notEqual (files "a.txt") (files "b.txt") "Arrays with different items differ."
            Expect.equal (hash (files "a.txt")) (hash (files "a.txt")) "Equal arrays hash alike."
            Expect.notEqual NullEntry (files "a.txt") "A null item is no array."
        testCase "an expression as the listing stays an expression" <| fun _ ->
            let yaml = "requirements:\n  - class: InlineJavascriptRequirement\n  - class: InitialWorkDirRequirement\n    listing: $(inputs.directory.listing)"
            let requirement = (decodeRequirements yaml).[1]
            Expect.equal requirement (InitialWorkDirExpressionRequirement (SchemaSaladString.Literal "$(inputs.directory.listing)")) "The scalar listing is the expression."
            let written, again = encodeAndDecodeAgain requirement
            Expect.stringContains written "listing: $(inputs.directory.listing)" "The expression is written as a string."
            Expect.isFalse (written.Contains "[") "The expression is no list."
            Expect.equal again.[0] requirement "The written text should decode to the same requirement."
        testCase "a list with one expression differs from the expression" <| fun _ ->
            let yaml = "requirements:\n  - class: InitialWorkDirRequirement\n    listing:\n      - $(inputs.directory.listing)"
            match (decodeRequirements yaml).[0] with
            | InitialWorkDirRequirement listing ->
                Expect.sequenceEqual listing (ResizeArray [ StringEntry (SchemaSaladString.Literal "$(inputs.directory.listing)") ]) "The list keeps its one entry."
            | other -> failwithf "Expected an InitialWorkDirRequirement, found %A" other
            Expect.notEqual (InitialWorkDirExpressionRequirement (SchemaSaladString.Literal "$(x)")) (InitialWorkDirRequirement (ResizeArray [ StringEntry (SchemaSaladString.Literal "$(x)") ])) "The two spellings are two requirements."
        testCase "a listing or an item that is a text without an expression is a decode error" <| fun _ ->
            let cases =
                [ "a text as the listing", "requirements[0].listing", "requirements:\n  - class: InitialWorkDirRequirement\n    listing: abc", "Expected a list of entries or an expression for the listing, found the text \"abc\". An expression holds $( or ${."
                  "an empty string as the listing", "requirements[0].listing", "requirements:\n  - class: InitialWorkDirRequirement\n    listing: \"\"", "Expected a list of entries or an expression for the listing"
                  "a text with a dollar and no bracket as the listing", "requirements[0].listing", "requirements:\n  - class: InitialWorkDirRequirement\n    listing: \"$x\"", "An expression holds $( or ${."
                  "a text as an item", "requirements[0].listing[1]", listingOf [ "- $(inputs.x)"; "- abc" ], "Expected an expression, a Dirent, a File or a Directory as an entry of the listing, found the text \"abc\". An expression holds $( or ${."
                  "a quoted text as an item", "requirements[0].listing[0]", listingOf [ "- \"null\"" ], "Expected an expression, a Dirent, a File or a Directory as an entry of the listing"
                  "a text as an item of a hint", "hints[0].listing[0]", "hints:\n  - class: InitialWorkDirRequirement\n    listing:\n      - abc", "Expected an expression, a Dirent, a File or a Directory as an entry of the listing" ]
            for (name, path, yaml, message) in cases do
                let decode () = if yaml.StartsWith "hints" then decodeHints yaml |> ignore else decodeRequirements yaml |> ignore
                match decodeError decode with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
                    Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
                    Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
                | None -> failwith (sprintf "%s must fail" name)
        testCase "a text with a parameter reference or a script is an expression, and so are the directives that name a file" <| fun _ ->
            let yaml =
                listingOf [ "- $(inputs.x)"
                            "- \"text $(inputs.x)\""
                            "- \"${ return 1; }\""
                            "- \"$(\""
                            "- {$include: notes.txt}"
                            "- {$import: other.yml}" ]
            match (decodeRequirements yaml).[0] with
            | InitialWorkDirRequirement listing ->
                Expect.sequenceEqual
                    listing
                    (ResizeArray [ StringEntry (SchemaSaladString.Literal "$(inputs.x)")
                                   StringEntry (SchemaSaladString.Literal "text $(inputs.x)")
                                   StringEntry (SchemaSaladString.Literal "${ return 1; }")
                                   StringEntry (SchemaSaladString.Literal "$(")
                                   StringEntry (SchemaSaladString.Include "notes.txt")
                                   StringEntry (SchemaSaladString.Import "other.yml") ])
                    "The expressions stay as written, and the text of a directive is not known to the decoder."
            | other -> failwithf "Expected an InitialWorkDirRequirement, found %A" other
            Expect.equal (decodeRequirements "requirements:\n  - class: InitialWorkDirRequirement\n    listing: \"${ return []; }\"").[0] (InitialWorkDirExpressionRequirement (SchemaSaladString.Literal "${ return []; }")) "A script as the listing is an expression."
        testCase "a mapping as the listing is a decode error at the listing" <| fun _ ->
            let yaml = "requirements:\n  - class: InitialWorkDirRequirement\n    listing:\n      class: File\n      location: a.txt"
            match decodeError (fun () -> decodeRequirements yaml |> ignore) with
            | Some error ->
                Expect.equal (DecodeError.pathText error) "requirements[0].listing" "The error shows the listing."
                Expect.stringContains error.Message "Expected a list of entries or an expression for the listing, found a mapping." "The error says what is wrong."
                Expect.isSome error.Span "The error has the place in the text."
            | None -> failwith "A mapping as the listing must fail."
        testCase "an item that CWL does not admit is a decode error at the item" <| fun _ ->
            let cases =
                [ "a mapping without entry or class", "requirements[0].listing[1]", listingOf [ "- $(inputs.x)"; "- foo: bar" ], "Expected a Dirent with an entry field"
                  "a string inside an array", "requirements[0].listing[0][1]", listingOf [ "- - class: File"; "    location: a.txt"; "  - $(inputs.x)" ], "Expected a File or a Directory with a class field in an array of the listing"
                  "a null inside an array", "requirements[0].listing[0][0]", listingOf [ "- [null]" ], "Expected a File or a Directory with a class field in an array of the listing"
                  "a Dirent inside an array", "requirements[0].listing[0][0]", listingOf [ "- [{entry: a}]" ], "Expected a File or a Directory with a class field in an array of the listing"
                  "an array inside an array", "requirements[0].listing[0][0]", listingOf [ "- [[{class: File, location: a.txt}]]" ], "Expected a File or a Directory with a class field in an array of the listing" ]
            for (name, path, yaml, message) in cases do
                match decodeError (fun () -> decodeRequirements yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the item" name)
                    Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
                    Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
                | None -> failwith (sprintf "%s must fail" name)
    ]

let testSchemaSaladDirectivesInRequirements =
    let requirementsOf (lines: string list) = "requirements:\n" + String.concat "\n" lines
    let writeAndRead (requirement: Requirement) =
        let written = Encode.encodeRequirement requirement |> Encode.writeYaml
        written, (decodeRequirements ("requirements:\n" + (written.TrimEnd().Split('\n') |> Array.mapi (fun index line -> (if index = 0 then "  - " else "    ") + line) |> String.concat "\n"))).[0]
    let include' (path: string) = SchemaSaladString.Include path

    testList "Schema Salad directives in requirements" [
        testCase "the entries of expressionLib keep a directive as a directive" <| fun _ ->
            let yaml = requirementsOf [ "  - class: InlineJavascriptRequirement"; "    expressionLib:"; "      - $include: lib.js"; "      - var x = 1;"; "      - $import: more.yml" ]
            let requirement = (decodeRequirements yaml).[0]
            match requirement with
            | InlineJavascriptRequirement value ->
                Expect.sequenceEqual value.ExpressionLib.Value (ResizeArray [ include' "lib.js"; SchemaSaladString.Literal "var x = 1;"; SchemaSaladString.Import "more.yml" ]) "The entries are a directive, a literal and a directive."
            | other -> failwithf "Expected an InlineJavascriptRequirement, found %A" other
            let written, again = writeAndRead requirement
            Expect.stringContains written "- {$include: lib.js}" "The include is written as a flow mapping."
            Expect.isFalse (written.Contains "\"$include") "No entry is the text of a directive."
            Expect.equal again requirement "The written text should decode to the same requirement."
        testCase "envValue keeps a directive in the list form" <| fun _ ->
            let listForm = requirementsOf [ "  - class: EnvVarRequirement"; "    envDef:"; "      - envName: A"; "        envValue:"; "          $include: a.txt"; "      - envName: B"; "        envValue: plain" ]
            let requirement = (decodeRequirements listForm).[0]
            match requirement with
            | EnvVarRequirement envs ->
                Expect.equal (ListEntry.values envs).[0].EnvValue (include' "a.txt") "The include is kept."
                Expect.equal (ListEntry.values envs).[1].EnvValue (SchemaSaladString.Literal "plain") "The literal is kept."
            | other -> failwithf "Expected an EnvVarRequirement, found %A" other
            let written, again = writeAndRead requirement
            Expect.stringContains written "$include: a.txt" "The include is written as a mapping."
            Expect.equal again requirement "The written text should decode to the same requirement."
        testCase "a directive cannot stand for the value of a map entry of envDef, as cwltool rejects it" <| fun _ ->
            for (name, line) in [ "an include", "A: {$include: a.txt}"; "an import", "A: {$import: env.yml}" ] do
                let yaml = requirementsOf [ "  - class: EnvVarRequirement"; "    envDef:"; "      " + line ]
                match (try decodeRequirements yaml |> ignore; None with YamlDecodeException error -> Some error) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) "requirements[0].envDef.A" (sprintf "%s: the error should show the entry" name)
                    Expect.stringContains error.Message "A directive cannot stand for the value of an entry of the envDef field" (sprintf "%s: the error should say what is wrong" name)
                    Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
                | None -> failwith (sprintf "%s must fail" name)
        testCase "an import stands in its place among the envDef entries, is written back there and reads back equal" <| fun _ ->
            let yaml = requirementsOf [ "  - class: EnvVarRequirement"; "    envDef:"; "      - {$import: more-env.yml}"; "      - envName: A"; "        envValue: b"; "      - {$import: last-env.yml}" ]
            let requirement = (decodeRequirements yaml).[0]
            match requirement with
            | EnvVarRequirement envs ->
                Expect.equal (List.ofSeq envs |> List.map (ListEntry.map (fun env -> env.EnvName))) [ ListEntry.Imported "more-env.yml"; ListEntry.Value (SchemaSaladString.Literal "A"); ListEntry.Imported "last-env.yml" ] "The imports stand around the definition."
                Expect.equal (ListEntry.values envs |> Seq.map (fun env -> env.EnvName) |> List.ofSeq) [ SchemaSaladString.Literal "A" ] "The values leave the imports out."
            | other -> failwithf "Expected an EnvVarRequirement, found %A" other
            let written, again = writeAndRead requirement
            Expect.isTrue (written.IndexOf "more-env.yml" < written.IndexOf "envName: A" && written.IndexOf "envName: A" < written.IndexOf "last-env.yml") "The entries keep their order."
            Expect.stringContains written "- {$import: more-env.yml}" "The import is written as an entry."
            Expect.equal again requirement "The written text should decode to the same requirement."
        testCase "envDef as one import holds that import and is written as a list, as in the types of a SchemaDefRequirement" <| fun _ ->
            let whole = (decodeRequirements (requirementsOf [ "  - class: EnvVarRequirement"; "    envDef: {$import: envs.yml}" ])).[0]
            let listed = (decodeRequirements (requirementsOf [ "  - class: EnvVarRequirement"; "    envDef: [{$import: envs.yml}]" ])).[0]
            match whole with
            | EnvVarRequirement envs -> Expect.equal (List.ofSeq envs) [ ListEntry.Imported "envs.yml" ] "The field holds the import."
            | other -> failwithf "Expected an EnvVarRequirement, found %A" other
            Expect.equal whole listed "Both spellings are the same requirement."
            let written, again = writeAndRead whole
            Expect.stringContains written "- {$import: envs.yml}" "The import is written as a list entry."
            Expect.equal again whole "The written text should decode to the same requirement."
        testCase "an include or a directive with another field cannot stand for an envDef entry" <| fun _ ->
            for (name, line, path, message) in
                [ "an include", "    envDef: [{$include: a.txt}]", "requirements[0].envDef[0]", "An $include yields the text of a file, but an entry of the envDef field takes an object."
                  "an import with another field", "    envDef: [{$import: a.yml, envName: A}]", "requirements[0].envDef[0]", "A directive has exactly one field"
                  "an include as the whole field", "    envDef: {$include: a.txt}", "requirements[0].envDef", "An $include yields the text of a file, but the envDef field takes an object." ] do
                match (try decodeRequirements (requirementsOf [ "  - class: EnvVarRequirement"; line ]) |> ignore; None with YamlDecodeException error -> Some error) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the entry" name)
                    Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
                    Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
                | None -> failwith (sprintf "%s must fail" name)
        testCase "the expression of WorkReuse, NetworkAccess and ToolTimeLimit keeps a directive" <| fun _ ->
            let cases =
                [ "WorkReuse", "enableReuse", WorkReuseExpressionRequirement (include' "reuse.txt")
                  "NetworkAccess", "networkAccess", NetworkAccessExpressionRequirement (include' "network.txt")
                  "ToolTimeLimit", "timelimit", ToolTimeLimitRequirement (ToolTimeLimitExpression (include' "limit.txt")) ]
            for className, field, expected in cases do
                let file = (match expected with
                            | WorkReuseExpressionRequirement _ -> "reuse.txt"
                            | NetworkAccessExpressionRequirement _ -> "network.txt"
                            | _ -> "limit.txt")
                let yaml = requirementsOf [ "  - class: " + className; "    " + field + ":"; "      $include: " + file ]
                let requirement = (decodeRequirements yaml).[0]
                Expect.equal requirement expected (sprintf "%s: the include is kept" className)
                let written, again = writeAndRead requirement
                Expect.stringContains written ("$include: " + file) (sprintf "%s: the include is written as a mapping" className)
                Expect.equal again requirement (sprintf "%s: the written text should decode to the same requirement" className)
        testCase "a resource that is an include stays an include" <| fun _ ->
            let yaml = requirementsOf [ "  - class: ResourceRequirement"; "    coresMin: 2"; "    ramMin:"; "      $include: ram.txt" ]
            let requirement = (decodeRequirements yaml).[0]
            match requirement with
            | ResourceRequirement resource ->
                Expect.equal (resource.TryGetInt64 "coresMin") (Some 2L) "A number stays a number."
                Expect.equal (resource.TryGetExpression "ramMin") (Some (include' "ram.txt")) "An include stays an include."
            | other -> failwithf "Expected a ResourceRequirement, found %A" other
            let written, again = writeAndRead requirement
            Expect.stringContains written "$include: ram.txt" "The include is written as a mapping."
            Expect.equal again requirement "The written text should decode to the same requirement."
        testCase "a mapping that is no directive is a decode error at its place" <| fun _ ->
            let cases =
                [ "an entry of expressionLib", "requirements[0].expressionLib[1]", requirementsOf [ "  - class: InlineJavascriptRequirement"; "    expressionLib:"; "      - var x = 1;"; "      - foo: bar" ]
                  "an enableReuse", "requirements[0].enableReuse", requirementsOf [ "  - class: WorkReuse"; "    enableReuse:"; "      foo: bar" ]
                  "a timelimit with a list", "requirements[0].timelimit", requirementsOf [ "  - class: ToolTimeLimit"; "    timelimit: [1, 2]" ]
                  "a resource", "requirements[0].coresMin", requirementsOf [ "  - class: ResourceRequirement"; "    coresMin:"; "      foo: bar" ] ]
            for name, path, yaml in cases do
                match decodeError (fun () -> decodeRequirements yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
                    Expect.stringContains error.Message "Expected a string or a directive, found" (sprintf "%s: the error should say what is wrong" name)
                    Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
                | None -> failwith (sprintf "%s must fail" name)
        testCase "a directive has exactly one field" <| fun _ ->
            let cases =
                [ "an import with another field in a listing", "requirements[0].listing[0]", requirementsOf [ "  - class: InitialWorkDirRequirement"; "    listing:"; "      - {$import: a.yml, extra: 1}" ]
                  "an include with another field in an envValue", "requirements[0].envDef[0].envValue", requirementsOf [ "  - class: EnvVarRequirement"; "    envDef:"; "      - envName: A"; "        envValue: {$include: a.txt, other: b}" ]
                  "an include and an import", "requirements[0].dockerFile", requirementsOf [ "  - class: DockerRequirement"; "    dockerFile: {$include: a, $import: b}" ]
                  "an import with another field in the types", "requirements[0].types[0]", requirementsOf [ "  - class: SchemaDefRequirement"; "    types:"; "      - {$import: t.yml, class: X}" ]
                  "an import with another field as the types", "requirements[0].types", requirementsOf [ "  - class: SchemaDefRequirement"; "    types: {$import: t.yml, extra: 1}" ] ]
            for (name, path, yaml) in cases do
                match decodeError (fun () -> decodeRequirements yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
                    Expect.stringContains error.Message "A directive has exactly one field" (sprintf "%s: the error should say what is wrong" name)
                    Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
                | None -> failwith (sprintf "%s must fail" name)
        testCase "the path of a directive is a string" <| fun _ ->
            let dockerFile (directive: string) = requirementsOf [ "  - class: DockerRequirement"; "    dockerFile: " + directive ]
            for (name, directive, message) in
                [ "an int", "{$import: 42}", "Expected the path of the $import directive to be a string, found the plain scalar 42, which the core schema reads as an int."
                  "a bool", "{$include: true}", "Expected the path of the $include directive to be a string, found the plain scalar true"
                  "a list", "{$import: [a.yml]}", "Expected the path of the $import directive to be a string, found a list."
                  "a mapping", "{$import: {a: b}}", "Expected the path of the $import directive to be a string, found a mapping."
                  "a null", "{$include: }", "Expected the path of the $include directive to be a string, found null."
                  "an empty string", "{$import: \"\"}", "The path of the $import directive is empty." ] do
                match decodeError (fun () -> decodeRequirements (dockerFile directive) |> ignore) with
                | Some error ->
                    let key = if directive.Contains "$include" then "$include" else "$import"
                    Expect.equal (DecodeError.pathText error) ("requirements[0].dockerFile." + key) (sprintf "%s: the error should show the path field" name)
                    Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
                    Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
                | None -> failwith (sprintf "%s must fail" name)
            match (decodeRequirements (dockerFile "{$import: \"42\"}")).[0] with
            | DockerRequirement docker -> Expect.equal docker.DockerFile (Some (SchemaSaladString.Import "42")) "A quoted number is a path."
            | other -> failwithf "Expected a DockerRequirement, found %A" other
        testCase "a mixin is a directive of its own, which stands for a string and for an object that a file completes" <| fun _ ->
            let docker = decodeRequirements (requirementsOf [ "  - class: DockerRequirement"; "    dockerFile: {$mixin: Dockerfile}" ])
            match docker.[0] with
            | DockerRequirement docker -> Expect.equal docker.DockerFile (Some (SchemaSaladString.Mixin "Dockerfile")) "A mixin on its own is a mixin, and not an import."
            | other -> failwithf "Expected a DockerRequirement, found %A" other
            Expect.notEqual (SchemaSaladString.Mixin "a") (SchemaSaladString.Import "a") "A mixin differs from an import of the same path."
            let written, again = writeAndRead docker.[0]
            Expect.stringContains written "dockerFile: {$mixin: Dockerfile}" "The mixin is written back as a mixin."
            Expect.equal again docker.[0] "The written text should decode to the same requirement."
            let requirements = decodeRequirements (requirementsOf [ "  - {$mixin: req1.yml, class: NetworkAccess, networkAccess: true}" ])
            match requirements.[0] with
            | MixedRequirement _ -> ()
            | other -> failwithf "Expected a mixed requirement, found %A" other
            let writtenMixed, againMixed = writeAndRead requirements.[0]
            Expect.stringContains writtenMixed "$mixin: req1.yml" "The mixin is written back."
            Expect.stringContains writtenMixed "class: NetworkAccess" "The fields beside the mixin are written back."
            Expect.equal againMixed requirements.[0] "The written text should decode to the same requirement."
        testCase "a mixin beside other fields stands for a string too, because the file decides whether the fields count" <| fun _ ->
            let docker = decodeRequirements (requirementsOf [ "  - class: DockerRequirement"; "    dockerFile: {$mixin: Dockerfile, extra: 1}" ])
            match docker.[0] with
            | DockerRequirement requirement ->
                match requirement.DockerFile with
                | Some (SchemaSaladString.Mixed raw) -> Expect.equal (SchemaSaladString.Mixed raw).Value "Dockerfile" "The value of the mixin is its path."
                | other -> failwithf "Expected a mixin beside a field, found %A" other
            | other -> failwithf "Expected a DockerRequirement, found %A" other
            Expect.notEqual (SchemaSaladString.Mixin "Dockerfile") (match docker.[0] with DockerRequirement requirement -> requirement.DockerFile.Value | _ -> SchemaSaladString.Literal "") "A mixin on its own differs from a mixin beside a field."
            let written, again = writeAndRead docker.[0]
            Expect.stringContains written "dockerFile: {$mixin: Dockerfile, extra: 1}" "The mixin and its field are written back as they were."
            Expect.equal again docker.[0] "The written text should decode to the same requirement."
            let other = decodeRequirements (requirementsOf [ "  - class: DockerRequirement"; "    dockerFile: {$mixin: Dockerfile, extra: 2}" ])
            Expect.notEqual other.[0] docker.[0] "Another field beside the mixin makes another requirement."
        testCase "an include cannot stand where CWL takes an object" <| fun _ ->
            let cases =
                [ "an entry of the types", "requirements[0].types[0]", requirementsOf [ "  - class: SchemaDefRequirement"; "    types:"; "      - {$include: t.yml}" ]
                  "the types as a whole", "requirements[0].types", requirementsOf [ "  - class: SchemaDefRequirement"; "    types: {$include: t.yml}" ] ]
            for (name, path, yaml) in cases do
                match decodeError (fun () -> decodeRequirements yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
                    Expect.stringContains error.Message "An $include yields the text of a file, but" (sprintf "%s: the error should say what is wrong" name)
                    Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
                | None -> failwith (sprintf "%s must fail" name)
        testCase "a path that YAML needs quoted is written quoted and reads back" <| fun _ ->
            for path in [ "a b.txt"; "x, y.txt"; "k: v.txt"; "q\"uote.txt"; "#hash.txt"; "[list].txt"; "{map}.txt"; "42" ] do
                for directive in [ SchemaSaladString.Include path; SchemaSaladString.Import path ] do
                    let requirement = DockerRequirement (DockerRequirement.create(dockerFileReference = directive))
                    let written = Encode.encodeRequirement requirement |> Encode.writeYaml
                    Expect.stringContains written "dockerFile: {$" (sprintf "%s: the directive is a flow mapping" path)
                    let again = (decodeRequirements ("requirements:\n  - " + written.Replace("\n", "\n    ").TrimEnd())).[0]
                    Expect.equal again requirement (sprintf "%s: the written directive reads back as the same requirement" path)
        testCase "an import in the requirements stands in its place and the field may be the import as a whole" <| fun _ ->
            let toolWith (lines: string list) =
                "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs: {}\noutputs: {}\n" + String.concat "\n" lines + "\n"
            let inList = Decode.decodeCommandLineTool (toolWith [ "requirements:"; "  - {$import: req.yml}"; "  - class: InlineJavascriptRequirement" ])
            Expect.equal inList.Requirements.Value.[0] (ImportedRequirement "req.yml") "The import is the first entry."
            Expect.equal inList.Requirements.Value.[1] (InlineJavascriptRequirement (InlineJavascriptRequirementValue())) "The requirement behind it stays."
            let written = Encode.encodeToolDescription inList
            Expect.stringContains written "requirements:\n  - {$import: req.yml}\n  - class: InlineJavascriptRequirement" "The import stays in its place."
            Expect.equal (Decode.decodeCommandLineTool written) inList "The written text should decode to the same tool."
            let whole = Decode.decodeCommandLineTool (toolWith [ "requirements: {$import: reqs.yml}" ])
            Expect.sequenceEqual whole.Requirements.Value [ ImportedRequirement "reqs.yml" ] "The field is a list with one import."
            Expect.stringContains (Encode.encodeToolDescription whole) "requirements: {$import: reqs.yml}\n" "The field is written as the directive again."
            Expect.equal (Decode.decodeCommandLineTool (Encode.encodeToolDescription whole)) whole "The written text should decode to the same tool."
            let asList = Decode.decodeCommandLineTool (toolWith [ "requirements:"; "  - {$import: reqs.yml}" ])
            Expect.equal whole asList "The two spellings are one model."
            Expect.stringContains (Encode.encodeToolDescription asList) "requirements:\n  - {$import: reqs.yml}" "A list stays a list."
            whole.Requirements.Value.Add(ShellCommandRequirement)
            let extended = Encode.encodeToolDescription whole
            Expect.stringContains extended "requirements:\n  - {$import: reqs.yml}\n  - class: ShellCommandRequirement" "A list with more than the import is written as a list."
            Expect.equal (Decode.decodeCommandLineTool extended).Requirements.Value.Count 2 "The longer list reads back."
        testCase "an import in the hints is an imported hint and never an unknown one" <| fun _ ->
            let hints = decodeHints "hints:\n  - {$import: hints.yml}\n  - class: acme:Thing\n    value: 1"
            Expect.equal hints.[0] (ImportedHint "hints.yml") "The import is an imported hint."
            Expect.isTrue (match hints.[1] with UnknownHint _ -> true | _ -> false) "A hint of another class stays unknown."
            Expect.isNone (HintEntry.tryAsRequirement hints.[0]) "An imported hint is no requirement."
            let tool = Decode.decodeCommandLineTool "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs: {}\noutputs: {}\nhints: {$import: hints.yml}\n"
            Expect.sequenceEqual tool.Hints.Value [ ImportedHint "hints.yml" ] "The field is a list with one imported hint."
            let written = Encode.encodeToolDescription tool
            Expect.stringContains written "hints: {$import: hints.yml}\n" "The field is written as the directive again."
            Expect.equal (Decode.decodeCommandLineTool written) tool "The written text should decode to the same tool."
        testCase "the requirements and the hints of a step keep an import as a whole" <| fun _ ->
            let yaml = "cwlVersion: v1.2\nclass: Workflow\ninputs: {}\noutputs: {}\nsteps:\n  s:\n    run: t.cwl\n    in: {}\n    out: []\n    requirements: {$import: reqs.yml}\n    hints:\n      - {$import: hints.yml}\n"
            let workflow = Decode.decodeWorkflow yaml
            Expect.sequenceEqual (ListEntry.values workflow.Steps).[0].Requirements.Value [ ImportedRequirement "reqs.yml" ] "The step has the imported requirements."
            Expect.sequenceEqual (ListEntry.values workflow.Steps).[0].Hints.Value [ ImportedHint "hints.yml" ] "The step has the imported hints."
            let written = Encode.encodeWorkflowDescription workflow
            Expect.stringContains written "requirements: {$import: reqs.yml}" "The requirements are the directive again."
            Expect.stringContains written "- {$import: hints.yml}" "The hints stay a list."
            Expect.equal (Decode.decodeWorkflow written) workflow "The written text should decode to the same workflow."
        testCase "a directive cannot stand for the value of an entry of the requirements or the hints in map form" <| fun _ ->
            let cases =
                [ "an import as a requirement", "requirements[0].InlineJavascriptRequirement", "requirements:\n  InlineJavascriptRequirement: {$import: req.yml}"
                  "an include as a requirement", "requirements[0].InlineJavascriptRequirement", "requirements:\n  InlineJavascriptRequirement: {$include: req.yml}"
                  "an import as a hint", "hints[0].InlineJavascriptRequirement", "hints:\n  InlineJavascriptRequirement: {$import: req.yml}" ]
            for (name, _, yaml) in cases do
                match decodeError (fun () -> Decode.decodeCommandLineTool ("cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs: {}\noutputs: {}\n" + yaml + "\n") |> ignore) with
                | Some error ->
                    Expect.stringContains error.Message "A directive cannot stand for the value of an entry of the" (sprintf "%s: the error should say what is wrong" name)
                    Expect.stringContains error.Message "Write the field as a list." (sprintf "%s: the error should say what to write" name)
                    Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
                | None -> failwith (sprintf "%s must fail" name)
            let mixin' = decodeRequirements "requirements:\n  InlineJavascriptRequirement: {$mixin: req.yml}"
            Expect.equal mixin'.Count 1 "A mixin as the value is a requirement of its own, as cwltool reads it."
            match mixin'.[0] with
            | MixedRequirement _ -> ()
            | other -> failwithf "Expected a mixed requirement, found %A" other
        testCase "an include cannot stand for a requirement or a hint" <| fun _ ->
            let tool (field: string) = "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs: {}\noutputs: {}\n" + field + "\n"
            let cases =
                [ "an entry of the requirements", "requirements[0]", tool "requirements:\n  - {$include: r.yml}", "an entry of the requirements"
                  "an entry of the hints", "hints[0]", tool "hints:\n  - {$include: h.yml}", "an entry of the hints"
                  "the requirements as a whole", "requirements", tool "requirements: {$include: r.yml}", "the requirements field"
                  "the hints as a whole", "hints", tool "hints: {$include: h.yml}", "the hints field" ]
            for (name, path, yaml, what) in cases do
                match decodeError (fun () -> Decode.decodeCommandLineTool yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
                    Expect.stringContains error.Message ("An $include yields the text of a file, but " + what + " takes an object.") (sprintf "%s: the error should say what is wrong" name)
                    Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
                | None -> failwith (sprintf "%s must fail" name)
        testCase "a type of an imported file of requirements is named by the path of the file" <| fun _ ->
            let tool (reference: string) =
                "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\nrequirements:\n  - {$import: sd.yml}\ninputs:\n  a: " + reference + "\noutputs: {}\n"
            Expect.equal (ListEntry.values (Decode.decodeCommandLineTool (tool "sd.yml#Color")).Inputs.Value).[0].Type_ (Some (Reference "sd.yml#Color")) "The path of the import and the name are the reference."
            Expect.equal (ListEntry.values (Decode.decodeCommandLineTool (tool "sd.yml#Color?")).Inputs.Value).[0].Type_ (Some (Union (ResizeArray [ Null; Reference "sd.yml#Color" ]))) "The optional spelling reads too."
            Expect.equal (ListEntry.values (Decode.decodeCommandLineTool (tool "\"file:///work/sd.yml#Color\"")).Inputs.Value).[0].Type_ (Some (Reference "file:///work/sd.yml#Color")) "An address with a scheme counts when the document imports a file."
            for (name, reference) in [ "a bare name", "Color"; "a name behind another path", "other.yml#Color"; "the name with a hash in front", "\"#sd.yml#Color\"" ] do
                match decodeError (fun () -> Decode.decodeCommandLineTool (tool reference) |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) "inputs.a" (sprintf "%s: the error should show the port" name)
                    Expect.stringContains error.Message "Invalid CWL type" (sprintf "%s: the error should say that the type is invalid" name)
                | None -> failwith (sprintf "%s must fail" name)
        testCase "a field that holds no directive is a decode error for a mapping" <| fun _ ->
            let yaml = requirementsOf [ "  - class: DockerRequirement"; "    cwltool:dockerRunOptions:"; "      - $include: options.txt" ]
            match decodeError (fun () -> decodeRequirements yaml |> ignore) with
            | Some error -> Expect.equal (DecodeError.pathText error) "requirements[0].cwltool:dockerRunOptions[0]" "The error shows the entry."
            | None -> failwith "A mapping in a list of strings must fail."
    ]

let testInvalidRequirementsAndHints =
    testList "Invalid requirements and hints" [
        testCase "a class alias that CWL does not know is a decode error at the class, and an unknown hint class stays unknown" <| fun _ ->
            for alias, field in [ "WorkReuseRequirement", "enableReuse: true"; "NetworkAccessRequirement", "networkAccess: true"; "InplaceUpdate", "inplaceUpdate: true"; "ToolTimeLimitRequirement", "timelimit: 5" ] do
                match decodeError (fun () -> decodeRequirements ("requirements:\n  - class: " + alias + "\n    " + field) |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) "requirements[0].class" (sprintf "%s: the error shows the class" alias)
                    Expect.stringContains error.Message ("Invalid or unsupported requirement class: " + alias) (sprintf "%s: the error names the class" alias)
                    Expect.isSome error.Span (sprintf "%s: the error has the place in the text" alias)
                | None -> failwith (sprintf "%s must fail as a requirement" alias)
                match decodeError (fun () -> decodeRequirements ("requirements:\n  " + alias + ":\n    " + field) |> ignore) with
                | Some error -> Expect.equal (DecodeError.pathText error) ("requirements." + alias) (sprintf "%s: the error shows the key in the map form" alias)
                | None -> failwith (sprintf "%s must fail as a key" alias)
                match (decodeHints ("hints:\n  - class: " + alias + "\n    " + field)).[0] with
                | UnknownHint unknown -> Expect.equal unknown.Class (Some alias) (sprintf "%s: a hint of this class stays unknown" alias)
                | other -> failwithf "Expected an unknown hint, found %A" other
        testCase "a requirement list or hint list that is neither a list nor a mapping is a decode error" <| fun _ ->
            match decodeError (fun () -> decodeRequirements "requirements: abc" |> ignore) with
            | Some error ->
                Expect.equal (DecodeError.pathText error) "requirements" "The error shows the field."
                Expect.stringContains error.Message "Expected a list or a mapping of requirements, found the text \"abc\"" "The error says what is wrong."
                Expect.isFalse (error.Message.Contains "root:") "The error shows no dump of the node."
                Expect.isSome error.Span "The error has the place in the text."
            | None -> failwith "A scalar requirements field must fail."
            match decodeError (fun () -> decodeHints "hints: abc" |> ignore) with
            | Some error ->
                Expect.equal (DecodeError.pathText error) "hints" "The error shows the field."
                Expect.stringContains error.Message "Expected a list or a mapping of hints, found the text \"abc\"" "The error says what is wrong."
                Expect.isSome error.Span "The error has the place in the text."
            | None -> failwith "A scalar hints field must fail."
        testCase "a map-form hint with a scalar or a list as the payload is a decode error and gets no value field" <| fun _ ->
            for payload in [ "abc"; "[a, b]" ] do
                match decodeError (fun () -> decodeHints ("hints:\n  DockerRequirement: " + payload) |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) "hints.DockerRequirement" (sprintf "payload %s: the error shows the hint" payload)
                    Expect.stringContains error.Message "Expected a mapping for the hint DockerRequirement" (sprintf "payload %s: the error says what is wrong" payload)
                    Expect.isSome error.Span (sprintf "payload %s: the error has the place in the text" payload)
                | None -> failwith (sprintf "payload %s must fail" payload)
        testCase "a loadListing that is no symbol, and an envDef or packages field of another shape are decode errors at the field" <| fun _ ->
            let cases =
                [ "a loadListing that is no symbol", "requirements[0].loadListing", "Expected one of no_listing, shallow_listing and deep_listing", "requirements:\n  - class: LoadListingRequirement\n    loadListing: some"
                  "an envDef that is a number", "requirements[0].envDef", "Expected a list or a mapping for envDef", "requirements:\n  - class: EnvVarRequirement\n    envDef: 5"
                  "a packages field that is a number", "requirements[0].packages", "Expected a list or a mapping for packages", "requirements:\n  - class: SoftwareRequirement\n    packages: 5" ]
            for name, path, message, yaml in cases do
                match decodeError (fun () -> decodeRequirements yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error shows the field" name)
                    Expect.stringContains error.Message message (sprintf "%s: the error says what is wrong" name)
                    Expect.isSome error.Span (sprintf "%s: the error has the place in the text" name)
                | None -> failwith (sprintf "%s must fail" name)
    ]

let testDynamicPayloadModel =
    testList "DynamicObj payload model" [
        testCase "CWL schema payloads keep known fields typed and overflow dynamic" <| fun _ ->
            let field = InputRecordField("sampleName", CWLType.String, doc = (DocText.literal "field docs"), label = (SchemaSaladString.Literal "Sample"))
            let recordSchema = InputRecordSchema(fields = ListEntry.ofValues [| field |], doc = (DocText.literal "record docs"), name = "SampleRecord")
            let arraySchema = InputArraySchema(CWLType.Record recordSchema, label = (SchemaSaladString.Literal "array label"))
            let enumSchema = InputEnumSchema(ResizeArray [| SchemaSaladString.Literal "A"; SchemaSaladString.Literal "B" |], name = "Choice")
            let dirent = DirentInstance(SchemaSaladString.Literal "$(inputs.sample)", entryname = SchemaSaladString.Literal "sample.txt")
            let package = SoftwarePackage(SchemaSaladString.Literal "samtools", version = literals [ "1.19" ])

            Expect.sequenceEqual InputRecordField.KnownFieldNames (Set [| "name"; "type"; "doc"; "label" |]) ""
            Expect.sequenceEqual InputRecordSchema.KnownFieldNames (Set [| "type"; "fields"; "label"; "doc"; "name" |]) ""
            Expect.sequenceEqual InputArraySchema.KnownFieldNames (Set [| "type"; "items"; "label"; "doc"; "name" |]) ""
            Expect.sequenceEqual InputEnumSchema.KnownFieldNames (Set [| "type"; "symbols"; "label"; "doc"; "name" |]) ""
            Expect.sequenceEqual DirentInstance.KnownFieldNames (Set [| "entry"; "entryname"; "writable" |]) ""

            Expect.sequenceEqual SoftwarePackage.KnownFieldNames (Set [| "package"; "version"; "specs" |]) ""

            for dynObj in [
                field :> DynamicObj
                recordSchema :> DynamicObj
                arraySchema :> DynamicObj
                enumSchema :> DynamicObj
                dirent :> DynamicObj
                package :> DynamicObj
            ] do
                Expect.equal (dynObj |> DynamicObjHelpers.dynamicPropertiesSnapshot |> Seq.length) 0 "Known fields should not be stored in dynamic overflow."
                DynObj.setProperty "arc:note" "keep me" dynObj
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:note" dynObj) (Some "keep me") "Unknown fields should stay in dynamic overflow."

        testCase "requirement payloads keep known fields typed and overflow dynamic" <| fun _ ->
            let docker = DockerRequirement.create(dockerPull = (SchemaSaladString.Literal "ubuntu:24.04"))
            let env = EnvironmentDef(SchemaSaladString.Literal "PATH", SchemaSaladString.Literal "/usr/bin")
            let loadListing = LoadListingRequirementValue(DeepListing)
            let workReuse = WorkReuseRequirementValue(false)
            let network = NetworkAccessRequirementValue(false)
            let inplace = InplaceUpdateRequirementValue(SchemaSaladBool.Value false)
            let inlineJs = InlineJavascriptRequirementValue(expressionLib = ResizeArray [| SchemaSaladString.Literal "helper.js" |])
            let unknownHint = HintUnknownValue(Some "acme:Hint", { Node = Decode.readYaml "class: acme:Hint" })

            Expect.sequenceEqual DockerRequirement.KnownFieldNames (Set [| "class"; "dockerPull"; "dockerFile"; "dockerImageId"; "dockerLoad"; "dockerImport"; "dockerOutputDirectory"; "cwltool:dockerRunOptions" |]) ""
            Expect.sequenceEqual EnvironmentDef.KnownFieldNames (Set [| "envName"; "envValue" |]) ""
            Expect.sequenceEqual LoadListingRequirementValue.KnownFieldNames (Set [| "class"; "loadListing" |]) ""
            Expect.sequenceEqual WorkReuseRequirementValue.KnownFieldNames (Set [| "class"; "enableReuse" |]) ""
            Expect.sequenceEqual NetworkAccessRequirementValue.KnownFieldNames (Set [| "class"; "networkAccess" |]) ""
            Expect.sequenceEqual InplaceUpdateRequirementValue.KnownFieldNames (Set [| "class"; "inplaceUpdate" |]) ""
            Expect.sequenceEqual InlineJavascriptRequirementValue.KnownFieldNames (Set [| "class"; "expressionLib" |]) ""
            Expect.sequenceEqual HintUnknownValue.KnownFieldNames (Set [| "class"; "raw" |]) ""

            for dynObj in [
                docker :> DynamicObj
                env :> DynamicObj
                loadListing :> DynamicObj
                workReuse :> DynamicObj
                network :> DynamicObj
                inplace :> DynamicObj
                inlineJs :> DynamicObj
                unknownHint :> DynamicObj
            ] do
                Expect.equal (dynObj |> DynamicObjHelpers.dynamicPropertiesSnapshot |> Seq.length) 0 "Known fields should not be stored in dynamic overflow."
                DynObj.setProperty "arc:note" "keep me" dynObj
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:note" dynObj) (Some "keep me") "Unknown fields should stay in dynamic overflow."
    ]

let testRequirementDynamicOverflowRoundtrip =
    let encodeDecodeRequirement requirement =
        let encoded = Encode.encodeRequirement requirement |> Encode.writeYaml
        let document = "requirements:\n  - " + encoded.Replace("\n", "\n    ")
        let decoded =
            Decode.readYaml document
            |> Decode.requirementsDecoder
            |> Option.get
        decoded.[0]

    testList "Requirement DynamicObj overflow roundtrip" [
        testCase "payload and nested entry overflow survives encode/decode" <| fun _ ->
            let inlineJs = InlineJavascriptRequirementValue(expressionLib = ResizeArray [| SchemaSaladString.Literal "helper.js" |])
            DynObj.setProperty "arc:inline note" "inline" inlineJs

            let loadListing = LoadListingRequirementValue(DeepListing)
            DynObj.setProperty "arc:load note" "load" loadListing

            let workReuse = WorkReuseRequirementValue(false)
            DynObj.setProperty "arc:reuse note" "reuse" workReuse

            let network = NetworkAccessRequirementValue(true)
            DynObj.setProperty "arc:network note" "network" network

            let inplace = InplaceUpdateRequirementValue(SchemaSaladBool.Value true)
            DynObj.setProperty "arc:inplace note" "inplace" inplace

            let dirent = DirentInstance(SchemaSaladString.Literal "contents", entryname = SchemaSaladString.Literal "file.txt")
            DynObj.setProperty "arc:dirent note" "dirent" dirent

            let env = EnvironmentDef(SchemaSaladString.Literal "ENV_NAME", SchemaSaladString.Literal "ENV_VALUE")
            DynObj.setProperty "arc:env note" "env" env

            let package = SoftwarePackage(SchemaSaladString.Literal "samtools", version = literals [ "1.19" ])
            DynObj.setProperty "arc:package note" "package" package

            let recordField = InputRecordField("sample", CWLType.String)
            DynObj.setProperty "arc:field note" "field" recordField
            let recordSchema = InputRecordSchema(fields = ListEntry.ofValues [| recordField |], name = "SampleRecord")
            DynObj.setProperty "arc:record note" "record" recordSchema
            let schemaDef = SchemaDefinition.Record recordSchema

            let cases =
                [
                    InlineJavascriptRequirement inlineJs, fun requirement ->
                        match requirement with
                        | InlineJavascriptRequirement value -> DynObj.tryGetTypedPropertyValue<string> "arc:inline note" value
                        | _ -> None
                    LoadListingRequirement loadListing, fun requirement ->
                        match requirement with
                        | LoadListingRequirement value -> DynObj.tryGetTypedPropertyValue<string> "arc:load note" value
                        | _ -> None
                    WorkReuseRequirement workReuse, fun requirement ->
                        match requirement with
                        | WorkReuseRequirement value -> DynObj.tryGetTypedPropertyValue<string> "arc:reuse note" value
                        | _ -> None
                    NetworkAccessRequirement network, fun requirement ->
                        match requirement with
                        | NetworkAccessRequirement value -> DynObj.tryGetTypedPropertyValue<string> "arc:network note" value
                        | _ -> None
                    InplaceUpdateRequirement inplace, fun requirement ->
                        match requirement with
                        | InplaceUpdateRequirement value -> DynObj.tryGetTypedPropertyValue<string> "arc:inplace note" value
                        | _ -> None
                    InitialWorkDirRequirement (ResizeArray [| DirentEntry dirent |]), fun requirement ->
                        match requirement with
                        | InitialWorkDirRequirement listing ->
                            match listing.[0] with
                            | DirentEntry value -> DynObj.tryGetTypedPropertyValue<string> "arc:dirent note" value
                            | _ -> None
                        | _ -> None
                    EnvVarRequirement (ListEntry.ofValues [| env |]), fun requirement ->
                        match requirement with
                        | EnvVarRequirement envs -> DynObj.tryGetTypedPropertyValue<string> "arc:env note" (ListEntry.values envs).[0]
                        | _ -> None
                    SoftwareRequirement (ListEntry.ofValues [| package |]), fun requirement ->
                        match requirement with
                        | SoftwareRequirement packages -> DynObj.tryGetTypedPropertyValue<string> "arc:package note" (ListEntry.values packages).[0]
                        | _ -> None
                    SchemaDefRequirement (ResizeArray [| Defined schemaDef |]), fun requirement ->
                        match requirement with
                        | SchemaDefRequirement definitions ->
                            match definedOf definitions.[0] with
                            | SchemaDefinition.Record schema ->
                                let field = (InputRecordSchema.getFieldValuesOrEmpty schema).[0]
                                let schemaOverflow = DynObj.tryGetTypedPropertyValue<string> "arc:record note" schema
                                let fieldOverflow = DynObj.tryGetTypedPropertyValue<string> "arc:field note" field
                                if schemaOverflow = Some "record" && fieldOverflow = Some "field" then Some "schema"
                                else None
                            | _ -> None
                        | _ -> None
                ]

            for requirement, getOverflow in cases do
                let roundTripped = encodeDecodeRequirement requirement
                Expect.isSome (getOverflow roundTripped) $"Overflow should survive for %A{requirement}."
    ]

let main = 
    testList "Requirement" [
        testRequirementDecode
        testDecodeAllRequirementSyntaxes
        testRequirementEncode
        testSchemaDefTypeReferences
        testSchemaDefEncoding
        testInitialWorkDirFileDirectoryEntries
        testInitialWorkDirListingForms
        testSchemaSaladDirectivesInRequirements
        testInvalidRequirementsAndHints
        testDynamicPayloadModel
        testRequirementDynamicOverflowRoundtrip
    ]


