import { toList, head as head_1, toArray, filter, tryFindIndex, maxBy } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { arrayHash, equalArrays, numberHash, disposeSafe, getEnumerator, compare, equals, safeHash, comparePrimitives } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { addToDict, getItemFromDict } from "../../fable_modules/fable-library-js.4.22.0/MapUtil.js";
import { value as value_2 } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { mapIndexed, pick, exists, ofSeq, empty, head, tail as tail_1, isEmpty, cons } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { printf, toConsole } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { List_tryPickAndRemove, Dictionary_tryFind } from "../Helper/Collections.js";
import { CompositeCell } from "./CompositeCell.js";
import { max } from "../../fable_modules/fable-library-js.4.22.0/Double.js";
import { setItem, item, map, iterateIndexed } from "../../fable_modules/fable-library-js.4.22.0/Array.js";
import { tryFind, ofArray } from "../../fable_modules/fable-library-js.4.22.0/Map.js";
import { Array_groupBy } from "../../fable_modules/fable-library-js.4.22.0/Seq2.js";
import { FSharpSet__Contains, ofArray as ofArray_1 } from "../../fable_modules/fable-library-js.4.22.0/Set.js";
import { toString } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { rangeDouble } from "../../fable_modules/fable-library-js.4.22.0/Range.js";
import { op_UnaryNegation_Int32 } from "../../fable_modules/fable-library-js.4.22.0/Int32.js";
import { Dictionary } from "../../fable_modules/fable-library-js.4.22.0/MutableMap.js";

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

export function boxHashValues(colCount, values) {
    let hash = 0;
    const rowCount = getRowCount(values) | 0;
    for (let col = 0; col <= (colCount - 1); col++) {
        for (let row = 0; row <= (rowCount - 1); row++) {
            hash = ((((-1640531527 + safeHash(getItemFromDict(values, [col, row]))) + (hash << 6)) + (hash >> 2)) | 0);
        }
    }
    return hash;
}

export function $007CIsUniqueExistingHeader$007C_$007C(existingHeaders, input) {
    switch (input.tag) {
        case 3:
        case 2:
        case 1:
        case 0:
        case 13:
            return undefined;
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
        return undefined;
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
                        Index2: value_2(hasDuplicate),
                    }, duplicateList) : duplicateList);
                    headerList_mut = tail;
                    continue loop;
                }
            }
            break;
        }
    };
    return loop(0, empty(), ofSeq(filter((x) => !x.IsTermColumn, existingHeaders)));
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
    column.Validate(true);
}

export function SanityChecks_validate(headers, values, raiseException) {
    let clo, clo_1;
    let isValid = true;
    let en = getEnumerator(values);
    while (isValid && en["System.Collections.IEnumerator.MoveNext"]()) {
        let matchValue;
        let copyOfStruct = en["System.Collections.Generic.IEnumerator`1.get_Current"]();
        matchValue = copyOfStruct[0];
        let cell;
        let copyOfStruct_1 = en["System.Collections.Generic.IEnumerator`1.get_Current"]();
        cell = copyOfStruct_1[1];
        const header = headers[matchValue[0]];
        const headerIsData = header.IsDataColumn;
        const headerIsFreetext = !header.IsTermColumn && !header.IsDataColumn;
        const cellIsNotFreetext = !cell.isFreeText;
        if (headerIsData && (!cell.isData && cellIsNotFreetext)) {
            (raiseException ? ((message) => {
                throw new Error(message);
            }) : ((clo = toConsole(printf("%s")), (arg) => {
                clo(arg);
            })))(`Invalid combination of header \`${header}\` and cell \`${cell}\`. Data header should contain either Data or Freetext cells.`);
            isValid = false;
        }
        if (headerIsFreetext && cellIsNotFreetext) {
            (raiseException ? ((message_1) => {
                throw new Error(message_1);
            }) : ((clo_1 = toConsole(printf("%s")), (arg_1) => {
                clo_1(arg_1);
            })))(`Invalid combination of header \`${header}\` and cell \`${cell}\`. Freetext header should not contain non-freetext cells.`);
            isValid = false;
        }
    }
    return isValid;
}

export function Unchecked_tryGetCellAt(column, row, cells) {
    return Dictionary_tryFind([column, row], cells);
}

/**
 * Add or update a cell in the dictionary.
 */
export function Unchecked_setCellAt(columnIndex, rowIndex, c, cells) {
    cells.set([columnIndex, rowIndex], c);
}

/**
 * Add a cell to the dictionary. If a cell already exists at the given position, it fails.
 */
export function Unchecked_addCellAt(columnIndex, rowIndex, c, cells) {
    addToDict(cells, [columnIndex, rowIndex], c);
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

/**
 * 
 */
export function Unchecked_addColumn(newHeader, newCells, index, forceReplace, onlyHeaders, headers, values) {
    let numberOfNewColumns = 1;
    let index_1 = index;
    const hasDuplicateUnique = tryFindDuplicateUnique(newHeader, headers);
    if (!forceReplace && (hasDuplicateUnique != null)) {
        throw new Error(`Invalid new column \`${newHeader}\`. Table already contains header of the same type on index \`${value_2(hasDuplicateUnique)}\``);
    }
    if (hasDuplicateUnique != null) {
        numberOfNewColumns = 0;
        index_1 = (value_2(hasDuplicateUnique) | 0);
    }
    const matchValue = getColumnCount(headers) | 0;
    const matchValue_1 = getRowCount(values) | 0;
    const startColCount = matchValue | 0;
    if (hasDuplicateUnique != null) {
        Unchecked_removeHeader(index_1, headers);
    }
    headers.splice(index_1, 0, newHeader);
    if ((index_1 < startColCount) && (hasDuplicateUnique == null)) {
        const lastColumnIndex = max(startColCount - 1, 0) | 0;
        for (let columnIndex = lastColumnIndex; columnIndex >= index_1; columnIndex--) {
            for (let rowIndex = 0; rowIndex <= matchValue_1; rowIndex++) {
                Unchecked_moveCellTo(columnIndex, rowIndex, columnIndex + numberOfNewColumns, rowIndex, values);
            }
        }
    }
    if (!onlyHeaders) {
        if (hasDuplicateUnique != null) {
            Unchecked_removeColumnCells(index_1, values);
        }
        const f = (index_1 >= startColCount) ? ((tupledArg) => ((values_1) => {
            const value = addToDict(values_1, [tupledArg[0], tupledArg[1]], tupledArg[2]);
        })) : ((tupledArg_1) => ((cells) => {
            Unchecked_setCellAt(tupledArg_1[0], tupledArg_1[1], tupledArg_1[2], cells);
        }));
        iterateIndexed((rowIndex_3, cell_1) => {
            f([index_1, rowIndex_3, cell_1])(values);
        }, newCells);
    }
}

export function Unchecked_fillMissingCells(headers, values) {
    let col;
    const rowCount = getRowCount(values) | 0;
    const columnCount = getColumnCount(headers) | 0;
    const columnKeyGroups = ofArray(Array_groupBy((tuple) => tuple[0], toArray(values.keys()), {
        Equals: (x, y) => (x === y),
        GetHashCode: numberHash,
    }), {
        Compare: comparePrimitives,
    });
    for (let columnIndex = 0; columnIndex <= (columnCount - 1); columnIndex++) {
        const header = headers[columnIndex];
        const matchValue = tryFind(columnIndex, columnKeyGroups);
        if (matchValue == null) {
            const defaultCell_1 = Unchecked_getEmptyCellForHeader(header, undefined);
            for (let rowIndex_1 = 0; rowIndex_1 <= (rowCount - 1); rowIndex_1++) {
                Unchecked_addCellAt(columnIndex, rowIndex_1, defaultCell_1.Copy(), values);
            }
        }
        else if ((col = matchValue, col.length === rowCount)) {
            const col_1 = matchValue;
        }
        else {
            const col_2 = matchValue;
            const defaultCell = Unchecked_getEmptyCellForHeader(header, getItemFromDict(values, head_1(col_2)));
            const rowKeys = ofArray_1(map((tuple_1) => tuple_1[1], col_2, Int32Array), {
                Compare: comparePrimitives,
            });
            for (let rowIndex = 0; rowIndex <= (rowCount - 1); rowIndex++) {
                if (!FSharpSet__Contains(rowKeys, rowIndex)) {
                    Unchecked_addCellAt(columnIndex, rowIndex, defaultCell.Copy(), values);
                }
            }
        }
    }
}

/**
 * Increases the table size to the given new row count and fills the new rows with the last value of the column
 */
export function Unchecked_extendToRowCount(rowCount, headers, values) {
    const columnCount = getColumnCount(headers) | 0;
    const previousRowCount = getRowCount(values) | 0;
    for (let columnIndex = 0; columnIndex <= (columnCount - 1); columnIndex++) {
        const lastValue = getItemFromDict(values, [columnIndex, previousRowCount - 1]);
        for (let rowIndex = previousRowCount - 1; rowIndex <= (rowCount - 1); rowIndex++) {
            Unchecked_setCellAt(columnIndex, rowIndex, lastValue, values);
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
        increaseRowIndices = undefined;
    }
    const setNewCells = iterateIndexed((columnIndex_1, cell) => {
        Unchecked_setCellAt(columnIndex_1, index, cell, values);
    }, newCells);
}

export function Unchecked_addRows(index, newRows, headers, values) {
    const rowCount = getRowCount(values) | 0;
    const columnCount = getColumnCount(headers) | 0;
    const numNewRows = newRows.length | 0;
    let increaseRowIndices;
    if (index < rowCount) {
        const lastRowIndex = max(rowCount - 1, 0) | 0;
        for (let rowIndex = lastRowIndex; rowIndex >= index; rowIndex--) {
            for (let columnIndex = 0; columnIndex <= (columnCount - 1); columnIndex++) {
                Unchecked_moveCellTo(columnIndex, rowIndex, columnIndex, rowIndex + numNewRows, values);
            }
        }
    }
    else {
        increaseRowIndices = undefined;
    }
    let currentRowIndex = index;
    for (let idx = 0; idx <= (newRows.length - 1); idx++) {
        const setNewCells = iterateIndexed((columnIndex_1, cell) => {
            Unchecked_setCellAt(columnIndex_1, currentRowIndex, cell, values);
        }, item(idx, newRows));
        currentRowIndex = ((currentRowIndex + 1) | 0);
    }
}

/**
 * Returns true, if two composite headers share the same main header string
 */
export function Unchecked_compositeHeaderMainColumnEqual(ch1, ch2) {
    return toString(ch1) === toString(ch2);
}

/**
 * Moves a column from one position to another
 * 
 * This function moves the column from `fromCol` to `toCol` and shifts all columns in between accordingly
 */
export function Unchecked_moveColumnTo(rowCount, fromCol, toCol, headers, values) {
    const patternInput = (fromCol < toCol) ? [-1, fromCol + 1, toCol] : [1, fromCol - 1, toCol];
    const shiftStart = patternInput[1] | 0;
    const shiftEnd = patternInput[2] | 0;
    const shift = patternInput[0] | 0;
    const header = headers[fromCol];
    const enumerator = getEnumerator(toList(rangeDouble(shiftStart, op_UnaryNegation_Int32(shift), shiftEnd)));
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const c = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]() | 0;
            setItem(headers, c + shift, headers[c]);
        }
    }
    finally {
        disposeSafe(enumerator);
    }
    setItem(headers, toCol, header);
    for (let r = 0; r <= (rowCount - 1); r++) {
        const cell = getItemFromDict(values, [fromCol, r]);
        const enumerator_1 = getEnumerator(toList(rangeDouble(shiftStart, op_UnaryNegation_Int32(shift), shiftEnd)));
        try {
            while (enumerator_1["System.Collections.IEnumerator.MoveNext"]()) {
                const c_1 = enumerator_1["System.Collections.Generic.IEnumerator`1.get_Current"]() | 0;
                values.set([c_1 + shift, r], getItemFromDict(values, [c_1, r]));
            }
        }
        finally {
            disposeSafe(enumerator_1);
        }
        values.set([toCol, r], cell);
    }
}

/**
 * From a list of rows consisting of headers and values, creates a list of combined headers and the values as a sparse matrix
 * 
 * The values cant be directly taken as they are, as there is no guarantee that the headers are aligned
 * 
 * This function aligns the headers and values by the main header string
 * 
 * If keepOrder is true, the order of values per row is kept intact, otherwise the values are allowed to be reordered
 */
export function Unchecked_alignByHeaders(keepOrder, rows) {
    const headers = [];
    const values = new Dictionary([], {
        Equals: equalArrays,
        GetHashCode: arrayHash,
    });
    const loop = (colI_mut, rows_2_mut) => {
        loop:
        while (true) {
            const colI = colI_mut, rows_2 = rows_2_mut;
            if (!exists((arg) => !isEmpty(arg), rows_2)) {
                return [headers, values];
            }
            else {
                const firstElem = pick((l) => (isEmpty(l) ? undefined : head(l)), rows_2)[0];
                void (headers.push(firstElem));
                colI_mut = (colI + 1);
                rows_2_mut = mapIndexed((rowI, l_1) => {
                    if (keepOrder) {
                        if (!isEmpty(l_1)) {
                            if (Unchecked_compositeHeaderMainColumnEqual(head(l_1)[0], firstElem)) {
                                addToDict(values, [colI, rowI], head(l_1)[1]);
                                return tail_1(l_1);
                            }
                            else {
                                return l_1;
                            }
                        }
                        else {
                            return empty();
                        }
                    }
                    else {
                        const patternInput = List_tryPickAndRemove((tupledArg) => {
                            if (Unchecked_compositeHeaderMainColumnEqual(tupledArg[0], firstElem)) {
                                return tupledArg[1];
                            }
                            else {
                                return undefined;
                            }
                        }, l_1);
                        const newL = patternInput[1];
                        const firstMatch = patternInput[0];
                        if (firstMatch == null) {
                            return newL;
                        }
                        else {
                            addToDict(values, [colI, rowI], firstMatch);
                            return newL;
                        }
                    }
                }, rows_2);
                continue loop;
            }
            break;
        }
    };
    return loop(0, rows);
}

