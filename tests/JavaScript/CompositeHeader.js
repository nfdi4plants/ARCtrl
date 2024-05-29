import { equal, deepEqual, notEqual } from 'assert';
import { CompositeHeader, IOType } from "./ARCtrl/index.js"
import { OntologyAnnotation } from './ARCtrl/index.js';
import { assertEqual } from './ARCtrl/fable_modules/fable-library-js.4.16.0/Util.js';

function tests_IOType() {
    describe('IOType', function () {
        it('cases', function () {
            let cases = IOType.Cases
            //console.log(cases)
            equal(cases.length, 5);
        });
        it('Create non Freetext', function () {
            for (let mycase of IOType.Cases) {
                let tag = mycase[0]
                let iotype = new IOType(tag, [])
                switch (tag) {
                    case 0:
                        equal(iotype.asInput, "Input [Source Name]");
                        break;
                    case 1:
                        equal(iotype.asInput, "Input [Sample Name]");
                        break;
                    case 2:
                        equal(iotype.asInput, "Input [Data]");
                        break;
                    case 3:
                        equal(iotype.asInput, "Input [Material]");
                        break;
                    case 4:
                        equal(iotype.asInput, "Input [undefined]");
                        break;
                }
            }
        });
        it('Create FreeText', function () {
            let freetext = new IOType(4, ["My FreeTextValue"])
            let asinput = freetext.asInput
            equal(asinput, "Input [My FreeTextValue]")
        });
        it('Helper Members', function () {
            let so1 = IOType.source()
            let so2 = new IOType(0, [])
            assertEqual(so1, so2);

            let sa1 = IOType.sample()
            let sa2 = new IOType(1, [])
            assertEqual(sa1, sa2);

            let ra1 = IOType.data()
            let ra2 = new IOType(2, [])
            assertEqual(ra1, ra2);

            let ma1 = IOType.material()
            let ma2 = new IOType(3, [])
            assertEqual(ma1, ma2);

            let ft = "My FreeTextValue"
            let ft1 = IOType.freeText(ft)
            let ft2 = new IOType(4, [ft])
            assertEqual(ft1, ft2);

        });
    });
}

describe('CompositeHeader', function () {
    tests_IOType();
    it("Input", function () {
        let iotype = new IOType(4, ["My FreeTextValue"])
        let header = new CompositeHeader(11, [iotype])
        let actual = header.toString()
        equal(actual, "Input [My FreeTextValue]")
    });
    it("FreeText", function () {
        let header = new CompositeHeader(13, ["My FreeTextValue"])
        let actual = header.toString()
        equal(actual, "My FreeTextValue")
    });
    it("Term", function () {
        let oa = new OntologyAnnotation("My OA Name")
        let header = new CompositeHeader(0, [oa])
        let actual = header.toString()
        //console.log(CompositeHeader.Cases)
        equal(actual, "Component [My OA Name]")
    });
    it('jsGetColumnMetaType', function () {
        let cases = CompositeHeader.Cases
        let oa = new OntologyAnnotation("My OA Name")
        let iotype = new IOType(0, [])
        let stringExample = "My Example"
        for (let mycase of cases) {
            let tag = mycase[0]
            let code = CompositeHeader.jsGetColumnMetaType(tag)
            switch (code) {
                case 0:
                    let header1 = new CompositeHeader(tag, [])
                    equal((header1.IsSingleColumn || header1.IsFeaturedColumn), true);
                    break;
                case 1:
                    let header2 = new CompositeHeader(tag, [oa])
                    equal(header2.IsTermColumn, true);
                    break;
                case 2:
                    let header3 = new CompositeHeader(tag, [iotype])
                    equal(header3.IsIOType, true);
                    break;
                case 3:
                    let header4 = new CompositeHeader(tag, [stringExample])
                    equal(header4.isFreeText, true);
                    break;
            }
        }
    });
    it('Primary Constructor', function () {
      let oa = new OntologyAnnotation("My OA Name", "NCIT", "http://purl.obolibrary.org/obo/NCIT_C12345")
      let c1 = CompositeHeader.component(oa)
      let c2 = new CompositeHeader(0, [oa])
      assertEqual(c1, c2);
    });
    it('characteristic - Helper Members', function () {
      let oa = new OntologyAnnotation("My OA Name", "NCIT", "http://purl.obolibrary.org/obo/NCIT_C12345")
      let ch1 = CompositeHeader.characteristic(oa)
      let ch2 = new CompositeHeader(1, [oa])
      assertEqual(ch1, ch2);
    });
    it('factor - Helper Members', function () {
      let oa = new OntologyAnnotation("My OA Name", "NCIT", "http://purl.obolibrary.org/obo/NCIT_C12345")
      let f1 = CompositeHeader.factor(oa)
      let f2 = new CompositeHeader(2, [oa])
      assertEqual(f1, f2);
    });
    it('parameter - Helper Members', function () {  
      let oa = new OntologyAnnotation("My OA Name", "NCIT", "http://purl.obolibrary.org/obo/NCIT_C12345")
      let p1 = CompositeHeader.parameter(oa)
      let p2 = new CompositeHeader(3, [oa])
      assertEqual(p1, p2);
    });
    it('protocolType - Helper Members', function () {  
      let pt1 = CompositeHeader.protocolType()
      let pt2 = new CompositeHeader(4, [])
      assertEqual(pt1, pt2);
    }); 
    it('protocolDescription - Helper Members', function () {  
      let pd1 = CompositeHeader.protocolDescription()
      let pd2 = new CompositeHeader(5, [])
      assertEqual(pd1, pd2);
    });
    it('protocolUri - Helper Members', function () {  
      let pu1 = CompositeHeader.protocolUri()
      let pu2 = new CompositeHeader(6, [])
      assertEqual(pu1, pu2);
    });
    it('protocolVersion - Helper Members', function () {  
      let pv1 = CompositeHeader.protocolVersion()
      let pv2 = new CompositeHeader(7, [])
      assertEqual(pv1, pv2);
    });
    it('protocolREF - Helper Members', function () {  
      let pr1 = CompositeHeader.protocolREF()
      let pr2 = new CompositeHeader(8, [])
      assertEqual(pr1, pr2);
    });
    it('performer - Helper Members', function () {  
      let pe1 = CompositeHeader.performer()
      let pe2 = new CompositeHeader(9, [])
      assertEqual(pe1, pe2);
    });
    it('date - Helper Members', function () {  
      let d1 = CompositeHeader.date()
      let d2 = new CompositeHeader(10, [])
      assertEqual(d1, d2);
    });
    it('sample - Helper Members', function () {  
      let iotype = IOType.sample()
      let i1 = CompositeHeader.input(iotype)
      let i2 = new CompositeHeader(11, [iotype])
      assertEqual(i1, i2);
    });
    it('output - Helper Members', function () {  
      let iotype = IOType.sample()
      let o1 = CompositeHeader.output(iotype)
      let o2 = new CompositeHeader(12, [iotype])
      assertEqual(o1, o2);
    });
    it('freeText - Helper Members', function () {  
        let ft = "My FreeTextValue"
        let ft1 = CompositeHeader.freeText(ft)
        let ft2 = new CompositeHeader(13, [ft])
        assertEqual(ft1, ft2);
    });
});