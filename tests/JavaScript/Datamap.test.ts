import { describe, expect, it } from 'vitest';
import { Comment, Datamap, DataContext, OntologyAnnotation } from '../../src/ARCtrl/index.ts';
import { DataFile } from '../../src/ARCtrl/ts/Core/DataFile.ts';

describe('Datamap primary constructor', () => {
    it('constructs from an empty context array', () => {
        const datamap = new Datamap([]);

        expect(datamap.DataContexts).toStrictEqual([]);
        expect(datamap.StaticHash).toBe(0);
    });

    it('retains contexts in positional order with nested data and ontology fields', () => {
        const contexts = [
            new DataContext(
                'context-1', 'data/results.csv#row=1', new DataFile(1, []), 'text/csv', 'RFC 7111',
                new OntologyAnnotation('mass', 'MS', 'MS:1000001'),
                new OntologyAnnotation('gram', 'UO', 'UO:0000021'),
                new OntologyAnnotation('sample', 'OBI', 'OBI:0000747'),
                'Mass', 'A measured mass', 'run-1', [new Comment('quality', 'checked')],
            ),
            new DataContext('context-2', 'data/image.png'),
        ];
        const datamap = new Datamap(contexts);

        expect(datamap.DataContexts).toBe(contexts);
        expect(datamap.DataContexts).toHaveLength(2);
        expect(datamap.StaticHash).toBe(0);
        const first = datamap.DataContexts[0];
        expect(first.ID).toBe('context-1');
        expect(first.Name).toBe('data/results.csv#row=1');
        expect(first.FilePath).toBe('data/results.csv');
        expect(first.Selector).toBe('row=1');
        expect(first.DataType?.tag).toBe(1);
        expect(first.DataType?.fields).toStrictEqual([]);
        expect(first.Format).toBe('text/csv');
        expect(first.SelectorFormat).toBe('RFC 7111');
        expect(first.Explication?.Name).toBe('mass');
        expect(first.Explication?.TermSourceREF).toBe('MS');
        expect(first.Explication?.TermAccessionNumber).toBe('MS:1000001');
        expect(first.Unit?.Name).toBe('gram');
        expect(first.Unit?.TermSourceREF).toBe('UO');
        expect(first.Unit?.TermAccessionNumber).toBe('UO:0000021');
        expect(first.ObjectType?.Name).toBe('sample');
        expect(first.ObjectType?.TermSourceREF).toBe('OBI');
        expect(first.ObjectType?.TermAccessionNumber).toBe('OBI:0000747');
        expect(first.Label).toBe('Mass');
        expect(first.Description).toBe('A measured mass');
        expect(first.GeneratedBy).toBe('run-1');
        expect(first.Comments[0].Name).toBe('quality');
        expect(first.Comments[0].Value).toBe('checked');
        expect(datamap.DataContexts[1].ID).toBe('context-2');
        expect(datamap.DataContexts[1].Name).toBe('data/image.png');
    });
});
