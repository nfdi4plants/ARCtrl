import { map, append, choose, tryPick } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { value } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { ofArray } from "../../../fable_modules/fable-library.4.1.4/Map.js";
import { equals, comparePrimitives } from "../../../fable_modules/fable-library.4.1.4/Util.js";

/**
 * If a comment with the given key exists in the [], return its value, else return None
 */
export function tryItem(key, comments) {
    return tryPick((c) => {
        const matchValue = c.Name;
        let matchResult, n_1;
        if (matchValue != null) {
            if (matchValue === key) {
                matchResult = 0;
                n_1 = matchValue;
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
export function containsKey(key, comments) {
    return comments.some((c) => {
        const matchValue = c.Name;
        let matchResult, n_1;
        if (matchValue != null) {
            if (matchValue === key) {
                matchResult = 0;
                n_1 = matchValue;
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
export function item(key, comments) {
    return value(tryItem(key, comments));
}

/**
 * Create a map of comment keys to comment values
 */
export function toMap(comments) {
    return ofArray(choose((c) => {
        const matchValue = c.Name;
        if (matchValue != null) {
            return [matchValue, c.Value];
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
export function add(comment, comments) {
    return append(comments, [comment]);
}

/**
 * Add the given comment to the comment [] if it doesnt exist, else replace it
 */
export function set$(comment, comments) {
    if (containsKey(value(comment.Name), comments)) {
        return map((c) => {
            if (equals(c.Name, comment.Name)) {
                return comment;
            }
            else {
                return c;
            }
        }, comments);
    }
    else {
        return append(comments, [comment]);
    }
}

/**
 * Returns a new comment [] where comments with the given key are filtered out
 */
export function dropByKey(key, comments) {
    return comments.filter((c) => {
        const matchValue = c.Name;
        let matchResult, n_1;
        if (matchValue != null) {
            if (matchValue === key) {
                matchResult = 0;
                n_1 = matchValue;
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

