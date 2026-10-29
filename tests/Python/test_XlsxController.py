import pytest

from src.ARCtrl import (
    ArcAssay, ArcInvestigation, ArcRun, ArcStudy, ArcTable, ArcWorkflow,
    Comment, Component, DataContext, Datamap, OntologyAnnotation,
    OntologySourceReference, Person, Publication, ScalarValue, XlsxController,
)
from .test_JsonController import (
    assert_fields_equal, assert_value_equal, mk_default_assay, mk_default_study, mk_default_table,
)


def roundtrip(controller, value):
    actual = controller.from_fs_workbook(controller.to_fs_workbook(value))
    return actual[0] if isinstance(value, ArcStudy) else actual


class TestInvestigation:
    def test_json(self):
        investigation = ArcInvestigation.init("My Investigation")
        actual = roundtrip(XlsxController.Investigation, investigation)
        assert actual.Identifier == investigation.Identifier


def test_assay():
    assay = mk_default_assay()
    actual = roundtrip(XlsxController.Assay, assay)
    assert isinstance(actual, ArcAssay)
    assert_fields_equal(actual, assay, "Identifier Title Description MeasurementType TechnologyType TechnologyPlatform Performers Comments")
    assert actual.Investigation is assay.Investigation


def test_study():
    study = mk_default_study()
    actual = roundtrip(XlsxController.Study, study)
    assert isinstance(actual, ArcStudy)
    assert_fields_equal(actual, study, "Identifier Title Description SubmissionDate PublicReleaseDate Publications Contacts StudyDesignDescriptors RegisteredAssayIdentifiers Comments")


@pytest.mark.parametrize("kind,factory", [
    ("Assay", mk_default_assay), ("Study", mk_default_study),
    ("Run", lambda: ArcRun("run-1")),
])
def test_empty_table_is_dropped(kind, factory):
    value = factory()
    if kind == "Run":
        value.AddTable(ArcTable.init("Measurements"))
    assert value.TableCount == 1
    assert value.Tables[0].Name == "Measurements"
    actual = roundtrip(getattr(XlsxController, kind), value)
    assert actual.TableCount == 0
    assert actual.Tables == []


@pytest.mark.parametrize("kind,factory", [
    ("Assay", mk_default_assay), ("Study", mk_default_study),
    ("Run", lambda: ArcRun("run-1")),
])
def test_filled_table_is_preserved(kind, factory):
    value = factory()
    if kind == "Run":
        value.AddTable(ArcTable.init("Measurements"))
    table = mk_default_table()
    value.AddTable(table)
    assert value.TableCount == 2
    assert [table.Name for table in value.Tables] == ["Measurements", "Filled Measurements"]
    actual = roundtrip(getattr(XlsxController, kind), value)
    assert actual.TableCount == 1
    result = actual.Tables[0]
    assert isinstance(result, ArcTable)
    assert result.Name == table.Name
    if kind == "Run":
        assert_fields_equal(result, table, "ColumnCount RowCount")
        assert result.GetCellAt(3, 0).AsFreeText == "first row"
        assert result.GetCellAt(3, 1).AsFreeText == "second row"
        parameter_index = next(index for index, header in enumerate(result.Headers) if header.is_parameter)
        unitized = result.GetCellAt(parameter_index, 0)
        assert unitized.is_unitized
        number, unit = unitized.AsUnitized
        assert number == "5"
        assert unit.Name == "gram"
        assert unit.TermSourceREF == "UO"


@pytest.mark.skip(reason="Matches JS TODO: Datamap spreadsheet parsing drops IDs and replaces absent metadata with empty values")
@pytest.mark.parametrize("kind,factory", [("Assay", mk_default_assay), ("Study", mk_default_study)])
def test_datamap_roundtrip_pending_in_javascript(kind, factory):
    value = factory()
    actual = roundtrip(getattr(XlsxController, kind), value)
    assert_value_equal(actual.Datamap, value.Datamap)


def test_workflow():
    workflow = ArcWorkflow(
        "workflow-1", "Workflow title", "Workflow description",
        OntologyAnnotation("data analysis", "OBI", "http://purl.obolibrary.org/obo/OBI_0200000"),
        "workflows/workflow-1/workflow.cwl", "1.2.3", ["subworkflow-1", "subworkflow-2"],
        [OntologyAnnotation("temperature", "PATO", "PATO:0000146"), OntologyAnnotation("duration", "PATO", "PATO:0001309")],
        [Component(ScalarValue.from_string("instrument"), None, OntologyAnnotation("equipment", "OBI", "OBI:0000968"))],
        None, [Person("0000-0002-1825-0097", "Frey", "Kevin")], None, [Comment("lastUpdated", "2026-10-29")],
    )
    actual = roundtrip(XlsxController.Workflow, workflow)
    assert isinstance(actual, ArcWorkflow)
    assert_fields_equal(actual, workflow, "Identifier Title Description WorkflowType URI Version SubWorkflowIdentifiers Parameters Components Contacts Comments")


def test_run():
    run = ArcRun(
        "run-1", "Run title", "Run description",
        OntologyAnnotation("metabolite profiling", "OBI", "http://purl.obolibrary.org/obo/OBI_0000366"),
        OntologyAnnotation("mass spectrometry", "OBI", "http://purl.obolibrary.org/obo/OBI_0000470"),
        OntologyAnnotation("mass spectrometer"), ["workflow-1", "workflow-2"], [], None,
        [Person("0000-0002-1825-0097", "Frey", "Kevin")], None, [], [Comment("lastUpdated", "2026-10-29")],
    )
    actual = roundtrip(XlsxController.Run, run)
    assert isinstance(actual, ArcRun)
    assert_fields_equal(actual, run, "Identifier Title Description MeasurementType TechnologyType TechnologyPlatform WorkflowIdentifiers Performers Comments")
    assert actual.Tables == []


def test_investigation():
    study, assay = mk_default_study(), mk_default_assay()
    investigation = ArcInvestigation(
        "investigation-1", "Investigation title", "Investigation description", "2026-10-20", "2026-10-29",
        [OntologySourceReference("Ontology for Biomedical Investigations", "http://purl.obolibrary.org/obo/obi.owl", "OBI", "2026-10-01")],
        [Publication("12345678", "10.1000/example", "Kevin Frey", "ARC study")],
        [Person("0000-0002-1825-0097", "Frey", "Kevin")], [assay], [study], [], [],
        [study.Identifier], [Comment("lastUpdated", "2026-10-29")],
    )
    actual = roundtrip(XlsxController.Investigation, investigation)
    assert isinstance(actual, ArcInvestigation)
    assert_fields_equal(actual, investigation, "Identifier Title Description SubmissionDate PublicReleaseDate OntologySourceReferences Publications Contacts Comments")
    assert actual.RegisteredStudyIdentifiers == [study.Identifier]
    assert len(actual.Studies) == 1
    assert isinstance(actual.Studies[0], ArcStudy)
    assert_fields_equal(actual.Studies[0], study, "Identifier Title")
    assert actual.Studies[0].RegisteredAssayIdentifiers == [assay.Identifier]
    assert actual.Studies[0].Investigation is actual
    assert len(actual.Assays) == 1
    assert isinstance(actual.Assays[0], ArcAssay)
    assert_fields_equal(actual.Assays[0], assay, "Identifier Title")
    assert actual.Assays[0].Investigation is actual


def test_investigation_no_registered_studies():
    investigation = ArcInvestigation.init("investigation-1")
    actual = roundtrip(XlsxController.Investigation, investigation)
    assert actual.Identifier == investigation.Identifier
    assert actual.RegisteredStudyIdentifiers == []
    assert actual.Studies == []
    assert actual.Assays == []


def test_empty_datamap():
    actual = roundtrip(XlsxController.Datamap, Datamap([]))
    assert isinstance(actual, Datamap)
    assert actual.DataContexts == []


def test_populated_datamap():
    # Spreadsheet columns preserve locations and metadata, but do not contain IDs or DataType.
    contexts = [
        DataContext(
            None, "data/results.csv#row=1", None, "text/csv", "RFC 7111",
            OntologyAnnotation("mass", "MS", "MS:1000001"), OntologyAnnotation("gram", "UO", "UO:0000021"),
            OntologyAnnotation("sample", "OBI", "OBI:0000747"), "Mass", "A measured mass", "run-1", [Comment("quality", "checked")],
        ),
        DataContext(
            None, "data/results.csv#row=2", None, "text/csv", "RFC 7111",
            OntologyAnnotation("mass", "MS", "MS:1000001"), OntologyAnnotation("gram", "UO", "UO:0000021"),
            OntologyAnnotation("sample", "OBI", "OBI:0000747"), "Second mass", "Another measured mass", "run-2", [Comment("quality", "reviewed")],
        ),
    ]
    actual = roundtrip(XlsxController.Datamap, Datamap(contexts))
    assert isinstance(actual, Datamap)
    assert len(actual.DataContexts) == 2
    for context2, context in zip(actual.DataContexts, contexts):
        assert isinstance(context2, DataContext)
        assert_fields_equal(context2, context, "Name FilePath Selector Format SelectorFormat Explication Unit ObjectType Label Description GeneratedBy Comments")
