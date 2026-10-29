import { describe, expect, it } from 'vitest';
import { ArcTable, Comment, CompositeCell, CompositeHeader, IOType, OntologyAnnotation } from '../../src/ARCtrl/index.ts';
import { Data } from '../../src/ARCtrl/ts/Core/Data.ts';
import { DataFile } from '../../src/ARCtrl/ts/Core/DataFile.ts';

describe('ArcTable primary constructor', () => {
    it('constructs with only a name', () => {
        const table = new ArcTable('Empty table');

        expect(table.Name).toBe('Empty table');
        expect(table.Headers).toStrictEqual([]);
        expect(table.ColumnCount).toBe(0);
        expect(table.RowCount).toBe(0);
        expect(table.Columns).toStrictEqual([]);
    });

    it('preserves the name, headers, and column-major cells with nested payloads', () => {
        const annotation = new OntologyAnnotation('organism', 'OBI', 'OBI:0100026', [new Comment('term', 'curated')]);
        const unit = new OntologyAnnotation('gram', 'UO', 'UO:0000021');
        const data = new Data('table-data', 'data/results.csv#row=4', new DataFile(1, []), 'text/csv', 'RFC 7111', [new Comment('quality', 'checked')]);
        const headers = [
            new CompositeHeader(1, [annotation]),
            new CompositeHeader(3, [new OntologyAnnotation('mass')]),
            new CompositeHeader(11, [new IOType(2, [])]),
            new CompositeHeader(13, ['Notes']),
        ];
        const columns = [
            [new CompositeCell(0, [annotation]), new CompositeCell(0, [new OntologyAnnotation('mouse')])],
            [new CompositeCell(2, ['5', unit]), new CompositeCell(2, ['7', unit])],
            [new CompositeCell(3, [data]), new CompositeCell(3, [new Data(undefined, 'data/second.csv')])],
            [new CompositeCell(1, ['first row']), new CompositeCell(1, ['second row'])],
        ];
        const table = new ArcTable('Measurements', headers, columns);

        expect(table.Name).toBe('Measurements');
        expect(table.Headers).toStrictEqual(headers);
        expect(table.ColumnCount).toBe(4);
        expect(table.RowCount).toBe(2);
        columns.forEach((column, columnIndex) => {
            column.forEach((cell, rowIndex) => {
                expect(table.GetCellAt(columnIndex, rowIndex)).toStrictEqual(cell);
            });
        });
        const characteristicHeader = table.Headers[0];
        expect(characteristicHeader.tag).toBe(1);
        if (characteristicHeader.tag === 1) {
            expect(characteristicHeader.fields[0]).toBe(annotation);
            expect(characteristicHeader.fields[0].Name).toBe('organism');
            expect(characteristicHeader.fields[0].TermSourceREF).toBe('OBI');
            expect(characteristicHeader.fields[0].TermAccessionNumber).toBe('OBI:0100026');
            expect(characteristicHeader.fields[0].Comments[0].Name).toBe('term');
            expect(characteristicHeader.fields[0].Comments[0].Value).toBe('curated');
        }
        const parameterHeader = table.Headers[1];
        expect(parameterHeader.tag).toBe(3);
        if (parameterHeader.tag === 3) {
            expect(parameterHeader.fields[0].Name).toBe('mass');
        }
        const inputHeader = table.Headers[2];
        expect(inputHeader.tag).toBe(11);
        if (inputHeader.tag === 11) {
            expect(inputHeader.fields[0].tag).toBe(2);
            expect(inputHeader.fields[0].fields).toStrictEqual([]);
        }
        expect(table.Headers[3].fields).toStrictEqual(['Notes']);
        const unitized = table.GetCellAt(1, 0);
        expect(unitized.tag).toBe(2);
        if (unitized.tag === 2) {
            expect(unitized.fields[0]).toBe('5');
            expect(unitized.fields[1].Name).toBe('gram');
            expect(unitized.fields[1].TermSourceREF).toBe('UO');
            expect(unitized.fields[1].TermAccessionNumber).toBe('UO:0000021');
        }
        const dataCell = table.GetCellAt(2, 0);
        expect(dataCell.tag).toBe(3);
        if (dataCell.tag === 3) {
            expect(dataCell.fields[0].ID).toBe('table-data');
            expect(dataCell.fields[0].Name).toBe('data/results.csv#row=4');
            expect(dataCell.fields[0].FilePath).toBe('data/results.csv');
            expect(dataCell.fields[0].Selector).toBe('row=4');
            expect(dataCell.fields[0].DataType?.tag).toBe(1);
            expect(dataCell.fields[0].Format).toBe('text/csv');
            expect(dataCell.fields[0].SelectorFormat).toBe('RFC 7111');
            expect(dataCell.fields[0].Comments[0].Name).toBe('quality');
            expect(dataCell.fields[0].Comments[0].Value).toBe('checked');
        }
    });
});
