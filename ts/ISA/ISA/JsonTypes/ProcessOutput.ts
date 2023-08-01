import { Union, toString } from "../../../fable_modules/fable-library-ts/Types.js";
import { Material_create_76090C97, Material_getUnits_43A4149B, Material_$reflection, Material } from "./Material.js";
import { printf, toText } from "../../../fable_modules/fable-library-ts/String.js";
import { Data_create_Z748D099, Data_$reflection, Data } from "./Data.js";
import { Sample_create_3A6378D6, Sample_getUnits_Z23050B6A, Sample_setFactorValues, Sample_get_empty, Sample_$reflection, Sample } from "./Sample.js";
import { IISAPrintable } from "../Printer.js";
import { union_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { unwrap, map, defaultArg, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { empty, choose, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { MaterialAttributeValue } from "./MaterialAttributeValue.js";
import { MaterialAttribute } from "./MaterialAttribute.js";
import { FactorValue } from "./FactorValue.js";
import { OntologyAnnotation } from "./OntologyAnnotation.js";
import { Source } from "./Source.js";
import { DataFile_DerivedDataFile, DataFile_RawDataFile, DataFile_ImageFile } from "./DataFile.js";

export type ProcessOutput_$union = 
    | ProcessOutput<0>
    | ProcessOutput<1>
    | ProcessOutput<2>

export type ProcessOutput_$cases = {
    0: ["Sample", [Sample]],
    1: ["Data", [Data]],
    2: ["Material", [Material]]
}

export function ProcessOutput_Sample(Item: Sample) {
    return new ProcessOutput<0>(0, [Item]);
}

export function ProcessOutput_Data(Item: Data) {
    return new ProcessOutput<1>(1, [Item]);
}

export function ProcessOutput_Material(Item: Material) {
    return new ProcessOutput<2>(2, [Item]);
}

export class ProcessOutput<Tag extends keyof ProcessOutput_$cases> extends Union<Tag, ProcessOutput_$cases[Tag][0]> implements IISAPrintable {
    constructor(readonly tag: Tag, readonly fields: ProcessOutput_$cases[Tag][1]) {
        super();
    }
    cases() {
        return ["Sample", "Data", "Material"];
    }
    Print(): string {
        const this$ = this as ProcessOutput_$union;
        return toString(this$);
    }
    PrintCompact(): string {
        const this$ = this as ProcessOutput_$union;
        switch (this$.tag) {
            case /* Material */ 2: {
                const m: Material = this$.fields[0];
                const arg_1: string = m.PrintCompact();
                return toText(printf("Material {%s}"))(arg_1);
            }
            case /* Data */ 1: {
                const d: Data = this$.fields[0];
                const arg_2: string = d.PrintCompact();
                return toText(printf("Data {%s}"))(arg_2);
            }
            default: {
                const s: Sample = this$.fields[0];
                const arg: string = s.PrintCompact();
                return toText(printf("Sample {%s}"))(arg);
            }
        }
    }
}

export function ProcessOutput_$reflection(): TypeInfo {
    return union_type("ISA.ProcessOutput", [], ProcessOutput, () => [[["Item", Sample_$reflection()]], [["Item", Data_$reflection()]], [["Item", Material_$reflection()]]]);
}

export function ProcessOutput__get_TryName(this$: ProcessOutput_$union): Option<string> {
    switch (this$.tag) {
        case /* Material */ 2:
            return this$.fields[0].Name;
        case /* Data */ 1:
            return this$.fields[0].Name;
        default:
            return this$.fields[0].Name;
    }
}

export function ProcessOutput__get_Name(this$: ProcessOutput_$union): string {
    return defaultArg(ProcessOutput__get_TryName(this$), "");
}

export function ProcessOutput_get_Default(): ProcessOutput_$union {
    return ProcessOutput_Sample(Sample_get_empty());
}

/**
 * Returns name of processOutput
 */
export function ProcessOutput_tryGetName_Z4A02997C(po: ProcessOutput_$union): Option<string> {
    return ProcessOutput__get_TryName(po);
}

/**
 * Returns name of processInput
 */
export function ProcessOutput_getName_Z4A02997C(po: ProcessOutput_$union): string {
    return ProcessOutput__get_Name(po);
}

/**
 * Returns true, if given name equals name of processOutput
 */
export function ProcessOutput_nameEquals(name: string, po: ProcessOutput_$union): boolean {
    return ProcessOutput__get_Name(po) === name;
}

/**
 * Returns true, if Process Output is Sample
 */
export function ProcessOutput_isSample_Z4A02997C(po: ProcessOutput_$union): boolean {
    if (po.tag === /* Sample */ 0) {
        return true;
    }
    else {
        return false;
    }
}

/**
 * Returns true, if Process Output is Data
 */
export function ProcessOutput_isData_Z4A02997C(po: ProcessOutput_$union): boolean {
    if (po.tag === /* Data */ 1) {
        return true;
    }
    else {
        return false;
    }
}

/**
 * Returns true, if Process Output is Material
 */
export function ProcessOutput_isMaterial_Z4A02997C(po: ProcessOutput_$union): boolean {
    if (po.tag === /* Material */ 2) {
        return true;
    }
    else {
        return false;
    }
}

/**
 * Returns true, if Process Output is Sample
 */
export function ProcessOutput__isSample(this$: ProcessOutput_$union): boolean {
    return ProcessOutput_isSample_Z4A02997C(this$);
}

/**
 * Returns true, if Process Output is Data
 */
export function ProcessOutput__isData(this$: ProcessOutput_$union): boolean {
    return ProcessOutput_isData_Z4A02997C(this$);
}

/**
 * Returns true, if Process Output is Material
 */
export function ProcessOutput__isMaterial(this$: ProcessOutput_$union): boolean {
    return ProcessOutput_isMaterial_Z4A02997C(this$);
}

/**
 * If given process output is a sample, returns it, else returns None
 */
export function ProcessOutput_trySample_Z4A02997C(po: ProcessOutput_$union): Option<Sample> {
    if (po.tag === /* Sample */ 0) {
        return po.fields[0];
    }
    else {
        return void 0;
    }
}

/**
 * If given process output is a data, returns it, else returns None
 */
export function ProcessOutput_tryData_Z4A02997C(po: ProcessOutput_$union): Option<Data> {
    if (po.tag === /* Data */ 1) {
        return po.fields[0];
    }
    else {
        return void 0;
    }
}

/**
 * If given process output is a material, returns it, else returns None
 */
export function ProcessOutput_tryMaterial_Z4A02997C(po: ProcessOutput_$union): Option<Material> {
    if (po.tag === /* Material */ 2) {
        return po.fields[0];
    }
    else {
        return void 0;
    }
}

/**
 * If given process output contains characteristics, returns them
 */
export function ProcessOutput_tryGetCharacteristicValues_Z4A02997C(po: ProcessOutput_$union): Option<FSharpList<MaterialAttributeValue>> {
    switch (po.tag) {
        case /* Material */ 2:
            return po.fields[0].Characteristics;
        case /* Data */ 1:
            return void 0;
        default:
            return po.fields[0].Characteristics;
    }
}

/**
 * If given process output contains characteristics, returns them
 */
export function ProcessOutput_tryGetCharacteristics_Z4A02997C(po: ProcessOutput_$union): Option<FSharpList<MaterialAttribute>> {
    return map<FSharpList<MaterialAttributeValue>, FSharpList<MaterialAttribute>>((list: FSharpList<MaterialAttributeValue>): FSharpList<MaterialAttribute> => choose<MaterialAttributeValue, MaterialAttribute>((c: MaterialAttributeValue): Option<MaterialAttribute> => c.Category, list), ProcessOutput_tryGetCharacteristicValues_Z4A02997C(po));
}

/**
 * If given process output contains factors, returns them
 */
export function ProcessOutput_tryGetFactorValues_Z4A02997C(po: ProcessOutput_$union): Option<FSharpList<FactorValue>> {
    switch (po.tag) {
        case /* Material */ 2:
            return void 0;
        case /* Data */ 1:
            return void 0;
        default:
            return po.fields[0].FactorValues;
    }
}

export function ProcessOutput_setFactorValues(values: FSharpList<FactorValue>, po: ProcessOutput_$union): ProcessOutput_$union {
    switch (po.tag) {
        case /* Material */ 2:
            return po;
        case /* Data */ 1:
            return po;
        default:
            return ProcessOutput_Sample(Sample_setFactorValues(values, po.fields[0]));
    }
}

export function ProcessOutput_getFactorValues_Z4A02997C(po: ProcessOutput_$union): FSharpList<FactorValue> {
    return defaultArg(ProcessOutput_tryGetFactorValues_Z4A02997C(po), empty<FactorValue>());
}

/**
 * If given process output contains units, returns them
 */
export function ProcessOutput_getUnits_Z4A02997C(po: ProcessOutput_$union): FSharpList<OntologyAnnotation> {
    switch (po.tag) {
        case /* Material */ 2:
            return Material_getUnits_43A4149B(po.fields[0]);
        case /* Data */ 1:
            return empty<OntologyAnnotation>();
        default:
            return Sample_getUnits_Z23050B6A(po.fields[0]);
    }
}

export function ProcessOutput_createSample_Z445EF6B3(name: string, characteristics?: FSharpList<MaterialAttributeValue>, factors?: FSharpList<FactorValue>, derivesFrom?: FSharpList<Source>): ProcessOutput_$union {
    return ProcessOutput_Sample(Sample_create_3A6378D6(void 0, name, unwrap(characteristics), unwrap(factors), unwrap(derivesFrom)));
}

export function ProcessOutput_createMaterial_ZEED0B34(name: string, characteristics?: FSharpList<MaterialAttributeValue>, derivesFrom?: FSharpList<Material>): ProcessOutput_$union {
    return ProcessOutput_Material(Material_create_76090C97(void 0, name, void 0, unwrap(characteristics), unwrap(derivesFrom)));
}

export function ProcessOutput_createImageFile_Z721C83C5(name: string): ProcessOutput_$union {
    return ProcessOutput_Data(Data_create_Z748D099(void 0, name, DataFile_ImageFile()));
}

export function ProcessOutput_createRawData_Z721C83C5(name: string): ProcessOutput_$union {
    return ProcessOutput_Data(Data_create_Z748D099(void 0, name, DataFile_RawDataFile()));
}

export function ProcessOutput_createDerivedData_Z721C83C5(name: string): ProcessOutput_$union {
    return ProcessOutput_Data(Data_create_Z748D099(void 0, name, DataFile_DerivedDataFile()));
}

