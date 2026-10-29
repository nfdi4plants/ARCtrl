"""Native Python counterparts of JavaScript/CWLConstructors.test.ts."""

import pytest
from fable_library.util import equal_arrays

from src.ARCtrl import cwl as C


def expect_properties(actual, **expected):
    # Expected public names are independent of generated storage names.
    for name, value in expected.items():
        assert getattr(actual, name) == value, name


def expect_union(actual, tag, name, fields):
    assert actual.tag == tag
    assert actual.cases()[tag] == name
    assert equal_arrays(actual.fields, fields)


@pytest.fixture
def values():
    from src.ARCtrl.py.fable_modules.yamlicious.yamlicious_types import (
        YAMLContent, YAMLElement_Value,
    )

    string_type = C.CWLType.string()
    yaml_content = YAMLContent("sample.txt", None, None, None, None)
    yaml_value = YAMLElement_Value(yaml_content)
    literal = C.SchemaSaladString.literal("Hello")
    include = C.SchemaSaladString.include("message.txt")
    file = C.FileInstance("file:///data/sample.txt", "/data/sample.txt")
    directory = C.DirectoryInstance("file:///data", "/data")
    dirent = C.DirentInstance(literal)
    listing = C.LoadListingEnum.deep_listing()
    input = C.CWLInput("message", string_type)
    output = C.CWLOutput("result", string_type)
    input_binding = C.InputBinding("--message", 2)
    output_binding = C.OutputBinding("result.txt")
    requirement = C.Requirement.docker_requirement(C.DockerRequirement("alpine:3"))
    hint = C.HintEntry.known_hint(requirement)
    run = C.WorkflowStepRun.run_string("tool.cwl")
    step_input = C.StepInput("message")
    step_output = C.StepOutput.step_output_string("result")
    step = C.WorkflowStep("echo", [step_input], [step_output], run)
    tool = C.CWLToolDescription([output])
    workflow = C.CWLWorkflowDescription([step], [input], [output])
    expression_tool = C.CWLExpressionToolDescription([output], "$(inputs.message)")
    operation = C.CWLOperationDescription([input], [output])
    return locals()


class TestFilesAndSchemas:
    def test_file_instance(self, values):
        expect_properties(C.FileInstance(), **dict.fromkeys([
            "Location", "Path", "Basename", "Dirname", "Nameroot", "Nameext",
            "Checksum", "Size", "SecondaryFiles", "Format", "Contents",
        ]))
        yaml = values["yaml_value"]
        actual = C.FileInstance("file:///data/input.txt", "/data/input.txt", "input.txt",
                                "/data", "input", ".txt", "sha1$abc", 42, yaml, "text/plain", "contents")
        expect_properties(actual, Location="file:///data/input.txt", Path="/data/input.txt",
                          Basename="input.txt", Dirname="/data", Nameroot="input", Nameext=".txt",
                          Checksum="sha1$abc", Size=42, SecondaryFiles=yaml, Format="text/plain", Contents="contents")

    def test_directory_instance(self, values):
        expect_properties(C.DirectoryInstance(), Location=None, Path=None, Basename=None, Listing=None)
        yaml = values["yaml_value"]
        expect_properties(C.DirectoryInstance("file:///data", "/data", "data", yaml),
                          Location="file:///data", Path="/data", Basename="data", Listing=yaml)

    def test_dirent_instance(self, values):
        literal, include = values["literal"], values["include"]
        expect_properties(C.DirentInstance(literal), Entry=literal, Entryname=None, Writable=None)
        expect_properties(C.DirentInstance(include, literal, False), Entry=include, Entryname=literal, Writable=False)

    def test_input_enum_schema(self):
        expect_properties(C.InputEnumSchema([]), Symbols=[], Label=None, Doc=None, Name=None)
        expect_properties(C.InputEnumSchema(["red", "blue"], "Colour", "Allowed colours", "ColourType"),
                          Symbols=["red", "blue"], Label="Colour", Doc="Allowed colours", Name="ColourType")

    def test_input_record_field(self, values):
        typ = values["string_type"]
        expect_properties(C.InputRecordField("message", typ), Name="message", Type=typ, Doc=None, Label=None)
        expect_properties(C.InputRecordField("message", typ, "A message", "Message"),
                          Name="message", Type=typ, Doc="A message", Label="Message")

    def test_input_record_schema(self, values):
        expect_properties(C.InputRecordSchema(), Fields=None, Label=None, Doc=None, Name=None)
        fields = [C.InputRecordField("message", values["string_type"])]
        expect_properties(C.InputRecordSchema(fields, "Record", "Record documentation", "MessageRecord"),
                          Fields=fields, Label="Record", Doc="Record documentation", Name="MessageRecord")

    def test_input_array_schema(self, values):
        typ = values["string_type"]
        expect_properties(C.InputArraySchema(typ), Items=typ, Label=None, Doc=None, Name=None)
        expect_properties(C.InputArraySchema(typ, "Messages", "Array documentation", "MessagesType"),
                          Items=typ, Label="Messages", Doc="Array documentation", Name="MessagesType")

    def test_schema_def_requirement_type(self, values):
        typ = values["string_type"]
        expect_properties(C.SchemaDefRequirementType("Message", typ), Name="Message", Type_=typ)

    def test_software_package(self):
        expect_properties(C.SoftwarePackage("echo"), Package="echo", Version=None, Specs=None)
        expect_properties(C.SoftwarePackage("echo", ["1.0", "2.0"], ["https://example.org/echo"]),
                          Package="echo", Version=["1.0", "2.0"], Specs=["https://example.org/echo"])


class TestInputsOutputsAndParameters:
    @pytest.mark.parametrize("separate,load_contents,shell_quote", [
        (True, False, False), (False, True, False), (False, False, True),
    ])
    def test_input_binding_independent_booleans(self, separate, load_contents, shell_quote):
        expect_properties(C.InputBinding(None, None, None, separate, load_contents, None, shell_quote),
                          Separate=separate, LoadContents=load_contents, ShellQuote=shell_quote)

    @pytest.mark.parametrize("optional,streamable,load_contents", [
        (True, False, False), (False, True, False), (False, False, True),
    ])
    def test_cwl_input_independent_booleans(self, optional, streamable, load_contents):
        expect_properties(C.CWLInput("message", None, None, optional, None, None, streamable,
                                     None, None, load_contents),
                          Optional=optional, Streamable=streamable, LoadContents=load_contents)

    def test_input_binding(self):
        expect_properties(C.InputBinding(), Prefix=None, Position=None, ItemSeparator=None,
                          Separate=None, LoadContents=None, ValueFrom=None, ShellQuote=None)
        # Python exposes the Position getter as a public property.
        expect_properties(C.InputBinding("--message", 0, ",", False, True, "$(self)", False),
                          Prefix="--message", Position=0, ItemSeparator=",", Separate=False,
                          LoadContents=True, ValueFrom="$(self)", ShellQuote=False)

    def test_cwl_input(self, values):
        expect_properties(C.CWLInput("message"), Name="message", **dict.fromkeys([
            "Type_", "InputBinding", "Optional", "Label", "SecondaryFiles", "Streamable",
            "Doc", "Format", "LoadContents", "LoadListing", "DefaultValue",
        ]))
        typ, binding, yaml, listing = (values[k] for k in ("string_type", "input_binding", "yaml_value", "listing"))
        expect_properties(C.CWLInput("message", typ, binding, False, "Message", yaml, False,
                                     "Input documentation", "text/plain", True, listing, yaml),
                          Name="message", Type_=typ, InputBinding=binding, Optional=False, Label="Message",
                          SecondaryFiles=yaml, Streamable=False, Doc="Input documentation", Format="text/plain",
                          LoadContents=True, LoadListing=listing, DefaultValue=yaml)

    def test_output_binding(self, values):
        expect_properties(C.OutputBinding(), Glob=None, LoadContents=None, LoadListing=None, OutputEval=None)
        listing = values["listing"]
        expect_properties(C.OutputBinding("*.txt", False, listing, "$(self[0])"),
                          Glob="*.txt", LoadContents=False, LoadListing=listing, OutputEval="$(self[0])")

    def test_cwl_output(self, values):
        expect_properties(C.CWLOutput("result"), Name="result", **dict.fromkeys([
            "Type_", "OutputBinding", "OutputSource", "Label", "SecondaryFiles", "Streamable", "Doc", "Format",
        ]))
        typ, binding, yaml = (values[k] for k in ("string_type", "output_binding", "yaml_value"))
        source = C.OutputSource.multiple(["echo/result", "other/result"])
        expect_properties(C.CWLOutput("result", typ, binding, source, "Result", yaml, False,
                                      "Output documentation", "text/plain"),
                          Name="result", Type_=typ, OutputBinding=binding, OutputSource=source, Label="Result",
                          SecondaryFiles=yaml, Streamable=False, Doc="Output documentation", Format="text/plain")

    def test_parameter_record_field(self):
        value = C.CWLParameterValue.array([C.CWLParameterValue.string("hello")])
        expect_properties(C.CWLParameterRecordField("messages", value), Name="messages", Value=value)

    def test_parameter_reference(self, values):
        expect_properties(C.CWLParameterReference("message"), Key="message", Values=[], Value=None, Type=None)
        flat = C.CWLParameterReference("message", ["hello", "world"])
        expect_properties(flat, Key="message", Values=["hello", "world"], Type=None)
        assert flat.Value.tag == 7
        assert flat.Value == C.CWLParameterValue.array([
            C.CWLParameterValue.string("hello"), C.CWLParameterValue.string("world"),
        ])
        value, typ = C.CWLParameterValue.string("explicit"), values["string_type"]
        expect_properties(C.CWLParameterReference("message", ["ignored"], value, typ),
                          Key="message", Values=["explicit"], Value=value, Type=typ)


class TestRequirementsAndHints:
    def test_docker_requirement(self, values):
        expect_properties(C.DockerRequirement(), **dict.fromkeys([
            "DockerPull", "DockerFile", "DockerImageId", "DockerLoad", "DockerImport",
            "DockerOutputDirectory", "DockerRunOptions",
        ]))
        include = values["include"]
        expect_properties(C.DockerRequirement("alpine:3", include, "sha256:abc", "image.tar",
                                              "https://example.org/image.tar", "/output", ["--read-only"]),
                          DockerPull="alpine:3", DockerFile=include, DockerImageId="sha256:abc", DockerLoad="image.tar",
                          DockerImport="https://example.org/image.tar", DockerOutputDirectory="/output",
                          DockerRunOptions=["--read-only"])

    def test_environment_def(self):
        expect_properties(C.EnvironmentDef("EMPTY", ""), EnvName="EMPTY", EnvValue="")
        expect_properties(C.EnvironmentDef("MESSAGE", "hello"), EnvName="MESSAGE", EnvValue="hello")

    def test_load_listing_requirement_value(self, values):
        listing = values["listing"]
        expect_properties(C.LoadListingRequirementValue(listing), LoadListing=listing)

    @pytest.mark.parametrize("value", [False, True])
    def test_boolean_requirement_values(self, value):
        expect_properties(C.WorkReuseRequirementValue(value), EnableReuse=value)
        expect_properties(C.NetworkAccessRequirementValue(value), NetworkAccess=value)
        expect_properties(C.InplaceUpdateRequirementValue(value), InplaceUpdate=value)

    def test_resource_requirement_instance(self):
        expect_properties(C.ResourceRequirementInstance(), **dict.fromkeys([
            "CoresMin", "CoresMax", "RamMin", "RamMax", "TmpdirMin", "TmpdirMax", "OutdirMin", "OutdirMax",
        ]))
        expect_properties(C.ResourceRequirementInstance(1, "$(inputs.cores)", 256, 512, 0, 1024, 2048, "$(inputs.disk)"),
                          CoresMin=1, CoresMax="$(inputs.cores)", RamMin=256, RamMax=512,
                          TmpdirMin=0, TmpdirMax=1024, OutdirMin=2048, OutdirMax="$(inputs.disk)")

    def test_inline_javascript_requirement_value(self):
        expect_properties(C.InlineJavascriptRequirementValue(), ExpressionLib=None)
        lib = ["function echo(x) { return x; }"]
        expect_properties(C.InlineJavascriptRequirementValue(lib), ExpressionLib=lib)

    def test_hint_unknown_value(self, values):
        yaml = values["yaml_value"]
        expect_properties(C.HintUnknownValue(None, yaml), Class=None, Raw=yaml)
        expect_properties(C.HintUnknownValue("example:Extension", yaml), Class="example:Extension", Raw=yaml)


class TestWorkflowSteps:
    def test_step_input(self, values):
        expect_properties(C.StepInput("message"), Id="message", **dict.fromkeys([
            "Source", "DefaultValue", "ValueFrom", "LinkMerge", "PickValue", "Doc", "LoadContents", "LoadListing", "Label",
        ]))
        yaml = values["yaml_value"]
        merge, pick = C.LinkMergeMethod.merge_flattened(), C.PickValueMethod.first_non_null()
        expect_properties(C.StepInput("message", ["workflow/message"], yaml, "$(self)", merge, pick,
                                       "Step input documentation", False, "deep_listing", "Message"),
                          Id="message", Source=["workflow/message"], DefaultValue=yaml, ValueFrom="$(self)",
                          LinkMerge=merge, PickValue=pick, Doc="Step input documentation", LoadContents=False,
                          LoadListing="deep_listing", Label="Message")

    def test_step_output_parameter(self):
        expect_properties(C.StepOutputParameter("result"), Id="result")

    def test_workflow_step(self, values):
        si, so, run, requirement, hint = (values[k] for k in ("step_input", "step_output", "run", "requirement", "hint"))
        expect_properties(C.WorkflowStep("echo", [si], [so], run), Id="echo", In=[si], Out=[so], Run=run,
                          Label=None, Doc=None, Scatter=None, ScatterMethod=None, When_=None, Requirements=None, Hints=None)
        scatter = C.ScatterMethod.flat_cross_product()
        expect_properties(C.WorkflowStep("echo", [si], [so], run, "Echo", "Step documentation", ["message"], scatter,
                                          "$(inputs.message != null)", [requirement], [hint]),
                          Id="echo", In=[si], Out=[so], Run=run, Label="Echo", Doc="Step documentation", Scatter=["message"],
                          ScatterMethod=scatter, When_="$(inputs.message != null)", Requirements=[requirement], Hints=[hint])


class TestProcessingDescriptions:
    @pytest.fixture
    def annotations(self, values):
        metadata = C.EnvironmentDef("creator", "constructor test")
        return dict(CWLVersion="v1.1", Requirements=[values["requirement"]], Hints=[values["hint"]],
                    Intent=["https://example.org/echo"], Metadata=metadata,
                    Label="Echo", Doc="Processing documentation", Id="#echo")

    @pytest.fixture
    def absent_annotations(self):
        return dict(CWLVersion="v1.2", **dict.fromkeys([
            "Requirements", "Hints", "Intent", "Metadata", "Label", "Doc", "Id",
        ]))

    def test_tool(self, values, annotations, absent_annotations):
        expect_properties(C.CWLToolDescription([]), **absent_annotations, Outputs=[], **dict.fromkeys([
            "BaseCommand", "Arguments", "Stdin", "Stderr", "Stdout", "SuccessCodes", "TemporaryFailCodes",
            "PermanentFailCodes", "Inputs",
        ]))
        output, input, yaml = (values[k] for k in ("output", "input", "yaml_value"))
        a = annotations
        expect_properties(C.CWLToolDescription([output], "v1.1", ["echo", "hello"], yaml, "input.txt", "error.log",
                                              "output.txt", [0], [75], [1, 2], a["Requirements"], a["Hints"], a["Intent"],
                                              [input], a["Metadata"], "Echo", "Processing documentation", "#echo"),
                          **a, Outputs=[output], BaseCommand=["echo", "hello"], Arguments=yaml, Stdin="input.txt",
                          Stderr="error.log", Stdout="output.txt", SuccessCodes=[0], TemporaryFailCodes=[75],
                          PermanentFailCodes=[1, 2], Inputs=[input])

    def test_workflow(self, values, annotations, absent_annotations):
        expect_properties(C.CWLWorkflowDescription([], [], []), **absent_annotations, Steps=[], Inputs=[], Outputs=[])
        step, input, output = (values[k] for k in ("step", "input", "output"))
        a = annotations
        expect_properties(C.CWLWorkflowDescription([step], [input], [output], "v1.1", a["Requirements"], a["Hints"],
                                                   a["Intent"], a["Metadata"], "Echo", "Processing documentation", "#echo"),
                          **a, Steps=[step], Inputs=[input], Outputs=[output])

    def test_expression_tool(self, values, annotations, absent_annotations):
        expect_properties(C.CWLExpressionToolDescription([], "$(inputs.message)"), **absent_annotations,
                          Outputs=[], Expression="$(inputs.message)", Inputs=None)
        input, output, a = values["input"], values["output"], annotations
        expect_properties(C.CWLExpressionToolDescription([output], "$(inputs.message)", "v1.1", a["Requirements"],
                                                         a["Hints"], a["Intent"], [input], a["Metadata"],
                                                         "Echo", "Processing documentation", "#echo"),
                          **a, Outputs=[output], Expression="$(inputs.message)", Inputs=[input])

    def test_operation(self, values, annotations, absent_annotations):
        expect_properties(C.CWLOperationDescription([], []), **absent_annotations, Inputs=[], Outputs=[])
        input, output, a = values["input"], values["output"], annotations
        expect_properties(C.CWLOperationDescription([input], [output], "v1.1", a["Requirements"], a["Hints"], a["Intent"],
                                                   a["Metadata"], "Echo", "Processing documentation", "#echo"),
                          **a, Inputs=[input], Outputs=[output])


class TestUnionFactoryHelpers:
    @pytest.mark.parametrize("tag,name,factory,value", [
        (0, "Literal", "literal", "literal text"),
        (1, "Include", "include", "contents.txt"),
        (2, "Import", "import_", "tool.cwl"),
    ])
    def test_schema_salad_string(self, tag, name, factory, value):
        expect_union(getattr(C.SchemaSaladString, factory)(value), tag, name, [value])

    @pytest.mark.parametrize("tag,name,factory", [
        (3, "String", "string"), (4, "Int", "int"), (5, "Long", "long"),
        (6, "Float", "float"), (7, "Double", "double"), (8, "Boolean", "boolean"),
        (9, "Stdout", "stdout"), (10, "Null", "null"),
    ])
    def test_cwl_primitive_type(self, tag, name, factory):
        expect_union(getattr(C.CWLType, factory)(), tag, name, [])

    @pytest.mark.parametrize("tag,name,factory,key", [
        (0, "File", "file", "file"), (1, "Directory", "directory", "directory"),
        (2, "Dirent", "dirent", "dirent"), (11, "Array", "array", "array_schema"),
        (12, "Record", "record", "record_schema"), (13, "Enum", "enum", "enum_schema"),
        (14, "Union", "union", "union_types"),
    ])
    def test_cwl_nested_type(self, values, tag, name, factory, key):
        nested = dict(values, array_schema=C.InputArraySchema(values["string_type"]),
                      record_schema=C.InputRecordSchema([C.InputRecordField("message", values["string_type"])]),
                      enum_schema=C.InputEnumSchema(["red", "blue"]),
                      union_types=[values["string_type"], C.CWLType.null()])[key]
        expect_union(getattr(C.CWLType, factory)(nested), tag, name, [nested])

    def test_file_and_directory_default_instances(self):
        expect_union(C.CWLType.file(), 0, "File", [C.FileInstance()])
        expect_union(C.CWLType.directory(), 1, "Directory", [C.DirectoryInstance()])

    @pytest.mark.parametrize("tag,name,factory,key", [
        (0, "Null", "null", None), (1, "String", "string", "string"),
        (2, "Int", "int", "int"), (3, "Float", "float", "float"),
        (4, "Boolean", "boolean", "boolean"), (5, "File", "file", "file"),
        (6, "Directory", "directory", "directory"), (7, "Array", "array", "array"),
        (8, "Record", "record", "record"),
    ])
    def test_parameter_value(self, values, tag, name, factory, key):
        payloads = dict(values, string="hello", int=9007199254740993, float=1.25, boolean=False,
                        array=[C.CWLParameterValue.string("hello"), C.CWLParameterValue.null()],
                        record=[C.CWLParameterRecordField("message", C.CWLParameterValue.string("hello"))])
        fields = [] if key is None else [payloads[key]]
        expect_union(getattr(C.CWLParameterValue, factory)(*fields), tag, name, fields)

    @pytest.mark.parametrize("tag,name,factory,value", [
        (0, "Single", "single", "echo/result"),
        (1, "Multiple", "multiple", ["echo/result", "other/result"]),
    ])
    def test_output_source(self, tag, name, factory, value):
        expect_union(getattr(C.OutputSource, factory)(value), tag, name, [value])

    @pytest.mark.parametrize("tag,name,factory", [
        (0, "NoListing", "no_listing"), (1, "ShallowListing", "shallow_listing"),
        (2, "DeepListing", "deep_listing"),
    ])
    def test_load_listing_enum(self, tag, name, factory):
        expect_union(getattr(C.LoadListingEnum, factory)(), tag, name, [])

    def test_tool_time_limit_value(self):
        expect_union(C.ToolTimeLimitValue.tool_time_limit_seconds(120), 0, "ToolTimeLimitSeconds", [120])
        expression = "$(inputs.timeout)"
        expect_union(C.ToolTimeLimitValue.tool_time_limit_expression(expression), 1, "ToolTimeLimitExpression", [expression])

    @pytest.mark.parametrize("tag,name,factory,key", [
        (0, "DirentEntry", "dirent_entry", "dirent"), (1, "StringEntry", "string_entry", "include"),
        (2, "FileEntry", "file_entry", "file"), (3, "DirectoryEntry", "directory_entry", "directory"),
    ])
    def test_initial_work_dir_entry(self, values, tag, name, factory, key):
        value = values[key]
        expect_union(getattr(C.InitialWorkDirEntry, factory)(value), tag, name, [value])

    @pytest.mark.parametrize("tag,name,factory,key", [
        (0, "InlineJavascriptRequirement", "inline_javascript_requirement", "inline_javascript"),
        (1, "SchemaDefRequirement", "schema_def_requirement", "schema_defs"),
        (2, "DockerRequirement", "docker_requirement", "docker"),
        (3, "SoftwareRequirement", "software_requirement", "software"),
        (4, "LoadListingRequirement", "load_listing_requirement", "load_listing"),
        (5, "InitialWorkDirRequirement", "initial_work_dir_requirement", "work_dir"),
        (6, "EnvVarRequirement", "env_var_requirement", "environment"),
        (7, "ShellCommandRequirement", "shell_command_requirement", None),
        (8, "ResourceRequirement", "resource_requirement", "resources"),
        (9, "WorkReuseRequirement", "work_reuse_requirement", "work_reuse"),
        (10, "WorkReuseExpressionRequirement", "work_reuse_expression_requirement", "work_reuse_expression"),
        (11, "NetworkAccessRequirement", "network_access_requirement", "network"),
        (12, "NetworkAccessExpressionRequirement", "network_access_expression_requirement", "network_expression"),
        (13, "InplaceUpdateRequirement", "inplace_update_requirement", "inplace_update"),
        (14, "ToolTimeLimitRequirement", "tool_time_limit_requirement", "time_limit"),
        (15, "SubworkflowFeatureRequirement", "subworkflow_feature_requirement", None),
        (16, "ScatterFeatureRequirement", "scatter_feature_requirement", None),
        (17, "MultipleInputFeatureRequirement", "multiple_input_feature_requirement", None),
        (18, "StepInputExpressionRequirement", "step_input_expression_requirement", None),
    ])
    def test_requirement(self, values, tag, name, factory, key):
        payloads = dict(
            inline_javascript=C.InlineJavascriptRequirementValue(["function echo(x) { return x; }"]),
            schema_defs=[C.SchemaDefRequirementType("Message", values["string_type"])],
            docker=C.DockerRequirement("alpine:3"), software=[C.SoftwarePackage("echo", ["1.0"])],
            load_listing=C.LoadListingRequirementValue(values["listing"]),
            work_dir=[C.InitialWorkDirEntry.dirent_entry(values["dirent"])],
            environment=[C.EnvironmentDef("MESSAGE", "hello")], resources=C.ResourceRequirementInstance(1),
            work_reuse=C.WorkReuseRequirementValue(False), network=C.NetworkAccessRequirementValue(False),
            inplace_update=C.InplaceUpdateRequirementValue(True), time_limit=C.ToolTimeLimitValue.tool_time_limit_seconds(60),
            work_reuse_expression="$(inputs.reuse)", network_expression="$(inputs.network)",
        )
        fields = [] if key is None else [payloads[key]]
        expect_union(getattr(C.Requirement, factory)(*fields), tag, name, fields)

    def test_hint_entry(self, values):
        requirement = values["requirement"]
        unknown = C.HintUnknownValue("example:Extension", values["yaml_value"])
        expect_union(C.HintEntry.known_hint(requirement), 0, "KnownHint", [requirement])
        expect_union(C.HintEntry.unknown_hint(unknown), 1, "UnknownHint", [unknown])

    @pytest.mark.parametrize("union,tag,name,factory", [
        ("LinkMergeMethod", 0, "MergeNested", "merge_nested"),
        ("LinkMergeMethod", 1, "MergeFlattened", "merge_flattened"),
        ("PickValueMethod", 0, "FirstNonNull", "first_non_null"),
        ("PickValueMethod", 1, "TheOnlyNonNull", "the_only_non_null"),
        ("PickValueMethod", 2, "AllNonNull", "all_non_null"),
        ("ScatterMethod", 0, "DotProduct", "dot_product"),
        ("ScatterMethod", 1, "NestedCrossProduct", "nested_cross_product"),
        ("ScatterMethod", 2, "FlatCrossProduct", "flat_cross_product"),
    ])
    def test_workflow_methods(self, union, tag, name, factory):
        expect_union(getattr(getattr(C, union), factory)(), tag, name, [])

    def test_step_output(self):
        parameter = C.StepOutputParameter("result")
        expect_union(C.StepOutput.step_output_string("result"), 0, "StepOutputString", ["result"])
        expect_union(C.StepOutput.step_output_record(parameter), 1, "StepOutputRecord", [parameter])

    @pytest.mark.parametrize("tag,name,factory,key", [
        (0, "RunString", "run_string", "run_path"),
        (1, "RunCommandLineTool", "run_command_line_tool", "tool"),
        (2, "RunWorkflow", "run_workflow", "workflow"),
        (3, "RunExpressionTool", "run_expression_tool", "expression_tool"),
        (4, "RunOperation", "run_operation", "operation"),
    ])
    def test_workflow_step_run(self, values, tag, name, factory, key):
        value = dict(values, run_path="tool.cwl")[key]
        expect_union(getattr(C.WorkflowStepRun, factory)(value), tag, name, [value])

    @pytest.mark.parametrize("tag,name,factory,key", [
        (0, "CommandLineTool", "command_line_tool", "tool"),
        (1, "Workflow", "workflow", "workflow"),
        (2, "ExpressionTool", "expression_tool", "expression_tool"),
        (3, "Operation", "operation", "operation"),
    ])
    def test_processing_unit(self, values, tag, name, factory, key):
        value = values[key]
        expect_union(getattr(C.CWLProcessingUnit, factory)(value), tag, name, [value])


class TestNestedYamlValues:
    def test_yaml_content(self):
        from src.ARCtrl.py.fable_modules.yamlicious.yamlicious_types import (
            YAMLContent, ScalarStyle_Block, BlockScalarStyle_Literal, ChompingMode_Keep,
        )

        expect_properties(YAMLContent("hello", None, None, None, None),
                          value="hello", comment=None, anchor=None, tag=None, style=None)
        style = ScalarStyle_Block(BlockScalarStyle_Literal(), ChompingMode_Keep(), 2)
        expect_properties(YAMLContent("hello\nworld", "comment", "message", "!!str", style),
                          value="hello\nworld", comment="comment", anchor="message", tag="!!str", style=style)

    @pytest.mark.parametrize("union,tag,name", [
        ("BlockScalarStyle", 0, "Literal"), ("BlockScalarStyle", 1, "Folded"),
        ("ChompingMode", 0, "Strip"), ("ChompingMode", 1, "Clip"), ("ChompingMode", 2, "Keep"),
        ("ScalarStyle", 0, "Plain"), ("ScalarStyle", 1, "SingleQuoted"), ("ScalarStyle", 2, "DoubleQuoted"),
    ])
    def test_scalar_union_cases(self, union, tag, name):
        from src.ARCtrl.py.fable_modules.yamlicious import yamlicious_types as Y

        expect_union(getattr(Y, f"{union}_{name}")(), tag, name, [])

    def test_block_scalar_indentation(self):
        from src.ARCtrl.py.fable_modules.yamlicious.yamlicious_types import (
            ScalarStyle_Block, BlockScalarStyle_Folded, ChompingMode_Strip,
        )

        block, chomping = BlockScalarStyle_Folded(), ChompingMode_Strip()
        expect_union(ScalarStyle_Block(block, chomping, None), 3, "Block", [block, chomping, None])
        expect_union(ScalarStyle_Block(block, chomping, 4), 3, "Block", [block, chomping, 4])

    @pytest.mark.parametrize("tag,name", [
        (0, "Mapping"), (1, "Value"), (2, "Sequence"), (3, "Object"), (4, "Comment"),
        (5, "DocumentStart"), (6, "DocumentEnd"), (7, "Alias"), (8, "Nil"),
    ])
    def test_yaml_element(self, values, tag, name):
        from src.ARCtrl.py.fable_modules.yamlicious import yamlicious_types as Y
        from fable_library.list import of_array

        yaml, content = values["yaml_value"], values["yaml_content"]
        fields = [
            [Y.YAMLContent("message", None, None, None, None), yaml], [content], [of_array([yaml])],
            [of_array([Y.YAMLElement_Mapping(content, yaml)])], ["a comment"], [], [], ["message"], [],
        ][tag]
        expect_union(getattr(Y, f"YAMLElement_{name}")(*fields), tag, name, fields)
