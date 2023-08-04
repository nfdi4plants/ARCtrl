import { Union } from "../../../fable_modules/fable-library-ts/Types.js";
import { union_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";

export type DataFile_$union = 
    | DataFile<0>
    | DataFile<1>
    | DataFile<2>

export type DataFile_$cases = {
    0: ["RawDataFile", []],
    1: ["DerivedDataFile", []],
    2: ["ImageFile", []]
}

export function DataFile_RawDataFile() {
    return new DataFile<0>(0, []);
}

export function DataFile_DerivedDataFile() {
    return new DataFile<1>(1, []);
}

export function DataFile_ImageFile() {
    return new DataFile<2>(2, []);
}

export class DataFile<Tag extends keyof DataFile_$cases> extends Union<Tag, DataFile_$cases[Tag][0]> {
    constructor(readonly tag: Tag, readonly fields: DataFile_$cases[Tag][1]) {
        super();
    }
    cases() {
        return ["RawDataFile", "DerivedDataFile", "ImageFile"];
    }
}

export function DataFile_$reflection(): TypeInfo {
    return union_type("ARCtrl.ISA.DataFile", [], DataFile, () => [[], [], []]);
}

export function DataFile_get_RawDataFileJson(): string {
    return "Raw Data File";
}

export function DataFile_get_DerivedDataFileJson(): string {
    return "Derived Data File";
}

export function DataFile_get_ImageFileJson(): string {
    return "Image File";
}

export function DataFile__get_AsString(this$: DataFile_$union): string {
    switch (this$.tag) {
        case /* DerivedDataFile */ 1:
            return "DerivedDataFileJson";
        case /* ImageFile */ 2:
            return "ImageFileJson";
        default:
            return "RawDataFileJson";
    }
}

export function DataFile__get_IsDerivedData(this$: DataFile_$union): boolean {
    if (this$.tag === /* DerivedDataFile */ 1) {
        return true;
    }
    else {
        return false;
    }
}

export function DataFile__get_IsRawData(this$: DataFile_$union): boolean {
    if (this$.tag === /* RawDataFile */ 0) {
        return true;
    }
    else {
        return false;
    }
}

export function DataFile__get_IsImage(this$: DataFile_$union): boolean {
    if (this$.tag === /* ImageFile */ 2) {
        return true;
    }
    else {
        return false;
    }
}

