import { reverse, cons, iterate, iterateIndexed, map as map_1, initialize, singleton, length, empty, ofArray } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { OntologyAnnotation_toString_473B9D79, OntologyAnnotation_get_empty, OntologyAnnotation_fromString_Z7D8EB286 } from "../../ISA/JsonTypes/OntologyAnnotation.js";
import { Component_toAggregatedStrings, ProtocolParameter_toAggregatedStrings, Option_fromValueWithDefault, Component_fromAggregatedStrings, ProtocolParameter_fromAggregatedStrings } from "../Conversions.js";
import { Protocol_create_Z4D717767, Protocol_make } from "../../ISA/JsonTypes/Protocol.js";
import { defaultArg, map } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { URIModule_fromString } from "../../ISA/JsonTypes/URI.js";
import { SparseTable_ToRows_584133C0, SparseTable_FromRows_Z5579EC29, SparseTable, SparseTable_Create_Z2192E64B, SparseTable__TryGetValue_11FD62A8, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_Z15A4F148 } from "../SparseTable.js";
import { Comment_toString, Comment_fromString } from "../Comment.js";
import { addToDict } from "../../../fable_modules/fable-library.4.1.4/MapUtil.js";
import { List_distinct } from "../../../fable_modules/fable-library.4.1.4/Seq2.js";
import { stringHash } from "../../../fable_modules/fable-library.4.1.4/Util.js";

export const nameLabel = "Name";

export const protocolTypeLabel = "Type";

export const typeTermAccessionNumberLabel = "Type Term Accession Number";

export const typeTermSourceREFLabel = "Type Term Source REF";

export const descriptionLabel = "Description";

export const uriLabel = "URI";

export const versionLabel = "Version";

export const parametersNameLabel = "Parameters Name";

export const parametersTermAccessionNumberLabel = "Parameters Term Accession Number";

export const parametersTermSourceREFLabel = "Parameters Term Source REF";

export const componentsNameLabel = "Components Name";

export const componentsTypeLabel = "Components Type";

export const componentsTypeTermAccessionNumberLabel = "Components Type Term Accession Number";

export const componentsTypeTermSourceREFLabel = "Components Type Term Source REF";

export const labels = ofArray([nameLabel, protocolTypeLabel, typeTermAccessionNumberLabel, typeTermSourceREFLabel, descriptionLabel, uriLabel, versionLabel, parametersNameLabel, parametersTermAccessionNumberLabel, parametersTermSourceREFLabel, componentsNameLabel, componentsTypeLabel, componentsTypeTermAccessionNumberLabel, componentsTypeTermSourceREFLabel]);

export function fromString(name, protocolType, typeTermAccessionNumber, typeTermSourceREF, description, uri, version, parametersName, parametersTermAccessionNumber, parametersTermSourceREF, componentsName, componentsType, componentsTypeTermAccessionNumber, componentsTypeTermSourceREF, comments) {
    const protocolType_1 = OntologyAnnotation_fromString_Z7D8EB286(protocolType, typeTermSourceREF, typeTermAccessionNumber);
    const parameters = ProtocolParameter_fromAggregatedStrings(";", parametersName, parametersTermSourceREF, parametersTermAccessionNumber);
    const components = Component_fromAggregatedStrings(";", componentsName, componentsType, componentsTypeTermSourceREF, componentsTypeTermAccessionNumber);
    return Protocol_make(void 0, map(URIModule_fromString, Option_fromValueWithDefault("", name)), Option_fromValueWithDefault(OntologyAnnotation_get_empty(), protocolType_1), Option_fromValueWithDefault("", description), map(URIModule_fromString, Option_fromValueWithDefault("", uri)), Option_fromValueWithDefault("", version), Option_fromValueWithDefault(empty(), parameters), Option_fromValueWithDefault(empty(), components), Option_fromValueWithDefault(empty(), comments));
}

export function fromSparseTable(matrix) {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        return singleton(Protocol_create_Z4D717767(void 0, void 0, void 0, void 0, void 0, void 0, void 0, void 0, SparseTable_GetEmptyComments_Z15A4F148(matrix)));
    }
    else {
        return initialize(matrix.ColumnCount, (i) => {
            const comments_1 = map_1((k) => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i])), matrix.CommentKeys);
            return fromString(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [nameLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [protocolTypeLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [typeTermAccessionNumberLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [typeTermSourceREFLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [descriptionLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [uriLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [versionLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [parametersNameLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [parametersTermAccessionNumberLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [parametersTermSourceREFLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [componentsNameLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [componentsTypeLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [componentsTypeTermAccessionNumberLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [componentsTypeTermSourceREFLabel, i]), comments_1);
        });
    }
}

export function toSparseTable(protocols) {
    const matrix = SparseTable_Create_Z2192E64B(void 0, labels, void 0, length(protocols) + 1);
    let commentKeys = empty();
    iterateIndexed((i, p) => {
        const i_1 = (i + 1) | 0;
        const pt_1 = OntologyAnnotation_toString_473B9D79(defaultArg(p.ProtocolType, OntologyAnnotation_get_empty()), true);
        const pAgg = ProtocolParameter_toAggregatedStrings(";", defaultArg(p.Parameters, empty()));
        const cAgg = Component_toAggregatedStrings(";", defaultArg(p.Components, empty()));
        addToDict(matrix.Matrix, [nameLabel, i_1], defaultArg(p.Name, ""));
        addToDict(matrix.Matrix, [protocolTypeLabel, i_1], pt_1.TermName);
        addToDict(matrix.Matrix, [typeTermAccessionNumberLabel, i_1], pt_1.TermAccessionNumber);
        addToDict(matrix.Matrix, [typeTermSourceREFLabel, i_1], pt_1.TermSourceREF);
        addToDict(matrix.Matrix, [descriptionLabel, i_1], defaultArg(p.Description, ""));
        addToDict(matrix.Matrix, [uriLabel, i_1], defaultArg(p.Uri, ""));
        addToDict(matrix.Matrix, [versionLabel, i_1], defaultArg(p.Version, ""));
        addToDict(matrix.Matrix, [parametersNameLabel, i_1], pAgg.TermNameAgg);
        addToDict(matrix.Matrix, [parametersTermAccessionNumberLabel, i_1], pAgg.TermAccessionNumberAgg);
        addToDict(matrix.Matrix, [parametersTermSourceREFLabel, i_1], pAgg.TermSourceREFAgg);
        addToDict(matrix.Matrix, [componentsNameLabel, i_1], cAgg.NameAgg);
        addToDict(matrix.Matrix, [componentsTypeLabel, i_1], cAgg.TermNameAgg);
        addToDict(matrix.Matrix, [componentsTypeTermAccessionNumberLabel, i_1], cAgg.TermAccessionNumberAgg);
        addToDict(matrix.Matrix, [componentsTypeTermSourceREFLabel, i_1], cAgg.TermSourceREFAgg);
        const matchValue = p.Comments;
        if (matchValue != null) {
            iterate((comment) => {
                const patternInput = Comment_toString(comment);
                const n = patternInput[0];
                commentKeys = cons(n, commentKeys);
                addToDict(matrix.Matrix, [n, i_1], patternInput[1]);
            }, matchValue);
        }
    }, protocols);
    return new SparseTable(matrix.Matrix, matrix.Keys, reverse(List_distinct(commentKeys, {
        Equals: (x, y) => (x === y),
        GetHashCode: stringHash,
    })), matrix.ColumnCount);
}

export function fromRows(prefix, lineNumber, rows) {
    const tupledArg = (prefix == null) ? SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber) : SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber, prefix);
    return [tupledArg[0], tupledArg[1], tupledArg[2], fromSparseTable(tupledArg[3])];
}

export function toRows(prefix, protocols) {
    const m = toSparseTable(protocols);
    if (prefix == null) {
        return SparseTable_ToRows_584133C0(m);
    }
    else {
        return SparseTable_ToRows_584133C0(m, prefix);
    }
}

