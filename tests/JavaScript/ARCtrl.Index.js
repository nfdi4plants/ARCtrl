import { equal, deepEqual } from 'assert';
import { OntologyAnnotation, ArcAssay, ArcInvestigation, ArcTable } from './ARCtrl/index.js'

//import file './ARCtrl/index.js' is generated by FAKE never by NPM.
describe('Auto Generated Index', function () {
    it('OntologyAnnotation', function () {
        let actual = OntologyAnnotation.fromString("My OA")
        equal(actual.NameText, "My OA")
    });
    it('ArcAssay', function () {
        let actual = new ArcAssay("My Assay", null, null, null, [ArcTable.init("MyTable")])
        equal(actual.Identifier, "My Assay", "Id")
        equal(actual.TableCount, 1, "TableCount")
    });
    it('ArcInvestigation', function () {
        let actual = new ArcInvestigation("My Investigation", "Super Awesome Experiment")
        equal(actual.Identifier, "My Investigation", "Id")
        equal(actual.Title, "Super Awesome Experiment", "Title")
    });
});