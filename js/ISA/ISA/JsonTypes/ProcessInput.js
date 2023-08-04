import { Union, toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { printf, toText } from "../../../fable_modules/fable-library.4.1.4/String.js";
import { Source_create_7A281ED9, Source_getUnits_Z28BE5327, Source_setCharacteristicValues, Source_get_empty, Source_$reflection } from "./Source.js";
import { Sample_create_E50ED22, Sample_getUnits_Z29207F1E, Sample_setCharacteristicValues, Sample_$reflection } from "./Sample.js";
import { Data_create_Z326CF519, Data_$reflection } from "./Data.js";
import { Material_create_Z31BE6CDD, Material_getUnits_Z42815C11, Material_setCharacteristicValues, Material_$reflection } from "./Material.js";
import { union_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { unwrap, map, defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { empty, choose } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { DataFile } from "./DataFile.js";

export class ProcessInput extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["Source", "Sample", "Data", "Material"];
    }
    Print() {
        const this$ = this;
        return toString(this$);
    }
    PrintCompact() {
        const this$ = this;
        switch (this$.tag) {
            case 0: {
                const arg_1 = this$.fields[0].PrintCompact();
                return toText(printf("Source {%s}"))(arg_1);
            }
            case 3: {
                const arg_2 = this$.fields[0].PrintCompact();
                return toText(printf("Material {%s}"))(arg_2);
            }
            case 2: {
                const arg_3 = this$.fields[0].PrintCompact();
                return toText(printf("Data {%s}"))(arg_3);
            }
            default: {
                const arg = this$.fields[0].PrintCompact();
                return toText(printf("Sample {%s}"))(arg);
            }
        }
    }
}

export function ProcessInput_$reflection() {
    return union_type("ARCtrl.ISA.ProcessInput", [], ProcessInput, () => [[["Item", Source_$reflection()]], [["Item", Sample_$reflection()]], [["Item", Data_$reflection()]], [["Item", Material_$reflection()]]]);
}

export function ProcessInput__get_TryName(this$) {
    switch (this$.tag) {
        case 0:
            return this$.fields[0].Name;
        case 3:
            return this$.fields[0].Name;
        case 2:
            return this$.fields[0].Name;
        default:
            return this$.fields[0].Name;
    }
}

export function ProcessInput__get_Name(this$) {
    return defaultArg(ProcessInput__get_TryName(this$), "");
}

export function ProcessInput_get_Default() {
    return new ProcessInput(0, [Source_get_empty()]);
}

/**
 * Returns name of processInput
 */
export function ProcessInput_tryGetName_102B6859(pi) {
    return ProcessInput__get_TryName(pi);
}

/**
 * Returns name of processInput
 */
export function ProcessInput_getName_102B6859(pi) {
    return ProcessInput__get_Name(pi);
}

/**
 * Returns true, if given name equals name of processInput
 */
export function ProcessInput_nameEquals(name, pi) {
    return ProcessInput__get_Name(pi) === name;
}

/**
 * Returns true, if Process Input is Sample
 */
export function ProcessInput_isSample_102B6859(pi) {
    if (pi.tag === 1) {
        return true;
    }
    else {
        return false;
    }
}

/**
 * Returns true, if Process Input is Source
 */
export function ProcessInput_isSource_102B6859(pi) {
    if (pi.tag === 0) {
        return true;
    }
    else {
        return false;
    }
}

/**
 * Returns true, if Process Input is Data
 */
export function ProcessInput_isData_102B6859(pi) {
    if (pi.tag === 2) {
        return true;
    }
    else {
        return false;
    }
}

/**
 * Returns true, if Process Input is Material
 */
export function ProcessInput_isMaterial_102B6859(pi) {
    if (pi.tag === 3) {
        return true;
    }
    else {
        return false;
    }
}

/**
 * Returns true, if Process Input is Source
 */
export function ProcessInput__isSource(this$) {
    return ProcessInput_isSource_102B6859(this$);
}

/**
 * Returns true, if Process Input is Sample
 */
export function ProcessInput__isSample(this$) {
    return ProcessInput_isSample_102B6859(this$);
}

/**
 * Returns true, if Process Input is Data
 */
export function ProcessInput__isData(this$) {
    return ProcessInput_isData_102B6859(this$);
}

/**
 * Returns true, if Process Input is Material
 */
export function ProcessInput__isMaterial(this$) {
    return ProcessInput_isMaterial_102B6859(this$);
}

/**
 * If given process input is a sample, returns it, else returns None
 */
export function ProcessInput_trySample_102B6859(pi) {
    if (pi.tag === 1) {
        return pi.fields[0];
    }
    else {
        return void 0;
    }
}

/**
 * If given process input is a source, returns it, else returns None
 */
export function ProcessInput_trySource_102B6859(pi) {
    if (pi.tag === 0) {
        return pi.fields[0];
    }
    else {
        return void 0;
    }
}

/**
 * If given process input is a data, returns it, else returns None
 */
export function ProcessInput_tryData_102B6859(pi) {
    if (pi.tag === 2) {
        return pi.fields[0];
    }
    else {
        return void 0;
    }
}

/**
 * If given process input is a material, returns it, else returns None
 */
export function ProcessInput_tryMaterial_102B6859(pi) {
    if (pi.tag === 3) {
        return pi.fields[0];
    }
    else {
        return void 0;
    }
}

export function ProcessInput_setCharacteristicValues(characteristics, pi) {
    switch (pi.tag) {
        case 0:
            return new ProcessInput(0, [Source_setCharacteristicValues(characteristics, pi.fields[0])]);
        case 3:
            return new ProcessInput(3, [Material_setCharacteristicValues(characteristics, pi.fields[0])]);
        case 2:
            return pi;
        default:
            return new ProcessInput(1, [Sample_setCharacteristicValues(characteristics, pi.fields[0])]);
    }
}

/**
 * If given process input contains characteristics, returns them
 */
export function ProcessInput_tryGetCharacteristicValues_102B6859(pi) {
    switch (pi.tag) {
        case 0:
            return pi.fields[0].Characteristics;
        case 3:
            return pi.fields[0].Characteristics;
        case 2:
            return void 0;
        default:
            return pi.fields[0].Characteristics;
    }
}

/**
 * If given process input contains characteristics, returns them
 */
export function ProcessInput_tryGetCharacteristics_102B6859(pi) {
    return map((list) => choose((c) => c.Category, list), ProcessInput_tryGetCharacteristicValues_102B6859(pi));
}

export function ProcessInput_getCharacteristicValues_102B6859(pi) {
    return defaultArg(ProcessInput_tryGetCharacteristicValues_102B6859(pi), empty());
}

/**
 * If given process output contains units, returns them
 */
export function ProcessInput_getUnits_102B6859(pi) {
    switch (pi.tag) {
        case 1:
            return Sample_getUnits_Z29207F1E(pi.fields[0]);
        case 3:
            return Material_getUnits_Z42815C11(pi.fields[0]);
        case 2:
            return empty();
        default:
            return Source_getUnits_Z28BE5327(pi.fields[0]);
    }
}

export function ProcessInput_createSource_7888CE42(name, characteristics) {
    return new ProcessInput(0, [Source_create_7A281ED9(void 0, name, unwrap(characteristics))]);
}

export function ProcessInput_createSample_Z6DF16D07(name, characteristics, factors, derivesFrom) {
    return new ProcessInput(1, [Sample_create_E50ED22(void 0, name, unwrap(characteristics), unwrap(factors), unwrap(derivesFrom))]);
}

export function ProcessInput_createMaterial_2363974C(name, characteristics, derivesFrom) {
    return new ProcessInput(3, [Material_create_Z31BE6CDD(void 0, name, void 0, unwrap(characteristics), unwrap(derivesFrom))]);
}

export function ProcessInput_createImageFile_Z721C83C5(name) {
    return new ProcessInput(2, [Data_create_Z326CF519(void 0, name, new DataFile(2, []))]);
}

export function ProcessInput_createRawData_Z721C83C5(name) {
    return new ProcessInput(2, [Data_create_Z326CF519(void 0, name, new DataFile(0, []))]);
}

export function ProcessInput_createDerivedData_Z721C83C5(name) {
    return new ProcessInput(2, [Data_create_Z326CF519(void 0, name, new DataFile(1, []))]);
}

