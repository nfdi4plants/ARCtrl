import { Record } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { getRecordFields, makeRecord, record_type, list_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { OntologyAnnotation_$reflection } from "./OntologyAnnotation.js";
import { Data_$reflection } from "./Data.js";
import { AssayMaterials_get_empty, AssayMaterials_make, AssayMaterials_$reflection } from "./AssayMaterials.js";
import { MaterialAttribute_$reflection } from "./MaterialAttribute.js";
import { Process_$reflection } from "./Process.js";
import { Comment$_$reflection } from "./Comment.js";
import { empty, filter, map, singleton, append, exists, tryFind } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { safeHash, structuralHash, equals } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { map2 as map2_2 } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { Dict_tryFind, Dict_ofSeqWithMerge, Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { fromValueWithDefault, mapDefault } from "../OptionExtensions.js";
import { updateProtocols, getProtocols, getFactors, getOutputsWithFactorBy, getOutputsWithCharacteristicBy, getInputsWithCharacteristicBy, getParameters, getOutputsWithParameterBy, getInputsWithParameterBy, getMaterials, getSamples, getSources, getCharacteristics, getUnits, getData } from "./ProcessSequence.js";
import { map as map_1, defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { Update_UpdateOptions } from "../Update.js";
import { List_distinct } from "../../../fable_modules/fable-library.4.1.4/Seq2.js";
import { Sample_$reflection } from "./Sample.js";
import { Material_$reflection } from "./Material.js";
import { toFail } from "../../../fable_modules/fable-library.4.1.4/String.js";

export class Assay extends Record {
    constructor(ID, FileName, MeasurementType, TechnologyType, TechnologyPlatform, DataFiles, Materials, CharacteristicCategories, UnitCategories, ProcessSequence, Comments) {
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

export function Assay_$reflection() {
    return record_type("ARCtrl.ISA.Assay", [], Assay, () => [["ID", option_type(string_type)], ["FileName", option_type(string_type)], ["MeasurementType", option_type(OntologyAnnotation_$reflection())], ["TechnologyType", option_type(OntologyAnnotation_$reflection())], ["TechnologyPlatform", option_type(string_type)], ["DataFiles", option_type(list_type(Data_$reflection()))], ["Materials", option_type(AssayMaterials_$reflection())], ["CharacteristicCategories", option_type(list_type(MaterialAttribute_$reflection()))], ["UnitCategories", option_type(list_type(OntologyAnnotation_$reflection()))], ["ProcessSequence", option_type(list_type(Process_$reflection()))], ["Comments", option_type(list_type(Comment$_$reflection()))]]);
}

export function Assay_make(id, fileName, measurementType, technologyType, technologyPlatform, dataFiles, materials, characteristicCategories, unitCategories, processSequence, comments) {
    return new Assay(id, fileName, measurementType, technologyType, technologyPlatform, dataFiles, materials, characteristicCategories, unitCategories, processSequence, comments);
}

export function Assay_create_3D372A24(Id, FileName, MeasurementType, TechnologyType, TechnologyPlatform, DataFiles, Materials, CharacteristicCategories, UnitCategories, ProcessSequence, Comments) {
    return Assay_make(Id, FileName, MeasurementType, TechnologyType, TechnologyPlatform, DataFiles, Materials, CharacteristicCategories, UnitCategories, ProcessSequence, Comments);
}

export function Assay_get_empty() {
    return Assay_create_3D372A24();
}

/**
 * If an assay with the given identfier exists in the list, returns it
 */
export function Assay_tryGetByFileName(fileName, assays) {
    return tryFind((a) => equals(a.FileName, fileName), assays);
}

/**
 * If an assay with the given identfier exists in the list, returns true
 */
export function Assay_existsByFileName(fileName, assays) {
    return exists((a) => equals(a.FileName, fileName), assays);
}

/**
 * Adds the given assay to the assays
 */
export function Assay_add(assays, assay) {
    return append(assays, singleton(assay));
}

/**
 * Updates all assays for which the predicate returns true with the given assays values
 */
export function Assay_updateBy(predicate, updateOption, assay, assays) {
    if (exists(predicate, assays)) {
        return map((a) => {
            if (predicate(a)) {
                const this$ = updateOption;
                const recordType_1 = a;
                const recordType_2 = assay;
                switch (this$.tag) {
                    case 2:
                        return makeRecord(Assay_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                    case 1:
                        return makeRecord(Assay_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                    case 3:
                        return makeRecord(Assay_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
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
export function Assay_updateByFileName(updateOption, assay, assays) {
    return Assay_updateBy((a) => equals(a.FileName, assay.FileName), updateOption, assay, assays);
}

/**
 * Updates all assays with the same name as the given assay with its values
 */
export function Assay_removeByFileName(fileName, assays) {
    return filter((a) => !equals(a.FileName, fileName), assays);
}

/**
 * Returns comments of an assay
 */
export function Assay_getComments_722A269D(assay) {
    return assay.Comments;
}

/**
 * Applies function f on comments of an assay
 */
export function Assay_mapComments(f, assay) {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, assay.CharacteristicCategories, assay.UnitCategories, assay.ProcessSequence, mapDefault(empty(), f, assay.Comments));
}

/**
 * Replaces comments of an assay by given comment list
 */
export function Assay_setComments(assay, comments) {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, assay.CharacteristicCategories, assay.UnitCategories, assay.ProcessSequence, comments);
}

/**
 * Returns data files of an assay
 */
export function Assay_getData_722A269D(assay) {
    const processSequenceData = getData(defaultArg(assay.ProcessSequence, empty()));
    const updateOptions = new Update_UpdateOptions(3, []);
    const mapping = (d) => d.Name;
    const list1 = defaultArg(assay.DataFiles, empty());
    const list2 = processSequenceData;
    try {
        const map1 = Dict_ofSeqWithMerge((arg, arg_1) => {
            const this$ = updateOptions;
            const recordType_1 = arg;
            const recordType_2 = arg_1;
            switch (this$.tag) {
                case 2:
                    return makeRecord(Data_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                case 1:
                    return makeRecord(Data_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                case 3:
                    return makeRecord(Data_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                default:
                    return recordType_2;
            }
        }, map((v) => [mapping(v), v], list1));
        const map2 = Dict_ofSeqWithMerge((arg_2, arg_3) => {
            const this$_1 = updateOptions;
            const recordType_1_1 = arg_2;
            const recordType_2_1 = arg_3;
            switch (this$_1.tag) {
                case 2:
                    return makeRecord(Data_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1)));
                case 1:
                    return makeRecord(Data_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1)));
                case 3:
                    return makeRecord(Data_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1)));
                default:
                    return recordType_2_1;
            }
        }, map((v_1) => [mapping(v_1), v_1], list2));
        return map((k) => {
            const matchValue = Dict_tryFind(k, map1);
            const matchValue_1 = Dict_tryFind(k, map2);
            if (matchValue == null) {
                if (matchValue_1 == null) {
                    throw new Error("If this fails, then I don\'t know how to program");
                }
                else {
                    const v2_1 = matchValue_1;
                    return v2_1;
                }
            }
            else if (matchValue_1 == null) {
                const v1_1 = matchValue;
                return v1_1;
            }
            else {
                const v1 = matchValue;
                const v2 = matchValue_1;
                const this$_2 = updateOptions;
                const recordType_1_2 = v1;
                const recordType_2_2 = v2;
                switch (this$_2.tag) {
                    case 2:
                        return makeRecord(Data_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2)));
                    case 1:
                        return makeRecord(Data_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2)));
                    case 3:
                        return makeRecord(Data_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2)));
                    default:
                        return recordType_2_2;
                }
            }
        }, List_distinct(append(map(mapping, list1), map(mapping, list2)), {
            Equals: equals,
            GetHashCode: structuralHash,
        }));
    }
    catch (err) {
        throw new Error(`Could not mergeUpdate ${"Data"} list: 
${err.message}`);
    }
}

/**
 * Applies function f on data files of an assay
 */
export function Assay_mapData(f, assay) {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, mapDefault(empty(), f, assay.DataFiles), assay.Materials, assay.CharacteristicCategories, assay.UnitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * Replaces data files of an assay by given data file list
 */
export function Assay_setData(assay, dataFiles) {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, dataFiles, assay.Materials, assay.CharacteristicCategories, assay.UnitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * Returns unit categories of an assay
 */
export function Assay_getUnitCategories_722A269D(assay) {
    return List_distinct(append(getUnits(defaultArg(assay.ProcessSequence, empty())), defaultArg(assay.UnitCategories, empty())), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Applies function f on unit categories of an assay
 */
export function Assay_mapUnitCategories(f, assay) {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, assay.CharacteristicCategories, mapDefault(empty(), f, assay.UnitCategories), assay.ProcessSequence, assay.Comments);
}

/**
 * Replaces unit categories of an assay by given unit categorie list
 */
export function Assay_setUnitCategories(assay, unitCategories) {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, assay.CharacteristicCategories, unitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * Returns characteristic categories of an assay
 */
export function Assay_getCharacteristics_722A269D(assay) {
    return List_distinct(append(getCharacteristics(defaultArg(assay.ProcessSequence, empty())), defaultArg(assay.CharacteristicCategories, empty())), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Applies function f on characteristic categories of an assay
 */
export function Assay_mapCharacteristics(f, assay) {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, mapDefault(empty(), f, assay.CharacteristicCategories), assay.UnitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * Replaces characteristic categories of an assay by given characteristic categorie list
 */
export function Assay_setCharacteristics(assay, characteristics) {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, characteristics, assay.UnitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * Returns measurement type of an assay
 */
export function Assay_getMeasurementType_722A269D(assay) {
    return assay.MeasurementType;
}

/**
 * Applies function f on measurement type of an assay
 */
export function Assay_mapMeasurementType(f, assay) {
    return new Assay(assay.ID, assay.FileName, map_1(f, assay.MeasurementType), assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, assay.CharacteristicCategories, assay.UnitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * Replaces measurement type of an assay by given measurement type
 */
export function Assay_setMeasurementType(assay, measurementType) {
    return new Assay(assay.ID, assay.FileName, measurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, assay.CharacteristicCategories, assay.UnitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * Returns technology type of an assay
 */
export function Assay_getTechnologyType_722A269D(assay) {
    return assay.TechnologyType;
}

/**
 * Applies function f on technology type of an assay
 */
export function Assay_mapTechnologyType(f, assay) {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, map_1(f, assay.TechnologyType), assay.TechnologyPlatform, assay.DataFiles, assay.Materials, assay.CharacteristicCategories, assay.UnitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * Replaces technology type of an assay by given technology type
 */
export function Assay_setTechnologyType(assay, technologyType) {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, technologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, assay.CharacteristicCategories, assay.UnitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * Returns processes of an assay
 */
export function Assay_getProcesses_722A269D(assay) {
    return defaultArg(assay.ProcessSequence, empty());
}

/**
 * Applies function f on processes of an assay
 */
export function Assay_mapProcesses(f, assay) {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, assay.CharacteristicCategories, assay.UnitCategories, mapDefault(empty(), f, assay.ProcessSequence), assay.Comments);
}

/**
 * Replaces processes of an assay by given processe list
 */
export function Assay_setProcesses(assay, processes) {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, assay.Materials, assay.CharacteristicCategories, assay.UnitCategories, processes, assay.Comments);
}

export function Assay_getSources_722A269D(assay) {
    return getSources(Assay_getProcesses_722A269D(assay));
}

export function Assay_getSamples_722A269D(assay) {
    return getSamples(Assay_getProcesses_722A269D(assay));
}

/**
 * Returns materials of an assay
 */
export function Assay_getMaterials_722A269D(assay) {
    const processSequenceMaterials = getMaterials(defaultArg(assay.ProcessSequence, empty()));
    const processSequenceSamples = getSamples(defaultArg(assay.ProcessSequence, empty()));
    const matchValue = assay.Materials;
    if (matchValue == null) {
        return AssayMaterials_make(fromValueWithDefault(empty(), processSequenceSamples), fromValueWithDefault(empty(), processSequenceMaterials));
    }
    else {
        const mat = matchValue;
        let samples;
        const updateOptions = new Update_UpdateOptions(3, []);
        const mapping = (s) => s.Name;
        const list1 = defaultArg(mat.Samples, empty());
        const list2 = processSequenceSamples;
        try {
            const map1 = Dict_ofSeqWithMerge((arg, arg_1) => {
                const this$ = updateOptions;
                const recordType_1 = arg;
                const recordType_2 = arg_1;
                switch (this$.tag) {
                    case 2:
                        return makeRecord(Sample_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                    case 1:
                        return makeRecord(Sample_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                    case 3:
                        return makeRecord(Sample_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                    default:
                        return recordType_2;
                }
            }, map((v) => [mapping(v), v], list1));
            const map2 = Dict_ofSeqWithMerge((arg_2, arg_3) => {
                const this$_1 = updateOptions;
                const recordType_1_1 = arg_2;
                const recordType_2_1 = arg_3;
                switch (this$_1.tag) {
                    case 2:
                        return makeRecord(Sample_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1)));
                    case 1:
                        return makeRecord(Sample_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1)));
                    case 3:
                        return makeRecord(Sample_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1)));
                    default:
                        return recordType_2_1;
                }
            }, map((v_1) => [mapping(v_1), v_1], list2));
            samples = map((k) => {
                const matchValue_1 = Dict_tryFind(k, map1);
                const matchValue_1_1 = Dict_tryFind(k, map2);
                if (matchValue_1 == null) {
                    if (matchValue_1_1 == null) {
                        throw new Error("If this fails, then I don\'t know how to program");
                    }
                    else {
                        const v2_1 = matchValue_1_1;
                        return v2_1;
                    }
                }
                else if (matchValue_1_1 == null) {
                    const v1_1 = matchValue_1;
                    return v1_1;
                }
                else {
                    const v1 = matchValue_1;
                    const v2 = matchValue_1_1;
                    const this$_2 = updateOptions;
                    const recordType_1_2 = v1;
                    const recordType_2_2 = v2;
                    switch (this$_2.tag) {
                        case 2:
                            return makeRecord(Sample_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2)));
                        case 1:
                            return makeRecord(Sample_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2)));
                        case 3:
                            return makeRecord(Sample_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2)));
                        default:
                            return recordType_2_2;
                    }
                }
            }, List_distinct(append(map(mapping, list1), map(mapping, list2)), {
                Equals: equals,
                GetHashCode: structuralHash,
            }));
        }
        catch (err) {
            throw new Error(`Could not mergeUpdate ${"Sample"} list: 
${err.message}`);
        }
        let materials;
        const updateOptions_1 = new Update_UpdateOptions(3, []);
        const mapping_5 = (m) => m.Name;
        const list1_1 = defaultArg(mat.OtherMaterials, empty());
        const list2_1 = processSequenceMaterials;
        try {
            const map1_1 = Dict_ofSeqWithMerge((arg_6, arg_1_1) => {
                const this$_3 = updateOptions_1;
                const recordType_1_3 = arg_6;
                const recordType_2_3 = arg_1_1;
                switch (this$_3.tag) {
                    case 2:
                        return makeRecord(Material_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3)));
                    case 1:
                        return makeRecord(Material_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3)));
                    case 3:
                        return makeRecord(Material_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3)));
                    default:
                        return recordType_2_3;
                }
            }, map((v_2) => [mapping_5(v_2), v_2], list1_1));
            const map2_1 = Dict_ofSeqWithMerge((arg_2_1, arg_3_1) => {
                const this$_4 = updateOptions_1;
                const recordType_1_4 = arg_2_1;
                const recordType_2_4 = arg_3_1;
                switch (this$_4.tag) {
                    case 2:
                        return makeRecord(Material_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4)));
                    case 1:
                        return makeRecord(Material_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4)));
                    case 3:
                        return makeRecord(Material_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4)));
                    default:
                        return recordType_2_4;
                }
            }, map((v_1_1) => [mapping_5(v_1_1), v_1_1], list2_1));
            materials = map((k_1) => {
                const matchValue_3 = Dict_tryFind(k_1, map1_1);
                const matchValue_1_2 = Dict_tryFind(k_1, map2_1);
                if (matchValue_3 == null) {
                    if (matchValue_1_2 == null) {
                        throw new Error("If this fails, then I don\'t know how to program");
                    }
                    else {
                        const v2_1_1 = matchValue_1_2;
                        return v2_1_1;
                    }
                }
                else if (matchValue_1_2 == null) {
                    const v1_1_1 = matchValue_3;
                    return v1_1_1;
                }
                else {
                    const v1_2 = matchValue_3;
                    const v2_2 = matchValue_1_2;
                    const this$_5 = updateOptions_1;
                    const recordType_1_5 = v1_2;
                    const recordType_2_5 = v2_2;
                    switch (this$_5.tag) {
                        case 2:
                            return makeRecord(Material_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5)));
                        case 1:
                            return makeRecord(Material_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5)));
                        case 3:
                            return makeRecord(Material_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5)));
                        default:
                            return recordType_2_5;
                    }
                }
            }, List_distinct(append(map(mapping_5, list1_1), map(mapping_5, list2_1)), {
                Equals: equals,
                GetHashCode: structuralHash,
            }));
        }
        catch (err_1) {
            throw new Error(`Could not mergeUpdate ${"Material"} list: 
${err_1.message}`);
        }
        return AssayMaterials_make(fromValueWithDefault(empty(), samples), fromValueWithDefault(empty(), materials));
    }
}

/**
 * Applies function f on materials of an assay
 */
export function Assay_mapMaterials(f, assay) {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, map_1(f, assay.Materials), assay.CharacteristicCategories, assay.UnitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * Replaces materials of an assay by given assay materials
 */
export function Assay_setMaterials(assay, materials) {
    return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, assay.DataFiles, materials, assay.CharacteristicCategories, assay.UnitCategories, assay.ProcessSequence, assay.Comments);
}

/**
 * If the assay contains a process implementing the given parameter, return the list of input files together with their according parameter values of this parameter
 */
export function Assay_getInputsWithParameterBy(predicate, assay) {
    return map_1((processSequence) => getInputsWithParameterBy(predicate, processSequence), assay.ProcessSequence);
}

/**
 * If the assay contains a process implementing the given parameter, return the list of output files together with their according parameter values of this parameter
 */
export function Assay_getOutputsWithParameterBy(predicate, assay) {
    return map_1((processSequence) => getOutputsWithParameterBy(predicate, processSequence), assay.ProcessSequence);
}

/**
 * Returns the parameters implemented by the processes contained in this assay
 */
export function Assay_getParameters_722A269D(assay) {
    return map_1(getParameters, assay.ProcessSequence);
}

/**
 * If the assay contains a process implementing the given parameter, return the list of input files together with their according parameter values of this parameter
 */
export function Assay_getInputsWithCharacteristicBy(predicate, assay) {
    return map_1((processSequence) => getInputsWithCharacteristicBy(predicate, processSequence), assay.ProcessSequence);
}

/**
 * If the assay contains a process implementing the given parameter, return the list of output files together with their according parameter values of this parameter
 */
export function Assay_getOutputsWithCharacteristicBy(predicate, assay) {
    return map_1((processSequence) => getOutputsWithCharacteristicBy(predicate, processSequence), assay.ProcessSequence);
}

/**
 * If the assay contains a process implementing the given factor, return the list of output files together with their according factor values of this factor
 */
export function Assay_getOutputsWithFactorBy(predicate, assay) {
    return map_1((processSequence) => getOutputsWithFactorBy(predicate, processSequence), assay.ProcessSequence);
}

/**
 * Returns the factors implemented by the processes contained in this assay
 */
export function Assay_getFactors_722A269D(assay) {
    return getFactors(defaultArg(assay.ProcessSequence, empty()));
}

/**
 * Returns the protocols implemented by the processes contained in this assay
 */
export function Assay_getProtocols_722A269D(assay) {
    return getProtocols(defaultArg(assay.ProcessSequence, empty()));
}

export function Assay_update_722A269D(assay) {
    let v_1;
    try {
        return new Assay(assay.ID, assay.FileName, assay.MeasurementType, assay.TechnologyType, assay.TechnologyPlatform, fromValueWithDefault(empty(), Assay_getData_722A269D(assay)), (v_1 = Assay_getMaterials_722A269D(assay), fromValueWithDefault(AssayMaterials_get_empty(), v_1)), fromValueWithDefault(empty(), Assay_getCharacteristics_722A269D(assay)), fromValueWithDefault(empty(), Assay_getUnitCategories_722A269D(assay)), assay.ProcessSequence, assay.Comments);
    }
    catch (err) {
        return toFail(`Could not update assay ${assay.FileName}: 
${err.message}`);
    }
}

export function Assay_updateProtocols(protocols, assay) {
    try {
        return Assay_mapProcesses((processSequence) => updateProtocols(protocols, processSequence), assay);
    }
    catch (err) {
        return toFail(`Could not update assay protocols ${assay.FileName}: 
${err.message}`);
    }
}

