from src.ARCtrl.cwl import CWLType
from src.ARCtrl.py.CWL.cwltypes import CWLType_File, CWLType_String, FileInstance

class TestPublicName:
    """The public name has to be the union class, not the type alias over its cases."""

    def test_isinstance(self):
        assert isinstance(CWLType_File(FileInstance()), CWLType)

    def test_members_are_reachable(self):
        assert CWLType_String().Equals(CWLType_String())
