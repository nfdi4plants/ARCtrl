import { Record } from "../fable_modules/fable-library-js.4.22.0/Types.js";
import { class_type, array_type, record_type, option_type, string_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { DynamicObj_$reflection, DynamicObj } from "../fable_modules/DynamicObj.4.0.3/DynamicObj.fs.js";

export class StepInput extends Record {
    constructor(Id, Source, DefaultValue, ValueFrom) {
        super();
        this.Id = Id;
        this.Source = Source;
        this.DefaultValue = DefaultValue;
        this.ValueFrom = ValueFrom;
    }
}

export function StepInput_$reflection() {
    return record_type("ARCtrl.CWL.StepInput", [], StepInput, () => [["Id", string_type], ["Source", option_type(string_type)], ["DefaultValue", option_type(string_type)], ["ValueFrom", option_type(string_type)]]);
}

export class StepOutput extends Record {
    constructor(Id) {
        super();
        this.Id = Id;
    }
}

export function StepOutput_$reflection() {
    return record_type("ARCtrl.CWL.StepOutput", [], StepOutput, () => [["Id", array_type(string_type)]]);
}

export class WorkflowStep extends DynamicObj {
    constructor(id, in_, out_, run, requirements, hints) {
        super();
        this._id = id;
        this._in = in_;
        this._out = out_;
        this._run = run;
        this._requirements = requirements;
        this._hints = hints;
    }
    get Id() {
        const this$ = this;
        return this$._id;
    }
    set Id(id) {
        const this$ = this;
        this$._id = id;
    }
    get In() {
        const this$ = this;
        return this$._in;
    }
    set In(in_) {
        const this$ = this;
        this$._in = in_;
    }
    get Out() {
        const this$ = this;
        return this$._out;
    }
    set Out(out_) {
        const this$ = this;
        this$._out = out_;
    }
    get Run() {
        const this$ = this;
        return this$._run;
    }
    set Run(run) {
        const this$ = this;
        this$._run = run;
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
}

export function WorkflowStep_$reflection() {
    return class_type("ARCtrl.CWL.WorkflowStep", undefined, WorkflowStep, DynamicObj_$reflection());
}

export function WorkflowStep_$ctor_4159DCB0(id, in_, out_, run, requirements, hints) {
    return new WorkflowStep(id, in_, out_, run, requirements, hints);
}

