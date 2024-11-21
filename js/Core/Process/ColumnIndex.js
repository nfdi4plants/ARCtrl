import { tryParse } from "../../fable_modules/fable-library-js.4.22.0/Int32.js";
import { FSharpRef } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { Comment$ } from "../Comment.js";
import { equals, int32ToString } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { tryItem } from "../CommentList.js";
import { findIndex } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { value, bind } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { Factor } from "./Factor.js";
import { MaterialAttribute_fromString_5980DC03 } from "./MaterialAttribute.js";
import { ProtocolParameter } from "./ProtocolParameter.js";
import { Component_fromISAString_7C9A7CF8 } from "./Component.js";

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
        return undefined;
    }
}

export const orderName = "ColumnIndex";

export function createOrderComment(index) {
    return Comment$.create(orderName, int32ToString(index));
}

export function tryGetIndex(comments) {
    const matchValue = tryItem(orderName, comments);
    if (matchValue != null) {
        const ci = matchValue;
        const i = findIndex((c) => equals(c.Name, orderName), comments) | 0;
        comments.splice(i, 1);
        return tryInt(ci);
    }
    else {
        return undefined;
    }
}

export function setOntologyAnnotationIndexInplace(i, oa) {
    void (oa.Comments.push(createOrderComment(i)));
}

export function setOntologyAnnotationIndex(i, oa) {
    const oac = oa.Copy();
    setOntologyAnnotationIndexInplace(i, oac);
    return oac;
}

export function tryGetOntologyAnnotationIndex(oa) {
    return tryGetIndex(oa.Comments);
}

export function tryGetParameterIndex(param) {
    return bind((oa) => tryGetIndex(oa.Comments), param.ParameterName);
}

export function tryGetParameterColumnIndex(paramValue) {
    return bind(tryGetParameterIndex, paramValue.Category);
}

export function tryGetFactorIndex(factor) {
    return bind((oa) => tryGetIndex(oa.Comments), factor.FactorType);
}

export function tryGetFactorColumnIndex(factorValue) {
    return bind(tryGetFactorIndex, factorValue.Category);
}

export function tryGetCharacteristicIndex(characteristic) {
    return bind((oa) => tryGetIndex(oa.Comments), characteristic.CharacteristicType);
}

export function tryGetCharacteristicColumnIndex(characteristicValue) {
    return bind(tryGetCharacteristicIndex, characteristicValue.Category);
}

export function tryGetComponentIndex(comp) {
    return bind((oa) => tryGetIndex(oa.Comments), comp.ComponentType);
}

/**
 * Create a ISAJson Factor from ISATab string entries
 */
export function ARCtrl_OntologyAnnotation__OntologyAnnotation_fromStringWithColumnIndex_Static(name, term, source, accession, valueIndex) {
    return Factor.fromString(name, term, source, accession, [createOrderComment(valueIndex)]);
}

export function ARCtrl_OntologyAnnotation__OntologyAnnotation_getColumnIndex_Static_ZDED3A0F(f) {
    return value(tryGetOntologyAnnotationIndex(f));
}

export function ARCtrl_OntologyAnnotation__OntologyAnnotation_GetColumnIndex(this$) {
    return value(tryGetOntologyAnnotationIndex(this$));
}

export function ARCtrl_OntologyAnnotation__OntologyAnnotation_tryGetColumnIndex_Static_ZDED3A0F(f) {
    return tryGetOntologyAnnotationIndex(f);
}

export function ARCtrl_OntologyAnnotation__OntologyAnnotation_TryGetColumnIndex(this$) {
    return tryGetOntologyAnnotationIndex(this$);
}

export function ARCtrl_OntologyAnnotation__OntologyAnnotation_setColumnIndex_Static(i, oa) {
    return setOntologyAnnotationIndex(i, oa);
}

export function ARCtrl_OntologyAnnotation__OntologyAnnotation_SetColumnIndex_Z524259A4(this$, i) {
    setOntologyAnnotationIndexInplace(i, this$);
}

/**
 * Create a ISAJson Factor from ISATab string entries
 */
export function ARCtrl_Process_Factor__Factor_fromStringWithColumnIndex_Static(name, term, source, accession, valueIndex) {
    return Factor.fromString(name, term, source, accession, [createOrderComment(valueIndex)]);
}

export function ARCtrl_Process_Factor__Factor_getColumnIndex_Static_7206F0D9(f) {
    return value(tryGetFactorIndex(f));
}

export function ARCtrl_Process_Factor__Factor_GetColumnIndex(this$) {
    return value(tryGetFactorIndex(this$));
}

export function ARCtrl_Process_Factor__Factor_tryGetColumnIndex_Static_7206F0D9(f) {
    return tryGetFactorIndex(f);
}

export function ARCtrl_Process_Factor__Factor_TryGetColumnIndex(this$) {
    return tryGetFactorIndex(this$);
}

export function ARCtrl_Process_FactorValue__FactorValue_getColumnIndex_Static_7105C732(f) {
    return value(tryGetFactorColumnIndex(f));
}

export function ARCtrl_Process_FactorValue__FactorValue_GetColumnIndex(this$) {
    return value(tryGetFactorColumnIndex(this$));
}

export function ARCtrl_Process_FactorValue__FactorValue_tryGetColumnIndex_Static_7105C732(f) {
    return tryGetFactorColumnIndex(f);
}

export function ARCtrl_Process_FactorValue__FactorValue_TryGetColumnIndex(this$) {
    return tryGetFactorColumnIndex(this$);
}

/**
 * Create a ISAJson characteristic from ISATab string entries
 */
export function ARCtrl_Process_MaterialAttribute__MaterialAttribute_fromStringWithColumnIndex_Static(term, source, accession, valueIndex) {
    return MaterialAttribute_fromString_5980DC03(term, source, accession, [createOrderComment(valueIndex)]);
}

export function ARCtrl_Process_MaterialAttribute__MaterialAttribute_getColumnIndex_Static_Z1E3B85DD(m) {
    return value(tryGetCharacteristicIndex(m));
}

export function ARCtrl_Process_MaterialAttribute__MaterialAttribute_GetColumnIndex(this$) {
    return value(tryGetCharacteristicIndex(this$));
}

export function ARCtrl_Process_MaterialAttribute__MaterialAttribute_tryGetColumnIndex_Static_Z1E3B85DD(m) {
    return tryGetCharacteristicIndex(m);
}

export function ARCtrl_Process_MaterialAttribute__MaterialAttribute_TryGetColumnIndex(this$) {
    return tryGetCharacteristicIndex(this$);
}

export function ARCtrl_Process_MaterialAttributeValue__MaterialAttributeValue_getColumnIndex_Static_Z772273B8(m) {
    return value(tryGetCharacteristicColumnIndex(m));
}

export function ARCtrl_Process_MaterialAttributeValue__MaterialAttributeValue_GetColumnIndex(this$) {
    return value(tryGetCharacteristicColumnIndex(this$));
}

export function ARCtrl_Process_MaterialAttributeValue__MaterialAttributeValue_tryGetColumnIndex_Static_Z772273B8(m) {
    return tryGetCharacteristicColumnIndex(m);
}

export function ARCtrl_Process_MaterialAttributeValue__MaterialAttributeValue_TryGetColumnIndex(this$) {
    return tryGetCharacteristicColumnIndex(this$);
}

/**
 * Create a ISAJson parameter from ISATab string entries
 */
export function ARCtrl_Process_ProtocolParameter__ProtocolParameter_fromStringWithColumnIndex_Static(term, source, accession, valueIndex) {
    return ProtocolParameter.fromString(term, source, accession, [createOrderComment(valueIndex)]);
}

export function ARCtrl_Process_ProtocolParameter__ProtocolParameter_getColumnIndex_Static_Z11F87B15(p) {
    return value(tryGetParameterIndex(p));
}

export function ARCtrl_Process_ProtocolParameter__ProtocolParameter_GetColumnIndex(this$) {
    return value(tryGetParameterIndex(this$));
}

export function ARCtrl_Process_ProtocolParameter__ProtocolParameter_tryGetColumnIndex_Static_Z11F87B15(p) {
    return tryGetParameterIndex(p);
}

export function ARCtrl_Process_ProtocolParameter__ProtocolParameter_TryGetColumnIndex(this$) {
    return tryGetParameterIndex(this$);
}

export function ARCtrl_Process_ProcessParameterValue__ProcessParameterValue_getColumnIndex_Static_Z1576263(p) {
    return value(tryGetParameterColumnIndex(p));
}

export function ARCtrl_Process_ProcessParameterValue__ProcessParameterValue_GetColumnIndex(this$) {
    return value(tryGetParameterColumnIndex(this$));
}

export function ARCtrl_Process_ProcessParameterValue__ProcessParameterValue_tryGetColumnIndex_Static_Z1576263(p) {
    return tryGetParameterColumnIndex(p);
}

export function ARCtrl_Process_ProcessParameterValue__ProcessParameterValue_TryGetColumnIndex(this$) {
    return tryGetParameterColumnIndex(this$);
}

/**
 * Create a ISAJson Factor from ISATab string entries
 */
export function ARCtrl_Process_Component__Component_fromStringWithColumnIndex_Static(name, term, source, accession, valueIndex) {
    return Component_fromISAString_7C9A7CF8(name, term, source, accession, [createOrderComment(valueIndex)]);
}

export function ARCtrl_Process_Component__Component_getColumnIndex_Static_Z685B8F25(f) {
    return value(tryGetComponentIndex(f));
}

export function ARCtrl_Process_Component__Component_GetColumnIndex(this$) {
    return value(tryGetComponentIndex(this$));
}

export function ARCtrl_Process_Component__Component_tryGetColumnIndex_Static_Z685B8F25(f) {
    return tryGetComponentIndex(f);
}

export function ARCtrl_Process_Component__Component_TryGetColumnIndex(this$) {
    return tryGetComponentIndex(this$);
}

