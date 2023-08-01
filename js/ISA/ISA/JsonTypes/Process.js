import { Record, toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { zip, tryPick, map as map_1, tryFind, append, collect, choose, empty, length } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { map, value as value_4, bind, defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { printf, toText } from "../../../fable_modules/fable-library.4.1.4/String.js";
import { getRecordFields, makeRecord, record_type, list_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { Protocol_$reflection } from "./Protocol.js";
import { ProcessParameterValue_$reflection } from "./ProcessParameterValue.js";
import { ProcessInput_tryMaterial_Z38E7E853, ProcessInput_trySample_Z38E7E853, ProcessInput_tryData_Z38E7E853, ProcessInput_trySource_Z38E7E853, ProcessInput_tryGetCharacteristicValues_Z38E7E853, ProcessInput_$reflection } from "./ProcessInput.js";
import { ProcessOutput_tryGetFactorValues_Z4A02997C, ProcessOutput_tryGetCharacteristicValues_Z4A02997C, ProcessOutput_$reflection } from "./ProcessOutput.js";
import { Comment$_$reflection } from "./Comment.js";
import { create, match } from "../../../fable_modules/fable-library.4.1.4/RegExp.js";
import { parse } from "../../../fable_modules/fable-library.4.1.4/Int32.js";
import { List_distinct } from "../../../fable_modules/fable-library.4.1.4/Seq2.js";
import { safeHash, equals } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { ProtocolParameter_get_empty } from "./ProtocolParameter.js";
import { fromValueWithDefault } from "../OptionExtensions.js";
import { Update_UpdateOptions } from "../Update.js";
import { map2 } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";

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
    return record_type("ISA.Process", [], Process, () => [["ID", option_type(string_type)], ["Name", option_type(string_type)], ["ExecutesProtocol", option_type(Protocol_$reflection())], ["ParameterValues", option_type(list_type(ProcessParameterValue_$reflection()))], ["Performer", option_type(string_type)], ["Date", option_type(string_type)], ["PreviousProcess", option_type(Process_$reflection())], ["NextProcess", option_type(Process_$reflection())], ["Inputs", option_type(list_type(ProcessInput_$reflection()))], ["Outputs", option_type(list_type(ProcessOutput_$reflection()))], ["Comments", option_type(list_type(Comment$_$reflection()))]]);
}

export function Process_make(id, name, executesProtocol, parameterValues, performer, date, previousProcess, nextProcess, inputs, outputs, comments) {
    return new Process(id, name, executesProtocol, parameterValues, performer, date, previousProcess, nextProcess, inputs, outputs, comments);
}

export function Process_create_236DF576(Id, Name, ExecutesProtocol, ParameterValues, Performer, Date$, PreviousProcess, NextProcess, Inputs, Outputs, Comments) {
    return Process_make(Id, Name, ExecutesProtocol, ParameterValues, Performer, Date$, PreviousProcess, NextProcess, Inputs, Outputs, Comments);
}

export function Process_get_empty() {
    return Process_create_236DF576();
}

export function Process_composeName(processNameRoot, i) {
    return `${processNameRoot}_${i}`;
}

export function Process_decomposeName_Z721C83C5(name) {
    const r = match(create("(?<name>.+)_(?<num>\\d+)"), name);
    if (r != null) {
        return [(r.groups && r.groups.name) || "", parse((r.groups && r.groups.num) || "", 511, false, 32)];
    }
    else {
        return [name, void 0];
    }
}

/**
 * Returns the name of the protocol the given process executes
 */
export function Process_tryGetProtocolName_30EF9E7B(p) {
    return bind((p_1) => p_1.Name, p.ExecutesProtocol);
}

/**
 * Returns the name of the protocol the given process executes
 */
export function Process_getProtocolName_30EF9E7B(p) {
    return value_4(bind((p_1) => p_1.Name, p.ExecutesProtocol));
}

/**
 * Returns the parameter values describing the process
 */
export function Process_getParameterValues_30EF9E7B(p) {
    return defaultArg(p.ParameterValues, empty());
}

/**
 * Returns the parameters describing the process
 */
export function Process_getParameters_30EF9E7B(p) {
    return choose((pv) => pv.Category, Process_getParameterValues_30EF9E7B(p));
}

/**
 * Returns the characteristics describing the inputs of the process
 */
export function Process_getInputCharacteristicValues_30EF9E7B(p) {
    const matchValue = p.Inputs;
    if (matchValue == null) {
        return empty();
    }
    else {
        return List_distinct(collect((inp) => defaultArg(ProcessInput_tryGetCharacteristicValues_Z38E7E853(inp), empty()), matchValue), {
            Equals: equals,
            GetHashCode: safeHash,
        });
    }
}

/**
 * Returns the characteristics describing the outputs of the process
 */
export function Process_getOutputCharacteristicValues_30EF9E7B(p) {
    const matchValue = p.Outputs;
    if (matchValue == null) {
        return empty();
    }
    else {
        return List_distinct(collect((out) => defaultArg(ProcessOutput_tryGetCharacteristicValues_Z4A02997C(out), empty()), matchValue), {
            Equals: equals,
            GetHashCode: safeHash,
        });
    }
}

/**
 * Returns the characteristic values describing the inputs and outputs of the process
 */
export function Process_getCharacteristicValues_30EF9E7B(p) {
    return List_distinct(append(Process_getInputCharacteristicValues_30EF9E7B(p), Process_getOutputCharacteristicValues_30EF9E7B(p)), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns the characteristics describing the inputs and outputs of the process
 */
export function Process_getCharacteristics_30EF9E7B(p) {
    return List_distinct(choose((cv) => cv.Category, Process_getCharacteristicValues_30EF9E7B(p)), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns the factor values of the samples of the process
 */
export function Process_getFactorValues_30EF9E7B(p) {
    return List_distinct(collect((arg_1) => defaultArg(ProcessOutput_tryGetFactorValues_Z4A02997C(arg_1), empty()), defaultArg(p.Outputs, empty())), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns the factors of the samples of the process
 */
export function Process_getFactors_30EF9E7B(p) {
    return List_distinct(choose((fv) => fv.Category, Process_getFactorValues_30EF9E7B(p)), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns the units of the process
 */
export function Process_getUnits_30EF9E7B(p) {
    return append(choose((cv) => cv.Unit, Process_getCharacteristicValues_30EF9E7B(p)), append(choose((pv) => pv.Unit, Process_getParameterValues_30EF9E7B(p)), choose((fv) => fv.Unit, Process_getFactorValues_30EF9E7B(p))));
}

/**
 * If the process implements the given parameter, return the list of input files together with their according parameter values of this parameter
 */
export function Process_tryGetInputsWithParameterBy(predicate, p) {
    const matchValue = p.ParameterValues;
    if (matchValue == null) {
        return void 0;
    }
    else {
        const matchValue_1 = tryFind((pv) => predicate(defaultArg(pv.Category, ProtocolParameter_get_empty())), matchValue);
        if (matchValue_1 == null) {
            return void 0;
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
        return void 0;
    }
    else {
        const matchValue_1 = tryFind((pv) => predicate(defaultArg(pv.Category, ProtocolParameter_get_empty())), matchValue);
        if (matchValue_1 == null) {
            return void 0;
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
        return void 0;
    }
    else {
        return fromValueWithDefault(empty(), choose((i) => tryPick((mv) => {
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
                    return void 0;
            }
        }, defaultArg(ProcessInput_tryGetCharacteristicValues_Z38E7E853(i), empty())), matchValue));
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
            return fromValueWithDefault(empty(), choose((tupledArg) => tryPick((mv) => {
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
                        return void 0;
                }
            }, defaultArg(ProcessInput_tryGetCharacteristicValues_Z38E7E853(tupledArg[0]), empty())), zip(is, os)));
        default:
            return void 0;
    }
}

/**
 * If the process implements the given factor, return the list of output files together with their according factor values of this factor
 */
export function Process_tryGetOutputsWithFactorBy(predicate, p) {
    const matchValue = p.Outputs;
    if (matchValue == null) {
        return void 0;
    }
    else {
        return fromValueWithDefault(empty(), choose((o) => tryPick((mv) => {
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
                    return void 0;
            }
        }, defaultArg(ProcessOutput_tryGetFactorValues_Z4A02997C(o), empty())), matchValue));
    }
}

export function Process_getSources_30EF9E7B(p) {
    return List_distinct(choose(ProcessInput_trySource_Z38E7E853, defaultArg(p.Inputs, empty())), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

export function Process_getData_30EF9E7B(p) {
    return List_distinct(append(choose(ProcessInput_tryData_Z38E7E853, defaultArg(p.Inputs, empty())), choose(ProcessInput_tryData_Z38E7E853, defaultArg(p.Inputs, empty()))), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

export function Process_getSamples_30EF9E7B(p) {
    return List_distinct(append(choose(ProcessInput_trySample_Z38E7E853, defaultArg(p.Inputs, empty())), choose(ProcessInput_trySample_Z38E7E853, defaultArg(p.Inputs, empty()))), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

export function Process_getMaterials_30EF9E7B(p) {
    return List_distinct(append(choose(ProcessInput_tryMaterial_Z38E7E853, defaultArg(p.Inputs, empty())), choose(ProcessInput_tryMaterial_Z38E7E853, defaultArg(p.Inputs, empty()))), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

export function Process_updateProtocol(referenceProtocols, p) {
    let this$, recordType_1, recordType_2;
    const matchValue = p.ExecutesProtocol;
    let matchResult, protocol_1;
    if (matchValue != null) {
        if (matchValue.Name != null) {
            matchResult = 0;
            protocol_1 = matchValue;
        }
        else {
            matchResult = 1;
        }
    }
    else {
        matchResult = 1;
    }
    switch (matchResult) {
        case 0: {
            const matchValue_1 = tryFind((prot) => (value_4(prot.Name) === defaultArg(protocol_1.Name, "")), referenceProtocols);
            if (matchValue_1 != null) {
                return new Process(p.ID, p.Name, (this$ = (new Update_UpdateOptions(3, [])), (recordType_1 = protocol_1, (recordType_2 = matchValue_1, (this$.tag === 2) ? makeRecord(Protocol_$reflection(), map2(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) : ((this$.tag === 1) ? makeRecord(Protocol_$reflection(), map2(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2))) : ((this$.tag === 3) ? makeRecord(Protocol_$reflection(), map2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) : recordType_2))))), p.ParameterValues, p.Performer, p.Date, p.PreviousProcess, p.NextProcess, p.Inputs, p.Outputs, p.Comments);
            }
            else {
                return p;
            }
        }
        default:
            return p;
    }
}

