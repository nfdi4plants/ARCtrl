from src.ARCtrl import cwl as CWL
from src.ARCtrl import (
    ArcRun, ArcTable, Comment, DataContext, Datamap, OntologyAnnotation, Person,
)


def test_primary_constructor_minimal():
    run = ArcRun("run-1")
    assert isinstance(run, ArcRun)
    assert run.Identifier == "run-1"
    assert run.Title is None
    assert run.Description is None
    assert run.MeasurementType is None
    assert run.TechnologyType is None
    assert run.TechnologyPlatform is None
    assert run.WorkflowIdentifiers == []
    assert run.Tables == []
    assert run.Datamap is None
    assert run.Performers == []
    assert run.CWLDescription is None
    assert run.CWLInput == []
    assert run.Comments == []
    assert run.Investigation is None


def test_primary_constructor():
    measurement_type = OntologyAnnotation("metabolite profiling")
    technology_type = OntologyAnnotation("mass spectrometry")
    technology_platform = OntologyAnnotation("mass spectrometer")
    table = ArcTable("Measurements")
    datamap = Datamap([DataContext("data-1", "dataset/results.csv")])
    performer = Person("0000-0002-1825-0097", "Frey", "Kevin")
    cwl = CWL.CWLProcessingUnit.command_line_tool(CWL.CWLToolDescription([], "v1.2", ["echo"]))
    input = CWL.CWLParameterReference("message", ["hello"])
    comment = Comment("lastUpdated", "2026-10-29")
    run = ArcRun(
        "run-1",
        "Run title",
        "Run description",
        measurement_type,
        technology_type,
        technology_platform,
        ["workflow-1"],
        [table],
        datamap,
        [performer],
        cwl,
        [input],
        [comment],
    )
    assert isinstance(run, ArcRun)
    assert run.Identifier == "run-1"
    assert run.Title == "Run title"
    assert run.Description == "Run description"
    assert run.MeasurementType is measurement_type
    assert run.TechnologyType is technology_type
    assert run.TechnologyPlatform is technology_platform
    assert run.WorkflowIdentifiers == ["workflow-1"]
    assert run.Tables == [table]
    assert run.Datamap is datamap
    assert run.Performers == [performer]
    assert run.CWLDescription is cwl
    assert run.CWLInput == [input]
    assert run.Comments == [comment]


def test_creates_independent_default_comments():
    first = ArcRun("first")
    second = ArcRun("second")
    first.Comments.append(Comment("key", "value"))
    assert second.Comments == []
