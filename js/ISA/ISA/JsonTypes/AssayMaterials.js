import { Record } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { Sample_$reflection } from "./Sample.js";
import { record_type, option_type, list_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { Material_$reflection } from "./Material.js";
import { defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { empty } from "../../../fable_modules/fable-library.4.1.4/List.js";

export class AssayMaterials extends Record {
    constructor(Samples, OtherMaterials) {
        super();
        this.Samples = Samples;
        this.OtherMaterials = OtherMaterials;
    }
}

export function AssayMaterials_$reflection() {
    return record_type("ARCtrl.ISA.AssayMaterials", [], AssayMaterials, () => [["Samples", option_type(list_type(Sample_$reflection()))], ["OtherMaterials", option_type(list_type(Material_$reflection()))]]);
}

export function AssayMaterials_make(samples, otherMaterials) {
    return new AssayMaterials(samples, otherMaterials);
}

export function AssayMaterials_create_Z253F0553(Samples, OtherMaterials) {
    return AssayMaterials_make(Samples, OtherMaterials);
}

export function AssayMaterials_get_empty() {
    return AssayMaterials_create_Z253F0553();
}

export function AssayMaterials_getMaterials_35E61745(am) {
    return defaultArg(am.OtherMaterials, empty());
}

export function AssayMaterials_getSamples_35E61745(am) {
    return defaultArg(am.Samples, empty());
}

