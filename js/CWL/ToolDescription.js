import { defaultArg, unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { DynamicObj_$reflection, DynamicObj } from "../fable_modules/DynamicObj.4.0.3/DynamicObj.fs.js";
import { class_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class CWLToolDescription extends DynamicObj {
    constructor(outputs, cwlVersion, baseCommand, requirements, hints, inputs, metadata) {
        super();
        this._cwlVersion = defaultArg(cwlVersion, "v1.2");
        this._outputs = outputs;
        this._baseCommand = baseCommand;
        this._requirements = requirements;
        this._hints = hints;
        this._inputs = inputs;
        this._metadata = metadata;
    }
    get CWLVersion() {
        const this$ = this;
        return this$._cwlVersion;
    }
    set CWLVersion(version) {
        const this$ = this;
        this$._cwlVersion = version;
    }
    get Outputs() {
        const this$ = this;
        return this$._outputs;
    }
    set Outputs(outputs) {
        const this$ = this;
        this$._outputs = outputs;
    }
    get BaseCommand() {
        const this$ = this;
        return unwrap(this$._baseCommand);
    }
    set BaseCommand(baseCommand) {
        const this$ = this;
        this$._baseCommand = baseCommand;
    }
    get Requirements() {
        const this$ = this;
        return unwrap(this$._requirements);
    }
    set Requirements(requirements) {
        const this$ = this;
        this$._requirements = requirements;
    }
    get Hints() {
        const this$ = this;
        return unwrap(this$._hints);
    }
    set Hints(hints) {
        const this$ = this;
        this$._hints = hints;
    }
    get Inputs() {
        const this$ = this;
        return unwrap(this$._inputs);
    }
    set Inputs(inputs) {
        const this$ = this;
        this$._inputs = inputs;
    }
    get Metadata() {
        const this$ = this;
        return unwrap(this$._metadata);
    }
    set Metadata(metadata) {
        const this$ = this;
        this$._metadata = metadata;
    }
}

export function CWLToolDescription_$reflection() {
    return class_type("ARCtrl.CWL.CWLToolDescription", undefined, CWLToolDescription, DynamicObj_$reflection());
}

export function CWLToolDescription_$ctor_Z3224DB85(outputs, cwlVersion, baseCommand, requirements, hints, inputs, metadata) {
    return new CWLToolDescription(outputs, cwlVersion, baseCommand, requirements, hints, inputs, metadata);
}

