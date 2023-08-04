import { toString, nil, object as object_36 } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { FSharpList, ofArray, choose } from "../../fable_modules/fable-library-ts/List.js";
import { uncurry2, equals } from "../../fable_modules/fable-library-ts/Util.js";
import { value as value_25, Option } from "../../fable_modules/fable-library-ts/Option.js";
import { tryInclude } from "./GEncode.js";
import { Sample_decoder, Source_decoder, Sample_encoder, Source_encoder } from "./Data.js";
import { MaterialAttribute_decoder, MaterialAttribute_encoder, Material_decoder, Material_encoder } from "./Material.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions_$ctor, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID, ConverterOptions } from "./ConverterOptions.js";
import { string, IOptionalGetter, IGetters, list as list_1, object as object_37 } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { FSharpResult$2_$union } from "../../fable_modules/fable-library-ts/Choice.js";
import { Source } from "../ISA/JsonTypes/Source.js";
import { ErrorReason_$union } from "../../fable_modules/Thoth.Json.10.1.0/Types.fs.js";
import { Sample } from "../ISA/JsonTypes/Sample.js";
import { Material } from "../ISA/JsonTypes/Material.js";
import { StudyMaterials } from "../ISA/JsonTypes/StudyMaterials.js";
import { replace } from "../../fable_modules/fable-library-ts/String.js";
import { URIModule_toString } from "../ISA/JsonTypes/URI.js";
import { Study } from "../ISA/JsonTypes/Study.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library-ts/Seq.js";
import { decoder as decoder_10, encoder } from "./Publication.js";
import { decoder as decoder_11, encoder as encoder_1 } from "./Person.js";
import { OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "./Ontology.js";
import { Protocol_decoder, Protocol_encoder } from "./Protocol.js";
import { Process_decoder, Process_encoder } from "./Process.js";
import { Assay_decoder, Assay_encoder } from "./Assay.js";
import { Factor_decoder, Factor_encoder } from "./Factor.js";
import { decoder as decoder_12, encoder as encoder_2 } from "./Comment.js";
import { fromString, uri } from "./Decode.js";
import { Publication } from "../ISA/JsonTypes/Publication.js";
import { Person } from "../ISA/JsonTypes/Person.js";
import { OntologyAnnotation } from "../ISA/JsonTypes/OntologyAnnotation.js";
import { Protocol } from "../ISA/JsonTypes/Protocol.js";
import { Assay } from "../ISA/JsonTypes/Assay.js";
import { Factor } from "../ISA/JsonTypes/Factor.js";
import { MaterialAttribute } from "../ISA/JsonTypes/MaterialAttribute.js";
import { Process } from "../ISA/JsonTypes/Process.js";
import { Comment$ } from "../ISA/JsonTypes/Comment.js";
import { ArcStudy } from "../ISA/ArcTypes/ArcStudy.js";

export function StudyMaterials_encoder(options: ConverterOptions, oa: any): any {
    return object_36(choose<[string, any], [string, any]>((tupledArg: [string, any]): Option<[string, any]> => {
        const v: any = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v] as [string, any];
        }
    }, ofArray([tryInclude<string>("sources", (oa_1: any): any => Source_encoder(options, oa_1), oa["Sources"]), tryInclude<string>("samples", (oa_2: any): any => Sample_encoder(options, oa_2), oa["Samples"]), tryInclude<string>("otherMaterials", (oa_3: any): any => Material_encoder(options, oa_3), oa["OtherMaterials"])])));
}

export function StudyMaterials_decoder(options: ConverterOptions): ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<StudyMaterials, [string, ErrorReason_$union]>)) {
    return (path_3: string): ((arg0: any) => FSharpResult$2_$union<StudyMaterials, [string, ErrorReason_$union]>) => ((v: any): FSharpResult$2_$union<StudyMaterials, [string, ErrorReason_$union]> => object_37<StudyMaterials>((get$: IGetters): StudyMaterials => {
        let objectArg: IOptionalGetter, objectArg_1: IOptionalGetter, objectArg_2: IOptionalGetter;
        return new StudyMaterials((objectArg = get$.Optional, objectArg.Field<FSharpList<Source>>("sources", (path: string, value: any): FSharpResult$2_$union<FSharpList<Source>, [string, ErrorReason_$union]> => list_1<Source>((s: string, json: any): FSharpResult$2_$union<Source, [string, ErrorReason_$union]> => Source_decoder(options, s, json), path, value))), (objectArg_1 = get$.Optional, objectArg_1.Field<FSharpList<Sample>>("samples", (path_1: string, value_1: any): FSharpResult$2_$union<FSharpList<Sample>, [string, ErrorReason_$union]> => list_1<Sample>((s_1: string, json_1: any): FSharpResult$2_$union<Sample, [string, ErrorReason_$union]> => Sample_decoder(options, s_1, json_1), path_1, value_1))), (objectArg_2 = get$.Optional, objectArg_2.Field<FSharpList<Material>>("otherMaterials", (path_2: string, value_2: any): FSharpResult$2_$union<FSharpList<Material>, [string, ErrorReason_$union]> => list_1<Material>((s_2: string, json_2: any): FSharpResult$2_$union<Material, [string, ErrorReason_$union]> => Material_decoder(options, s_2, json_2), path_2, value_2))));
    }, path_3, v));
}

export function Study_genID(s: Study): string {
    const matchValue: Option<string> = s.ID;
    if (matchValue == null) {
        const matchValue_1: Option<string> = s.FileName;
        if (matchValue_1 == null) {
            const matchValue_2: Option<string> = s.Identifier;
            if (matchValue_2 == null) {
                const matchValue_3: Option<string> = s.Title;
                if (matchValue_3 == null) {
                    return "#EmptyStudy";
                }
                else {
                    return "#Study_" + replace(value_25(matchValue_3), " ", "_");
                }
            }
            else {
                return "#Study_" + replace(value_25(matchValue_2), " ", "_");
            }
        }
        else {
            return "#Study" + replace(value_25(matchValue_1), " ", "_");
        }
    }
    else {
        return URIModule_toString(value_25(matchValue));
    }
}

export function Study_encoder(options: ConverterOptions, oa: any): any {
    return object_36(choose<[string, any], [string, any]>((tupledArg: [string, any]): Option<[string, any]> => {
        const v: any = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v] as [string, any];
        }
    }, toList<[string, any]>(delay<[string, any]>((): Iterable<[string, any]> => {
        let value: any, s_1: string;
        return append<[string, any]>(ConverterOptions__get_SetID(options) ? singleton<[string, any]>(["@id", (value = Study_genID(oa as Study), (typeof value === "string") ? ((s_1 = (value as string), s_1)) : nil)] as [string, any]) : singleton<[string, any]>(tryInclude<string>("@id", (value_2: any): any => {
            let s_2: string;
            const value_3: any = value_2;
            return (typeof value_3 === "string") ? ((s_2 = (value_3 as string), s_2)) : nil;
        }, oa["ID"])), delay<[string, any]>((): Iterable<[string, any]> => {
            let value_5: any, s_3: string;
            return append<[string, any]>(ConverterOptions__get_IncludeType(options) ? singleton<[string, any]>(["@type", (value_5 = "Study", (typeof value_5 === "string") ? ((s_3 = (value_5 as string), s_3)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("filename", (value_7: any): any => {
                let s_4: string;
                const value_8: any = value_7;
                return (typeof value_8 === "string") ? ((s_4 = (value_8 as string), s_4)) : nil;
            }, oa["FileName"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("identifier", (value_10: any): any => {
                let s_5: string;
                const value_11: any = value_10;
                return (typeof value_11 === "string") ? ((s_5 = (value_11 as string), s_5)) : nil;
            }, oa["Identifier"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("title", (value_13: any): any => {
                let s_6: string;
                const value_14: any = value_13;
                return (typeof value_14 === "string") ? ((s_6 = (value_14 as string), s_6)) : nil;
            }, oa["Title"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("description", (value_16: any): any => {
                let s_7: string;
                const value_17: any = value_16;
                return (typeof value_17 === "string") ? ((s_7 = (value_17 as string), s_7)) : nil;
            }, oa["Description"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("submissionDate", (value_19: any): any => {
                let s_8: string;
                const value_20: any = value_19;
                return (typeof value_20 === "string") ? ((s_8 = (value_20 as string), s_8)) : nil;
            }, oa["SubmissionDate"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("publicReleaseDate", (value_22: any): any => {
                let s_9: string;
                const value_23: any = value_22;
                return (typeof value_23 === "string") ? ((s_9 = (value_23 as string), s_9)) : nil;
            }, oa["PublicReleaseDate"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("publications", (oa_1: any): any => encoder(options, oa_1), oa["Publications"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("people", (oa_2: any): any => encoder_1(options, oa_2), oa["Contacts"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("studyDesignDescriptors", (oa_3: any): any => OntologyAnnotation_encoder(options, oa_3), oa["StudyDesignDescriptors"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("protocols", (oa_4: any): any => Protocol_encoder(options, oa_4), oa["Protocols"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("materials", (oa_5: any): any => StudyMaterials_encoder(options, oa_5), oa["Materials"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("processSequence", (oa_6: any): any => Process_encoder(options, oa_6), oa["ProcessSequence"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("assays", (oa_7: any): any => Assay_encoder(options, oa_7), oa["Assays"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("factors", (oa_8: any): any => Factor_encoder(options, oa_8), oa["Factors"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("characteristicCategories", (oa_9: any): any => MaterialAttribute_encoder(options, oa_9), oa["CharacteristicCategories"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("unitCategories", (oa_10: any): any => OntologyAnnotation_encoder(options, oa_10), oa["UnitCategories"])), delay<[string, any]>((): Iterable<[string, any]> => singleton<[string, any]>(tryInclude<string>("comments", (comment: any): any => encoder_2(options, comment), oa["Comments"]))))))))))))))))))))))))))))))))))));
        }));
    }))));
}

export function Study_decoder(options: ConverterOptions): ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Study, [string, ErrorReason_$union]>)) {
    return (path_16: string): ((arg0: any) => FSharpResult$2_$union<Study, [string, ErrorReason_$union]>) => ((v: any): FSharpResult$2_$union<Study, [string, ErrorReason_$union]> => object_37<Study>((get$: IGetters): Study => {
        let arg_33: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<Process>, [string, ErrorReason_$union]>)), decoder_8: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Process, [string, ErrorReason_$union]>)), objectArg_16: IOptionalGetter, arg_35: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]>)), decoder_9: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Comment$, [string, ErrorReason_$union]>)), objectArg_17: IOptionalGetter;
        let ID: Option<string>;
        const objectArg: IOptionalGetter = get$.Optional;
        ID = objectArg.Field<string>("@id", uri);
        let FileName: Option<string>;
        const objectArg_1: IOptionalGetter = get$.Optional;
        FileName = objectArg_1.Field<string>("filename", string);
        let Identifier: Option<string>;
        const objectArg_2: IOptionalGetter = get$.Optional;
        Identifier = objectArg_2.Field<string>("identifier", string);
        let Title: Option<string>;
        const objectArg_3: IOptionalGetter = get$.Optional;
        Title = objectArg_3.Field<string>("title", string);
        let Description: Option<string>;
        const objectArg_4: IOptionalGetter = get$.Optional;
        Description = objectArg_4.Field<string>("description", string);
        let SubmissionDate: Option<string>;
        const objectArg_5: IOptionalGetter = get$.Optional;
        SubmissionDate = objectArg_5.Field<string>("submissionDate", string);
        let PublicReleaseDate: Option<string>;
        const objectArg_6: IOptionalGetter = get$.Optional;
        PublicReleaseDate = objectArg_6.Field<string>("publicReleaseDate", string);
        let Publications: Option<FSharpList<Publication>>;
        let arg_15: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<Publication>, [string, ErrorReason_$union]>));
        const decoder: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Publication, [string, ErrorReason_$union]>)) = decoder_10(options);
        arg_15 = ((path_6: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<Publication>, [string, ErrorReason_$union]>) => ((value_6: any): FSharpResult$2_$union<FSharpList<Publication>, [string, ErrorReason_$union]> => list_1<Publication>(uncurry2(decoder), path_6, value_6)));
        const objectArg_7: IOptionalGetter = get$.Optional;
        Publications = objectArg_7.Field<FSharpList<Publication>>("publications", uncurry2(arg_15));
        let Contacts: Option<FSharpList<Person>>;
        let arg_17: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<Person>, [string, ErrorReason_$union]>));
        const decoder_1: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Person, [string, ErrorReason_$union]>)) = decoder_11(options);
        arg_17 = ((path_7: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<Person>, [string, ErrorReason_$union]>) => ((value_7: any): FSharpResult$2_$union<FSharpList<Person>, [string, ErrorReason_$union]> => list_1<Person>(uncurry2(decoder_1), path_7, value_7)));
        const objectArg_8: IOptionalGetter = get$.Optional;
        Contacts = objectArg_8.Field<FSharpList<Person>>("people", uncurry2(arg_17));
        let StudyDesignDescriptors: Option<FSharpList<OntologyAnnotation>>;
        let arg_19: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<OntologyAnnotation>, [string, ErrorReason_$union]>));
        const decoder_2: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<OntologyAnnotation, [string, ErrorReason_$union]>)) = OntologyAnnotation_decoder(options);
        arg_19 = ((path_8: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<OntologyAnnotation>, [string, ErrorReason_$union]>) => ((value_8: any): FSharpResult$2_$union<FSharpList<OntologyAnnotation>, [string, ErrorReason_$union]> => list_1<OntologyAnnotation>(uncurry2(decoder_2), path_8, value_8)));
        const objectArg_9: IOptionalGetter = get$.Optional;
        StudyDesignDescriptors = objectArg_9.Field<FSharpList<OntologyAnnotation>>("studyDesignDescriptors", uncurry2(arg_19));
        let Protocols: Option<FSharpList<Protocol>>;
        let arg_21: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<Protocol>, [string, ErrorReason_$union]>));
        const decoder_3: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Protocol, [string, ErrorReason_$union]>)) = Protocol_decoder(options);
        arg_21 = ((path_9: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<Protocol>, [string, ErrorReason_$union]>) => ((value_9: any): FSharpResult$2_$union<FSharpList<Protocol>, [string, ErrorReason_$union]> => list_1<Protocol>(uncurry2(decoder_3), path_9, value_9)));
        const objectArg_10: IOptionalGetter = get$.Optional;
        Protocols = objectArg_10.Field<FSharpList<Protocol>>("protocols", uncurry2(arg_21));
        let Materials: Option<StudyMaterials>;
        const arg_23: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<StudyMaterials, [string, ErrorReason_$union]>)) = StudyMaterials_decoder(options);
        const objectArg_11: IOptionalGetter = get$.Optional;
        Materials = objectArg_11.Field<StudyMaterials>("materials", uncurry2(arg_23));
        let Assays: Option<FSharpList<Assay>>;
        let arg_25: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<Assay>, [string, ErrorReason_$union]>));
        const decoder_4: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Assay, [string, ErrorReason_$union]>)) = Assay_decoder(options);
        arg_25 = ((path_10: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<Assay>, [string, ErrorReason_$union]>) => ((value_10: any): FSharpResult$2_$union<FSharpList<Assay>, [string, ErrorReason_$union]> => list_1<Assay>(uncurry2(decoder_4), path_10, value_10)));
        const objectArg_12: IOptionalGetter = get$.Optional;
        Assays = objectArg_12.Field<FSharpList<Assay>>("assays", uncurry2(arg_25));
        let Factors: Option<FSharpList<Factor>>;
        let arg_27: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<Factor>, [string, ErrorReason_$union]>));
        const decoder_5: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Factor, [string, ErrorReason_$union]>)) = Factor_decoder(options);
        arg_27 = ((path_11: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<Factor>, [string, ErrorReason_$union]>) => ((value_11: any): FSharpResult$2_$union<FSharpList<Factor>, [string, ErrorReason_$union]> => list_1<Factor>(uncurry2(decoder_5), path_11, value_11)));
        const objectArg_13: IOptionalGetter = get$.Optional;
        Factors = objectArg_13.Field<FSharpList<Factor>>("factors", uncurry2(arg_27));
        let CharacteristicCategories: Option<FSharpList<MaterialAttribute>>;
        let arg_29: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<MaterialAttribute>, [string, ErrorReason_$union]>));
        const decoder_6: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<MaterialAttribute, [string, ErrorReason_$union]>)) = MaterialAttribute_decoder(options);
        arg_29 = ((path_12: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<MaterialAttribute>, [string, ErrorReason_$union]>) => ((value_12: any): FSharpResult$2_$union<FSharpList<MaterialAttribute>, [string, ErrorReason_$union]> => list_1<MaterialAttribute>(uncurry2(decoder_6), path_12, value_12)));
        const objectArg_14: IOptionalGetter = get$.Optional;
        CharacteristicCategories = objectArg_14.Field<FSharpList<MaterialAttribute>>("characteristicCategories", uncurry2(arg_29));
        let UnitCategories: Option<FSharpList<OntologyAnnotation>>;
        let arg_31: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<OntologyAnnotation>, [string, ErrorReason_$union]>));
        const decoder_7: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<OntologyAnnotation, [string, ErrorReason_$union]>)) = OntologyAnnotation_decoder(options);
        arg_31 = ((path_13: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<OntologyAnnotation>, [string, ErrorReason_$union]>) => ((value_13: any): FSharpResult$2_$union<FSharpList<OntologyAnnotation>, [string, ErrorReason_$union]> => list_1<OntologyAnnotation>(uncurry2(decoder_7), path_13, value_13)));
        const objectArg_15: IOptionalGetter = get$.Optional;
        UnitCategories = objectArg_15.Field<FSharpList<OntologyAnnotation>>("unitCategories", uncurry2(arg_31));
        return new Study(ID, FileName, Identifier, Title, Description, SubmissionDate, PublicReleaseDate, Publications, Contacts, StudyDesignDescriptors, Protocols, Materials, (arg_33 = ((decoder_8 = Process_decoder(options), (path_14: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<Process>, [string, ErrorReason_$union]>) => ((value_14: any): FSharpResult$2_$union<FSharpList<Process>, [string, ErrorReason_$union]> => list_1<Process>(uncurry2(decoder_8), path_14, value_14)))), (objectArg_16 = get$.Optional, objectArg_16.Field<FSharpList<Process>>("processSequence", uncurry2(arg_33)))), Assays, Factors, CharacteristicCategories, UnitCategories, (arg_35 = ((decoder_9 = decoder_12(options), (path_15: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]>) => ((value_15: any): FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]> => list_1<Comment$>(uncurry2(decoder_9), path_15, value_15)))), (objectArg_17 = get$.Optional, objectArg_17.Field<FSharpList<Comment$>>("comments", uncurry2(arg_35)))));
    }, path_16, v));
}

export function Study_fromString(s: string): Study {
    return fromString<Study>(uncurry2(Study_decoder(ConverterOptions_$ctor())), s);
}

export function Study_toString(p: Study): string {
    return toString(2, Study_encoder(ConverterOptions_$ctor(), p));
}

/**
 * exports in json-ld format
 */
export function Study_toStringLD(s: Study): string {
    let returnVal: ConverterOptions;
    return toString(2, Study_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), s));
}

export function ArcStudy_fromString(s: string): ArcStudy {
    const arg: Study = fromString<Study>(uncurry2(Study_decoder(ConverterOptions_$ctor())), s);
    return ArcStudy.fromStudy(arg);
}

export function ArcStudy_toString(a: ArcStudy): string {
    return toString(2, Study_encoder(ConverterOptions_$ctor(), a.ToStudy()));
}

/**
 * exports in json-ld format
 */
export function ArcStudy_toStringLD(a: ArcStudy): string {
    let returnVal: ConverterOptions;
    return toString(2, Study_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), a.ToStudy()));
}

