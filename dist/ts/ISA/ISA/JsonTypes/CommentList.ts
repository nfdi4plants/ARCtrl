import { map, append, choose, tryPick } from "../../../fable_modules/fable-library-ts/Array.js";
import { value, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { Comment$ } from "./Comment.js";
import { FSharpMap, ofArray } from "../../../fable_modules/fable-library-ts/Map.js";
import { equals, comparePrimitives } from "../../../fable_modules/fable-library-ts/Util.js";

/**
 * If a comment with the given key exists in the [], return its value, else return None
 */
export function tryItem(key: string, comments: Comment$[]): Option<string> {
    return tryPick<Comment$, string>((c: Comment$): Option<string> => {
        const matchValue: Option<string> = c.Name;
        let matchResult: int32, n_1: string;
        if (matchValue != null) {
            if (value(matchValue) === key) {
                matchResult = 0;
                n_1 = value(matchValue);
            }
            else {
                matchResult = 1;
            }
        }
        else {
            matchResult = 1;
        }
        switch (matchResult) {
            case 0:
                return c.Value;
            default:
                return void 0;
        }
    }, comments);
}

/**
 * Returns true, if the key exists in the []
 */
export function containsKey(key: string, comments: Comment$[]): boolean {
    return comments.some((c: Comment$): boolean => {
        const matchValue: Option<string> = c.Name;
        let matchResult: int32, n_1: string;
        if (matchValue != null) {
            if (value(matchValue) === key) {
                matchResult = 0;
                n_1 = value(matchValue);
            }
            else {
                matchResult = 1;
            }
        }
        else {
            matchResult = 1;
        }
        switch (matchResult) {
            case 0:
                return true;
            default:
                return false;
        }
    });
}

/**
 * If a comment with the given key exists in the [], return its value
 */
export function item(key: string, comments: Comment$[]): string {
    return value(tryItem(key, comments));
}

/**
 * Create a map of comment keys to comment values
 */
export function toMap(comments: Comment$[]): FSharpMap<string, Option<string>> {
    return ofArray<string, Option<string>>(choose<Comment$, [string, Option<string>]>((c: Comment$): Option<[string, Option<string>]> => {
        const matchValue: Option<string> = c.Name;
        if (matchValue != null) {
            return [value(matchValue), c.Value] as [string, Option<string>];
        }
        else {
            return void 0;
        }
    }, comments), {
        Compare: comparePrimitives,
    });
}

/**
 * Adds the given comment to the comment []
 */
export function add(comment: Comment$, comments: Comment$[]): Comment$[] {
    return append<Comment$>(comments, [comment]);
}

/**
 * Add the given comment to the comment [] if it doesnt exist, else replace it
 */
export function set$(comment: Comment$, comments: Comment$[]): Comment$[] {
    if (containsKey(value(comment.Name), comments)) {
        return map<Comment$, Comment$>((c: Comment$): Comment$ => {
            if (equals(c.Name, comment.Name)) {
                return comment;
            }
            else {
                return c;
            }
        }, comments);
    }
    else {
        return append<Comment$>(comments, [comment]);
    }
}

/**
 * Returns a new comment [] where comments with the given key are filtered out
 */
export function dropByKey(key: string, comments: Comment$[]): Comment$[] {
    return comments.filter((c: Comment$): boolean => {
        const matchValue: Option<string> = c.Name;
        let matchResult: int32, n_1: string;
        if (matchValue != null) {
            if (value(matchValue) === key) {
                matchResult = 0;
                n_1 = value(matchValue);
            }
            else {
                matchResult = 1;
            }
        }
        else {
            matchResult = 1;
        }
        switch (matchResult) {
            case 0:
                return false;
            default:
                return true;
        }
    });
}

