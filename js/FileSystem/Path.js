import { trim, join, trimStart, trimEnd, split as split_1 } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { last, mapIndexed } from "../fable_modules/fable-library-js.4.22.0/Array.js";

export const seperators = ["/", "\\"];

export function split(path) {
    const array = split_1(path, seperators, undefined, 3);
    return array.filter((p) => {
        if (p !== "") {
            return p !== ".";
        }
        else {
            return false;
        }
    });
}

export function combine(path1, path2) {
    return (trimEnd(path1, ...seperators) + "/") + trimStart(path2, ...seperators);
}

export function combineMany(paths) {
    return join("/", mapIndexed((i, p) => {
        if (i === 0) {
            return trimEnd(p, ...seperators);
        }
        else if (i === (paths.length - 1)) {
            return trimStart(p, ...seperators);
        }
        else {
            return trim(p, ...seperators);
        }
    }, paths));
}

export function getFileName(path) {
    return last(split(path));
}

/**
 * Checks if `path` points to a file with the name `fileName`
 */
export function isFile(fileName, path) {
    return getFileName(path) === fileName;
}

export function getAssayFolderPath(assayIdentifier) {
    return combine("assays", assayIdentifier);
}

export function getStudyFolderPath(studyIdentifier) {
    return combine("studies", studyIdentifier);
}

