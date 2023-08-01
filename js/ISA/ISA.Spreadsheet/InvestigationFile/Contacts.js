import { reverse, cons, iterate, iterateIndexed, map, initialize, singleton, length, empty, ofArray } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { OntologyAnnotation_toAggregatedStrings, Option_fromValueWithDefault, OntologyAnnotation_fromAggregatedStrings } from "../Conversions.js";
import { Person_create_28E835CB, Person_make } from "../../ISA/JsonTypes/Person.js";
import { SparseTable_ToRows_584133C0, SparseTable_FromRows_Z5579EC29, SparseTable, SparseTable_Create_Z2192E64B, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_Z15A4F148 } from "../SparseTable.js";
import { Comment_toString, Comment_fromString } from "../Comment.js";
import { unwrap, defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { addToDict } from "../../../fable_modules/fable-library.4.1.4/MapUtil.js";
import { List_distinct } from "../../../fable_modules/fable-library.4.1.4/Seq2.js";
import { stringHash } from "../../../fable_modules/fable-library.4.1.4/Util.js";

export const lastNameLabel = "Last Name";

export const firstNameLabel = "First Name";

export const midInitialsLabel = "Mid Initials";

export const emailLabel = "Email";

export const phoneLabel = "Phone";

export const faxLabel = "Fax";

export const addressLabel = "Address";

export const affiliationLabel = "Affiliation";

export const rolesLabel = "Roles";

export const rolesTermAccessionNumberLabel = "Roles Term Accession Number";

export const rolesTermSourceREFLabel = "Roles Term Source REF";

export const labels = ofArray([lastNameLabel, firstNameLabel, midInitialsLabel, emailLabel, phoneLabel, faxLabel, addressLabel, affiliationLabel, rolesLabel, rolesTermAccessionNumberLabel, rolesTermSourceREFLabel]);

export function fromString(lastName, firstName, midInitials, email, phone, fax, address, affiliation, role, rolesTermAccessionNumber, rolesTermSourceREF, comments) {
    const roles = OntologyAnnotation_fromAggregatedStrings(";", role, rolesTermSourceREF, rolesTermAccessionNumber);
    return Person_make(void 0, Option_fromValueWithDefault("", lastName), Option_fromValueWithDefault("", firstName), Option_fromValueWithDefault("", midInitials), Option_fromValueWithDefault("", email), Option_fromValueWithDefault("", phone), Option_fromValueWithDefault("", fax), Option_fromValueWithDefault("", address), Option_fromValueWithDefault("", affiliation), Option_fromValueWithDefault(empty(), roles), Option_fromValueWithDefault(empty(), comments));
}

export function fromSparseTable(matrix) {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        return singleton(Person_create_28E835CB(void 0, void 0, void 0, void 0, void 0, void 0, void 0, void 0, void 0, void 0, SparseTable_GetEmptyComments_Z15A4F148(matrix)));
    }
    else {
        return initialize(matrix.ColumnCount, (i) => {
            const comments_1 = map((k) => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i])), matrix.CommentKeys);
            return fromString(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [lastNameLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [firstNameLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [midInitialsLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [emailLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [phoneLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [faxLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [addressLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [affiliationLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [rolesLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [rolesTermAccessionNumberLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [rolesTermSourceREFLabel, i]), comments_1);
        });
    }
}

export function toSparseTable(persons) {
    const matrix = SparseTable_Create_Z2192E64B(void 0, labels, void 0, length(persons) + 1);
    let commentKeys = empty();
    iterateIndexed((i, p) => {
        const i_1 = (i + 1) | 0;
        const rAgg = OntologyAnnotation_toAggregatedStrings(";", defaultArg(p.Roles, empty()));
        addToDict(matrix.Matrix, [lastNameLabel, i_1], defaultArg(p.LastName, ""));
        addToDict(matrix.Matrix, [firstNameLabel, i_1], defaultArg(p.FirstName, ""));
        addToDict(matrix.Matrix, [midInitialsLabel, i_1], defaultArg(p.MidInitials, ""));
        addToDict(matrix.Matrix, [emailLabel, i_1], defaultArg(p.EMail, ""));
        addToDict(matrix.Matrix, [phoneLabel, i_1], defaultArg(p.Phone, ""));
        addToDict(matrix.Matrix, [faxLabel, i_1], defaultArg(p.Fax, ""));
        addToDict(matrix.Matrix, [addressLabel, i_1], defaultArg(p.Address, ""));
        addToDict(matrix.Matrix, [affiliationLabel, i_1], defaultArg(p.Affiliation, ""));
        addToDict(matrix.Matrix, [rolesLabel, i_1], rAgg.TermNameAgg);
        addToDict(matrix.Matrix, [rolesTermAccessionNumberLabel, i_1], rAgg.TermAccessionNumberAgg);
        addToDict(matrix.Matrix, [rolesTermSourceREFLabel, i_1], rAgg.TermSourceREFAgg);
        const matchValue = p.Comments;
        if (matchValue != null) {
            iterate((comment) => {
                const patternInput = Comment_toString(comment);
                const n = patternInput[0];
                commentKeys = cons(n, commentKeys);
                addToDict(matrix.Matrix, [n, i_1], patternInput[1]);
            }, matchValue);
        }
    }, persons);
    return new SparseTable(matrix.Matrix, matrix.Keys, reverse(List_distinct(commentKeys, {
        Equals: (x, y) => (x === y),
        GetHashCode: stringHash,
    })), matrix.ColumnCount);
}

export function fromRows(prefix, lineNumber, rows) {
    const tupledArg = SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber, unwrap(prefix));
    return [tupledArg[0], tupledArg[1], tupledArg[2], fromSparseTable(tupledArg[3])];
}

export function toRows(prefix, persons) {
    const m = toSparseTable(persons);
    if (prefix == null) {
        return SparseTable_ToRows_584133C0(m);
    }
    else {
        return SparseTable_ToRows_584133C0(m, prefix);
    }
}

