from src.ARCtrl import cwl as CWL
from src.ARCtrl import (
    ArcWorkflow, Comment, DataContext, Datamap, OntologyAnnotation, Person,
    Component, ScalarValue,
)


def test_primary_constructor_minimal():
    workflow = ArcWorkflow("workflow-1")
    assert isinstance(workflow, ArcWorkflow)
    assert workflow.Identifier == "workflow-1"
    assert workflow.Title is None
    assert workflow.Description is None
    assert workflow.WorkflowType is None
    assert workflow.URI is None
    assert workflow.Version is None
    assert workflow.SubWorkflowIdentifiers == []
    assert workflow.Parameters == []
    assert workflow.Components == []
    assert workflow.Datamap is None
    assert workflow.Contacts == []
    assert workflow.CWLDescription is None
    assert workflow.Comments == []
    assert workflow.Investigation is None


def test_primary_constructor():
    workflow_type = OntologyAnnotation("data analysis")
    parameter = OntologyAnnotation("temperature")
    component = Component(ScalarValue.from_string("instrument"), None, OntologyAnnotation("equipment"))
    datamap = Datamap([DataContext("data-1", "dataset/results.csv")])
    contact = Person("0000-0002-1825-0097", "Frey", "Kevin")
    cwl = CWL.CWLProcessingUnit.command_line_tool(CWL.CWLToolDescription([], "v1.2", ["echo"]))
    comment = Comment("lastUpdated", "2026-10-29")
    workflow = ArcWorkflow(
        "workflow-1",
        "Workflow title",
        "Workflow description",
        workflow_type,
        "workflows/workflow-1/workflow.cwl",
        "1.2.3",
        ["subworkflow-1"],
        [parameter],
        [component],
        datamap,
        [contact],
        cwl,
        [comment],
    )
    assert isinstance(workflow, ArcWorkflow)
    assert workflow.Identifier == "workflow-1"
    assert workflow.Title == "Workflow title"
    assert workflow.Description == "Workflow description"
    assert workflow.WorkflowType is workflow_type
    assert workflow.URI == "workflows/workflow-1/workflow.cwl"
    assert workflow.Version == "1.2.3"
    assert workflow.SubWorkflowIdentifiers == ["subworkflow-1"]
    assert workflow.Parameters == [parameter]
    assert workflow.Components == [component]
    assert workflow.Datamap is datamap
    assert workflow.Contacts == [contact]
    assert workflow.CWLDescription is cwl
    assert workflow.Comments == [comment]


def test_creates_independent_default_comments():
    first = ArcWorkflow("first")
    second = ArcWorkflow("second")
    first.Comments.append(Comment("key", "value"))
    assert second.Comments == []
