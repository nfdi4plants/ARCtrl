"""Native Python counterparts of JavaScript/CWLConstructors.test.ts."""

import pytest
from fable_library.util import equal_arrays

from src.ARCtrl import cwl as C
from src.ARCtrl.py.CWL.cwltypes import (
    RawYaml, SchemaSaladBool_Value, FileSize_Bytes, SchemaDefinition_Array,
)
from src.ARCtrl.py.CWL.inputs import BindingPosition_Number, InputFormat_Single
from src.ARCtrl.py.CWL.outputs import OutputGlob_Single
from src.ARCtrl.py.CWL.requirements import SchemaDefEntry_Defined
from src.ARCtrl.py.fable_modules.yamlicious.node import YamlNodeModule_plain


# Strings, booleans and lists of the CWL model take the union that also holds a Schema Salad directive.
def lit(text):
    return C.SchemaSaladString.literal(text)


def boolean(value):
    return SchemaSaladBool_Value(value)


def doc(text):
    return C.DocText.literal(text)


def entries(items):
    return C.ListEntry.of_values(items)


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
    from src.ARCtrl.py.fable_modules.yamlicious.model import YamlScalar, ScalarStyle_Plain

    string_type = C.CWLType.string()
    yaml_content = YamlScalar("sample.txt", ScalarStyle_Plain(), None, None)
    yaml_value = RawYaml(YamlNodeModule_plain("sample.txt"))
    literal = C.SchemaSaladString.literal("Hello")
    include = C.SchemaSaladString.include("message.txt")
    file = C.FileInstance(lit("file:///data/sample.txt"), lit("/data/sample.txt"))
    directory = C.DirectoryInstance(lit("file:///data"), lit("/data"))
    dirent = C.DirentInstance(literal)
    listing = C.LoadListingEnum.deep_listing()
    input = C.CWLInput("message", string_type)
    output = C.CWLOutput("result", string_type)
    input_binding = C.InputBinding(lit("--message"), BindingPosition_Number(2))
    output_binding = C.OutputBinding(OutputGlob_Single(lit("result.txt")))
    requirement = C.Requirement.docker_requirement(C.DockerRequirement(lit("alpine:3")))
    hint = C.HintEntry.known_hint(requirement)
    run = C.WorkflowStepRun.run_string("tool.cwl")
    step_input = C.StepInput("message")
    step_output = C.StepOutput.step_output_string("result")
    step = C.WorkflowStep("echo", entries([step_input]), [step_output], run)
    tool = C.CWLToolDescription(entries([output]))
    workflow = C.CWLWorkflowDescription(entries([step]), entries([input]), entries([output]))
    expression_tool = C.CWLExpressionToolDescription(entries([output]), lit("$(inputs.message)"))
    operation = C.CWLOperationDescription(entries([input]), entries([output]))
    return locals()


class TestFilesAndSchemas:
    def test_file_instance(self, values):
        expect_properties(C.FileInstance(), **dict.fromkeys([
            "Location", "Path", "Basename", "Dirname", "Nameroot", "Nameext",
            "Checksum", "Size", "SecondaryFiles", "Format", "Contents",
        ]))
        yaml = values["yaml_value"]
        actual = C.FileInstance(lit("file:///data/input.txt"), lit("/data/input.txt"), lit("input.txt"),
                                lit("/data"), lit("input"), lit(".txt"), lit("sha1$abc"), FileSize_Bytes(42), yaml,
                                lit("text/plain"), lit("contents"))
        expect_properties(actual, Location=lit("file:///data/input.txt"), Path=lit("/data/input.txt"),
                          Basename=lit("input.txt"), Dirname=lit("/data"), Nameroot=lit("input"), Nameext=lit(".txt"),
                          Checksum=lit("sha1$abc"), Size=FileSize_Bytes(42), SecondaryFiles=yaml,
                          Format=lit("text/plain"), Contents=lit("contents"))

    def test_directory_instance(self, values):
        expect_properties(C.DirectoryInstance(), Location=None, Path=None, Basename=None, Listing=None)
        yaml = values["yaml_value"]
        expect_properties(C.DirectoryInstance(lit("file:///data"), lit("/data"), lit("data"), yaml),
                          Location=lit("file:///data"), Path=lit("/data"), Basename=lit("data"), Listing=yaml)

    def test_dirent_instance(self, values):
        literal, include = values["literal"], values["include"]
        expect_properties(C.DirentInstance(literal), Entry=literal, Entryname=None, Writable=None)
        expect_properties(C.DirentInstance(include, literal, boolean(False)), Entry=include, Entryname=literal,
                          Writable=boolean(False))

    def test_input_enum_schema(self):
        expect_properties(C.InputEnumSchema([]), Symbols=[], Label=None, Doc=None, Name=None)
        expect_properties(C.InputEnumSchema([lit("red"), lit("blue")], lit("Colour"), doc("Allowed colours"), "ColourType"),
                          Symbols=[lit("red"), lit("blue")], Label=lit("Colour"), Doc=doc("Allowed colours"),
                          Name="ColourType")

    def test_input_record_field(self, values):
        typ = values["string_type"]
        expect_properties(C.InputRecordField("message", typ), Name="message", Type=typ, Doc=None, Label=None)
        expect_properties(C.InputRecordField("message", typ, doc("A message"), lit("Message")),
                          Name="message", Type=typ, Doc=doc("A message"), Label=lit("Message"))

    def test_input_record_schema(self, values):
        expect_properties(C.InputRecordSchema(), Fields=None, Label=None, Doc=None, Name=None)
        fields = entries([C.InputRecordField("message", values["string_type"])])
        expect_properties(C.InputRecordSchema(fields, lit("Record"), doc("Record documentation"), "MessageRecord"),
                          Fields=fields, Label=lit("Record"), Doc=doc("Record documentation"), Name="MessageRecord")

    def test_input_array_schema(self, values):
        typ = values["string_type"]
        expect_properties(C.InputArraySchema(typ), Items=typ, Label=None, Doc=None, Name=None)
        expect_properties(C.InputArraySchema(typ, lit("Messages"), doc("Array documentation"), "MessagesType"),
                          Items=typ, Label=lit("Messages"), Doc=doc("Array documentation"), Name="MessagesType")

    def test_schema_definition(self, values):
        typ = values["string_type"]
        schema = C.InputArraySchema(typ, None, None, "Message")
        definition = SchemaDefinition_Array(schema)
        expect_union(definition, 2, "Array", [schema])
        expect_properties(definition.fields[0], Name="Message", Items=typ)

    def test_software_package(self):
        expect_properties(C.SoftwarePackage(lit("echo")), Package=lit("echo"), Version=None, Specs=None)
        expect_properties(C.SoftwarePackage(lit("echo"), [lit("1.0"), lit("2.0")], [lit("https://example.org/echo")]),
                          Package=lit("echo"), Version=[lit("1.0"), lit("2.0")], Specs=[lit("https://example.org/echo")])


class TestInputsOutputsAndParameters:
    @pytest.mark.parametrize("separate,load_contents,shell_quote", [
        (True, False, False), (False, True, False), (False, False, True),
    ])
    def test_input_binding_independent_booleans(self, separate, load_contents, shell_quote):
        expect_properties(C.InputBinding(None, None, None, boolean(separate), boolean(load_contents), None,
                                         boolean(shell_quote)),
                          Separate=boolean(separate), LoadContents=boolean(load_contents), ShellQuote=boolean(shell_quote))

    @pytest.mark.parametrize("optional,streamable,load_contents", [
        (True, False, False), (False, True, False), (False, False, True),
    ])
    def test_cwl_input_independent_booleans(self, values, optional, streamable, load_contents):
        # Optional is true exactly when the type admits null.
        typ = C.CWLType.union([values["string_type"], C.CWLType.null()]) if optional else None
        expect_properties(C.CWLInput("message", typ, None, None, None, boolean(streamable),
                                     None, None, boolean(load_contents)),
                          Optional=optional, Streamable=boolean(streamable), LoadContents=boolean(load_contents))

    def test_input_binding(self):
        expect_properties(C.InputBinding(), Prefix=None, Position=None, ItemSeparator=None,
                          Separate=None, LoadContents=None, ValueFrom=None, ShellQuote=None)
        # Python exposes the Position getter as a public property.
        expect_properties(C.InputBinding(lit("--message"), BindingPosition_Number(0), lit(","), boolean(False),
                                         boolean(True), lit("$(self)"), boolean(False)),
                          Prefix=lit("--message"), Position=BindingPosition_Number(0), ItemSeparator=lit(","),
                          Separate=boolean(False), LoadContents=boolean(True), ValueFrom=lit("$(self)"),
                          ShellQuote=boolean(False))

    def test_cwl_input(self, values):
        expect_properties(C.CWLInput("message"), Name="message", Optional=False, **dict.fromkeys([
            "Type_", "InputBinding", "Label", "SecondaryFiles", "Streamable",
            "Doc", "Format", "LoadContents", "LoadListing", "DefaultValue",
        ]))
        typ, binding, yaml, listing = (values[k] for k in ("string_type", "input_binding", "yaml_value", "listing"))
        binding_entry, format = C.ListEntry.of_values([binding])[0], InputFormat_Single(lit("text/plain"))
        expect_properties(C.CWLInput("message", typ, binding_entry, lit("Message"), yaml, boolean(False),
                                     doc("Input documentation"), format, boolean(True), listing, yaml),
                          Name="message", Type_=typ, InputBinding=binding_entry, Optional=False, Label=lit("Message"),
                          SecondaryFiles=yaml, Streamable=boolean(False), Doc=doc("Input documentation"), Format=format,
                          LoadContents=boolean(True), LoadListing=listing, DefaultValue=yaml)

    def test_output_binding(self, values):
        expect_properties(C.OutputBinding(), Glob=None, LoadContents=None, LoadListing=None, OutputEval=None)
        listing = values["listing"]
        glob = OutputGlob_Single(lit("*.txt"))
        expect_properties(C.OutputBinding(glob, boolean(False), listing, lit("$(self[0])")),
                          Glob=glob, LoadContents=boolean(False), LoadListing=listing, OutputEval=lit("$(self[0])"))

    def test_cwl_output(self, values):
        expect_properties(C.CWLOutput("result"), Name="result", **dict.fromkeys([
            "Type_", "OutputBinding", "OutputSource", "Label", "SecondaryFiles", "Streamable", "Doc", "Format",
        ]))
        typ, binding, yaml = (values[k] for k in ("string_type", "output_binding", "yaml_value"))
        source = C.OutputSource.multiple([lit("echo/result"), lit("other/result")])
        binding_entry = C.ListEntry.of_values([binding])[0]
        expect_properties(C.CWLOutput("result", typ, binding_entry, source, lit("Result"), yaml, boolean(False),
                                      doc("Output documentation"), lit("text/plain")),
                          Name="result", Type_=typ, OutputBinding=binding_entry, OutputSource=source, Label=lit("Result"),
                          SecondaryFiles=yaml, Streamable=boolean(False), Doc=doc("Output documentation"),
                          Format=lit("text/plain"))

    def test_parameter_record_field(self):
        value = C.CWLParameterValue.array([C.CWLParameterValue.string("hello")])
        expect_properties(C.CWLParameterRecordField("messages", value), Name="messages", Value=value)

    def test_parameter_reference(self, values):
        expect_properties(C.CWLParameterReference("message"), Key="message", Values=[], Value=None)
        flat = C.CWLParameterReference("message", ["hello", "world"])
        expect_properties(flat, Key="message", Values=["hello", "world"])
        assert flat.Value.tag == 7
        assert flat.Value == C.CWLParameterValue.array([
            C.CWLParameterValue.string("hello"), C.CWLParameterValue.string("world"),
        ])
        value = C.CWLParameterValue.string("explicit")
        expect_properties(C.CWLParameterReference("message", ["ignored"], value),
                          Key="message", Values=["explicit"], Value=value)


class TestRequirementsAndHints:
    def test_docker_requirement(self, values):
        expect_properties(C.DockerRequirement(), **dict.fromkeys([
            "DockerPull", "DockerFile", "DockerImageId", "DockerLoad", "DockerImport",
            "DockerOutputDirectory", "DockerRunOptions",
        ]))
        include = values["include"]
        expect_properties(C.DockerRequirement(lit("alpine:3"), include, lit("sha256:abc"), lit("image.tar"),
                                              lit("https://example.org/image.tar"), lit("/output"), ["--read-only"]),
                          DockerPull=lit("alpine:3"), DockerFile=include, DockerImageId=lit("sha256:abc"),
                          DockerLoad=lit("image.tar"), DockerImport=lit("https://example.org/image.tar"),
                          DockerOutputDirectory=lit("/output"), DockerRunOptions=["--read-only"])

    def test_environment_def(self):
        expect_properties(C.EnvironmentDef(lit("EMPTY"), lit("")), EnvName=lit("EMPTY"), EnvValue=lit(""))
        expect_properties(C.EnvironmentDef(lit("MESSAGE"), lit("hello")), EnvName=lit("MESSAGE"), EnvValue=lit("hello"))

    def test_load_listing_requirement_value(self, values):
        listing = values["listing"]
        expect_properties(C.LoadListingRequirementValue(listing), LoadListing=listing)

    @pytest.mark.parametrize("value", [False, True])
    def test_boolean_requirement_values(self, value):
        expect_properties(C.WorkReuseRequirementValue(value), EnableReuse=value)
        expect_properties(C.NetworkAccessRequirementValue(value), NetworkAccess=value)
        expect_properties(C.InplaceUpdateRequirementValue(boolean(value)), InplaceUpdate=boolean(value))

    def test_resource_requirement_instance(self):
        expect_properties(C.ResourceRequirementInstance(), **dict.fromkeys([
            "CoresMin", "CoresMax", "RamMin", "RamMax", "TmpdirMin", "TmpdirMax", "OutdirMin", "OutdirMax",
        ]))
        expect_properties(C.ResourceRequirementInstance(1, "$(inputs.cores)", 256, 512, 0, 1024, 2048, "$(inputs.disk)"),
                          CoresMin=1, CoresMax="$(inputs.cores)", RamMin=256, RamMax=512,
                          TmpdirMin=0, TmpdirMax=1024, OutdirMin=2048, OutdirMax="$(inputs.disk)")

    def test_inline_javascript_requirement_value(self):
        expect_properties(C.InlineJavascriptRequirementValue(), ExpressionLib=None)
        lib = [lit("function echo(x) { return x; }")]
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
        source = C.OutputSource.multiple([lit("workflow/message")])
        expect_properties(C.StepInput("message", source, yaml, lit("$(self)"), merge, pick,
                                       doc("Step input documentation"), boolean(False), C.LoadListingEnum.deep_listing(),
                                       lit("Message")),
                          Id="message", Source=source, DefaultValue=yaml, ValueFrom=lit("$(self)"),
                          LinkMerge=merge, PickValue=pick, Doc=doc("Step input documentation"), LoadContents=boolean(False),
                          LoadListing=C.LoadListingEnum.deep_listing(), Label=lit("Message"))

    def test_step_output_parameter(self):
        expect_properties(C.StepOutputParameter("result"), Id="result")

    def test_workflow_step(self, values):
        si, so, run, requirement, hint = (values[k] for k in ("step_input", "step_output", "run", "requirement", "hint"))
        expect_properties(C.WorkflowStep("echo", entries([si]), [so], run), Id="echo", In=entries([si]), Out=[so], Run=run,
                          Label=None, Doc=None, Scatter=None, ScatterMethod=None, When_=None, Requirements=None, Hints=None)
        scatter = C.ScatterMethod.flat_cross_product()
        expect_properties(C.WorkflowStep("echo", entries([si]), [so], run, lit("Echo"), doc("Step documentation"),
                                          [lit("message")], scatter, lit("$(inputs.message != null)"), [requirement], [hint]),
                          Id="echo", In=entries([si]), Out=[so], Run=run, Label=lit("Echo"), Doc=doc("Step documentation"),
                          Scatter=[lit("message")], ScatterMethod=scatter, When_=lit("$(inputs.message != null)"),
                          Requirements=[requirement], Hints=[hint])


class TestProcessingDescriptions:
    @pytest.fixture
    def annotations(self, values):
        metadata = C.EnvironmentDef(lit("creator"), lit("constructor test"))
        return dict(CWLVersion="v1.1", Requirements=[values["requirement"]], Hints=[values["hint"]],
                    Intent=[lit("https://example.org/echo")], Metadata=metadata,
                    Label=lit("Echo"), Doc=doc("Processing documentation"), Id="#echo")

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
        expect_properties(C.CWLToolDescription(entries([output]), "v1.1", [lit("echo"), lit("hello")], yaml,
                                              lit("input.txt"), lit("error.log"), lit("output.txt"), entries([0]),
                                              entries([75]), entries([1, 2]), a["Requirements"], a["Hints"], a["Intent"],
                                              entries([input]), a["Metadata"], lit("Echo"), doc("Processing documentation"),
                                              "#echo"),
                          **a, Outputs=entries([output]), BaseCommand=[lit("echo"), lit("hello")], Arguments=yaml,
                          Stdin=lit("input.txt"), Stderr=lit("error.log"), Stdout=lit("output.txt"),
                          SuccessCodes=entries([0]), TemporaryFailCodes=entries([75]),
                          PermanentFailCodes=entries([1, 2]), Inputs=entries([input]))

    def test_workflow(self, values, annotations, absent_annotations):
        expect_properties(C.CWLWorkflowDescription([], [], []), **absent_annotations, Steps=[], Inputs=[], Outputs=[])
        step, input, output = (values[k] for k in ("step", "input", "output"))
        a = annotations
        expect_properties(C.CWLWorkflowDescription(entries([step]), entries([input]), entries([output]), "v1.1",
                                                   a["Requirements"], a["Hints"], a["Intent"], a["Metadata"], lit("Echo"),
                                                   doc("Processing documentation"), "#echo"),
                          **a, Steps=entries([step]), Inputs=entries([input]), Outputs=entries([output]))

    def test_expression_tool(self, values, annotations, absent_annotations):
        expect_properties(C.CWLExpressionToolDescription([], lit("$(inputs.message)")), **absent_annotations,
                          Outputs=[], Expression=lit("$(inputs.message)"), Inputs=None)
        input, output, a = values["input"], values["output"], annotations
        expect_properties(C.CWLExpressionToolDescription(entries([output]), lit("$(inputs.message)"), "v1.1",
                                                         a["Requirements"], a["Hints"], a["Intent"], entries([input]),
                                                         a["Metadata"], lit("Echo"), doc("Processing documentation"),
                                                         "#echo"),
                          **a, Outputs=entries([output]), Expression=lit("$(inputs.message)"), Inputs=entries([input]))

    def test_operation(self, values, annotations, absent_annotations):
        expect_properties(C.CWLOperationDescription([], []), **absent_annotations, Inputs=[], Outputs=[])
        input, output, a = values["input"], values["output"], annotations
        expect_properties(C.CWLOperationDescription(entries([input]), entries([output]), "v1.1", a["Requirements"],
                                                   a["Hints"], a["Intent"], a["Metadata"], lit("Echo"),
                                                   doc("Processing documentation"), "#echo"),
                          **a, Inputs=entries([input]), Outputs=entries([output]))


class TestUnionFactoryHelpers:
    @pytest.mark.parametrize("tag,name,factory,value", [
        (0, "Literal", "literal", "literal text"),
        (1, "Include", "include", "contents.txt"),
        (2, "Import", "import_", "tool.cwl"),
        (3, "Mixin", "mixin", "fields.yml"),
    ])
    def test_schema_salad_string(self, tag, name, factory, value):
        expect_union(getattr(C.SchemaSaladString, factory)(value), tag, name, [value])

    def test_schema_salad_string_mixed(self, values):
        yaml = values["yaml_value"]
        expect_union(C.SchemaSaladString.mixed(yaml), 4, "Mixed", [yaml])

    @pytest.mark.parametrize("tag,name,factory", [
        (0, "File", "file"), (1, "Directory", "directory"),
        (2, "String", "string"), (3, "Int", "int"), (4, "Long", "long"),
        (5, "Float", "float"), (6, "Double", "double"), (7, "Boolean", "boolean"),
        (8, "Any", "any"), (9, "Stdout", "stdout"), (10, "Stderr", "stderr"), (11, "Stdin", "stdin"),
        (17, "Null", "null"),
    ])
    def test_cwl_primitive_type(self, tag, name, factory):
        expect_union(getattr(C.CWLType, factory)(), tag, name, [])

    @pytest.mark.parametrize("tag,name,factory,key", [
        (12, "Reference", "reference", "reference"), (13, "ImportedType", "imported_type", "imported_path"),
        (14, "IncludedType", "included_type", "included_path"), (15, "MixedType", "mixed_type", "yaml_value"),
        (16, "RawType", "raw_type", "yaml_value"), (18, "Array", "array", "array_schema"),
        (19, "Record", "record", "record_schema"), (20, "Enum", "enum", "enum_schema"),
        (21, "Union", "union", "union_types"),
    ])
    def test_cwl_nested_type(self, values, tag, name, factory, key):
        nested = dict(values, array_schema=C.InputArraySchema(values["string_type"]),
                      record_schema=C.InputRecordSchema(entries([C.InputRecordField("message", values["string_type"])])),
                      enum_schema=C.InputEnumSchema([lit("red"), lit("blue")]),
                      union_types=[values["string_type"], C.CWLType.null()],
                      reference="#Message", imported_path="types.yml", included_path="type.txt")[key]
        expect_union(getattr(C.CWLType, factory)(nested), tag, name, [nested])

    def test_file_and_directory_take_no_instance(self):
        expect_union(C.CWLType.file(), 0, "File", [])
        expect_union(C.CWLType.directory(), 1, "Directory", [])

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
        (0, "Single", "single", lit("echo/result")),
        (1, "Multiple", "multiple", [lit("echo/result"), lit("other/result")]),
    ])
    def test_output_source(self, tag, name, factory, value):
        expect_union(getattr(C.OutputSource, factory)(value), tag, name, [value])

    @pytest.mark.parametrize("tag,name,factory", [
        (0, "NoListing", "no_listing"), (1, "ShallowListing", "shallow_listing"),
        (2, "DeepListing", "deep_listing"),
    ])
    def test_load_listing_enum(self, tag, name, factory):
        expect_union(getattr(C.LoadListingEnum, factory)(), tag, name, [])

    @pytest.mark.parametrize("tag,name,factory,key", [
        (3, "ImportedLoadListing", "imported_load_listing", "imported_path"),
        (4, "IncludedLoadListing", "included_load_listing", "included_path"),
        (5, "MixedLoadListing", "mixed_load_listing", "yaml_value"),
    ])
    def test_load_listing_directive(self, values, tag, name, factory, key):
        value = dict(values, imported_path="listing.yml", included_path="listing.txt")[key]
        expect_union(getattr(C.LoadListingEnum, factory)(value), tag, name, [value])

    def test_tool_time_limit_value(self):
        expect_union(C.ToolTimeLimitValue.tool_time_limit_seconds(120), 0, "ToolTimeLimitSeconds", [120])
        expression = lit("$(inputs.timeout)")
        expect_union(C.ToolTimeLimitValue.tool_time_limit_expression(expression), 1, "ToolTimeLimitExpression", [expression])

    @pytest.mark.parametrize("tag,name,factory,key", [
        (0, "DirentEntry", "dirent_entry", "dirent"), (1, "StringEntry", "string_entry", "include"),
        (2, "FileEntry", "file_entry", "file"), (3, "DirectoryEntry", "directory_entry", "directory"),
        (5, "FilesEntry", "files_entry", "files"), (6, "RawEntry", "raw_entry", "yaml_value"),
    ])
    def test_initial_work_dir_entry(self, values, tag, name, factory, key):
        value = dict(values, files=[])[key]
        expect_union(getattr(C.InitialWorkDirEntry, factory)(value), tag, name, [value])

    def test_initial_work_dir_null_entry(self):
        expect_union(C.InitialWorkDirEntry.null_entry(), 4, "NullEntry", [])

    @pytest.mark.parametrize("tag,name,factory,key", [
        (0, "InlineJavascriptRequirement", "inline_javascript_requirement", "inline_javascript"),
        (1, "SchemaDefRequirement", "schema_def_requirement", "schema_defs"),
        (2, "DockerRequirement", "docker_requirement", "docker"),
        (3, "SoftwareRequirement", "software_requirement", "software"),
        (4, "LoadListingRequirement", "load_listing_requirement", "load_listing"),
        (5, "InitialWorkDirRequirement", "initial_work_dir_requirement", "work_dir"),
        (6, "InitialWorkDirExpressionRequirement", "initial_work_dir_expression_requirement", "work_dir_expression"),
        (7, "EnvVarRequirement", "env_var_requirement", "environment"),
        (8, "ShellCommandRequirement", "shell_command_requirement", None),
        (9, "ResourceRequirement", "resource_requirement", "resources"),
        (10, "WorkReuseRequirement", "work_reuse_requirement", "work_reuse"),
        (11, "WorkReuseExpressionRequirement", "work_reuse_expression_requirement", "work_reuse_expression"),
        (12, "NetworkAccessRequirement", "network_access_requirement", "network"),
        (13, "NetworkAccessExpressionRequirement", "network_access_expression_requirement", "network_expression"),
        (14, "InplaceUpdateRequirement", "inplace_update_requirement", "inplace_update"),
        (15, "ToolTimeLimitRequirement", "tool_time_limit_requirement", "time_limit"),
        (16, "SubworkflowFeatureRequirement", "subworkflow_feature_requirement", None),
        (17, "ScatterFeatureRequirement", "scatter_feature_requirement", None),
        (18, "MultipleInputFeatureRequirement", "multiple_input_feature_requirement", None),
        (19, "StepInputExpressionRequirement", "step_input_expression_requirement", None),
        (20, "ImportedRequirement", "imported_requirement", "imported_path"),
        (21, "MixedRequirement", "mixed_requirement", "yaml_value"),
        (22, "RawRequirement", "raw_requirement", "yaml_value"),
    ])
    def test_requirement(self, values, tag, name, factory, key):
        schema = C.InputArraySchema(values["string_type"], None, None, "Message")
        payloads = dict(
            inline_javascript=C.InlineJavascriptRequirementValue([lit("function echo(x) { return x; }")]),
            schema_defs=[SchemaDefEntry_Defined(SchemaDefinition_Array(schema))],
            docker=C.DockerRequirement(lit("alpine:3")), software=entries([C.SoftwarePackage(lit("echo"), [lit("1.0")])]),
            load_listing=C.LoadListingRequirementValue(values["listing"]),
            work_dir=[C.InitialWorkDirEntry.dirent_entry(values["dirent"])],
            environment=entries([C.EnvironmentDef(lit("MESSAGE"), lit("hello"))]), resources=C.ResourceRequirementInstance(1),
            work_reuse=C.WorkReuseRequirementValue(False), network=C.NetworkAccessRequirementValue(False),
            inplace_update=C.InplaceUpdateRequirementValue(boolean(True)),
            time_limit=C.ToolTimeLimitValue.tool_time_limit_seconds(60),
            work_dir_expression=lit("$(inputs.listing)"),
            work_reuse_expression=lit("$(inputs.reuse)"), network_expression=lit("$(inputs.network)"),
            imported_path="requirements.yml", yaml_value=values["yaml_value"],
        )
        fields = [] if key is None else [payloads[key]]
        expect_union(getattr(C.Requirement, factory)(*fields), tag, name, fields)

    def test_hint_entry(self, values):
        requirement = values["requirement"]
        unknown = C.HintUnknownValue("example:Extension", values["yaml_value"])
        expect_union(C.HintEntry.known_hint(requirement), 0, "KnownHint", [requirement])
        expect_union(C.HintEntry.unknown_hint(unknown), 1, "UnknownHint", [unknown])
        yaml = values["yaml_value"]
        expect_union(C.HintEntry.imported_hint("hints.yml"), 2, "ImportedHint", ["hints.yml"])
        expect_union(C.HintEntry.mixed_hint(yaml), 3, "MixedHint", [yaml])
        expect_union(C.HintEntry.raw_hint(yaml), 4, "RawHint", [yaml])

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

    @pytest.mark.parametrize("union,tag,name,factory,key", [
        ("LinkMergeMethod", 2, "ImportedLinkMerge", "imported_link_merge", "imported_path"),
        ("LinkMergeMethod", 3, "IncludedLinkMerge", "included_link_merge", "included_path"),
        ("LinkMergeMethod", 4, "MixedLinkMerge", "mixed_link_merge", "yaml_value"),
        ("PickValueMethod", 3, "ImportedPickValue", "imported_pick_value", "imported_path"),
        ("PickValueMethod", 4, "IncludedPickValue", "included_pick_value", "included_path"),
        ("PickValueMethod", 5, "MixedPickValue", "mixed_pick_value", "yaml_value"),
        ("ScatterMethod", 3, "ImportedScatterMethod", "imported_scatter_method", "imported_path"),
        ("ScatterMethod", 4, "IncludedScatterMethod", "included_scatter_method", "included_path"),
        ("ScatterMethod", 5, "MixedScatterMethod", "mixed_scatter_method", "yaml_value"),
    ])
    def test_workflow_method_directives(self, values, union, tag, name, factory, key):
        value = dict(values, imported_path="method.yml", included_path="method.txt")[key]
        expect_union(getattr(getattr(C, union), factory)(value), tag, name, [value])

    def test_step_output(self, values):
        parameter = C.StepOutputParameter("result")
        expect_union(C.StepOutput.step_output_string("result"), 0, "StepOutputString", ["result"])
        expect_union(C.StepOutput.step_output_record(parameter), 1, "StepOutputRecord", [parameter])
        yaml = values["yaml_value"]
        expect_union(C.StepOutput.step_output_include("output.txt"), 2, "StepOutputInclude", ["output.txt"])
        expect_union(C.StepOutput.step_output_import("outputs.yml"), 3, "StepOutputImport", ["outputs.yml"])
        expect_union(C.StepOutput.step_output_mixed(yaml), 4, "StepOutputMixed", [yaml])

    @pytest.mark.parametrize("tag,name,factory,key", [
        (0, "RunString", "run_string", "run_path"),
        (1, "RunImport", "run_import", "run_path"),
        (2, "RunMixed", "run_mixed", "yaml_value"),
        (3, "RunRaw", "run_raw", "yaml_value"),
        (4, "RunCommandLineTool", "run_command_line_tool", "tool"),
        (5, "RunWorkflow", "run_workflow", "workflow"),
        (6, "RunExpressionTool", "run_expression_tool", "expression_tool"),
        (7, "RunOperation", "run_operation", "operation"),
    ])
    def test_workflow_step_run(self, values, tag, name, factory, key):
        value = dict(values, run_path="tool.cwl")[key]
        expect_union(getattr(C.WorkflowStepRun, factory)(value), tag, name, [value])

    @pytest.mark.parametrize("tag,name,factory,key", [
        (0, "CommandLineTool", "command_line_tool", "tool"),
        (1, "Workflow", "workflow", "workflow"),
        (2, "ExpressionTool", "expression_tool", "expression_tool"),
        (3, "Operation", "operation", "operation"),
        (4, "RawProcess", "raw_process", "yaml_value"),
    ])
    def test_processing_unit(self, values, tag, name, factory, key):
        value = values[key]
        expect_union(getattr(C.CWLProcessingUnit, factory)(value), tag, name, [value])


class TestNestedYamlValues:
    def test_yaml_content(self):
        from src.ARCtrl.py.fable_modules.yamlicious.model import (
            YamlScalar, YamlComment, NodeProperties, TagName, YamlTag_Named,
            ScalarStyle_Plain, ScalarStyle_Block, BlockScalarStyle_Literal, ChompingMode_Keep,
        )

        # A YAMLicious 2.0 scalar holds the value, the style and the header comment. The tag and the anchor are
        # properties of the node.
        expect_properties(YamlScalar("hello", ScalarStyle_Plain(), None, None),
                          value="hello", style=ScalarStyle_Plain(), header_comment=None, layout=None)
        style = ScalarStyle_Block(BlockScalarStyle_Literal(), ChompingMode_Keep(), 2)
        comment = YamlComment("comment", None)
        expect_properties(YamlScalar("hello\nworld", style, comment, None),
                          value="hello\nworld", style=style, header_comment=comment)
        tag = YamlTag_Named(TagName("tag:yaml.org,2002:", "str"))
        expect_properties(NodeProperties(tag, "message"), tag=tag, anchor="message")

    @pytest.mark.parametrize("union,tag,name", [
        ("BlockScalarStyle", 0, "Literal"), ("BlockScalarStyle", 1, "Folded"),
        ("ChompingMode", 0, "Strip"), ("ChompingMode", 1, "Clip"), ("ChompingMode", 2, "Keep"),
        ("ScalarStyle", 0, "Plain"), ("ScalarStyle", 1, "SingleQuoted"), ("ScalarStyle", 2, "DoubleQuoted"),
    ])
    def test_scalar_union_cases(self, union, tag, name):
        from src.ARCtrl.py.fable_modules.yamlicious import model as Y

        expect_union(getattr(Y, f"{union}_{name}")(), tag, name, [])

    def test_block_scalar_indentation(self):
        from src.ARCtrl.py.fable_modules.yamlicious.model import (
            ScalarStyle_Block, BlockScalarStyle_Folded, ChompingMode_Strip,
        )

        block, chomping = BlockScalarStyle_Folded(), ChompingMode_Strip()
        expect_union(ScalarStyle_Block(block, chomping, None), 3, "Block", [block, chomping, None])
        expect_union(ScalarStyle_Block(block, chomping, 4), 3, "Block", [block, chomping, 4])

    # YAMLicious 2.0 has the node kinds Scalar, Mapping, Sequence and Alias. A comment line is trivia, and the
    # document markers are fields of a document.
    @pytest.mark.parametrize("union,tag,name", [
        ("YamlNodeKind", 0, "Scalar"), ("YamlNodeKind", 1, "Mapping"), ("YamlNodeKind", 2, "Sequence"),
        ("YamlNodeKind", 3, "Alias"), ("Trivia", 0, "FullLineComment"),
    ])
    def test_yaml_element(self, values, union, tag, name):
        from src.ARCtrl.py.fable_modules.yamlicious import model as Y
        from src.ARCtrl.py.fable_modules.yamlicious.node import YamlNodeModule_entry, YamlNodeModule_item
        from fable_library.list import of_array

        node, content = values["yaml_value"].node, values["yaml_content"]
        fields = {
            "Scalar": [content],
            "Mapping": [Y.Collection_1(Y.CollectionStyle_Block(), None,
                                       of_array([YamlNodeModule_entry(YamlNodeModule_plain("message"), node)]),
                                       of_array([]))],
            "Sequence": [Y.Collection_1(Y.CollectionStyle_Block(), None, of_array([YamlNodeModule_item(node)]),
                                        of_array([]))],
            "Alias": ["message"],
            "FullLineComment": [Y.YamlComment("a comment", None)],
        }[name]
        expect_union(getattr(Y, f"{union}_{name}")(*fields), tag, name, fields)
