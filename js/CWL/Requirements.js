import { Union, FSharpRef, Record } from "../fable_modules/fable-library-js.4.22.0/Types.js";
import { union_type, float64_type, array_type, record_type, class_type, option_type, string_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { defaultOf } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { setOptionalProperty } from "../fable_modules/DynamicObj.4.0.3/DynObj.fs.js";
import { DynamicObj_$reflection, DynamicObj } from "../fable_modules/DynamicObj.4.0.3/DynamicObj.fs.js";
import { CWLType_$reflection, SoftwarePackage_$reflection, SchemaDefRequirementType_$reflection } from "./CWLTypes.js";

export class DockerRequirement extends Record {
    constructor(DockerPull, DockerFile, DockerImageId) {
        super();
        this.DockerPull = DockerPull;
        this.DockerFile = DockerFile;
        this.DockerImageId = DockerImageId;
    }
}

export function DockerRequirement_$reflection() {
    return record_type("ARCtrl.CWL.DockerRequirement", [], DockerRequirement, () => [["DockerPull", option_type(string_type)], ["DockerFile", option_type(class_type("Microsoft.FSharp.Collections.FSharpMap`2", [string_type, string_type]))], ["DockerImageId", option_type(string_type)]]);
}

export class EnvironmentDef extends Record {
    constructor(EnvName, EnvValue) {
        super();
        this.EnvName = EnvName;
        this.EnvValue = EnvValue;
    }
}

export function EnvironmentDef_$reflection() {
    return record_type("ARCtrl.CWL.EnvironmentDef", [], EnvironmentDef, () => [["EnvName", string_type], ["EnvValue", string_type]]);
}

export class ResourceRequirementInstance extends DynamicObj {
    constructor(coresMin, coresMax, ramMin, ramMax, tmpdirMin, tmpdirMax, outdirMin, outdirMax) {
        super();
        const this$ = new FSharpRef(defaultOf());
        this$.contents = this;
        this["init@24"] = 1;
        setOptionalProperty("coresMin", coresMin, this$.contents);
        setOptionalProperty("coresMax", coresMax, this$.contents);
        setOptionalProperty("ramMin", ramMin, this$.contents);
        setOptionalProperty("ramMax", ramMax, this$.contents);
        setOptionalProperty("tmpdirMin", tmpdirMin, this$.contents);
        setOptionalProperty("tmpdirMax", tmpdirMax, this$.contents);
        setOptionalProperty("outdirMin", outdirMin, this$.contents);
        setOptionalProperty("outdirMax", outdirMax, this$.contents);
    }
}

export function ResourceRequirementInstance_$reflection() {
    return class_type("ARCtrl.CWL.ResourceRequirementInstance", undefined, ResourceRequirementInstance, DynamicObj_$reflection());
}

export function ResourceRequirementInstance_$ctor_D76FC00(coresMin, coresMax, ramMin, ramMax, tmpdirMin, tmpdirMax, outdirMin, outdirMax) {
    return new ResourceRequirementInstance(coresMin, coresMax, ramMin, ramMax, tmpdirMin, tmpdirMax, outdirMin, outdirMax);
}

export class Requirement extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["InlineJavascriptRequirement", "SchemaDefRequirement", "DockerRequirement", "SoftwareRequirement", "InitialWorkDirRequirement", "EnvVarRequirement", "ShellCommandRequirement", "ResourceRequirement", "WorkReuseRequirement", "NetworkAccessRequirement", "InplaceUpdateRequirement", "ToolTimeLimitRequirement", "SubworkflowFeatureRequirement", "ScatterFeatureRequirement", "MultipleInputFeatureRequirement", "StepInputExpressionRequirement"];
    }
}

export function Requirement_$reflection() {
    return union_type("ARCtrl.CWL.Requirement", [], Requirement, () => [[], [["Item", array_type(SchemaDefRequirementType_$reflection())]], [["Item", DockerRequirement_$reflection()]], [["Item", array_type(SoftwarePackage_$reflection())]], [["Item", array_type(CWLType_$reflection())]], [["Item", array_type(EnvironmentDef_$reflection())]], [], [["Item", ResourceRequirementInstance_$reflection()]], [], [], [], [["Item", float64_type]], [], [], [], []]);
}

