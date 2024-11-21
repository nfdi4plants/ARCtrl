import { DynamicObj_$reflection, DynamicObj } from "../fable_modules/DynamicObj.4.0.3/DynamicObj.fs.js";
import { class_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { value as value_1, unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";

export class LDContext extends DynamicObj {
    constructor() {
        super();
    }
}

export function LDContext_$reflection() {
    return class_type("ARCtrl.ROCrate.LDContext", undefined, LDContext, DynamicObj_$reflection());
}

export function LDContext_$ctor() {
    return new LDContext();
}

export class LDObject extends DynamicObj {
    constructor(id, schemaType, additionalType) {
        super();
        this.id = id;
        this["schemaType@22"] = schemaType;
        this["additionalType@23"] = additionalType;
    }
    get Id() {
        const this$ = this;
        return this$.id;
    }
    get SchemaType() {
        const this$ = this;
        return this$["schemaType@22"];
    }
    set SchemaType(value) {
        const this$ = this;
        this$["schemaType@22"] = value;
    }
    get AdditionalType() {
        const this$ = this;
        return unwrap(this$["additionalType@23"]);
    }
    set AdditionalType(value) {
        const this$ = this;
        this$["additionalType@23"] = value;
    }
    SetContext(context) {
        const this$ = this;
        this$.SetProperty("@context", context);
    }
    static setContext(context) {
        return (roc) => {
            roc.SetContext(context);
        };
    }
    TryGetContext() {
        const this$ = this;
        const matchValue = this$.TryGetPropertyValue("@context");
        if (matchValue != null) {
            const o = value_1(matchValue);
            return (o instanceof DynamicObj) ? o : undefined;
        }
        else {
            return undefined;
        }
    }
    static tryGetContext() {
        return (roc) => roc.TryGetContext();
    }
    RemoveContext() {
        const this$ = this;
        return this$.RemoveProperty("@context");
    }
    static removeContext() {
        return (roc) => roc.RemoveContext();
    }
    get SchemaType() {
        const this$ = this;
        return this$["schemaType@22"];
    }
    set SchemaType(value) {
        const this$ = this;
        this$["schemaType@22"] = value;
    }
    get Id() {
        const this$ = this;
        return this$.id;
    }
    get AdditionalType() {
        const this$ = this;
        return unwrap(this$["additionalType@23"]);
    }
    set AdditionalType(value) {
        const this$ = this;
        this$["additionalType@23"] = value;
    }
}

export function LDObject_$reflection() {
    return class_type("ARCtrl.ROCrate.LDObject", undefined, LDObject, DynamicObj_$reflection());
}

export function LDObject_$ctor_Z2FC25A28(id, schemaType, additionalType) {
    return new LDObject(id, schemaType, additionalType);
}

