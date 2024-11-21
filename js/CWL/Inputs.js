import { FSharpRef, Record } from "../fable_modules/fable-library-js.4.22.0/Types.js";
import { class_type, record_type, bool_type, int32_type, option_type, string_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { value, unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { CWLType } from "./CWLTypes.js";
import { defaultOf } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { setOptionalProperty } from "../fable_modules/DynamicObj.4.0.3/DynObj.fs.js";
import { DynamicObj_$reflection, DynamicObj } from "../fable_modules/DynamicObj.4.0.3/DynamicObj.fs.js";

export class InputBinding extends Record {
    constructor(Prefix, Position, ItemSeparator, Separate) {
        super();
        this.Prefix = Prefix;
        this.Position = Position;
        this.ItemSeparator = ItemSeparator;
        this.Separate = Separate;
    }
}

export function InputBinding_$reflection() {
    return record_type("ARCtrl.CWL.InputBinding", [], InputBinding, () => [["Prefix", option_type(string_type)], ["Position", option_type(int32_type)], ["ItemSeparator", option_type(string_type)], ["Separate", option_type(bool_type)]]);
}

export class CWLInput extends DynamicObj {
    constructor(name, type_, inputBinding, optional) {
        super();
        const this$ = new FSharpRef(defaultOf());
        this.name = name;
        this$.contents = this;
        this["init@14"] = 1;
        setOptionalProperty("type", type_, this$.contents);
        setOptionalProperty("inputBinding", inputBinding, this$.contents);
        setOptionalProperty("optional", optional, this$.contents);
    }
    get Name() {
        const this$ = this;
        return this$.name;
    }
    get Type_() {
        let matchValue, o;
        const this$ = this;
        return unwrap((matchValue = this$.TryGetPropertyValue("type"), (matchValue != null) ? ((o = value(matchValue), (o instanceof CWLType) ? o : undefined)) : undefined));
    }
    get InputBinding() {
        let matchValue, o;
        const this$ = this;
        return unwrap((matchValue = this$.TryGetPropertyValue("inputBinding"), (matchValue != null) ? ((o = value(matchValue), (o instanceof InputBinding) ? o : undefined)) : undefined));
    }
    get Optional() {
        let matchValue, o;
        const this$ = this;
        return unwrap((matchValue = this$.TryGetPropertyValue("optional"), (matchValue != null) ? ((o = value(matchValue), (typeof o === "boolean") ? o : undefined)) : undefined));
    }
}

export function CWLInput_$reflection() {
    return class_type("ARCtrl.CWL.CWLInput", undefined, CWLInput, DynamicObj_$reflection());
}

export function CWLInput_$ctor_Z3A15BEDB(name, type_, inputBinding, optional) {
    return new CWLInput(name, type_, inputBinding, optional);
}

