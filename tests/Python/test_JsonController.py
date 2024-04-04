from .ARCtrl.arctrl import OntologyAnnotation, JsonController, ArcInvestigation

class TestInvestigation:
    
    def test_json(self):
        i = ArcInvestigation.init("My Investigation")
        actual = JsonController.Investigation().to_json_string(i)
        expected = """{"Identifier":"My Investigation"}"""
        assert actual == expected