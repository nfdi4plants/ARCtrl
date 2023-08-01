import { defaultArg, map as map_2, unwrap } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { disposeSafe, getEnumerator, equals } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { map as map_1, empty } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { toList, removeAt, tryFindIndex, map } from "../../../fable_modules/fable-library.4.1.4/Seq.js";
import { ArcAssay } from "./ArcAssay.js";
import { ArcTables_fromProcesses_Z31821267, ArcTables__get_Tables, ArcTables__GetProcesses, ArcTables__GetRow_Z18115A39, ArcTables__GetRowAt_Z37302880, ArcTables__UpdateRow_Z5E65B4B1, ArcTables__UpdateRowAt_Z596C2D98, ArcTables__RemoveRow_Z18115A39, ArcTables__RemoveRowAt_Z37302880, ArcTables__AddRow_1177C4AF, ArcTables__AddRowAt_Z57F91678, ArcTables__GetColumn_Z18115A39, ArcTables__GetColumnAt_Z37302880, ArcTables__UpdateColumn_Z774BF72A, ArcTables__UpdateColumnAt_Z155350AF, ArcTables__RemoveColumn_Z18115A39, ArcTables__RemoveColumnAt_Z37302880, ArcTables__AddColumn_Z4FC90944, ArcTables__AddColumnAt_6647579B, ArcTables__RenameTable_Z384F8060, ArcTables__RenameTableAt_Z176EF219, ArcTables__MapTable_4E415F2F, ArcTables__MapTableAt_61602D68, ArcTables__RemoveTable_Z721C83C5, ArcTables__RemoveTableAt_Z524259A4, ArcTables__UpdateTable_4976F045, ArcTables__UpdateTableAt_66578202, ArcTables__GetTable_Z721C83C5, ArcTables__GetTableAt_Z524259A4, ArcTables__InitTables_7B28792B, ArcTables__InitTable_3B406CA4, ArcTables__AddTables_3601F24E, ArcTables__AddTable_16F700A1, ArcTables__get_TableNames, ArcTables_$ctor_Z68BECB99, ArcTables__get_Count } from "./ArcTables.js";
import { fromValueWithDefault } from "../OptionExtensions.js";
import { getUnits, getCharacteristics, getMaterials, getSamples, getSources, getProtocols } from "../JsonTypes/ProcessSequence.js";
import { StudyMaterials_get_empty, StudyMaterials_create_Z460D555F } from "../JsonTypes/StudyMaterials.js";
import { Study_identifierFromFileName, createMissingIdentifier, Study_fileNameFromIdentifier, isMissingIdentifier } from "./Identifier.js";
import { Study_create_Z6C8AB268 } from "../JsonTypes/Study.js";
import { class_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";

export class ArcStudy {
    constructor(identifier, title, description, submissionDate, publicReleaseDate, publications, contacts, studyDesignDescriptors, tables, assays, factors, comments) {
        this.submissionDate = submissionDate;
        const publications_1 = defaultArg(publications, empty());
        const contacts_1 = defaultArg(contacts, empty());
        const studyDesignDescriptors_1 = defaultArg(studyDesignDescriptors, empty());
        const tables_1 = defaultArg(tables, []);
        const assays_1 = defaultArg(assays, []);
        const factors_1 = defaultArg(factors, empty());
        const comments_1 = defaultArg(comments, empty());
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
    get Identifier() {
        const this$ = this;
        return this$["identifier@25"];
    }
    set Identifier(i) {
        const this$ = this;
        this$["identifier@25"] = i;
    }
    get Title() {
        const __ = this;
        return unwrap(__["Title@"]);
    }
    set Title(v) {
        const __ = this;
        __["Title@"] = v;
    }
    get Description() {
        const __ = this;
        return unwrap(__["Description@"]);
    }
    set Description(v) {
        const __ = this;
        __["Description@"] = v;
    }
    get SubmissionDate() {
        const __ = this;
        return unwrap(__["SubmissionDate@"]);
    }
    set SubmissionDate(v) {
        const __ = this;
        __["SubmissionDate@"] = v;
    }
    get PublicReleaseDate() {
        const __ = this;
        return unwrap(__["PublicReleaseDate@"]);
    }
    set PublicReleaseDate(v) {
        const __ = this;
        __["PublicReleaseDate@"] = v;
    }
    get Publications() {
        const __ = this;
        return __["Publications@"];
    }
    set Publications(v) {
        const __ = this;
        __["Publications@"] = v;
    }
    get Contacts() {
        const __ = this;
        return __["Contacts@"];
    }
    set Contacts(v) {
        const __ = this;
        __["Contacts@"] = v;
    }
    get StudyDesignDescriptors() {
        const __ = this;
        return __["StudyDesignDescriptors@"];
    }
    set StudyDesignDescriptors(v) {
        const __ = this;
        __["StudyDesignDescriptors@"] = v;
    }
    get Tables() {
        const __ = this;
        return __["Tables@"];
    }
    set Tables(v) {
        const __ = this;
        __["Tables@"] = v;
    }
    get Assays() {
        const __ = this;
        return __["Assays@"];
    }
    set Assays(v) {
        const __ = this;
        __["Assays@"] = v;
    }
    get Factors() {
        const __ = this;
        return __["Factors@"];
    }
    set Factors(v) {
        const __ = this;
        __["Factors@"] = v;
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
        return new ArcStudy(identifier);
    }
    static create(identifier, title, description, submissionDate, publicReleaseDate, publications, contacts, studyDesignDescriptors, tables, assays, factors, comments) {
        return new ArcStudy(identifier, unwrap(title), unwrap(description), unwrap(submissionDate), unwrap(publicReleaseDate), unwrap(publications), unwrap(contacts), unwrap(studyDesignDescriptors), unwrap(tables), unwrap(assays), unwrap(factors), unwrap(comments));
    }
    static make(identifier, title, description, submissionDate, publicReleaseDate, publications, contacts, studyDesignDescriptors, tables, assays, factors, comments) {
        return new ArcStudy(identifier, unwrap(title), unwrap(description), unwrap(submissionDate), unwrap(publicReleaseDate), publications, contacts, studyDesignDescriptors, tables, assays, factors, comments);
    }
    get isEmpty() {
        const this$ = this;
        return (((((((((equals(this$.Title, void 0) && equals(this$.Description, void 0)) && equals(this$.SubmissionDate, void 0)) && equals(this$.PublicReleaseDate, void 0)) && equals(this$.Publications, empty())) && equals(this$.Contacts, empty())) && equals(this$.StudyDesignDescriptors, empty())) && (this$.Tables.length === 0)) && (this$.Assays.length === 0)) && equals(this$.Factors, empty())) && equals(this$.Comments, empty());
    }
    static get FileName() {
        return "isa.study.xlsx";
    }
    get AssayCount() {
        const this$ = this;
        return this$.Assays.length | 0;
    }
    get AssayIdentifiers() {
        const this$ = this;
        return map((x) => x.Identifier, this$.Assays);
    }
    AddAssay(assay) {
        const this$ = this;
        const assay_1 = assay;
        const matchValue = tryFindIndex((x) => (x.Identifier === assay_1.Identifier), this$.Assays);
        if (matchValue == null) {
        }
        else {
            throw new Error(`Cannot create assay with name ${assay_1.Identifier}, as assay names must be unique and assay at index ${matchValue} has the same name.`);
        }
        void (this$.Assays.push(assay));
    }
    static addAssay(assay) {
        return (study) => {
            const newStudy = study.Copy();
            newStudy.AddAssay(assay);
            return newStudy;
        };
    }
    InitAssay(assayName) {
        const this$ = this;
        const assay = new ArcAssay(assayName);
        this$.AddAssay(assay);
    }
    static initAssay(assayName) {
        return (study) => {
            const newStudy = study.Copy();
            newStudy.InitAssay(assayName);
            return newStudy;
        };
    }
    RemoveAssayAt(index) {
        const this$ = this;
        this$.Assays.splice(index, 1);
    }
    static removeAssayAt(index) {
        return (study) => {
            const newStudy = study.Copy();
            newStudy.RemoveAssayAt(index);
            return newStudy;
        };
    }
    SetAssayAt(index, assay) {
        const this$ = this;
        const assay_1 = assay;
        const matchValue = tryFindIndex((x) => (x.Identifier === assay_1.Identifier), removeAt(index, this$.Assays));
        if (matchValue == null) {
        }
        else {
            throw new Error(`Cannot create assay with name ${assay_1.Identifier}, as assay names must be unique and assay at index ${matchValue} has the same name.`);
        }
        this$.Assays[index] = assay;
    }
    static setAssayAt(index, assay) {
        return (study) => {
            const newStudy = study.Copy();
            newStudy.SetAssayAt(index, assay);
            return newStudy;
        };
    }
    SetAssay(assayIdentifier, assay) {
        const this$ = this;
        const index = this$.GetAssayIndex(assayIdentifier) | 0;
        this$.Assays[index] = assay;
    }
    static setAssay(assayIdentifier, assay) {
        return (study) => {
            const newStudy = study.Copy();
            newStudy.SetAssay(assayIdentifier, assay);
            return newStudy;
        };
    }
    GetAssayIndex(assayIdentifier) {
        const this$ = this;
        const index = this$.Assays.findIndex((a) => (a.Identifier === assayIdentifier)) | 0;
        if (index === -1) {
            throw new Error(`Unable to find assay with specified identifier '${assayIdentifier}'!`);
        }
        return index | 0;
    }
    static GetAssayIndex(assayIdentifier) {
        return (study) => study.GetAssayIndex(assayIdentifier);
    }
    GetAssayAt(index) {
        const this$ = this;
        return this$.Assays[index];
    }
    static getAssayAt(index) {
        return (study) => {
            const newStudy = study.Copy();
            return newStudy.GetAssayAt(index);
        };
    }
    GetAssay(assayIdentifier) {
        const this$ = this;
        const index = this$.GetAssayIndex(assayIdentifier) | 0;
        return this$.GetAssayAt(index);
    }
    static getAssay(assayIdentifier) {
        return (study) => {
            const newStudy = study.Copy();
            return newStudy.GetAssay(assayIdentifier);
        };
    }
    get TableCount() {
        const this$ = this;
        return ArcTables__get_Count(ArcTables_$ctor_Z68BECB99(this$.Tables)) | 0;
    }
    get TableNames() {
        const this$ = this;
        return ArcTables__get_TableNames(ArcTables_$ctor_Z68BECB99(this$.Tables));
    }
    AddTable(table, index) {
        const this$ = this;
        ArcTables__AddTable_16F700A1(ArcTables_$ctor_Z68BECB99(this$.Tables), table, unwrap(index));
    }
    static addTable(table, index) {
        return (study) => {
            const c = study.Copy();
            c.AddTable(table, unwrap(index));
            return c;
        };
    }
    AddTables(tables, index) {
        const this$ = this;
        ArcTables__AddTables_3601F24E(ArcTables_$ctor_Z68BECB99(this$.Tables), tables, unwrap(index));
    }
    static addTables(tables, index) {
        return (study) => {
            const c = study.Copy();
            c.AddTables(tables, unwrap(index));
            return c;
        };
    }
    InitTable(tableName, index) {
        const this$ = this;
        ArcTables__InitTable_3B406CA4(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, unwrap(index));
    }
    static initTable(tableName, index) {
        return (study) => {
            const c = study.Copy();
            c.InitTable(tableName, unwrap(index));
            return c;
        };
    }
    InitTables(tableNames, index) {
        const this$ = this;
        ArcTables__InitTables_7B28792B(ArcTables_$ctor_Z68BECB99(this$.Tables), tableNames, unwrap(index));
    }
    static initTables(tableNames, index) {
        return (study) => {
            const c = study.Copy();
            c.InitTables(tableNames, unwrap(index));
            return c;
        };
    }
    GetTableAt(index) {
        const this$ = this;
        return ArcTables__GetTableAt_Z524259A4(ArcTables_$ctor_Z68BECB99(this$.Tables), index);
    }
    static getTableAt(index) {
        return (study) => {
            const newAssay = study.Copy();
            return newAssay.GetTableAt(index);
        };
    }
    GetTable(name) {
        const this$ = this;
        return ArcTables__GetTable_Z721C83C5(ArcTables_$ctor_Z68BECB99(this$.Tables), name);
    }
    static getTable(name) {
        return (study) => {
            const newAssay = study.Copy();
            return newAssay.GetTable(name);
        };
    }
    UpdateTableAt(index, table) {
        const this$ = this;
        ArcTables__UpdateTableAt_66578202(ArcTables_$ctor_Z68BECB99(this$.Tables), index, table);
    }
    static updateTableAt(index, table) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.UpdateTableAt(index, table);
            return newAssay;
        };
    }
    UpdateTable(name, table) {
        const this$ = this;
        ArcTables__UpdateTable_4976F045(ArcTables_$ctor_Z68BECB99(this$.Tables), name, table);
    }
    static updateTable(name, table) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.UpdateTable(name, table);
            return newAssay;
        };
    }
    RemoveTableAt(index) {
        const this$ = this;
        ArcTables__RemoveTableAt_Z524259A4(ArcTables_$ctor_Z68BECB99(this$.Tables), index);
    }
    static removeTableAt(index) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.RemoveTableAt(index);
            return newAssay;
        };
    }
    RemoveTable(name) {
        const this$ = this;
        ArcTables__RemoveTable_Z721C83C5(ArcTables_$ctor_Z68BECB99(this$.Tables), name);
    }
    static removeTable(name) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.RemoveTable(name);
            return newAssay;
        };
    }
    MapTableAt(index, updateFun) {
        const this$ = this;
        ArcTables__MapTableAt_61602D68(ArcTables_$ctor_Z68BECB99(this$.Tables), index, updateFun);
    }
    static mapTableAt(index, updateFun) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.MapTableAt(index, updateFun);
            return newAssay;
        };
    }
    MapTable(name, updateFun) {
        const this$ = this;
        ArcTables__MapTable_4E415F2F(ArcTables_$ctor_Z68BECB99(this$.Tables), name, updateFun);
    }
    static mapTable(name, updateFun) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.MapTable(name, updateFun);
            return newAssay;
        };
    }
    RenameTableAt(index, newName) {
        const this$ = this;
        ArcTables__RenameTableAt_Z176EF219(ArcTables_$ctor_Z68BECB99(this$.Tables), index, newName);
    }
    static renameTableAt(index, newName) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.RenameTableAt(index, newName);
            return newAssay;
        };
    }
    RenameTable(name, newName) {
        const this$ = this;
        ArcTables__RenameTable_Z384F8060(ArcTables_$ctor_Z68BECB99(this$.Tables), name, newName);
    }
    static renameTable(name, newName) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.RenameTable(name, newName);
            return newAssay;
        };
    }
    AddColumnAt(tableIndex, header, cells, columnIndex, forceReplace) {
        const this$ = this;
        ArcTables__AddColumnAt_6647579B(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
    }
    static addColumnAt(tableIndex, header, cells, columnIndex, forceReplace) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.AddColumnAt(tableIndex, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
            return newAssay;
        };
    }
    AddColumn(tableName, header, cells, columnIndex, forceReplace) {
        const this$ = this;
        ArcTables__AddColumn_Z4FC90944(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
    }
    static addColumn(tableName, header, cells, columnIndex, forceReplace) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.AddColumn(tableName, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
            return newAssay;
        };
    }
    RemoveColumnAt(tableIndex, columnIndex) {
        const this$ = this;
        ArcTables__RemoveColumnAt_Z37302880(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, columnIndex);
    }
    static removeColumnAt(tableIndex, columnIndex) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.RemoveColumnAt(tableIndex, columnIndex);
            return newAssay;
        };
    }
    RemoveColumn(tableName, columnIndex) {
        const this$ = this;
        ArcTables__RemoveColumn_Z18115A39(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, columnIndex);
    }
    static removeColumn(tableName, columnIndex) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.RemoveColumn(tableName, columnIndex);
            return newAssay;
        };
    }
    UpdateColumnAt(tableIndex, columnIndex, header, cells) {
        const this$ = this;
        ArcTables__UpdateColumnAt_Z155350AF(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, columnIndex, header, unwrap(cells));
    }
    static updateColumnAt(tableIndex, columnIndex, header, cells) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.UpdateColumnAt(tableIndex, columnIndex, header, unwrap(cells));
            return newAssay;
        };
    }
    UpdateColumn(tableName, columnIndex, header, cells) {
        const this$ = this;
        ArcTables__UpdateColumn_Z774BF72A(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, columnIndex, header, unwrap(cells));
    }
    static updateColumn(tableName, columnIndex, header, cells) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.UpdateColumn(tableName, columnIndex, header, unwrap(cells));
            return newAssay;
        };
    }
    GetColumnAt(tableIndex, columnIndex) {
        const this$ = this;
        return ArcTables__GetColumnAt_Z37302880(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, columnIndex);
    }
    static getColumnAt(tableIndex, columnIndex) {
        return (study) => {
            const newAssay = study.Copy();
            return newAssay.GetColumnAt(tableIndex, columnIndex);
        };
    }
    GetColumn(tableName, columnIndex) {
        const this$ = this;
        return ArcTables__GetColumn_Z18115A39(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, columnIndex);
    }
    static getColumn(tableName, columnIndex) {
        return (study) => {
            const newAssay = study.Copy();
            return newAssay.GetColumn(tableName, columnIndex);
        };
    }
    AddRowAt(tableIndex, cells, rowIndex) {
        const this$ = this;
        ArcTables__AddRowAt_Z57F91678(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, unwrap(cells), unwrap(rowIndex));
    }
    static addRowAt(tableIndex, cells, rowIndex) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.AddRowAt(tableIndex, unwrap(cells), unwrap(rowIndex));
            return newAssay;
        };
    }
    AddRow(tableName, cells, rowIndex) {
        const this$ = this;
        ArcTables__AddRow_1177C4AF(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, unwrap(cells), unwrap(rowIndex));
    }
    static addRow(tableName, cells, rowIndex) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.AddRow(tableName, unwrap(cells), unwrap(rowIndex));
            return newAssay;
        };
    }
    RemoveRowAt(tableIndex, rowIndex) {
        const this$ = this;
        ArcTables__RemoveRowAt_Z37302880(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, rowIndex);
    }
    static removeRowAt(tableIndex, rowIndex) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.RemoveColumnAt(tableIndex, rowIndex);
            return newAssay;
        };
    }
    RemoveRow(tableName, rowIndex) {
        const this$ = this;
        ArcTables__RemoveRow_Z18115A39(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, rowIndex);
    }
    static removeRow(tableName, rowIndex) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.RemoveRow(tableName, rowIndex);
            return newAssay;
        };
    }
    UpdateRowAt(tableIndex, rowIndex, cells) {
        const this$ = this;
        ArcTables__UpdateRowAt_Z596C2D98(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, rowIndex, cells);
    }
    static updateRowAt(tableIndex, rowIndex, cells) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.UpdateRowAt(tableIndex, rowIndex, cells);
            return newAssay;
        };
    }
    UpdateRow(tableName, rowIndex, cells) {
        const this$ = this;
        ArcTables__UpdateRow_Z5E65B4B1(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, rowIndex, cells);
    }
    static updateRow(tableName, rowIndex, cells) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.UpdateRow(tableName, rowIndex, cells);
            return newAssay;
        };
    }
    GetRowAt(tableIndex, rowIndex) {
        const this$ = this;
        return ArcTables__GetRowAt_Z37302880(ArcTables_$ctor_Z68BECB99(this$.Tables), tableIndex, rowIndex);
    }
    static getRowAt(tableIndex, rowIndex) {
        return (study) => {
            const newAssay = study.Copy();
            return newAssay.GetRowAt(tableIndex, rowIndex);
        };
    }
    GetRow(tableName, rowIndex) {
        const this$ = this;
        return ArcTables__GetRow_Z18115A39(ArcTables_$ctor_Z68BECB99(this$.Tables), tableName, rowIndex);
    }
    static getRow(tableName, rowIndex) {
        return (study) => {
            const newAssay = study.Copy();
            return newAssay.GetRow(tableName, rowIndex);
        };
    }
    Copy() {
        const this$ = this;
        const newTables = [];
        const newAssays = [];
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
        let enumerator_1 = getEnumerator(this$.Assays);
        try {
            while (enumerator_1["System.Collections.IEnumerator.MoveNext"]()) {
                const study = enumerator_1["System.Collections.Generic.IEnumerator`1.get_Current"]();
                const copy_1 = study.Copy();
                void (newAssays.push(copy_1));
            }
        }
        finally {
            disposeSafe(enumerator_1);
        }
        return new ArcStudy(this$.Identifier, unwrap(this$.Title), unwrap(this$.Description), unwrap(this$.submissionDate), unwrap(this$.PublicReleaseDate), this$.Publications, this$.Contacts, this$.StudyDesignDescriptors, newTables, newAssays, this$.Factors, this$.Comments);
    }
    ToStudy() {
        const this$ = this;
        const processSeq = ArcTables__GetProcesses(ArcTables_$ctor_Z68BECB99(this$.Tables));
        const protocols = fromValueWithDefault(empty(), getProtocols(processSeq));
        const assays = fromValueWithDefault(empty(), map_1((a) => a.ToAssay(), toList(this$.Assays)));
        let studyMaterials;
        const v_5 = StudyMaterials_create_Z460D555F(unwrap(fromValueWithDefault(empty(), getSources(processSeq))), unwrap(fromValueWithDefault(empty(), getSamples(processSeq))), unwrap(fromValueWithDefault(empty(), getMaterials(processSeq))));
        studyMaterials = fromValueWithDefault(StudyMaterials_get_empty(), v_5);
        const patternInput = isMissingIdentifier(this$.Identifier) ? [void 0, void 0] : [this$.Identifier, Study_fileNameFromIdentifier(this$.Identifier)];
        return Study_create_Z6C8AB268(void 0, unwrap(patternInput[1]), unwrap(patternInput[0]), unwrap(this$.Title), unwrap(this$.Description), unwrap(this$.SubmissionDate), unwrap(this$.PublicReleaseDate), unwrap(fromValueWithDefault(empty(), this$.Publications)), unwrap(fromValueWithDefault(empty(), this$.Contacts)), unwrap(fromValueWithDefault(empty(), this$.StudyDesignDescriptors)), unwrap(protocols), unwrap(studyMaterials), unwrap(fromValueWithDefault(empty(), processSeq)), unwrap(assays), unwrap(fromValueWithDefault(empty(), this$.Factors)), unwrap(fromValueWithDefault(empty(), getCharacteristics(processSeq))), unwrap(fromValueWithDefault(empty(), getUnits(processSeq))), unwrap(fromValueWithDefault(empty(), this$.Comments)));
    }
    static fromStudy(s) {
        const tables = map_2((arg_1) => ArcTables__get_Tables(ArcTables_fromProcesses_Z31821267(arg_1)), s.ProcessSequence);
        let identifer;
        const matchValue = s.FileName;
        identifer = ((matchValue == null) ? createMissingIdentifier() : Study_identifierFromFileName(matchValue));
        const assays = map_2((arg_4) => {
            const arg_3 = map_1((arg_2) => ArcAssay.fromAssay(arg_2), arg_4);
            return Array.from(arg_3);
        }, s.Assays);
        return ArcStudy.create(identifer, unwrap(s.Title), unwrap(s.Description), unwrap(s.SubmissionDate), unwrap(s.PublicReleaseDate), unwrap(s.Publications), unwrap(s.Contacts), unwrap(s.StudyDesignDescriptors), unwrap(tables), unwrap(assays), unwrap(s.Factors), unwrap(s.Comments));
    }
}

export function ArcStudy_$reflection() {
    return class_type("ISA.ArcStudy", void 0, ArcStudy);
}

export function ArcStudy_$ctor_Z337E69A6(identifier, title, description, submissionDate, publicReleaseDate, publications, contacts, studyDesignDescriptors, tables, assays, factors, comments) {
    return new ArcStudy(identifier, title, description, submissionDate, publicReleaseDate, publications, contacts, studyDesignDescriptors, tables, assays, factors, comments);
}

