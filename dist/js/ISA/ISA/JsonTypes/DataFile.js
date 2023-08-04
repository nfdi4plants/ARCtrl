import { Union } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { union_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";

export class DataFile extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["RawDataFile", "DerivedDataFile", "ImageFile"];
    }
}

export function DataFile_$reflection() {
    return union_type("ARCtrl.ISA.DataFile", [], DataFile, () => [[], [], []]);
}

export function DataFile_get_RawDataFileJson() {
    return "Raw Data File";
}

export function DataFile_get_DerivedDataFileJson() {
    return "Derived Data File";
}

export function DataFile_get_ImageFileJson() {
    return "Image File";
}

export function DataFile__get_AsString(this$) {
    switch (this$.tag) {
        case 1:
            return "DerivedDataFileJson";
        case 2:
            return "ImageFileJson";
        default:
            return "RawDataFileJson";
    }
}

export function DataFile__get_IsDerivedData(this$) {
    if (this$.tag === 1) {
        return true;
    }
    else {
        return false;
    }
}

export function DataFile__get_IsRawData(this$) {
    if (this$.tag === 0) {
        return true;
    }
    else {
        return false;
    }
}

export function DataFile__get_IsImage(this$) {
    if (this$.tag === 2) {
        return true;
    }
    else {
        return false;
    }
}

