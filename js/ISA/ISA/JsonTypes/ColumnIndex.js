import { tryParse } from "../../../fable_modules/fable-library.4.1.4/Int32.js";
import { FSharpRef } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { Comment_fromString } from "./Comment.js";
import { int32ToString } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { value, bind } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { tryItem } from "./CommentList.js";
import { cons, singleton } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { OntologyAnnotation } from "./OntologyAnnotation.js";
import { Factor_fromString_Z5D76503E } from "./Factor.js";
import { MaterialAttribute_fromString_703AFBF9 } from "./MaterialAttribute.js";
import { ProtocolParameter_fromString_703AFBF9 } from "./ProtocolParameter.js";
import { Component_fromString_55205B02 } from "./Component.js";

function tryInt(str) {
    let matchValue;
    let outArg = 0;
    matchValue = [tryParse(str, 511, false, 32, new FSharpRef(() => outArg, (v) => {
        outArg = (v | 0);
    })), outArg];
    if (matchValue[0]) {
        return matchValue[1];
    }
    else {
        return void 0;
    }
}

export const orderName = "ColumnIndex";

export function createOrderComment(index) {
    return Comment_fromString(orderName, int32ToString(index));
}

export function tryGetIndex(comments) {
    return bind(tryInt, tryItem(orderName, comments));
}

export function setOntologyAnnotationIndex(i, oa) {
    let matchValue, cs;
    return new OntologyAnnotation(oa.ID, oa.Name, oa.TermSourceREF, oa.LocalID, oa.TermAccessionNumber, (matchValue = oa.Comments, (matchValue == null) ? singleton(createOrderComment(i)) : ((cs = matchValue, cons(createOrderComment(i), cs)))));
}

export function tryGetOntologyAnnotationIndex(oa) {
    return bind(tryGetIndex, oa.Comments);
}

export function tryGetParameterIndex(param) {
    return bind((oa) => bind(tryGetIndex, oa.Comments), param.ParameterName);
}

export function tryGetParameterColumnIndex(paramValue) {
    return bind(tryGetParameterIndex, paramValue.Category);
}

export function tryGetFactorIndex(factor) {
    return bind((oa) => bind(tryGetIndex, oa.Comments), factor.FactorType);
}

export function tryGetFactorColumnIndex(factorValue) {
    return bind(tryGetFactorIndex, factorValue.Category);
}

export function tryGetCharacteristicIndex(characteristic) {
    return bind((oa) => bind(tryGetIndex, oa.Comments), characteristic.CharacteristicType);
}

export function tryGetCharacteristicColumnIndex(characteristicValue) {
    return bind(tryGetCharacteristicIndex, characteristicValue.Category);
}

export function tryGetComponentIndex(comp) {
    return bind((oa) => bind(tryGetIndex, oa.Comments), comp.ComponentType);
}

/**
 * Create a ISAJson Factor from ISATab string entries
 */
export function ISA_OntologyAnnotation__OntologyAnnotation_fromStringWithColumnIndex_Static(name, term, source, accession, valueIndex) {
    return Factor_fromString_Z5D76503E(name, term, source, accession, singleton(createOrderComment(valueIndex)));
}

export function ISA_OntologyAnnotation__OntologyAnnotation_getColumnIndex_Static_2FC95D30(f) {
    return value(tryGetOntologyAnnotationIndex(f));
}

export function ISA_OntologyAnnotation__OntologyAnnotation_GetColumnIndex(this$) {
    return value(tryGetOntologyAnnotationIndex(this$));
}

export function ISA_OntologyAnnotation__OntologyAnnotation_tryGetColumnIndex_Static_2FC95D30(f) {
    return tryGetOntologyAnnotationIndex(f);
}

export function ISA_OntologyAnnotation__OntologyAnnotation_TryGetColumnIndex(this$) {
    return tryGetOntologyAnnotationIndex(this$);
}

export function ISA_OntologyAnnotation__OntologyAnnotation_setColumnIndex_Static(i, oa) {
    return setOntologyAnnotationIndex(i, oa);
}

export function ISA_OntologyAnnotation__OntologyAnnotation_SetColumnIndex_Z524259A4(this$, i) {
    return setOntologyAnnotationIndex(i, this$);
}

/**
 * Create a ISAJson Factor from ISATab string entries
 */
export function ISA_Factor__Factor_fromStringWithColumnIndex_Static(name, term, source, accession, valueIndex) {
    return Factor_fromString_Z5D76503E(name, term, source, accession, singleton(createOrderComment(valueIndex)));
}

export function ISA_Factor__Factor_getColumnIndex_Static_E353FDD(f) {
    return value(tryGetFactorIndex(f));
}

export function ISA_Factor__Factor_GetColumnIndex(this$) {
    return value(tryGetFactorIndex(this$));
}

export function ISA_Factor__Factor_tryGetColumnIndex_Static_E353FDD(f) {
    return tryGetFactorIndex(f);
}

export function ISA_Factor__Factor_TryGetColumnIndex(this$) {
    return tryGetFactorIndex(this$);
}

export function ISA_FactorValue__FactorValue_getColumnIndex_Static_2A4175B6(f) {
    return value(tryGetFactorColumnIndex(f));
}

export function ISA_FactorValue__FactorValue_GetColumnIndex(this$) {
    return value(tryGetFactorColumnIndex(this$));
}

export function ISA_FactorValue__FactorValue_tryGetColumnIndex_Static_2A4175B6(f) {
    return tryGetFactorColumnIndex(f);
}

export function ISA_FactorValue__FactorValue_TryGetColumnIndex(this$) {
    return tryGetFactorColumnIndex(this$);
}

/**
 * Create a ISAJson characteristic from ISATab string entries
 */
export function ISA_MaterialAttribute__MaterialAttribute_fromStringWithColumnIndex_Static(term, source, accession, valueIndex) {
    return MaterialAttribute_fromString_703AFBF9(term, source, accession, singleton(createOrderComment(valueIndex)));
}

export function ISA_MaterialAttribute__MaterialAttribute_getColumnIndex_Static_Z6476E859(m) {
    return value(tryGetCharacteristicIndex(m));
}

export function ISA_MaterialAttribute__MaterialAttribute_GetColumnIndex(this$) {
    return value(tryGetCharacteristicIndex(this$));
}

export function ISA_MaterialAttribute__MaterialAttribute_tryGetColumnIndex_Static_Z6476E859(m) {
    return tryGetCharacteristicIndex(m);
}

export function ISA_MaterialAttribute__MaterialAttribute_TryGetColumnIndex(this$) {
    return tryGetCharacteristicIndex(this$);
}

export function ISA_MaterialAttributeValue__MaterialAttributeValue_getColumnIndex_Static_6A64994C(m) {
    return value(tryGetCharacteristicColumnIndex(m));
}

export function ISA_MaterialAttributeValue__MaterialAttributeValue_GetColumnIndex(this$) {
    return value(tryGetCharacteristicColumnIndex(this$));
}

export function ISA_MaterialAttributeValue__MaterialAttributeValue_tryGetColumnIndex_Static_6A64994C(m) {
    return tryGetCharacteristicColumnIndex(m);
}

export function ISA_MaterialAttributeValue__MaterialAttributeValue_TryGetColumnIndex(this$) {
    return tryGetCharacteristicColumnIndex(this$);
}

/**
 * Create a ISAJson parameter from ISATab string entries
 */
export function ISA_ProtocolParameter__ProtocolParameter_fromStringWithColumnIndex_Static(term, source, accession, valueIndex) {
    return ProtocolParameter_fromString_703AFBF9(term, source, accession, singleton(createOrderComment(valueIndex)));
}

export function ISA_ProtocolParameter__ProtocolParameter_getColumnIndex_Static_2762A46F(p) {
    return value(tryGetParameterIndex(p));
}

export function ISA_ProtocolParameter__ProtocolParameter_GetColumnIndex(this$) {
    return value(tryGetParameterIndex(this$));
}

export function ISA_ProtocolParameter__ProtocolParameter_tryGetColumnIndex_Static_2762A46F(p) {
    return tryGetParameterIndex(p);
}

export function ISA_ProtocolParameter__ProtocolParameter_TryGetColumnIndex(this$) {
    return tryGetParameterIndex(this$);
}

export function ISA_ProcessParameterValue__ProcessParameterValue_getColumnIndex_Static_39585819(p) {
    return value(tryGetParameterColumnIndex(p));
}

export function ISA_ProcessParameterValue__ProcessParameterValue_GetColumnIndex(this$) {
    return value(tryGetParameterColumnIndex(this$));
}

export function ISA_ProcessParameterValue__ProcessParameterValue_tryGetColumnIndex_Static_39585819(p) {
    return tryGetParameterColumnIndex(p);
}

export function ISA_ProcessParameterValue__ProcessParameterValue_TryGetColumnIndex(this$) {
    return tryGetParameterColumnIndex(this$);
}

/**
 * Create a ISAJson Factor from ISATab string entries
 */
export function ISA_Component__Component_fromStringWithColumnIndex_Static(name, term, source, accession, valueIndex) {
    return Component_fromString_55205B02(name, term, source, accession, singleton(createOrderComment(valueIndex)));
}

export function ISA_Component__Component_getColumnIndex_Static_Z7E9B32A1(f) {
    return value(tryGetComponentIndex(f));
}

export function ISA_Component__Component_GetColumnIndex(this$) {
    return value(tryGetComponentIndex(this$));
}

export function ISA_Component__Component_tryGetColumnIndex_Static_Z7E9B32A1(f) {
    return tryGetComponentIndex(f);
}

export function ISA_Component__Component_TryGetColumnIndex(this$) {
    return tryGetComponentIndex(this$);
}

