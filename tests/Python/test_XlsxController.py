from .ARCtrl.arctrl import OntologyAnnotation, XlsxController, ArcInvestigation

class TestInvestigation:
    
    def test_json(self):
        i = ArcInvestigation.init("My Investigation")
        fswb = XlsxController.Investigation().to_fs_workbook(i)
        i2 = XlsxController.Investigation().from_fs_workbook(fswb)
        assert i.Identifier == i2.Identifier