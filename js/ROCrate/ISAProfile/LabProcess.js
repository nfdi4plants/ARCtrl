import { setOptionalProperty, setProperty, tryGetPropertyValue } from "../../fable_modules/DynamicObj.4.0.3/DynObj.fs.js";
import { unwrap, value as value_1 } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { defaultOf } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { FSharpRef } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { LDObject_$reflection, LDObject } from "../LDObject.js";
import { class_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class LabProcess extends LDObject {
    constructor(id, name, agent, object, result, additionalType, executesLabProtocol, parameterValue, endTime, disambiguatingDescription) {
        super(id, "bioschemas.org/LabProcess", unwrap(additionalType));
        const this$ = new FSharpRef(defaultOf());
        this$.contents = this;
        this["init@8"] = 1;
        setProperty("name", name, this$.contents);
        setProperty("agent", agent, this$.contents);
        setProperty("object", object, this$.contents);
        setProperty("result", result, this$.contents);
        setOptionalProperty("executesLabProtocol", executesLabProtocol, this$.contents);
        setOptionalProperty("parameterValue", parameterValue, this$.contents);
        setOptionalProperty("endTime", endTime, this$.contents);
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
                throw new Error(`Property '${"name"}' is set on this '${"LabProcess"}' object but cannot be cast to '${"String"}'`);
            }
            else {
                return matchValue;
            }
        }
        else {
            throw new Error(`No property '${"name"}' set on this '${"LabProcess"}' object although it is mandatory. Was it created correctly?`);
        }
    }
    static get getName() {
        return (lp) => lp.GetName();
    }
    GetAgent() {
        const this$ = this;
        const obj = this$;
        if (tryGetPropertyValue("agent", obj) != null) {
            let matchValue;
            const matchValue_1 = obj.TryGetPropertyValue("agent");
            if (matchValue_1 != null) {
                const o = value_1(matchValue_1);
                matchValue = ((typeof o === "string") ? o : undefined);
            }
            else {
                matchValue = undefined;
            }
            if (matchValue == null) {
                throw new Error(`Property '${"agent"}' is set on this '${"LabProcess"}' object but cannot be cast to '${"String"}'`);
            }
            else {
                return matchValue;
            }
        }
        else {
            throw new Error(`No property '${"agent"}' set on this '${"LabProcess"}' object although it is mandatory. Was it created correctly?`);
        }
    }
    static get getAgent() {
        return (lp) => lp.GetAgent();
    }
    GetObject() {
        const this$ = this;
        const obj = this$;
        if (tryGetPropertyValue("object", obj) != null) {
            let matchValue;
            const matchValue_1 = obj.TryGetPropertyValue("object");
            if (matchValue_1 != null) {
                const o = value_1(matchValue_1);
                matchValue = ((typeof o === "string") ? o : undefined);
            }
            else {
                matchValue = undefined;
            }
            if (matchValue == null) {
                throw new Error(`Property '${"object"}' is set on this '${"LabProcess"}' object but cannot be cast to '${"String"}'`);
            }
            else {
                return matchValue;
            }
        }
        else {
            throw new Error(`No property '${"object"}' set on this '${"LabProcess"}' object although it is mandatory. Was it created correctly?`);
        }
    }
    static get getObject() {
        return (lp) => lp.GetObject();
    }
    GetResult() {
        const this$ = this;
        const obj = this$;
        if (tryGetPropertyValue("result", obj) != null) {
            let matchValue;
            const matchValue_1 = obj.TryGetPropertyValue("result");
            if (matchValue_1 != null) {
                const o = value_1(matchValue_1);
                matchValue = ((typeof o === "string") ? o : undefined);
            }
            else {
                matchValue = undefined;
            }
            if (matchValue == null) {
                throw new Error(`Property '${"result"}' is set on this '${"LabProcess"}' object but cannot be cast to '${"String"}'`);
            }
            else {
                return matchValue;
            }
        }
        else {
            throw new Error(`No property '${"result"}' set on this '${"LabProcess"}' object although it is mandatory. Was it created correctly?`);
        }
    }
    static get getResult() {
        return (lp) => lp.GetResult();
    }
}

export function LabProcess_$reflection() {
    return class_type("ARCtrl.ROCrate.LabProcess", undefined, LabProcess, LDObject_$reflection());
}

export function LabProcess_$ctor_Z33971C5D(id, name, agent, object, result, additionalType, executesLabProtocol, parameterValue, endTime, disambiguatingDescription) {
    return new LabProcess(id, name, agent, object, result, additionalType, executesLabProtocol, parameterValue, endTime, disambiguatingDescription);
}

