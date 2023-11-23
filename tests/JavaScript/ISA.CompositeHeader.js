import { equal, deepEqual, notEqual } from 'assert';
import { CompositeHeader, IOType } from "./ARCtrl/ISA/ISA/ArcTypes/CompositeHeader.js"
import { OntologyAnnotation } from './ARCtrl/ISA/ISA/JsonTypes/OntologyAnnotation.js';
import { assertEqual } from './ARCtrl/fable_modules/fable-library.4.5.0/Util.js';

function tests_IOType() {
    describe('IOType', function () {
        it('cases', function () {
            let cases = IOType.Cases
            //console.log(cases)
            equal(cases.length, 7);
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
                        equal(iotype.asInput, "Input [Raw Data File]");
                        break;
                    case 3:
                        equal(iotype.asInput, "Input [Derived Data File]");
                        break;
                    case 4:
                        equal(iotype.asInput, "Input [Image File]");
                        break;
                    case 5:
                        equal(iotype.asInput, "Input [Material]");
                        break;
                    case 6:
                        equal(iotype.asInput, "Input [undefined]");
                        break;
                }
            }
        });
        it('Create FreeText', function () {
            let freetext = new IOType(6, ["My FreeTextValue"])
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

            let ra1 = IOType.rawDataFile()
            let ra2 = new IOType(2, [])
            assertEqual(ra1, ra2);

            let da1 = IOType.derivedDataFile()
            let da2 = new IOType(3, [])
            assertEqual(da1, da2);

            let im1 = IOType.imageFile()
            let im2 = new IOType(4, [])
            //let imb = equals(im1, im2);
            assertEqual(im1, im2);

            let ma1 = IOType.material()
            let ma2 = new IOType(5, [])
            assertEqual(ma1, ma2);

            let ft = "My FreeTextValue"
            let ft1 = IOType.freeText(ft)
            let ft2 = new IOType(6, [ft])
            assertEqual(ft1, ft2);

        });
    });
}

describe('CompositeHeader', function () {
    tests_IOType();
    it("Input", function () {
        let iotype = new IOType(6, ["My FreeTextValue"])
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
        let oa = OntologyAnnotation.fromString("My OA Name")
        let header = new CompositeHeader(0, [oa])
        let actual = header.toString()
        //console.log(CompositeHeader.Cases)
        equal(actual, "Component [My OA Name]")
    });
    it('jsGetColumnMetaType', function () {
        let cases = CompositeHeader.Cases
        let oa = OntologyAnnotation.fromString("My OA Name")
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
    it('Helper Members', function () {
        let oa = OntologyAnnotation.fromString("My OA Name", "NCIT", "http://purl.obolibrary.org/obo/NCIT_C12345")
        let c1 = CompositeHeader.component(oa)
        let c2 = new CompositeHeader(0, [oa])
        assertEqual(c1, c2);

        let ch1 = CompositeHeader.characteristic(oa)
        let ch2 = new CompositeHeader(1, [oa])
        assertEqual(ch1, ch2);

        let f1 = CompositeHeader.factor(oa)
        let f2 = new CompositeHeader(2, [oa])
        assertEqual(f1, f2);

        let p1 = CompositeHeader.parameter(oa)
        let p2 = new CompositeHeader(3, [oa])
        assertEqual(p1, p2);

        let pt1 = CompositeHeader.protocolType()
        let pt2 = new CompositeHeader(4, [])
        assertEqual(pt1, pt2);

        let pd1 = CompositeHeader.protocolDescription()
        let pd2 = new CompositeHeader(5, [])
        assertEqual(pd1, pd2);

        let pu1 = CompositeHeader.protocolUri()
        let pu2 = new CompositeHeader(6, [])
        assertEqual(pu1, pu2);

        let pv1 = CompositeHeader.protocolVersion()
        let pv2 = new CompositeHeader(7, [])
        assertEqual(pv1, pv2);

        let pr1 = CompositeHeader.protocolREF()
        let pr2 = new CompositeHeader(8, [])
        assertEqual(pr1, pr2);

        let pe1 = CompositeHeader.performer()
        let pe2 = new CompositeHeader(9, [])
        assertEqual(pe1, pe2);

        let d1 = CompositeHeader.date()
        let d2 = new CompositeHeader(10, [])
        assertEqual(d1, d2);

        let iotype = IOType.sample()
        let i1 = CompositeHeader.input(iotype)
        let i2 = new CompositeHeader(11, [iotype])
        assertEqual(i1, i2);

        let o1 = CompositeHeader.output(iotype)
        let o2 = new CompositeHeader(12, [iotype])
        assertEqual(o1, o2);

        let ft = "My FreeTextValue"
        let ft1 = CompositeHeader.freeText(ft)
        let ft2 = new CompositeHeader(13, [ft])
        assertEqual(ft1, ft2);

    });
});