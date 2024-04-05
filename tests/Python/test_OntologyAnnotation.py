from .ARCtrl.arctrl import OntologyAnnotation, JsonController, ArcInvestigation

class TestOntologyAnnotation:
      
    def test_create(self):
        oa = OntologyAnnotation("instrument model", "MS", "MS:1234567")
        assert oa.NameText == "instrument model"
        