import { describe, expect, it } from "vitest";
import { ofArray } from "@fable-org/fable-library-ts/List.ts";
import { CWL } from "../../src/ARCtrl/index.ts";
import { YamlNodeModule_plain } from "../../src/ARCtrl/ts/fable_modules/YAMLicious.2.0.0/Node.fs.ts";
import { RawYaml } from "../../src/ARCtrl/ts/CWL/CWLTypes.ts";

const {
    SchemaSaladString, CWLExpressionToolDescription, CWLOperationDescription, OutputSource,
    HintEntry, HintUnknownValue, InitialWorkDirEntry, InlineJavascriptRequirementValue,
    InplaceUpdateRequirementValue, LoadListingEnum, LoadListingRequirementValue,
    NetworkAccessRequirementValue, ToolTimeLimitValue, WorkReuseRequirementValue,
    LinkMergeMethod, PickValueMethod, ScatterMethod, StepOutputParameter, WorkflowStepRun,
    SchemaSaladBool, FileSize, DocText, ListEntry, BindingPosition, InputFormat, OutputGlob,
    SchemaDefinition, SchemaDefEntry,
} = CWL;

// Expected public properties are listed explicitly, independently of generated storage names.
function expectProperties(actual: object, expected: Record<string, unknown>) {
    for (const [property, value] of Object.entries(expected)) {
        expect((actual as Record<string, unknown>)[property], property).toEqual(value);
    }
}

// Strings, booleans and lists of the CWL model take the union that also holds a Schema Salad directive.
const lit = (text: string) => SchemaSaladString.literal(text);
const bool = (value: boolean) => new SchemaSaladBool(0, [value]);
const doc = (text: string) => DocText.literal(text);
const values = <T,>(items: T[]) => ListEntry.ofValues(items);

const stringType = CWL.CWLType.string();
const yamlValue = new RawYaml(YamlNodeModule_plain("sample.txt"));
const literal = SchemaSaladString.literal("Hello");
const include = SchemaSaladString.include("message.txt");
const file = new CWL.FileInstance(lit("file:///data/sample.txt"), lit("/data/sample.txt"));
const directory = new CWL.DirectoryInstance(lit("file:///data"), lit("/data"));
const dirent = new CWL.DirentInstance(literal);
const listing = LoadListingEnum.deepListing();
const input = new CWL.CWLInput("message", stringType);
const output = new CWL.CWLOutput("result", stringType);
const inputBinding = new CWL.InputBinding(lit("--message"), new BindingPosition(0, [2]));
const outputBinding = new CWL.OutputBinding(new OutputGlob(0, [lit("result.txt")]));
const requirement = CWL.Requirement.dockerRequirement(new CWL.DockerRequirement(lit("alpine:3")));
const hint = HintEntry.knownHint(requirement);
const run = WorkflowStepRun.runString("tool.cwl");
const stepInput = new CWL.StepInput("message");
const stepOutput = CWL.StepOutput.stepOutputString("result");
const step = new CWL.WorkflowStep("echo", values([stepInput]), [stepOutput], run);
const tool = new CWL.CWLToolDescription(values([output]));
const workflow = new CWL.CWLWorkflowDescription(values([step]), values([input]), values([output]));
const expressionTool = new CWLExpressionToolDescription(values([output]), lit("$(inputs.message)"));
const operation = new CWLOperationDescription(values([input]), values([output]));

describe("CWL primary constructors: files and schemas", () => {
    it("FileInstance accepts omitted options and every positional field", () => {
        const absent = {
            Location: undefined, Path: undefined, Basename: undefined, Dirname: undefined,
            Nameroot: undefined, Nameext: undefined, Checksum: undefined, Size: undefined,
            SecondaryFiles: undefined, Format: undefined, Contents: undefined,
        };
        expectProperties(new CWL.FileInstance(), absent);
        const actual = new CWL.FileInstance(
            lit("file:///data/input.txt"), lit("/data/input.txt"), lit("input.txt"), lit("/data"), lit("input"), lit(".txt"),
            lit("sha1$abc"), new FileSize(0, [42n]), yamlValue, lit("text/plain"), lit("contents"),
        );
        expectProperties(actual, {
            Location: lit("file:///data/input.txt"), Path: lit("/data/input.txt"), Basename: lit("input.txt"),
            Dirname: lit("/data"), Nameroot: lit("input"), Nameext: lit(".txt"), Checksum: lit("sha1$abc"), Size: new FileSize(0, [42n]),
            SecondaryFiles: yamlValue, Format: lit("text/plain"), Contents: lit("contents"),
        });
    });

    it("DirectoryInstance accepts omitted options and a YAML listing", () => {
        expectProperties(new CWL.DirectoryInstance(), {
            Location: undefined, Path: undefined, Basename: undefined, Listing: undefined,
        });
        expectProperties(new CWL.DirectoryInstance(lit("file:///data"), lit("/data"), lit("data"), yamlValue), {
            Location: lit("file:///data"), Path: lit("/data"), Basename: lit("data"), Listing: yamlValue,
        });
    });

    it("DirentInstance keeps literal/directive values and false writable", () => {
        expectProperties(new CWL.DirentInstance(literal), {
            Entry: literal, Entryname: undefined, Writable: undefined,
        });
        expectProperties(new CWL.DirentInstance(include, literal, bool(false)), {
            Entry: include, Entryname: literal, Writable: bool(false),
        });
    });

    it("InputEnumSchema accepts required symbols and optional annotations", () => {
        expectProperties(new CWL.InputEnumSchema([]), {
            Symbols: [], Label: undefined, Doc: undefined, Name: undefined,
        });
        expectProperties(new CWL.InputEnumSchema([lit("red"), lit("blue")], lit("Colour"), doc("Allowed colours"), "ColourType"), {
            Symbols: [lit("red"), lit("blue")], Label: lit("Colour"), Doc: doc("Allowed colours"), Name: "ColourType",
        });
    });

    it("InputRecordField keeps its nested type and annotations", () => {
        expectProperties(new CWL.InputRecordField("message", stringType), {
            Name: "message", Type: stringType, Doc: undefined, Label: undefined,
        });
        expectProperties(new CWL.InputRecordField("message", stringType, doc("A message"), lit("Message")), {
            Name: "message", Type: stringType, Doc: doc("A message"), Label: lit("Message"),
        });
    });

    it("InputRecordSchema accepts absent fields and populated fields", () => {
        expectProperties(new CWL.InputRecordSchema(), {
            Fields: undefined, Label: undefined, Doc: undefined, Name: undefined,
        });
        const fields = values([new CWL.InputRecordField("message", stringType)]);
        expectProperties(new CWL.InputRecordSchema(fields, lit("Record"), doc("Record documentation"), "MessageRecord"), {
            Fields: fields, Label: lit("Record"), Doc: doc("Record documentation"), Name: "MessageRecord",
        });
    });

    it("InputArraySchema accepts required items and optional annotations", () => {
        expectProperties(new CWL.InputArraySchema(stringType), {
            Items: stringType, Label: undefined, Doc: undefined, Name: undefined,
        });
        expectProperties(new CWL.InputArraySchema(stringType, lit("Messages"), doc("Array documentation"), "MessagesType"), {
            Items: stringType, Label: lit("Messages"), Doc: doc("Array documentation"), Name: "MessagesType",
        });
    });

    it("SchemaDefinition keeps the named schema of a SchemaDefRequirement type", () => {
        const schema = new CWL.InputArraySchema(stringType, undefined, undefined, "Message");
        const definition = new SchemaDefinition(2, [schema]);
        expectUnion(definition, 2, "Array", [schema]);
        expectProperties(definition.fields[0], { Name: "Message", Items: stringType });
    });

    it("SoftwarePackage accepts a name alone and version/specification arrays", () => {
        expectProperties(new CWL.SoftwarePackage(lit("echo")), { Package: lit("echo"), Version: undefined, Specs: undefined });
        expectProperties(new CWL.SoftwarePackage(lit("echo"), [lit("1.0"), lit("2.0")], [lit("https://example.org/echo")]), {
            Package: lit("echo"), Version: [lit("1.0"), lit("2.0")], Specs: [lit("https://example.org/echo")],
        });
    });
});

describe("CWL primary constructors: inputs, outputs and parameters", () => {
    it.each([
        [true, false, false],
        [false, true, false],
        [false, false, true],
    ])("InputBinding keeps Separate=%s, LoadContents=%s, ShellQuote=%s independent", (separate, loadContents, shellQuote) => {
        expectProperties(new CWL.InputBinding(undefined, undefined, undefined, bool(separate), bool(loadContents), undefined, bool(shellQuote)), {
            Separate: bool(separate), LoadContents: bool(loadContents), ShellQuote: bool(shellQuote),
        });
    });

    it.each([
        [true, false, false],
        [false, true, false],
        [false, false, true],
    ])("CWLInput keeps Optional=%s, Streamable=%s, LoadContents=%s independent", (optional, streamable, loadContents) => {
        // Optional is true exactly when the type admits null.
        const type = optional ? CWL.CWLType.union([stringType, CWL.CWLType.null()]) : undefined;
        expectProperties(new CWL.CWLInput(
            "message", type, undefined, undefined, undefined, bool(streamable),
            undefined, undefined, bool(loadContents),
        ), { Optional: optional, Streamable: bool(streamable), LoadContents: bool(loadContents) });
    });

    it("InputBinding accepts omitted options and every positional field", () => {
        const absent = {
            Prefix: undefined, _position: undefined, ItemSeparator: undefined, Separate: undefined,
            LoadContents: undefined, ValueFrom: undefined, ShellQuote: undefined,
        };
        expectProperties(new CWL.InputBinding(), absent);
        // Position has only a setter in the generated API; its stored constructor value is observable here.
        expectProperties(new CWL.InputBinding(lit("--message"), new BindingPosition(0, [0]), lit(","), bool(false), bool(true), lit("$(self)"), bool(false)), {
            Prefix: lit("--message"), _position: new BindingPosition(0, [0]), ItemSeparator: lit(","), Separate: bool(false),
            LoadContents: bool(true), ValueFrom: lit("$(self)"), ShellQuote: bool(false),
        });
    });

    it("CWLInput keeps every positional option, including false values and YAML defaults", () => {
        expectProperties(new CWL.CWLInput("message"), {
            Name: "message", Type_: undefined, InputBinding: undefined, Optional: false,
            Label: undefined, SecondaryFiles: undefined, Streamable: undefined, Doc: undefined,
            Format: undefined, LoadContents: undefined, LoadListing: undefined, DefaultValue: undefined,
        });
        const bindingEntry = new ListEntry(0, [inputBinding]);
        const format = new InputFormat(0, [lit("text/plain")]);
        expectProperties(new CWL.CWLInput(
            "message", stringType, bindingEntry, lit("Message"), yamlValue, bool(false),
            doc("Input documentation"), format, bool(true), listing, yamlValue,
        ), {
            Name: "message", Type_: stringType, InputBinding: bindingEntry, Optional: false,
            Label: lit("Message"), SecondaryFiles: yamlValue, Streamable: bool(false), Doc: doc("Input documentation"),
            Format: format, LoadContents: bool(true), LoadListing: listing, DefaultValue: yamlValue,
        });
    });

    it("OutputBinding accepts omitted options and every positional field", () => {
        expectProperties(new CWL.OutputBinding(), {
            Glob: undefined, LoadContents: undefined, LoadListing: undefined, OutputEval: undefined,
        });
        const glob = new OutputGlob(0, [lit("*.txt")]);
        expectProperties(new CWL.OutputBinding(glob, bool(false), listing, lit("$(self[0])")), {
            Glob: glob, LoadContents: bool(false), LoadListing: listing, OutputEval: lit("$(self[0])"),
        });
    });

    it("CWLOutput keeps every positional option", () => {
        expectProperties(new CWL.CWLOutput("result"), {
            Name: "result", Type_: undefined, OutputBinding: undefined, OutputSource: undefined,
            Label: undefined, SecondaryFiles: undefined, Streamable: undefined, Doc: undefined, Format: undefined,
        });
        const source = OutputSource.multiple([lit("echo/result"), lit("other/result")]);
        const bindingEntry = new ListEntry(0, [outputBinding]);
        expectProperties(new CWL.CWLOutput(
            "result", stringType, bindingEntry, source, lit("Result"), yamlValue, bool(false), doc("Output documentation"), lit("text/plain"),
        ), {
            Name: "result", Type_: stringType, OutputBinding: bindingEntry, OutputSource: source,
            Label: lit("Result"), SecondaryFiles: yamlValue, Streamable: bool(false), Doc: doc("Output documentation"), Format: lit("text/plain"),
        });
    });

    it("CWLParameterRecordField keeps its required name and recursive value", () => {
        const value = CWL.CWLParameterValue.array([CWL.CWLParameterValue.string("hello")]);
        expectProperties(new CWL.CWLParameterRecordField("messages", value), { Name: "messages", Value: value });
    });

    it("CWLParameterReference accepts no value, flat values, and an explicit value", () => {
        expectProperties(new CWL.CWLParameterReference("message"), {
            Key: "message", Values: [], Value: undefined,
        });
        const flat = new CWL.CWLParameterReference("message", ["hello", "world"]);
        expectProperties(flat, { Key: "message", Values: ["hello", "world"] });
        expect(flat.Value?.tag).toBe(7);
        expect(flat.Value).toEqual(CWL.CWLParameterValue.array([
            CWL.CWLParameterValue.string("hello"), CWL.CWLParameterValue.string("world"),
        ]));
        const value = CWL.CWLParameterValue.string("explicit");
        expectProperties(new CWL.CWLParameterReference("message", ["ignored"], value), {
            Key: "message", Values: ["explicit"], Value: value,
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
            lit("alpine:3"), include, lit("sha256:abc"), lit("image.tar"), lit("https://example.org/image.tar"), lit("/output"), ["--read-only"],
        ), {
            DockerPull: lit("alpine:3"), DockerFile: include, DockerImageId: lit("sha256:abc"), DockerLoad: lit("image.tar"),
            DockerImport: lit("https://example.org/image.tar"), DockerOutputDirectory: lit("/output"), DockerRunOptions: ["--read-only"],
        });
    });

    it("EnvironmentDef keeps both required strings, including an empty value", () => {
        expectProperties(new CWL.EnvironmentDef(lit("EMPTY"), lit("")), { EnvName: lit("EMPTY"), EnvValue: lit("") });
        expectProperties(new CWL.EnvironmentDef(lit("MESSAGE"), lit("hello")), { EnvName: lit("MESSAGE"), EnvValue: lit("hello") });
    });

    it("LoadListingRequirementValue keeps its required listing mode", () => {
        expectProperties(new LoadListingRequirementValue(listing), { LoadListing: listing });
    });

    it.each([false, true])("boolean requirement values preserve %s", (value) => {
        expectProperties(new WorkReuseRequirementValue(value), { EnableReuse: value });
        expectProperties(new NetworkAccessRequirementValue(value), { NetworkAccess: value });
        expectProperties(new InplaceUpdateRequirementValue(bool(value)), { InplaceUpdate: bool(value) });
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
        expectProperties(new InlineJavascriptRequirementValue([lit("function echo(x) { return x; }")]), {
            ExpressionLib: [lit("function echo(x) { return x; }")],
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
        const source = OutputSource.multiple([lit("workflow/message")]);
        expectProperties(new CWL.StepInput(
            "message", source, yamlValue, lit("$(self)"), merge, pick,
            doc("Step input documentation"), bool(false), LoadListingEnum.deepListing(), lit("Message"),
        ), {
            Id: "message", Source: source, DefaultValue: yamlValue, ValueFrom: lit("$(self)"),
            LinkMerge: merge, PickValue: pick, Doc: doc("Step input documentation"), LoadContents: bool(false),
            LoadListing: LoadListingEnum.deepListing(), Label: lit("Message"),
        });
    });

    it("StepOutputParameter keeps its required id", () => {
        expectProperties(new StepOutputParameter("result"), { Id: "result" });
    });

    it("WorkflowStep keeps required nested values and all optional annotations", () => {
        expectProperties(new CWL.WorkflowStep("echo", values([stepInput]), [stepOutput], run), {
            Id: "echo", In: values([stepInput]), Out: [stepOutput], Run: run, Label: undefined, Doc: undefined,
            Scatter: undefined, ScatterMethod: undefined, When_: undefined, Requirements: undefined, Hints: undefined,
        });
        const scatter = ScatterMethod.flatCrossProduct();
        expectProperties(new CWL.WorkflowStep(
            "echo", values([stepInput]), [stepOutput], run, lit("Echo"), doc("Step documentation"), [lit("message")], scatter,
            lit("$(inputs.message != null)"), [requirement], [hint],
        ), {
            Id: "echo", In: values([stepInput]), Out: [stepOutput], Run: run, Label: lit("Echo"), Doc: doc("Step documentation"),
            Scatter: [lit("message")], ScatterMethod: scatter, When_: lit("$(inputs.message != null)"),
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
    const metadata = new CWL.EnvironmentDef(lit("creator"), lit("constructor test"));
    const annotations = {
        CWLVersion: "v1.1", Requirements: [requirement], Hints: [hint], Intent: [lit("https://example.org/echo")],
        Metadata: metadata, Label: lit("Echo"), Doc: doc("Processing documentation"), Id: "#echo",
    };

    it("CWLToolDescription defaults its version and accepts every positional option", () => {
        expectProperties(new CWL.CWLToolDescription([]), {
            ...absentAnnotations, Outputs: [], BaseCommand: undefined, Arguments: undefined,
            Stdin: undefined, Stderr: undefined, Stdout: undefined, SuccessCodes: undefined,
            TemporaryFailCodes: undefined, PermanentFailCodes: undefined, Inputs: undefined,
        });
        expectProperties(new CWL.CWLToolDescription(
            values([output]), "v1.1", [lit("echo"), lit("hello")], yamlValue, lit("input.txt"), lit("error.log"), lit("output.txt"),
            values([0]), values([75]), values([1, 2]), [requirement], [hint], [lit("https://example.org/echo")], values([input]), metadata,
            lit("Echo"), doc("Processing documentation"), "#echo",
        ), {
            ...annotations, Outputs: values([output]), BaseCommand: [lit("echo"), lit("hello")], Arguments: yamlValue,
            Stdin: lit("input.txt"), Stderr: lit("error.log"), Stdout: lit("output.txt"), SuccessCodes: values([0]),
            TemporaryFailCodes: values([75]), PermanentFailCodes: values([1, 2]), Inputs: values([input]),
        });
    });

    it("CWLWorkflowDescription defaults its version and keeps its complete nested graph", () => {
        expectProperties(new CWL.CWLWorkflowDescription([], [], []), {
            ...absentAnnotations, Steps: [], Inputs: [], Outputs: [],
        });
        expectProperties(new CWL.CWLWorkflowDescription(
            values([step]), values([input]), values([output]), "v1.1", [requirement], [hint], [lit("https://example.org/echo")],
            metadata, lit("Echo"), doc("Processing documentation"), "#echo",
        ), { ...annotations, Steps: values([step]), Inputs: values([input]), Outputs: values([output]) });
    });

    it("CWLExpressionToolDescription defaults its version and keeps every positional option", () => {
        expectProperties(new CWLExpressionToolDescription([], lit("$(inputs.message)")), {
            ...absentAnnotations, Outputs: [], Expression: lit("$(inputs.message)"), Inputs: undefined,
        });
        expectProperties(new CWLExpressionToolDescription(
            values([output]), lit("$(inputs.message)"), "v1.1", [requirement], [hint], [lit("https://example.org/echo")],
            values([input]), metadata, lit("Echo"), doc("Processing documentation"), "#echo",
        ), { ...annotations, Outputs: values([output]), Expression: lit("$(inputs.message)"), Inputs: values([input]) });
    });

    it("CWLOperationDescription defaults its version and keeps every positional option", () => {
        expectProperties(new CWLOperationDescription([], []), { ...absentAnnotations, Inputs: [], Outputs: [] });
        expectProperties(new CWLOperationDescription(
            values([input]), values([output]), "v1.1", [requirement], [hint], [lit("https://example.org/echo")],
            metadata, lit("Echo"), doc("Processing documentation"), "#echo",
        ), { ...annotations, Inputs: values([input]), Outputs: values([output]) });
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
        [3, "Mixin", SchemaSaladString.mixin("fields.yml"), ["fields.yml"]],
        [4, "Mixed", SchemaSaladString.mixed(yamlValue), [yamlValue]],
    ];
    it.each(saladCases)("SchemaSaladString %s %s", (tag, name, actual, fields) => {
        expectUnion(actual, tag, name, fields);
    });

    const arraySchema = new CWL.InputArraySchema(stringType);
    const recordSchema = new CWL.InputRecordSchema(values([new CWL.InputRecordField("message", stringType)]));
    const enumSchema = new CWL.InputEnumSchema([lit("red"), lit("blue")]);
    const unionTypes = [stringType, CWL.CWLType.null()];
    const typeCases: FactoryCase[] = [
        [0, "File", CWL.CWLType.file(), []],
        [1, "Directory", CWL.CWLType.directory(), []],
        [2, "String", CWL.CWLType.string(), []], [3, "Int", CWL.CWLType.int(), []],
        [4, "Long", CWL.CWLType.long(), []], [5, "Float", CWL.CWLType.float(), []],
        [6, "Double", CWL.CWLType.double(), []], [7, "Boolean", CWL.CWLType.boolean(), []],
        [8, "Any", CWL.CWLType.any(), []],
        [9, "Stdout", CWL.CWLType.stdout(), []], [10, "Stderr", CWL.CWLType.stderr(), []],
        [11, "Stdin", CWL.CWLType.stdin(), []],
        [12, "Reference", CWL.CWLType.reference("#Message"), ["#Message"]],
        [13, "ImportedType", CWL.CWLType.importedType("types.yml"), ["types.yml"]],
        [14, "IncludedType", CWL.CWLType.includedType("type.txt"), ["type.txt"]],
        [15, "MixedType", CWL.CWLType.mixedType(yamlValue), [yamlValue]],
        [16, "RawType", CWL.CWLType.rawType(yamlValue), [yamlValue]],
        [17, "Null", CWL.CWLType.null(), []],
        [18, "Array", CWL.CWLType.array(arraySchema), [arraySchema]],
        [19, "Record", CWL.CWLType.record(recordSchema), [recordSchema]],
        [20, "Enum", CWL.CWLType.enum(enumSchema), [enumSchema]],
        [21, "Union", CWL.CWLType.union(unionTypes), [unionTypes]],
    ];
    it.each(typeCases)("CWLType %s %s", (tag, name, actual, fields) => {
        expectUnion(actual, tag, name, fields);
    });

    it("CWLType file and directory factories take no instance", () => {
        expectUnion(CWL.CWLType.file(), 0, "File", []);
        expectUnion(CWL.CWLType.directory(), 1, "Directory", []);
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
        [0, "Single", OutputSource.single(lit("echo/result")), [lit("echo/result")]],
        [1, "Multiple", OutputSource.multiple([lit("echo/result"), lit("other/result")]), [[lit("echo/result"), lit("other/result")]]],
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
    const listingDirectiveCases: FactoryCase[] = [
        [3, "ImportedLoadListing", LoadListingEnum.importedLoadListing("listing.yml"), ["listing.yml"]],
        [4, "IncludedLoadListing", LoadListingEnum.includedLoadListing("listing.txt"), ["listing.txt"]],
        [5, "MixedLoadListing", LoadListingEnum.mixedLoadListing(yamlValue), [yamlValue]],
    ];
    it.each(listingDirectiveCases)("LoadListingEnum %s %s", (tag, name, actual, fields) => {
        expectUnion(actual, tag, name, fields);
    });

    it("ToolTimeLimitValue preserves integer seconds and an expression", () => {
        expectUnion(ToolTimeLimitValue.toolTimeLimitSeconds(120n), 0, "ToolTimeLimitSeconds", [120n]);
        expectUnion(ToolTimeLimitValue.toolTimeLimitExpression(lit("$(inputs.timeout)")), 1, "ToolTimeLimitExpression", [lit("$(inputs.timeout)")]);
    });

    const workDirCases: FactoryCase[] = [
        [0, "DirentEntry", InitialWorkDirEntry.direntEntry(dirent), [dirent]],
        [1, "StringEntry", InitialWorkDirEntry.stringEntry(include), [include]],
        [2, "FileEntry", InitialWorkDirEntry.fileEntry(file), [file]],
        [3, "DirectoryEntry", InitialWorkDirEntry.directoryEntry(directory), [directory]],
        [4, "NullEntry", InitialWorkDirEntry.nullEntry(), []],
        [5, "FilesEntry", InitialWorkDirEntry.filesEntry([]), [[]]],
        [6, "RawEntry", InitialWorkDirEntry.rawEntry(yamlValue), [yamlValue]],
    ];
    it.each(workDirCases)("InitialWorkDirEntry %s %s", (tag, name, actual, fields) => {
        expectUnion(actual, tag, name, fields);
    });

    const inlineJavascript = new InlineJavascriptRequirementValue([lit("function echo(x) { return x; }")]);
    const schemaDefs = [new SchemaDefEntry(0, [new SchemaDefinition(2, [new CWL.InputArraySchema(stringType, undefined, undefined, "Message")])])];
    const docker = new CWL.DockerRequirement(lit("alpine:3"));
    const software = values([new CWL.SoftwarePackage(lit("echo"), [lit("1.0")])]);
    const loadListing = new LoadListingRequirementValue(listing);
    const workDir = [InitialWorkDirEntry.direntEntry(dirent)];
    const environment = values([new CWL.EnvironmentDef(lit("MESSAGE"), lit("hello"))]);
    const resources = new CWL.ResourceRequirementInstance(1);
    const workReuse = new WorkReuseRequirementValue(false);
    const network = new NetworkAccessRequirementValue(false);
    const inplaceUpdate = new InplaceUpdateRequirementValue(bool(true));
    const timeLimit = ToolTimeLimitValue.toolTimeLimitSeconds(60n);
    const requirementCases: FactoryCase[] = [
        [0, "InlineJavascriptRequirement", CWL.Requirement.inlineJavascriptRequirement(inlineJavascript), [inlineJavascript]],
        [1, "SchemaDefRequirement", CWL.Requirement.schemaDefRequirement(schemaDefs), [schemaDefs]],
        [2, "DockerRequirement", CWL.Requirement.dockerRequirement(docker), [docker]],
        [3, "SoftwareRequirement", CWL.Requirement.softwareRequirement(software), [software]],
        [4, "LoadListingRequirement", CWL.Requirement.loadListingRequirement(loadListing), [loadListing]],
        [5, "InitialWorkDirRequirement", CWL.Requirement.initialWorkDirRequirement(workDir), [workDir]],
        [6, "InitialWorkDirExpressionRequirement", CWL.Requirement.initialWorkDirExpressionRequirement(lit("$(inputs.listing)")), [lit("$(inputs.listing)")]],
        [7, "EnvVarRequirement", CWL.Requirement.envVarRequirement(environment), [environment]],
        [8, "ShellCommandRequirement", CWL.Requirement.shellCommandRequirement(), []],
        [9, "ResourceRequirement", CWL.Requirement.resourceRequirement(resources), [resources]],
        [10, "WorkReuseRequirement", CWL.Requirement.workReuseRequirement(workReuse), [workReuse]],
        [11, "WorkReuseExpressionRequirement", CWL.Requirement.workReuseExpressionRequirement(lit("$(inputs.reuse)")), [lit("$(inputs.reuse)")]],
        [12, "NetworkAccessRequirement", CWL.Requirement.networkAccessRequirement(network), [network]],
        [13, "NetworkAccessExpressionRequirement", CWL.Requirement.networkAccessExpressionRequirement(lit("$(inputs.network)")), [lit("$(inputs.network)")]],
        [14, "InplaceUpdateRequirement", CWL.Requirement.inplaceUpdateRequirement(inplaceUpdate), [inplaceUpdate]],
        [15, "ToolTimeLimitRequirement", CWL.Requirement.toolTimeLimitRequirement(timeLimit), [timeLimit]],
        [16, "SubworkflowFeatureRequirement", CWL.Requirement.subworkflowFeatureRequirement(), []],
        [17, "ScatterFeatureRequirement", CWL.Requirement.scatterFeatureRequirement(), []],
        [18, "MultipleInputFeatureRequirement", CWL.Requirement.multipleInputFeatureRequirement(), []],
        [19, "StepInputExpressionRequirement", CWL.Requirement.stepInputExpressionRequirement(), []],
        [20, "ImportedRequirement", CWL.Requirement.importedRequirement("requirements.yml"), ["requirements.yml"]],
        [21, "MixedRequirement", CWL.Requirement.mixedRequirement(yamlValue), [yamlValue]],
        [22, "RawRequirement", CWL.Requirement.rawRequirement(yamlValue), [yamlValue]],
    ];
    it.each(requirementCases)("Requirement %s %s", (tag, name, actual, fields) => {
        expectUnion(actual, tag, name, fields);
    });

    it("HintEntry preserves both known and extension hints", () => {
        const unknown = new HintUnknownValue("example:Extension", yamlValue);
        expectUnion(HintEntry.knownHint(requirement), 0, "KnownHint", [requirement]);
        expectUnion(HintEntry.unknownHint(unknown), 1, "UnknownHint", [unknown]);
        expectUnion(HintEntry.importedHint("hints.yml"), 2, "ImportedHint", ["hints.yml"]);
        expectUnion(HintEntry.mixedHint(yamlValue), 3, "MixedHint", [yamlValue]);
        expectUnion(HintEntry.rawHint(yamlValue), 4, "RawHint", [yamlValue]);
    });

    it.each([
        [0, "MergeNested", LinkMergeMethod.mergeNested()], [1, "MergeFlattened", LinkMergeMethod.mergeFlattened()],
    ] as const)(
        "LinkMergeMethod %s %s", (tag, name, actual) => expectUnion(actual, tag, name, []),
    );
    const methodDirectiveCases: [union: string, ...FactoryCase][] = [
        ["LinkMergeMethod", 2, "ImportedLinkMerge", LinkMergeMethod.importedLinkMerge("merge.yml"), ["merge.yml"]],
        ["LinkMergeMethod", 3, "IncludedLinkMerge", LinkMergeMethod.includedLinkMerge("merge.txt"), ["merge.txt"]],
        ["LinkMergeMethod", 4, "MixedLinkMerge", LinkMergeMethod.mixedLinkMerge(yamlValue), [yamlValue]],
        ["PickValueMethod", 3, "ImportedPickValue", PickValueMethod.importedPickValue("pick.yml"), ["pick.yml"]],
        ["PickValueMethod", 4, "IncludedPickValue", PickValueMethod.includedPickValue("pick.txt"), ["pick.txt"]],
        ["PickValueMethod", 5, "MixedPickValue", PickValueMethod.mixedPickValue(yamlValue), [yamlValue]],
        ["ScatterMethod", 3, "ImportedScatterMethod", ScatterMethod.importedScatterMethod("scatter.yml"), ["scatter.yml"]],
        ["ScatterMethod", 4, "IncludedScatterMethod", ScatterMethod.includedScatterMethod("scatter.txt"), ["scatter.txt"]],
        ["ScatterMethod", 5, "MixedScatterMethod", ScatterMethod.mixedScatterMethod(yamlValue), [yamlValue]],
    ];
    it.each(methodDirectiveCases)("%s %s %s", (_union, tag, name, actual, fields) => {
        expectUnion(actual, tag, name, fields);
    });
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
        expectUnion(CWL.StepOutput.stepOutputInclude("output.txt"), 2, "StepOutputInclude", ["output.txt"]);
        expectUnion(CWL.StepOutput.stepOutputImport("outputs.yml"), 3, "StepOutputImport", ["outputs.yml"]);
        expectUnion(CWL.StepOutput.stepOutputMixed(yamlValue), 4, "StepOutputMixed", [yamlValue]);
    });

    const runCases: FactoryCase[] = [
        [0, "RunString", WorkflowStepRun.runString("tool.cwl"), ["tool.cwl"]],
        [1, "RunImport", WorkflowStepRun.runImport("tool.cwl"), ["tool.cwl"]],
        [2, "RunMixed", WorkflowStepRun.runMixed(yamlValue), [yamlValue]],
        [3, "RunRaw", WorkflowStepRun.runRaw(yamlValue), [yamlValue]],
        [4, "RunCommandLineTool", WorkflowStepRun.runCommandLineTool(tool), [tool]],
        [5, "RunWorkflow", WorkflowStepRun.runWorkflow(workflow), [workflow]],
        [6, "RunExpressionTool", WorkflowStepRun.runExpressionTool(expressionTool), [expressionTool]],
        [7, "RunOperation", WorkflowStepRun.runOperation(operation), [operation]],
    ];
    it.each(runCases)("WorkflowStepRun %s %s", (tag, name, actual, fields) => {
        expectUnion(actual, tag, name, fields);
    });

    const processingUnitCases: FactoryCase[] = [
        [0, "CommandLineTool", CWL.CWLProcessingUnit.commandLineTool(tool), [tool]],
        [1, "Workflow", CWL.CWLProcessingUnit.workflow(workflow), [workflow]],
        [2, "ExpressionTool", CWL.CWLProcessingUnit.expressionTool(expressionTool), [expressionTool]],
        [3, "Operation", CWL.CWLProcessingUnit.operation(operation), [operation]],
        [4, "RawProcess", CWL.CWLProcessingUnit.rawProcess(yamlValue), [yamlValue]],
    ];
    it.each(processingUnitCases)("CWLProcessingUnit %s %s", (tag, name, actual, fields) => {
        expectUnion(actual, tag, name, fields);
    });
});