import { describe, expect, it } from 'vitest';
import { Comment, DataContext, OntologyAnnotation } from '../../src/ARCtrl/index.ts';
import { DataFile } from '../../src/ARCtrl/ts/Core/DataFile.ts';

describe('DataContext primary constructor', () => {
    it('constructs with omitted optional arguments', () => {
        const context = new DataContext();

        expect(context.ID).toBeUndefined();
        expect(context.Name).toBeUndefined();
        expect(context.FilePath).toBeUndefined();
        expect(context.Selector).toBeUndefined();
        expect(context.DataType).toBeUndefined();
        expect(context.Format).toBeUndefined();
        expect(context.SelectorFormat).toBeUndefined();
        expect(context.Explication).toBeUndefined();
        expect(context.Unit).toBeUndefined();
        expect(context.ObjectType).toBeUndefined();
        expect(context.Label).toBeUndefined();
        expect(context.Description).toBeUndefined();
        expect(context.GeneratedBy).toBeUndefined();
        expect(context.Comments).toStrictEqual([]);
    });

    it('preserves all inherited and context-specific positional arguments', () => {
        const explication = new OntologyAnnotation('mass', 'MS', 'MS:1000001', [new Comment('explication', 'measured')]);
        const unit = new OntologyAnnotation('gram', 'UO', 'UO:0000021', [new Comment('unit', 'SI')]);
        const objectType = new OntologyAnnotation('sample', 'OBI', 'OBI:0000747', [new Comment('object', 'biological')]);
        const dataType = new DataFile(0, []);
        const comments = [new Comment('review', 'approved')];
        const context = new DataContext(
            'context-id', 'data/mass.csv#row=2', dataType, 'text/csv', 'RFC 7111',
            explication, unit, objectType, 'Sample mass', 'Measured sample mass', 'run-1', comments,
        );

        expect(context.ID).toBe('context-id');
        expect(context.Name).toBe('data/mass.csv#row=2');
        expect(context.FilePath).toBe('data/mass.csv');
        expect(context.Selector).toBe('row=2');
        expect(context.DataType).toBe(dataType);
        expect(context.DataType?.tag).toBe(0);
        expect(context.DataType?.fields).toStrictEqual([]);
        expect(context.Format).toBe('text/csv');
        expect(context.SelectorFormat).toBe('RFC 7111');
        expect(context.Explication).toBe(explication);
        expect(context.Unit).toBe(unit);
        expect(context.ObjectType).toBe(objectType);
        for (const [annotation, name, source, accession, commentName, commentValue] of [
            [context.Explication, 'mass', 'MS', 'MS:1000001', 'explication', 'measured'],
            [context.Unit, 'gram', 'UO', 'UO:0000021', 'unit', 'SI'],
            [context.ObjectType, 'sample', 'OBI', 'OBI:0000747', 'object', 'biological'],
        ] as const) {
            expect(annotation?.Name).toBe(name);
            expect(annotation?.TermSourceREF).toBe(source);
            expect(annotation?.TermAccessionNumber).toBe(accession);
            expect(annotation?.Comments[0].Name).toBe(commentName);
            expect(annotation?.Comments[0].Value).toBe(commentValue);
        }
        expect(context.Label).toBe('Sample mass');
        expect(context.Description).toBe('Measured sample mass');
        expect(context.GeneratedBy).toBe('run-1');
        expect(context.Comments).toBe(comments);
        expect(context.Comments[0].Name).toBe('review');
        expect(context.Comments[0].Value).toBe('approved');
    });
});
