from .ARCtrl.arctrl import OntologyAnnotation, JsonController, ArcInvestigation

class TestOntologyAnnotation:
      
    def test_create(self):
        oa = OntologyAnnotation("instrument model", "MS", "MS:1234567")
        assert oa.NameText == "instrument model"
        
class TestInvestigation:
    
    def test_json(self):
        i = ArcInvestigation.init("My Investigation")
        actual = JsonController.Investigation().to_json_string()(i)
        expected = """{"Identifier":"My Investigation"}"""
        assert actual == expected