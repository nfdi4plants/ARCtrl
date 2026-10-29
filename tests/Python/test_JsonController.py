import json
from dataclasses import fields as dataclass_fields, is_dataclass
from datetime import datetime, timezone
from uuid import UUID

import pytest

from src.ARCtrl import (
    ARC, ArcAssay, ArcInvestigation, ArcRun, ArcStudy, ArcTable, ArcWorkflow,
    Comment, Component, CompositeCell, CompositeHeader, Data, DataContext,
    DataFile, Datamap, IOType, JsonController,
    OntologyAnnotation, OntologySourceReference, Organisation, Person,
    Publication, ScalarValue, Template,
)
from src.ARCtrl.rocrate import LDGraph, LDNode


def mk_data_context():
    return DataContext(
        "context-1", "dataset/results.csv#row=2", DataFile.derived_data_file(),
        "text/csv", "RFC 7111", OntologyAnnotation("mass", "MS", "MS:1000001"),
        OntologyAnnotation("gram", "UO", "UO:0000021"),
        OntologyAnnotation("sample", "OBI", "OBI:0000747"), "Sample mass",
        "Measured sample mass", "run-1", [Comment("quality", "checked")],
    )


def mk_person():
    return Person(
        "0000-0002-1825-0097", "Frey", "Kevin", "K. F.", "kevin@example.org",
        "+49 123 456", "+49 123 457", "Example Street 1", "Example University",
        [OntologyAnnotation("investigator", "OBI", "OBI:0000202")],
        [Comment("department", "biology")],
    )


def mk_default_table():
    annotation = OntologyAnnotation("organism", "OBI", "OBI:0100026", [Comment("term", "curated")])
    unit = OntologyAnnotation("gram", "UO", "UO:0000021")
    data = Data("table-data", "data/results.csv#row=4", DataFile.derived_data_file(),
                "text/csv", "RFC 7111", [Comment("quality", "checked")])
    return ArcTable("Filled Measurements", [
        CompositeHeader.characteristic(annotation),
        CompositeHeader.parameter(OntologyAnnotation("mass")),
        CompositeHeader.input(IOType.data()), CompositeHeader.free_text("Notes"),
    ], [
        [CompositeCell.term(annotation), CompositeCell.term(OntologyAnnotation("mouse"))],
        [CompositeCell.unitized("5", unit), CompositeCell.unitized("7", unit)],
        [CompositeCell.data(data), CompositeCell.data(Data(None, "data/second.csv"))],
        [CompositeCell.free_text("first row"), CompositeCell.free_text("second row")],
    ])


def mk_default_assay():
    return ArcAssay(
        "assay-1", "Assay title", "Assay description",
        OntologyAnnotation("metabolite profiling"), OntologyAnnotation("mass spectrometry"),
        OntologyAnnotation("mass spectrometer"), [ArcTable.init("Measurements")],
        Datamap([DataContext("data-1", "dataset/results.csv")]),
        [Person("0000-0002-1825-0097", "Frey", "Kevin")],
        [Comment("lastUpdated", "2026-10-29")],
    )


def mk_default_study():
    return ArcStudy(
        "study-1", "Study title", "Study description", "2026-10-20", "2026-10-29",
        [Publication("12345678", "10.1000/example", "Kevin Frey", "ARC study")],
        [Person("0000-0002-1825-0097", "Frey", "Kevin")],
        [OntologyAnnotation("metabolite profiling")], [ArcTable.init("Measurements")],
        Datamap([DataContext("data-1", "dataset/results.csv")]), ["assay-1"],
        [Comment("lastUpdated", "2026-10-29")],
    )


def mk_workflow():
    return ArcWorkflow(
        "workflow-1", "Workflow title", "Workflow description", OntologyAnnotation("data analysis"),
        "workflows/workflow-1/workflow.cwl", "1.2.3", ["subworkflow-1"],
        [OntologyAnnotation("temperature")],
        [Component(ScalarValue.from_string("instrument"), None, OntologyAnnotation("equipment"))],
        Datamap([mk_data_context()]), [mk_person()], None, [Comment("version", "reviewed")],
    )


def mk_run():
    return ArcRun(
        "run-1", "Run title", "Run description", OntologyAnnotation("metabolite profiling"),
        OntologyAnnotation("mass spectrometry"), OntologyAnnotation("mass spectrometer"),
        ["workflow-1"], [mk_default_table()], Datamap([mk_data_context()]), [mk_person()],
        None, None, [Comment("quality", "checked")],
    )


def mk_investigation():
    assay, study = mk_default_assay(), mk_default_study()
    assay.AddTable(mk_default_table())
    study.AddTable(mk_default_table())
    return ArcInvestigation(
        "investigation-1", "Investigation title", "Investigation description", "2026-10-20", "2026-10-29",
        [OntologySourceReference("Ontology description", "ontology.obo", "OBI", "1.0")],
        [Publication("12345678", "10.1000/example", "Kevin Frey", "ARC study")],
        [mk_person()], [assay], [study], None, None, [study.Identifier], [Comment("quality", "checked")],
    )


def assert_value_equal(actual, expected):
    """Compare nested values, including classes whose Python == compares identity."""
    assert type(actual) is type(expected)
    if isinstance(expected, (list, tuple)):
        assert len(actual) == len(expected)
        for item, expected_item in zip(actual, expected):
            assert_value_equal(item, expected_item)
        return
    if is_dataclass(expected):
        for field in dataclass_fields(expected):
            assert_value_equal(getattr(actual, field.name), getattr(expected, field.name))
        return
    fields_by_type = {
        Comment: "Name Value",
        OntologyAnnotation: "Name TermSourceREF TermAccessionNumber Comments",
        OntologySourceReference: "Description File Name Version Comments",
        Person: "ORCID LastName FirstName MidInitials EMail Phone Fax Address Affiliation Roles Comments",
        Publication: "PubMedID DOI Authors Title Status Comments",
        Data: "ID Name FilePath Selector DataType Format SelectorFormat Comments",
        DataContext: "ID Name FilePath Selector DataType Format SelectorFormat Comments Explication Unit ObjectType Label Description GeneratedBy",
        Datamap: "DataContexts",
    }
    fields = fields_by_type.get(type(expected))
    if fields:
        assert_fields_equal(actual, expected, fields)
    else:
        assert actual == expected


def assert_fields_equal(actual, expected, fields):
    for field in fields.split():
        assert_value_equal(getattr(actual, field), getattr(expected, field))


def assert_tables_equal(actual, expected):
    assert len(actual) == len(expected)
    for table2, table in zip(actual, expected):
        assert isinstance(table2, ArcTable)
        assert_fields_equal(table2, table, "Name Headers RowCount ColumnCount Columns")


def roundtrip(controller, value, compressed=False):
    if compressed:
        return controller.from_compressed_json_string(controller.to_compressed_json_string(value))
    return controller.from_json_string(controller.to_json_string(value))


class TestInvestigation:
    def test_json(self):
        investigation = ArcInvestigation.init("My Investigation")
        assert JsonController.Investigation.to_json_string(investigation) == '{"Identifier":"My Investigation"}'


@pytest.mark.parametrize("controller,factory,expected_type", [
    (JsonController.OntologyAnnotation, lambda: OntologyAnnotation("instrument model", "MS", "MS:08239081"), OntologyAnnotation),
    (JsonController.Person, mk_person, Person),
    (JsonController.Comment, lambda: Comment("quality", "checked"), Comment),
    (JsonController.OntologySourceReference, lambda: OntologySourceReference("Ontology description", "ontology.obo", "OBI", "1.0", [Comment("source", "curated")]), OntologySourceReference),
    (JsonController.Publication, lambda: Publication("12345678", "10.1000/example", "Kevin Frey", "ARC study", OntologyAnnotation("published", "OBI", "OBI:0000605"), [Comment("year", "2026")]), Publication),
    (JsonController.Data, lambda: Data("data-1", "dataset/results.csv#row=2", DataFile.derived_data_file(), "text/csv", "RFC 7111", [Comment("quality", "checked")]), Data),
    (JsonController.DataContext, mk_data_context, DataContext),
    (JsonController.DataContext, lambda: DataContext(), DataContext),
])
def test_json_value_roundtrip(controller, factory, expected_type):
    value = factory()
    actual = roundtrip(controller, value)
    assert isinstance(actual, expected_type)
    assert_value_equal(actual, value)


@pytest.mark.parametrize("populated", [False, True])
def test_datamap(populated):
    datamap = Datamap([mk_data_context(), DataContext("context-2", "dataset/image.png")] if populated else [])
    actual = roundtrip(JsonController.Datamap, datamap)
    assert isinstance(actual, Datamap)
    assert_value_equal(actual.DataContexts, datamap.DataContexts)
    if populated:
        assert isinstance(actual.DataContexts[0], DataContext)


@pytest.mark.parametrize("compressed", [False, True])
@pytest.mark.parametrize("kind,factory,expected_type,fields", [
    ("Assay", mk_default_assay, ArcAssay, "Identifier Title Description MeasurementType TechnologyType TechnologyPlatform Performers Comments Datamap"),
    ("Study", mk_default_study, ArcStudy, "Identifier Title Description SubmissionDate PublicReleaseDate Publications Contacts StudyDesignDescriptors RegisteredAssayIdentifiers Comments Datamap"),
    ("Workflow", mk_workflow, ArcWorkflow, "Identifier Title Description WorkflowType URI Version SubWorkflowIdentifiers Parameters Components Datamap Contacts Comments"),
    ("Run", mk_run, ArcRun, "Identifier Title Description MeasurementType TechnologyType TechnologyPlatform WorkflowIdentifiers Datamap Performers Comments"),
])
def test_entity_json_roundtrip(compressed, kind, factory, expected_type, fields):
    value = factory()
    if kind in ("Assay", "Study"):
        value.AddTable(mk_default_table())
    actual = roundtrip(getattr(JsonController, kind), value, compressed)
    assert isinstance(actual, expected_type)
    assert_fields_equal(actual, value, fields)
    if kind != "Workflow":
        assert_tables_equal(actual.Tables, value.Tables)
    assert actual.Investigation is None


@pytest.mark.parametrize("compressed", [False, True])
def test_investigation_json_roundtrip(compressed):
    investigation = mk_investigation()
    actual = roundtrip(JsonController.Investigation, investigation, compressed)
    assert isinstance(actual, ArcInvestigation)
    assert_fields_equal(actual, investigation, "Identifier Title Description SubmissionDate PublicReleaseDate OntologySourceReferences Publications Contacts RegisteredStudyIdentifiers Comments")
    assert len(actual.Assays) == len(actual.Studies) == 1
    assert isinstance(actual.Assays[0], ArcAssay)
    assert isinstance(actual.Studies[0], ArcStudy)
    for kind, children, expected_children in (("Assay", actual.Assays, investigation.Assays), ("Study", actual.Studies, investigation.Studies)):
        controller = getattr(JsonController, kind)
        assert json.loads(controller.to_json_string(children[0])) == json.loads(controller.to_json_string(expected_children[0]))
        assert_tables_equal(children[0].Tables, expected_children[0].Tables)
        assert children[0].Investigation is actual
    assert actual.RegisteredStudies[0] is actual.Studies[0]
    assert actual.Studies[0].RegisteredAssays[0] is actual.Assays[0]


@pytest.mark.parametrize("compressed", [False, True])
def test_table_json_roundtrip(compressed):
    table = mk_default_table()
    actual = roundtrip(JsonController.ArcTable, table, compressed)
    assert_tables_equal([actual], [table])


@pytest.mark.parametrize("compressed", [False, True])
def test_template_json_roundtrip(compressed):
    template = Template(
        UUID("12345678-1234-1234-1234-123456789abc"), mk_default_table(), "Measurements", "Measurement template",
        Organisation.of_string("Example organisation"), "1.2.3", [mk_person()],
        [OntologyAnnotation("repository")], [OntologyAnnotation("metabolomics")],
        datetime(2026, 10, 29, 10, tzinfo=timezone.utc),
    )
    actual = roundtrip(JsonController.Template, template, compressed)
    assert isinstance(actual, Template)
    assert_fields_equal(actual, template, "Id Name Description Organisation Version Authors EndpointRepositories Tags")
    assert actual.LastUpdated.timestamp() == template.LastUpdated.timestamp()
    assert_tables_equal([actual.Table], [template.Table])


@pytest.mark.parametrize("format", ["isa", "ro_crate"])
@pytest.mark.parametrize("kind", ["Comment", "OntologySourceReference", "Publication", "Data"])
def test_other_json_formats(format, kind):
    values = {
        "Comment": Comment("quality", "checked"),
        "OntologySourceReference": OntologySourceReference("Ontology description", "ontology.obo", "OBI", "1.0", [Comment("source", "curated")]),
        "Publication": Publication("12345678", "10.1000/example", "Frey, Kevin", "ARC study", OntologyAnnotation("published", "OBI", "OBI:0000605"), [Comment("year", "2026")]),
        "Data": Data("data-1", "dataset/results.csv#row=2", DataFile.derived_data_file(), None if format == "isa" else "text/csv", None if format == "isa" else "RFC 7111", [Comment("quality", "checked")]),
    }
    value = values[kind]
    controller = getattr(JsonController, kind)
    suffix = "isajson_string" if format == "isa" else "rocrate_json_string"
    encode = getattr(controller, "to_" + suffix)
    decode = getattr(controller, "from_" + suffix)
    encoded = encode(value, 2, True) if format == "isa" and kind in ("Publication", "Data") else encode(value, 2)
    assert '\n  "' in encoded
    actual = decode(encoded)
    assert isinstance(actual, type(value))
    assert_value_equal(actual, value)


@pytest.mark.parametrize("data_file", [DataFile.derived_data_file(), DataFile.raw_data_file(), DataFile.image_file()])
def test_data_file(data_file):
    for format in ("isa", "ro_crate"):
        suffix = "isajson_string" if format == "isa" else "rocrate_json_string"
        encoded = getattr(JsonController.DataFile, "to_" + suffix)(data_file)
        assert_value_equal(getattr(JsonController.DataFile, "from_" + suffix)(encoded), data_file)


@pytest.mark.parametrize("io_type", [IOType.source(), IOType.sample(), IOType.data(), IOType.material(), IOType.free_text("Custom input")])
def test_io_type(io_type):
    actual = roundtrip(JsonController.IOType, io_type)
    assert isinstance(actual, IOType)
    assert_value_equal(actual, io_type)


@pytest.mark.parametrize("cell", [
    CompositeCell.term(OntologyAnnotation("organism", "OBI", "OBI:0100026", [Comment("term", "curated")])),
    CompositeCell.free_text('A value with "quotes", a newline\n and ünicode'),
    CompositeCell.unitized("5", OntologyAnnotation("gram", "UO", "UO:0000021")),
    CompositeCell.data(Data("data-1", "dataset/results.csv#row=2", DataFile.derived_data_file())),
])
def test_composite_cell(cell):
    actual = roundtrip(JsonController.CompositeCell, cell)
    assert isinstance(actual, CompositeCell)
    assert_value_equal(actual, cell)


@pytest.mark.parametrize("header", [
    CompositeHeader.parameter(OntologyAnnotation("temperature", "PATO", "PATO:0000146")),
    CompositeHeader.characteristic(OntologyAnnotation("organism")),
    CompositeHeader.factor(OntologyAnnotation("treatment")), CompositeHeader.component(OntologyAnnotation("instrument")),
    CompositeHeader.protocol_type(), CompositeHeader.protocol_ref(), CompositeHeader.protocol_description(),
    CompositeHeader.protocol_uri(), CompositeHeader.protocol_version(), CompositeHeader.performer(), CompositeHeader.date(),
    CompositeHeader.input(IOType.source()), CompositeHeader.output(IOType.data()),
    CompositeHeader.free_text("Notes"), CompositeHeader.comment("quality"),
])
def test_composite_header(header):
    actual = roundtrip(JsonController.CompositeHeader, header)
    assert isinstance(actual, CompositeHeader)
    assert_value_equal(actual, header)


def test_json_indentation():
    comment = Comment("quality", "checked")
    encoded = JsonController.Comment.to_json_string(comment, 4)
    assert '\n    "' in encoded
    assert_value_equal(JsonController.Comment.from_json_string(encoded), comment)


@pytest.mark.parametrize("controller,value", [
    (JsonController.Comment, "invalid JSON"), (JsonController.DataContext, "{}"),
    (JsonController.CompositeCell, '{"celltype":"Unknown","values":[]}'),
    (JsonController.ArcTable, "{}"), (JsonController.Template, "{}"),
])
def test_invalid_json_is_rejected(controller, value):
    with pytest.raises(Exception):
        controller.from_json_string(value)


def test_ld_node_ro_crate():
    node = LDNode("#person-1", ["Person"])
    node.SetProperty("name", "Kevin Frey")
    encoded = JsonController.LDNode.to_rocrate_json_string()(node)
    actual = JsonController.LDNode.from_rocrate_json_string(encoded)
    assert isinstance(actual, LDNode)
    assert json.loads(JsonController.LDNode.to_rocrate_json_string()(actual)) == json.loads(encoded)
    assert json.loads(encoded)["@id"] == "#person-1"
    assert json.loads(encoded)["name"] == "Kevin Frey"


def test_ld_graph_ro_crate():
    node = LDNode("#person-1", ["Person"])
    node.SetProperty("name", "Kevin Frey")
    graph = LDGraph(None, [node])
    encoded = JsonController.LDGraph.to_rocrate_json_string()(graph)
    actual = JsonController.LDGraph.from_rocrate_json_string(encoded)
    assert isinstance(actual, LDGraph)
    assert len(actual.Nodes) == 1
    assert json.loads(JsonController.LDGraph.to_rocrate_json_string()(actual)) == json.loads(encoded)


def test_arc_ro_crate():
    arc = ARC("arc-1", "ARC title", "ARC description")
    encoded = JsonController.ARC.to_rocrate_json_string()(arc)
    actual = JsonController.ARC.from_rocrate_json_string(encoded)
    assert isinstance(actual, ARC)
    assert_fields_equal(actual, arc, "Identifier Title Description")
