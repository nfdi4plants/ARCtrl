import { setOptionalProperty, setProperty, tryGetPropertyValue } from "../../fable_modules/DynamicObj.4.0.3/DynObj.fs.js";
import { unwrap, value as value_1 } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { defaultOf } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { FSharpRef } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { LDObject_$reflection, LDObject } from "../LDObject.js";
import { class_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class Sample extends LDObject {
    constructor(id, name, additionalType, additionalProperty, derivesFrom) {
        super(id, "bioschemas.org/Sample", unwrap(additionalType));
        const this$ = new FSharpRef(defaultOf());
        this$.contents = this;
        this["init@8"] = 1;
        setProperty("name", name, this$.contents);
        setOptionalProperty("additionalProperty", additionalProperty, this$.contents);
        setOptionalProperty("derivesFrom", derivesFrom, this$.contents);
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
                throw new Error(`Property '${"name"}' is set on this '${"Sample"}' object but cannot be cast to '${"String"}'`);
            }
            else {
                return matchValue;
            }
        }
        else {
            throw new Error(`No property '${"name"}' set on this '${"Sample"}' object although it is mandatory. Was it created correctly?`);
        }
    }
    static get getName() {
        return (s) => s.GetName();
    }
}

export function Sample_$reflection() {
    return class_type("ARCtrl.ROCrate.Sample", undefined, Sample, LDObject_$reflection());
}

export function Sample_$ctor_7AC741F8(id, name, additionalType, additionalProperty, derivesFrom) {
    return new Sample(id, name, additionalType, additionalProperty, derivesFrom);
}

