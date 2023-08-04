import { Record } from "../../../fable_modules/fable-library-ts/Types.js";
import { defaultArg, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { empty, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { Source_$reflection, Source } from "./Source.js";
import { Sample_$reflection, Sample } from "./Sample.js";
import { Material_$reflection, Material } from "./Material.js";
import { IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { record_type, option_type, list_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";

export class StudyMaterials extends Record implements IEquatable<StudyMaterials> {
    readonly Sources: Option<FSharpList<Source>>;
    readonly Samples: Option<FSharpList<Sample>>;
    readonly OtherMaterials: Option<FSharpList<Material>>;
    constructor(Sources: Option<FSharpList<Source>>, Samples: Option<FSharpList<Sample>>, OtherMaterials: Option<FSharpList<Material>>) {
        super();
        this.Sources = Sources;
        this.Samples = Samples;
        this.OtherMaterials = OtherMaterials;
    }
}

export function StudyMaterials_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.StudyMaterials", [], StudyMaterials, () => [["Sources", option_type(list_type(Source_$reflection()))], ["Samples", option_type(list_type(Sample_$reflection()))], ["OtherMaterials", option_type(list_type(Material_$reflection()))]]);
}

export function StudyMaterials_make(sources: Option<FSharpList<Source>>, samples: Option<FSharpList<Sample>>, otherMaterials: Option<FSharpList<Material>>): StudyMaterials {
    return new StudyMaterials(sources, samples, otherMaterials);
}

export function StudyMaterials_create_1BE9FA55(Sources?: FSharpList<Source>, Samples?: FSharpList<Sample>, OtherMaterials?: FSharpList<Material>): StudyMaterials {
    return StudyMaterials_make(Sources, Samples, OtherMaterials);
}

export function StudyMaterials_get_empty(): StudyMaterials {
    return StudyMaterials_create_1BE9FA55();
}

export function StudyMaterials_getMaterials_Z34D4FD6D(am: StudyMaterials): FSharpList<Material> {
    return defaultArg(am.OtherMaterials, empty<Material>());
}

export function StudyMaterials_getSamples_Z34D4FD6D(am: StudyMaterials): FSharpList<Sample> {
    return defaultArg(am.Samples, empty<Sample>());
}

export function StudyMaterials_getSources_Z34D4FD6D(am: StudyMaterials): FSharpList<Source> {
    return defaultArg(am.Sources, empty<Source>());
}

