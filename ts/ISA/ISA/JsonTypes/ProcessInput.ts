import { Union, toString } from "../../../fable_modules/fable-library-ts/Types.js";
import { Source_create_Z32235993, Source_getUnits_Z220A6393, Source_setCharacteristicValues, Source_get_empty, Source_$reflection, Source } from "./Source.js";
import { printf, toText } from "../../../fable_modules/fable-library-ts/String.js";
import { Material_create_76090C97, Material_getUnits_43A4149B, Material_setCharacteristicValues, Material_$reflection, Material } from "./Material.js";
import { Data_create_Z748D099, Data_$reflection, Data } from "./Data.js";
import { Sample_create_3A6378D6, Sample_getUnits_Z23050B6A, Sample_setCharacteristicValues, Sample_$reflection, Sample } from "./Sample.js";
import { IISAPrintable } from "../Printer.js";
import { union_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { unwrap, map, defaultArg, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { empty, choose, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { MaterialAttributeValue } from "./MaterialAttributeValue.js";
import { MaterialAttribute } from "./MaterialAttribute.js";
import { OntologyAnnotation } from "./OntologyAnnotation.js";
import { FactorValue } from "./FactorValue.js";
import { DataFile_DerivedDataFile, DataFile_RawDataFile, DataFile_ImageFile } from "./DataFile.js";

export type ProcessInput_$union = 
    | ProcessInput<0>
    | ProcessInput<1>
    | ProcessInput<2>
    | ProcessInput<3>

export type ProcessInput_$cases = {
    0: ["Source", [Source]],
    1: ["Sample", [Sample]],
    2: ["Data", [Data]],
    3: ["Material", [Material]]
}

export function ProcessInput_Source(Item: Source) {
    return new ProcessInput<0>(0, [Item]);
}

export function ProcessInput_Sample(Item: Sample) {
    return new ProcessInput<1>(1, [Item]);
}

export function ProcessInput_Data(Item: Data) {
    return new ProcessInput<2>(2, [Item]);
}

export function ProcessInput_Material(Item: Material) {
    return new ProcessInput<3>(3, [Item]);
}

export class ProcessInput<Tag extends keyof ProcessInput_$cases> extends Union<Tag, ProcessInput_$cases[Tag][0]> implements IISAPrintable {
    constructor(readonly tag: Tag, readonly fields: ProcessInput_$cases[Tag][1]) {
        super();
    }
    cases() {
        return ["Source", "Sample", "Data", "Material"];
    }
    Print(): string {
        const this$ = this as ProcessInput_$union;
        return toString(this$);
    }
    PrintCompact(): string {
        const this$ = this as ProcessInput_$union;
        switch (this$.tag) {
            case /* Source */ 0: {
                const s_1: Source = this$.fields[0];
                const arg_1: string = s_1.PrintCompact();
                return toText(printf("Source {%s}"))(arg_1);
            }
            case /* Material */ 3: {
                const m: Material = this$.fields[0];
                const arg_2: string = m.PrintCompact();
                return toText(printf("Material {%s}"))(arg_2);
            }
            case /* Data */ 2: {
                const d: Data = this$.fields[0];
                const arg_3: string = d.PrintCompact();
                return toText(printf("Data {%s}"))(arg_3);
            }
            default: {
                const s: Sample = this$.fields[0];
                const arg: string = s.PrintCompact();
                return toText(printf("Sample {%s}"))(arg);
            }
        }
    }
}

export function ProcessInput_$reflection(): TypeInfo {
    return union_type("ISA.ProcessInput", [], ProcessInput, () => [[["Item", Source_$reflection()]], [["Item", Sample_$reflection()]], [["Item", Data_$reflection()]], [["Item", Material_$reflection()]]]);
}

export function ProcessInput__get_TryName(this$: ProcessInput_$union): Option<string> {
    switch (this$.tag) {
        case /* Source */ 0:
            return this$.fields[0].Name;
        case /* Material */ 3:
            return this$.fields[0].Name;
        case /* Data */ 2:
            return this$.fields[0].Name;
        default:
            return this$.fields[0].Name;
    }
}

export function ProcessInput__get_Name(this$: ProcessInput_$union): string {
    return defaultArg(ProcessInput__get_TryName(this$), "");
}

export function ProcessInput_get_Default(): ProcessInput_$union {
    return ProcessInput_Source(Source_get_empty());
}

/**
 * Returns name of processInput
 */
export function ProcessInput_tryGetName_Z38E7E853(pi: ProcessInput_$union): Option<string> {
    return ProcessInput__get_TryName(pi);
}

/**
 * Returns name of processInput
 */
export function ProcessInput_getName_Z38E7E853(pi: ProcessInput_$union): string {
    return ProcessInput__get_Name(pi);
}

/**
 * Returns true, if given name equals name of processInput
 */
export function ProcessInput_nameEquals(name: string, pi: ProcessInput_$union): boolean {
    return ProcessInput__get_Name(pi) === name;
}

/**
 * Returns true, if Process Input is Sample
 */
export function ProcessInput_isSample_Z38E7E853(pi: ProcessInput_$union): boolean {
    if (pi.tag === /* Sample */ 1) {
        return true;
    }
    else {
        return false;
    }
}

/**
 * Returns true, if Process Input is Source
 */
export function ProcessInput_isSource_Z38E7E853(pi: ProcessInput_$union): boolean {
    if (pi.tag === /* Source */ 0) {
        return true;
    }
    else {
        return false;
    }
}

/**
 * Returns true, if Process Input is Data
 */
export function ProcessInput_isData_Z38E7E853(pi: ProcessInput_$union): boolean {
    if (pi.tag === /* Data */ 2) {
        return true;
    }
    else {
        return false;
    }
}

/**
 * Returns true, if Process Input is Material
 */
export function ProcessInput_isMaterial_Z38E7E853(pi: ProcessInput_$union): boolean {
    if (pi.tag === /* Material */ 3) {
        return true;
    }
    else {
        return false;
    }
}

/**
 * Returns true, if Process Input is Source
 */
export function ProcessInput__isSource(this$: ProcessInput_$union): boolean {
    return ProcessInput_isSource_Z38E7E853(this$);
}

/**
 * Returns true, if Process Input is Sample
 */
export function ProcessInput__isSample(this$: ProcessInput_$union): boolean {
    return ProcessInput_isSample_Z38E7E853(this$);
}

/**
 * Returns true, if Process Input is Data
 */
export function ProcessInput__isData(this$: ProcessInput_$union): boolean {
    return ProcessInput_isData_Z38E7E853(this$);
}

/**
 * Returns true, if Process Input is Material
 */
export function ProcessInput__isMaterial(this$: ProcessInput_$union): boolean {
    return ProcessInput_isMaterial_Z38E7E853(this$);
}

/**
 * If given process input is a sample, returns it, else returns None
 */
export function ProcessInput_trySample_Z38E7E853(pi: ProcessInput_$union): Option<Sample> {
    if (pi.tag === /* Sample */ 1) {
        return pi.fields[0];
    }
    else {
        return void 0;
    }
}

/**
 * If given process input is a source, returns it, else returns None
 */
export function ProcessInput_trySource_Z38E7E853(pi: ProcessInput_$union): Option<Source> {
    if (pi.tag === /* Source */ 0) {
        return pi.fields[0];
    }
    else {
        return void 0;
    }
}

/**
 * If given process input is a data, returns it, else returns None
 */
export function ProcessInput_tryData_Z38E7E853(pi: ProcessInput_$union): Option<Data> {
    if (pi.tag === /* Data */ 2) {
        return pi.fields[0];
    }
    else {
        return void 0;
    }
}

/**
 * If given process input is a material, returns it, else returns None
 */
export function ProcessInput_tryMaterial_Z38E7E853(pi: ProcessInput_$union): Option<Material> {
    if (pi.tag === /* Material */ 3) {
        return pi.fields[0];
    }
    else {
        return void 0;
    }
}

export function ProcessInput_setCharacteristicValues(characteristics: FSharpList<MaterialAttributeValue>, pi: ProcessInput_$union): ProcessInput_$union {
    switch (pi.tag) {
        case /* Source */ 0:
            return ProcessInput_Source(Source_setCharacteristicValues(characteristics, pi.fields[0]));
        case /* Material */ 3:
            return ProcessInput_Material(Material_setCharacteristicValues(characteristics, pi.fields[0]));
        case /* Data */ 2:
            return pi;
        default:
            return ProcessInput_Sample(Sample_setCharacteristicValues(characteristics, pi.fields[0]));
    }
}

/**
 * If given process input contains characteristics, returns them
 */
export function ProcessInput_tryGetCharacteristicValues_Z38E7E853(pi: ProcessInput_$union): Option<FSharpList<MaterialAttributeValue>> {
    switch (pi.tag) {
        case /* Source */ 0:
            return pi.fields[0].Characteristics;
        case /* Material */ 3:
            return pi.fields[0].Characteristics;
        case /* Data */ 2:
            return void 0;
        default:
            return pi.fields[0].Characteristics;
    }
}

/**
 * If given process input contains characteristics, returns them
 */
export function ProcessInput_tryGetCharacteristics_Z38E7E853(pi: ProcessInput_$union): Option<FSharpList<MaterialAttribute>> {
    return map<FSharpList<MaterialAttributeValue>, FSharpList<MaterialAttribute>>((list: FSharpList<MaterialAttributeValue>): FSharpList<MaterialAttribute> => choose<MaterialAttributeValue, MaterialAttribute>((c: MaterialAttributeValue): Option<MaterialAttribute> => c.Category, list), ProcessInput_tryGetCharacteristicValues_Z38E7E853(pi));
}

export function ProcessInput_getCharacteristicValues_Z38E7E853(pi: ProcessInput_$union): FSharpList<MaterialAttributeValue> {
    return defaultArg(ProcessInput_tryGetCharacteristicValues_Z38E7E853(pi), empty<MaterialAttributeValue>());
}

/**
 * If given process output contains units, returns them
 */
export function ProcessInput_getUnits_Z38E7E853(pi: ProcessInput_$union): FSharpList<OntologyAnnotation> {
    switch (pi.tag) {
        case /* Sample */ 1:
            return Sample_getUnits_Z23050B6A(pi.fields[0]);
        case /* Material */ 3:
            return Material_getUnits_43A4149B(pi.fields[0]);
        case /* Data */ 2:
            return empty<OntologyAnnotation>();
        default:
            return Source_getUnits_Z220A6393(pi.fields[0]);
    }
}

export function ProcessInput_createSource_Z3083890A(name: string, characteristics?: FSharpList<MaterialAttributeValue>): ProcessInput_$union {
    return ProcessInput_Source(Source_create_Z32235993(void 0, name, unwrap(characteristics)));
}

export function ProcessInput_createSample_Z445EF6B3(name: string, characteristics?: FSharpList<MaterialAttributeValue>, factors?: FSharpList<FactorValue>, derivesFrom?: FSharpList<Source>): ProcessInput_$union {
    return ProcessInput_Sample(Sample_create_3A6378D6(void 0, name, unwrap(characteristics), unwrap(factors), unwrap(derivesFrom)));
}

export function ProcessInput_createMaterial_ZEED0B34(name: string, characteristics?: FSharpList<MaterialAttributeValue>, derivesFrom?: FSharpList<Material>): ProcessInput_$union {
    return ProcessInput_Material(Material_create_76090C97(void 0, name, void 0, unwrap(characteristics), unwrap(derivesFrom)));
}

export function ProcessInput_createImageFile_Z721C83C5(name: string): ProcessInput_$union {
    return ProcessInput_Data(Data_create_Z748D099(void 0, name, DataFile_ImageFile()));
}

export function ProcessInput_createRawData_Z721C83C5(name: string): ProcessInput_$union {
    return ProcessInput_Data(Data_create_Z748D099(void 0, name, DataFile_RawDataFile()));
}

export function ProcessInput_createDerivedData_Z721C83C5(name: string): ProcessInput_$union {
    return ProcessInput_Data(Data_create_Z748D099(void 0, name, DataFile_DerivedDataFile()));
}

