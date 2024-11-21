import { Dictionary_tryFind, Dictionary_ofSeq } from "./Collections.js";

export function OntobeeParser(tsr, localTan) {
    return `${"http://purl.obolibrary.org/obo/"}${tsr}_${localTan}`;
}

export function BioregistryParser(tsr, localTan) {
    return `${"https://bioregistry.io/"}${tsr}:${localTan}`;
}

export function OntobeeDPBOParser(tsr, localTan) {
    return `${"http://purl.org/nfdi4plants/ontology/dpbo/"}${tsr}_${localTan}`;
}

export function MSParser(tsr, localTan) {
    return `${"https://www.ebi.ac.uk/ols4/ontologies/ms/classes/http%253A%252F%252Fpurl.obolibrary.org%252Fobo%252F"}${tsr}_${localTan}`;
}

export function POParser(tsr, localTan) {
    return `${"https://www.ebi.ac.uk/ols4/ontologies/po/classes/http%253A%252F%252Fpurl.obolibrary.org%252Fobo%252F"}${tsr}_${localTan}`;
}

export function ROParser(tsr, localTan) {
    return `${"https://www.ebi.ac.uk/ols4/ontologies/ro/classes/http%253A%252F%252Fpurl.obolibrary.org%252Fobo%252F"}${tsr}_${localTan}`;
}

export const uriParserCollection = Dictionary_ofSeq([["DPBO", (tsr) => ((localTan) => OntobeeDPBOParser(tsr, localTan))], ["MS", (tsr_1) => ((localTan_1) => MSParser(tsr_1, localTan_1))], ["PO", (tsr_2) => ((localTan_2) => POParser(tsr_2, localTan_2))], ["RO", (tsr_3) => ((localTan_3) => ROParser(tsr_3, localTan_3))], ["ENVO", (tsr_4) => ((localTan_4) => BioregistryParser(tsr_4, localTan_4))], ["CHEBI", (tsr_5) => ((localTan_5) => BioregistryParser(tsr_5, localTan_5))], ["GO", (tsr_6) => ((localTan_6) => BioregistryParser(tsr_6, localTan_6))], ["OBI", (tsr_7) => ((localTan_7) => BioregistryParser(tsr_7, localTan_7))], ["PATO", (tsr_8) => ((localTan_8) => BioregistryParser(tsr_8, localTan_8))], ["PECO", (tsr_9) => ((localTan_9) => BioregistryParser(tsr_9, localTan_9))], ["TO", (tsr_10) => ((localTan_10) => BioregistryParser(tsr_10, localTan_10))], ["UO", (tsr_11) => ((localTan_11) => BioregistryParser(tsr_11, localTan_11))], ["EFO", (tsr_12) => ((localTan_12) => BioregistryParser(tsr_12, localTan_12))], ["NCIT", (tsr_13) => ((localTan_13) => BioregistryParser(tsr_13, localTan_13))], ["OMP", (tsr_14) => ((localTan_14) => BioregistryParser(tsr_14, localTan_14))]]);

export function createOAUri(tsr, localTan) {
    const matchValue = Dictionary_tryFind(tsr, uriParserCollection);
    if (matchValue == null) {
        return OntobeeParser(tsr, localTan);
    }
    else {
        return matchValue(tsr)(localTan);
    }
}

