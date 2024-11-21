import { LDObject_$reflection, LDObject } from "../LDObject.js";
import { unwrap } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { class_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class Dataset extends LDObject {
    constructor(id, additionalType) {
        super(id, "schema.org/Dataset", unwrap(additionalType));
    }
}

export function Dataset_$reflection() {
    return class_type("ARCtrl.ROCrate.Dataset", undefined, Dataset, LDObject_$reflection());
}

export function Dataset_$ctor_27AED5E3(id, additionalType) {
    return new Dataset(id, additionalType);
}

