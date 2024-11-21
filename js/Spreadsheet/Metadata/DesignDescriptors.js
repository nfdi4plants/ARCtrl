import { ofArray } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { toRows as toRows_1, fromRows as fromRows_1, toSparseTable as toSparseTable_1, fromSparseTable as fromSparseTable_1 } from "./OntologyAnnotation.js";

export const designTypeLabel = "Type";

export const designTypeTermAccessionNumberLabel = "Type Term Accession Number";

export const designTypeTermSourceREFLabel = "Type Term Source REF";

export const labels = ofArray([designTypeLabel, designTypeTermAccessionNumberLabel, designTypeTermSourceREFLabel]);

export function fromSparseTable(matrix) {
    return fromSparseTable_1(designTypeLabel, designTypeTermSourceREFLabel, designTypeTermAccessionNumberLabel, matrix);
}

export function toSparseTable(designs) {
    return toSparseTable_1(designTypeLabel, designTypeTermSourceREFLabel, designTypeTermAccessionNumberLabel, designs);
}

export function fromRows(prefix, lineNumber, rows) {
    return fromRows_1(prefix, designTypeLabel, designTypeTermSourceREFLabel, designTypeTermAccessionNumberLabel, lineNumber, rows);
}

export function toRows(prefix, designs) {
    return toRows_1(prefix, designTypeLabel, designTypeTermSourceREFLabel, designTypeTermAccessionNumberLabel, designs);
}

