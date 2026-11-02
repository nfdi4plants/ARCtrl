from src.ARCtrl import (
    ArcStudy, ArcTable, Comment, DataContext, Datamap, OntologyAnnotation, Person,
    Publication,
)


def make_default_study():
    publication = Publication("12345678", "10.1000/example", "Kevin Frey", "ARC study")
    contact = Person("0000-0002-1825-0097", "Frey", "Kevin")
    study_design = OntologyAnnotation("metabolite profiling")
    table = ArcTable("Measurements")
    datamap = Datamap([DataContext("data-1", "dataset/results.csv")])
    comment = Comment("lastUpdated", "2026-10-29")
    study = ArcStudy(
        "study-1",
        "Study title",
        "Study description",
        "2026-10-20",
        "2026-10-29",
        [publication],
        [contact],
        [study_design],
        [table],
        datamap,
        ["assay-1"],
        [comment],
    )
    return study


def test_primary_constructor_minimal():
    study = ArcStudy("study-1")
    assert isinstance(study, ArcStudy)
    assert study.Identifier == "study-1"
    assert study.Title is None
    assert study.Description is None
    assert study.SubmissionDate is None
    assert study.PublicReleaseDate is None
    assert study.Publications == []
    assert study.Contacts == []
    assert study.StudyDesignDescriptors == []
    assert study.Tables == []
    assert study.Datamap is None
    assert study.RegisteredAssayIdentifiers == []
    assert study.Comments == []
    assert study.Investigation is None


def test_primary_constructor():
    publication = Publication("12345678", "10.1000/example", "Kevin Frey", "ARC study")
    contact = Person("0000-0002-1825-0097", "Frey", "Kevin")
    study_design = OntologyAnnotation("metabolite profiling")
    table = ArcTable("Measurements")
    datamap = Datamap([DataContext("data-1", "dataset/results.csv")])
    comment = Comment("lastUpdated", "2026-10-29")
    study = ArcStudy(
        "study-1", "Study title", "Study description", "2026-10-20", "2026-10-29",
        [publication], [contact], [study_design], [table], datamap, ["assay-1"], [comment],
    )
    assert isinstance(study, ArcStudy)
    assert study.Identifier == "study-1"
    assert study.Title == "Study title"
    assert study.Description == "Study description"
    assert study.SubmissionDate == "2026-10-20"
    assert study.PublicReleaseDate == "2026-10-29"
    assert study.Publications == [publication]
    assert study.Contacts == [contact]
    assert study.StudyDesignDescriptors == [study_design]
    assert study.Tables == [table]
    assert study.Datamap == datamap
    assert study.RegisteredAssayIdentifiers == ["assay-1"]
    assert study.Comments == [comment]


def test_creates_independent_default_comments():
    first = ArcStudy("first")
    second = ArcStudy("second")
    first.Comments.append(Comment("key", "value"))
    assert second.Comments == []
