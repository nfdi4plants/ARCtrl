import pytest

from src.ARCtrl import DataFile


@pytest.mark.parametrize("factory, label, predicate", [
    (DataFile.raw_data_file, "Raw Data File", "IsRawData"),
    (DataFile.derived_data_file, "Derived Data File", "IsDerivedData"),
    (DataFile.image_file, "Image File", "IsImage"),
])
def test_primary_constructor_cases(factory, label, predicate):
    data_file = factory()
    assert isinstance(data_file, DataFile)
    assert list(data_file.fields) == []
    assert data_file.AsString == label
    assert getattr(data_file, predicate)
