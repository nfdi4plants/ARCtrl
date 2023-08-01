import { trim, join, trimStart, trimEnd, split as split_1 } from "../fable_modules/fable-library-ts/String.js";
import { last, mapIndexed } from "../fable_modules/fable-library-ts/Array.js";
import { int32 } from "../fable_modules/fable-library-ts/Int32.js";

export const seperators: string[] = ["/", "\\"];

export function split(path: string): string[] {
    return split_1(path, seperators, void 0, 3);
}

export function combine(path1: string, path2: string): string {
    return (trimEnd(path1, ...seperators) + "/") + trimStart(path2, ...seperators);
}

export function combineMany(paths: string[]): string {
    return join("/", mapIndexed<string, string>((i: int32, p: string): string => {
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

export function getFileName(path: string): string {
    return last<string>(split(path));
}

/**
 * Checks if `path` points to a file with the name `fileName`
 */
export function isFile(fileName: string, path: string): boolean {
    return getFileName(path) === fileName;
}

