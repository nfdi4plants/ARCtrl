import { describe, expect, it } from 'vitest';
import { Comment, Component, OntologyAnnotation, ScalarValue } from '../../src/ARCtrl/index.ts';
import { value } from '@fable-org/fable-library-ts/Option.ts';

describe('Component primary constructor', () => {
    it('accepts undefined for each optional record field', () => {
        const component = new Component(undefined, undefined, undefined);

        expect(component.ComponentValue).toBeUndefined();
        expect(component.ComponentUnit).toBeUndefined();
        expect(component.ComponentType).toBeUndefined();
    });

    it('preserves all positional arguments and nested ontology values', () => {
        const valueAnnotation = new OntologyAnnotation('buffer', 'CHEBI', 'CHEBI:35225', [new Comment('value', 'prepared')]);
        const scalar = new ScalarValue(0, [valueAnnotation]);
        const unit = new OntologyAnnotation('milliliter', 'UO', 'UO:0000098', [new Comment('unit', 'volume')]);
        const type = new OntologyAnnotation('reagent', 'OBI', 'OBI:0000047', [new Comment('type', 'chemical')]);
        const component = new Component(scalar, unit, type);

        expect(component.ComponentValue).toBe(scalar);
        expect(scalar.tag).toBe(0);
        expect(scalar.fields[0]).toBe(valueAnnotation);
        expect(component.ComponentUnit).toBe(unit);
        expect(component.ComponentType).toBe(type);
        for (const [annotation, name, source, accession, commentName, commentValue] of [
            [scalar.fields[0], 'buffer', 'CHEBI', 'CHEBI:35225', 'value', 'prepared'],
            [value(component.ComponentUnit), 'milliliter', 'UO', 'UO:0000098', 'unit', 'volume'],
            [value(component.ComponentType), 'reagent', 'OBI', 'OBI:0000047', 'type', 'chemical'],
        ] as const) {
            expect(annotation?.Name).toBe(name);
            expect(annotation?.TermSourceREF).toBe(source);
            expect(annotation?.TermAccessionNumber).toBe(accession);
            expect(annotation?.Comments[0].Name).toBe(commentName);
            expect(annotation?.Comments[0].Value).toBe(commentValue);
        }
    });
});
