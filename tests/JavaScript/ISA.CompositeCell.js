import { equal, deepEqual, notEqual } from 'assert';
import { CompositeCell } from "./ARCtrl/ISA/ISA/ArcTypes/CompositeCell.js"
import { OntologyAnnotation } from './ARCtrl/ISA/ISA/JsonTypes/OntologyAnnotation.js';
import { assertEqual } from './ARCtrl/fable_modules/fable-library.4.5.0/Util.js';




describe('CompositeCell', function () {

    it('Helper Members', function () {
        let oa = OntologyAnnotation.fromString("My OA Name", "NCIT", "http://purl.obolibrary.org/obo/NCIT_C12345")
        let t1 = CompositeCell.term(oa)
        let t2 = new CompositeCell(0, [oa])
        assertEqual(t1, t2);

        let ft = "My FreeTextValue"
        let ft1 = CompositeCell.freeText(ft)
        let ft2 = new CompositeCell(1, [ft])
        assertEqual(ft1, ft2);

        let v = 5
        let u1 = CompositeCell.unitized(5, oa)
        let u2 = new CompositeCell(2, [v, oa])
        assertEqual(u1, u2);
    });
});