import { Record } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { Source_$reflection } from "./Source.js";
import { record_type, option_type, list_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { Sample_$reflection } from "./Sample.js";
import { Material_$reflection } from "./Material.js";
import { defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { empty } from "../../../fable_modules/fable-library.4.1.4/List.js";

export class StudyMaterials extends Record {
    constructor(Sources, Samples, OtherMaterials) {
        super();
        this.Sources = Sources;
        this.Samples = Samples;
        this.OtherMaterials = OtherMaterials;
    }
}

export function StudyMaterials_$reflection() {
    return record_type("ISA.StudyMaterials", [], StudyMaterials, () => [["Sources", option_type(list_type(Source_$reflection()))], ["Samples", option_type(list_type(Sample_$reflection()))], ["OtherMaterials", option_type(list_type(Material_$reflection()))]]);
}

export function StudyMaterials_make(sources, samples, otherMaterials) {
    return new StudyMaterials(sources, samples, otherMaterials);
}

export function StudyMaterials_create_Z460D555F(Sources, Samples, OtherMaterials) {
    return StudyMaterials_make(Sources, Samples, OtherMaterials);
}

export function StudyMaterials_get_empty() {
    return StudyMaterials_create_Z460D555F();
}

export function StudyMaterials_getMaterials_6A1922E7(am) {
    return defaultArg(am.OtherMaterials, empty());
}

export function StudyMaterials_getSamples_6A1922E7(am) {
    return defaultArg(am.Samples, empty());
}

export function StudyMaterials_getSources_6A1922E7(am) {
    return defaultArg(am.Sources, empty());
}

