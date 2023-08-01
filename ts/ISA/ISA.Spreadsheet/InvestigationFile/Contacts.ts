import { reverse, cons, iterate, iterateIndexed, map, initialize, singleton, length, empty, FSharpList, ofArray } from "../../../fable_modules/fable-library-ts/List.js";
import { OntologyAnnotation_toAggregatedStrings, Option_fromValueWithDefault, OntologyAnnotation_fromAggregatedStrings } from "../Conversions.js";
import { OntologyAnnotation } from "../../ISA/JsonTypes/OntologyAnnotation.js";
import { Person_create_28E835CB, Person, Person_make } from "../../ISA/JsonTypes/Person.js";
import { Remark, Comment$ } from "../../ISA/JsonTypes/Comment.js";
import { SparseTable_ToRows_584133C0, SparseTable_FromRows_Z5579EC29, SparseTable_Create_Z2192E64B, SparseTable, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_Z15A4F148 } from "../SparseTable.js";
import { Comment_toString, Comment_fromString } from "../Comment.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { unwrap, value as value_1, Option, defaultArg } from "../../../fable_modules/fable-library-ts/Option.js";
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

export function fromString(lastName: string, firstName: string, midInitials: string, email: string, phone: string, fax: string, address: string, affiliation: string, role: string, rolesTermAccessionNumber: string, rolesTermSourceREF: string, comments: FSharpList<Comment$>): Person {
    const roles: FSharpList<OntologyAnnotation> = OntologyAnnotation_fromAggregatedStrings(";", role, rolesTermSourceREF, rolesTermAccessionNumber);
    return Person_make(void 0, Option_fromValueWithDefault<string>("", lastName), Option_fromValueWithDefault<string>("", firstName), Option_fromValueWithDefault<string>("", midInitials), Option_fromValueWithDefault<string>("", email), Option_fromValueWithDefault<string>("", phone), Option_fromValueWithDefault<string>("", fax), Option_fromValueWithDefault<string>("", address), Option_fromValueWithDefault<string>("", affiliation), Option_fromValueWithDefault<FSharpList<OntologyAnnotation>>(empty<OntologyAnnotation>(), roles), Option_fromValueWithDefault<FSharpList<Comment$>>(empty<Comment$>(), comments));
}

export function fromSparseTable(matrix: SparseTable): FSharpList<Person> {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        return singleton(Person_create_28E835CB(void 0, void 0, void 0, void 0, void 0, void 0, void 0, void 0, void 0, void 0, SparseTable_GetEmptyComments_Z15A4F148(matrix)));
    }
    else {
        return initialize<Person>(matrix.ColumnCount, (i: int32): Person => {
            const comments_1: FSharpList<Comment$> = map<string, Comment$>((k: string): Comment$ => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i] as [string, int32])), matrix.CommentKeys);
            return fromString(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [lastNameLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [firstNameLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [midInitialsLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [emailLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [phoneLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [faxLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [addressLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [affiliationLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [rolesLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [rolesTermAccessionNumberLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [rolesTermSourceREFLabel, i] as [string, int32]), comments_1);
        });
    }
}

export function toSparseTable(persons: FSharpList<Person>): SparseTable {
    const matrix: SparseTable = SparseTable_Create_Z2192E64B(void 0, labels, void 0, length(persons) + 1);
    let commentKeys: FSharpList<string> = empty<string>();
    iterateIndexed<Person>((i: int32, p: Person): void => {
        const i_1: int32 = (i + 1) | 0;
        const rAgg: { TermAccessionNumberAgg: string, TermNameAgg: string, TermSourceREFAgg: string } = OntologyAnnotation_toAggregatedStrings(";", defaultArg(p.Roles, empty<OntologyAnnotation>()));
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
        const matchValue: Option<FSharpList<Comment$>> = p.Comments;
        if (matchValue != null) {
            iterate<Comment$>((comment: Comment$): void => {
                const patternInput: [string, string] = Comment_toString(comment);
                const n: string = patternInput[0];
                commentKeys = cons(n, commentKeys);
                addToDict(matrix.Matrix, [n, i_1] as [string, int32], patternInput[1]);
            }, value_1(matchValue));
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
        return SparseTable_ToRows_584133C0(m);
    }
    else {
        return SparseTable_ToRows_584133C0(m, value_1(prefix));
    }
}

