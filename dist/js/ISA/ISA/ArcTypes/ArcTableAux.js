import { getItemFromDict, addToDict } from "../../../fable_modules/fable-library.4.1.4/MapUtil.js";
import { map as map_2, defaultArg, unwrap, value as value_8, some } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { empty as empty_2, singleton as singleton_1, append, delay, map, tryPick, toList, indexed, initialize, filter, tryFindIndex, maxBy } from "../../../fable_modules/fable-library.4.1.4/Seq.js";
import { arrayHash, equalArrays, stringHash, int32ToString, compareArrays, disposeSafe, getEnumerator, compare, equals, comparePrimitives } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { mapIndexed, pick, exists, zip, append as append_1, sortBy, tryPick as tryPick_1, item, length, map as map_1, ofArray, singleton, choose, ofSeq, empty as empty_1, head, tail as tail_1, isEmpty, cons } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { CompositeCell } from "./CompositeCell.js";
import { max } from "../../../fable_modules/fable-library.4.1.4/Double.js";
import { iterateIndexed } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { FSharpSet__get_MinimumElement, FSharpSet__get_IsEmpty, difference, ofSeq as ofSeq_1 } from "../../../fable_modules/fable-library.4.1.4/Set.js";
import { Value_fromString_Z721C83C5, Value } from "../JsonTypes/Value.js";
import { Component_fromOptions } from "../JsonTypes/Component.js";
import { ProcessParameterValue_create_569825F3 } from "../JsonTypes/ProcessParameterValue.js";
import { ProtocolParameter_create_Z6C54B221 } from "../JsonTypes/ProtocolParameter.js";
import { FactorValue_create_18335379 } from "../JsonTypes/FactorValue.js";
import { Factor } from "../JsonTypes/Factor.js";
import { toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { MaterialAttributeValue_create_7F714043 } from "../JsonTypes/MaterialAttributeValue.js";
import { MaterialAttribute_create_Z6C54B221 } from "../JsonTypes/MaterialAttribute.js";
import { ProcessInput_getCharacteristicValues_102B6859, ProcessInput__isMaterial, ProcessInput__isData, ProcessInput_setCharacteristicValues, ProcessInput__get_Name, ProcessInput__isSource, ProcessInput__isSample, ProcessInput, ProcessInput_createDerivedData_Z721C83C5, ProcessInput_createRawData_Z721C83C5, ProcessInput_createImageFile_Z721C83C5, ProcessInput_createMaterial_2363974C, ProcessInput_createSample_Z6DF16D07, ProcessInput_createSource_7888CE42 } from "../JsonTypes/ProcessInput.js";
import { remove, printf, toFail } from "../../../fable_modules/fable-library.4.1.4/String.js";
import { ProcessOutput_getFactorValues_11830B70, ProcessOutput__isMaterial, ProcessOutput__isData, ProcessOutput_setFactorValues, ProcessOutput__get_Name, ProcessOutput__isSample, ProcessOutput, ProcessOutput_createDerivedData_Z721C83C5, ProcessOutput_createRawData_Z721C83C5, ProcessOutput_createImageFile_Z721C83C5, ProcessOutput_createMaterial_2363974C, ProcessOutput_createSample_Z6DF16D07 } from "../JsonTypes/ProcessOutput.js";
import { IOType, CompositeHeader } from "./CompositeHeader.js";
import { tryGetFactorColumnIndex, tryGetCharacteristicColumnIndex, tryGetComponentIndex, tryGetParameterColumnIndex, ARCtrl_ISA_OntologyAnnotation__OntologyAnnotation_SetColumnIndex_Z524259A4 } from "../JsonTypes/ColumnIndex.js";
import { Source_create_7A281ED9 } from "../JsonTypes/Source.js";
import { Sample_create_E50ED22 } from "../JsonTypes/Sample.js";
import { mapOrDefault, fromValueWithDefault } from "../OptionExtensions.js";
import { Protocol_make } from "../JsonTypes/Protocol.js";
import { Process_decomposeName_Z721C83C5, Process_make } from "../JsonTypes/Process.js";
import { List_groupBy } from "../../../fable_modules/fable-library.4.1.4/Seq2.js";
import { createMissingIdentifier } from "./Identifier.js";
import { Dictionary } from "../../../fable_modules/fable-library.4.1.4/MutableMap.js";

/**
 * Returns the dictionary with the binding added to the given dictionary.
 * If a binding with the given key already exists in the input dictionary, the existing binding is replaced by the new binding in the result dictionary.
 */
export function Dictionary_addOrUpdateInPlace(key, value, table) {
    if (table.has(key)) {
        table.set(key, value);
    }
    else {
        addToDict(table, key, value);
    }
    return table;
}

/**
 * Lookup an element in the dictionary, returning a <c>Some</c> value if the element is in the domain
 * of the dictionary and <c>None</c> if not.
 */
export function Dictionary_tryFind(key, table) {
    if (table.has(key)) {
        return some(getItemFromDict(table, key));
    }
    else {
        return void 0;
    }
}

export function getColumnCount(headers) {
    return headers.length;
}

export function getRowCount(values) {
    if (values.size === 0) {
        return 0;
    }
    else {
        return (1 + maxBy((tuple) => tuple[1], values.keys(), {
            Compare: comparePrimitives,
        })[1]) | 0;
    }
}

export function $007CIsUniqueExistingHeader$007C_$007C(existingHeaders, input) {
    switch (input.tag) {
        case 3:
        case 2:
        case 1:
        case 0:
        case 13:
            return void 0;
        case 12:
            return tryFindIndex((h) => (h.tag === 12), existingHeaders);
        case 11:
            return tryFindIndex((h_1) => (h_1.tag === 11), existingHeaders);
        default:
            return tryFindIndex((h_2) => equals(h_2, input), existingHeaders);
    }
}

/**
 * Returns the column index of the duplicate unique column in `existingHeaders`.
 */
export function tryFindDuplicateUnique(newHeader, existingHeaders) {
    const activePatternResult = $007CIsUniqueExistingHeader$007C_$007C(existingHeaders, newHeader);
    if (activePatternResult != null) {
        const index = activePatternResult | 0;
        return index;
    }
    else {
        return void 0;
    }
}

/**
 * Returns the column index of the duplicate unique column in `existingHeaders`.
 */
export function tryFindDuplicateUniqueInArray(existingHeaders) {
    const loop = (i_mut, duplicateList_mut, headerList_mut) => {
        loop:
        while (true) {
            const i = i_mut, duplicateList = duplicateList_mut, headerList = headerList_mut;
            let matchResult, header, tail;
            if (isEmpty(headerList)) {
                matchResult = 0;
            }
            else if (isEmpty(tail_1(headerList))) {
                matchResult = 0;
            }
            else {
                matchResult = 1;
                header = head(headerList);
                tail = tail_1(headerList);
            }
            switch (matchResult) {
                case 0:
                    return duplicateList;
                default: {
                    const hasDuplicate = tryFindDuplicateUnique(header, tail);
                    i_mut = (i + 1);
                    duplicateList_mut = ((hasDuplicate != null) ? cons({
                        HeaderType: header,
                        Index1: i,
                        Index2: value_8(hasDuplicate),
                    }, duplicateList) : duplicateList);
                    headerList_mut = tail;
                    continue loop;
                }
            }
            break;
        }
    };
    return loop(0, empty_1(), ofSeq(filter((x) => !x.IsTermColumn, existingHeaders)));
}

/**
 * Checks if given column index is valid for given number of columns.
 * 
 * if `allowAppend` = true => `0 < index <= columnCount`
 * 
 * if `allowAppend` = false => `0 < index < columnCount`
 */
export function SanityChecks_validateColumnIndex(index, columnCount, allowAppend) {
    let x, y;
    if (index < 0) {
        throw new Error("Cannot insert CompositeColumn at index < 0.");
    }
    if ((x = (index | 0), (y = (columnCount | 0), allowAppend ? (compare(x, y) > 0) : (compare(x, y) >= 0)))) {
        throw new Error(`Specified index is out of table range! Table contains only ${columnCount} columns.`);
    }
}

/**
 * Checks if given index is valid for given number of rows.
 * 
 * if `allowAppend` = true => `0 < index <= rowCount`
 * 
 * if `allowAppend` = false => `0 < index < rowCount`
 */
export function SanityChecks_validateRowIndex(index, rowCount, allowAppend) {
    let x, y;
    if (index < 0) {
        throw new Error("Cannot insert CompositeColumn at index < 0.");
    }
    if ((x = (index | 0), (y = (rowCount | 0), allowAppend ? (compare(x, y) > 0) : (compare(x, y) >= 0)))) {
        throw new Error(`Specified index is out of table range! Table contains only ${rowCount} rows.`);
    }
}

export function SanityChecks_validateColumn(column) {
    column.validate(true);
}

export function Unchecked_tryGetCellAt(column, row, cells) {
    return Dictionary_tryFind([column, row], cells);
}

export function Unchecked_setCellAt(columnIndex, rowIndex, c, cells) {
    Dictionary_addOrUpdateInPlace([columnIndex, rowIndex], c, cells);
}

export function Unchecked_moveCellTo(fromCol, fromRow, toCol, toRow, cells) {
    const matchValue = Dictionary_tryFind([fromCol, fromRow], cells);
    if (matchValue == null) {
    }
    else {
        const c = matchValue;
        cells.delete([fromCol, fromRow]);
        const value_1 = Unchecked_setCellAt(toCol, toRow, c, cells);
    }
}

export function Unchecked_removeHeader(index, headers) {
    headers.splice(index, 1);
}

/**
 * Remove cells of one Column, change index of cells with higher index to index - 1
 */
export function Unchecked_removeColumnCells(index, cells) {
    let enumerator = getEnumerator(cells);
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const activePatternResult = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
            const c = activePatternResult[0][0] | 0;
            if (c === index) {
                cells.delete([c, activePatternResult[0][1]]);
            }
        }
    }
    finally {
        disposeSafe(enumerator);
    }
}

/**
 * Remove cells of one Column, change index of cells with higher index to index - 1
 */
export function Unchecked_removeColumnCells_withIndexChange(index, columnCount, rowCount, cells) {
    for (let col = index; col <= (columnCount - 1); col++) {
        for (let row = 0; row <= (rowCount - 1); row++) {
            if (col === index) {
                cells.delete([col, row]);
            }
            else if (col > index) {
                Unchecked_moveCellTo(col, row, col - 1, row, cells);
            }
        }
    }
}

export function Unchecked_removeRowCells(rowIndex, cells) {
    let enumerator = getEnumerator(cells);
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const activePatternResult = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
            const r = activePatternResult[0][1] | 0;
            if (r === rowIndex) {
                cells.delete([activePatternResult[0][0], r]);
            }
        }
    }
    finally {
        disposeSafe(enumerator);
    }
}

/**
 * Remove cells of one Row, change index of cells with higher index to index - 1
 */
export function Unchecked_removeRowCells_withIndexChange(rowIndex, columnCount, rowCount, cells) {
    for (let row = rowIndex; row <= (rowCount - 1); row++) {
        for (let col = 0; col <= (columnCount - 1); col++) {
            if (row === rowIndex) {
                cells.delete([col, row]);
            }
            else if (row > rowIndex) {
                Unchecked_moveCellTo(col, row, col, row - 1, cells);
            }
        }
    }
}

/**
 * Get an empty cell fitting for related column header.
 * 
 * `columCellOption` is used to decide between `CompositeCell.Term` or `CompositeCell.Unitized`. `columCellOption` can be any other cell in the same column, preferably the first one.
 */
export function Unchecked_getEmptyCellForHeader(header, columCellOption) {
    const matchValue = header.IsTermColumn;
    if (matchValue) {
        let matchResult;
        if (columCellOption == null) {
            matchResult = 0;
        }
        else {
            switch (columCellOption.tag) {
                case 0: {
                    matchResult = 0;
                    break;
                }
                case 2: {
                    matchResult = 1;
                    break;
                }
                default:
                    matchResult = 2;
            }
        }
        switch (matchResult) {
            case 0:
                return CompositeCell.emptyTerm;
            case 1:
                return CompositeCell.emptyUnitized;
            default:
                throw new Error("[extendBodyCells] This should never happen, IsTermColumn header must be paired with either term or unitized cell.");
        }
    }
    else {
        return CompositeCell.emptyFreeText;
    }
}

export function Unchecked_addColumn(newHeader, newCells, index, forceReplace, headers, values) {
    let numberOfNewColumns = 1;
    let index_1 = index;
    const hasDuplicateUnique = tryFindDuplicateUnique(newHeader, headers);
    if (!forceReplace && (hasDuplicateUnique != null)) {
        throw new Error(`Invalid new column \`${newHeader}\`. Table already contains header of the same type on index \`${value_8(hasDuplicateUnique)}\``);
    }
    if (hasDuplicateUnique != null) {
        numberOfNewColumns = 0;
        index_1 = (value_8(hasDuplicateUnique) | 0);
    }
    const matchValue = getColumnCount(headers) | 0;
    const matchValue_1 = getRowCount(values) | 0;
    const startColCount = matchValue | 0;
    let setNewHeader;
    if (hasDuplicateUnique != null) {
        Unchecked_removeHeader(index_1, headers);
    }
    setNewHeader = headers.splice(index_1, 0, newHeader);
    let increaseColumnIndices;
    if ((index_1 < startColCount) && (hasDuplicateUnique == null)) {
        const lastColumnIndex = max(startColCount - 1, 0) | 0;
        for (let columnIndex = lastColumnIndex; columnIndex >= index_1; columnIndex--) {
            for (let rowIndex = 0; rowIndex <= matchValue_1; rowIndex++) {
                Unchecked_moveCellTo(columnIndex, rowIndex, columnIndex + numberOfNewColumns, rowIndex, values);
            }
        }
    }
    else {
        increaseColumnIndices = void 0;
    }
    let setNewCells;
    if (hasDuplicateUnique != null) {
        Unchecked_removeColumnCells(index_1, values);
    }
    setNewCells = iterateIndexed((rowIndex_1, cell) => {
        Unchecked_setCellAt(index_1, rowIndex_1, cell, values);
    }, newCells);
}

export function Unchecked_fillMissingCells(headers, values) {
    const rowCount = getRowCount(values) | 0;
    const lastColumnIndex = (getColumnCount(headers) - 1) | 0;
    const keys = values.keys();
    for (let columnIndex = 0; columnIndex <= lastColumnIndex; columnIndex++) {
        const colKeys = ofSeq_1(filter((tupledArg) => (tupledArg[0] === columnIndex), keys), {
            Compare: compareArrays,
        });
        const missingKeys = difference(ofSeq_1(initialize(rowCount, (i) => [columnIndex, i]), {
            Compare: compareArrays,
        }), colKeys);
        if (!FSharpSet__get_IsEmpty(missingKeys)) {
            const empty = Unchecked_getEmptyCellForHeader(headers[columnIndex], FSharpSet__get_IsEmpty(colKeys) ? void 0 : getItemFromDict(values, FSharpSet__get_MinimumElement(colKeys)));
            const enumerator = getEnumerator(missingKeys);
            try {
                while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                    const forLoopVar = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                    Unchecked_setCellAt(forLoopVar[0], forLoopVar[1], empty, values);
                }
            }
            finally {
                disposeSafe(enumerator);
            }
        }
    }
}

export function Unchecked_addRow(index, newCells, headers, values) {
    const rowCount = getRowCount(values) | 0;
    const columnCount = getColumnCount(headers) | 0;
    let increaseRowIndices;
    if (index < rowCount) {
        const lastRowIndex = max(rowCount - 1, 0) | 0;
        for (let rowIndex = lastRowIndex; rowIndex >= index; rowIndex--) {
            for (let columnIndex = 0; columnIndex <= (columnCount - 1); columnIndex++) {
                Unchecked_moveCellTo(columnIndex, rowIndex, columnIndex, rowIndex + 1, values);
            }
        }
    }
    else {
        increaseRowIndices = void 0;
    }
    const setNewCells = iterateIndexed((columnIndex_1, cell) => {
        Unchecked_setCellAt(columnIndex_1, index, cell, values);
    }, newCells);
}

/**
 * Convert a CompositeCell to a ISA Value and Unit tuple.
 */
export function JsonTypes_valueOfCell(value) {
    switch (value.tag) {
        case 0:
            return [new Value(0, [value.fields[0]]), void 0];
        case 2:
            return [Value_fromString_Z721C83C5(value.fields[0]), value.fields[1]];
        default:
            return [Value_fromString_Z721C83C5(value.fields[0]), void 0];
    }
}

/**
 * Convert a CompositeHeader and Cell tuple to a ISA Component
 */
export function JsonTypes_composeComponent(header, value) {
    const patternInput = JsonTypes_valueOfCell(value);
    return Component_fromOptions(patternInput[0], patternInput[1], header.ToTerm());
}

/**
 * Convert a CompositeHeader and Cell tuple to a ISA ProcessParameterValue
 */
export function JsonTypes_composeParameterValue(header, value) {
    const patternInput = JsonTypes_valueOfCell(value);
    return ProcessParameterValue_create_569825F3(ProtocolParameter_create_Z6C54B221(void 0, header.ToTerm()), patternInput[0], unwrap(patternInput[1]));
}

/**
 * Convert a CompositeHeader and Cell tuple to a ISA FactorValue
 */
export function JsonTypes_composeFactorValue(header, value) {
    const patternInput = JsonTypes_valueOfCell(value);
    return FactorValue_create_18335379(void 0, Factor.create(void 0, toString(header), header.ToTerm()), patternInput[0], unwrap(patternInput[1]));
}

/**
 * Convert a CompositeHeader and Cell tuple to a ISA MaterialAttributeValue
 */
export function JsonTypes_composeCharacteristicValue(header, value) {
    const patternInput = JsonTypes_valueOfCell(value);
    return MaterialAttributeValue_create_7F714043(void 0, MaterialAttribute_create_Z6C54B221(void 0, header.ToTerm()), patternInput[0], unwrap(patternInput[1]));
}

/**
 * Convert a CompositeHeader and Cell tuple to a ISA ProcessInput
 */
export function JsonTypes_composeProcessInput(header, value) {
    let matchResult;
    if (header.tag === 11) {
        switch (header.fields[0].tag) {
            case 0: {
                matchResult = 0;
                break;
            }
            case 1: {
                matchResult = 1;
                break;
            }
            case 5: {
                matchResult = 2;
                break;
            }
            case 4: {
                matchResult = 3;
                break;
            }
            case 2: {
                matchResult = 4;
                break;
            }
            case 3: {
                matchResult = 5;
                break;
            }
            default:
                matchResult = 6;
        }
    }
    else {
        matchResult = 6;
    }
    switch (matchResult) {
        case 0:
            return ProcessInput_createSource_7888CE42(toString(value));
        case 1:
            return ProcessInput_createSample_Z6DF16D07(toString(value));
        case 2:
            return ProcessInput_createMaterial_2363974C(toString(value));
        case 3:
            return ProcessInput_createImageFile_Z721C83C5(toString(value));
        case 4:
            return ProcessInput_createRawData_Z721C83C5(toString(value));
        case 5:
            return ProcessInput_createDerivedData_Z721C83C5(toString(value));
        default:
            return toFail(printf("Could not parse input header %O"))(header);
    }
}

/**
 * Convert a CompositeHeader and Cell tuple to a ISA ProcessOutput
 */
export function JsonTypes_composeProcessOutput(header, value) {
    let matchResult;
    if (header.tag === 12) {
        switch (header.fields[0].tag) {
            case 1: {
                matchResult = 0;
                break;
            }
            case 5: {
                matchResult = 1;
                break;
            }
            case 4: {
                matchResult = 2;
                break;
            }
            case 2: {
                matchResult = 3;
                break;
            }
            case 3: {
                matchResult = 4;
                break;
            }
            default:
                matchResult = 5;
        }
    }
    else {
        matchResult = 5;
    }
    switch (matchResult) {
        case 0:
            return ProcessOutput_createSample_Z6DF16D07(toString(value));
        case 1:
            return ProcessOutput_createMaterial_2363974C(toString(value));
        case 2:
            return ProcessOutput_createImageFile_Z721C83C5(toString(value));
        case 3:
            return ProcessOutput_createRawData_Z721C83C5(toString(value));
        case 4:
            return ProcessOutput_createDerivedData_Z721C83C5(toString(value));
        default:
            return toFail(printf("Could not parse output header %O"))(header);
    }
}

/**
 * Convert an ISA Value and Unit tuple to a CompositeCell
 */
export function JsonTypes_cellOfValue(value, unit) {
    const value_2 = defaultArg(value, new Value(3, [""]));
    let matchResult, oa, text, u, f, u_1, f_1, i, u_2, i_1;
    switch (value_2.tag) {
        case 3: {
            if (unit != null) {
                if (value_2.fields[0] === "") {
                    matchResult = 2;
                    u = unit;
                }
                else {
                    matchResult = 7;
                }
            }
            else {
                matchResult = 1;
                text = value_2.fields[0];
            }
            break;
        }
        case 2: {
            if (unit == null) {
                matchResult = 4;
                f_1 = value_2.fields[0];
            }
            else {
                matchResult = 3;
                f = value_2.fields[0];
                u_1 = unit;
            }
            break;
        }
        case 1: {
            if (unit == null) {
                matchResult = 6;
                i_1 = value_2.fields[0];
            }
            else {
                matchResult = 5;
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
                matchResult = 7;
            }
    }
    switch (matchResult) {
        case 0:
            return new CompositeCell(0, [oa]);
        case 1:
            return new CompositeCell(1, [text]);
        case 2:
            return new CompositeCell(2, ["", u]);
        case 3:
            return new CompositeCell(2, [f.toString(), u_1]);
        case 4:
            return new CompositeCell(1, [f_1.toString()]);
        case 5:
            return new CompositeCell(2, [int32ToString(i), u_2]);
        case 6:
            return new CompositeCell(1, [int32ToString(i_1)]);
        default:
            return toFail(printf("Could not parse value %O with unit %O"))(value_2)(unit);
    }
}

/**
 * Convert an ISA Component to a CompositeHeader and Cell tuple
 */
export function JsonTypes_decomposeComponent(c) {
    return [new CompositeHeader(0, [value_8(c.ComponentType)]), JsonTypes_cellOfValue(c.ComponentValue, c.ComponentUnit)];
}

/**
 * Convert an ISA ProcessParameterValue to a CompositeHeader and Cell tuple
 */
export function JsonTypes_decomposeParameterValue(ppv) {
    return [new CompositeHeader(3, [value_8(value_8(ppv.Category).ParameterName)]), JsonTypes_cellOfValue(ppv.Value, ppv.Unit)];
}

/**
 * Convert an ISA FactorValue to a CompositeHeader and Cell tuple
 */
export function JsonTypes_decomposeFactorValue(fv) {
    return [new CompositeHeader(2, [value_8(value_8(fv.Category).FactorType)]), JsonTypes_cellOfValue(fv.Value, fv.Unit)];
}

/**
 * Convert an ISA MaterialAttributeValue to a CompositeHeader and Cell tuple
 */
export function JsonTypes_decomposeCharacteristicValue(cv) {
    return [new CompositeHeader(1, [value_8(value_8(cv.Category).CharacteristicType)]), JsonTypes_cellOfValue(cv.Value, cv.Unit)];
}

/**
 * Convert an ISA ProcessOutput to a CompositeHeader and Cell tuple
 */
export function JsonTypes_decomposeProcessInput(pi) {
    switch (pi.tag) {
        case 1:
            return [new CompositeHeader(11, [new IOType(1, [])]), new CompositeCell(1, [defaultArg(pi.fields[0].Name, "")])];
        case 3:
            return [new CompositeHeader(11, [new IOType(5, [])]), new CompositeCell(1, [defaultArg(pi.fields[0].Name, "")])];
        case 2: {
            const d = pi.fields[0];
            const dataType = value_8(d.DataType);
            switch (dataType.tag) {
                case 0:
                    return [new CompositeHeader(11, [new IOType(2, [])]), new CompositeCell(1, [defaultArg(d.Name, "")])];
                case 1:
                    return [new CompositeHeader(11, [new IOType(3, [])]), new CompositeCell(1, [defaultArg(d.Name, "")])];
                default:
                    return [new CompositeHeader(11, [new IOType(4, [])]), new CompositeCell(1, [defaultArg(d.Name, "")])];
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
            return [new CompositeHeader(12, [new IOType(5, [])]), new CompositeCell(1, [defaultArg(po.fields[0].Name, "")])];
        case 1: {
            const d = po.fields[0];
            const dataType = value_8(d.DataType);
            switch (dataType.tag) {
                case 0:
                    return [new CompositeHeader(12, [new IOType(2, [])]), new CompositeCell(1, [defaultArg(d.Name, "")])];
                case 1:
                    return [new CompositeHeader(12, [new IOType(3, [])]), new CompositeCell(1, [defaultArg(d.Name, "")])];
                default:
                    return [new CompositeHeader(12, [new IOType(4, [])]), new CompositeCell(1, [defaultArg(d.Name, "")])];
            }
        }
        default:
            return [new CompositeHeader(12, [new IOType(1, [])]), new CompositeCell(1, [defaultArg(po.fields[0].Name, "")])];
    }
}

/**
 * If the headers of a node depict a component, returns a function for parsing the values of the matrix to the values of this component
 */
export function ProcessParsing_tryComponentGetter(generalI, valueI, valueHeader) {
    if (valueHeader.tag === 0) {
        const cat = new CompositeHeader(0, [ARCtrl_ISA_OntologyAnnotation__OntologyAnnotation_SetColumnIndex_Z524259A4(valueHeader.fields[0], valueI)]);
        return (matrix) => ((i) => JsonTypes_composeComponent(cat, getItemFromDict(matrix, [generalI, i])));
    }
    else {
        return void 0;
    }
}

/**
 * If the headers of a node depict a protocolType, returns a function for parsing the values of the matrix to the values of this type
 */
export function ProcessParsing_tryParameterGetter(generalI, valueI, valueHeader) {
    if (valueHeader.tag === 3) {
        const cat = new CompositeHeader(3, [ARCtrl_ISA_OntologyAnnotation__OntologyAnnotation_SetColumnIndex_Z524259A4(valueHeader.fields[0], valueI)]);
        return (matrix) => ((i) => JsonTypes_composeParameterValue(cat, getItemFromDict(matrix, [generalI, i])));
    }
    else {
        return void 0;
    }
}

export function ProcessParsing_tryFactorGetter(generalI, valueI, valueHeader) {
    if (valueHeader.tag === 2) {
        const cat = new CompositeHeader(2, [ARCtrl_ISA_OntologyAnnotation__OntologyAnnotation_SetColumnIndex_Z524259A4(valueHeader.fields[0], valueI)]);
        return (matrix) => ((i) => JsonTypes_composeFactorValue(cat, getItemFromDict(matrix, [generalI, i])));
    }
    else {
        return void 0;
    }
}

export function ProcessParsing_tryCharacteristicGetter(generalI, valueI, valueHeader) {
    if (valueHeader.tag === 1) {
        const cat = new CompositeHeader(1, [ARCtrl_ISA_OntologyAnnotation__OntologyAnnotation_SetColumnIndex_Z524259A4(valueHeader.fields[0], valueI)]);
        return (matrix) => ((i) => JsonTypes_composeCharacteristicValue(cat, getItemFromDict(matrix, [generalI, i])));
    }
    else {
        return void 0;
    }
}

/**
 * If the headers of a node depict a protocolType, returns a function for parsing the values of the matrix to the values of this type
 */
export function ProcessParsing_tryGetProtocolTypeGetter(generalI, header) {
    if (header.tag === 4) {
        return (matrix) => ((i) => getItemFromDict(matrix, [generalI, i]).AsTerm);
    }
    else {
        return void 0;
    }
}

export function ProcessParsing_tryGetProtocolREFGetter(generalI, header) {
    if (header.tag === 8) {
        return (matrix) => ((i) => getItemFromDict(matrix, [generalI, i]).AsFreeText);
    }
    else {
        return void 0;
    }
}

export function ProcessParsing_tryGetProtocolDescriptionGetter(generalI, header) {
    if (header.tag === 5) {
        return (matrix) => ((i) => getItemFromDict(matrix, [generalI, i]).AsFreeText);
    }
    else {
        return void 0;
    }
}

export function ProcessParsing_tryGetProtocolURIGetter(generalI, header) {
    if (header.tag === 6) {
        return (matrix) => ((i) => getItemFromDict(matrix, [generalI, i]).AsFreeText);
    }
    else {
        return void 0;
    }
}

export function ProcessParsing_tryGetProtocolVersionGetter(generalI, header) {
    if (header.tag === 7) {
        return (matrix) => ((i) => getItemFromDict(matrix, [generalI, i]).AsFreeText);
    }
    else {
        return void 0;
    }
}

export function ProcessParsing_tryGetInputGetter(generalI, header) {
    if (header.tag === 11) {
        return (matrix) => ((i) => JsonTypes_composeProcessInput(header, getItemFromDict(matrix, [generalI, i])));
    }
    else {
        return void 0;
    }
}

export function ProcessParsing_tryGetOutputGetter(generalI, header) {
    if (header.tag === 12) {
        return (matrix) => ((i) => JsonTypes_composeProcessOutput(header, getItemFromDict(matrix, [generalI, i])));
    }
    else {
        return void 0;
    }
}

/**
 * Given the header sequence of an ArcTable, returns a function for parsing each row of the table to a process
 */
export function ProcessParsing_getProcessGetter(processNameRoot, headers) {
    const headers_1 = indexed(headers);
    const valueHeaders = toList(indexed(filter((arg) => arg[1].IsCvParamColumn, headers_1)));
    const charGetters = choose((tupledArg) => {
        const _arg = tupledArg[1];
        return ProcessParsing_tryCharacteristicGetter(_arg[0], tupledArg[0], _arg[1]);
    }, valueHeaders);
    const factorValueGetters = choose((tupledArg_1) => {
        const _arg_1 = tupledArg_1[1];
        return ProcessParsing_tryFactorGetter(_arg_1[0], tupledArg_1[0], _arg_1[1]);
    }, valueHeaders);
    const parameterValueGetters = choose((tupledArg_2) => {
        const _arg_2 = tupledArg_2[1];
        return ProcessParsing_tryParameterGetter(_arg_2[0], tupledArg_2[0], _arg_2[1]);
    }, valueHeaders);
    const componentGetters = choose((tupledArg_3) => {
        const _arg_3 = tupledArg_3[1];
        return ProcessParsing_tryComponentGetter(_arg_3[0], tupledArg_3[0], _arg_3[1]);
    }, valueHeaders);
    const protocolTypeGetter = tryPick((tupledArg_4) => ProcessParsing_tryGetProtocolTypeGetter(tupledArg_4[0], tupledArg_4[1]), headers_1);
    const protocolREFGetter = tryPick((tupledArg_5) => ProcessParsing_tryGetProtocolREFGetter(tupledArg_5[0], tupledArg_5[1]), headers_1);
    const protocolDescriptionGetter = tryPick((tupledArg_6) => ProcessParsing_tryGetProtocolDescriptionGetter(tupledArg_6[0], tupledArg_6[1]), headers_1);
    const protocolURIGetter = tryPick((tupledArg_7) => ProcessParsing_tryGetProtocolURIGetter(tupledArg_7[0], tupledArg_7[1]), headers_1);
    const protocolVersionGetter = tryPick((tupledArg_8) => ProcessParsing_tryGetProtocolVersionGetter(tupledArg_8[0], tupledArg_8[1]), headers_1);
    let inputGetter_1;
    const matchValue = tryPick((tupledArg_9) => ProcessParsing_tryGetInputGetter(tupledArg_9[0], tupledArg_9[1]), headers_1);
    if (matchValue == null) {
        inputGetter_1 = ((matrix_1) => ((i_1) => singleton(new ProcessInput(0, [Source_create_7A281ED9(void 0, `${processNameRoot}_Input_${i_1}`, toList(map((f_1) => f_1(matrix_1)(i_1), charGetters)))]))));
    }
    else {
        const inputGetter = matchValue;
        inputGetter_1 = ((matrix) => ((i) => {
            const chars = toList(map((f) => f(matrix)(i), charGetters));
            const input = inputGetter(matrix)(i);
            return (!(ProcessInput__isSample(input) ? true : ProcessInput__isSource(input)) && !isEmpty(chars)) ? ofArray([input, ProcessInput_createSample_Z6DF16D07(ProcessInput__get_Name(input), chars)]) : singleton(ProcessInput_setCharacteristicValues(chars, input));
        }));
    }
    let outputGetter_1;
    const matchValue_1 = tryPick((tupledArg_10) => ProcessParsing_tryGetOutputGetter(tupledArg_10[0], tupledArg_10[1]), headers_1);
    if (matchValue_1 == null) {
        outputGetter_1 = ((matrix_3) => ((i_3) => singleton(new ProcessOutput(0, [Sample_create_E50ED22(void 0, `${processNameRoot}_Output_${i_3}`, void 0, toList(map((f_3) => f_3(matrix_3)(i_3), factorValueGetters)))]))));
    }
    else {
        const outputGetter = matchValue_1;
        outputGetter_1 = ((matrix_2) => ((i_2) => {
            const factors = toList(map((f_2) => f_2(matrix_2)(i_2), factorValueGetters));
            const output = outputGetter(matrix_2)(i_2);
            return (!ProcessOutput__isSample(output) && !isEmpty(factors)) ? ofArray([output, ProcessOutput_createSample_Z6DF16D07(ProcessOutput__get_Name(output), void 0, factors)]) : singleton(ProcessOutput_setFactorValues(factors, output));
        }));
    }
    return (matrix_4) => ((i_4) => {
        const pn = fromValueWithDefault("", processNameRoot);
        const paramvalues = fromValueWithDefault(empty_1(), map_1((f_4) => f_4(matrix_4)(i_4), parameterValueGetters));
        const parameters = map_2((list_5) => map_1((pv) => value_8(pv.Category), list_5), paramvalues);
        const protocol = Protocol_make(void 0, mapOrDefault(pn, (f_5) => f_5(matrix_4)(i_4), protocolREFGetter), map_2((f_7) => f_7(matrix_4)(i_4), protocolTypeGetter), map_2((f_8) => f_8(matrix_4)(i_4), protocolDescriptionGetter), map_2((f_9) => f_9(matrix_4)(i_4), protocolURIGetter), map_2((f_10) => f_10(matrix_4)(i_4), protocolVersionGetter), parameters, fromValueWithDefault(empty_1(), map_1((f_11) => f_11(matrix_4)(i_4), componentGetters)), void 0);
        let patternInput;
        const inputs = inputGetter_1(matrix_4)(i_4);
        const outputs = outputGetter_1(matrix_4)(i_4);
        patternInput = (((length(inputs) === 1) && (length(outputs) === 2)) ? [ofArray([item(0, inputs), item(0, inputs)]), outputs] : (((length(inputs) === 2) && (length(outputs) === 1)) ? [inputs, ofArray([item(0, outputs), item(0, outputs)])] : [inputs, outputs]));
        return Process_make(void 0, pn, protocol, paramvalues, void 0, void 0, void 0, void 0, patternInput[0], patternInput[1], void 0);
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
        if ((x.Name != null) && (Process_decomposeName_Z721C83C5(value_8(x.Name))[1] != null)) {
            return Process_decomposeName_Z721C83C5(value_8(x.Name))[0];
        }
        else if ((x.ExecutesProtocol != null) && (value_8(x.ExecutesProtocol).Name != null)) {
            return value_8(value_8(x.ExecutesProtocol).Name);
        }
        else if ((x.Name != null) && (value_8(x.Name).indexOf("_") >= 0)) {
            const lastUnderScoreIndex = value_8(x.Name).lastIndexOf("_") | 0;
            return remove(value_8(x.Name), lastUnderScoreIndex);
        }
        else if ((x.ExecutesProtocol != null) && (value_8(x.ExecutesProtocol).ID != null)) {
            return value_8(value_8(x.ExecutesProtocol).ID);
        }
        else {
            return createMissingIdentifier();
        }
    }, ps, {
        Equals: (x_1, y) => (x_1 === y),
        GetHashCode: stringHash,
    });
}

export function ProcessParsing_processToRows(p) {
    let list_3;
    const pvs = map_1((ppv) => [JsonTypes_decomposeParameterValue(ppv), tryGetParameterColumnIndex(ppv)], defaultArg(p.ParameterValues, empty_1()));
    let components;
    const matchValue = p.ExecutesProtocol;
    components = ((matchValue == null) ? empty_1() : map_1((ppv_1) => [JsonTypes_decomposeComponent(ppv_1), tryGetComponentIndex(ppv_1)], defaultArg(matchValue.Components, empty_1())));
    let protVals;
    const matchValue_1 = p.ExecutesProtocol;
    if (matchValue_1 == null) {
        protVals = empty_1();
    }
    else {
        const prot_1 = matchValue_1;
        protVals = toList(delay(() => append((prot_1.Name != null) ? singleton_1([new CompositeHeader(8, []), new CompositeCell(1, [value_8(prot_1.Name)])]) : empty_2(), delay(() => append((prot_1.ProtocolType != null) ? singleton_1([new CompositeHeader(4, []), new CompositeCell(0, [value_8(prot_1.ProtocolType)])]) : empty_2(), delay(() => append((prot_1.Description != null) ? singleton_1([new CompositeHeader(5, []), new CompositeCell(1, [value_8(prot_1.Description)])]) : empty_2(), delay(() => append((prot_1.Uri != null) ? singleton_1([new CompositeHeader(6, []), new CompositeCell(1, [value_8(prot_1.Uri)])]) : empty_2(), delay(() => ((prot_1.Version != null) ? singleton_1([new CompositeHeader(7, []), new CompositeCell(1, [value_8(prot_1.Version)])]) : empty_2())))))))))));
    }
    return map_1((tupledArg_1) => {
        const ios = tupledArg_1[1];
        const inputForCharas = defaultArg(tryPick_1((tupledArg_2) => {
            const i_2 = tupledArg_2[0];
            if (ProcessInput__isSource(i_2) ? true : ProcessInput__isSample(i_2)) {
                return i_2;
            }
            else {
                return void 0;
            }
        }, ios), head(ios)[0]);
        const inputForType = defaultArg(tryPick_1((tupledArg_3) => {
            const i_3 = tupledArg_3[0];
            if (ProcessInput__isData(i_3) ? true : ProcessInput__isMaterial(i_3)) {
                return i_3;
            }
            else {
                return void 0;
            }
        }, ios), head(ios)[0]);
        const chars = map_1((cv) => [JsonTypes_decomposeCharacteristicValue(cv), tryGetCharacteristicColumnIndex(cv)], ProcessInput_getCharacteristicValues_102B6859(inputForCharas));
        const outputForFactors = defaultArg(tryPick_1((tupledArg_4) => {
            const o_4 = tupledArg_4[1];
            if (ProcessOutput__isSample(o_4)) {
                return o_4;
            }
            else {
                return void 0;
            }
        }, ios), head(ios)[1]);
        const outputForType = defaultArg(tryPick_1((tupledArg_5) => {
            const o_5 = tupledArg_5[1];
            if (ProcessOutput__isData(o_5) ? true : ProcessOutput__isMaterial(o_5)) {
                return o_5;
            }
            else {
                return void 0;
            }
        }, ios), head(ios)[1]);
        const vals = map_1((tuple_5) => tuple_5[0], sortBy((arg_2) => defaultArg(arg_2[1], 10000), append_1(chars, append_1(components, append_1(pvs, map_1((fv) => [JsonTypes_decomposeFactorValue(fv), tryGetFactorColumnIndex(fv)], ProcessOutput_getFactorValues_11830B70(outputForFactors))))), {
            Compare: comparePrimitives,
        }));
        return toList(delay(() => append(singleton_1(JsonTypes_decomposeProcessInput(inputForType)), delay(() => append(protVals, delay(() => append(vals, delay(() => singleton_1(JsonTypes_decomposeProcessOutput(outputForType))))))))));
    }, List_groupBy((tupledArg) => [ProcessInput__get_Name(tupledArg[0]), ProcessOutput__get_Name(tupledArg[1])], (list_3 = value_8(p.Outputs), zip(value_8(p.Inputs), list_3)), {
        Equals: equalArrays,
        GetHashCode: arrayHash,
    }));
}

/**
 * Returns true, if two composite headers share the same main header string
 */
export function ProcessParsing_compositeHeaderEqual(ch1, ch2) {
    return toString(ch1) === toString(ch2);
}

/**
 * From a list of rows consisting of headers and values, creates a list of combined headers and the values as a sparse matrix
 * 
 * The values cant be directly taken as they are, as there is no guarantee that the headers are aligned
 * 
 * This function aligns the headers and values by the main header string
 */
export function ProcessParsing_alignByHeaders(rows) {
    const headers = [];
    const values = new Dictionary([], {
        Equals: equalArrays,
        GetHashCode: arrayHash,
    });
    const loop = (colI_mut, rows_2_mut) => {
        loop:
        while (true) {
            const colI = colI_mut, rows_2 = rows_2_mut;
            if (!exists((arg_1) => !isEmpty(arg_1), rows_2)) {
                return [headers, values];
            }
            else {
                const firstElem = pick((l) => (isEmpty(l) ? void 0 : head(l)), rows_2)[0];
                void (headers.push(firstElem));
                colI_mut = (colI + 1);
                rows_2_mut = mapIndexed((rowI, l_1) => {
                    if (!isEmpty(l_1)) {
                        if (ProcessParsing_compositeHeaderEqual(head(l_1)[0], firstElem)) {
                            addToDict(values, [colI, rowI], head(l_1)[1]);
                            return tail_1(l_1);
                        }
                        else {
                            return l_1;
                        }
                    }
                    else {
                        return empty_1();
                    }
                }, rows_2);
                continue loop;
            }
            break;
        }
    };
    return loop(0, rows);
}

