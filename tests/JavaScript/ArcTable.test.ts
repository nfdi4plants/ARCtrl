import { describe, expect, it } from 'vitest';
import { ArcTable, Comment, CompositeCell, CompositeHeader, Data, DataFile, IOType, OntologyAnnotation } from '../../src/ARCtrl/index.ts';
function addBasicColumns(table: ArcTable) {
    table.AddColumn(CompositeHeader.input(IOType.source()))
    table.AddColumn(CompositeHeader.ProtocolREF)
    table.AddColumn(CompositeHeader.output(IOType.sample()))
}

function addFilledParameterColumn(table: ArcTable) {
    table.AddColumn(
        CompositeHeader.parameter(new OntologyAnnotation('mass', 'UO', 'UO:0000021')),
        [
            CompositeCell.createUnitizedFromString('5', "kg", "UO", "UO:0000022"),
            CompositeCell.createUnitizedFromString('6', "kg", "UO", "UO:0000022"),
            CompositeCell.createUnitizedFromString('7', "kg", "UO", "UO:0000022"),
            CompositeCell.createUnitizedFromString('8', "kg", "UO", "UO:0000022"),
            CompositeCell.createUnitizedFromString('9', "kg", "UO", "UO:0000022"),
            CompositeCell.createUnitizedFromString('10', "kg", "UO", "UO:0000022"),
            CompositeCell.createUnitizedFromString('11', "kg", "UO", "UO:0000022")
        ]
    )
}

export const mkDefaultTable = () => {
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
    const table = new ArcTable('Filled Measurements', headers, columns);
    return table;
}

describe('ArcTable', () => {
    describe('primary constructor', () => {
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
            const table = mkDefaultTable()

            expect(table.Name).toBe('Filled Measurements');
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
                expect(characteristicHeader.fields[0]).toEqual(annotation);
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
    describe("Table operations", function () {
        it("add columns to the table", function () {
            const table = new ArcTable("table-1");
            addBasicColumns(table);
            expect(table.ColumnCount, "ColumnCount").toBe(3)
            expect(table.Headers.length, "table.Headers.length").toBe(3);
            expect(table.RowCount, "RowCount").toBe(0)
        });
        it("Add columns then rows", function () {
            const table = new ArcTable("table-1");
            addBasicColumns(table);
            table.AddRow();
            expect(table.RowCount, "RowCount after adding a row").toBe(1);
            const cell = table.GetCellAt(0, 0);
            expect(cell).toBeDefined();
            expect(cell.isFreeText).toBe(true);
            expect(cell.fields).toStrictEqual([""]);
        });
        it("Add filled columns with parameters", function () {
            const table = new ArcTable("table-1");
            addFilledParameterColumn(table);
            expect(table.ColumnCount, "ColumnCount").toBe(1);
            expect(table.RowCount, "RowCount").toBe(7);
            const cell = table.GetCellAt(0, 0);
            expect(cell).toBeDefined();
            expect(cell.isUnitized).toBe(true);
            expect(cell.fields).toBeInstanceOf(Array)
            expect(cell.fields[0]).toBe('5');
            expect(cell.fields[1]).toBeDefined();
            if(cell.fields[1]) {
                expect(cell.fields[1].Name).toBe('kg');
                expect(cell.fields[1].TermSourceREF).toBe('UO');
                expect(cell.fields[1].TermAccessionNumber).toBe('UO:0000022');
            } else {
                throw new Error("Expected cell.fields[1] to be defined");
            }
        });
    });
});
