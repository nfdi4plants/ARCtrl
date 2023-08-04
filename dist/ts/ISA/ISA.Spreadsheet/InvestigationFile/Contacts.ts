import { reverse, cons, iterateIndexed, empty, map, toArray, initialize, singleton, length, FSharpList, ofArray } from "../../../fable_modules/fable-library-ts/List.js";
import { OntologyAnnotation_toAggregatedStrings, Option_fromValueWithDefault, OntologyAnnotation_fromAggregatedStrings } from "../Conversions.js";
import { OntologyAnnotation } from "../../ISA/JsonTypes/OntologyAnnotation.js";
import { unwrap, value as value_1, defaultArg, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { Remark, Comment$ } from "../../ISA/JsonTypes/Comment.js";
import { Person } from "../../ISA/JsonTypes/Person.js";
import { SparseTable_ToRows_6A3D4534, SparseTable_FromRows_Z5579EC29, SparseTable_Create_Z2192E64B, SparseTable, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_651559CC } from "../SparseTable.js";
import { Comment_toString, Comment_fromString } from "../Comment.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { addToDict } from "../../../fable_modules/fable-library-ts/MapUtil.js";
import { List_distinct } from "../../../fable_modules/fable-library-ts/Seq2.js";
import { IEnumerator, stringHash } from "../../../fable_modules/fable-library-ts/Util.js";

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

export const labels: FSharpList<string> = ofArray([lastNameLabel, firstNameLabel, midInitialsLabel, emailLabel, phoneLabel, faxLabel, addressLabel, affiliationLabel, rolesLabel, rolesTermAccessionNumberLabel, rolesTermSourceREFLabel]);

export function fromString(lastName: string, firstName: string, midInitials: string, email: string, phone: string, fax: string, address: string, affiliation: string, role: string, rolesTermAccessionNumber: string, rolesTermSourceREF: string, comments: Comment$[]): Person {
    const roles: OntologyAnnotation[] = OntologyAnnotation_fromAggregatedStrings(";", role, rolesTermSourceREF, rolesTermAccessionNumber);
    const arg_1: Option<string> = Option_fromValueWithDefault<string>("", lastName);
    const arg_2: Option<string> = Option_fromValueWithDefault<string>("", firstName);
    const arg_3: Option<string> = Option_fromValueWithDefault<string>("", midInitials);
    const arg_4: Option<string> = Option_fromValueWithDefault<string>("", email);
    const arg_5: Option<string> = Option_fromValueWithDefault<string>("", phone);
    const arg_6: Option<string> = Option_fromValueWithDefault<string>("", fax);
    const arg_7: Option<string> = Option_fromValueWithDefault<string>("", address);
    const arg_8: Option<string> = Option_fromValueWithDefault<string>("", affiliation);
    const arg_9: Option<OntologyAnnotation[]> = Option_fromValueWithDefault<OntologyAnnotation[]>([], roles);
    const arg_10: Option<Comment$[]> = Option_fromValueWithDefault<Comment$[]>([], comments);
    return Person.make(void 0, arg_1, arg_2, arg_3, arg_4, arg_5, arg_6, arg_7, arg_8, arg_9, arg_10);
}

export function fromSparseTable(matrix: SparseTable): FSharpList<Person> {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        const comments: Comment$[] = SparseTable_GetEmptyComments_651559CC(matrix);
        return singleton(Person.create(void 0, void 0, void 0, void 0, void 0, void 0, void 0, void 0, void 0, void 0, comments));
    }
    else {
        return initialize<Person>(matrix.ColumnCount, (i: int32): Person => {
            const comments_1: Comment$[] = toArray<Comment$>(map<string, Comment$>((k: string): Comment$ => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i] as [string, int32])), matrix.CommentKeys));
            return fromString(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [lastNameLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [firstNameLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [midInitialsLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [emailLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [phoneLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [faxLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [addressLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [affiliationLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [rolesLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [rolesTermAccessionNumberLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [rolesTermSourceREFLabel, i] as [string, int32]), comments_1);
        });
    }
}

export function toSparseTable(persons: FSharpList<Person>): SparseTable {
    const matrix: SparseTable = SparseTable_Create_Z2192E64B(void 0, labels, void 0, length(persons) + 1);
    let commentKeys: FSharpList<string> = empty<string>();
    iterateIndexed<Person>((i: int32, p: Person): void => {
        const i_1: int32 = (i + 1) | 0;
        const rAgg: { TermAccessionNumberAgg: string, TermNameAgg: string, TermSourceREFAgg: string } = OntologyAnnotation_toAggregatedStrings(";", defaultArg(p.Roles, []));
        addToDict(matrix.Matrix, [lastNameLabel, i_1] as [string, int32], defaultArg(p.LastName, ""));
        addToDict(matrix.Matrix, [firstNameLabel, i_1] as [string, int32], defaultArg(p.FirstName, ""));
        addToDict(matrix.Matrix, [midInitialsLabel, i_1] as [string, int32], defaultArg(p.MidInitials, ""));
        addToDict(matrix.Matrix, [emailLabel, i_1] as [string, int32], defaultArg(p.EMail, ""));
        addToDict(matrix.Matrix, [phoneLabel, i_1] as [string, int32], defaultArg(p.Phone, ""));
        addToDict(matrix.Matrix, [faxLabel, i_1] as [string, int32], defaultArg(p.Fax, ""));
        addToDict(matrix.Matrix, [addressLabel, i_1] as [string, int32], defaultArg(p.Address, ""));
        addToDict(matrix.Matrix, [affiliationLabel, i_1] as [string, int32], defaultArg(p.Affiliation, ""));
        addToDict(matrix.Matrix, [rolesLabel, i_1] as [string, int32], rAgg.TermNameAgg);
        addToDict(matrix.Matrix, [rolesTermAccessionNumberLabel, i_1] as [string, int32], rAgg.TermAccessionNumberAgg);
        addToDict(matrix.Matrix, [rolesTermSourceREFLabel, i_1] as [string, int32], rAgg.TermSourceREFAgg);
        const matchValue: Option<Comment$[]> = p.Comments;
        if (matchValue != null) {
            const array: Comment$[] = value_1(matchValue);
            array.forEach((comment: Comment$): void => {
                const patternInput: [string, string] = Comment_toString(comment);
                const n: string = patternInput[0];
                commentKeys = cons(n, commentKeys);
                addToDict(matrix.Matrix, [n, i_1] as [string, int32], patternInput[1]);
            });
        }
    }, persons);
    return new SparseTable(matrix.Matrix, matrix.Keys, reverse<string>(List_distinct<string>(commentKeys, {
        Equals: (x: string, y: string): boolean => (x === y),
        GetHashCode: stringHash,
    })), matrix.ColumnCount);
}

export function fromRows(prefix: Option<string>, lineNumber: int32, rows: IEnumerator<Iterable<[int32, string]>>): [Option<string>, int32, FSharpList<Remark>, FSharpList<Person>] {
    const tupledArg: [Option<string>, int32, FSharpList<Remark>, SparseTable] = SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber, unwrap(prefix));
    return [tupledArg[0], tupledArg[1], tupledArg[2], fromSparseTable(tupledArg[3])] as [Option<string>, int32, FSharpList<Remark>, FSharpList<Person>];
}

export function toRows(prefix: Option<string>, persons: FSharpList<Person>): Iterable<Iterable<[int32, string]>> {
    const m: SparseTable = toSparseTable(persons);
    if (prefix == null) {
        return SparseTable_ToRows_6A3D4534(m);
    }
    else {
        return SparseTable_ToRows_6A3D4534(m, value_1(prefix));
    }
}

