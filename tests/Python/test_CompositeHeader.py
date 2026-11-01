import pytest

from src.ARCtrl import Comment, CompositeHeader, IOType, OntologyAnnotation
from src.ARCtrl.py.Core.Table.composite_header import (
    CompositeHeader_Component, CompositeHeader_Characteristic, CompositeHeader_Factor,
    CompositeHeader_Parameter, CompositeHeader_ProtocolType, CompositeHeader_ProtocolDescription,
    CompositeHeader_ProtocolUri, CompositeHeader_ProtocolVersion, CompositeHeader_ProtocolREF,
    CompositeHeader_Performer, CompositeHeader_Date, CompositeHeader_Input, CompositeHeader_Output,
    CompositeHeader_FreeText, CompositeHeader_Comment,
)

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


@pytest.mark.parametrize("constructor, factory, tag, name", [
    (CompositeHeader_Component, CompositeHeader.component, 0, "Component"),
    (CompositeHeader_Characteristic, CompositeHeader.characteristic, 1, "Characteristic"),
    (CompositeHeader_Factor, CompositeHeader.factor, 2, "Factor"),
    (CompositeHeader_Parameter, CompositeHeader.parameter, 3, "Parameter"),
])
def test_term_primary_constructors(constructor, factory, tag, name):
    annotation = OntologyAnnotation("temperature", "PATO", "PATO:0000146", [Comment("term", "curated")])
    header = constructor(annotation)
    assert header == factory(annotation)
    assert header.tag == tag
    assert header.cases()[tag] == name
    assert len(header.fields) == 1
    assert header.ToTerm() is annotation
    assert (header.ToTerm().Name, header.ToTerm().TermSourceREF, header.ToTerm().TermAccessionNumber) == (
        "temperature", "PATO", "PATO:0000146",
    )
    assert [(c.Name, c.Value) for c in header.ToTerm().Comments] == [("term", "curated")]
    assert str(header) == f"{name} [temperature]"


@pytest.mark.parametrize("constructor, factory, tag, name, label", [
    (CompositeHeader_ProtocolType, CompositeHeader.protocol_type, 4, "ProtocolType", "Protocol Type"),
    (CompositeHeader_ProtocolDescription, CompositeHeader.protocol_description, 5, "ProtocolDescription", "Protocol Description"),
    (CompositeHeader_ProtocolUri, CompositeHeader.protocol_uri, 6, "ProtocolUri", "Protocol Uri"),
    (CompositeHeader_ProtocolVersion, CompositeHeader.protocol_version, 7, "ProtocolVersion", "Protocol Version"),
    (CompositeHeader_ProtocolREF, CompositeHeader.protocol_ref, 8, "ProtocolREF", "Protocol REF"),
    (CompositeHeader_Performer, CompositeHeader.performer, 9, "Performer", "Performer"),
    (CompositeHeader_Date, CompositeHeader.date, 10, "Date", "Date"),
])
def test_empty_primary_constructors(constructor, factory, tag, name, label):
    header = constructor()
    assert header == factory()
    assert header.tag == tag
    assert list(header.fields) == []
    assert header.cases()[tag] == name
    assert str(header) == label


@pytest.mark.parametrize("constructor, factory, tag, name", [
    (CompositeHeader_Input, CompositeHeader.input, 11, "Input"),
    (CompositeHeader_Output, CompositeHeader.output, 12, "Output"),
])
def test_io_primary_constructors(constructor, factory, tag, name):
    io_type = IOType.free_text("custom material")
    header = constructor(io_type)
    assert header == factory(io_type)
    assert header.tag == tag
    assert len(header.fields) == 1
    assert header.TryIOType() is io_type
    assert io_type.tag == 4
    assert list(io_type.fields) == ["custom material"]
    assert header.cases()[tag] == name
    assert str(header) == f"{name} [custom material]"


@pytest.mark.parametrize("constructor, factory, tag, name, value, label", [
    (CompositeHeader_FreeText, CompositeHeader.free_text, 13, "FreeText", "Extra information", "Extra information"),
    (CompositeHeader_Comment, CompositeHeader.comment, 14, "Comment", "Review", "Comment [Review]"),
])
def test_string_primary_constructors(constructor, factory, tag, name, value, label):
    header = constructor(value)
    assert header == factory(value)
    assert header.tag == tag
    assert list(header.fields) == [value]
    assert header.cases()[tag] == name
    assert str(header) == label
