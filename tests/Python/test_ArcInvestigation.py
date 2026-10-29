from src.ARCtrl import (
    ArcAssay, ArcInvestigation, ArcRun, ArcStudy, ArcWorkflow, Comment,
    OntologySourceReference, Person, Publication, Remark,
)


def test_primary_constructor_minimal():
    investigation = ArcInvestigation("investigation-1")
    assert isinstance(investigation, ArcInvestigation)
    assert investigation.Identifier == "investigation-1"
    assert investigation.Title is None
    assert investigation.Description is None
    assert investigation.SubmissionDate is None
    assert investigation.PublicReleaseDate is None
    assert investigation.OntologySourceReferences == []
    assert investigation.Publications == []
    assert investigation.Contacts == []
    assert investigation.Assays == []
    assert investigation.Studies == []
    assert investigation.Workflows == []
    assert investigation.Runs == []
    assert investigation.RegisteredStudyIdentifiers == []
    assert investigation.Comments == []
    assert investigation.Remarks == []


def test_primary_constructor():
    ontology_sources = [
        OntologySourceReference("First ontology"),
        OntologySourceReference("Second ontology"),
    ]
    publication = Publication("12345678", "10.1000/example", "Kevin Frey", "ARC study")
    contact = Person("0000-0002-1825-0097", "Frey", "Kevin")
    assay = ArcAssay("assay-1")
    study = ArcStudy("study-1")
    workflow = ArcWorkflow("workflow-1")
    run = ArcRun("run-1")
    comment = Comment("lastUpdated", "2026-10-29")
    remark = Remark(7, "Investigation remark")
    study.RegisteredAssayIdentifiers.append("assay-1")
    run.WorkflowIdentifiers.append("workflow-1")
    investigation = ArcInvestigation(
        "My cool ARC",
        "My cool ARC title",
        "My cool ARC description",
        "2026-10-20",
        "2026-10-29",
        ontology_sources,
        [publication],
        [contact],
        [assay],
        [study],
        [workflow],
        [run],
        ["study-1"],
        [comment],
        [remark],
    )
    assert isinstance(investigation, ArcInvestigation)
    assert investigation.Identifier == "My cool ARC"
    assert investigation.Title == "My cool ARC title"
    assert investigation.Description == "My cool ARC description"
    assert investigation.SubmissionDate == "2026-10-20"
    assert investigation.PublicReleaseDate == "2026-10-29"
    assert investigation.OntologySourceReferences == ontology_sources
    assert investigation.Publications == [publication]
    assert investigation.Contacts == [contact]
    assert investigation.Assays == [assay]
    assert investigation.Studies == [study]
    assert investigation.Workflows == [workflow]
    assert investigation.Runs == [run]
    assert investigation.RegisteredStudyIdentifiers == ["study-1"]
    assert investigation.Comments == [comment]
    assert investigation.Remarks == [remark]
    assert assay.Investigation is investigation
    assert study.Investigation is investigation
    assert workflow.Investigation is investigation
    assert run.Investigation is investigation
    assert investigation.RegisteredStudies == [study]
    assert study.RegisteredAssays == [assay]
    assert run.Workflows == [workflow]


def test_creates_independent_default_comments():
    first = ArcInvestigation("first")
    second = ArcInvestigation("second")
    first.Comments.append(Comment("key", "value"))
    assert second.Comments == []
