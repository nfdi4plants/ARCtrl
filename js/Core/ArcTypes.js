import { value as value_4, defaultArg, map, unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { collect, append as append_3, tryFind, removeAt, iterate, tryFindIndex, exists, choose, item, map as map_1, delay, toList, fold, length, forAll, contains, filter, toArray } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { defaultOf, safeHash, equals, disposeSafe, getEnumerator, stringHash } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { ResizeArray_choose, ResizeArray_filter, ResizeArray_map } from "./Helper/Collections.js";
import { DataMap__Copy } from "./DataMap.js";
import { addRangeInPlace, removeInPlace, contains as contains_1 } from "../fable_modules/fable-library-js.4.22.0/Array.js";
import { printf, toText } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { rangeDouble } from "../fable_modules/fable-library-js.4.22.0/Range.js";
import { boxHashSeq, boxHashOption, boxHashArray } from "./Helper/HashCodes.js";
import { checkValidCharacters } from "./Helper/Identifier.js";
import { ArcTablesAux_applyIOMap, ArcTablesAux_getIOMap, ArcTables_$reflection, ArcTables } from "./Table/ArcTables.js";
import { class_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { Array_distinct } from "../fable_modules/fable-library-js.4.22.0/Seq2.js";
import { FSharpRef } from "../fable_modules/fable-library-js.4.22.0/Types.js";

export class ArcAssay extends ArcTables {
    constructor(identifier, measurementType, technologyType, technologyPlatform, tables, datamap, performers, comments) {
        super(defaultArg(tables, []));
        let identifier_1;
        const performers_1 = defaultArg(performers, []);
        const comments_1 = defaultArg(comments, []);
        this["identifier@109"] = ((identifier_1 = identifier.trim(), (checkValidCharacters(identifier_1), identifier_1)));
        this.investigation = undefined;
        this["measurementType@114"] = measurementType;
        this["technologyType@115"] = technologyType;
        this["technologyPlatform@116"] = technologyPlatform;
        this.dataMap = datamap;
        this["performers@118-1"] = performers_1;
        this["comments@119-1"] = comments_1;
        this.staticHash = 0;
    }
    get Identifier() {
        const this$ = this;
        return this$["identifier@109"];
    }
    set Identifier(i) {
        const this$ = this;
        this$["identifier@109"] = i;
    }
    get Investigation() {
        const this$ = this;
        return unwrap(this$.investigation);
    }
    set Investigation(i) {
        const this$ = this;
        this$.investigation = i;
    }
    get MeasurementType() {
        const this$ = this;
        return unwrap(this$["measurementType@114"]);
    }
    set MeasurementType(n) {
        const this$ = this;
        this$["measurementType@114"] = n;
    }
    get TechnologyType() {
        const this$ = this;
        return unwrap(this$["technologyType@115"]);
    }
    set TechnologyType(n) {
        const this$ = this;
        this$["technologyType@115"] = n;
    }
    get TechnologyPlatform() {
        const this$ = this;
        return unwrap(this$["technologyPlatform@116"]);
    }
    set TechnologyPlatform(n) {
        const this$ = this;
        this$["technologyPlatform@116"] = n;
    }
    get DataMap() {
        const this$ = this;
        return unwrap(this$.dataMap);
    }
    set DataMap(n) {
        const this$ = this;
        this$.dataMap = n;
    }
    get Performers() {
        const this$ = this;
        return this$["performers@118-1"];
    }
    set Performers(n) {
        const this$ = this;
        this$["performers@118-1"] = n;
    }
    get Comments() {
        const this$ = this;
        return this$["comments@119-1"];
    }
    set Comments(n) {
        const this$ = this;
        this$["comments@119-1"] = n;
    }
    get StaticHash() {
        const this$ = this;
        return this$.staticHash | 0;
    }
    set StaticHash(h) {
        const this$ = this;
        this$.staticHash = (h | 0);
    }
    static init(identifier) {
        return new ArcAssay(identifier);
    }
    static create(identifier, measurementType, technologyType, technologyPlatform, tables, datamap, performers, comments) {
        return new ArcAssay(identifier, unwrap(measurementType), unwrap(technologyType), unwrap(technologyPlatform), unwrap(tables), unwrap(datamap), unwrap(performers), unwrap(comments));
    }
    static make(identifier, measurementType, technologyType, technologyPlatform, tables, datamap, performers, comments) {
        return new ArcAssay(identifier, unwrap(measurementType), unwrap(technologyType), unwrap(technologyPlatform), tables, unwrap(datamap), performers, comments);
    }
    static get FileName() {
        return "isa.assay.xlsx";
    }
    get StudiesRegisteredIn() {
        const this$ = this;
        const matchValue = this$.Investigation;
        if (matchValue == null) {
            return [];
        }
        else {
            const i = matchValue;
            return toArray(filter((s) => {
                const source = s.RegisteredAssayIdentifiers;
                return contains(this$.Identifier, source, {
                    Equals: (x, y) => (x === y),
                    GetHashCode: stringHash,
                });
            }, i.Studies));
        }
    }
    static addTable(table, index) {
        return (assay) => {
            const c = assay.Copy();
            c.AddTable(table, unwrap(index));
            return c;
        };
    }
    static addTables(tables, index) {
        return (assay) => {
            const c = assay.Copy();
            c.AddTables(tables, unwrap(index));
            return c;
        };
    }
    static initTable(tableName, index) {
        return (assay) => {
            const c = assay.Copy();
            return [c, c.InitTable(tableName, unwrap(index))];
        };
    }
    static initTables(tableNames, index) {
        return (assay) => {
            const c = assay.Copy();
            c.InitTables(tableNames, unwrap(index));
            return c;
        };
    }
    static getTableAt(index) {
        return (assay) => {
            const newAssay = assay.Copy();
            return newAssay.GetTableAt(index);
        };
    }
    static getTable(name) {
        return (assay) => {
            const newAssay = assay.Copy();
            return newAssay.GetTable(name);
        };
    }
    static updateTableAt(index, table) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.UpdateTableAt(index, table);
            return newAssay;
        };
    }
    static updateTable(name, table) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.UpdateTable(name, table);
            return newAssay;
        };
    }
    static setTableAt(index, table) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.SetTableAt(index, table);
            return newAssay;
        };
    }
    static setTable(name, table) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.SetTable(name, table);
            return newAssay;
        };
    }
    static removeTableAt(index) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.RemoveTableAt(index);
            return newAssay;
        };
    }
    static removeTable(name) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.RemoveTable(name);
            return newAssay;
        };
    }
    static mapTableAt(index, updateFun) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.MapTableAt(index, updateFun);
            return newAssay;
        };
    }
    static updateTable(name, updateFun) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.MapTable(name, updateFun);
            return newAssay;
        };
    }
    static renameTableAt(index, newName) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.RenameTableAt(index, newName);
            return newAssay;
        };
    }
    static renameTable(name, newName) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.RenameTable(name, newName);
            return newAssay;
        };
    }
    static addColumnAt(tableIndex, header, cells, columnIndex, forceReplace) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.AddColumnAt(tableIndex, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
            return newAssay;
        };
    }
    static addColumn(tableName, header, cells, columnIndex, forceReplace) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.AddColumn(tableName, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
            return newAssay;
        };
    }
    static removeColumnAt(tableIndex, columnIndex) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.RemoveColumnAt(tableIndex, columnIndex);
            return newAssay;
        };
    }
    static removeColumn(tableName, columnIndex) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.RemoveColumn(tableName, columnIndex);
            return newAssay;
        };
    }
    static updateColumnAt(tableIndex, columnIndex, header, cells) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.UpdateColumnAt(tableIndex, columnIndex, header, unwrap(cells));
            return newAssay;
        };
    }
    static updateColumn(tableName, columnIndex, header, cells) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.UpdateColumn(tableName, columnIndex, header, unwrap(cells));
            return newAssay;
        };
    }
    static getColumnAt(tableIndex, columnIndex) {
        return (assay) => {
            const newAssay = assay.Copy();
            return newAssay.GetColumnAt(tableIndex, columnIndex);
        };
    }
    static getColumn(tableName, columnIndex) {
        return (assay) => {
            const newAssay = assay.Copy();
            return newAssay.GetColumn(tableName, columnIndex);
        };
    }
    static addRowAt(tableIndex, cells, rowIndex) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.AddRowAt(tableIndex, unwrap(cells), unwrap(rowIndex));
            return newAssay;
        };
    }
    static addRow(tableName, cells, rowIndex) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.AddRow(tableName, unwrap(cells), unwrap(rowIndex));
            return newAssay;
        };
    }
    static removeRowAt(tableIndex, rowIndex) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.RemoveColumnAt(tableIndex, rowIndex);
            return newAssay;
        };
    }
    static removeRow(tableName, rowIndex) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.RemoveRow(tableName, rowIndex);
            return newAssay;
        };
    }
    static updateRowAt(tableIndex, rowIndex, cells) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.UpdateRowAt(tableIndex, rowIndex, cells);
            return newAssay;
        };
    }
    static updateRow(tableName, rowIndex, cells) {
        return (assay) => {
            const newAssay = assay.Copy();
            newAssay.UpdateRow(tableName, rowIndex, cells);
            return newAssay;
        };
    }
    static getRowAt(tableIndex, rowIndex) {
        return (assay) => {
            const newAssay = assay.Copy();
            return newAssay.GetRowAt(tableIndex, rowIndex);
        };
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
        const nextTables = ResizeArray_map((c) => c.Copy(), this$.Tables);
        const nextComments = ResizeArray_map((c_1) => c_1.Copy(), this$.Comments);
        const nextDataMap = map(DataMap__Copy, this$.DataMap);
        const nextPerformers = ResizeArray_map((c_2) => c_2.Copy(), this$.Performers);
        const identifier = this$.Identifier;
        const measurementType = this$.MeasurementType;
        const technologyType = this$.TechnologyType;
        const technologyPlatform = this$.TechnologyPlatform;
        return ArcAssay.make(identifier, measurementType, technologyType, technologyPlatform, nextTables, nextDataMap, nextPerformers, nextComments);
    }
    UpdateBy(assay, onlyReplaceExisting, appendSequences) {
        const this$ = this;
        const onlyReplaceExisting_1 = defaultArg(onlyReplaceExisting, false);
        const appendSequences_1 = defaultArg(appendSequences, false);
        const updateAlways = !onlyReplaceExisting_1;
        if ((assay.MeasurementType != null) ? true : updateAlways) {
            this$.MeasurementType = assay.MeasurementType;
        }
        if ((assay.TechnologyType != null) ? true : updateAlways) {
            this$.TechnologyType = assay.TechnologyType;
        }
        if ((assay.TechnologyPlatform != null) ? true : updateAlways) {
            this$.TechnologyPlatform = assay.TechnologyPlatform;
        }
        if ((assay.Tables.length !== 0) ? true : updateAlways) {
            let s;
            const origin = this$.Tables;
            const next = assay.Tables;
            if (!appendSequences_1) {
                s = ResizeArray_map((x) => x, next);
            }
            else {
                const combined = [];
                let enumerator = getEnumerator(origin);
                try {
                    while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                        const e = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                        if (!contains_1(e, combined, {
                            Equals: equals,
                            GetHashCode: safeHash,
                        })) {
                            void (combined.push(e));
                        }
                    }
                }
                finally {
                    disposeSafe(enumerator);
                }
                let enumerator_1 = getEnumerator(next);
                try {
                    while (enumerator_1["System.Collections.IEnumerator.MoveNext"]()) {
                        const e_1 = enumerator_1["System.Collections.Generic.IEnumerator`1.get_Current"]();
                        if (!contains_1(e_1, combined, {
                            Equals: equals,
                            GetHashCode: safeHash,
                        })) {
                            void (combined.push(e_1));
                        }
                    }
                }
                finally {
                    disposeSafe(enumerator_1);
                }
                s = combined;
            }
            this$.Tables = s;
        }
        if ((assay.Performers.length !== 0) ? true : updateAlways) {
            let s_1;
            const origin_1 = this$.Performers;
            const next_1 = assay.Performers;
            if (!appendSequences_1) {
                s_1 = ResizeArray_map((x_3) => x_3, next_1);
            }
            else {
                const combined_1 = [];
                let enumerator_2 = getEnumerator(origin_1);
                try {
                    while (enumerator_2["System.Collections.IEnumerator.MoveNext"]()) {
                        const e_2 = enumerator_2["System.Collections.Generic.IEnumerator`1.get_Current"]();
                        if (!contains_1(e_2, combined_1, {
                            Equals: equals,
                            GetHashCode: safeHash,
                        })) {
                            void (combined_1.push(e_2));
                        }
                    }
                }
                finally {
                    disposeSafe(enumerator_2);
                }
                let enumerator_1_1 = getEnumerator(next_1);
                try {
                    while (enumerator_1_1["System.Collections.IEnumerator.MoveNext"]()) {
                        const e_1_1 = enumerator_1_1["System.Collections.Generic.IEnumerator`1.get_Current"]();
                        if (!contains_1(e_1_1, combined_1, {
                            Equals: equals,
                            GetHashCode: safeHash,
                        })) {
                            void (combined_1.push(e_1_1));
                        }
                    }
                }
                finally {
                    disposeSafe(enumerator_1_1);
                }
                s_1 = combined_1;
            }
            this$.Performers = s_1;
        }
        if ((assay.Comments.length !== 0) ? true : updateAlways) {
            let s_2;
            const origin_2 = this$.Comments;
            const next_2 = assay.Comments;
            if (!appendSequences_1) {
                s_2 = ResizeArray_map((x_6) => x_6, next_2);
            }
            else {
                const combined_2 = [];
                let enumerator_3 = getEnumerator(origin_2);
                try {
                    while (enumerator_3["System.Collections.IEnumerator.MoveNext"]()) {
                        const e_3 = enumerator_3["System.Collections.Generic.IEnumerator`1.get_Current"]();
                        if (!contains_1(e_3, combined_2, {
                            Equals: equals,
                            GetHashCode: safeHash,
                        })) {
                            void (combined_2.push(e_3));
                        }
                    }
                }
                finally {
                    disposeSafe(enumerator_3);
                }
                let enumerator_1_2 = getEnumerator(next_2);
                try {
                    while (enumerator_1_2["System.Collections.IEnumerator.MoveNext"]()) {
                        const e_1_2 = enumerator_1_2["System.Collections.Generic.IEnumerator`1.get_Current"]();
                        if (!contains_1(e_1_2, combined_2, {
                            Equals: equals,
                            GetHashCode: safeHash,
                        })) {
                            void (combined_2.push(e_1_2));
                        }
                    }
                }
                finally {
                    disposeSafe(enumerator_1_2);
                }
                s_2 = combined_2;
            }
            this$.Comments = s_2;
        }
    }
    toString() {
        const this$ = this;
        const arg = this$.Identifier;
        const arg_1 = this$.MeasurementType;
        const arg_2 = this$.TechnologyType;
        const arg_3 = this$.TechnologyPlatform;
        const arg_4 = this$.Tables;
        const arg_5 = this$.Performers;
        const arg_6 = this$.Comments;
        return toText(printf("ArcAssay({\r\n    Identifier = \"%s\",\r\n    MeasurementType = %A,\r\n    TechnologyType = %A,\r\n    TechnologyPlatform = %A,\r\n    Tables = %A,\r\n    Performers = %A,\r\n    Comments = %A\r\n})"))(arg)(arg_1)(arg_2)(arg_3)(arg_4)(arg_5)(arg_6);
    }
    AddToInvestigation(investigation) {
        const this$ = this;
        this$.Investigation = investigation;
    }
    RemoveFromInvestigation() {
        const this$ = this;
        this$.Investigation = undefined;
    }
    UpdateReferenceByAssayFile(assay, onlyReplaceExisting) {
        const this$ = this;
        const updateAlways = !defaultArg(onlyReplaceExisting, false);
        if ((assay.MeasurementType != null) ? true : updateAlways) {
            this$.MeasurementType = assay.MeasurementType;
        }
        if ((assay.TechnologyPlatform != null) ? true : updateAlways) {
            this$.TechnologyPlatform = assay.TechnologyPlatform;
        }
        if ((assay.TechnologyType != null) ? true : updateAlways) {
            this$.TechnologyType = assay.TechnologyType;
        }
        if ((assay.Tables.length !== 0) ? true : updateAlways) {
            this$.Tables = assay.Tables;
        }
        if ((assay.Comments.length !== 0) ? true : updateAlways) {
            this$.Comments = assay.Comments;
        }
        this$.DataMap = assay.DataMap;
        if ((assay.Performers.length !== 0) ? true : updateAlways) {
            this$.Performers = assay.Performers;
        }
    }
    StructurallyEquals(other) {
        let a, b, a_1, b_1, a_2, b_2;
        const this$ = this;
        return forAll((x) => (x === true), [this$.Identifier === other.Identifier, equals(this$.MeasurementType, other.MeasurementType), equals(this$.TechnologyType, other.TechnologyType), equals(this$.TechnologyPlatform, other.TechnologyPlatform), equals(this$.DataMap, other.DataMap), (a = this$.Tables, (b = other.Tables, (length(a) === length(b)) && fold((acc, e) => {
            if (acc) {
                return e;
            }
            else {
                return false;
            }
        }, true, toList(delay(() => map_1((i_1) => equals(item(i_1, a), item(i_1, b)), rangeDouble(0, 1, length(a) - 1))))))), (a_1 = this$.Performers, (b_1 = other.Performers, (length(a_1) === length(b_1)) && fold((acc_1, e_1) => {
            if (acc_1) {
                return e_1;
            }
            else {
                return false;
            }
        }, true, toList(delay(() => map_1((i_2) => equals(item(i_2, a_1), item(i_2, b_1)), rangeDouble(0, 1, length(a_1) - 1))))))), (a_2 = this$.Comments, (b_2 = other.Comments, (length(a_2) === length(b_2)) && fold((acc_2, e_2) => {
            if (acc_2) {
                return e_2;
            }
            else {
                return false;
            }
        }, true, toList(delay(() => map_1((i_3) => equals(item(i_3, a_2), item(i_3, b_2)), rangeDouble(0, 1, length(a_2) - 1)))))))]);
    }
    ReferenceEquals(other) {
        const this$ = this;
        return this$ === other;
    }
    Equals(other) {
        let assay;
        const this$ = this;
        return (other instanceof ArcAssay) && ((assay = other, this$.StructurallyEquals(assay)));
    }
    GetLightHashCode() {
        const this$ = this;
        return boxHashArray([this$.Identifier, boxHashOption(this$.MeasurementType), boxHashOption(this$.TechnologyType), boxHashOption(this$.TechnologyPlatform), boxHashSeq(this$.Tables), boxHashSeq(this$.Performers), boxHashSeq(this$.Comments)]) | 0;
    }
    GetHashCode() {
        const this$ = this;
        return boxHashArray([this$.Identifier, boxHashOption(this$.MeasurementType), boxHashOption(this$.TechnologyType), boxHashOption(this$.TechnologyPlatform), boxHashOption(this$.DataMap), boxHashSeq(this$.Tables), boxHashSeq(this$.Performers), boxHashSeq(this$.Comments)]) | 0;
    }
}

export function ArcAssay_$reflection() {
    return class_type("ARCtrl.ArcAssay", undefined, ArcAssay, ArcTables_$reflection());
}

export function ArcAssay_$ctor_Z4900C8CC(identifier, measurementType, technologyType, technologyPlatform, tables, datamap, performers, comments) {
    return new ArcAssay(identifier, measurementType, technologyType, technologyPlatform, tables, datamap, performers, comments);
}

export class ArcStudy extends ArcTables {
    constructor(identifier, title, description, submissionDate, publicReleaseDate, publications, contacts, studyDesignDescriptors, tables, datamap, registeredAssayIdentifiers, comments) {
        super(defaultArg(tables, []));
        let identifier_1;
        const publications_1 = defaultArg(publications, []);
        const contacts_1 = defaultArg(contacts, []);
        const studyDesignDescriptors_1 = defaultArg(studyDesignDescriptors, []);
        const registeredAssayIdentifiers_1 = defaultArg(registeredAssayIdentifiers, []);
        const comments_1 = defaultArg(comments, []);
        this["identifier@533"] = ((identifier_1 = identifier.trim(), (checkValidCharacters(identifier_1), identifier_1)));
        this.investigation = undefined;
        this["title@538"] = title;
        this["description@539"] = description;
        this["submissionDate@540"] = submissionDate;
        this["publicReleaseDate@541"] = publicReleaseDate;
        this["publications@542-1"] = publications_1;
        this["contacts@543-1"] = contacts_1;
        this["studyDesignDescriptors@544-1"] = studyDesignDescriptors_1;
        this["datamap@545"] = datamap;
        this["registeredAssayIdentifiers@546-1"] = registeredAssayIdentifiers_1;
        this["comments@547-1"] = comments_1;
        this.staticHash = 0;
    }
    get Identifier() {
        const this$ = this;
        return this$["identifier@533"];
    }
    set Identifier(i) {
        const this$ = this;
        this$["identifier@533"] = i;
    }
    get Investigation() {
        const this$ = this;
        return unwrap(this$.investigation);
    }
    set Investigation(i) {
        const this$ = this;
        this$.investigation = i;
    }
    get Title() {
        const this$ = this;
        return unwrap(this$["title@538"]);
    }
    set Title(n) {
        const this$ = this;
        this$["title@538"] = n;
    }
    get Description() {
        const this$ = this;
        return unwrap(this$["description@539"]);
    }
    set Description(n) {
        const this$ = this;
        this$["description@539"] = n;
    }
    get SubmissionDate() {
        const this$ = this;
        return unwrap(this$["submissionDate@540"]);
    }
    set SubmissionDate(n) {
        const this$ = this;
        this$["submissionDate@540"] = n;
    }
    get PublicReleaseDate() {
        const this$ = this;
        return unwrap(this$["publicReleaseDate@541"]);
    }
    set PublicReleaseDate(n) {
        const this$ = this;
        this$["publicReleaseDate@541"] = n;
    }
    get Publications() {
        const this$ = this;
        return this$["publications@542-1"];
    }
    set Publications(n) {
        const this$ = this;
        this$["publications@542-1"] = n;
    }
    get Contacts() {
        const this$ = this;
        return this$["contacts@543-1"];
    }
    set Contacts(n) {
        const this$ = this;
        this$["contacts@543-1"] = n;
    }
    get StudyDesignDescriptors() {
        const this$ = this;
        return this$["studyDesignDescriptors@544-1"];
    }
    set StudyDesignDescriptors(n) {
        const this$ = this;
        this$["studyDesignDescriptors@544-1"] = n;
    }
    get DataMap() {
        const this$ = this;
        return unwrap(this$["datamap@545"]);
    }
    set DataMap(n) {
        const this$ = this;
        this$["datamap@545"] = n;
    }
    get RegisteredAssayIdentifiers() {
        const this$ = this;
        return this$["registeredAssayIdentifiers@546-1"];
    }
    set RegisteredAssayIdentifiers(n) {
        const this$ = this;
        this$["registeredAssayIdentifiers@546-1"] = n;
    }
    get Comments() {
        const this$ = this;
        return this$["comments@547-1"];
    }
    set Comments(n) {
        const this$ = this;
        this$["comments@547-1"] = n;
    }
    get StaticHash() {
        const this$ = this;
        return this$.staticHash | 0;
    }
    set StaticHash(h) {
        const this$ = this;
        this$.staticHash = (h | 0);
    }
    static init(identifier) {
        return new ArcStudy(identifier);
    }
    static create(identifier, title, description, submissionDate, publicReleaseDate, publications, contacts, studyDesignDescriptors, tables, datamap, registeredAssayIdentifiers, comments) {
        return new ArcStudy(identifier, unwrap(title), unwrap(description), unwrap(submissionDate), unwrap(publicReleaseDate), unwrap(publications), unwrap(contacts), unwrap(studyDesignDescriptors), unwrap(tables), unwrap(datamap), unwrap(registeredAssayIdentifiers), unwrap(comments));
    }
    static make(identifier, title, description, submissionDate, publicReleaseDate, publications, contacts, studyDesignDescriptors, tables, datamap, registeredAssayIdentifiers, comments) {
        return new ArcStudy(identifier, unwrap(title), unwrap(description), unwrap(submissionDate), unwrap(publicReleaseDate), publications, contacts, studyDesignDescriptors, tables, unwrap(datamap), registeredAssayIdentifiers, comments);
    }
    get isEmpty() {
        const this$ = this;
        return ((((((((equals(this$.Title, undefined) && equals(this$.Description, undefined)) && equals(this$.SubmissionDate, undefined)) && equals(this$.PublicReleaseDate, undefined)) && (this$.Publications.length === 0)) && (this$.Contacts.length === 0)) && (this$.StudyDesignDescriptors.length === 0)) && (this$.Tables.length === 0)) && (this$.RegisteredAssayIdentifiers.length === 0)) && (this$.Comments.length === 0);
    }
    static get FileName() {
        return "isa.study.xlsx";
    }
    get RegisteredAssayIdentifierCount() {
        const this$ = this;
        return this$.RegisteredAssayIdentifiers.length | 0;
    }
    get RegisteredAssayCount() {
        const this$ = this;
        return this$.RegisteredAssays.length | 0;
    }
    get RegisteredAssays() {
        const this$ = this;
        let inv;
        const investigation = this$.Investigation;
        if (investigation != null) {
            inv = investigation;
        }
        else {
            throw new Error("Cannot execute this function. Object is not part of ArcInvestigation.");
        }
        const collection = choose((assayIdentifier) => inv.TryGetAssay(assayIdentifier), this$.RegisteredAssayIdentifiers);
        return Array.from(collection);
    }
    get VacantAssayIdentifiers() {
        const this$ = this;
        let inv;
        const investigation = this$.Investigation;
        if (investigation != null) {
            inv = investigation;
        }
        else {
            throw new Error("Cannot execute this function. Object is not part of ArcInvestigation.");
        }
        const collection = filter((arg) => !inv.ContainsAssay(arg), this$.RegisteredAssayIdentifiers);
        return Array.from(collection);
    }
    AddRegisteredAssay(assay) {
        const this$ = this;
        let inv;
        const investigation = this$.Investigation;
        if (investigation != null) {
            inv = investigation;
        }
        else {
            throw new Error("Cannot execute this function. Object is not part of ArcInvestigation.");
        }
        inv.AddAssay(assay);
        inv.RegisterAssay(this$.Identifier, assay.Identifier);
    }
    static addRegisteredAssay(assay) {
        return (study) => {
            const newStudy = study.Copy();
            newStudy.AddRegisteredAssay(assay);
            return newStudy;
        };
    }
    InitRegisteredAssay(assayIdentifier) {
        const this$ = this;
        const assay = new ArcAssay(assayIdentifier);
        this$.AddRegisteredAssay(assay);
        return assay;
    }
    static initRegisteredAssay(assayIdentifier) {
        return (study) => {
            const copy = study.Copy();
            return [copy, copy.InitRegisteredAssay(assayIdentifier)];
        };
    }
    RegisterAssay(assayIdentifier) {
        const this$ = this;
        if (contains(assayIdentifier, this$.RegisteredAssayIdentifiers, {
            Equals: (x, y) => (x === y),
            GetHashCode: stringHash,
        })) {
            throw new Error(`Assay \`${assayIdentifier}\` is already registered on the study.`);
        }
        void (this$.RegisteredAssayIdentifiers.push(assayIdentifier));
    }
    static registerAssay(assayIdentifier) {
        return (study) => {
            const copy = study.Copy();
            copy.RegisterAssay(assayIdentifier);
            return copy;
        };
    }
    DeregisterAssay(assayIdentifier) {
        const this$ = this;
        removeInPlace(assayIdentifier, this$.RegisteredAssayIdentifiers, {
            Equals: (x, y) => (x === y),
            GetHashCode: stringHash,
        });
    }
    static deregisterAssay(assayIdentifier) {
        return (study) => {
            const copy = study.Copy();
            copy.DeregisterAssay(assayIdentifier);
            return copy;
        };
    }
    GetRegisteredAssay(assayIdentifier) {
        const this$ = this;
        if (!contains(assayIdentifier, this$.RegisteredAssayIdentifiers, {
            Equals: (x, y) => (x === y),
            GetHashCode: stringHash,
        })) {
            throw new Error(`Assay \`${assayIdentifier}\` is not registered on the study.`);
        }
        let inv;
        const investigation = this$.Investigation;
        if (investigation != null) {
            inv = investigation;
        }
        else {
            throw new Error("Cannot execute this function. Object is not part of ArcInvestigation.");
        }
        return inv.GetAssay(assayIdentifier);
    }
    static getRegisteredAssay(assayIdentifier) {
        return (study) => {
            const copy = study.Copy();
            return copy.GetRegisteredAssay(assayIdentifier);
        };
    }
    static getRegisteredAssays() {
        return (study) => {
            const copy = study.Copy();
            return copy.RegisteredAssays;
        };
    }
    GetRegisteredAssaysOrIdentifier() {
        const this$ = this;
        const matchValue = this$.Investigation;
        if (matchValue == null) {
            return ResizeArray_map((identifier_1) => ArcAssay.init(identifier_1), this$.RegisteredAssayIdentifiers);
        }
        else {
            const i = matchValue;
            return ResizeArray_map((identifier) => {
                const matchValue_1 = i.TryGetAssay(identifier);
                if (matchValue_1 == null) {
                    return ArcAssay.init(identifier);
                }
                else {
                    return matchValue_1;
                }
            }, this$.RegisteredAssayIdentifiers);
        }
    }
    static getRegisteredAssaysOrIdentifier() {
        return (study) => {
            const copy = study.Copy();
            return copy.GetRegisteredAssaysOrIdentifier();
        };
    }
    static addTable(table, index) {
        return (study) => {
            const c = study.Copy();
            c.AddTable(table, unwrap(index));
            return c;
        };
    }
    static addTables(tables, index) {
        return (study) => {
            const c = study.Copy();
            c.AddTables(tables, unwrap(index));
            return c;
        };
    }
    static initTable(tableName, index) {
        return (study) => {
            const c = study.Copy();
            return [c, c.InitTable(tableName, unwrap(index))];
        };
    }
    static initTables(tableNames, index) {
        return (study) => {
            const c = study.Copy();
            c.InitTables(tableNames, unwrap(index));
            return c;
        };
    }
    static getTableAt(index) {
        return (study) => {
            const newAssay = study.Copy();
            return newAssay.GetTableAt(index);
        };
    }
    static getTable(name) {
        return (study) => {
            const newAssay = study.Copy();
            return newAssay.GetTable(name);
        };
    }
    static updateTableAt(index, table) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.UpdateTableAt(index, table);
            return newAssay;
        };
    }
    static updateTable(name, table) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.UpdateTable(name, table);
            return newAssay;
        };
    }
    static setTableAt(index, table) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.SetTableAt(index, table);
            return newAssay;
        };
    }
    static setTable(name, table) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.SetTable(name, table);
            return newAssay;
        };
    }
    static removeTableAt(index) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.RemoveTableAt(index);
            return newAssay;
        };
    }
    static removeTable(name) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.RemoveTable(name);
            return newAssay;
        };
    }
    static mapTableAt(index, updateFun) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.MapTableAt(index, updateFun);
            return newAssay;
        };
    }
    static mapTable(name, updateFun) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.MapTable(name, updateFun);
            return newAssay;
        };
    }
    static renameTableAt(index, newName) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.RenameTableAt(index, newName);
            return newAssay;
        };
    }
    static renameTable(name, newName) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.RenameTable(name, newName);
            return newAssay;
        };
    }
    static addColumnAt(tableIndex, header, cells, columnIndex, forceReplace) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.AddColumnAt(tableIndex, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
            return newAssay;
        };
    }
    static addColumn(tableName, header, cells, columnIndex, forceReplace) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.AddColumn(tableName, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
            return newAssay;
        };
    }
    static removeColumnAt(tableIndex, columnIndex) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.RemoveColumnAt(tableIndex, columnIndex);
            return newAssay;
        };
    }
    static removeColumn(tableName, columnIndex) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.RemoveColumn(tableName, columnIndex);
            return newAssay;
        };
    }
    static updateColumnAt(tableIndex, columnIndex, header, cells) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.UpdateColumnAt(tableIndex, columnIndex, header, unwrap(cells));
            return newAssay;
        };
    }
    static updateColumn(tableName, columnIndex, header, cells) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.UpdateColumn(tableName, columnIndex, header, unwrap(cells));
            return newAssay;
        };
    }
    static getColumnAt(tableIndex, columnIndex) {
        return (study) => {
            const newAssay = study.Copy();
            return newAssay.GetColumnAt(tableIndex, columnIndex);
        };
    }
    static getColumn(tableName, columnIndex) {
        return (study) => {
            const newAssay = study.Copy();
            return newAssay.GetColumn(tableName, columnIndex);
        };
    }
    static addRowAt(tableIndex, cells, rowIndex) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.AddRowAt(tableIndex, unwrap(cells), unwrap(rowIndex));
            return newAssay;
        };
    }
    static addRow(tableName, cells, rowIndex) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.AddRow(tableName, unwrap(cells), unwrap(rowIndex));
            return newAssay;
        };
    }
    static removeRowAt(tableIndex, rowIndex) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.RemoveColumnAt(tableIndex, rowIndex);
            return newAssay;
        };
    }
    static removeRow(tableName, rowIndex) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.RemoveRow(tableName, rowIndex);
            return newAssay;
        };
    }
    static updateRowAt(tableIndex, rowIndex, cells) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.UpdateRowAt(tableIndex, rowIndex, cells);
            return newAssay;
        };
    }
    static updateRow(tableName, rowIndex, cells) {
        return (study) => {
            const newAssay = study.Copy();
            newAssay.UpdateRow(tableName, rowIndex, cells);
            return newAssay;
        };
    }
    static getRowAt(tableIndex, rowIndex) {
        return (study) => {
            const newAssay = study.Copy();
            return newAssay.GetRowAt(tableIndex, rowIndex);
        };
    }
    static getRow(tableName, rowIndex) {
        return (study) => {
            const newAssay = study.Copy();
            return newAssay.GetRow(tableName, rowIndex);
        };
    }
    AddToInvestigation(investigation) {
        const this$ = this;
        this$.Investigation = investigation;
    }
    RemoveFromInvestigation() {
        const this$ = this;
        this$.Investigation = undefined;
    }
    Copy(copyInvestigationRef) {
        const this$ = this;
        const copyInvestigationRef_1 = defaultArg(copyInvestigationRef, false);
        const nextTables = [];
        const nextAssayIdentifiers = Array.from(this$.RegisteredAssayIdentifiers);
        let enumerator = getEnumerator(this$.Tables);
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const table = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                const copy = table.Copy();
                void (nextTables.push(copy));
            }
        }
        finally {
            disposeSafe(enumerator);
        }
        const nextComments = ResizeArray_map((c) => c.Copy(), this$.Comments);
        const nextContacts = ResizeArray_map((c_1) => c_1.Copy(), this$.Contacts);
        const nextPublications = ResizeArray_map((c_2) => c_2.Copy(), this$.Publications);
        const nextStudyDesignDescriptors = ResizeArray_map((c_3) => c_3.Copy(), this$.StudyDesignDescriptors);
        const nextDataMap = map(DataMap__Copy, this$.DataMap);
        let study;
        const identifier = this$.Identifier;
        const title = this$.Title;
        const description = this$.Description;
        const submissionDate = this$.SubmissionDate;
        const publicReleaseDate = this$.PublicReleaseDate;
        study = ArcStudy.make(identifier, title, description, submissionDate, publicReleaseDate, nextPublications, nextContacts, nextStudyDesignDescriptors, nextTables, nextDataMap, nextAssayIdentifiers, nextComments);
        if (copyInvestigationRef_1) {
            study.Investigation = this$.Investigation;
        }
        return study;
    }
    UpdateReferenceByStudyFile(study, onlyReplaceExisting, keepUnusedRefTables) {
        const this$ = this;
        const updateAlways = !defaultArg(onlyReplaceExisting, false);
        if ((study.Title != null) ? true : updateAlways) {
            this$.Title = study.Title;
        }
        if ((study.Description != null) ? true : updateAlways) {
            this$.Description = study.Description;
        }
        if ((study.SubmissionDate != null) ? true : updateAlways) {
            this$.SubmissionDate = study.SubmissionDate;
        }
        if ((study.PublicReleaseDate != null) ? true : updateAlways) {
            this$.PublicReleaseDate = study.PublicReleaseDate;
        }
        if ((study.Publications.length !== 0) ? true : updateAlways) {
            this$.Publications = study.Publications;
        }
        if ((study.Contacts.length !== 0) ? true : updateAlways) {
            this$.Contacts = study.Contacts;
        }
        if ((study.StudyDesignDescriptors.length !== 0) ? true : updateAlways) {
            this$.StudyDesignDescriptors = study.StudyDesignDescriptors;
        }
        if ((study.Tables.length !== 0) ? true : updateAlways) {
            const tables = ArcTables.updateReferenceTablesBySheets(new ArcTables(this$.Tables), new ArcTables(study.Tables), unwrap(keepUnusedRefTables));
            this$.Tables = tables.Tables;
        }
        this$.DataMap = study.DataMap;
        if ((study.RegisteredAssayIdentifiers.length !== 0) ? true : updateAlways) {
            this$.RegisteredAssayIdentifiers = study.RegisteredAssayIdentifiers;
        }
        if ((study.Comments.length !== 0) ? true : updateAlways) {
            this$.Comments = study.Comments;
        }
    }
    StructurallyEquals(other) {
        let a, b, a_1, b_1, a_2, b_2, a_3, b_3, a_4, b_4, a_5, b_5;
        const this$ = this;
        return forAll((x) => (x === true), [this$.Identifier === other.Identifier, equals(this$.Title, other.Title), equals(this$.Description, other.Description), equals(this$.SubmissionDate, other.SubmissionDate), equals(this$.PublicReleaseDate, other.PublicReleaseDate), equals(this$.DataMap, other.DataMap), (a = this$.Publications, (b = other.Publications, (length(a) === length(b)) && fold((acc, e) => {
            if (acc) {
                return e;
            }
            else {
                return false;
            }
        }, true, toList(delay(() => map_1((i_1) => equals(item(i_1, a), item(i_1, b)), rangeDouble(0, 1, length(a) - 1))))))), (a_1 = this$.Contacts, (b_1 = other.Contacts, (length(a_1) === length(b_1)) && fold((acc_1, e_1) => {
            if (acc_1) {
                return e_1;
            }
            else {
                return false;
            }
        }, true, toList(delay(() => map_1((i_2) => equals(item(i_2, a_1), item(i_2, b_1)), rangeDouble(0, 1, length(a_1) - 1))))))), (a_2 = this$.StudyDesignDescriptors, (b_2 = other.StudyDesignDescriptors, (length(a_2) === length(b_2)) && fold((acc_2, e_2) => {
            if (acc_2) {
                return e_2;
            }
            else {
                return false;
            }
        }, true, toList(delay(() => map_1((i_3) => equals(item(i_3, a_2), item(i_3, b_2)), rangeDouble(0, 1, length(a_2) - 1))))))), (a_3 = this$.Tables, (b_3 = other.Tables, (length(a_3) === length(b_3)) && fold((acc_3, e_3) => {
            if (acc_3) {
                return e_3;
            }
            else {
                return false;
            }
        }, true, toList(delay(() => map_1((i_4) => equals(item(i_4, a_3), item(i_4, b_3)), rangeDouble(0, 1, length(a_3) - 1))))))), (a_4 = this$.RegisteredAssayIdentifiers, (b_4 = other.RegisteredAssayIdentifiers, (length(a_4) === length(b_4)) && fold((acc_4, e_4) => {
            if (acc_4) {
                return e_4;
            }
            else {
                return false;
            }
        }, true, toList(delay(() => map_1((i_5) => (item(i_5, a_4) === item(i_5, b_4)), rangeDouble(0, 1, length(a_4) - 1))))))), (a_5 = this$.Comments, (b_5 = other.Comments, (length(a_5) === length(b_5)) && fold((acc_5, e_5) => {
            if (acc_5) {
                return e_5;
            }
            else {
                return false;
            }
        }, true, toList(delay(() => map_1((i_6) => equals(item(i_6, a_5), item(i_6, b_5)), rangeDouble(0, 1, length(a_5) - 1)))))))]);
    }
    ReferenceEquals(other) {
        const this$ = this;
        return this$ === other;
    }
    toString() {
        const this$ = this;
        const arg = this$.Identifier;
        const arg_1 = this$.Title;
        const arg_2 = this$.Description;
        const arg_3 = this$.SubmissionDate;
        const arg_4 = this$.PublicReleaseDate;
        const arg_5 = this$.Publications;
        const arg_6 = this$.Contacts;
        const arg_7 = this$.StudyDesignDescriptors;
        const arg_8 = this$.Tables;
        const arg_9 = this$.RegisteredAssayIdentifiers;
        const arg_10 = this$.Comments;
        return toText(printf("ArcStudy {\r\n    Identifier = %A,\r\n    Title = %A,\r\n    Description = %A,\r\n    SubmissionDate = %A,\r\n    PublicReleaseDate = %A,\r\n    Publications = %A,\r\n    Contacts = %A,\r\n    StudyDesignDescriptors = %A,\r\n    Tables = %A,\r\n    RegisteredAssayIdentifiers = %A,\r\n    Comments = %A,\r\n}"))(arg)(arg_1)(arg_2)(arg_3)(arg_4)(arg_5)(arg_6)(arg_7)(arg_8)(arg_9)(arg_10);
    }
    Equals(other) {
        let s;
        const this$ = this;
        return (other instanceof ArcStudy) && ((s = other, this$.StructurallyEquals(s)));
    }
    GetHashCode() {
        const this$ = this;
        return boxHashArray([this$.Identifier, boxHashOption(this$.Title), boxHashOption(this$.Description), boxHashOption(this$.SubmissionDate), boxHashOption(this$.PublicReleaseDate), boxHashOption(this$.DataMap), boxHashSeq(this$.Publications), boxHashSeq(this$.Contacts), boxHashSeq(this$.StudyDesignDescriptors), boxHashSeq(this$.Tables), boxHashSeq(this$.RegisteredAssayIdentifiers), boxHashSeq(this$.Comments)]) | 0;
    }
    GetLightHashCode() {
        const this$ = this;
        return boxHashArray([this$.Identifier, boxHashOption(this$.Title), boxHashOption(this$.Description), boxHashOption(this$.SubmissionDate), boxHashOption(this$.PublicReleaseDate), boxHashSeq(this$.Publications), boxHashSeq(this$.Contacts), boxHashSeq(this$.StudyDesignDescriptors), boxHashSeq(this$.Tables), boxHashSeq(this$.RegisteredAssayIdentifiers), boxHashSeq(this$.Comments)]) | 0;
    }
}

export function ArcStudy_$reflection() {
    return class_type("ARCtrl.ArcStudy", undefined, ArcStudy, ArcTables_$reflection());
}

export function ArcStudy_$ctor_64321D5B(identifier, title, description, submissionDate, publicReleaseDate, publications, contacts, studyDesignDescriptors, tables, datamap, registeredAssayIdentifiers, comments) {
    return new ArcStudy(identifier, title, description, submissionDate, publicReleaseDate, publications, contacts, studyDesignDescriptors, tables, datamap, registeredAssayIdentifiers, comments);
}

export class ArcInvestigation {
    constructor(identifier, title, description, submissionDate, publicReleaseDate, ontologySourceReferences, publications, contacts, assays, studies, registeredStudyIdentifiers, comments, remarks) {
        const this$ = new FSharpRef(defaultOf());
        this$.contents = this;
        const ontologySourceReferences_1 = defaultArg(ontologySourceReferences, []);
        const publications_1 = defaultArg(publications, []);
        const contacts_1 = defaultArg(contacts, []);
        let assays_1;
        const ass = defaultArg(assays, []);
        let enumerator = getEnumerator(ass);
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const a = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                a.Investigation = this$.contents;
            }
        }
        finally {
            disposeSafe(enumerator);
        }
        assays_1 = ass;
        let studies_1;
        const sss = defaultArg(studies, []);
        let enumerator_1 = getEnumerator(sss);
        try {
            while (enumerator_1["System.Collections.IEnumerator.MoveNext"]()) {
                const s = enumerator_1["System.Collections.Generic.IEnumerator`1.get_Current"]();
                s.Investigation = this$.contents;
            }
        }
        finally {
            disposeSafe(enumerator_1);
        }
        studies_1 = sss;
        const registeredStudyIdentifiers_1 = defaultArg(registeredStudyIdentifiers, []);
        const comments_1 = defaultArg(comments, []);
        const remarks_1 = defaultArg(remarks, []);
        this["identifier@1122"] = identifier;
        this["title@1123"] = title;
        this["description@1124"] = description;
        this["submissionDate@1125"] = submissionDate;
        this["publicReleaseDate@1126"] = publicReleaseDate;
        this["ontologySourceReferences@1127-1"] = ontologySourceReferences_1;
        this["publications@1128-1"] = publications_1;
        this["contacts@1129-1"] = contacts_1;
        this["assays@1130-1"] = assays_1;
        this["studies@1131-1"] = studies_1;
        this["registeredStudyIdentifiers@1132-1"] = registeredStudyIdentifiers_1;
        this["comments@1133-1"] = comments_1;
        this["remarks@1134-1"] = remarks_1;
        this.staticHash = 0;
        this["init@1103"] = 1;
    }
    get Identifier() {
        const this$ = this;
        return this$["identifier@1122"];
    }
    set Identifier(i) {
        const this$ = this;
        this$["identifier@1122"] = i;
    }
    get Title() {
        const this$ = this;
        return unwrap(this$["title@1123"]);
    }
    set Title(n) {
        const this$ = this;
        this$["title@1123"] = n;
    }
    get Description() {
        const this$ = this;
        return unwrap(this$["description@1124"]);
    }
    set Description(n) {
        const this$ = this;
        this$["description@1124"] = n;
    }
    get SubmissionDate() {
        const this$ = this;
        return unwrap(this$["submissionDate@1125"]);
    }
    set SubmissionDate(n) {
        const this$ = this;
        this$["submissionDate@1125"] = n;
    }
    get PublicReleaseDate() {
        const this$ = this;
        return unwrap(this$["publicReleaseDate@1126"]);
    }
    set PublicReleaseDate(n) {
        const this$ = this;
        this$["publicReleaseDate@1126"] = n;
    }
    get OntologySourceReferences() {
        const this$ = this;
        return this$["ontologySourceReferences@1127-1"];
    }
    set OntologySourceReferences(n) {
        const this$ = this;
        this$["ontologySourceReferences@1127-1"] = n;
    }
    get Publications() {
        const this$ = this;
        return this$["publications@1128-1"];
    }
    set Publications(n) {
        const this$ = this;
        this$["publications@1128-1"] = n;
    }
    get Contacts() {
        const this$ = this;
        return this$["contacts@1129-1"];
    }
    set Contacts(n) {
        const this$ = this;
        this$["contacts@1129-1"] = n;
    }
    get Assays() {
        const this$ = this;
        return this$["assays@1130-1"];
    }
    set Assays(n) {
        const this$ = this;
        this$["assays@1130-1"] = n;
    }
    get Studies() {
        const this$ = this;
        return this$["studies@1131-1"];
    }
    set Studies(n) {
        const this$ = this;
        this$["studies@1131-1"] = n;
    }
    get RegisteredStudyIdentifiers() {
        const this$ = this;
        return this$["registeredStudyIdentifiers@1132-1"];
    }
    set RegisteredStudyIdentifiers(n) {
        const this$ = this;
        this$["registeredStudyIdentifiers@1132-1"] = n;
    }
    get Comments() {
        const this$ = this;
        return this$["comments@1133-1"];
    }
    set Comments(n) {
        const this$ = this;
        this$["comments@1133-1"] = n;
    }
    get Remarks() {
        const this$ = this;
        return this$["remarks@1134-1"];
    }
    set Remarks(n) {
        const this$ = this;
        this$["remarks@1134-1"] = n;
    }
    get StaticHash() {
        const this$ = this;
        return this$.staticHash | 0;
    }
    set StaticHash(h) {
        const this$ = this;
        this$.staticHash = (h | 0);
    }
    static get FileName() {
        return "isa.investigation.xlsx";
    }
    static init(identifier) {
        return new ArcInvestigation(identifier);
    }
    static create(identifier, title, description, submissionDate, publicReleaseDate, ontologySourceReferences, publications, contacts, assays, studies, registeredStudyIdentifiers, comments, remarks) {
        return new ArcInvestigation(identifier, unwrap(title), unwrap(description), unwrap(submissionDate), unwrap(publicReleaseDate), unwrap(ontologySourceReferences), unwrap(publications), unwrap(contacts), unwrap(assays), unwrap(studies), unwrap(registeredStudyIdentifiers), unwrap(comments), unwrap(remarks));
    }
    static make(identifier, title, description, submissionDate, publicReleaseDate, ontologySourceReferences, publications, contacts, assays, studies, registeredStudyIdentifiers, comments, remarks) {
        return new ArcInvestigation(identifier, unwrap(title), unwrap(description), unwrap(submissionDate), unwrap(publicReleaseDate), ontologySourceReferences, publications, contacts, assays, studies, registeredStudyIdentifiers, comments, remarks);
    }
    get AssayCount() {
        const this$ = this;
        return this$.Assays.length | 0;
    }
    get AssayIdentifiers() {
        const this$ = this;
        return Array.from(map_1((x) => x.Identifier, this$.Assays));
    }
    get UnregisteredAssays() {
        const this$ = this;
        return ResizeArray_filter((a) => !exists((s) => exists((i) => (i === a.Identifier), s.RegisteredAssayIdentifiers), this$.RegisteredStudies), this$.Assays);
    }
    AddAssay(assay, registerIn) {
        const this$ = this;
        const assayIdent = assay.Identifier;
        const matchValue = tryFindIndex((x_1) => (x_1 === assayIdent), map_1((x) => x.Identifier, this$.Assays));
        if (matchValue == null) {
        }
        else {
            throw new Error(`Cannot create assay with name ${assayIdent}, as assay names must be unique and assay at index ${matchValue} has the same name.`);
        }
        assay.Investigation = this$;
        void (this$.Assays.push(assay));
        if (registerIn != null) {
            let enumerator = getEnumerator(value_4(registerIn));
            try {
                while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                    const study = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                    study.RegisterAssay(assay.Identifier);
                }
            }
            finally {
                disposeSafe(enumerator);
            }
        }
    }
    static addAssay(assay, registerIn) {
        return (inv) => {
            const newInvestigation = inv.Copy();
            newInvestigation.AddAssay(assay, unwrap(registerIn));
            return newInvestigation;
        };
    }
    InitAssay(assayIdentifier, registerIn) {
        const this$ = this;
        const assay = new ArcAssay(assayIdentifier);
        this$.AddAssay(assay, unwrap(registerIn));
        return assay;
    }
    static initAssay(assayIdentifier, registerIn) {
        return (inv) => {
            const newInvestigation = inv.Copy();
            return newInvestigation.InitAssay(assayIdentifier, unwrap(registerIn));
        };
    }
    DeleteAssayAt(index) {
        const this$ = this;
        this$.Assays.splice(index, 1);
    }
    static deleteAssayAt(index) {
        return (inv) => {
            const newInvestigation = inv.Copy();
            newInvestigation.DeleteAssayAt(index);
            return newInvestigation;
        };
    }
    DeleteAssay(assayIdentifier) {
        const this$ = this;
        const index = this$.GetAssayIndex(assayIdentifier) | 0;
        this$.DeleteAssayAt(index);
    }
    static deleteAssay(assayIdentifier) {
        return (inv) => {
            const newInv = inv.Copy();
            newInv.DeleteAssay(assayIdentifier);
            return newInv;
        };
    }
    RemoveAssayAt(index) {
        const this$ = this;
        const ident = this$.GetAssayAt(index).Identifier;
        this$.Assays.splice(index, 1);
        let enumerator = getEnumerator(this$.Studies);
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const study = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                study.DeregisterAssay(ident);
            }
        }
        finally {
            disposeSafe(enumerator);
        }
    }
    static removeAssayAt(index) {
        return (inv) => {
            const newInvestigation = inv.Copy();
            newInvestigation.RemoveAssayAt(index);
            return newInvestigation;
        };
    }
    RemoveAssay(assayIdentifier) {
        const this$ = this;
        const index = this$.GetAssayIndex(assayIdentifier) | 0;
        this$.RemoveAssayAt(index);
    }
    static removeAssay(assayIdentifier) {
        return (inv) => {
            const newInv = inv.Copy();
            newInv.RemoveAssay(assayIdentifier);
            return newInv;
        };
    }
    RenameAssay(oldIdentifier, newIdentifier) {
        const this$ = this;
        iterate((a) => {
            if (a.Identifier === oldIdentifier) {
                a.Identifier = newIdentifier;
            }
        }, this$.Assays);
        iterate((s) => {
            const index = tryFindIndex((ai) => (ai === oldIdentifier), s.RegisteredAssayIdentifiers);
            if (index != null) {
                const index_1 = index | 0;
                s.RegisteredAssayIdentifiers[index_1] = newIdentifier;
            }
        }, this$.Studies);
    }
    static renameAssay(oldIdentifier, newIdentifier) {
        return (inv) => {
            const newInv = inv.Copy();
            newInv.RenameAssay(oldIdentifier, newIdentifier);
            return newInv;
        };
    }
    SetAssayAt(index, assay) {
        const this$ = this;
        const assayIdent = assay.Identifier;
        const matchValue = tryFindIndex((x) => (x === assayIdent), map_1((a) => a.Identifier, removeAt(index, this$.Assays)));
        if (matchValue == null) {
        }
        else {
            throw new Error(`Cannot create assay with name ${assayIdent}, as assay names must be unique and assay at index ${matchValue} has the same name.`);
        }
        assay.Investigation = this$;
        this$.Assays[index] = assay;
        this$.DeregisterMissingAssays();
    }
    static setAssayAt(index, assay) {
        return (inv) => {
            const newInvestigation = inv.Copy();
            newInvestigation.SetAssayAt(index, assay);
            return newInvestigation;
        };
    }
    SetAssay(assayIdentifier, assay) {
        const this$ = this;
        const index = this$.GetAssayIndex(assayIdentifier) | 0;
        this$.SetAssayAt(index, assay);
    }
    static setAssay(assayIdentifier, assay) {
        return (inv) => {
            const newInvestigation = inv.Copy();
            newInvestigation.SetAssay(assayIdentifier, assay);
            return newInvestigation;
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
    static getAssayIndex(assayIdentifier) {
        return (inv) => inv.GetAssayIndex(assayIdentifier);
    }
    GetAssayAt(index) {
        const this$ = this;
        return this$.Assays[index];
    }
    static getAssayAt(index) {
        return (inv) => {
            const newInvestigation = inv.Copy();
            return newInvestigation.GetAssayAt(index);
        };
    }
    GetAssay(assayIdentifier) {
        const this$ = this;
        const matchValue = this$.TryGetAssay(assayIdentifier);
        if (matchValue == null) {
            throw new Error(ArcTypesAux_ErrorMsgs_unableToFindAssayIdentifier(assayIdentifier, this$.Identifier));
        }
        else {
            return matchValue;
        }
    }
    static getAssay(assayIdentifier) {
        return (inv) => {
            const newInvestigation = inv.Copy();
            return newInvestigation.GetAssay(assayIdentifier);
        };
    }
    TryGetAssay(assayIdentifier) {
        const this$ = this;
        return tryFind((a) => (a.Identifier === assayIdentifier), this$.Assays);
    }
    static tryGetAssay(assayIdentifier) {
        return (inv) => {
            const newInvestigation = inv.Copy();
            return newInvestigation.TryGetAssay(assayIdentifier);
        };
    }
    ContainsAssay(assayIdentifier) {
        const this$ = this;
        return exists((a) => (a.Identifier === assayIdentifier), this$.Assays);
    }
    static containsAssay(assayIdentifier) {
        return (inv) => inv.ContainsAssay(assayIdentifier);
    }
    get RegisteredStudyIdentifierCount() {
        const this$ = this;
        return this$.RegisteredStudyIdentifiers.length | 0;
    }
    get RegisteredStudies() {
        const this$ = this;
        return ResizeArray_choose((identifier) => this$.TryGetStudy(identifier), this$.RegisteredStudyIdentifiers);
    }
    get RegisteredStudyCount() {
        const this$ = this;
        return this$.RegisteredStudies.length | 0;
    }
    get VacantStudyIdentifiers() {
        const this$ = this;
        return ResizeArray_filter((arg) => !this$.ContainsStudy(arg), this$.RegisteredStudyIdentifiers);
    }
    get StudyCount() {
        const this$ = this;
        return this$.Studies.length | 0;
    }
    get StudyIdentifiers() {
        const this$ = this;
        return toArray(map_1((x) => x.Identifier, this$.Studies));
    }
    get UnregisteredStudies() {
        const this$ = this;
        return ResizeArray_filter((s) => {
            let source, x;
            return !((source = this$.RegisteredStudyIdentifiers, exists((x = s.Identifier, (y) => (x === y)), source)));
        }, this$.Studies);
    }
    AddStudy(study) {
        const this$ = this;
        const study_1 = study;
        const matchValue = tryFindIndex((x) => (x.Identifier === study_1.Identifier), this$.Studies);
        if (matchValue == null) {
        }
        else {
            throw new Error(`Cannot create study with name ${study_1.Identifier}, as study names must be unique and study at index ${matchValue} has the same name.`);
        }
        study.Investigation = this$;
        void (this$.Studies.push(study));
    }
    static addStudy(study) {
        return (inv) => {
            const copy = inv.Copy();
            copy.AddStudy(study);
            return copy;
        };
    }
    InitStudy(studyIdentifier) {
        const this$ = this;
        const study = ArcStudy.init(studyIdentifier);
        this$.AddStudy(study);
        return study;
    }
    static initStudy(studyIdentifier) {
        return (inv) => {
            const copy = inv.Copy();
            return [copy, copy.InitStudy(studyIdentifier)];
        };
    }
    RegisterStudy(studyIdentifier) {
        const this$ = this;
        const studyIdent = studyIdentifier;
        const matchValue = tryFind((x) => (x === studyIdent), this$.StudyIdentifiers);
        if (matchValue != null) {
        }
        else {
            throw new Error(`The given study with identifier '${studyIdent}' must be added to Investigation before it can be registered.`);
        }
        const studyIdent_1 = studyIdentifier;
        if (contains(studyIdent_1, this$.RegisteredStudyIdentifiers, {
            Equals: (x_1, y) => (x_1 === y),
            GetHashCode: stringHash,
        })) {
            throw new Error(`Study with identifier '${studyIdent_1}' is already registered!`);
        }
        void (this$.RegisteredStudyIdentifiers.push(studyIdentifier));
    }
    static registerStudy(studyIdentifier) {
        return (inv) => {
            const copy = inv.Copy();
            copy.RegisterStudy(studyIdentifier);
            return copy;
        };
    }
    AddRegisteredStudy(study) {
        const this$ = this;
        this$.AddStudy(study);
        this$.RegisterStudy(study.Identifier);
    }
    static addRegisteredStudy(study) {
        return (inv) => {
            const copy = inv.Copy();
            const study_1 = study.Copy();
            copy.AddRegisteredStudy(study_1);
            return copy;
        };
    }
    DeleteStudyAt(index) {
        const this$ = this;
        this$.Studies.splice(index, 1);
    }
    static deleteStudyAt(index) {
        return (i) => {
            const copy = i.Copy();
            copy.DeleteStudyAt(index);
            return copy;
        };
    }
    DeleteStudy(studyIdentifier) {
        const this$ = this;
        const index = this$.Studies.findIndex((s) => (s.Identifier === studyIdentifier)) | 0;
        this$.DeleteStudyAt(index);
    }
    static deleteStudy(studyIdentifier) {
        return (i) => {
            const copy = i.Copy();
            copy.DeleteStudy(studyIdentifier);
            return copy;
        };
    }
    RemoveStudyAt(index) {
        const this$ = this;
        const ident = this$.GetStudyAt(index).Identifier;
        this$.Studies.splice(index, 1);
        this$.DeregisterStudy(ident);
    }
    static removeStudyAt(index) {
        return (inv) => {
            const newInv = inv.Copy();
            newInv.RemoveStudyAt(index);
            return newInv;
        };
    }
    RemoveStudy(studyIdentifier) {
        const this$ = this;
        const index = this$.GetStudyIndex(studyIdentifier) | 0;
        this$.RemoveStudyAt(index);
    }
    static removeStudy(studyIdentifier) {
        return (inv) => {
            const copy = inv.Copy();
            copy.RemoveStudy(studyIdentifier);
            return copy;
        };
    }
    RenameStudy(oldIdentifier, newIdentifier) {
        const this$ = this;
        iterate((s) => {
            if (s.Identifier === oldIdentifier) {
                s.Identifier = newIdentifier;
            }
        }, this$.Studies);
        const index = tryFindIndex((si) => (si === oldIdentifier), this$.RegisteredStudyIdentifiers);
        if (index != null) {
            const index_1 = index | 0;
            this$.RegisteredStudyIdentifiers[index_1] = newIdentifier;
        }
    }
    static renameStudy(oldIdentifier, newIdentifier) {
        return (inv) => {
            const newInv = inv.Copy();
            newInv.RenameStudy(oldIdentifier, newIdentifier);
            return newInv;
        };
    }
    SetStudyAt(index, study) {
        const this$ = this;
        const study_1 = study;
        const matchValue = tryFindIndex((x) => (x.Identifier === study_1.Identifier), removeAt(index, this$.Studies));
        if (matchValue == null) {
        }
        else {
            throw new Error(`Cannot create study with name ${study_1.Identifier}, as study names must be unique and study at index ${matchValue} has the same name.`);
        }
        study.Investigation = this$;
        this$.Studies[index] = study;
    }
    static setStudyAt(index, study) {
        return (inv) => {
            const newInv = inv.Copy();
            newInv.SetStudyAt(index, study);
            return newInv;
        };
    }
    SetStudy(studyIdentifier, study) {
        const this$ = this;
        const index = this$.GetStudyIndex(studyIdentifier) | 0;
        this$.SetStudyAt(index, study);
    }
    static setStudy(studyIdentifier, study) {
        return (inv) => {
            const newInv = inv.Copy();
            newInv.SetStudy(studyIdentifier, study);
            return newInv;
        };
    }
    GetStudyIndex(studyIdentifier) {
        const this$ = this;
        const index = this$.Studies.findIndex((s) => (s.Identifier === studyIdentifier)) | 0;
        if (index === -1) {
            throw new Error(`Unable to find study with specified identifier '${studyIdentifier}'!`);
        }
        return index | 0;
    }
    static getStudyIndex(studyIdentifier) {
        return (inv) => inv.GetStudyIndex(studyIdentifier);
    }
    GetStudyAt(index) {
        const this$ = this;
        return this$.Studies[index];
    }
    static getStudyAt(index) {
        return (inv) => {
            const newInv = inv.Copy();
            return newInv.GetStudyAt(index);
        };
    }
    GetStudy(studyIdentifier) {
        const this$ = this;
        const matchValue = this$.TryGetStudy(studyIdentifier);
        if (matchValue == null) {
            throw new Error(ArcTypesAux_ErrorMsgs_unableToFindStudyIdentifier(studyIdentifier, this$.Identifier));
        }
        else {
            return matchValue;
        }
    }
    static getStudy(studyIdentifier) {
        return (inv) => {
            const newInv = inv.Copy();
            return newInv.GetStudy(studyIdentifier);
        };
    }
    TryGetStudy(studyIdentifier) {
        const this$ = this;
        return tryFind((s) => (s.Identifier === studyIdentifier), this$.Studies);
    }
    static tryGetStudy(studyIdentifier) {
        return (inv) => {
            const newInv = inv.Copy();
            return newInv.TryGetStudy(studyIdentifier);
        };
    }
    ContainsStudy(studyIdentifier) {
        const this$ = this;
        return exists((s) => (s.Identifier === studyIdentifier), this$.Studies);
    }
    static containsStudy(studyIdentifier) {
        return (inv) => inv.ContainsStudy(studyIdentifier);
    }
    RegisterAssayAt(studyIndex, assayIdentifier) {
        const this$ = this;
        const study = this$.GetStudyAt(studyIndex);
        const matchValue = tryFind((x) => (x === assayIdentifier), map_1((a) => a.Identifier, this$.Assays));
        if (matchValue != null) {
        }
        else {
            throw new Error("The given assay must be added to Investigation before it can be registered.");
        }
        const assayIdent_1 = assayIdentifier;
        const matchValue_1 = tryFindIndex((x_1) => (x_1 === assayIdent_1), study.RegisteredAssayIdentifiers);
        if (matchValue_1 == null) {
        }
        else {
            throw new Error(`Cannot create assay with name ${assayIdent_1}, as assay names must be unique and assay at index ${matchValue_1} has the same name.`);
        }
        study.RegisterAssay(assayIdentifier);
    }
    static registerAssayAt(studyIndex, assayIdentifier) {
        return (inv) => {
            const copy = inv.Copy();
            copy.RegisterAssayAt(studyIndex, assayIdentifier);
            return copy;
        };
    }
    RegisterAssay(studyIdentifier, assayIdentifier) {
        const this$ = this;
        const index = this$.GetStudyIndex(studyIdentifier) | 0;
        this$.RegisterAssayAt(index, assayIdentifier);
    }
    static registerAssay(studyIdentifier, assayIdentifier) {
        return (inv) => {
            const copy = inv.Copy();
            copy.RegisterAssay(studyIdentifier, assayIdentifier);
            return copy;
        };
    }
    DeregisterAssayAt(studyIndex, assayIdentifier) {
        const this$ = this;
        const study = this$.GetStudyAt(studyIndex);
        study.DeregisterAssay(assayIdentifier);
    }
    static deregisterAssayAt(studyIndex, assayIdentifier) {
        return (inv) => {
            const copy = inv.Copy();
            copy.DeregisterAssayAt(studyIndex, assayIdentifier);
            return copy;
        };
    }
    DeregisterAssay(studyIdentifier, assayIdentifier) {
        const this$ = this;
        const index = this$.GetStudyIndex(studyIdentifier) | 0;
        this$.DeregisterAssayAt(index, assayIdentifier);
    }
    static deregisterAssay(studyIdentifier, assayIdentifier) {
        return (inv) => {
            const copy = inv.Copy();
            copy.DeregisterAssay(studyIdentifier, assayIdentifier);
            return copy;
        };
    }
    DeregisterStudy(studyIdentifier) {
        const this$ = this;
        removeInPlace(studyIdentifier, this$.RegisteredStudyIdentifiers, {
            Equals: (x, y) => (x === y),
            GetHashCode: stringHash,
        });
    }
    static deregisterStudy(studyIdentifier) {
        return (i) => {
            const copy = i.Copy();
            copy.DeregisterStudy(studyIdentifier);
            return copy;
        };
    }
    GetAllPersons() {
        const this$ = this;
        const persons = [];
        let enumerator = getEnumerator(this$.Assays);
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const a = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                addRangeInPlace(a.Performers, persons);
            }
        }
        finally {
            disposeSafe(enumerator);
        }
        let enumerator_1 = getEnumerator(this$.Studies);
        try {
            while (enumerator_1["System.Collections.IEnumerator.MoveNext"]()) {
                const s = enumerator_1["System.Collections.Generic.IEnumerator`1.get_Current"]();
                addRangeInPlace(s.Contacts, persons);
            }
        }
        finally {
            disposeSafe(enumerator_1);
        }
        addRangeInPlace(this$.Contacts, persons);
        return Array_distinct(Array.from(persons), {
            Equals: equals,
            GetHashCode: safeHash,
        });
    }
    GetAllPublications() {
        const this$ = this;
        const pubs = [];
        let enumerator = getEnumerator(this$.Studies);
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const s = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                addRangeInPlace(s.Publications, pubs);
            }
        }
        finally {
            disposeSafe(enumerator);
        }
        addRangeInPlace(this$.Publications, pubs);
        return Array_distinct(Array.from(pubs), {
            Equals: equals,
            GetHashCode: safeHash,
        });
    }
    DeregisterMissingAssays() {
        const this$ = this;
        const inv = this$;
        const existingAssays = inv.AssayIdentifiers;
        let enumerator = getEnumerator(inv.Studies);
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const study = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                const rai = study.RegisteredAssayIdentifiers;
                let enumerator_1 = getEnumerator(Array.from(rai));
                try {
                    while (enumerator_1["System.Collections.IEnumerator.MoveNext"]()) {
                        const registeredAssay = enumerator_1["System.Collections.Generic.IEnumerator`1.get_Current"]();
                        if (!contains(registeredAssay, existingAssays, {
                            Equals: (x, y) => (x === y),
                            GetHashCode: stringHash,
                        })) {
                            const value_1 = study.DeregisterAssay(registeredAssay);
                        }
                    }
                }
                finally {
                    disposeSafe(enumerator_1);
                }
            }
        }
        finally {
            disposeSafe(enumerator);
        }
    }
    static deregisterMissingAssays() {
        return (inv) => {
            const copy = inv.Copy();
            copy.DeregisterMissingAssays();
            return copy;
        };
    }
    UpdateIOTypeByEntityID() {
        let collection;
        const this$ = this;
        const ioMap = ArcTablesAux_getIOMap((collection = toList(delay(() => append_3(collect((study) => study.Tables, this$.Studies), delay(() => collect((assay) => assay.Tables, this$.Assays))))), Array.from(collection)));
        let enumerator = getEnumerator(this$.Studies);
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const study_1 = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                ArcTablesAux_applyIOMap(ioMap, study_1.Tables);
            }
        }
        finally {
            disposeSafe(enumerator);
        }
        let enumerator_1 = getEnumerator(this$.Assays);
        try {
            while (enumerator_1["System.Collections.IEnumerator.MoveNext"]()) {
                const assay_1 = enumerator_1["System.Collections.Generic.IEnumerator`1.get_Current"]();
                ArcTablesAux_applyIOMap(ioMap, assay_1.Tables);
            }
        }
        finally {
            disposeSafe(enumerator_1);
        }
    }
    Copy() {
        const this$ = this;
        const nextAssays = [];
        const nextStudies = [];
        let enumerator = getEnumerator(this$.Assays);
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const assay = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                const copy = assay.Copy();
                void (nextAssays.push(copy));
            }
        }
        finally {
            disposeSafe(enumerator);
        }
        let enumerator_1 = getEnumerator(this$.Studies);
        try {
            while (enumerator_1["System.Collections.IEnumerator.MoveNext"]()) {
                const study = enumerator_1["System.Collections.Generic.IEnumerator`1.get_Current"]();
                const copy_1 = study.Copy();
                void (nextStudies.push(copy_1));
            }
        }
        finally {
            disposeSafe(enumerator_1);
        }
        const nextComments = ResizeArray_map((c) => c.Copy(), this$.Comments);
        const nextRemarks = ResizeArray_map((c_1) => c_1.Copy(), this$.Remarks);
        const nextContacts = ResizeArray_map((c_2) => c_2.Copy(), this$.Contacts);
        const nextPublications = ResizeArray_map((c_3) => c_3.Copy(), this$.Publications);
        const nextOntologySourceReferences = ResizeArray_map((c_4) => c_4.Copy(), this$.OntologySourceReferences);
        const nextStudyIdentifiers = Array.from(this$.RegisteredStudyIdentifiers);
        return new ArcInvestigation(this$.Identifier, unwrap(this$.Title), unwrap(this$.Description), unwrap(this$.SubmissionDate), unwrap(this$.PublicReleaseDate), nextOntologySourceReferences, nextPublications, nextContacts, nextAssays, nextStudies, nextStudyIdentifiers, nextComments, nextRemarks);
    }
    StructurallyEquals(other) {
        let a, b, a_1, b_1, a_2, b_2, a_3, b_3, a_4, b_4, a_5, b_5, a_6, b_6, a_7, b_7;
        const this$ = this;
        return forAll((x) => (x === true), [this$.Identifier === other.Identifier, equals(this$.Title, other.Title), equals(this$.Description, other.Description), equals(this$.SubmissionDate, other.SubmissionDate), equals(this$.PublicReleaseDate, other.PublicReleaseDate), (a = this$.Publications, (b = other.Publications, (length(a) === length(b)) && fold((acc, e) => {
            if (acc) {
                return e;
            }
            else {
                return false;
            }
        }, true, toList(delay(() => map_1((i_1) => equals(item(i_1, a), item(i_1, b)), rangeDouble(0, 1, length(a) - 1))))))), (a_1 = this$.Contacts, (b_1 = other.Contacts, (length(a_1) === length(b_1)) && fold((acc_1, e_1) => {
            if (acc_1) {
                return e_1;
            }
            else {
                return false;
            }
        }, true, toList(delay(() => map_1((i_2) => equals(item(i_2, a_1), item(i_2, b_1)), rangeDouble(0, 1, length(a_1) - 1))))))), (a_2 = this$.OntologySourceReferences, (b_2 = other.OntologySourceReferences, (length(a_2) === length(b_2)) && fold((acc_2, e_2) => {
            if (acc_2) {
                return e_2;
            }
            else {
                return false;
            }
        }, true, toList(delay(() => map_1((i_3) => equals(item(i_3, a_2), item(i_3, b_2)), rangeDouble(0, 1, length(a_2) - 1))))))), (a_3 = this$.Assays, (b_3 = other.Assays, (length(a_3) === length(b_3)) && fold((acc_3, e_3) => {
            if (acc_3) {
                return e_3;
            }
            else {
                return false;
            }
        }, true, toList(delay(() => map_1((i_4) => equals(item(i_4, a_3), item(i_4, b_3)), rangeDouble(0, 1, length(a_3) - 1))))))), (a_4 = this$.Studies, (b_4 = other.Studies, (length(a_4) === length(b_4)) && fold((acc_4, e_4) => {
            if (acc_4) {
                return e_4;
            }
            else {
                return false;
            }
        }, true, toList(delay(() => map_1((i_5) => equals(item(i_5, a_4), item(i_5, b_4)), rangeDouble(0, 1, length(a_4) - 1))))))), (a_5 = this$.RegisteredStudyIdentifiers, (b_5 = other.RegisteredStudyIdentifiers, (length(a_5) === length(b_5)) && fold((acc_5, e_5) => {
            if (acc_5) {
                return e_5;
            }
            else {
                return false;
            }
        }, true, toList(delay(() => map_1((i_6) => (item(i_6, a_5) === item(i_6, b_5)), rangeDouble(0, 1, length(a_5) - 1))))))), (a_6 = this$.Comments, (b_6 = other.Comments, (length(a_6) === length(b_6)) && fold((acc_6, e_6) => {
            if (acc_6) {
                return e_6;
            }
            else {
                return false;
            }
        }, true, toList(delay(() => map_1((i_7) => equals(item(i_7, a_6), item(i_7, b_6)), rangeDouble(0, 1, length(a_6) - 1))))))), (a_7 = this$.Remarks, (b_7 = other.Remarks, (length(a_7) === length(b_7)) && fold((acc_7, e_7) => {
            if (acc_7) {
                return e_7;
            }
            else {
                return false;
            }
        }, true, toList(delay(() => map_1((i_8) => equals(item(i_8, a_7), item(i_8, b_7)), rangeDouble(0, 1, length(a_7) - 1)))))))]);
    }
    ReferenceEquals(other) {
        const this$ = this;
        return this$ === other;
    }
    toString() {
        const this$ = this;
        const arg = this$.Identifier;
        const arg_1 = this$.Title;
        const arg_2 = this$.Description;
        const arg_3 = this$.SubmissionDate;
        const arg_4 = this$.PublicReleaseDate;
        const arg_5 = this$.OntologySourceReferences;
        const arg_6 = this$.Publications;
        const arg_7 = this$.Contacts;
        const arg_8 = this$.Assays;
        const arg_9 = this$.Studies;
        const arg_10 = this$.RegisteredStudyIdentifiers;
        const arg_11 = this$.Comments;
        const arg_12 = this$.Remarks;
        return toText(printf("ArcInvestigation {\r\n    Identifier = %A,\r\n    Title = %A,\r\n    Description = %A,\r\n    SubmissionDate = %A,\r\n    PublicReleaseDate = %A,\r\n    OntologySourceReferences = %A,\r\n    Publications = %A,\r\n    Contacts = %A,\r\n    Assays = %A,\r\n    Studies = %A,\r\n    RegisteredStudyIdentifiers = %A,\r\n    Comments = %A,\r\n    Remarks = %A,\r\n}"))(arg)(arg_1)(arg_2)(arg_3)(arg_4)(arg_5)(arg_6)(arg_7)(arg_8)(arg_9)(arg_10)(arg_11)(arg_12);
    }
    Equals(other) {
        let i;
        const this$ = this;
        return (other instanceof ArcInvestigation) && ((i = other, this$.StructurallyEquals(i)));
    }
    GetHashCode() {
        const this$ = this;
        return boxHashArray([this$.Identifier, boxHashOption(this$.Title), boxHashOption(this$.Description), boxHashOption(this$.SubmissionDate), boxHashOption(this$.PublicReleaseDate), boxHashSeq(this$.Publications), boxHashSeq(this$.Contacts), boxHashSeq(this$.OntologySourceReferences), boxHashSeq(this$.Assays), boxHashSeq(this$.Studies), boxHashSeq(this$.RegisteredStudyIdentifiers), boxHashSeq(this$.Comments), boxHashSeq(this$.Remarks)]) | 0;
    }
    GetLightHashCode() {
        const this$ = this;
        return boxHashArray([this$.Identifier, boxHashOption(this$.Title), boxHashOption(this$.Description), boxHashOption(this$.SubmissionDate), boxHashOption(this$.PublicReleaseDate), boxHashSeq(this$.Publications), boxHashSeq(this$.Contacts), boxHashSeq(this$.OntologySourceReferences), boxHashSeq(this$.RegisteredStudyIdentifiers), boxHashSeq(this$.Comments), boxHashSeq(this$.Remarks)]) | 0;
    }
}

export function ArcInvestigation_$reflection() {
    return class_type("ARCtrl.ArcInvestigation", undefined, ArcInvestigation);
}

export function ArcInvestigation_$ctor_Z2ED5C612(identifier, title, description, submissionDate, publicReleaseDate, ontologySourceReferences, publications, contacts, assays, studies, registeredStudyIdentifiers, comments, remarks) {
    return new ArcInvestigation(identifier, title, description, submissionDate, publicReleaseDate, ontologySourceReferences, publications, contacts, assays, studies, registeredStudyIdentifiers, comments, remarks);
}

export function ArcTypesAux_ErrorMsgs_unableToFindAssayIdentifier(assayIdentifier, investigationIdentifier) {
    return `Error. Unable to find assay with identifier '${assayIdentifier}' in investigation ${investigationIdentifier}.`;
}

export function ArcTypesAux_ErrorMsgs_unableToFindStudyIdentifier(studyIdentifer, investigationIdentifier) {
    return `Error. Unable to find study with identifier '${studyIdentifer}' in investigation ${investigationIdentifier}.`;
}

