import { trim, join, trimStart, trimEnd, split as split_1 } from "../fable_modules/fable-library.4.1.4/String.js";
import { last, mapIndexed } from "../fable_modules/fable-library.4.1.4/Array.js";

export const seperators = ["/", "\\"];

export function split(path) {
    return split_1(path, seperators, void 0, 3);
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

