import { describe, expect, it } from 'vitest';
import { DataFile } from '../../src/ARCtrl/index.ts';

describe('DataFile primary constructor', () => {
    it("RawDataFile", () => {
        const dataFile = DataFile.rawDataFile()
        expect(dataFile.fields).toStrictEqual([]);
        expect(dataFile.AsString).toBe("Raw Data File");
    });
    it("DerivedDataFile", () => {
        const dataFile = DataFile.derivedDataFile()
        expect(dataFile.fields).toStrictEqual([]);
        expect(dataFile.AsString).toBe("Derived Data File");
    });

    it("ImageFile", () => {
        const dataFile = DataFile.imageFile()
        expect(dataFile.fields).toStrictEqual([]);
        expect(dataFile.AsString).toBe("Image File");
    });
});
