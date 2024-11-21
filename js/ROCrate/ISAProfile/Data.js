import { setOptionalProperty, setProperty, tryGetPropertyValue } from "../../fable_modules/DynamicObj.4.0.3/DynObj.fs.js";
import { unwrap, value as value_1 } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { defaultOf } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { FSharpRef } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { LDObject_$reflection, LDObject } from "../LDObject.js";
import { class_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class Data extends LDObject {
    constructor(id, name, additionalType, comment, encodingFormat, disambiguatingDescription) {
        super(id, "schema.org/MediaObject", unwrap(additionalType));
        const this$ = new FSharpRef(defaultOf());
        this$.contents = this;
        this["init@8"] = 1;
        setProperty("name", name, this$.contents);
        setOptionalProperty("comment", comment, this$.contents);
        setOptionalProperty("encodingFormat", encodingFormat, this$.contents);
        setOptionalProperty("disambiguatingDescription", disambiguatingDescription, this$.contents);
    }
    GetName() {
        const this$ = this;
        const obj = this$;
        if (tryGetPropertyValue("name", obj) != null) {
            let matchValue;
            const matchValue_1 = obj.TryGetPropertyValue("name");
            if (matchValue_1 != null) {
                const o = value_1(matchValue_1);
                matchValue = ((typeof o === "string") ? o : undefined);
            }
            else {
                matchValue = undefined;
            }
            if (matchValue == null) {
                throw new Error(`Property '${"name"}' is set on this '${"Data"}' object but cannot be cast to '${"String"}'`);
            }
            else {
                return matchValue;
            }
        }
        else {
            throw new Error(`No property '${"name"}' set on this '${"Data"}' object although it is mandatory. Was it created correctly?`);
        }
    }
    static get getName() {
        return (d) => d.GetName();
    }
}

export function Data_$reflection() {
    return class_type("ARCtrl.ROCrate.Data", undefined, Data, LDObject_$reflection());
}

export function Data_$ctor_Z68810720(id, name, additionalType, comment, encodingFormat, disambiguatingDescription) {
    return new Data(id, name, additionalType, comment, encodingFormat, disambiguatingDescription);
}

