import { reverse, cons, iterateIndexed, empty, map, initialize, singleton, length, ofArray } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { Person_setCommentFromORCID, Person_setOrcidFromComments } from "../../Core/Conversion.js";
import { OntologyAnnotation_toAggregatedStrings, OntologyAnnotation_fromAggregatedStrings } from "./Conversions.js";
import { Person } from "../../Core/Person.js";
import { SparseTable_ToRows_759CAFC1, SparseTable_FromRows_Z5579EC29, SparseTable, SparseTable_Create_Z2192E64B, SparseTable__TryGetValue_11FD62A8, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_3ECCA699 } from "./SparseTable.js";
import { Comment_toString, Comment_fromString } from "./Comment.js";
import { addToDict } from "../../fable_modules/fable-library-js.4.22.0/MapUtil.js";
import { unwrap, defaultArg } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { ResizeArray_iter } from "../../Core/Helper/Collections.js";
import { List_distinct } from "../../fable_modules/fable-library-js.4.22.0/Seq2.js";
import { stringHash } from "../../fable_modules/fable-library-js.4.22.0/Util.js";

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
    let roles_1;
    return Person_setOrcidFromComments((roles_1 = OntologyAnnotation_fromAggregatedStrings(";", role, rolesTermSourceREF, rolesTermAccessionNumber), Person.make(undefined, lastName, firstName, midInitials, email, phone, fax, address, affiliation, roles_1, comments)));
}

export function fromSparseTable(matrix) {
    let returnVal;
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        const comments = SparseTable_GetEmptyComments_3ECCA699(matrix);
        return singleton((returnVal = Person.create(), (returnVal.Comments = comments, returnVal)));
    }
    else {
        return initialize(matrix.ColumnCount, (i) => {
            let comments_1;
            const collection = map((k) => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i])), matrix.CommentKeys);
            comments_1 = Array.from(collection);
            return fromString(SparseTable__TryGetValue_11FD62A8(matrix, [lastNameLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [firstNameLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [midInitialsLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [emailLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [phoneLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [faxLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [addressLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [affiliationLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [rolesLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [rolesTermAccessionNumberLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [rolesTermSourceREFLabel, i]), comments_1);
        });
    }
}

export function toSparseTable(persons) {
    const matrix = SparseTable_Create_Z2192E64B(undefined, labels, undefined, length(persons) + 1);
    let commentKeys = empty();
    iterateIndexed((i, p) => {
        const i_1 = (i + 1) | 0;
        const rAgg = OntologyAnnotation_toAggregatedStrings(";", Array.from(p.Roles));
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
        ResizeArray_iter((comment) => {
            const patternInput = Comment_toString(comment);
            const n = patternInput[0];
            commentKeys = cons(n, commentKeys);
            addToDict(matrix.Matrix, [n, i_1], patternInput[1]);
        }, p.Comments);
    }, map(Person_setCommentFromORCID, persons));
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
        return SparseTable_ToRows_759CAFC1(m);
    }
    else {
        return SparseTable_ToRows_759CAFC1(m, prefix);
    }
}

