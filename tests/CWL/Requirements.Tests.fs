module Tests.Requirements

open ARCtrl.CWL
open YAMLicious
open YAMLicious.YAMLiciousTypes
open DynamicObj
open TestingUtils
open TestingUtils.CWL

open Fable.Pyxpecto
open TestObjects.CWL

let decodeRequirements (cwl: string) =
    cwl
    |> Decode.read
    |> Decode.requirementsDecoder
    |> fun r -> r.Value

let decodeHints (cwl: string) =
    cwl
    |> Decode.read
    |> Decode.hintsDecoder
    |> fun h -> h.Value

let findRequirement reqs predicate =
    reqs
    |> Seq.tryFind predicate
    |> Option.defaultWith (fun () -> failwith "Required requirement not found")

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

            testCase "Malformed known hint payload falls back to UnknownHint" <| fun _ ->
                let yaml = """hints:
  - class: DockerRequirement
    dockerPull:
      nested: invalid"""
                let hints = decodeHints yaml
                Expect.equal hints.Count 1 "Expected one hint entry"
                match hints.[0] with
                | UnknownHint unknownHint ->
                    Expect.equal unknownHint.Class (Some "DockerRequirement") "Malformed known hint should preserve class in UnknownHint"
                | _ ->
                    failwith "Expected UnknownHint fallback for malformed known hint payload"

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
                    let expectedExpressionLib = ResizeArray [| "$(function() { return 1; })"; "$(function() { return 2; })" |]
                    Expect.sequenceEqual actualExpressionLib expectedExpressionLib "expressionLib entries should decode"
                | _ ->
                    failwith "Expected InlineJavascriptRequirement"
            testCase "Decode scalar expressionLib form" <| fun _ ->
                let yaml = """requirements:
  - class: InlineJavascriptRequirement
    expressionLib: $(function() { return 42; })"""
                let reqs = decodeRequirements yaml
                let requirement = findRequirement reqs (function InlineJavascriptRequirement _ -> true | _ -> false)
                match requirement with
                | InlineJavascriptRequirement inlineJavascriptRequirement ->
                    let expressionLib = Expect.wantSome inlineJavascriptRequirement.ExpressionLib "Scalar expressionLib should normalize to a single entry array."
                    Expect.sequenceEqual expressionLib (ResizeArray [| "$(function() { return 42; })" |]) "Scalar expressionLib should be preserved."
                | _ ->
                    failwith "Expected InlineJavascriptRequirement"

            testCase "Encode emits expressionLib when present" <| fun _ ->
                let requirement =
                    InlineJavascriptRequirement (InlineJavascriptRequirementValue(expressionLib = ResizeArray [| "$(function() { return 1; })" |]))
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
                        DockerRequirement.create(dockerImageId = "devcontainer", dockerFileReference = SchemaSaladString.Include "FSharpArcCapsule/Dockerfile")
                    )
                let actual = dockerItem
                Expect.equal actual expected "Mismatch or Wrong requirement type: Type get of Decode Class Syntax for DockerRequirement, can only be DockerRequirement"
            testCase "Mapping Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsMappingFileContent
                let dockerItem = findRequirement reqs (function DockerRequirement _ -> true | _ -> false)
                let expected =
                    Requirement.DockerRequirement (
                        DockerRequirement.create(dockerImageId = "devcontainer", dockerFileReference = SchemaSaladString.Include "FSharpArcCapsule/Dockerfile")
                    )
                let actual = dockerItem
                Expect.equal actual expected "Mismatch or Wrong requirement type: Type get of Decode Mapping Syntax for DockerRequirement, can only be DockerRequirement"
            testCase "Json Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsJSONFileContent
                let dockerItem = findRequirement reqs (function DockerRequirement _ -> true | _ -> false)
                let expected =
                    Requirement.DockerRequirement (
                        DockerRequirement.create(dockerImageId = "devcontainer", dockerFileReference = SchemaSaladString.Include "FSharpArcCapsule/Dockerfile")
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
                        Decode.read document
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
                            DirentEntry (DirentInstance(SchemaSaladString.Literal "$(inputs.arcDirectory)", entryname = SchemaSaladString.Literal "arc", writable = true))
                            DirentEntry (DirentInstance(SchemaSaladString.Literal "$(inputs.outputDirectory)", writable = true))
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
                            DirentEntry (DirentInstance(SchemaSaladString.Literal "$(inputs.arcDirectory)", entryname = SchemaSaladString.Literal "arc", writable = true))
                            DirentEntry (DirentInstance(SchemaSaladString.Literal "$(inputs.outputDirectory)", writable = true))
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
                            DirentEntry (DirentInstance(SchemaSaladString.Literal "$(inputs.arcDirectory)", entryname = SchemaSaladString.Literal "arc", writable = true))
                            DirentEntry (DirentInstance(SchemaSaladString.Literal "$(inputs.outputDirectory)", writable = true))
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
            testCase "Scalar listing form decodes as single listing entry" <| fun _ ->
                let yaml = """requirements:
  - class: InitialWorkDirRequirement
    listing: $(inputs.stageDirectory)"""
                let reqs = decodeRequirements yaml
                let initialWorkDirItem = findRequirement reqs (function InitialWorkDirRequirement _ -> true | _ -> false)
                match initialWorkDirItem with
                | InitialWorkDirRequirement listing ->
                    let expected = ResizeArray [| StringEntry (SchemaSaladString.Literal "$(inputs.stageDirectory)") |]
                    Expect.sequenceEqual listing expected "Scalar listing form should decode to a single StringEntry."
                | _ ->
                    failwith "Wrong requirement type: expected InitialWorkDirRequirement"
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

            testCase "Entry field takes precedence over class field" <| fun _ ->
                let yaml = """requirements:
  - class: InitialWorkDirRequirement
    listing:
      - class: File
        entry: $(inputs.arcDirectory)
        writable: true"""
                let reqs = decodeRequirements yaml
                let initialWorkDirItem = findRequirement reqs (function InitialWorkDirRequirement _ -> true | _ -> false)
                match initialWorkDirItem with
                | InitialWorkDirRequirement listing ->
                    Expect.equal listing.Count 1 "Expected one listing entry"
                    match listing.[0] with
                    | DirentEntry dirent ->
                        Expect.equal dirent.Entry (SchemaSaladString.Literal "$(inputs.arcDirectory)") "entry should decode as Dirent entry"
                    | _ ->
                        failwith "Expected DirentEntry when both entry and class are present"
                | _ ->
                    failwith "Wrong requirement type: expected InitialWorkDirRequirement"
        ]
        testList "EnvVarRequirement" [
            testCase "Class Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsClassFileContent
                let envVarItem = findRequirement reqs (function EnvVarRequirement _ -> true | _ -> false)
                let expected = EnvVarRequirement (ResizeArray [| EnvironmentDef("DOTNET_NOLOGO", "true"); EnvironmentDef("TEST", "false") |])
                let actual = envVarItem
                match actual, expected with
                | EnvVarRequirement a, EnvVarRequirement e ->
                    Expect.sequenceEqual a e "EnvVarRequirement mismatch: Type of Decode Class Syntax for EnvVarRequirement"
                | _ ->
                    failwith "Wrong requirement type: Type of Decode Class Syntax for EnvVarRequirement can only be EnvVarRequirement"
            testCase "Mapping Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsMappingFileContent
                let envVarItem = findRequirement reqs (function EnvVarRequirement _ -> true | _ -> false)
                let expected = EnvVarRequirement (ResizeArray [| EnvironmentDef("DOTNET_NOLOGO", "true"); EnvironmentDef("TEST", "false") |])
                let actual = envVarItem
                match actual, expected with
                | EnvVarRequirement a, EnvVarRequirement e ->
                    Expect.sequenceEqual a e "EnvVarRequirement mismatch: Type of Decode Mapping Syntax for EnvVarRequirement"
                | _ ->
                    failwith "Wrong requirement type: Type of Decode Mapping Syntax for EnvVarRequirement can only be EnvVarRequirement"
            testCase "Json Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsJSONFileContent
                let envVarItem = findRequirement reqs (function EnvVarRequirement _ -> true | _ -> false)
                let expected = EnvVarRequirement (ResizeArray [| EnvironmentDef("DOTNET_NOLOGO", "true"); EnvironmentDef("TEST", "false") |])
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
      TEST: "false"""
                let reqs = decodeRequirements yaml
                let envVarItem = findRequirement reqs (function EnvVarRequirement _ -> true | _ -> false)
                match envVarItem with
                | EnvVarRequirement envs ->
                    let expected = ResizeArray [| EnvironmentDef("DOTNET_NOLOGO", "true"); EnvironmentDef("TEST", "false") |]
                    Expect.sequenceEqual envs expected "EnvVar map shorthand should decode to normalized EnvironmentDef list."
                | _ ->
                    failwith "Wrong requirement type: expected EnvVarRequirement"

            testCase "Map shorthand unquoted boolean decodes as string literal" <| fun _ ->
                let yaml = """requirements:
  - class: EnvVarRequirement
    envDef:
      FLAG: true"""
                let reqs = decodeRequirements yaml
                let envVarItem = findRequirement reqs (function EnvVarRequirement _ -> true | _ -> false)
                match envVarItem with
                | EnvVarRequirement envs ->
                    Expect.equal envs.Count 1 "Expected one map-shorthand environment definition"
                    Expect.equal envs.[0].EnvName "FLAG" "Map key should decode as env name"
                    Expect.equal envs.[0].EnvValue "true" "Unquoted YAML boolean should normalize to string value"
                | _ ->
                    failwith "Wrong requirement type: expected EnvVarRequirement"

            testCase "Map shorthand decode then default encode uses array form" <| fun _ ->
                let yaml = """requirements:
  - class: EnvVarRequirement
    envDef:
      DOTNET_NOLOGO: "true"
      TEST: "false"""
                let reqs = decodeRequirements yaml
                let envVarItem = findRequirement reqs (function EnvVarRequirement _ -> true | _ -> false)
                let encoded = Encode.encodeRequirement envVarItem |> Encode.writeYaml
                Expect.stringContains encoded "envDef:" "Encoded requirement should include envDef"
                Expect.stringContains encoded "envName: DOTNET_NOLOGO" "Default encoder should emit array-form envDef entries"
                Expect.stringContains encoded "envValue: \"true\"" "Boolean-like strings should remain quoted in default array encoder"

            testCase "Compact map encode helper emits envDef map" <| fun _ ->
                let envs = ResizeArray [| EnvironmentDef("DOTNET_NOLOGO", "true"); EnvironmentDef("TEST", "false") |]
                let encoded = Encode.encodeEnvVarRequirementCompactMap envs |> Encode.writeYaml
                Expect.stringContains encoded "class: EnvVarRequirement" "EnvVar compact helper should emit class"
                Expect.stringContains encoded "envDef:" "EnvVar compact helper should emit envDef map"
                Expect.stringContains encoded "DOTNET_NOLOGO" "EnvVar compact helper should emit map keys"
        ]
        testList "SoftwareRequirement" [
            testCase "Class Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsClassFileContent
                let softwareItem = findRequirement reqs (function SoftwareRequirement _ -> true | _ -> false)
                let expected = SoftwareRequirement (ResizeArray [| SoftwarePackage("interproscan", version = ResizeArray [| "5.21-60" |], specs = ResizeArray [| "https://identifiers.org/rrid/RRID:SCR_005829" |]) |])
                let actual = softwareItem
                match actual, expected with
                | SoftwareRequirement actualType, SoftwareRequirement expectedType ->
                    let a = actualType.[0]
                    let e = expectedType.[0]
                    Expect.equal a.Package e.Package $"SoftwareRequirement.Package mismatch. expected = '{e.Package}', actual = '{a.Package}'"
                    Expect.sequenceEqual a.Specs.Value e.Specs.Value $"SoftwareRequirement.Specs mismatch. expected = {e.Specs.Value}, actual = {a.Specs.Value}"
                    Expect.sequenceEqual a.Version.Value e.Version.Value $"SoftwareRequirement.Version mismatch. expected = {e.Version.Value}, actual = {a.Version.Value}"
                | _ ->
                    failwith "Wrong requirement type: Type of Decode Class Syntax for SoftwareRequirement can only be SoftwareRequirement"
            testCase "Mapping Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsMappingFileContent
                let softwareItem = findRequirement reqs (function SoftwareRequirement _ -> true | _ -> false)
                let expected = SoftwareRequirement (ResizeArray [| SoftwarePackage("interproscan", version = ResizeArray [| "5.21-60" |], specs = ResizeArray [| "https://identifiers.org/rrid/RRID:SCR_005829" |]) |])
                let actual = softwareItem
                match actual, expected with
                | SoftwareRequirement actualType, SoftwareRequirement expectedType ->
                    let a = actualType.[0]
                    let e = expectedType.[0]
                    Expect.equal a.Package e.Package $"SoftwareRequirement.Package mismatch. expected = '{e.Package}', actual = '{a.Package}'"
                    Expect.sequenceEqual a.Specs.Value e.Specs.Value $"SoftwareRequirement.Specs mismatch. expected = {e.Specs.Value}, actual = {a.Specs.Value}"
                    Expect.sequenceEqual a.Version.Value e.Version.Value $"SoftwareRequirement.Version mismatch. expected = {e.Version.Value}, actual = {a.Version.Value}"
                | _ ->
                    failwith "Wrong requirement type: Type of Decode Mapping Syntax for SoftwareRequirement can only be SoftwareRequirement"
            testCase "Json Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsJSONFileContent
                let softwareItem = findRequirement reqs (function SoftwareRequirement _ -> true | _ -> false)
                let expected = SoftwareRequirement (ResizeArray [| SoftwarePackage("interproscan", version = ResizeArray [| "5.21-60" |], specs = ResizeArray [| "https://identifiers.org/rrid/RRID:SCR_005829" |]) |])
                let actual = softwareItem
                match actual, expected with
                | SoftwareRequirement actualType, SoftwareRequirement expectedType ->
                    let a = actualType.[0]
                    let e = expectedType.[0]
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
                | SoftwareRequirement packages ->
                    Expect.equal packages.Count 1 "One package should decode from shorthand map"
                    Expect.equal packages.[0].Package "blast" "Package name should decode from map key"
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
                | SoftwareRequirement packages ->
                    Expect.equal packages.Count 1 "One package should decode from object map form"
                    let package = packages.[0]
                    Expect.equal package.Package "interproscan" "Package should decode from map key"
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
                | SoftwareRequirement packages ->
                    Expect.equal packages.Count 1 "One package should decode from version-only map form"
                    let package = packages.[0]
                    Expect.equal package.Package "interproscan" "Package should decode from map key"
                    Expect.isSome package.Version "Version should decode from map object"
                    Expect.isNone package.Specs "Specs should be absent for version-only map object"
                | _ ->
                    failwith "Wrong requirement type: expected SoftwareRequirement"

            testCase "Compact map encode helper emits packages map" <| fun _ ->
                let packages =
                    ResizeArray [|
                        SoftwarePackage(
                            "interproscan",
                            version = ResizeArray [| "5.21-60" |],
                            specs = ResizeArray [| "https://identifiers.org/rrid/RRID:SCR_005829" |]
                        )
                    |]
                let encoded = Encode.encodeSoftwareRequirementCompactMap packages |> Encode.writeYaml
                Expect.stringContains encoded "class: SoftwareRequirement" "Software compact helper should emit class"
                Expect.stringContains encoded "packages:" "Software compact helper should emit packages map"
                Expect.stringContains encoded "interproscan" "Software compact helper should emit package key"
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
                            dockerPull = "ghcr.io/example/tool:1.0.0",
                            dockerFileReference = SchemaSaladString.Literal "./Dockerfile",
                            dockerImageId = "tool-image",
                            dockerLoad = "docker-archive:///tmp/tool.tar",
                            dockerImport = "https://example.org/images/tool.sif",
                            dockerOutputDirectory = "/work/out"
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
            testCase "WorkReuse and InplaceUpdate default to true when payload omitted" <| fun _ ->
                let yaml = """requirements:
  - class: WorkReuse
  - class: InplaceUpdateRequirement"""
                let reqs = decodeRequirements yaml
                let workReuse = findRequirement reqs (function WorkReuseRequirement _ -> true | _ -> false)
                let inplace = findRequirement reqs (function InplaceUpdateRequirement _ -> true | _ -> false)
                Expect.equal workReuse (WorkReuseRequirement (WorkReuseRequirementValue(true))) "WorkReuse without explicit payload should default to true."
                Expect.equal inplace (InplaceUpdateRequirement (InplaceUpdateRequirementValue(true))) "InplaceUpdateRequirement without payload should default to true."
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
                Expect.equal inplace (InplaceUpdateRequirement (InplaceUpdateRequirementValue(false))) "InplaceUpdateRequirement inplaceUpdate=false should decode."
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
                let expected = ToolTimeLimitRequirement (ToolTimeLimitExpression "$(inputs.max_runtime_seconds)")
                Expect.equal requirement expected "Expression timelimit should decode to ToolTimeLimitExpression."
            testCase "Encode numeric timelimit as integer scalar" <| fun _ ->
                let requirement = ToolTimeLimitRequirement (ToolTimeLimitSeconds 300L)
                let encoded = Encode.encodeRequirement requirement |> Encode.writeYaml
                Expect.stringContains encoded "timelimit: 300" "Numeric timelimit should encode as integer scalar"

            testCase "Decode float timelimit as expression fallback" <| fun _ ->
                let yaml = """requirements:
  - class: ToolTimeLimit
    timelimit: 3.5"""
                let reqs = decodeRequirements yaml
                let requirement = findRequirement reqs (function ToolTimeLimitRequirement _ -> true | _ -> false)
                let expected = ToolTimeLimitRequirement (ToolTimeLimitExpression "3.5")
                Expect.equal requirement expected "Float timelimit should decode via expression fallback."
            testCase "Decode negative numeric timelimit fails" <| fun _ ->
                let yaml = """requirements:
  - class: ToolTimeLimit
    timelimit: -1"""
                Expect.throws
                    (fun _ -> decodeRequirements yaml |> ignore)
                    "Negative ToolTimeLimit value should fail decoding."
        ]
        testList "ResourceRequirement" [
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
                    let outdirMin = resourceRequirement.OutdirMin.Value :?> string
                    Expect.equal coresMin 2L "coresMin should decode to int64"
                    Expect.equal coresMax 922337203685477580L "coresMax should decode to int64"
                    Expect.equal ramMin 4.5 "ramMin should decode to float"
                    Expect.equal outdirMin "$(inputs.outdir_min)" "Expression strings should be preserved"
                    Expect.equal (resourceRequirement.TryGetInt64("coresMin")) (Some 2L) "Typed int64 getter should return normalized value"
                    Expect.equal (resourceRequirement.TryGetFloat("ramMin")) (Some 4.5) "Typed float getter should return normalized value"
                    Expect.equal (resourceRequirement.TryGetExpression("outdirMin")) (Some "$(inputs.outdir_min)") "Typed expression getter should return normalized value"
                | _ ->
                    failwith "Expected ResourceRequirement"

            testCase "known fields are typed fields, not dynamic overflow" <| fun _ ->
                let resourceRequirement =
                    ResourceRequirementInstance(coresMin = 2L, ramMin = 4.5, outdirMin = "$(inputs.outdir_min)")

                Expect.sequenceEqual
                    ResourceRequirementInstance.KnownFieldNames
                    (ResizeArray [| "class"; "coresMin"; "coresMax"; "ramMin"; "ramMax"; "tmpdirMin"; "tmpdirMax"; "outdirMin"; "outdirMax" |])
                    "ResourceRequirement known fields should be declared on the type."
                Expect.isEmpty
                    (resourceRequirement |> DynamicObjHelpers.dynamicPropertiesSnapshot)
                    "ResourceRequirement known fields should not be stored as dynamic properties."
                Expect.equal (resourceRequirement.TryGetInt64("coresMin")) (Some 2L) "Typed int64 getter should read known field."
                Expect.equal (resourceRequirement.TryGetFloat("ramMin")) (Some 4.5) "Typed float getter should read known field."
                Expect.equal (resourceRequirement.TryGetExpression("outdirMin")) (Some "$(inputs.outdir_min)") "Typed expression getter should read known field."

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
                    Decode.requirementArrayDecoder (YAMLElement.Object [ YAMLElement.Sequence [ encodedElement ] ])
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
            testCase "Decode legacy map style schema definition entries" <| fun _ ->
                let yaml = """requirements:
  - class: SchemaDefRequirement
    types:
      - SampleId: string"""
                let reqs = decodeRequirements yaml
                let requirement = findRequirement reqs (function SchemaDefRequirement _ -> true | _ -> false)
                match requirement with
                | SchemaDefRequirement definitions ->
                    let expected = ResizeArray [| SchemaDefRequirementType("SampleId", CWLType.String) |]
                    Expect.sequenceEqual definitions expected "Legacy map-style schema definitions should decode into explicit schema-def entries."
                | _ ->
                    failwith "Expected SchemaDefRequirement"
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
                    Expect.equal defs.[0].Name "SampleRecord" "Schema definition name should be preserved."
                    match defs.[0].Type_ with
                    | Record recordSchema ->
                        let fields = Expect.wantSome recordSchema.Fields "Record schema should keep fields."
                        Expect.equal fields.Count 1 "Record schema should contain one field."
                        Expect.equal fields.[0].Name "sampleName" "Field name should decode."
                        Expect.equal fields.[0].Type CWLType.String "Field type should decode."
                    | other ->
                        failwith $"Expected record schema type but got %A{other}"
                | _ ->
                    failwith "Expected SchemaDefRequirement"
        ]
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
                        DirentEntry (DirentInstance(SchemaSaladString.Literal "$(inputs.outputDirectory)", entryname = SchemaSaladString.Literal "outdir", writable = true))
                    |]
                let req = InitialWorkDirRequirement listing
                let yaml = Encode.encodeRequirement req |> Encode.writeYaml
                let indented = yaml.Replace("\n", "\n    ")
                let document = "requirements:\n  - " + indented
                let decoded =
                    Decode.read document
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
                        DirentEntry (DirentInstance(SchemaSaladString.Include "scripts/bootstrap.sh", entryname = SchemaSaladString.Import "scripts/name.txt", writable = false))
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
                    Decode.read document
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
                    Decode.read document
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
                            dockerPull = "ghcr.io/example/tool:1.0.0",
                            dockerFileReference = SchemaSaladString.Literal "./Dockerfile",
                            dockerImageId = "tool-image",
                            dockerLoad = "docker-archive:///tmp/tool.tar",
                            dockerImport = "https://example.org/images/tool.sif",
                            dockerOutputDirectory = "/work/out"
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
                    Decode.read document
                    |> Decode.requirementsDecoder
                    |> Option.get
                Expect.equal decoded.[0] requirement "dockerFile $import directive should roundtrip."
            testCase "LoadListingRequirement roundtrips" <| fun _ ->
                let requirement = LoadListingRequirement (LoadListingRequirementValue(DeepListing))
                let yaml = Encode.encodeRequirement requirement |> Encode.writeYaml
                let document = "requirements:\n  - " + yaml.Replace("\n", "\n    ")
                let decoded =
                    Decode.read document
                    |> Decode.requirementsDecoder
                    |> Option.get
                Expect.equal decoded.[0] requirement "LoadListingRequirement should roundtrip through encode/decode."
            testCase "WorkReuse/NetworkAccess/InplaceUpdate payloads roundtrip" <| fun _ ->
                let requirements =
                    ResizeArray [|
                        WorkReuseRequirement (WorkReuseRequirementValue(false))
                        NetworkAccessRequirement (NetworkAccessRequirementValue(false))
                        InplaceUpdateRequirement (InplaceUpdateRequirementValue(false))
                    |]
                let encodedLines =
                    requirements
                    |> Seq.map (fun requirement -> "  - " + (Encode.encodeRequirement requirement |> Encode.writeYaml).Replace("\n", "\n    "))
                    |> String.concat "\n"
                let document = "requirements:\n" + encodedLines
                let decoded =
                    Decode.read document
                    |> Decode.requirementsDecoder
                    |> Option.get
                Expect.sequenceEqual decoded requirements "Payload-bearing requirements should roundtrip."
            testCase "ToolTimeLimit expression form roundtrips" <| fun _ ->
                let requirement = ToolTimeLimitRequirement (ToolTimeLimitExpression "$(inputs.max_runtime_seconds)")
                let yaml = Encode.encodeRequirement requirement |> Encode.writeYaml
                let document = "requirements:\n  - " + yaml.Replace("\n", "\n    ")
                let decoded =
                    Decode.read document
                    |> Decode.requirementsDecoder
                    |> Option.get
                Expect.equal decoded.[0] requirement "Expression timelimit should roundtrip."
            testCase "SchemaDefRequirement roundtrips with explicit typed representation" <| fun _ ->
                let requirement =
                    SchemaDefRequirement (ResizeArray [| SchemaDefRequirementType("SampleId", CWLType.String) |])
                let yaml = Encode.encodeRequirement requirement |> Encode.writeYaml
                Expect.stringContains yaml "name: SampleId" "SchemaDefRequirement should encode canonical name field."
                Expect.stringContains yaml "type: string" "SchemaDefRequirement should encode canonical type field."
                Expect.isFalse (yaml.Contains("SampleId: string")) "Canonical encoding should avoid legacy map-style schema entries."
                let document = "requirements:\n  - " + yaml.Replace("\n", "\n    ")
                let decoded =
                    Decode.read document
                    |> Decode.requirementsDecoder
                    |> Option.get
                match decoded.[0] with
                | SchemaDefRequirement definitions ->
                    let expected = ResizeArray [| SchemaDefRequirementType("SampleId", CWLType.String) |]
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
            let file = FileInstance(path = "/tmp/input.txt", basename = "input.txt")
            let directory = DirectoryInstance(path = "/tmp/output", basename = "output")
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
                    Expect.equal file.Path (Some "/tmp/in.txt") "File path should decode as a typed field."
                    Expect.equal file.Basename (Some "in.txt") "File basename should decode as a typed field."
                    Expect.equal file.Checksum (Some "sha1$abc") "File checksum should decode as a typed field."
                    Expect.equal file.Size (Some 42L) "File size should decode as a typed field."
                    Expect.equal directory.Path (Some "/tmp/out") "Directory path should decode as a typed field."
                    Expect.equal directory.Basename (Some "out") "Directory basename should decode as a typed field."
                    Expect.isSome directory.Listing "Directory listing should decode as a typed field."
                    Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:file note" file) (Some "keep file overflow") "File extension should remain overflow."
                    Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:dir note" directory) (Some "keep dir overflow") "Directory extension should remain overflow."
                    Expect.isNone (DynObj.tryGetTypedPropertyValue<string> "path" file) "Known File fields should not be overflow."
                    Expect.isNone (DynObj.tryGetTypedPropertyValue<string> "path" directory) "Known Directory fields should not be overflow."
                | _ -> failwith "Expected File and Directory entries"
            | _ ->
                failwith "Expected InitialWorkDirRequirement"
    ]

let testDynamicPayloadModel =
    testList "DynamicObj payload model" [
        testCase "CWL schema payloads keep known fields typed and overflow dynamic" <| fun _ ->
            let field = InputRecordField("sampleName", CWLType.String, doc = "field docs", label = "Sample")
            let recordSchema = InputRecordSchema(fields = ResizeArray [| field |], doc = "record docs", name = "SampleRecord")
            let arraySchema = InputArraySchema(CWLType.Record recordSchema, label = "array label")
            let enumSchema = InputEnumSchema(ResizeArray [| "A"; "B" |], name = "Choice")
            let dirent = DirentInstance(SchemaSaladString.Literal "$(inputs.sample)", entryname = SchemaSaladString.Literal "sample.txt")
            let schemaDef = SchemaDefRequirementType("SampleRecord", CWLType.Record recordSchema)
            let package = SoftwarePackage("samtools", version = ResizeArray [| "1.19" |])

            Expect.sequenceEqual InputRecordField.KnownFieldNames (ResizeArray [| "name"; "type"; "doc"; "label" |]) ""
            Expect.sequenceEqual InputRecordSchema.KnownFieldNames (ResizeArray [| "type"; "fields"; "label"; "doc"; "name" |]) ""
            Expect.sequenceEqual InputArraySchema.KnownFieldNames (ResizeArray [| "type"; "items"; "label"; "doc"; "name" |]) ""
            Expect.sequenceEqual InputEnumSchema.KnownFieldNames (ResizeArray [| "type"; "symbols"; "label"; "doc"; "name" |]) ""
            Expect.sequenceEqual DirentInstance.KnownFieldNames (ResizeArray [| "entry"; "entryname"; "writable" |]) ""
            Expect.sequenceEqual SchemaDefRequirementType.KnownFieldNames (ResizeArray [| "name"; "type" |]) ""
            Expect.sequenceEqual SoftwarePackage.KnownFieldNames (ResizeArray [| "package"; "version"; "specs" |]) ""

            for dynObj in [
                field :> DynamicObj
                recordSchema :> DynamicObj
                arraySchema :> DynamicObj
                enumSchema :> DynamicObj
                dirent :> DynamicObj
                schemaDef :> DynamicObj
                package :> DynamicObj
            ] do
                Expect.equal (dynObj |> DynamicObjHelpers.dynamicPropertiesSnapshot |> Seq.length) 0 "Known fields should not be stored in dynamic overflow."
                DynObj.setProperty "arc:note" "keep me" dynObj
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:note" dynObj) (Some "keep me") "Unknown fields should stay in dynamic overflow."

        testCase "requirement payloads keep known fields typed and overflow dynamic" <| fun _ ->
            let docker = DockerRequirement.create(dockerPull = "ubuntu:24.04")
            let env = EnvironmentDef("PATH", "/usr/bin")
            let loadListing = LoadListingRequirementValue(DeepListing)
            let workReuse = WorkReuseRequirementValue(false)
            let network = NetworkAccessRequirementValue(false)
            let inplace = InplaceUpdateRequirementValue(false)
            let inlineJs = InlineJavascriptRequirementValue(expressionLib = ResizeArray [| "helper.js" |])
            let unknownHint = HintUnknownValue(Some "acme:Hint", Decode.read "class: acme:Hint")

            Expect.sequenceEqual DockerRequirement.KnownFieldNames (ResizeArray [| "class"; "dockerPull"; "dockerFile"; "dockerImageId"; "dockerLoad"; "dockerImport"; "dockerOutputDirectory"; "cwltool:dockerRunOptions" |]) ""
            Expect.sequenceEqual EnvironmentDef.KnownFieldNames (ResizeArray [| "envName"; "envValue" |]) ""
            Expect.sequenceEqual LoadListingRequirementValue.KnownFieldNames (ResizeArray [| "class"; "loadListing" |]) ""
            Expect.sequenceEqual WorkReuseRequirementValue.KnownFieldNames (ResizeArray [| "class"; "enableReuse" |]) ""
            Expect.sequenceEqual NetworkAccessRequirementValue.KnownFieldNames (ResizeArray [| "class"; "networkAccess" |]) ""
            Expect.sequenceEqual InplaceUpdateRequirementValue.KnownFieldNames (ResizeArray [| "class"; "inplaceUpdate" |]) ""
            Expect.sequenceEqual InlineJavascriptRequirementValue.KnownFieldNames (ResizeArray [| "class"; "expressionLib" |]) ""
            Expect.sequenceEqual HintUnknownValue.KnownFieldNames (ResizeArray [| "class"; "raw" |]) ""

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
            Decode.read document
            |> Decode.requirementsDecoder
            |> Option.get
        decoded.[0]

    testList "Requirement DynamicObj overflow roundtrip" [
        testCase "payload and nested entry overflow survives encode/decode" <| fun _ ->
            let inlineJs = InlineJavascriptRequirementValue(expressionLib = ResizeArray [| "helper.js" |])
            DynObj.setProperty "arc:inline note" "inline" inlineJs

            let loadListing = LoadListingRequirementValue(DeepListing)
            DynObj.setProperty "arc:load note" "load" loadListing

            let workReuse = WorkReuseRequirementValue(false)
            DynObj.setProperty "arc:reuse note" "reuse" workReuse

            let network = NetworkAccessRequirementValue(true)
            DynObj.setProperty "arc:network note" "network" network

            let inplace = InplaceUpdateRequirementValue(true)
            DynObj.setProperty "arc:inplace note" "inplace" inplace

            let dirent = DirentInstance(SchemaSaladString.Literal "contents", entryname = SchemaSaladString.Literal "file.txt")
            DynObj.setProperty "arc:dirent note" "dirent" dirent

            let env = EnvironmentDef("ENV_NAME", "ENV_VALUE")
            DynObj.setProperty "arc:env note" "env" env

            let package = SoftwarePackage("samtools", version = ResizeArray [| "1.19" |])
            DynObj.setProperty "arc:package note" "package" package

            let recordField = InputRecordField("sample", CWLType.String)
            DynObj.setProperty "arc:field note" "field" recordField
            let recordSchema = InputRecordSchema(fields = ResizeArray [| recordField |], name = "SampleRecord")
            DynObj.setProperty "arc:record note" "record" recordSchema
            let schemaDef = SchemaDefRequirementType("SampleRecord", CWLType.Record recordSchema)
            DynObj.setProperty "arc:schema note" "schema" schemaDef

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
                    EnvVarRequirement (ResizeArray [| env |]), fun requirement ->
                        match requirement with
                        | EnvVarRequirement envs -> DynObj.tryGetTypedPropertyValue<string> "arc:env note" envs.[0]
                        | _ -> None
                    SoftwareRequirement (ResizeArray [| package |]), fun requirement ->
                        match requirement with
                        | SoftwareRequirement packages -> DynObj.tryGetTypedPropertyValue<string> "arc:package note" packages.[0]
                        | _ -> None
                    SchemaDefRequirement (ResizeArray [| schemaDef |]), fun requirement ->
                        match requirement with
                        | SchemaDefRequirement definitions ->
                            match definitions.[0].Type_ with
                            | CWLType.Record schema ->
                                let field = schema.Fields.Value.[0]
                                let wrapperOverflow = DynObj.tryGetTypedPropertyValue<string> "arc:schema note" definitions.[0]
                                let schemaOverflow = DynObj.tryGetTypedPropertyValue<string> "arc:record note" schema
                                let fieldOverflow = DynObj.tryGetTypedPropertyValue<string> "arc:field note" field
                                if wrapperOverflow = Some "schema" && schemaOverflow = Some "record" && fieldOverflow = Some "field" then Some "schema"
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
        testInitialWorkDirFileDirectoryEntries
        testDynamicPayloadModel
        testRequirementDynamicOverflowRoundtrip
    ]


