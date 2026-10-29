import pytest

from src.ARCtrl import Comment, OntologyAnnotation, ScalarValue
from src.ARCtrl.py.Core.scalar_value import (
    ScalarValue_Ontology, ScalarValue_Int, ScalarValue_Float, ScalarValue_Name,
)


def test_ontology_primary_constructor():
    annotation = OntologyAnnotation("temperature", "PATO", "PATO:0000146", [Comment("source", "ontology")])
    scalar = ScalarValue_Ontology(annotation)
    assert isinstance(scalar, ScalarValue)
    assert scalar.tag == 0
    assert scalar.cases()[scalar.tag] == "Ontology"
    assert len(scalar.fields) == 1
    assert scalar.AsOntology() is annotation
    assert scalar.IsAnOntology
    assert (scalar.AsOntology().Name, scalar.AsOntology().TermSourceREF, scalar.AsOntology().TermAccessionNumber) == (
        "temperature", "PATO", "PATO:0000146",
    )
    assert [(c.Name, c.Value) for c in scalar.AsOntology().Comments] == [("source", "ontology")]


@pytest.mark.parametrize("constructor, value, tag, name, accessor", [
    (ScalarValue_Int, -42, 1, "Int", "AsInt"),
    (ScalarValue_Float, 12.75, 2, "Float", "AsFloat"),
    (ScalarValue_Name, "instrument A", 3, "Name", "AsName"),
])
def test_scalar_primary_constructor(constructor, value, tag, name, accessor):
    scalar = constructor(value)
    assert isinstance(scalar, ScalarValue)
    assert scalar.tag == tag
    assert scalar.cases()[tag] == name
    assert list(scalar.fields) == [value]
    assert getattr(scalar, accessor)() == value
