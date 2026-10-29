import { describe, expect, it } from "vitest";
import { ofArray } from "@fable-org/fable-library-ts/List.ts";
import { CWL } from "../../src/ARCtrl/index.ts";
import type { CWLProcessingUnit_$cases } from "../../src/ARCtrl/ts/CWL/CWLProcessingUnit.ts";
import type { CWLType_$cases } from "../../src/ARCtrl/ts/CWL/CWLTypes.ts";
import type { OutputSource_$cases } from "../../src/ARCtrl/ts/CWL/Outputs.ts";
import type { CWLParameterValue_$cases } from "../../src/ARCtrl/ts/CWL/ParameterValue.ts";
import type { InitialWorkDirEntry_$cases, Requirement_$cases } from "../../src/ARCtrl/ts/CWL/Requirements.ts";
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

const stringType = new CWL.CWLType(3, []);
const yamlContent = new YAMLContent("sample.txt", undefined, undefined, undefined, undefined);
const yamlValue = new YAMLElement(1, [yamlContent]);
const literal = new SchemaSaladString(0, ["Hello"]);
const include = new SchemaSaladString(1, ["message.txt"]);
const file = new CWL.FileInstance("file:///data/sample.txt", "/data/sample.txt");
const directory = new CWL.DirectoryInstance("file:///data", "/data");
const dirent = new CWL.DirentInstance(literal);
const listing = new LoadListingEnum(2, []);
const input = new CWL.CWLInput("message", stringType);
const output = new CWL.CWLOutput("result", stringType);
const inputBinding = new CWL.InputBinding("--message", 2);
const outputBinding = new CWL.OutputBinding("result.txt");
const requirement = new CWL.Requirement(2, [new CWL.DockerRequirement("alpine:3")]);
const hint = new HintEntry(0, [requirement]);
const run = new WorkflowStepRun(0, ["tool.cwl"]);
const stepInput = new CWL.StepInput("message");
const stepOutput = new CWL.StepOutput(0, ["result"]);
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
        const source = new OutputSource(1, [["echo/result", "other/result"]]);
        expectProperties(new CWL.CWLOutput(
            "result", stringType, outputBinding, source, "Result", yamlValue, false, "Output documentation", "text/plain",
        ), {
            Name: "result", Type_: stringType, OutputBinding: outputBinding, OutputSource: source,
            Label: "Result", SecondaryFiles: yamlValue, Streamable: false, Doc: "Output documentation", Format: "text/plain",
        });
    });

    it("CWLParameterRecordField keeps its required name and recursive value", () => {
        const value = new CWL.CWLParameterValue(7, [[new CWL.CWLParameterValue(1, ["hello"])]]);
        expectProperties(new CWL.CWLParameterRecordField("messages", value), { Name: "messages", Value: value });
    });

    it("CWLParameterReference accepts no value, flat values, and an explicit typed value", () => {
        expectProperties(new CWL.CWLParameterReference("message"), {
            Key: "message", Values: [], Value: undefined, Type: undefined,
        });
        const flat = new CWL.CWLParameterReference("message", ["hello", "world"]);
        expectProperties(flat, { Key: "message", Values: ["hello", "world"], Type: undefined });
        expect(flat.Value?.tag).toBe(7);
        expect(flat.Value?.fields).toEqual([[new CWL.CWLParameterValue(1, ["hello"]), new CWL.CWLParameterValue(1, ["world"])]]);
        const value = new CWL.CWLParameterValue(1, ["explicit"]);
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
        const merge = new LinkMergeMethod(1, []);
        const pick = new PickValueMethod(0, []);
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
        const scatter = new ScatterMethod(2, []);
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

// Explicit case tables exercise primary tag/fields constructors, including every recursive branch.
function expectUnion(actual: { tag: number; fields: unknown[]; cases(): string[] }, tag: number, name: string, fields: unknown[]) {
    expect(actual.tag).toBe(tag);
    expect(actual.cases()[tag]).toBe(name);
    expect(actual.fields).toEqual(fields);
}

describe("CWL union primary constructors", () => {
    it.each([
        [0, "Literal", "literal text"], [1, "Include", "contents.txt"], [2, "Import", "tool.cwl"],
    ] as const)("SchemaSaladString %s %s", (tag, name, text) => {
        expectUnion(new SchemaSaladString(tag, [text]), tag, name, [text]);
    });

    const typeCases: Array<[keyof CWLType_$cases, string, CWLType_$cases[keyof CWLType_$cases][1]]> = [
        [0, "File", [file]], [1, "Directory", [directory]], [2, "Dirent", [dirent]],
        [3, "String", []], [4, "Int", []], [5, "Long", []], [6, "Float", []],
        [7, "Double", []], [8, "Boolean", []], [9, "Stdout", []], [10, "Null", []],
        [11, "Array", [new CWL.InputArraySchema(stringType)]],
        [12, "Record", [new CWL.InputRecordSchema([new CWL.InputRecordField("message", stringType)])]],
        [13, "Enum", [new CWL.InputEnumSchema(["red", "blue"])]],
        [14, "Union", [[stringType, new CWL.CWLType(10, [])]]],
    ];
    it.each(typeCases)("CWLType %s %s", (tag, name, fields) => {
        expectUnion(new CWL.CWLType(tag, fields), tag, name, fields);
    });

    const parameterCases: Array<[keyof CWLParameterValue_$cases, string, CWLParameterValue_$cases[keyof CWLParameterValue_$cases][1]]> = [
        [0, "Null", []], [1, "String", ["hello"]], [2, "Int", [9007199254740993n]],
        [3, "Float", [1.25]], [4, "Boolean", [false]], [5, "File", [file]], [6, "Directory", [directory]],
        [7, "Array", [[new CWL.CWLParameterValue(1, ["hello"]), new CWL.CWLParameterValue(0, [])]]],
        [8, "Record", [[new CWL.CWLParameterRecordField("message", new CWL.CWLParameterValue(1, ["hello"]))]]],
    ];
    it.each(parameterCases)("CWLParameterValue %s %s", (tag, name, fields) => {
        expectUnion(new CWL.CWLParameterValue(tag, fields), tag, name, fields);
    });

    const sourceCases: Array<[keyof OutputSource_$cases, string, OutputSource_$cases[keyof OutputSource_$cases][1]]> = [
        [0, "Single", ["echo/result"]], [1, "Multiple", [["echo/result", "other/result"]]],
    ];
    it.each(sourceCases)(
        "OutputSource %s %s", (tag, name, fields) => {
            expectUnion(new OutputSource(tag, fields), tag, name, fields);
        },
    );

    it.each([[0, "NoListing"], [1, "ShallowListing"], [2, "DeepListing"]] as const)(
        "LoadListingEnum %s %s", (tag, name) => expectUnion(new LoadListingEnum(tag, []), tag, name, []),
    );

    it("ToolTimeLimitValue preserves integer seconds and an expression", () => {
        expectUnion(new ToolTimeLimitValue(0, [120n]), 0, "ToolTimeLimitSeconds", [120n]);
        expectUnion(new ToolTimeLimitValue(1, ["$(inputs.timeout)"]), 1, "ToolTimeLimitExpression", ["$(inputs.timeout)"]);
    });

    const workDirCases: Array<[keyof InitialWorkDirEntry_$cases, string, InitialWorkDirEntry_$cases[keyof InitialWorkDirEntry_$cases][1]]> = [
        [0, "DirentEntry", [dirent]], [1, "StringEntry", [include]],
        [2, "FileEntry", [file]], [3, "DirectoryEntry", [directory]],
    ];
    it.each(workDirCases)("InitialWorkDirEntry %s %s", (tag, name, fields) => {
        expectUnion(new InitialWorkDirEntry(tag, fields), tag, name, fields);
    });

    const requirementCases: Array<[keyof Requirement_$cases, string, Requirement_$cases[keyof Requirement_$cases][1]]> = [
        [0, "InlineJavascriptRequirement", [new InlineJavascriptRequirementValue(["function echo(x) { return x; }"])]],
        [1, "SchemaDefRequirement", [[new CWL.SchemaDefRequirementType("Message", stringType)]]],
        [2, "DockerRequirement", [new CWL.DockerRequirement("alpine:3")]],
        [3, "SoftwareRequirement", [[new CWL.SoftwarePackage("echo", ["1.0"])]]],
        [4, "LoadListingRequirement", [new LoadListingRequirementValue(listing)]],
        [5, "InitialWorkDirRequirement", [[new InitialWorkDirEntry(0, [dirent])]]],
        [6, "EnvVarRequirement", [[new CWL.EnvironmentDef("MESSAGE", "hello")]]],
        [7, "ShellCommandRequirement", []], [8, "ResourceRequirement", [new CWL.ResourceRequirementInstance(1)]],
        [9, "WorkReuseRequirement", [new WorkReuseRequirementValue(false)]],
        [10, "WorkReuseExpressionRequirement", ["$(inputs.reuse)"]],
        [11, "NetworkAccessRequirement", [new NetworkAccessRequirementValue(false)]],
        [12, "NetworkAccessExpressionRequirement", ["$(inputs.network)"]],
        [13, "InplaceUpdateRequirement", [new InplaceUpdateRequirementValue(true)]],
        [14, "ToolTimeLimitRequirement", [new ToolTimeLimitValue(0, [60n])]],
        [15, "SubworkflowFeatureRequirement", []], [16, "ScatterFeatureRequirement", []],
        [17, "MultipleInputFeatureRequirement", []], [18, "StepInputExpressionRequirement", []],
    ];
    it.each(requirementCases)("Requirement %s %s", (tag, name, fields) => {
        expectUnion(new CWL.Requirement(tag, fields), tag, name, fields);
    });

    it("HintEntry preserves both known and extension hints", () => {
        const unknown = new HintUnknownValue("example:Extension", yamlValue);
        expectUnion(new HintEntry(0, [requirement]), 0, "KnownHint", [requirement]);
        expectUnion(new HintEntry(1, [unknown]), 1, "UnknownHint", [unknown]);
    });

    it.each([[0, "MergeNested"], [1, "MergeFlattened"]] as const)(
        "LinkMergeMethod %s %s", (tag, name) => expectUnion(new LinkMergeMethod(tag, []), tag, name, []),
    );
    it.each([[0, "FirstNonNull"], [1, "TheOnlyNonNull"], [2, "AllNonNull"]] as const)(
        "PickValueMethod %s %s", (tag, name) => expectUnion(new PickValueMethod(tag, []), tag, name, []),
    );
    it.each([[0, "DotProduct"], [1, "NestedCrossProduct"], [2, "FlatCrossProduct"]] as const)(
        "ScatterMethod %s %s", (tag, name) => expectUnion(new ScatterMethod(tag, []), tag, name, []),
    );

    it("StepOutput preserves a string id and a parameter record", () => {
        const parameter = new StepOutputParameter("result");
        expectUnion(new CWL.StepOutput(0, ["result"]), 0, "StepOutputString", ["result"]);
        expectUnion(new CWL.StepOutput(1, [parameter]), 1, "StepOutputRecord", [parameter]);
    });

    it.each([
        [0, "RunString", "tool.cwl"], [1, "RunCommandLineTool", tool], [2, "RunWorkflow", workflow],
        [3, "RunExpressionTool", expressionTool], [4, "RunOperation", operation],
    ] as const)("WorkflowStepRun %s %s", (tag, name, value) => {
        expectUnion(new WorkflowStepRun(tag, [value]), tag, name, [value]);
    });

    const processingUnitCases: Array<[keyof CWLProcessingUnit_$cases, string, CWLProcessingUnit_$cases[keyof CWLProcessingUnit_$cases][1]]> = [
        [0, "CommandLineTool", [tool]], [1, "Workflow", [workflow]],
        [2, "ExpressionTool", [expressionTool]], [3, "Operation", [operation]],
    ];
    it.each(processingUnitCases)("CWLProcessingUnit %s %s", (tag, name, fields) => {
        expectUnion(new CWL.CWLProcessingUnit(tag, fields), tag, name, fields);
    });
});

describe("YAML values nested in CWL constructors", () => {
    it("YAMLContent accepts absent options and full scalar annotations", () => {
        expectProperties(new YAMLContent("hello", undefined, undefined, undefined, undefined), {
            Value: "hello", Comment: undefined, Anchor: undefined, Tag: undefined, Style: undefined,
        });
        const style = new ScalarStyle(3, [new BlockScalarStyle(0, []), new ChompingMode(2, []), 2]);
        expectProperties(new YAMLContent("hello\nworld", "comment", "message", "!!str", style), {
            Value: "hello\nworld", Comment: "comment", Anchor: "message", Tag: "!!str", Style: style,
        });
    });

    it.each([[0, "Literal"], [1, "Folded"]] as const)(
        "BlockScalarStyle %s %s", (tag, name) => expectUnion(new BlockScalarStyle(tag, []), tag, name, []),
    );
    it.each([[0, "Strip"], [1, "Clip"], [2, "Keep"]] as const)(
        "ChompingMode %s %s", (tag, name) => expectUnion(new ChompingMode(tag, []), tag, name, []),
    );
    it.each([[0, "Plain"], [1, "SingleQuoted"], [2, "DoubleQuoted"]] as const)(
        "ScalarStyle %s %s", (tag, name) => expectUnion(new ScalarStyle(tag, []), tag, name, []),
    );
    it("ScalarStyle Block accepts omitted indentation and explicit indentation", () => {
        const block = new BlockScalarStyle(1, []);
        const chomping = new ChompingMode(0, []);
        expectUnion(new ScalarStyle(3, [block, chomping, undefined]), 3, "Block", [block, chomping, undefined]);
        expectUnion(new ScalarStyle(3, [block, chomping, 4]), 3, "Block", [block, chomping, 4]);
    });

    const yamlCases: Array<[keyof YAMLElement_$cases, string, YAMLElement_$cases[keyof YAMLElement_$cases][1]]> = [
        [0, "Mapping", [new YAMLContent("message", undefined, undefined, undefined, undefined), yamlValue]],
        [1, "Value", [yamlContent]], [2, "Sequence", [ofArray([yamlValue])]],
        [3, "Object", [ofArray([new YAMLElement(0, [yamlContent, yamlValue])])]],
        [4, "Comment", ["a comment"]], [5, "DocumentStart", []], [6, "DocumentEnd", []],
        [7, "Alias", ["message"]], [8, "Nil", []],
    ];
    it.each(yamlCases)("YAMLElement %s %s", (tag, name, fields) => {
        expectUnion(new YAMLElement(tag, fields), tag, name, fields);
    });
});
