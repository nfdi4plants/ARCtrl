import { remove, toFail, printf, toText } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { Comment$ } from "./Comment.js";
import { zip as zip_1, fold, empty as empty_1, singleton as singleton_1, append, delay, map as map_1, filter, toList, indexed, tryPick, choose } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { unwrap, value as value_10, map, defaultArg } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { Option_fromValueWithDefault, ResizeArray_filter } from "./Helper/Collections.js";
import { Value as Value_2 } from "./Value.js";
import { Component_create_Z2F0B38C7 } from "./Process/Component.js";
import { ProtocolParameter } from "./Process/ProtocolParameter.js";
import { ProcessParameterValue } from "./Process/ProcessParameterValue.js";
import { FactorValue_create_30BDC49 } from "./Process/FactorValue.js";
import { Factor } from "./Process/Factor.js";
import { toString } from "../fable_modules/fable-library-js.4.22.0/Types.js";
import { MaterialAttributeValue_create_ZE1D108D } from "./Process/MaterialAttributeValue.js";
import { MaterialAttribute_create_A220A8A } from "./Process/MaterialAttribute.js";
import { ProcessInput_getCharacteristicValues_5B3D5BA9, ProcessInput__isMaterial, ProcessInput__isData, ProcessInput_setCharacteristicValues, ProcessInput__get_Name, ProcessInput__isSource, ProcessInput__isSample, ProcessInput, ProcessInput_createSource_Z5E00540E, ProcessInput_createRawData_Z721C83C5, ProcessInput_createMaterial_4452CB4C, ProcessInput_createSample_Z598187B7 } from "./Process/ProcessInput.js";
import { ProcessOutput_getFactorValues_Z42C11600, ProcessOutput__isMaterial, ProcessOutput__isData, ProcessOutput_setFactorValues, ProcessOutput__get_Name, ProcessOutput__isSample, ProcessOutput, ProcessOutput_createRawData_Z721C83C5, ProcessOutput_createMaterial_4452CB4C, ProcessOutput_createSample_Z598187B7 } from "./Process/ProcessOutput.js";
import { CompositeCell } from "./Table/CompositeCell.js";
import { OntologyAnnotation } from "./OntologyAnnotation.js";
import { safeHash, equals, disposeSafe, getEnumerator, comparePrimitives, arrayHash, equalArrays, stringHash, int32ToString } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { IOType, CompositeHeader } from "./Table/CompositeHeader.js";
import { ActivePatterns_$007CRegex$007C_$007C } from "./Helper/Regex.js";
import { ARCtrl_Process_Component__Component_TryGetColumnIndex, ARCtrl_Process_ProtocolParameter__ProtocolParameter_TryGetColumnIndex, tryGetFactorColumnIndex, tryGetCharacteristicColumnIndex, tryGetComponentIndex, tryGetParameterColumnIndex, ARCtrl_OntologyAnnotation__OntologyAnnotation_setColumnIndex_Static, ARCtrl_OntologyAnnotation__OntologyAnnotation_SetColumnIndex_Z524259A4 } from "./Process/ColumnIndex.js";
import { getItemFromDict } from "../fable_modules/fable-library-js.4.22.0/MapUtil.js";
import { initialize, zip, append as append_1, sortBy, head, tryPick as tryPick_1, collect, mapIndexed, item, map as map_2, empty, length, ofArray, isEmpty, singleton, choose as choose_1 } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { Source_create_Z5CA08497 } from "./Process/Source.js";
import { Sample_create_62424AD2 } from "./Process/Sample.js";
import { Process_create_Z7C1F7FA9, Process_decomposeName_Z721C83C5, Process_make, Process_composeName } from "./Process/Process.js";
import { Protocol_create_Z414665E7, Protocol_addComponent, Protocol_addParameter, Protocol_setName, Protocol_setDescription, Protocol_setUri, Protocol_setVersion, Protocol_setProtocolType, Protocol_make } from "./Process/Protocol.js";
import { List_distinct, List_groupBy } from "../fable_modules/fable-library-js.4.22.0/Seq2.js";
import { createMissingIdentifier } from "./Helper/Identifier.js";
import { boxHashSeq, boxHashOption } from "./Helper/HashCodes.js";
import { ArcTable } from "./Table/ArcTable.js";
import { singleton as singleton_2 } from "../fable_modules/fable-library-js.4.22.0/Array.js";
import { rangeDouble } from "../fable_modules/fable-library-js.4.22.0/Range.js";
import { Unchecked_alignByHeaders } from "./Table/ArcTableAux.js";
import { ArcTables } from "./Table/ArcTables.js";

export const Person_orcidKey = "ORCID";

export const Person_AssayIdentifierPrefix = "performer (ARC:00000168)";

export const Person_createAssayIdentifierKey = (() => {
    const clo_1 = toText(printf("%s %s"))(Person_AssayIdentifierPrefix);
    return clo_1;
})();

export function Person_setSourceAssayComment(person, assayIdentifier) {
    const person_1 = person.Copy();
    const comment = new Comment$(Person_createAssayIdentifierKey(assayIdentifier), assayIdentifier);
    void (person_1.Comments.push(comment));
    return person_1;
}

/**
 * This functions helps encoding/decoding ISA-JSON. It returns a sequence of ArcAssay-Identifiers.
 */
export function Person_getSourceAssayIdentifiersFromComments(person) {
    return choose((c) => {
        if (defaultArg(map((n) => n.startsWith(Person_AssayIdentifierPrefix), c.Name), false)) {
            return c.Value;
        }
        else {
            return undefined;
        }
    }, person.Comments);
}

export function Person_removeSourceAssayComments(person) {
    return ResizeArray_filter((c) => {
        if (c.Name != null) {
            return !value_10(c.Name).startsWith(Person_AssayIdentifierPrefix);
        }
        else {
            return false;
        }
    }, person.Comments);
}

export function Person_setOrcidFromComments(person) {
    const person_1 = person.Copy();
    const isOrcidComment = (c) => {
        if (c.Name != null) {
            return value_10(c.Name).toLocaleUpperCase().endsWith(Person_orcidKey);
        }
        else {
            return false;
        }
    };
    const patternInput = [tryPick((c_1) => {
        if (isOrcidComment(c_1)) {
            return c_1.Value;
        }
        else {
            return undefined;
        }
    }, person_1.Comments), ResizeArray_filter((arg) => !isOrcidComment(arg), person_1.Comments)];
    person_1.ORCID = patternInput[0];
    person_1.Comments = patternInput[1];
    return person_1;
}

export function Person_setCommentFromORCID(person) {
    const person_1 = person.Copy();
    const matchValue = person_1.ORCID;
    if (matchValue == null) {
    }
    else {
        const orcid = matchValue;
        const comment = Comment$.create(Person_orcidKey, orcid);
        void (person_1.Comments.push(comment));
    }
    return person_1;
}

/**
 * Convert a CompositeCell to a ISA Value and Unit tuple.
 */
export function JsonTypes_valueOfCell(value) {
    switch (value.tag) {
        case 0:
            if (value.fields[0].isEmpty()) {
                return [undefined, undefined];
            }
            else {
                return [new Value_2(0, [value.fields[0]]), undefined];
            }
        case 2:
            return [(value.fields[0] === "") ? undefined : Value_2.fromString(value.fields[0]), value.fields[1].isEmpty() ? undefined : value.fields[1]];
        case 3:
            throw new Error("Data cell should not be parsed to isa value");
        default:
            if (value.fields[0] === "") {
                return [undefined, undefined];
            }
            else {
                return [Value_2.fromString(value.fields[0]), undefined];
            }
    }
}

/**
 * Convert a CompositeHeader and Cell tuple to a ISA Component
 */
export function JsonTypes_composeComponent(header, value) {
    const patternInput = JsonTypes_valueOfCell(value);
    return Component_create_Z2F0B38C7(unwrap(patternInput[0]), unwrap(patternInput[1]), header.ToTerm());
}

/**
 * Convert a CompositeHeader and Cell tuple to a ISA ProcessParameterValue
 */
export function JsonTypes_composeParameterValue(header, value) {
    const patternInput = JsonTypes_valueOfCell(value);
    const p = ProtocolParameter.create(undefined, header.ToTerm());
    return ProcessParameterValue.create(p, unwrap(patternInput[0]), unwrap(patternInput[1]));
}

/**
 * Convert a CompositeHeader and Cell tuple to a ISA FactorValue
 */
export function JsonTypes_composeFactorValue(header, value) {
    const patternInput = JsonTypes_valueOfCell(value);
    return FactorValue_create_30BDC49(undefined, Factor.create(toString(header), header.ToTerm()), unwrap(patternInput[0]), unwrap(patternInput[1]));
}

/**
 * Convert a CompositeHeader and Cell tuple to a ISA MaterialAttributeValue
 */
export function JsonTypes_composeCharacteristicValue(header, value) {
    const patternInput = JsonTypes_valueOfCell(value);
    return MaterialAttributeValue_create_ZE1D108D(undefined, MaterialAttribute_create_A220A8A(undefined, header.ToTerm()), unwrap(patternInput[0]), unwrap(patternInput[1]));
}

export function JsonTypes_composeFreetextMaterialName(headerFT, name) {
    return `${headerFT}=${name}`;
}

/**
 * Convert a CompositeHeader and Cell tuple to a ISA ProcessInput
 */
export function JsonTypes_composeProcessInput(header, value) {
    if (header.tag === 11) {
        switch (header.fields[0].tag) {
            case 1:
                return ProcessInput_createSample_Z598187B7(toString(value));
            case 3:
                return ProcessInput_createMaterial_4452CB4C(toString(value));
            case 2:
                return ProcessInput_createRawData_Z721C83C5(toString(value));
            case 4:
                return ProcessInput_createMaterial_4452CB4C(JsonTypes_composeFreetextMaterialName(header.fields[0].fields[0], toString(value)));
            default:
                return ProcessInput_createSource_Z5E00540E(toString(value));
        }
    }
    else {
        return toFail(printf("Could not parse input header %O"))(header);
    }
}

/**
 * Convert a CompositeHeader and Cell tuple to a ISA ProcessOutput
 */
export function JsonTypes_composeProcessOutput(header, value) {
    let matchResult, ft;
    if (header.tag === 12) {
        switch (header.fields[0].tag) {
            case 3: {
                matchResult = 1;
                break;
            }
            case 2: {
                matchResult = 2;
                break;
            }
            case 4: {
                matchResult = 3;
                ft = header.fields[0].fields[0];
                break;
            }
            default:
                matchResult = 0;
        }
    }
    else {
        matchResult = 4;
    }
    switch (matchResult) {
        case 0:
            return ProcessOutput_createSample_Z598187B7(toString(value));
        case 1:
            return ProcessOutput_createMaterial_4452CB4C(toString(value));
        case 2:
            return ProcessOutput_createRawData_Z721C83C5(toString(value));
        case 3:
            return ProcessOutput_createMaterial_4452CB4C(JsonTypes_composeFreetextMaterialName(ft, toString(value)));
        default:
            return toFail(printf("Could not parse output header %O"))(header);
    }
}

/**
 * Convert an ISA Value and Unit tuple to a CompositeCell
 */
export function JsonTypes_cellOfValue(value, unit) {
    const value_2 = defaultArg(value, new Value_2(3, [""]));
    let matchResult, oa, text, name, u, f, u_1, f_1, i, u_2, i_1;
    switch (value_2.tag) {
        case 3: {
            if (value_2.fields[0] === "") {
                if (unit != null) {
                    matchResult = 3;
                    name = value_2.fields[0];
                    u = unit;
                }
                else {
                    matchResult = 1;
                }
            }
            else if (unit != null) {
                matchResult = 3;
                name = value_2.fields[0];
                u = unit;
            }
            else {
                matchResult = 2;
                text = value_2.fields[0];
            }
            break;
        }
        case 2: {
            if (unit == null) {
                matchResult = 5;
                f_1 = value_2.fields[0];
            }
            else {
                matchResult = 4;
                f = value_2.fields[0];
                u_1 = unit;
            }
            break;
        }
        case 1: {
            if (unit == null) {
                matchResult = 7;
                i_1 = value_2.fields[0];
            }
            else {
                matchResult = 6;
                i = value_2.fields[0];
                u_2 = unit;
            }
            break;
        }
        default:
            if (unit == null) {
                matchResult = 0;
                oa = value_2.fields[0];
            }
            else {
                matchResult = 8;
            }
    }
    switch (matchResult) {
        case 0:
            return new CompositeCell(0, [oa]);
        case 1:
            return new CompositeCell(0, [new OntologyAnnotation()]);
        case 2:
            return new CompositeCell(0, [new OntologyAnnotation(text)]);
        case 3:
            return new CompositeCell(2, [name, u]);
        case 4:
            return new CompositeCell(2, [f.toString(), u_1]);
        case 5:
            return new CompositeCell(2, [f_1.toString(), new OntologyAnnotation()]);
        case 6:
            return new CompositeCell(2, [int32ToString(i), u_2]);
        case 7:
            return new CompositeCell(2, [int32ToString(i_1), new OntologyAnnotation()]);
        default:
            return toFail(printf("Could not parse value %O with unit %O"))(value_2)(unit);
    }
}

/**
 * Convert an ISA Component to a CompositeHeader and Cell tuple
 */
export function JsonTypes_decomposeComponent(c) {
    return [new CompositeHeader(0, [value_10(c.ComponentType)]), JsonTypes_cellOfValue(c.ComponentValue, c.ComponentUnit)];
}

/**
 * Convert an ISA ProcessParameterValue to a CompositeHeader and Cell tuple
 */
export function JsonTypes_decomposeParameterValue(ppv) {
    return [new CompositeHeader(3, [value_10(value_10(ppv.Category).ParameterName)]), JsonTypes_cellOfValue(ppv.Value, ppv.Unit)];
}

/**
 * Convert an ISA FactorValue to a CompositeHeader and Cell tuple
 */
export function JsonTypes_decomposeFactorValue(fv) {
    return [new CompositeHeader(2, [value_10(value_10(fv.Category).FactorType)]), JsonTypes_cellOfValue(fv.Value, fv.Unit)];
}

/**
 * Convert an ISA MaterialAttributeValue to a CompositeHeader and Cell tuple
 */
export function JsonTypes_decomposeCharacteristicValue(cv) {
    return [new CompositeHeader(1, [value_10(value_10(cv.Category).CharacteristicType)]), JsonTypes_cellOfValue(cv.Value, cv.Unit)];
}

/**
 * Convert an ISA ProcessOutput to a CompositeHeader and Cell tuple
 */
export function JsonTypes_decomposeProcessInput(pi) {
    switch (pi.tag) {
        case 1:
            return [new CompositeHeader(11, [new IOType(1, [])]), new CompositeCell(1, [defaultArg(pi.fields[0].Name, "")])];
        case 3:
            return [new CompositeHeader(11, [new IOType(3, [])]), new CompositeCell(1, [defaultArg(pi.fields[0].Name, "")])];
        case 2: {
            const d = pi.fields[0];
            const dataType = value_10(d.DataType);
            switch (dataType.tag) {
                case 0:
                    return [new CompositeHeader(11, [new IOType(2, [])]), new CompositeCell(1, [defaultArg(d.Name, "")])];
                case 1:
                    return [new CompositeHeader(11, [new IOType(2, [])]), new CompositeCell(1, [defaultArg(d.Name, "")])];
                default:
                    return [new CompositeHeader(11, [new IOType(2, [])]), new CompositeCell(1, [defaultArg(d.Name, "")])];
            }
        }
        default:
            return [new CompositeHeader(11, [new IOType(0, [])]), new CompositeCell(1, [defaultArg(pi.fields[0].Name, "")])];
    }
}

/**
 * Convert an ISA ProcessOutput to a CompositeHeader and Cell tuple
 */
export function JsonTypes_decomposeProcessOutput(po) {
    switch (po.tag) {
        case 2:
            return [new CompositeHeader(12, [new IOType(3, [])]), new CompositeCell(1, [defaultArg(po.fields[0].Name, "")])];
        case 1: {
            const d = po.fields[0];
            const dataType = value_10(d.DataType);
            switch (dataType.tag) {
                case 0:
                    return [new CompositeHeader(12, [new IOType(2, [])]), new CompositeCell(1, [defaultArg(d.Name, "")])];
                case 1:
                    return [new CompositeHeader(12, [new IOType(2, [])]), new CompositeCell(1, [defaultArg(d.Name, "")])];
                default:
                    return [new CompositeHeader(12, [new IOType(2, [])]), new CompositeCell(1, [defaultArg(d.Name, "")])];
            }
        }
        default:
            return [new CompositeHeader(12, [new IOType(1, [])]), new CompositeCell(1, [defaultArg(po.fields[0].Name, "")])];
    }
}

/**
 * This function creates a string containing the name and the ontology short-string of the given ontology annotation term
 * 
 * TechnologyPlatforms are plain strings in ISA-JSON.
 * 
 * This function allows us, to parse them as an ontology term.
 */
export function JsonTypes_composeTechnologyPlatform(tp) {
    const matchValue = tp.TANInfo;
    if (matchValue == null) {
        return `${tp.NameText}`;
    }
    else {
        return `${tp.NameText} (${tp.TermAccessionShort})`;
    }
}

/**
 * This function parses the given string containing the name and the ontology short-string of the given ontology annotation term
 * 
 * TechnologyPlatforms are plain strings in ISA-JSON.
 * 
 * This function allows us, to parse them as an ontology term.
 */
export function JsonTypes_decomposeTechnologyPlatform(name) {
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C("(?<value>[^\\(]+) \\((?<ontology>[^(]*:[^)]*)\\)", name);
    if (activePatternResult != null) {
        const r = activePatternResult;
        let oa;
        const tan = (r.groups && r.groups.ontology) || "";
        oa = OntologyAnnotation.fromTermAnnotation(tan);
        const v = (r.groups && r.groups.value) || "";
        return OntologyAnnotation.create(v, unwrap(oa.TermSourceREF), unwrap(oa.TermAccessionNumber));
    }
    else {
        return OntologyAnnotation.create(name);
    }
}

/**
 * If the given headers depict a component, returns a function for parsing the values of the matrix to the values of this component
 */
export function ProcessParsing_tryComponentGetter(generalI, valueI, valueHeader) {
    if (valueHeader.tag === 0) {
        const newOA = valueHeader.fields[0].Copy();
        ARCtrl_OntologyAnnotation__OntologyAnnotation_SetColumnIndex_Z524259A4(newOA, valueI);
        return (matrix) => ((i) => JsonTypes_composeComponent(new CompositeHeader(0, [newOA]), getItemFromDict(matrix, [generalI, i])));
    }
    else {
        return undefined;
    }
}

/**
 * If the given headers depict a parameter, returns a function for parsing the values of the matrix to the values of this type
 */
export function ProcessParsing_tryParameterGetter(generalI, valueI, valueHeader) {
    if (valueHeader.tag === 3) {
        const cat = new CompositeHeader(3, [ARCtrl_OntologyAnnotation__OntologyAnnotation_setColumnIndex_Static(valueI, valueHeader.fields[0])]);
        return (matrix) => ((i_1) => JsonTypes_composeParameterValue(cat, getItemFromDict(matrix, [generalI, i_1])));
    }
    else {
        return undefined;
    }
}

/**
 * If the given headers depict a factor, returns a function for parsing the values of the matrix to the values of this type
 */
export function ProcessParsing_tryFactorGetter(generalI, valueI, valueHeader) {
    if (valueHeader.tag === 2) {
        const cat = new CompositeHeader(2, [ARCtrl_OntologyAnnotation__OntologyAnnotation_setColumnIndex_Static(valueI, valueHeader.fields[0])]);
        return (matrix) => ((i_1) => JsonTypes_composeFactorValue(cat, getItemFromDict(matrix, [generalI, i_1])));
    }
    else {
        return undefined;
    }
}

/**
 * If the given headers depict a protocolType, returns a function for parsing the values of the matrix to the values of this type
 */
export function ProcessParsing_tryCharacteristicGetter(generalI, valueI, valueHeader) {
    if (valueHeader.tag === 1) {
        const cat = new CompositeHeader(1, [ARCtrl_OntologyAnnotation__OntologyAnnotation_setColumnIndex_Static(valueI, valueHeader.fields[0])]);
        return (matrix) => ((i_1) => JsonTypes_composeCharacteristicValue(cat, getItemFromDict(matrix, [generalI, i_1])));
    }
    else {
        return undefined;
    }
}

/**
 * If the given headers depict a protocolType, returns a function for parsing the values of the matrix to the values of this type
 */
export function ProcessParsing_tryGetProtocolTypeGetter(generalI, header) {
    if (header.tag === 4) {
        return (matrix) => ((i) => getItemFromDict(matrix, [generalI, i]).AsTerm);
    }
    else {
        return undefined;
    }
}

/**
 * If the given headers depict a protocolREF, returns a function for parsing the values of the matrix to the values of this type
 */
export function ProcessParsing_tryGetProtocolREFGetter(generalI, header) {
    if (header.tag === 8) {
        return (matrix) => ((i) => getItemFromDict(matrix, [generalI, i]).AsFreeText);
    }
    else {
        return undefined;
    }
}

/**
 * If the given headers depict a protocolDescription, returns a function for parsing the values of the matrix to the values of this type
 */
export function ProcessParsing_tryGetProtocolDescriptionGetter(generalI, header) {
    if (header.tag === 5) {
        return (matrix) => ((i) => getItemFromDict(matrix, [generalI, i]).AsFreeText);
    }
    else {
        return undefined;
    }
}

/**
 * If the given headers depict a protocolURI, returns a function for parsing the values of the matrix to the values of this type
 */
export function ProcessParsing_tryGetProtocolURIGetter(generalI, header) {
    if (header.tag === 6) {
        return (matrix) => ((i) => getItemFromDict(matrix, [generalI, i]).AsFreeText);
    }
    else {
        return undefined;
    }
}

/**
 * If the given headers depict a protocolVersion, returns a function for parsing the values of the matrix to the values of this type
 */
export function ProcessParsing_tryGetProtocolVersionGetter(generalI, header) {
    if (header.tag === 7) {
        return (matrix) => ((i) => getItemFromDict(matrix, [generalI, i]).AsFreeText);
    }
    else {
        return undefined;
    }
}

/**
 * If the given headers depict an input, returns a function for parsing the values of the matrix to the values of this type
 */
export function ProcessParsing_tryGetInputGetter(generalI, header) {
    if (header.tag === 11) {
        return (matrix) => ((i) => JsonTypes_composeProcessInput(header, getItemFromDict(matrix, [generalI, i])));
    }
    else {
        return undefined;
    }
}

/**
 * If the given headers depict an output, returns a function for parsing the values of the matrix to the values of this type
 */
export function ProcessParsing_tryGetOutputGetter(generalI, header) {
    if (header.tag === 12) {
        return (matrix) => ((i) => JsonTypes_composeProcessOutput(header, getItemFromDict(matrix, [generalI, i])));
    }
    else {
        return undefined;
    }
}

/**
 * If the given headers depict a comment, returns a function for parsing the values of the matrix to the values of this type
 */
export function ProcessParsing_tryGetCommentGetter(generalI, header) {
    if (header.tag === 14) {
        return (matrix) => ((i) => (new Comment$(header.fields[0], getItemFromDict(matrix, [generalI, i]).AsFreeText)));
    }
    else {
        return undefined;
    }
}

/**
 * Given the header sequence of an ArcTable, returns a function for parsing each row of the table to a process
 */
export function ProcessParsing_getProcessGetter(processNameRoot, headers) {
    const headers_1 = indexed(headers);
    const valueHeaders = toList(indexed(filter((arg) => arg[1].IsCvParamColumn, headers_1)));
    const charGetters = choose_1((tupledArg) => {
        const _arg = tupledArg[1];
        return ProcessParsing_tryCharacteristicGetter(_arg[0], tupledArg[0], _arg[1]);
    }, valueHeaders);
    const factorValueGetters = choose_1((tupledArg_1) => {
        const _arg_1 = tupledArg_1[1];
        return ProcessParsing_tryFactorGetter(_arg_1[0], tupledArg_1[0], _arg_1[1]);
    }, valueHeaders);
    const parameterValueGetters = choose_1((tupledArg_2) => {
        const _arg_2 = tupledArg_2[1];
        return ProcessParsing_tryParameterGetter(_arg_2[0], tupledArg_2[0], _arg_2[1]);
    }, valueHeaders);
    const componentGetters = choose_1((tupledArg_3) => {
        const _arg_3 = tupledArg_3[1];
        return ProcessParsing_tryComponentGetter(_arg_3[0], tupledArg_3[0], _arg_3[1]);
    }, valueHeaders);
    const protocolTypeGetter = tryPick((tupledArg_4) => ProcessParsing_tryGetProtocolTypeGetter(tupledArg_4[0], tupledArg_4[1]), headers_1);
    const protocolREFGetter = tryPick((tupledArg_5) => ProcessParsing_tryGetProtocolREFGetter(tupledArg_5[0], tupledArg_5[1]), headers_1);
    const protocolDescriptionGetter = tryPick((tupledArg_6) => ProcessParsing_tryGetProtocolDescriptionGetter(tupledArg_6[0], tupledArg_6[1]), headers_1);
    const protocolURIGetter = tryPick((tupledArg_7) => ProcessParsing_tryGetProtocolURIGetter(tupledArg_7[0], tupledArg_7[1]), headers_1);
    const protocolVersionGetter = tryPick((tupledArg_8) => ProcessParsing_tryGetProtocolVersionGetter(tupledArg_8[0], tupledArg_8[1]), headers_1);
    const commentGetters = toList(choose((tupledArg_9) => ProcessParsing_tryGetCommentGetter(tupledArg_9[0], tupledArg_9[1]), headers_1));
    let inputGetter_1;
    const matchValue = tryPick((tupledArg_10) => ProcessParsing_tryGetInputGetter(tupledArg_10[0], tupledArg_10[1]), headers_1);
    if (matchValue == null) {
        inputGetter_1 = ((matrix_1) => ((i_1) => singleton(new ProcessInput(0, [Source_create_Z5CA08497(undefined, `${processNameRoot}_Input_${i_1}`, toList(map_1((f_1) => f_1(matrix_1)(i_1), charGetters)))]))));
    }
    else {
        const inputGetter = matchValue;
        inputGetter_1 = ((matrix) => ((i) => {
            const chars = toList(map_1((f) => f(matrix)(i), charGetters));
            const input = inputGetter(matrix)(i);
            return (!(ProcessInput__isSample(input) ? true : ProcessInput__isSource(input)) && !isEmpty(chars)) ? ofArray([input, ProcessInput_createSample_Z598187B7(ProcessInput__get_Name(input), chars)]) : singleton((length(chars) > 0) ? ProcessInput_setCharacteristicValues(chars, input) : input);
        }));
    }
    let outputGetter_1;
    const matchValue_1 = tryPick((tupledArg_11) => ProcessParsing_tryGetOutputGetter(tupledArg_11[0], tupledArg_11[1]), headers_1);
    if (matchValue_1 == null) {
        outputGetter_1 = ((matrix_3) => ((i_3) => singleton(new ProcessOutput(0, [Sample_create_62424AD2(undefined, `${processNameRoot}_Output_${i_3}`, undefined, toList(map_1((f_3) => f_3(matrix_3)(i_3), factorValueGetters)))]))));
    }
    else {
        const outputGetter = matchValue_1;
        outputGetter_1 = ((matrix_2) => ((i_2) => {
            const factors = toList(map_1((f_2) => f_2(matrix_2)(i_2), factorValueGetters));
            const output = outputGetter(matrix_2)(i_2);
            return (!ProcessOutput__isSample(output) && !isEmpty(factors)) ? ofArray([output, ProcessOutput_createSample_Z598187B7(ProcessOutput__get_Name(output), undefined, factors)]) : singleton((length(factors) > 0) ? ProcessOutput_setFactorValues(factors, output) : output);
        }));
    }
    return (matrix_4) => ((i_4) => {
        const pn = map((p) => Process_composeName(p, i_4), Option_fromValueWithDefault("", processNameRoot));
        const paramvalues = Option_fromValueWithDefault(empty(), map_2((f_4) => f_4(matrix_4)(i_4), parameterValueGetters));
        const parameters = map((list_5) => map_2((pv) => value_10(pv.Category), list_5), paramvalues);
        const comments = Option_fromValueWithDefault(empty(), map_2((f_5) => f_5(matrix_4)(i_4), commentGetters));
        let protocol;
        const p_1 = Protocol_make(undefined, map((f_6) => f_6(matrix_4)(i_4), protocolREFGetter), map((f_7) => f_7(matrix_4)(i_4), protocolTypeGetter), map((f_8) => f_8(matrix_4)(i_4), protocolDescriptionGetter), map((f_9) => f_9(matrix_4)(i_4), protocolURIGetter), map((f_10) => f_10(matrix_4)(i_4), protocolVersionGetter), parameters, Option_fromValueWithDefault(empty(), map_2((f_11) => f_11(matrix_4)(i_4), componentGetters)), undefined);
        let matchResult;
        if (p_1.Name == null) {
            if (p_1.ProtocolType == null) {
                if (p_1.Description == null) {
                    if (p_1.Uri == null) {
                        if (p_1.Version == null) {
                            if (p_1.Components == null) {
                                matchResult = 0;
                            }
                            else {
                                matchResult = 1;
                            }
                        }
                        else {
                            matchResult = 1;
                        }
                    }
                    else {
                        matchResult = 1;
                    }
                }
                else {
                    matchResult = 1;
                }
            }
            else {
                matchResult = 1;
            }
        }
        else {
            matchResult = 1;
        }
        switch (matchResult) {
            case 0: {
                protocol = undefined;
                break;
            }
            default:
                protocol = p_1;
        }
        let patternInput;
        const inputs = inputGetter_1(matrix_4)(i_4);
        const outputs = outputGetter_1(matrix_4)(i_4);
        patternInput = (((length(inputs) === 1) && (length(outputs) === 2)) ? [ofArray([item(0, inputs), item(0, inputs)]), outputs] : (((length(inputs) === 2) && (length(outputs) === 1)) ? [inputs, ofArray([item(0, outputs), item(0, outputs)])] : [inputs, outputs]));
        return Process_make(undefined, pn, protocol, paramvalues, undefined, undefined, undefined, undefined, patternInput[0], patternInput[1], comments);
    });
}

/**
 * Groups processes by their name, or by the name of the protocol they execute
 * 
 * Process names are taken from the Worksheet name and numbered: SheetName_1, SheetName_2, etc.
 * 
 * This function decomposes this name into a root name and a number, and groups processes by root name.
 */
export function ProcessParsing_groupProcesses(ps) {
    return List_groupBy((x) => {
        if ((x.Name != null) && (Process_decomposeName_Z721C83C5(value_10(x.Name))[1] != null)) {
            return Process_decomposeName_Z721C83C5(value_10(x.Name))[0];
        }
        else if ((x.ExecutesProtocol != null) && (value_10(x.ExecutesProtocol).Name != null)) {
            return value_10(value_10(x.ExecutesProtocol).Name);
        }
        else if ((x.Name != null) && (value_10(x.Name).indexOf("_") >= 0)) {
            const lastUnderScoreIndex = value_10(x.Name).lastIndexOf("_") | 0;
            return remove(value_10(x.Name), lastUnderScoreIndex);
        }
        else if (x.Name != null) {
            return value_10(x.Name);
        }
        else if ((x.ExecutesProtocol != null) && (value_10(x.ExecutesProtocol).ID != null)) {
            return value_10(value_10(x.ExecutesProtocol).ID);
        }
        else {
            return createMissingIdentifier();
        }
    }, ps, {
        Equals: (x_1, y) => (x_1 === y),
        GetHashCode: stringHash,
    });
}

/**
 * Merges processes with the same name, protocol and param values
 */
export function ProcessParsing_mergeIdenticalProcesses(processes) {
    const l = List_groupBy((x) => {
        if ((x.Name != null) && (Process_decomposeName_Z721C83C5(value_10(x.Name))[1] != null)) {
            return [Process_decomposeName_Z721C83C5(value_10(x.Name))[0], boxHashOption(x.ExecutesProtocol), map(boxHashSeq, x.ParameterValues), map(boxHashSeq, x.Comments)];
        }
        else if ((x.ExecutesProtocol != null) && (value_10(x.ExecutesProtocol).Name != null)) {
            return [value_10(value_10(x.ExecutesProtocol).Name), boxHashOption(x.ExecutesProtocol), map(boxHashSeq, x.ParameterValues), map(boxHashSeq, x.Comments)];
        }
        else {
            return [createMissingIdentifier(), boxHashOption(x.ExecutesProtocol), map(boxHashSeq, x.ParameterValues), map(boxHashSeq, x.Comments)];
        }
    }, processes, {
        Equals: equalArrays,
        GetHashCode: arrayHash,
    });
    return mapIndexed((i, tupledArg) => {
        const processes_1 = tupledArg[1];
        const n = tupledArg[0][0];
        const pVs = item(0, processes_1).ParameterValues;
        const inputs = Option_fromValueWithDefault(empty(), collect((p) => defaultArg(p.Inputs, empty()), processes_1));
        const outputs = Option_fromValueWithDefault(empty(), collect((p_1) => defaultArg(p_1.Outputs, empty()), processes_1));
        return Process_create_Z7C1F7FA9(undefined, (length(l) > 1) ? Process_composeName(n, i) : n, unwrap(item(0, processes_1).ExecutesProtocol), unwrap(pVs), undefined, undefined, undefined, undefined, unwrap(inputs), unwrap(outputs), unwrap(item(0, processes_1).Comments));
    }, l);
}

export function ProcessParsing_processToRows(p) {
    let list_4;
    const pvs = map_2((ppv) => [JsonTypes_decomposeParameterValue(ppv), tryGetParameterColumnIndex(ppv)], defaultArg(p.ParameterValues, empty()));
    let components;
    const matchValue = p.ExecutesProtocol;
    components = ((matchValue == null) ? empty() : map_2((ppv_1) => [JsonTypes_decomposeComponent(ppv_1), tryGetComponentIndex(ppv_1)], defaultArg(matchValue.Components, empty())));
    let protVals;
    const matchValue_1 = p.ExecutesProtocol;
    if (matchValue_1 == null) {
        protVals = empty();
    }
    else {
        const prot_1 = matchValue_1;
        protVals = toList(delay(() => append((prot_1.Name != null) ? singleton_1([new CompositeHeader(8, []), new CompositeCell(1, [value_10(prot_1.Name)])]) : empty_1(), delay(() => append((prot_1.ProtocolType != null) ? singleton_1([new CompositeHeader(4, []), new CompositeCell(0, [value_10(prot_1.ProtocolType)])]) : empty_1(), delay(() => append((prot_1.Description != null) ? singleton_1([new CompositeHeader(5, []), new CompositeCell(1, [value_10(prot_1.Description)])]) : empty_1(), delay(() => append((prot_1.Uri != null) ? singleton_1([new CompositeHeader(6, []), new CompositeCell(1, [value_10(prot_1.Uri)])]) : empty_1(), delay(() => ((prot_1.Version != null) ? singleton_1([new CompositeHeader(7, []), new CompositeCell(1, [value_10(prot_1.Version)])]) : empty_1())))))))))));
    }
    const comments = map_2((c) => [new CompositeHeader(14, [defaultArg(c.Name, "")]), new CompositeCell(1, [defaultArg(c.Value, "")])], defaultArg(p.Comments, empty()));
    return map_2((tupledArg_1) => {
        const ios = tupledArg_1[1];
        const inputForCharas = defaultArg(tryPick_1((tupledArg_2) => {
            const i_2 = tupledArg_2[0];
            if (ProcessInput__isSource(i_2) ? true : ProcessInput__isSample(i_2)) {
                return i_2;
            }
            else {
                return undefined;
            }
        }, ios), head(ios)[0]);
        const inputForType = defaultArg(tryPick_1((tupledArg_3) => {
            const i_3 = tupledArg_3[0];
            if (ProcessInput__isData(i_3) ? true : ProcessInput__isMaterial(i_3)) {
                return i_3;
            }
            else {
                return undefined;
            }
        }, ios), head(ios)[0]);
        const chars = map_2((cv) => [JsonTypes_decomposeCharacteristicValue(cv), tryGetCharacteristicColumnIndex(cv)], ProcessInput_getCharacteristicValues_5B3D5BA9(inputForCharas));
        const outputForFactors = defaultArg(tryPick_1((tupledArg_4) => {
            const o_4 = tupledArg_4[1];
            if (ProcessOutput__isSample(o_4)) {
                return o_4;
            }
            else {
                return undefined;
            }
        }, ios), head(ios)[1]);
        const outputForType = defaultArg(tryPick_1((tupledArg_5) => {
            const o_5 = tupledArg_5[1];
            if (ProcessOutput__isData(o_5) ? true : ProcessOutput__isMaterial(o_5)) {
                return o_5;
            }
            else {
                return undefined;
            }
        }, ios), head(ios)[1]);
        const vals = map_2((tuple_5) => tuple_5[0], sortBy((arg) => defaultArg(arg[1], 10000), append_1(chars, append_1(components, append_1(pvs, map_2((fv) => [JsonTypes_decomposeFactorValue(fv), tryGetFactorColumnIndex(fv)], ProcessOutput_getFactorValues_Z42C11600(outputForFactors))))), {
            Compare: comparePrimitives,
        }));
        return toList(delay(() => append(singleton_1(JsonTypes_decomposeProcessInput(inputForType)), delay(() => append(protVals, delay(() => append(vals, delay(() => append(comments, delay(() => singleton_1(JsonTypes_decomposeProcessOutput(outputForType))))))))))));
    }, List_groupBy((tupledArg) => [ProcessInput__get_Name(tupledArg[0]), ProcessOutput__get_Name(tupledArg[1])], (list_4 = defaultArg(p.Outputs, empty()), zip(defaultArg(p.Inputs, empty()), list_4)), {
        Equals: equalArrays,
        GetHashCode: arrayHash,
    }));
}

export function ARCtrl_CompositeHeader__CompositeHeader_TryParameter(this$) {
    if (this$.tag === 3) {
        return ProtocolParameter.create(undefined, this$.fields[0]);
    }
    else {
        return undefined;
    }
}

export function ARCtrl_CompositeHeader__CompositeHeader_TryFactor(this$) {
    if (this$.tag === 2) {
        return Factor.create(undefined, this$.fields[0]);
    }
    else {
        return undefined;
    }
}

export function ARCtrl_CompositeHeader__CompositeHeader_TryCharacteristic(this$) {
    if (this$.tag === 1) {
        return MaterialAttribute_create_A220A8A(undefined, this$.fields[0]);
    }
    else {
        return undefined;
    }
}

export function ARCtrl_CompositeHeader__CompositeHeader_TryComponent(this$) {
    if (this$.tag === 0) {
        return Component_create_Z2F0B38C7(undefined, undefined, this$.fields[0]);
    }
    else {
        return undefined;
    }
}

/**
 * This function is used to improve interoperability with ISA-JSON types. It is not recommended for default ARCtrl usage.
 */
export function ARCtrl_CompositeCell__CompositeCell_fromValue_Static_Z6986DF18(value, unit) {
    return JsonTypes_cellOfValue(value, unit);
}

export function CompositeRow_toProtocol(tableName, row) {
    return fold((p, hc) => {
        let matchResult, oa, v, v_1, v_2, v_3, oa_1, oa_2, unit, v_4, oa_3, t;
        switch (hc[0].tag) {
            case 4: {
                if (hc[1].tag === 0) {
                    matchResult = 0;
                    oa = hc[1].fields[0];
                }
                else {
                    matchResult = 8;
                }
                break;
            }
            case 7: {
                if (hc[1].tag === 1) {
                    matchResult = 1;
                    v = hc[1].fields[0];
                }
                else {
                    matchResult = 8;
                }
                break;
            }
            case 6: {
                if (hc[1].tag === 1) {
                    matchResult = 2;
                    v_1 = hc[1].fields[0];
                }
                else {
                    matchResult = 8;
                }
                break;
            }
            case 5: {
                if (hc[1].tag === 1) {
                    matchResult = 3;
                    v_2 = hc[1].fields[0];
                }
                else {
                    matchResult = 8;
                }
                break;
            }
            case 8: {
                if (hc[1].tag === 1) {
                    matchResult = 4;
                    v_3 = hc[1].fields[0];
                }
                else {
                    matchResult = 8;
                }
                break;
            }
            case 3: {
                matchResult = 5;
                oa_1 = hc[0].fields[0];
                break;
            }
            case 0: {
                switch (hc[1].tag) {
                    case 2: {
                        matchResult = 6;
                        oa_2 = hc[0].fields[0];
                        unit = hc[1].fields[1];
                        v_4 = hc[1].fields[0];
                        break;
                    }
                    case 0: {
                        matchResult = 7;
                        oa_3 = hc[0].fields[0];
                        t = hc[1].fields[0];
                        break;
                    }
                    default:
                        matchResult = 8;
                }
                break;
            }
            default:
                matchResult = 8;
        }
        switch (matchResult) {
            case 0:
                return Protocol_setProtocolType(p, oa);
            case 1:
                return Protocol_setVersion(p, v);
            case 2:
                return Protocol_setUri(p, v_1);
            case 3:
                return Protocol_setDescription(p, v_2);
            case 4:
                return Protocol_setName(p, v_3);
            case 5:
                return Protocol_addParameter(ProtocolParameter.create(undefined, oa_1), p);
            case 6:
                return Protocol_addComponent(Component_create_Z2F0B38C7(Value_2.fromString(v_4), unit, oa_2), p);
            case 7:
                return Protocol_addComponent(Component_create_Z2F0B38C7(new Value_2(0, [t]), undefined, oa_3), p);
            default:
                return p;
        }
    }, Protocol_create_Z414665E7(undefined, tableName), row);
}

/**
 * Create a new table from an ISA protocol.
 * 
 * The table will have at most one row, with the protocol information and the component values
 */
export function ARCtrl_ArcTable__ArcTable_fromProtocol_Static_3BF20962(p) {
    const t = ArcTable.init(defaultArg(p.Name, createMissingIdentifier()));
    const enumerator = getEnumerator(defaultArg(p.Parameters, empty()));
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const pp = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
            t.AddColumn(new CompositeHeader(3, [value_10(pp.ParameterName)]), undefined, unwrap(ARCtrl_Process_ProtocolParameter__ProtocolParameter_TryGetColumnIndex(pp)));
        }
    }
    finally {
        disposeSafe(enumerator);
    }
    const enumerator_1 = getEnumerator(defaultArg(p.Components, empty()));
    try {
        while (enumerator_1["System.Collections.IEnumerator.MoveNext"]()) {
            const c = enumerator_1["System.Collections.Generic.IEnumerator`1.get_Current"]();
            const v_1 = map((arg) => singleton_2(ARCtrl_CompositeCell__CompositeCell_fromValue_Static_Z6986DF18(arg, unwrap(c.ComponentUnit))), c.ComponentValue);
            t.AddColumn(new CompositeHeader(3, [value_10(c.ComponentType)]), unwrap(v_1), unwrap(ARCtrl_Process_Component__Component_TryGetColumnIndex(c)));
        }
    }
    finally {
        disposeSafe(enumerator_1);
    }
    map((d) => {
        t.AddProtocolDescriptionColumn([d]);
    }, p.Description);
    map((d_1) => {
        t.AddProtocolVersionColumn([d_1]);
    }, p.Version);
    map((d_2) => {
        t.AddProtocolTypeColumn([d_2]);
    }, p.ProtocolType);
    map((d_3) => {
        t.AddProtocolUriColumn([d_3]);
    }, p.Uri);
    map((d_4) => {
        t.AddProtocolNameColumn([d_4]);
    }, p.Name);
    return t;
}

/**
 * Returns the list of protocols executed in this ArcTable
 */
export function ARCtrl_ArcTable__ArcTable_GetProtocols(this$) {
    let source;
    if (this$.RowCount === 0) {
        return singleton((source = this$.Headers, fold((p, h) => {
            switch (h.tag) {
                case 4:
                    return Protocol_setProtocolType(p, new OntologyAnnotation());
                case 7:
                    return Protocol_setVersion(p, "");
                case 6:
                    return Protocol_setUri(p, "");
                case 5:
                    return Protocol_setDescription(p, "");
                case 8:
                    return Protocol_setName(p, "");
                case 3:
                    return Protocol_addParameter(ProtocolParameter.create(undefined, h.fields[0]), p);
                case 0:
                    return Protocol_addComponent(Component_create_Z2F0B38C7(undefined, undefined, h.fields[0]), p);
                default:
                    return p;
            }
        }, Protocol_create_Z414665E7(undefined, this$.Name), source)));
    }
    else {
        return List_distinct(initialize(this$.RowCount, (i) => {
            let row;
            const source_2 = this$.GetRow(i, true);
            row = zip_1(this$.Headers, source_2);
            return CompositeRow_toProtocol(this$.Name, row);
        }), {
            Equals: equals,
            GetHashCode: safeHash,
        });
    }
}

/**
 * Returns the list of processes specidified in this ArcTable
 */
export function ARCtrl_ArcTable__ArcTable_GetProcesses(this$) {
    if (this$.RowCount === 0) {
        return singleton(Process_create_Z7C1F7FA9(undefined, this$.Name));
    }
    else {
        let getter;
        const clo = ProcessParsing_getProcessGetter(this$.Name, this$.Headers);
        getter = ((arg) => {
            const clo_1 = clo(arg);
            return clo_1;
        });
        return ProcessParsing_mergeIdenticalProcesses(toList(delay(() => map_1((i) => getter(this$.Values)(i), rangeDouble(0, 1, this$.RowCount - 1)))));
    }
}

/**
 * Create a new table from a list of processes
 * 
 * The name will be used as the sheet name
 * 
 * The processes SHOULD have the same headers, or even execute the same protocol
 */
export function ARCtrl_ArcTable__ArcTable_fromProcesses_Static(name, ps) {
    const tupledArg = Unchecked_alignByHeaders(true, collect(ProcessParsing_processToRows, ps));
    return ArcTable.create(name, tupledArg[0], tupledArg[1]);
}

/**
 * Return a list of all the processes in all the tables.
 */
export function ARCtrl_ArcTables__ArcTables_GetProcesses(this$) {
    return collect(ARCtrl_ArcTable__ArcTable_GetProcesses, toList(this$.Tables));
}

/**
 * Create a collection of tables from a list of processes.
 * 
 * For this, the processes are grouped by nameroot ("nameroot_1", "nameroot_2" ...) or exectued protocol if no name exists
 * 
 * Then each group is converted to a table with this nameroot as sheetname
 */
export function ARCtrl_ArcTables__ArcTables_fromProcesses_Static_62A3309D(ps) {
    let collection;
    return new ArcTables((collection = map_2((tupledArg) => {
        const tupledArg_1 = Unchecked_alignByHeaders(true, collect(ProcessParsing_processToRows, tupledArg[1]));
        return ArcTable.create(tupledArg[0], tupledArg_1[0], tupledArg_1[1]);
    }, ProcessParsing_groupProcesses(ps)), Array.from(collection)));
}

