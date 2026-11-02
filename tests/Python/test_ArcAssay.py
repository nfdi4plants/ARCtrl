from src.ARCtrl import (
    ArcAssay, ArcTable, Comment, DataContext, Datamap, OntologyAnnotation, Person,
)


def make_default_assay():
    measurement_type = OntologyAnnotation("metabolite profiling")
    technology_type = OntologyAnnotation("mass spectrometry")
    technology_platform = OntologyAnnotation("mass spectrometer")
    table = ArcTable("Measurements")
    datamap = Datamap([DataContext("data-1", "dataset/results.csv")])
    performer = Person("0000-0002-1825-0097", "Frey", "Kevin")
    comment = Comment("lastUpdated", "2026-10-29")
    assay = ArcAssay(
        "assay-1",
        "Assay title",
        "Assay description",
        measurement_type,
        technology_type,
        technology_platform,
        [table],
        datamap,
        [performer],
        [comment],
    )
    return assay


def test_primary_constructor_minimal():
    assay = ArcAssay("assay-1")
    assert isinstance(assay, ArcAssay)
    assert assay.Identifier == "assay-1"
    assert assay.Title is None
    assert assay.Description is None
    assert assay.MeasurementType is None
    assert assay.TechnologyType is None
    assert assay.TechnologyPlatform is None
    assert assay.Tables == []
    assert assay.Datamap is None
    assert assay.Performers == []
    assert assay.Comments == []
    assert assay.Investigation is None


def test_primary_constructor():
    measurement_type = OntologyAnnotation("metabolite profiling")
    technology_type = OntologyAnnotation("mass spectrometry")
    technology_platform = OntologyAnnotation("mass spectrometer")
    table = ArcTable("Measurements")
    datamap = Datamap([DataContext("data-1", "dataset/results.csv")])
    performer = Person("0000-0002-1825-0097", "Frey", "Kevin")
    comment = Comment("lastUpdated", "2026-10-29")
    assay = ArcAssay(
        "assay-1", "Assay title", "Assay description", measurement_type,
        technology_type, technology_platform, [table], datamap, [performer], [comment],
    )
    assert isinstance(assay, ArcAssay)
    assert assay.Identifier == "assay-1"
    assert assay.Title == "Assay title"
    assert assay.Description == "Assay description"
    assert assay.MeasurementType == measurement_type
    assert assay.TechnologyType == technology_type
    assert assay.TechnologyPlatform == technology_platform
    assert assay.Tables == [table]
    assert assay.Datamap == datamap
    assert assay.Performers == [performer]
    assert assay.Comments == [comment]


def test_creates_independent_default_comments():
    first = ArcAssay("first")
    second = ArcAssay("second")
    first.Comments.append(Comment("key", "value"))
    assert second.Comments == []
