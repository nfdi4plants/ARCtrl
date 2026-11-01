import { equal, deepEqual, notEqual } from 'assert';
import { CompositeCell, OntologyAnnotation } from "../../src/ARCtrl/index.ts"
import { expect, it, describe } from 'vitest';
import { Comment, Data, DataFile } from '../../src/ARCtrl/index.ts';

describe('CompositeCell', function () {
    it('Primary Constructor', function() {
        let oa = new OntologyAnnotation("My OA Name", "NCIT", "http://purl.obolibrary.org/obo/NCIT_C12345")
        let t1 = CompositeCell.term(oa)
        let t2 = new CompositeCell(0, [oa])
        expect(t1).toStrictEqual(t2);
    });
    it('FreeText Helper Member', function () {
      let ft = "My FreeTextValue"
      let ft1 = CompositeCell.freeText(ft)
      let ft2 = new CompositeCell(1, [ft])
      expect(ft1).toStrictEqual(ft2);
    });
    it('Unitized Helper Members', function () {
        let oa = new OntologyAnnotation("My OA Name", "NCIT", "http://purl.obolibrary.org/obo/NCIT_C12345")
        let v = "5"
        let u1 = CompositeCell.unitized("5", oa)
        let u2 = new CompositeCell(2, [v, oa])
        expect(u1).toStrictEqual(u2);
    });
});

describe('CompositeCell primary constructor cases', () => {
    it('preserves the Term tag and nested ontology argument', () => {
        const annotation = new OntologyAnnotation('temperature', 'PATO', 'PATO:0000146', [new Comment('term', 'curated')]);
        const cell = new CompositeCell(0, [annotation]);

        expect(cell.tag).toBe(0);
        expect(cell.isTerm).toBe(true);
        expect(cell.fields).toHaveLength(1);
        expect(cell.fields[0]).toBe(annotation);
        expect(cell.fields[0].Name).toBe('temperature');
        expect(cell.fields[0].TermSourceREF).toBe('PATO');
        expect(cell.fields[0].TermAccessionNumber).toBe('PATO:0000146');
        expect(cell.fields[0].Comments[0].Name).toBe('term');
        expect(cell.fields[0].Comments[0].Value).toBe('curated');
    });

    it('preserves the FreeText tag and string argument', () => {
        const cell = new CompositeCell(1, ['measurement note']);

        expect(cell.tag).toBe(1);
        expect(cell.isFreeText).toBe(true);
        expect(cell.fields).toStrictEqual(['measurement note']);
    });

    it('preserves both Unitized positional arguments', () => {
        const unit = new OntologyAnnotation('gram', 'UO', 'UO:0000021', [new Comment('unit', 'SI')]);
        const cell = new CompositeCell(2, ['5.25', unit]);

        expect(cell.tag).toBe(2);
        expect(cell.isUnitized).toBe(true);
        expect(cell.fields).toHaveLength(2);
        expect(cell.fields[0]).toBe('5.25');
        expect(cell.fields[1]).toBe(unit);
        expect(cell.fields[1].Name).toBe('gram');
        expect(cell.fields[1].TermSourceREF).toBe('UO');
        expect(cell.fields[1].TermAccessionNumber).toBe('UO:0000021');
        expect(cell.fields[1].Comments[0].Name).toBe('unit');
        expect(cell.fields[1].Comments[0].Value).toBe('SI');
    });

    it('preserves the Data tag and all nested data arguments', () => {
        const data = new Data('cell-data', 'data/raw.csv#row=3', new DataFile(0, []), 'text/csv', 'RFC 7111', [new Comment('source', 'instrument')]);
        const cell = new CompositeCell(3, [data]);

        expect(cell.tag).toBe(3);
        expect(cell.isData).toBe(true);
        expect(cell.fields).toHaveLength(1);
        expect(cell.fields[0]).toBe(data);
        expect(cell.fields[0].ID).toBe('cell-data');
        expect(cell.fields[0].Name).toBe('data/raw.csv#row=3');
        expect(cell.fields[0].FilePath).toBe('data/raw.csv');
        expect(cell.fields[0].Selector).toBe('row=3');
        expect(cell.fields[0].DataType?.tag).toBe(0);
        expect(cell.fields[0].DataType?.fields).toStrictEqual([]);
        expect(cell.fields[0].Format).toBe('text/csv');
        expect(cell.fields[0].SelectorFormat).toBe('RFC 7111');
        expect(cell.fields[0].Comments[0].Name).toBe('source');
        expect(cell.fields[0].Comments[0].Value).toBe('instrument');
    });
});
