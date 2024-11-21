import { defaultArg, unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { DynamicObj_$reflection, DynamicObj } from "../fable_modules/DynamicObj.4.0.3/DynamicObj.fs.js";
import { class_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class CWLWorkflowDescription extends DynamicObj {
    constructor(steps, inputs, outputs, cwlVersion, requirements, hints, metadata) {
        super();
        this._cwlVersion = defaultArg(cwlVersion, "v1.2");
        this._steps = steps;
        this._inputs = inputs;
        this._outputs = outputs;
        this._requirements = requirements;
        this._hints = hints;
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
    get Steps() {
        const this$ = this;
        return this$._steps;
    }
    set Steps(steps) {
        const this$ = this;
        this$._steps = steps;
    }
    get Inputs() {
        const this$ = this;
        return this$._inputs;
    }
    set Inputs(inputs) {
        const this$ = this;
        this$._inputs = inputs;
    }
    get Outputs() {
        const this$ = this;
        return this$._outputs;
    }
    set Outputs(outputs) {
        const this$ = this;
        this$._outputs = outputs;
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
    get Metadata() {
        const this$ = this;
        return unwrap(this$._metadata);
    }
    set Metadata(metadata) {
        const this$ = this;
        this$._metadata = metadata;
    }
}

export function CWLWorkflowDescription_$reflection() {
    return class_type("ARCtrl.CWL.CWLWorkflowDescription", undefined, CWLWorkflowDescription, DynamicObj_$reflection());
}

export function CWLWorkflowDescription_$ctor_704BBD06(steps, inputs, outputs, cwlVersion, requirements, hints, metadata) {
    return new CWLWorkflowDescription(steps, inputs, outputs, cwlVersion, requirements, hints, metadata);
}

