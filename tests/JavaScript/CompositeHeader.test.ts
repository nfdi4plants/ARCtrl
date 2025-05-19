import { equal, deepEqual, notEqual } from 'assert';
import { CompositeHeader, IOType, OntologyAnnotation } from "../../src/ARCtrl/index.ts"
import { expect, it, describe } from 'vitest';

function tests_IOType() {
    describe('IOType', function () {
        it('cases', function () {
            let cases = IOType.Cases
            //console.log(cases)
            expect(cases.length).toStrictEqual(5);
        });
        it('Create non Freetext', function () {
            for (let mycase of IOType.Cases) {
                let tag = mycase[0]
                let iotype = new IOType(tag, [])
                
                switch (tag) {
                    case 0:
                        expect(iotype.asInput).toStrictEqual("Input [Source Name]");
                        break;
                    case 1:
                        expect(iotype.asInput).toStrictEqual("Input [Sample Name]");
                        break;
                    case 2:
                        expect(iotype.asInput).toStrictEqual("Input [Data]");
                        break;
                    case 3:
                        expect(iotype.asInput).toStrictEqual("Input [Material]");
                        break;
                    case 4:
                        expect(iotype.asInput).toStrictEqual("Input [undefined]");
                        break;
                }
            }
        });
        it('Create FreeText', function () {
            let freetext = new IOType(4, ["My FreeTextValue"])
            let asinput = freetext.asInput
            expect(asinput).toStrictEqual("Input [My FreeTextValue]")
        });
        it('Helper Members', function () {
            let so1 = IOType.source()
            let so2 = new IOType(0, [])
            expect(so1).toStrictEqual(so2);

            let sa1 = IOType.sample()
            let sa2 = new IOType(1, [])
            expect(sa1).toStrictEqual(sa2);

            let ra1 = IOType.data()
            let ra2 = new IOType(2, [])
            expect(ra1).toStrictEqual(ra2);

            let ma1 = IOType.material()
            let ma2 = new IOType(3, [])
            expect(ma1).toStrictEqual(ma2);

            let ft = "My FreeTextValue"
            let ft1 = IOType.freeText(ft)
            let ft2 = new IOType(4, [ft])
            expect(ft1).toStrictEqual(ft2);

        });
    });
}

describe('CompositeHeader', function () {
    tests_IOType();
    it("Input", function () {
        let iotype = new IOType(4, ["My FreeTextValue"])
        let header = new CompositeHeader(11, [iotype])
        let actual = header.toString()
        expect(actual).toStrictEqual("Input [My FreeTextValue]")
    });
    it("FreeText", function () {
        let header = new CompositeHeader(13, ["My FreeTextValue"])
        let actual = header.toString()
        expect(actual).toStrictEqual("My FreeTextValue")
    });
    it("Comment", function () {
        let header = new CompositeHeader(14, ["My Comment"])
        let actual = header.toString()
        expect(actual).toStrictEqual("Comment [My Comment]")
    });
    it("Term", function () {
        let oa = new OntologyAnnotation("My OA Name")
        let header = new CompositeHeader(0, [oa])
        let actual = header.toString()
        //console.log(CompositeHeader.Cases)
        expect(actual).toStrictEqual("Component [My OA Name]")
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
                    expect((header1.IsSingleColumn || header1.IsFeaturedColumn)).toStrictEqual(true);
                    break;
                case 1:
                    let header2 = new CompositeHeader(tag, [oa])
                    expect(header2.IsTermColumn).toStrictEqual(true);
                    break;
                case 2:
                    let header3 = new CompositeHeader(tag, [iotype])
                    expect(header3.IsIOType).toStrictEqual(true);
                    break;
                case 3:
                    let header4 = new CompositeHeader(tag, [stringExample])
                    expect(header4.isFreeText || header4.isComment).toStrictEqual(true);
                    break;
            }
        }
    });
    it('Primary Constructor', function () {
      let oa = new OntologyAnnotation("My OA Name", "NCIT", "http://purl.obolibrary.org/obo/NCIT_C12345")
      let c1 = CompositeHeader.component(oa)
      let c2 = new CompositeHeader(0, [oa])
      expect(c1).toStrictEqual(c2);
    });
    it('characteristic - Helper Members', function () {
      let oa = new OntologyAnnotation("My OA Name", "NCIT", "http://purl.obolibrary.org/obo/NCIT_C12345")
      let ch1 = CompositeHeader.characteristic(oa)
      let ch2 = new CompositeHeader(1, [oa])
      expect(ch1).toStrictEqual(ch2);
    });
    it('factor - Helper Members', function () {
      let oa = new OntologyAnnotation("My OA Name", "NCIT", "http://purl.obolibrary.org/obo/NCIT_C12345")
      let f1 = CompositeHeader.factor(oa)
      let f2 = new CompositeHeader(2, [oa])
      expect(f1).toStrictEqual(f2);
    });
    it('parameter - Helper Members', function () {  
      let oa = new OntologyAnnotation("My OA Name", "NCIT", "http://purl.obolibrary.org/obo/NCIT_C12345")
      let p1 = CompositeHeader.parameter(oa)
      let p2 = new CompositeHeader(3, [oa])
      expect(p1).toStrictEqual(p2);
    });
    it('protocolType - Helper Members', function () {  
      let pt1 = CompositeHeader.protocolType()
      let pt2 = new CompositeHeader(4, [])
      expect(pt1).toStrictEqual(pt2);
    }); 
    it('protocolDescription - Helper Members', function () {  
      let pd1 = CompositeHeader.protocolDescription()
      let pd2 = new CompositeHeader(5, [])
      expect(pd1).toStrictEqual(pd2);
    });
    it('protocolUri - Helper Members', function () {  
      let pu1 = CompositeHeader.protocolUri()
      let pu2 = new CompositeHeader(6, [])
      expect(pu1).toStrictEqual(pu2);
    });
    it('protocolVersion - Helper Members', function () {  
      let pv1 = CompositeHeader.protocolVersion()
      let pv2 = new CompositeHeader(7, [])
      expect(pv1).toStrictEqual(pv2);
    });
    it('protocolREF - Helper Members', function () {  
      let pr1 = CompositeHeader.protocolREF()
      let pr2 = new CompositeHeader(8, [])
      expect(pr1).toStrictEqual(pr2);
    });
    it('performer - Helper Members', function () {  
      let pe1 = CompositeHeader.performer()
      let pe2 = new CompositeHeader(9, [])
      expect(pe1).toStrictEqual(pe2);
    });
    it('date - Helper Members', function () {  
      let d1 = CompositeHeader.date()
      let d2 = new CompositeHeader(10, [])
      expect(d1).toStrictEqual(d2);
    });
    it('sample - Helper Members', function () {  
      let iotype = IOType.sample()
      let i1 = CompositeHeader.input(iotype)
      let i2 = new CompositeHeader(11, [iotype])
      expect(i1).toStrictEqual(i2);
    });
    it('output - Helper Members', function () {  
      let iotype = IOType.sample()
      let o1 = CompositeHeader.output(iotype)
      let o2 = new CompositeHeader(12, [iotype])
      expect(o1).toStrictEqual(o2);
    });
    it('freeText - Helper Members', function () {  
        let ft = "My FreeTextValue"
        let ft1 = CompositeHeader.freeText(ft)
        let ft2 = new CompositeHeader(13, [ft])
        expect(ft1).toStrictEqual(ft2);
    });
});