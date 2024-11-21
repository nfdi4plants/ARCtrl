import { toString, Record, FSharpException } from "../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, list_type, string_type, class_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { ofSeq, append as append_1, reverse, map, empty, ofArray } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { toRows, fromRows, toSparseTable, fromSparseTable } from "./Metadata/OntologyAnnotation.js";
import { Comment$_$reflection } from "../Core/Comment.js";
import { Comment_fromString } from "./Metadata/Comment.js";
import { SparseRowModule_fromAllValues, SparseRowModule_getAllValues, SparseRowModule_fromFsRow, SparseRowModule_writeToSheet, SparseRowModule_fromValues, SparseRowModule_tryGetValueAt, SparseTable_ToRows_759CAFC1, SparseTable_FromRows_Z5579EC29, SparseTable, SparseTable_Create_Z2192E64B, SparseTable__TryGetValueDefault_5BAE6133 } from "./Metadata/SparseTable.js";
import { createMissingIdentifier } from "../Core/Helper/Identifier.js";
import { addToDict } from "../fable_modules/fable-library-js.4.22.0/MapUtil.js";
import { List_distinct } from "../fable_modules/fable-library-js.4.22.0/Seq2.js";
import { getEnumerator, stringHash } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { tryPick, iterateIndexed, tryFind, toList, delay, singleton, append, head, exists, map as map_1 } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { Person_orcidKey } from "../Core/Conversion.js";
import { toArray } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { toRows as toRows_1, fromRows as fromRows_1 } from "./Metadata/Contacts.js";
import { parse } from "../fable_modules/fable-library-js.4.22.0/Guid.js";
import { Template, Organisation } from "../Core/Template.js";
import { FsWorksheet } from "../fable_modules/FsSpreadsheet.6.3.0-alpha.4/FsWorksheet.fs.js";
import { printf, toConsole } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { toFsWorksheet, tryFromFsWorksheet } from "./AnnotationTable/ArcTable.js";
import { now } from "../fable_modules/fable-library-js.4.22.0/Date.js";
import { FsWorkbook } from "../fable_modules/FsSpreadsheet.6.3.0-alpha.4/FsWorkbook.fs.js";

export class TemplateReadError extends FSharpException {
    constructor(Data0) {
        super();
        this.Data0 = Data0;
    }
}

export function TemplateReadError_$reflection() {
    return class_type("ARCtrl.Spreadsheet.TemplateReadError", undefined, TemplateReadError, class_type("System.Exception"));
}

export const Metadata_ER_labels = ofArray(["ER", "ER Term Accession Number", "ER Term Source REF"]);

export function Metadata_ER_fromSparseTable(matrix) {
    return fromSparseTable("ER", "ER Term Source REF", "ER Term Accession Number", matrix);
}

export function Metadata_ER_toSparseTable(designs) {
    return toSparseTable("ER", "ER Term Source REF", "ER Term Accession Number", designs);
}

export function Metadata_ER_fromRows(prefix, rows) {
    const patternInput = fromRows(prefix, "ER", "ER Term Source REF", "ER Term Accession Number", 0, rows);
    return [patternInput[0], patternInput[3]];
}

export function Metadata_ER_toRows(prefix, designs) {
    return toRows(prefix, "ER", "ER Term Source REF", "ER Term Accession Number", designs);
}

export const Metadata_Tags_labels = ofArray(["Tags", "Tags Term Accession Number", "Tags Term Source REF"]);

export function Metadata_Tags_fromSparseTable(matrix) {
    return fromSparseTable("Tags", "Tags Term Source REF", "Tags Term Accession Number", matrix);
}

export function Metadata_Tags_toSparseTable(designs) {
    return toSparseTable("Tags", "Tags Term Source REF", "Tags Term Accession Number", designs);
}

export function Metadata_Tags_fromRows(prefix, rows) {
    const patternInput = fromRows(prefix, "Tags", "Tags Term Source REF", "Tags Term Accession Number", 0, rows);
    return [patternInput[0], patternInput[3]];
}

export function Metadata_Tags_toRows(prefix, designs) {
    return toRows(prefix, "Tags", "Tags Term Source REF", "Tags Term Accession Number", designs);
}

export class Metadata_Template_TemplateInfo extends Record {
    constructor(Id, Name, Version, Description, Organisation, Table, Comments) {
        super();
        this.Id = Id;
        this.Name = Name;
        this.Version = Version;
        this.Description = Description;
        this.Organisation = Organisation;
        this.Table = Table;
        this.Comments = Comments;
    }
}

export function Metadata_Template_TemplateInfo_$reflection() {
    return record_type("ARCtrl.Spreadsheet.Metadata.Template.TemplateInfo", [], Metadata_Template_TemplateInfo, () => [["Id", string_type], ["Name", string_type], ["Version", string_type], ["Description", string_type], ["Organisation", string_type], ["Table", string_type], ["Comments", list_type(Comment$_$reflection())]]);
}

export function Metadata_Template_TemplateInfo_create(id, name, version, description, organisation, table, comments) {
    return new Metadata_Template_TemplateInfo(id, name, version, description, organisation, table, comments);
}

export function Metadata_Template_TemplateInfo_get_empty() {
    return Metadata_Template_TemplateInfo_create("", "", "", "", "", "", empty());
}

export function Metadata_Template_TemplateInfo_get_Labels() {
    return ofArray(["Id", "Name", "Version", "Description", "Organisation", "Table"]);
}

export function Metadata_Template_TemplateInfo_FromSparseTable_3ECCA699(matrix) {
    const comments = map((k) => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, 0])), matrix.CommentKeys);
    return Metadata_Template_TemplateInfo_create(SparseTable__TryGetValueDefault_5BAE6133(matrix, createMissingIdentifier(), ["Id", 0]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Name", 0]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Version", 0]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Description", 0]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Organisation", 0]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Table", 0]), comments);
}

export function Metadata_Template_TemplateInfo_ToSparseTable_48C39BE1(template) {
    let copyOfStruct, copyOfStruct_1;
    const matrix = SparseTable_Create_Z2192E64B(undefined, Metadata_Template_TemplateInfo_get_Labels(), undefined, 2);
    let commentKeys = empty();
    addToDict(matrix.Matrix, ["Id", 1], ((copyOfStruct = template.Id, copyOfStruct)).startsWith("MISSING_IDENTIFIER_") ? "" : ((copyOfStruct_1 = template.Id, copyOfStruct_1)));
    addToDict(matrix.Matrix, ["Name", 1], template.Name);
    addToDict(matrix.Matrix, ["Version", 1], template.Version);
    addToDict(matrix.Matrix, ["Description", 1], template.Description);
    addToDict(matrix.Matrix, ["Organisation", 1], toString(template.Organisation));
    addToDict(matrix.Matrix, ["Table", 1], template.Table.Name);
    return new SparseTable(matrix.Matrix, matrix.Keys, reverse(List_distinct(commentKeys, {
        Equals: (x, y) => (x === y),
        GetHashCode: stringHash,
    })), matrix.ColumnCount);
}

export function Metadata_Template_TemplateInfo_fromRows_9F97F2A(rows) {
    const tupledArg = SparseTable_FromRows_Z5579EC29(rows, Metadata_Template_TemplateInfo_get_Labels(), 0);
    return [tupledArg[0], Metadata_Template_TemplateInfo_FromSparseTable_3ECCA699(tupledArg[3])];
}

export function Metadata_Template_TemplateInfo_toRows_48C39BE1(template) {
    return SparseTable_ToRows_759CAFC1(Metadata_Template_TemplateInfo_ToSparseTable_48C39BE1(template));
}

export function Metadata_Template_mapDeprecatedKeys(rows) {
    const s = map_1((r) => map_1((tupledArg) => {
        const k = tupledArg[0] | 0;
        const v = tupledArg[1];
        if (k === 0) {
            switch (v) {
                case "#AUTHORS list":
                    return [k, "AUTHORS"];
                case "#ER list":
                    return [k, "ERS"];
                case "#TAGS list":
                    return [k, "TAGS"];
                case "Authors ORCID":
                    return [k, `Comment[${Person_orcidKey}]`];
                case "Authors Last Name":
                    return [k, "Author Last Name"];
                case "Authors First Name":
                    return [k, "Author First Name"];
                case "Authors Mid Initials":
                    return [k, "Author Mid Initials"];
                case "Authors Email":
                    return [k, "Author Email"];
                case "Authors Phone":
                    return [k, "Author Phone"];
                case "Authors Fax":
                    return [k, "Author Fax"];
                case "Authors Address":
                    return [k, "Author Address"];
                case "Authors Affiliation":
                    return [k, "Author Affiliation"];
                case "Authors Role":
                    return [k, "Author Roles"];
                case "Authors Role Term Accession Number":
                    return [k, "Author Roles Term Accession Number"];
                case "Authors Role Term Source REF":
                    return [k, "Author Roles Term Source REF"];
                default:
                    return [k, v];
            }
        }
        else {
            return [k, v];
        }
    }, r), rows);
    if (exists((v_32) => (v_32 === "TEMPLATE"), toArray(SparseRowModule_tryGetValueAt(0, head(s))))) {
        return s;
    }
    else {
        return append(singleton(SparseRowModule_fromValues(["TEMPLATE"])), s);
    }
}

export function Metadata_Template_fromRows(rows) {
    const loop = (en_mut, lastLine_mut, templateInfo_mut, ers_mut, tags_mut, authors_mut) => {
        loop:
        while (true) {
            const en = en_mut, lastLine = lastLine_mut, templateInfo = templateInfo_mut, ers = ers_mut, tags = tags_mut, authors = authors_mut;
            let matchResult, k_3, k_4, k_5, k_6;
            if (lastLine != null) {
                switch (lastLine) {
                    case "ERS": {
                        matchResult = 0;
                        k_3 = lastLine;
                        break;
                    }
                    case "TAGS": {
                        matchResult = 1;
                        k_4 = lastLine;
                        break;
                    }
                    case "AUTHORS": {
                        matchResult = 2;
                        k_5 = lastLine;
                        break;
                    }
                    default: {
                        matchResult = 3;
                        k_6 = lastLine;
                    }
                }
            }
            else {
                matchResult = 3;
                k_6 = lastLine;
            }
            switch (matchResult) {
                case 0: {
                    const patternInput = Metadata_ER_fromRows(undefined, en);
                    en_mut = en;
                    lastLine_mut = patternInput[0];
                    templateInfo_mut = templateInfo;
                    ers_mut = append_1(ers, patternInput[1]);
                    tags_mut = tags;
                    authors_mut = authors;
                    continue loop;
                }
                case 1: {
                    const patternInput_1 = Metadata_Tags_fromRows(undefined, en);
                    en_mut = en;
                    lastLine_mut = patternInput_1[0];
                    templateInfo_mut = templateInfo;
                    ers_mut = ers;
                    tags_mut = append_1(tags, patternInput_1[1]);
                    authors_mut = authors;
                    continue loop;
                }
                case 2: {
                    const patternInput_2 = fromRows_1("Author", 0, en);
                    en_mut = en;
                    lastLine_mut = patternInput_2[0];
                    templateInfo_mut = templateInfo;
                    ers_mut = ers;
                    tags_mut = tags;
                    authors_mut = append_1(authors, patternInput_2[3]);
                    continue loop;
                }
                default:
                    return [templateInfo, ers, tags, authors];
            }
            break;
        }
    };
    const en_1 = getEnumerator(Metadata_Template_mapDeprecatedKeys(rows));
    en_1["System.Collections.IEnumerator.MoveNext"]();
    const patternInput_3 = Metadata_Template_TemplateInfo_fromRows_9F97F2A(en_1);
    return loop(en_1, patternInput_3[0], patternInput_3[1], empty(), empty(), empty());
}

export function Metadata_Template_toRows(template) {
    return delay(() => append(singleton(SparseRowModule_fromValues(["TEMPLATE"])), delay(() => append(Metadata_Template_TemplateInfo_toRows_48C39BE1(template), delay(() => append(singleton(SparseRowModule_fromValues(["ERS"])), delay(() => append(Metadata_ER_toRows(undefined, toList(template.EndpointRepositories)), delay(() => append(singleton(SparseRowModule_fromValues(["TAGS"])), delay(() => append(Metadata_Tags_toRows(undefined, toList(template.Tags)), delay(() => append(singleton(SparseRowModule_fromValues(["AUTHORS"])), delay(() => toRows_1("Author", ofSeq(template.Authors)))))))))))))))));
}

export const Template_metadataSheetName = "isa_template";

export const Template_obsoleteMetadataSheetName = "SwateTemplateMetadata";

export function Template_fromParts(templateInfo, ers, tags, authors, table, lastUpdated) {
    const id = parse(templateInfo.Id);
    const organisation = Organisation.ofString(templateInfo.Organisation);
    const authors_1 = Array.from(authors);
    const repos = Array.from(ers);
    const tags_1 = Array.from(tags);
    return Template.make(id, table, templateInfo.Name, templateInfo.Description, organisation, templateInfo.Version, authors_1, repos, tags_1, lastUpdated);
}

export function Template_isMetadataSheetName(name) {
    if (name === Template_metadataSheetName) {
        return true;
    }
    else {
        return name === Template_obsoleteMetadataSheetName;
    }
}

export function Template_isMetadataSheet(sheet) {
    return Template_isMetadataSheetName(sheet.Name);
}

export function Template_tryGetMetadataSheet(doc) {
    return tryFind(Template_isMetadataSheet, doc.GetWorksheets());
}

export function Template_toMetadataSheet(template) {
    const sheet = new FsWorksheet(Template_metadataSheetName);
    iterateIndexed((rowI, r) => {
        SparseRowModule_writeToSheet(rowI + 1, r, sheet);
    }, Metadata_Template_toRows(template));
    return sheet;
}

export function Template_fromMetadataSheet(sheet) {
    return Metadata_Template_fromRows(map_1(SparseRowModule_fromFsRow, sheet.Rows));
}

export function Template_toMetadataCollection(template) {
    return map_1(SparseRowModule_getAllValues, Metadata_Template_toRows(template));
}

export function Template_fromMetadataCollection(collection) {
    return Metadata_Template_fromRows(map_1(SparseRowModule_fromAllValues, collection));
}

/**
 * Reads an assay from a spreadsheet
 */
export function Template_fromFsWorkbook(doc) {
    let matchValue_1, matchValue_3, ws_3, matchValue_4, ws_2, matchValue_2;
    let patternInput;
    const matchValue = Template_tryGetMetadataSheet(doc);
    if (matchValue == null) {
        toConsole(printf("Could not find metadata sheet with sheetname \"isa_template\" or deprecated sheetname \"Template\""));
        patternInput = [Metadata_Template_TemplateInfo_get_empty(), empty(), empty(), empty()];
    }
    else {
        patternInput = Template_fromMetadataSheet(matchValue);
    }
    const templateInfo = patternInput[0];
    const sheets = doc.GetWorksheets();
    return Template_fromParts(templateInfo, patternInput[1], patternInput[2], patternInput[3], (matchValue_1 = tryPick((ws) => {
        if (exists((t) => (t.Name === templateInfo.Table), ws.Tables)) {
            return ws;
        }
        else {
            return undefined;
        }
    }, sheets), (matchValue_1 == null) ? ((matchValue_3 = tryPick((ws_1) => {
        if (ws_1.Name === templateInfo.Table) {
            return ws_1;
        }
        else {
            return undefined;
        }
    }, sheets), (matchValue_3 == null) ? (() => {
        throw new TemplateReadError(`No worksheet or table with name \`${templateInfo.Table}\` found`);
    })() : ((ws_3 = matchValue_3, (matchValue_4 = tryFromFsWorksheet(ws_3), (matchValue_4 == null) ? (() => {
        throw new TemplateReadError(`Ws with name \`${ws_3.Name}\` could not be converted to a table`);
    })() : matchValue_4))))) : ((ws_2 = matchValue_1, (matchValue_2 = tryFromFsWorksheet(ws_2), (matchValue_2 == null) ? (() => {
        throw new TemplateReadError(`Ws with name \`${ws_2.Name}\` could not be converted to a table`);
    })() : matchValue_2)))), now());
}

export function Template_toFsWorkbook(template) {
    const doc = new FsWorkbook();
    const metaDataSheet = Template_toMetadataSheet(template);
    doc.AddWorksheet(metaDataSheet);
    const sheet = toFsWorksheet(undefined, template.Table);
    doc.AddWorksheet(sheet);
    return doc;
}

