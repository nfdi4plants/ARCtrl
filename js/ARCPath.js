import { combine } from "./FileSystem/Path.js";
import { Assay } from "./ISA/Assay.js";
import { map } from "./fable_modules/fable-library.4.1.4/Array.js";

export const assaySubFolderNames = ["dataset", "protocols"];

export function combineAssayFolderPath(assay) {
    return combine("assays", Assay.getIdentifier(assay));
}

export function combineAssayProtocolsFolderPath(assay) {
    return combine(combineAssayFolderPath(assay), "protocols");
}

export function combineAssayDatasetFolderPath(assay) {
    return combine(combineAssayFolderPath(assay), "dataset");
}

export function combineAssaySubfolderPaths(assay) {
    const assayFolder = combineAssayFolderPath(assay);
    return map((subfolderName) => combine(assayFolder, subfolderName), assaySubFolderNames);
}

