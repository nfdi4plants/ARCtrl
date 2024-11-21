import { setOptionalProperty, setProperty, tryGetPropertyValue } from "../../fable_modules/DynamicObj.4.0.3/DynObj.fs.js";
import { value as value_1 } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { defaultOf } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { FSharpRef } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { Dataset_$reflection, Dataset } from "./Dataset.js";
import { class_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class Investigation extends Dataset {
    constructor(id, identifier, citation, comment, creator, dateCreated, dateModified, datePublished, hasPart, headline, mentions, url, description) {
        super(id, "Investigation");
        const this$ = new FSharpRef(defaultOf());
        this$.contents = this;
        this["init@8"] = 1;
        setProperty("identifier", identifier, this$.contents);
        setOptionalProperty("citation", citation, this$.contents);
        setOptionalProperty("comment", comment, this$.contents);
        setOptionalProperty("creator", creator, this$.contents);
        setOptionalProperty("dateCreated", dateCreated, this$.contents);
        setOptionalProperty("dateModified", dateModified, this$.contents);
        setOptionalProperty("datePublished", datePublished, this$.contents);
        setOptionalProperty("hasPart", hasPart, this$.contents);
        setOptionalProperty("headline", headline, this$.contents);
        setOptionalProperty("mentions", mentions, this$.contents);
        setOptionalProperty("url", url, this$.contents);
        setOptionalProperty("description", description, this$.contents);
    }
    GetIdentifier() {
        const this$ = this;
        const obj = this$;
        if (tryGetPropertyValue("identifier", obj) != null) {
            let matchValue;
            const matchValue_1 = obj.TryGetPropertyValue("identifier");
            if (matchValue_1 != null) {
                const o = value_1(matchValue_1);
                matchValue = ((typeof o === "string") ? o : undefined);
            }
            else {
                matchValue = undefined;
            }
            if (matchValue == null) {
                throw new Error(`Property '${"identifier"}' is set on this '${"Investigation"}' object but cannot be cast to '${"String"}'`);
            }
            else {
                return matchValue;
            }
        }
        else {
            throw new Error(`No property '${"identifier"}' set on this '${"Investigation"}' object although it is mandatory. Was it created correctly?`);
        }
    }
    static get getIdentifier() {
        return (inv) => inv.GetIdentifier();
    }
}

export function Investigation_$reflection() {
    return class_type("ARCtrl.ROCrate.Investigation", undefined, Investigation, Dataset_$reflection());
}

export function Investigation_$ctor_Z47833D48(id, identifier, citation, comment, creator, dateCreated, dateModified, datePublished, hasPart, headline, mentions, url, description) {
    return new Investigation(id, identifier, citation, comment, creator, dateCreated, dateModified, datePublished, hasPart, headline, mentions, url, description);
}

