import { Record } from "../fable_modules/fable-library.4.1.4/Types.js";
import { record_type, array_type, option_type, string_type } from "../fable_modules/fable-library.4.1.4/Reflection.js";
import { Assay_$reflection } from "./Assay.js";

export class Study extends Record {
    constructor(Identifier, Assays) {
        super();
        this.Identifier = Identifier;
        this.Assays = Assays;
    }
    static create({ Identifier, Assays }) {
        return new Study(Identifier, Assays);
    }
    static tryGetAssayByID(assayIdentifier, study) {
        throw new Error();
    }
    static updateAssayByID(assay, assayIdentifier, study) {
        Study.tryGetAssayByID(assayIdentifier, study);
        throw new Error();
    }
    static addAssay(assay, study) {
        throw new Error();
    }
}

export function Study_$reflection() {
    return record_type("ISA.Study", [], Study, () => [["Identifier", option_type(string_type)], ["Assays", option_type(array_type(Assay_$reflection()))]]);
}

