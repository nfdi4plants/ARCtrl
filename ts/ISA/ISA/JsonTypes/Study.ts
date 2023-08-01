import { Record } from "../../../fable_modules/fable-library-ts/Types.js";
import { map as map_1, value as value_3, defaultArg, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { collect, empty, filter, map, singleton, append, exists, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { Publication_$reflection, Publication } from "./Publication.js";
import { Person_$reflection, Person } from "./Person.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation } from "./OntologyAnnotation.js";
import { Protocol_$reflection, Protocol } from "./Protocol.js";
import { StudyMaterials_get_empty, StudyMaterials_make, StudyMaterials_$reflection, StudyMaterials } from "./StudyMaterials.js";
import { Process_$reflection, Process } from "./Process.js";
import { Assay_update_Z269B5B97, Assay_updateProtocols, Assay_getMaterials_Z269B5B97, Assay_getSamples_Z269B5B97, Assay_getSources_Z269B5B97, Assay_getUnitCategories_Z269B5B97, Assay_getFactors_Z269B5B97, Assay_getCharacteristics_Z269B5B97, Assay_getProtocols_Z269B5B97, Assay_$reflection, Assay } from "./Assay.js";
import { Factor_$reflection, Factor } from "./Factor.js";
import { MaterialAttribute_$reflection, MaterialAttribute } from "./MaterialAttribute.js";
import { Comment$_$reflection, Comment$ } from "./Comment.js";
import { stringHash, safeHash, structuralHash, IMap, equals, IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { getRecordFields, makeRecord, record_type, list_type, option_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { Update_UpdateOptions_UpdateByExistingAppendLists, Update_UpdateOptions_$union } from "../Update.js";
import { map2 as map2_2 } from "../../../fable_modules/fable-library-ts/Array.js";
import { Dict_tryFind, Dict_ofSeqWithMerge, Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { fromValueWithDefault, mapDefault } from "../OptionExtensions.js";
import { updateProtocols, getMaterials, getSamples, getSources, getUnits, getFactors, getCharacteristics, getProtocols } from "./ProcessSequence.js";
import { List_distinct } from "../../../fable_modules/fable-library-ts/Seq2.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { Source_$reflection, Source } from "./Source.js";
import { Sample_$reflection, Sample } from "./Sample.js";
import { Material_$reflection, Material } from "./Material.js";
import { AssayMaterials_getMaterials_E3447B1 } from "./AssayMaterials.js";
import { toFail } from "../../../fable_modules/fable-library-ts/String.js";

export class Study extends Record implements IEquatable<Study> {
    readonly ID: Option<string>;
    readonly FileName: Option<string>;
    readonly Identifier: Option<string>;
    readonly Title: Option<string>;
    readonly Description: Option<string>;
    readonly SubmissionDate: Option<string>;
    readonly PublicReleaseDate: Option<string>;
    readonly Publications: Option<FSharpList<Publication>>;
    readonly Contacts: Option<FSharpList<Person>>;
    readonly StudyDesignDescriptors: Option<FSharpList<OntologyAnnotation>>;
    readonly Protocols: Option<FSharpList<Protocol>>;
    readonly Materials: Option<StudyMaterials>;
    readonly ProcessSequence: Option<FSharpList<Process>>;
    readonly Assays: Option<FSharpList<Assay>>;
    readonly Factors: Option<FSharpList<Factor>>;
    readonly CharacteristicCategories: Option<FSharpList<MaterialAttribute>>;
    readonly UnitCategories: Option<FSharpList<OntologyAnnotation>>;
    readonly Comments: Option<FSharpList<Comment$>>;
    constructor(ID: Option<string>, FileName: Option<string>, Identifier: Option<string>, Title: Option<string>, Description: Option<string>, SubmissionDate: Option<string>, PublicReleaseDate: Option<string>, Publications: Option<FSharpList<Publication>>, Contacts: Option<FSharpList<Person>>, StudyDesignDescriptors: Option<FSharpList<OntologyAnnotation>>, Protocols: Option<FSharpList<Protocol>>, Materials: Option<StudyMaterials>, ProcessSequence: Option<FSharpList<Process>>, Assays: Option<FSharpList<Assay>>, Factors: Option<FSharpList<Factor>>, CharacteristicCategories: Option<FSharpList<MaterialAttribute>>, UnitCategories: Option<FSharpList<OntologyAnnotation>>, Comments: Option<FSharpList<Comment$>>) {
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

export function Study_$reflection(): TypeInfo {
    return record_type("ISA.Study", [], Study, () => [["ID", option_type(string_type)], ["FileName", option_type(string_type)], ["Identifier", option_type(string_type)], ["Title", option_type(string_type)], ["Description", option_type(string_type)], ["SubmissionDate", option_type(string_type)], ["PublicReleaseDate", option_type(string_type)], ["Publications", option_type(list_type(Publication_$reflection()))], ["Contacts", option_type(list_type(Person_$reflection()))], ["StudyDesignDescriptors", option_type(list_type(OntologyAnnotation_$reflection()))], ["Protocols", option_type(list_type(Protocol_$reflection()))], ["Materials", option_type(StudyMaterials_$reflection())], ["ProcessSequence", option_type(list_type(Process_$reflection()))], ["Assays", option_type(list_type(Assay_$reflection()))], ["Factors", option_type(list_type(Factor_$reflection()))], ["CharacteristicCategories", option_type(list_type(MaterialAttribute_$reflection()))], ["UnitCategories", option_type(list_type(OntologyAnnotation_$reflection()))], ["Comments", option_type(list_type(Comment$_$reflection()))]]);
}

export function Study_make(id: Option<string>, filename: Option<string>, identifier: Option<string>, title: Option<string>, description: Option<string>, submissionDate: Option<string>, publicReleaseDate: Option<string>, publications: Option<FSharpList<Publication>>, contacts: Option<FSharpList<Person>>, studyDesignDescriptors: Option<FSharpList<OntologyAnnotation>>, protocols: Option<FSharpList<Protocol>>, materials: Option<StudyMaterials>, processSequence: Option<FSharpList<Process>>, assays: Option<FSharpList<Assay>>, factors: Option<FSharpList<Factor>>, characteristicCategories: Option<FSharpList<MaterialAttribute>>, unitCategories: Option<FSharpList<OntologyAnnotation>>, comments: Option<FSharpList<Comment$>>): Study {
    return new Study(id, filename, identifier, title, description, submissionDate, publicReleaseDate, publications, contacts, studyDesignDescriptors, protocols, materials, processSequence, assays, factors, characteristicCategories, unitCategories, comments);
}

export function Study_create_Z6C8AB268(Id?: string, FileName?: string, Identifier?: string, Title?: string, Description?: string, SubmissionDate?: string, PublicReleaseDate?: string, Publications?: FSharpList<Publication>, Contacts?: FSharpList<Person>, StudyDesignDescriptors?: FSharpList<OntologyAnnotation>, Protocols?: FSharpList<Protocol>, Materials?: StudyMaterials, ProcessSequence?: FSharpList<Process>, Assays?: FSharpList<Assay>, Factors?: FSharpList<Factor>, CharacteristicCategories?: FSharpList<MaterialAttribute>, UnitCategories?: FSharpList<OntologyAnnotation>, Comments?: FSharpList<Comment$>): Study {
    return Study_make(Id, FileName, Identifier, Title, Description, SubmissionDate, PublicReleaseDate, Publications, Contacts, StudyDesignDescriptors, Protocols, Materials, ProcessSequence, Assays, Factors, CharacteristicCategories, UnitCategories, Comments);
}

export function Study_get_empty(): Study {
    return Study_create_Z6C8AB268();
}

/**
 * If an study with the given identfier exists in the list, returns true
 */
export function Study_existsByIdentifier(identifier: string, studies: FSharpList<Study>): boolean {
    return exists<Study>((s: Study): boolean => equals(s.Identifier, identifier), studies);
}

/**
 * Adds the given study to the studies
 */
export function Study_add(studies: FSharpList<Study>, study: Study): FSharpList<Study> {
    return append<Study>(studies, singleton(study));
}

/**
 * Updates all studies for which the predicate returns true with the given study values
 */
export function Study_updateBy(predicate: ((arg0: Study) => boolean), updateOption: Update_UpdateOptions_$union, study: Study, studies: FSharpList<Study>): FSharpList<Study> {
    if (exists<Study>(predicate, studies)) {
        return map<Study, Study>((a: Study): Study => {
            if (predicate(a)) {
                const this$: Update_UpdateOptions_$union = updateOption;
                const recordType_1: Study = a;
                const recordType_2: Study = study;
                return (this$.tag === /* UpdateAllAppendLists */ 2) ? (makeRecord(Study_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Study) : ((this$.tag === /* UpdateByExisting */ 1) ? (makeRecord(Study_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Study) : ((this$.tag === /* UpdateByExistingAppendLists */ 3) ? (makeRecord(Study_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Study) : recordType_2));
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
export function Study_updateByIdentifier(updateOption: Update_UpdateOptions_$union, study: Study, studies: FSharpList<Study>): FSharpList<Study> {
    return Study_updateBy((s: Study): boolean => equals(s.Identifier, study.Identifier), updateOption, study, studies);
}

/**
 * If a study with the given identifier exists in the list, removes it
 */
export function Study_removeByIdentifier(identifier: string, studies: FSharpList<Study>): FSharpList<Study> {
    return filter<Study>((s: Study): boolean => !equals(s.Identifier, identifier), studies);
}

/**
 * Returns assays of a study
 */
export function Study_getAssays_Z27CB2981(study: Study): FSharpList<Assay> {
    return defaultArg(study.Assays, empty<Assay>());
}

/**
 * Applies function f to the assays of a study
 */
export function Study_mapAssays(f: ((arg0: FSharpList<Assay>) => FSharpList<Assay>), study: Study): Study {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, study.Contacts, study.StudyDesignDescriptors, study.Protocols, study.Materials, study.ProcessSequence, mapDefault<FSharpList<Assay>>(empty<Assay>(), f, study.Assays), study.Factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Replaces study assays with the given assay list
 */
export function Study_setAssays(study: Study, assays: FSharpList<Assay>): Study {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, study.Contacts, study.StudyDesignDescriptors, study.Protocols, study.Materials, study.ProcessSequence, assays, study.Factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Applies function f to the factors of a study
 */
export function Study_mapFactors(f: ((arg0: FSharpList<Factor>) => FSharpList<Factor>), study: Study): Study {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, study.Contacts, study.StudyDesignDescriptors, study.Protocols, study.Materials, study.ProcessSequence, study.Assays, mapDefault<FSharpList<Factor>>(empty<Factor>(), f, study.Factors), study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Replaces study factors with the given assay list
 */
export function Study_setFactors(study: Study, factors: FSharpList<Factor>): Study {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, study.Contacts, study.StudyDesignDescriptors, study.Protocols, study.Materials, study.ProcessSequence, study.Assays, factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Applies function f to the protocols of a study
 */
export function Study_mapProtocols(f: ((arg0: FSharpList<Protocol>) => FSharpList<Protocol>), study: Study): Study {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, study.Contacts, study.StudyDesignDescriptors, mapDefault<FSharpList<Protocol>>(empty<Protocol>(), f, study.Protocols), study.Materials, study.ProcessSequence, study.Assays, study.Factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Replaces study protocols with the given assay list
 */
export function Study_setProtocols(study: Study, protocols: FSharpList<Protocol>): Study {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, study.Contacts, study.StudyDesignDescriptors, protocols, study.Materials, study.ProcessSequence, study.Assays, study.Factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Returns all contacts of a study
 */
export function Study_getContacts_Z27CB2981(study: Study): FSharpList<Person> {
    return defaultArg(study.Contacts, empty<Person>());
}

/**
 * Applies function f to contacts of a study
 */
export function Study_mapContacts(f: ((arg0: FSharpList<Person>) => FSharpList<Person>), study: Study): Study {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, mapDefault<FSharpList<Person>>(empty<Person>(), f, study.Contacts), study.StudyDesignDescriptors, study.Protocols, study.Materials, study.ProcessSequence, study.Assays, study.Factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Replaces contacts of a study with the given person list
 */
export function Study_setContacts(study: Study, persons: FSharpList<Person>): Study {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, persons, study.StudyDesignDescriptors, study.Protocols, study.Materials, study.ProcessSequence, study.Assays, study.Factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Returns publications of a study
 */
export function Study_getPublications_Z27CB2981(study: Study): FSharpList<Publication> {
    return defaultArg(study.Publications, empty<Publication>());
}

/**
 * Applies function f to publications of the study
 */
export function Study_mapPublications(f: ((arg0: FSharpList<Publication>) => FSharpList<Publication>), study: Study): Study {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, mapDefault<FSharpList<Publication>>(empty<Publication>(), f, study.Publications), study.Contacts, study.StudyDesignDescriptors, study.Protocols, study.Materials, study.ProcessSequence, study.Assays, study.Factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Replaces publications of a study with the given publication list
 */
export function Study_setPublications(study: Study, publications: FSharpList<Publication>): Study {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, publications, study.Contacts, study.StudyDesignDescriptors, study.Protocols, study.Materials, study.ProcessSequence, study.Assays, study.Factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Returns study design descriptors of a study
 */
export function Study_getDescriptors_Z27CB2981(study: Study): FSharpList<OntologyAnnotation> {
    return defaultArg(study.StudyDesignDescriptors, empty<OntologyAnnotation>());
}

/**
 * Applies function f to to study design descriptors of a study
 */
export function Study_mapDescriptors(f: ((arg0: FSharpList<OntologyAnnotation>) => FSharpList<OntologyAnnotation>), study: Study): Study {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, study.Contacts, mapDefault<FSharpList<OntologyAnnotation>>(empty<OntologyAnnotation>(), f, study.StudyDesignDescriptors), study.Protocols, study.Materials, study.ProcessSequence, study.Assays, study.Factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Replaces study design descriptors with the given ontology annotation list
 */
export function Study_setDescriptors(study: Study, descriptors: FSharpList<OntologyAnnotation>): Study {
    return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, study.Contacts, descriptors, study.Protocols, study.Materials, study.ProcessSequence, study.Assays, study.Factors, study.CharacteristicCategories, study.UnitCategories, study.Comments);
}

/**
 * Returns processSequence of study
 */
export function Study_getProcesses_Z27CB2981(study: Study): FSharpList<Process> {
    return defaultArg(study.ProcessSequence, empty<Process>());
}

/**
 * Returns protocols of a study
 */
export function Study_getProtocols_Z27CB2981(study: Study): FSharpList<Protocol> {
    const processSequenceProtocols: FSharpList<Protocol> = getProtocols(Study_getProcesses_Z27CB2981(study));
    const assaysProtocols: FSharpList<Protocol> = collect<Assay, Protocol>(Assay_getProtocols_Z269B5B97, Study_getAssays_Z27CB2981(study));
    const updateOptions_2: Update_UpdateOptions_$union = Update_UpdateOptions_UpdateByExistingAppendLists();
    const mapping_6 = (p_1: Protocol): Option<string> => p_1.Name;
    const list1_1: FSharpList<Protocol> = defaultArg(study.Protocols, empty<Protocol>());
    let list2_1: FSharpList<Protocol>;
    const updateOptions: Update_UpdateOptions_$union = Update_UpdateOptions_UpdateByExistingAppendLists();
    const mapping_1 = (p: Protocol): Option<string> => p.Name;
    const list1: FSharpList<Protocol> = assaysProtocols;
    const list2: FSharpList<Protocol> = processSequenceProtocols;
    try {
        const map1: IMap<Option<string>, Protocol> = Dict_ofSeqWithMerge<Protocol, Option<string>>((arg_1: Protocol, arg_1_1: Protocol): Protocol => {
            const this$: Update_UpdateOptions_$union = updateOptions;
            const recordType_1: Protocol = arg_1;
            const recordType_2: Protocol = arg_1_1;
            switch (this$.tag) {
                case /* UpdateAllAppendLists */ 2:
                    return makeRecord(Protocol_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Protocol;
                case /* UpdateByExisting */ 1:
                    return makeRecord(Protocol_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Protocol;
                case /* UpdateByExistingAppendLists */ 3:
                    return makeRecord(Protocol_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Protocol;
                default:
                    return recordType_2;
            }
        }, map<Protocol, [Option<string>, Protocol]>((v: Protocol): [Option<string>, Protocol] => ([mapping_1(v), v] as [Option<string>, Protocol]), list1));
        const map2: IMap<Option<string>, Protocol> = Dict_ofSeqWithMerge<Protocol, Option<string>>((arg_2: Protocol, arg_3: Protocol): Protocol => {
            const this$_1: Update_UpdateOptions_$union = updateOptions;
            const recordType_1_1: Protocol = arg_2;
            const recordType_2_1: Protocol = arg_3;
            switch (this$_1.tag) {
                case /* UpdateAllAppendLists */ 2:
                    return makeRecord(Protocol_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1))) as Protocol;
                case /* UpdateByExisting */ 1:
                    return makeRecord(Protocol_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1))) as Protocol;
                case /* UpdateByExistingAppendLists */ 3:
                    return makeRecord(Protocol_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1))) as Protocol;
                default:
                    return recordType_2_1;
            }
        }, map<Protocol, [Option<string>, Protocol]>((v_1: Protocol): [Option<string>, Protocol] => ([mapping_1(v_1), v_1] as [Option<string>, Protocol]), list2));
        list2_1 = map<Option<string>, Protocol>((k: Option<string>): Protocol => {
            const matchValue: Option<Protocol> = Dict_tryFind<Option<string>, Protocol>(k, map1);
            const matchValue_1: Option<Protocol> = Dict_tryFind<Option<string>, Protocol>(k, map2);
            if (matchValue == null) {
                if (matchValue_1 == null) {
                    throw new Error("If this fails, then I don\'t know how to program");
                }
                else {
                    const v2_1: Protocol = value_3(matchValue_1);
                    return v2_1;
                }
            }
            else if (matchValue_1 == null) {
                const v1_1: Protocol = value_3(matchValue);
                return v1_1;
            }
            else {
                const v1: Protocol = value_3(matchValue);
                const v2: Protocol = value_3(matchValue_1);
                const this$_2: Update_UpdateOptions_$union = updateOptions;
                const recordType_1_2: Protocol = v1;
                const recordType_2_2: Protocol = v2;
                switch (this$_2.tag) {
                    case /* UpdateAllAppendLists */ 2:
                        return makeRecord(Protocol_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2))) as Protocol;
                    case /* UpdateByExisting */ 1:
                        return makeRecord(Protocol_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2))) as Protocol;
                    case /* UpdateByExistingAppendLists */ 3:
                        return makeRecord(Protocol_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2))) as Protocol;
                    default:
                        return recordType_2_2;
                }
            }
        }, List_distinct<Option<string>>(append<Option<string>>(map<Protocol, Option<string>>(mapping_1, list1), map<Protocol, Option<string>>(mapping_1, list2)), {
            Equals: equals,
            GetHashCode: structuralHash,
        }));
    }
    catch (err: any) {
        throw new Error(`Could not mergeUpdate ${"Protocol"} list: 
${err.message}`);
    }
    try {
        const map1_1: IMap<Option<string>, Protocol> = Dict_ofSeqWithMerge<Protocol, Option<string>>((arg_6: Protocol, arg_1_2: Protocol): Protocol => {
            const this$_3: Update_UpdateOptions_$union = updateOptions_2;
            const recordType_1_3: Protocol = arg_6;
            const recordType_2_3: Protocol = arg_1_2;
            switch (this$_3.tag) {
                case /* UpdateAllAppendLists */ 2:
                    return makeRecord(Protocol_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3))) as Protocol;
                case /* UpdateByExisting */ 1:
                    return makeRecord(Protocol_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3))) as Protocol;
                case /* UpdateByExistingAppendLists */ 3:
                    return makeRecord(Protocol_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3))) as Protocol;
                default:
                    return recordType_2_3;
            }
        }, map<Protocol, [Option<string>, Protocol]>((v_2: Protocol): [Option<string>, Protocol] => ([mapping_6(v_2), v_2] as [Option<string>, Protocol]), list1_1));
        const map2_1: IMap<Option<string>, Protocol> = Dict_ofSeqWithMerge<Protocol, Option<string>>((arg_2_1: Protocol, arg_3_1: Protocol): Protocol => {
            const this$_4: Update_UpdateOptions_$union = updateOptions_2;
            const recordType_1_4: Protocol = arg_2_1;
            const recordType_2_4: Protocol = arg_3_1;
            switch (this$_4.tag) {
                case /* UpdateAllAppendLists */ 2:
                    return makeRecord(Protocol_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4))) as Protocol;
                case /* UpdateByExisting */ 1:
                    return makeRecord(Protocol_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4))) as Protocol;
                case /* UpdateByExistingAppendLists */ 3:
                    return makeRecord(Protocol_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4))) as Protocol;
                default:
                    return recordType_2_4;
            }
        }, map<Protocol, [Option<string>, Protocol]>((v_1_1: Protocol): [Option<string>, Protocol] => ([mapping_6(v_1_1), v_1_1] as [Option<string>, Protocol]), list2_1));
        return map<Option<string>, Protocol>((k_1: Option<string>): Protocol => {
            const matchValue_3: Option<Protocol> = Dict_tryFind<Option<string>, Protocol>(k_1, map1_1);
            const matchValue_1_1: Option<Protocol> = Dict_tryFind<Option<string>, Protocol>(k_1, map2_1);
            if (matchValue_3 == null) {
                if (matchValue_1_1 == null) {
                    throw new Error("If this fails, then I don\'t know how to program");
                }
                else {
                    const v2_1_1: Protocol = value_3(matchValue_1_1);
                    return v2_1_1;
                }
            }
            else if (matchValue_1_1 == null) {
                const v1_1_1: Protocol = value_3(matchValue_3);
                return v1_1_1;
            }
            else {
                const v1_2: Protocol = value_3(matchValue_3);
                const v2_2: Protocol = value_3(matchValue_1_1);
                const this$_5: Update_UpdateOptions_$union = updateOptions_2;
                const recordType_1_5: Protocol = v1_2;
                const recordType_2_5: Protocol = v2_2;
                switch (this$_5.tag) {
                    case /* UpdateAllAppendLists */ 2:
                        return makeRecord(Protocol_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5))) as Protocol;
                    case /* UpdateByExisting */ 1:
                        return makeRecord(Protocol_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5))) as Protocol;
                    case /* UpdateByExistingAppendLists */ 3:
                        return makeRecord(Protocol_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5))) as Protocol;
                    default:
                        return recordType_2_5;
                }
            }
        }, List_distinct<Option<string>>(append<Option<string>>(map<Protocol, Option<string>>(mapping_6, list1_1), map<Protocol, Option<string>>(mapping_6, list2_1)), {
            Equals: equals,
            GetHashCode: structuralHash,
        }));
    }
    catch (err_1: any) {
        throw new Error(`Could not mergeUpdate ${"Protocol"} list: 
${err_1.message}`);
    }
}

/**
 * Returns Characteristics of the study
 */
export function Study_getCharacteristics_Z27CB2981(study: Study): FSharpList<MaterialAttribute> {
    return List_distinct<MaterialAttribute>(append(getCharacteristics(Study_getProcesses_Z27CB2981(study)), append(collect<Assay, MaterialAttribute>(Assay_getCharacteristics_Z269B5B97, Study_getAssays_Z27CB2981(study)), defaultArg(study.CharacteristicCategories, empty<MaterialAttribute>()))), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns factors of the study
 */
export function Study_getFactors_Z27CB2981(study: Study): FSharpList<Factor> {
    return List_distinct<Factor>(append(getFactors(Study_getProcesses_Z27CB2981(study)), append(collect<Assay, Factor>(Assay_getFactors_Z269B5B97, Study_getAssays_Z27CB2981(study)), defaultArg(study.Factors, empty<Factor>()))), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns unit categories of the study
 */
export function Study_getUnitCategories_Z27CB2981(study: Study): FSharpList<OntologyAnnotation> {
    return List_distinct<OntologyAnnotation>(append(getUnits(Study_getProcesses_Z27CB2981(study)), append(collect<Assay, OntologyAnnotation>(Assay_getUnitCategories_Z269B5B97, Study_getAssays_Z27CB2981(study)), defaultArg(study.UnitCategories, empty<OntologyAnnotation>()))), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns sources of the study
 */
export function Study_getSources_Z27CB2981(study: Study): FSharpList<Source> {
    const processSequenceSources: FSharpList<Source> = getSources(Study_getProcesses_Z27CB2981(study));
    const assaysSources: FSharpList<Source> = collect<Assay, Source>(Assay_getSources_Z269B5B97, Study_getAssays_Z27CB2981(study));
    const updateOptions_2: Update_UpdateOptions_$union = Update_UpdateOptions_UpdateByExistingAppendLists();
    const mapping_6 = (s_2: Source): string => defaultArg(s_2.Name, "");
    let list1_1: FSharpList<Source>;
    const matchValue: Option<StudyMaterials> = study.Materials;
    list1_1 = ((matchValue == null) ? empty<Source>() : defaultArg(value_3(matchValue).Sources, empty<Source>()));
    let list2_1: FSharpList<Source>;
    const updateOptions: Update_UpdateOptions_$union = Update_UpdateOptions_UpdateByExistingAppendLists();
    const mapping_1 = (s: Source): string => defaultArg(s.Name, "");
    const list1: FSharpList<Source> = assaysSources;
    const list2: FSharpList<Source> = processSequenceSources;
    try {
        const map1: IMap<string, Source> = Dict_ofSeqWithMerge<Source, string>((arg_1: Source, arg_1_1: Source): Source => {
            const this$: Update_UpdateOptions_$union = updateOptions;
            const recordType_1: Source = arg_1;
            const recordType_2: Source = arg_1_1;
            switch (this$.tag) {
                case /* UpdateAllAppendLists */ 2:
                    return makeRecord(Source_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Source;
                case /* UpdateByExisting */ 1:
                    return makeRecord(Source_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Source;
                case /* UpdateByExistingAppendLists */ 3:
                    return makeRecord(Source_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Source;
                default:
                    return recordType_2;
            }
        }, map<Source, [string, Source]>((v: Source): [string, Source] => ([mapping_1(v), v] as [string, Source]), list1));
        const map2: IMap<string, Source> = Dict_ofSeqWithMerge<Source, string>((arg_2: Source, arg_3: Source): Source => {
            const this$_1: Update_UpdateOptions_$union = updateOptions;
            const recordType_1_1: Source = arg_2;
            const recordType_2_1: Source = arg_3;
            switch (this$_1.tag) {
                case /* UpdateAllAppendLists */ 2:
                    return makeRecord(Source_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1))) as Source;
                case /* UpdateByExisting */ 1:
                    return makeRecord(Source_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1))) as Source;
                case /* UpdateByExistingAppendLists */ 3:
                    return makeRecord(Source_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1))) as Source;
                default:
                    return recordType_2_1;
            }
        }, map<Source, [string, Source]>((v_1: Source): [string, Source] => ([mapping_1(v_1), v_1] as [string, Source]), list2));
        list2_1 = map<string, Source>((k: string): Source => {
            const matchValue_1: Option<Source> = Dict_tryFind<string, Source>(k, map1);
            const matchValue_1_1: Option<Source> = Dict_tryFind<string, Source>(k, map2);
            if (matchValue_1 == null) {
                if (matchValue_1_1 == null) {
                    throw new Error("If this fails, then I don\'t know how to program");
                }
                else {
                    const v2_1: Source = value_3(matchValue_1_1);
                    return v2_1;
                }
            }
            else if (matchValue_1_1 == null) {
                const v1_1: Source = value_3(matchValue_1);
                return v1_1;
            }
            else {
                const v1: Source = value_3(matchValue_1);
                const v2: Source = value_3(matchValue_1_1);
                const this$_2: Update_UpdateOptions_$union = updateOptions;
                const recordType_1_2: Source = v1;
                const recordType_2_2: Source = v2;
                switch (this$_2.tag) {
                    case /* UpdateAllAppendLists */ 2:
                        return makeRecord(Source_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2))) as Source;
                    case /* UpdateByExisting */ 1:
                        return makeRecord(Source_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2))) as Source;
                    case /* UpdateByExistingAppendLists */ 3:
                        return makeRecord(Source_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2))) as Source;
                    default:
                        return recordType_2_2;
                }
            }
        }, List_distinct<string>(append<string>(map<Source, string>(mapping_1, list1), map<Source, string>(mapping_1, list2)), {
            Equals: (x: string, y: string): boolean => (x === y),
            GetHashCode: stringHash,
        }));
    }
    catch (err: any) {
        throw new Error(`Could not mergeUpdate ${"Source"} list: 
${err.message}`);
    }
    try {
        const map1_1: IMap<string, Source> = Dict_ofSeqWithMerge<Source, string>((arg_6: Source, arg_1_2: Source): Source => {
            const this$_3: Update_UpdateOptions_$union = updateOptions_2;
            const recordType_1_3: Source = arg_6;
            const recordType_2_3: Source = arg_1_2;
            switch (this$_3.tag) {
                case /* UpdateAllAppendLists */ 2:
                    return makeRecord(Source_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3))) as Source;
                case /* UpdateByExisting */ 1:
                    return makeRecord(Source_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3))) as Source;
                case /* UpdateByExistingAppendLists */ 3:
                    return makeRecord(Source_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3))) as Source;
                default:
                    return recordType_2_3;
            }
        }, map<Source, [string, Source]>((v_2: Source): [string, Source] => ([mapping_6(v_2), v_2] as [string, Source]), list1_1));
        const map2_1: IMap<string, Source> = Dict_ofSeqWithMerge<Source, string>((arg_2_1: Source, arg_3_1: Source): Source => {
            const this$_4: Update_UpdateOptions_$union = updateOptions_2;
            const recordType_1_4: Source = arg_2_1;
            const recordType_2_4: Source = arg_3_1;
            switch (this$_4.tag) {
                case /* UpdateAllAppendLists */ 2:
                    return makeRecord(Source_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4))) as Source;
                case /* UpdateByExisting */ 1:
                    return makeRecord(Source_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4))) as Source;
                case /* UpdateByExistingAppendLists */ 3:
                    return makeRecord(Source_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4))) as Source;
                default:
                    return recordType_2_4;
            }
        }, map<Source, [string, Source]>((v_1_1: Source): [string, Source] => ([mapping_6(v_1_1), v_1_1] as [string, Source]), list2_1));
        return map<string, Source>((k_1: string): Source => {
            const matchValue_3: Option<Source> = Dict_tryFind<string, Source>(k_1, map1_1);
            const matchValue_1_2: Option<Source> = Dict_tryFind<string, Source>(k_1, map2_1);
            if (matchValue_3 == null) {
                if (matchValue_1_2 == null) {
                    throw new Error("If this fails, then I don\'t know how to program");
                }
                else {
                    const v2_1_1: Source = value_3(matchValue_1_2);
                    return v2_1_1;
                }
            }
            else if (matchValue_1_2 == null) {
                const v1_1_1: Source = value_3(matchValue_3);
                return v1_1_1;
            }
            else {
                const v1_2: Source = value_3(matchValue_3);
                const v2_2: Source = value_3(matchValue_1_2);
                const this$_5: Update_UpdateOptions_$union = updateOptions_2;
                const recordType_1_5: Source = v1_2;
                const recordType_2_5: Source = v2_2;
                switch (this$_5.tag) {
                    case /* UpdateAllAppendLists */ 2:
                        return makeRecord(Source_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5))) as Source;
                    case /* UpdateByExisting */ 1:
                        return makeRecord(Source_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5))) as Source;
                    case /* UpdateByExistingAppendLists */ 3:
                        return makeRecord(Source_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5))) as Source;
                    default:
                        return recordType_2_5;
                }
            }
        }, List_distinct<string>(append<string>(map<Source, string>(mapping_6, list1_1), map<Source, string>(mapping_6, list2_1)), {
            Equals: (x_1: string, y_1: string): boolean => (x_1 === y_1),
            GetHashCode: stringHash,
        }));
    }
    catch (err_1: any) {
        throw new Error(`Could not mergeUpdate ${"Source"} list: 
${err_1.message}`);
    }
}

/**
 * Returns sources of the study
 */
export function Study_getSamples_Z27CB2981(study: Study): FSharpList<Sample> {
    const processSequenceSamples: FSharpList<Sample> = getSamples(Study_getProcesses_Z27CB2981(study));
    const assaysSamples: FSharpList<Sample> = collect<Assay, Sample>(Assay_getSamples_Z269B5B97, Study_getAssays_Z27CB2981(study));
    const updateOptions_2: Update_UpdateOptions_$union = Update_UpdateOptions_UpdateByExistingAppendLists();
    const mapping_6 = (s_2: Sample): string => defaultArg(s_2.Name, "");
    let list1_1: FSharpList<Sample>;
    const matchValue: Option<StudyMaterials> = study.Materials;
    list1_1 = ((matchValue == null) ? empty<Sample>() : defaultArg(value_3(matchValue).Samples, empty<Sample>()));
    let list2_1: FSharpList<Sample>;
    const updateOptions: Update_UpdateOptions_$union = Update_UpdateOptions_UpdateByExistingAppendLists();
    const mapping_1 = (s: Sample): string => defaultArg(s.Name, "");
    const list1: FSharpList<Sample> = assaysSamples;
    const list2: FSharpList<Sample> = processSequenceSamples;
    try {
        const map1: IMap<string, Sample> = Dict_ofSeqWithMerge<Sample, string>((arg_1: Sample, arg_1_1: Sample): Sample => {
            const this$: Update_UpdateOptions_$union = updateOptions;
            const recordType_1: Sample = arg_1;
            const recordType_2: Sample = arg_1_1;
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
        }, map<Sample, [string, Sample]>((v: Sample): [string, Sample] => ([mapping_1(v), v] as [string, Sample]), list1));
        const map2: IMap<string, Sample> = Dict_ofSeqWithMerge<Sample, string>((arg_2: Sample, arg_3: Sample): Sample => {
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
        }, map<Sample, [string, Sample]>((v_1: Sample): [string, Sample] => ([mapping_1(v_1), v_1] as [string, Sample]), list2));
        list2_1 = map<string, Sample>((k: string): Sample => {
            const matchValue_1: Option<Sample> = Dict_tryFind<string, Sample>(k, map1);
            const matchValue_1_1: Option<Sample> = Dict_tryFind<string, Sample>(k, map2);
            if (matchValue_1 == null) {
                if (matchValue_1_1 == null) {
                    throw new Error("If this fails, then I don\'t know how to program");
                }
                else {
                    const v2_1: Sample = value_3(matchValue_1_1);
                    return v2_1;
                }
            }
            else if (matchValue_1_1 == null) {
                const v1_1: Sample = value_3(matchValue_1);
                return v1_1;
            }
            else {
                const v1: Sample = value_3(matchValue_1);
                const v2: Sample = value_3(matchValue_1_1);
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
        }, List_distinct<string>(append<string>(map<Sample, string>(mapping_1, list1), map<Sample, string>(mapping_1, list2)), {
            Equals: (x: string, y: string): boolean => (x === y),
            GetHashCode: stringHash,
        }));
    }
    catch (err: any) {
        throw new Error(`Could not mergeUpdate ${"Sample"} list: 
${err.message}`);
    }
    try {
        const map1_1: IMap<string, Sample> = Dict_ofSeqWithMerge<Sample, string>((arg_6: Sample, arg_1_2: Sample): Sample => {
            const this$_3: Update_UpdateOptions_$union = updateOptions_2;
            const recordType_1_3: Sample = arg_6;
            const recordType_2_3: Sample = arg_1_2;
            switch (this$_3.tag) {
                case /* UpdateAllAppendLists */ 2:
                    return makeRecord(Sample_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3))) as Sample;
                case /* UpdateByExisting */ 1:
                    return makeRecord(Sample_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3))) as Sample;
                case /* UpdateByExistingAppendLists */ 3:
                    return makeRecord(Sample_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_3), getRecordFields(recordType_2_3))) as Sample;
                default:
                    return recordType_2_3;
            }
        }, map<Sample, [string, Sample]>((v_2: Sample): [string, Sample] => ([mapping_6(v_2), v_2] as [string, Sample]), list1_1));
        const map2_1: IMap<string, Sample> = Dict_ofSeqWithMerge<Sample, string>((arg_2_1: Sample, arg_3_1: Sample): Sample => {
            const this$_4: Update_UpdateOptions_$union = updateOptions_2;
            const recordType_1_4: Sample = arg_2_1;
            const recordType_2_4: Sample = arg_3_1;
            switch (this$_4.tag) {
                case /* UpdateAllAppendLists */ 2:
                    return makeRecord(Sample_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4))) as Sample;
                case /* UpdateByExisting */ 1:
                    return makeRecord(Sample_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4))) as Sample;
                case /* UpdateByExistingAppendLists */ 3:
                    return makeRecord(Sample_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_4), getRecordFields(recordType_2_4))) as Sample;
                default:
                    return recordType_2_4;
            }
        }, map<Sample, [string, Sample]>((v_1_1: Sample): [string, Sample] => ([mapping_6(v_1_1), v_1_1] as [string, Sample]), list2_1));
        return map<string, Sample>((k_1: string): Sample => {
            const matchValue_3: Option<Sample> = Dict_tryFind<string, Sample>(k_1, map1_1);
            const matchValue_1_2: Option<Sample> = Dict_tryFind<string, Sample>(k_1, map2_1);
            if (matchValue_3 == null) {
                if (matchValue_1_2 == null) {
                    throw new Error("If this fails, then I don\'t know how to program");
                }
                else {
                    const v2_1_1: Sample = value_3(matchValue_1_2);
                    return v2_1_1;
                }
            }
            else if (matchValue_1_2 == null) {
                const v1_1_1: Sample = value_3(matchValue_3);
                return v1_1_1;
            }
            else {
                const v1_2: Sample = value_3(matchValue_3);
                const v2_2: Sample = value_3(matchValue_1_2);
                const this$_5: Update_UpdateOptions_$union = updateOptions_2;
                const recordType_1_5: Sample = v1_2;
                const recordType_2_5: Sample = v2_2;
                switch (this$_5.tag) {
                    case /* UpdateAllAppendLists */ 2:
                        return makeRecord(Sample_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5))) as Sample;
                    case /* UpdateByExisting */ 1:
                        return makeRecord(Sample_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5))) as Sample;
                    case /* UpdateByExistingAppendLists */ 3:
                        return makeRecord(Sample_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_5), getRecordFields(recordType_2_5))) as Sample;
                    default:
                        return recordType_2_5;
                }
            }
        }, List_distinct<string>(append<string>(map<Sample, string>(mapping_6, list1_1), map<Sample, string>(mapping_6, list2_1)), {
            Equals: (x_1: string, y_1: string): boolean => (x_1 === y_1),
            GetHashCode: stringHash,
        }));
    }
    catch (err_1: any) {
        throw new Error(`Could not mergeUpdate ${"Sample"} list: 
${err_1.message}`);
    }
}

/**
 * Returns materials of the study
 */
export function Study_getMaterials_Z27CB2981(study: Study): StudyMaterials {
    const processSequenceMaterials: FSharpList<Material> = getMaterials(Study_getProcesses_Z27CB2981(study));
    const assaysMaterials: FSharpList<Material> = collect<Assay, Material>((arg_2: Assay): FSharpList<Material> => AssayMaterials_getMaterials_E3447B1(Assay_getMaterials_Z269B5B97(arg_2)), Study_getAssays_Z27CB2981(study));
    let materials: FSharpList<Material>;
    const updateOptions_2: Update_UpdateOptions_$union = Update_UpdateOptions_UpdateByExistingAppendLists();
    const mapping_6 = (s_2: Material): Option<string> => s_2.Name;
    let list1_1: FSharpList<Material>;
    const matchValue: Option<StudyMaterials> = study.Materials;
    list1_1 = ((matchValue == null) ? empty<Material>() : defaultArg(value_3(matchValue).OtherMaterials, empty<Material>()));
    let list2_1: FSharpList<Material>;
    const updateOptions: Update_UpdateOptions_$union = Update_UpdateOptions_UpdateByExistingAppendLists();
    const mapping_1 = (s: Material): Option<string> => s.Name;
    const list1: FSharpList<Material> = assaysMaterials;
    const list2: FSharpList<Material> = processSequenceMaterials;
    try {
        const map1: IMap<Option<string>, Material> = Dict_ofSeqWithMerge<Material, Option<string>>((arg_3: Material, arg_1_1: Material): Material => {
            const this$: Update_UpdateOptions_$union = updateOptions;
            const recordType_1: Material = arg_3;
            const recordType_2: Material = arg_1_1;
            switch (this$.tag) {
                case /* UpdateAllAppendLists */ 2:
                    return makeRecord(Material_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Material;
                case /* UpdateByExisting */ 1:
                    return makeRecord(Material_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Material;
                case /* UpdateByExistingAppendLists */ 3:
                    return makeRecord(Material_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Material;
                default:
                    return recordType_2;
            }
        }, map<Material, [Option<string>, Material]>((v: Material): [Option<string>, Material] => ([mapping_1(v), v] as [Option<string>, Material]), list1));
        const map2: IMap<Option<string>, Material> = Dict_ofSeqWithMerge<Material, Option<string>>((arg_2_1: Material, arg_3_1: Material): Material => {
            const this$_1: Update_UpdateOptions_$union = updateOptions;
            const recordType_1_1: Material = arg_2_1;
            const recordType_2_1: Material = arg_3_1;
            switch (this$_1.tag) {
                case /* UpdateAllAppendLists */ 2:
                    return makeRecord(Material_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1))) as Material;
                case /* UpdateByExisting */ 1:
                    return makeRecord(Material_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1))) as Material;
                case /* UpdateByExistingAppendLists */ 3:
                    return makeRecord(Material_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_1), getRecordFields(recordType_2_1))) as Material;
                default:
                    return recordType_2_1;
            }
        }, map<Material, [Option<string>, Material]>((v_1: Material): [Option<string>, Material] => ([mapping_1(v_1), v_1] as [Option<string>, Material]), list2));
        list2_1 = map<Option<string>, Material>((k: Option<string>): Material => {
            const matchValue_1: Option<Material> = Dict_tryFind<Option<string>, Material>(k, map1);
            const matchValue_1_1: Option<Material> = Dict_tryFind<Option<string>, Material>(k, map2);
            if (matchValue_1 == null) {
                if (matchValue_1_1 == null) {
                    throw new Error("If this fails, then I don\'t know how to program");
                }
                else {
                    const v2_1: Material = value_3(matchValue_1_1);
                    return v2_1;
                }
            }
            else if (matchValue_1_1 == null) {
                const v1_1: Material = value_3(matchValue_1);
                return v1_1;
            }
            else {
                const v1: Material = value_3(matchValue_1);
                const v2: Material = value_3(matchValue_1_1);
                const this$_2: Update_UpdateOptions_$union = updateOptions;
                const recordType_1_2: Material = v1;
                const recordType_2_2: Material = v2;
                switch (this$_2.tag) {
                    case /* UpdateAllAppendLists */ 2:
                        return makeRecord(Material_$reflection(), map2_2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2))) as Material;
                    case /* UpdateByExisting */ 1:
                        return makeRecord(Material_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2))) as Material;
                    case /* UpdateByExistingAppendLists */ 3:
                        return makeRecord(Material_$reflection(), map2_2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1_2), getRecordFields(recordType_2_2))) as Material;
                    default:
                        return recordType_2_2;
                }
            }
        }, List_distinct<Option<string>>(append<Option<string>>(map<Material, Option<string>>(mapping_1, list1), map<Material, Option<string>>(mapping_1, list2)), {
            Equals: equals,
            GetHashCode: structuralHash,
        }));
    }
    catch (err: any) {
        throw new Error(`Could not mergeUpdate ${"Material"} list: 
${err.message}`);
    }
    try {
        const map1_1: IMap<Option<string>, Material> = Dict_ofSeqWithMerge<Material, Option<string>>((arg_6: Material, arg_1_2: Material): Material => {
            const this$_3: Update_UpdateOptions_$union = updateOptions_2;
            const recordType_1_3: Material = arg_6;
            const recordType_2_3: Material = arg_1_2;
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
        }, map<Material, [Option<string>, Material]>((v_2: Material): [Option<string>, Material] => ([mapping_6(v_2), v_2] as [Option<string>, Material]), list1_1));
        const map2_1: IMap<Option<string>, Material> = Dict_ofSeqWithMerge<Material, Option<string>>((arg_2_2: Material, arg_3_2: Material): Material => {
            const this$_4: Update_UpdateOptions_$union = updateOptions_2;
            const recordType_1_4: Material = arg_2_2;
            const recordType_2_4: Material = arg_3_2;
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
        }, map<Material, [Option<string>, Material]>((v_1_1: Material): [Option<string>, Material] => ([mapping_6(v_1_1), v_1_1] as [Option<string>, Material]), list2_1));
        materials = map<Option<string>, Material>((k_1: Option<string>): Material => {
            const matchValue_3: Option<Material> = Dict_tryFind<Option<string>, Material>(k_1, map1_1);
            const matchValue_1_2: Option<Material> = Dict_tryFind<Option<string>, Material>(k_1, map2_1);
            if (matchValue_3 == null) {
                if (matchValue_1_2 == null) {
                    throw new Error("If this fails, then I don\'t know how to program");
                }
                else {
                    const v2_1_1: Material = value_3(matchValue_1_2);
                    return v2_1_1;
                }
            }
            else if (matchValue_1_2 == null) {
                const v1_1_1: Material = value_3(matchValue_3);
                return v1_1_1;
            }
            else {
                const v1_2: Material = value_3(matchValue_3);
                const v2_2: Material = value_3(matchValue_1_2);
                const this$_5: Update_UpdateOptions_$union = updateOptions_2;
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
        }, List_distinct<Option<string>>(append<Option<string>>(map<Material, Option<string>>(mapping_6, list1_1), map<Material, Option<string>>(mapping_6, list2_1)), {
            Equals: equals,
            GetHashCode: structuralHash,
        }));
    }
    catch (err_1: any) {
        throw new Error(`Could not mergeUpdate ${"Material"} list: 
${err_1.message}`);
    }
    const sources: FSharpList<Source> = Study_getSources_Z27CB2981(study);
    const samples: FSharpList<Sample> = Study_getSamples_Z27CB2981(study);
    return StudyMaterials_make(fromValueWithDefault<FSharpList<Source>>(empty<Source>(), sources), fromValueWithDefault<FSharpList<Sample>>(empty<Sample>(), samples), fromValueWithDefault<FSharpList<Material>>(empty<Material>(), materials));
}

export function Study_update_Z27CB2981(study: Study): Study {
    try {
        const protocols: FSharpList<Protocol> = Study_getProtocols_Z27CB2981(study);
        let Materials: Option<StudyMaterials>;
        const v: StudyMaterials = Study_getMaterials_Z27CB2981(study);
        Materials = fromValueWithDefault<StudyMaterials>(StudyMaterials_get_empty(), v);
        const Assays: Option<FSharpList<Assay>> = map_1<FSharpList<Assay>, FSharpList<Assay>>((list: FSharpList<Assay>): FSharpList<Assay> => map<Assay, Assay>((arg_3: Assay): Assay => ((arg_2: Assay): Assay => Assay_updateProtocols(protocols, arg_2))(Assay_update_Z269B5B97(arg_3)), list), study.Assays);
        const Protocols: Option<FSharpList<Protocol>> = fromValueWithDefault<FSharpList<Protocol>>(empty<Protocol>(), protocols);
        const Factors: Option<FSharpList<Factor>> = fromValueWithDefault<FSharpList<Factor>>(empty<Factor>(), Study_getFactors_Z27CB2981(study));
        const CharacteristicCategories: Option<FSharpList<MaterialAttribute>> = fromValueWithDefault<FSharpList<MaterialAttribute>>(empty<MaterialAttribute>(), Study_getCharacteristics_Z27CB2981(study));
        const UnitCategories: Option<FSharpList<OntologyAnnotation>> = fromValueWithDefault<FSharpList<OntologyAnnotation>>(empty<OntologyAnnotation>(), Study_getUnitCategories_Z27CB2981(study));
        return new Study(study.ID, study.FileName, study.Identifier, study.Title, study.Description, study.SubmissionDate, study.PublicReleaseDate, study.Publications, study.Contacts, study.StudyDesignDescriptors, Protocols, Materials, map_1<FSharpList<Process>, FSharpList<Process>>((processSequence: FSharpList<Process>): FSharpList<Process> => updateProtocols(protocols, processSequence), study.ProcessSequence), Assays, Factors, CharacteristicCategories, UnitCategories, study.Comments);
    }
    catch (err: any) {
        return toFail(`Could not update study ${study.Identifier}: 
${err.message}`);
    }
}

