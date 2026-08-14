from src.ARCtrl import IOType

class TestFactories:

    def test_source(self):
        assert str(IOType.source()) == "Source Name"

    def test_sample(self):
        assert str(IOType.sample()) == "Sample Name"

    def test_of_string(self):
        assert IOType.of_string("Source Name") == IOType.source()
