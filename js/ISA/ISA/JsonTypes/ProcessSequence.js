import { List_groupBy, List_distinct } from "../../../fable_modules/fable-library.4.1.4/Seq2.js";
import { singleton, zip, map, empty, collect, concat, filter, choose } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { value as value_3, defaultArg, bind } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { arrayHash, equalArrays, comparePrimitives, safeHash, equals, stringHash } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { Process_updateProtocol, Process_getMaterials_716E708F, Process_getSamples_716E708F, Process_getSources_716E708F, Process_getData_716E708F, Process_getUnits_716E708F, Process_getFactors_716E708F, Process_tryGetOutputsWithFactorBy, Process_tryGetOutputsWithCharacteristicBy, Process_tryGetInputsWithCharacteristicBy, Process_getCharacteristics_716E708F, Process_getParameters_716E708F, Process_tryGetOutputsWithParameterBy, Process_tryGetInputsWithParameterBy } from "./Process.js";
import { FSharpSet__Contains, ofList } from "../../../fable_modules/fable-library.4.1.4/Set.js";
import { ProcessOutput__get_Name, ProcessOutput_getName_11830B70 } from "./ProcessOutput.js";
import { ProcessInput__get_Name, ProcessInput_getName_102B6859 } from "./ProcessInput.js";
import { FSharpMap__TryFind, ofList as ofList_1 } from "../../../fable_modules/fable-library.4.1.4/Map.js";

/**
 * Returns the names of the protocols the given processes impelement
 */
export function getProtocolNames(processSequence) {
    return List_distinct(choose((p) => bind((protocol) => protocol.Name, p.ExecutesProtocol), processSequence), {
        Equals: (x, y) => (x === y),
        GetHashCode: stringHash,
    });
}

/**
 * Returns the protocols the given processes impelement
 */
export function getProtocols(processSequence) {
    return List_distinct(choose((p) => p.ExecutesProtocol, processSequence), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns a list of the processes, containing only the ones executing a protocol for which the predicate returns true
 */
export function filterByProtocolBy(predicate, processSequence) {
    return filter((p) => {
        const matchValue = p.ExecutesProtocol;
        let matchResult, protocol_1;
        if (matchValue != null) {
            if (predicate(matchValue)) {
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
            case 0:
                return true;
            default:
                return false;
        }
    }, processSequence);
}

/**
 * Returns a list of the processes, containing only the ones executing a protocol with the given name
 */
export function filterByProtocolName(protocolName, processSequence) {
    return filterByProtocolBy((p) => equals(p.Name, protocolName), processSequence);
}

/**
 * If the processes contain a process implementing the given parameter, return the list of input files together with their according parameter values of this parameter
 */
export function getInputsWithParameterBy(predicate, processSequence) {
    return concat(choose((arg_1) => Process_tryGetInputsWithParameterBy(predicate, arg_1), processSequence));
}

/**
 * If the processes contain a process implementing the given parameter, return the list of output files together with their according parameter values of this parameter
 */
export function getOutputsWithParameterBy(predicate, processSequence) {
    return concat(choose((arg_1) => Process_tryGetOutputsWithParameterBy(predicate, arg_1), processSequence));
}

/**
 * Returns the parameters implemented by the processes contained in these processes
 */
export function getParameters(processSequence) {
    return List_distinct(collect(Process_getParameters_716E708F, processSequence), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns the characteristics describing the inputs and outputs of the processssequence
 */
export function getCharacteristics(processSequence) {
    return List_distinct(collect(Process_getCharacteristics_716E708F, processSequence), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * If the processes contain a process implementing the given parameter, return the list of input files together with their according parameter values of this parameter
 */
export function getInputsWithCharacteristicBy(predicate, processSequence) {
    return concat(choose((arg_1) => Process_tryGetInputsWithCharacteristicBy(predicate, arg_1), processSequence));
}

/**
 * If the processes contain a process implementing the given parameter, return the list of output files together with their according parameter values of this parameter
 */
export function getOutputsWithCharacteristicBy(predicate, processSequence) {
    return concat(choose((arg_1) => Process_tryGetOutputsWithCharacteristicBy(predicate, arg_1), processSequence));
}

/**
 * If the processes contain a process implementing the given factor, return the list of output files together with their according factor values of this factor
 */
export function getOutputsWithFactorBy(predicate, processSequence) {
    return concat(choose((arg_1) => Process_tryGetOutputsWithFactorBy(predicate, arg_1), processSequence));
}

/**
 * Returns the factors implemented by the processes contained in these processes
 */
export function getFactors(processSequence) {
    return List_distinct(collect(Process_getFactors_716E708F, processSequence), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns the initial inputs final outputs of the processSequence, to which no processPoints
 */
export function getRootInputs(processSequence) {
    const inputs = collect((p) => defaultArg(p.Inputs, empty()), processSequence);
    const outputs = ofList(collect((p_1) => map(ProcessOutput_getName_11830B70, defaultArg(p_1.Outputs, empty())), processSequence), {
        Compare: comparePrimitives,
    });
    return filter((i) => !FSharpSet__Contains(outputs, ProcessInput_getName_102B6859(i)), inputs);
}

/**
 * Returns the final outputs of the processSequence, which point to no further nodes
 */
export function getFinalOutputs(processSequence) {
    const inputs = ofList(collect((p) => map(ProcessInput_getName_102B6859, defaultArg(p.Inputs, empty())), processSequence), {
        Compare: comparePrimitives,
    });
    return filter((o) => !FSharpSet__Contains(inputs, ProcessOutput_getName_11830B70(o)), collect((p_1) => defaultArg(p_1.Outputs, empty()), processSequence));
}

/**
 * Returns the initial inputs final outputs of the processSequence, to which no processPoints
 */
export function getRootInputOf(processSequence, sample) {
    const mappings = ofList_1(map((tupledArg) => [tupledArg[0], map((tuple_1) => tuple_1[1], tupledArg[1])], List_groupBy((tuple) => tuple[0], collect((p) => List_distinct(zip(map(ProcessOutput__get_Name, value_3(p.Outputs)), map(ProcessInput__get_Name, value_3(p.Inputs))), {
        Equals: equalArrays,
        GetHashCode: arrayHash,
    }), processSequence), {
        Equals: (x_1, y_1) => (x_1 === y_1),
        GetHashCode: stringHash,
    })), {
        Compare: comparePrimitives,
    });
    const loop = (lastState_mut, state_mut) => {
        loop:
        while (true) {
            const lastState = lastState_mut, state = state_mut;
            if (equals(lastState, state)) {
                return state;
            }
            else {
                lastState_mut = state;
                state_mut = collect((s) => defaultArg(FSharpMap__TryFind(mappings, s), singleton(s)), state);
                continue loop;
            }
            break;
        }
    };
    return loop(empty(), singleton(sample));
}

/**
 * Returns the final outputs of the processSequence, which point to no further nodes
 */
export function getFinalOutputsOf(processSequence, sample) {
    const mappings = ofList_1(map((tupledArg) => [tupledArg[0], map((tuple_1) => tuple_1[1], tupledArg[1])], List_groupBy((tuple) => tuple[0], collect((p) => List_distinct(zip(map(ProcessInput__get_Name, value_3(p.Inputs)), map(ProcessOutput__get_Name, value_3(p.Outputs))), {
        Equals: equalArrays,
        GetHashCode: arrayHash,
    }), processSequence), {
        Equals: (x_1, y_1) => (x_1 === y_1),
        GetHashCode: stringHash,
    })), {
        Compare: comparePrimitives,
    });
    const loop = (lastState_mut, state_mut) => {
        loop:
        while (true) {
            const lastState = lastState_mut, state = state_mut;
            if (equals(lastState, state)) {
                return state;
            }
            else {
                lastState_mut = state;
                state_mut = collect((s) => defaultArg(FSharpMap__TryFind(mappings, s), singleton(s)), state);
                continue loop;
            }
            break;
        }
    };
    return loop(empty(), singleton(sample));
}

export function getUnits(processSequence) {
    return List_distinct(collect(Process_getUnits_716E708F, processSequence), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns the data the given processes contain
 */
export function getData(processSequence) {
    return List_distinct(collect(Process_getData_716E708F, processSequence), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

export function getSources(processSequence) {
    return List_distinct(collect(Process_getSources_716E708F, processSequence), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

export function getSamples(processSequence) {
    return List_distinct(collect(Process_getSamples_716E708F, processSequence), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

export function getMaterials(processSequence) {
    return List_distinct(collect(Process_getMaterials_716E708F, processSequence), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

export function updateProtocols(protocols, processSequence) {
    return map((arg_1) => Process_updateProtocol(protocols, arg_1), processSequence);
}

