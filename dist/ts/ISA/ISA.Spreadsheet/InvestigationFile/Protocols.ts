import { reverse, cons, iterate, iterateIndexed, map as map_1, initialize, singleton, length, empty, FSharpList, ofArray } from "../../../fable_modules/fable-library-ts/List.js";
import { OntologyAnnotation } from "../../ISA/JsonTypes/OntologyAnnotation.js";
import { Component_toAggregatedStrings, ProtocolParameter_toAggregatedStrings, Option_fromValueWithDefault, Component_fromAggregatedStrings, ProtocolParameter_fromAggregatedStrings } from "../Conversions.js";
import { ProtocolParameter } from "../../ISA/JsonTypes/ProtocolParameter.js";
import { Component } from "../../ISA/JsonTypes/Component.js";
import { Protocol_create_Z7DFD6E67, Protocol, Protocol_make } from "../../ISA/JsonTypes/Protocol.js";
import { value as value_3, defaultArg, Option, map } from "../../../fable_modules/fable-library-ts/Option.js";
import { URIModule_fromString } from "../../ISA/JsonTypes/URI.js";
import { Remark, Comment$ } from "../../ISA/JsonTypes/Comment.js";
import { SparseTable_ToRows_6A3D4534, SparseTable_FromRows_Z5579EC29, SparseTable_Create_Z2192E64B, SparseTable, SparseTable__TryGetValue_11FD62A8, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_651559CC } from "../SparseTable.js";
import { Comment_toString, Comment_fromString } from "../Comment.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { addToDict } from "../../../fable_modules/fable-library-ts/MapUtil.js";
import { List_distinct } from "../../../fable_modules/fable-library-ts/Seq2.js";
import { IEnumerator, stringHash } from "../../../fable_modules/fable-library-ts/Util.js";

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

export const labels: FSharpList<string> = ofArray([nameLabel, protocolTypeLabel, typeTermAccessionNumberLabel, typeTermSourceREFLabel, descriptionLabel, uriLabel, versionLabel, parametersNameLabel, parametersTermAccessionNumberLabel, parametersTermSourceREFLabel, componentsNameLabel, componentsTypeLabel, componentsTypeTermAccessionNumberLabel, componentsTypeTermSourceREFLabel]);

export function fromString(name: string, protocolType: string, typeTermAccessionNumber: Option<string>, typeTermSourceREF: Option<string>, description: string, uri: string, version: string, parametersName: string, parametersTermAccessionNumber: string, parametersTermSourceREF: string, componentsName: string, componentsType: string, componentsTypeTermAccessionNumber: string, componentsTypeTermSourceREF: string, comments: FSharpList<Comment$>): Protocol {
    const protocolType_1: OntologyAnnotation = OntologyAnnotation.fromString(protocolType, typeTermSourceREF, typeTermAccessionNumber);
    const parameters: FSharpList<ProtocolParameter> = ofArray<ProtocolParameter>(ProtocolParameter_fromAggregatedStrings(";", parametersName, parametersTermSourceREF, parametersTermAccessionNumber));
    const components: FSharpList<Component> = Component_fromAggregatedStrings(";", componentsName, componentsType, componentsTypeTermSourceREF, componentsTypeTermAccessionNumber);
    return Protocol_make(void 0, map<string, string>(URIModule_fromString, Option_fromValueWithDefault<string>("", name)), Option_fromValueWithDefault<OntologyAnnotation>(OntologyAnnotation.empty, protocolType_1), Option_fromValueWithDefault<string>("", description), map<string, string>(URIModule_fromString, Option_fromValueWithDefault<string>("", uri)), Option_fromValueWithDefault<string>("", version), Option_fromValueWithDefault<FSharpList<ProtocolParameter>>(empty<ProtocolParameter>(), parameters), Option_fromValueWithDefault<FSharpList<Component>>(empty<Component>(), components), Option_fromValueWithDefault<FSharpList<Comment$>>(empty<Comment$>(), comments));
}

export function fromSparseTable(matrix: SparseTable): FSharpList<Protocol> {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        return singleton(Protocol_create_Z7DFD6E67(void 0, void 0, void 0, void 0, void 0, void 0, void 0, void 0, ofArray<Comment$>(SparseTable_GetEmptyComments_651559CC(matrix))));
    }
    else {
        return initialize<Protocol>(matrix.ColumnCount, (i: int32): Protocol => {
            const comments_1: FSharpList<Comment$> = map_1<string, Comment$>((k: string): Comment$ => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i] as [string, int32])), matrix.CommentKeys);
            return fromString(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [nameLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [protocolTypeLabel, i] as [string, int32]), SparseTable__TryGetValue_11FD62A8(matrix, [typeTermAccessionNumberLabel, i] as [string, int32]), SparseTable__TryGetValue_11FD62A8(matrix, [typeTermSourceREFLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [descriptionLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [uriLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [versionLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [parametersNameLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [parametersTermAccessionNumberLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [parametersTermSourceREFLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [componentsNameLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [componentsTypeLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [componentsTypeTermAccessionNumberLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [componentsTypeTermSourceREFLabel, i] as [string, int32]), comments_1);
        });
    }
}

export function toSparseTable(protocols: FSharpList<Protocol>): SparseTable {
    const matrix: SparseTable = SparseTable_Create_Z2192E64B(void 0, labels, void 0, length(protocols) + 1);
    let commentKeys: FSharpList<string> = empty<string>();
    iterateIndexed<Protocol>((i: int32, p: Protocol): void => {
        const i_1: int32 = (i + 1) | 0;
        let pt_1: { TermAccessionNumber: string, TermName: string, TermSourceREF: string };
        const pt: OntologyAnnotation = defaultArg(p.ProtocolType, OntologyAnnotation.empty);
        pt_1 = OntologyAnnotation.toString(pt, true);
        const pAgg: { TermAccessionNumberAgg: string, TermNameAgg: string, TermSourceREFAgg: string } = ProtocolParameter_toAggregatedStrings(";", defaultArg(p.Parameters, empty<ProtocolParameter>()));
        const cAgg: { NameAgg: string, TermAccessionNumberAgg: string, TermNameAgg: string, TermSourceREFAgg: string } = Component_toAggregatedStrings(";", defaultArg(p.Components, empty<Component>()));
        addToDict(matrix.Matrix, [nameLabel, i_1] as [string, int32], defaultArg(p.Name, ""));
        addToDict(matrix.Matrix, [protocolTypeLabel, i_1] as [string, int32], pt_1.TermName);
        addToDict(matrix.Matrix, [typeTermAccessionNumberLabel, i_1] as [string, int32], pt_1.TermAccessionNumber);
        addToDict(matrix.Matrix, [typeTermSourceREFLabel, i_1] as [string, int32], pt_1.TermSourceREF);
        addToDict(matrix.Matrix, [descriptionLabel, i_1] as [string, int32], defaultArg(p.Description, ""));
        addToDict(matrix.Matrix, [uriLabel, i_1] as [string, int32], defaultArg(p.Uri, ""));
        addToDict(matrix.Matrix, [versionLabel, i_1] as [string, int32], defaultArg(p.Version, ""));
        addToDict(matrix.Matrix, [parametersNameLabel, i_1] as [string, int32], pAgg.TermNameAgg);
        addToDict(matrix.Matrix, [parametersTermAccessionNumberLabel, i_1] as [string, int32], pAgg.TermAccessionNumberAgg);
        addToDict(matrix.Matrix, [parametersTermSourceREFLabel, i_1] as [string, int32], pAgg.TermSourceREFAgg);
        addToDict(matrix.Matrix, [componentsNameLabel, i_1] as [string, int32], cAgg.NameAgg);
        addToDict(matrix.Matrix, [componentsTypeLabel, i_1] as [string, int32], cAgg.TermNameAgg);
        addToDict(matrix.Matrix, [componentsTypeTermAccessionNumberLabel, i_1] as [string, int32], cAgg.TermAccessionNumberAgg);
        addToDict(matrix.Matrix, [componentsTypeTermSourceREFLabel, i_1] as [string, int32], cAgg.TermSourceREFAgg);
        const matchValue: Option<FSharpList<Comment$>> = p.Comments;
        if (matchValue != null) {
            iterate<Comment$>((comment: Comment$): void => {
                const patternInput: [string, string] = Comment_toString(comment);
                const n: string = patternInput[0];
                commentKeys = cons(n, commentKeys);
                addToDict(matrix.Matrix, [n, i_1] as [string, int32], patternInput[1]);
            }, value_3(matchValue));
        }
    }, protocols);
    return new SparseTable(matrix.Matrix, matrix.Keys, reverse<string>(List_distinct<string>(commentKeys, {
        Equals: (x: string, y: string): boolean => (x === y),
        GetHashCode: stringHash,
    })), matrix.ColumnCount);
}

export function fromRows(prefix: Option<string>, lineNumber: int32, rows: IEnumerator<Iterable<[int32, string]>>): [Option<string>, int32, FSharpList<Remark>, FSharpList<Protocol>] {
    const tupledArg: [Option<string>, int32, FSharpList<Remark>, SparseTable] = (prefix == null) ? SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber) : SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber, value_3(prefix));
    return [tupledArg[0], tupledArg[1], tupledArg[2], fromSparseTable(tupledArg[3])] as [Option<string>, int32, FSharpList<Remark>, FSharpList<Protocol>];
}

export function toRows(prefix: Option<string>, protocols: FSharpList<Protocol>): Iterable<Iterable<[int32, string]>> {
    const m: SparseTable = toSparseTable(protocols);
    if (prefix == null) {
        return SparseTable_ToRows_6A3D4534(m);
    }
    else {
        return SparseTable_ToRows_6A3D4534(m, value_3(prefix));
    }
}

