import { describe, expect, it } from 'vitest';
import { Comment, Data, DataFile } from '../../src/ARCtrl/index.ts';

describe('Data primary constructor', () => {
    it('constructs with omitted optional arguments', () => {
        const data = new Data();

        expect(data.ID).toBeUndefined();
        expect(data.Name).toBeUndefined();
        expect(data.FilePath).toBeUndefined();
        expect(data.Selector).toBeUndefined();
        expect(data.DataType).toBeUndefined();
        expect(data.Format).toBeUndefined();
        expect(data.SelectorFormat).toBeUndefined();
        expect(data.Comments).toStrictEqual([]);
    });

    it('preserves every positional argument and splits the name into path and selector', () => {
        const dataType = new DataFile(1, []);
        const comments = [new Comment('source', 'instrument')];
        const data = new Data('data-id', 'data/results.csv#row=7', dataType, 'text/csv', 'RFC 7111', comments);

        expect(data.ID).toBe('data-id');
        expect(data.Name).toBe('data/results.csv#row=7');
        expect(data.FilePath).toBe('data/results.csv');
        expect(data.Selector).toBe('row=7');
        expect(data.DataType).toBe(dataType);
        expect(data.DataType?.tag).toBe(1);
        expect(data.DataType?.fields).toStrictEqual([]);
        expect(data.Format).toBe('text/csv');
        expect(data.SelectorFormat).toBe('RFC 7111');
        expect(data.Comments).toBe(comments);
        expect(data.Comments[0].Name).toBe('source');
        expect(data.Comments[0].Value).toBe('instrument');
    });
});
