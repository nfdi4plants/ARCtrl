import { describe, expect, it } from "vitest";
import { ofArray } from "@fable-org/fable-library-ts/List.ts";
import { CWL } from "../../src/ARCtrl/index.ts";
import {
    BlockScalarStyle, ChompingMode, ScalarStyle, YAMLContent, YAMLElement,
    type YAMLElement_$cases,
} from "../../src/ARCtrl/ts/fable_modules/YAMLicious.1.0.0/YAMLiciousTypes.fs.ts";

const {
    SchemaSaladString, CWLExpressionToolDescription, CWLOperationDescription, OutputSource,
    HintEntry, HintUnknownValue, InitialWorkDirEntry, InlineJavascriptRequirementValue,
    InplaceUpdateRequirementValue, LoadListingEnum, LoadListingRequirementValue,
    NetworkAccessRequirementValue, ToolTimeLimitValue, WorkReuseRequirementValue,
    LinkMergeMethod, PickValueMethod, ScatterMethod, StepOutputParameter, WorkflowStepRun,
} = CWL;

// Expected public properties are listed explicitly, independently of generated storage names.
function expectProperties(actual: object, expected: Record<string, unknown>) {
    for (const [property, value] of Object.entries(expected)) {
        expect((actual as Record<string, unknown>)[property], property).toEqual(value);
    }
}

const stringType = CWL.CWLType.string();
const yamlContent = new YAMLContent("sample.txt", undefined, undefined, undefined, undefined);
const yamlValue = new YAMLElement(1, [yamlContent]);
const literal = SchemaSaladString.literal("Hello");
const include = SchemaSaladString.include("message.txt");
const file = new CWL.FileInstance("file:///data/sample.txt", "/data/sample.txt");
const directory = new CWL.DirectoryInstance("file:///data", "/data");
const dirent = new CWL.DirentInstance(literal);
const listing = LoadListingEnum.deepListing();
const input = new CWL.CWLInput("message", stringType);
const output = new CWL.CWLOutput("result", stringType);
const inputBinding = new CWL.InputBinding("--message", 2);
const outputBinding = new CWL.OutputBinding("result.txt");
const requirement = CWL.Requirement.dockerRequirement(new CWL.DockerRequirement("alpine:3"));
const hint = HintEntry.knownHint(requirement);
const run = WorkflowStepRun.runString("tool.cwl");
const stepInput = new CWL.StepInput("message");
const stepOutput = CWL.StepOutput.stepOutputString("result");
const step = new CWL.WorkflowStep("echo", [stepInput], [stepOutput], run);
const tool = new CWL.CWLToolDescription([output]);
const workflow = new CWL.CWLWorkflowDescription([step], [input], [output]);
const expressionTool = new CWLExpressionToolDescription([output], "$(inputs.message)");
const operation = new CWLOperationDescription([input], [output]);

describe("CWL primary constructors: files and schemas", () => {
    it("FileInstance accepts omitted options and every positional field", () => {
        const absent = {
            Location: undefined, Path: undefined, Basename: undefined, Dirname: undefined,
            Nameroot: undefined, Nameext: undefined, Checksum: undefined, Size: undefined,
            SecondaryFiles: undefined, Format: undefined, Contents: undefined,
        };
        expectProperties(new CWL.FileInstance(), absent);
        const actual = new CWL.FileInstance(
            "file:///data/input.txt", "/data/input.txt", "input.txt", "/data", "input", ".txt",
            "sha1$abc", 42n, yamlValue, "text/plain", "contents",
        );
        expectProperties(actual, {
            Location: "file:///data/input.txt", Path: "/data/input.txt", Basename: "input.txt",
            Dirname: "/data", Nameroot: "input", Nameext: ".txt", Checksum: "sha1$abc", Size: 42n,
            SecondaryFiles: yamlValue, Format: "text/plain", Contents: "contents",
        });
    });

    it("DirectoryInstance accepts omitted options and a YAML listing", () => {
        expectProperties(new CWL.DirectoryInstance(), {
            Location: undefined, Path: undefined, Basename: undefined, Listing: undefined,
        });
        expectProperties(new CWL.DirectoryInstance("file:///data", "/data", "data", yamlValue), {
            Location: "file:///data", Path: "/data", Basename: "data", Listing: yamlValue,
        });
    });

    it("DirentInstance keeps literal/directive values and false writable", () => {
        expectProperties(new CWL.DirentInstance(literal), {
            Entry: literal, Entryname: undefined, Writable: undefined,
        });
        expectProperties(new CWL.DirentInstance(include, literal, false), {
            Entry: include, Entryname: literal, Writable: false,
        });
    });

    it("InputEnumSchema accepts required symbols and optional annotations", () => {
        expectProperties(new CWL.InputEnumSchema([]), {
            Symbols: [], Label: undefined, Doc: undefined, Name: undefined,
        });
        expectProperties(new CWL.InputEnumSchema(["red", "blue"], "Colour", "Allowed colours", "ColourType"), {
            Symbols: ["red", "blue"], Label: "Colour", Doc: "Allowed colours", Name: "ColourType",
        });
    });

    it("InputRecordField keeps its nested type and annotations", () => {
        expectProperties(new CWL.InputRecordField("message", stringType), {
            Name: "message", Type: stringType, Doc: undefined, Label: undefined,
        });
        expectProperties(new CWL.InputRecordField("message", stringType, "A message", "Message"), {
            Name: "message", Type: stringType, Doc: "A message", Label: "Message",
        });
    });

    it("InputRecordSchema accepts absent fields and populated fields", () => {
        expectProperties(new CWL.InputRecordSchema(), {
            Fields: undefined, Label: undefined, Doc: undefined, Name: undefined,
        });
        const fields = [new CWL.InputRecordField("message", stringType)];
        expectProperties(new CWL.InputRecordSchema(fields, "Record", "Record documentation", "MessageRecord"), {
            Fields: fields, Label: "Record", Doc: "Record documentation", Name: "MessageRecord",
        });
    });

    it("InputArraySchema accepts required items and optional annotations", () => {
        expectProperties(new CWL.InputArraySchema(stringType), {
            Items: stringType, Label: undefined, Doc: undefined, Name: undefined,
        });
        expectProperties(new CWL.InputArraySchema(stringType, "Messages", "Array documentation", "MessagesType"), {
            Items: stringType, Label: "Messages", Doc: "Array documentation", Name: "MessagesType",
        });
    });

    it("SchemaDefRequirementType keeps both required arguments", () => {
        expectProperties(new CWL.SchemaDefRequirementType("Message", stringType), {
            Name: "Message", Type_: stringType,
        });
    });

    it("SoftwarePackage accepts a name alone and version/specification arrays", () => {
        expectProperties(new CWL.SoftwarePackage("echo"), { Package: "echo", Version: undefined, Specs: undefined });
        expectProperties(new CWL.SoftwarePackage("echo", ["1.0", "2.0"], ["https://example.org/echo"]), {
            Package: "echo", Version: ["1.0", "2.0"], Specs: ["https://example.org/echo"],
        });
    });
});

describe("CWL primary constructors: inputs, outputs and parameters", () => {
    it.each([
        [true, false, false],
        [false, true, false],
        [false, false, true],
    ])("InputBinding keeps Separate=%s, LoadContents=%s, ShellQuote=%s independent", (separate, loadContents, shellQuote) => {
        expectProperties(new CWL.InputBinding(undefined, undefined, undefined, separate, loadContents, undefined, shellQuote), {
            Separate: separate, LoadContents: loadContents, ShellQuote: shellQuote,
        });
    });

    it.each([
        [true, false, false],
        [false, true, false],
        [false, false, true],
    ])("CWLInput keeps Optional=%s, Streamable=%s, LoadContents=%s independent", (optional, streamable, loadContents) => {
        expectProperties(new CWL.CWLInput(
            "message", undefined, undefined, optional, undefined, undefined, streamable,
            undefined, undefined, loadContents,
        ), { Optional: optional, Streamable: streamable, LoadContents: loadContents });
    });

    it("InputBinding accepts omitted options and every positional field", () => {
        const absent = {
            Prefix: undefined, _position: undefined, ItemSeparator: undefined, Separate: undefined,
            LoadContents: undefined, ValueFrom: undefined, ShellQuote: undefined,
        };
        expectProperties(new CWL.InputBinding(), absent);
        // Position has only a setter in the generated API; its stored constructor value is observable here.
        expectProperties(new CWL.InputBinding("--message", 0, ",", false, true, "$(self)", false), {
            Prefix: "--message", _position: 0, ItemSeparator: ",", Separate: false,
            LoadContents: true, ValueFrom: "$(self)", ShellQuote: false,
        });
    });

    it("CWLInput keeps every positional option, including false values and YAML defaults", () => {
        expectProperties(new CWL.CWLInput("message"), {
            Name: "message", Type_: undefined, InputBinding: undefined, Optional: undefined,
            Label: undefined, SecondaryFiles: undefined, Streamable: undefined, Doc: undefined,
            Format: undefined, LoadContents: undefined, LoadListing: undefined, DefaultValue: undefined,
        });
        expectProperties(new CWL.CWLInput(
            "message", stringType, inputBinding, false, "Message", yamlValue, false,
            "Input documentation", "text/plain", true, listing, yamlValue,
        ), {
            Name: "message", Type_: stringType, InputBinding: inputBinding, Optional: false,
            Label: "Message", SecondaryFiles: yamlValue, Streamable: false, Doc: "Input documentation",
            Format: "text/plain", LoadContents: true, LoadListing: listing, DefaultValue: yamlValue,
        });
    });

    it("OutputBinding accepts omitted options and every positional field", () => {
        expectProperties(new CWL.OutputBinding(), {
            Glob: undefined, LoadContents: undefined, LoadListing: undefined, OutputEval: undefined,
        });
        expectProperties(new CWL.OutputBinding("*.txt", false, listing, "$(self[0])"), {
            Glob: "*.txt", LoadContents: false, LoadListing: listing, OutputEval: "$(self[0])",
        });
    });

    it("CWLOutput keeps every positional option", () => {
        expectProperties(new CWL.CWLOutput("result"), {
            Name: "result", Type_: undefined, OutputBinding: undefined, OutputSource: undefined,
            Label: undefined, SecondaryFiles: undefined, Streamable: undefined, Doc: undefined, Format: undefined,
        });
        const source = OutputSource.multiple(["echo/result", "other/result"]);
        expectProperties(new CWL.CWLOutput(
            "result", stringType, outputBinding, source, "Result", yamlValue, false, "Output documentation", "text/plain",
        ), {
            Name: "result", Type_: stringType, OutputBinding: outputBinding, OutputSource: source,
            Label: "Result", SecondaryFiles: yamlValue, Streamable: false, Doc: "Output documentation", Format: "text/plain",
        });
    });

    it("CWLParameterRecordField keeps its required name and recursive value", () => {
        const value = CWL.CWLParameterValue.array([CWL.CWLParameterValue.string("hello")]);
        expectProperties(new CWL.CWLParameterRecordField("messages", value), { Name: "messages", Value: value });
    });

    it("CWLParameterReference accepts no value, flat values, and an explicit typed value", () => {
        expectProperties(new CWL.CWLParameterReference("message"), {
            Key: "message", Values: [], Value: undefined, Type: undefined,
        });
        const flat = new CWL.CWLParameterReference("message", ["hello", "world"]);
        expectProperties(flat, { Key: "message", Values: ["hello", "world"], Type: undefined });
        expect(flat.Value?.tag).toBe(7);
        expect(flat.Value).toEqual(CWL.CWLParameterValue.array([
            CWL.CWLParameterValue.string("hello"), CWL.CWLParameterValue.string("world"),
        ]));
        const value = CWL.CWLParameterValue.string("explicit");
        expectProperties(new CWL.CWLParameterReference("message", ["ignored"], value, stringType), {
            Key: "message", Values: ["explicit"], Value: value, Type: stringType,
        });
    });
});

describe("CWL primary constructors: requirements and hints", () => {
    it("DockerRequirement accepts omitted options and every positional field", () => {
        expectProperties(new CWL.DockerRequirement(), {
            DockerPull: undefined, DockerFile: undefined, DockerImageId: undefined, DockerLoad: undefined,
            DockerImport: undefined, DockerOutputDirectory: undefined, DockerRunOptions: undefined,
        });
        expectProperties(new CWL.DockerRequirement(
            "alpine:3", include, "sha256:abc", "image.tar", "https://example.org/image.tar", "/output", ["--read-only"],
        ), {
            DockerPull: "alpine:3", DockerFile: include, DockerImageId: "sha256:abc", DockerLoad: "image.tar",
            DockerImport: "https://example.org/image.tar", DockerOutputDirectory: "/output", DockerRunOptions: ["--read-only"],
        });
    });

    it("EnvironmentDef keeps both required strings, including an empty value", () => {
        expectProperties(new CWL.EnvironmentDef("EMPTY", ""), { EnvName: "EMPTY", EnvValue: "" });
        expectProperties(new CWL.EnvironmentDef("MESSAGE", "hello"), { EnvName: "MESSAGE", EnvValue: "hello" });
    });

    it("LoadListingRequirementValue keeps its required listing mode", () => {
        expectProperties(new LoadListingRequirementValue(listing), { LoadListing: listing });
    });

    it.each([false, true])("boolean requirement values preserve %s", (value) => {
        expectProperties(new WorkReuseRequirementValue(value), { EnableReuse: value });
        expectProperties(new NetworkAccessRequirementValue(value), { NetworkAccess: value });
        expectProperties(new InplaceUpdateRequirementValue(value), { InplaceUpdate: value });
    });

    it("ResourceRequirementInstance accepts absent resources and all numeric/expression options", () => {
        expectProperties(new CWL.ResourceRequirementInstance(), {
            CoresMin: undefined, CoresMax: undefined, RamMin: undefined, RamMax: undefined,
            TmpdirMin: undefined, TmpdirMax: undefined, OutdirMin: undefined, OutdirMax: undefined,
        });
        expectProperties(new CWL.ResourceRequirementInstance(1, "$(inputs.cores)", 256, 512, 0, 1024, 2048, "$(inputs.disk)"), {
            CoresMin: 1, CoresMax: "$(inputs.cores)", RamMin: 256, RamMax: 512,
            TmpdirMin: 0, TmpdirMax: 1024, OutdirMin: 2048, OutdirMax: "$(inputs.disk)",
        });
    });

    it("InlineJavascriptRequirementValue accepts absent and populated libraries", () => {
        expectProperties(new InlineJavascriptRequirementValue(), { ExpressionLib: undefined });
        expectProperties(new InlineJavascriptRequirementValue(["function echo(x) { return x; }"]), {
            ExpressionLib: ["function echo(x) { return x; }"],
        });
    });

    it("HintUnknownValue accepts an absent class and a named extension with raw YAML", () => {
        expectProperties(new HintUnknownValue(undefined, yamlValue), { Class: undefined, Raw: yamlValue });
        expectProperties(new HintUnknownValue("example:Extension", yamlValue), { Class: "example:Extension", Raw: yamlValue });
    });
});

describe("CWL primary constructors: workflow steps", () => {
    it("StepInput accepts its id alone and every positional option", () => {
        expectProperties(new CWL.StepInput("message"), {
            Id: "message", Source: undefined, DefaultValue: undefined, ValueFrom: undefined,
            LinkMerge: undefined, PickValue: undefined, Doc: undefined, LoadContents: undefined,
            LoadListing: undefined, Label: undefined,
        });
        const merge = LinkMergeMethod.mergeFlattened();
        const pick = PickValueMethod.firstNonNull();
        expectProperties(new CWL.StepInput(
            "message", ["workflow/message"], yamlValue, "$(self)", merge, pick,
            "Step input documentation", false, "deep_listing", "Message",
        ), {
            Id: "message", Source: ["workflow/message"], DefaultValue: yamlValue, ValueFrom: "$(self)",
            LinkMerge: merge, PickValue: pick, Doc: "Step input documentation", LoadContents: false,
            LoadListing: "deep_listing", Label: "Message",
        });
    });

    it("StepOutputParameter keeps its required id", () => {
        expectProperties(new StepOutputParameter("result"), { Id: "result" });
    });

    it("WorkflowStep keeps required nested values and all optional annotations", () => {
        expectProperties(new CWL.WorkflowStep("echo", [stepInput], [stepOutput], run), {
            Id: "echo", In: [stepInput], Out: [stepOutput], Run: run, Label: undefined, Doc: undefined,
            Scatter: undefined, ScatterMethod: undefined, When_: undefined, Requirements: undefined, Hints: undefined,
        });
        const scatter = ScatterMethod.flatCrossProduct();
        expectProperties(new CWL.WorkflowStep(
            "echo", [stepInput], [stepOutput], run, "Echo", "Step documentation", ["message"], scatter,
            "$(inputs.message != null)", [requirement], [hint],
        ), {
            Id: "echo", In: [stepInput], Out: [stepOutput], Run: run, Label: "Echo", Doc: "Step documentation",
            Scatter: ["message"], ScatterMethod: scatter, When_: "$(inputs.message != null)",
            Requirements: [requirement], Hints: [hint],
        });
    });
});

describe("CWL primary constructors: processing descriptions", () => {
    const absentAnnotations = {
        CWLVersion: "v1.2", Requirements: undefined, Hints: undefined, Intent: undefined,
        Metadata: undefined, Label: undefined, Doc: undefined, Id: undefined,
    };
    // A domain DynamicObj subclass provides a concrete metadata payload without testing the base class.
    const metadata = new CWL.EnvironmentDef("creator", "constructor test");
    const annotations = {
        CWLVersion: "v1.1", Requirements: [requirement], Hints: [hint], Intent: ["https://example.org/echo"],
        Metadata: metadata, Label: "Echo", Doc: "Processing documentation", Id: "#echo",
    };

    it("CWLToolDescription defaults its version and accepts every positional option", () => {
        expectProperties(new CWL.CWLToolDescription([]), {
            ...absentAnnotations, Outputs: [], BaseCommand: undefined, Arguments: undefined,
            Stdin: undefined, Stderr: undefined, Stdout: undefined, SuccessCodes: undefined,
            TemporaryFailCodes: undefined, PermanentFailCodes: undefined, Inputs: undefined,
        });
        expectProperties(new CWL.CWLToolDescription(
            [output], "v1.1", ["echo", "hello"], yamlValue, "input.txt", "error.log", "output.txt",
            [0], [75], [1, 2], [requirement], [hint], ["https://example.org/echo"], [input], metadata,
            "Echo", "Processing documentation", "#echo",
        ), {
            ...annotations, Outputs: [output], BaseCommand: ["echo", "hello"], Arguments: yamlValue,
            Stdin: "input.txt", Stderr: "error.log", Stdout: "output.txt", SuccessCodes: [0],
            TemporaryFailCodes: [75], PermanentFailCodes: [1, 2], Inputs: [input],
        });
    });

    it("CWLWorkflowDescription defaults its version and keeps its complete nested graph", () => {
        expectProperties(new CWL.CWLWorkflowDescription([], [], []), {
            ...absentAnnotations, Steps: [], Inputs: [], Outputs: [],
        });
        expectProperties(new CWL.CWLWorkflowDescription(
            [step], [input], [output], "v1.1", [requirement], [hint], ["https://example.org/echo"],
            metadata, "Echo", "Processing documentation", "#echo",
        ), { ...annotations, Steps: [step], Inputs: [input], Outputs: [output] });
    });

    it("CWLExpressionToolDescription defaults its version and keeps every positional option", () => {
        expectProperties(new CWLExpressionToolDescription([], "$(inputs.message)"), {
            ...absentAnnotations, Outputs: [], Expression: "$(inputs.message)", Inputs: undefined,
        });
        expectProperties(new CWLExpressionToolDescription(
            [output], "$(inputs.message)", "v1.1", [requirement], [hint], ["https://example.org/echo"],
            [input], metadata, "Echo", "Processing documentation", "#echo",
        ), { ...annotations, Outputs: [output], Expression: "$(inputs.message)", Inputs: [input] });
    });

    it("CWLOperationDescription defaults its version and keeps every positional option", () => {
        expectProperties(new CWLOperationDescription([], []), { ...absentAnnotations, Inputs: [], Outputs: [] });
        expectProperties(new CWLOperationDescription(
            [input], [output], "v1.1", [requirement], [hint], ["https://example.org/echo"],
            metadata, "Echo", "Processing documentation", "#echo",
        ), { ...annotations, Inputs: [input], Outputs: [output] });
    });
});

type UnionValue = { tag: number; fields: unknown[]; cases(): string[] };
type FactoryCase = [tag: number, name: string, actual: UnionValue, fields: unknown[]];

function expectUnion(actual: UnionValue, tag: number, name: string, fields: unknown[]) {
    expect(actual.tag).toBe(tag);
    expect(actual.cases()[tag]).toBe(name);
    expect(actual.fields).toEqual(fields);
}

describe("CWL union factory helpers", () => {
    const saladCases: FactoryCase[] = [
        [0, "Literal", SchemaSaladString.literal("literal text"), ["literal text"]],
        [1, "Include", SchemaSaladString.include("contents.txt"), ["contents.txt"]],
        [2, "Import", SchemaSaladString.import("tool.cwl"), ["tool.cwl"]],
    ];
    it.each(saladCases)("SchemaSaladString %s %s", (tag, name, actual, fields) => {
        expectUnion(actual, tag, name, fields);
    });

    const arraySchema = new CWL.InputArraySchema(stringType);
    const recordSchema = new CWL.InputRecordSchema([new CWL.InputRecordField("message", stringType)]);
    const enumSchema = new CWL.InputEnumSchema(["red", "blue"]);
    const unionTypes = [stringType, CWL.CWLType.null()];
    const typeCases: FactoryCase[] = [
        [0, "File", CWL.CWLType.file(file), [file]],
        [1, "Directory", CWL.CWLType.directory(directory), [directory]],
        [2, "Dirent", CWL.CWLType.dirent(dirent), [dirent]],
        [3, "String", CWL.CWLType.string(), []], [4, "Int", CWL.CWLType.int(), []],
        [5, "Long", CWL.CWLType.long(), []], [6, "Float", CWL.CWLType.float(), []],
        [7, "Double", CWL.CWLType.double(), []], [8, "Boolean", CWL.CWLType.boolean(), []],
        [9, "Stdout", CWL.CWLType.stdout(), []], [10, "Null", CWL.CWLType.null(), []],
        [11, "Array", CWL.CWLType.array(arraySchema), [arraySchema]],
        [12, "Record", CWL.CWLType.record(recordSchema), [recordSchema]],
        [13, "Enum", CWL.CWLType.enum(enumSchema), [enumSchema]],
        [14, "Union", CWL.CWLType.union(unionTypes), [unionTypes]],
    ];
    it.each(typeCases)("CWLType %s %s", (tag, name, actual, fields) => {
        expectUnion(actual, tag, name, fields);
    });

    it("CWLType file and directory factories still accept omitted instances", () => {
        expectUnion(CWL.CWLType.file(), 0, "File", [new CWL.FileInstance()]);
        expectUnion(CWL.CWLType.directory(), 1, "Directory", [new CWL.DirectoryInstance()]);
    });

    const parameterArray = [CWL.CWLParameterValue.string("hello"), CWL.CWLParameterValue.null()];
    const parameterRecord = [new CWL.CWLParameterRecordField("message", CWL.CWLParameterValue.string("hello"))];
    const parameterCases: FactoryCase[] = [
        [0, "Null", CWL.CWLParameterValue.null(), []],
        [1, "String", CWL.CWLParameterValue.string("hello"), ["hello"]],
        [2, "Int", CWL.CWLParameterValue.int(9007199254740993n), [9007199254740993n]],
        [3, "Float", CWL.CWLParameterValue.float(1.25), [1.25]],
        [4, "Boolean", CWL.CWLParameterValue.boolean(false), [false]],
        [5, "File", CWL.CWLParameterValue.file(file), [file]],
        [6, "Directory", CWL.CWLParameterValue.directory(directory), [directory]],
        [7, "Array", CWL.CWLParameterValue.array(parameterArray), [parameterArray]],
        [8, "Record", CWL.CWLParameterValue.record(parameterRecord), [parameterRecord]],
    ];
    it.each(parameterCases)("CWLParameterValue %s %s", (tag, name, actual, fields) => {
        expectUnion(actual, tag, name, fields);
    });

    const sourceCases: FactoryCase[] = [
        [0, "Single", OutputSource.single("echo/result"), ["echo/result"]],
        [1, "Multiple", OutputSource.multiple(["echo/result", "other/result"]), [["echo/result", "other/result"]]],
    ];
    it.each(sourceCases)(
        "OutputSource %s %s", (tag, name, actual, fields) => {
            expectUnion(actual, tag, name, fields);
        },
    );

    it.each([
        [0, "NoListing", LoadListingEnum.noListing()],
        [1, "ShallowListing", LoadListingEnum.shallowListing()],
        [2, "DeepListing", LoadListingEnum.deepListing()],
    ] as const)(
        "LoadListingEnum %s %s", (tag, name, actual) => expectUnion(actual, tag, name, []),
    );

    it("ToolTimeLimitValue preserves integer seconds and an expression", () => {
        expectUnion(ToolTimeLimitValue.toolTimeLimitSeconds(120n), 0, "ToolTimeLimitSeconds", [120n]);
        expectUnion(ToolTimeLimitValue.toolTimeLimitExpression("$(inputs.timeout)"), 1, "ToolTimeLimitExpression", ["$(inputs.timeout)"]);
    });

    const workDirCases: FactoryCase[] = [
        [0, "DirentEntry", InitialWorkDirEntry.direntEntry(dirent), [dirent]],
        [1, "StringEntry", InitialWorkDirEntry.stringEntry(include), [include]],
        [2, "FileEntry", InitialWorkDirEntry.fileEntry(file), [file]],
        [3, "DirectoryEntry", InitialWorkDirEntry.directoryEntry(directory), [directory]],
    ];
    it.each(workDirCases)("InitialWorkDirEntry %s %s", (tag, name, actual, fields) => {
        expectUnion(actual, tag, name, fields);
    });

    const inlineJavascript = new InlineJavascriptRequirementValue(["function echo(x) { return x; }"]);
    const schemaDefs = [new CWL.SchemaDefRequirementType("Message", stringType)];
    const docker = new CWL.DockerRequirement("alpine:3");
    const software = [new CWL.SoftwarePackage("echo", ["1.0"])];
    const loadListing = new LoadListingRequirementValue(listing);
    const workDir = [InitialWorkDirEntry.direntEntry(dirent)];
    const environment = [new CWL.EnvironmentDef("MESSAGE", "hello")];
    const resources = new CWL.ResourceRequirementInstance(1);
    const workReuse = new WorkReuseRequirementValue(false);
    const network = new NetworkAccessRequirementValue(false);
    const inplaceUpdate = new InplaceUpdateRequirementValue(true);
    const timeLimit = ToolTimeLimitValue.toolTimeLimitSeconds(60n);
    const requirementCases: FactoryCase[] = [
        [0, "InlineJavascriptRequirement", CWL.Requirement.inlineJavascriptRequirement(inlineJavascript), [inlineJavascript]],
        [1, "SchemaDefRequirement", CWL.Requirement.schemaDefRequirement(schemaDefs), [schemaDefs]],
        [2, "DockerRequirement", CWL.Requirement.dockerRequirement(docker), [docker]],
        [3, "SoftwareRequirement", CWL.Requirement.softwareRequirement(software), [software]],
        [4, "LoadListingRequirement", CWL.Requirement.loadListingRequirement(loadListing), [loadListing]],
        [5, "InitialWorkDirRequirement", CWL.Requirement.initialWorkDirRequirement(workDir), [workDir]],
        [6, "EnvVarRequirement", CWL.Requirement.envVarRequirement(environment), [environment]],
        [7, "ShellCommandRequirement", CWL.Requirement.shellCommandRequirement(), []],
        [8, "ResourceRequirement", CWL.Requirement.resourceRequirement(resources), [resources]],
        [9, "WorkReuseRequirement", CWL.Requirement.workReuseRequirement(workReuse), [workReuse]],
        [10, "WorkReuseExpressionRequirement", CWL.Requirement.workReuseExpressionRequirement("$(inputs.reuse)"), ["$(inputs.reuse)"]],
        [11, "NetworkAccessRequirement", CWL.Requirement.networkAccessRequirement(network), [network]],
        [12, "NetworkAccessExpressionRequirement", CWL.Requirement.networkAccessExpressionRequirement("$(inputs.network)"), ["$(inputs.network)"]],
        [13, "InplaceUpdateRequirement", CWL.Requirement.inplaceUpdateRequirement(inplaceUpdate), [inplaceUpdate]],
        [14, "ToolTimeLimitRequirement", CWL.Requirement.toolTimeLimitRequirement(timeLimit), [timeLimit]],
        [15, "SubworkflowFeatureRequirement", CWL.Requirement.subworkflowFeatureRequirement(), []],
        [16, "ScatterFeatureRequirement", CWL.Requirement.scatterFeatureRequirement(), []],
        [17, "MultipleInputFeatureRequirement", CWL.Requirement.multipleInputFeatureRequirement(), []],
        [18, "StepInputExpressionRequirement", CWL.Requirement.stepInputExpressionRequirement(), []],
    ];
    it.each(requirementCases)("Requirement %s %s", (tag, name, actual, fields) => {
        expectUnion(actual, tag, name, fields);
    });

    it("HintEntry preserves both known and extension hints", () => {
        const unknown = new HintUnknownValue("example:Extension", yamlValue);
        expectUnion(HintEntry.knownHint(requirement), 0, "KnownHint", [requirement]);
        expectUnion(HintEntry.unknownHint(unknown), 1, "UnknownHint", [unknown]);
    });

    it.each([
        [0, "MergeNested", LinkMergeMethod.mergeNested()], [1, "MergeFlattened", LinkMergeMethod.mergeFlattened()],
    ] as const)(
        "LinkMergeMethod %s %s", (tag, name, actual) => expectUnion(actual, tag, name, []),
    );
    it.each([
        [0, "FirstNonNull", PickValueMethod.firstNonNull()],
        [1, "TheOnlyNonNull", PickValueMethod.theOnlyNonNull()],
        [2, "AllNonNull", PickValueMethod.allNonNull()],
    ] as const)(
        "PickValueMethod %s %s", (tag, name, actual) => expectUnion(actual, tag, name, []),
    );
    it.each([
        [0, "DotProduct", ScatterMethod.dotProduct()],
        [1, "NestedCrossProduct", ScatterMethod.nestedCrossProduct()],
        [2, "FlatCrossProduct", ScatterMethod.flatCrossProduct()],
    ] as const)(
        "ScatterMethod %s %s", (tag, name, actual) => expectUnion(actual, tag, name, []),
    );

    it("StepOutput preserves a string id and a parameter record", () => {
        const parameter = new StepOutputParameter("result");
        expectUnion(CWL.StepOutput.stepOutputString("result"), 0, "StepOutputString", ["result"]);
        expectUnion(CWL.StepOutput.stepOutputRecord(parameter), 1, "StepOutputRecord", [parameter]);
    });

    const runCases: FactoryCase[] = [
        [0, "RunString", WorkflowStepRun.runString("tool.cwl"), ["tool.cwl"]],
        [1, "RunCommandLineTool", WorkflowStepRun.runCommandLineTool(tool), [tool]],
        [2, "RunWorkflow", WorkflowStepRun.runWorkflow(workflow), [workflow]],
        [3, "RunExpressionTool", WorkflowStepRun.runExpressionTool(expressionTool), [expressionTool]],
        [4, "RunOperation", WorkflowStepRun.runOperation(operation), [operation]],
    ];
    it.each(runCases)("WorkflowStepRun %s %s", (tag, name, actual, fields) => {
        expectUnion(actual, tag, name, fields);
    });

    const processingUnitCases: FactoryCase[] = [
        [0, "CommandLineTool", CWL.CWLProcessingUnit.commandLineTool(tool), [tool]],
        [1, "Workflow", CWL.CWLProcessingUnit.workflow(workflow), [workflow]],
        [2, "ExpressionTool", CWL.CWLProcessingUnit.expressionTool(expressionTool), [expressionTool]],
        [3, "Operation", CWL.CWLProcessingUnit.operation(operation), [operation]],
    ];
    it.each(processingUnitCases)("CWLProcessingUnit %s %s", (tag, name, actual, fields) => {
        expectUnion(actual, tag, name, fields);
    });
});