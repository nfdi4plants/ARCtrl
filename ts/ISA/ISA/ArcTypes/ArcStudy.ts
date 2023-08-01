import { defaultArg, map as map_2, value, Option, unwrap } from "../../../fable_modules/fable-library-ts/Option.js";
import { map as map_1, empty, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { Publication } from "../JsonTypes/Publication.js";
import { Person } from "../JsonTypes/Person.js";
import { OntologyAnnotation } from "../JsonTypes/OntologyAnnotation.js";
import { ArcTable } from "./ArcTable.js";
import { ArcAssay } from "./ArcAssay.js";
import { Factor } from "../JsonTypes/Factor.js";
import { Comment$ } from "../JsonTypes/Comment.js";
import { disposeSafe, getEnumerator, equals } from "../../../fable_modules/fable-library-ts/Util.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { toList, removeAt, tryFindIndex, map } from "../../../fable_modules/fable-library-ts/Seq.js";
import { ArcTables_fromProcesses_Z31821267, ArcTables__get_Tables, ArcTables__GetProcesses, ArcTables__GetRow_Z18115A39, ArcTables__GetRowAt_Z37302880, ArcTables__UpdateRow_Z5E65B4B1, ArcTables__UpdateRowAt_Z596C2D98, ArcTables__RemoveRow_Z18115A39, ArcTables__RemoveRowAt_Z37302880, ArcTables__AddRow_1177C4AF, ArcTables__AddRowAt_Z57F91678, ArcTables__GetColumn_Z18115A39, ArcTables__GetColumnAt_Z37302880, ArcTables__UpdateColumn_Z774BF72A, ArcTables__UpdateColumnAt_Z155350AF, ArcTables__RemoveColumn_Z18115A39, ArcTables__RemoveColumnAt_Z37302880, ArcTables__AddColumn_Z4FC90944, ArcTables__AddColumnAt_6647579B, ArcTables__RenameTable_Z384F8060, ArcTables__RenameTableAt_Z176EF219, ArcTables__MapTable_4E415F2F, ArcTables__MapTableAt_61602D68, ArcTables__RemoveTable_Z721C83C5, ArcTables__RemoveTableAt_Z524259A4, ArcTables__UpdateTable_4976F045, ArcTables__UpdateTableAt_66578202, ArcTables__GetTable_Z721C83C5, ArcTables__GetTableAt_Z524259A4, ArcTables__InitTables_7B28792B, ArcTables__InitTable_3B406CA4, ArcTables__AddTables_3601F24E, ArcTables__AddTable_16F700A1, ArcTables__get_TableNames, ArcTables_$ctor_Z68BECB99, ArcTables__get_Count } from "./ArcTables.js";
import { CompositeHeader_$union } from "./CompositeHeader.js";
import { CompositeCell_$union } from "./CompositeCell.js";
import { CompositeColumn } from "./CompositeColumn.js";
import { Process } from "../JsonTypes/Process.js";
import { fromValueWithDefault } from "../OptionExtensions.js";
import { Protocol } from "../JsonTypes/Protocol.js";
import { getUnits, getCharacteristics, getMaterials, getSamples, getSources, getProtocols } from "../JsonTypes/ProcessSequence.js";
import { Assay } from "../JsonTypes/Assay.js";
import { StudyMaterials_get_empty, StudyMaterials_create_Z460D555F, StudyMaterials } from "../JsonTypes/StudyMaterials.js";
import { Source } from "../JsonTypes/Source.js";
import { Sample } from "../JsonTypes/Sample.js";
import { Material } from "../JsonTypes/Material.js";
import { Study_identifierFromFileName, createMissingIdentifier, Study_fileNameFromIdentifier, isMissingIdentifier } from "./Identifier.js";
import { Study, Study_create_Z6C8AB268 } from "../JsonTypes/Study.js";
import { MaterialAttribute } from "../JsonTypes/MaterialAttribute.js";
import { class_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";

export class ArcStudy {
    readonly submissionDate: Option<string>;
    "identifier@25": string;
    "Title@": Option<string>;
    "Description@": Option<string>;
    "SubmissionDate@": Option<string>;
    "PublicReleaseDate@": Option<string>;
    "Publications@": FSharpList<Publication>;
    "Contacts@": FSharpList<Person>;
    "StudyDesignDescriptors@": FSharpList<OntologyAnnotation>;
    "Tables@": ArcTable[];
    "Assays@": ArcAssay[];
    "Factors@": FSharpList<Factor>;
    "Comments@": FSharpList<Comment$>;
    constructor(identifier: string, title?: string, description?: string, submissionDate?: string, publicReleaseDate?: string, publications?: FSharpList<Publication>, contacts?: FSharpList<Person>, studyDesignDescriptors?: FSharpList<OntologyAnnotation>, tables?: ArcTable[], assays?: ArcAssay[], factors?: FSharpList<Factor>, comments?: FSharpList<Comment$>) {
        this.submissionDate = submissionDate;
        const publications_1: FSharpList<Publication> = defaultArg<FSharpList<Publication>>(publications, empty<Publication>());
        const contacts_1: FSharpList<Person> = defaultArg<FSharpList<Person>>(contacts, empty<Person>());
        const studyDesignDescriptors_1: FSharpList<OntologyAnnotation> = defaultArg<FSharpList<OntologyAnnotation>>(studyDesignDescriptors, empty<OntologyAnnotation>());
        const tables_1: ArcTable[] = defaultArg<ArcTable[]>(tables, []);
        const assays_1: ArcAssay[] = defaultArg<ArcAssay[]>(assays, []);
        const factors_1: FSharpList<Factor> = defaultArg<FSharpList<Factor>>(factors, empty<Factor>());
        const comments_1: FSharpList<Comment$> = defaultArg<FSharpList<Comment$>>(comments, empty<Comment$>());
        this["identifier@25"] = identifier;
        this["Title@"] = title;
        this["Description@"] = description;
        this["SubmissionDate@"] = this.submissionDate;
        this["PublicReleaseDate@"] = publicReleaseDate;
        this["Publications@"] = publications_1;
        this["Contacts@"] = contacts_1;
        this["StudyDesignDescriptors@"] = studyDesignDescriptors_1;
        this["Tables@"] = tables_1;
        this["Assays@"] = assays_1;
        this["Factors@"] = factors_1;
        this["Comments@"] = comments_1;
    }
    get Identifier(): string {
        const this$: ArcStudy = this;
        return this$["identifier@25"];
    }
    set Identifier(i: string) {
        const this$: ArcStudy = this;
        this$["identifier@25"] = i;
    }
    get Title(): string | undefined {
        const __: ArcStudy = this;
        return unwrap(__["Title@"]);
    }
    set Title(v: Option<string>) {
        const __: ArcStudy = this;
        __["Title@"] = v;
    }
    get Description(): string | undefined {
        const __: ArcStudy = this;
        return unwrap(__["Description@"]);
    }
    set Description(v: Option<string>) {
        const __: ArcStudy = this;
        __["Description@"] = v;
    }
    get SubmissionDate(): string | undefined {
        const __: ArcStudy = this;
        return unwrap(__["SubmissionDate@"]);
    }
    set SubmissionDate(v: Option<string>) {
        const __: ArcStudy = this;
        __["SubmissionDate@"] = v;
    }
    get PublicReleaseDate(): string | undefined {
        const __: ArcStudy = this;
        return unwrap(__["PublicReleaseDate@"]);
    }
    set PublicReleaseDate(v: Option<string>) {
        const __: ArcStudy = this;
        __["PublicReleaseDate@"] = v;
    }
    get Publications(): FSharpList<Publication> {
        const __: ArcStudy = this;
        return __["Publications@"];
    }
    set Publications(v: FSharpList<Publication>) {
        const __: ArcStudy = this;
        __["Publications@"] = v;
    }
    get Contacts(): FSharpList<Person> {
        const __: ArcStudy = this;
        return __["Contacts@"];
    }
    set Contacts(v: FSharpList<Person>) {
        const __: ArcStudy = this;
        __["Contacts@"] = v;
    }
    get StudyDesignDescriptors(): FSharpList<OntologyAnnotation> {
        const __: ArcStudy = this;
        return __["StudyDesignDescriptors@"];
    }
    set StudyDesignDescriptors(v: FSharpList<OntologyAnnotation>) {
        const __: ArcStudy = this;
        __["StudyDesignDescriptors@"] = v;
    }
    get Tables(): ArcTable[] {
        const __: ArcStudy = this;
        return __["Tables@"];
    }
    set Tables(v: ArcTable[]) {
        const __: ArcStudy = this;
        __["Tables@"] = v;
    }
    get Assays(): ArcAssay[] {
        const __: ArcStudy = this;
        return __["Assays@"];
    }
    set Assays(v: ArcAssay[]) {
        const __: ArcStudy = this;
        __["Assays@"] = v;
    }
    get Factors(): FSharpList<Factor> {
        const __: ArcStudy = this;
        return __["Factors@"];
    }
    set Factors(v: FSharpList<Factor>) {
        const __: ArcStudy = this;
        __["Factors@"] = v;
    }
    get Comments(): FSharpList<Comment$> {
        const __: ArcStudy = this;
        return __["Comments@"];
    }
    set Comments(v: FSharpList<Comment$>) {
        const __: ArcStudy = this;
        __["Comments@"] = v;
    }
    static init(identifier: string): ArcStudy {
        return new ArcStudy(identifier);
    }
    static create(identifier: string, title?: string, description?: string, submissionDate?: string, publicReleaseDate?: string, publications?: FSharpList<Publication>, contacts?: FSharpList<Person>, studyDesignDescriptors?: FSharpList<OntologyAnnotation>, tables?: ArcTable[], assays?: ArcAssay[], factors?: FSharpList<Factor>, comments?: FSharpList<Comment$>): ArcStudy {
        return new ArcStudy(identifier, unwrap(title), unwrap(description), unwrap(submissionDate), unwrap(publicReleaseDate), unwrap(publications), unwrap(contacts), unwrap(studyDesignDescriptors), unwrap(tables), unwrap(assays), unwrap(factors), unwrap(comments));
    }
    static make(identifier: string, title: Option<string>, description: Option<string>, submissionDate: Option<string>, publicReleaseDate: Option<string>, publications: FSharpList<Publication>, contacts: FSharpList<Person>, studyDesignDescriptors: FSharpList<OntologyAnnotation>, tables: ArcTable[], assays: ArcAssay[], factors: FSharpList<Factor>, comments: FSharpList<Comment$>): ArcStudy {
        return new ArcStudy(identifier, unwrap(title), unwrap(description), unwrap(submissionDate), unwrap(publicReleaseDate), publications, contacts, studyDesignDescriptors, tables, assays, factors, comments);
    }
    get isEmpty(): boolean {
        const this$: ArcStudy = this;
        return (((((((((equals(this$.Title, void 0) && equals(this$.Description, void 0)) && equals(this$.SubmissionDate, void 0)) && equals(this$.PublicReleaseDate, void 0)) && equals(this$.Publications, empty<Publication>())) && equals(this$.Contacts, empty<Person>())) && equals(this$.StudyDesignDescriptors, empty<OntologyAnnotation>())) && (this$.Tables.length === 0)) && (this$.Assays.length === 0)) && equals(this$.Factors, empty<Factor>())) && equals(this$.Comments, empty<Comment$>());
    }
    static get FileName(): string {
        return "isa.study.xlsx";
    }
    get AssayCount(): int32 {
        const this$: ArcStudy = this;
        return this$.Assays.length | 0;
    }
    get AssayIdentifiers(): Iterable<string> {
        const this$: ArcStudy = this;
        return map<ArcAssay, string>((x: ArcAssay): string => x.Identifier, this$.Assays);
    }
    AddAssay(assay: ArcAssay): void {
        const this$: ArcStudy = this;
        const assay_1: ArcAssay = assay;
        const matchValue: Option<int32> = tryFindIndex<ArcAssay>((x: ArcAssay): boolean => (x.Identifier === assay_1.Identifier), this$.Assays);
        if (matchValue == null) {
        }
        else {
            throw new Error(`Cannot create assay with name ${assay_1.Identifier}, as assay names must be unique and assay at index ${value(matchValue)} has the same name.`);
        }
        void (this$.Assays.push(assay));
    }
    static addAssay(assay: ArcAssay): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newStudy: ArcStudy = study.Copy();
            newStudy.AddAssay(assay);
            return newStudy;
        };
    }
    InitAssay(assayName: string): void {
        const this$: ArcStudy = this;
        const assay: ArcAssay = new ArcAssay(assayName);
        this$.AddAssay(assay);
    }
    static initAssay(assayName: string): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newStudy: ArcStudy = study.Copy();
            newStudy.InitAssay(assayName);
            return newStudy;
        };
    }
    RemoveAssayAt(index: int32): void {
        const this$: ArcStudy = this;
        this$.Assays.splice(index, 1);
    }
    static removeAssayAt(index: int32): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newStudy: ArcStudy = study.Copy();
            newStudy.RemoveAssayAt(index);
            return newStudy;
        };
    }
    SetAssayAt(index: int32, assay: ArcAssay): void {
        const this$: ArcStudy = this;
        const assay_1: ArcAssay = assay;
        const matchValue: Option<int32> = tryFindIndex<ArcAssay>((x: ArcAssay): boolean => (x.Identifier === assay_1.Identifier), removeAt<ArcAssay>(index, this$.Assays));
        if (matchValue == null) {
        }
        else {
            throw new Error(`Cannot create assay with name ${assay_1.Identifier}, as assay names must be unique and assay at index ${value(matchValue)} has the same name.`);
        }
        this$.Assays[index] = assay;
    }
    static setAssayAt(index: int32, assay: ArcAssay): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newStudy: ArcStudy = study.Copy();
            newStudy.SetAssayAt(index, assay);
            return newStudy;
        };
    }
    SetAssay(assayIdentifier: string, assay: ArcAssay): void {
        const this$: ArcStudy = this;
        const index: int32 = this$.GetAssayIndex(assayIdentifier) | 0;
        this$.Assays[index] = assay;
    }
    static setAssay(assayIdentifier: string, assay: ArcAssay): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newStudy: ArcStudy = study.Copy();
            newStudy.SetAssay(assayIdentifier, assay);
            return newStudy;
        };
    }
    GetAssayIndex(assayIdentifier: string): int32 {
        const this$: ArcStudy = this;
        const index: int32 = this$.Assays.findIndex((a: ArcAssay): boolean => (a.Identifier === assayIdentifier)) | 0;
        if (index === -1) {
            throw new Error(`Unable to find assay with specified identifier '${assayIdentifier}'!`);
        }
        return index | 0;
    }
    static GetAssayIndex(assayIdentifier: string): ((arg0: ArcStudy) => int32) {
        return (study: ArcStudy): int32 => study.GetAssayIndex(assayIdentifier);
    }
    GetAssayAt(index: int32): ArcAssay {
        const this$: ArcStudy = this;
        return this$.Assays[index];
    }
    static getAssayAt(index: int32): ((arg0: ArcStudy) => ArcAssay) {
        return (study: ArcStudy): ArcAssay => {
            const newStudy: ArcStudy = study.Copy();
            return newStudy.GetAssayAt(index);
        };
    }
    GetAssay(assayIdentifier: string): ArcAssay {
        const this$: ArcStudy = this;
        const index: int32 = this$.GetAssayIndex(assayIdentifier) | 0;
        return this$.GetAssayAt(index);
    }
    static getAssay(assayIdentifier: string): ((arg0: ArcStudy) => ArcAssay) {
        return (study: ArcStudy): ArcAssay => {
            const newStudy: ArcStudy = study.Copy();
            return newStudy.GetAssay(assayIdentifier);
        };
    }
    get TableCount(): int32 {
        const this$: ArcStudy = this;
        return ArcTables__get_Count(ArcTables_$ctor_Z68BECB99(this$.Tables)) | 0;
    }
    get TableNames(): FSharpList<string> {
        const this$: ArcStudy = this;
        return ArcTables__get_TableNames(ArcTables_$ctor_Z68BECB99(this$.Tables));
    }
    AddTable(table: ArcTable, index?: int32): void {
        const this$: ArcStudy = this;
        ArcTables__AddTable_16F700A1(ArcTables_$ctor_Z68BECB99(this$.Tables), table, unwrap(index));
    }
    static addTable(table: ArcTable, index?: int32): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const c: ArcStudy = study.Copy();
            c.AddTable(table, unwrap(index));
            return c;
        };
    }
    AddTables(tables: Iterable<ArcTable>, index?: int32): void {
        const this$: ArcStudy = this;
        ArcTables__AddTables_3601F24E(ArcTables_$ctor_Z68BECB99(this$.Tables), tables, unwrap(index));
    }
    static addTables(tables: Iterable<ArcTable>, index?: int32): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const c: ArcStudy = study.Copy();
            c.AddTables(tables, unwrap(index));
            return c;
        };
    }
    InitTable(tableName: string, index?: int32): void {
        const this$: ArcStudy = this;
        ArcTables__InitTable_3B406CA4(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, unwrap(index));
    }
    static initTable(tableName: string, index?: int32): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const c: ArcStudy = study.Copy();
            c.InitTable(tableName, unwrap(index));
            return c;
        };
    }
    InitTables(tableNames: Iterable<string>, index?: int32): void {
        const this$: ArcStudy = this;
        ArcTables__InitTables_7B28792B(ArcTables_$ctor_Z68BECB99(this$.Tables), tableNames, unwrap(index));
    }
    static initTables(tableNames: Iterable<string>, index?: int32): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const c: ArcStudy = study.Copy();
            c.InitTables(tableNames, unwrap(index));
            return c;
        };
    }
    GetTableAt(index: int32): ArcTable {
        const this$: ArcStudy = this;
        return ArcTables__GetTableAt_Z524259A4(ArcTables_$ctor_Z68BECB99(this$.Tables), index);
    }
    static getTableAt(index: int32): ((arg0: ArcStudy) => ArcTable) {
        return (study: ArcStudy): ArcTable => {
            const newAssay: ArcStudy = study.Copy();
            return newAssay.GetTableAt(index);
        };
    }
    GetTable(name: string): ArcTable {
        const this$: ArcStudy = this;
        return ArcTables__GetTable_Z721C83C5(ArcTables_$ctor_Z68BECB99(this$.Tables), name);
    }
    static getTable(name: string): ((arg0: ArcStudy) => ArcTable) {
        return (study: ArcStudy): ArcTable => {
            const newAssay: ArcStudy = study.Copy();
            return newAssay.GetTable(name);
        };
    }
    UpdateTableAt(index: int32, table: ArcTable): void {
        const this$: ArcStudy = this;
        ArcTables__UpdateTableAt_66578202(ArcTables_$ctor_Z68BECB99(this$.Tables), index, table);
    }
    static updateTableAt(index: int32, table: ArcTable): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newAssay: ArcStudy = study.Copy();
            newAssay.UpdateTableAt(index, table);
            return newAssay;
        };
    }
    UpdateTable(name: string, table: ArcTable): void {
        const this$: ArcStudy = this;
        ArcTables__UpdateTable_4976F045(ArcTables_$ctor_Z68BECB99(this$.Tables), name, table);
    }
    static updateTable(name: string, table: ArcTable): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newAssay: ArcStudy = study.Copy();
            newAssay.UpdateTable(name, table);
            return newAssay;
        };
    }
    RemoveTableAt(index: int32): void {
        const this$: ArcStudy = this;
        ArcTables__RemoveTableAt_Z524259A4(ArcTables_$ctor_Z68BECB99(this$.Tables), index);
    }
    static removeTableAt(index: int32): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newAssay: ArcStudy = study.Copy();
            newAssay.RemoveTableAt(index);
            return newAssay;
        };
    }
    RemoveTable(name: string): void {
        const this$: ArcStudy = this;
        ArcTables__RemoveTable_Z721C83C5(ArcTables_$ctor_Z68BECB99(this$.Tables), name);
    }
    static removeTable(name: string): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newAssay: ArcStudy = study.Copy();
            newAssay.RemoveTable(name);
            return newAssay;
        };
    }
    MapTableAt(index: int32, updateFun: ((arg0: ArcTable) => void)): void {
        const this$: ArcStudy = this;
        ArcTables__MapTableAt_61602D68(ArcTables_$ctor_Z68BECB99(this$.Tables), index, updateFun);
    }
    static mapTableAt(index: int32, updateFun: ((arg0: ArcTable) => void)): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newAssay: ArcStudy = study.Copy();
            newAssay.MapTableAt(index, updateFun);
            return newAssay;
        };
    }
    MapTable(name: string, updateFun: ((arg0: ArcTable) => void)): void {
        const this$: ArcStudy = this;
        ArcTables__MapTable_4E415F2F(ArcTables_$ctor_Z68BECB99(this$.Tables), name, updateFun);
    }
    static mapTable(name: string, updateFun: ((arg0: ArcTable) => void)): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newAssay: ArcStudy = study.Copy();
            newAssay.MapTable(name, updateFun);
            return newAssay;
        };
    }
    RenameTableAt(index: int32, newName: string): void {
        const this$: ArcStudy = this;
        ArcTables__RenameTableAt_Z176EF219(ArcTables_$ctor_Z68BECB99(this$.Tables), index, newName);
    }
    static renameTableAt(index: int32, newName: string): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newAssay: ArcStudy = study.Copy();
            newAssay.RenameTableAt(index, newName);
            return newAssay;
        };
    }
    RenameTable(name: string, newName: string): void {
        const this$: ArcStudy = this;
        ArcTables__RenameTable_Z384F8060(ArcTables_$ctor_Z68BECB99(this$.Tables), name, newName);
    }
    static renameTable(name: string, newName: string): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newAssay: ArcStudy = study.Copy();
            newAssay.RenameTable(name, newName);
            return newAssay;
        };
    }
    AddColumnAt(tableIndex: int32, header: CompositeHeader_$union, cells?: CompositeCell_$union[], columnIndex?: int32, forceReplace?: boolean): void {
        const this$: ArcStudy = this;
        ArcTables__AddColumnAt_6647579B(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
    }
    static addColumnAt(tableIndex: int32, header: CompositeHeader_$union, cells?: CompositeCell_$union[], columnIndex?: int32, forceReplace?: boolean): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newAssay: ArcStudy = study.Copy();
            newAssay.AddColumnAt(tableIndex, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
            return newAssay;
        };
    }
    AddColumn(tableName: string, header: CompositeHeader_$union, cells?: CompositeCell_$union[], columnIndex?: int32, forceReplace?: boolean): void {
        const this$: ArcStudy = this;
        ArcTables__AddColumn_Z4FC90944(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
    }
    static addColumn(tableName: string, header: CompositeHeader_$union, cells?: CompositeCell_$union[], columnIndex?: int32, forceReplace?: boolean): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newAssay: ArcStudy = study.Copy();
            newAssay.AddColumn(tableName, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
            return newAssay;
        };
    }
    RemoveColumnAt(tableIndex: int32, columnIndex: int32): void {
        const this$: ArcStudy = this;
        ArcTables__RemoveColumnAt_Z37302880(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, columnIndex);
    }
    static removeColumnAt(tableIndex: int32, columnIndex: int32): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newAssay: ArcStudy = study.Copy();
            newAssay.RemoveColumnAt(tableIndex, columnIndex);
            return newAssay;
        };
    }
    RemoveColumn(tableName: string, columnIndex: int32): void {
        const this$: ArcStudy = this;
        ArcTables__RemoveColumn_Z18115A39(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, columnIndex);
    }
    static removeColumn(tableName: string, columnIndex: int32): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newAssay: ArcStudy = study.Copy();
            newAssay.RemoveColumn(tableName, columnIndex);
            return newAssay;
        };
    }
    UpdateColumnAt(tableIndex: int32, columnIndex: int32, header: CompositeHeader_$union, cells?: CompositeCell_$union[]): void {
        const this$: ArcStudy = this;
        ArcTables__UpdateColumnAt_Z155350AF(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, columnIndex, header, unwrap(cells));
    }
    static updateColumnAt(tableIndex: int32, columnIndex: int32, header: CompositeHeader_$union, cells?: CompositeCell_$union[]): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newAssay: ArcStudy = study.Copy();
            newAssay.UpdateColumnAt(tableIndex, columnIndex, header, unwrap(cells));
            return newAssay;
        };
    }
    UpdateColumn(tableName: string, columnIndex: int32, header: CompositeHeader_$union, cells?: CompositeCell_$union[]): void {
        const this$: ArcStudy = this;
        ArcTables__UpdateColumn_Z774BF72A(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, columnIndex, header, unwrap(cells));
    }
    static updateColumn(tableName: string, columnIndex: int32, header: CompositeHeader_$union, cells?: CompositeCell_$union[]): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newAssay: ArcStudy = study.Copy();
            newAssay.UpdateColumn(tableName, columnIndex, header, unwrap(cells));
            return newAssay;
        };
    }
    GetColumnAt(tableIndex: int32, columnIndex: int32): CompositeColumn {
        const this$: ArcStudy = this;
        return ArcTables__GetColumnAt_Z37302880(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, columnIndex);
    }
    static getColumnAt(tableIndex: int32, columnIndex: int32): ((arg0: ArcStudy) => CompositeColumn) {
        return (study: ArcStudy): CompositeColumn => {
            const newAssay: ArcStudy = study.Copy();
            return newAssay.GetColumnAt(tableIndex, columnIndex);
        };
    }
    GetColumn(tableName: string, columnIndex: int32): CompositeColumn {
        const this$: ArcStudy = this;
        return ArcTables__GetColumn_Z18115A39(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, columnIndex);
    }
    static getColumn(tableName: string, columnIndex: int32): ((arg0: ArcStudy) => CompositeColumn) {
        return (study: ArcStudy): CompositeColumn => {
            const newAssay: ArcStudy = study.Copy();
            return newAssay.GetColumn(tableName, columnIndex);
        };
    }
    AddRowAt(tableIndex: int32, cells?: CompositeCell_$union[], rowIndex?: int32): void {
        const this$: ArcStudy = this;
        ArcTables__AddRowAt_Z57F91678(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, unwrap(cells), unwrap(rowIndex));
    }
    static addRowAt(tableIndex: int32, cells?: CompositeCell_$union[], rowIndex?: int32): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newAssay: ArcStudy = study.Copy();
            newAssay.AddRowAt(tableIndex, unwrap(cells), unwrap(rowIndex));
            return newAssay;
        };
    }
    AddRow(tableName: string, cells?: CompositeCell_$union[], rowIndex?: int32): void {
        const this$: ArcStudy = this;
        ArcTables__AddRow_1177C4AF(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, unwrap(cells), unwrap(rowIndex));
    }
    static addRow(tableName: string, cells?: CompositeCell_$union[], rowIndex?: int32): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newAssay: ArcStudy = study.Copy();
            newAssay.AddRow(tableName, unwrap(cells), unwrap(rowIndex));
            return newAssay;
        };
    }
    RemoveRowAt(tableIndex: int32, rowIndex: int32): void {
        const this$: ArcStudy = this;
        ArcTables__RemoveRowAt_Z37302880(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, rowIndex);
    }
    static removeRowAt(tableIndex: int32, rowIndex: int32): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newAssay: ArcStudy = study.Copy();
            newAssay.RemoveColumnAt(tableIndex, rowIndex);
            return newAssay;
        };
    }
    RemoveRow(tableName: string, rowIndex: int32): void {
        const this$: ArcStudy = this;
        ArcTables__RemoveRow_Z18115A39(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, rowIndex);
    }
    static removeRow(tableName: string, rowIndex: int32): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newAssay: ArcStudy = study.Copy();
            newAssay.RemoveRow(tableName, rowIndex);
            return newAssay;
        };
    }
    UpdateRowAt(tableIndex: int32, rowIndex: int32, cells: CompositeCell_$union[]): void {
        const this$: ArcStudy = this;
        ArcTables__UpdateRowAt_Z596C2D98(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, rowIndex, cells);
    }
    static updateRowAt(tableIndex: int32, rowIndex: int32, cells: CompositeCell_$union[]): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newAssay: ArcStudy = study.Copy();
            newAssay.UpdateRowAt(tableIndex, rowIndex, cells);
            return newAssay;
        };
    }
    UpdateRow(tableName: string, rowIndex: int32, cells: CompositeCell_$union[]): void {
        const this$: ArcStudy = this;
        ArcTables__UpdateRow_Z5E65B4B1(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, rowIndex, cells);
    }
    static updateRow(tableName: string, rowIndex: int32, cells: CompositeCell_$union[]): ((arg0: ArcStudy) => ArcStudy) {
        return (study: ArcStudy): ArcStudy => {
            const newAssay: ArcStudy = study.Copy();
            newAssay.UpdateRow(tableName, rowIndex, cells);
            return newAssay;
        };
    }
    GetRowAt(tableIndex: int32, rowIndex: int32): CompositeCell_$union[] {
        const this$: ArcStudy = this;
        return ArcTables__GetRowAt_Z37302880(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, rowIndex);
    }
    static getRowAt(tableIndex: int32, rowIndex: int32): ((arg0: ArcStudy) => CompositeCell_$union[]) {
        return (study: ArcStudy): CompositeCell_$union[] => {
            const newAssay: ArcStudy = study.Copy();
            return newAssay.GetRowAt(tableIndex, rowIndex);
        };
    }
    GetRow(tableName: string, rowIndex: int32): CompositeCell_$union[] {
        const this$: ArcStudy = this;
        return ArcTables__GetRow_Z18115A39(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, rowIndex);
    }
    static getRow(tableName: string, rowIndex: int32): ((arg0: ArcStudy) => CompositeCell_$union[]) {
        return (study: ArcStudy): CompositeCell_$union[] => {
            const newAssay: ArcStudy = study.Copy();
            return newAssay.GetRow(tableName, rowIndex);
        };
    }
    Copy(): ArcStudy {
        const this$: ArcStudy = this;
        const newTables: ArcTable[] = [];
        const newAssays: ArcAssay[] = [];
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
        let enumerator_1: any = getEnumerator(this$.Assays);
        try {
            while (enumerator_1["System.Collections.IEnumerator.MoveNext"]()) {
                const study: ArcAssay = enumerator_1["System.Collections.Generic.IEnumerator`1.get_Current"]();
                const copy_1: ArcAssay = study.Copy();
                void (newAssays.push(copy_1));
            }
        }
        finally {
            disposeSafe(enumerator_1);
        }
        return new ArcStudy(this$.Identifier, unwrap(this$.Title), unwrap(this$.Description), unwrap(this$.submissionDate), unwrap(this$.PublicReleaseDate), this$.Publications, this$.Contacts, this$.StudyDesignDescriptors, newTables, newAssays, this$.Factors, this$.Comments);
    }
    ToStudy(): Study {
        const this$: ArcStudy = this;
        const processSeq: FSharpList<Process> = ArcTables__GetProcesses(ArcTables_$ctor_Z68BECB99(this$.Tables));
        const protocols: Option<FSharpList<Protocol>> = fromValueWithDefault<FSharpList<Protocol>>(empty<Protocol>(), getProtocols(processSeq));
        const assays: Option<FSharpList<Assay>> = fromValueWithDefault<FSharpList<Assay>>(empty<Assay>(), map_1<ArcAssay, Assay>((a: ArcAssay): Assay => a.ToAssay(), toList<ArcAssay>(this$.Assays)));
        let studyMaterials: Option<StudyMaterials>;
        const v_5: StudyMaterials = StudyMaterials_create_Z460D555F(unwrap(fromValueWithDefault<FSharpList<Source>>(empty<Source>(), getSources(processSeq))), unwrap(fromValueWithDefault<FSharpList<Sample>>(empty<Sample>(), getSamples(processSeq))), unwrap(fromValueWithDefault<FSharpList<Material>>(empty<Material>(), getMaterials(processSeq))));
        studyMaterials = fromValueWithDefault<StudyMaterials>(StudyMaterials_get_empty(), v_5);
        const patternInput: [Option<string>, Option<string>] = isMissingIdentifier(this$.Identifier) ? ([void 0, void 0] as [Option<string>, Option<string>]) : ([this$.Identifier, Study_fileNameFromIdentifier(this$.Identifier)] as [Option<string>, Option<string>]);
        return Study_create_Z6C8AB268(void 0, unwrap(patternInput[1]), unwrap(patternInput[0]), unwrap(this$.Title), unwrap(this$.Description), unwrap(this$.SubmissionDate), unwrap(this$.PublicReleaseDate), unwrap(fromValueWithDefault<FSharpList<Publication>>(empty<Publication>(), this$.Publications)), unwrap(fromValueWithDefault<FSharpList<Person>>(empty<Person>(), this$.Contacts)), unwrap(fromValueWithDefault<FSharpList<OntologyAnnotation>>(empty<OntologyAnnotation>(), this$.StudyDesignDescriptors)), unwrap(protocols), unwrap(studyMaterials), unwrap(fromValueWithDefault<FSharpList<Process>>(empty<Process>(), processSeq)), unwrap(assays), unwrap(fromValueWithDefault<FSharpList<Factor>>(empty<Factor>(), this$.Factors)), unwrap(fromValueWithDefault<FSharpList<MaterialAttribute>>(empty<MaterialAttribute>(), getCharacteristics(processSeq))), unwrap(fromValueWithDefault<FSharpList<OntologyAnnotation>>(empty<OntologyAnnotation>(), getUnits(processSeq))), unwrap(fromValueWithDefault<FSharpList<Comment$>>(empty<Comment$>(), this$.Comments)));
    }
    static fromStudy(s: Study): ArcStudy {
        const tables: Option<ArcTable[]> = map_2<FSharpList<Process>, ArcTable[]>((arg_1: FSharpList<Process>): ArcTable[] => ArcTables__get_Tables(ArcTables_fromProcesses_Z31821267(arg_1)), s.ProcessSequence);
        let identifer: string;
        const matchValue: Option<string> = s.FileName;
        identifer = ((matchValue == null) ? createMissingIdentifier() : Study_identifierFromFileName(value(matchValue)));
        const assays: Option<ArcAssay[]> = map_2<FSharpList<Assay>, ArcAssay[]>((arg_4: FSharpList<Assay>): ArcAssay[] => {
            const arg_3: FSharpList<ArcAssay> = map_1<Assay, ArcAssay>((arg_2: Assay): ArcAssay => ArcAssay.fromAssay(arg_2), arg_4);
            return Array.from(arg_3);
        }, s.Assays);
        return ArcStudy.create(identifer, unwrap(s.Title), unwrap(s.Description), unwrap(s.SubmissionDate), unwrap(s.PublicReleaseDate), unwrap(s.Publications), unwrap(s.Contacts), unwrap(s.StudyDesignDescriptors), unwrap(tables), unwrap(assays), unwrap(s.Factors), unwrap(s.Comments));
    }
}

export function ArcStudy_$reflection(): TypeInfo {
    return class_type("ISA.ArcStudy", void 0, ArcStudy);
}

export function ArcStudy_$ctor_Z337E69A6(identifier: string, title?: string, description?: string, submissionDate?: string, publicReleaseDate?: string, publications?: FSharpList<Publication>, contacts?: FSharpList<Person>, studyDesignDescriptors?: FSharpList<OntologyAnnotation>, tables?: ArcTable[], assays?: ArcAssay[], factors?: FSharpList<Factor>, comments?: FSharpList<Comment$>): ArcStudy {
    return new ArcStudy(identifier, title, description, submissionDate, publicReleaseDate, publications, contacts, studyDesignDescriptors, tables, assays, factors, comments);
}

