import { defaultArg, value, map as map_1, Option, unwrap } from "../../../fable_modules/fable-library-ts/Option.js";
import { OntologyAnnotation } from "../JsonTypes/OntologyAnnotation.js";
import { ArcTable } from "./ArcTable.js";
import { Person } from "../JsonTypes/Person.js";
import { Comment$ } from "../JsonTypes/Comment.js";
import { ArcTables_fromProcesses_134EBDED, ArcTables__get_Tables, ArcTables__GetProcesses, ArcTables__GetRow_Z18115A39, ArcTables__GetRowAt_Z37302880, ArcTables__UpdateRow_1BFE2CFB, ArcTables__UpdateRowAt_1CF7B5DC, ArcTables__RemoveRow_Z18115A39, ArcTables__RemoveRowAt_Z37302880, ArcTables__AddRow_Z16315DE5, ArcTables__AddRowAt_Z12CDB784, ArcTables__GetColumn_Z18115A39, ArcTables__GetColumnAt_Z37302880, ArcTables__UpdateColumn_Z6083042A, ArcTables__UpdateColumnAt_21300791, ArcTables__RemoveColumn_Z18115A39, ArcTables__RemoveColumnAt_Z37302880, ArcTables__AddColumn_1FF50D3C, ArcTables__AddColumnAt_6A9784DB, ArcTables__RenameTable_Z384F8060, ArcTables__RenameTableAt_Z176EF219, ArcTables__MapTable_27DD7B1B, ArcTables__MapTableAt_8FC095C, ArcTables__RemoveTable_Z721C83C5, ArcTables__RemoveTableAt_Z524259A4, ArcTables__UpdateTable_51766571, ArcTables__UpdateTableAt_7E571736, ArcTables__GetTable_Z721C83C5, ArcTables__GetTableAt_Z524259A4, ArcTables__InitTables_7B28792B, ArcTables__InitTable_3B406CA4, ArcTables__AddTables_Z2D453886, ArcTables__AddTable_EC12B15, ArcTables__get_TableNames, ArcTables_$ctor_Z18C2F36D, ArcTables__get_Count } from "./ArcTables.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { toArray, ofArray, empty, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { CompositeHeader_$union } from "./CompositeHeader.js";
import { CompositeCell_$union } from "./CompositeCell.js";
import { CompositeColumn } from "./CompositeColumn.js";
import { disposeSafe, getEnumerator } from "../../../fable_modules/fable-library-ts/Util.js";
import { map } from "../../../fable_modules/fable-library-ts/Array.js";
import { Process } from "../JsonTypes/Process.js";
import { AssayMaterials_get_empty, AssayMaterials_create_Z253F0553, AssayMaterials } from "../JsonTypes/AssayMaterials.js";
import { fromValueWithDefault } from "../OptionExtensions.js";
import { Sample } from "../JsonTypes/Sample.js";
import { getUnits, getCharacteristics, getData, getMaterials, getSamples } from "../JsonTypes/ProcessSequence.js";
import { Material } from "../JsonTypes/Material.js";
import { Assay, Assay_create_3D372A24 } from "../JsonTypes/Assay.js";
import { Assay_identifierFromFileName, createMissingIdentifier, Assay_fileNameFromIdentifier, isMissingIdentifier } from "./Identifier.js";
import { Data } from "../JsonTypes/Data.js";
import { MaterialAttribute } from "../JsonTypes/MaterialAttribute.js";
import { class_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";

export class ArcAssay {
    "identifier@13": string;
    "MeasurementType@": Option<OntologyAnnotation>;
    "TechnologyType@": Option<OntologyAnnotation>;
    "TechnologyPlatform@": Option<string>;
    "Tables@": ArcTable[];
    "Performers@": Person[];
    "Comments@": Comment$[];
    constructor(identifier: string, measurementType?: OntologyAnnotation, technologyType?: OntologyAnnotation, technologyPlatform?: string, tables?: ArcTable[], performers?: Person[], comments?: Comment$[]) {
        const tables_1: ArcTable[] = defaultArg<ArcTable[]>(tables, []);
        const performers_1: Person[] = defaultArg<Person[]>(performers, []);
        const comments_1: Comment$[] = defaultArg<Comment$[]>(comments, []);
        this["identifier@13"] = identifier;
        this["MeasurementType@"] = measurementType;
        this["TechnologyType@"] = technologyType;
        this["TechnologyPlatform@"] = technologyPlatform;
        this["Tables@"] = tables_1;
        this["Performers@"] = performers_1;
        this["Comments@"] = comments_1;
    }
    get Identifier(): string {
        const this$: ArcAssay = this;
        return this$["identifier@13"];
    }
    set Identifier(i: string) {
        const this$: ArcAssay = this;
        this$["identifier@13"] = i;
    }
    static get FileName(): string {
        return "isa.assay.xlsx";
    }
    get MeasurementType(): OntologyAnnotation | undefined {
        const __: ArcAssay = this;
        return unwrap(__["MeasurementType@"]);
    }
    set MeasurementType(v: Option<OntologyAnnotation>) {
        const __: ArcAssay = this;
        __["MeasurementType@"] = v;
    }
    get TechnologyType(): OntologyAnnotation | undefined {
        const __: ArcAssay = this;
        return unwrap(__["TechnologyType@"]);
    }
    set TechnologyType(v: Option<OntologyAnnotation>) {
        const __: ArcAssay = this;
        __["TechnologyType@"] = v;
    }
    get TechnologyPlatform(): string | undefined {
        const __: ArcAssay = this;
        return unwrap(__["TechnologyPlatform@"]);
    }
    set TechnologyPlatform(v: Option<string>) {
        const __: ArcAssay = this;
        __["TechnologyPlatform@"] = v;
    }
    get Tables(): ArcTable[] {
        const __: ArcAssay = this;
        return __["Tables@"];
    }
    set Tables(v: ArcTable[]) {
        const __: ArcAssay = this;
        __["Tables@"] = v;
    }
    get Performers(): Person[] {
        const __: ArcAssay = this;
        return __["Performers@"];
    }
    set Performers(v: Person[]) {
        const __: ArcAssay = this;
        __["Performers@"] = v;
    }
    get Comments(): Comment$[] {
        const __: ArcAssay = this;
        return __["Comments@"];
    }
    set Comments(v: Comment$[]) {
        const __: ArcAssay = this;
        __["Comments@"] = v;
    }
    static init(identifier: string): ArcAssay {
        return new ArcAssay(identifier);
    }
    static create(identifier: string, measurementType?: OntologyAnnotation, technologyType?: OntologyAnnotation, technologyPlatform?: string, tables?: ArcTable[], performers?: Person[], comments?: Comment$[]): ArcAssay {
        return new ArcAssay(identifier, unwrap(measurementType), unwrap(technologyType), unwrap(technologyPlatform), unwrap(tables), unwrap(performers), unwrap(comments));
    }
    static make(identifier: string, measurementType: Option<OntologyAnnotation>, technologyType: Option<OntologyAnnotation>, technologyPlatform: Option<string>, tables: ArcTable[], performers: Person[], comments: Comment$[]): ArcAssay {
        return new ArcAssay(identifier, unwrap(measurementType), unwrap(technologyType), unwrap(technologyPlatform), tables, performers, comments);
    }
    get TableCount(): int32 {
        const this$: ArcAssay = this;
        return ArcTables__get_Count(ArcTables_$ctor_Z18C2F36D(this$.Tables)) | 0;
    }
    get TableNames(): FSharpList<string> {
        const this$: ArcAssay = this;
        return ArcTables__get_TableNames(ArcTables_$ctor_Z18C2F36D(this$.Tables));
    }
    AddTable(table: ArcTable, index?: int32): void {
        const this$: ArcAssay = this;
        ArcTables__AddTable_EC12B15(ArcTables_$ctor_Z18C2F36D(this$.Tables), table, unwrap(index));
    }
    static addTable(table: ArcTable, index?: int32): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const c: ArcAssay = assay.Copy();
            c.AddTable(table, unwrap(index));
            return c;
        };
    }
    AddTables(tables: Iterable<ArcTable>, index?: int32): void {
        const this$: ArcAssay = this;
        ArcTables__AddTables_Z2D453886(ArcTables_$ctor_Z18C2F36D(this$.Tables), tables, unwrap(index));
    }
    static addTables(tables: Iterable<ArcTable>, index?: int32): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const c: ArcAssay = assay.Copy();
            c.AddTables(tables, unwrap(index));
            return c;
        };
    }
    InitTable(tableName: string, index?: int32): ArcTable {
        const this$: ArcAssay = this;
        return ArcTables__InitTable_3B406CA4(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableName, unwrap(index));
    }
    static initTable(tableName: string, index?: int32): ((arg0: ArcAssay) => ArcTable) {
        return (assay: ArcAssay): ArcTable => {
            const c: ArcAssay = assay.Copy();
            return c.InitTable(tableName, unwrap(index));
        };
    }
    InitTables(tableNames: Iterable<string>, index?: int32): void {
        const this$: ArcAssay = this;
        ArcTables__InitTables_7B28792B(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableNames, unwrap(index));
    }
    static initTables(tableNames: Iterable<string>, index?: int32): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const c: ArcAssay = assay.Copy();
            c.InitTables(tableNames, unwrap(index));
            return c;
        };
    }
    GetTableAt(index: int32): ArcTable {
        const this$: ArcAssay = this;
        return ArcTables__GetTableAt_Z524259A4(ArcTables_$ctor_Z18C2F36D(this$.Tables), index);
    }
    static getTableAt(index: int32): ((arg0: ArcAssay) => ArcTable) {
        return (assay: ArcAssay): ArcTable => {
            const newAssay: ArcAssay = assay.Copy();
            return newAssay.GetTableAt(index);
        };
    }
    GetTable(name: string): ArcTable {
        const this$: ArcAssay = this;
        return ArcTables__GetTable_Z721C83C5(ArcTables_$ctor_Z18C2F36D(this$.Tables), name);
    }
    static getTable(name: string): ((arg0: ArcAssay) => ArcTable) {
        return (assay: ArcAssay): ArcTable => {
            const newAssay: ArcAssay = assay.Copy();
            return newAssay.GetTable(name);
        };
    }
    UpdateTableAt(index: int32, table: ArcTable): void {
        const this$: ArcAssay = this;
        ArcTables__UpdateTableAt_7E571736(ArcTables_$ctor_Z18C2F36D(this$.Tables), index, table);
    }
    static updateTableAt(index: int32, table: ArcTable): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const newAssay: ArcAssay = assay.Copy();
            newAssay.UpdateTableAt(index, table);
            return newAssay;
        };
    }
    UpdateTable(name: string, table: ArcTable): void {
        const this$: ArcAssay = this;
        ArcTables__UpdateTable_51766571(ArcTables_$ctor_Z18C2F36D(this$.Tables), name, table);
    }
    static updateTable(name: string, table: ArcTable): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const newAssay: ArcAssay = assay.Copy();
            newAssay.UpdateTable(name, table);
            return newAssay;
        };
    }
    RemoveTableAt(index: int32): void {
        const this$: ArcAssay = this;
        ArcTables__RemoveTableAt_Z524259A4(ArcTables_$ctor_Z18C2F36D(this$.Tables), index);
    }
    static removeTableAt(index: int32): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const newAssay: ArcAssay = assay.Copy();
            newAssay.RemoveTableAt(index);
            return newAssay;
        };
    }
    RemoveTable(name: string): void {
        const this$: ArcAssay = this;
        ArcTables__RemoveTable_Z721C83C5(ArcTables_$ctor_Z18C2F36D(this$.Tables), name);
    }
    static removeTable(name: string): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const newAssay: ArcAssay = assay.Copy();
            newAssay.RemoveTable(name);
            return newAssay;
        };
    }
    MapTableAt(index: int32, updateFun: ((arg0: ArcTable) => void)): void {
        const this$: ArcAssay = this;
        ArcTables__MapTableAt_8FC095C(ArcTables_$ctor_Z18C2F36D(this$.Tables), index, updateFun);
    }
    static mapTableAt(index: int32, updateFun: ((arg0: ArcTable) => void)): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const newAssay: ArcAssay = assay.Copy();
            newAssay.MapTableAt(index, updateFun);
            return newAssay;
        };
    }
    MapTable(name: string, updateFun: ((arg0: ArcTable) => void)): void {
        const this$: ArcAssay = this;
        ArcTables__MapTable_27DD7B1B(ArcTables_$ctor_Z18C2F36D(this$.Tables), name, updateFun);
    }
    static updateTable(name: string, updateFun: ((arg0: ArcTable) => void)): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const newAssay: ArcAssay = assay.Copy();
            newAssay.MapTable(name, updateFun);
            return newAssay;
        };
    }
    RenameTableAt(index: int32, newName: string): void {
        const this$: ArcAssay = this;
        ArcTables__RenameTableAt_Z176EF219(ArcTables_$ctor_Z18C2F36D(this$.Tables), index, newName);
    }
    static renameTableAt(index: int32, newName: string): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const newAssay: ArcAssay = assay.Copy();
            newAssay.RenameTableAt(index, newName);
            return newAssay;
        };
    }
    RenameTable(name: string, newName: string): void {
        const this$: ArcAssay = this;
        ArcTables__RenameTable_Z384F8060(ArcTables_$ctor_Z18C2F36D(this$.Tables), name, newName);
    }
    static renameTable(name: string, newName: string): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const newAssay: ArcAssay = assay.Copy();
            newAssay.RenameTable(name, newName);
            return newAssay;
        };
    }
    AddColumnAt(tableIndex: int32, header: CompositeHeader_$union, cells?: CompositeCell_$union[], columnIndex?: int32, forceReplace?: boolean): void {
        const this$: ArcAssay = this;
        ArcTables__AddColumnAt_6A9784DB(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableIndex, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
    }
    static addColumnAt(tableIndex: int32, header: CompositeHeader_$union, cells?: CompositeCell_$union[], columnIndex?: int32, forceReplace?: boolean): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const newAssay: ArcAssay = assay.Copy();
            newAssay.AddColumnAt(tableIndex, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
            return newAssay;
        };
    }
    AddColumn(tableName: string, header: CompositeHeader_$union, cells?: CompositeCell_$union[], columnIndex?: int32, forceReplace?: boolean): void {
        const this$: ArcAssay = this;
        ArcTables__AddColumn_1FF50D3C(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableName, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
    }
    static addColumn(tableName: string, header: CompositeHeader_$union, cells?: CompositeCell_$union[], columnIndex?: int32, forceReplace?: boolean): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const newAssay: ArcAssay = assay.Copy();
            newAssay.AddColumn(tableName, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
            return newAssay;
        };
    }
    RemoveColumnAt(tableIndex: int32, columnIndex: int32): void {
        const this$: ArcAssay = this;
        ArcTables__RemoveColumnAt_Z37302880(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableIndex, columnIndex);
    }
    static removeColumnAt(tableIndex: int32, columnIndex: int32): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const newAssay: ArcAssay = assay.Copy();
            newAssay.RemoveColumnAt(tableIndex, columnIndex);
            return newAssay;
        };
    }
    RemoveColumn(tableName: string, columnIndex: int32): void {
        const this$: ArcAssay = this;
        ArcTables__RemoveColumn_Z18115A39(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableName, columnIndex);
    }
    static removeColumn(tableName: string, columnIndex: int32): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const newAssay: ArcAssay = assay.Copy();
            newAssay.RemoveColumn(tableName, columnIndex);
            return newAssay;
        };
    }
    UpdateColumnAt(tableIndex: int32, columnIndex: int32, header: CompositeHeader_$union, cells?: CompositeCell_$union[]): void {
        const this$: ArcAssay = this;
        ArcTables__UpdateColumnAt_21300791(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableIndex, columnIndex, header, unwrap(cells));
    }
    static updateColumnAt(tableIndex: int32, columnIndex: int32, header: CompositeHeader_$union, cells?: CompositeCell_$union[]): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const newAssay: ArcAssay = assay.Copy();
            newAssay.UpdateColumnAt(tableIndex, columnIndex, header, unwrap(cells));
            return newAssay;
        };
    }
    UpdateColumn(tableName: string, columnIndex: int32, header: CompositeHeader_$union, cells?: CompositeCell_$union[]): void {
        const this$: ArcAssay = this;
        ArcTables__UpdateColumn_Z6083042A(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableName, columnIndex, header, unwrap(cells));
    }
    static updateColumn(tableName: string, columnIndex: int32, header: CompositeHeader_$union, cells?: CompositeCell_$union[]): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const newAssay: ArcAssay = assay.Copy();
            newAssay.UpdateColumn(tableName, columnIndex, header, unwrap(cells));
            return newAssay;
        };
    }
    GetColumnAt(tableIndex: int32, columnIndex: int32): CompositeColumn {
        const this$: ArcAssay = this;
        return ArcTables__GetColumnAt_Z37302880(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableIndex, columnIndex);
    }
    static getColumnAt(tableIndex: int32, columnIndex: int32): ((arg0: ArcAssay) => CompositeColumn) {
        return (assay: ArcAssay): CompositeColumn => {
            const newAssay: ArcAssay = assay.Copy();
            return newAssay.GetColumnAt(tableIndex, columnIndex);
        };
    }
    GetColumn(tableName: string, columnIndex: int32): CompositeColumn {
        const this$: ArcAssay = this;
        return ArcTables__GetColumn_Z18115A39(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableName, columnIndex);
    }
    static getColumn(tableName: string, columnIndex: int32): ((arg0: ArcAssay) => CompositeColumn) {
        return (assay: ArcAssay): CompositeColumn => {
            const newAssay: ArcAssay = assay.Copy();
            return newAssay.GetColumn(tableName, columnIndex);
        };
    }
    AddRowAt(tableIndex: int32, cells?: CompositeCell_$union[], rowIndex?: int32): void {
        const this$: ArcAssay = this;
        ArcTables__AddRowAt_Z12CDB784(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableIndex, unwrap(cells), unwrap(rowIndex));
    }
    static addRowAt(tableIndex: int32, cells?: CompositeCell_$union[], rowIndex?: int32): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const newAssay: ArcAssay = assay.Copy();
            newAssay.AddRowAt(tableIndex, unwrap(cells), unwrap(rowIndex));
            return newAssay;
        };
    }
    AddRow(tableName: string, cells?: CompositeCell_$union[], rowIndex?: int32): void {
        const this$: ArcAssay = this;
        ArcTables__AddRow_Z16315DE5(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableName, unwrap(cells), unwrap(rowIndex));
    }
    static addRow(tableName: string, cells?: CompositeCell_$union[], rowIndex?: int32): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const newAssay: ArcAssay = assay.Copy();
            newAssay.AddRow(tableName, unwrap(cells), unwrap(rowIndex));
            return newAssay;
        };
    }
    RemoveRowAt(tableIndex: int32, rowIndex: int32): void {
        const this$: ArcAssay = this;
        ArcTables__RemoveRowAt_Z37302880(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableIndex, rowIndex);
    }
    static removeRowAt(tableIndex: int32, rowIndex: int32): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const newAssay: ArcAssay = assay.Copy();
            newAssay.RemoveColumnAt(tableIndex, rowIndex);
            return newAssay;
        };
    }
    RemoveRow(tableName: string, rowIndex: int32): void {
        const this$: ArcAssay = this;
        ArcTables__RemoveRow_Z18115A39(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableName, rowIndex);
    }
    static removeRow(tableName: string, rowIndex: int32): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const newAssay: ArcAssay = assay.Copy();
            newAssay.RemoveRow(tableName, rowIndex);
            return newAssay;
        };
    }
    UpdateRowAt(tableIndex: int32, rowIndex: int32, cells: CompositeCell_$union[]): void {
        const this$: ArcAssay = this;
        ArcTables__UpdateRowAt_1CF7B5DC(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableIndex, rowIndex, cells);
    }
    static updateRowAt(tableIndex: int32, rowIndex: int32, cells: CompositeCell_$union[]): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const newAssay: ArcAssay = assay.Copy();
            newAssay.UpdateRowAt(tableIndex, rowIndex, cells);
            return newAssay;
        };
    }
    UpdateRow(tableName: string, rowIndex: int32, cells: CompositeCell_$union[]): void {
        const this$: ArcAssay = this;
        ArcTables__UpdateRow_1BFE2CFB(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableName, rowIndex, cells);
    }
    static updateRow(tableName: string, rowIndex: int32, cells: CompositeCell_$union[]): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const newAssay: ArcAssay = assay.Copy();
            newAssay.UpdateRow(tableName, rowIndex, cells);
            return newAssay;
        };
    }
    GetRowAt(tableIndex: int32, rowIndex: int32): CompositeCell_$union[] {
        const this$: ArcAssay = this;
        return ArcTables__GetRowAt_Z37302880(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableIndex, rowIndex);
    }
    static getRowAt(tableIndex: int32, rowIndex: int32): ((arg0: ArcAssay) => CompositeCell_$union[]) {
        return (assay: ArcAssay): CompositeCell_$union[] => {
            const newAssay: ArcAssay = assay.Copy();
            return newAssay.GetRowAt(tableIndex, rowIndex);
        };
    }
    GetRow(tableName: string, rowIndex: int32): CompositeCell_$union[] {
        const this$: ArcAssay = this;
        return ArcTables__GetRow_Z18115A39(ArcTables_$ctor_Z18C2F36D(this$.Tables), tableName, rowIndex);
    }
    static getRow(tableName: string, rowIndex: int32): ((arg0: ArcAssay) => CompositeCell_$union[]) {
        return (assay: ArcAssay): CompositeCell_$union[] => {
            const newAssay: ArcAssay = assay.Copy();
            return newAssay.GetRow(tableName, rowIndex);
        };
    }
    static setPerformers(performers: Person[], assay: ArcAssay): ArcAssay {
        assay.Performers = performers;
        return assay;
    }
    Copy(): ArcAssay {
        const this$: ArcAssay = this;
        const nextTables: ArcTable[] = [];
        let enumerator: any = getEnumerator(this$.Tables);
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const table: ArcTable = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                const copy: ArcTable = table.Copy();
                void (nextTables.push(copy));
            }
        }
        finally {
            disposeSafe(enumerator);
        }
        const nextComments: Comment$[] = map<Comment$, Comment$>((c: Comment$): Comment$ => c.Copy(), this$.Comments);
        const nextPerformers: Person[] = map<Person, Person>((c_1: Person): Person => c_1.Copy(), this$.Performers);
        return new ArcAssay(this$.Identifier, unwrap(this$.MeasurementType), unwrap(this$.TechnologyType), unwrap(this$.TechnologyPlatform), nextTables, nextPerformers, nextComments);
    }
    ToAssay(): Assay {
        const this$: ArcAssay = this;
        const processSeq: FSharpList<Process> = ArcTables__GetProcesses(ArcTables_$ctor_Z18C2F36D(this$.Tables));
        let assayMaterials: Option<AssayMaterials>;
        const v_2: AssayMaterials = AssayMaterials_create_Z253F0553(unwrap(fromValueWithDefault<FSharpList<Sample>>(empty<Sample>(), getSamples(processSeq))), unwrap(fromValueWithDefault<FSharpList<Material>>(empty<Material>(), getMaterials(processSeq))));
        assayMaterials = fromValueWithDefault<AssayMaterials>(AssayMaterials_get_empty(), v_2);
        return Assay_create_3D372A24(void 0, unwrap(isMissingIdentifier(this$.Identifier) ? void 0 : Assay_fileNameFromIdentifier(this$.Identifier)), unwrap(this$.MeasurementType), unwrap(this$.TechnologyType), unwrap(this$.TechnologyPlatform), unwrap(fromValueWithDefault<FSharpList<Data>>(empty<Data>(), getData(processSeq))), unwrap(assayMaterials), unwrap(fromValueWithDefault<FSharpList<MaterialAttribute>>(empty<MaterialAttribute>(), getCharacteristics(processSeq))), unwrap(fromValueWithDefault<FSharpList<OntologyAnnotation>>(empty<OntologyAnnotation>(), getUnits(processSeq))), unwrap(fromValueWithDefault<FSharpList<Process>>(empty<Process>(), processSeq)), unwrap(fromValueWithDefault<FSharpList<Comment$>>(empty<Comment$>(), ofArray<Comment$>(this$.Comments))));
    }
    static fromAssay(a: Assay): ArcAssay {
        const tables: Option<ArcTable[]> = map_1<FSharpList<Process>, ArcTable[]>((arg_1: FSharpList<Process>): ArcTable[] => ArcTables__get_Tables(ArcTables_fromProcesses_134EBDED(arg_1)), a.ProcessSequence);
        let identifer: string;
        const matchValue: Option<string> = a.FileName;
        identifer = ((matchValue == null) ? createMissingIdentifier() : Assay_identifierFromFileName(value(matchValue)));
        return ArcAssay.create(identifer, unwrap(map_1<OntologyAnnotation, OntologyAnnotation>((x: OntologyAnnotation): OntologyAnnotation => x.Copy(), a.MeasurementType)), unwrap(map_1<OntologyAnnotation, OntologyAnnotation>((x_1: OntologyAnnotation): OntologyAnnotation => x_1.Copy(), a.TechnologyType)), unwrap(a.TechnologyPlatform), unwrap(tables), void 0, unwrap(map_1<FSharpList<Comment$>, Comment$[]>(toArray, a.Comments)));
    }
}

export function ArcAssay_$reflection(): TypeInfo {
    return class_type("ARCtrl.ISA.ArcAssay", void 0, ArcAssay);
}

export function ArcAssay_$ctor_Z2A43082B(identifier: string, measurementType?: OntologyAnnotation, technologyType?: OntologyAnnotation, technologyPlatform?: string, tables?: ArcTable[], performers?: Person[], comments?: Comment$[]): ArcAssay {
    return new ArcAssay(identifier, measurementType, technologyType, technologyPlatform, tables, performers, comments);
}

