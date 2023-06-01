import { Record } from "../fable_modules/fable-library.4.1.4/Types.js";
import { record_type, option_type, string_type } from "../fable_modules/fable-library.4.1.4/Reflection.js";

export class Assay extends Record {
    constructor(FileName) {
        super();
        this.FileName = FileName;
    }
    static create({ FileName }) {
        return new Assay(FileName);
    }
    static getIdentifier(assay) {
        throw new Error();
    }
}

export function Assay_$reflection() {
    return record_type("ISA.Assay", [], Assay, () => [["FileName", option_type(string_type)]]);
}

