import { Assay_$reflection, Assay } from "./Assay.js";
import { Option } from "../fable_modules/fable-library-ts/Option.js";
import { Record } from "../fable_modules/fable-library-ts/Types.js";
import { IComparable, IEquatable } from "../fable_modules/fable-library-ts/Util.js";
import { record_type, array_type, option_type, string_type, TypeInfo } from "../fable_modules/fable-library-ts/Reflection.js";

export class Study extends Record implements IEquatable<Study>, IComparable<Study> {
    readonly Identifier: Option<string>;
    readonly Assays: Option<Assay[]>;
    constructor(Identifier: Option<string>, Assays: Option<Assay[]>) {
        super();
        this.Identifier = Identifier;
        this.Assays = Assays;
    }
    static create({ Identifier, Assays }: {Identifier?: string, Assays?: Assay[] }): Study {
        return new Study(Identifier, Assays);
    }
    static tryGetAssayByID(assayIdentifier: string, study: Study): Option<Assay> {
        throw new Error();
    }
    static updateAssayByID(assay: Assay, assayIdentifier: string, study: Study): Study {
        Study.tryGetAssayByID(assayIdentifier, study);
        throw new Error();
    }
    static addAssay(assay: Assay, study: Study): Study {
        throw new Error();
    }
}

export function Study_$reflection(): TypeInfo {
    return record_type("ISA.Study", [], Study, () => [["Identifier", option_type(string_type)], ["Assays", option_type(array_type(Assay_$reflection()))]]);
}

