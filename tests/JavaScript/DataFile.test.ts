import { describe, expect, it } from 'vitest';
import { DataFile, DataFile__get_AsString } from '../../src/ARCtrl/ts/Core/DataFile.ts';

describe('DataFile primary constructor', () => {
    it.each([
        [0, 'RawDataFile', 'Raw Data File'],
        [1, 'DerivedDataFile', 'Derived Data File'],
        [2, 'ImageFile', 'Image File'],
    ] as const)('constructs %s (%s)', (tag, caseName, displayName) => {
        const dataFile = tag === 0
            ? new DataFile(0, [])
            : tag === 1
                ? new DataFile(1, [])
                : new DataFile(2, []);

        expect(dataFile.tag).toBe(tag);
        expect(dataFile.fields).toStrictEqual([]);
        expect(dataFile.cases()[dataFile.tag]).toBe(caseName);
        expect(DataFile__get_AsString(dataFile)).toBe(displayName);
    });
});
