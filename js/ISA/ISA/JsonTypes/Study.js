import { Record } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { getRecordFields, makeRecord, record_type, list_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { Publication_$reflection } from "./Publication.js";
import { Person_$reflection } from "./Person.js";
import { OntologyAnnotation_$reflection } from "./OntologyAnnotation.js";
import { Protocol_$reflection } from "./Protocol.js";
import { StudyMaterials_get_empty, StudyMaterials_make, StudyMaterials_$reflection } from "./StudyMaterials.js";
import { Process_$reflection } from "./Process.js";
import { Assay_update_Z269B5B97, Assay_updateProtocols, Assay_getMaterials_Z269B5B97, Assay_getSamples_Z269B5B97, Assay_getSources_Z269B5B97, Assay_getUnitCategories_Z269B5B97, Assay_getFactors_Z269B5B97, Assay_getCharacteristics_Z269B5B97, Assay_getProtocols_Z269B5B97, Assay_$reflection } from "./Assay.js";
import { Factor_$reflection } from "./Factor.js";
import { MaterialAttribute_$reflection } from "./MaterialAttribute.js";
import { Comment$_$reflection } from "./Comment.js";
import { collect, empty, filter, map, singleton, append, exists } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { stringHash, safeHash, structuralHash, equals } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { map2 as map2_2 } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { Dict_tryFind, Dict_ofSeqWithMerge, Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { map as map_1, defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { fromValueWithDefault, mapDefault } from "../OptionExtensions.js";
import { updateProtocols, getMaterials, getSamples, getSources, getUnits, getFactors, getCharacteristics, getProtocols } from "./ProcessSequence.js";
import { Update_UpdateOptions } from "../Update.js";
import { List_distinct } from "../../../fable_modules/fable-library.4.1.4/Seq2.js";
import { Source_$reflection } from "./Source.js";
import { Sample_$reflection } from "./Sample.js";
import { AssayMaterials_getMaterials_E3447B1 } from "./AssayMaterials.js";
import { Material_$reflection } from "./Material.js";
import { toFail } from "../../../fable_modules/fable-library.4.1.4/String.js";

export class Study extends Record {
    constructor(ID, FileName, Identifier, Title, Description, SubmissionDate, PublicReleaseDate, Publications, Contacts, StudyDesignDescriptors, Protocols, Materials, ProcessSequence, Assays, Factors, CharacteristicCategories, UnitCategories, Comments) {
        super();
        this.ID = ID;
        this.FileName = FileName;
        this.Identifier = Identifier;
        this.Title = Title;
        this.Description = Description;
        this.SubmissionDate = SubmissionDate;
        this.PublicReleaseDate = PublicReleaseDate;
        this.Publications = Publications;
        this.Contacts = Contacts;
        this.StudyDesignDescriptors = StudyDesignDescriptors;
        this.Protocols = Protocols;
        this.Materials = Materials;
        this.ProcessSequence = ProcessSequence;
        this.Assays = Assays;
        this.Factors = Factors;
        this.CharacteristicCategories = CharacteristicCategories;
        this.UnitCategories = UnitCategories;
        this.Comments = Comments;
    }
}

export function Study_$reflection() {
    return record_type("ISA.Study", [], Study, () => [["ID", option_type(string_type)], ["FileName", option_type(string_type)], ["Identifier", option_type(string_type)], ["Title", option_type(string_type)], ["Description", option_type(string_type)], ["SubmissionDate", option_type(string_type)], ["PublicReleaseDate", option_type(string_type)], ["Publications", option_type(list_type(Publication_$reflection()))], ["Contacts", option_type(list_type(Person_$reflection()))], ["StudyDesignDescriptors", option_type(list_type(OntologyAnnotation_$reflection()))], ["Protocols", option_type(list_type(Protocol_$reflection()))], ["Materials", option_type(StudyMaterials_$reflection())], ["ProcessSequence", option_type(list_type(Process_$reflection()))], ["Assays", option_type(list_type(Assay_$reflection()))], ["Factors", option_type(list_type(Factor_$reflection()))], ["CharacteristicCategories", option_type(list_type(MaterialAttribute_$reflection()))], ["UnitCategories", option_type(list_type(OntologyAnnotation_$reflection()))], ["Comments", option_type(list_type(Comment$_$reflection()))]]);
}

export function Study_make(id, filename, identifier, title, description, submissionDate, publicReleaseDate, publications, contacts, studyDesignDescriptors, protocols, materials, processSequence, assays, factors, characteristicCategories, unitCategories, comments) {
    return new Study(id, filename, identifier, title, description, submissionDate, publicReleaseDate, publications, contacts, studyDesignDescriptors, protocols, materials, processSequence, assays, factors, characteristicCategories, unitCategories, comments);
}

export function Study_create_Z6C8AB268(Id, FileName, Identifier, Title, Description, SubmissionDate, PublicReleaseDate, Publications, Contacts, StudyDesignDescriptors, Protocols, Materials, ProcessSequence, Assays, Factors, CharacteristicCategories, UnitCategories, Comments) {
    return Study_make(Id, FileName, Identifier, Title, Description, SubmissionDate, PublicReleaseDate, Publications, Contacts, StudyDesignDescriptors, Protocols, Materials, ProcessSequence, Assays, Factors, CharacteristicCategories, UnitCategories, Comments);
}

export function Study_get_empty() {
    return Study_create_Z6C8AB268();
}

/**
 * If an study with the given identfier exists in the list, returns true
 */
export function Study_existsByIdentifier(identifier, studies) {
    return exists((s) => equals(s.Identifier, identifier), studies);
}

/**
 * Adds the given study to the studies
 */
export function Study_add(studies, study) {
    return append(studies, singleton(study));
}

/**
 * Updates all studies for which the predicate returns true with the given study values
 */
export function Study_updateBy(predicate, updateOption, study, studies) {
    if (exists(predicate, studies)) {
        return map((a) => {
            if (predicate(a)) {
                const this$ = updateOption;
                const recordType_1 = a;
                const recordType_2 = study;
                return (this$.tag === 2) ? makeRecord(Study_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) : ((this$.tag === 1) ? makeRecord(Study_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2))) : ((this$.tag === 3) ? makeRecord(Study_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) : recordType_2));
            }
            else {
                return a;
            }
        }, studies);
    }
    else {
        return studies;
    }
}

/**
 * Updates all studies with the same identifier as the given study with its values
 */
export function Study_updateByIdentifier(updateOption, study, studies) {
    return Study_updateBy((s) => equals(s.Identifier, study.Identifier), updateOption, study, studies);
}

/**
 * If a study with the given identifier exists in the list, removes it
 */
export function Study_removeByIdentifier(identifier, studies) {
    return filter((s) => !equals(s.Identifier, identifier), studies);
}

/**
 * Returns assays of a study
 */
export function Study_getAssays_Z27CB2981(study) {
    return defaultArg(study.Assays, empty());
}

/**
 * Applies function f to the assays of a study
 */
export function Study_mapAssays(f, study) {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, study.Contacts, study.StudyDesignDescriptors, study.Protocols, study.Materials, study.ProcessSequence, mapDefault(empty(), f, study.Assays), study.Factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Replaces study assays with the given assay list
 */
export function Study_setAssays(study, assays) {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, study.Contacts, study.StudyDesignDescriptors, study.Protocols, study.Materials, study.ProcessSequence, assays, study.Factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Applies function f to the factors of a study
 */
export function Study_mapFactors(f, study) {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, study.Contacts, study.StudyDesignDescriptors, study.Protocols, study.Materials, study.ProcessSequence, study.Assays, mapDefault(empty(), f, study.Factors), study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Replaces study factors with the given assay list
 */
export function Study_setFactors(study, factors) {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, study.Contacts, study.StudyDesignDescriptors, study.Protocols, study.Materials, study.ProcessSequence, study.Assays, factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Applies function f to the protocols of a study
 */
export function Study_mapProtocols(f, study) {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, study.Contacts, study.StudyDesignDescriptors, mapDefault(empty(), f, study.Protocols), study.Materials, study.ProcessSequence, study.Assays, study.Factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Replaces study protocols with the given assay list
 */
export function Study_setProtocols(study, protocols) {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, study.Contacts, study.StudyDesignDescriptors, protocols, study.Materials, study.ProcessSequence, study.Assays, study.Factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Returns all contacts of a study
 */
export function Study_getContacts_Z27CB2981(study) {
    return defaultArg(study.Contacts, empty());
}

/**
 * Applies function f to contacts of a study
 */
export function Study_mapContacts(f, study) {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, mapDefault(empty(), f, study.Contacts), study.StudyDesignDescriptors, study.Protocols, study.Materials, study.ProcessSequence, study.Assays, study.Factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Replaces contacts of a study with the given person list
 */
export function Study_setContacts(study, persons) {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, persons, study.StudyDesignDescriptors, study.Protocols, study.Materials, study.ProcessSequence, study.Assays, study.Factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Returns publications of a study
 */
export function Study_getPublications_Z27CB2981(study) {
    return defaultArg(study.Publications, empty());
}

/**
 * Applies function f to publications of the study
 */
export function Study_mapPublications(f, study) {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, mapDefault(empty(), f, study.Publications), study.Contacts, study.StudyDesignDescriptors, study.Protocols, study.Materials, study.ProcessSequence, study.Assays, study.Factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Replaces publications of a study with the given publication list
 */
export function Study_setPublications(study, publications) {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, publications, study.Contacts, study.StudyDesignDescriptors, study.Protocols, study.Materials, study.ProcessSequence, study.Assays, study.Factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Returns study design descriptors of a study
 */
export function Study_getDescriptors_Z27CB2981(study) {
    return defaultArg(study.StudyDesignDescriptors, empty());
}

/**
 * Applies function f to to study design descriptors of a study
 */
export function Study_mapDescriptors(f, study) {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, study.Contacts, mapDefault(empty(), f, study.StudyDesignDescriptors), study.Protocols, study.Materials, study.ProcessSequence, study.Assays, study.Factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Replaces study design descriptors with the given ontology annotation list
 */
export function Study_setDescriptors(study, descriptors) {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, study.Contacts, descriptors, study.Protocols, study.Materials, study.ProcessSequence, study.Assays, study.Factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Returns processSequence of study
 */
export function Study_getProcesses_Z27CB2981(study) {
    return defaultArg(study.ProcessSequence, empty());
}

/**
 * Returns protocols of a study
 */
export function Study_getProtocols_Z27CB2981(study) {
    const processSequenceProtocols = getProtocols(Study_getProcesses_Z27CB2981(study));
    const assaysProtocols = collect(Assay_getProtocols_Z269B5B97, Study_getAssays_Z27CB2981(study));
    const updateOptions_2 = new Update_UpdateOptions(3, []);
    const mapping_6 = (p_1) => p_1.Name;
    const list1_1 = defaultArg(study.Protocols, empty());
    let list2_1;
    const updateOptions = new Update_UpdateOptions(3, []);
    const mapping_1 = (p) => p.Name;
    const list1 = assaysProtocols;
    const list2 = processSequenceProtocols;
    try {
        const map1 = Dict_ofSeqWithMerge((arg_1, arg_1_1) => {
            const this$ = updateOptions;
            const recordType_1 = arg_1;
            const recordType_2 = arg_1_1;
            switch (this$.tag) {
                case 2:
                    return makeRecord(Protocol_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                case 1:
                    return makeRecord(Protocol_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                case 3:
                    return makeRecord(Protocol_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                default:
                    return recordType_2;
            }
        }, map((v) => [mapping_1(v), v], list1));
        const map2 = Dict_ofSeqWithMerge((arg_2, arg_3) => {
            const this$_1 = updateOptions;
            const recordType_1_1 = arg_2;
            const recordType_2_1 = arg_3;
            switch (this$_1.tag) {
                case 2:
                    return makeRecord(Protocol_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1)));
                case 1:
                    return makeRecord(Protocol_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1)));
                case 3:
                    return makeRecord(Protocol_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1)));
                default:
                    return recordType_2_1;
            }
        }, map((v_1) => [mapping_1(v_1), v_1], list2));
        list2_1 = map((k) => {
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
                        return makeRecord(Protocol_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2)));
                    case 1:
                        return makeRecord(Protocol_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2)));
                    case 3:
                        return makeRecord(Protocol_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2)));
                    default:
                        return recordType_2_2;
                }
            }
        }, List_distinct(append(map(mapping_1, list1), map(mapping_1, list2)), {
            Equals: equals,
            GetHashCode: structuralHash,
        }));
    }
    catch (err) {
        throw new Error(`Could not mergeUpdate ${"Protocol"} list: 
${err.message}`);
    }
    try {
        const map1_1 = Dict_ofSeqWithMerge((arg_6, arg_1_2) => {
            const this$_3 = updateOptions_2;
            const recordType_1_3 = arg_6;
            const recordType_2_3 = arg_1_2;
            switch (this$_3.tag) {
                case 2:
                    return makeRecord(Protocol_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3)));
                case 1:
                    return makeRecord(Protocol_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3)));
                case 3:
                    return makeRecord(Protocol_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3)));
                default:
                    return recordType_2_3;
            }
        }, map((v_2) => [mapping_6(v_2), v_2], list1_1));
        const map2_1 = Dict_ofSeqWithMerge((arg_2_1, arg_3_1) => {
            const this$_4 = updateOptions_2;
            const recordType_1_4 = arg_2_1;
            const recordType_2_4 = arg_3_1;
            switch (this$_4.tag) {
                case 2:
                    return makeRecord(Protocol_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4)));
                case 1:
                    return makeRecord(Protocol_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4)));
                case 3:
                    return makeRecord(Protocol_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4)));
                default:
                    return recordType_2_4;
            }
        }, map((v_1_1) => [mapping_6(v_1_1), v_1_1], list2_1));
        return map((k_1) => {
            const matchValue_3 = Dict_tryFind(k_1, map1_1);
            const matchValue_1_1 = Dict_tryFind(k_1, map2_1);
            if (matchValue_3 == null) {
                if (matchValue_1_1 == null) {
                    throw new Error("If this fails, then I don\'t know how to program");
                }
                else {
                    const v2_1_1 = matchValue_1_1;
                    return v2_1_1;
                }
            }
            else if (matchValue_1_1 == null) {
                const v1_1_1 = matchValue_3;
                return v1_1_1;
            }
            else {
                const v1_2 = matchValue_3;
                const v2_2 = matchValue_1_1;
                const this$_5 = updateOptions_2;
                const recordType_1_5 = v1_2;
                const recordType_2_5 = v2_2;
                switch (this$_5.tag) {
                    case 2:
                        return makeRecord(Protocol_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5)));
                    case 1:
                        return makeRecord(Protocol_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5)));
                    case 3:
                        return makeRecord(Protocol_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5)));
                    default:
                        return recordType_2_5;
                }
            }
        }, List_distinct(append(map(mapping_6, list1_1), map(mapping_6, list2_1)), {
            Equals: equals,
            GetHashCode: structuralHash,
        }));
    }
    catch (err_1) {
        throw new Error(`Could not mergeUpdate ${"Protocol"} list: 
${err_1.message}`);
    }
}

/**
 * Returns Characteristics of the study
 */
export function Study_getCharacteristics_Z27CB2981(study) {
    return List_distinct(append(getCharacteristics(Study_getProcesses_Z27CB2981(study)), append(collect(Assay_getCharacteristics_Z269B5B97, Study_getAssays_Z27CB2981(study)), defaultArg(study.CharacteristicCategories, empty()))), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns factors of the study
 */
export function Study_getFactors_Z27CB2981(study) {
    return List_distinct(append(getFactors(Study_getProcesses_Z27CB2981(study)), append(collect(Assay_getFactors_Z269B5B97, Study_getAssays_Z27CB2981(study)), defaultArg(study.Factors, empty()))), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns unit categories of the study
 */
export function Study_getUnitCategories_Z27CB2981(study) {
    return List_distinct(append(getUnits(Study_getProcesses_Z27CB2981(study)), append(collect(Assay_getUnitCategories_Z269B5B97, Study_getAssays_Z27CB2981(study)), defaultArg(study.UnitCategories, empty()))), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns sources of the study
 */
export function Study_getSources_Z27CB2981(study) {
    const processSequenceSources = getSources(Study_getProcesses_Z27CB2981(study));
    const assaysSources = collect(Assay_getSources_Z269B5B97, Study_getAssays_Z27CB2981(study));
    const updateOptions_2 = new Update_UpdateOptions(3, []);
    const mapping_6 = (s_2) => defaultArg(s_2.Name, "");
    let list1_1;
    const matchValue = study.Materials;
    list1_1 = ((matchValue == null) ? empty() : defaultArg(matchValue.Sources, empty()));
    let list2_1;
    const updateOptions = new Update_UpdateOptions(3, []);
    const mapping_1 = (s) => defaultArg(s.Name, "");
    const list1 = assaysSources;
    const list2 = processSequenceSources;
    try {
        const map1 = Dict_ofSeqWithMerge((arg_1, arg_1_1) => {
            const this$ = updateOptions;
            const recordType_1 = arg_1;
            const recordType_2 = arg_1_1;
            switch (this$.tag) {
                case 2:
                    return makeRecord(Source_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                case 1:
                    return makeRecord(Source_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                case 3:
                    return makeRecord(Source_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                default:
                    return recordType_2;
            }
        }, map((v) => [mapping_1(v), v], list1));
        const map2 = Dict_ofSeqWithMerge((arg_2, arg_3) => {
            const this$_1 = updateOptions;
            const recordType_1_1 = arg_2;
            const recordType_2_1 = arg_3;
            switch (this$_1.tag) {
                case 2:
                    return makeRecord(Source_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1)));
                case 1:
                    return makeRecord(Source_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1)));
                case 3:
                    return makeRecord(Source_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1)));
                default:
                    return recordType_2_1;
            }
        }, map((v_1) => [mapping_1(v_1), v_1], list2));
        list2_1 = map((k) => {
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
                        return makeRecord(Source_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2)));
                    case 1:
                        return makeRecord(Source_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2)));
                    case 3:
                        return makeRecord(Source_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2)));
                    default:
                        return recordType_2_2;
                }
            }
        }, List_distinct(append(map(mapping_1, list1), map(mapping_1, list2)), {
            Equals: (x, y) => (x === y),
            GetHashCode: stringHash,
        }));
    }
    catch (err) {
        throw new Error(`Could not mergeUpdate ${"Source"} list: 
${err.message}`);
    }
    try {
        const map1_1 = Dict_ofSeqWithMerge((arg_6, arg_1_2) => {
            const this$_3 = updateOptions_2;
            const recordType_1_3 = arg_6;
            const recordType_2_3 = arg_1_2;
            switch (this$_3.tag) {
                case 2:
                    return makeRecord(Source_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3)));
                case 1:
                    return makeRecord(Source_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3)));
                case 3:
                    return makeRecord(Source_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3)));
                default:
                    return recordType_2_3;
            }
        }, map((v_2) => [mapping_6(v_2), v_2], list1_1));
        const map2_1 = Dict_ofSeqWithMerge((arg_2_1, arg_3_1) => {
            const this$_4 = updateOptions_2;
            const recordType_1_4 = arg_2_1;
            const recordType_2_4 = arg_3_1;
            switch (this$_4.tag) {
                case 2:
                    return makeRecord(Source_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4)));
                case 1:
                    return makeRecord(Source_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4)));
                case 3:
                    return makeRecord(Source_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4)));
                default:
                    return recordType_2_4;
            }
        }, map((v_1_1) => [mapping_6(v_1_1), v_1_1], list2_1));
        return map((k_1) => {
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
                const this$_5 = updateOptions_2;
                const recordType_1_5 = v1_2;
                const recordType_2_5 = v2_2;
                switch (this$_5.tag) {
                    case 2:
                        return makeRecord(Source_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5)));
                    case 1:
                        return makeRecord(Source_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5)));
                    case 3:
                        return makeRecord(Source_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5)));
                    default:
                        return recordType_2_5;
                }
            }
        }, List_distinct(append(map(mapping_6, list1_1), map(mapping_6, list2_1)), {
            Equals: (x_1, y_1) => (x_1 === y_1),
            GetHashCode: stringHash,
        }));
    }
    catch (err_1) {
        throw new Error(`Could not mergeUpdate ${"Source"} list: 
${err_1.message}`);
    }
}

/**
 * Returns sources of the study
 */
export function Study_getSamples_Z27CB2981(study) {
    const processSequenceSamples = getSamples(Study_getProcesses_Z27CB2981(study));
    const assaysSamples = collect(Assay_getSamples_Z269B5B97, Study_getAssays_Z27CB2981(study));
    const updateOptions_2 = new Update_UpdateOptions(3, []);
    const mapping_6 = (s_2) => defaultArg(s_2.Name, "");
    let list1_1;
    const matchValue = study.Materials;
    list1_1 = ((matchValue == null) ? empty() : defaultArg(matchValue.Samples, empty()));
    let list2_1;
    const updateOptions = new Update_UpdateOptions(3, []);
    const mapping_1 = (s) => defaultArg(s.Name, "");
    const list1 = assaysSamples;
    const list2 = processSequenceSamples;
    try {
        const map1 = Dict_ofSeqWithMerge((arg_1, arg_1_1) => {
            const this$ = updateOptions;
            const recordType_1 = arg_1;
            const recordType_2 = arg_1_1;
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
        }, map((v) => [mapping_1(v), v], list1));
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
        }, map((v_1) => [mapping_1(v_1), v_1], list2));
        list2_1 = map((k) => {
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
        }, List_distinct(append(map(mapping_1, list1), map(mapping_1, list2)), {
            Equals: (x, y) => (x === y),
            GetHashCode: stringHash,
        }));
    }
    catch (err) {
        throw new Error(`Could not mergeUpdate ${"Sample"} list: 
${err.message}`);
    }
    try {
        const map1_1 = Dict_ofSeqWithMerge((arg_6, arg_1_2) => {
            const this$_3 = updateOptions_2;
            const recordType_1_3 = arg_6;
            const recordType_2_3 = arg_1_2;
            switch (this$_3.tag) {
                case 2:
                    return makeRecord(Sample_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3)));
                case 1:
                    return makeRecord(Sample_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3)));
                case 3:
                    return makeRecord(Sample_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3)));
                default:
                    return recordType_2_3;
            }
        }, map((v_2) => [mapping_6(v_2), v_2], list1_1));
        const map2_1 = Dict_ofSeqWithMerge((arg_2_1, arg_3_1) => {
            const this$_4 = updateOptions_2;
            const recordType_1_4 = arg_2_1;
            const recordType_2_4 = arg_3_1;
            switch (this$_4.tag) {
                case 2:
                    return makeRecord(Sample_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4)));
                case 1:
                    return makeRecord(Sample_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4)));
                case 3:
                    return makeRecord(Sample_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4)));
                default:
                    return recordType_2_4;
            }
        }, map((v_1_1) => [mapping_6(v_1_1), v_1_1], list2_1));
        return map((k_1) => {
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
                const this$_5 = updateOptions_2;
                const recordType_1_5 = v1_2;
                const recordType_2_5 = v2_2;
                switch (this$_5.tag) {
                    case 2:
                        return makeRecord(Sample_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5)));
                    case 1:
                        return makeRecord(Sample_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5)));
                    case 3:
                        return makeRecord(Sample_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5)));
                    default:
                        return recordType_2_5;
                }
            }
        }, List_distinct(append(map(mapping_6, list1_1), map(mapping_6, list2_1)), {
            Equals: (x_1, y_1) => (x_1 === y_1),
            GetHashCode: stringHash,
        }));
    }
    catch (err_1) {
        throw new Error(`Could not mergeUpdate ${"Sample"} list: 
${err_1.message}`);
    }
}

/**
 * Returns materials of the study
 */
export function Study_getMaterials_Z27CB2981(study) {
    const processSequenceMaterials = getMaterials(Study_getProcesses_Z27CB2981(study));
    const assaysMaterials = collect((arg_2) => AssayMaterials_getMaterials_E3447B1(Assay_getMaterials_Z269B5B97(arg_2)), Study_getAssays_Z27CB2981(study));
    let materials;
    const updateOptions_2 = new Update_UpdateOptions(3, []);
    const mapping_6 = (s_2) => s_2.Name;
    let list1_1;
    const matchValue = study.Materials;
    list1_1 = ((matchValue == null) ? empty() : defaultArg(matchValue.OtherMaterials, empty()));
    let list2_1;
    const updateOptions = new Update_UpdateOptions(3, []);
    const mapping_1 = (s) => s.Name;
    const list1 = assaysMaterials;
    const list2 = processSequenceMaterials;
    try {
        const map1 = Dict_ofSeqWithMerge((arg_3, arg_1_1) => {
            const this$ = updateOptions;
            const recordType_1 = arg_3;
            const recordType_2 = arg_1_1;
            switch (this$.tag) {
                case 2:
                    return makeRecord(Material_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                case 1:
                    return makeRecord(Material_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                case 3:
                    return makeRecord(Material_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                default:
                    return recordType_2;
            }
        }, map((v) => [mapping_1(v), v], list1));
        const map2 = Dict_ofSeqWithMerge((arg_2_1, arg_3_1) => {
            const this$_1 = updateOptions;
            const recordType_1_1 = arg_2_1;
            const recordType_2_1 = arg_3_1;
            switch (this$_1.tag) {
                case 2:
                    return makeRecord(Material_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1)));
                case 1:
                    return makeRecord(Material_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1)));
                case 3:
                    return makeRecord(Material_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1)));
                default:
                    return recordType_2_1;
            }
        }, map((v_1) => [mapping_1(v_1), v_1], list2));
        list2_1 = map((k) => {
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
                        return makeRecord(Material_$reflection(), map2_2(Update_updateAppend, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2)));
                    case 1:
                        return makeRecord(Material_$reflection(), map2_2(Update_updateOnlyByExisting, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2)));
                    case 3:
                        return makeRecord(Material_$reflection(), map2_2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2)));
                    default:
                        return recordType_2_2;
                }
            }
        }, List_distinct(append(map(mapping_1, list1), map(mapping_1, list2)), {
            Equals: equals,
            GetHashCode: structuralHash,
        }));
    }
    catch (err) {
        throw new Error(`Could not mergeUpdate ${"Material"} list: 
${err.message}`);
    }
    try {
        const map1_1 = Dict_ofSeqWithMerge((arg_6, arg_1_2) => {
            const this$_3 = updateOptions_2;
            const recordType_1_3 = arg_6;
            const recordType_2_3 = arg_1_2;
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
        }, map((v_2) => [mapping_6(v_2), v_2], list1_1));
        const map2_1 = Dict_ofSeqWithMerge((arg_2_2, arg_3_2) => {
            const this$_4 = updateOptions_2;
            const recordType_1_4 = arg_2_2;
            const recordType_2_4 = arg_3_2;
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
        }, map((v_1_1) => [mapping_6(v_1_1), v_1_1], list2_1));
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
                const this$_5 = updateOptions_2;
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
        }, List_distinct(append(map(mapping_6, list1_1), map(mapping_6, list2_1)), {
            Equals: equals,
            GetHashCode: structuralHash,
        }));
    }
    catch (err_1) {
        throw new Error(`Could not mergeUpdate ${"Material"} list: 
${err_1.message}`);
    }
    const sources = Study_getSources_Z27CB2981(study);
    const samples = Study_getSamples_Z27CB2981(study);
    return StudyMaterials_make(fromValueWithDefault(empty(), sources), fromValueWithDefault(empty(), samples), fromValueWithDefault(empty(), materials));
}

export function Study_update_Z27CB2981(study) {
    try {
        const protocols = Study_getProtocols_Z27CB2981(study);
        let Materials;
        const v = Study_getMaterials_Z27CB2981(study);
        Materials = fromValueWithDefault(StudyMaterials_get_empty(), v);
        const Assays = map_1((list) => map((arg_3) => ((arg_2) => Assay_updateProtocols(protocols, arg_2))(Assay_update_Z269B5B97(arg_3)), list), study.Assays);
        const Protocols = fromValueWithDefault(empty(), protocols);
        const Factors = fromValueWithDefault(empty(), Study_getFactors_Z27CB2981(study));
        const CharacteristicCategories = fromValueWithDefault(empty(), Study_getCharacteristics_Z27CB2981(study));
        const UnitCategories = fromValueWithDefault(empty(), Study_getUnitCategories_Z27CB2981(study));
        return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, study.Contacts, study.StudyDesignDescriptors, Protocols, Materials, map_1((processSequence) => updateProtocols(protocols, processSequence), study.ProcessSequence), Assays, Factors, CharacteristicCategories, UnitCategories, study.Comments);
    }
    catch (err) {
        return toFail(`Could not update study ${study.Identifier}: 
${err.message}`);
    }
}

