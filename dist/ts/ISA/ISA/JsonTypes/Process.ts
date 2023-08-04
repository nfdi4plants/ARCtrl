import { Record, toString } from "../../../fable_modules/fable-library-ts/Types.js";
import { zip, tryPick, map as map_1, tryFind, append, collect, choose, FSharpList, empty, length } from "../../../fable_modules/fable-library-ts/List.js";
import { map, value as value_4, bind, Option, defaultArg } from "../../../fable_modules/fable-library-ts/Option.js";
import { ProcessInput_tryMaterial_102B6859, ProcessInput_trySample_102B6859, ProcessInput_tryData_102B6859, ProcessInput_trySource_102B6859, ProcessInput_tryGetCharacteristicValues_102B6859, ProcessInput_$reflection, ProcessInput_$union } from "./ProcessInput.js";
import { parse, int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { ProcessOutput_tryGetFactorValues_11830B70, ProcessOutput_tryGetCharacteristicValues_11830B70, ProcessOutput_$reflection, ProcessOutput_$union } from "./ProcessOutput.js";
import { ProcessParameterValue_$reflection, ProcessParameterValue } from "./ProcessParameterValue.js";
import { printf, toText } from "../../../fable_modules/fable-library-ts/String.js";
import { Protocol_$reflection, Protocol } from "./Protocol.js";
import { Comment$_$reflection, Comment$ } from "./Comment.js";
import { safeHash, equals, IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { IISAPrintable } from "../Printer.js";
import { getRecordFields, makeRecord, record_type, list_type, option_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { create, match } from "../../../fable_modules/fable-library-ts/RegExp.js";
import { ProtocolParameter_get_empty, ProtocolParameter } from "./ProtocolParameter.js";
import { MaterialAttributeValue } from "./MaterialAttributeValue.js";
import { List_distinct } from "../../../fable_modules/fable-library-ts/Seq2.js";
import { MaterialAttribute } from "./MaterialAttribute.js";
import { FactorValue } from "./FactorValue.js";
import { Factor } from "./Factor.js";
import { OntologyAnnotation } from "./OntologyAnnotation.js";
import { fromValueWithDefault } from "../OptionExtensions.js";
import { Source } from "./Source.js";
import { Data } from "./Data.js";
import { Sample } from "./Sample.js";
import { Material } from "./Material.js";
import { Update_UpdateOptions_$union, Update_UpdateOptions_UpdateByExistingAppendLists } from "../Update.js";
import { map2 } from "../../../fable_modules/fable-library-ts/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";

export class Process extends Record implements IEquatable<Process>, IISAPrintable {
    readonly ID: Option<string>;
    readonly Name: Option<string>;
    readonly ExecutesProtocol: Option<Protocol>;
    readonly ParameterValues: Option<FSharpList<ProcessParameterValue>>;
    readonly Performer: Option<string>;
    readonly Date: Option<string>;
    readonly PreviousProcess: Option<Process>;
    readonly NextProcess: Option<Process>;
    readonly Inputs: Option<FSharpList<ProcessInput_$union>>;
    readonly Outputs: Option<FSharpList<ProcessOutput_$union>>;
    readonly Comments: Option<FSharpList<Comment$>>;
    constructor(ID: Option<string>, Name: Option<string>, ExecutesProtocol: Option<Protocol>, ParameterValues: Option<FSharpList<ProcessParameterValue>>, Performer: Option<string>, Date$: Option<string>, PreviousProcess: Option<Process>, NextProcess: Option<Process>, Inputs: Option<FSharpList<ProcessInput_$union>>, Outputs: Option<FSharpList<ProcessOutput_$union>>, Comments: Option<FSharpList<Comment$>>) {
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
    Print(): string {
        const this$: Process = this;
        return toString(this$);
    }
    PrintCompact(): string {
        const this$: Process = this;
        const inputCount: int32 = length<ProcessInput_$union>(defaultArg(this$.Inputs, empty<ProcessInput_$union>())) | 0;
        const outputCount: int32 = length<ProcessOutput_$union>(defaultArg(this$.Outputs, empty<ProcessOutput_$union>())) | 0;
        const paramCount: int32 = length<ProcessParameterValue>(defaultArg(this$.ParameterValues, empty<ProcessParameterValue>())) | 0;
        const name: string = defaultArg(this$.Name, "Unnamed Process");
        return toText(printf("%s [%i Inputs -> %i Params -> %i Outputs]"))(name)(inputCount)(paramCount)(outputCount);
    }
}

export function Process_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.Process", [], Process, () => [["ID", option_type(string_type)], ["Name", option_type(string_type)], ["ExecutesProtocol", option_type(Protocol_$reflection())], ["ParameterValues", option_type(list_type(ProcessParameterValue_$reflection()))], ["Performer", option_type(string_type)], ["Date", option_type(string_type)], ["PreviousProcess", option_type(Process_$reflection())], ["NextProcess", option_type(Process_$reflection())], ["Inputs", option_type(list_type(ProcessInput_$reflection()))], ["Outputs", option_type(list_type(ProcessOutput_$reflection()))], ["Comments", option_type(list_type(Comment$_$reflection()))]]);
}

export function Process_make(id: Option<string>, name: Option<string>, executesProtocol: Option<Protocol>, parameterValues: Option<FSharpList<ProcessParameterValue>>, performer: Option<string>, date: Option<string>, previousProcess: Option<Process>, nextProcess: Option<Process>, inputs: Option<FSharpList<ProcessInput_$union>>, outputs: Option<FSharpList<ProcessOutput_$union>>, comments: Option<FSharpList<Comment$>>): Process {
    return new Process(id, name, executesProtocol, parameterValues, performer, date, previousProcess, nextProcess, inputs, outputs, comments);
}

export function Process_create_Z42860F3E(Id?: string, Name?: string, ExecutesProtocol?: Protocol, ParameterValues?: FSharpList<ProcessParameterValue>, Performer?: string, Date$?: string, PreviousProcess?: Process, NextProcess?: Process, Inputs?: FSharpList<ProcessInput_$union>, Outputs?: FSharpList<ProcessOutput_$union>, Comments?: FSharpList<Comment$>): Process {
    return Process_make(Id, Name, ExecutesProtocol, ParameterValues, Performer, Date$, PreviousProcess, NextProcess, Inputs, Outputs, Comments);
}

export function Process_get_empty(): Process {
    return Process_create_Z42860F3E();
}

export function Process_composeName(processNameRoot: string, i: int32): string {
    return `${processNameRoot}_${i}`;
}

export function Process_decomposeName_Z721C83C5(name: string): [string, Option<int32>] {
    const r: any = match(create("(?<name>.+)_(?<num>\\d+)"), name);
    if (r != null) {
        return [(r.groups && r.groups.name) || "", parse((r.groups && r.groups.num) || "", 511, false, 32)] as [string, Option<int32>];
    }
    else {
        return [name, void 0] as [string, Option<int32>];
    }
}

/**
 * Returns the name of the protocol the given process executes
 */
export function Process_tryGetProtocolName_716E708F(p: Process): Option<string> {
    return bind<Protocol, string>((p_1: Protocol): Option<string> => p_1.Name, p.ExecutesProtocol);
}

/**
 * Returns the name of the protocol the given process executes
 */
export function Process_getProtocolName_716E708F(p: Process): string {
    return value_4(bind<Protocol, string>((p_1: Protocol): Option<string> => p_1.Name, p.ExecutesProtocol));
}

/**
 * Returns the parameter values describing the process
 */
export function Process_getParameterValues_716E708F(p: Process): FSharpList<ProcessParameterValue> {
    return defaultArg(p.ParameterValues, empty<ProcessParameterValue>());
}

/**
 * Returns the parameters describing the process
 */
export function Process_getParameters_716E708F(p: Process): FSharpList<ProtocolParameter> {
    return choose<ProcessParameterValue, ProtocolParameter>((pv: ProcessParameterValue): Option<ProtocolParameter> => pv.Category, Process_getParameterValues_716E708F(p));
}

/**
 * Returns the characteristics describing the inputs of the process
 */
export function Process_getInputCharacteristicValues_716E708F(p: Process): FSharpList<MaterialAttributeValue> {
    const matchValue: Option<FSharpList<ProcessInput_$union>> = p.Inputs;
    if (matchValue == null) {
        return empty<MaterialAttributeValue>();
    }
    else {
        return List_distinct<MaterialAttributeValue>(collect<ProcessInput_$union, MaterialAttributeValue>((inp: ProcessInput_$union): FSharpList<MaterialAttributeValue> => defaultArg(ProcessInput_tryGetCharacteristicValues_102B6859(inp), empty<MaterialAttributeValue>()), value_4(matchValue)), {
            Equals: equals,
            GetHashCode: safeHash,
        });
    }
}

/**
 * Returns the characteristics describing the outputs of the process
 */
export function Process_getOutputCharacteristicValues_716E708F(p: Process): FSharpList<MaterialAttributeValue> {
    const matchValue: Option<FSharpList<ProcessOutput_$union>> = p.Outputs;
    if (matchValue == null) {
        return empty<MaterialAttributeValue>();
    }
    else {
        return List_distinct<MaterialAttributeValue>(collect<ProcessOutput_$union, MaterialAttributeValue>((out: ProcessOutput_$union): FSharpList<MaterialAttributeValue> => defaultArg(ProcessOutput_tryGetCharacteristicValues_11830B70(out), empty<MaterialAttributeValue>()), value_4(matchValue)), {
            Equals: equals,
            GetHashCode: safeHash,
        });
    }
}

/**
 * Returns the characteristic values describing the inputs and outputs of the process
 */
export function Process_getCharacteristicValues_716E708F(p: Process): FSharpList<MaterialAttributeValue> {
    return List_distinct<MaterialAttributeValue>(append(Process_getInputCharacteristicValues_716E708F(p), Process_getOutputCharacteristicValues_716E708F(p)), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns the characteristics describing the inputs and outputs of the process
 */
export function Process_getCharacteristics_716E708F(p: Process): FSharpList<MaterialAttribute> {
    return List_distinct<MaterialAttribute>(choose<MaterialAttributeValue, MaterialAttribute>((cv: MaterialAttributeValue): Option<MaterialAttribute> => cv.Category, Process_getCharacteristicValues_716E708F(p)), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns the factor values of the samples of the process
 */
export function Process_getFactorValues_716E708F(p: Process): FSharpList<FactorValue> {
    return List_distinct<FactorValue>(collect<ProcessOutput_$union, FactorValue>((arg_1: ProcessOutput_$union): FSharpList<FactorValue> => defaultArg(ProcessOutput_tryGetFactorValues_11830B70(arg_1), empty<FactorValue>()), defaultArg(p.Outputs, empty<ProcessOutput_$union>())), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns the factors of the samples of the process
 */
export function Process_getFactors_716E708F(p: Process): FSharpList<Factor> {
    return List_distinct<Factor>(choose<FactorValue, Factor>((fv: FactorValue): Option<Factor> => fv.Category, Process_getFactorValues_716E708F(p)), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

/**
 * Returns the units of the process
 */
export function Process_getUnits_716E708F(p: Process): FSharpList<OntologyAnnotation> {
    return append(choose<MaterialAttributeValue, OntologyAnnotation>((cv: MaterialAttributeValue): Option<OntologyAnnotation> => cv.Unit, Process_getCharacteristicValues_716E708F(p)), append(choose<ProcessParameterValue, OntologyAnnotation>((pv: ProcessParameterValue): Option<OntologyAnnotation> => pv.Unit, Process_getParameterValues_716E708F(p)), choose<FactorValue, OntologyAnnotation>((fv: FactorValue): Option<OntologyAnnotation> => fv.Unit, Process_getFactorValues_716E708F(p))));
}

/**
 * If the process implements the given parameter, return the list of input files together with their according parameter values of this parameter
 */
export function Process_tryGetInputsWithParameterBy(predicate: ((arg0: ProtocolParameter) => boolean), p: Process): Option<FSharpList<[ProcessInput_$union, ProcessParameterValue]>> {
    const matchValue: Option<FSharpList<ProcessParameterValue>> = p.ParameterValues;
    if (matchValue == null) {
        return void 0;
    }
    else {
        const matchValue_1: Option<ProcessParameterValue> = tryFind<ProcessParameterValue>((pv: ProcessParameterValue): boolean => predicate(defaultArg(pv.Category, ProtocolParameter_get_empty())), value_4(matchValue));
        if (matchValue_1 == null) {
            return void 0;
        }
        else {
            const paramValue: ProcessParameterValue = value_4(matchValue_1);
            return map<FSharpList<ProcessInput_$union>, FSharpList<[ProcessInput_$union, ProcessParameterValue]>>((list_1: FSharpList<ProcessInput_$union>): FSharpList<[ProcessInput_$union, ProcessParameterValue]> => map_1<ProcessInput_$union, [ProcessInput_$union, ProcessParameterValue]>((i: ProcessInput_$union): [ProcessInput_$union, ProcessParameterValue] => ([i, paramValue] as [ProcessInput_$union, ProcessParameterValue]), list_1), p.Inputs);
        }
    }
}

/**
 * If the process implements the given parameter, return the list of output files together with their according parameter values of this parameter
 */
export function Process_tryGetOutputsWithParameterBy(predicate: ((arg0: ProtocolParameter) => boolean), p: Process): Option<FSharpList<[ProcessOutput_$union, ProcessParameterValue]>> {
    const matchValue: Option<FSharpList<ProcessParameterValue>> = p.ParameterValues;
    if (matchValue == null) {
        return void 0;
    }
    else {
        const matchValue_1: Option<ProcessParameterValue> = tryFind<ProcessParameterValue>((pv: ProcessParameterValue): boolean => predicate(defaultArg(pv.Category, ProtocolParameter_get_empty())), value_4(matchValue));
        if (matchValue_1 == null) {
            return void 0;
        }
        else {
            const paramValue: ProcessParameterValue = value_4(matchValue_1);
            return map<FSharpList<ProcessOutput_$union>, FSharpList<[ProcessOutput_$union, ProcessParameterValue]>>((list_1: FSharpList<ProcessOutput_$union>): FSharpList<[ProcessOutput_$union, ProcessParameterValue]> => map_1<ProcessOutput_$union, [ProcessOutput_$union, ProcessParameterValue]>((i: ProcessOutput_$union): [ProcessOutput_$union, ProcessParameterValue] => ([i, paramValue] as [ProcessOutput_$union, ProcessParameterValue]), list_1), p.Outputs);
        }
    }
}

/**
 * If the process implements the given characteristic, return the list of input files together with their according characteristic values of this characteristic
 */
export function Process_tryGetInputsWithCharacteristicBy(predicate: ((arg0: MaterialAttribute) => boolean), p: Process): Option<FSharpList<[ProcessInput_$union, MaterialAttributeValue]>> {
    const matchValue: Option<FSharpList<ProcessInput_$union>> = p.Inputs;
    if (matchValue == null) {
        return void 0;
    }
    else {
        return fromValueWithDefault<FSharpList<[ProcessInput_$union, MaterialAttributeValue]>>(empty<[ProcessInput_$union, MaterialAttributeValue]>(), choose<ProcessInput_$union, [ProcessInput_$union, MaterialAttributeValue]>((i: ProcessInput_$union): Option<[ProcessInput_$union, MaterialAttributeValue]> => tryPick<MaterialAttributeValue, [ProcessInput_$union, MaterialAttributeValue]>((mv: MaterialAttributeValue): Option<[ProcessInput_$union, MaterialAttributeValue]> => {
            const matchValue_1: Option<MaterialAttribute> = mv.Category;
            let matchResult: int32, m_1: MaterialAttribute;
            if (matchValue_1 != null) {
                if (predicate(value_4(matchValue_1))) {
                    matchResult = 0;
                    m_1 = value_4(matchValue_1);
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
                    return [i, mv] as [ProcessInput_$union, MaterialAttributeValue];
                default:
                    return void 0;
            }
        }, defaultArg(ProcessInput_tryGetCharacteristicValues_102B6859(i), empty<MaterialAttributeValue>())), value_4(matchValue)));
    }
}

/**
 * If the process implements the given characteristic, return the list of output files together with their according characteristic values of this characteristic
 */
export function Process_tryGetOutputsWithCharacteristicBy(predicate: ((arg0: MaterialAttribute) => boolean), p: Process): Option<FSharpList<[ProcessOutput_$union, MaterialAttributeValue]>> {
    const matchValue: Option<FSharpList<ProcessInput_$union>> = p.Inputs;
    const matchValue_1: Option<FSharpList<ProcessOutput_$union>> = p.Outputs;
    let matchResult: int32, is: FSharpList<ProcessInput_$union>, os: FSharpList<ProcessOutput_$union>;
    if (matchValue != null) {
        if (matchValue_1 != null) {
            matchResult = 0;
            is = value_4(matchValue);
            os = value_4(matchValue_1);
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
            return fromValueWithDefault<FSharpList<[ProcessOutput_$union, MaterialAttributeValue]>>(empty<[ProcessOutput_$union, MaterialAttributeValue]>(), choose<[ProcessInput_$union, ProcessOutput_$union], [ProcessOutput_$union, MaterialAttributeValue]>((tupledArg: [ProcessInput_$union, ProcessOutput_$union]): Option<[ProcessOutput_$union, MaterialAttributeValue]> => tryPick<MaterialAttributeValue, [ProcessOutput_$union, MaterialAttributeValue]>((mv: MaterialAttributeValue): Option<[ProcessOutput_$union, MaterialAttributeValue]> => {
                const matchValue_3: Option<MaterialAttribute> = mv.Category;
                let matchResult_1: int32, m_1: MaterialAttribute;
                if (matchValue_3 != null) {
                    if (predicate(value_4(matchValue_3))) {
                        matchResult_1 = 0;
                        m_1 = value_4(matchValue_3);
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
                        return [tupledArg[1], mv] as [ProcessOutput_$union, MaterialAttributeValue];
                    default:
                        return void 0;
                }
            }, defaultArg(ProcessInput_tryGetCharacteristicValues_102B6859(tupledArg[0]), empty<MaterialAttributeValue>())), zip<ProcessInput_$union, ProcessOutput_$union>(is!, os!)));
        default:
            return void 0;
    }
}

/**
 * If the process implements the given factor, return the list of output files together with their according factor values of this factor
 */
export function Process_tryGetOutputsWithFactorBy(predicate: ((arg0: Factor) => boolean), p: Process): Option<FSharpList<[ProcessOutput_$union, FactorValue]>> {
    const matchValue: Option<FSharpList<ProcessOutput_$union>> = p.Outputs;
    if (matchValue == null) {
        return void 0;
    }
    else {
        return fromValueWithDefault<FSharpList<[ProcessOutput_$union, FactorValue]>>(empty<[ProcessOutput_$union, FactorValue]>(), choose<ProcessOutput_$union, [ProcessOutput_$union, FactorValue]>((o: ProcessOutput_$union): Option<[ProcessOutput_$union, FactorValue]> => tryPick<FactorValue, [ProcessOutput_$union, FactorValue]>((mv: FactorValue): Option<[ProcessOutput_$union, FactorValue]> => {
            const matchValue_1: Option<Factor> = mv.Category;
            let matchResult: int32, m_1: Factor;
            if (matchValue_1 != null) {
                if (predicate(value_4(matchValue_1))) {
                    matchResult = 0;
                    m_1 = value_4(matchValue_1);
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
                    return [o, mv] as [ProcessOutput_$union, FactorValue];
                default:
                    return void 0;
            }
        }, defaultArg(ProcessOutput_tryGetFactorValues_11830B70(o), empty<FactorValue>())), value_4(matchValue)));
    }
}

export function Process_getSources_716E708F(p: Process): FSharpList<Source> {
    return List_distinct<Source>(choose<ProcessInput_$union, Source>(ProcessInput_trySource_102B6859, defaultArg(p.Inputs, empty<ProcessInput_$union>())), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

export function Process_getData_716E708F(p: Process): FSharpList<Data> {
    return List_distinct<Data>(append(choose<ProcessInput_$union, Data>(ProcessInput_tryData_102B6859, defaultArg(p.Inputs, empty<ProcessInput_$union>())), choose<ProcessInput_$union, Data>(ProcessInput_tryData_102B6859, defaultArg(p.Inputs, empty<ProcessInput_$union>()))), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

export function Process_getSamples_716E708F(p: Process): FSharpList<Sample> {
    return List_distinct<Sample>(append(choose<ProcessInput_$union, Sample>(ProcessInput_trySample_102B6859, defaultArg(p.Inputs, empty<ProcessInput_$union>())), choose<ProcessInput_$union, Sample>(ProcessInput_trySample_102B6859, defaultArg(p.Inputs, empty<ProcessInput_$union>()))), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

export function Process_getMaterials_716E708F(p: Process): FSharpList<Material> {
    return List_distinct<Material>(append(choose<ProcessInput_$union, Material>(ProcessInput_tryMaterial_102B6859, defaultArg(p.Inputs, empty<ProcessInput_$union>())), choose<ProcessInput_$union, Material>(ProcessInput_tryMaterial_102B6859, defaultArg(p.Inputs, empty<ProcessInput_$union>()))), {
        Equals: equals,
        GetHashCode: safeHash,
    });
}

export function Process_updateProtocol(referenceProtocols: FSharpList<Protocol>, p: Process): Process {
    let this$: Update_UpdateOptions_$union, recordType_1: Protocol, recordType_2: Protocol;
    const matchValue: Option<Protocol> = p.ExecutesProtocol;
    let matchResult: int32, protocol_1: Protocol;
    if (matchValue != null) {
        if (value_4(matchValue).Name != null) {
            matchResult = 0;
            protocol_1 = value_4(matchValue);
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
            const matchValue_1: Option<Protocol> = tryFind<Protocol>((prot: Protocol): boolean => (value_4(prot.Name) === defaultArg(protocol_1!.Name, "")), referenceProtocols);
            if (matchValue_1 != null) {
                return new Process(p.ID, p.Name, (this$ = Update_UpdateOptions_UpdateByExistingAppendLists(), (recordType_1 = protocol_1!, (recordType_2 = value_4(matchValue_1), (this$.tag === /* UpdateAllAppendLists */ 2) ? (makeRecord(Protocol_$reflection(), map2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Protocol) : ((this$.tag === /* UpdateByExisting */ 1) ? (makeRecord(Protocol_$reflection(), map2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Protocol) : ((this$.tag === /* UpdateByExistingAppendLists */ 3) ? (makeRecord(Protocol_$reflection(), map2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Protocol) : recordType_2))))), p.ParameterValues, p.Performer, p.Date, p.PreviousProcess, p.NextProcess, p.Inputs, p.Outputs, p.Comments);
            }
            else {
                return p;
            }
        }
        default:
            return p;
    }
}

