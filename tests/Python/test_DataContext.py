from src.ARCtrl import Comment, DataContext, DataFile, OntologyAnnotation


def test_primary_constructor_minimal():
    context = DataContext()

    for field in (
        "ID", "Name", "FilePath", "Selector", "DataType", "Format",
        "SelectorFormat", "Explication", "Unit", "ObjectType", "Label",
        "Description", "GeneratedBy",
    ):
        assert getattr(context, field) is None, field
    assert context.Comments == []


def test_primary_constructor_preserves_inherited_and_context_fields():
    explication = OntologyAnnotation("mass", "MS", "MS:1000001", [Comment("explication", "measured")])
    unit = OntologyAnnotation("gram", "UO", "UO:0000021", [Comment("unit", "SI")])
    object_type = OntologyAnnotation("sample", "OBI", "OBI:0000747", [Comment("object", "biological")])
    data_type = DataFile.raw_data_file()
    comments = [Comment("review", "approved")]
    context = DataContext(
        "context-id", "data/mass.csv#row=2", data_type, "text/csv", "RFC 7111",
        explication, unit, object_type, "Sample mass", "Measured sample mass",
        "run-1", comments,
    )

    assert context.ID == "context-id"
    assert context.Name == "data/mass.csv#row=2"
    assert context.FilePath == "data/mass.csv"
    assert context.Selector == "row=2"
    assert context.DataType is data_type
    assert context.DataType.tag == 0
    assert list(context.DataType.fields) == []
    assert context.Format == "text/csv"
    assert context.SelectorFormat == "RFC 7111"
    assert context.Explication is explication
    assert context.Unit is unit
    assert context.ObjectType is object_type
    for annotation, name, source, accession, comment_name, comment_value in (
        (context.Explication, "mass", "MS", "MS:1000001", "explication", "measured"),
        (context.Unit, "gram", "UO", "UO:0000021", "unit", "SI"),
        (context.ObjectType, "sample", "OBI", "OBI:0000747", "object", "biological"),
    ):
        assert annotation.Name == name
        assert annotation.TermSourceREF == source
        assert annotation.TermAccessionNumber == accession
        assert annotation.Comments[0].Name == comment_name
        assert annotation.Comments[0].Value == comment_value
    assert context.Label == "Sample mass"
    assert context.Description == "Measured sample mass"
    assert context.GeneratedBy == "run-1"
    assert context.Comments is comments
    assert context.Comments[0].Name == "review"
    assert context.Comments[0].Value == "approved"
