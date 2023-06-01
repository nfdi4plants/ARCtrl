import { combine } from "./FileSystem/Path.js";
import { Assay } from "./ISA/Assay.js";
import { map } from "./fable_modules/fable-library-ts/Array.js";

export const assaySubFolderNames: string[] = ["dataset", "protocols"];

export function combineAssayFolderPath(assay: Assay): string {
    return combine("assays", Assay.getIdentifier<string>(assay));
}

export function combineAssayProtocolsFolderPath(assay: Assay): string {
    return combine(combineAssayFolderPath(assay), "protocols");
}

export function combineAssayDatasetFolderPath(assay: Assay): string {
    return combine(combineAssayFolderPath(assay), "dataset");
}

export function combineAssaySubfolderPaths(assay: Assay): string[] {
    const assayFolder: string = combineAssayFolderPath(assay);
    return map<string, string>((subfolderName: string): string => combine(assayFolder, subfolderName), assaySubFolderNames);
}

