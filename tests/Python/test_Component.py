from src.ARCtrl import Comment, Component, OntologyAnnotation
from src.ARCtrl.py.Core.scalar_value import ScalarValue_Ontology


class TestPrimaryConstructor:
    def test_optional_record_fields(self):
        component = Component(None, None, None)
        assert component.component_value is None
        assert component.component_unit is None
        assert component.component_type is None

    def test_all_arguments(self):
        annotation = OntologyAnnotation("buffer", "CHEBI", "CHEBI:35225", [Comment("value", "prepared")])
        scalar = ScalarValue_Ontology(annotation)
        unit = OntologyAnnotation("milliliter", "UO", "UO:0000098", [Comment("unit", "volume")])
        component_type = OntologyAnnotation("reagent", "OBI", "OBI:0000047", [Comment("type", "chemical")])
        component = Component(scalar, unit, component_type)
        assert component.component_value is scalar
        assert scalar.AsOntology() is annotation
        assert component.component_unit is unit
        assert component.component_type is component_type
        for actual, name, source, accession, comment_name, comment_value in [
            (component.component_value.AsOntology(), "buffer", "CHEBI", "CHEBI:35225", "value", "prepared"),
            (component.component_unit, "milliliter", "UO", "UO:0000098", "unit", "volume"),
            (component.component_type, "reagent", "OBI", "OBI:0000047", "type", "chemical"),
        ]:
            assert actual.Name == name
            assert actual.TermSourceREF == source
            assert actual.TermAccessionNumber == accession
            assert [(c.Name, c.Value) for c in actual.Comments] == [(comment_name, comment_value)]
