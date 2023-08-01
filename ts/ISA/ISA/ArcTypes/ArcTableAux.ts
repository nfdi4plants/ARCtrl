import { getItemFromDict, addToDict } from "../../../fable_modules/fable-library-ts/MapUtil.js";
import { map as map_2, defaultArg, unwrap, value as value_8, Option, some } from "../../../fable_modules/fable-library-ts/Option.js";
import { CompositeHeader_ProtocolVersion, CompositeHeader_ProtocolUri, CompositeHeader_ProtocolDescription, CompositeHeader_ProtocolType, CompositeHeader_ProtocolREF, IOType_$union, CompositeHeader_Output, IOType_Source, IOType_ImageFile, IOType_DerivedDataFile, IOType_RawDataFile, IOType_Material, CompositeHeader_Input, IOType_Sample, CompositeHeader_Characteristic, CompositeHeader_Factor, CompositeHeader_Parameter, CompositeHeader_Component, CompositeHeader_$union } from "./CompositeHeader.js";
import { float64, int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { empty as empty_2, singleton as singleton_1, append, delay, map, tryPick, toList, indexed, initialize, filter, tryFindIndex, maxBy } from "../../../fable_modules/fable-library-ts/Seq.js";
import { arrayHash, equalArrays, stringHash, int32ToString, IDisposable, IEnumerator, compareArrays, disposeSafe, getEnumerator, compare, equals, IMap, comparePrimitives } from "../../../fable_modules/fable-library-ts/Util.js";
import { CompositeCell_Unitized, CompositeCell_FreeText, CompositeCell_Term, CompositeCell, CompositeCell_$union } from "./CompositeCell.js";
import { mapIndexed, pick, exists, zip, append as append_1, sortBy, tryPick as tryPick_1, item, length, map as map_1, ofArray, singleton, choose, ofSeq, empty as empty_1, head, tail as tail_1, isEmpty, FSharpList, cons } from "../../../fable_modules/fable-library-ts/List.js";
import { CompositeColumn } from "./CompositeColumn.js";
import { max } from "../../../fable_modules/fable-library-ts/Double.js";
import { iterateIndexed } from "../../../fable_modules/fable-library-ts/Array.js";
import { FSharpSet__get_MinimumElement, FSharpSet__get_IsEmpty, difference, FSharpSet, ofSeq as ofSeq_1 } from "../../../fable_modules/fable-library-ts/Set.js";
import { Value_Name, Value_fromString_Z721C83C5, Value_$union, Value_Ontology } from "../JsonTypes/Value.js";
import { OntologyAnnotation } from "../JsonTypes/OntologyAnnotation.js";
import { Component, Component_fromOptions } from "../JsonTypes/Component.js";
import { ProcessParameterValue, ProcessParameterValue_create_2A3A2A47 } from "../JsonTypes/ProcessParameterValue.js";
import { ProtocolParameter, ProtocolParameter_create_2769312B } from "../JsonTypes/ProtocolParameter.js";
import { FactorValue, FactorValue_create_Z54E26173 } from "../JsonTypes/FactorValue.js";
import { Factor_create_Z3D2B374F } from "../JsonTypes/Factor.js";
import { toString } from "../../../fable_modules/fable-library-ts/Types.js";
import { MaterialAttributeValue, MaterialAttributeValue_create_163BDE77 } from "../JsonTypes/MaterialAttributeValue.js";
import { MaterialAttribute_create_2769312B } from "../JsonTypes/MaterialAttribute.js";
import { ProcessInput_getCharacteristicValues_Z38E7E853, ProcessInput__isMaterial, ProcessInput__isData, ProcessInput_setCharacteristicValues, ProcessInput__get_Name, ProcessInput__isSource, ProcessInput__isSample, ProcessInput_Source, ProcessInput_$union, ProcessInput_createDerivedData_Z721C83C5, ProcessInput_createRawData_Z721C83C5, ProcessInput_createImageFile_Z721C83C5, ProcessInput_createMaterial_ZEED0B34, ProcessInput_createSample_Z445EF6B3, ProcessInput_createSource_Z3083890A } from "../JsonTypes/ProcessInput.js";
import { remove, printf, toFail } from "../../../fable_modules/fable-library-ts/String.js";
import { ProcessOutput_getFactorValues_Z4A02997C, ProcessOutput__isMaterial, ProcessOutput__isData, ProcessOutput_setFactorValues, ProcessOutput__get_Name, ProcessOutput__isSample, ProcessOutput_Sample, ProcessOutput_$union, ProcessOutput_createDerivedData_Z721C83C5, ProcessOutput_createRawData_Z721C83C5, ProcessOutput_createImageFile_Z721C83C5, ProcessOutput_createMaterial_ZEED0B34, ProcessOutput_createSample_Z445EF6B3 } from "../JsonTypes/ProcessOutput.js";
import { Data } from "../JsonTypes/Data.js";
import { DataFile_$union } from "../JsonTypes/DataFile.js";
import { tryGetFactorColumnIndex, tryGetCharacteristicColumnIndex, tryGetComponentIndex, tryGetParameterColumnIndex, ISA_OntologyAnnotation__OntologyAnnotation_SetColumnIndex_Z524259A4 } from "../JsonTypes/ColumnIndex.js";
import { Source_create_Z32235993 } from "../JsonTypes/Source.js";
import { Sample_create_3A6378D6 } from "../JsonTypes/Sample.js";
import { mapOrDefault, fromValueWithDefault } from "../OptionExtensions.js";
import { Protocol, Protocol_make } from "../JsonTypes/Protocol.js";
import { Process_decomposeName_Z721C83C5, Process, Process_make } from "../JsonTypes/Process.js";
import { List_groupBy } from "../../../fable_modules/fable-library-ts/Seq2.js";
import { createMissingIdentifier } from "./Identifier.js";
import { Dictionary } from "../../../fable_modules/fable-library-ts/MutableMap.js";

/**
 * Returns the dictionary with the binding added to the given dictionary.
 * If a binding with the given key already exists in the input dictionary, the existing binding is replaced by the new binding in the result dictionary.
 */
export function Dictionary_addOrUpdateInPlace<$a, $b>(key: $a, value: $b, table: any): any {
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
export function Dictionary_tryFind<$a, $b>(key: $a, table: any): Option<$b> {
    if (table.has(key)) {
        return some(getItemFromDict(table, key));
    }
    else {
        return void 0;
    }
}

export function getColumnCount(headers: CompositeHeader_$union[]): int32 {
    return headers.length;
}

export function getRowCount(values: IMap<[int32, int32], CompositeCell_$union>): int32 {
    if (values.size === 0) {
        return 0;
    }
    else {
        return (1 + maxBy<[int32, int32], int32>((tuple: [int32, int32]): int32 => tuple[1], values.keys(), {
            Compare: comparePrimitives,
        })[1]) | 0;
    }
}

export function $007CIsUniqueExistingHeader$007C_$007C(existingHeaders: Iterable<CompositeHeader_$union>, input: CompositeHeader_$union): Option<int32> {
    switch (input.tag) {
        case /* Parameter */ 3:
        case /* Factor */ 2:
        case /* Characteristic */ 1:
        case /* Component */ 0:
        case /* FreeText */ 13:
            return void 0;
        case /* Output */ 12:
            return tryFindIndex<CompositeHeader_$union>((h: CompositeHeader_$union): boolean => (h.tag === /* Output */ 12), existingHeaders);
        case /* Input */ 11:
            return tryFindIndex<CompositeHeader_$union>((h_1: CompositeHeader_$union): boolean => (h_1.tag === /* Input */ 11), existingHeaders);
        default:
            return tryFindIndex<CompositeHeader_$union>((h_2: CompositeHeader_$union): boolean => equals(h_2, input), existingHeaders);
    }
}

/**
 * Returns the column index of the duplicate unique column in `existingHeaders`.
 */
export function tryFindDuplicateUnique(newHeader: CompositeHeader_$union, existingHeaders: Iterable<CompositeHeader_$union>): Option<int32> {
    const activePatternResult: Option<int32> = $007CIsUniqueExistingHeader$007C_$007C(existingHeaders, newHeader);
    if (activePatternResult != null) {
        const index: int32 = value_8(activePatternResult) | 0;
        return index;
    }
    else {
        return void 0;
    }
}

/**
 * Returns the column index of the duplicate unique column in `existingHeaders`.
 */
export function tryFindDuplicateUniqueInArray(existingHeaders: Iterable<CompositeHeader_$union>): FSharpList<{ HeaderType: CompositeHeader_$union, Index1: int32, Index2: int32 }> {
    const loop = (i_mut: int32, duplicateList_mut: FSharpList<{ HeaderType: CompositeHeader_$union, Index1: int32, Index2: int32 }>, headerList_mut: FSharpList<CompositeHeader_$union>): FSharpList<{ HeaderType: CompositeHeader_$union, Index1: int32, Index2: int32 }> => {
        loop:
        while (true) {
            const i: int32 = i_mut, duplicateList: FSharpList<{ HeaderType: CompositeHeader_$union, Index1: int32, Index2: int32 }> = duplicateList_mut, headerList: FSharpList<CompositeHeader_$union> = headerList_mut;
            let matchResult: int32, header: CompositeHeader_$union, tail: FSharpList<CompositeHeader_$union>;
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
                    const hasDuplicate: Option<int32> = tryFindDuplicateUnique(header!, tail!);
                    i_mut = (i + 1);
                    duplicateList_mut = ((hasDuplicate != null) ? cons({
                        HeaderType: header!,
                        Index1: i,
                        Index2: value_8(hasDuplicate),
                    }, duplicateList) : duplicateList);
                    headerList_mut = tail!;
                    continue loop;
                }
            }
            break;
        }
    };
    return loop(0, empty_1<{ HeaderType: CompositeHeader_$union, Index1: int32, Index2: int32 }>(), ofSeq<CompositeHeader_$union>(filter<CompositeHeader_$union>((x: CompositeHeader_$union): boolean => !x.IsTermColumn, existingHeaders)));
}

/**
 * Checks if given column index is valid for given number of columns.
 * 
 * if `allowAppend` = true => `0 < index <= columnCount`
 * 
 * if `allowAppend` = false => `0 < index < columnCount`
 */
export function SanityChecks_validateColumnIndex(index: int32, columnCount: int32, allowAppend: boolean): void {
    let x: int32, y: int32;
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
export function SanityChecks_validateRowIndex(index: int32, rowCount: int32, allowAppend: boolean): void {
    let x: int32, y: int32;
    if (index < 0) {
        throw new Error("Cannot insert CompositeColumn at index < 0.");
    }
    if ((x = (index | 0), (y = (rowCount | 0), allowAppend ? (compare(x, y) > 0) : (compare(x, y) >= 0)))) {
        throw new Error(`Specified index is out of table range! Table contains only ${rowCount} rows.`);
    }
}

export function SanityChecks_validateColumn(column: CompositeColumn): void {
    column.validate(true);
}

export function Unchecked_tryGetCellAt(column: int32, row: int32, cells: IMap<[int32, int32], CompositeCell_$union>): Option<CompositeCell_$union> {
    return Dictionary_tryFind<[int32, int32], CompositeCell_$union>([column, row] as [int32, int32], cells);
}

export function Unchecked_setCellAt(columnIndex: int32, rowIndex: int32, c: CompositeCell_$union, cells: IMap<[int32, int32], CompositeCell_$union>): void {
    Dictionary_addOrUpdateInPlace<[int32, int32], CompositeCell_$union>([columnIndex, rowIndex] as [int32, int32], c, cells);
}

export function Unchecked_moveCellTo(fromCol: int32, fromRow: int32, toCol: int32, toRow: int32, cells: IMap<[int32, int32], CompositeCell_$union>): void {
    const matchValue: Option<CompositeCell_$union> = Dictionary_tryFind<[int32, int32], CompositeCell_$union>([fromCol, fromRow] as [int32, int32], cells);
    if (matchValue == null) {
    }
    else {
        const c: CompositeCell_$union = value_8(matchValue);
        cells.delete([fromCol, fromRow] as [int32, int32]);
        const value_1: any = Unchecked_setCellAt(toCol, toRow, c, cells);
    }
}

export function Unchecked_removeHeader(index: int32, headers: CompositeHeader_$union[]): void {
    headers.splice(index, 1);
}

/**
 * Remove cells of one Column, change index of cells with higher index to index - 1
 */
export function Unchecked_removeColumnCells(index: int32, cells: IMap<[int32, int32], CompositeCell_$union>): void {
    let enumerator: any = getEnumerator(cells);
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const activePatternResult: [[int32, int32], CompositeCell_$union] = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
            const c: int32 = activePatternResult[0][0] | 0;
            if (c === index) {
                cells.delete([c, activePatternResult[0][1]] as [int32, int32]);
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
export function Unchecked_removeColumnCells_withIndexChange(index: int32, columnCount: int32, rowCount: int32, cells: IMap<[int32, int32], CompositeCell_$union>): void {
    for (let col: int32 = index; col <= (columnCount - 1); col++) {
        for (let row = 0; row <= (rowCount - 1); row++) {
            if (col === index) {
                cells.delete([col, row] as [int32, int32]);
            }
            else if (col > index) {
                Unchecked_moveCellTo(col, row, col - 1, row, cells);
            }
        }
    }
}

export function Unchecked_removeRowCells(rowIndex: int32, cells: IMap<[int32, int32], CompositeCell_$union>): void {
    let enumerator: any = getEnumerator(cells);
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const activePatternResult: [[int32, int32], CompositeCell_$union] = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
            const r: int32 = activePatternResult[0][1] | 0;
            if (r === rowIndex) {
                cells.delete([activePatternResult[0][0], r] as [int32, int32]);
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
export function Unchecked_removeRowCells_withIndexChange(rowIndex: int32, columnCount: int32, rowCount: int32, cells: IMap<[int32, int32], CompositeCell_$union>): void {
    for (let row: int32 = rowIndex; row <= (rowCount - 1); row++) {
        for (let col = 0; col <= (columnCount - 1); col++) {
            if (row === rowIndex) {
                cells.delete([col, row] as [int32, int32]);
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
export function Unchecked_getEmptyCellForHeader(header: CompositeHeader_$union, columCellOption: Option<CompositeCell_$union>): CompositeCell_$union {
    const matchValue: boolean = header.IsTermColumn;
    if (matchValue) {
        let matchResult: int32;
        if (columCellOption == null) {
            matchResult = 0;
        }
        else {
            switch (value_8(columCellOption).tag) {
                case /* Term */ 0: {
                    matchResult = 0;
                    break;
                }
                case /* Unitized */ 2: {
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

export function Unchecked_addColumn(newHeader: CompositeHeader_$union, newCells: CompositeCell_$union[], index: int32, forceReplace: boolean, headers: CompositeHeader_$union[], values: IMap<[int32, int32], CompositeCell_$union>): void {
    let numberOfNewColumns = 1;
    let index_1: int32 = index;
    const hasDuplicateUnique: Option<int32> = tryFindDuplicateUnique(newHeader, headers);
    if (!forceReplace && (hasDuplicateUnique != null)) {
        throw new Error(`Invalid new column \`${newHeader}\`. Table already contains header of the same type on index \`${value_8(hasDuplicateUnique)}\``);
    }
    if (hasDuplicateUnique != null) {
        numberOfNewColumns = 0;
        index_1 = (value_8(hasDuplicateUnique) | 0);
    }
    const matchValue: int32 = getColumnCount(headers) | 0;
    const matchValue_1: int32 = getRowCount(values) | 0;
    const startColCount: int32 = matchValue | 0;
    let setNewHeader: any;
    if (hasDuplicateUnique != null) {
        Unchecked_removeHeader(index_1, headers);
    }
    setNewHeader = headers.splice(index_1, 0, newHeader);
    let increaseColumnIndices: any;
    if ((index_1 < startColCount) && (hasDuplicateUnique == null)) {
        const lastColumnIndex: int32 = max(startColCount - 1, 0) | 0;
        for (let columnIndex: int32 = lastColumnIndex; columnIndex >= index_1; columnIndex--) {
            for (let rowIndex = 0; rowIndex <= matchValue_1; rowIndex++) {
                Unchecked_moveCellTo(columnIndex, rowIndex, columnIndex + numberOfNewColumns, rowIndex, values);
            }
        }
    }
    else {
        increaseColumnIndices = void 0;
    }
    let setNewCells: any;
    if (hasDuplicateUnique != null) {
        Unchecked_removeColumnCells(index_1, values);
    }
    setNewCells = iterateIndexed<CompositeCell_$union>((rowIndex_1: int32, cell: CompositeCell_$union): void => {
        Unchecked_setCellAt(index_1, rowIndex_1, cell, values);
    }, newCells);
}

export function Unchecked_fillMissingCells(headers: CompositeHeader_$union[], values: IMap<[int32, int32], CompositeCell_$union>): void {
    const rowCount: int32 = getRowCount(values) | 0;
    const lastColumnIndex: int32 = (getColumnCount(headers) - 1) | 0;
    const keys: any = values.keys();
    for (let columnIndex = 0; columnIndex <= lastColumnIndex; columnIndex++) {
        const colKeys: FSharpSet<[int32, int32]> = ofSeq_1<[int32, int32]>(filter<[int32, int32]>((tupledArg: [int32, int32]): boolean => (tupledArg[0] === columnIndex), keys), {
            Compare: compareArrays,
        });
        const missingKeys: FSharpSet<[int32, int32]> = difference<[int32, int32]>(ofSeq_1<[int32, int32]>(initialize<[int32, int32]>(rowCount, (i: int32): [int32, int32] => ([columnIndex, i] as [int32, int32])), {
            Compare: compareArrays,
        }), colKeys);
        if (!FSharpSet__get_IsEmpty(missingKeys)) {
            const empty: CompositeCell_$union = Unchecked_getEmptyCellForHeader(headers[columnIndex], FSharpSet__get_IsEmpty(colKeys) ? void 0 : getItemFromDict(values, FSharpSet__get_MinimumElement(colKeys)));
            const enumerator: IEnumerator<[int32, int32]> = getEnumerator(missingKeys);
            try {
                while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                    const forLoopVar: [int32, int32] = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                    Unchecked_setCellAt(forLoopVar[0], forLoopVar[1], empty, values);
                }
            }
            finally {
                disposeSafe(enumerator as IDisposable);
            }
        }
    }
}

export function Unchecked_addRow(index: int32, newCells: CompositeCell_$union[], headers: CompositeHeader_$union[], values: IMap<[int32, int32], CompositeCell_$union>): void {
    const rowCount: int32 = getRowCount(values) | 0;
    const columnCount: int32 = getColumnCount(headers) | 0;
    let increaseRowIndices: any;
    if (index < rowCount) {
        const lastRowIndex: int32 = max(rowCount - 1, 0) | 0;
        for (let rowIndex: int32 = lastRowIndex; rowIndex >= index; rowIndex--) {
            for (let columnIndex = 0; columnIndex <= (columnCount - 1); columnIndex++) {
                Unchecked_moveCellTo(columnIndex, rowIndex, columnIndex, rowIndex + 1, values);
            }
        }
    }
    else {
        increaseRowIndices = void 0;
    }
    const setNewCells: any = iterateIndexed<CompositeCell_$union>((columnIndex_1: int32, cell: CompositeCell_$union): void => {
        Unchecked_setCellAt(columnIndex_1, index, cell, values);
    }, newCells);
}

/**
 * Convert a CompositeCell to a ISA Value and Unit tuple.
 */
export function JsonTypes_valueOfCell(value: CompositeCell_$union): [Value_$union, Option<OntologyAnnotation>] {
    switch (value.tag) {
        case /* Term */ 0:
            return [Value_Ontology(value.fields[0]), void 0] as [Value_$union, Option<OntologyAnnotation>];
        case /* Unitized */ 2: {
            const unit: OntologyAnnotation = value.fields[1];
            return [Value_fromString_Z721C83C5(value.fields[0]), unit] as [Value_$union, Option<OntologyAnnotation>];
        }
        default:
            return [Value_fromString_Z721C83C5(value.fields[0]), void 0] as [Value_$union, Option<OntologyAnnotation>];
    }
}

/**
 * Convert a CompositeHeader and Cell tuple to a ISA Component
 */
export function JsonTypes_composeComponent(header: CompositeHeader_$union, value: CompositeCell_$union): Component {
    const patternInput: [Value_$union, Option<OntologyAnnotation>] = JsonTypes_valueOfCell(value);
    return Component_fromOptions(patternInput[0], patternInput[1], header.ToTerm());
}

/**
 * Convert a CompositeHeader and Cell tuple to a ISA ProcessParameterValue
 */
export function JsonTypes_composeParameterValue(header: CompositeHeader_$union, value: CompositeCell_$union): ProcessParameterValue {
    const patternInput: [Value_$union, Option<OntologyAnnotation>] = JsonTypes_valueOfCell(value);
    return ProcessParameterValue_create_2A3A2A47(ProtocolParameter_create_2769312B(void 0, header.ToTerm()), patternInput[0], unwrap(patternInput[1]));
}

/**
 * Convert a CompositeHeader and Cell tuple to a ISA FactorValue
 */
export function JsonTypes_composeFactorValue(header: CompositeHeader_$union, value: CompositeCell_$union): FactorValue {
    const patternInput: [Value_$union, Option<OntologyAnnotation>] = JsonTypes_valueOfCell(value);
    return FactorValue_create_Z54E26173(void 0, Factor_create_Z3D2B374F(void 0, toString(header), header.ToTerm()), patternInput[0], unwrap(patternInput[1]));
}

/**
 * Convert a CompositeHeader and Cell tuple to a ISA MaterialAttributeValue
 */
export function JsonTypes_composeCharacteristicValue(header: CompositeHeader_$union, value: CompositeCell_$union): MaterialAttributeValue {
    const patternInput: [Value_$union, Option<OntologyAnnotation>] = JsonTypes_valueOfCell(value);
    return MaterialAttributeValue_create_163BDE77(void 0, MaterialAttribute_create_2769312B(void 0, header.ToTerm()), patternInput[0], unwrap(patternInput[1]));
}

/**
 * Convert a CompositeHeader and Cell tuple to a ISA ProcessInput
 */
export function JsonTypes_composeProcessInput(header: CompositeHeader_$union, value: CompositeCell_$union): ProcessInput_$union {
    let matchResult: int32;
    if (header.tag === /* Input */ 11) {
        switch (header.fields[0].tag) {
            case /* Source */ 0: {
                matchResult = 0;
                break;
            }
            case /* Sample */ 1: {
                matchResult = 1;
                break;
            }
            case /* Material */ 5: {
                matchResult = 2;
                break;
            }
            case /* ImageFile */ 4: {
                matchResult = 3;
                break;
            }
            case /* RawDataFile */ 2: {
                matchResult = 4;
                break;
            }
            case /* DerivedDataFile */ 3: {
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
            return ProcessInput_createSource_Z3083890A(toString(value));
        case 1:
            return ProcessInput_createSample_Z445EF6B3(toString(value));
        case 2:
            return ProcessInput_createMaterial_ZEED0B34(toString(value));
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
export function JsonTypes_composeProcessOutput(header: CompositeHeader_$union, value: CompositeCell_$union): ProcessOutput_$union {
    let matchResult: int32;
    if (header.tag === /* Output */ 12) {
        switch (header.fields[0].tag) {
            case /* Sample */ 1: {
                matchResult = 0;
                break;
            }
            case /* Material */ 5: {
                matchResult = 1;
                break;
            }
            case /* ImageFile */ 4: {
                matchResult = 2;
                break;
            }
            case /* RawDataFile */ 2: {
                matchResult = 3;
                break;
            }
            case /* DerivedDataFile */ 3: {
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
            return ProcessOutput_createSample_Z445EF6B3(toString(value));
        case 1:
            return ProcessOutput_createMaterial_ZEED0B34(toString(value));
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
export function JsonTypes_cellOfValue(value: Option<Value_$union>, unit: Option<OntologyAnnotation>): CompositeCell_$union {
    const value_2: Value_$union = defaultArg(value, Value_Name(""));
    let matchResult: int32, oa: OntologyAnnotation, text: string, u: OntologyAnnotation, f: float64, u_1: OntologyAnnotation, f_1: float64, i: int32, u_2: OntologyAnnotation, i_1: int32;
    switch (value_2.tag) {
        case /* Name */ 3: {
            if (unit != null) {
                if (value_2.fields[0] === "") {
                    matchResult = 2;
                    u = value_8(unit);
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
        case /* Float */ 2: {
            if (unit == null) {
                matchResult = 4;
                f_1 = value_2.fields[0];
            }
            else {
                matchResult = 3;
                f = value_2.fields[0];
                u_1 = value_8(unit);
            }
            break;
        }
        case /* Int */ 1: {
            if (unit == null) {
                matchResult = 6;
                i_1 = value_2.fields[0];
            }
            else {
                matchResult = 5;
                i = value_2.fields[0];
                u_2 = value_8(unit);
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
            return CompositeCell_Term(oa!);
        case 1:
            return CompositeCell_FreeText(text!);
        case 2:
            return CompositeCell_Unitized("", u!);
        case 3:
            return CompositeCell_Unitized(f!.toString(), u_1!);
        case 4:
            return CompositeCell_FreeText(f_1!.toString());
        case 5:
            return CompositeCell_Unitized(int32ToString(i!), u_2!);
        case 6:
            return CompositeCell_FreeText(int32ToString(i_1!));
        default:
            return toFail(printf("Could not parse value %O with unit %O"))(value_2)(unit);
    }
}

/**
 * Convert an ISA Component to a CompositeHeader and Cell tuple
 */
export function JsonTypes_decomposeComponent(c: Component): [CompositeHeader_$union, CompositeCell_$union] {
    return [CompositeHeader_Component(value_8(c.ComponentType)), JsonTypes_cellOfValue(c.ComponentValue, c.ComponentUnit)] as [CompositeHeader_$union, CompositeCell_$union];
}

/**
 * Convert an ISA ProcessParameterValue to a CompositeHeader and Cell tuple
 */
export function JsonTypes_decomposeParameterValue(ppv: ProcessParameterValue): [CompositeHeader_$union, CompositeCell_$union] {
    return [CompositeHeader_Parameter(value_8(value_8(ppv.Category).ParameterName)), JsonTypes_cellOfValue(ppv.Value, ppv.Unit)] as [CompositeHeader_$union, CompositeCell_$union];
}

/**
 * Convert an ISA FactorValue to a CompositeHeader and Cell tuple
 */
export function JsonTypes_decomposeFactorValue(fv: FactorValue): [CompositeHeader_$union, CompositeCell_$union] {
    return [CompositeHeader_Factor(value_8(value_8(fv.Category).FactorType)), JsonTypes_cellOfValue(fv.Value, fv.Unit)] as [CompositeHeader_$union, CompositeCell_$union];
}

/**
 * Convert an ISA MaterialAttributeValue to a CompositeHeader and Cell tuple
 */
export function JsonTypes_decomposeCharacteristicValue(cv: MaterialAttributeValue): [CompositeHeader_$union, CompositeCell_$union] {
    return [CompositeHeader_Characteristic(value_8(value_8(cv.Category).CharacteristicType)), JsonTypes_cellOfValue(cv.Value, cv.Unit)] as [CompositeHeader_$union, CompositeCell_$union];
}

/**
 * Convert an ISA ProcessOutput to a CompositeHeader and Cell tuple
 */
export function JsonTypes_decomposeProcessInput(pi: ProcessInput_$union): [CompositeHeader_$union, CompositeCell_$union] {
    switch (pi.tag) {
        case /* Sample */ 1:
            return [CompositeHeader_Input(IOType_Sample()), CompositeCell_FreeText(defaultArg(pi.fields[0].Name, ""))] as [CompositeHeader_$union, CompositeCell_$union];
        case /* Material */ 3:
            return [CompositeHeader_Input(IOType_Material()), CompositeCell_FreeText(defaultArg(pi.fields[0].Name, ""))] as [CompositeHeader_$union, CompositeCell_$union];
        case /* Data */ 2: {
            const d: Data = pi.fields[0];
            const dataType: DataFile_$union = value_8(d.DataType);
            switch (dataType.tag) {
                case /* RawDataFile */ 0:
                    return [CompositeHeader_Input(IOType_RawDataFile()), CompositeCell_FreeText(defaultArg(d.Name, ""))] as [CompositeHeader_$union, CompositeCell_$union];
                case /* DerivedDataFile */ 1:
                    return [CompositeHeader_Input(IOType_DerivedDataFile()), CompositeCell_FreeText(defaultArg(d.Name, ""))] as [CompositeHeader_$union, CompositeCell_$union];
                default:
                    return [CompositeHeader_Input(IOType_ImageFile()), CompositeCell_FreeText(defaultArg(d.Name, ""))] as [CompositeHeader_$union, CompositeCell_$union];
            }
        }
        default:
            return [CompositeHeader_Input(IOType_Source()), CompositeCell_FreeText(defaultArg(pi.fields[0].Name, ""))] as [CompositeHeader_$union, CompositeCell_$union];
    }
}

/**
 * Convert an ISA ProcessOutput to a CompositeHeader and Cell tuple
 */
export function JsonTypes_decomposeProcessOutput(po: ProcessOutput_$union): [CompositeHeader_$union, CompositeCell_$union] {
    switch (po.tag) {
        case /* Material */ 2:
            return [CompositeHeader_Output(IOType_Material()), CompositeCell_FreeText(defaultArg(po.fields[0].Name, ""))] as [CompositeHeader_$union, CompositeCell_$union];
        case /* Data */ 1: {
            const d: Data = po.fields[0];
            const dataType: DataFile_$union = value_8(d.DataType);
            switch (dataType.tag) {
                case /* RawDataFile */ 0:
                    return [CompositeHeader_Output(IOType_RawDataFile()), CompositeCell_FreeText(defaultArg(d.Name, ""))] as [CompositeHeader_$union, CompositeCell_$union];
                case /* DerivedDataFile */ 1:
                    return [CompositeHeader_Output(IOType_DerivedDataFile()), CompositeCell_FreeText(defaultArg(d.Name, ""))] as [CompositeHeader_$union, CompositeCell_$union];
                default:
                    return [CompositeHeader_Output(IOType_ImageFile()), CompositeCell_FreeText(defaultArg(d.Name, ""))] as [CompositeHeader_$union, CompositeCell_$union];
            }
        }
        default:
            return [CompositeHeader_Output(IOType_Sample()), CompositeCell_FreeText(defaultArg(po.fields[0].Name, ""))] as [CompositeHeader_$union, CompositeCell_$union];
    }
}

/**
 * If the headers of a node depict a component, returns a function for parsing the values of the matrix to the values of this component
 */
export function ProcessParsing_tryComponentGetter(generalI: int32, valueI: int32, valueHeader: CompositeHeader_$union): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => Component))> {
    if (valueHeader.tag === /* Component */ 0) {
        const cat: CompositeHeader_$union = CompositeHeader_Component(ISA_OntologyAnnotation__OntologyAnnotation_SetColumnIndex_Z524259A4(valueHeader.fields[0], valueI));
        return (matrix: IMap<[int32, int32], CompositeCell_$union>): ((arg0: int32) => Component) => ((i: int32): Component => JsonTypes_composeComponent(cat, getItemFromDict(matrix, [generalI, i] as [int32, int32])));
    }
    else {
        return void 0;
    }
}

/**
 * If the headers of a node depict a protocolType, returns a function for parsing the values of the matrix to the values of this type
 */
export function ProcessParsing_tryParameterGetter(generalI: int32, valueI: int32, valueHeader: CompositeHeader_$union): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => ProcessParameterValue))> {
    if (valueHeader.tag === /* Parameter */ 3) {
        const cat: CompositeHeader_$union = CompositeHeader_Parameter(ISA_OntologyAnnotation__OntologyAnnotation_SetColumnIndex_Z524259A4(valueHeader.fields[0], valueI));
        return (matrix: IMap<[int32, int32], CompositeCell_$union>): ((arg0: int32) => ProcessParameterValue) => ((i: int32): ProcessParameterValue => JsonTypes_composeParameterValue(cat, getItemFromDict(matrix, [generalI, i] as [int32, int32])));
    }
    else {
        return void 0;
    }
}

export function ProcessParsing_tryFactorGetter(generalI: int32, valueI: int32, valueHeader: CompositeHeader_$union): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => FactorValue))> {
    if (valueHeader.tag === /* Factor */ 2) {
        const cat: CompositeHeader_$union = CompositeHeader_Factor(ISA_OntologyAnnotation__OntologyAnnotation_SetColumnIndex_Z524259A4(valueHeader.fields[0], valueI));
        return (matrix: IMap<[int32, int32], CompositeCell_$union>): ((arg0: int32) => FactorValue) => ((i: int32): FactorValue => JsonTypes_composeFactorValue(cat, getItemFromDict(matrix, [generalI, i] as [int32, int32])));
    }
    else {
        return void 0;
    }
}

export function ProcessParsing_tryCharacteristicGetter(generalI: int32, valueI: int32, valueHeader: CompositeHeader_$union): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => MaterialAttributeValue))> {
    if (valueHeader.tag === /* Characteristic */ 1) {
        const cat: CompositeHeader_$union = CompositeHeader_Characteristic(ISA_OntologyAnnotation__OntologyAnnotation_SetColumnIndex_Z524259A4(valueHeader.fields[0], valueI));
        return (matrix: IMap<[int32, int32], CompositeCell_$union>): ((arg0: int32) => MaterialAttributeValue) => ((i: int32): MaterialAttributeValue => JsonTypes_composeCharacteristicValue(cat, getItemFromDict(matrix, [generalI, i] as [int32, int32])));
    }
    else {
        return void 0;
    }
}

/**
 * If the headers of a node depict a protocolType, returns a function for parsing the values of the matrix to the values of this type
 */
export function ProcessParsing_tryGetProtocolTypeGetter(generalI: int32, header: CompositeHeader_$union): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => OntologyAnnotation))> {
    if (header.tag === /* ProtocolType */ 4) {
        return (matrix: IMap<[int32, int32], CompositeCell_$union>): ((arg0: int32) => OntologyAnnotation) => ((i: int32): OntologyAnnotation => getItemFromDict(matrix, [generalI, i] as [int32, int32]).AsTerm);
    }
    else {
        return void 0;
    }
}

export function ProcessParsing_tryGetProtocolREFGetter(generalI: int32, header: CompositeHeader_$union): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string))> {
    if (header.tag === /* ProtocolREF */ 8) {
        return (matrix: IMap<[int32, int32], CompositeCell_$union>): ((arg0: int32) => string) => ((i: int32): string => getItemFromDict(matrix, [generalI, i] as [int32, int32]).AsFreeText);
    }
    else {
        return void 0;
    }
}

export function ProcessParsing_tryGetProtocolDescriptionGetter(generalI: int32, header: CompositeHeader_$union): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string))> {
    if (header.tag === /* ProtocolDescription */ 5) {
        return (matrix: IMap<[int32, int32], CompositeCell_$union>): ((arg0: int32) => string) => ((i: int32): string => getItemFromDict(matrix, [generalI, i] as [int32, int32]).AsFreeText);
    }
    else {
        return void 0;
    }
}

export function ProcessParsing_tryGetProtocolURIGetter(generalI: int32, header: CompositeHeader_$union): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string))> {
    if (header.tag === /* ProtocolUri */ 6) {
        return (matrix: IMap<[int32, int32], CompositeCell_$union>): ((arg0: int32) => string) => ((i: int32): string => getItemFromDict(matrix, [generalI, i] as [int32, int32]).AsFreeText);
    }
    else {
        return void 0;
    }
}

export function ProcessParsing_tryGetProtocolVersionGetter(generalI: int32, header: CompositeHeader_$union): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string))> {
    if (header.tag === /* ProtocolVersion */ 7) {
        return (matrix: IMap<[int32, int32], CompositeCell_$union>): ((arg0: int32) => string) => ((i: int32): string => getItemFromDict(matrix, [generalI, i] as [int32, int32]).AsFreeText);
    }
    else {
        return void 0;
    }
}

export function ProcessParsing_tryGetInputGetter(generalI: int32, header: CompositeHeader_$union): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => ProcessInput_$union))> {
    if (header.tag === /* Input */ 11) {
        const io: IOType_$union = header.fields[0];
        return (matrix: IMap<[int32, int32], CompositeCell_$union>): ((arg0: int32) => ProcessInput_$union) => ((i: int32): ProcessInput_$union => JsonTypes_composeProcessInput(header, getItemFromDict(matrix, [generalI, i] as [int32, int32])));
    }
    else {
        return void 0;
    }
}

export function ProcessParsing_tryGetOutputGetter(generalI: int32, header: CompositeHeader_$union): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => ProcessOutput_$union))> {
    if (header.tag === /* Output */ 12) {
        const io: IOType_$union = header.fields[0];
        return (matrix: IMap<[int32, int32], CompositeCell_$union>): ((arg0: int32) => ProcessOutput_$union) => ((i: int32): ProcessOutput_$union => JsonTypes_composeProcessOutput(header, getItemFromDict(matrix, [generalI, i] as [int32, int32])));
    }
    else {
        return void 0;
    }
}

/**
 * Given the header sequence of an ArcTable, returns a function for parsing each row of the table to a process
 */
export function ProcessParsing_getProcessGetter(processNameRoot: string, headers: Iterable<CompositeHeader_$union>): ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => Process)) {
    const headers_1: Iterable<[int32, CompositeHeader_$union]> = indexed<CompositeHeader_$union>(headers);
    const valueHeaders: FSharpList<[int32, [int32, CompositeHeader_$union]]> = toList<[int32, [int32, CompositeHeader_$union]]>(indexed<[int32, CompositeHeader_$union]>(filter<[int32, CompositeHeader_$union]>((arg: [int32, CompositeHeader_$union]): boolean => arg[1].IsCvParamColumn, headers_1)));
    const charGetters: FSharpList<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => MaterialAttributeValue))> = choose<[int32, [int32, CompositeHeader_$union]], ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => MaterialAttributeValue))>((tupledArg: [int32, [int32, CompositeHeader_$union]]): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => MaterialAttributeValue))> => {
        const _arg: [int32, CompositeHeader_$union] = tupledArg[1];
        return ProcessParsing_tryCharacteristicGetter(_arg[0], tupledArg[0], _arg[1]);
    }, valueHeaders);
    const factorValueGetters: FSharpList<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => FactorValue))> = choose<[int32, [int32, CompositeHeader_$union]], ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => FactorValue))>((tupledArg_1: [int32, [int32, CompositeHeader_$union]]): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => FactorValue))> => {
        const _arg_1: [int32, CompositeHeader_$union] = tupledArg_1[1];
        return ProcessParsing_tryFactorGetter(_arg_1[0], tupledArg_1[0], _arg_1[1]);
    }, valueHeaders);
    const parameterValueGetters: FSharpList<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => ProcessParameterValue))> = choose<[int32, [int32, CompositeHeader_$union]], ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => ProcessParameterValue))>((tupledArg_2: [int32, [int32, CompositeHeader_$union]]): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => ProcessParameterValue))> => {
        const _arg_2: [int32, CompositeHeader_$union] = tupledArg_2[1];
        return ProcessParsing_tryParameterGetter(_arg_2[0], tupledArg_2[0], _arg_2[1]);
    }, valueHeaders);
    const componentGetters: FSharpList<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => Component))> = choose<[int32, [int32, CompositeHeader_$union]], ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => Component))>((tupledArg_3: [int32, [int32, CompositeHeader_$union]]): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => Component))> => {
        const _arg_3: [int32, CompositeHeader_$union] = tupledArg_3[1];
        return ProcessParsing_tryComponentGetter(_arg_3[0], tupledArg_3[0], _arg_3[1]);
    }, valueHeaders);
    const protocolTypeGetter: Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => OntologyAnnotation))> = tryPick<[int32, CompositeHeader_$union], ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => OntologyAnnotation))>((tupledArg_4: [int32, CompositeHeader_$union]): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => OntologyAnnotation))> => ProcessParsing_tryGetProtocolTypeGetter(tupledArg_4[0], tupledArg_4[1]), headers_1);
    const protocolREFGetter: Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string))> = tryPick<[int32, CompositeHeader_$union], ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string))>((tupledArg_5: [int32, CompositeHeader_$union]): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string))> => ProcessParsing_tryGetProtocolREFGetter(tupledArg_5[0], tupledArg_5[1]), headers_1);
    const protocolDescriptionGetter: Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string))> = tryPick<[int32, CompositeHeader_$union], ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string))>((tupledArg_6: [int32, CompositeHeader_$union]): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string))> => ProcessParsing_tryGetProtocolDescriptionGetter(tupledArg_6[0], tupledArg_6[1]), headers_1);
    const protocolURIGetter: Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string))> = tryPick<[int32, CompositeHeader_$union], ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string))>((tupledArg_7: [int32, CompositeHeader_$union]): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string))> => ProcessParsing_tryGetProtocolURIGetter(tupledArg_7[0], tupledArg_7[1]), headers_1);
    const protocolVersionGetter: Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string))> = tryPick<[int32, CompositeHeader_$union], ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string))>((tupledArg_8: [int32, CompositeHeader_$union]): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string))> => ProcessParsing_tryGetProtocolVersionGetter(tupledArg_8[0], tupledArg_8[1]), headers_1);
    let inputGetter_1: ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => FSharpList<ProcessInput_$union>));
    const matchValue: Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => ProcessInput_$union))> = tryPick<[int32, CompositeHeader_$union], ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => ProcessInput_$union))>((tupledArg_9: [int32, CompositeHeader_$union]): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => ProcessInput_$union))> => ProcessParsing_tryGetInputGetter(tupledArg_9[0], tupledArg_9[1]), headers_1);
    if (matchValue == null) {
        inputGetter_1 = ((matrix_1: IMap<[int32, int32], CompositeCell_$union>): ((arg0: int32) => FSharpList<ProcessInput_$union>) => ((i_1: int32): FSharpList<ProcessInput_$union> => singleton(ProcessInput_Source(Source_create_Z32235993(void 0, `${processNameRoot}_Input_${i_1}`, toList<MaterialAttributeValue>(map<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => MaterialAttributeValue)), MaterialAttributeValue>((f_1: ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => MaterialAttributeValue))): MaterialAttributeValue => f_1(matrix_1)(i_1), charGetters)))))));
    }
    else {
        const inputGetter: ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => ProcessInput_$union)) = value_8(matchValue);
        inputGetter_1 = ((matrix: IMap<[int32, int32], CompositeCell_$union>): ((arg0: int32) => FSharpList<ProcessInput_$union>) => ((i: int32): FSharpList<ProcessInput_$union> => {
            const chars: FSharpList<MaterialAttributeValue> = toList<MaterialAttributeValue>(map<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => MaterialAttributeValue)), MaterialAttributeValue>((f: ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => MaterialAttributeValue))): MaterialAttributeValue => f(matrix)(i), charGetters));
            const input: ProcessInput_$union = inputGetter(matrix)(i);
            return (!(ProcessInput__isSample(input) ? true : ProcessInput__isSource(input)) && !isEmpty(chars)) ? ofArray([input, ProcessInput_createSample_Z445EF6B3(ProcessInput__get_Name(input), chars)]) : singleton(ProcessInput_setCharacteristicValues(chars, input));
        }));
    }
    let outputGetter_1: ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => FSharpList<ProcessOutput_$union>));
    const matchValue_1: Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => ProcessOutput_$union))> = tryPick<[int32, CompositeHeader_$union], ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => ProcessOutput_$union))>((tupledArg_10: [int32, CompositeHeader_$union]): Option<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => ProcessOutput_$union))> => ProcessParsing_tryGetOutputGetter(tupledArg_10[0], tupledArg_10[1]), headers_1);
    if (matchValue_1 == null) {
        outputGetter_1 = ((matrix_3: IMap<[int32, int32], CompositeCell_$union>): ((arg0: int32) => FSharpList<ProcessOutput_$union>) => ((i_3: int32): FSharpList<ProcessOutput_$union> => singleton(ProcessOutput_Sample(Sample_create_3A6378D6(void 0, `${processNameRoot}_Output_${i_3}`, void 0, toList<FactorValue>(map<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => FactorValue)), FactorValue>((f_3: ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => FactorValue))): FactorValue => f_3(matrix_3)(i_3), factorValueGetters)))))));
    }
    else {
        const outputGetter: ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => ProcessOutput_$union)) = value_8(matchValue_1);
        outputGetter_1 = ((matrix_2: IMap<[int32, int32], CompositeCell_$union>): ((arg0: int32) => FSharpList<ProcessOutput_$union>) => ((i_2: int32): FSharpList<ProcessOutput_$union> => {
            const factors: FSharpList<FactorValue> = toList<FactorValue>(map<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => FactorValue)), FactorValue>((f_2: ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => FactorValue))): FactorValue => f_2(matrix_2)(i_2), factorValueGetters));
            const output: ProcessOutput_$union = outputGetter(matrix_2)(i_2);
            return (!ProcessOutput__isSample(output) && !isEmpty(factors)) ? ofArray([output, ProcessOutput_createSample_Z445EF6B3(ProcessOutput__get_Name(output), void 0, factors)]) : singleton(ProcessOutput_setFactorValues(factors, output));
        }));
    }
    return (matrix_4: IMap<[int32, int32], CompositeCell_$union>): ((arg0: int32) => Process) => ((i_4: int32): Process => {
        const pn: Option<string> = fromValueWithDefault<string>("", processNameRoot);
        const paramvalues: Option<FSharpList<ProcessParameterValue>> = fromValueWithDefault<FSharpList<ProcessParameterValue>>(empty_1<ProcessParameterValue>(), map_1<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => ProcessParameterValue)), ProcessParameterValue>((f_4: ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => ProcessParameterValue))): ProcessParameterValue => f_4(matrix_4)(i_4), parameterValueGetters));
        const parameters: Option<FSharpList<ProtocolParameter>> = map_2<FSharpList<ProcessParameterValue>, FSharpList<ProtocolParameter>>((list_5: FSharpList<ProcessParameterValue>): FSharpList<ProtocolParameter> => map_1<ProcessParameterValue, ProtocolParameter>((pv: ProcessParameterValue): ProtocolParameter => value_8(pv.Category), list_5), paramvalues);
        const protocol: Protocol = Protocol_make(void 0, mapOrDefault<string, ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string))>(pn, (f_5: ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string))): string => f_5(matrix_4)(i_4), protocolREFGetter), map_2<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => OntologyAnnotation)), OntologyAnnotation>((f_7: ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => OntologyAnnotation))): OntologyAnnotation => f_7(matrix_4)(i_4), protocolTypeGetter), map_2<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string)), string>((f_8: ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string))): string => f_8(matrix_4)(i_4), protocolDescriptionGetter), map_2<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string)), string>((f_9: ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string))): string => f_9(matrix_4)(i_4), protocolURIGetter), map_2<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string)), string>((f_10: ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => string))): string => f_10(matrix_4)(i_4), protocolVersionGetter), parameters, fromValueWithDefault<FSharpList<Component>>(empty_1<Component>(), map_1<((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => Component)), Component>((f_11: ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => Component))): Component => f_11(matrix_4)(i_4), componentGetters)), void 0);
        let patternInput: [FSharpList<ProcessInput_$union>, FSharpList<ProcessOutput_$union>];
        const inputs: FSharpList<ProcessInput_$union> = inputGetter_1(matrix_4)(i_4);
        const outputs: FSharpList<ProcessOutput_$union> = outputGetter_1(matrix_4)(i_4);
        patternInput = (((length(inputs) === 1) && (length(outputs) === 2)) ? ([ofArray([item(0, inputs), item(0, inputs)]), outputs] as [FSharpList<ProcessInput_$union>, FSharpList<ProcessOutput_$union>]) : (((length(inputs) === 2) && (length(outputs) === 1)) ? ([inputs, ofArray([item(0, outputs), item(0, outputs)])] as [FSharpList<ProcessInput_$union>, FSharpList<ProcessOutput_$union>]) : ([inputs, outputs] as [FSharpList<ProcessInput_$union>, FSharpList<ProcessOutput_$union>])));
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
export function ProcessParsing_groupProcesses(ps: FSharpList<Process>): FSharpList<[string, FSharpList<Process>]> {
    return List_groupBy<Process, string>((x: Process): string => {
        if ((x.Name != null) && (Process_decomposeName_Z721C83C5(value_8(x.Name))[1] != null)) {
            return Process_decomposeName_Z721C83C5(value_8(x.Name))[0];
        }
        else if ((x.ExecutesProtocol != null) && (value_8(x.ExecutesProtocol).Name != null)) {
            return value_8(value_8(x.ExecutesProtocol).Name);
        }
        else if ((x.Name != null) && (value_8(x.Name).indexOf("_") >= 0)) {
            const lastUnderScoreIndex: int32 = value_8(x.Name).lastIndexOf("_") | 0;
            return remove(value_8(x.Name), lastUnderScoreIndex);
        }
        else if ((x.ExecutesProtocol != null) && (value_8(x.ExecutesProtocol).ID != null)) {
            return value_8(value_8(x.ExecutesProtocol).ID);
        }
        else {
            return createMissingIdentifier();
        }
    }, ps, {
        Equals: (x_1: string, y: string): boolean => (x_1 === y),
        GetHashCode: stringHash,
    });
}

export function ProcessParsing_processToRows(p: Process): FSharpList<FSharpList<[CompositeHeader_$union, CompositeCell_$union]>> {
    let list_3: FSharpList<ProcessOutput_$union>;
    const pvs: FSharpList<[[CompositeHeader_$union, CompositeCell_$union], Option<int32>]> = map_1<ProcessParameterValue, [[CompositeHeader_$union, CompositeCell_$union], Option<int32>]>((ppv: ProcessParameterValue): [[CompositeHeader_$union, CompositeCell_$union], Option<int32>] => ([JsonTypes_decomposeParameterValue(ppv), tryGetParameterColumnIndex(ppv)] as [[CompositeHeader_$union, CompositeCell_$union], Option<int32>]), defaultArg(p.ParameterValues, empty_1<ProcessParameterValue>()));
    let components: FSharpList<[[CompositeHeader_$union, CompositeCell_$union], Option<int32>]>;
    const matchValue: Option<Protocol> = p.ExecutesProtocol;
    components = ((matchValue == null) ? empty_1<[[CompositeHeader_$union, CompositeCell_$union], Option<int32>]>() : map_1<Component, [[CompositeHeader_$union, CompositeCell_$union], Option<int32>]>((ppv_1: Component): [[CompositeHeader_$union, CompositeCell_$union], Option<int32>] => ([JsonTypes_decomposeComponent(ppv_1), tryGetComponentIndex(ppv_1)] as [[CompositeHeader_$union, CompositeCell_$union], Option<int32>]), defaultArg(value_8(matchValue).Components, empty_1<Component>())));
    let protVals: FSharpList<[CompositeHeader_$union, CompositeCell_$union]>;
    const matchValue_1: Option<Protocol> = p.ExecutesProtocol;
    if (matchValue_1 == null) {
        protVals = empty_1<[CompositeHeader_$union, CompositeCell_$union]>();
    }
    else {
        const prot_1: Protocol = value_8(matchValue_1);
        protVals = toList<[CompositeHeader_$union, CompositeCell_$union]>(delay<[CompositeHeader_$union, CompositeCell_$union]>((): Iterable<[CompositeHeader_$union, CompositeCell_$union]> => append<[CompositeHeader_$union, CompositeCell_$union]>((prot_1.Name != null) ? singleton_1<[CompositeHeader_$union, CompositeCell_$union]>([CompositeHeader_ProtocolREF(), CompositeCell_FreeText(value_8(prot_1.Name))] as [CompositeHeader_$union, CompositeCell_$union]) : empty_2<[CompositeHeader_$union, CompositeCell_$union]>(), delay<[CompositeHeader_$union, CompositeCell_$union]>((): Iterable<[CompositeHeader_$union, CompositeCell_$union]> => append<[CompositeHeader_$union, CompositeCell_$union]>((prot_1.ProtocolType != null) ? singleton_1<[CompositeHeader_$union, CompositeCell_$union]>([CompositeHeader_ProtocolType(), CompositeCell_Term(value_8(prot_1.ProtocolType))] as [CompositeHeader_$union, CompositeCell_$union]) : empty_2<[CompositeHeader_$union, CompositeCell_$union]>(), delay<[CompositeHeader_$union, CompositeCell_$union]>((): Iterable<[CompositeHeader_$union, CompositeCell_$union]> => append<[CompositeHeader_$union, CompositeCell_$union]>((prot_1.Description != null) ? singleton_1<[CompositeHeader_$union, CompositeCell_$union]>([CompositeHeader_ProtocolDescription(), CompositeCell_FreeText(value_8(prot_1.Description))] as [CompositeHeader_$union, CompositeCell_$union]) : empty_2<[CompositeHeader_$union, CompositeCell_$union]>(), delay<[CompositeHeader_$union, CompositeCell_$union]>((): Iterable<[CompositeHeader_$union, CompositeCell_$union]> => append<[CompositeHeader_$union, CompositeCell_$union]>((prot_1.Uri != null) ? singleton_1<[CompositeHeader_$union, CompositeCell_$union]>([CompositeHeader_ProtocolUri(), CompositeCell_FreeText(value_8(prot_1.Uri))] as [CompositeHeader_$union, CompositeCell_$union]) : empty_2<[CompositeHeader_$union, CompositeCell_$union]>(), delay<[CompositeHeader_$union, CompositeCell_$union]>((): Iterable<[CompositeHeader_$union, CompositeCell_$union]> => ((prot_1.Version != null) ? singleton_1<[CompositeHeader_$union, CompositeCell_$union]>([CompositeHeader_ProtocolVersion(), CompositeCell_FreeText(value_8(prot_1.Version))] as [CompositeHeader_$union, CompositeCell_$union]) : empty_2<[CompositeHeader_$union, CompositeCell_$union]>())))))))))));
    }
    return map_1<[[string, string], FSharpList<[ProcessInput_$union, ProcessOutput_$union]>], FSharpList<[CompositeHeader_$union, CompositeCell_$union]>>((tupledArg_1: [[string, string], FSharpList<[ProcessInput_$union, ProcessOutput_$union]>]): FSharpList<[CompositeHeader_$union, CompositeCell_$union]> => {
        const ios: FSharpList<[ProcessInput_$union, ProcessOutput_$union]> = tupledArg_1[1];
        const inputForCharas: ProcessInput_$union = defaultArg(tryPick_1<[ProcessInput_$union, ProcessOutput_$union], ProcessInput_$union>((tupledArg_2: [ProcessInput_$union, ProcessOutput_$union]): Option<ProcessInput_$union> => {
            const i_2: ProcessInput_$union = tupledArg_2[0];
            if (ProcessInput__isSource(i_2) ? true : ProcessInput__isSample(i_2)) {
                return i_2;
            }
            else {
                return void 0;
            }
        }, ios), head(ios)[0]);
        const inputForType: ProcessInput_$union = defaultArg(tryPick_1<[ProcessInput_$union, ProcessOutput_$union], ProcessInput_$union>((tupledArg_3: [ProcessInput_$union, ProcessOutput_$union]): Option<ProcessInput_$union> => {
            const i_3: ProcessInput_$union = tupledArg_3[0];
            if (ProcessInput__isData(i_3) ? true : ProcessInput__isMaterial(i_3)) {
                return i_3;
            }
            else {
                return void 0;
            }
        }, ios), head(ios)[0]);
        const chars: FSharpList<[[CompositeHeader_$union, CompositeCell_$union], Option<int32>]> = map_1<MaterialAttributeValue, [[CompositeHeader_$union, CompositeCell_$union], Option<int32>]>((cv: MaterialAttributeValue): [[CompositeHeader_$union, CompositeCell_$union], Option<int32>] => ([JsonTypes_decomposeCharacteristicValue(cv), tryGetCharacteristicColumnIndex(cv)] as [[CompositeHeader_$union, CompositeCell_$union], Option<int32>]), ProcessInput_getCharacteristicValues_Z38E7E853(inputForCharas));
        const outputForFactors: ProcessOutput_$union = defaultArg(tryPick_1<[ProcessInput_$union, ProcessOutput_$union], ProcessOutput_$union>((tupledArg_4: [ProcessInput_$union, ProcessOutput_$union]): Option<ProcessOutput_$union> => {
            const o_4: ProcessOutput_$union = tupledArg_4[1];
            if (ProcessOutput__isSample(o_4)) {
                return o_4;
            }
            else {
                return void 0;
            }
        }, ios), head(ios)[1]);
        const outputForType: ProcessOutput_$union = defaultArg(tryPick_1<[ProcessInput_$union, ProcessOutput_$union], ProcessOutput_$union>((tupledArg_5: [ProcessInput_$union, ProcessOutput_$union]): Option<ProcessOutput_$union> => {
            const o_5: ProcessOutput_$union = tupledArg_5[1];
            if (ProcessOutput__isData(o_5) ? true : ProcessOutput__isMaterial(o_5)) {
                return o_5;
            }
            else {
                return void 0;
            }
        }, ios), head(ios)[1]);
        const vals: FSharpList<[CompositeHeader_$union, CompositeCell_$union]> = map_1<[[CompositeHeader_$union, CompositeCell_$union], Option<int32>], [CompositeHeader_$union, CompositeCell_$union]>((tuple_5: [[CompositeHeader_$union, CompositeCell_$union], Option<int32>]): [CompositeHeader_$union, CompositeCell_$union] => tuple_5[0], sortBy<[[CompositeHeader_$union, CompositeCell_$union], Option<int32>], int32>((arg_2: [[CompositeHeader_$union, CompositeCell_$union], Option<int32>]): int32 => defaultArg(arg_2[1], 10000), append_1(chars, append_1(components, append_1(pvs, map_1<FactorValue, [[CompositeHeader_$union, CompositeCell_$union], Option<int32>]>((fv: FactorValue): [[CompositeHeader_$union, CompositeCell_$union], Option<int32>] => ([JsonTypes_decomposeFactorValue(fv), tryGetFactorColumnIndex(fv)] as [[CompositeHeader_$union, CompositeCell_$union], Option<int32>]), ProcessOutput_getFactorValues_Z4A02997C(outputForFactors))))), {
            Compare: comparePrimitives,
        }));
        return toList<[CompositeHeader_$union, CompositeCell_$union]>(delay<[CompositeHeader_$union, CompositeCell_$union]>((): Iterable<[CompositeHeader_$union, CompositeCell_$union]> => append<[CompositeHeader_$union, CompositeCell_$union]>(singleton_1<[CompositeHeader_$union, CompositeCell_$union]>(JsonTypes_decomposeProcessInput(inputForType)), delay<[CompositeHeader_$union, CompositeCell_$union]>((): Iterable<[CompositeHeader_$union, CompositeCell_$union]> => append<[CompositeHeader_$union, CompositeCell_$union]>(protVals, delay<[CompositeHeader_$union, CompositeCell_$union]>((): Iterable<[CompositeHeader_$union, CompositeCell_$union]> => append<[CompositeHeader_$union, CompositeCell_$union]>(vals, delay<[CompositeHeader_$union, CompositeCell_$union]>((): Iterable<[CompositeHeader_$union, CompositeCell_$union]> => singleton_1<[CompositeHeader_$union, CompositeCell_$union]>(JsonTypes_decomposeProcessOutput(outputForType))))))))));
    }, List_groupBy<[ProcessInput_$union, ProcessOutput_$union], [string, string]>((tupledArg: [ProcessInput_$union, ProcessOutput_$union]): [string, string] => ([ProcessInput__get_Name(tupledArg[0]), ProcessOutput__get_Name(tupledArg[1])] as [string, string]), (list_3 = value_8(p.Outputs), zip<ProcessInput_$union, ProcessOutput_$union>(value_8(p.Inputs), list_3)), {
        Equals: equalArrays,
        GetHashCode: arrayHash,
    }));
}

/**
 * Returns true, if two composite headers share the same main header string
 */
export function ProcessParsing_compositeHeaderEqual(ch1: CompositeHeader_$union, ch2: CompositeHeader_$union): boolean {
    return toString(ch1) === toString(ch2);
}

/**
 * From a list of rows consisting of headers and values, creates a list of combined headers and the values as a sparse matrix
 * 
 * The values cant be directly taken as they are, as there is no guarantee that the headers are aligned
 * 
 * This function aligns the headers and values by the main header string
 */
export function ProcessParsing_alignByHeaders(rows: FSharpList<FSharpList<[CompositeHeader_$union, CompositeCell_$union]>>): [CompositeHeader_$union[], IMap<[int32, int32], CompositeCell_$union>] {
    const headers: CompositeHeader_$union[] = [];
    const values: IMap<[int32, int32], CompositeCell_$union> = new Dictionary<[int32, int32], CompositeCell_$union>([], {
        Equals: equalArrays,
        GetHashCode: arrayHash,
    });
    const loop = (colI_mut: int32, rows_2_mut: FSharpList<FSharpList<[CompositeHeader_$union, CompositeCell_$union]>>): [CompositeHeader_$union[], IMap<[int32, int32], CompositeCell_$union>] => {
        loop:
        while (true) {
            const colI: int32 = colI_mut, rows_2: FSharpList<FSharpList<[CompositeHeader_$union, CompositeCell_$union]>> = rows_2_mut;
            if (!exists<FSharpList<[CompositeHeader_$union, CompositeCell_$union]>>((arg_1: FSharpList<[CompositeHeader_$union, CompositeCell_$union]>): boolean => !isEmpty(arg_1), rows_2)) {
                return [headers, values] as [CompositeHeader_$union[], IMap<[int32, int32], CompositeCell_$union>];
            }
            else {
                const firstElem: CompositeHeader_$union = pick<FSharpList<[CompositeHeader_$union, CompositeCell_$union]>, [CompositeHeader_$union, CompositeCell_$union]>((l: FSharpList<[CompositeHeader_$union, CompositeCell_$union]>): Option<[CompositeHeader_$union, CompositeCell_$union]> => (isEmpty(l) ? void 0 : head<[CompositeHeader_$union, CompositeCell_$union]>(l)), rows_2)[0];
                void (headers.push(firstElem));
                colI_mut = (colI + 1);
                rows_2_mut = mapIndexed<FSharpList<[CompositeHeader_$union, CompositeCell_$union]>, FSharpList<[CompositeHeader_$union, CompositeCell_$union]>>((rowI: int32, l_1: FSharpList<[CompositeHeader_$union, CompositeCell_$union]>): FSharpList<[CompositeHeader_$union, CompositeCell_$union]> => {
                    if (!isEmpty(l_1)) {
                        if (ProcessParsing_compositeHeaderEqual(head(l_1)[0], firstElem)) {
                            addToDict(values, [colI, rowI] as [int32, int32], head(l_1)[1]);
                            return tail_1(l_1);
                        }
                        else {
                            return l_1;
                        }
                    }
                    else {
                        return empty_1<[CompositeHeader_$union, CompositeCell_$union]>();
                    }
                }, rows_2);
                continue loop;
            }
            break;
        }
    };
    return loop(0, rows);
}

