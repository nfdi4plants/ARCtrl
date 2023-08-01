import { value as value_18, Option } from "../../fable_modules/fable-library-ts/Option.js";
import { Value_getText_Z277CD705, Value_$union } from "../ISA/JsonTypes/Value.js";
import { ProtocolParameter_getNameText_2762A46F, ProtocolParameter } from "../ISA/JsonTypes/ProtocolParameter.js";
import { replace } from "../../fable_modules/fable-library-ts/String.js";
import { int32 } from "../../fable_modules/fable-library-ts/Int32.js";
import { ProcessParameterValue } from "../ISA/JsonTypes/ProcessParameterValue.js";
import { list as list_2, toString, nil, object as object_22 } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { map, FSharpList, choose } from "../../fable_modules/fable-library-ts/List.js";
import { uncurry2, equals } from "../../fable_modules/fable-library-ts/Util.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library-ts/Seq.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions_$ctor, ConverterOptions, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID } from "./ConverterOptions.js";
import { tryInclude } from "./GEncode.js";
import { Protocol_decoder, Protocol_encoder, ProtocolParameter_decoder, ProtocolParameter_encoder } from "./Protocol.js";
import { Value_decoder, Value_encoder } from "./Factor.js";
import { OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "./Ontology.js";
import { list as list_1, string, IOptionalGetter, IGetters, object as object_23 } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { FSharpResult$2_Ok, FSharpResult$2_Error, FSharpResult$2_$union } from "../../fable_modules/fable-library-ts/Choice.js";
import { ErrorReason_$union } from "../../fable_modules/Thoth.Json.10.1.0/Types.fs.js";
import { OntologyAnnotation } from "../ISA/JsonTypes/OntologyAnnotation.js";
import { uri, fromString } from "./Decode.js";
import { ProcessInput_Source, ProcessInput_Sample, ProcessInput_Data, ProcessInput_Material, ProcessInput_$union, ProcessInput } from "../ISA/JsonTypes/ProcessInput.js";
import { Data_decoder, Sample_decoder, Source_decoder, Source_encoder, Data_encoder, Sample_encoder } from "./Data.js";
import { Material_decoder, Material_encoder } from "./Material.js";
import { Source } from "../ISA/JsonTypes/Source.js";
import { Sample } from "../ISA/JsonTypes/Sample.js";
import { Data } from "../ISA/JsonTypes/Data.js";
import { Material } from "../ISA/JsonTypes/Material.js";
import { ProcessOutput_Sample, ProcessOutput_Data, ProcessOutput_Material, ProcessOutput_$union, ProcessOutput } from "../ISA/JsonTypes/ProcessOutput.js";
import { URIModule_toString } from "../ISA/JsonTypes/URI.js";
import { Process } from "../ISA/JsonTypes/Process.js";
import { decoder as decoder_4, encoder } from "./Comment.js";
import { Protocol } from "../ISA/JsonTypes/Protocol.js";
import { Comment$ } from "../ISA/JsonTypes/Comment.js";

export function ProcessParameterValue_genID(p: ProcessParameterValue): string {
    const matchValue: Option<Value_$union> = p.Value;
    const matchValue_1: Option<ProtocolParameter> = p.Category;
    let matchResult: int32, c: ProtocolParameter, v: Value_$union;
    if (matchValue != null) {
        if (matchValue_1 != null) {
            matchResult = 0;
            c = value_18(matchValue_1);
            v = value_18(matchValue);
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
            return (("#Param_" + replace(ProtocolParameter_getNameText_2762A46F(c!), " ", "_")) + "_") + replace(Value_getText_Z277CD705(v!), " ", "_");
        default:
            return "#EmptyParameterValue";
    }
}

export function ProcessParameterValue_encoder(options: ConverterOptions, oa: any): any {
    return object_22(choose<[string, any], [string, any]>((tupledArg: [string, any]): Option<[string, any]> => {
        const v: any = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v] as [string, any];
        }
    }, toList<[string, any]>(delay<[string, any]>((): Iterable<[string, any]> => {
        let value: any, s: string;
        return append<[string, any]>(ConverterOptions__get_SetID(options) ? singleton<[string, any]>(["@id", (value = ProcessParameterValue_genID(oa as ProcessParameterValue), (typeof value === "string") ? ((s = (value as string), s)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => {
            let value_2: any, s_1: string;
            return append<[string, any]>(ConverterOptions__get_IncludeType(options) ? singleton<[string, any]>(["@type", (value_2 = "ProcessParameterValue", (typeof value_2 === "string") ? ((s_1 = (value_2 as string), s_1)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("category", (oa_1: any): any => ProtocolParameter_encoder(options, oa_1), oa["Category"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("value", (value_4: any): any => Value_encoder(options, value_4), oa["Value"])), delay<[string, any]>((): Iterable<[string, any]> => singleton<[string, any]>(tryInclude<string>("unit", (oa_2: any): any => OntologyAnnotation_encoder(options, oa_2), oa["Unit"]))))))));
        }));
    }))));
}

export function ProcessParameterValue_decoder(options: ConverterOptions): ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<ProcessParameterValue, [string, ErrorReason_$union]>)) {
    return (path: string): ((arg0: any) => FSharpResult$2_$union<ProcessParameterValue, [string, ErrorReason_$union]>) => ((v: any): FSharpResult$2_$union<ProcessParameterValue, [string, ErrorReason_$union]> => object_23<ProcessParameterValue>((get$: IGetters): ProcessParameterValue => {
        let arg_1: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<ProtocolParameter, [string, ErrorReason_$union]>)), objectArg: IOptionalGetter, objectArg_1: IOptionalGetter, arg_5: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<OntologyAnnotation, [string, ErrorReason_$union]>)), objectArg_2: IOptionalGetter;
        return new ProcessParameterValue((arg_1 = ProtocolParameter_decoder(options), (objectArg = get$.Optional, objectArg.Field<ProtocolParameter>("category", uncurry2(arg_1)))), (objectArg_1 = get$.Optional, objectArg_1.Field<Value_$union>("value", (s: string, json: any): FSharpResult$2_$union<Value_$union, [string, ErrorReason_$union]> => Value_decoder(options, s, json))), (arg_5 = OntologyAnnotation_decoder(options), (objectArg_2 = get$.Optional, objectArg_2.Field<OntologyAnnotation>("unit", uncurry2(arg_5)))));
    }, path, v));
}

export function ProcessParameterValue_fromString(s: string): ProcessParameterValue {
    return fromString<ProcessParameterValue>(uncurry2(ProcessParameterValue_decoder(ConverterOptions_$ctor())), s);
}

export function ProcessParameterValue_toString(p: ProcessParameterValue): string {
    return toString(2, ProcessParameterValue_encoder(ConverterOptions_$ctor(), p));
}

/**
 * exports in json-ld format
 */
export function ProcessParameterValue_toStringLD(p: ProcessParameterValue): string {
    let returnVal: ConverterOptions;
    return toString(2, ProcessParameterValue_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), p));
}

export function ProcessInput_encoder(options: ConverterOptions, value: any): any {
    if (value instanceof ProcessInput) {
        if (value.tag === /* Sample */ 1) {
            return Sample_encoder(options, (value as ProcessInput<1>).fields[0]);
        }
        else if (value.tag === /* Data */ 2) {
            return Data_encoder(options, (value as ProcessInput<2>).fields[0]);
        }
        else if (value.tag === /* Material */ 3) {
            return Material_encoder(options, (value as ProcessInput<3>).fields[0]);
        }
        else {
            return Source_encoder(options, (value as ProcessInput<0>).fields[0]);
        }
    }
    else {
        return nil;
    }
}

export function ProcessInput_decoder(options: ConverterOptions, s: string, json: any): FSharpResult$2_$union<ProcessInput_$union, [string, ErrorReason_$union]> {
    const matchValue: FSharpResult$2_$union<Source, [string, ErrorReason_$union]> = Source_decoder(options, s, json);
    if (matchValue.tag === /* Error */ 1) {
        const matchValue_1: FSharpResult$2_$union<Sample, [string, ErrorReason_$union]> = Sample_decoder(options, s, json);
        if (matchValue_1.tag === /* Error */ 1) {
            const matchValue_2: FSharpResult$2_$union<Data, [string, ErrorReason_$union]> = Data_decoder(options, s, json);
            if (matchValue_2.tag === /* Error */ 1) {
                const matchValue_3: FSharpResult$2_$union<Material, [string, ErrorReason_$union]> = Material_decoder(options, s, json);
                if (matchValue_3.tag === /* Error */ 1) {
                    return FSharpResult$2_Error<ProcessInput_$union, [string, ErrorReason_$union]>(matchValue_3.fields[0]);
                }
                else {
                    return FSharpResult$2_Ok<ProcessInput_$union, [string, ErrorReason_$union]>(ProcessInput_Material(matchValue_3.fields[0]));
                }
            }
            else {
                return FSharpResult$2_Ok<ProcessInput_$union, [string, ErrorReason_$union]>(ProcessInput_Data(matchValue_2.fields[0]));
            }
        }
        else {
            return FSharpResult$2_Ok<ProcessInput_$union, [string, ErrorReason_$union]>(ProcessInput_Sample(matchValue_1.fields[0]));
        }
    }
    else {
        return FSharpResult$2_Ok<ProcessInput_$union, [string, ErrorReason_$union]>(ProcessInput_Source(matchValue.fields[0]));
    }
}

export function ProcessInput_fromString(s: string): ProcessInput_$union {
    let options: ConverterOptions;
    return fromString<ProcessInput_$union>(uncurry2((options = ConverterOptions_$ctor(), (s_1: string): ((arg0: any) => FSharpResult$2_$union<ProcessInput_$union, [string, ErrorReason_$union]>) => ((json: any): FSharpResult$2_$union<ProcessInput_$union, [string, ErrorReason_$union]> => ProcessInput_decoder(options, s_1, json)))), s);
}

export function ProcessInput_toString(m: ProcessInput_$union): string {
    return toString(2, ProcessInput_encoder(ConverterOptions_$ctor(), m));
}

export function ProcessInput_toStringLD(m: ProcessInput_$union): string {
    let returnVal: ConverterOptions;
    return toString(2, ProcessInput_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), m));
}

export function ProcessOutput_encoder(options: ConverterOptions, value: any): any {
    if (value instanceof ProcessOutput) {
        if (value.tag === /* Data */ 1) {
            return Data_encoder(options, (value as ProcessOutput<1>).fields[0]);
        }
        else if (value.tag === /* Material */ 2) {
            return Material_encoder(options, (value as ProcessOutput<2>).fields[0]);
        }
        else {
            return Sample_encoder(options, (value as ProcessOutput<0>).fields[0]);
        }
    }
    else {
        return nil;
    }
}

export function ProcessOutput_decoder(options: ConverterOptions, s: string, json: any): FSharpResult$2_$union<ProcessOutput_$union, [string, ErrorReason_$union]> {
    const matchValue: FSharpResult$2_$union<Sample, [string, ErrorReason_$union]> = Sample_decoder(options, s, json);
    if (matchValue.tag === /* Error */ 1) {
        const matchValue_1: FSharpResult$2_$union<Data, [string, ErrorReason_$union]> = Data_decoder(options, s, json);
        if (matchValue_1.tag === /* Error */ 1) {
            const matchValue_2: FSharpResult$2_$union<Material, [string, ErrorReason_$union]> = Material_decoder(options, s, json);
            if (matchValue_2.tag === /* Error */ 1) {
                return FSharpResult$2_Error<ProcessOutput_$union, [string, ErrorReason_$union]>(matchValue_2.fields[0]);
            }
            else {
                return FSharpResult$2_Ok<ProcessOutput_$union, [string, ErrorReason_$union]>(ProcessOutput_Material(matchValue_2.fields[0]));
            }
        }
        else {
            return FSharpResult$2_Ok<ProcessOutput_$union, [string, ErrorReason_$union]>(ProcessOutput_Data(matchValue_1.fields[0]));
        }
    }
    else {
        return FSharpResult$2_Ok<ProcessOutput_$union, [string, ErrorReason_$union]>(ProcessOutput_Sample(matchValue.fields[0]));
    }
}

export function ProcessOutput_fromString(s: string): ProcessOutput_$union {
    let options: ConverterOptions;
    return fromString<ProcessOutput_$union>(uncurry2((options = ConverterOptions_$ctor(), (s_1: string): ((arg0: any) => FSharpResult$2_$union<ProcessOutput_$union, [string, ErrorReason_$union]>) => ((json: any): FSharpResult$2_$union<ProcessOutput_$union, [string, ErrorReason_$union]> => ProcessOutput_decoder(options, s_1, json)))), s);
}

export function ProcessOutput_toString(m: ProcessInput_$union): string {
    return toString(2, ProcessOutput_encoder(ConverterOptions_$ctor(), m));
}

export function Process_genID(p: Process): string {
    const matchValue: Option<string> = p.ID;
    if (matchValue == null) {
        const matchValue_1: Option<string> = p.Name;
        if (matchValue_1 == null) {
            return "#EmptyProcess";
        }
        else {
            return "#Process_" + replace(value_18(matchValue_1), " ", "_");
        }
    }
    else {
        return URIModule_toString(value_18(matchValue));
    }
}

export function Process_encoder(options: ConverterOptions, oa: any): any {
    return object_22(choose<[string, any], [string, any]>((tupledArg: [string, any]): Option<[string, any]> => {
        const v: any = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v] as [string, any];
        }
    }, toList<[string, any]>(delay<[string, any]>((): Iterable<[string, any]> => {
        let value: any, s: string;
        return append<[string, any]>(ConverterOptions__get_SetID(options) ? singleton<[string, any]>(["@id", (value = Process_genID(oa as Process), (typeof value === "string") ? ((s = (value as string), s)) : nil)] as [string, any]) : singleton<[string, any]>(tryInclude<string>("@id", (value_2: any): any => {
            let s_1: string;
            const value_3: any = value_2;
            return (typeof value_3 === "string") ? ((s_1 = (value_3 as string), s_1)) : nil;
        }, oa["ID"])), delay<[string, any]>((): Iterable<[string, any]> => {
            let value_5: any, s_2: string;
            return append<[string, any]>(ConverterOptions__get_IncludeType(options) ? singleton<[string, any]>(["@type", (value_5 = "Process", (typeof value_5 === "string") ? ((s_2 = (value_5 as string), s_2)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("name", (value_7: any): any => {
                let s_3: string;
                const value_8: any = value_7;
                return (typeof value_8 === "string") ? ((s_3 = (value_8 as string), s_3)) : nil;
            }, oa["Name"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("executesProtocol", (oa_1: any): any => Protocol_encoder(options, oa_1), oa["ExecutesProtocol"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("parameterValues", (oa_2: any): any => ProcessParameterValue_encoder(options, oa_2), oa["ParameterValues"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("performer", (value_10: any): any => {
                let s_4: string;
                const value_11: any = value_10;
                return (typeof value_11 === "string") ? ((s_4 = (value_11 as string), s_4)) : nil;
            }, oa["Performer"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("date", (value_13: any): any => {
                let s_5: string;
                const value_14: any = value_13;
                return (typeof value_14 === "string") ? ((s_5 = (value_14 as string), s_5)) : nil;
            }, oa["Date"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("previousProcess", (oa_3: any): any => Process_encoder(options, oa_3), oa["PreviousProcess"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("nextProcess", (oa_4: any): any => Process_encoder(options, oa_4), oa["NextProcess"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("inputs", (value_16: any): any => ProcessInput_encoder(options, value_16), oa["Inputs"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("outputs", (value_17: any): any => ProcessOutput_encoder(options, value_17), oa["Outputs"])), delay<[string, any]>((): Iterable<[string, any]> => singleton<[string, any]>(tryInclude<string>("comments", (comment: any): any => encoder(options, comment), oa["Comments"]))))))))))))))))))))));
        }));
    }))));
}

export function Process_decoder(options: ConverterOptions): ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Process, [string, ErrorReason_$union]>)) {
    return (path_7: string): ((arg0: any) => FSharpResult$2_$union<Process, [string, ErrorReason_$union]>) => ((v: any): FSharpResult$2_$union<Process, [string, ErrorReason_$union]> => object_23<Process>((get$: IGetters): Process => {
        let objectArg: IOptionalGetter, objectArg_1: IOptionalGetter, arg_5: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Protocol, [string, ErrorReason_$union]>)), objectArg_2: IOptionalGetter, arg_7: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<ProcessParameterValue>, [string, ErrorReason_$union]>)), decoder: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<ProcessParameterValue, [string, ErrorReason_$union]>)), objectArg_3: IOptionalGetter, objectArg_4: IOptionalGetter, objectArg_5: IOptionalGetter, arg_13: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Process, [string, ErrorReason_$union]>)), objectArg_6: IOptionalGetter, arg_15: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Process, [string, ErrorReason_$union]>)), objectArg_7: IOptionalGetter, objectArg_8: IOptionalGetter, objectArg_9: IOptionalGetter, arg_21: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]>)), decoder_3: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Comment$, [string, ErrorReason_$union]>)), objectArg_10: IOptionalGetter;
        return new Process((objectArg = get$.Optional, objectArg.Field<string>("@id", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field<string>("name", string)), (arg_5 = Protocol_decoder(options), (objectArg_2 = get$.Optional, objectArg_2.Field<Protocol>("executesProtocol", uncurry2(arg_5)))), (arg_7 = ((decoder = ProcessParameterValue_decoder(options), (path_1: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<ProcessParameterValue>, [string, ErrorReason_$union]>) => ((value_1: any): FSharpResult$2_$union<FSharpList<ProcessParameterValue>, [string, ErrorReason_$union]> => list_1<ProcessParameterValue>(uncurry2(decoder), path_1, value_1)))), (objectArg_3 = get$.Optional, objectArg_3.Field<FSharpList<ProcessParameterValue>>("parameterValues", uncurry2(arg_7)))), (objectArg_4 = get$.Optional, objectArg_4.Field<string>("performer", string)), (objectArg_5 = get$.Optional, objectArg_5.Field<string>("date", string)), (arg_13 = Process_decoder(options), (objectArg_6 = get$.Optional, objectArg_6.Field<Process>("previousProcess", uncurry2(arg_13)))), (arg_15 = Process_decoder(options), (objectArg_7 = get$.Optional, objectArg_7.Field<Process>("nextProcess", uncurry2(arg_15)))), (objectArg_8 = get$.Optional, objectArg_8.Field<FSharpList<ProcessInput_$union>>("inputs", (path_4: string, value_4: any): FSharpResult$2_$union<FSharpList<ProcessInput_$union>, [string, ErrorReason_$union]> => list_1<ProcessInput_$union>((s_1: string, json_1: any): FSharpResult$2_$union<ProcessInput_$union, [string, ErrorReason_$union]> => ProcessInput_decoder(options, s_1, json_1), path_4, value_4))), (objectArg_9 = get$.Optional, objectArg_9.Field<FSharpList<ProcessOutput_$union>>("outputs", (path_5: string, value_5: any): FSharpResult$2_$union<FSharpList<ProcessOutput_$union>, [string, ErrorReason_$union]> => list_1<ProcessOutput_$union>((s_2: string, json_2: any): FSharpResult$2_$union<ProcessOutput_$union, [string, ErrorReason_$union]> => ProcessOutput_decoder(options, s_2, json_2), path_5, value_5))), (arg_21 = ((decoder_3 = decoder_4(options), (path_6: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]>) => ((value_6: any): FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]> => list_1<Comment$>(uncurry2(decoder_3), path_6, value_6)))), (objectArg_10 = get$.Optional, objectArg_10.Field<FSharpList<Comment$>>("comments", uncurry2(arg_21)))));
    }, path_7, v));
}

export function Process_fromString(s: string): Process {
    return fromString<Process>(uncurry2(Process_decoder(ConverterOptions_$ctor())), s);
}

export function Process_toString(p: Process): string {
    return toString(2, Process_encoder(ConverterOptions_$ctor(), p));
}

/**
 * exports in json-ld format
 */
export function Process_toStringLD(p: Process): string {
    let returnVal: ConverterOptions;
    return toString(2, Process_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), p));
}

export function ProcessSequence_fromString(s: string): FSharpList<Process> {
    let decoder: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Process, [string, ErrorReason_$union]>));
    return fromString<FSharpList<Process>>(uncurry2((decoder = Process_decoder(ConverterOptions_$ctor()), (path: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<Process>, [string, ErrorReason_$union]>) => ((value: any): FSharpResult$2_$union<FSharpList<Process>, [string, ErrorReason_$union]> => list_1<Process>(uncurry2(decoder), path, value)))), s);
}

export function ProcessSequence_toString(p: FSharpList<Process>): string {
    let options: ConverterOptions;
    return toString(2, list_2(map<Process, any>((options = ConverterOptions_$ctor(), (oa: Process): any => Process_encoder(options, oa)), p)));
}

/**
 * exports in json-ld format
 */
export function ProcessSequence_toStringLD(p: FSharpList<Process>): string {
    let options: ConverterOptions, returnVal: ConverterOptions;
    return toString(2, list_2(map<Process, any>((options = ((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal))), (oa: Process): any => Process_encoder(options, oa)), p)));
}

