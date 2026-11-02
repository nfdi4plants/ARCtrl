import pytest

from src.ARCtrl import IOType
from src.ARCtrl.py.Core.Table.composite_header import (
    IOType_Source, IOType_Sample, IOType_Data, IOType_Material, IOType_FreeText,
)

class TestFactories:

    def test_source(self):
        assert str(IOType.source()) == "Source Name"

    def test_sample(self):
        assert str(IOType.sample()) == "Sample Name"

    def test_of_string(self):
        assert IOType.of_string("Source Name") == IOType.source()


@pytest.mark.parametrize("constructor, factory, tag, name, label", [
    (IOType_Source, IOType.source, 0, "Source", "Source Name"),
    (IOType_Sample, IOType.sample, 1, "Sample", "Sample Name"),
    (IOType_Data, IOType.data, 2, "Data", "Data"),
    (IOType_Material, IOType.material, 3, "Material", "Material"),
])
def test_primary_constructor_cases(constructor, factory, tag, name, label):
    io_type = constructor()
    assert io_type == factory()
    assert io_type.tag == tag
    assert list(io_type.fields) == []
    assert io_type.cases()[tag] == name
    assert io_type.as_input == f"Input [{label}]"
    assert io_type.as_output == f"Output [{label}]"


def test_free_text_primary_constructor():
    io_type = IOType_FreeText("custom material")
    assert io_type == IOType.free_text("custom material")
    assert io_type.tag == 4
    assert list(io_type.fields) == ["custom material"]
    assert io_type.as_input == "Input [custom material]"
    assert io_type.as_output == "Output [custom material]"
