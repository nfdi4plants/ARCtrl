from src.ARCtrl import CompositeHeader, IOType, OntologyAnnotation

class TestFactories:

    def test_input(self):
        header = CompositeHeader.input(IOType.source())
        assert str(header) == "Input [Source Name]"

    def test_output(self):
        header = CompositeHeader.output(IOType.sample())
        assert str(header) == "Output [Sample Name]"

    def test_parameter(self):
        oa = OntologyAnnotation("temperature", "NCIT", "NCIT:C25206")
        header = CompositeHeader.parameter(oa)
        assert header.ToTerm().NameText == "temperature"

    def test_comment(self):
        assert str(CompositeHeader.comment("c")) == "Comment [c]"
