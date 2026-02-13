module Tests.Requirements

open ARCtrl.CWL
open YAMLicious
open TestingUtils
open TestingUtils.CWL

open Fable.Pyxpecto
open TestObjects.CWL

let decodeRequirements (cwl: string) =
    cwl
    |> Decode.read
    |> Decode.requirementsDecoder
    |> fun r -> r.Value

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
        testList "DockerRequirement" [
            testCase "Class Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsClassFileContent
                let dockerItem = findRequirement reqs (function DockerRequirement _ -> true | _ -> false)
                let expected =
                    DockerRequirement {
                        DockerPull = None
                        DockerFile = Some (Include "FSharpArcCapsule/Dockerfile")
                        DockerImageId = Some "devcontainer"
                        DockerLoad = None
                        DockerImport = None
                        DockerOutputDirectory = None
                    }
                let actual = dockerItem
                Expect.equal actual expected "Mismatch or Wrong requirement type: Type get of Decode Class Syntax for DockerRequirement, can only be DockerRequirement"
            testCase "Mapping Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsMappingFileContent
                let dockerItem = findRequirement reqs (function DockerRequirement _ -> true | _ -> false)
                let expected =
                    DockerRequirement {
                        DockerPull = None
                        DockerFile = Some (Include "FSharpArcCapsule/Dockerfile")
                        DockerImageId = Some "devcontainer"
                        DockerLoad = None
                        DockerImport = None
                        DockerOutputDirectory = None
                    }
                let actual = dockerItem
                Expect.equal actual expected "Mismatch or Wrong requirement type: Type get of Decode Mapping Syntax for DockerRequirement, can only be DockerRequirement"
            testCase "Json Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsJSONFileContent
                let dockerItem = findRequirement reqs (function DockerRequirement _ -> true | _ -> false)
                let expected =
                    DockerRequirement {
                        DockerPull = None
                        DockerFile = Some (Include "FSharpArcCapsule/Dockerfile")
                        DockerImageId = Some "devcontainer"
                        DockerLoad = None
                        DockerImport = None
                        DockerOutputDirectory = None
                    }
                let actual = dockerItem
                Expect.equal actual expected "Mismatch or Wrong requirement type: Type get of Decode Json Syntax for DockerRequirement, can only be DockerRequirement"
        ]
        testList "InitialWorkDirRequirement" [
            testCase "Class Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsClassFileContent
                let initialWorkDirItem = findRequirement reqs (function InitialWorkDirRequirement _ -> true | _ -> false)
                let expected =
                    InitialWorkDirRequirement (
                        ResizeArray [|
                            DirentEntry { Entryname = Some (Literal "arc"); Entry = Literal "$(inputs.arcDirectory)"; Writable = Some true }
                            DirentEntry { Entryname = None; Entry = Literal "$(inputs.outputDirectory)"; Writable = Some true }
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
                            DirentEntry { Entryname = Some (Literal "arc"); Entry = Literal "$(inputs.arcDirectory)"; Writable = Some true }
                            DirentEntry { Entryname = None; Entry = Literal "$(inputs.outputDirectory)"; Writable = Some true }
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
                            DirentEntry { Entryname = Some (Literal "arc"); Entry = Literal "$(inputs.arcDirectory)"; Writable = Some true }
                            DirentEntry { Entryname = None; Entry = Literal "$(inputs.outputDirectory)"; Writable = Some true }
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
                            StringEntry (Literal "$(inputs.stageDirectory)")
                            StringEntry (Literal "$(inputs.outputDirectory)")
                        |]
                    Expect.sequenceEqual listing expected "String listing entries should decode into StringEntry values."
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
                    let expected = ResizeArray [| StringEntry (Include "scripts/dirent.js") |]
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
                    let expected = ResizeArray [| StringEntry (Import "scripts/dirent.js") |]
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
                            DirentEntry {
                                Entry = Include "scripts/bootstrap.sh"
                                Entryname = Some (Literal "script-name.txt")
                                Writable = None
                            }
                        |]
                    Expect.sequenceEqual listing expected "Dirent entry include wrapper should preserve directive kind."
                | _ ->
                    failwith "Wrong requirement type: expected InitialWorkDirRequirement"
        ]
        testList "EnvVarRequirement" [
            testCase "Class Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsClassFileContent
                let envVarItem = findRequirement reqs (function EnvVarRequirement _ -> true | _ -> false)
                let expected = EnvVarRequirement (ResizeArray [|{EnvName = "DOTNET_NOLOGO"; EnvValue = "true"}; {EnvName = "TEST"; EnvValue = "false"}|])
                let actual = envVarItem
                match actual, expected with
                | EnvVarRequirement a, EnvVarRequirement e ->
                    Expect.sequenceEqual a e "EnvVarRequirement mismatch: Type of Decode Class Syntax for EnvVarRequirement"
                | _ ->
                    failwith "Wrong requirement type: Type of Decode Class Syntax for EnvVarRequirement can only be EnvVarRequirement"
            testCase "Mapping Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsMappingFileContent
                let envVarItem = findRequirement reqs (function EnvVarRequirement _ -> true | _ -> false)
                let expected = EnvVarRequirement (ResizeArray [|{EnvName = "DOTNET_NOLOGO"; EnvValue = "true"}; {EnvName = "TEST"; EnvValue = "false"}|])
                let actual = envVarItem
                match actual, expected with
                | EnvVarRequirement a, EnvVarRequirement e ->
                    Expect.sequenceEqual a e "EnvVarRequirement mismatch: Type of Decode Mapping Syntax for EnvVarRequirement"
                | _ ->
                    failwith "Wrong requirement type: Type of Decode Mapping Syntax for EnvVarRequirement can only be EnvVarRequirement"
            testCase "Json Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsJSONFileContent
                let envVarItem = findRequirement reqs (function EnvVarRequirement _ -> true | _ -> false)
                let expected = EnvVarRequirement (ResizeArray [|{EnvName = "DOTNET_NOLOGO"; EnvValue = "true"}; {EnvName = "TEST"; EnvValue = "false"}|])
                let actual = envVarItem
                match actual, expected with
                | EnvVarRequirement a, EnvVarRequirement e ->
                    Expect.sequenceEqual a e "EnvVarRequirement mismatch: Type of Decode Json Syntax for EnvVarRequirement"
                | _ ->
                    failwith "Wrong requirement type: Type of Decode Json Syntax for EnvVarRequirement can only be EnvVarRequirement"
        ]
        testList "SoftwareRequirement" [
            testCase "Class Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsClassFileContent
                let softwareItem = findRequirement reqs (function SoftwareRequirement _ -> true | _ -> false)
                let expected = SoftwareRequirement (ResizeArray [|{Package = "interproscan"; Specs = Some (ResizeArray [| "https://identifiers.org/rrid/RRID:SCR_005829" |]); Version = Some (ResizeArray[| "5.21-60" |])}|])
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
                let expected = SoftwareRequirement (ResizeArray [|{Package = "interproscan"; Specs = Some (ResizeArray [| "https://identifiers.org/rrid/RRID:SCR_005829" |]); Version = Some (ResizeArray[| "5.21-60" |])}|])
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
                let expected = SoftwareRequirement (ResizeArray [|{Package = "interproscan"; Specs = Some (ResizeArray [| "https://identifiers.org/rrid/RRID:SCR_005829" |]); Version = Some (ResizeArray[| "5.21-60" |])}|])
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
        ]
        testList "NetworkAccess" [
            testCase "Class Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsClassFileContent
                let networkAccessItem = findRequirement reqs (function NetworkAccessRequirement _ -> true | _ -> false)
                let expected = NetworkAccessRequirement { NetworkAccess = true }
                let actual = networkAccessItem
                Expect.equal actual expected "Type of Decode Classs Syntax for NetworkAccess, Requirement can only be NetworkAccess"
            testCase "Mapping Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsMappingFileContent
                let networkAccessItem = findRequirement reqs (function NetworkAccessRequirement _ -> true | _ -> false)
                let expected = NetworkAccessRequirement { NetworkAccess = true }
                let actual = networkAccessItem
                Expect.equal actual expected "Type of Decode Mapping Syntax for NetworkAccess, Requirement can only be NetworkAccess"
            testCase "Json Syntax" <| fun _ ->
                let reqs = decodeRequirements TestObjects.CWL.Requirements.requirementsJSONFileContent
                let networkAccessItem = findRequirement reqs (function NetworkAccessRequirement _ -> true | _ -> false)
                let expected = NetworkAccessRequirement { NetworkAccess = true }
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
                    DockerRequirement {
                        DockerPull = Some "ghcr.io/example/tool:1.0.0"
                        DockerFile = Some (Literal "./Dockerfile")
                        DockerImageId = Some "tool-image"
                        DockerLoad = Some "docker-archive:///tmp/tool.tar"
                        DockerImport = Some "https://example.org/images/tool.sif"
                        DockerOutputDirectory = Some "/work/out"
                    }
                Expect.equal dockerItem expected "Canonical docker fields should decode into the typed DockerRequirement model."
            testCase "Decode dockerFile $import wrapper and preserve directive kind" <| fun _ ->
                let yaml = """requirements:
  - class: DockerRequirement
    dockerFile:
      $import: ./Dockerfile"""
                let reqs = decodeRequirements yaml
                let dockerItem = findRequirement reqs (function DockerRequirement _ -> true | _ -> false)
                let expected =
                    DockerRequirement {
                        DockerPull = None
                        DockerFile = Some (Import "./Dockerfile")
                        DockerImageId = None
                        DockerLoad = None
                        DockerImport = None
                        DockerOutputDirectory = None
                    }
                Expect.equal dockerItem expected "dockerFile $import wrapper should decode into Import and survive type mapping."
            testCase "Decode dockerFile map with both $include and $import prefers $include" <| fun _ ->
                let yaml = """requirements:
  - class: DockerRequirement
    dockerFile:
      $include: ./Dockerfile.include
      $import: ./Dockerfile.import"""
                let reqs = decodeRequirements yaml
                let dockerItem = findRequirement reqs (function DockerRequirement _ -> true | _ -> false)
                let expected =
                    DockerRequirement {
                        DockerPull = None
                        DockerFile = Some (Include "./Dockerfile.include")
                        DockerImageId = None
                        DockerLoad = None
                        DockerImport = None
                        DockerOutputDirectory = None
                    }
                Expect.equal dockerItem expected "Malformed dockerFile maps with both directives should deterministically prefer $include."
        ]
        testList "LoadListingRequirement" [
            testCase "Class Syntax" <| fun _ ->
                let yaml = """requirements:
  - class: LoadListingRequirement
    loadListing: deep_listing"""
                let reqs = decodeRequirements yaml
                let requirement = findRequirement reqs (function LoadListingRequirement _ -> true | _ -> false)
                let expected = LoadListingRequirement { LoadListing = "deep_listing" }
                Expect.equal requirement expected "Class-array syntax should decode LoadListingRequirement payload."
            testCase "Mapping Syntax" <| fun _ ->
                let yaml = """requirements:
  LoadListingRequirement:
    loadListing: shallow_listing"""
                let reqs = decodeRequirements yaml
                let requirement = findRequirement reqs (function LoadListingRequirement _ -> true | _ -> false)
                let expected = LoadListingRequirement { LoadListing = "shallow_listing" }
                Expect.equal requirement expected "Mapping syntax should decode LoadListingRequirement payload."
            testCase "Json Syntax" <| fun _ ->
                let yaml = """requirements: { LoadListingRequirement: { loadListing: "no_listing" } }"""
                let reqs = decodeRequirements yaml
                let requirement = findRequirement reqs (function LoadListingRequirement _ -> true | _ -> false)
                let expected = LoadListingRequirement { LoadListing = "no_listing" }
                Expect.equal requirement expected "JSON object syntax should decode LoadListingRequirement payload."
            testCase "Default value when field omitted" <| fun _ ->
                let yaml = """requirements:
  - class: LoadListingRequirement"""
                let reqs = decodeRequirements yaml
                let requirement = findRequirement reqs (function LoadListingRequirement _ -> true | _ -> false)
                let expected = LoadListingRequirement { LoadListing = "no_listing" }
                Expect.equal requirement expected "Missing loadListing should default to no_listing."
        ]
        testList "Payloaded requirement defaults and custom values" [
            testCase "WorkReuse and InplaceUpdate default to true when payload omitted" <| fun _ ->
                let yaml = """requirements:
  - class: WorkReuse
  - class: InplaceUpdateRequirement"""
                let reqs = decodeRequirements yaml
                let workReuse = findRequirement reqs (function WorkReuseRequirement _ -> true | _ -> false)
                let inplace = findRequirement reqs (function InplaceUpdateRequirement _ -> true | _ -> false)
                Expect.equal workReuse (WorkReuseRequirement { EnableReuse = true }) "WorkReuse without explicit payload should default to true."
                Expect.equal inplace (InplaceUpdateRequirement { InplaceUpdate = true }) "InplaceUpdateRequirement without payload should default to true."
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
                Expect.equal workReuse (WorkReuseRequirement { EnableReuse = false }) "WorkReuse enableReuse=false should decode."
                Expect.equal network (NetworkAccessRequirement { NetworkAccess = false }) "NetworkAccess networkAccess=false should decode."
                Expect.equal inplace (InplaceUpdateRequirement { InplaceUpdate = false }) "InplaceUpdateRequirement inplaceUpdate=false should decode."
        ]
        testList "ToolTimeLimitRequirement" [
            testCase "Decode numeric timelimit" <| fun _ ->
                let yaml = """requirements:
  - class: ToolTimeLimit
    timelimit: 120"""
                let reqs = decodeRequirements yaml
                let requirement = findRequirement reqs (function ToolTimeLimitRequirement _ -> true | _ -> false)
                let expected = ToolTimeLimitRequirement (ToolTimeLimitSeconds 120.0)
                Expect.equal requirement expected "Numeric timelimit should decode to ToolTimeLimitSeconds."
            testCase "Decode expression timelimit" <| fun _ ->
                let yaml = """requirements:
  - class: ToolTimeLimit
    timelimit: $(inputs.max_runtime_seconds)"""
                let reqs = decodeRequirements yaml
                let requirement = findRequirement reqs (function ToolTimeLimitRequirement _ -> true | _ -> false)
                let expected = ToolTimeLimitRequirement (ToolTimeLimitExpression "$(inputs.max_runtime_seconds)")
                Expect.equal requirement expected "Expression timelimit should decode to ToolTimeLimitExpression."
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
                    let expected = ResizeArray [| { Name = "SampleId"; Type_ = CWLType.String } |]
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
                        StringEntry (Literal "$(inputs.stageDirectory)")
                        DirentEntry { Entry = Literal "$(inputs.outputDirectory)"; Entryname = Some (Literal "outdir"); Writable = Some true }
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
                        StringEntry (Include "scripts/load.js")
                        StringEntry (Import "scripts/manifest.yml")
                        DirentEntry { Entry = Include "scripts/bootstrap.sh"; Entryname = Some (Import "scripts/name.txt"); Writable = Some false }
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
        ]
        testList "Canonical requirement encoding" [
            testCase "DockerRequirement.create prefers dockerFileReference over dockerFile" <| fun _ ->
                let created =
                    DockerRequirement.create(
                        dockerFile = "./Dockerfile.literal",
                        dockerFileReference = Include "./Dockerfile.include"
                    )
                Expect.equal created.DockerFile (Some (Include "./Dockerfile.include")) "dockerFileReference should take precedence when both inputs are provided."
            testCase "DockerRequirement encodes dockerFile as canonical string and includes extended fields" <| fun _ ->
                let requirement =
                    DockerRequirement {
                        DockerPull = Some "ghcr.io/example/tool:1.0.0"
                        DockerFile = Some (Literal "./Dockerfile")
                        DockerImageId = Some "tool-image"
                        DockerLoad = Some "docker-archive:///tmp/tool.tar"
                        DockerImport = Some "https://example.org/images/tool.sif"
                        DockerOutputDirectory = Some "/work/out"
                    }
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
            testCase "createFromLegacyMap keeps first recognized directive and drops extra keys" <| fun _ ->
                let legacy =
                    DockerRequirement.createFromLegacyMap(
                        dockerImageId = "tool-image",
                        dockerFileMap = Map [ "$include", "./Dockerfile"; "version", "latest" ]
                    )
                let encoded = Encode.encodeRequirement (DockerRequirement legacy) |> Encode.writeYaml
                Expect.stringContains encoded "$include: ./Dockerfile" "Legacy map should preserve include syntax during re-encode."
                Expect.isFalse (encoded.Contains("version: latest")) "Non-canonical legacy keys should be dropped during normalization."
            testCase "DockerRequirement $import syntax roundtrips" <| fun _ ->
                let requirement =
                    DockerRequirement {
                        DockerPull = None
                        DockerFile = Some (Import "./Dockerfile")
                        DockerImageId = None
                        DockerLoad = None
                        DockerImport = None
                        DockerOutputDirectory = None
                    }
                let yaml = Encode.encodeRequirement requirement |> Encode.writeYaml
                Expect.stringContains yaml "$import: ./Dockerfile" "dockerFile should preserve $import directive during encode."
                let document = "requirements:\n  - " + yaml.Replace("\n", "\n    ")
                let decoded =
                    Decode.read document
                    |> Decode.requirementsDecoder
                    |> Option.get
                Expect.equal decoded.[0] requirement "dockerFile $import directive should roundtrip."
            testCase "LoadListingRequirement roundtrips" <| fun _ ->
                let requirement = LoadListingRequirement { LoadListing = "deep_listing" }
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
                        WorkReuseRequirement { EnableReuse = false }
                        NetworkAccessRequirement { NetworkAccess = false }
                        InplaceUpdateRequirement { InplaceUpdate = false }
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
                    SchemaDefRequirement (ResizeArray [| { Name = "SampleId"; Type_ = CWLType.String } |])
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
                    let expected = ResizeArray [| { Name = "SampleId"; Type_ = CWLType.String } |]
                    Expect.sequenceEqual definitions expected "SchemaDefRequirement should roundtrip with explicit Name/Type_ entries."
                | _ ->
                    failwith "Expected SchemaDefRequirement"
        ]
    ]


let main = 
    testList "Requirement" [
        testRequirementDecode
        testDecodeAllRequirementSyntaxes
        testRequirementEncode
    ]
