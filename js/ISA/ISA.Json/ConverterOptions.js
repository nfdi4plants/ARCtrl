import { class_type } from "../../fable_modules/fable-library.4.1.4/Reflection.js";

export class ConverterOptions {
    constructor() {
        this.setID = false;
        this.includeType = false;
    }
}

export function ConverterOptions_$reflection() {
    return class_type("ISA.Json.ConverterOptions", void 0, ConverterOptions);
}

export function ConverterOptions_$ctor() {
    return new ConverterOptions();
}

export function ConverterOptions__get_SetID(this$) {
    return this$.setID;
}

export function ConverterOptions__set_SetID_Z1FBCCD16(this$, setId) {
    this$.setID = setId;
}

export function ConverterOptions__get_IncludeType(this$) {
    return this$.includeType;
}

export function ConverterOptions__set_IncludeType_Z1FBCCD16(this$, iT) {
    this$.includeType = iT;
}

