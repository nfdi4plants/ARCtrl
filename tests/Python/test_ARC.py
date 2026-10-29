from datetime import datetime, timezone

from src.ARCtrl import cwl as CWL
from src.ARCtrl import (
    ARC, ArcAssay, ArcInvestigation, ArcRun, ArcStudy, ArcWorkflow, Comment,
    FileSystem, FileSystemTree, OntologySourceReference, Person, Publication, Remark,
    Commit, License,
)


def test_primary_constructor_minimal():
    arc = ARC("My cool ARC")
    assert isinstance(arc, ARC)
    assert isinstance(arc, ArcInvestigation)
    assert arc.Identifier == "My cool ARC"
    assert arc.Title is None
    assert arc.Description is None
    assert arc.SubmissionDate is None
    assert arc.PublicReleaseDate is None
    assert arc.OntologySourceReferences == []
    assert arc.Publications == []
    assert arc.Contacts == []
    assert arc.Assays == []
    assert arc.Studies == []
    assert arc.Workflows == []
    assert arc.Runs == []
    assert arc.RegisteredStudyIdentifiers == []
    assert arc.Comments == []
    assert arc.Remarks == []
    assert arc.License is None
    assert isinstance(arc.FileSystem, FileSystem)
    assert list(arc.FileSystem.history) == []
    assert "isa.investigation.xlsx" in arc.FileSystem.tree.ToFilePaths()


def test_primary_constructor():
    arc = ARC(
        "My cool ARC",
        "My cool ARC title",
        "My cool ARC description",
        "2026-10-20",
        "2026-10-29",
        [
            OntologySourceReference(
                "My cool description", "my/cool/filepath", "My cool file name", "2.0.0-alpha.01+meta.b6",
            ),
            OntologySourceReference(
                "Another description",
                "another/cool/filepath",
                "Another cool file name",
                "2.0.0-alpha.01+meta.b6",
            ),
        ],
    )
    assert isinstance(arc, ARC)
    assert arc.Identifier == "My cool ARC"
    assert arc.Title == "My cool ARC title"
    assert arc.Description == "My cool ARC description"
    assert arc.SubmissionDate == "2026-10-20"
    assert arc.PublicReleaseDate == "2026-10-29"
    assert len(arc.OntologySourceReferences) == 2
    assert arc.OntologySourceReferences[0].Description == "My cool description"
    assert arc.OntologySourceReferences[1].Description == "Another description"


def test_primary_constructor_with_all_nested_types():
    ontology_source = OntologySourceReference("Ontology description", "ontology.obo", "OBI", "1.0")
    publication = Publication("12345678", "10.1000/example", "Kevin Frey", "ARC study")
    contact = Person("0000-0002-1825-0097", "Frey", "Kevin")
    assay = ArcAssay("assay-1")
    study = ArcStudy("study-1")
    workflow = ArcWorkflow("workflow-1")
    run = ArcRun("run-1")
    cwl = CWL.CWLProcessingUnit.command_line_tool(CWL.CWLToolDescription([]))
    comment = Comment("lastUpdated", "2026-10-29")
    remark = Remark(7, "Investigation remark")
    commit = Commit("abc123", "Kevin", "kevin@example.org", datetime(2026, 10, 29, 10, tzinfo=timezone.utc), "Initial ARC")
    fs = FileSystem(FileSystemTree.create_folder("root", [FileSystemTree.create_file("README.md")]), [commit])
    license = License("fulltext", "Custom license text", "COPYING")
    study.RegisteredAssayIdentifiers.append("assay-1")
    run.WorkflowIdentifiers.append("workflow-1")
    workflow.CWLDescription = cwl
    run.CWLDescription = cwl
    arc = ARC(
        "My cool ARC",
        "My cool ARC title",
        "My cool ARC description",
        "2026-10-20",
        "2026-10-29",
        [ontology_source],
        [publication],
        [contact],
        [assay],
        [study],
        [workflow],
        [run],
        ["study-1"],
        [comment],
        [remark],
        fs,
        license,
    )
    assert isinstance(arc, ARC)
    assert arc.Identifier == "My cool ARC"
    assert arc.Title == "My cool ARC title"
    assert arc.Description == "My cool ARC description"
    assert arc.SubmissionDate == "2026-10-20"
    assert arc.PublicReleaseDate == "2026-10-29"
    assert arc.OntologySourceReferences == [ontology_source]
    assert arc.Publications == [publication]
    assert arc.Contacts == [contact]
    assert arc.Assays == [assay]
    assert arc.Studies == [study]
    assert arc.Workflows == [workflow]
    assert arc.Runs == [run]
    assert arc.RegisteredStudyIdentifiers == ["study-1"]
    assert arc.Comments == [comment]
    assert arc.Remarks == [remark]
    assert arc.License is license
    assert list(arc.FileSystem.history) == [commit]
    assert assay.Investigation is arc
    assert study.Investigation is arc
    assert workflow.Investigation is arc
    assert run.Investigation is arc
    assert arc.RegisteredStudies == [study]
    assert study.RegisteredAssays == [assay]
    assert run.Workflows == [workflow]
    paths = arc.FileSystem.tree.ToFilePaths()
    assert "README.md" in paths
    assert "COPYING" in paths
    assert "isa.investigation.xlsx" in paths
    assert "assays/assay-1/isa.assay.xlsx" in paths
    assert "studies/study-1/isa.study.xlsx" in paths
    assert "workflows/workflow-1/workflow.cwl" in paths
    assert "runs/run-1/run.cwl" in paths


def test_does_not_share_default_collections_or_filesystem_between_instances():
    first = ARC("first")
    second = ARC("second")
    first.Comments.append(Comment("key", "value"))
    first.FileSystem.history.append(Commit("abc123", "Kevin", "kevin@example.org", datetime(2026, 10, 29, 10, tzinfo=timezone.utc), "Initial ARC"))
    assert second.Comments == []
    assert list(second.FileSystem.history) == []
