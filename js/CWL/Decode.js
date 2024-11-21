import { disposeSafe, getEnumerator } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { setOptionalProperty, setProperty } from "../fable_modules/DynamicObj.4.0.3/DynObj.fs.js";
import { DynamicObj } from "../fable_modules/DynamicObj.4.0.3/DynamicObj.fs.js";
import { ofArray, tail, head, isEmpty, empty, map, iterate } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { read, int, float, resizearray, map as map_1, bool, string, object } from "../fable_modules/YAMLicious.0.0.3/Decode.fs.js";
import { replace, toFail, printf, toText } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { CWLOutput, OutputBinding } from "./Outputs.js";
import { SchemaDefRequirementType_$ctor_541DA560, SoftwarePackage, DirectoryInstance_$ctor, FileInstance_$ctor, CWLType, DirentInstance } from "./CWLTypes.js";
import { length, item, singleton, empty as empty_1, append, collect, delay, toArray } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { getItemFromDict } from "../fable_modules/fable-library-js.4.22.0/MapUtil.js";
import { Requirement, ResourceRequirementInstance_$ctor_D76FC00, EnvironmentDef, DockerRequirement } from "./Requirements.js";
import { some } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { FSharpMap__get_Values, FSharpMap__get_Keys } from "../fable_modules/fable-library-js.4.22.0/Map.js";
import { CWLInput, InputBinding } from "./Inputs.js";
import { WorkflowStep, StepOutput, StepInput } from "./WorkflowSteps.js";
import { CWLToolDescription } from "./ToolDescription.js";
import { CWLWorkflowDescription } from "./WorkflowDescription.js";

export function ResizeArray_map(f, a) {
    const b = [];
    let enumerator = getEnumerator(a);
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const i = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
            void (b.push(f(i)));
        }
    }
    finally {
        disposeSafe(enumerator);
    }
    return b;
}

/**
 * Decode key value pairs into a dynamic object, while preserving their tree structure
 */
export function Decode_overflowDecoder(dynObj, dict) {
    let enumerator = getEnumerator(dict);
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const e = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
            const matchValue = e[1];
            let matchResult, v, s;
            if (matchValue.tag === 3) {
                if (!isEmpty(matchValue.fields[0])) {
                    switch (head(matchValue.fields[0]).tag) {
                        case 1: {
                            if (isEmpty(tail(matchValue.fields[0]))) {
                                matchResult = 0;
                                v = head(matchValue.fields[0]).fields[0];
                            }
                            else {
                                matchResult = 2;
                            }
                            break;
                        }
                        case 2: {
                            if (isEmpty(tail(matchValue.fields[0]))) {
                                matchResult = 1;
                                s = head(matchValue.fields[0]).fields[0];
                            }
                            else {
                                matchResult = 2;
                            }
                            break;
                        }
                        default:
                            matchResult = 2;
                    }
                }
                else {
                    matchResult = 2;
                }
            }
            else {
                matchResult = 2;
            }
            switch (matchResult) {
                case 0: {
                    setProperty(e[0], v.Value, dynObj);
                    break;
                }
                case 1: {
                    const newDynObj = new DynamicObj();
                    iterate((x) => {
                        setProperty(e[0], x, dynObj);
                    }, map((arg) => Decode_overflowDecoder(newDynObj, object((get$) => get$.Overflow.FieldList(empty()), arg)), s));
                    break;
                }
                case 2: {
                    setProperty(e[0], e[1], dynObj);
                    break;
                }
            }
        }
    }
    finally {
        disposeSafe(enumerator);
    }
    return dynObj;
}

/**
 * Decode a YAMLElement which is either a string or expression into a string
 */
export function Decode_decodeStringOrExpression(yEle) {
    let matchResult, v, c, v_1;
    switch (yEle.tag) {
        case 1: {
            matchResult = 0;
            v = yEle.fields[0];
            break;
        }
        case 3: {
            if (!isEmpty(yEle.fields[0])) {
                switch (head(yEle.fields[0]).tag) {
                    case 1: {
                        if (isEmpty(tail(yEle.fields[0]))) {
                            matchResult = 0;
                            v = head(yEle.fields[0]).fields[0];
                        }
                        else {
                            matchResult = 2;
                        }
                        break;
                    }
                    case 0: {
                        if (head(yEle.fields[0]).fields[1].tag === 3) {
                            if (!isEmpty(head(yEle.fields[0]).fields[1].fields[0])) {
                                if (head(head(yEle.fields[0]).fields[1].fields[0]).tag === 1) {
                                    if (isEmpty(tail(head(yEle.fields[0]).fields[1].fields[0]))) {
                                        if (isEmpty(tail(yEle.fields[0]))) {
                                            matchResult = 1;
                                            c = head(yEle.fields[0]).fields[0];
                                            v_1 = head(head(yEle.fields[0]).fields[1].fields[0]).fields[0];
                                        }
                                        else {
                                            matchResult = 2;
                                        }
                                    }
                                    else {
                                        matchResult = 2;
                                    }
                                }
                                else {
                                    matchResult = 2;
                                }
                            }
                            else {
                                matchResult = 2;
                            }
                        }
                        else {
                            matchResult = 2;
                        }
                        break;
                    }
                    default:
                        matchResult = 2;
                }
            }
            else {
                matchResult = 2;
            }
            break;
        }
        default:
            matchResult = 2;
    }
    switch (matchResult) {
        case 0:
            return v.Value;
        case 1:
            return toText(printf("%s: %s"))(c.Value)(v_1.Value);
        default:
            return toFail(printf("%A"))(yEle);
    }
}

export const Decode_outputBindingGlobDecoder = (value_1) => object((get$) => {
    let objectArg;
    return new OutputBinding((objectArg = get$.Optional, objectArg.Field("glob", string)));
}, value_1);

export const Decode_outputBindingDecoder = (value) => object((get$) => {
    const objectArg = get$.Optional;
    return objectArg.Field("outputBinding", Decode_outputBindingGlobDecoder);
}, value);

export const Decode_direntDecoder = (value_1) => object((get$) => {
    let objectArg, objectArg_1, objectArg_2;
    return new CWLType(2, [new DirentInstance((objectArg = get$.Required, objectArg.Field("entry", Decode_decodeStringOrExpression)), (objectArg_1 = get$.Optional, objectArg_1.Field("entryname", Decode_decodeStringOrExpression)), (objectArg_2 = get$.Optional, objectArg_2.Field("writable", bool)))]);
}, value_1);

export const Decode_cwlArrayTypeDecoder = (value_1) => object((get$) => {
    let objectArg_1;
    let items;
    const objectArg = get$.Required;
    items = objectArg.Field("items", string);
    switch (items) {
        case "File":
            return new CWLType(11, [new CWLType(0, [FileInstance_$ctor()])]);
        case "Directory":
            return new CWLType(11, [new CWLType(1, [DirectoryInstance_$ctor()])]);
        case "Dirent":
            return new CWLType(11, [(objectArg_1 = get$.Required, objectArg_1.Field("listing", Decode_direntDecoder))]);
        case "string":
            return new CWLType(11, [new CWLType(3, [])]);
        case "int":
            return new CWLType(11, [new CWLType(4, [])]);
        case "long":
            return new CWLType(11, [new CWLType(5, [])]);
        case "float":
            return new CWLType(11, [new CWLType(6, [])]);
        case "double":
            return new CWLType(11, [new CWLType(7, [])]);
        case "boolean":
            return new CWLType(11, [new CWLType(8, [])]);
        default:
            throw new Error("Invalid CWL type");
    }
}, value_1);

/**
 * Match the input string to the possible CWL types and checks if it is optional
 */
export function Decode_cwlTypeStringMatcher(t, get$) {
    let objectArg, objectArg_1;
    const patternInput = t.endsWith("?") ? [true, replace(t, "?", "")] : [false, t];
    const newT = patternInput[1];
    return [(newT === "File") ? (new CWLType(0, [FileInstance_$ctor()])) : ((newT === "Directory") ? (new CWLType(1, [DirectoryInstance_$ctor()])) : ((newT === "Dirent") ? ((objectArg = get$.Required, objectArg.Field("listing", Decode_direntDecoder))) : ((newT === "string") ? (new CWLType(3, [])) : ((newT === "int") ? (new CWLType(4, [])) : ((newT === "long") ? (new CWLType(5, [])) : ((newT === "float") ? (new CWLType(6, [])) : ((newT === "double") ? (new CWLType(7, [])) : ((newT === "boolean") ? (new CWLType(8, [])) : ((newT === "File[]") ? (new CWLType(11, [new CWLType(0, [FileInstance_$ctor()])])) : ((newT === "Directory[]") ? (new CWLType(11, [new CWLType(1, [DirectoryInstance_$ctor()])])) : ((newT === "Dirent[]") ? (new CWLType(11, [(objectArg_1 = get$.Required, objectArg_1.Field("listing", Decode_direntDecoder))])) : ((newT === "string[]") ? (new CWLType(11, [new CWLType(3, [])])) : ((newT === "int[]") ? (new CWLType(11, [new CWLType(4, [])])) : ((newT === "long[]") ? (new CWLType(11, [new CWLType(5, [])])) : ((newT === "float[]") ? (new CWLType(11, [new CWLType(6, [])])) : ((newT === "double[]") ? (new CWLType(11, [new CWLType(7, [])])) : ((newT === "boolean[]") ? (new CWLType(11, [new CWLType(8, [])])) : ((newT === "stdout") ? (new CWLType(9, [])) : ((newT === "null") ? (new CWLType(10, [])) : (() => {
        throw new Error("Invalid CWL type");
    })()))))))))))))))))))), patternInput[0]];
}

export const Decode_cwlTypeDecoder = (value_1) => object((get$) => {
    let objectArg_1;
    let cwlType;
    const objectArg = get$.Required;
    cwlType = objectArg.Field("type", (value) => {
        let matchResult, v, o;
        switch (value.tag) {
            case 1: {
                matchResult = 0;
                v = value.fields[0];
                break;
            }
            case 3: {
                if (!isEmpty(value.fields[0])) {
                    if (head(value.fields[0]).tag === 1) {
                        if (isEmpty(tail(value.fields[0]))) {
                            matchResult = 0;
                            v = head(value.fields[0]).fields[0];
                        }
                        else {
                            matchResult = 1;
                            o = value.fields[0];
                        }
                    }
                    else {
                        matchResult = 1;
                        o = value.fields[0];
                    }
                }
                else {
                    matchResult = 1;
                    o = value.fields[0];
                }
                break;
            }
            default:
                matchResult = 2;
        }
        switch (matchResult) {
            case 0:
                return v.Value;
            case 1:
                return undefined;
            default:
                throw new Error("Unexpected YAMLElement");
        }
    });
    if (cwlType == null) {
        return [(objectArg_1 = get$.Required, objectArg_1.Field("type", Decode_cwlArrayTypeDecoder)), false];
    }
    else {
        return Decode_cwlTypeStringMatcher(cwlType, get$);
    }
}, value_1);

export const Decode_outputArrayDecoder = (value_2) => object((get$) => {
    const dict = get$.Overflow.FieldList(empty());
    const collection = toArray(delay(() => collect((key) => {
        const value = getItemFromDict(dict, key);
        const outputBinding = Decode_outputBindingDecoder(value);
        let outputSource;
        const objectArg = get$.Optional;
        outputSource = objectArg.Field("outputSource", string);
        const output = new CWLOutput(key, (value.tag === 3) ? (!isEmpty(value.fields[0]) ? ((head(value.fields[0]).tag === 1) ? (isEmpty(tail(value.fields[0])) ? Decode_cwlTypeStringMatcher(head(value.fields[0]).fields[0].Value, get$)[0] : Decode_cwlTypeDecoder(value)[0]) : Decode_cwlTypeDecoder(value)[0]) : Decode_cwlTypeDecoder(value)[0]) : Decode_cwlTypeDecoder(value)[0]);
        return append((outputBinding != null) ? ((setOptionalProperty("outputBinding", outputBinding, output), empty_1())) : empty_1(), delay(() => append((outputSource != null) ? ((setOptionalProperty("outputSource", outputSource, output), empty_1())) : empty_1(), delay(() => singleton(output)))));
    }, dict.keys())));
    return Array.from(collection);
}, value_2);

export const Decode_outputsDecoder = (value) => object((get$) => {
    const objectArg = get$.Required;
    return objectArg.Field("outputs", Decode_outputArrayDecoder);
}, value);

/**
 * Decode a YAMLElement into a DockerRequirement
 */
export function Decode_dockerRequirementDecoder(get$) {
    let objectArg, objectArg_1, objectArg_2;
    return new DockerRequirement((objectArg = get$.Optional, objectArg.Field("dockerPull", string)), (objectArg_1 = get$.Optional, objectArg_1.Field("dockerFile", (value_1) => map_1((x) => x, string, value_1))), (objectArg_2 = get$.Optional, objectArg_2.Field("dockerImageId", string)));
}

/**
 * Decode a YAMLElement into an EnvVarRequirement array
 */
export function Decode_envVarRequirementDecoder(get$) {
    const objectArg = get$.Required;
    return objectArg.Field("envDef", (value_3) => resizearray((value_2) => object((get2) => {
        let objectArg_1, objectArg_2;
        return new EnvironmentDef((objectArg_1 = get2.Required, objectArg_1.Field("envName", string)), (objectArg_2 = get2.Required, objectArg_2.Field("envValue", string)));
    }, value_2), value_3));
}

/**
 * Decode a YAMLElement into a SoftwareRequirement array
 */
export function Decode_softwareRequirementDecoder(get$) {
    const objectArg = get$.Required;
    return objectArg.Field("packages", (value_6) => resizearray((value_5) => object((get2) => {
        let objectArg_1, objectArg_2, objectArg_3;
        return new SoftwarePackage((objectArg_1 = get2.Required, objectArg_1.Field("package", string)), (objectArg_2 = get2.Optional, objectArg_2.Field("version", (value_1) => resizearray(string, value_1))), (objectArg_3 = get2.Optional, objectArg_3.Field("specs", (value_3) => resizearray(string, value_3))));
    }, value_5), value_6));
}

/**
 * Decode a YAMLElement into a InitialWorkDirRequirement array
 */
export function Decode_initialWorkDirRequirementDecoder(get$) {
    const objectArg = get$.Required;
    return objectArg.Field("listing", (value) => resizearray(Decode_direntDecoder, value));
}

/**
 * Decode a YAMLElement into a ResourceRequirementInstance
 */
export function Decode_resourceRequirementDecoder(get$) {
    let objectArg, objectArg_1, objectArg_2, objectArg_3, objectArg_4, objectArg_5, objectArg_6, objectArg_7;
    return ResourceRequirementInstance_$ctor_D76FC00(some((objectArg = get$.Optional, objectArg.Field("coresMin", (x) => x))), some((objectArg_1 = get$.Optional, objectArg_1.Field("coresMax", (x_1) => x_1))), some((objectArg_2 = get$.Optional, objectArg_2.Field("ramMin", (x_2) => x_2))), some((objectArg_3 = get$.Optional, objectArg_3.Field("ramMax", (x_3) => x_3))), some((objectArg_4 = get$.Optional, objectArg_4.Field("tmpdirMin", (x_4) => x_4))), some((objectArg_5 = get$.Optional, objectArg_5.Field("tmpdirMax", (x_5) => x_5))), some((objectArg_6 = get$.Optional, objectArg_6.Field("outdirMin", (x_6) => x_6))), some((objectArg_7 = get$.Optional, objectArg_7.Field("outdirMax", (x_7) => x_7))));
}

/**
 * Decode a YAMLElement into a SchemaDefRequirementType array
 */
export function Decode_schemaDefRequirementDecoder(get$) {
    let objectArg;
    return ResizeArray_map((m) => SchemaDefRequirementType_$ctor_541DA560(item(0, FSharpMap__get_Keys(m)), item(0, FSharpMap__get_Values(m))), (objectArg = get$.Required, objectArg.Field("types", (value_2) => resizearray((value) => map_1((x) => x, string, value), value_2))));
}

/**
 * Decode a YAMLElement into a ToolTimeLimitRequirement
 */
export function Decode_toolTimeLimitRequirementDecoder(get$) {
    const objectArg = get$.Required;
    return objectArg.Field("timelimit", float);
}

export const Decode_requirementArrayDecoder = (value_2) => resizearray((value_1) => object((get$) => {
    let cls;
    const objectArg = get$.Required;
    cls = objectArg.Field("class", string);
    switch (cls) {
        case "InlineJavascriptRequirement":
            return new Requirement(0, []);
        case "SchemaDefRequirement":
            return new Requirement(1, [Decode_schemaDefRequirementDecoder(get$)]);
        case "DockerRequirement":
            return new Requirement(2, [Decode_dockerRequirementDecoder(get$)]);
        case "SoftwareRequirement":
            return new Requirement(3, [Decode_softwareRequirementDecoder(get$)]);
        case "InitialWorkDirRequirement":
            return new Requirement(4, [Decode_initialWorkDirRequirementDecoder(get$)]);
        case "EnvVarRequirement":
            return new Requirement(5, [Decode_envVarRequirementDecoder(get$)]);
        case "ShellCommandRequirement":
            return new Requirement(6, []);
        case "ResourceRequirement":
            return new Requirement(7, [Decode_resourceRequirementDecoder(get$)]);
        case "WorkReuse":
            return new Requirement(8, []);
        case "NetworkAccess":
            return new Requirement(9, []);
        case "InplaceUpdateRequirement":
            return new Requirement(10, []);
        case "ToolTimeLimit":
            return new Requirement(11, [Decode_toolTimeLimitRequirementDecoder(get$)]);
        case "SubworkflowFeatureRequirement":
            return new Requirement(12, []);
        case "ScatterFeatureRequirement":
            return new Requirement(13, []);
        case "MultipleInputFeatureRequirement":
            return new Requirement(14, []);
        case "StepInputExpressionRequirement":
            return new Requirement(15, []);
        default:
            throw new Error("Invalid requirement");
    }
}, value_1), value_2);

export const Decode_requirementsDecoder = (value) => object((get$) => {
    const objectArg = get$.Optional;
    return objectArg.Field("requirements", Decode_requirementArrayDecoder);
}, value);

export const Decode_hintsDecoder = (value) => object((get$) => {
    const objectArg = get$.Optional;
    return objectArg.Field("hints", Decode_requirementArrayDecoder);
}, value);

export const Decode_inputBindingDecoder = (value_5) => object((get$) => {
    const objectArg = get$.Optional;
    return objectArg.Field("inputBinding", (value_4) => object((get$0027) => {
        let objectArg_1, objectArg_2, objectArg_3, objectArg_4;
        return new InputBinding((objectArg_1 = get$0027.Optional, objectArg_1.Field("prefix", string)), (objectArg_2 = get$0027.Optional, objectArg_2.Field("position", int)), (objectArg_3 = get$0027.Optional, objectArg_3.Field("itemSeparator", string)), (objectArg_4 = get$0027.Optional, objectArg_4.Field("separate", bool)));
    }, value_4));
}, value_5);

export const Decode_inputArrayDecoder = (value_1) => object((get$) => {
    const dict = get$.Overflow.FieldList(empty());
    const collection = toArray(delay(() => collect((key) => {
        const value = getItemFromDict(dict, key);
        const inputBinding = Decode_inputBindingDecoder(value);
        const patternInput = (value.tag === 3) ? (!isEmpty(value.fields[0]) ? ((head(value.fields[0]).tag === 1) ? (isEmpty(tail(value.fields[0])) ? Decode_cwlTypeStringMatcher(head(value.fields[0]).fields[0].Value, get$) : Decode_cwlTypeDecoder(value)) : Decode_cwlTypeDecoder(value)) : Decode_cwlTypeDecoder(value)) : Decode_cwlTypeDecoder(value);
        const input = new CWLInput(key, patternInput[0]);
        return append((inputBinding != null) ? ((setOptionalProperty("inputBinding", inputBinding, input), empty_1())) : empty_1(), delay(() => append(patternInput[1] ? ((setOptionalProperty("optional", true, input), empty_1())) : empty_1(), delay(() => singleton(input)))));
    }, dict.keys())));
    return Array.from(collection);
}, value_1);

export const Decode_inputsDecoder = (value) => object((get$) => {
    const objectArg = get$.Optional;
    return objectArg.Field("inputs", Decode_inputArrayDecoder);
}, value);

export const Decode_baseCommandDecoder = (value_2) => object((get$) => {
    const objectArg = get$.Optional;
    return objectArg.Field("baseCommand", (value) => resizearray(string, value));
}, value_2);

export const Decode_versionDecoder = (value_1) => object((get$) => {
    const objectArg = get$.Required;
    return objectArg.Field("cwlVersion", string);
}, value_1);

export const Decode_classDecoder = (value_1) => object((get$) => {
    const objectArg = get$.Required;
    return objectArg.Field("class", string);
}, value_1);

export function Decode_stringOptionFieldDecoder(field) {
    return (value_1) => object((get$) => {
        const objectArg = get$.Optional;
        return objectArg.Field(field, string);
    }, value_1);
}

export function Decode_stringFieldDecoder(field) {
    return (value_1) => object((get$) => {
        const objectArg = get$.Required;
        return objectArg.Field(field, string);
    }, value_1);
}

export const Decode_inputStepDecoder = (value_1) => object((get$) => {
    const dict = get$.Overflow.FieldList(empty());
    const collection = toArray(delay(() => collect((key) => {
        let s1, s2;
        const value = getItemFromDict(dict, key);
        return singleton(new StepInput(key, (s1 = ((value.tag === 3) ? (!isEmpty(value.fields[0]) ? ((head(value.fields[0]).tag === 1) ? (isEmpty(tail(value.fields[0])) ? head(value.fields[0]).fields[0].Value : undefined) : undefined) : undefined) : undefined), (s2 = Decode_stringOptionFieldDecoder("source")(value), (s1 != null) ? s1 : ((s2 != null) ? s2 : undefined))), Decode_stringOptionFieldDecoder("default")(value), Decode_stringOptionFieldDecoder("valueFrom")(value)));
    }, dict.keys())));
    return Array.from(collection);
}, value_1);

export const Decode_outputStepsDecoder = (value_2) => object((get$) => {
    const objectArg = get$.Required;
    return objectArg.Field("out", (value) => resizearray(string, value));
}, value_2);

export const Decode_stepArrayDecoder = (value_1) => object((get$) => {
    const dict = get$.Overflow.FieldList(empty());
    const collection = toArray(delay(() => collect((key) => {
        const value = getItemFromDict(dict, key);
        const run = Decode_stringFieldDecoder("run")(value);
        const inputs = object((get$_1) => {
            const objectArg = get$_1.Required;
            return objectArg.Field("in", Decode_inputStepDecoder);
        }, value);
        const outputs = new StepOutput(Decode_outputStepsDecoder(value));
        const requirements = Decode_requirementsDecoder(value);
        const hints = Decode_hintsDecoder(value);
        const wfStep = new WorkflowStep(key, inputs, outputs, run);
        return append((requirements != null) ? ((wfStep.Requirements = requirements, empty_1())) : empty_1(), delay(() => append((hints != null) ? ((wfStep.Hints = hints, empty_1())) : empty_1(), delay(() => singleton(wfStep)))));
    }, dict.keys())));
    return Array.from(collection);
}, value_1);

export const Decode_stepsDecoder = (value) => object((get$) => {
    const objectArg = get$.Required;
    return objectArg.Field("steps", Decode_stepArrayDecoder);
}, value);

/**
 * Decode a CWL file string written in the YAML format into a CWLToolDescription
 */
export function Decode_decodeCommandLineTool(cwl) {
    const yamlCWL = read(cwl);
    const cwlVersion = Decode_versionDecoder(yamlCWL);
    const outputs = Decode_outputsDecoder(yamlCWL);
    const inputs = Decode_inputsDecoder(yamlCWL);
    const requirements = Decode_requirementsDecoder(yamlCWL);
    const hints = Decode_hintsDecoder(yamlCWL);
    const baseCommand = Decode_baseCommandDecoder(yamlCWL);
    const description = new CWLToolDescription(outputs, cwlVersion);
    let metadata;
    const md = new DynamicObj();
    object((get$) => Decode_overflowDecoder(md, get$.Overflow.FieldList(ofArray(["inputs", "outputs", "class", "id", "label", "doc", "requirements", "hints", "cwlVersion", "baseCommand", "arguments", "stdin", "stderr", "stdout", "successCodes", "temporaryFailCodes", "permanentFailCodes"]))), yamlCWL);
    metadata = md;
    object((get$_1) => Decode_overflowDecoder(description, get$_1.MultipleOptional.FieldList(ofArray(["id", "label", "doc", "arguments", "stdin", "stderr", "stdout", "successCodes", "temporaryFailCodes", "permanentFailCodes"]))), yamlCWL);
    if (inputs != null) {
        description.Inputs = inputs;
    }
    if (requirements != null) {
        description.Requirements = requirements;
    }
    if (hints != null) {
        description.Hints = hints;
    }
    if (baseCommand != null) {
        description.BaseCommand = baseCommand;
    }
    if (length(metadata.GetProperties(false)) > 0) {
        description.Metadata = metadata;
    }
    return description;
}

/**
 * Decode a CWL file string written in the YAML format into a CWLWorkflowDescription
 */
export function Decode_decodeWorkflow(cwl) {
    const yamlCWL = read(cwl);
    const cwlVersion = Decode_versionDecoder(yamlCWL);
    const outputs = Decode_outputsDecoder(yamlCWL);
    let inputs;
    const matchValue = Decode_inputsDecoder(yamlCWL);
    if (matchValue == null) {
        throw new Error("Inputs are required for a workflow");
    }
    else {
        inputs = matchValue;
    }
    const requirements = Decode_requirementsDecoder(yamlCWL);
    const hints = Decode_hintsDecoder(yamlCWL);
    const description = new CWLWorkflowDescription(Decode_stepsDecoder(yamlCWL), inputs, outputs, cwlVersion);
    let metadata;
    const md = new DynamicObj();
    object((get$) => Decode_overflowDecoder(md, get$.Overflow.FieldList(ofArray(["inputs", "outputs", "class", "steps", "id", "label", "doc", "requirements", "hints", "cwlVersion"]))), yamlCWL);
    metadata = md;
    object((get$_1) => Decode_overflowDecoder(description, get$_1.MultipleOptional.FieldList(ofArray(["id", "label", "doc"]))), yamlCWL);
    if (requirements != null) {
        description.Requirements = requirements;
    }
    if (hints != null) {
        description.Hints = hints;
    }
    if (length(metadata.GetProperties(false)) > 0) {
        description.Metadata = metadata;
    }
    return description;
}

