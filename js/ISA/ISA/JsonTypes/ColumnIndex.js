import { tryParse } from "../../../fable_modules/fable-library.4.1.4/Int32.js";
import { FSharpRef } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { Comment_fromString } from "./Comment.js";
import { int32ToString } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { value, bind } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { tryItem } from "./CommentList.js";
import { append } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { OntologyAnnotation } from "./OntologyAnnotation.js";
import { Factor_fromString_E4803FF } from "./Factor.js";
import { MaterialAttribute_fromString_Z2304A83C } from "./MaterialAttribute.js";
import { ProtocolParameter_fromString_Z2304A83C } from "./ProtocolParameter.js";
import { Component_fromString_Z61E08C1 } from "./Component.js";

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
    return new OntologyAnnotation(oa.ID, oa.Name, oa.TermSourceREF, oa.LocalID, oa.TermAccessionNumber, (matchValue = oa.Comments, (matchValue == null) ? [createOrderComment(i)] : ((cs = matchValue, append([createOrderComment(i)], cs)))));
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
export function ARCtrl_ISA_OntologyAnnotation__OntologyAnnotation_fromStringWithColumnIndex_Static(name, term, source, accession, valueIndex) {
    return Factor_fromString_E4803FF(name, term, source, accession, [createOrderComment(valueIndex)]);
}

export function ARCtrl_ISA_OntologyAnnotation__OntologyAnnotation_getColumnIndex_Static_Z4C0FE73C(f) {
    return value(tryGetOntologyAnnotationIndex(f));
}

export function ARCtrl_ISA_OntologyAnnotation__OntologyAnnotation_GetColumnIndex(this$) {
    return value(tryGetOntologyAnnotationIndex(this$));
}

export function ARCtrl_ISA_OntologyAnnotation__OntologyAnnotation_tryGetColumnIndex_Static_Z4C0FE73C(f) {
    return tryGetOntologyAnnotationIndex(f);
}

export function ARCtrl_ISA_OntologyAnnotation__OntologyAnnotation_TryGetColumnIndex(this$) {
    return tryGetOntologyAnnotationIndex(this$);
}

export function ARCtrl_ISA_OntologyAnnotation__OntologyAnnotation_setColumnIndex_Static(i, oa) {
    return setOntologyAnnotationIndex(i, oa);
}

export function ARCtrl_ISA_OntologyAnnotation__OntologyAnnotation_SetColumnIndex_Z524259A4(this$, i) {
    return setOntologyAnnotationIndex(i, this$);
}

/**
 * Create a ISAJson Factor from ISATab string entries
 */
export function ARCtrl_ISA_Factor__Factor_fromStringWithColumnIndex_Static(name, term, source, accession, valueIndex) {
    return Factor_fromString_E4803FF(name, term, source, accession, [createOrderComment(valueIndex)]);
}

export function ARCtrl_ISA_Factor__Factor_getColumnIndex_Static_Z55333BD7(f) {
    return value(tryGetFactorIndex(f));
}

export function ARCtrl_ISA_Factor__Factor_GetColumnIndex(this$) {
    return value(tryGetFactorIndex(this$));
}

export function ARCtrl_ISA_Factor__Factor_tryGetColumnIndex_Static_Z55333BD7(f) {
    return tryGetFactorIndex(f);
}

export function ARCtrl_ISA_Factor__Factor_TryGetColumnIndex(this$) {
    return tryGetFactorIndex(this$);
}

export function ARCtrl_ISA_FactorValue__FactorValue_getColumnIndex_Static_Z2623397E(f) {
    return value(tryGetFactorColumnIndex(f));
}

export function ARCtrl_ISA_FactorValue__FactorValue_GetColumnIndex(this$) {
    return value(tryGetFactorColumnIndex(this$));
}

export function ARCtrl_ISA_FactorValue__FactorValue_tryGetColumnIndex_Static_Z2623397E(f) {
    return tryGetFactorColumnIndex(f);
}

export function ARCtrl_ISA_FactorValue__FactorValue_TryGetColumnIndex(this$) {
    return tryGetFactorColumnIndex(this$);
}

/**
 * Create a ISAJson characteristic from ISATab string entries
 */
export function ARCtrl_ISA_MaterialAttribute__MaterialAttribute_fromStringWithColumnIndex_Static(term, source, accession, valueIndex) {
    return MaterialAttribute_fromString_Z2304A83C(term, source, accession, [createOrderComment(valueIndex)]);
}

export function ARCtrl_ISA_MaterialAttribute__MaterialAttribute_getColumnIndex_Static_Z5F39696D(m) {
    return value(tryGetCharacteristicIndex(m));
}

export function ARCtrl_ISA_MaterialAttribute__MaterialAttribute_GetColumnIndex(this$) {
    return value(tryGetCharacteristicIndex(this$));
}

export function ARCtrl_ISA_MaterialAttribute__MaterialAttribute_tryGetColumnIndex_Static_Z5F39696D(m) {
    return tryGetCharacteristicIndex(m);
}

export function ARCtrl_ISA_MaterialAttribute__MaterialAttribute_TryGetColumnIndex(this$) {
    return tryGetCharacteristicIndex(this$);
}

export function ARCtrl_ISA_MaterialAttributeValue__MaterialAttributeValue_getColumnIndex_Static_43A5B238(m) {
    return value(tryGetCharacteristicColumnIndex(m));
}

export function ARCtrl_ISA_MaterialAttributeValue__MaterialAttributeValue_GetColumnIndex(this$) {
    return value(tryGetCharacteristicColumnIndex(this$));
}

export function ARCtrl_ISA_MaterialAttributeValue__MaterialAttributeValue_tryGetColumnIndex_Static_43A5B238(m) {
    return tryGetCharacteristicColumnIndex(m);
}

export function ARCtrl_ISA_MaterialAttributeValue__MaterialAttributeValue_TryGetColumnIndex(this$) {
    return tryGetCharacteristicColumnIndex(this$);
}

/**
 * Create a ISAJson parameter from ISATab string entries
 */
export function ARCtrl_ISA_ProtocolParameter__ProtocolParameter_fromStringWithColumnIndex_Static(term, source, accession, valueIndex) {
    return ProtocolParameter_fromString_Z2304A83C(term, source, accession, [createOrderComment(valueIndex)]);
}

export function ARCtrl_ISA_ProtocolParameter__ProtocolParameter_getColumnIndex_Static_Z3A4310A5(p) {
    return value(tryGetParameterIndex(p));
}

export function ARCtrl_ISA_ProtocolParameter__ProtocolParameter_GetColumnIndex(this$) {
    return value(tryGetParameterIndex(this$));
}

export function ARCtrl_ISA_ProtocolParameter__ProtocolParameter_tryGetColumnIndex_Static_Z3A4310A5(p) {
    return tryGetParameterIndex(p);
}

export function ARCtrl_ISA_ProtocolParameter__ProtocolParameter_TryGetColumnIndex(this$) {
    return tryGetParameterIndex(this$);
}

export function ARCtrl_ISA_ProcessParameterValue__ProcessParameterValue_getColumnIndex_Static_5FD7232D(p) {
    return value(tryGetParameterColumnIndex(p));
}

export function ARCtrl_ISA_ProcessParameterValue__ProcessParameterValue_GetColumnIndex(this$) {
    return value(tryGetParameterColumnIndex(this$));
}

export function ARCtrl_ISA_ProcessParameterValue__ProcessParameterValue_tryGetColumnIndex_Static_5FD7232D(p) {
    return tryGetParameterColumnIndex(p);
}

export function ARCtrl_ISA_ProcessParameterValue__ProcessParameterValue_TryGetColumnIndex(this$) {
    return tryGetParameterColumnIndex(this$);
}

/**
 * Create a ISAJson Factor from ISATab string entries
 */
export function ARCtrl_ISA_Component__Component_fromStringWithColumnIndex_Static(name, term, source, accession, valueIndex) {
    return Component_fromString_Z61E08C1(name, term, source, accession, [createOrderComment(valueIndex)]);
}

export function ARCtrl_ISA_Component__Component_getColumnIndex_Static_Z609B8895(f) {
    return value(tryGetComponentIndex(f));
}

export function ARCtrl_ISA_Component__Component_GetColumnIndex(this$) {
    return value(tryGetComponentIndex(this$));
}

export function ARCtrl_ISA_Component__Component_tryGetColumnIndex_Static_Z609B8895(f) {
    return tryGetComponentIndex(f);
}

export function ARCtrl_ISA_Component__Component_TryGetColumnIndex(this$) {
    return tryGetComponentIndex(this$);
}

