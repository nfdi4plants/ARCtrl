import { defaultArg, value, map, Option, unwrap } from "../../../fable_modules/fable-library-ts/Option.js";
import { OntologyAnnotation } from "../JsonTypes/OntologyAnnotation.js";
import { ArcTable } from "./ArcTable.js";
import { empty, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { Person } from "../JsonTypes/Person.js";
import { Comment$ } from "../JsonTypes/Comment.js";
import { ArcTables_fromProcesses_Z31821267, ArcTables__get_Tables, ArcTables__GetProcesses, ArcTables__GetRow_Z18115A39, ArcTables__GetRowAt_Z37302880, ArcTables__UpdateRow_Z5E65B4B1, ArcTables__UpdateRowAt_Z596C2D98, ArcTables__RemoveRow_Z18115A39, ArcTables__RemoveRowAt_Z37302880, ArcTables__AddRow_1177C4AF, ArcTables__AddRowAt_Z57F91678, ArcTables__GetColumn_Z18115A39, ArcTables__GetColumnAt_Z37302880, ArcTables__UpdateColumn_Z774BF72A, ArcTables__UpdateColumnAt_Z155350AF, ArcTables__RemoveColumn_Z18115A39, ArcTables__RemoveColumnAt_Z37302880, ArcTables__AddColumn_Z4FC90944, ArcTables__AddColumnAt_6647579B, ArcTables__RenameTable_Z384F8060, ArcTables__RenameTableAt_Z176EF219, ArcTables__MapTable_4E415F2F, ArcTables__MapTableAt_61602D68, ArcTables__RemoveTable_Z721C83C5, ArcTables__RemoveTableAt_Z524259A4, ArcTables__UpdateTable_4976F045, ArcTables__UpdateTableAt_66578202, ArcTables__GetTable_Z721C83C5, ArcTables__GetTableAt_Z524259A4, ArcTables__InitTables_7B28792B, ArcTables__InitTable_3B406CA4, ArcTables__AddTables_3601F24E, ArcTables__AddTable_16F700A1, ArcTables__get_TableNames, ArcTables_$ctor_Z68BECB99, ArcTables__get_Count } from "./ArcTables.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { CompositeHeader_$union } from "./CompositeHeader.js";
import { CompositeCell_$union } from "./CompositeCell.js";
import { CompositeColumn } from "./CompositeColumn.js";
import { disposeSafe, getEnumerator } from "../../../fable_modules/fable-library-ts/Util.js";
import { Process } from "../JsonTypes/Process.js";
import { AssayMaterials_get_empty, AssayMaterials_create_1CB3546D, AssayMaterials } from "../JsonTypes/AssayMaterials.js";
import { fromValueWithDefault } from "../OptionExtensions.js";
import { Sample } from "../JsonTypes/Sample.js";
import { getUnits, getCharacteristics, getData, getMaterials, getSamples } from "../JsonTypes/ProcessSequence.js";
import { Material } from "../JsonTypes/Material.js";
import { Assay, Assay_create_ABF59A4 } from "../JsonTypes/Assay.js";
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
    "Performers@": FSharpList<Person>;
    "Comments@": FSharpList<Comment$>;
    constructor(identifier: string, measurementType?: OntologyAnnotation, technologyType?: OntologyAnnotation, technologyPlatform?: string, tables?: ArcTable[], performers?: FSharpList<Person>, comments?: FSharpList<Comment$>) {
        const tables_1: ArcTable[] = defaultArg<ArcTable[]>(tables, []);
        const performers_1: FSharpList<Person> = defaultArg<FSharpList<Person>>(performers, empty<Person>());
        const comments_1: FSharpList<Comment$> = defaultArg<FSharpList<Comment$>>(comments, empty<Comment$>());
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
    get Performers(): FSharpList<Person> {
        const __: ArcAssay = this;
        return __["Performers@"];
    }
    set Performers(v: FSharpList<Person>) {
        const __: ArcAssay = this;
        __["Performers@"] = v;
    }
    get Comments(): FSharpList<Comment$> {
        const __: ArcAssay = this;
        return __["Comments@"];
    }
    set Comments(v: FSharpList<Comment$>) {
        const __: ArcAssay = this;
        __["Comments@"] = v;
    }
    static init(identifier: string): ArcAssay {
        return new ArcAssay(identifier);
    }
    static create(identifier: string, measurementType?: OntologyAnnotation, technologyType?: OntologyAnnotation, technologyPlatform?: string, tables?: ArcTable[], performers?: FSharpList<Person>, comments?: FSharpList<Comment$>): ArcAssay {
        return new ArcAssay(identifier, unwrap(measurementType), unwrap(technologyType), unwrap(technologyPlatform), unwrap(tables), unwrap(performers), unwrap(comments));
    }
    static make(identifier: string, measurementType: Option<OntologyAnnotation>, technologyType: Option<OntologyAnnotation>, technologyPlatform: Option<string>, tables: ArcTable[], performers: FSharpList<Person>, comments: FSharpList<Comment$>): ArcAssay {
        return new ArcAssay(identifier, unwrap(measurementType), unwrap(technologyType), unwrap(technologyPlatform), tables, performers, comments);
    }
    get TableCount(): int32 {
        const this$: ArcAssay = this;
        return ArcTables__get_Count(ArcTables_$ctor_Z68BECB99(this$.Tables)) | 0;
    }
    get TableNames(): FSharpList<string> {
        const this$: ArcAssay = this;
        return ArcTables__get_TableNames(ArcTables_$ctor_Z68BECB99(this$.Tables));
    }
    AddTable(table: ArcTable, index?: int32): void {
        const this$: ArcAssay = this;
        ArcTables__AddTable_16F700A1(ArcTables_$ctor_Z68BECB99(this$.Tables), table, unwrap(index));
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
        ArcTables__AddTables_3601F24E(ArcTables_$ctor_Z68BECB99(this$.Tables), tables, unwrap(index));
    }
    static addTables(tables: Iterable<ArcTable>, index?: int32): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const c: ArcAssay = assay.Copy();
            c.AddTables(tables, unwrap(index));
            return c;
        };
    }
    InitTable(tableName: string, index?: int32): void {
        const this$: ArcAssay = this;
        ArcTables__InitTable_3B406CA4(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, unwrap(index));
    }
    static initTable(tableName: string, index?: int32): ((arg0: ArcAssay) => ArcAssay) {
        return (assay: ArcAssay): ArcAssay => {
            const c: ArcAssay = assay.Copy();
            c.InitTable(tableName, unwrap(index));
            return c;
        };
    }
    InitTables(tableNames: Iterable<string>, index?: int32): void {
        const this$: ArcAssay = this;
        ArcTables__InitTables_7B28792B(ArcTables_$ctor_Z68BECB99(this$.Tables), tableNames, unwrap(index));
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
        return ArcTables__GetTableAt_Z524259A4(ArcTables_$ctor_Z68BECB99(this$.Tables), index);
    }
    static getTableAt(index: int32): ((arg0: ArcAssay) => ArcTable) {
        return (assay: ArcAssay): ArcTable => {
            const newAssay: ArcAssay = assay.Copy();
            return newAssay.GetTableAt(index);
        };
    }
    GetTable(name: string): ArcTable {
        const this$: ArcAssay = this;
        return ArcTables__GetTable_Z721C83C5(ArcTables_$ctor_Z68BECB99(this$.Tables), name);
    }
    static getTable(name: string): ((arg0: ArcAssay) => ArcTable) {
        return (assay: ArcAssay): ArcTable => {
            const newAssay: ArcAssay = assay.Copy();
            return newAssay.GetTable(name);
        };
    }
    UpdateTableAt(index: int32, table: ArcTable): void {
        const this$: ArcAssay = this;
        ArcTables__UpdateTableAt_66578202(ArcTables_$ctor_Z68BECB99(this$.Tables), index, table);
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
        ArcTables__UpdateTable_4976F045(ArcTables_$ctor_Z68BECB99(this$.Tables), name, table);
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
        ArcTables__RemoveTableAt_Z524259A4(ArcTables_$ctor_Z68BECB99(this$.Tables), index);
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
        ArcTables__RemoveTable_Z721C83C5(ArcTables_$ctor_Z68BECB99(this$.Tables), name);
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
        ArcTables__MapTableAt_61602D68(ArcTables_$ctor_Z68BECB99(this$.Tables), index, updateFun);
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
        ArcTables__MapTable_4E415F2F(ArcTables_$ctor_Z68BECB99(this$.Tables), name, updateFun);
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
        ArcTables__RenameTableAt_Z176EF219(ArcTables_$ctor_Z68BECB99(this$.Tables), index, newName);
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
        ArcTables__RenameTable_Z384F8060(ArcTables_$ctor_Z68BECB99(this$.Tables), name, newName);
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
        ArcTables__AddColumnAt_6647579B(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
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
        ArcTables__AddColumn_Z4FC90944(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
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
        ArcTables__RemoveColumnAt_Z37302880(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, columnIndex);
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
        ArcTables__RemoveColumn_Z18115A39(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, columnIndex);
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
        ArcTables__UpdateColumnAt_Z155350AF(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, columnIndex, header, unwrap(cells));
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
        ArcTables__UpdateColumn_Z774BF72A(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, columnIndex, header, unwrap(cells));
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
        return ArcTables__GetColumnAt_Z37302880(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, columnIndex);
    }
    static getColumnAt(tableIndex: int32, columnIndex: int32): ((arg0: ArcAssay) => CompositeColumn) {
        return (assay: ArcAssay): CompositeColumn => {
            const newAssay: ArcAssay = assay.Copy();
            return newAssay.GetColumnAt(tableIndex, columnIndex);
        };
    }
    GetColumn(tableName: string, columnIndex: int32): CompositeColumn {
        const this$: ArcAssay = this;
        return ArcTables__GetColumn_Z18115A39(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, columnIndex);
    }
    static getColumn(tableName: string, columnIndex: int32): ((arg0: ArcAssay) => CompositeColumn) {
        return (assay: ArcAssay): CompositeColumn => {
            const newAssay: ArcAssay = assay.Copy();
            return newAssay.GetColumn(tableName, columnIndex);
        };
    }
    AddRowAt(tableIndex: int32, cells?: CompositeCell_$union[], rowIndex?: int32): void {
        const this$: ArcAssay = this;
        ArcTables__AddRowAt_Z57F91678(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, unwrap(cells), unwrap(rowIndex));
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
        ArcTables__AddRow_1177C4AF(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, unwrap(cells), unwrap(rowIndex));
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
        ArcTables__RemoveRowAt_Z37302880(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, rowIndex);
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
        ArcTables__RemoveRow_Z18115A39(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, rowIndex);
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
        ArcTables__UpdateRowAt_Z596C2D98(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, rowIndex, cells);
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
        ArcTables__UpdateRow_Z5E65B4B1(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, rowIndex, cells);
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
        return ArcTables__GetRowAt_Z37302880(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, rowIndex);
    }
    static getRowAt(tableIndex: int32, rowIndex: int32): ((arg0: ArcAssay) => CompositeCell_$union[]) {
        return (assay: ArcAssay): CompositeCell_$union[] => {
            const newAssay: ArcAssay = assay.Copy();
            return newAssay.GetRowAt(tableIndex, rowIndex);
        };
    }
    GetRow(tableName: string, rowIndex: int32): CompositeCell_$union[] {
        const this$: ArcAssay = this;
        return ArcTables__GetRow_Z18115A39(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, rowIndex);
    }
    static getRow(tableName: string, rowIndex: int32): ((arg0: ArcAssay) => CompositeCell_$union[]) {
        return (assay: ArcAssay): CompositeCell_$union[] => {
            const newAssay: ArcAssay = assay.Copy();
            return newAssay.GetRow(tableName, rowIndex);
        };
    }
    static setPerformers(performers: FSharpList<Person>, assay: ArcAssay): ArcAssay {
        assay.Performers = performers;
        return assay;
    }
    Copy(): ArcAssay {
        const this$: ArcAssay = this;
        const newTables: ArcTable[] = [];
        let enumerator: any = getEnumerator(this$.Tables);
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const table: ArcTable = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                const copy: ArcTable = table.Copy();
                void (newTables.push(copy));
            }
        }
        finally {
            disposeSafe(enumerator);
        }
        return new ArcAssay(this$.Identifier, unwrap(this$.MeasurementType), unwrap(this$.TechnologyType), unwrap(this$.TechnologyPlatform), newTables, this$.Performers, this$.Comments);
    }
    ToAssay(): Assay {
        const this$: ArcAssay = this;
        const processSeq: FSharpList<Process> = ArcTables__GetProcesses(ArcTables_$ctor_Z68BECB99(this$.Tables));
        let assayMaterials: Option<AssayMaterials>;
        const v_2: AssayMaterials = AssayMaterials_create_1CB3546D(unwrap(fromValueWithDefault<FSharpList<Sample>>(empty<Sample>(), getSamples(processSeq))), unwrap(fromValueWithDefault<FSharpList<Material>>(empty<Material>(), getMaterials(processSeq))));
        assayMaterials = fromValueWithDefault<AssayMaterials>(AssayMaterials_get_empty(), v_2);
        return Assay_create_ABF59A4(void 0, unwrap(isMissingIdentifier(this$.Identifier) ? void 0 : Assay_fileNameFromIdentifier(this$.Identifier)), unwrap(this$.MeasurementType), unwrap(this$.TechnologyType), unwrap(this$.TechnologyPlatform), unwrap(fromValueWithDefault<FSharpList<Data>>(empty<Data>(), getData(processSeq))), unwrap(assayMaterials), unwrap(fromValueWithDefault<FSharpList<MaterialAttribute>>(empty<MaterialAttribute>(), getCharacteristics(processSeq))), unwrap(fromValueWithDefault<FSharpList<OntologyAnnotation>>(empty<OntologyAnnotation>(), getUnits(processSeq))), unwrap(fromValueWithDefault<FSharpList<Process>>(empty<Process>(), processSeq)), unwrap(fromValueWithDefault<FSharpList<Comment$>>(empty<Comment$>(), this$.Comments)));
    }
    static fromAssay(a: Assay): ArcAssay {
        const tables: Option<ArcTable[]> = map<FSharpList<Process>, ArcTable[]>((arg_1: FSharpList<Process>): ArcTable[] => ArcTables__get_Tables(ArcTables_fromProcesses_Z31821267(arg_1)), a.ProcessSequence);
        let identifer: string;
        const matchValue: Option<string> = a.FileName;
        identifer = ((matchValue == null) ? createMissingIdentifier() : Assay_identifierFromFileName(value(matchValue)));
        return ArcAssay.create(identifer, unwrap(a.MeasurementType), unwrap(a.TechnologyType), unwrap(a.TechnologyPlatform), unwrap(tables), void 0, unwrap(a.Comments));
    }
}

export function ArcAssay_$reflection(): TypeInfo {
    return class_type("ISA.ArcAssay", void 0, ArcAssay);
}

export function ArcAssay_$ctor_4D629C41(identifier: string, measurementType?: OntologyAnnotation, technologyType?: OntologyAnnotation, technologyPlatform?: string, tables?: ArcTable[], performers?: FSharpList<Person>, comments?: FSharpList<Comment$>): ArcAssay {
    return new ArcAssay(identifier, measurementType, technologyType, technologyPlatform, tables, performers, comments);
}

