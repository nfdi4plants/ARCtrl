import { Union } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { union_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";

export class MaterialType extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["ExtractName", "LabeledExtractName"];
    }
}

export function MaterialType_$reflection() {
    return union_type("ISA.MaterialType", [], MaterialType, () => [[], []]);
}

export function MaterialType_create_Z721C83C5(t) {
    switch (t) {
        case "Extract Name":
            return new MaterialType(0, []);
        case "Labeled Extract Name":
            return new MaterialType(1, []);
        default:
            throw new Error("No other value than \"Extract Name\" or \"Labeled Extract Name\" allowed for materialtype");
    }
}

/**
 * Returns the type of the MaterialType
 */
export function MaterialType__get_AsString(this$) {
    if (this$.tag === 1) {
        return "Labeled Extract";
    }
    else {
        return "Extract";
    }
}

