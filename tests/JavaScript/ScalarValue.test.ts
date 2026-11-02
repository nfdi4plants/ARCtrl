import { describe, expect, it } from 'vitest';
import { Comment, OntologyAnnotation, ScalarValue } from '../../src/ARCtrl/index.ts';

describe('ScalarValue primary constructor', () => {
    it('constructs the Ontology case with its nested annotation', () => {
        const annotation = new OntologyAnnotation('temperature', 'PATO', 'PATO:0000146', [new Comment('source', 'ontology')]);
        const scalar = new ScalarValue(0, [annotation]);

        expect(scalar.tag).toBe(0);
        expect(scalar.cases()[scalar.tag]).toBe('Ontology');
        expect(scalar.fields).toHaveLength(1);
        expect(scalar.fields[0]).toBe(annotation);
        expect(scalar.fields[0].Name).toBe('temperature');
        expect(scalar.fields[0].TermSourceREF).toBe('PATO');
        expect(scalar.fields[0].TermAccessionNumber).toBe('PATO:0000146');
        expect(scalar.fields[0].Comments[0].Name).toBe('source');
        expect(scalar.fields[0].Comments[0].Value).toBe('ontology');
    });

    it('constructs the Int case', () => {
        const scalar = new ScalarValue(1, [-42]);

        expect(scalar.tag).toBe(1);
        expect(scalar.cases()[scalar.tag]).toBe('Int');
        expect(scalar.fields).toStrictEqual([-42]);
    });

    it('constructs the Float case without losing its fractional part', () => {
        const scalar = new ScalarValue(2, [12.75]);

        expect(scalar.tag).toBe(2);
        expect(scalar.cases()[scalar.tag]).toBe('Float');
        expect(scalar.fields).toStrictEqual([12.75]);
    });

    it('constructs the Name case', () => {
        const scalar = new ScalarValue(3, ['instrument A']);

        expect(scalar.tag).toBe(3);
        expect(scalar.cases()[scalar.tag]).toBe('Name');
        expect(scalar.fields).toStrictEqual(['instrument A']);
    });
});
