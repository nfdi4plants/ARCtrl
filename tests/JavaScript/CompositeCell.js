import { equal, deepEqual, notEqual } from 'assert';
import { CompositeCell } from "./ARCtrl/index.js"
import { OntologyAnnotation } from './ARCtrl/index.js';
import { assertEqual } from './ARCtrl/fable_modules/fable-library-js.4.15.0/Util.js';

describe('CompositeCell', function () {
    it('Primary Constructor', function() {
        let oa = new OntologyAnnotation("My OA Name", "NCIT", "http://purl.obolibrary.org/obo/NCIT_C12345")
        let t1 = CompositeCell.term(oa)
        let t2 = new CompositeCell(0, [oa])
        assertEqual(t1, t2);
    });
    it('FreeText Helper Member', function () {
      let ft = "My FreeTextValue"
      let ft1 = CompositeCell.freeText(ft)
      let ft2 = new CompositeCell(1, [ft])
      assertEqual(ft1, ft2);
    });
    it('Unitized Helper Members', function () {
        let oa = new OntologyAnnotation("My OA Name", "NCIT", "http://purl.obolibrary.org/obo/NCIT_C12345") 
        let v = 5
        let u1 = CompositeCell.unitized(5, oa)
        let u2 = new CompositeCell(2, [v, oa])
        assertEqual(u1, u2);
    });
});