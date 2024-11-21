import { setOptionalProperty, setProperty, tryGetPropertyValue } from "../../fable_modules/DynamicObj.4.0.3/DynObj.fs.js";
import { unwrap, value as value_1 } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { defaultOf } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { FSharpRef } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { LDObject_$reflection, LDObject } from "../LDObject.js";
import { class_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class PropertyValue extends LDObject {
    constructor(id, name, value, propertyID, unitCode, unitText, valueReference, additionalType) {
        super(id, "schema.org/PropertyValue", unwrap(additionalType));
        const this$ = new FSharpRef(defaultOf());
        this$.contents = this;
        this["init@8"] = 1;
        setProperty("name", name, this$.contents);
        setProperty("value", value, this$.contents);
        setOptionalProperty("propertyID", propertyID, this$.contents);
        setOptionalProperty("unitCode", unitCode, this$.contents);
        setOptionalProperty("unitText", unitText, this$.contents);
        setOptionalProperty("valueReference", valueReference, this$.contents);
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
                throw new Error(`Property '${"name"}' is set on this '${"PropertyValue"}' object but cannot be cast to '${"String"}'`);
            }
            else {
                return matchValue;
            }
        }
        else {
            throw new Error(`No property '${"name"}' set on this '${"PropertyValue"}' object although it is mandatory. Was it created correctly?`);
        }
    }
    static get getName() {
        return (lp) => lp.GetName();
    }
    GetValue() {
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
                throw new Error(`Property '${"name"}' is set on this '${"PropertyValue"}' object but cannot be cast to '${"String"}'`);
            }
            else {
                return matchValue;
            }
        }
        else {
            throw new Error(`No property '${"name"}' set on this '${"PropertyValue"}' object although it is mandatory. Was it created correctly?`);
        }
    }
    static get getValue() {
        return (lp) => lp.GetValue();
    }
}

export function PropertyValue_$reflection() {
    return class_type("ARCtrl.ROCrate.PropertyValue", undefined, PropertyValue, LDObject_$reflection());
}

export function PropertyValue_$ctor_7804003(id, name, value, propertyID, unitCode, unitText, valueReference, additionalType) {
    return new PropertyValue(id, name, value, propertyID, unitCode, unitText, valueReference, additionalType);
}

