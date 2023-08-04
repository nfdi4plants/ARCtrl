import { Union, toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { printf, toText } from "../../../fable_modules/fable-library.4.1.4/String.js";
import { Sample_create_E50ED22, Sample_getUnits_Z29207F1E, Sample_setFactorValues, Sample_get_empty, Sample_$reflection } from "./Sample.js";
import { Data_create_Z326CF519, Data_$reflection } from "./Data.js";
import { Material_create_Z31BE6CDD, Material_getUnits_Z42815C11, Material_$reflection } from "./Material.js";
import { union_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { unwrap, map, defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { empty, choose } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { DataFile } from "./DataFile.js";

export class ProcessOutput extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["Sample", "Data", "Material"];
    }
    Print() {
        const this$ = this;
        return toString(this$);
    }
    PrintCompact() {
        const this$ = this;
        switch (this$.tag) {
            case 2: {
                const arg_1 = this$.fields[0].PrintCompact();
                return toText(printf("Material {%s}"))(arg_1);
            }
            case 1: {
                const arg_2 = this$.fields[0].PrintCompact();
                return toText(printf("Data {%s}"))(arg_2);
            }
            default: {
                const arg = this$.fields[0].PrintCompact();
                return toText(printf("Sample {%s}"))(arg);
            }
        }
    }
}

export function ProcessOutput_$reflection() {
    return union_type("ARCtrl.ISA.ProcessOutput", [], ProcessOutput, () => [[["Item", Sample_$reflection()]], [["Item", Data_$reflection()]], [["Item", Material_$reflection()]]]);
}

export function ProcessOutput__get_TryName(this$) {
    switch (this$.tag) {
        case 2:
            return this$.fields[0].Name;
        case 1:
            return this$.fields[0].Name;
        default:
            return this$.fields[0].Name;
    }
}

export function ProcessOutput__get_Name(this$) {
    return defaultArg(ProcessOutput__get_TryName(this$), "");
}

export function ProcessOutput_get_Default() {
    return new ProcessOutput(0, [Sample_get_empty()]);
}

/**
 * Returns name of processOutput
 */
export function ProcessOutput_tryGetName_11830B70(po) {
    return ProcessOutput__get_TryName(po);
}

/**
 * Returns name of processInput
 */
export function ProcessOutput_getName_11830B70(po) {
    return ProcessOutput__get_Name(po);
}

/**
 * Returns true, if given name equals name of processOutput
 */
export function ProcessOutput_nameEquals(name, po) {
    return ProcessOutput__get_Name(po) === name;
}

/**
 * Returns true, if Process Output is Sample
 */
export function ProcessOutput_isSample_11830B70(po) {
    if (po.tag === 0) {
        return true;
    }
    else {
        return false;
    }
}

/**
 * Returns true, if Process Output is Data
 */
export function ProcessOutput_isData_11830B70(po) {
    if (po.tag === 1) {
        return true;
    }
    else {
        return false;
    }
}

/**
 * Returns true, if Process Output is Material
 */
export function ProcessOutput_isMaterial_11830B70(po) {
    if (po.tag === 2) {
        return true;
    }
    else {
        return false;
    }
}

/**
 * Returns true, if Process Output is Sample
 */
export function ProcessOutput__isSample(this$) {
    return ProcessOutput_isSample_11830B70(this$);
}

/**
 * Returns true, if Process Output is Data
 */
export function ProcessOutput__isData(this$) {
    return ProcessOutput_isData_11830B70(this$);
}

/**
 * Returns true, if Process Output is Material
 */
export function ProcessOutput__isMaterial(this$) {
    return ProcessOutput_isMaterial_11830B70(this$);
}

/**
 * If given process output is a sample, returns it, else returns None
 */
export function ProcessOutput_trySample_11830B70(po) {
    if (po.tag === 0) {
        return po.fields[0];
    }
    else {
        return void 0;
    }
}

/**
 * If given process output is a data, returns it, else returns None
 */
export function ProcessOutput_tryData_11830B70(po) {
    if (po.tag === 1) {
        return po.fields[0];
    }
    else {
        return void 0;
    }
}

/**
 * If given process output is a material, returns it, else returns None
 */
export function ProcessOutput_tryMaterial_11830B70(po) {
    if (po.tag === 2) {
        return po.fields[0];
    }
    else {
        return void 0;
    }
}

/**
 * If given process output contains characteristics, returns them
 */
export function ProcessOutput_tryGetCharacteristicValues_11830B70(po) {
    switch (po.tag) {
        case 2:
            return po.fields[0].Characteristics;
        case 1:
            return void 0;
        default:
            return po.fields[0].Characteristics;
    }
}

/**
 * If given process output contains characteristics, returns them
 */
export function ProcessOutput_tryGetCharacteristics_11830B70(po) {
    return map((list) => choose((c) => c.Category, list), ProcessOutput_tryGetCharacteristicValues_11830B70(po));
}

/**
 * If given process output contains factors, returns them
 */
export function ProcessOutput_tryGetFactorValues_11830B70(po) {
    switch (po.tag) {
        case 2:
            return void 0;
        case 1:
            return void 0;
        default:
            return po.fields[0].FactorValues;
    }
}

export function ProcessOutput_setFactorValues(values, po) {
    switch (po.tag) {
        case 2:
            return po;
        case 1:
            return po;
        default:
            return new ProcessOutput(0, [Sample_setFactorValues(values, po.fields[0])]);
    }
}

export function ProcessOutput_getFactorValues_11830B70(po) {
    return defaultArg(ProcessOutput_tryGetFactorValues_11830B70(po), empty());
}

/**
 * If given process output contains units, returns them
 */
export function ProcessOutput_getUnits_11830B70(po) {
    switch (po.tag) {
        case 2:
            return Material_getUnits_Z42815C11(po.fields[0]);
        case 1:
            return empty();
        default:
            return Sample_getUnits_Z29207F1E(po.fields[0]);
    }
}

export function ProcessOutput_createSample_Z6DF16D07(name, characteristics, factors, derivesFrom) {
    return new ProcessOutput(0, [Sample_create_E50ED22(void 0, name, unwrap(characteristics), unwrap(factors), unwrap(derivesFrom))]);
}

export function ProcessOutput_createMaterial_2363974C(name, characteristics, derivesFrom) {
    return new ProcessOutput(2, [Material_create_Z31BE6CDD(void 0, name, void 0, unwrap(characteristics), unwrap(derivesFrom))]);
}

export function ProcessOutput_createImageFile_Z721C83C5(name) {
    return new ProcessOutput(1, [Data_create_Z326CF519(void 0, name, new DataFile(2, []))]);
}

export function ProcessOutput_createRawData_Z721C83C5(name) {
    return new ProcessOutput(1, [Data_create_Z326CF519(void 0, name, new DataFile(0, []))]);
}

export function ProcessOutput_createDerivedData_Z721C83C5(name) {
    return new ProcessOutput(1, [Data_create_Z326CF519(void 0, name, new DataFile(1, []))]);
}

