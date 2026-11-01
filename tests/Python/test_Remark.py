import pytest

from src.ARCtrl import Remark


@pytest.mark.parametrize("line, value", [(0, ""), (12, "A metadata remark")])
def test_primary_constructor(line, value):
    remark = Remark(line, value)
    assert isinstance(remark, Remark)
    assert remark.line == line
    assert remark.value == value
