import { tryParse, int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { FSharpRef } from "../../../fable_modules/fable-library-ts/Types.js";
import { value, bind, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { Comment$, Comment_fromString } from "./Comment.js";
import { int32ToString } from "../../../fable_modules/fable-library-ts/Util.js";
import { tryItem } from "./CommentList.js";
import { cons, singleton, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { OntologyAnnotation } from "./OntologyAnnotation.js";
import { ProtocolParameter_fromString_703AFBF9, ProtocolParameter } from "./ProtocolParameter.js";
import { ProcessParameterValue } from "./ProcessParameterValue.js";
import { Factor_fromString_Z5D76503E, Factor } from "./Factor.js";
import { FactorValue } from "./FactorValue.js";
import { MaterialAttribute_fromString_703AFBF9, MaterialAttribute } from "./MaterialAttribute.js";
import { MaterialAttributeValue } from "./MaterialAttributeValue.js";
import { Component_fromString_55205B02, Component } from "./Component.js";

function tryInt(str: string): Option<int32> {
    let matchValue: [boolean, int32];
    let outArg = 0;
    matchValue = ([tryParse(str, 511, false, 32, new FSharpRef<int32>((): int32 => outArg, (v: int32): void => {
        outArg = (v | 0);
    })), outArg] as [boolean, int32]);
    if (matchValue[0]) {
        return matchValue[1];
    }
    else {
        return void 0;
    }
}

export const orderName = "ColumnIndex";

export function createOrderComment(index: int32): Comment$ {
    return Comment_fromString(orderName, int32ToString(index));
}

export function tryGetIndex(comments: FSharpList<Comment$>): Option<int32> {
    return bind<string, int32>(tryInt, tryItem(orderName, comments));
}

export function setOntologyAnnotationIndex(i: int32, oa: OntologyAnnotation): OntologyAnnotation {
    let matchValue: Option<FSharpList<Comment$>>, cs: FSharpList<Comment$>;
    return new OntologyAnnotation(oa.ID, oa.Name, oa.TermSourceREF, oa.LocalID, oa.TermAccessionNumber, (matchValue = oa.Comments, (matchValue == null) ? singleton(createOrderComment(i)) : ((cs = value(matchValue), cons(createOrderComment(i), cs)))));
}

export function tryGetOntologyAnnotationIndex(oa: OntologyAnnotation): Option<int32> {
    return bind<FSharpList<Comment$>, int32>(tryGetIndex, oa.Comments);
}

export function tryGetParameterIndex(param: ProtocolParameter): Option<int32> {
    return bind<OntologyAnnotation, int32>((oa: OntologyAnnotation): Option<int32> => bind<FSharpList<Comment$>, int32>(tryGetIndex, oa.Comments), param.ParameterName);
}

export function tryGetParameterColumnIndex(paramValue: ProcessParameterValue): Option<int32> {
    return bind<ProtocolParameter, int32>(tryGetParameterIndex, paramValue.Category);
}

export function tryGetFactorIndex(factor: Factor): Option<int32> {
    return bind<OntologyAnnotation, int32>((oa: OntologyAnnotation): Option<int32> => bind<FSharpList<Comment$>, int32>(tryGetIndex, oa.Comments), factor.FactorType);
}

export function tryGetFactorColumnIndex(factorValue: FactorValue): Option<int32> {
    return bind<Factor, int32>(tryGetFactorIndex, factorValue.Category);
}

export function tryGetCharacteristicIndex(characteristic: MaterialAttribute): Option<int32> {
    return bind<OntologyAnnotation, int32>((oa: OntologyAnnotation): Option<int32> => bind<FSharpList<Comment$>, int32>(tryGetIndex, oa.Comments), characteristic.CharacteristicType);
}

export function tryGetCharacteristicColumnIndex(characteristicValue: MaterialAttributeValue): Option<int32> {
    return bind<MaterialAttribute, int32>(tryGetCharacteristicIndex, characteristicValue.Category);
}

export function tryGetComponentIndex(comp: Component): Option<int32> {
    return bind<OntologyAnnotation, int32>((oa: OntologyAnnotation): Option<int32> => bind<FSharpList<Comment$>, int32>(tryGetIndex, oa.Comments), comp.ComponentType);
}

/**
 * Create a ISAJson Factor from ISATab string entries
 */
export function ISA_OntologyAnnotation__OntologyAnnotation_fromStringWithColumnIndex_Static(name: string, term: string, source: string, accession: string, valueIndex: int32): Factor {
    return Factor_fromString_Z5D76503E(name, term, source, accession, singleton(createOrderComment(valueIndex)));
}

export function ISA_OntologyAnnotation__OntologyAnnotation_getColumnIndex_Static_2FC95D30(f: OntologyAnnotation): int32 {
    return value(tryGetOntologyAnnotationIndex(f));
}

export function ISA_OntologyAnnotation__OntologyAnnotation_GetColumnIndex(this$: OntologyAnnotation): int32 {
    return value(tryGetOntologyAnnotationIndex(this$));
}

export function ISA_OntologyAnnotation__OntologyAnnotation_tryGetColumnIndex_Static_2FC95D30(f: OntologyAnnotation): Option<int32> {
    return tryGetOntologyAnnotationIndex(f);
}

export function ISA_OntologyAnnotation__OntologyAnnotation_TryGetColumnIndex(this$: OntologyAnnotation): Option<int32> {
    return tryGetOntologyAnnotationIndex(this$);
}

export function ISA_OntologyAnnotation__OntologyAnnotation_setColumnIndex_Static(i: int32, oa: OntologyAnnotation): OntologyAnnotation {
    return setOntologyAnnotationIndex(i, oa);
}

export function ISA_OntologyAnnotation__OntologyAnnotation_SetColumnIndex_Z524259A4(this$: OntologyAnnotation, i: int32): OntologyAnnotation {
    return setOntologyAnnotationIndex(i, this$);
}

/**
 * Create a ISAJson Factor from ISATab string entries
 */
export function ISA_Factor__Factor_fromStringWithColumnIndex_Static(name: string, term: string, source: string, accession: string, valueIndex: int32): Factor {
    return Factor_fromString_Z5D76503E(name, term, source, accession, singleton(createOrderComment(valueIndex)));
}

export function ISA_Factor__Factor_getColumnIndex_Static_E353FDD(f: Factor): int32 {
    return value(tryGetFactorIndex(f));
}

export function ISA_Factor__Factor_GetColumnIndex(this$: Factor): int32 {
    return value(tryGetFactorIndex(this$));
}

export function ISA_Factor__Factor_tryGetColumnIndex_Static_E353FDD(f: Factor): Option<int32> {
    return tryGetFactorIndex(f);
}

export function ISA_Factor__Factor_TryGetColumnIndex(this$: Factor): Option<int32> {
    return tryGetFactorIndex(this$);
}

export function ISA_FactorValue__FactorValue_getColumnIndex_Static_2A4175B6(f: FactorValue): int32 {
    return value(tryGetFactorColumnIndex(f));
}

export function ISA_FactorValue__FactorValue_GetColumnIndex(this$: FactorValue): int32 {
    return value(tryGetFactorColumnIndex(this$));
}

export function ISA_FactorValue__FactorValue_tryGetColumnIndex_Static_2A4175B6(f: FactorValue): Option<int32> {
    return tryGetFactorColumnIndex(f);
}

export function ISA_FactorValue__FactorValue_TryGetColumnIndex(this$: FactorValue): Option<int32> {
    return tryGetFactorColumnIndex(this$);
}

/**
 * Create a ISAJson characteristic from ISATab string entries
 */
export function ISA_MaterialAttribute__MaterialAttribute_fromStringWithColumnIndex_Static(term: string, source: string, accession: string, valueIndex: int32): MaterialAttribute {
    return MaterialAttribute_fromString_703AFBF9(term, source, accession, singleton(createOrderComment(valueIndex)));
}

export function ISA_MaterialAttribute__MaterialAttribute_getColumnIndex_Static_Z6476E859(m: MaterialAttribute): int32 {
    return value(tryGetCharacteristicIndex(m));
}

export function ISA_MaterialAttribute__MaterialAttribute_GetColumnIndex(this$: MaterialAttribute): int32 {
    return value(tryGetCharacteristicIndex(this$));
}

export function ISA_MaterialAttribute__MaterialAttribute_tryGetColumnIndex_Static_Z6476E859(m: MaterialAttribute): Option<int32> {
    return tryGetCharacteristicIndex(m);
}

export function ISA_MaterialAttribute__MaterialAttribute_TryGetColumnIndex(this$: MaterialAttribute): Option<int32> {
    return tryGetCharacteristicIndex(this$);
}

export function ISA_MaterialAttributeValue__MaterialAttributeValue_getColumnIndex_Static_6A64994C(m: MaterialAttributeValue): int32 {
    return value(tryGetCharacteristicColumnIndex(m));
}

export function ISA_MaterialAttributeValue__MaterialAttributeValue_GetColumnIndex(this$: MaterialAttributeValue): int32 {
    return value(tryGetCharacteristicColumnIndex(this$));
}

export function ISA_MaterialAttributeValue__MaterialAttributeValue_tryGetColumnIndex_Static_6A64994C(m: MaterialAttributeValue): Option<int32> {
    return tryGetCharacteristicColumnIndex(m);
}

export function ISA_MaterialAttributeValue__MaterialAttributeValue_TryGetColumnIndex(this$: MaterialAttributeValue): Option<int32> {
    return tryGetCharacteristicColumnIndex(this$);
}

/**
 * Create a ISAJson parameter from ISATab string entries
 */
export function ISA_ProtocolParameter__ProtocolParameter_fromStringWithColumnIndex_Static(term: string, source: string, accession: string, valueIndex: int32): ProtocolParameter {
    return ProtocolParameter_fromString_703AFBF9(term, source, accession, singleton(createOrderComment(valueIndex)));
}

export function ISA_ProtocolParameter__ProtocolParameter_getColumnIndex_Static_2762A46F(p: ProtocolParameter): int32 {
    return value(tryGetParameterIndex(p));
}

export function ISA_ProtocolParameter__ProtocolParameter_GetColumnIndex(this$: ProtocolParameter): int32 {
    return value(tryGetParameterIndex(this$));
}

export function ISA_ProtocolParameter__ProtocolParameter_tryGetColumnIndex_Static_2762A46F(p: ProtocolParameter): Option<int32> {
    return tryGetParameterIndex(p);
}

export function ISA_ProtocolParameter__ProtocolParameter_TryGetColumnIndex(this$: ProtocolParameter): Option<int32> {
    return tryGetParameterIndex(this$);
}

export function ISA_ProcessParameterValue__ProcessParameterValue_getColumnIndex_Static_39585819(p: ProcessParameterValue): int32 {
    return value(tryGetParameterColumnIndex(p));
}

export function ISA_ProcessParameterValue__ProcessParameterValue_GetColumnIndex(this$: ProcessParameterValue): int32 {
    return value(tryGetParameterColumnIndex(this$));
}

export function ISA_ProcessParameterValue__ProcessParameterValue_tryGetColumnIndex_Static_39585819(p: ProcessParameterValue): Option<int32> {
    return tryGetParameterColumnIndex(p);
}

export function ISA_ProcessParameterValue__ProcessParameterValue_TryGetColumnIndex(this$: ProcessParameterValue): Option<int32> {
    return tryGetParameterColumnIndex(this$);
}

/**
 * Create a ISAJson Factor from ISATab string entries
 */
export function ISA_Component__Component_fromStringWithColumnIndex_Static(name: string, term: string, source: string, accession: string, valueIndex: int32): Component {
    return Component_fromString_55205B02(name, term, source, accession, singleton(createOrderComment(valueIndex)));
}

export function ISA_Component__Component_getColumnIndex_Static_Z7E9B32A1(f: Component): int32 {
    return value(tryGetComponentIndex(f));
}

export function ISA_Component__Component_GetColumnIndex(this$: Component): int32 {
    return value(tryGetComponentIndex(this$));
}

export function ISA_Component__Component_tryGetColumnIndex_Static_Z7E9B32A1(f: Component): Option<int32> {
    return tryGetComponentIndex(f);
}

export function ISA_Component__Component_TryGetColumnIndex(this$: Component): Option<int32> {
    return tryGetComponentIndex(this$);
}

