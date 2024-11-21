import { Union } from "../fable_modules/fable-library-js.4.22.0/Types.js";
import { union_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class ConverterOptions extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["ARCtrl", "ROCrate", "ISAJson"];
    }
}

export function ConverterOptions_$reflection() {
    return union_type("ARCtrl.Json.ConverterOptions", [], ConverterOptions, () => [[], [], []]);
}

