import { FSharpRef, Record } from "../fable_modules/fable-library-js.4.22.0/Types.js";
import { class_type, record_type, option_type, string_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { value, unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { CWLType } from "./CWLTypes.js";
import { defaultOf } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { setOptionalProperty } from "../fable_modules/DynamicObj.4.0.3/DynObj.fs.js";
import { DynamicObj_$reflection, DynamicObj } from "../fable_modules/DynamicObj.4.0.3/DynamicObj.fs.js";

export class OutputBinding extends Record {
    constructor(Glob) {
        super();
        this.Glob = Glob;
    }
}

export function OutputBinding_$reflection() {
    return record_type("ARCtrl.CWL.OutputBinding", [], OutputBinding, () => [["Glob", option_type(string_type)]]);
}

export class CWLOutput extends DynamicObj {
    constructor(name, type_, outputBinding, outputSource) {
        super();
        const this$ = new FSharpRef(defaultOf());
        this.name = name;
        this$.contents = this;
        this["init@11"] = 1;
        setOptionalProperty("type", type_, this$.contents);
        setOptionalProperty("outputBinding", outputBinding, this$.contents);
        setOptionalProperty("outputSource", outputSource, this$.contents);
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
    get OutputBinding() {
        let matchValue, o;
        const this$ = this;
        return unwrap((matchValue = this$.TryGetPropertyValue("outputBinding"), (matchValue != null) ? ((o = value(matchValue), (o instanceof OutputBinding) ? o : undefined)) : undefined));
    }
    get OutputSource() {
        let matchValue, o;
        const this$ = this;
        return unwrap((matchValue = this$.TryGetPropertyValue("outputSource"), (matchValue != null) ? ((o = value(matchValue), (typeof o === "string") ? o : undefined)) : undefined));
    }
}

export function CWLOutput_$reflection() {
    return class_type("ARCtrl.CWL.CWLOutput", undefined, CWLOutput, DynamicObj_$reflection());
}

export function CWLOutput_$ctor_744035D(name, type_, outputBinding, outputSource) {
    return new CWLOutput(name, type_, outputBinding, outputSource);
}

