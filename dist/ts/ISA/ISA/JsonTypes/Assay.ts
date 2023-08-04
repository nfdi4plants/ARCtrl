import { Record } from "../../../fable_modules/fable-library-ts/Types.js";
import { map as map_1, value as value_4, defaultArg, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation } from "./OntologyAnnotation.js";
import { empty, filter, map, singleton, append, exists, tryFind, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { Data_$reflection, Data } from "./Data.js";
import { AssayMaterials_get_empty, AssayMaterials_make, AssayMaterials_$reflection, AssayMaterials } from "./AssayMaterials.js";
import { MaterialAttribute_$reflection, MaterialAttribute } from "./MaterialAttribute.js";
import { Process_$reflection, Process } from "./Process.js";
import { Comment$_$reflection, Comment$ } from "./Comment.js";
import { safeHash, structuralHash, IMap, equals, IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { getRecordFields, makeRecord, record_type, list_type, option_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { Update_UpdateOptions_UpdateByExistingAppendLists, Update_UpdateOptions_$union } from "../Update.js";
import { map2 as map2_2 } from "../../../fable_modules/fable-library-ts/Array.js";
import { Dict_tryFind, Dict_ofSeqWithMerge, Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { fromValueWithDefault, mapDefault } from "../OptionExtensions.js";
import { updateProtocols, getProtocols, getFactors, getOutputsWithFactorBy, getOutputsWithCharacteristicBy, getInputsWithCharacteristicBy, getParameters, getOutputsWithParameterBy, getInputsWithParameterBy, getMaterials, getSamples, getSources, getCharacteristics, getUnits, getData } from "./ProcessSequence.js";
import { List_distinct } from "../../../fable_modules/fable-library-ts/Seq2.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { Source } from "./Source.js";
import { Sample_$reflection, Sample } from "./Sample.js";
import { Material_$reflection, Material } from "./Material.js";
import { ProcessInput_$union } from "./ProcessInput.js";
import { ProcessParameterValue } from "./ProcessParameterValue.js";
import { ProtocolParameter } from "./ProtocolParameter.js";
import { ProcessOutput_$union } from "./ProcessOutput.js";
import { MaterialAttributeValue } from "./MaterialAttributeValue.js";
import { FactorValue } from "./FactorValue.js";
import { Factor } from "./Factor.js";
import { Protocol } from "./Protocol.js";
import { toFail } from "../../../fable_modules/fable-library-ts/String.js";

export class Assay extends Record implements IEquatable<Assay> {
    readonly ID: Option<string>;
    readonly FileName: Option<string>;
    readonly MeasurementType: Option<OntologyAnnotation>;
    readonly TechnologyType: Option<OntologyAnnotation>;
    readonly TechnologyPlatform: Option<string>;
    readonly DataFiles: Option<FSharpList<Data>>;
    readonly Materials: Option<AssayMaterials>;
    readonly CharacteristicCategories: Option<FSharpList<MaterialAttribute>>;
    readonly UnitCategories: Option<FSharpList<OntologyAnnotation>>;
    readonly ProcessSequence: Option<FSharpList<Process>>;
    readonly Comments: Option<FSharpList<Comment$>>;
    constructor(ID: Option<string>, FileName: Option<string>, MeasurementType: Option<OntologyAnnotation>, TechnologyType: Option<OntologyAnnotation>, TechnologyPlatform: Option<string>, DataFiles: Option<FSharpList<Data>>, Materials: Option<AssayMaterials>, CharacteristicCategories: Option<FSharpList<MaterialAttribute>>, UnitCategories: Option<FSharpList<OntologyAnnotation>>, ProcessSequence: Option<FSharpList<Process>>, Comments: Option<FSharpList<Comment$>>) {
        super();
        this.ID = ID;
        this.FileName = FileName;
        this.MeasurementType = MeasurementType;
        this.TechnologyType = TechnologyType;
        this.TechnologyPlatform = TechnologyPlatform;
        this.DataFiles = DataFiles;
        this.Materials = Materials;
        this.CharacteristicCategories = CharacteristicCategories;
        this.UnitCategories = UnitCategories;
        this.ProcessSequence = ProcessSequence;
        this.Comments = Comments;
    }
}

export function Assay_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.Assay", [], Assay, () => [["ID", option_type(string_type)], ["FileName", option_type(string_type)], ["MeasurementType", option_type(OntologyAnnotation_$reflection())], ["TechnologyType", option_type(OntologyAnnotation_$reflection())], ["TechnologyPlatform", option_type(string_type)], ["DataFiles", option_type(list_type(Data_$reflection()))], ["Materials", option_type(AssayMaterials_$reflection())], ["CharacteristicCategories", option_type(list_type(MaterialAttribute_$reflection()))], ["UnitCategories", option_type(list_type(OntologyAnnotation_$reflection()))], ["ProcessSequence", option_type(list_type(Process_$reflection()))], ["Comments", option_type(list_type(Comment$_$reflection()))]]);
}

export function Assay_make(id: Option<string>, fileName: Option<string>, measurementType: Option<OntologyAnnotation>, technologyType: Option<OntologyAnnotation>, technologyPlatform: Option<string>, dataFiles: Option<FSharpList<Data>>, materials: Option<AssayMaterials>, characteristicCategories: Option<FSharpList<MaterialAttribute>>, unitCategories: Option<FSharpList<OntologyAnnotation>>, processSequence: Option<FSharpList<Process>>, comments: Option<FSharpList<Comment$>>): Assay {
    return new Assay(id, fileName, measurementType, technologyType, technologyPlatform, dataFiles, materials, characteristicCategories, unitCategories, processSequence, comments);
}

export function Assay_create_3D372A24(Id?: string, FileName?: string, MeasurementType?: OntologyAnnotation, TechnologyType?: OntologyAnnotation, TechnologyPlatform?: string, DataFiles?: FSharpList<Data>, Materials?: AssayMaterials, CharacteristicCategories?: FSharpList<MaterialAttribute>, UnitCategories?: FSharpList<OntologyAnnotation>, ProcessSequence?: FSharpList<Process>, Comments?: FSharpList<Comment$>): Assay {
    return Assay_make(Id, FileName, MeasurementType, TechnologyType, TechnologyPlatform, DataFiles, Materials, CharacteristicCategories, UnitCategories, ProcessSequence, Comments);
}

export function Assay_get_empty(): Assay {
    return Assay_create_3D372A24();
}

/**
 * If an assay with the given identfier exists in the list, returns it
 */
export function Assay_tryGetByFileName(fileName: string, assays: FSharpList<Assay>): Option<Assay> {
    return tryFind<Assay>((a: Assay): boolean => equals(a.FileName, fileName), assays);
}

/**
 * If an assay with the given identfier exists in the list, returns true
 */
export function Assay_existsByFileName(fileName: string, assays: FSharpList<Assay>): boolean {
    return exists<Assay>((a: Assay): boolean => equals(a.FileName, fileName), assays);
}

/**
 * Adds the given assay to the assays
 */
export function Assay_add(assays: FSharpList<Assay>, assay: Assay): FSharpList<Assay> {
    return append<Assay>(assays, singleton(assay));
}

/**
 * Updates all assays for which the predicate returns true with the given assays values
 */
export function Assay_updateBy(predicate: ((arg0: Assay) => boolean), updateOption: Update_UpdateOptions_$union, assay: Assay, assays: FSharpList<Assay>): FSharpList<Assay> {
    if (exists<Assay>(predicate, assays)) {
        return map<Assay, Assay>((a: Assay): Assay => {
            if (predicate(a)) {
                const this$: Update_UpdateOptions_$union = updateOption;
                const recordType_1: Assay = a;
                const recordType_2: Assay = assay;
                switch (this$.tag) {
                    case /* UpdateAllAppendLists */ 2:
                        return makeRecord(Assay_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Assay;
                    case /* UpdateByExisting */ 1:
                        return makeRecord(Assay_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Assay;
                    case /* UpdateByExistingAppendLists */ 3:
                        return makeRecord(Assay_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Assay;
                    default:
                        return recordType_2;
                }
            }
            else {
                return a;
            }
        }, assays);
    }
    else {
        return assays;
    }
}

/**
 * If an assay with the same fileName as the given assay exists in the study exists, updates it with the given assay values
 */
export function Assay_updateByFileName(updateOption: Update_UpdateOptions_$union, assay: Assay, assays: FSharpList<Assay>): FSharpList<Assay> {
    return Assay_updateBy((a: Assay): boolean => equals(a.FileName, assay.FileName), updateOption, assay, assays);
}

/**
 * Updates all assays with the same name as the given assay with its values
 */
export function Assay_removeByFileName(fileName: string, assays: FSharpList<Assay>): FSharpList<Assay> {
    return filter<Assay>((a: Assay): boolean => !equals(a.FileName, fileName), assays);
}

/**
 * Returns comments of an assay
 */
export function Assay_getComments_722A269D(assay: Assay): Option<FSharpList<Comment$>> {
    return assay.Comments;
}

/**
 * Applies function f on comments of an assay
 */
export function Assay_mapComments(f: ((arg0: FSharpList<Comment$>) => FSharpList<Comment$>), assay: Assay): Assay {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, assay.CharacteristicCategories, assay.UnitCategories, assay.ProcessSequence, mapDefault<FSharpList<Comment$>>(empty<Comment$>(), f, assay.Comments));
}

/**
 * Replaces comments of an assay by given comment list
 */
export function Assay_setComments(assay: Assay, comments: FSharpList<Comment$>): Assay {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, assay.CharacteristicCategories, assay.UnitCategories, assay.ProcessSequence, comments);
}

/**
 * Returns data files of an assay
 */
export function Assay_getData_722A269D(assay: Assay): FSharpList<Data> {
    const processSequenceData: FSharpList<Data> = getData(defaultArg(assay.ProcessSequence, empty<Process>()));
    const updateOptions: Update_UpdateOptions_$union = Update_UpdateOptions_UpdateByExistingAppendLists();
    const mapping = (d: Data): Option<string> => d.Name;
    const list1: FSharpList<Data> = defaultArg(assay.DataFiles, empty<Data>());
    const list2: FSharpList<Data> = processSequenceData;
    try {
        const map1: IMap<Option<string>, Data> = Dict_ofSeqWithMerge<Data, Option<string>>((arg: Data, arg_1: Data): Data => {
            const this$: Update_UpdateOptions_$union = updateOptions;
            const recordType_1: Data = arg;
            const recordType_2: Data = arg_1;
            switch (this$.tag) {
                case /* UpdateAllAppendLists */ 2:
                    return makeRecord(Data_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Data;
                case /* UpdateByExisting */ 1:
                    return makeRecord(Data_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Data;
                case /* UpdateByExistingAppendLists */ 3:
                    return makeRecord(Data_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Data;
                default:
                    return recordType_2;
            }
        }, map<Data, [Option<string>, Data]>((v: Data): [Option<string>, Data] => ([mapping(v), v] as [Option<string>, Data]), list1));
        const map2: IMap<Option<string>, Data> = Dict_ofSeqWithMerge<Data, Option<string>>((arg_2: Data, arg_3: Data): Data => {
            const this$_1: Update_UpdateOptions_$union = updateOptions;
            const recordType_1_1: Data = arg_2;
            const recordType_2_1: Data = arg_3;
            switch (this$_1.tag) {
                case /* UpdateAllAppendLists */ 2:
                    return makeRecord(Data_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1))) as Data;
                case /* UpdateByExisting */ 1:
                    return makeRecord(Data_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1))) as Data;
                case /* UpdateByExistingAppendLists */ 3:
                    return makeRecord(Data_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1))) as Data;
                default:
                    return recordType_2_1;
            }
        }, map<Data, [Option<string>, Data]>((v_1: Data): [Option<string>, Data] => ([mapping(v_1), v_1] as [Option<string>, Data]), list2));
        return map<Option<string>, Data>((k: Option<string>): Data => {
            const matchValue: Option<Data> = Dict_tryFind<Option<string>, Data>(k, map1);
            const matchValue_1: Option<Data> = Dict_tryFind<Option<string>, Data>(k, map2);
            if (matchValue == null) {
                if (matchValue_1 == null) {
                    throw new Error("If this fails, then I don\'t know how to program");
                }
                else {
                    const v2_1: Data = value_4(matchValue_1);
                    return v2_1;
                }
            }
            else if (matchValue_1 == null) {
                const v1_1: Data = value_4(matchValue);
                return v1_1;
            }
            else {
                const v1: Data = value_4(matchValue);
                const v2: Data = value_4(matchValue_1);
                const this$_2: Update_UpdateOptions_$union = updateOptions;
                const recordType_1_2: Data = v1;
                const recordType_2_2: Data = v2;
                switch (this$_2.tag) {
                    case /* UpdateAllAppendLists */ 2:
                        return makeRecord(Data_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2))) as Data;
                    case /* UpdateByExisting */ 1:
                        return makeRecord(Data_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2))) as Data;
                    case /* UpdateByExistingAppendLists */ 3:
                        return makeRecord(Data_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2))) as Data;
                    default:
                        return recordType_2_2;
                }
            }
        }, List_distinct<Option<string>>(append<Option<string>>(map<Data, Option<string>>(mapping, list1), map<Data, Option<string>>(mapping, list2)), {
            Equals: equals,
            GetHashCode: structuralHash,
        }));
    }
    catch (err: any) {
        throw new Error(`Could not mergeUpdate ${"Data"} list: 
${err.message}`);
    }
}

/**
 * Applies function f on data files of an assay
 */
export function Assay_mapData(f: ((arg0: FSharpList<Data>) => FSharpList<Data>), assay: Assay): Assay {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, mapDefault<FSharpList<Data>>(empty<Data>(), f, assay.DataFiles), assay.Materials, assay.CharacteristicCategories, assay.UnitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * Replaces data files of an assay by given data file list
 */
export function Assay_setData(assay: Assay, dataFiles: FSharpList<Data>): Assay {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, dataFiles, assay.Materials, assay.CharacteristicCategories, assay.UnitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * Returns unit categories of an assay
 */
export function Assay_getUnitCategories_722A269D(assay: Assay): FSharpList<OntologyAnnotation> {
    return List_distinct<OntologyAnnotation>(append(getUnits(defaultArg(assay.ProcessSequence, empty<Process>())), defaultArg(assay.UnitCategories, empty<OntologyAnnotation>())), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Applies function f on unit categories of an assay
 */
export function Assay_mapUnitCategories(f: ((arg0: FSharpList<OntologyAnnotation>) => FSharpList<OntologyAnnotation>), assay: Assay): Assay {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, assay.CharacteristicCategories, mapDefault<FSharpList<OntologyAnnotation>>(empty<OntologyAnnotation>(), f, assay.UnitCategories), assay.ProcessSequence, assay.Comments);
}

/**
 * Replaces unit categories of an assay by given unit categorie list
 */
export function Assay_setUnitCategories(assay: Assay, unitCategories: FSharpList<OntologyAnnotation>): Assay {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, assay.CharacteristicCategories, unitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * Returns characteristic categories of an assay
 */
export function Assay_getCharacteristics_722A269D(assay: Assay): FSharpList<MaterialAttribute> {
    return List_distinct<MaterialAttribute>(append(getCharacteristics(defaultArg(assay.ProcessSequence, empty<Process>())), defaultArg(assay.CharacteristicCategories, empty<MaterialAttribute>())), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Applies function f on characteristic categories of an assay
 */
export function Assay_mapCharacteristics(f: ((arg0: FSharpList<MaterialAttribute>) => FSharpList<MaterialAttribute>), assay: Assay): Assay {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, mapDefault<FSharpList<MaterialAttribute>>(empty<MaterialAttribute>(), f, assay.CharacteristicCategories), assay.UnitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * Replaces characteristic categories of an assay by given characteristic categorie list
 */
export function Assay_setCharacteristics(assay: Assay, characteristics: FSharpList<MaterialAttribute>): Assay {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, characteristics, assay.UnitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * Returns measurement type of an assay
 */
export function Assay_getMeasurementType_722A269D(assay: Assay): Option<OntologyAnnotation> {
    return assay.MeasurementType;
}

/**
 * Applies function f on measurement type of an assay
 */
export function Assay_mapMeasurementType(f: ((arg0: OntologyAnnotation) => OntologyAnnotation), assay: Assay): Assay {
    return new Assay(assay.ID, assay.FileName, map_1<OntologyAnnotation, OntologyAnnotation>(f, assay.MeasurementType), assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, assay.CharacteristicCategories, assay.UnitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * Replaces measurement type of an assay by given measurement type
 */
export function Assay_setMeasurementType(assay: Assay, measurementType: OntologyAnnotation): Assay {
    return new Assay(assay.ID, assay.FileName, measurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, assay.CharacteristicCategories, assay.UnitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * Returns technology type of an assay
 */
export function Assay_getTechnologyType_722A269D(assay: Assay): Option<OntologyAnnotation> {
    return assay.TechnologyType;
}

/**
 * Applies function f on technology type of an assay
 */
export function Assay_mapTechnologyType(f: ((arg0: OntologyAnnotation) => OntologyAnnotation), assay: Assay): Assay {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, map_1<OntologyAnnotation, OntologyAnnotation>(f, assay.TechnologyType), assay.TechnologyPlatform, assay.DataFiles, assay.Materials, assay.CharacteristicCategories, assay.UnitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * Replaces technology type of an assay by given technology type
 */
export function Assay_setTechnologyType(assay: Assay, technologyType: OntologyAnnotation): Assay {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, technologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, assay.CharacteristicCategories, assay.UnitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * Returns processes of an assay
 */
export function Assay_getProcesses_722A269D(assay: Assay): FSharpList<Process> {
    return defaultArg(assay.ProcessSequence, empty<Process>());
}

/**
 * Applies function f on processes of an assay
 */
export function Assay_mapProcesses(f: ((arg0: FSharpList<Process>) => FSharpList<Process>), assay: Assay): Assay {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, assay.CharacteristicCategories, assay.UnitCategories, mapDefault<FSharpList<Process>>(empty<Process>(), f, assay.ProcessSequence), assay.Comments);
}

/**
 * Replaces processes of an assay by given processe list
 */
export function Assay_setProcesses(assay: Assay, processes: FSharpList<Process>): Assay {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, assay.CharacteristicCategories, assay.UnitCategories, processes, assay.Comments);
}

export function Assay_getSources_722A269D(assay: Assay): FSharpList<Source> {
    return getSources(Assay_getProcesses_722A269D(assay));
}

export function Assay_getSamples_722A269D(assay: Assay): FSharpList<Sample> {
    return getSamples(Assay_getProcesses_722A269D(assay));
}

/**
 * Returns materials of an assay
 */
export function Assay_getMaterials_722A269D(assay: Assay): AssayMaterials {
    const processSequenceMaterials: FSharpList<Material> = getMaterials(defaultArg(assay.ProcessSequence, empty<Process>()));
    const processSequenceSamples: FSharpList<Sample> = getSamples(defaultArg(assay.ProcessSequence, empty<Process>()));
    const matchValue: Option<AssayMaterials> = assay.Materials;
    if (matchValue == null) {
        return AssayMaterials_make(fromValueWithDefault<FSharpList<Sample>>(empty<Sample>(), processSequenceSamples), fromValueWithDefault<FSharpList<Material>>(empty<Material>(), processSequenceMaterials));
    }
    else {
        const mat: AssayMaterials = value_4(matchValue);
        let samples: FSharpList<Sample>;
        const updateOptions: Update_UpdateOptions_$union = Update_UpdateOptions_UpdateByExistingAppendLists();
        const mapping = (s: Sample): Option<string> => s.Name;
        const list1: FSharpList<Sample> = defaultArg(mat.Samples, empty<Sample>());
        const list2: FSharpList<Sample> = processSequenceSamples;
        try {
            const map1: IMap<Option<string>, Sample> = Dict_ofSeqWithMerge<Sample, Option<string>>((arg: Sample, arg_1: Sample): Sample => {
                const this$: Update_UpdateOptions_$union = updateOptions;
                const recordType_1: Sample = arg;
                const recordType_2: Sample = arg_1;
                switch (this$.tag) {
                    case /* UpdateAllAppendLists */ 2:
                        return makeRecord(Sample_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Sample;
                    case /* UpdateByExisting */ 1:
                        return makeRecord(Sample_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Sample;
                    case /* UpdateByExistingAppendLists */ 3:
                        return makeRecord(Sample_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Sample;
                    default:
                        return recordType_2;
                }
            }, map<Sample, [Option<string>, Sample]>((v: Sample): [Option<string>, Sample] => ([mapping(v), v] as [Option<string>, Sample]), list1));
            const map2: IMap<Option<string>, Sample> = Dict_ofSeqWithMerge<Sample, Option<string>>((arg_2: Sample, arg_3: Sample): Sample => {
                const this$_1: Update_UpdateOptions_$union = updateOptions;
                const recordType_1_1: Sample = arg_2;
                const recordType_2_1: Sample = arg_3;
                switch (this$_1.tag) {
                    case /* UpdateAllAppendLists */ 2:
                        return makeRecord(Sample_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1))) as Sample;
                    case /* UpdateByExisting */ 1:
                        return makeRecord(Sample_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1))) as Sample;
                    case /* UpdateByExistingAppendLists */ 3:
                        return makeRecord(Sample_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1))) as Sample;
                    default:
                        return recordType_2_1;
                }
            }, map<Sample, [Option<string>, Sample]>((v_1: Sample): [Option<string>, Sample] => ([mapping(v_1), v_1] as [Option<string>, Sample]), list2));
            samples = map<Option<string>, Sample>((k: Option<string>): Sample => {
                const matchValue_1: Option<Sample> = Dict_tryFind<Option<string>, Sample>(k, map1);
                const matchValue_1_1: Option<Sample> = Dict_tryFind<Option<string>, Sample>(k, map2);
                if (matchValue_1 == null) {
                    if (matchValue_1_1 == null) {
                        throw new Error("If this fails, then I don\'t know how to program");
                    }
                    else {
                        const v2_1: Sample = value_4(matchValue_1_1);
                        return v2_1;
                    }
                }
                else if (matchValue_1_1 == null) {
                    const v1_1: Sample = value_4(matchValue_1);
                    return v1_1;
                }
                else {
                    const v1: Sample = value_4(matchValue_1);
                    const v2: Sample = value_4(matchValue_1_1);
                    const this$_2: Update_UpdateOptions_$union = updateOptions;
                    const recordType_1_2: Sample = v1;
                    const recordType_2_2: Sample = v2;
                    switch (this$_2.tag) {
                        case /* UpdateAllAppendLists */ 2:
                            return makeRecord(Sample_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2))) as Sample;
                        case /* UpdateByExisting */ 1:
                            return makeRecord(Sample_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2))) as Sample;
                        case /* UpdateByExistingAppendLists */ 3:
                            return makeRecord(Sample_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2))) as Sample;
                        default:
                            return recordType_2_2;
                    }
                }
            }, List_distinct<Option<string>>(append<Option<string>>(map<Sample, Option<string>>(mapping, list1), map<Sample, Option<string>>(mapping, list2)), {
                Equals: equals,
                GetHashCode: structuralHash,
            }));
        }
        catch (err: any) {
            throw new Error(`Could not mergeUpdate ${"Sample"} list: 
${err.message}`);
        }
        let materials: FSharpList<Material>;
        const updateOptions_1: Update_UpdateOptions_$union = Update_UpdateOptions_UpdateByExistingAppendLists();
        const mapping_5 = (m: Material): Option<string> => m.Name;
        const list1_1: FSharpList<Material> = defaultArg(mat.OtherMaterials, empty<Material>());
        const list2_1: FSharpList<Material> = processSequenceMaterials;
        try {
            const map1_1: IMap<Option<string>, Material> = Dict_ofSeqWithMerge<Material, Option<string>>((arg_6: Material, arg_1_1: Material): Material => {
                const this$_3: Update_UpdateOptions_$union = updateOptions_1;
                const recordType_1_3: Material = arg_6;
                const recordType_2_3: Material = arg_1_1;
                switch (this$_3.tag) {
                    case /* UpdateAllAppendLists */ 2:
                        return makeRecord(Material_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3))) as Material;
                    case /* UpdateByExisting */ 1:
                        return makeRecord(Material_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3))) as Material;
                    case /* UpdateByExistingAppendLists */ 3:
                        return makeRecord(Material_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3))) as Material;
                    default:
                        return recordType_2_3;
                }
            }, map<Material, [Option<string>, Material]>((v_2: Material): [Option<string>, Material] => ([mapping_5(v_2), v_2] as [Option<string>, Material]), list1_1));
            const map2_1: IMap<Option<string>, Material> = Dict_ofSeqWithMerge<Material, Option<string>>((arg_2_1: Material, arg_3_1: Material): Material => {
                const this$_4: Update_UpdateOptions_$union = updateOptions_1;
                const recordType_1_4: Material = arg_2_1;
                const recordType_2_4: Material = arg_3_1;
                switch (this$_4.tag) {
                    case /* UpdateAllAppendLists */ 2:
                        return makeRecord(Material_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4))) as Material;
                    case /* UpdateByExisting */ 1:
                        return makeRecord(Material_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4))) as Material;
                    case /* UpdateByExistingAppendLists */ 3:
                        return makeRecord(Material_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4))) as Material;
                    default:
                        return recordType_2_4;
                }
            }, map<Material, [Option<string>, Material]>((v_1_1: Material): [Option<string>, Material] => ([mapping_5(v_1_1), v_1_1] as [Option<string>, Material]), list2_1));
            materials = map<Option<string>, Material>((k_1: Option<string>): Material => {
                const matchValue_3: Option<Material> = Dict_tryFind<Option<string>, Material>(k_1, map1_1);
                const matchValue_1_2: Option<Material> = Dict_tryFind<Option<string>, Material>(k_1, map2_1);
                if (matchValue_3 == null) {
                    if (matchValue_1_2 == null) {
                        throw new Error("If this fails, then I don\'t know how to program");
                    }
                    else {
                        const v2_1_1: Material = value_4(matchValue_1_2);
                        return v2_1_1;
                    }
                }
                else if (matchValue_1_2 == null) {
                    const v1_1_1: Material = value_4(matchValue_3);
                    return v1_1_1;
                }
                else {
                    const v1_2: Material = value_4(matchValue_3);
                    const v2_2: Material = value_4(matchValue_1_2);
                    const this$_5: Update_UpdateOptions_$union = updateOptions_1;
                    const recordType_1_5: Material = v1_2;
                    const recordType_2_5: Material = v2_2;
                    switch (this$_5.tag) {
                        case /* UpdateAllAppendLists */ 2:
                            return makeRecord(Material_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5))) as Material;
                        case /* UpdateByExisting */ 1:
                            return makeRecord(Material_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5))) as Material;
                        case /* UpdateByExistingAppendLists */ 3:
                            return makeRecord(Material_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5))) as Material;
                        default:
                            return recordType_2_5;
                    }
                }
            }, List_distinct<Option<string>>(append<Option<string>>(map<Material, Option<string>>(mapping_5, list1_1), map<Material, Option<string>>(mapping_5, list2_1)), {
                Equals: equals,
                GetHashCode: structuralHash,
            }));
        }
        catch (err_1: any) {
            throw new Error(`Could not mergeUpdate ${"Material"} list: 
${err_1.message}`);
        }
        return AssayMaterials_make(fromValueWithDefault<FSharpList<Sample>>(empty<Sample>(), samples), fromValueWithDefault<FSharpList<Material>>(empty<Material>(), materials));
    }
}

/**
 * Applies function f on materials of an assay
 */
export function Assay_mapMaterials(f: ((arg0: AssayMaterials) => AssayMaterials), assay: Assay): Assay {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, map_1<AssayMaterials, AssayMaterials>(f, assay.Materials), assay.CharacteristicCategories, assay.UnitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * Replaces materials of an assay by given assay materials
 */
export function Assay_setMaterials(assay: Assay, materials: AssayMaterials): Assay {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, materials, assay.CharacteristicCategories, assay.UnitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * If the assay contains a process implementing the given parameter, return the list of input files together with their according parameter values of this parameter
 */
export function Assay_getInputsWithParameterBy(predicate: ((arg0: ProtocolParameter) => boolean), assay: Assay): Option<FSharpList<[ProcessInput_$union, ProcessParameterValue]>> {
    return map_1<FSharpList<Process>, FSharpList<[ProcessInput_$union, ProcessParameterValue]>>((processSequence: FSharpList<Process>): FSharpList<[ProcessInput_$union, ProcessParameterValue]> => getInputsWithParameterBy(predicate, processSequence), assay.ProcessSequence);
}

/**
 * If the assay contains a process implementing the given parameter, return the list of output files together with their according parameter values of this parameter
 */
export function Assay_getOutputsWithParameterBy(predicate: ((arg0: ProtocolParameter) => boolean), assay: Assay): Option<FSharpList<[ProcessOutput_$union, ProcessParameterValue]>> {
    return map_1<FSharpList<Process>, FSharpList<[ProcessOutput_$union, ProcessParameterValue]>>((processSequence: FSharpList<Process>): FSharpList<[ProcessOutput_$union, ProcessParameterValue]> => getOutputsWithParameterBy(predicate, processSequence), assay.ProcessSequence);
}

/**
 * Returns the parameters implemented by the processes contained in this assay
 */
export function Assay_getParameters_722A269D(assay: Assay): Option<FSharpList<ProtocolParameter>> {
    return map_1<FSharpList<Process>, FSharpList<ProtocolParameter>>(getParameters, assay.ProcessSequence);
}

/**
 * If the assay contains a process implementing the given parameter, return the list of input files together with their according parameter values of this parameter
 */
export function Assay_getInputsWithCharacteristicBy(predicate: ((arg0: MaterialAttribute) => boolean), assay: Assay): Option<FSharpList<[ProcessInput_$union, MaterialAttributeValue]>> {
    return map_1<FSharpList<Process>, FSharpList<[ProcessInput_$union, MaterialAttributeValue]>>((processSequence: FSharpList<Process>): FSharpList<[ProcessInput_$union, MaterialAttributeValue]> => getInputsWithCharacteristicBy(predicate, processSequence), assay.ProcessSequence);
}

/**
 * If the assay contains a process implementing the given parameter, return the list of output files together with their according parameter values of this parameter
 */
export function Assay_getOutputsWithCharacteristicBy(predicate: ((arg0: MaterialAttribute) => boolean), assay: Assay): Option<FSharpList<[ProcessOutput_$union, MaterialAttributeValue]>> {
    return map_1<FSharpList<Process>, FSharpList<[ProcessOutput_$union, MaterialAttributeValue]>>((processSequence: FSharpList<Process>): FSharpList<[ProcessOutput_$union, MaterialAttributeValue]> => getOutputsWithCharacteristicBy(predicate, processSequence), assay.ProcessSequence);
}

/**
 * If the assay contains a process implementing the given factor, return the list of output files together with their according factor values of this factor
 */
export function Assay_getOutputsWithFactorBy(predicate: ((arg0: Factor) => boolean), assay: Assay): Option<FSharpList<[ProcessOutput_$union, FactorValue]>> {
    return map_1<FSharpList<Process>, FSharpList<[ProcessOutput_$union, FactorValue]>>((processSequence: FSharpList<Process>): FSharpList<[ProcessOutput_$union, FactorValue]> => getOutputsWithFactorBy(predicate, processSequence), assay.ProcessSequence);
}

/**
 * Returns the factors implemented by the processes contained in this assay
 */
export function Assay_getFactors_722A269D(assay: Assay): FSharpList<Factor> {
    return getFactors(defaultArg(assay.ProcessSequence, empty<Process>()));
}

/**
 * Returns the protocols implemented by the processes contained in this assay
 */
export function Assay_getProtocols_722A269D(assay: Assay): FSharpList<Protocol> {
    return getProtocols(defaultArg(assay.ProcessSequence, empty<Process>()));
}

export function Assay_update_722A269D(assay: Assay): Assay {
    let v_1: AssayMaterials;
    try {
        return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, fromValueWithDefault<FSharpList<Data>>(empty<Data>(), Assay_getData_722A269D(assay)), (v_1 = Assay_getMaterials_722A269D(assay), fromValueWithDefault<AssayMaterials>(AssayMaterials_get_empty(), v_1)), fromValueWithDefault<FSharpList<MaterialAttribute>>(empty<MaterialAttribute>(), Assay_getCharacteristics_722A269D(assay)), fromValueWithDefault<FSharpList<OntologyAnnotation>>(empty<OntologyAnnotation>(), Assay_getUnitCategories_722A269D(assay)), assay.ProcessSequence, assay.Comments);
    }
    catch (err: any) {
        return toFail(`Could not update assay ${assay.FileName}: 
${err.message}`);
    }
}

export function Assay_updateProtocols(protocols: FSharpList<Protocol>, assay: Assay): Assay {
    try {
        return Assay_mapProcesses((processSequence: FSharpList<Process>): FSharpList<Process> => updateProtocols(protocols, processSequence), assay);
    }
    catch (err: any) {
        return toFail(`Could not update assay protocols ${assay.FileName}: 
${err.message}`);
    }
}

