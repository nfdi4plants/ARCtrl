import { defaultArg, map, unwrap } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { ArcTables_fromProcesses_134EBDED, ArcTables__get_Tables, ArcTables__GetProcesses, ArcTables__GetRow_Z18115A39, ArcTables__GetRowAt_Z37302880, ArcTables__UpdateRow_1BFE2CFB, ArcTables__UpdateRowAt_1CF7B5DC, ArcTables__RemoveRow_Z18115A39, ArcTables__RemoveRowAt_Z37302880, ArcTables__AddRow_Z16315DE5, ArcTables__AddRowAt_Z12CDB784, ArcTables__GetColumn_Z18115A39, ArcTables__GetColumnAt_Z37302880, ArcTables__UpdateColumn_Z6083042A, ArcTables__UpdateColumnAt_21300791, ArcTables__RemoveColumn_Z18115A39, ArcTables__RemoveColumnAt_Z37302880, ArcTables__AddColumn_1FF50D3C, ArcTables__AddColumnAt_6A9784DB, ArcTables__RenameTable_Z384F8060, ArcTables__RenameTableAt_Z176EF219, ArcTables__MapTable_27DD7B1B, ArcTables__MapTableAt_8FC095C, ArcTables__RemoveTable_Z721C83C5, ArcTables__RemoveTableAt_Z524259A4, ArcTables__UpdateTable_51766571, ArcTables__UpdateTableAt_7E571736, ArcTables__GetTable_Z721C83C5, ArcTables__GetTableAt_Z524259A4, ArcTables__InitTables_7B28792B, ArcTables__InitTable_3B406CA4, ArcTables__AddTables_Z2D453886, ArcTables__AddTable_EC12B15, ArcTables__get_TableNames, ArcTables_$ctor_Z18C2F36D, ArcTables__get_Count } from "./ArcTables.js";
import { disposeSafe, getEnumerator } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { AssayMaterials_get_empty, AssayMaterials_create_Z253F0553 } from "../JsonTypes/AssayMaterials.js";
import { fromValueWithDefault } from "../OptionExtensions.js";
import { toArray, ofArray, empty } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { getUnits, getCharacteristics, getData, getMaterials, getSamples } from "../JsonTypes/ProcessSequence.js";
import { Assay_create_3D372A24 } from "../JsonTypes/Assay.js";
import { Assay_identifierFromFileName, createMissingIdentifier, Assay_fileNameFromIdentifier, isMissingIdentifier } from "./Identifier.js";
import { class_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";

export class ArcAssay {
    constructor(identifier, measurementType, technologyType, technologyPlatform, tables, performers, comments) {
        const tables_1 = defaultArg(tables, []);
        const performers_1 = defaultArg(performers, []);
        const comments_1 = defaultArg(comments, []);
        this["identifier@13"] = identifier;
        this["MeasurementType@"] = measurementType;
        this["TechnologyType@"] = technologyType;
        this["TechnologyPlatform@"] = technologyPlatform;
        this["Tables@"] = tables_1;
        this["Performers@"] = performers_1;
        this["Comments@"] = comments_1;
    }
    get Identifier() {
        const this$ = this;
        return this$["identifier@13"];
    }
    set Identifier(i) {
        const this$ = this;
        this$["identifier@13"] = i;
    }
    static get FileName() {
        return "isa.assay.xlsx";
    }
    get MeasurementType() {
        const __ = this;
        return unwrap(__["MeasurementType@"]);
    }
    set MeasurementType(v) {
        const __ = this;
        __["MeasurementType@"] = v;
    }
    get TechnologyType() {
        const __ = this;
        return unwrap(__["TechnologyType@"]);
    }
    set TechnologyType(v) {
        const __ = this;
        __["TechnologyType@"] = v;
    }
    get TechnologyPlatform() {
        const __ = this;
        return unwrap(__["TechnologyPlatform@"]);
    }
    set TechnologyPlatform(v) {
        const __ = this;
        __["TechnologyPlatform@"] = v;
    }
    get Tables() {
        const __ = this;
        return __["Tables@"];
    }
    set Tables(v) {
        const __ = this;
        __["Tables@"] = v;
    }
    get Performers() {
        const __ = this;
        return __["Performers@"];
    }
    set Performers(v) {
        const __ = this;
        __["Performers@"] = v;
    }
    get Comments() {
        const __ = this;
        return __["Comments@"];
    }
    set Comments(v) {
        const __ = this;
        __["Comments@"] = v;
    }
    static init(identifier) {
        return new ArcAssay(identifier);
    }
    static create(identifier, measurementType, technologyType, technologyPlatform, tables, performers, comments) {
        return new ArcAssay(identifier, unwrap(measurementType), unwrap(technologyType), unwrap(technologyPlatform), unwrap(tables), unwrap(performers), unwrap(comments));
    }
    static make(identifier, measurementType, technologyType, technologyPlatform, tables, performers, comments) {
        return new ArcAssay(identifier, unwrap(measurementType), unwrap(technologyType), unwrap(technologyPlatform), tables, performers, comments);
    }
    get TableCount() {
        const this$ = this;
        return ArcTables__get_Count(ArcTables_$ctor_Z18C2F36D(this$.Tables)) | 0;
    }
    get TableNames() {
        const this$ = this;
        return ArcTables__get_TableNames(ArcTables_$ctor_Z18C2F36D(this$.Tables));
    }
    AddTable(table, index) {
        const this$ = this;
        ArcTables__AddTable_EC12B15(ArcTables_$ctor_Z18C2F36D(this$.Tables), table, unwrap(index));
    }
    static addTable(table, index) {
        return (assay) => {
            const c = assay.Copy();
            c.AddTable(table, unwrap(index));
            return c;
        };
    }
    AddTables(tables, index) {
        const this$ = this;
        ArcTables__AddTables_Z2D453886(ArcTables_$ctor_Z18C2F36D(this$.Tables), tables, unwrap(index));
    }
    static addTables(tables, index) {
        return (assay) => {
            const c = assay.Copy();
            c.AddTables(tables, unwrap(index));
            return c;
        };
    }
    InitTable(tableName, index) {
        const this$ = this;
        return ArcTables__InitTable_3B406CA4(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableName, unwrap(index));
    }
    static initTable(tableName, index) {
        return (assay) => {
            const c = assay.Copy();
            return c.InitTable(tableName, unwrap(index));
        };
    }
    InitTables(tableNames, index) {
        const this$ = this;
        ArcTables__InitTables_7B28792B(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableNames, unwrap(index));
    }
    static initTables(tableNames, index) {
        return (assay) => {
            const c = assay.Copy();
            c.InitTables(tableNames, unwrap(index));
            return c;
        };
    }
    GetTableAt(index) {
        const this$ = this;
        return ArcTables__GetTableAt_Z524259A4(ArcTables_$ctor_Z18C2F36D(this$.Tables), index);
    }
    static getTableAt(index) {
        return (assay) => {
            const newAssay = assay.Copy();
            return newAssay.GetTableAt(index);
        };
    }
    GetTable(name) {
        const this$ = this;
        return ArcTables__GetTable_Z721C83C5(ArcTables_$ctor_Z18C2F36D(this$.Tables), name);
    }
    static getTable(name) {
        return (assay) => {
            const newAssay = assay.Copy();
            return newAssay.GetTable(name);
        };
    }
    UpdateTableAt(index, table) {
        const this$ = this;
        ArcTables__UpdateTableAt_7E571736(ArcTables_$ctor_Z18C2F36D(this$.Tables), index, table);
    }
    static updateTableAt(index, table) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.UpdateTableAt(index, table);
            return newAssay;
        };
    }
    UpdateTable(name, table) {
        const this$ = this;
        ArcTables__UpdateTable_51766571(ArcTables_$ctor_Z18C2F36D(this$.Tables), name, table);
    }
    static updateTable(name, table) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.UpdateTable(name, table);
            return newAssay;
        };
    }
    RemoveTableAt(index) {
        const this$ = this;
        ArcTables__RemoveTableAt_Z524259A4(ArcTables_$ctor_Z18C2F36D(this$.Tables), index);
    }
    static removeTableAt(index) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.RemoveTableAt(index);
            return newAssay;
        };
    }
    RemoveTable(name) {
        const this$ = this;
        ArcTables__RemoveTable_Z721C83C5(ArcTables_$ctor_Z18C2F36D(this$.Tables), name);
    }
    static removeTable(name) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.RemoveTable(name);
            return newAssay;
        };
    }
    MapTableAt(index, updateFun) {
        const this$ = this;
        ArcTables__MapTableAt_8FC095C(ArcTables_$ctor_Z18C2F36D(this$.Tables), index, updateFun);
    }
    static mapTableAt(index, updateFun) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.MapTableAt(index, updateFun);
            return newAssay;
        };
    }
    MapTable(name, updateFun) {
        const this$ = this;
        ArcTables__MapTable_27DD7B1B(ArcTables_$ctor_Z18C2F36D(this$.Tables), name, updateFun);
    }
    static updateTable(name, updateFun) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.MapTable(name, updateFun);
            return newAssay;
        };
    }
    RenameTableAt(index, newName) {
        const this$ = this;
        ArcTables__RenameTableAt_Z176EF219(ArcTables_$ctor_Z18C2F36D(this$.Tables), index, newName);
    }
    static renameTableAt(index, newName) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.RenameTableAt(index, newName);
            return newAssay;
        };
    }
    RenameTable(name, newName) {
        const this$ = this;
        ArcTables__RenameTable_Z384F8060(ArcTables_$ctor_Z18C2F36D(this$.Tables), name, newName);
    }
    static renameTable(name, newName) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.RenameTable(name, newName);
            return newAssay;
        };
    }
    AddColumnAt(tableIndex, header, cells, columnIndex, forceReplace) {
        const this$ = this;
        ArcTables__AddColumnAt_6A9784DB(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableIndex, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
    }
    static addColumnAt(tableIndex, header, cells, columnIndex, forceReplace) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.AddColumnAt(tableIndex, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
            return newAssay;
        };
    }
    AddColumn(tableName, header, cells, columnIndex, forceReplace) {
        const this$ = this;
        ArcTables__AddColumn_1FF50D3C(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableName, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
    }
    static addColumn(tableName, header, cells, columnIndex, forceReplace) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.AddColumn(tableName, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
            return newAssay;
        };
    }
    RemoveColumnAt(tableIndex, columnIndex) {
        const this$ = this;
        ArcTables__RemoveColumnAt_Z37302880(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableIndex, columnIndex);
    }
    static removeColumnAt(tableIndex, columnIndex) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.RemoveColumnAt(tableIndex, columnIndex);
            return newAssay;
        };
    }
    RemoveColumn(tableName, columnIndex) {
        const this$ = this;
        ArcTables__RemoveColumn_Z18115A39(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableName, columnIndex);
    }
    static removeColumn(tableName, columnIndex) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.RemoveColumn(tableName, columnIndex);
            return newAssay;
        };
    }
    UpdateColumnAt(tableIndex, columnIndex, header, cells) {
        const this$ = this;
        ArcTables__UpdateColumnAt_21300791(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableIndex, columnIndex, header, unwrap(cells));
    }
    static updateColumnAt(tableIndex, columnIndex, header, cells) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.UpdateColumnAt(tableIndex, columnIndex, header, unwrap(cells));
            return newAssay;
        };
    }
    UpdateColumn(tableName, columnIndex, header, cells) {
        const this$ = this;
        ArcTables__UpdateColumn_Z6083042A(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableName, columnIndex, header, unwrap(cells));
    }
    static updateColumn(tableName, columnIndex, header, cells) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.UpdateColumn(tableName, columnIndex, header, unwrap(cells));
            return newAssay;
        };
    }
    GetColumnAt(tableIndex, columnIndex) {
        const this$ = this;
        return ArcTables__GetColumnAt_Z37302880(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableIndex, columnIndex);
    }
    static getColumnAt(tableIndex, columnIndex) {
        return (assay) => {
            const newAssay = assay.Copy();
            return newAssay.GetColumnAt(tableIndex, columnIndex);
        };
    }
    GetColumn(tableName, columnIndex) {
        const this$ = this;
        return ArcTables__GetColumn_Z18115A39(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableName, columnIndex);
    }
    static getColumn(tableName, columnIndex) {
        return (assay) => {
            const newAssay = assay.Copy();
            return newAssay.GetColumn(tableName, columnIndex);
        };
    }
    AddRowAt(tableIndex, cells, rowIndex) {
        const this$ = this;
        ArcTables__AddRowAt_Z12CDB784(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableIndex, unwrap(cells), unwrap(rowIndex));
    }
    static addRowAt(tableIndex, cells, rowIndex) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.AddRowAt(tableIndex, unwrap(cells), unwrap(rowIndex));
            return newAssay;
        };
    }
    AddRow(tableName, cells, rowIndex) {
        const this$ = this;
        ArcTables__AddRow_Z16315DE5(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableName, unwrap(cells), unwrap(rowIndex));
    }
    static addRow(tableName, cells, rowIndex) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.AddRow(tableName, unwrap(cells), unwrap(rowIndex));
            return newAssay;
        };
    }
    RemoveRowAt(tableIndex, rowIndex) {
        const this$ = this;
        ArcTables__RemoveRowAt_Z37302880(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableIndex, rowIndex);
    }
    static removeRowAt(tableIndex, rowIndex) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.RemoveColumnAt(tableIndex, rowIndex);
            return newAssay;
        };
    }
    RemoveRow(tableName, rowIndex) {
        const this$ = this;
        ArcTables__RemoveRow_Z18115A39(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableName, rowIndex);
    }
    static removeRow(tableName, rowIndex) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.RemoveRow(tableName, rowIndex);
            return newAssay;
        };
    }
    UpdateRowAt(tableIndex, rowIndex, cells) {
        const this$ = this;
        ArcTables__UpdateRowAt_1CF7B5DC(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableIndex, rowIndex, cells);
    }
    static updateRowAt(tableIndex, rowIndex, cells) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.UpdateRowAt(tableIndex, rowIndex, cells);
            return newAssay;
        };
    }
    UpdateRow(tableName, rowIndex, cells) {
        const this$ = this;
        ArcTables__UpdateRow_1BFE2CFB(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableName, rowIndex, cells);
    }
    static updateRow(tableName, rowIndex, cells) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.UpdateRow(tableName, rowIndex, cells);
            return newAssay;
        };
    }
    GetRowAt(tableIndex, rowIndex) {
        const this$ = this;
        return ArcTables__GetRowAt_Z37302880(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableIndex, rowIndex);
    }
    static getRowAt(tableIndex, rowIndex) {
        return (assay) => {
            const newAssay = assay.Copy();
            return newAssay.GetRowAt(tableIndex, rowIndex);
        };
    }
    GetRow(tableName, rowIndex) {
        const this$ = this;
        return ArcTables__GetRow_Z18115A39(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableName, rowIndex);
    }
    static getRow(tableName, rowIndex) {
        return (assay) => {
            const newAssay = assay.Copy();
            return newAssay.GetRow(tableName, rowIndex);
        };
    }
    static setPerformers(performers, assay) {
        assay.Performers = performers;
        return assay;
    }
    Copy() {
        const this$ = this;
        const newTables = [];
        let enumerator = getEnumerator(this$.Tables);
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const table = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                const copy = table.Copy();
                void (newTables.push(copy));
            }
        }
        finally {
            disposeSafe(enumerator);
        }
        return new ArcAssay(this$.Identifier, unwrap(this$.MeasurementType), unwrap(this$.TechnologyType), unwrap(this$.TechnologyPlatform), newTables, this$.Performers, this$.Comments);
    }
    ToAssay() {
        const this$ = this;
        const processSeq = ArcTables__GetProcesses(ArcTables_$ctor_Z18C2F36D(this$.Tables));
        let assayMaterials;
        const v_2 = AssayMaterials_create_Z253F0553(unwrap(fromValueWithDefault(empty(), getSamples(processSeq))), unwrap(fromValueWithDefault(empty(), getMaterials(processSeq))));
        assayMaterials = fromValueWithDefault(AssayMaterials_get_empty(), v_2);
        return Assay_create_3D372A24(void 0, unwrap(isMissingIdentifier(this$.Identifier) ? void 0 : Assay_fileNameFromIdentifier(this$.Identifier)), unwrap(this$.MeasurementType), unwrap(this$.TechnologyType), unwrap(this$.TechnologyPlatform), unwrap(fromValueWithDefault(empty(), getData(processSeq))), unwrap(assayMaterials), unwrap(fromValueWithDefault(empty(), getCharacteristics(processSeq))), unwrap(fromValueWithDefault(empty(), getUnits(processSeq))), unwrap(fromValueWithDefault(empty(), processSeq)), unwrap(fromValueWithDefault(empty(), ofArray(this$.Comments))));
    }
    static fromAssay(a) {
        const tables = map((arg_1) => ArcTables__get_Tables(ArcTables_fromProcesses_134EBDED(arg_1)), a.ProcessSequence);
        let identifer;
        const matchValue = a.FileName;
        identifer = ((matchValue == null) ? createMissingIdentifier() : Assay_identifierFromFileName(matchValue));
        return ArcAssay.create(identifer, unwrap(a.MeasurementType), unwrap(a.TechnologyType), unwrap(a.TechnologyPlatform), unwrap(tables), void 0, unwrap(map(toArray, a.Comments)));
    }
}

export function ArcAssay_$reflection() {
    return class_type("ARCtrl.ISA.ArcAssay", void 0, ArcAssay);
}

export function ArcAssay_$ctor_Z2A43082B(identifier, measurementType, technologyType, technologyPlatform, tables, performers, comments) {
    return new ArcAssay(identifier, measurementType, technologyType, technologyPlatform, tables, performers, comments);
}

