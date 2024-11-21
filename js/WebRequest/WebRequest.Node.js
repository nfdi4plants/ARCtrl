import { awaitPromise } from "../fable_modules/fable-library-js.4.22.0/Async.js";
import { fetch$ } from "../fable_modules/Fable.Fetch.2.6.0/Fetch.fs.js";
import { empty } from "../fable_modules/fable-library-js.4.22.0/List.js";

export function isNode() {
    return typeof process !== "undefined" &&
process.versions != null &&
process.versions.node != null;;
}

export function downloadFile(url) {
    let pr_1, pr;
    return awaitPromise((pr_1 = ((pr = fetch$(url, empty()), pr.then((res) => res.text()))), pr_1.then((txt) => txt)));
}

