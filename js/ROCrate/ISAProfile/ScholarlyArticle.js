import { setOptionalProperty, setProperty, tryGetPropertyValue } from "../../fable_modules/DynamicObj.4.0.3/DynObj.fs.js";
import { unwrap, value as value_1 } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { defaultOf } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { FSharpRef } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { LDObject_$reflection, LDObject } from "../LDObject.js";
import { class_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class ScholarlyArticle extends LDObject {
    constructor(id, headline, identifier, additionalType, author, url, creativeWorkStatus, disambiguatingDescription) {
        super(id, "schema.org/ScholarlyArticle", unwrap(additionalType));
        const this$ = new FSharpRef(defaultOf());
        this$.contents = this;
        this["init@8"] = 1;
        setProperty("headline", headline, this$.contents);
        setProperty("identifier", identifier, this$.contents);
        setOptionalProperty("author", author, this$.contents);
        setOptionalProperty("url", url, this$.contents);
        setOptionalProperty("creativeWorkStatus", creativeWorkStatus, this$.contents);
        setOptionalProperty("disambiguatingDescription", disambiguatingDescription, this$.contents);
    }
    GetHeadline() {
        const this$ = this;
        const obj = this$;
        if (tryGetPropertyValue("headline", obj) != null) {
            let matchValue;
            const matchValue_1 = obj.TryGetPropertyValue("headline");
            if (matchValue_1 != null) {
                const o = value_1(matchValue_1);
                matchValue = ((typeof o === "string") ? o : undefined);
            }
            else {
                matchValue = undefined;
            }
            if (matchValue == null) {
                throw new Error(`Property '${"headline"}' is set on this '${"ScholarlyArticle"}' object but cannot be cast to '${"String"}'`);
            }
            else {
                return matchValue;
            }
        }
        else {
            throw new Error(`No property '${"headline"}' set on this '${"ScholarlyArticle"}' object although it is mandatory. Was it created correctly?`);
        }
    }
    static get getHeadline() {
        return (s) => s.GetHeadline();
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
                throw new Error(`Property '${"identifier"}' is set on this '${"ScholarlyArticle"}' object but cannot be cast to '${"String"}'`);
            }
            else {
                return matchValue;
            }
        }
        else {
            throw new Error(`No property '${"identifier"}' set on this '${"ScholarlyArticle"}' object although it is mandatory. Was it created correctly?`);
        }
    }
    static get getIdentifier() {
        return (s) => s.GetIdentifier();
    }
}

export function ScholarlyArticle_$reflection() {
    return class_type("ARCtrl.ROCrate.ScholarlyArticle", undefined, ScholarlyArticle, LDObject_$reflection());
}

export function ScholarlyArticle_$ctor_11693003(id, headline, identifier, additionalType, author, url, creativeWorkStatus, disambiguatingDescription) {
    return new ScholarlyArticle(id, headline, identifier, additionalType, author, url, creativeWorkStatus, disambiguatingDescription);
}

