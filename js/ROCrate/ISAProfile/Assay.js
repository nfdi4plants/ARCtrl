import { setOptionalProperty, setProperty, tryGetPropertyValue } from "../../fable_modules/DynamicObj.4.0.3/DynObj.fs.js";
import { value as value_1 } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { defaultOf } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { FSharpRef } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { Dataset_$reflection, Dataset } from "./Dataset.js";
import { class_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class Assay extends Dataset {
    constructor(id, identifier, about, comment, creator, hasPart, measurementMethod, measurementTechnique, url, variableMeasured) {
        super(id, "Assay");
        const this$ = new FSharpRef(defaultOf());
        this$.contents = this;
        this["init@8"] = 1;
        setProperty("identifier", identifier, this$.contents);
        setOptionalProperty("measurementMethod", measurementMethod, this$.contents);
        setOptionalProperty("measurementTechnique", measurementTechnique, this$.contents);
        setOptionalProperty("variableMeasured", variableMeasured, this$.contents);
        setOptionalProperty("about", about, this$.contents);
        setOptionalProperty("comment", comment, this$.contents);
        setOptionalProperty("creator", creator, this$.contents);
        setOptionalProperty("hasPart", hasPart, this$.contents);
        setOptionalProperty("url", url, this$.contents);
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
                throw new Error(`Property '${"identifier"}' is set on this '${"Assay"}' object but cannot be cast to '${"String"}'`);
            }
            else {
                return matchValue;
            }
        }
        else {
            throw new Error(`No property '${"identifier"}' set on this '${"Assay"}' object although it is mandatory. Was it created correctly?`);
        }
    }
    static get getIdentifier() {
        return (ass) => ass.GetIdentifier();
    }
}

export function Assay_$reflection() {
    return class_type("ARCtrl.ROCrate.Assay", undefined, Assay, Dataset_$reflection());
}

export function Assay_$ctor_Z318F9460(id, identifier, about, comment, creator, hasPart, measurementMethod, measurementTechnique, url, variableMeasured) {
    return new Assay(id, identifier, about, comment, creator, hasPart, measurementMethod, measurementTechnique, url, variableMeasured);
}

