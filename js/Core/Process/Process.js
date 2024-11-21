import { Record, toString } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { zip, tryPick, map as map_1, tryFind, append, collect, choose, empty, length } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { map, value as value_4, bind, defaultArg } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { printf, toText } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { record_type, list_type, option_type, string_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { Protocol_$reflection } from "./Protocol.js";
import { ProcessParameterValue_$reflection } from "./ProcessParameterValue.js";
import { ProcessInput_tryMaterial_5B3D5BA9, ProcessInput_trySample_5B3D5BA9, ProcessInput_tryData_5B3D5BA9, ProcessInput_trySource_5B3D5BA9, ProcessInput_tryGetCharacteristicValues_5B3D5BA9, ProcessInput_$reflection } from "./ProcessInput.js";
import { ProcessOutput_tryMaterial_Z42C11600, ProcessOutput_trySample_Z42C11600, ProcessOutput_tryData_Z42C11600, ProcessOutput_tryGetFactorValues_Z42C11600, ProcessOutput_tryGetCharacteristicValues_Z42C11600, ProcessOutput_$reflection } from "./ProcessOutput.js";
import { Comment$_$reflection } from "../Comment.js";
import { ActivePatterns_$007CRegex$007C_$007C } from "../Helper/Regex.js";
import { parse } from "../../fable_modules/fable-library-js.4.22.0/Int32.js";
import { List_distinct } from "../../fable_modules/fable-library-js.4.22.0/Seq2.js";
import { safeHash, equals } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { ProtocolParameter } from "./ProtocolParameter.js";
import { Option_fromValueWithDefault } from "../Helper/Collections.js";

export class Process extends Record {
    constructor(ID, Name, ExecutesProtocol, ParameterValues, Performer, Date$, PreviousProcess, NextProcess, Inputs, Outputs, Comments) {
        super();
        this.ID = ID;
        this.Name = Name;
        this.ExecutesProtocol = ExecutesProtocol;
        this.ParameterValues = ParameterValues;
        this.Performer = Performer;
        this.Date = Date$;
        this.PreviousProcess = PreviousProcess;
        this.NextProcess = NextProcess;
        this.Inputs = Inputs;
        this.Outputs = Outputs;
        this.Comments = Comments;
    }
    Print() {
        const this$ = this;
        return toString(this$);
    }
    PrintCompact() {
        const this$ = this;
        const inputCount = length(defaultArg(this$.Inputs, empty())) | 0;
        const outputCount = length(defaultArg(this$.Outputs, empty())) | 0;
        const paramCount = length(defaultArg(this$.ParameterValues, empty())) | 0;
        const name = defaultArg(this$.Name, "Unnamed Process");
        return toText(printf("%s [%i Inputs -> %i Params -> %i Outputs]"))(name)(inputCount)(paramCount)(outputCount);
    }
}

export function Process_$reflection() {
    return record_type("ARCtrl.Process.Process", [], Process, () => [["ID", option_type(string_type)], ["Name", option_type(string_type)], ["ExecutesProtocol", option_type(Protocol_$reflection())], ["ParameterValues", option_type(list_type(ProcessParameterValue_$reflection()))], ["Performer", option_type(string_type)], ["Date", option_type(string_type)], ["PreviousProcess", option_type(Process_$reflection())], ["NextProcess", option_type(Process_$reflection())], ["Inputs", option_type(list_type(ProcessInput_$reflection()))], ["Outputs", option_type(list_type(ProcessOutput_$reflection()))], ["Comments", option_type(list_type(Comment$_$reflection()))]]);
}

export function Process_make(id, name, executesProtocol, parameterValues, performer, date, previousProcess, nextProcess, inputs, outputs, comments) {
    return new Process(id, name, executesProtocol, parameterValues, performer, date, previousProcess, nextProcess, inputs, outputs, comments);
}

export function Process_create_Z7C1F7FA9(Id, Name, ExecutesProtocol, ParameterValues, Performer, Date$, PreviousProcess, NextProcess, Inputs, Outputs, Comments) {
    return Process_make(Id, Name, ExecutesProtocol, ParameterValues, Performer, Date$, PreviousProcess, NextProcess, Inputs, Outputs, Comments);
}

export function Process_get_empty() {
    return Process_create_Z7C1F7FA9();
}

export function Process_composeName(processNameRoot, i) {
    return `${processNameRoot}_${i}`;
}

export function Process_decomposeName_Z721C83C5(name) {
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C("(?<name>.+)_(?<num>\\d+)", name);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return [(r.groups && r.groups.name) || "", parse((r.groups && r.groups.num) || "", 511, false, 32)];
    }
    else {
        return [name, undefined];
    }
}

/**
 * Returns the name of the protocol the given process executes
 */
export function Process_tryGetProtocolName_763471FF(p) {
    return bind((p_1) => p_1.Name, p.ExecutesProtocol);
}

/**
 * Returns the name of the protocol the given process executes
 */
export function Process_getProtocolName_763471FF(p) {
    return value_4(bind((p_1) => p_1.Name, p.ExecutesProtocol));
}

/**
 * Returns the parameter values describing the process
 */
export function Process_getParameterValues_763471FF(p) {
    return defaultArg(p.ParameterValues, empty());
}

/**
 * Returns the parameters describing the process
 */
export function Process_getParameters_763471FF(p) {
    return choose((pv) => pv.Category, Process_getParameterValues_763471FF(p));
}

/**
 * Returns the characteristics describing the inputs of the process
 */
export function Process_getInputCharacteristicValues_763471FF(p) {
    const matchValue = p.Inputs;
    if (matchValue == null) {
        return empty();
    }
    else {
        return List_distinct(collect((inp) => defaultArg(ProcessInput_tryGetCharacteristicValues_5B3D5BA9(inp), empty()), matchValue), {
            Equals: equals,
            GetHashCode: safeHash,
        });
    }
}

/**
 * Returns the characteristics describing the outputs of the process
 */
export function Process_getOutputCharacteristicValues_763471FF(p) {
    const matchValue = p.Outputs;
    if (matchValue == null) {
        return empty();
    }
    else {
        return List_distinct(collect((out) => defaultArg(ProcessOutput_tryGetCharacteristicValues_Z42C11600(out), empty()), matchValue), {
            Equals: equals,
            GetHashCode: safeHash,
        });
    }
}

/**
 * Returns the characteristic values describing the inputs and outputs of the process
 */
export function Process_getCharacteristicValues_763471FF(p) {
    return List_distinct(append(Process_getInputCharacteristicValues_763471FF(p), Process_getOutputCharacteristicValues_763471FF(p)), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns the characteristics describing the inputs and outputs of the process
 */
export function Process_getCharacteristics_763471FF(p) {
    return List_distinct(choose((cv) => cv.Category, Process_getCharacteristicValues_763471FF(p)), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns the factor values of the samples of the process
 */
export function Process_getFactorValues_763471FF(p) {
    return List_distinct(collect((arg) => defaultArg(ProcessOutput_tryGetFactorValues_Z42C11600(arg), empty()), defaultArg(p.Outputs, empty())), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns the factors of the samples of the process
 */
export function Process_getFactors_763471FF(p) {
    return List_distinct(choose((fv) => fv.Category, Process_getFactorValues_763471FF(p)), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns the units of the process
 */
export function Process_getUnits_763471FF(p) {
    return append(choose((cv) => cv.Unit, Process_getCharacteristicValues_763471FF(p)), append(choose((pv) => pv.Unit, Process_getParameterValues_763471FF(p)), choose((fv) => fv.Unit, Process_getFactorValues_763471FF(p))));
}

/**
 * If the process implements the given parameter, return the list of input files together with their according parameter values of this parameter
 */
export function Process_tryGetInputsWithParameterBy(predicate, p) {
    const matchValue = p.ParameterValues;
    if (matchValue == null) {
        return undefined;
    }
    else {
        const matchValue_1 = tryFind((pv) => predicate(defaultArg(pv.Category, ProtocolParameter.empty)), matchValue);
        if (matchValue_1 == null) {
            return undefined;
        }
        else {
            const paramValue = matchValue_1;
            return map((list_1) => map_1((i) => [i, paramValue], list_1), p.Inputs);
        }
    }
}

/**
 * If the process implements the given parameter, return the list of output files together with their according parameter values of this parameter
 */
export function Process_tryGetOutputsWithParameterBy(predicate, p) {
    const matchValue = p.ParameterValues;
    if (matchValue == null) {
        return undefined;
    }
    else {
        const matchValue_1 = tryFind((pv) => predicate(defaultArg(pv.Category, ProtocolParameter.empty)), matchValue);
        if (matchValue_1 == null) {
            return undefined;
        }
        else {
            const paramValue = matchValue_1;
            return map((list_1) => map_1((i) => [i, paramValue], list_1), p.Outputs);
        }
    }
}

/**
 * If the process implements the given characteristic, return the list of input files together with their according characteristic values of this characteristic
 */
export function Process_tryGetInputsWithCharacteristicBy(predicate, p) {
    const matchValue = p.Inputs;
    if (matchValue == null) {
        return undefined;
    }
    else {
        return Option_fromValueWithDefault(empty(), choose((i) => tryPick((mv) => {
            const matchValue_1 = mv.Category;
            let matchResult, m_1;
            if (matchValue_1 != null) {
                if (predicate(matchValue_1)) {
                    matchResult = 0;
                    m_1 = matchValue_1;
                }
                else {
                    matchResult = 1;
                }
            }
            else {
                matchResult = 1;
            }
            switch (matchResult) {
                case 0:
                    return [i, mv];
                default:
                    return undefined;
            }
        }, defaultArg(ProcessInput_tryGetCharacteristicValues_5B3D5BA9(i), empty())), matchValue));
    }
}

/**
 * If the process implements the given characteristic, return the list of output files together with their according characteristic values of this characteristic
 */
export function Process_tryGetOutputsWithCharacteristicBy(predicate, p) {
    const matchValue = p.Inputs;
    const matchValue_1 = p.Outputs;
    let matchResult, is, os;
    if (matchValue != null) {
        if (matchValue_1 != null) {
            matchResult = 0;
            is = matchValue;
            os = matchValue_1;
        }
        else {
            matchResult = 1;
        }
    }
    else {
        matchResult = 1;
    }
    switch (matchResult) {
        case 0:
            return Option_fromValueWithDefault(empty(), choose((tupledArg) => tryPick((mv) => {
                const matchValue_3 = mv.Category;
                let matchResult_1, m_1;
                if (matchValue_3 != null) {
                    if (predicate(matchValue_3)) {
                        matchResult_1 = 0;
                        m_1 = matchValue_3;
                    }
                    else {
                        matchResult_1 = 1;
                    }
                }
                else {
                    matchResult_1 = 1;
                }
                switch (matchResult_1) {
                    case 0:
                        return [tupledArg[1], mv];
                    default:
                        return undefined;
                }
            }, defaultArg(ProcessInput_tryGetCharacteristicValues_5B3D5BA9(tupledArg[0]), empty())), zip(is, os)));
        default:
            return undefined;
    }
}

/**
 * If the process implements the given factor, return the list of output files together with their according factor values of this factor
 */
export function Process_tryGetOutputsWithFactorBy(predicate, p) {
    const matchValue = p.Outputs;
    if (matchValue == null) {
        return undefined;
    }
    else {
        return Option_fromValueWithDefault(empty(), choose((o) => tryPick((mv) => {
            const matchValue_1 = mv.Category;
            let matchResult, m_1;
            if (matchValue_1 != null) {
                if (predicate(matchValue_1)) {
                    matchResult = 0;
                    m_1 = matchValue_1;
                }
                else {
                    matchResult = 1;
                }
            }
            else {
                matchResult = 1;
            }
            switch (matchResult) {
                case 0:
                    return [o, mv];
                default:
                    return undefined;
            }
        }, defaultArg(ProcessOutput_tryGetFactorValues_Z42C11600(o), empty())), matchValue));
    }
}

export function Process_getSources_763471FF(p) {
    return List_distinct(choose(ProcessInput_trySource_5B3D5BA9, defaultArg(p.Inputs, empty())), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

export function Process_getData_763471FF(p) {
    return List_distinct(append(choose(ProcessInput_tryData_5B3D5BA9, defaultArg(p.Inputs, empty())), choose(ProcessOutput_tryData_Z42C11600, defaultArg(p.Outputs, empty()))), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

export function Process_getSamples_763471FF(p) {
    return List_distinct(append(choose(ProcessInput_trySample_5B3D5BA9, defaultArg(p.Inputs, empty())), choose(ProcessOutput_trySample_Z42C11600, defaultArg(p.Outputs, empty()))), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

export function Process_getMaterials_763471FF(p) {
    return List_distinct(append(choose(ProcessInput_tryMaterial_5B3D5BA9, defaultArg(p.Inputs, empty())), choose(ProcessOutput_tryMaterial_Z42C11600, defaultArg(p.Outputs, empty()))), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

