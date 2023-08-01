import { List_groupBy, List_distinct } from "../../../fable_modules/fable-library-ts/Seq2.js";
import { singleton, zip, map, empty, collect, concat, filter, FSharpList, choose } from "../../../fable_modules/fable-library-ts/List.js";
import { defaultArg, value as value_3, Option, bind } from "../../../fable_modules/fable-library-ts/Option.js";
import { Protocol } from "./Protocol.js";
import { Process_updateProtocol, Process_getMaterials_30EF9E7B, Process_getSamples_30EF9E7B, Process_getSources_30EF9E7B, Process_getData_30EF9E7B, Process_getUnits_30EF9E7B, Process_getFactors_30EF9E7B, Process_tryGetOutputsWithFactorBy, Process_tryGetOutputsWithCharacteristicBy, Process_tryGetInputsWithCharacteristicBy, Process_getCharacteristics_30EF9E7B, Process_getParameters_30EF9E7B, Process_tryGetOutputsWithParameterBy, Process_tryGetInputsWithParameterBy, Process } from "./Process.js";
import { arrayHash, equalArrays, comparePrimitives, safeHash, equals, stringHash } from "../../../fable_modules/fable-library-ts/Util.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { ProcessInput__get_Name, ProcessInput_getName_Z38E7E853, ProcessInput_$union } from "./ProcessInput.js";
import { ProcessParameterValue } from "./ProcessParameterValue.js";
import { ProtocolParameter } from "./ProtocolParameter.js";
import { ProcessOutput__get_Name, ProcessOutput_getName_Z4A02997C, ProcessOutput_$union } from "./ProcessOutput.js";
import { MaterialAttribute } from "./MaterialAttribute.js";
import { MaterialAttributeValue } from "./MaterialAttributeValue.js";
import { FactorValue } from "./FactorValue.js";
import { Factor } from "./Factor.js";
import { FSharpSet__Contains, FSharpSet, ofList } from "../../../fable_modules/fable-library-ts/Set.js";
import { FSharpMap__TryFind, FSharpMap, ofList as ofList_1 } from "../../../fable_modules/fable-library-ts/Map.js";
import { OntologyAnnotation } from "./OntologyAnnotation.js";
import { Data } from "./Data.js";
import { Source } from "./Source.js";
import { Sample } from "./Sample.js";
import { Material } from "./Material.js";

/**
 * Returns the names of the protocols the given processes impelement
 */
export function getProtocolNames(processSequence: FSharpList<Process>): FSharpList<string> {
    return List_distinct<string>(choose<Process, string>((p: Process): Option<string> => bind<Protocol, string>((protocol: Protocol): Option<string> => protocol.Name, p.ExecutesProtocol), processSequence), {
        Equals: (x: string, y: string): boolean => (x === y),
        GetHashCode: stringHash,
    });
}

/**
 * Returns the protocols the given processes impelement
 */
export function getProtocols(processSequence: FSharpList<Process>): FSharpList<Protocol> {
    return List_distinct<Protocol>(choose<Process, Protocol>((p: Process): Option<Protocol> => p.ExecutesProtocol, processSequence), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns a list of the processes, containing only the ones executing a protocol for which the predicate returns true
 */
export function filterByProtocolBy(predicate: ((arg0: Protocol) => boolean), processSequence: FSharpList<Process>): FSharpList<Process> {
    return filter<Process>((p: Process): boolean => {
        const matchValue: Option<Protocol> = p.ExecutesProtocol;
        let matchResult: int32, protocol_1: Protocol;
        if (matchValue != null) {
            if (predicate(value_3(matchValue))) {
                matchResult = 0;
                protocol_1 = value_3(matchValue);
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
export function filterByProtocolName(protocolName: string, processSequence: FSharpList<Process>): FSharpList<Process> {
    return filterByProtocolBy((p: Protocol): boolean => equals(p.Name, protocolName), processSequence);
}

/**
 * If the processes contain a process implementing the given parameter, return the list of input files together with their according parameter values of this parameter
 */
export function getInputsWithParameterBy(predicate: ((arg0: ProtocolParameter) => boolean), processSequence: FSharpList<Process>): FSharpList<[ProcessInput_$union, ProcessParameterValue]> {
    return concat<[ProcessInput_$union, ProcessParameterValue]>(choose<Process, FSharpList<[ProcessInput_$union, ProcessParameterValue]>>((arg_1: Process): Option<FSharpList<[ProcessInput_$union, ProcessParameterValue]>> => Process_tryGetInputsWithParameterBy(predicate, arg_1), processSequence));
}

/**
 * If the processes contain a process implementing the given parameter, return the list of output files together with their according parameter values of this parameter
 */
export function getOutputsWithParameterBy(predicate: ((arg0: ProtocolParameter) => boolean), processSequence: FSharpList<Process>): FSharpList<[ProcessOutput_$union, ProcessParameterValue]> {
    return concat<[ProcessOutput_$union, ProcessParameterValue]>(choose<Process, FSharpList<[ProcessOutput_$union, ProcessParameterValue]>>((arg_1: Process): Option<FSharpList<[ProcessOutput_$union, ProcessParameterValue]>> => Process_tryGetOutputsWithParameterBy(predicate, arg_1), processSequence));
}

/**
 * Returns the parameters implemented by the processes contained in these processes
 */
export function getParameters(processSequence: FSharpList<Process>): FSharpList<ProtocolParameter> {
    return List_distinct<ProtocolParameter>(collect<Process, ProtocolParameter>(Process_getParameters_30EF9E7B, processSequence), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns the characteristics describing the inputs and outputs of the processssequence
 */
export function getCharacteristics(processSequence: FSharpList<Process>): FSharpList<MaterialAttribute> {
    return List_distinct<MaterialAttribute>(collect<Process, MaterialAttribute>(Process_getCharacteristics_30EF9E7B, processSequence), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * If the processes contain a process implementing the given parameter, return the list of input files together with their according parameter values of this parameter
 */
export function getInputsWithCharacteristicBy(predicate: ((arg0: MaterialAttribute) => boolean), processSequence: FSharpList<Process>): FSharpList<[ProcessInput_$union, MaterialAttributeValue]> {
    return concat<[ProcessInput_$union, MaterialAttributeValue]>(choose<Process, FSharpList<[ProcessInput_$union, MaterialAttributeValue]>>((arg_1: Process): Option<FSharpList<[ProcessInput_$union, MaterialAttributeValue]>> => Process_tryGetInputsWithCharacteristicBy(predicate, arg_1), processSequence));
}

/**
 * If the processes contain a process implementing the given parameter, return the list of output files together with their according parameter values of this parameter
 */
export function getOutputsWithCharacteristicBy(predicate: ((arg0: MaterialAttribute) => boolean), processSequence: FSharpList<Process>): FSharpList<[ProcessOutput_$union, MaterialAttributeValue]> {
    return concat<[ProcessOutput_$union, MaterialAttributeValue]>(choose<Process, FSharpList<[ProcessOutput_$union, MaterialAttributeValue]>>((arg_1: Process): Option<FSharpList<[ProcessOutput_$union, MaterialAttributeValue]>> => Process_tryGetOutputsWithCharacteristicBy(predicate, arg_1), processSequence));
}

/**
 * If the processes contain a process implementing the given factor, return the list of output files together with their according factor values of this factor
 */
export function getOutputsWithFactorBy(predicate: ((arg0: Factor) => boolean), processSequence: FSharpList<Process>): FSharpList<[ProcessOutput_$union, FactorValue]> {
    return concat<[ProcessOutput_$union, FactorValue]>(choose<Process, FSharpList<[ProcessOutput_$union, FactorValue]>>((arg_1: Process): Option<FSharpList<[ProcessOutput_$union, FactorValue]>> => Process_tryGetOutputsWithFactorBy(predicate, arg_1), processSequence));
}

/**
 * Returns the factors implemented by the processes contained in these processes
 */
export function getFactors(processSequence: FSharpList<Process>): FSharpList<Factor> {
    return List_distinct<Factor>(collect<Process, Factor>(Process_getFactors_30EF9E7B, processSequence), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns the initial inputs final outputs of the processSequence, to which no processPoints
 */
export function getRootInputs(processSequence: FSharpList<Process>): FSharpList<ProcessInput_$union> {
    const inputs: FSharpList<ProcessInput_$union> = collect<Process, ProcessInput_$union>((p: Process): FSharpList<ProcessInput_$union> => defaultArg(p.Inputs, empty<ProcessInput_$union>()), processSequence);
    const outputs: FSharpSet<string> = ofList<string>(collect<Process, string>((p_1: Process): FSharpList<string> => map<ProcessOutput_$union, string>(ProcessOutput_getName_Z4A02997C, defaultArg(p_1.Outputs, empty<ProcessOutput_$union>())), processSequence), {
        Compare: comparePrimitives,
    });
    return filter<ProcessInput_$union>((i: ProcessInput_$union): boolean => !FSharpSet__Contains(outputs, ProcessInput_getName_Z38E7E853(i)), inputs);
}

/**
 * Returns the final outputs of the processSequence, which point to no further nodes
 */
export function getFinalOutputs(processSequence: FSharpList<Process>): FSharpList<ProcessOutput_$union> {
    const inputs: FSharpSet<string> = ofList<string>(collect<Process, string>((p: Process): FSharpList<string> => map<ProcessInput_$union, string>(ProcessInput_getName_Z38E7E853, defaultArg(p.Inputs, empty<ProcessInput_$union>())), processSequence), {
        Compare: comparePrimitives,
    });
    return filter<ProcessOutput_$union>((o: ProcessOutput_$union): boolean => !FSharpSet__Contains(inputs, ProcessOutput_getName_Z4A02997C(o)), collect<Process, ProcessOutput_$union>((p_1: Process): FSharpList<ProcessOutput_$union> => defaultArg(p_1.Outputs, empty<ProcessOutput_$union>()), processSequence));
}

/**
 * Returns the initial inputs final outputs of the processSequence, to which no processPoints
 */
export function getRootInputOf(processSequence: FSharpList<Process>, sample: string): FSharpList<string> {
    const mappings: FSharpMap<string, FSharpList<string>> = ofList_1<string, FSharpList<string>>(map<[string, FSharpList<[string, string]>], [string, FSharpList<string>]>((tupledArg: [string, FSharpList<[string, string]>]): [string, FSharpList<string>] => ([tupledArg[0], map<[string, string], string>((tuple_1: [string, string]): string => tuple_1[1], tupledArg[1])] as [string, FSharpList<string>]), List_groupBy<[string, string], string>((tuple: [string, string]): string => tuple[0], collect<Process, [string, string]>((p: Process): FSharpList<[string, string]> => List_distinct<[string, string]>(zip<string, string>(map<ProcessOutput_$union, string>(ProcessOutput__get_Name, value_3(p.Outputs)), map<ProcessInput_$union, string>(ProcessInput__get_Name, value_3(p.Inputs))), {
        Equals: equalArrays,
        GetHashCode: arrayHash,
    }), processSequence), {
        Equals: (x_1: string, y_1: string): boolean => (x_1 === y_1),
        GetHashCode: stringHash,
    })), {
        Compare: comparePrimitives,
    });
    const loop = (lastState_mut: FSharpList<string>, state_mut: FSharpList<string>): FSharpList<string> => {
        loop:
        while (true) {
            const lastState: FSharpList<string> = lastState_mut, state: FSharpList<string> = state_mut;
            if (equals(lastState, state)) {
                return state;
            }
            else {
                lastState_mut = state;
                state_mut = collect<string, string>((s: string): FSharpList<string> => defaultArg(FSharpMap__TryFind(mappings, s), singleton(s)), state);
                continue loop;
            }
            break;
        }
    };
    return loop(empty<string>(), singleton(sample));
}

/**
 * Returns the final outputs of the processSequence, which point to no further nodes
 */
export function getFinalOutputsOf(processSequence: FSharpList<Process>, sample: string): FSharpList<string> {
    const mappings: FSharpMap<string, FSharpList<string>> = ofList_1<string, FSharpList<string>>(map<[string, FSharpList<[string, string]>], [string, FSharpList<string>]>((tupledArg: [string, FSharpList<[string, string]>]): [string, FSharpList<string>] => ([tupledArg[0], map<[string, string], string>((tuple_1: [string, string]): string => tuple_1[1], tupledArg[1])] as [string, FSharpList<string>]), List_groupBy<[string, string], string>((tuple: [string, string]): string => tuple[0], collect<Process, [string, string]>((p: Process): FSharpList<[string, string]> => List_distinct<[string, string]>(zip<string, string>(map<ProcessInput_$union, string>(ProcessInput__get_Name, value_3(p.Inputs)), map<ProcessOutput_$union, string>(ProcessOutput__get_Name, value_3(p.Outputs))), {
        Equals: equalArrays,
        GetHashCode: arrayHash,
    }), processSequence), {
        Equals: (x_1: string, y_1: string): boolean => (x_1 === y_1),
        GetHashCode: stringHash,
    })), {
        Compare: comparePrimitives,
    });
    const loop = (lastState_mut: FSharpList<string>, state_mut: FSharpList<string>): FSharpList<string> => {
        loop:
        while (true) {
            const lastState: FSharpList<string> = lastState_mut, state: FSharpList<string> = state_mut;
            if (equals(lastState, state)) {
                return state;
            }
            else {
                lastState_mut = state;
                state_mut = collect<string, string>((s: string): FSharpList<string> => defaultArg(FSharpMap__TryFind(mappings, s), singleton(s)), state);
                continue loop;
            }
            break;
        }
    };
    return loop(empty<string>(), singleton(sample));
}

export function getUnits(processSequence: FSharpList<Process>): FSharpList<OntologyAnnotation> {
    return List_distinct<OntologyAnnotation>(collect<Process, OntologyAnnotation>(Process_getUnits_30EF9E7B, processSequence), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns the data the given processes contain
 */
export function getData(processSequence: FSharpList<Process>): FSharpList<Data> {
    return List_distinct<Data>(collect<Process, Data>(Process_getData_30EF9E7B, processSequence), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

export function getSources(processSequence: FSharpList<Process>): FSharpList<Source> {
    return List_distinct<Source>(collect<Process, Source>(Process_getSources_30EF9E7B, processSequence), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

export function getSamples(processSequence: FSharpList<Process>): FSharpList<Sample> {
    return List_distinct<Sample>(collect<Process, Sample>(Process_getSamples_30EF9E7B, processSequence), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

export function getMaterials(processSequence: FSharpList<Process>): FSharpList<Material> {
    return List_distinct<Material>(collect<Process, Material>(Process_getMaterials_30EF9E7B, processSequence), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

export function updateProtocols(protocols: FSharpList<Protocol>, processSequence: FSharpList<Process>): FSharpList<Process> {
    return map<Process, Process>((arg_1: Process): Process => Process_updateProtocol(protocols, arg_1), processSequence);
}

