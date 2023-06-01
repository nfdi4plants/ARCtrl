import { Record } from "../fable_modules/fable-library-ts/Types.js";
import { Option } from "../fable_modules/fable-library-ts/Option.js";
import { IComparable, IEquatable } from "../fable_modules/fable-library-ts/Util.js";
import { record_type, option_type, string_type, TypeInfo } from "../fable_modules/fable-library-ts/Reflection.js";

export class Assay extends Record implements IEquatable<Assay>, IComparable<Assay> {
    readonly FileName: Option<string>;
    constructor(FileName: Option<string>) {
        super();
        this.FileName = FileName;
    }
    static create({ FileName }: {FileName?: string }): Assay {
        return new Assay(FileName);
    }
    static getIdentifier<$a>(assay: Assay): $a {
        throw new Error();
    }
}

export function Assay_$reflection(): TypeInfo {
    return record_type("ISA.Assay", [], Assay, () => [["FileName", option_type(string_type)]]);
}

