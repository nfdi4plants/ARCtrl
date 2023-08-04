import { Union } from "../../../fable_modules/fable-library-ts/Types.js";
import { union_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";

export type MaterialType_$union = 
    | MaterialType<0>
    | MaterialType<1>

export type MaterialType_$cases = {
    0: ["ExtractName", []],
    1: ["LabeledExtractName", []]
}

export function MaterialType_ExtractName() {
    return new MaterialType<0>(0, []);
}

export function MaterialType_LabeledExtractName() {
    return new MaterialType<1>(1, []);
}

export class MaterialType<Tag extends keyof MaterialType_$cases> extends Union<Tag, MaterialType_$cases[Tag][0]> {
    constructor(readonly tag: Tag, readonly fields: MaterialType_$cases[Tag][1]) {
        super();
    }
    cases() {
        return ["ExtractName", "LabeledExtractName"];
    }
}

export function MaterialType_$reflection(): TypeInfo {
    return union_type("ARCtrl.ISA.MaterialType", [], MaterialType, () => [[], []]);
}

export function MaterialType_create_Z721C83C5(t: string): MaterialType_$union {
    switch (t) {
        case "Extract Name":
            return MaterialType_ExtractName();
        case "Labeled Extract Name":
            return MaterialType_LabeledExtractName();
        default:
            throw new Error("No other value than \"Extract Name\" or \"Labeled Extract Name\" allowed for materialtype");
    }
}

/**
 * Returns the type of the MaterialType
 */
export function MaterialType__get_AsString(this$: MaterialType_$union): string {
    if (this$.tag === /* LabeledExtractName */ 1) {
        return "Labeled Extract";
    }
    else {
        return "Extract";
    }
}

