import { class_type, TypeInfo } from "../../fable_modules/fable-library-ts/Reflection.js";

export class ConverterOptions {
    setID: boolean;
    includeType: boolean;
    constructor() {
        this.setID = false;
        this.includeType = false;
    }
}

export function ConverterOptions_$reflection(): TypeInfo {
    return class_type("ISA.Json.ConverterOptions", void 0, ConverterOptions);
}

export function ConverterOptions_$ctor(): ConverterOptions {
    return new ConverterOptions();
}

export function ConverterOptions__get_SetID(this$: ConverterOptions): boolean {
    return this$.setID;
}

export function ConverterOptions__set_SetID_Z1FBCCD16(this$: ConverterOptions, setId: boolean): void {
    this$.setID = setId;
}

export function ConverterOptions__get_IncludeType(this$: ConverterOptions): boolean {
    return this$.includeType;
}

export function ConverterOptions__set_IncludeType_Z1FBCCD16(this$: ConverterOptions, iT: boolean): void {
    this$.includeType = iT;
}

