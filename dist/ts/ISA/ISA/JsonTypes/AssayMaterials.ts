import { Record } from "../../../fable_modules/fable-library-ts/Types.js";
import { defaultArg, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { empty, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { Sample_$reflection, Sample } from "./Sample.js";
import { Material_$reflection, Material } from "./Material.js";
import { IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { record_type, option_type, list_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";

export class AssayMaterials extends Record implements IEquatable<AssayMaterials> {
    readonly Samples: Option<FSharpList<Sample>>;
    readonly OtherMaterials: Option<FSharpList<Material>>;
    constructor(Samples: Option<FSharpList<Sample>>, OtherMaterials: Option<FSharpList<Material>>) {
        super();
        this.Samples = Samples;
        this.OtherMaterials = OtherMaterials;
    }
}

export function AssayMaterials_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.AssayMaterials", [], AssayMaterials, () => [["Samples", option_type(list_type(Sample_$reflection()))], ["OtherMaterials", option_type(list_type(Material_$reflection()))]]);
}

export function AssayMaterials_make(samples: Option<FSharpList<Sample>>, otherMaterials: Option<FSharpList<Material>>): AssayMaterials {
    return new AssayMaterials(samples, otherMaterials);
}

export function AssayMaterials_create_Z253F0553(Samples?: FSharpList<Sample>, OtherMaterials?: FSharpList<Material>): AssayMaterials {
    return AssayMaterials_make(Samples, OtherMaterials);
}

export function AssayMaterials_get_empty(): AssayMaterials {
    return AssayMaterials_create_Z253F0553();
}

export function AssayMaterials_getMaterials_35E61745(am: AssayMaterials): FSharpList<Material> {
    return defaultArg(am.OtherMaterials, empty<Material>());
}

export function AssayMaterials_getSamples_35E61745(am: AssayMaterials): FSharpList<Sample> {
    return defaultArg(am.Samples, empty<Sample>());
}

